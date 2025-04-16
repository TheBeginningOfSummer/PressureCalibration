using CSharpKit.Communication;
using CSharpKit.DataManagement;

namespace Module
{
    public abstract class Sensor
    {
        #region 数据
        public virtual List<RawData> RawDataList { get; set; } = [];
        public virtual List<Validation> ValidationDataList { get; set; } = [];
        public ICoefficient? CoefficientData { get; set; }
        #endregion

        #region 参数
        public byte DeviceAddress { get; set; }//所在采集卡的地址
        public int SensorIndex { get; set; }//采集卡中的哪一路
        public int Uid { get; set; } = 0;

        private string result = "Default";
        public string Result
        {
            get { return result; }
            set
            {
                if (value == "Default")
                {
                    //结果初始化
                    result = value;
                }
                else if (value == "NG")
                {
                    if (result == "Default")
                    {
                        //SingleYield = SingleYield * 100 / 101;
                        result = value;
                    }
                }
                else if (value == "GOOD")
                {
                    if (result == "Default")
                    {
                        //SingleYield = (SingleYield * 100 + 1) / 101;
                        result = value;
                    }
                }
                else
                {
                    result += value;
                }
            }
        }//测试结果
        public bool IsFused { get; set; } = false;//是否烧录过
        public List<decimal> OutputP { get; set; } = [];
        public List<decimal> OutputT { get; set; } = [];
        #endregion

        public byte I2CAddress = 0x7F;
        public byte Speed = 0x00;
        public Label SensorInfo = new()
        {
            AutoSize = false,
            Size = new Size(55, 55),
            BackColor = Color.Gray,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 9)
        };

        public void SetLabelLoc(Point point)
        {
            SensorInfo.Location = point;
        }

        public void SetSensorInfo(decimal t, decimal p)
        {
            if (SensorInfo.IsHandleCreated)
                SensorInfo.Invoke(() => SensorInfo.Text = $"[{SensorIndex + 1}]{Environment.NewLine}{t:N2}℃{Environment.NewLine}{p:N2}");
            else
                SensorInfo.Text = $"[{SensorIndex + 1}]{Environment.NewLine}{t:N2}℃{Environment.NewLine}{p:N2}";
        }

        public void SetSensorColor(decimal t, decimal targetT = 15, decimal offset = 1)
        {
            Color color;
            if (t > targetT + Math.Abs(offset))
                color = Color.Red;
            else if (t < targetT - Math.Abs(offset))
                color = Color.LightSkyBlue;
            else
                color = Color.Orange;
            if (SensorInfo.IsHandleCreated)
                SensorInfo.Invoke(() => SensorInfo.BackColor = color);
            else
                SensorInfo.BackColor = color;
        }

        #region 寄存器操作
        public SendData Read(byte address, byte count)
        {
            byte channelByte1 = 0x00;
            byte channelByte2 = 0x00;
            byte[] sendBytes;
            SendData sendData;
            if (SensorIndex >= 0 && SensorIndex < 8)
            {
                channelByte1 = 0x00;
                channelByte2 = BytesTool.SetBit(channelByte2, (ushort)SensorIndex, true);
            }
            else if (SensorIndex >= 8 && SensorIndex < 16)
            {
                channelByte1 = BytesTool.SetBit(channelByte1, (ushort)(SensorIndex - 8), true);
                channelByte2 = 0x00;
            }
            else
            {
                channelByte1 = 0xFF;
                channelByte2 = 0xFF;
                sendBytes = [0x24, DeviceAddress, 0x80, 0x20, channelByte1, channelByte2, Speed, I2CAddress, address, count];
                sendData = new(CRC16.CRC16_1(sendBytes), 12 + 16 * count);
                return sendData;
            }
            sendBytes = [0x24, DeviceAddress, 0x80, 0x20, channelByte1, channelByte2, Speed, I2CAddress, address, count];
            sendData = new(CRC16.CRC16_1(sendBytes), 12 + count);
            return sendData;
        }
        public SendData Write(byte address, byte count, byte[] data)
        {
            byte channelByte1 = 0x00;
            byte channelByte2 = 0x00;
            SendData sendData;
            if (SensorIndex >= 0 && SensorIndex < 8)
            {
                channelByte1 = 0x00;
                channelByte2 = BytesTool.SetBit(channelByte2, (ushort)SensorIndex, true);
            }
            else if (SensorIndex >= 8 && SensorIndex < 16)
            {
                channelByte1 = BytesTool.SetBit(channelByte1, (ushort)(SensorIndex - 8), true);
                channelByte2 = 0x00;
            }
            else
            {
                channelByte1 = 0xFF;
                channelByte2 = 0xFF;
            }

            byte[] header = [0x24, DeviceAddress, 0x80, 0x30, channelByte1, channelByte2, Speed, I2CAddress, address, count];
            byte[] sendBytes = BytesTool.SpliceBytes(header, data);
            sendData = new(CRC16.CRC16_1(sendBytes), 12);
            return sendData;
        }
        public SendData Fuse(byte address = 0x34, byte count = 28)
        {
            byte channelByte1 = 0x00;
            byte channelByte2 = 0x00;
            SendData sendData;
            if (SensorIndex >= 0 && SensorIndex < 8)
            {
                channelByte1 = 0x00;
                channelByte2 = BytesTool.SetBit(channelByte2, (ushort)SensorIndex, true);
            }
            else if (SensorIndex >= 8 && SensorIndex < 16)
            {
                channelByte1 = BytesTool.SetBit(channelByte1, (ushort)(SensorIndex - 8), true);
                channelByte2 = 0x00;
            }
            else
            {
                channelByte1 = 0xFF;
                channelByte2 = 0xFF;
            }
            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x31, channelByte1, channelByte2, Speed, I2CAddress, address, count];
            sendData = new SendData(CRC16.CRC16_1(sendBytes), 12);
            return sendData;
        }
        #endregion

        public Sensor()
        {

        }

        public void SetUID()
        {
            foreach (var data in RawDataList)
                data.Uid = Uid;
            foreach (var data in ValidationDataList)
                data.Uid = Uid;
        }

        public void InitializeData(decimal[] tPoints, decimal[] pPoints)
        {
            Result = "Default";
            IsFused = false;
            RawDataList.Clear();
            ValidationDataList.Clear();
            for (int i = 0; i < tPoints.Length; i++)
            {
                for (int j = 0; j < pPoints.Length; j++)
                {
                    RawData data = new()
                    {
                        T_idx = i + 1,
                        P_idx = j + 1,
                        SetT = tPoints[i],
                        SetP = pPoints[j]
                    };
                    RawDataList.Add(data);
                }
            }
        }
        /// <summary>
        /// 显示数据
        /// </summary>
        public string ShowData()
        {
            string message = "";
            message += "计算数据：" + Environment.NewLine;
            message += CoefficientData?.Show() + Environment.NewLine;
            message += "    采集的数据：" + Environment.NewLine;
            for (int i = 0; i < RawDataList.Count; i++)
                message += $"[{i}]{RawDataList[i].Show()}{Environment.NewLine}";
            message += "    验证的数据：" + Environment.NewLine;
            for (int i = 0; i < ValidationDataList.Count; i++)
                message += $"[{i}]{ValidationDataList[i].Show()}{Environment.NewLine}";
            message += "---------------------------------------------------------------------------------------------------" + Environment.NewLine;
            return message;
        }

        public abstract void Calculate();
        public abstract void Validate(double maxPDiff = 10, double maxTDiff = 0.5);
        
    }

    public class SensorBOE2520 : Sensor
    {
        public SensorBOE2520(byte deviceAddress, int sensorIndex, byte i2CAddress = 0x20, byte speed = 0x00)
        {
            DeviceAddress = deviceAddress;
            SensorIndex = sensorIndex;
            I2CAddress = i2CAddress;
            Speed = speed;
            SensorInfo.Name = $"LB[D{DeviceAddress}S{SensorIndex}]";
            SensorInfo.Tag = $"D{DeviceAddress}S{SensorIndex}";
            SensorInfo.Text = $"[{SensorIndex + 1}]";
        }

        #region 数据采集(不使用)
        // 得到对应传感器的温度测量值的索引值
        private int GetTempIndex()
        {
            if (SensorIndex >= 4 && SensorIndex <= 7) return 1;
            if (SensorIndex >= 8 && SensorIndex <= 11) return 2;
            if (SensorIndex >= 12 && SensorIndex <= 15) return 3;
            return 0;
        }
        //得到当前传感器的温度
        public decimal ReadTemperature(SerialPortTool connection)
        {
            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x21, 0x00, 0x0F, 0x00, 0x48, 0x00, 0x02];
            var result = connection.WriteRead(CRC16.CRC16_1(sendBytes), 20);
            short v1 = 0, v2 = 0, v3 = 0, v4 = 0;
            if (result.Length >= 8)
            {
                v1 = BitConverter.ToInt16([result[17], result[16]]);
                v2 = BitConverter.ToInt16([result[15], result[14]]);
                v3 = BitConverter.ToInt16([result[13], result[12]]);
                v4 = BitConverter.ToInt16([result[11], result[10]]);
            }
            decimal[] temp = [v1 * 0.0078125M, v2 * 0.0078125M, v3 * 0.0078125M, v4 * 0.0078125M];
            return temp[GetTempIndex()];
        }

        //读取uid初始化
        public void Initialize1(SerialPortTool connection)
        {
            connection.WriteRead(Write(0x06, 1, [0x80]));
            Thread.Sleep(10);
            connection.WriteRead(Write(0x7F, 1, [0x67]));
            connection.WriteRead(Write(0x50, 1, [0xFD]));
            connection.WriteRead(Write(0x51, 1, [0xFD]));
            Thread.Sleep(10);
        }
        //采集数据初始化
        public void Initialize2(SerialPortTool connection)
        {
            connection.WriteRead(Write(0x06, 1, [0x08]));
            connection.WriteRead(Write(0x06, 1, [0x80]));
            Thread.Sleep(10);
            connection.WriteRead(Write(0x7F, 1, [0x67]));
            connection.WriteRead(Write(0x56, 1, [0x01]));
            connection.WriteRead(Write(0x07, 1, [0x08]));
            connection.WriteRead(Write(0x08, 1, [0x01]));
            connection.WriteRead(Write(0x09, 1, [0x1A]));
            connection.WriteRead(Write(0x06, 1, [0x13]));
            Thread.Sleep(10);
        }
        //烧录条件
        public void Initialize3(SerialPortTool connection)
        {
            connection.WriteRead(Write(0x50, 1, [0x08]));
            Thread.Sleep(10);
            connection.WriteRead(Write(0x51, 1, [0x08]));
            Thread.Sleep(10);
        }
        //得到芯片Uid
        public int GetSensorUID(SerialPortTool connection, int type = 0)
        {
            Initialize1(connection);
            var uidResult = ReceivedData.ParseData(connection.WriteRead(Read(0x02, 4)));//读取所有传感器的uid数据
            byte[] uidBytes = uidResult[SensorIndex].Data;
            if (uidResult[SensorIndex].IsEffective && uidBytes.Length == 4)
            {
                Array.Reverse(uidBytes);
                Uid = BitConverter.ToInt32(uidBytes);
            }
            return Uid;
        }
        //得到芯片数据
        public void GetSensorData(SerialPortTool connection, out int temperature, out int pressure)
        {
            temperature = 0;
            pressure = 0;
            Initialize2(connection);
            var pressResult = ReceivedData.ParseData(connection.WriteRead(Read(0x17, 3)));
            var tempResult = ReceivedData.ParseData(connection.WriteRead(Read(0x1A, 2)));
            byte[] tempBytes = new byte[4];
            if (pressResult[SensorIndex].IsEffective && tempResult[SensorIndex].IsEffective)
            {
                tempResult[SensorIndex].Data.CopyTo(tempBytes, 0);
                temperature = BitConverter.ToInt32(tempBytes);
                pressResult[SensorIndex].Data.CopyTo(tempBytes, 0);
                pressure = BitConverter.ToInt32(tempBytes);
            }
        }
        //采集一个数据
        public RawData GetData(SerialPortTool connection, int AcquisitionTimes)
        {
            RawData data = new()
            {
                Uid = GetSensorUID(connection)
            };

            List<RawData> averageList = [];
            for (int i = 0; i < AcquisitionTimes; i++)
            {
                RawData averageData = new();
                GetSensorData(connection, out int temp, out int press);
                averageData.TRaw = temp;
                averageData.PRaw = press;
                averageList.Add(averageData);
            }

            int praw = 0;
            int traw = 0;
            for (int i = 0; i < AcquisitionTimes; i++)
            {
                praw += averageList[i].PRaw;
                traw += averageList[i].TRaw;
            }

            data.PRaw = praw / AcquisitionTimes;
            data.TRaw = traw / AcquisitionTimes;
            data.TRef = ReadTemperature(connection);
            return data;
        }
        #endregion

        #region 数据处理
        /// <summary>
        /// 计算系数
        /// </summary>
        /// <param name="method">计算方法，false为9系数计算</param>
        public override void Calculate()
        {
            CEBOE2520? calib = Calculation.StartCalibration9(RawDataList);
            if (calib == null) return;
            CoefficientData = calib;
            CoefficientData.Date = DateTime.Now.ToString("G");
        }
        /// <summary>
        /// 压力验证
        /// </summary>
        /// <param name="sensorP">传感器读出压力值</param>
        /// <param name="sensorT">传感器读出温度值</param>
        /// <param name="paceRef">实际压力值</param>
        /// <param name="tProbe">实际温度值</param>
        public override void Validate(double maxPDiff = 10, double maxTDiff = 0.5)
        {
            foreach (var verifyItem in ValidationDataList)
            {
                if (CoefficientData == null) continue;
                verifyItem.Validate(CoefficientData);
                if (Math.Abs(verifyItem.PResidual) > maxPDiff) Result = "NG";
                else if (Math.Abs(verifyItem.TResidual) > maxTDiff) Result = "NG";
                else
                {
                    Result = "GOOD";
                }
            }
        }
        #endregion
    }

    public class SensorZXC6862 : Sensor
    {
        public class MinBridgeOffset
        {
            public int SensorIndex { get; private set; } = 0;
            public byte BridgeOffset { get; set; } = 0x00;
            public int MinPressure { get; set; } = 0;

            public MinBridgeOffset(int index, byte bridgeOffset, int minPress)
            {
                SensorIndex = index;
                BridgeOffset = bridgeOffset;
                MinPressure = minPress;
            }

            public void SetValue(byte bridgeOffset, int minPress)
            {
                if (Math.Abs(minPress) < Math.Abs(MinPressure))
                {
                    MinPressure = minPress;
                    BridgeOffset = bridgeOffset;
                }
            }

        }
        public class TargetGain
        {
            public int SensorIndex { get; private set; } = 0;
            public byte AMPGain { get; set; } = 0x00;
            public int Pressure { get; set; } = 0;

            public TargetGain(int index, byte gain, int press)
            {
                SensorIndex = index;
                AMPGain = gain;
                Pressure = press;
            }

            public void SetValue(byte ampGain, int pressure, int targetP = 700000)
            {
                int dP1 = Math.Abs(Pressure - targetP);
                int dP2 = Math.Abs(pressure - targetP);
                if (dP2 < dP1)
                {
                    AMPGain = ampGain;
                    Pressure = pressure;
                }
            }
        }

        #region 标定数据
        public int[] POffset { get; set; } = new int[32];
        public int[] PGain { get; set; } = new int[16];
        #endregion

        public SensorZXC6862(byte deviceAddress, int sensorIndex, byte i2CAddress = 0x77, byte speed = 0x07)
        {
            DeviceAddress = deviceAddress;
            SensorIndex = sensorIndex;
            I2CAddress = i2CAddress;
            Speed = speed;
            SensorInfo.Name = $"LB[D{DeviceAddress}S{SensorIndex}]";
            SensorInfo.Tag = $"D{DeviceAddress}S{SensorIndex}";
            SensorInfo.Text = $"[{SensorIndex + 1}]";
        }

        #region 数据处理
        /// <summary>
        /// 计算系数
        /// </summary>
        /// <param name="method">计算方法，false为9系数计算</param>
        public override void Calculate()
        {
            CEZXC6862? calibration = Calculation.StartCalibration(RawDataList);
            if (calibration == null) return;
            CoefficientData = calibration;
            CoefficientData.Date = DateTime.Now.ToString("G");
        }
        /// <summary>
        /// 压力验证
        /// </summary>
        /// <param name="sensorP">传感器读出压力值</param>
        /// <param name="sensorT">传感器读出温度值</param>
        /// <param name="paceRef">实际压力值</param>
        /// <param name="tProbe">实际温度值</param>
        public override void Validate(double maxPDiff = 10, double maxTDiff = 0.5)
        {
            foreach (var verifyItem in ValidationDataList)
            {
                if (CoefficientData == null) continue;
                verifyItem.Validate(CoefficientData);
                if (Math.Abs(verifyItem.PResidual) > maxPDiff) Result = "NG";
                else if (Math.Abs(verifyItem.TResidual) > maxTDiff) Result = "NG";
                else
                {
                    Result = "GOOD";
                }
            }
        }
        #endregion
    }

    public class SensorZXW7570 : Sensor
    {
        public SensorZXW7570(byte deviceAddress, int sensorIndex, byte i2CAddress = 0x7F, byte speed = 0x07)
        {
            DeviceAddress = deviceAddress;
            SensorIndex = sensorIndex;
            I2CAddress = i2CAddress;
            Speed = speed;
            SensorInfo.Name = $"LB[D{DeviceAddress}S{SensorIndex}]";
            SensorInfo.Tag = $"D{DeviceAddress}S{SensorIndex}";
            SensorInfo.Text = $"[{SensorIndex + 1}]";
        }

        public override void Calculate()
        {
            throw new NotImplementedException();
        }

        public override void Validate(double maxPDiff = 10, double maxTDiff = 0.5)
        {
            throw new NotImplementedException();
        }

    }
}
