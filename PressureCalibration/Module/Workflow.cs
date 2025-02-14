using CSharpKit.Communication;
using CSharpKit.FileManagement;
using Services;
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
            NotifyRecord.Record(message, NotifyRecord.LogType.Modification);
        }

        public void Warning(string message, IAcqState? state = null)
        {
            WorkProcess?.Invoke($"Warning: {message}");
            NotifyRecord.Record(message, NotifyRecord.LogType.Warning);
            if (state != null) SetState(state);
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
        public ZmotionMotionControl Motion = Get<ZmotionMotionControl>();
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
        /// 数据监控键值
        /// </summary>
        public readonly List<string> DisplayedKeys = [];

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
            //初始化通信参数
            SerialPara = new();
            //初始化需监视数据的键值
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
                    DisplayedKeys.Add($"D{i}T{j}");
            }
            DisplayedKeys.Add("P1");
            DisplayedKeys.Add("P2");
            DataMonitor.Initialize([.. DisplayedKeys]);
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

        public void UpdateMonitorData(Dictionary<string, double> monitorData, int cardIndex, decimal[]? temp, decimal? press)
        {
            if (cardIndex < 1 || cardIndex > CardAmount) return;
            if (cardIndex == 1)
            {
                monitorData["Time"] = DateTime.Now.ToOADate();
                if (press == null)
                    monitorData["P1"] = (double)Pace.GetPress(isTest: CalPara.IsTestVer);
                else
                    monitorData["P1"] = (double)press;
            }
            temp ??= GroupDic[cardIndex].ReadTemperature(isTest: CalPara.IsTestVer);
            for (int j = 0; j < temp.Length; j++)
            {
                if (monitorData.ContainsKey($"D{cardIndex}T{j + 1}"))
                    monitorData[$"D{cardIndex}T{j + 1}"] = (double)temp[j];
            }
        }

        /// <summary>
        /// 采集所有采集卡的标定数据
        /// </summary>
        /// <param name="setP">设置压力，用于寻找目标数据</param>
        /// <param name="setT">设置温度，用于寻找目标数据</param>
        public bool GetData(decimal setP, decimal setT)
        {
            Dictionary<string, double> monitorData = DataMonitor.GetDataContainer([.. DisplayedKeys]);
            for (int i = 1; i <= CardAmount; i++)
            {
                //采集数据
                var temp = GroupDic[i].GetData(CalPara.AcquisitionCount, setP, setT, out decimal pressure, CalPara.IsTestVer);
                //采集监视数据
                UpdateMonitorData(monitorData, i, temp, pressure);
                //暂停
                if (IsSuspend)
                {
                    WorkProcess?.Invoke($"暂停");
                    return false;
                }
            }
            DataMonitor.Cache.Writer.TryWrite(monitorData);
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
        /// <param name="targetT">目标温度</param>
        /// <returns></returns>
        public bool WaitTemperature(decimal targetT)
        {
            Dictionary<string, double> temp1 = [];
            Dictionary<string, double> temp2 = [];
            for (int i = 0; i <= CalPara.TTimeout; i++)
            {
                bool isTempOK = true;
                double minT = 99;
                double maxT = 0;
                //采集温度数据
                Dictionary<string, double> monitorData = DataMonitor.GetDataContainer([.. DisplayedKeys]);
                for (int m = 1; m <= CardAmount; m++)
                {
                    //采集监视数据
                    UpdateMonitorData(monitorData, m, null, null);
                }
                DataMonitor.Cache.Writer.TryWrite(monitorData);
                //温度数据检测
                foreach (var tempData in monitorData)
                {
                    if (CalPara.IsTestVer) break;//跳过检测
                    if (tempData.Key == "Time") continue;
                    if (tempData.Key == "P1") continue;
                    if (tempData.Key == "P2") continue;
                    if (minT > tempData.Value) minT = tempData.Value;
                    if (maxT < tempData.Value) maxT = tempData.Value;
                    if (tempData.Value == -256)
                    {
                        Warning($"得到温度传感器数据失败。采集卡{tempData.Key}");
                        continue;
                    }
                    if (Math.Abs((decimal)tempData.Value - targetT) > 1)
                    {
                        //WorkProcess?.Invoke($"采集卡{GroupDic[i].DeviceAddress},温度{j + 1}[{temp[j]}]与目标温度差超过规定值");
                        isTempOK = false;
                    }
                }
                //参考温度最大最小差值
                if (maxT < minT)
                {
                    //WorkProcess?.Invoke($"最大最小温度记录错误");
                    isTempOK = false;
                }
                if ((maxT - minT) > 1)
                {
                    //WorkProcess?.Invoke($"最大最小温度差超过规定值");
                    isTempOK = false;
                }

                if (i == 0) temp1 = monitorData;
                //温度在范围内时检测
                if (isTempOK)
                {
                    //每10S检测
                    if (i % 10 == 0)
                    {
                        bool isOK = true;
                        temp2 = monitorData;
                        foreach (var item in temp1)
                        {
                            if (item.Key == "Time") continue;
                            if (item.Key == "P1") continue;
                            if (item.Key == "P2") continue;
                            var diff = temp2[item.Key] - temp1[item.Key];
                            if (diff > 0.1) isOK = false;
                        }
                        temp1 = monitorData;
                        //温度达标转换下一阶段
                        if (isOK)
                        {
                            WorkProcess?.Invoke($"温度OK。");
                            return true;
                        }
                    }
                }
                Thread.Sleep(800);

                if (IsSuspend)
                {
                    WorkProcess?.Invoke($"暂停");
                    return false;
                }
            }
            Warning($"采集温度时间已超时。请检查温度后重新启动。");
            return false;
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
                Pace.SetPress(setPPara[i], CalPara.IsTestVer);
                //等待压力(5S)
                for (int j = 0; j < CalPara.PressDelay; j++)
                {
                    count++; 
                    //得到压力
                    decimal result = Pace.GetPress(isTest: CalPara.IsTestVer);
                    //数据容器，采集监视数据
                    Dictionary<string, double> monitorData = DataMonitor.GetDataContainer([.. DisplayedKeys]);
                    for (int k = 1; k <= CardAmount; k++)
                    {
                        //采集监视数据
                        UpdateMonitorData(monitorData, k, null, result);
                    }
                    DataMonitor.Cache.Writer.TryWrite(monitorData);
                    //检测压力差值
                    if (Math.Abs(result - setPPara[i]) > CalPara.MaxPressureDiff) j--;
                    //超时处理
                    if (count >= CalPara.PTimeout)
                    {
                        Warning($"采集压力时间超时。");
                        Pace.Vent(CalPara.IsTestVer);
                        return false;
                    }
                    Thread.Sleep(1000);
                }
                //采集数据
                if (GetData(setPPara[i], targetT)) return false;
                WorkProcess?.Invoke($"完成采集压力{setPPara[i]}");
            }
            Pace.Vent(CalPara.IsTestVer);
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
                        Warning($"采集验证压力时间已超时。");
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

        public bool Check()
        {
            //计时
            int count = 0; decimal result = 0; //decimal temp = 0;
            //设置检测压力
            Pace.SetPress(CalPara.CheckPressure, CalPara.IsTestVer);
            //等待压力(5S)
            for (int j = 0; j < CalPara.PressDelay; j++)
            {
                count++;
                //得到压力
                result = Pace.GetPress();
                //检测压力差值
                if (Math.Abs(result - CalPara.CheckPressure) > CalPara.MaxPressureDiff) j--;
                Thread.Sleep(1000);
                //超时处理
                if (count >= CalPara.PTimeout)
                {
                    Warning($"采集验证压力时间已超时。");
                    Pace.Vent();
                    return false;
                }
            }
            //检测并保存标定数据
            for (int i = 1; i <= GroupDic.Count; i++)
            {
                GroupDic[i].GetSensorsValue(out decimal[] tempArray, out decimal[] pressArray);
                for (int j = 0; j < GroupDic[i].SensorCount; j++)
                {
                    if ((pressArray[j] - result) > CalPara.CheckPressureDiff)
                    {
                        GroupDic[i].SensorDataGroup[j].Result = "Check";
                    }
                }
                if (CalPara.IsSave)
                    GroupDic[i].SaveDatabase().Wait();
            }
            return true;
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

            try
            {
                if (!context.CalPara.IsTestVer)
                {
                    //设置温度
                    context.Tec.TECOnOff(true);
                    context.Tec.SetTemp((short)(targetTemp * 100));
                }
                if (context.WaitTemperature(targetTemp))
                {
                    context.WorkProcess?.Invoke($"温度OK。");
                    context.SetState(new WaitP());
                    context.Run();
                }
            }
            catch (Exception e)
            {
                context.Warning(e.Message);
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
                    if (!context.GroupDic[i].CheckData(actualTData[context.CurrentTempIndex], out List<int> index))
                    {
                        string message = "";
                        foreach (var item in index)
                        {
                            message += $"[{item}]";
                        }
                        context.Warning($"采集卡{i}芯片{message}数据采集失败");
                    }
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
                context.Warning($"计算出错。{e.Message}");
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
                context.Warning($"验证出错。{e.Message}");
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
                    context.Warning($"良率超过{context.OverallYield * 100:N2}%");
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
                        context.Warning($"采集卡{item.DeviceAddress}芯片{item.SensorIndex}写入失败");
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
                            context.Warning($"采集卡{item.DeviceAddress}芯片{item.SensorIndex}烧录失败");
                        }
                    }
                    else
                    {
                        context.Warning($"采集卡{item.DeviceAddress}非烧录消息");
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
            //context.WorkProcess?.Invoke("开始检查");
            //context.Check();//手动检测？
            context.PowerOff();
            context.SetState(new Idle());
        }

    }

}
