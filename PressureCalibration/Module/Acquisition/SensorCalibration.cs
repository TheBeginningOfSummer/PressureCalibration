using Calibration.Data;
using CSharpKit.Communication;
using CSharpKit.DataManagement;

namespace Module
{
    public class SensorCalibration
    {
        #region 标定数据
        public int Uid { get; set; } = 0;
        public List<RawData> CurrentRawData { get; set; } = [];
        public CalibrationData CurrentCalibrationData { get; set; } = new();
        public List<ValidationData> CurrentValidationData { get; set; } = [];
        #endregion

        #region 标定参数
        public byte DeviceAddress { get; set; }//所在采集卡的地址
        public int SensorIndex { get; set; }//采集卡中的哪一路
        public double SingleYield { get; set; } = 1.0;//良率
        public double MinYield { get; set; } = 0.8;//最小良率
        public bool ContinuousNG { get; set; } = false;//连续NG

        private string result = "Default";
        public string Result
        {
            get { return result; }
            set
            {
                if (value == "NG" && result != "NG")
                {
                    SingleYield = SingleYield * 100 / 101;
                }
                if (value == "GOOD" && result != "GOOD")
                {
                    SingleYield = (SingleYield * 100 + 1) / 101;
                }
                result = value;
            }
        }//测试结果
        public bool IsFused { get; set; } = false;//是否烧录过
        #endregion

        public SensorCalibration(byte deviceAddress, int sensorIndex)
        {
            DeviceAddress = deviceAddress;
            SensorIndex = sensorIndex;
        }

        #region 寄存器操作
        public static SendData Read(byte deviceAddress, int sensorAddress, byte address, byte count, byte speed = 0x00, byte chip = 0x20)
        {
            byte channelByte1 = 0x00;
            byte channelByte2 = 0x00;
            byte[] sendBytes;
            SendData sendData;
            if (sensorAddress >= 0 && sensorAddress < 8)
            {
                channelByte1 = 0x00;
                channelByte2 = BytesTool.SetBit(channelByte2, (ushort)sensorAddress, true);
            }
            else if (sensorAddress >= 8 && sensorAddress < 16)
            {
                channelByte1 = BytesTool.SetBit(channelByte1, (ushort)(sensorAddress - 8), true);
                channelByte2 = 0x00;
            }
            else
            {
                channelByte1 = 0xFF;
                channelByte2 = 0xFF;
                sendBytes = [0x24, deviceAddress, 0x80, 0x20, channelByte1, channelByte2, speed, chip, address, count];
                sendData = new(CRC16.CRC16_1(sendBytes), 12 + 16 * count);
                return sendData;
            }
            sendBytes = [0x24, deviceAddress, 0x80, 0x20, channelByte1, channelByte2, speed, chip, address, count];
            sendData = new(CRC16.CRC16_1(sendBytes), 12 + count);
            return sendData;
        }
        public SendData Read(byte address, byte count, byte speed = 0x00, byte chip = 0x20)
        {
            return Read(DeviceAddress, SensorIndex, address, count, speed, chip);
        }

        public static SendData Write(byte deviceAddress, int sensorAddress, byte address, byte count, byte[] data, byte speed = 0x00, byte chip = 0x20)
        {
            byte channelByte1 = 0x00;
            byte channelByte2 = 0x00;
            SendData sendData;
            if (sensorAddress >= 0 && sensorAddress < 8)
            {
                channelByte1 = 0x00;
                channelByte2 = BytesTool.SetBit(channelByte2, (ushort)sensorAddress, true);
            }
            else if (sensorAddress >= 8 && sensorAddress < 16)
            {
                channelByte1 = BytesTool.SetBit(channelByte1, (ushort)(sensorAddress - 8), true);
                channelByte2 = 0x00;
            }
            else
            {
                channelByte1 = 0xFF;
                channelByte2 = 0xFF;
            }

            byte[] header = [0x24, deviceAddress, 0x80, 0x30, channelByte1, channelByte2, speed, chip, address, count];
            byte[] sendBytes = BytesTool.SpliceBytes(header, data);
            sendData = new(CRC16.CRC16_1(sendBytes), 12);
            return sendData;
        }
        public SendData Write(byte address, byte count, byte[] data, byte speed = 0x00, byte chip = 0x20)
        {
            return Write(DeviceAddress, SensorIndex, address, count, data, speed, chip);
        }
        public SendData WriteFuseData(byte address = 0x34, byte length = 28)
        {
            SendData sendData = Write(address, length, CurrentCalibrationData.registerData);
            return sendData;
        }

        public static SendData Fuse(byte deviceAddress, int sensorAddress, byte address = 0x34, byte count = 28, byte speed = 0x00, byte chip = 0x20)
        {
            byte channelByte1 = 0x00;
            byte channelByte2 = 0x00;
            SendData sendData;
            if (sensorAddress >= 0 && sensorAddress < 8)
            {
                channelByte1 = 0x00;
                channelByte2 = BytesTool.SetBit(channelByte2, (ushort)sensorAddress, true);
            }
            else if (sensorAddress >= 8 && sensorAddress < 16)
            {
                channelByte1 = BytesTool.SetBit(channelByte1, (ushort)(sensorAddress - 8), true);
                channelByte2 = 0x00;
            }
            else
            {
                channelByte1 = 0xFF;
                channelByte2 = 0xFF;
            }
            byte[] sendBytes = [0x24, deviceAddress, 0x80, 0x31, channelByte1, channelByte2, speed, chip, address, count];
            sendData = new SendData(CRC16.CRC16_1(sendBytes), 12);
            return sendData;
        }
        public SendData Fuse(byte address = 0x34, byte count = 28, byte speed = 0x00, byte chip = 0x20)
        {
            return Fuse(DeviceAddress, SensorIndex, address, count, speed, chip);
        }
        #endregion

        #region 数据采集
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
            var result = connection.SendWithRead(CRC16.CRC16_1(sendBytes), 20);
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
            connection.SendWithRead(Write(0x06, 1, [0x80]));
            Thread.Sleep(10);
            connection.SendWithRead(Write(0x7F, 1, [0x67]));
            connection.SendWithRead(Write(0x50, 1, [0xFD]));
            connection.SendWithRead(Write(0x51, 1, [0xFD]));
            Thread.Sleep(10);
        }
        //采集数据初始化
        public void Initialize2(SerialPortTool connection)
        {
            connection.SendWithRead(Write(0x06, 1, [0x08]));
            connection.SendWithRead(Write(0x06, 1, [0x80]));
            Thread.Sleep(10);
            connection.SendWithRead(Write(0x7F, 1, [0x67]));
            connection.SendWithRead(Write(0x56, 1, [0x01]));
            connection.SendWithRead(Write(0x07, 1, [0x08]));
            connection.SendWithRead(Write(0x08, 1, [0x01]));
            connection.SendWithRead(Write(0x09, 1, [0x1A]));
            connection.SendWithRead(Write(0x06, 1, [0x13]));
            Thread.Sleep(10);
        }
        //烧录条件
        public void Initialize3(SerialPortTool connection)
        {
            connection.SendWithRead(Write(0x50, 1, [0x08]));
            Thread.Sleep(10);
            connection.SendWithRead(Write(0x51, 1, [0x08]));
            Thread.Sleep(10);
        }
        //得到芯片Uid
        public int GetSensorUID(SerialPortTool connection)
        {
            Initialize1(connection);
            var uidResult = ReceivedData.ParseData(connection.SendWithRead(Read(0x02, 4)));//读取所有传感器的uid数据
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
            var pressResult = ReceivedData.ParseData(connection.SendWithRead(Read(0x17, 3)));
            var tempResult = ReceivedData.ParseData(connection.SendWithRead(Read(0x1A, 2)));
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
                uid = GetSensorUID(connection)
            };

            List<RawData> averageList = [];
            for (int i = 0; i < AcquisitionTimes; i++)
            {
                RawData averageData = new();
                GetSensorData(connection, out int temp, out int press);
                averageData.UNCALTempCodes = temp;
                averageData.RAW_C = press;
                averageList.Add(averageData);
            }

            int praw = 0;
            int traw = 0;
            for (int i = 0; i < AcquisitionTimes; i++)
            {
                praw += averageList[i].RAW_C;
                traw += averageList[i].UNCALTempCodes;
            }

            data.RAW_C = praw / AcquisitionTimes;
            data.UNCALTempCodes = traw / AcquisitionTimes;
            data.TProbe = ReadTemperature(connection);
            return data;
        }
        #endregion

        #region 数据处理
        public void Initialize(CalibrationParameter parameter)
        {
            Result = "Default";
            IsFused = false;
            CurrentRawData.Clear();
            CurrentValidationData.Clear();
            for (int i = 0; i < parameter.TempaturePoints.Count; i++)
            {
                for (int j = 0; j < parameter.PressurePoints.Count; j++)
                {
                    RawData data = new()
                    {
                        T_idx = i + 1,
                        P_idx = j + 1,
                        SetT = parameter.TempaturePoints[i],
                        SetP = parameter.PressurePoints[j]
                    };
                    CurrentRawData.Add(data);
                }
            }
        }
        /// <summary>
        /// 计算系数
        /// </summary>
        /// <param name="method">计算方法，false为9系数计算</param>
        public void Calculate(bool method = true)
        {
            CalibrationData? calib;
            if (method)
                calib = Calculation.StartCalibration12(CurrentRawData);
            else
                calib = Calculation.StartCalibration9(CurrentRawData);
            if (calib != null) CurrentCalibrationData = calib;
            CurrentCalibrationData.Date = DateTime.Now.ToString("G");
        }
        /// <summary>
        /// 压力验证
        /// </summary>
        /// <param name="sensorP">传感器读出压力值</param>
        /// <param name="sensorT">传感器读出温度值</param>
        /// <param name="paceRef">实际压力值</param>
        /// <param name="tProbe">实际温度值</param>
        public void Verify(double maxPDiff = 10, double maxTDiff = 0.5)
        {
            foreach (var verifyItem in CurrentValidationData)
            {
                verifyItem.Verify(CurrentCalibrationData);
                if (Math.Abs(verifyItem.PResidual) > maxPDiff) Result = "NG";
                else if (Math.Abs(verifyItem.TResidual) > maxTDiff) Result = "NG";
                else
                {
                    if (Result != "NG") Result = "GOOD";
                }
            }
        }

        public bool CheckYield()
        {
            if (SingleYield < MinYield) return false;
            return true;
        }

        /// <summary>
        /// 导出数据到excel
        /// </summary>
        /// <returns></returns>
        public bool OutputExcel(string path = $"Data\\Excel\\")
        {
            try
            {
                if (ExcelOutput.Output(this, path, $"采集卡{DeviceAddress}位置{SensorIndex}UID[{CurrentRawData.FirstOrDefault()?.uid.ToString()}]{Result}"))
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 显示数据
        /// </summary>
        public string ShowData()
        {
            string message = "";
            message += "计算数据：" + Environment.NewLine;
            message += CurrentCalibrationData.Display() + Environment.NewLine;
            message += "    采集的数据：" + Environment.NewLine;
            for (int i = 0; i < CurrentRawData.Count; i++)
                message += $"[{i}]{CurrentRawData[i].Display()}{Environment.NewLine}";
            message += "    验证的数据：" + Environment.NewLine;
            for (int i = 0; i < CurrentValidationData.Count; i++)
                message += $"[{i}]{CurrentValidationData[i].Display()}{Environment.NewLine}";
            message += "---------------------------------------------------------------------------------------------------" + Environment.NewLine;
            return message;
        }
        #endregion
    }
}
