using CSharpKit;
using CSharpKit.FileManagement;
using cszmcaux;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;

namespace Services
{
    #region 基础类
    public abstract class BaseAxis : ISetting
    {
        #region 参数
        public string ControllerName { get; set; } = "Controller";
        public string Name { get; set; } = "DefaultAxis";
        public int Number { get; set; }

        public virtual double Type { get; set; }
        public virtual double Units { get; set; }
        public virtual double Sramp { get; set; }
        public virtual double Speed { get; set; }
        public virtual double Creep { get; set; }
        public virtual double JogSpeed { get; set; }
        public virtual double Accele { get; set; }
        public virtual double Decele { get; set; }
        public virtual double FastDecele { get; set; }
        public virtual double FsLimit { get; set; }//正软限位
        public virtual double RsLimit { get; set; }//负软限位
        #endregion

        #region 信号
        public virtual double DatumIn { get; set; }
        public virtual double ForwardIn { get; set; }
        public virtual double ReverseIn { get; set; }
        public virtual double ForwardJogIn { get; set; } = -1;
        public virtual double ReverseJogIn { get; set; } = -1;
        public virtual double FastJogIn { get; set; } = -1;
        #endregion

        #region 状态
        public virtual string State { get; set; } = "";
        public virtual bool IsMoving { get; set; }
        public virtual double TargetPosition { get; set; }
        public virtual double CurrentPosition { get; set; }
        public virtual double CurrentSpeed { get; set; }
        #endregion

        public BaseAxis() { }

        #region 方法
        public string Translate(string name)
        {
            return name switch
            {
                "ControllerName" => "控制器",
                "Name" => "轴名称",
                "Number" => "轴号",
                "Type" => "轴类型",
                "Units" => "脉冲当量",
                "Sramp" => "S曲线",
                "Speed" => "速度",
                "Creep" => "爬行速度",
                "JogSpeed" => "Jog速度",
                "Accele" => "加速度",
                "Decele" => "减速度",
                "FastDecele" => "快速减速度",
                "FsLimit" => "正软限位",
                "RsLimit" => "负软限位",
                "DatumIn" => "原点信号",
                "ForwardIn" => "正限位信号",
                "ReverseIn" => "负限位信号",
                "ForwardJogIn" => "Jog正向信号",
                "ReverseJogIn" => "Jog负向信号",
                "FastJogIn" => "快速Jog信号",
                _ => name,
            };
        }

        public abstract void Initialize();

        public abstract void DefPos(double position = 0);

        public abstract string GetAxisState();

        public abstract bool Enable();

        public abstract void Disenable();

        public abstract void Stop(int mode);

        public abstract void Wait();

        public abstract void Datum(int mode);

        public abstract void Forward();

        public abstract void Reverse();

        public abstract void SingleRelativeMove(double distance);

        public abstract void SingleAbsoluteMove(double coord);
        #endregion
    }

    public abstract class MotionControl : Loader
    {
        [JsonPropertyOrder(0)]
        public string Ip { get; set; } = "127.0.0.1";
        [JsonPropertyOrder(1)]
        public string Name { get; set; } = "Controller";

        public abstract void Initialize();
        public abstract bool Connect();
        public abstract void Disconnect();
        public abstract bool IsConnected();
        public abstract void Scram();
        //信号
        public abstract double[] GetInputs(int number);
        public abstract void SetInput(int num, int invert);
        public abstract void SetOutput(int num, int value);

        public MotionControl() { }
    }
    #endregion

    #region Trio
    //public class TrioAxis : BaseAxis
    //{
    //    #region 状态
    //    private bool isMoving = false;
    //    public override bool IsMoving
    //    {
    //        get
    //        {
    //            trio.GetAxisParameter(AxisParameter.IDLE, Number, out double movingStatus);
    //            if (movingStatus == 0)
    //                isMoving = true;
    //            else if (movingStatus == -1)
    //                isMoving = false;
    //            return isMoving;
    //        }
    //        set
    //        {
    //            isMoving = value;
    //        }
    //    }
    //    private double targetPosition;
    //    public override double TargetPosition
    //    {
    //        get
    //        {
    //            trio.GetAxisVariable("DPOS", Number, out targetPosition); // 获取第numAxis个轴目标位置的参数
    //            targetPosition = Math.Round(targetPosition, 2); // 保留两位小数
    //            return targetPosition;
    //        }
    //        set { targetPosition = value; }
    //    }
    //    private double currentPosition;
    //    public override double CurrentPosition
    //    {
    //        get
    //        {
    //            trio.GetAxisParameter(AxisParameter.MPOS, Number, out currentPosition);
    //            //trio.GetAxisVariable("MPOS", Number, out currentPosition); // 获取第numAxis个轴的实际位置的参数
    //            currentPosition = Math.Round(currentPosition, 2); // 保留两位小数
    //            return currentPosition;
    //        }
    //        set { currentPosition = value; }
    //    }
    //    private double currentSpeed;
    //    public override double CurrentSpeed
    //    {
    //        get
    //        {
    //            trio.GetAxisParameter(AxisParameter.MSPEED, Number, out currentSpeed);
    //            //trio.GetAxisVariable("MSPEED", Number, out currentSpeed); // 获取第numAxis个轴的实际速度
    //            currentSpeed = Math.Round(currentSpeed, 2); // 保留两位小数
    //            return currentSpeed;
    //        }
    //        set { currentSpeed = value; }
    //    }
    //    #endregion

    //    readonly TrioPC trio;

    //    public TrioAxis(TrioPC instance, string axisName, int axisNumber, string controllerName)
    //    {
    //        trio = instance;
    //        Name = axisName;
    //        Number = axisNumber;
    //        ControllerName = controllerName;

    //        axisConfig = new KeyValueManager($"{Name}.json", $"Config\\{ControllerName}\\Axes");
    //        LoadAxisConfig();
    //    }

    //    #region 设置
    //    /// <summary>
    //    /// 初始化轴参数
    //    /// </summary>
    //    public override void Initialize()
    //    {
    //        trio.SetAxisParameter(AxisParameter.ATYPE, Number, Type);
    //        trio.SetAxisVariable("UNITS", Number, Units); //脉冲当量
    //        trio.SetAxisParameter(AxisParameter.SRAMP, Number, Sramp);
    //        trio.SetAxisVariable("SPEED", Number, Speed); // 设置轴速度
    //        trio.SetAxisVariable("CREEP", Number, Creep); // 设置爬行速度
    //        trio.SetAxisVariable("JOGSPEED", Number, JogSpeed); // 设置Jog速度
    //        trio.SetAxisVariable("ACCEL", Number, Accele); // 设置加速度
    //        trio.SetAxisVariable("DECEL", Number, Decele); // 设置减速度
    //        trio.SetAxisParameter(AxisParameter.FASTDEC, Number, FastDecele);
    //        trio.SetAxisVariable("FS_LIMIT", Number, FsLimit); // 设置正向软限位（绝对位置）
    //        trio.SetAxisVariable("RS_LIMIT", Number, RsLimit); // 设置反向软限位（绝对位置）

    //        trio.SetAxisVariable("DATUM_IN", Number, DatumIn); // 回原点输入为输入0
    //        trio.SetAxisVariable("FWD_IN", Number, ForwardIn); // 正向软限位输入为输入1
    //        trio.SetAxisVariable("REV_IN", Number, ReverseIn); // 正向软限位输入为输入2
    //        trio.SetAxisVariable("FWD_JOG", Number, ForwardJogIn); // 设置正向JOG运动的输入
    //        trio.SetAxisVariable("REV_JOG", Number, ReverseJogIn); // 设置反向JOG输入
    //        trio.SetAxisParameter(AxisParameter.FAST_JOG, Number, FastJogIn);

    //        trio.SetAxisVariable("FE_LIMIT", Number, 20000); // 设置跟随误差极大值
    //        trio.SetAxisVariable("FE_RANGE", Number, 10000); // 设置跟随误差报告范围
    //        //trio.SetAxisVariable("REP_DIST", Number, 200000000000); // 设置重复距离
    //        trio.SetAxisVariable("SERVO", Number, 1); // SERVO=1:进行闭环运动，SERVO=0：开环运动
    //        trio.SetAxisVariable("AXIS_ENABLE", Number, 1); // 轴使能（其实初始时，每个轴默认处于使能状态）
    //    }
    //    /// <summary>
    //    /// 定义当前位置
    //    /// </summary>
    //    /// <param name="position">当前位置</param>
    //    public override void DefPos(double position = 0)
    //    {
    //        trio.Defpos(position, Number);
    //        trio.Execute("WAIT UNTIL OFFPOS=0");//Ensures DEFPOS is complete before next line
    //    }
    //    /// <summary>
    //    /// 更新轴状态
    //    /// </summary>
    //    public override void UpdateState()
    //    {
    //        double axisState = -1;
    //        trio.GetAxisVariable("AXISSTATUS", Number, out axisState);
    //        int axis0State = (int)axisState;
    //        if (axis0State == 0)
    //        {
    //            State = "轴状态：正  常";
    //        }
    //        else if (((axis0State >> 2) & 1) == 1)
    //        {
    //            State = "轴状态：与伺服通讯错误！";
    //        }
    //        else if (((axis0State >> 3) & 1) == 1)
    //        {
    //            State = "轴状态：伺服错误！";
    //        }
    //        else if (((axis0State >> 4) & 1) == 1)
    //        {
    //            State = "轴状态：正向硬限位报警！";
    //        }
    //        else if (((axis0State >> 5) & 1) == 1)
    //        {
    //            State = "轴状态：负向硬限位报警！";
    //        }
    //        else if (((axis0State >> 8) & 1) == 1)
    //        {
    //            State = "轴状态：跟随误差超限出错！";
    //        }
    //        else if (((axis0State >> 9) & 1) == 1)
    //        {
    //            State = "轴状态：超过正向软限位报警！";
    //        }
    //        else if (((axis0State >> 10) & 1) == 1)
    //        {
    //            State = "轴状态：超过负向软限位报警！";
    //        }
    //        else
    //        {
    //            State = "";
    //        }
    //    }
    //    #endregion

    //    #region 运动控制
    //    /// <summary>
    //    /// 使能
    //    /// </summary>
    //    /// <returns>1为成功-1为失败</returns>
    //    public override bool Enable()
    //    {
    //        double value = -1;
    //        trio.SetVariable("WDOG", 1);
    //        trio.GetVariable("WDOG", out value);
    //        if (value == 1) return true;
    //        else return false;
    //    }
    //    /// <summary>
    //    /// 关闭使能
    //    /// </summary>
    //    public override void Disenable()
    //    {
    //        trio.SetVariable("WDOG", 0);
    //    }

    //    public override void Stop(int mode = 2)
    //    {
    //        trio.Cancel(mode, Number);// 取消轴0上的运动
    //    }

    //    public override void Wait()
    //    {
    //        //trio.Execute($"WAIT IDLE AXIS({Number})");
    //        Thread.Sleep(100);
    //        do
    //        {
    //            Thread.Sleep(50);
    //        } while (IsMoving);
    //    }

    //    public override void Datum(int mode = 3)
    //    {
    //        Stop(); // 取消numAxis轴上的运动
    //        trio.Datum(mode, Number); // 模式3:以SPEED速度正向回原点; 模式4:以SPEED速度反向回原点 （已经通过SetAxisVariable设置了回原点输入DATUM_IN）
    //    }

    //    public override void Forward()
    //    {
    //        if (Number >= 0)
    //        {
    //            Stop();
    //            trio.Forward(Number);
    //        }
    //    }

    //    public override void Reverse()
    //    {
    //        if (Number >= 0)
    //        {
    //            Stop();
    //            trio.Reverse(Number);
    //        }
    //    }

    //    public override void SingleRelativeMove(double distance)
    //    {
    //        if (Number >= 0)
    //        {
    //            double[] dist = new double[] { distance };
    //            trio.MoveRel(dist, Number);
    //        }
    //    }

    //    public override void SingleAbsoluteMove(double coord)
    //    {
    //        if (Number >= 0)
    //        {
    //            double[] dist = new double[] { coord };
    //            trio.MoveAbs(dist, Number);
    //        }
    //    }

    //    public void RelativeMove(double[] dist, int axes)
    //    {
    //        if (Number >= 0)
    //        {
    //            trio.MoveRel(dist, axes, Number);
    //        }
    //    }

    //    public void AbsoluteMove(double[] dist, int axes)
    //    {
    //        if (Number >= 0)
    //        {
    //            trio.MoveAbs(dist, axes, Number);
    //        }
    //    }
    //    #endregion
    //}

    //[JsonDerivedType(typeof(TrioMotionControl), typeDiscriminator: "Trio")]
    //public class TrioMotionControl : MotionControl
    //{
    //    public readonly TrioPC Trio = new TrioPC();

    //    public TrioMotionControl(string controllerName, string ip, params string[] axisName)
    //    {
    //        Name = controllerName;
    //        IP = ip;
    //        for (int i = 0; i < axisName.Length; i++)
    //        {
    //            AxesName.Add(axisName[i]);
    //            AddAxis(new TrioAxis(Trio, axisName[i], i, Name));
    //        }
    //    }

    //    public TrioMotionControl()
    //    {
    //        //ReinitializeAxes();
    //    }

    //    #region 设置
    //    public override void Initialize()
    //    {
    //        Trio.SetVariable("LIMIT_BUFFERED", 64);//运动缓存区设为64条指令
    //    }

    //    public override bool Connect()
    //    {
    //        Trio.HostAddress = IP;
    //        return Trio.Open(PortType.Ethernet, PortId.EthernetREMOTE);
    //    }

    //    public override void Disconnect()
    //    {
    //        Trio.Close(PortId.EthernetREMOTE);
    //    }

    //    public override bool IsConnected()
    //    {
    //        return Trio.IsOpen(PortId.EthernetREMOTE);
    //    }

    //    public override void ReinitializeAxes()
    //    {
    //        Axes.Clear();
    //        for (int i = 0; i < AxesName.Count; i++)
    //            AddAxis(new TrioAxis(Trio, AxesName[i], i, Name));
    //    }

    //    public bool AddAxis(TrioAxis axis)
    //    {
    //        if (!Axes.ContainsKey(axis.Name))
    //        {
    //            Axes.Add(axis.Name, axis);
    //            return true;
    //        }
    //        else
    //            return false;
    //    }

    //    public override void AddAxis(string axisName)
    //    {
    //        if (AddAxis(new TrioAxis(Trio, axisName, Axes.Count, Name)))
    //        {
    //            //添加轴信息
    //            AxesName.Add(axisName);
    //        }
    //    }
    //    #endregion

    //    #region 控制
    //    /// <summary>
    //    /// 全部停止
    //    /// </summary>
    //    public override void Scram()
    //    {
    //        Trio.RapidStop(); // 取消所有的轴的当前运动
    //    }

    //    public void Wait()
    //    {
    //        Trio.Execute("WAIT IDLE");
    //    }

    //    public void Wait(int millisecond)
    //    {
    //        Trio.Execute($"WA({millisecond})"); //等待 20ms
    //    }
    //    #endregion

    //    #region 状态检测
    //    public bool ConnectState()
    //    {
    //        return Trio.IsOpen(PortId.EthernetREMOTE);
    //    }

    //    public bool EnableState()
    //    {
    //        Trio.GetVariable("WDOG", out double value);
    //        if (value == 1) return true;
    //        else return false;
    //    }

    //    public double ECState()
    //    {
    //        double state = -1;
    //        // 判断EC状态，若异常则进行相应的操作
    //        Trio.SetVr(0, -1); // 初始化VR(0)=-1，用于存放EC的状态
    //        Trio.Execute("ETHERCAT($22,0,0)"); // 将EtherCAT的状态返回到VR(0)中。EtherCAT指令详见Trio BASIC
    //        Trio.GetVr(0, out state);
    //        int i = 0;
    //        while (state != 3 && (i < 3))// 控制器未连接驱动器，则重新初始化EC
    //        {
    //            Trio.Execute("ETHERCAT(0,0)"); // 重新初始化EC
    //            Thread.Sleep(3000);
    //            Trio.Execute("ETHERCAT($22,0,0)");
    //            Trio.GetVr(0, out state);
    //            i++;
    //        }
    //        return state;
    //    }
    //    #endregion

    //    #region IO
    //    public override double[] GetInputs(int number)
    //    {
    //        double[] ioState = new double[number];
    //        for (int i = 0; i < number; i++)
    //        {
    //            Trio.In(i, i, out ioState[i]);
    //        }
    //        return ioState;
    //    }

    //    public override void SetInput(int num, int invert)
    //    {
    //        Trio.InvertIn(num, invert);
    //    }

    //    public override void SetOutput(int num, int value)
    //    {
    //        Trio.Op(num, num, value);
    //    }
    //    #endregion
    //}
    #endregion

    #region Zmotion
    public class ZmotionAxis : BaseAxis
    {
        #region 状态
        private string state = "";
        [JsonIgnore]
        public override string State
        {
            get
            {
                int stateNumber = 0;
                Zmcaux.ZAux_Direct_GetAxisStatus(Handle, Number, ref stateNumber);
                state = stateNumber switch
                {
                    2 => "随动误差超限告警",
                    4 => "与远程轴通讯出错",
                    8 => "远程驱动器报错",
                    16 => "正向硬限位",
                    32 => "反向硬限位",
                    64 => "找原点中",
                    128 => "HOLD 速度保持信号输入",
                    256 => "随动误差超限出错",
                    512 => "超过正向软限位",
                    1024 => "超过负向软限位",
                    2048 => "CANCEL 执行中",
                    4096 => "脉冲频率超过 MAX_SPEED 限制需要修改降速或修改MAX_SPEED",
                    16384 => "机械手指令坐标错误",
                    262144 => "电源异常",
                    1048576 => "轴速度保护",
                    2097152 => "运动中触发特殊运动指令失败",
                    4194304 => "告警信号输入",
                    8388608 => "轴进入了暂停状态",
                    _ => "",
                };
                return state;
            }
            set { state = value; }
        }
        private bool isMoving = false;
        [JsonIgnore]
        public override bool IsMoving
        {
            get
            {
                int movingStatus = -1;
                Zmcaux.ZAux_Direct_GetIfIdle(Handle, Number, ref movingStatus);
                if (movingStatus == 0)
                    isMoving = true;
                else if (movingStatus == -1)
                    isMoving = false;
                return isMoving;
            }
            set
            {
                isMoving = value;
            }
        }
        private float targetPosition;
        [JsonIgnore]
        public override double TargetPosition
        {
            get
            {
                Zmcaux.ZAux_Direct_GetDpos(Handle, Number, ref targetPosition);
                targetPosition = (float)Math.Round(targetPosition / (float)Units, 2); // 保留两位小数
                return targetPosition;
            }
            set { targetPosition = (float)value; }
        }
        private double currentPosition;
        [JsonIgnore]
        public override double CurrentPosition
        {
            get
            {
                float position = 0;
                Zmcaux.ZAux_Direct_GetMpos(Handle, Number, ref position);
                currentPosition = Math.Round(position / Units, 2); // 保留两位小数
                return currentPosition;
            }
            set { currentPosition = value; }
        }
        private double currentSpeed;
        [JsonIgnore]
        public override double CurrentSpeed
        {
            get
            {
                float speed = 0;
                Zmcaux.ZAux_Direct_GetMspeed(Handle, Number, ref speed);
                currentSpeed = Math.Round(speed / Units, 2); // 保留两位小数
                return currentSpeed;
            }
            set { currentSpeed = value; }
        }
        #endregion

        public IntPtr Handle;

        public ZmotionAxis(IntPtr handle, string controllerName, string axisName, int axisNumber)
        {
            Handle = handle;
            ControllerName = controllerName;
            Name = axisName;
            Number = axisNumber;
        }

        public ZmotionAxis()
        {

        }

        #region 设置
        public override void Initialize()
        {
            //Zmcaux.ZAux_Direct_SetInvertStep(Handle, Number, 256 * 100 + 0);
            Zmcaux.ZAux_Direct_SetAtype(Handle, Number, (int)Type);
            Zmcaux.ZAux_Direct_SetUnits(Handle, Number, (float)Units);
            Zmcaux.ZAux_Direct_SetSramp(Handle, Number, (float)Sramp * (float)Units);
            Zmcaux.ZAux_Direct_SetSpeed(Handle, Number, (float)Speed * (float)Units);
            Zmcaux.ZAux_Direct_SetCreep(Handle, Number, (float)Creep * (float)Units);
            Zmcaux.ZAux_Direct_SetJogSpeed(Handle, Number, (float)JogSpeed * (float)Units);
            Zmcaux.ZAux_Direct_SetAccel(Handle, Number, (float)Accele * (float)Units);
            Zmcaux.ZAux_Direct_SetDecel(Handle, Number, (float)Decele * (float)Units);
            Zmcaux.ZAux_Direct_SetFastDec(Handle, Number, (float)FastDecele * (float)Units);
            Zmcaux.ZAux_Direct_SetFsLimit(Handle, Number, (float)FsLimit * (float)Units);
            Zmcaux.ZAux_Direct_SetRsLimit(Handle, Number, (float)RsLimit * (float)Units);

            Zmcaux.ZAux_Direct_SetDatumIn(Handle, Number, (int)DatumIn);
            Zmcaux.ZAux_Direct_SetFwdIn(Handle, Number, (int)ForwardIn);
            Zmcaux.ZAux_Direct_SetRevIn(Handle, Number, (int)ReverseIn);
            Zmcaux.ZAux_Direct_SetFwdJog(Handle, Number, (int)ForwardJogIn);
            Zmcaux.ZAux_Direct_SetRevJog(Handle, Number, (int)ReverseJogIn);
            Zmcaux.ZAux_Direct_SetFastJog(Handle, Number, (int)FastJogIn);

            //Zmcaux.ZAux_Direct_SetLspeed(Handle, Number, Convert.ToSingle(arg[6]) * Units);
        }

        public override void DefPos(double position = 0)
        {
            Zmcaux.ZAux_Direct_Defpos(Handle, Number, (float)position);
        }

        public override string GetAxisState()
        {
            string message = "";
            message += $"{State}{Environment.NewLine}";
            message += $"当前位置：{CurrentPosition}{Environment.NewLine}";
            message += $"当前速度：{CurrentSpeed}{Environment.NewLine}";
            return message;
        }
        #endregion

        #region 运动控制
        /// <summary>
        /// 使能
        /// </summary>
        /// <returns>1为成功-1为失败</returns>
        public override bool Enable()
        {
            int result = Zmcaux.ZAux_Direct_SetAxisEnable(Handle, Number, 1);
            if (result == 0) return true;
            else return false;
        }
        /// <summary>
        /// 关闭使能
        /// </summary>
        public override void Disenable()
        {
            int result = Zmcaux.ZAux_Direct_SetAxisEnable(Handle, Number, 0);
        }

        public override void Stop(int mode)
        {
            Zmcaux.ZAux_Direct_Single_Cancel(Handle, Number, mode);
        }

        public override void Wait()
        {
            Thread.Sleep(100);
            do
            {
                Thread.Sleep(50);
            } while (IsMoving);
        }

        public override void Datum(int mode = 3)
        {
            Zmcaux.ZAux_Direct_Single_Datum(Handle, Number, mode);
        }

        public override void Forward()
        {
            Zmcaux.ZAux_Direct_Single_Vmove(Handle, Number, 1);
        }

        public override void Reverse()
        {
            Zmcaux.ZAux_Direct_Single_Vmove(Handle, Number, -1);
        }

        public override void SingleRelativeMove(double distance)
        {
            Zmcaux.ZAux_Direct_Single_Move(Handle, Number, (float)distance * (float)Units);
        }

        public override void SingleAbsoluteMove(double coord)
        {
            Zmcaux.ZAux_Direct_Single_MoveAbs(Handle, Number, (float)coord * (float)Units);
        }
        #endregion

        #region 总线指令
        public void CMDDatum(uint mode = 17)
        {
            Zmcaux.ZAux_BusCmd_Datum(Handle, (uint)Number, mode);
        }

        public bool SetTorque(float targetTorque, int maxRotateSpeed, int maxTorque = 1000)
        {
            int ret = 0;
            ret += Zmcaux.ZAux_BusCmd_SDOWriteAxis(Handle, (uint)Number, 0x6080, 0, 7, maxRotateSpeed);
            ret += Zmcaux.ZAux_BusCmd_SDOWriteAxis(Handle, (uint)Number, 0x6072, 0, 7, maxTorque);
            ret += Zmcaux.ZAux_Direct_SetDAC(Handle, (uint)Number, targetTorque);
            if (ret == 0) return true;
            else return false;
        }

        public int GetTorque()
        {
            int currentTorque = -1;
            Zmcaux.ZAux_BusCmd_GetDriveTorque(Handle, (uint)Number, ref currentTorque);
            return currentTorque;
        }

        public bool DriveClear()
        {
            int result = Zmcaux.ZAux_BusCmd_DriveClear(Handle, (uint)Number, 0);
            if (result == 0) return true;
            else return false;
        }
        #endregion
    }

    [JsonDerivedType(typeof(ZmotionMotionControl), typeDiscriminator: "Zmotion")]
    public class ZmotionMotionControl : MotionControl
    {
        [JsonPropertyOrder(2)]
        public Dictionary<string, ZmotionAxis> Axes { get; set; } = [];

        private BindingList<string> axesName = [];
        [JsonPropertyOrder(3)]
        public BindingList<string> AxesName
        {
            get { return axesName; }
            set
            {
                axesName = value;
                for (int i = 0; i < axesName.Count; i++)
                    AddAxis(new ZmotionAxis(Zmotion, Name, axesName[i], i), false);//加载轴（在Axes加载后，如缺失则补充轴实例）
            }
        }

        public IntPtr Zmotion;
        public int ErrorCode;

        public ZmotionMotionControl(string ip, string controllerName, params string[] axisName)
        {
            Ip = ip;
            Name = controllerName;

            for (int i = 0; i < axisName.Length; i++)
                AddAxis(new ZmotionAxis(Zmotion, Name, axisName[i], i));
        }

        public ZmotionMotionControl()
        {

        }

        private void UpdateAxisHandle(IntPtr zmotion)
        {
            foreach (var axis in Axes.Values)
            {
                axis.Handle = zmotion;
            }
        }

        #region 设置
        public override void Initialize()
        {
            //连接成功后所有轴参数重新初始化
            foreach (var axis in Axes.Values)
                axis.Initialize();
        }

        public void UploadBasFile(string filename, uint mode = 1)
        {
            ErrorCode += Zmcaux.ZAux_BasDown(Zmotion, filename, mode);
        }

        public void ECInitialize()
        {
            ErrorCode += Zmcaux.ZAux_BusCmd_InitBus(Zmotion);
        }

        public void ReECInitialize()
        {
            StringBuilder buffer = new(10240);
            ErrorCode += Zmcaux.ZAux_Execute(Zmotion, "RUNTASK 1,Ecat_Init", buffer, 0);
        }

        public override bool Connect()
        {
            //链接控制器 
            ErrorCode = Zmcaux.ZAux_OpenEth(Ip, out Zmotion);
            UpdateAxisHandle(Zmotion);
            if (Zmotion != (IntPtr)0)
                return true;
            else
                return false;
        }

        public override void Disconnect()
        {
            ErrorCode = Zmcaux.ZAux_Close(Zmotion);
            Zmotion = (IntPtr)0;
            UpdateAxisHandle(Zmotion);
        }

        public override bool IsConnected()
        {
            if (Zmotion == (IntPtr)0) return false;
            return true;
        }
        /// <summary>
        /// 添加轴
        /// </summary>
        /// <param name="axis">轴实例</param>
        /// <param name="isAddName">是否添加轴名称到绑定列表</param>
        /// <returns></returns>
        public bool AddAxis(ZmotionAxis axis, bool isAddName = true)
        {
            if (Axes.TryAdd(axis.Name, axis))
            {
                if (isAddName)
                    AxesName.Add(axis.Name);
                return true;
            }
            else return false;
        }
        /// <summary>
        /// 移除轴
        /// </summary>
        /// <param name="axisName">移除的轴名称</param>
        /// <param name="isRemoveName">是否从绑定列表移除轴名称</param>
        /// <returns></returns>
        public bool RemoveAxis(string axisName, bool isRemoveName = true)
        {
            if (Axes.Remove(axisName))
            {
                if (isRemoveName)
                    AxesName.Remove(axisName);
                return true;
            }
            else return false;
        }

        public bool AddAxis(string axisName, int number)
        {
            var axis = new ZmotionAxis(Zmotion, Name, axisName, number);
            return AddAxis(axis);
        }
        #endregion

        #region 控制
        /// <summary>
        /// 全部停止
        /// </summary>
        public override void Scram()
        {
            ErrorCode = Zmcaux.ZAux_Direct_Rapidstop(Zmotion, 0);
        }
        #endregion

        #region IO
        public override double[] GetInputs(int number)
        {
            double[] inputs = new double[number];
            for (int i = 0; i < number; i++)
            {
                inputs[i] = GetInput(i);
            }
            return inputs;
        }

        public override void SetInput(int num, int invert)
        {
            ErrorCode = Zmcaux.ZAux_Direct_SetInvertIn(Zmotion, num, invert);
        }

        public uint GetInput(int num)
        {
            uint value = 2;
            ErrorCode = Zmcaux.ZAux_Direct_GetIn(Zmotion, num, ref value);
            return value;
        }

        public override void SetOutput(int num, int value)
        {
            ErrorCode = Zmcaux.ZAux_Direct_SetOp(Zmotion, num, (uint)value);
        }

        public uint GetOutput(int num)
        {
            uint value = 2;
            ErrorCode = Zmcaux.ZAux_Direct_GetOp(Zmotion, num, ref value);
            return value;
        }
        #endregion

    }
    #endregion

}
