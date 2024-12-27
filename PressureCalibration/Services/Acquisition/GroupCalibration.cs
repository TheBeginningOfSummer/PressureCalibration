using Calibration.Data;
using CSharpKit.Communication;
using CSharpKit.DataManagement;
using CSharpKit.FileManagement;
using System.Collections.Concurrent;

namespace Calibration.Services
{
    public class GroupCalibration : ParameterManager
    {
        #region 参数
        /// <summary>
        /// 采集地址
        /// </summary>
        public byte DeviceAddress;
        /// <summary>
        /// 每组采集传感器的数量
        /// </summary>
        public int SensorCount = 16;
        /// <summary>
        /// 传感器选择数组
        /// </summary>
        public bool[] SelectedSensor = [];
        /// <summary>
        /// 当前温度的数据是否采集完成
        /// </summary>
        public bool IsAcqOK = false;
        #endregion

        /// <summary>
        /// 每个采集组采集16个传感器，序号0-15
        /// </summary>
        public readonly ConcurrentDictionary<int, SensorCalibration> SensorDataGroup = [];
        /// <summary>
        /// 采集卡连接
        /// </summary>
        public SerialPortTool Connection = new();

        public GroupCalibration(byte deviceAddress, int sensorCount = 16)
        {
            DeviceAddress = deviceAddress;
            SensorCount = sensorCount;

            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < SensorCount; i++)
            {
                SensorCalibration sensor = new(deviceAddress, i);
                sensor.Initialize(Get<CalibrationParameter>());
                SensorDataGroup.TryAdd(i, sensor);
                SelectedSensor[i] = false;
            }
        }

        #region 电源操作
        public byte[] Header(byte controlCode = 0x80, byte functionCode = 0x20)
        {
            return [0x24, DeviceAddress, controlCode, functionCode];
        }

        public byte[] GetDeviceInfo()
        {
            return Connection.SendWithRead(CRC16.CRC16_1(Header(functionCode: 0x00)), 9);
        }

        public byte[] PowerControl(float voltage = 0)
        {
            byte[] sendBytes = voltage switch
            {
                0 => Header(functionCode: 0x10),
                3.3f => Header(functionCode: 0x11),
                4.1f => Header(functionCode: 0x12),
                1.8f => Header(functionCode: 0x13),
                _ => Header(functionCode: 0x10),
            };
            return Connection.SendWithRead(CRC16.CRC16_1(sendBytes), 7);
        }

        public byte[] PowerOn(float voltage = 1.8f)
        {
            return PowerControl(voltage);
        }

        public byte[] PowerOff()
        {
            return PowerControl();
        }
        #endregion

        #region 寄存器操作
        public SendData Read(bool[] selectedSensor, byte address, byte count, byte speed = 0x00, byte chip = 0x20)
        {
            byte[] sensorAddressBytes = [0x00, 0x00];

            int j = 0;
            int selectedSensorCount = 0;
            for (int i = 0; i < Math.Min(selectedSensor.Length, SensorCount); i++)
            {
                if (i % 8 == 0 && i != 0) j++;
                if (selectedSensor[i]) selectedSensorCount++;
                sensorAddressBytes[j] = BytesTool.SetBit(sensorAddressBytes[j], (ushort)(i - j * 8), selectedSensor[i]);
            }
            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x20, sensorAddressBytes[1], sensorAddressBytes[0], speed, chip, address, count];
            SendData sendData = new(CRC16.CRC16_1(sendBytes), 12 + selectedSensorCount * count);
            return sendData;
        }
        public byte[] ReadAll(byte address, byte count, byte speed = 0x00, byte chip = 0x20)
        {
            byte[] sensorAddressBytes = [0xFF, 0xFF];
            int selectedSensorCount = 16;

            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x20, sensorAddressBytes[1], sensorAddressBytes[0], speed, chip, address, count];
            SendData sendData = new(CRC16.CRC16_1(sendBytes), 12 + selectedSensorCount * count);
            return Connection.SendWithRead(sendData);
        }

        public SendData WriteAll(byte address, byte count, byte[] data, byte speed = 0x00, byte chip = 0x20)
        {
            byte[] header = [0x24, DeviceAddress, 0x80, 0x30, 0xFF, 0xFF, speed, chip, address, count];
            byte[] sendBytes = BytesTool.SpliceBytes(header, data);
            SendData sendData = new(CRC16.CRC16_1(sendBytes), 12);
            return sendData;
        }
        public SendData WriteAll(byte address, byte count, List<byte[]> data, byte speed = 0x00, byte chip = 0x20)
        {
            List<byte> bytes = [];
            for (int i = data.Count - 1; i >= 0; i--)
                bytes.AddRange(data[i]);
            return WriteAll(address, count, bytes.ToArray(), speed, chip);
        }
        public byte[] WriteAllFuseData(byte address = 0x34, byte length = 28)
        {
            Initialize1();
            List<byte> bytes = [];
            for (int i = SensorDataGroup.Count - 1; i >= 0; i--)
                bytes.AddRange(SensorDataGroup[i].CurrentCalibrationData.registerData);
            return Connection.SendWithRead(WriteAll(address, length, [.. bytes]));
        }

        public SendData FuseAll(byte address = 0x34, byte count = 28, byte speed = 0x00, byte chip = 0x20)
        {
            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x31, 0xFF, 0xFF, speed, chip, address, count];
            SendData sendData = new(CRC16.CRC16_1(sendBytes), 12);
            return sendData;
        }

        public byte[] Fuse(bool[] selectedSensor, byte address = 0x34, byte count = 28, byte speed = 0x00, byte chip = 0x20)
        {
            Initialize3();
            byte[] sensorAddressBytes = [0x00, 0x00];

            int j = 0;
            for (int i = 0; i < Math.Min(selectedSensor.Length, SensorCount); i++)
            {
                if (i % 8 == 0 && i != 0) j++;//8字节计数
                sensorAddressBytes[j] = BytesTool.SetBit(sensorAddressBytes[j], (ushort)(i - j * 8), selectedSensor[i]);
            }

            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x31, sensorAddressBytes[1], sensorAddressBytes[0], speed, chip, address, count];
            SendData sendData = new(CRC16.CRC16_1(sendBytes), 12);
            return Connection.SendWithRead(sendData);
        }
        public byte[] Fuse(byte address = 0x34, byte count = 28, byte speed = 0x00, byte chip = 0x20)
        {
            return Fuse(SelectedSensor, address, count, speed, chip);
        }
        #endregion

        #region 数据采集
        private static int GetTempIndex(int sensorIndex)
        {
            if (sensorIndex >= 4 && sensorIndex <= 7) return 1;
            if (sensorIndex >= 8 && sensorIndex <= 11) return 2;
            if (sensorIndex >= 12 && sensorIndex <= 15) return 3;
            return 0;
        }
        /// <summary>
        /// 读取四路温度值
        /// </summary>
        /// <returns>读取的温度值</returns>
        public decimal[] ReadTemperature()
        {
            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x21, 0x00, 0x0F, 0x00, 0x48, 0x00, 0x02];
            var result = Connection.SendWithRead(CRC16.CRC16_1(sendBytes), 20);
            short v1 = 0, v2 = 0, v3 = 0, v4 = 0;
            if (result.Length >= 18)
            {
                v1 = BitConverter.ToInt16([result[17], result[16]]);
                v2 = BitConverter.ToInt16([result[15], result[14]]);
                v3 = BitConverter.ToInt16([result[13], result[12]]);
                v4 = BitConverter.ToInt16([result[11], result[10]]);
            }
            return [v1 * 0.0078125M, v2 * 0.0078125M, v3 * 0.0078125M, v4 * 0.0078125M];
        }

        public static byte[] GetArray(byte value, int length = 16)
        {
            byte[] array = new byte[length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
            return array;
        }
        public static byte[] GetArray(byte[] arrayValue, int length = 16)
        {
            List<byte> arrayList = [];
            for (int i = 0; i < length; i++)
            {
                arrayList.AddRange(arrayValue);
            }
            return arrayList.ToArray();
        }

        //读取uid初始化
        public void Initialize1()
        {
            Connection.SendWithRead(WriteAll(0x06, 1, GetArray(0x80, SensorCount)));
            Thread.Sleep(10);
            Connection.SendWithRead(WriteAll(0x7F, 1, GetArray(0x67, SensorCount)));
            Connection.SendWithRead(WriteAll(0x50, 2, GetArray(0xFD, SensorCount * 2)));
            Thread.Sleep(20);
        }
        //采集数据初始化
        public void Initialize2()
        {
            Connection.SendWithRead(WriteAll(0x06, 1, GetArray(0x08, SensorCount)));
            Connection.SendWithRead(WriteAll(0x06, 1, GetArray(0x80, SensorCount)));
            Thread.Sleep(10);
            Connection.SendWithRead(WriteAll(0x7F, 1, GetArray(0x67, SensorCount)));
            Connection.SendWithRead(WriteAll(0x56, 1, GetArray(0x01, SensorCount)));
            Connection.SendWithRead(WriteAll(0x07, 3, GetArray([0x08, 0x01, 0x1A], SensorCount)));
            Connection.SendWithRead(WriteAll(0x06, 1, GetArray(0x13, SensorCount)));
            Thread.Sleep(20);
        }
        //采集数据初始化
        public void InitializeOutput()
        {
            Connection.SendWithRead(WriteAll(0x06, 1, GetArray(0x08, SensorCount)));
            Connection.SendWithRead(WriteAll(0x06, 1, GetArray(0x80, SensorCount)));
            Thread.Sleep(10);
            //Connection.SendWithRead(WriteAll(0x7F, 1, GetArray(0x67, SensorCount)));
            Connection.SendWithRead(WriteAll(0x07, 3, GetArray([0x08, 0x01, 0x1A], SensorCount)));
            Connection.SendWithRead(WriteAll(0x06, 1, GetArray(0x13, SensorCount)));
            Thread.Sleep(20);
        }
        //烧录条件
        public void Initialize3()
        {
            Connection.SendWithRead(WriteAll(0x50, 2, GetArray(0x08, SensorCount * 2)));
            Thread.Sleep(20);
        }
        /// <summary>
        /// 得到采集卡所有芯片UID
        /// </summary>
        /// <returns></returns>
        public int[] GetSensorsUID()
        {
            int[] uidArray = new int[SensorCount];

            //采集UID
            Initialize1();
            var uidResult = ReceivedData.ParseData(ReadAll(0x02, 4), SensorCount);//读取所有传感器的uid数据
            byte[] uidBytes = new byte[4];
            for (int i = 0; i < SensorCount; i++)
            {
                if (uidResult[i].IsEffective)
                {
                    uidResult[i].Data.CopyTo(uidBytes, 0);
                    Array.Reverse(uidBytes);
                    uidArray[i] = BitConverter.ToInt32(uidBytes);
                }
            }
            return uidArray;
        }
        /// <summary>
        /// 得到采集卡所有芯片数据
        /// </summary>
        /// <param name="tempArray">温度数据</param>
        /// <param name="pressArray">压力数据</param>
        public void GetSensorsData(out int[] tempArray, out int[] pressArray)
        {
            tempArray = new int[SensorCount];
            pressArray = new int[SensorCount];

            Initialize2();
            var pressResult = ReceivedData.ParseData(ReadAll(0x17, 3), SensorCount);
            var tempResult = ReceivedData.ParseData(ReadAll(0x1A, 2), SensorCount);
            byte[] tempBytes = new byte[4];
            for (int i = 0; i < SensorCount; i++)
            {
                Array.Clear(tempBytes);
                if (pressResult[i].IsEffective && tempResult[i].IsEffective)
                {
                    tempResult[i].Data.CopyTo(tempBytes, 0);
                    tempArray[i] = BitConverter.ToInt32(tempBytes);
                    pressResult[i].Data.CopyTo(tempBytes, 0);
                    pressArray[i] = BitConverter.ToInt32(tempBytes);
                }
            }
        }
        /// <summary>
        /// 得到采集卡所有芯片输出
        /// </summary>
        /// <param name="tempArray">温度值</param>
        /// <param name="pressArray">压力值</param>
        public void GetSensorsValue(out decimal[] tempArray, out decimal[] pressArray)
        {
            tempArray = new decimal[SensorCount];
            pressArray = new decimal[SensorCount];

            InitializeOutput();
            var pressResult = ReceivedData.ParseData(ReadAll(0x17, 3), SensorCount);
            var tempResult = ReceivedData.ParseData(ReadAll(0x1A, 2), SensorCount);
            byte[] tempBytes = new byte[4];
            for (int i = 0; i < SensorCount; i++)
            {
                Array.Clear(tempBytes);
                if (pressResult[i].IsEffective && tempResult[i].IsEffective)
                {
                    tempResult[i].Data.CopyTo(tempBytes, 0);
                    tempArray[i] = BitConverter.ToInt32(tempBytes) / 128m - 273.15m;
                    pressResult[i].Data.CopyTo(tempBytes, 0);
                    pressArray[i] = BitConverter.ToInt32(tempBytes) / 64m;
                }
            }
        }

        /// <summary>
        /// 采集一次组中所有传感器的数据(单温度单压力)
        /// </summary>
        /// <param name="acquisitionTimes">采集次数</param>
        /// <param name="setP">设置压力</param>
        /// <param name="setT">设置温度</param>
        public decimal[] GetData(int acquisitionTimes, decimal setP, decimal setT)
        {
            //UID读取
            int[] uidArray = GetSensorsUID();
            for (int i = 0; i < SensorCount; i++)
            {
                var ori = SensorDataGroup[i].CurrentRawData.Where((arg) => (arg.SetP == setP) && (arg.SetT == setT)).First();
                SensorDataGroup[i].Uid = uidArray[i];
                ori.uid = uidArray[i];
            }
            //采集温度压力实时数据
            decimal press = Get<PressController>().GetPress();
            decimal[] currentTemp = ReadTemperature();
            //平均数存储列表
            List<RawData[]> averageList = [];
            //采集平均数据
            for (int j = 0; j < acquisitionTimes; j++)
            {
                RawData[] averageArray = new RawData[SensorCount];
                GetSensorsData(out int[] tempArray, out int[] pressArray);
                for (int i = 0; i < SensorCount; i++)
                {
                    averageArray[i] = new()
                    {
                        RAW_C = pressArray[i],
                        UNCALTempCodes = tempArray[i]
                    };
                }
                averageList.Add(averageArray);
            }
            //计算平均数
            for (int j = 0; j < SensorCount; j++)
            {
                int praw = 0;
                int traw = 0;
                for (int i = 0; i < acquisitionTimes; i++)
                {
                    praw += averageList[i][j].RAW_C;
                    traw += averageList[i][j].UNCALTempCodes;
                }
                var ori = SensorDataGroup[j].CurrentRawData.Where((arg) => (arg.SetP == setP) && (arg.SetT == setT)).First();
                ori.Date = DateTime.Now.ToString("G");
                ori.RAW_C = praw / acquisitionTimes;
                ori.UNCALTempCodes = traw / acquisitionTimes;
                ori.PACERef = press;
                ori.TProbe = currentTemp[GetTempIndex(j)];
            }
            return currentTemp;
        }
        /// <summary>
        /// 检查是否采集到数据
        /// </summary>
        /// <param name="setT">检查的设置温度</param>
        /// <returns></returns>
        public bool CheckData(decimal setT)
        {
            IsAcqOK = true;
            for (int i = 0; i < SensorCount; i++)
            {
                var sensorOriData = SensorDataGroup[i].CurrentRawData.Where((t) => t.SetT == setT).First();
                if (sensorOriData.UNCALTempCodes == 0 || sensorOriData.RAW_C == 0)
                {
                    IsAcqOK = false;
                    return false;
                }
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 初始化测试数据容器，清除数据并添加新的测试数据容器
        /// </summary>
        public void Initialize()
        {
            for (int i = 0; i < SensorCount; i++)
            {
                SensorDataGroup[i].Initialize(Get<CalibrationParameter>());
            }
        }

        public void CalGroup(bool method = true)
        {
            foreach (var sensorData in SensorDataGroup.Values)
            {
                sensorData.Calculate(method);
            }
        }

        public void AcqVerifyGroup(decimal setP = 0, decimal setT = 0)
        {
            decimal press = Get<PressController>().GetPress();
            decimal[] currentTemp = ReadTemperature();
            GetSensorsData(out int[] tempArray, out int[] pressArray);
            for (int i = 0; i < SensorCount; i++)
            {
                ValidationData validationData = new(SensorDataGroup[i].Uid, setP, setT, press, currentTemp[GetTempIndex(i)], pressArray[i], tempArray[i])
                {
                    Date = DateTime.Now.ToString("G")
                };
                SensorDataGroup[i].CurrentValidationData.Add(validationData);
            }
        }

        public void VerifyGroup(CalibrationParameter parameter)
        {
            foreach (var sensorData in SensorDataGroup.Values)
                sensorData.Verify(parameter.FusePDiff, parameter.FuseTDiff);
        }

        public void SelectSensor()
        {
            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < SensorDataGroup.Count; i++)
            {
                if (SensorDataGroup[i].Result == "GOOD") SelectedSensor[i] = true;
                if (SensorDataGroup[i].IsFused) SelectedSensor[i] = false;
            }
        }

        public bool SaveData(string path = "Data\\SensorData\\")
        {
            string dataDirectory = Application.StartupPath + path;
            if (!Directory.Exists(dataDirectory)) Directory.CreateDirectory(dataDirectory);
            //string currentTime = $"{CurrentOriData.FirstOrDefault()?.UID}[{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}]";
            string currentTime = $"[{DateTime.Now:yyyy-MM-dd HHmmss}]采集卡{DeviceAddress}";
            string fileName = currentTime + ".json";
            try
            {
                JsonManager.SaveJsonString(dataDirectory, fileName, SensorDataGroup);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void LoadData(string fileName, string path = "Data\\SensorData\\")
        {
            var loadData = JsonManager.ReadJsonString<ConcurrentDictionary<int, SensorCalibration>>(path, fileName);
            if (loadData == null) return;
            foreach (var item in loadData)
            {
                SensorDataGroup.AddOrUpdate(item.Key, item.Value, (key, oldValue) => { return item.Value; });
            }
        }

        public async Task SaveDatabase()
        {
            for (int i = 0; i < SensorDataGroup.Count; i++)
            {
                await Get<Database>().AddAllDataAsync<RawData>(SensorDataGroup[i].CurrentRawData);
                await Get<Database>().AddDataAsync(SensorDataGroup[i].CurrentCalibrationData);
                await Get<Database>().AddAllDataAsync<ValidationData>(SensorDataGroup[i].CurrentValidationData);
            }
        }
        /// <summary>
        /// 显示采集组数据
        /// </summary>
        /// <returns></returns>
        public string Show()
        {
            string message = "";
            for (int i = 0; i < SensorDataGroup.Count; i++)
            {
                message += $"[{i}]{SensorDataGroup[i].ShowData()}{Environment.NewLine}";
            }
            return $"设备地址：{DeviceAddress}  传感器数量：{SensorCount}{Environment.NewLine}{message}";
        }

    }
}
