using CSharpKit.Communication;
using CSharpKit.DataManagement;
using CSharpKit.FileManagement;
using Data;
using System.Collections.Concurrent;

namespace Module
{
    public abstract class Group
    {
        #region 参数
        /// <summary>
        /// I2C地址
        /// </summary>
        public byte I2CAddress = 0x7F;
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
        #endregion

        #region 组件
        protected readonly Random random = new();
        /// <summary>
        /// 参数和设备配置
        /// </summary>
        public readonly Config config = Config.Instance;
        /// <summary>
        /// 采集卡连接
        /// </summary>
        public SerialPortTool Connection = new();
        #endregion

        #region 电源操作
        public byte[] Header(byte controlCode = 0x80, byte functionCode = 0x20)
        {
            return [0x24, DeviceAddress, controlCode, functionCode];
        }

        public byte[] GetDeviceInfo()
        {
            return Connection.WriteRead(CRC16.CRC16_1(Header(functionCode: 0x00)), 9);
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
            return Connection.WriteRead(CRC16.CRC16_1(sendBytes), 7);
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
        /// <summary>
        /// 生成一个重复字节的数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] GetArray(byte value, int length = 16)
        {
            byte[] array = new byte[length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
            return array;
        }
        /// <summary>
        /// 生成一个重复字节数组的数组
        /// </summary>
        /// <param name="arrayValue"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] GetArray(byte[] arrayValue, int length = 16)
        {
            List<byte> arrayList = [];
            for (int i = 0; i < length; i++)
            {
                arrayList.AddRange(arrayValue);
            }
            return [.. arrayList];
        }

        public SendData Read(bool[] selectedSensor, byte address, byte count, byte speed = 0x00)
        {
            byte[] sensorAddressBytes = [0x00, 0x00];
            int selectedSensorCount = 0;

            int j = 0;
            for (int i = 0; i < Math.Min(selectedSensor.Length, SensorCount); i++)
            {
                if (i % 8 == 0 && i != 0) j++;
                if (selectedSensor[i]) selectedSensorCount++;
                sensorAddressBytes[j] = BytesTool.SetBit(sensorAddressBytes[j], (ushort)(i - j * 8), selectedSensor[i]);
            }

            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x20, sensorAddressBytes[1], sensorAddressBytes[0], speed, I2CAddress, address, count];
            SendData sendData = new(CRC16.CRC16_1(sendBytes), 12 + selectedSensorCount * count);
            return sendData;
        }
        public byte[] ReadAll(byte address, byte count, byte speed = 0x00)
        {
            byte[] sensorAddressBytes = [0xFF, 0xFF];
            int selectedSensorCount = SensorCount;

            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x20, sensorAddressBytes[1], sensorAddressBytes[0], speed, I2CAddress, address, count];
            SendData sendData = new(CRC16.CRC16_1(sendBytes), 12 + selectedSensorCount * count);
            return Connection.WriteRead(sendData);
        }

        public SendData WriteAll(byte address, byte count, byte[] data, byte speed = 0x00)
        {
            byte[] header = [0x24, DeviceAddress, 0x80, 0x30, 0xFF, 0xFF, speed, I2CAddress, address, count];
            byte[] sendBytes = BytesTool.SpliceBytes(header, data);
            SendData sendData = new(CRC16.CRC16_1(sendBytes), 12);
            return sendData;
        }
        public SendData WriteAll(byte address, byte count, List<byte[]> data, byte speed = 0x00)
        {
            List<byte> bytes = [];
            for (int i = data.Count - 1; i >= 0; i--)
                bytes.AddRange(data[i]);
            return WriteAll(address, count, bytes.ToArray(), speed);
        }
        #endregion

        public Label[] TInfo = new Label[4];
        /// <summary>
        /// 设置四路温度标签位置
        /// </summary>
        /// <param name="point">位置</param>
        /// <param name="index">第几路</param>
        public void SetLabelLoc(Point point, int index)
        {
            TInfo[index].Location = point;
        }
        /// <summary>
        /// 设置标签显示信息
        /// </summary>
        /// <param name="value">四路温度值</param>
        /// <param name="targetT">目标温度</param>
        /// <param name="offsetT">温度最大偏移</param>
        private void SetTInfo(decimal[] value, decimal targetT = 15, decimal offsetT = 1)
        {
            if (value.Length != TInfo.Length) return;
            for (int i = 0; i < TInfo.Length; i++)
            {
                Color color;
                if (value[i] > targetT + Math.Abs(offsetT))
                    color = Color.Red;
                else if (value[i] < targetT - Math.Abs(offsetT))
                    color = Color.LightSkyBlue;
                else
                    color = Color.Orange;
                if (TInfo[i].IsHandleCreated)
                {
                    TInfo[i].Invoke(() => TInfo[i].Text = $"{value[i]:N2}℃");
                    TInfo[i].BackColor = color;
                }
                else
                {
                    TInfo[i].Text = $"{value[i]:N2}℃";
                    TInfo[i].BackColor = color;
                }
            }
        }
        /// <summary>
        /// 读取四路温度值
        /// </summary>
        /// <returns>读取的温度值</returns>
        public decimal[] ReadTemperature(bool isTest = false)
        {
            if (isTest)
            {
                var v1 = random.NextDouble() * 100;
                var v2 = random.NextDouble() * 100;
                var v3 = random.NextDouble() * 100;
                var v4 = random.NextDouble() * 100;
                decimal[] value = [(decimal)v1, (decimal)v2, (decimal)v3, (decimal)v4];
                SetTInfo(value);
                return value;
            }
            else
            {
                short v1 = 0, v2 = 0, v3 = 0, v4 = 0;
                byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x21, 0x00, 0x0F, 0x00, 0x48, 0x00, 0x02];
                var result = Connection.WriteRead(CRC16.CRC16_1(sendBytes), 20);
                if (result.Length >= 18)
                {
                    v1 = BitConverter.ToInt16([result[17], result[16]]);
                    v2 = BitConverter.ToInt16([result[15], result[14]]);
                    v3 = BitConverter.ToInt16([result[13], result[12]]);
                    v4 = BitConverter.ToInt16([result[11], result[10]]);
                }
                decimal[] value = [v1 * 0.0078125M, v2 * 0.0078125M, v3 * 0.0078125M, v4 * 0.0078125M];
                SetTInfo(value);
                return value;
            }
        }
        /// <summary>
        /// 找到传感器对应的温度索引值
        /// </summary>
        /// <param name="sensorIndex"></param>
        /// <param name="tempCount"></param>
        /// <returns></returns>
        public virtual int GetTempIndex(int sensorIndex, int tempCount = 2)
        {
            if (tempCount == 4)
            {
                if (sensorIndex >= 4 && sensorIndex <= 7) return 1;
                if (sensorIndex >= 8 && sensorIndex <= 11) return 2;
                if (sensorIndex >= 12 && sensorIndex <= 15) return 3;
            }
            else if (tempCount == 2)
            {
                if (sensorIndex > 7) return 1;
            }
            return 0;
        }

        public abstract Sensor GetSensor(int sensorIndex);
        public abstract byte[] WriteAllFuseData(byte address = 0x34, byte length = 28);
        public abstract byte[] Fuse(byte address = 0x34, byte count = 28, byte speed = 0x00);
        public abstract int[] GetSensorsUID();
        public abstract void GetSensorsData(out int[] tRawArray, out int[] pRawArray);
        public abstract void GetSensorsOutput(out decimal[] tArray, out decimal[] pArray);
        public abstract decimal[] GetData(decimal setP, decimal setT, out decimal press, bool isTest = false);
        public abstract void ReinitializeData();
        public abstract void Calculate();
        public abstract void Verify(decimal setP = 0, decimal setT = 0, bool isTest = false);
        public abstract void SelectSensor();
        public abstract bool SaveData(string path = "Data\\SensorData\\");
        public abstract void LoadData(string fileName, string path = "Data\\SensorData\\");
        public abstract Task SaveDatabase();
        public abstract string Show();

        public Group()
        {
            for (int i = 0; i < TInfo.Length; i++)
            {
                TInfo[i] = new()
                {
                    AutoSize = false,
                    Size = new Size(55, 55),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9),
                    Text = "0℃"
                };
            }
        }
    }

    public class GroupBOE2520 : Group
    {
        /// <summary>
        /// 每个采集组采集16个传感器，序号0-15
        /// </summary>
        public readonly ConcurrentDictionary<int, SensorBOE2520> SensorDataGroup = [];

        public GroupBOE2520(SerialPortTool serialPort, byte deviceAddress, int sensorCount = 16) : base()
        {
            Connection = serialPort;
            DeviceAddress = deviceAddress;
            SensorCount = sensorCount;
            I2CAddress = 0x20;

            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < SensorCount; i++)
            {
                SensorBOE2520 sensor = new(deviceAddress, i);
                sensor.ReinitializeData(config.CP);
                SensorDataGroup.TryAdd(i, sensor);
                SelectedSensor[i] = false;
            }
        }

        #region BOE2520
        /// <summary>
        /// 读取uid初始化
        /// </summary>
        public void BOE2520UID()
        {
            Connection.WriteRead(WriteAll(0x06, 1, GetArray(0x80, SensorCount)));
            Thread.Sleep(10);
            Connection.WriteRead(WriteAll(0x7F, 1, GetArray(0x67, SensorCount)));
            Connection.WriteRead(WriteAll(0x50, 2, GetArray(0xFD, SensorCount * 2)));
            Thread.Sleep(20);
        }
        /// <summary>
        /// 采集数据初始化
        /// </summary>
        public void BOE2520RawData()
        {
            Connection.WriteRead(WriteAll(0x06, 1, GetArray(0x08, SensorCount)));
            Connection.WriteRead(WriteAll(0x06, 1, GetArray(0x80, SensorCount)));
            Thread.Sleep(10);
            Connection.WriteRead(WriteAll(0x7F, 1, GetArray(0x67, SensorCount)));
            Connection.WriteRead(WriteAll(0x56, 1, GetArray(0x01, SensorCount)));
            Connection.WriteRead(WriteAll(0x07, 3, GetArray([0x08, 0x01, 0x1A], SensorCount)));
            Connection.WriteRead(WriteAll(0x06, 1, GetArray(0x13, SensorCount)));
            Thread.Sleep(20);
        }
        /// <summary>
        /// 采集输出值
        /// </summary>
        public void BOE2520Output()
        {
            Connection.WriteRead(WriteAll(0x06, 1, GetArray(0x08, SensorCount)));
            Connection.WriteRead(WriteAll(0x06, 1, GetArray(0x80, SensorCount)));
            Thread.Sleep(10);
            //Connection.SendWithRead(WriteAll(0x7F, 1, GetArray(0x67, SensorCount)));
            Connection.WriteRead(WriteAll(0x07, 3, GetArray([0x08, 0x01, 0x1A], SensorCount)));
            Connection.WriteRead(WriteAll(0x06, 1, GetArray(0x13, SensorCount)));
            Thread.Sleep(20);
        }
        /// <summary>
        /// 烧录条件
        /// </summary>
        public void BOE2520Fuse()
        {
            Connection.WriteRead(WriteAll(0x50, 2, GetArray(0x08, SensorCount * 2)));
            Thread.Sleep(20);
        }
        #endregion

        public override int GetTempIndex(int sensorIndex, int tempCount = 4)
        {
            if (tempCount == 4)
            {
                if (sensorIndex >= 4 && sensorIndex <= 7) return 1;
                if (sensorIndex >= 8 && sensorIndex <= 11) return 2;
                if (sensorIndex >= 12 && sensorIndex <= 15) return 3;
            }
            else if (tempCount == 2)
            {
                if (sensorIndex > 7) return 1;
            }
            return 0;
        }
        public override Sensor GetSensor(int sensorIndex)
        {
            return SensorDataGroup[sensorIndex];
        }

        #region 烧录操作
        public override byte[] WriteAllFuseData(byte address = 0x34, byte length = 28)
        {
            BOE2520UID();
            List<byte> bytes = [];
            for (int i = SensorDataGroup.Count - 1; i >= 0; i--)
                bytes.AddRange(SensorDataGroup[i].CurrentCalibrationData.registerData);
            return Connection.WriteRead(WriteAll(address, length, [.. bytes]));
        }
        public byte[] Fuse(bool[] selectedSensor, byte address = 0x34, byte count = 28, byte speed = 0x00)
        {
            BOE2520Fuse();
            byte[] sensorAddressBytes = [0x00, 0x00];

            int j = 0;
            for (int i = 0; i < Math.Min(selectedSensor.Length, SensorCount); i++)
            {
                if (i % 8 == 0 && i != 0) j++;//8字节计数
                sensorAddressBytes[j] = BytesTool.SetBit(sensorAddressBytes[j], (ushort)(i - j * 8), selectedSensor[i]);
            }

            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x31, sensorAddressBytes[1], sensorAddressBytes[0], speed, I2CAddress, address, count];
            SendData sendData = new(CRC16.CRC16_1(sendBytes), 12);
            return Connection.WriteRead(sendData);
        }
        public override byte[] Fuse(byte address = 0x34, byte count = 28, byte speed = 0x00)
        {
            return Fuse(SelectedSensor, address, count, speed);
        }
        #endregion

        #region 数据采集
        /// <summary>
        /// 得到采集卡所有芯片UID
        /// </summary>
        /// <returns></returns>
        public override int[] GetSensorsUID()
        {
            int[] uidArray = new int[SensorCount];
            //采集UID
            BOE2520UID();
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
        /// <param name="tRawArray">温度数据</param>
        /// <param name="pRawArray">压力数据</param>
        public override void GetSensorsData(out int[] tRawArray, out int[] pRawArray)
        {
            tRawArray = new int[SensorCount];
            pRawArray = new int[SensorCount];
            BOE2520RawData();
            var pressResult = ReceivedData.ParseData(ReadAll(0x17, 3), SensorCount);
            var tempResult = ReceivedData.ParseData(ReadAll(0x1A, 2), SensorCount);
            byte[] tempBytes = new byte[4];
            for (int i = 0; i < SensorCount; i++)
            {
                Array.Clear(tempBytes);
                if (pressResult[i].IsEffective && tempResult[i].IsEffective)
                {
                    tempResult[i].Data.CopyTo(tempBytes, 0);
                    tRawArray[i] = BitConverter.ToInt32(tempBytes);
                    pressResult[i].Data.CopyTo(tempBytes, 0);
                    pRawArray[i] = BitConverter.ToInt32(tempBytes);
                }
            }
        }
        /// <summary>
        /// 得到采集卡所有芯片输出
        /// </summary>
        /// <param name="tArray">温度值</param>
        /// <param name="pArray">压力值</param>
        public override void GetSensorsOutput(out decimal[] tArray, out decimal[] pArray)
        {
            ReceivedData[] tempResult;
            ReceivedData[] pressResult;
            tArray = new decimal[SensorCount];
            pArray = new decimal[SensorCount];
            byte[] tempBytes = new byte[4];
            byte[] pressBytes = new byte[4];

            BOE2520Output();
            pressResult = ReceivedData.ParseData(ReadAll(0x17, 3), SensorCount);
            tempResult = ReceivedData.ParseData(ReadAll(0x1A, 2), SensorCount);
            for (int i = 0; i < SensorCount; i++)
            {
                if (pressResult[i].IsEffective && tempResult[i].IsEffective)
                {
                    Array.Clear(tempBytes);

                    tempResult[i].Data.CopyTo(tempBytes, 0);
                    tArray[i] = BitConverter.ToInt32(tempBytes) / 128m - 273.15m;
                    SensorDataGroup[i].OutputT.Add(tArray[i]);
                    
                    pressResult[i].Data.CopyTo(tempBytes, 0);
                    pArray[i] = BitConverter.ToInt32(tempBytes) / 64m;
                    SensorDataGroup[i].OutputP.Add(pArray[i]);

                    SensorDataGroup[i].SetSensorInfo(tArray[i], pArray[i]);
                }
            }
        }
        /// <summary>
        /// 采集一次组中所有传感器的数据(单温度单压力)
        /// </summary>
        /// <param name="acquisitionTimes">采集次数</param>
        /// <param name="setP">设置压力</param>
        /// <param name="setT">设置温度</param>
        public override decimal[] GetData(decimal setP, decimal setT, out decimal press, bool isTest = false)
        {
            int acquisitionTimes = 1;
            lock (Connection)
            {
                if (isTest)
                {
                    //采集温度压力实时数据
                    press = (decimal)random.NextDouble() * 1000000;
                    return ReadTemperature(isTest);
                }
                //UID读取
                int[] uidArray = GetSensorsUID();
                for (int i = 0; i < SensorCount; i++)
                {
                    var ori = SensorDataGroup[i].CurrentRawData.Where((arg) => (arg.SetP == setP) && (arg.SetT == setT)).First();
                    SensorDataGroup[i].Uid = uidArray[i];
                    ori.uid = uidArray[i];
                }
                //采集温度压力实时数据
                press = config.PACE.GetPress();
                decimal[] currentTemp = ReadTemperature(isTest);
                //平均数存储列表
                List<RawDataBOE2520[]> averageList = [];
                //采集平均数据
                for (int j = 0; j < acquisitionTimes; j++)
                {
                    RawDataBOE2520[] averageArray = new RawDataBOE2520[SensorCount];
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
        }
        #endregion

        /// <summary>
        /// 初始化测试数据容器，清除数据并添加新的测试数据容器
        /// </summary>
        public override void ReinitializeData()
        {
            //初始化每个传感器数据
            for (int i = 0; i < SensorCount; i++)
                SensorDataGroup[i].ReinitializeData(config.CP);
        }

        public override void Calculate()
        {
            foreach (var sensorData in SensorDataGroup.Values)
                sensorData.Calculate(true);
        }

        public override void Verify(decimal setP = 0, decimal setT = 0, bool isTest = false)
        {
            decimal press = config.PACE.GetPress(isTest: isTest);
            decimal[] currentTemp = ReadTemperature(isTest);
            if (isTest)
            {
                foreach (var sensorData in SensorDataGroup.Values)
                    sensorData.Verify(config.CP.FusePDiff, config.CP.FuseTDiff);
                return;
            }
            GetSensorsData(out int[] tempArray, out int[] pressArray);
            for (int i = 0; i < SensorCount; i++)
            {
                ValidationBOE2520 validationData = new(SensorDataGroup[i].Uid, setP, setT, press, currentTemp[GetTempIndex(i)], pressArray[i], tempArray[i])
                {
                    Date = DateTime.Now.ToString("G")
                };
                SensorDataGroup[i].CurrentValidationData.Add(validationData);
                SensorDataGroup[i].Verify(config.CP.FusePDiff, config.CP.FuseTDiff);
            }
        }

        public override void SelectSensor()
        {
            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < SensorDataGroup.Count; i++)
            {
                if (SensorDataGroup[i].Result == "GOOD") SelectedSensor[i] = true;
                if (SensorDataGroup[i].IsFused) SelectedSensor[i] = false;
            }
        }

        public override bool SaveData(string path = "Data\\SensorData\\")
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

        public override void LoadData(string fileName, string path = "Data\\SensorData\\")
        {
            var loadData = JsonManager.ReadJsonString<ConcurrentDictionary<int, SensorBOE2520>>(path, fileName);
            if (loadData == null) return;
            foreach (var item in loadData)
            {
                SensorDataGroup.AddOrUpdate(item.Key, item.Value, (key, oldValue) => { return item.Value; });
            }
        }

        public override async Task SaveDatabase()
        {
            for (int i = 0; i < SensorDataGroup.Count; i++)
            {
                await config.DB.AddAllDataAsync<RawDataBOE2520>(SensorDataGroup[i].CurrentRawData);
                await config.DB.AddDataAsync(SensorDataGroup[i].CurrentCalibrationData);
                await config.DB.AddAllDataAsync<ValidationBOE2520>(SensorDataGroup[i].CurrentValidationData);
            }
        }
        /// <summary>
        /// 显示采集组数据
        /// </summary>
        /// <returns></returns>
        public override string Show()
        {
            string message = "";
            for (int i = 0; i < SensorDataGroup.Count; i++)
            {
                message += $"[{i}]{SensorDataGroup[i].ShowData()}{Environment.NewLine}";
            }
            return $"设备地址：{DeviceAddress}  传感器数量：{SensorCount}{Environment.NewLine}{message}";
        }

    }

    public class GroupZXC6862 : Group
    {
        /// <summary>
        /// 每个采集组采集16个传感器，序号0-15
        /// </summary>
        public readonly ConcurrentDictionary<int, SensorZXC6862> SensorDataGroup = [];

        public GroupZXC6862(SerialPortTool serialPort, byte deviceAddress, int sensorCount = 16) : base()
        {
            Connection = serialPort;
            DeviceAddress = deviceAddress;
            SensorCount = sensorCount;
            I2CAddress = 0x77;
            
            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < SensorCount; i++)
            {
                SensorZXC6862 sensor = new(deviceAddress, i);
                sensor.ClearData(config.CP);
                SensorDataGroup.TryAdd(i, sensor);
                SelectedSensor[i] = false;
            }
        }

        public override int GetTempIndex(int sensorIndex, int tempCount = 2)
        {
            if (tempCount == 4)
            {
                if (sensorIndex >= 4 && sensorIndex <= 7) return 1;
                if (sensorIndex >= 8 && sensorIndex <= 11) return 2;
                if (sensorIndex >= 12 && sensorIndex <= 15) return 3;
            }
            else if (tempCount == 2)
            {
                if (sensorIndex > 7) return 1;
            }
            return 0;
        }
        public override Sensor GetSensor(int sensorIndex)
        {
            return SensorDataGroup[sensorIndex];
        }

        #region 烧录操作
        public override byte[] WriteAllFuseData(byte address = 0x34, byte length = 28)
        {
            
            List<byte> bytes = [];
            for (int i = SensorDataGroup.Count - 1; i >= 0; i--)
                bytes.AddRange(SensorDataGroup[i].CurrentCalibrationData.registerData);
            return Connection.WriteRead(WriteAll(address, length, [.. bytes]));
        }
        public byte[] Fuse(bool[] selectedSensor, byte address = 0x34, byte count = 28, byte speed = 0x00)
        {
            
            byte[] sensorAddressBytes = [0x00, 0x00];

            int j = 0;
            for (int i = 0; i < Math.Min(selectedSensor.Length, SensorCount); i++)
            {
                if (i % 8 == 0 && i != 0) j++;//8字节计数
                sensorAddressBytes[j] = BytesTool.SetBit(sensorAddressBytes[j], (ushort)(i - j * 8), selectedSensor[i]);
            }

            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x31, sensorAddressBytes[1], sensorAddressBytes[0], speed, I2CAddress, address, count];
            SendData sendData = new(CRC16.CRC16_1(sendBytes), 12);
            return Connection.WriteRead(sendData);
        }
        public override byte[] Fuse(byte address = 0x34, byte count = 28, byte speed = 0x00)
        {
            return Fuse(SelectedSensor, address, count, speed);
        }
        #endregion

        #region 数据采集
        /// <summary>
        /// 得到采集卡所有芯片UID
        /// </summary>
        /// <returns></returns>
        public override int[] GetSensorsUID()
        {
            int[] uidArray = new int[SensorCount];
            
            return uidArray;
        }
        /// <summary>
        /// 得到采集卡所有芯片数据
        /// </summary>
        /// <param name="tRawArray">温度数据</param>
        /// <param name="pRawArray">压力数据</param>
        public override void GetSensorsData(out int[] tRawArray, out int[] pRawArray)
        {
            tRawArray = new int[SensorCount];
            pRawArray = new int[SensorCount];
            
        }
        /// <summary>
        /// 得到采集卡所有芯片输出
        /// </summary>
        /// <param name="tArray">温度值</param>
        /// <param name="pArray">压力值</param>
        public override void GetSensorsOutput(out decimal[] tArray, out decimal[] pArray)
        {
            //ReceivedData[] tempResult;
            //ReceivedData[] pressResult;
            tArray = new decimal[SensorCount];
            pArray = new decimal[SensorCount];
            //byte[] tempBytes = new byte[2];
            //byte[] pressBytes = new byte[4];

        }
        /// <summary>
        /// 采集一次组中所有传感器的数据(单温度单压力)
        /// </summary>
        /// <param name="acquisitionTimes">采集次数</param>
        /// <param name="setP">设置压力</param>
        /// <param name="setT">设置温度</param>
        public override decimal[] GetData(decimal setP, decimal setT, out decimal press, bool isTest = false)
        {
            lock (Connection)
            {
                press = 0;
                return [];
            }
        }
        #endregion

        /// <summary>
        /// 初始化测试数据容器，清除数据并添加新的测试数据容器
        /// </summary>
        public override void ReinitializeData()
        {
            //初始化每个传感器数据
            for (int i = 0; i < SensorCount; i++)
                SensorDataGroup[i].ClearData(config.CP);
        }

        public override void Calculate()
        {
            foreach (var sensorData in SensorDataGroup.Values)
            {
                
            }
        }

        public override void Verify(decimal setP = 0, decimal setT = 0, bool isTest = false)
        {
            decimal press = config.PACE.GetPress(isTest: isTest);
            decimal[] currentTemp = ReadTemperature(isTest);
            if (isTest)
            {
                foreach (var sensorData in SensorDataGroup.Values)
                    sensorData.Verify(config.CP.FusePDiff, config.CP.FuseTDiff);
                return;
            }
            GetSensorsData(out int[] tempArray, out int[] pressArray);
            for (int i = 0; i < SensorCount; i++)
            {
                ValidationZXC6862 validationData = new(SensorDataGroup[i].Uid, setP, setT, press, currentTemp[GetTempIndex(i)], pressArray[i], tempArray[i])
                {
                    Date = DateTime.Now.ToString("G")
                };
                SensorDataGroup[i].CurrentValidationData.Add(validationData);
                SensorDataGroup[i].Verify(config.CP.FusePDiff, config.CP.FuseTDiff);
            }
        }

        public override void SelectSensor()
        {
            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < SensorDataGroup.Count; i++)
            {
                if (SensorDataGroup[i].Result == "GOOD") SelectedSensor[i] = true;
                if (SensorDataGroup[i].IsFused) SelectedSensor[i] = false;
            }
        }

        public override bool SaveData(string path = "Data\\SensorData\\")
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

        public override void LoadData(string fileName, string path = "Data\\SensorData\\")
        {
            var loadData = JsonManager.ReadJsonString<ConcurrentDictionary<int, SensorZXC6862>>(path, fileName);
            if (loadData == null) return;
            foreach (var item in loadData)
            {
                SensorDataGroup.AddOrUpdate(item.Key, item.Value, (key, oldValue) => { return item.Value; });
            }
        }

        public override async Task SaveDatabase()
        {
            for (int i = 0; i < SensorDataGroup.Count; i++)
            {
                await config.DB.AddAllDataAsync<RawDataZXC6862>(SensorDataGroup[i].CurrentRawData);
                await config.DB.AddDataAsync(SensorDataGroup[i].CurrentCalibrationData);
                await config.DB.AddAllDataAsync<ValidationZXC6862>(SensorDataGroup[i].CurrentValidationData);
            }
        }
        /// <summary>
        /// 显示采集组数据
        /// </summary>
        /// <returns></returns>
        public override string Show()
        {
            string message = "";
            for (int i = 0; i < SensorDataGroup.Count; i++)
            {
                message += $"[{i}]{SensorDataGroup[i].ShowData()}{Environment.NewLine}";
            }
            return $"设备地址：{DeviceAddress}  传感器数量：{SensorCount}{Environment.NewLine}{message}";
        }

    }

    public class GroupZXW7570 : Group
    {
        /// <summary>
        /// 每个采集组采集16个传感器，序号0-15
        /// </summary>
        public readonly ConcurrentDictionary<int, SensorZXW7570> SensorDataGroup = [];

        public GroupZXW7570(SerialPortTool serialPort, byte deviceAddress, int sensorCount = 16) : base()
        {
            Connection = serialPort;
            DeviceAddress = deviceAddress;
            SensorCount = sensorCount;
            I2CAddress = 0x7F;

            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < SensorCount; i++)
            {
                SensorZXW7570 sensor = new(deviceAddress, i);
                sensor.ClearData(config.CP);
                SensorDataGroup.TryAdd(i, sensor);
                SelectedSensor[i] = false;
            }
        }

        public override Sensor GetSensor(int sensorIndex)
        {
            return SensorDataGroup[sensorIndex];
        }

        #region 数据采集
        /// <summary>
        /// 得到采集卡所有芯片UID
        /// </summary>
        /// <returns></returns>
        public override int[] GetSensorsUID()
        {
            int[] uidArray = new int[SensorCount];
            //采集UID
            var uidResult = ReceivedData.ParseData(ReadAll(0x01, 1), SensorCount);//读取所有传感器的uid数据
            byte[] uidBytes = new byte[2];
            for (int i = 0; i < SensorCount; i++)
            {
                if (uidResult[i].IsEffective)
                {
                    uidResult[i].Data.CopyTo(uidBytes, 1);
                    Array.Reverse(uidBytes);
                    uidArray[i] = BitConverter.ToInt16(uidBytes);
                }
            }
            return uidArray;
        }
        /// <summary>
        /// 得到采集卡所有芯片数据
        /// </summary>
        /// <param name="tRawArray">温度数据</param>
        /// <param name="pRawArray">压力数据</param>
        public override void GetSensorsData(out int[] tRawArray, out int[] pRawArray)
        {
            tRawArray = new int[SensorCount];
            pRawArray = new int[SensorCount];
            
        }
        /// <summary>
        /// 得到采集卡所有芯片输出
        /// </summary>
        /// <param name="tArray">温度值</param>
        /// <param name="pArray">压力值</param>
        public override void GetSensorsOutput(out decimal[] tArray, out decimal[] pArray)
        {
            ReceivedData[] tempResult;
            ReceivedData[] pressResult;
            tArray = new decimal[SensorCount];
            pArray = new decimal[SensorCount];
            byte[] tempBytes = new byte[2];
            byte[] pressBytes = new byte[4];

            Connection.WriteRead(WriteAll(0x30, 1, GetArray(0x0A, SensorCount)));
            Thread.Sleep(10);
            pressResult = ReceivedData.ParseData(ReadAll(0x06, 3), SensorCount);
            tempResult = ReceivedData.ParseData(ReadAll(0x09, 2), SensorCount);
            for (int i = 0; i < SensorCount; i++)
            {
                if (pressResult[i].IsEffective && tempResult[i].IsEffective)
                {
                    Array.Clear(tempBytes);
                    Array.Clear(pressBytes);

                    tempResult[i].Data.CopyTo(tempBytes, 0);
                    Array.Reverse(tempBytes);
                    tArray[i] = BitConverter.ToInt16(tempBytes) / (decimal)Math.Pow(2, 8);
                    //SensorDataGroup[i].OutputT.Add(tArray[i]);

                    pressResult[i].Data.CopyTo(pressBytes, 1);
                    Array.Reverse(pressBytes);
                    pArray[i] = BitConverter.ToInt32(pressBytes) / (decimal)Math.Pow(2, 23);
                    //SensorDataGroup[i].OutputP.Add(pArray[i]);

                    SensorDataGroup[i].SetSensorInfo(tArray[i], pArray[i]);
                }
            }
        }
        #endregion

        public override void SelectSensor()
        {
            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < SensorDataGroup.Count; i++)
            {
                if (SensorDataGroup[i].Result == "GOOD") SelectedSensor[i] = true;
                if (SensorDataGroup[i].IsFused) SelectedSensor[i] = false;
            }
        }

        public override bool SaveData(string path = "Data\\SensorData\\")
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

        public override void LoadData(string fileName, string path = "Data\\SensorData\\")
        {
            var loadData = JsonManager.ReadJsonString<ConcurrentDictionary<int, SensorZXW7570>>(path, fileName);
            if (loadData == null) return;
            foreach (var item in loadData)
            {
                SensorDataGroup.AddOrUpdate(item.Key, item.Value, (key, oldValue) => { return item.Value; });
            }
        }

        public override async Task SaveDatabase()
        {
            for (int i = 0; i < SensorDataGroup.Count; i++)
            {
                await config.DB.AddAllDataAsync<RawDataZXW7570>(SensorDataGroup[i].CurrentRawData);
                await config.DB.AddDataAsync(SensorDataGroup[i].CurrentCalibrationData);
                await config.DB.AddAllDataAsync<ValidationZXW7570>(SensorDataGroup[i].CurrentValidationData);
            }
        }
        /// <summary>
        /// 显示采集组数据
        /// </summary>
        /// <returns></returns>
        public override string Show()
        {
            string message = "";
            
            return $"设备地址：{DeviceAddress}  传感器数量：{SensorCount}{Environment.NewLine}{message}";
        }

        public override byte[] WriteAllFuseData(byte address = 52, byte length = 28)
        {
            throw new NotImplementedException();
        }

        public override byte[] Fuse(byte address = 52, byte count = 28, byte speed = 0)
        {
            throw new NotImplementedException();
        }

        public override decimal[] GetData(decimal setP, decimal setT, out decimal press, bool isTest = false)
        {
            throw new NotImplementedException();
        }

        public override void ReinitializeData()
        {
            throw new NotImplementedException();
        }

        public override void Calculate()
        {
            throw new NotImplementedException();
        }

        public override void Verify(decimal setP = 0, decimal setT = 0, bool isTest = false)
        {
            throw new NotImplementedException();
        }
    }
}
