using Calibration.Data;
using CSharpKit.Communication;
using CSharpKit.FileManagement;
using System.Collections.Concurrent;

namespace Calibration.Services
{
    public interface IAcqState
    {
        void Process(Acquisition context);
    }

    public class Acquisition : ParameterManager
    {
        public Action<string>? WorkProcess;

        #region 状态模式
        private IAcqState currentState = new Idle();
        public IAcqState CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        public void SetState(IAcqState state)
        {
            CurrentState = state;
            string message = state.GetType().Name.Split('.').Last();
            WorkProcess?.Invoke($"当前状态: {message}");
        }

        public void Run()
        {
            CurrentState.Process(this);
        }
        #endregion

        #region 单例模式
        private static Acquisition? _instance;
        private static readonly object _instanceLock = new();
        public static Acquisition Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                        _instance = new Acquisition();
                }
                return _instance;
            }
        }
        #endregion

        #region 数据
        public CalibrationParameter CalPara = Get<CalibrationParameter>();
        public PressController Pace = Get<PressController>();
        public TECController Tec = Get<TECController>();
        public Dictionary<string, List<double>> TPData = [];
        #endregion

        public int TempIndex { get; set; }
        /// <summary>
        /// 数据采集组数量，用来初始化采集卡的地址
        /// </summary>
        public int CardAmount { get; set; } = 9;//采集卡数量
        /// <summary>
        /// 采集的数据，地址-采集卡
        /// </summary>
        public readonly ConcurrentDictionary<int, GroupCalibration> GroupDic = [];
        /// <summary>
        /// 采集卡连接
        /// </summary>
        public SerialPortTool Connection = new();

        public bool IsShowData = false;

        public Acquisition()
        {
            for (int i = 1; i <= CardAmount; i++)
            {
                var group = new GroupCalibration((byte)i)
                {
                    Connection = Connection
                };
                //分配给板卡1-9的设备地址
                GroupDic.TryAdd(i, group);
                TPData.TryAdd($"Time", []);
                for (int j = 1; j <= 4; j++)
                    TPData.TryAdd($"D{i}T{j}", []);
            }
            SerialPara = new();
        }

        public override string Translate(string name)
        {
            return name switch
            {
                nameof(CardAmount) => "采集卡数",
                _ => "",
            };
        }

        public void Initialize()
        {
            foreach (var group in GroupDic.Values)
                group.Initialize();
        }

        public void UpdatePara()
        {
            CalPara = Get<CalibrationParameter>();
            Pace = Get<PressController>();
            Tec = Get<TECController>();
        }

        public ReceivedData[] PowerOn()
        {
            ReceivedData[] receivedData = new ReceivedData[CardAmount];
            for (int i = 0; i < CardAmount; i++)
            {
                receivedData[i] = ReceivedData.ParseData(GroupDic[i + 1].PowerOn())[0];
            }
            return receivedData;
        }

        public ReceivedData[] PowerOff()
        {
            ReceivedData[] receivedData = new ReceivedData[CardAmount];
            for (int i = 0; i < CardAmount; i++)
            {
                receivedData[i] = ReceivedData.ParseData(GroupDic[i + 1].PowerOff())[0];
            }
            return receivedData;
        }

        public bool Open()
        {
            return Connection.OpenMySerialPort(SerialPara!);
        }

        public bool Close()
        {
            return Connection.CloseMySerialPort();
        }

        public void GetData(decimal setP, decimal setT)
        {
            if (IsShowData) TPData[$"Time"].Add(DateTime.Now.ToOADate());
            //采集数据
            for (int i = 1; i <= CardAmount; i++)
            {
                Thread.Sleep(10);
                var temp = GroupDic[i].GetData(Get<CalibrationParameter>().AcquisitionCount, setP, setT);
                for (int j = 0; j < temp.Length; j++)
                {
                    if (IsShowData) TPData[$"D{i}T{j + 1}"].Add((double)temp[j]);
                }
            }
        }

        public void Verify()
        {
            //标定计算
            foreach (var sensorGroup in GroupDic.Values)
            {
                //采集验证数据
                sensorGroup.AcqVerifyGroup();
                //验证计算
                sensorGroup.VerifyGroup(CalPara);
                //选择的芯片
                sensorGroup.SelectSensor();
            }
        }
        //等待所有采集卡温度达标
        public bool WaitTemperature(decimal targetTemperature, out TempTest tempTest)
        {
            List<decimal[]> tempList = [];
            tempTest = new() { Date = DateTime.Now.ToString("HH:mm:ss") };
            decimal minT = 99;
            decimal maxT = 0;
            if (IsShowData) TPData[$"Time"].Add(DateTime.Now.ToOADate());
            for (int i = 1; i <= CardAmount; i++)
            {
                var temp = GroupDic[i].ReadTemperature();
                tempTest.TempList.Add(temp);
                //判断目标温度范围
                for (int j = 0; j < temp.Length; j++)
                {
                    if (IsShowData) TPData[$"D{i}T{j + 1}"].Add((double)temp[j]);
                    if (temp[j] == -256m)
                    {
                        WorkProcess?.Invoke($"得到温度传感器数据失败。采集卡{GroupDic[i].DeviceAddress},温度{j + 1}");
                        continue;
                    }
                    if (Math.Abs(temp[j] - targetTemperature) > 1)
                    {
                        WorkProcess?.Invoke($"采集卡{GroupDic[i].DeviceAddress},温度{j + 1}[{temp[j]}]与目标温度差超过规定值");
                        return false;
                    }
                    if (minT > temp[j]) minT = temp[j];
                    if (maxT < temp[j]) maxT = temp[j];
                }
            }
            //参考温度最大最小差值
            if (maxT < minT)
            {
                WorkProcess?.Invoke($"最大最小温度记录错误");
                return false;
            }
            if ((maxT - minT) > 1)
            {
                WorkProcess?.Invoke($"最大最小温度差超过规定值");
                return false;
            }
            return true;
        }
        //等待所有压力值达标，采集数据
        public bool WaitPressure(decimal targetT)
        {
            decimal[] setPPara = [.. CalPara.PressurePoints];

            for (int i = 0; i < setPPara.Length; i++)
            {
                WorkProcess?.Invoke($"开始采集压力{setPPara[i]}");
                //计时
                int count = 0;
                //设置压力
                Pace.SetPress(setPPara[i]);
                //等待压力(5S)
                for (int j = 0; j < CalPara.PressDelay; j++)
                {
                    count++;
                    //得到压力
                    decimal result = Pace.GetPress();
                    //检测压力差值
                    if (Math.Abs(result - setPPara[i]) > CalPara.MaxPressureDiff) j--;
                    Thread.Sleep(1000);
                    //超时处理
                    if (count >= CalPara.PTimeout)
                    {
                        WorkProcess?.Invoke($"Warning:采集压力时间已超时。");
                        Pace.Vent();
                        return false;
                    }
                }
                //采集数据
                GetData(setPPara[i], targetT);
                WorkProcess?.Invoke($"完成采集压力{setPPara[i]}");
            }
            Pace.Vent();
            return true;
        }
        //等待所有压力值达标，采集验证数据
        public bool WaitVerifyPressure()
        {
            decimal[] setPData = [.. CalPara.VerifyPressures];

            foreach (decimal setPress in setPData)
            {
                WorkProcess?.Invoke($"开始采集压力{setPress}");
                //计时
                int count = 0;
                //设置压力
                Pace.SetPress(setPress);
                //等待压力(5S)
                for (int j = 0; j < CalPara.PressDelay; j++)
                {
                    count++;
                    //得到压力
                    decimal result = Pace.GetPress();
                    //检测压力差值
                    if (Math.Abs(result - setPress) > CalPara.MaxPressureDiff) j--;
                    Thread.Sleep(1000);
                    //超时处理
                    if (count >= CalPara.PTimeout)
                    {
                        WorkProcess?.Invoke($"Warning:采集验证压力时间已超时。");
                        Pace.Vent();
                        return false;
                    }
                }
                //验证数据
                Verify();
                WorkProcess?.Invoke($"完成采集压力{setPress}");
            }
            Pace.Vent();
            return true;
        }

        public void ClearTData()
        {
            foreach (var item in TPData)
            {
                item.Value.Clear();
            }
        }

    }

    public class Idle : IAcqState
    {
        public void Process(Acquisition context)
        {
            //context.SetState(new Initialize());
            context.Run();
        }
    }

    public class Warning : IAcqState
    {
        public IAcqState? PreSatae { get; set; }

        public void Process(Acquisition context)
        {
            if (PreSatae == null)
                context.SetState(new Idle());
            else
                context.SetState(PreSatae);
            context.Run();
        }
    }

    public class Initialize : IAcqState
    {
        public void Process(Acquisition context)
        {
            context.Initialize();//初始化采集数据
            context.TempIndex = 0;//温度索引
            var result = context.PowerOn();
            foreach (var item in result)
            {
                if (item == null)
                {
                    context.WorkProcess?.Invoke($"采集卡数量错误");
                    return;
                }
                if (item.DataType == ReceivedType.Timeout)
                {
                    context.WorkProcess?.Invoke($"采集卡{item.DeviceAddress}通信超时");
                    return;
                }
                if (item.IsFault)
                {
                    context.WorkProcess?.Invoke($"采集卡{item.DeviceAddress}短路");
                    return;
                }
            }
            context.SetState(new WaitT());
            context.Run();
        }
    }

    public class WaitT : IAcqState
    {
        public void Process(Acquisition context)
        {
            decimal[] actualTData = [.. context.CalPara.TempaturePoints];
            var targetTemp = actualTData[context.TempIndex];
            context.WorkProcess?.Invoke($"等待温度{targetTemp}");

            //计时
            int count = 0;
            //设置温度
            context.Tec.TECOnOff(true);
            context.Tec.SetTemp((short)(targetTemp * 100));
            TempTest temp1 = new();//零时温度组1
            TempTest temp2 = new();//零时温度组2
            //在超时时间内运行
            for (int i = 0; i <= context.CalPara.TTimeout; i++)
            {
                //每秒循环判定温度是否达标
                if (context.WaitTemperature(actualTData[context.TempIndex], out TempTest temp))//如果达到了温度范围标准
                {
                    if (i == 0) temp1 = temp;
                    else if (count % 10 == 0)//每10秒检测温度波动
                    {
                        bool isOK = true;
                        //得到当前温度
                        temp2 = temp;
                        //检测10S的温度波动，大于0.1度为不达标
                        for (int j = 0; j < temp2.TempList.Count; j++)
                        {
                            for (int k = 0; k < temp2.TempList[j].Length; k++)
                            {
                                var diff = temp2.TempList[j][k] - temp1.TempList[j][k];
                                if (diff > 0.1m) isOK = false;
                            }
                        }
                        //温度1变为当前温度
                        temp1 = temp;
                        //温度达标转换下一阶段
                        if (isOK)
                        {
                            context.WorkProcess?.Invoke($"温度OK。");
                            context.SetState(new WaitP());
                            context.Run();
                            break;
                        }
                    }
                }
                else
                {
                    if (i == 0) temp1 = temp;
                    if (i == context.CalPara.TTimeout)
                        context.WorkProcess?.Invoke($"Warning:采集温度时间已超时。请检查温度后重新启动。");
                }
                Thread.Sleep(800);
            }
        }
    }

    public class WaitP : IAcqState
    {
        public void Process(Acquisition context)
        {
            decimal[] actualTData = [.. context.CalPara.TempaturePoints];
            var targetTemp = actualTData[context.TempIndex];
            decimal[] setTData = [.. context.CalPara.SetTPoints];

            if (context.WaitPressure(targetTemp))
            {
                for (int i = 1; i <= context.GroupDic.Count; i++)
                {
                    if (!context.GroupDic[i].CheckData(actualTData[context.TempIndex]))
                        context.WorkProcess?.Invoke($"Warning:地址{i}采集卡有芯片数据采集失败");
                }

                if (context.TempIndex >= (setTData.Length - 1))
                {
                    context.TempIndex = 0;
                    context.SetState(new Calculate());
                    context.Run();
                }
                else
                {
                    context.TempIndex += 1;
                    context.SetState(new WaitT());
                    context.Run();
                }
            }
        }
    }

    public class Calculate : IAcqState
    {
        public void Process(Acquisition context)
        {
            context.WorkProcess?.Invoke("数据采集完成，开始计算");
            try
            {
                //标定计算
                foreach (var sensorGroup in context.GroupDic.Values)
                    sensorGroup.CalGroup(context.CalPara.Method);
                context.SetState(new Verify());
                context.Run();
            }
            catch (Exception e)
            {
                context.WorkProcess?.Invoke($"Warning:计算出错.{e.Message}");
            }
        }
    }

    public class Verify : IAcqState
    {
        public void Process(Acquisition context)
        {
            context.WorkProcess?.Invoke("完成系数计算，开始验证计算");
            try
            {
                Thread.Sleep(3000);
                if (context.WaitVerifyPressure())
                {
                    context.SetState(new WriteFuseData());
                    context.Run();
                }
            }
            catch (Exception e)
            {
                context.WorkProcess?.Invoke($"Warning:验证出错.{e.Message}");
            }
        }
    }

    public class WriteFuseData : IAcqState
    {
        public void Process(Acquisition context)
        {
            context.WorkProcess?.Invoke("数据计算完成，开始写入");
            foreach (var sensorGroup in context.GroupDic.Values)
            {
                //写入烧录数据
                var result = ReceivedData.ParseData(sensorGroup.WriteAllFuseData());
                foreach (var item in result)
                {
                    if (!item.IsEffective)
                    {
                        sensorGroup.SensorDataGroup[item.SensorIndex].Result = "NG";
                        context.WorkProcess?.Invoke($"Warning:采集卡{item.DeviceAddress}芯片{item.SensorIndex}写入失败");
                    }
                }

                sensorGroup.SelectSensor();
            }
            //context.SetState(new CheckFuseData());
            context.Run();
        }
    }

    public class Fuse : IAcqState
    {
        public void Process(Acquisition context)
        {
            //烧录芯片
            context.WorkProcess?.Invoke("开始烧录");
            foreach (var sensorGroup in context.GroupDic.Values)
            {
                var message = ReceivedData.ParseData(sensorGroup.Fuse(sensorGroup.SelectedSensor));
                //解析显示message
                foreach (var item in message)
                {
                    if (item.DataType == ReceivedType.Fuse)
                    {
                        if (item.IsEffective)
                        {
                            sensorGroup.SensorDataGroup[item.SensorIndex].IsFused = true;
                        }
                        else
                        {
                            sensorGroup.SensorDataGroup[item.SensorIndex].IsFused = false;
                            context.WorkProcess?.Invoke($"Warning:采集卡{item.DeviceAddress}芯片{item.SensorIndex}烧录失败");
                        }
                    }
                    else
                    {
                        context.WorkProcess?.Invoke($"Warning:采集卡{item.DeviceAddress}非烧录消息");
                    }
                }
            }
            //断电后上电
            context.PowerOff();
            Thread.Sleep(1000);
            context.PowerOn();
            context.SetState(new Check());
            context.Run();
        }
    }

    public class Check : IAcqState
    {
        public void Process(Acquisition context)
        {
            context.WorkProcess?.Invoke("开始检查");
            if (context.CalPara.IsSave)
                for (int i = 1; i <= context.GroupDic.Count; i++)
                    context.GroupDic[i].SaveDatabase().Wait();
            context.PowerOff();
            context.SetState(new Idle());
        }
    }

}
