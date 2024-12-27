using CSharpKit.Communication;
using CSharpKit.DataManagement;
using CSharpKit.FileManagement;

namespace Calibration.Services
{
    public class PressController : ParameterManager
    {
        //网口
        public TcpComm Connection = new();

        public PressController()
        {
            SocketPara = new();
        }

        public string SendCommand(string command)
        {
            if (Connection != null && Connection.IsOnline())
            {
                Connection.SendMsg(command, out string recvStr);
                return recvStr;
            }
            return "";
        }

        public bool Connect()
        {
            bool rt = Connection.ConnectServer(SocketPara!.Ip, SocketPara.Port);
            if (rt)
            {
                Connection.SendMsg(":REM", out string recvStr);
            }
            return rt;
        }

        public void Disconnect()
        {
            SendCommand(":LOC");
            Connection.Close();
        }

        //设置压力
        public void SetPress(decimal press)
        {
            SendCommand($":SOUR {press}");//控制模式
            Thread.Sleep(500);
            SendCommand(":OUTP1 1");//控制模式
        }

        public decimal GetPress(int count = 5)
        {
            decimal press = 0;
            for (int i = 0; i < count; i++)
            {
                if (Connection != null && Connection.IsOnline())
                {
                    //tcpComm.SendMsg(":OUTP1 0", out recvStr);//测量模式
                    Connection.SendMsg(":SENS:PRES:PSE?", out string recvStr);
                    if (recvStr.Split(' ').Length == 2)
                        press = Convert.ToDecimal(recvStr.Split(' ')[1]);
                }
                if (press != 0) break;
                Thread.Sleep(1000);
            }
            return press;
        }

        public bool TryGetPressure(decimal targetPressure, out decimal currentPress, decimal maxPressureDiff = 10m, int waitTime = 5)
        {
            int count = 0;
            while (true)
            {
                decimal? result = GetPress();
                if (result != null && Math.Abs(result.Value - targetPressure) <= maxPressureDiff)
                    count++;
                else
                    count--;
                Thread.Sleep(1000);
                if (count == waitTime)
                {
                    currentPress = result!.Value;
                    return true;
                }
                if (count == -1 * waitTime)
                {
                    currentPress = 0;
                    return false;
                }
            }
        }

        //通大气
        /*
         * :SOUR1:VENT 1通大气
         * 
         * 
         * 
         * :SOUR1:VENT 0  停止通大气
         * 
         * 
         * 
         * :SOUR1:VENT?
         *  0 - vent OK
            1 - vent in progress
            2 - vent completed
         * 
         *     
         */
        public void Vent()
        {
            SendCommand(":SOUR1:VENT 1");
        }

        public string SelectMode(int mode = 0)
        {
            return SendCommand($":OUTP1 {mode}");
        }
    }

    public class TECController : ParameterManager
    {
        public byte DeviceAddress { get; set; }

        //串口连接
        public SerialPortTool Connection = new();

        public TECController()
        {
            SerialPara = new();
        }

        public override string Translate(string name)
        {
            return name switch
            {
                nameof(DeviceAddress) => "设备地址",
                _ => "",
            };
        }

        public bool Open()
        {
            return Connection.OpenMySerialPort(SerialPara!);
        }

        public bool Close()
        {
            return Connection.CloseMySerialPort();
        }

        #region 寄存器操作
        public byte[] GetRegisters(byte deviceAddress, byte code, ushort registerAddress, ushort registerLength)
        {
            byte[] address = DataConverter.ValueToBytes(registerAddress);
            byte[] length = DataConverter.ValueToBytes(registerLength);
            byte[] sendData = [deviceAddress, code, address[0], address[1], length[0], length[1]];
            byte[] crcBytes = CRC16.CRC16_1(sendData);
            return Connection.SendWithRead(crcBytes, 5 + registerLength * 2);
        }

        public bool SetRegister(byte deviceAddress, ushort registerAddress, byte[] data)
        {
            byte[] address = DataConverter.ValueToBytes(registerAddress);
            byte[] head = [deviceAddress, 0x06, address[0], address[1]];
            byte[] sendData = BytesTool.SpliceBytes(head, data);
            byte[] crcBytes = CRC16.CRC16_1(sendData);
            byte[] received = Connection.SendWithRead(crcBytes, crcBytes.Length);
            if (received.Length == 0) return false;
            return true;
        }

        public bool SetRegisters(byte deviceAddress, ushort registerAddress, ushort registerLength, byte[] data)
        {
            byte[] address = DataConverter.ValueToBytes(registerAddress);
            byte[] length = DataConverter.ValueToBytes(registerLength);
            byte[] dataLength = DataConverter.ValueToBytes(registerLength * 2);
            byte[] head = [deviceAddress, 0x10, address[0], address[1], length[0], length[1], dataLength[3]];
            byte[] sendData = BytesTool.SpliceBytes(head, data);
            byte[] crcBytes = CRC16.CRC16_1(sendData);
            byte[] received = Connection.SendWithRead(crcBytes, 8);
            if (received.Length == 0) return false;
            return true;
        }

        public bool SetOnOff(ushort registerAddress, bool onOff = false)
        {
            if (onOff)
                return SetRegister(DeviceAddress, registerAddress, [0x00, 0x01]);
            else
                return SetRegister(DeviceAddress, registerAddress, [0x00, 0x00]);
        }

        public bool SetValue(ushort registerAddress, short value)
        {
            return SetRegister(DeviceAddress, registerAddress, DataConverter.ValueToBytes(value));
        }

        public bool SetValue(ushort registerAddress, ushort value)
        {
            return SetRegister(DeviceAddress, registerAddress, DataConverter.ValueToBytes(value));
        }
        #endregion

        #region 控制指令
        public string GetInfo()
        {
            byte[]? info = null;
            if (Connection.MySerialPort.IsOpen)
                info = GetRegisters(DeviceAddress, 0x04, 30000, 12);
            info ??= new byte[27];
            float ntc1 = BitConverter.ToInt16([info[4], info[3]]) / 100f;
            float currentWaterDischarge = BitConverter.ToUInt16([info[8], info[7]]) / 100f;
            float ntc1Set = BitConverter.ToInt16([info[10], info[9]]) / 100f;
            ushort ntc1Switch = BitConverter.ToUInt16([info[14], info[13]]);
            ushort pumpSwitch = BitConverter.ToUInt16([info[18], info[17]]);
            byte[] alarm = [info[19], info[20]];
            bool ntc1Alarm = BytesTool.GetBit(info[20], 0);
            bool waterDischargeAlarm = BytesTool.GetBit(info[20], 2);
            bool waterAlarm = BytesTool.GetBit(info[20], 3);
            short tec1PID = BitConverter.ToInt16([info[22], info[21]]);
            ushort waterDischargePulse = BitConverter.ToUInt16([info[26], info[25]]);

            return $"NTC:{ntc1}℃        水流量:{currentWaterDischarge}L{Environment.NewLine}" +
                $"NTC设置:{ntc1Set}℃{Environment.NewLine}" +
                $"NTC温控开关:{ntc1Switch}    水泵开关:{pumpSwitch}{Environment.NewLine}" +
                $"TECPID:{tec1PID}{Environment.NewLine}" +
                $"NTC报警:{ntc1Alarm}    水流量报警:{waterDischargeAlarm}{Environment.NewLine}" +
                $"水流报警:{waterAlarm}    水流量脉冲:{waterDischargePulse}";
        }

        /// <summary>
        /// 设置设备地址
        /// </summary>
        /// <param name="address">1-254</param>
        /// <returns></returns>
        public bool SetDeviceAddress(ushort address)
        {
            if (address >= 1 && address <= 254)
                return SetValue(40000, address);
            return false;
        }
        /// <summary>
        /// 继电器模式
        /// </summary>
        /// <param name="mode">0手动1水冷2风冷</param>
        /// <returns></returns>
        public bool RelayMode(ushort mode)
        {
            return SetValue(40005, mode);
        }
        /// <summary>
        /// 最大水流量
        /// </summary>
        /// <param name="value">2500为25.00L</param>
        /// <returns></returns>
        public bool MaxWaterDischarge(ushort value)
        {
            return SetValue(40006, value);
        }
        /// <summary>
        /// 最小水流量
        /// </summary>
        /// <param name="value">50为0.50L</param>
        /// <returns></returns>
        public bool MinWaterDischarge(ushort value)
        {
            return SetValue(40007, value);
        }
        /// <summary>
        /// 流量修正系数
        /// </summary>
        /// <param name="value">1000=1.000</param>
        /// <returns></returns>
        public bool WaterDischargeCorrection(ushort value)
        {
            return SetValue(40008, value);
        }
        /// <summary>
        /// NTC升温设置
        /// </summary>
        /// <param name="value">10=10℃/m</param>
        /// <returns></returns>
        public bool NTCIncrease(short value)
        {
            return SetValue(40009, value);
        }

        /// <summary>
        /// TEC开关
        /// </summary>
        /// <param name="onOff">开关</param>
        /// <returns></returns>
        public bool TECOnOff(bool onOff = false)
        {
            return SetOnOff(40040, onOff);
        }
        /// <summary>
        /// 水泵开关
        /// </summary>
        /// <param name="onOff">开关</param>
        /// <returns></returns>
        public bool PumpOnOff(bool onOff = false)
        {
            return SetOnOff(40042, onOff);
        }
        /// <summary>
        /// 温度报警开关
        /// </summary>
        /// <param name="onOff"></param>
        /// <returns></returns>
        public bool NTCAlarm(bool onOff = false)
        {
            return SetOnOff(40043, onOff);
        }
        /// <summary>
        /// 水流量报警
        /// </summary>
        /// <param name="onOff"></param>
        /// <returns></returns>
        public bool WaterDischargeAlarm(bool onOff = false)
        {
            return SetOnOff(40045, onOff);
        }
        /// <summary>
        /// 水量报警
        /// </summary>
        /// <param name="onOff"></param>
        /// <returns></returns>
        public bool WaterVolumeAlarm(bool onOff = false)
        {
            return SetOnOff(40046, onOff);
        }

        /// <summary>
        /// 温度设置
        /// </summary>
        /// <param name="temp">温度，2500为25℃</param>
        /// <returns></returns>
        public bool SetTemp(short temp = 2500)
        {
            return SetValue(40100, temp);
        }
        /// <summary>
        /// 输出电压设置
        /// </summary>
        /// <param name="value">1200=12V</param>
        /// <returns></returns>
        public bool SetVoltage(ushort value = 1200)
        {
            return SetValue(40101, value);
        }
        /// <summary>
        /// 补偿温度设置
        /// </summary>
        /// <param name="value">0000=0.00℃</param>
        /// <returns></returns>
        public bool SetCompensationTemp(short value = 0)
        {
            return SetValue(40102, value);
        }
        /// <summary>
        /// 最大温度设置
        /// </summary>
        /// <param name="value">12000=120.00℃</param>
        /// <returns></returns>
        public bool SetMaxTemp(short value = 12000)
        {
            return SetValue(40103, value);
        }
        /// <summary>
        /// 最小温度设置
        /// </summary>
        /// <param name="value">-4000=-40.00℃</param>
        /// <returns></returns>
        public bool SetMinTemp(short value = -4000)
        {
            return SetValue(40104, value);
        }
        /// <summary>
        /// B值设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetBValue(ushort value = 3435)
        {
            return SetValue(40105, value);
        }
        /// <summary>
        /// 设置Rt25
        /// </summary>
        /// <param name="value">10K</param>
        /// <returns></returns>
        public bool SetRt25(ushort value = 10)
        {
            return SetValue(40106, value);
        }
        /// <summary>
        /// P值设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetPValue(ushort value = 1100)
        {
            return SetValue(40107, value);
        }
        /// <summary>
        /// I值设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetIValue(ushort value = 400)
        {
            return SetValue(40108, value);
        }
        /// <summary>
        /// D值设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetDValue(ushort value = 200)
        {
            return SetValue(40109, value);
        }

        public bool SaveSetting()
        {
            return SetRegister(DeviceAddress, 40001, [0x08, 0x0A]);
        }

        public bool ClearAlarm()
        {
            return SetRegister(DeviceAddress, 40001, [0x14, 0x01]);
        }
        #endregion
    }

}
