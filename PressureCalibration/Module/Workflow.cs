using Calibration.Data;
using CSharpKit.Communication;
using CSharpKit.FileManagement;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace Module
{
    public interface IAcqState
    {
        void Process(Acquisition context);
    }
    /// <summary>
    /// 采集主体
    /// </summary>
    public class Acquisition : ParameterManager
    {
        public Action<string>? WorkProcess;

        #region 状态模式
        private IAcqState currentState = new Idle();
        [JsonIgnore]
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

        #region 参数与设备
        public CalibrationParameter CalPara = Get<CalibrationParameter>();
        public PressController Pace = Get<PressController>();
        public TECController Tec = Get<TECController>();
        /// <summary>
        /// 数据采集组数量，用来初始化采集卡的地址
        /// </summary>
        public int CardAmount { get; set; } = 9;//采集卡数量
        /// <summary>
        /// 整体良率
        /// </summary>
        public double OverallYield { get; set; } = 1.0;
        /// <summary>
        /// 最小良率
        /// </summary>
        public double MinYield { get; set; } = 0.9;
        #endregion

        /// <summary>
        /// 采集卡连接
        /// </summary>
        public SerialPortTool Connection = new();
        /// <summary>
        /// 采集的数据，地址-采集卡
        /// </summary>
        public readonly ConcurrentDictionary<int, GroupCalibration> GroupDic = [];
        /// <summary>
        /// 需要展示的数据
        /// </summary>
        public readonly ConcurrentDictionary<string, List<double>> DisplayedData = [];

        #region 控制变量
        /// <summary>
        /// 当前温度索引
        /// </summary>
        public int CurrentTempIndex = 0;
        /// <summary>
        /// 控制数据显示
        /// </summary>
        public bool IsShowData { get; set; } = false;
        /// <summary>
        /// 控制暂停
        /// </summary>
        public bool IsSuspend = false;
        #endregion

        public Acquisition()
        {
            //初始化采集时间数据
            DisplayedData.TryAdd($"Time", []);
            for (int i = 1; i <= CardAmount; i++)
            {
                //通信连接
                var group = new GroupCalibration((byte)i)
                {
                    Connection = Connection
                };
                //分配给板卡1-9的设备地址
                GroupDic.TryAdd(i, group);
                //初始化采集温度数据
                for (int j = 1; j <= 4; j++)
                    DisplayedData.TryAdd($"D{i}T{j}", []);
            }
            //初始化采集压力数据
            DisplayedData.TryAdd($"P1", []);
            DisplayedData.TryAdd($"P2", []);
            //初始化通信参数
            SerialPara = new();
        }

        public override string Translate(string name)
        {
            return name switch
            {
                nameof(CardAmount) => "采集卡数",
                nameof(OverallYield) => "总良率",
                nameof(MinYield) => "最小良率",
                nameof(IsShowData) => "数据显示",
                _ => name,
            };
        }

        public void Initialize()
        {
            foreach (var group in GroupDic.Values)
                group.Initialize();
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
        /// <summary>
        /// 采集所有采集卡的标定数据
        /// </summary>
        /// <param name="setP">设置压力，用于寻找目标数据</param>
        /// <param name="setT">设置温度，用于寻找目标数据</param>
        public bool GetData(decimal setP, decimal setT)
        {
            //采集时间
            if (IsShowData) DisplayedData[$"Time"].Add(DateTime.Now.ToOADate());
            for (int i = 1; i <= CardAmount; i++)
            {
                Thread.Sleep(10);
                //采集数据
                var temp = GroupDic[i].GetData(CalPara.AcquisitionCount, setP, setT, out decimal pressure, CalPara.IsTestVer);
                //展示温度数据
                for (int j = 0; j < temp.Length; j++)
                {
                    if (IsShowData) DisplayedData[$"D{i}T{j + 1}"].Add((double)temp[j]);
                }
                //展示压力数据
                if (IsShowData) DisplayedData[$"P1"].Add((double)pressure);
                if (IsSuspend)
                {
                    WorkProcess?.Invoke($"暂停");
                    return false;
                }
            }
            return true;
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
                //选择芯片
                sensorGroup.SelectSensor();
            }
        }
        /// <summary>
        /// 等待所有采集卡温度达标
        /// </summary>
        /// <param name="targetTemperature">目标温度</param>
        /// <param name="tempTest">采集到的所有采集卡的一组温度数据</param>
        /// <returns></returns>
        public bool WaitTemperature(decimal targetTemperature, out TempTest tempTest)
        {
            tempTest = new() { Date = DateTime.Now.ToString("HH:mm:ss") };
            decimal minT = 99;
            decimal maxT = 0;
            if (IsShowData) DisplayedData[$"Time"].Add(DateTime.Now.ToOADate());
            for (int i = 1; i <= CardAmount; i++)
            {
                var temp = GroupDic[i].ReadTemperature(CalPara.IsTestVer);
                tempTest.TempList.Add(temp);
                //判断目标温度范围
                for (int j = 0; j < temp.Length; j++)
                {
                    if (IsShowData) DisplayedData[$"D{i}T{j + 1}"].Add((double)temp[j]);
                    if (CalPara.IsTestVer) continue;//跳过检测
                    if (temp[j] == -256m)
                    {
                        WorkProcess?.Invoke($"得到温度传感器数据失败。采集卡{GroupDic[i].DeviceAddress},温度{j + 1}");
                        continue;
                    }
                    if (Math.Abs(temp[j] - targetTemperature) > 1)
                    {
                        //WorkProcess?.Invoke($"采集卡{GroupDic[i].DeviceAddress},温度{j + 1}[{temp[j]}]与目标温度差超过规定值");
                        return false;
                    }
                    if (minT > temp[j]) minT = temp[j];
                    if (maxT < temp[j]) maxT = temp[j];
                }
            }
            //参考温度最大最小差值
            if (maxT < minT)
            {
                //WorkProcess?.Invoke($"最大最小温度记录错误");
                return false;
            }
            if ((maxT - minT) > 1)
            {
                //WorkProcess?.Invoke($"最大最小温度差超过规定值");
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
                if (GetData(setPPara[i], targetT)) return false;
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

        public void ClearDisplayedData()
        {
            foreach (var item in DisplayedData)
            {
                item.Value.Clear();
            }
        }

        public bool OutputExcel(string path = $"Data\\Excel\\")
        {
            try
            {
                if (ExcelOutput.Output(GroupDic, path, $"数据汇总表"))
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

    public class Idle : IAcqState
    {
        public void Process(Acquisition context)
        {
            context.SetState(new Initialize());
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
            if (context.CalPara.IsTestVer)
            {
                context.SetState(new WaitT());
                context.Run();
                return;
            }
            context.Initialize();//初始化采集数据
            context.CurrentTempIndex = 0;//温度索引
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
            var targetTemp = actualTData[context.CurrentTempIndex];
            context.WorkProcess?.Invoke($"等待温度{targetTemp}");

            //计时
            int count = 0;
            if (!context.CalPara.IsTestVer)
            {
                //设置温度
                context.Tec.TECOnOff(true);
                context.Tec.SetTemp((short)(targetTemp * 100));
            }
            TempTest temp1 = new();//临时温度组1
            TempTest temp2 = new();//临时温度组2
            //在超时时间内运行
            for (int i = 0; i <= context.CalPara.TTimeout; i++)
            {
                //每秒循环判定温度是否达标
                if (context.WaitTemperature(actualTData[context.CurrentTempIndex], out TempTest temp))//如果达到了温度范围标准
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

                if (context.IsSuspend)
                {
                    context.WorkProcess?.Invoke($"暂停");
                    break;
                }
            }
        }
    }

    public class WaitP : IAcqState
    {
        public void Process(Acquisition context)
        {
            decimal[] actualTData = [.. context.CalPara.TempaturePoints];
            var targetTemp = actualTData[context.CurrentTempIndex];
            decimal[] setTData = [.. context.CalPara.SetTPoints];

            if (context.WaitPressure(targetTemp))
            {
                for (int i = 1; i <= context.GroupDic.Count; i++)
                {
                    if (!context.GroupDic[i].CheckData(actualTData[context.CurrentTempIndex]))
                        context.WorkProcess?.Invoke($"Warning:地址{i}采集卡有芯片数据采集失败");
                }

                if (context.CurrentTempIndex >= (setTData.Length - 1))
                {
                    context.CurrentTempIndex = 0;
                    context.SetState(new Calculate());
                    context.Run();
                }
                else
                {
                    context.CurrentTempIndex += 1;
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
            //遍历所有采集开
            foreach (var sensorGroup in context.GroupDic.Values)
            {
                //计算良率
                foreach (var item in sensorGroup.SensorDataGroup.Values)
                {
                    if (item.Result == "NG")
                    {
                        context.OverallYield = context.OverallYield * 100 / 101;
                    }
                    if (item.Result == "GOOD")
                    {
                        context.OverallYield = (context.OverallYield * 100 + 1) / 101;
                    }
                }
                //总良率判断
                if (context.OverallYield < context.MinYield)
                {
                    context.WorkProcess?.Invoke($"Warning:良率超过限制{context.OverallYield * 100:N2}%");
                    return;
                }
                //写入烧录数据
                var result = ReceivedData.ParseData(sensorGroup.WriteAllFuseData());
                //解析烧录结果
                foreach (var item in result)
                {
                    if (!item.IsEffective)
                    {
                        sensorGroup.SensorDataGroup[item.SensorIndex].Result = "NG";
                        context.WorkProcess?.Invoke($"Warning:采集卡{item.DeviceAddress}芯片{item.SensorIndex}写入失败");
                    }
                }
                //选择烧录芯片
                sensorGroup.SelectSensor();
            }
            context.SetState(new Fuse());
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
