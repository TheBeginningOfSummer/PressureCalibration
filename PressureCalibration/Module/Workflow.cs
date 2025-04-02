using CSharpKit.Communication;
using CSharpKit.FileManagement;
using Data;
using Services;
using System.Collections.Concurrent;
using System.ComponentModel;
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
    public class Acquisition : Loader
    {
        public Action<string>? WorkProcess;
        public Config CFG = Config.Instance;//此处加载可以保证加载完成，先于构造函数

        #region 单例模式
        private static Acquisition? _instance = null;
        private static readonly object _instanceLock = new();
        public static Acquisition Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                        _instance = Load<Acquisition>("Config", "[Device]ACQCard.json", nameof(Acquisition));
                }
                return _instance;
            }
        }
        #endregion

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
            if (CurrentState is Idle)
                IsRunning = false;
            else
                IsRunning = true;
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

        #region 参数、数据
        /// <summary>
        /// 数据采集组（采集卡）数量，可以用来初始化采集卡的地址
        /// </summary>
        public int CardAmount { get; set; } = 8;//采集卡数量
        /// <summary>
        /// 每个采集组的传感器数量
        /// </summary>
        public int SensorCount { get; set; } = 16;
        /// <summary>
        /// 传感器类型
        /// </summary>
        public string SensorType { get; set; } = "7570";

        private bool isTestVer = false;
        /// <summary>
        /// 测试版本
        /// </summary>
        public bool IsTestVer
        {
            get { return isTestVer; }
            set
            {
                isTestVer = value;
                OnPpChanged(nameof(IsTestVer));
            }
        }
        /// <summary>
        /// 整体良率
        /// </summary>
        public double OverallYield { get; set; } = 1.0;
        /// <summary>
        /// 最小良率
        /// </summary>
        public double MinYield { get; set; } = 0.9;
        /// <summary>
        /// 采集卡连接（保存）
        /// </summary>
        public BindingList<SerialPortTool> Connection { get; set; } = [];
        /// <summary>
        /// 采集的数据，地址-采集卡
        /// </summary>
        public readonly ConcurrentDictionary<int, Group> GroupDic = [];
        /// <summary>
        /// 与GroupDic关联的采集数据组，按通讯端口分组
        /// </summary>
        public readonly ConcurrentDictionary<string, List<Group>> GroupByCom = [];
        /// <summary>
        /// 数据监控键值
        /// </summary>
        public readonly List<string> DisplayedKeys = [];
        #endregion

        #region 控制变量
        private bool isCalibrate = true;
        /// <summary>
        /// 是否标定
        /// </summary>
        public bool IsCalibrate
        {
            get { return isCalibrate; }
            set
            {
                isCalibrate = value;
                OnPpChanged(nameof(IsCalibrate));
            }
        }

        private bool isShowData = false;
        /// <summary>
        /// 控制数据显示
        /// </summary>
        public bool IsShowData
        {
            get { return isShowData; }
            set
            {
                isShowData = value;
                OnPpChanged(nameof(IsShowData));
            }
        }
        /// <summary>
        /// 当前温度索引
        /// </summary>
        public int CurrentTempIndex = 0;
        /// <summary>
        /// 运行标记
        /// </summary>
        public bool IsRunning = false;
        /// <summary>
        /// 控制暂停
        /// </summary>
        public bool IsSuspend = false;
        #endregion

        #region 设备
        public Database DB;
        public CalibrationParameter CalPara;
        public MotionParameter MotionPara;
        public PressController Pace;
        public TECController Tec;
        public ZmotionMotionControl Motion;
        public InputMonitor InMonitor;
        #endregion

        public Acquisition()
        {
            DB = CFG.DB;//加载数据库
            CalPara = CFG.CP;
            MotionPara = CFG.MP;
            Pace = CFG.PACE;
            Tec = CFG.TEC;
            Motion = CFG.Zmotion;

            InMonitor = new InputMonitor(Motion);
        }

        public override string Translate(string name)
        {
            return name switch
            {
                nameof(CardAmount) => "采集卡数",
                nameof(SensorCount) => "传感器数",
                nameof(SensorType) => "类型",
                nameof(IsTestVer) => "测试版本",
                nameof(OverallYield) => "总良率",
                nameof(MinYield) => "最小良率",
                nameof(Connection) => "连接",
                nameof(IsCalibrate) => "是否标定",
                nameof(IsShowData) => "数据显示",
                _ => name,
            };
        }

        public override void LoaderInitialize()
        {
            //初始化通信端口数量
            for (int i = 0; i < CardAmount; i++)
            {
                if (Connection.Count < CardAmount)//端口过少时，添加端口
                {
                    if (i < Connection.Count)
                        continue;
                    else
                        Connection.Add(new SerialPortTool() { Name = $"端口{i + 1}" });
                }
            }
            //初始化采集组
            for (int i = 1; i <= CardAmount; i++)
            {
                Group group = new GroupBOE2520(Connection[i - 1], (byte)i, SensorCount);
                //初始化采集组
                switch (SensorType)
                {
                    case "BOE2520":
                        group = new GroupBOE2520(Connection[i - 1], (byte)i, SensorCount);
                        break;
                    case "6862":
                        group = new GroupZXC6862(Connection[i - 1], (byte)i, SensorCount);
                        break;
                    case "7570":
                        group = new GroupZXW7570(Connection[i - 1], (byte)i, SensorCount);
                        break;
                    default: break;
                }
                //分配给板卡1-9的设备地址
                GroupDic.TryAdd(i, group);
                //初始化采集温度数据
                for (int j = 1; j <= 4; j++)
                    DisplayedKeys.Add($"D{i}T{j}");
            }
            //按通信串口分组
            foreach (var acq in GroupDic.Values)
            {
                if (!GroupByCom.TryAdd(acq.Connection.PortName, [acq]))
                    GroupByCom[acq.Connection.PortName].Add(acq);
            }

            DisplayedKeys.Add("P1");
            //DisplayedKeys.Add("P2");
            DataMonitor.Initialize([.. DisplayedKeys]);
        }

        public void InitializeDevice()
        {
            #region 设备连接
            Open();
            if (Pace.Connect())
                WorkProcess?.Invoke("连接压力控制器成功！");
            else
                WorkProcess?.Invoke("连接压力控制器失败！");
            if (Tec.Open())
                WorkProcess?.Invoke("连接温度控制器成功！");
            else
                WorkProcess?.Invoke("连接温度控制器失败！");
            if (Motion.Connect())
            {
                Motion.ECInitialize();
                //轴参数重新初始化
                Motion.Initialize();
                InMonitor = new InputMonitor(Motion);
                Task.Run(InMonitor.UpdateInput);
                WorkProcess?.Invoke("连接控制卡成功！");
            }
            else
                WorkProcess?.Invoke("连接控制卡失败！");
            #endregion

        }

        #region 数据采集
        public void InitializeACQ()
        {
            WorkProcess?.Invoke("芯片初始化中……");
            CurrentTempIndex = 0;//温度索引
            foreach (var group in GroupDic.Values)
                group.InitializeACQ();
            WorkProcess?.Invoke("芯片初始化完成");
        }

        public void Open()
        {
            for (int i = 0; i < Connection.Count; i++)
            {
                if (Connection[i].Open())
                    WorkProcess?.Invoke($"连接采集卡端口{i + 1}成功！");
                else
                    WorkProcess?.Invoke($"连接采集卡端口{i + 1}失败！");
            }
        }

        public void Close()
        {
            for (int i = 0; i < Connection.Count; i++)
            {
                if (Connection[i].Close())
                    WorkProcess?.Invoke($"断开采集卡端口{i}成功！");
            }
        }

        public bool PowerOn()
        {
            ReceivedData[] receivedData = new ReceivedData[CardAmount];
            for (int i = 0; i < CardAmount; i++)
                receivedData[i] = ReceivedData.ParseData(GroupDic[i + 1].PowerOn())[0];
            foreach (var item in receivedData)
            {
                if (item == null)
                {
                    WorkProcess?.Invoke($"采集卡数量错误");
                    return false;
                }
                if (item.DataType == ReceivedType.Timeout)
                {
                    WorkProcess?.Invoke($"采集卡{item.DeviceAddress}通信超时");
                    return false;
                }
                if (item.IsFault)
                {
                    WorkProcess?.Invoke($"采集卡{item.DeviceAddress}短路");
                    return false;
                }
            }
            return true;
        }

        public void PowerOff()
        {
            ReceivedData[] receivedData = new ReceivedData[CardAmount];
            for (int i = 0; i < CardAmount; i++)
                receivedData[i] = ReceivedData.ParseData(GroupDic[i + 1].PowerOff())[0];
            foreach (var item in receivedData)
            {
                if (item == null)
                {
                    WorkProcess?.Invoke($"采集卡数量错误");
                    return;
                }
                if (item.DataType == ReceivedType.Timeout)
                {
                    WorkProcess?.Invoke($"采集卡{item.DeviceAddress}通信超时");
                    return;
                }
                if (item.IsFault)
                {
                    WorkProcess?.Invoke($"采集卡{item.DeviceAddress}短路");
                    return;
                }
            }
        }
        /// <summary>
        /// 得到温度和传感器输出的数据
        /// </summary>
        /// <param name="sensorTest">传感器的数据（一次采集的所有数据）</param>
        /// <param name="targetT">目标温度</param>
        /// <param name="offsetT">最大温度偏移</param>
        /// <returns>温度数据（一次采集的所有数据）</returns>
        public TempTest GetTestData(out SensorTest sensorTest, decimal targetT = 15, decimal offsetT = 1)
        {
            TempTest tempData = new();
            sensorTest = new();
            if (IsRunning) return tempData;
            for (int i = 1; i <= GroupDic.Count; i++)
            {
                try
                {
                    GroupDic[i].SetTargetT(targetT, offsetT);
                    decimal[] t = GroupDic[i].ReadTemperature(IsTestVer);
                    tempData.TempList.Add(t);
                    GroupDic[i].GetSensorsOutput(out decimal[] tArray, out decimal[] pArray, IsTestVer);
                    sensorTest.Temperature.Add(tArray);
                    sensorTest.Pressure.Add(pArray);
                }
                catch (Exception)
                {
                    return tempData;
                }
            }
            return tempData;
        }
        /// <summary>
        /// 得到监视数据
        /// </summary>
        /// <param name="monitorData">数据容器</param>
        /// <param name="group">采集组</param>
        /// <param name="temp">温度数据</param>
        /// <param name="press">压力数据</param>
        public void MonitoringData(ConcurrentDictionary<string, double> monitorData, Group group, decimal[]? temp, decimal? press)
        {
            //取某次采集的时间和温度数据
            if (group.DeviceAddress == 1)
            {
                //采集时间
                monitorData["Time"] = DateTime.Now.ToOADate();
                //采集压力
                if (press == null)
                    monitorData["P1"] = (double)Pace.GetPress(isTest: IsTestVer);
                else
                    monitorData["P1"] = (double)press;
            }
            //采集温度
            temp ??= group.ReadTemperature(IsTestVer);
            for (int j = 0; j < temp.Length; j++)
            {
                if (monitorData.ContainsKey($"D{group.DeviceAddress}T{j + 1}"))
                    monitorData[$"D{group.DeviceAddress}T{j + 1}"] = (double)temp[j];
            }
        }
        /// <summary>
        /// 采集所有采集卡的标定数据（单温度单压力）
        /// </summary>
        /// <param name="setP">设置压力，用于寻找目标数据,设为NaN时只采集监视数据</param>
        /// <param name="setT">设置温度，用于寻找目标数据,设为NaN时只采集监视数据</param>
        /// <returns>监视数据</returns>
        public ConcurrentDictionary<string, double> GetDataByTask(double setP, double setT)
        {
            //数据容器，采集监视数据
            ConcurrentDictionary<string, double> monitorData = DataMonitor.GetDataContainer(-1);
            //采集线程列表
            List<Task> taskList = [];
            //按分组采集
            foreach (var groupList in GroupByCom.Values)
            {
                var t = Task.Run(() =>
                {
                    foreach (var acq in groupList)
                    {
                        if (double.IsNaN(setP) || double.IsNaN(setT))
                        {
                            //采集监视数据
                            MonitoringData(monitorData, acq, null, null);
                        }
                        else
                        {
                            //采集计算数据
                            var temp = acq.GetData((decimal)setP, (decimal)setT, out decimal pressure, IsTestVer);
                            //采集监视数据
                            MonitoringData(monitorData, acq, temp, pressure);
                        }
                    }
                });
                taskList.Add(t);
            }
            //等待所有采集完成
            Task.WaitAll([.. taskList]);
            //检测数据是否完成，并加入缓存
            if (IsShowData)
                DataMonitor.Cache.Writer.TryWrite(monitorData.ToDictionary());
            return monitorData;
        }

        public void Verify()
        {
            //标定计算
            foreach (var sensorGroup in GroupDic.Values)
            {
                //采集验证数据
                sensorGroup.Verify();
                //选择芯片
                sensorGroup.SelectSensor();
            }
        }
        /// <summary>
        /// 等待所有采集卡采集温度达到目标温度
        /// </summary>
        /// <param name="targetT">目标温度</param>
        /// <returns></returns>
        public bool WaitTemperature(decimal targetT)
        {
            if (!IsTestVer)
            {
                //设置温度
                Tec.TECOnOff(true);
                Tec.SetTemp((short)(targetT * 100));
            }
            //ConcurrentDictionary<string, double> temp1 = [];
            //ConcurrentDictionary<string, double> temp2 = [];
            for (int i = 0; i <= CalPara.TTimeout; i++)
            {
                bool isOK = true;
                double minT = 90;
                double maxT = -20;
                //采集监视数据
                var monitorData = GetDataByTask(double.NaN, double.NaN);
                //温度数据检测，找到最小最大温度
                foreach (var tempData in monitorData)
                {
                    if (IsTestVer) break;//跳过检测
                    if (tempData.Key == "Time" || tempData.Key == "P1" || tempData.Key == "P2") continue;
                    if (minT > tempData.Value) minT = tempData.Value;
                    if (maxT < tempData.Value) maxT = tempData.Value;
                    if (tempData.Value == -256)
                    {
                        Warning($"得到温度传感器数据失败。采集卡{tempData.Key}");
                        continue;
                    }
                    if (Math.Abs((decimal)tempData.Value - targetT) > CalPara.MaxTemperatureDiff)
                    {
                        if (i % 10 == 0)
                            WorkProcess?.Invoke($"设备[{tempData.Key}]与目标温度差超过{CalPara.MaxTemperatureDiff}");
                        isOK = false;
                    }
                }
                //其他温度检测计算
                if (isOK)
                {
                    if (maxT < minT)
                    {
                        if (i % 10 == 0)
                            Warning($"最大最小温度错误Max[{maxT}] Min[{minT}]");
                        isOK = false;
                    }
                    if ((maxT - minT) > 1)
                    {
                        if (i % 10 == 0)
                            Warning($"最大温度与最小温度差值大于1，Max[{maxT}] Min[{minT}]");
                        isOK = false;
                    }
                }

                #region 温度波动检测
                //if (i == 0) temp1 = monitorData;
                ////温度在范围内时检测
                //if (isTempOK)
                //{
                //    //每10S检测
                //    if (i % 10 == 0)
                //    {
                //        bool isOK = true;
                //        temp2 = monitorData;
                //        foreach (var item in temp1)
                //        {
                //            if (item.Key == "Time") continue;
                //            if (item.Key == "P1") continue;
                //            if (item.Key == "P2") continue;
                //            var diff = temp2[item.Key] - temp1[item.Key];
                //            if (diff > 0.1) isOK = false;
                //        }
                //        temp1 = monitorData;
                //        //温度达标转换下一阶段
                //        if (isOK)
                //        {
                //            WorkProcess?.Invoke($"温度OK。");
                //            return true;
                //        }
                //    }
                //}
                #endregion

                if (IsSuspend)
                {
                    WorkProcess?.Invoke($"暂停");
                    return false;
                }

                if (isOK) return true;
                Thread.Sleep(800);
            }
            Warning($"采集温度时间已超时。请检查后重新启动。");
            return false;
        }
        /// <summary>
        /// 等待压力值达到目标压力
        /// </summary>
        /// <param name="targetP">目标压力</param>
        /// <returns></returns>
        public bool WaitPressure(decimal targetP)
        {
            //设置压力
            Pace.SetPress(targetP, IsTestVer);
            //等待压力(5S)
            for (int j = 0; j < CalPara.PTimeout; j++)
            {
                bool isOK = true;
                //采集监视数据
                GetDataByTask(double.NaN, double.NaN);
                //得到压力
                decimal result = Pace.GetPress(isTest: IsTestVer);
                //检测压力差值
                if (Math.Abs(result - targetP) > CalPara.MaxPressureDiff)
                {
                    if (j % 5 == 0)
                        WorkProcess?.Invoke($"与目标压度差超过{CalPara.MaxPressureDiff}");
                    isOK = false;
                }

                if (j >= CalPara.PressDelay)
                    if (isOK) return true;
                Thread.Sleep(900);
            }
            Warning($"采集压力时间超时。请检查后重新启动。");
            Pace.Vent(IsTestVer);
            return false;
        }
        /// <summary>
        /// 采集目标温度下的所有压力数据（单温度）
        /// </summary>
        /// <param name="targetT">目标温度</param>
        /// <returns></returns>
        public bool ACQCaliData(decimal targetT)
        {
            decimal[] setPPara = [.. CalPara.PressurePoints];

            for (int i = 0; i < setPPara.Length; i++)
            {
                WorkProcess?.Invoke($"开始采集压力{setPPara[i]}");
                if (WaitPressure(setPPara[i]))
                    GetDataByTask((double)setPPara[i], (double)targetT);
                else
                    return false;
                WorkProcess?.Invoke($"完成采集压力{setPPara[i]}");
            }
            return true;
        }
        /// <summary>
        /// 等待目标压力，采集验证数据
        /// </summary>
        /// <returns></returns>
        public bool ACQVerifyData()
        {
            decimal[] setPData = [.. CalPara.VerifyPressures];

            foreach (decimal setPress in setPData)
            {
                WorkProcess?.Invoke($"开始采集验证压力{setPress}");
                if (WaitPressure(setPress))
                    //验证数据
                    Verify();
                else
                    return false;
                WorkProcess?.Invoke($"完成采集压力{setPress}");
            }
            return true;
        }

        public bool GetSensorOutput()
        {
            decimal[] checkPArray = [.. CalPara.CheckPressures];

            for (int i = 0; i < checkPArray.Length; i++)
            {
                WorkProcess?.Invoke($"开始采集检测压力{checkPArray[i]}");
                if (WaitPressure(checkPArray[i]))
                {
                    decimal result = Pace.GetPress(isTest: IsTestVer);
                    foreach (var group in GroupDic.Values)
                    {
                        var temp = group.ReadTemperature(IsTestVer);
                        group.GetSensorsOutput(out decimal[] tempArray, out decimal[] pressArray);
                        for (int j = 0; j < tempArray.Length; j++)
                        {
                            var t = temp[group.GetTempIndex(j)];
                            if (Math.Abs(pressArray[j] - result) > CalPara.CheckPressureDiff && Math.Abs(tempArray[j] - t) > CalPara.CheckTemperatureDiff)
                                group.GetSensor(j).Result = "Check";
                            else
                                group.GetSensor(j).Result = "NG";
                        }
                        if (CalPara.IsSave)
                            group.SaveDatabase().Wait();
                    }
                }
                else
                    return false;
                WorkProcess?.Invoke($"完成采集检测压力{checkPArray[i]}");
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
        #endregion

        #region 运动控制
        public bool ReadyPosition()
        {
            try
            {
                var axis1 = Motion.Axes[Motion.AxesName[0]];
                var axis2 = Motion.Axes[Motion.AxesName[1]];
                if (!axis1.IsEnabled()) axis1.Enable();
                if (!axis2.IsEnabled()) axis2.Enable();

                //设置轴类型
                axis1.SetType(65);
                axis1.SetType(65);
                axis1.SingleAbsoluteMove(MotionPara.Axis1Work);
                axis2.SingleAbsoluteMove(MotionPara.Axis2Work);
                axis1.Wait();
                axis2.Wait();

                //设置轴类型
                axis2.SetType(67);
                axis2.SetTorque(MotionPara.Axis2Torque, MotionPara.Axis2RotateSpeed);
                Thread.Sleep(3000);
                axis2.Wait();
                Thread.Sleep(3000);
                //读取力矩？
                WorkProcess?.Invoke($"轴2当前转矩：{axis2.GetTorque()}");
                return true;
            }
            catch (Exception e)
            {
                Warning("运动至工作位置失败，" + e.Message);
                return false;
            }
        }

        public bool WorkingTorque()
        {
            try
            {
                var axis1 = Motion.Axes[Motion.AxesName[0]];
                var axis2 = Motion.Axes[Motion.AxesName[1]];
                
                //设置轴类型
                axis1.SetType(67);
                axis1.SetTorque(MotionPara.Axis1Torque, MotionPara.Axis1RotateSpeed);
                Thread.Sleep(3000);
                axis1.Wait();
                Thread.Sleep(3000);
                //读取力矩？
                WorkProcess?.Invoke($"轴1当前转矩：{axis1.GetTorque()}");
                return true;
            }
            catch (Exception e)
            {
                Warning("运动至工作位置失败，" + e.Message);
                return false;
            }
        }

        public void UnloadTorque()
        {
            try
            {
                var axis1 = Motion.Axes[Motion.AxesName[0]];
                var axis2 = Motion.Axes[Motion.AxesName[1]];
                //设置轴类型todo
                axis1.SetTorque(MotionPara.Axis1Torque / 2, MotionPara.Axis1RotateSpeed);
                Thread.Sleep(2000);
                axis1.SetTorque(MotionPara.Axis1Torque / 10, MotionPara.Axis1RotateSpeed);
                Thread.Sleep(2000);
                axis1.SetTorque(0, MotionPara.Axis1RotateSpeed);
                Thread.Sleep(2000);
                axis1.Wait();
                Thread.Sleep(3000);

                //设置轴类型todo
                axis2.SetTorque(MotionPara.Axis2Torque / 2, MotionPara.Axis2RotateSpeed);
                Thread.Sleep(2000);
                axis2.SetTorque(0, MotionPara.Axis2RotateSpeed);
                Thread.Sleep(2000);
                axis2.Wait();
                Thread.Sleep(3000);
            }
            catch (Exception e)
            {
                Warning("运动至上料位置失败，" + e.Message);
            }
        }

        public void UploadPosition()
        {
            try
            {
                var axis1 = Motion.Axes[Motion.AxesName[0]];
                var axis2 = Motion.Axes[Motion.AxesName[1]];
                
                //设置轴类型todo
                axis1.SetType(65);
                axis1.SingleAbsoluteMove(MotionPara.Axis1Work);
                axis1.Wait();

                //设置轴类型
                axis2.SetType(65);
                axis1.SingleAbsoluteMove(MotionPara.Axis1Upload);
                Thread.Sleep(2000);
                axis2.SingleAbsoluteMove(MotionPara.Axis2Upload);
                axis1.Wait();
                axis2.Wait();
            }
            catch (Exception e)
            {
                Warning("运动至上料位置失败，" + e.Message);
            }
        }
        #endregion

    }

    #region 状态
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
            if (context.IsTestVer)
            {
                if (context.IsCalibrate)
                    context.SetState(new WaitT());
                else
                    context.SetState(new WaitCheckTemperature());
                context.Run();
                return;
            }

            if (!context.PowerOn()) return;//打开电源
            context.InitializeACQ();//初始化采集数据
            
            if (context.IsCalibrate)
                context.SetState(new WaitT());
            else
                context.SetState(new WaitCheckTemperature());
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

            if (context.ACQCaliData(targetTemp))
            {
                //for (int i = 1; i <= context.GroupDic.Count; i++)
                //{
                //    if (!context.GroupDic[i].CheckData(actualTData[context.CurrentTempIndex], out List<int> index))
                //    {
                //        string message = "";
                //        foreach (var item in index)
                //        {
                //            message += $"[{item}]";
                //        }
                //        context.Warning($"采集卡{i}芯片{message}数据采集失败");
                //    }
                //}

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
                    sensorGroup.Calculate();
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
                Thread.Sleep(2000);
                if (context.ACQVerifyData())
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
                ////计算良率
                //foreach (var item in sensorGroup.SensorDataGroup.Values)
                //{
                //    if (item.Result == "NG")
                //    {
                //        context.OverallYield = context.OverallYield * 100 / 101;
                //    }
                //    if (item.Result == "GOOD")
                //    {
                //        context.OverallYield = (context.OverallYield * 100 + 1) / 101;
                //    }
                //}
                ////总良率判断
                //if (context.OverallYield < context.MinYield)
                //{
                //    context.Warning($"良率超过{context.OverallYield * 100:N2}%");
                //    return;
                //}
                //写入烧录数据
                var result = ReceivedData.ParseData(sensorGroup.WriteAllFuseData());
                //解析烧录结果
                foreach (var item in result)
                {
                    if (!item.IsEffective)
                    {
                        sensorGroup.GetSensor(item.SensorIndex).Result = "NG";
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
                var message = ReceivedData.ParseData(sensorGroup.Fuse());
                //解析显示message
                foreach (var item in message)
                {
                    if (item.DataType == ReceivedType.Fuse)
                    {
                        if (item.IsEffective)
                        {
                            sensorGroup.GetSensor(item.SensorIndex).IsFused = true;
                        }
                        else
                        {
                            sensorGroup.GetSensor(item.SensorIndex).IsFused = false;
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

    public class WaitCheckTemperature : IAcqState
    {
        public void Process(Acquisition context)
        {
            decimal[] checkTArray = [.. context.CalPara.CheckTemperatures];
            var targetT = checkTArray[context.CurrentTempIndex];
            context.WorkProcess?.Invoke($"等待温度{targetT}");

            try
            {
                if (context.WaitTemperature(targetT))
                {
                    context.WorkProcess?.Invoke($"温度OK。");
                    context.SetState(new WaitCheckPressure());
                    context.Run();
                }
            }
            catch (Exception e)
            {
                context.Warning(e.Message);
            }
        }
    }

    public class WaitCheckPressure : IAcqState
    {
        public void Process(Acquisition context)
        {
            decimal[] checkTArray = [.. context.CalPara.CheckTemperatures];

            if (context.GetSensorOutput())
            {
                if (context.CurrentTempIndex >= (checkTArray.Length - 1))
                {
                    context.CurrentTempIndex = 0;
                    context.SetState(new Idle());
                    context.Run();
                }
                else
                {
                    context.CurrentTempIndex += 1;
                    context.SetState(new WaitCheckTemperature());
                    context.Run();
                }
            }
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
    #endregion

}
