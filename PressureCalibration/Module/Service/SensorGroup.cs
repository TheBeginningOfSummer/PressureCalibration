using CSharpKit.Communication;
using CSharpKit.DataManagement;
using CSharpKit.FileManagement;
using System.Collections.Concurrent;
using System.ComponentModel;
using static Module.SensorZXC6862;

namespace Module
{
    public abstract class Group
    {
        #region 参数
        /// <summary>
        /// 采集的温度点
        /// </summary>
        public BindingList<decimal> TempaturePoints { get; set; } = [15, 30, 50];
        /// <summary>
        /// 采集的压力点
        /// </summary>
        public BindingList<decimal> PressurePoints { get; set; } = [55000, 65000, 80000, 90000, 105000];
        /// <summary>
        /// 验证的压力点
        /// </summary>
        public BindingList<decimal> ValidatePressures { get; set; } = [50000, 100000, 110000];

        private bool isFuse = false;
        /// <summary>
        /// 是否烧录
        /// </summary>
        public bool IsFuse
        {
            get { return isFuse; }
            set { isFuse = value; }
        }
        /// <summary>
        /// 烧录的最大温差
        /// </summary>
        public double FuseTDiff { get; set; } = 0.5;
        /// <summary>
        /// 烧录时的最大差值
        /// </summary>
        public double FusePDiff { get; set; } = 30;

        /// <summary>
        /// I2C地址
        /// </summary>
        public byte I2CAddress { get; set; } = 0x7F;
        /// <summary>
        /// I2C速度
        /// </summary>
        public byte I2CSpeed { get; set; } = 0x00;
        /// <summary>
        /// 采集地址
        /// </summary>
        public byte DeviceAddress { get; set; }
        /// <summary>
        /// 每组采集传感器的数量
        /// </summary>
        public int SensorCount { get; set; } = 16;
        /// <summary>
        /// 采集卡连接
        /// </summary>
        public SerialPortTool Connection { get; set; } = new();

        /// <summary>
        /// 传感器选择数组
        /// </summary>
        public bool[] SelectedSensor = [];
        /// <summary>
        /// 当前目标温度
        /// </summary>
        public decimal TargetT = 15;
        /// <summary>
        /// 目标温度上下限
        /// </summary>
        public decimal OffsetT = 1;
        #endregion

        #region 组件
        protected readonly Random random = new();
        public readonly PressController PACE;
        public readonly Database DB;
        #endregion

        public readonly ConcurrentDictionary<int, Sensor> Sensors = [];
        public Sensor GetSensor(int sensorIndex)
        {
            return Sensors[sensorIndex];
        }
        public Label[] TInfo = new Label[4];
        public void SetTargetT(decimal targetT = 15, decimal offsetT = 1)
        {
            TargetT = targetT;
            OffsetT = offsetT;
        }
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
        public void SetTLabelInfo(decimal[] value)
        {
            if (value.Length != TInfo.Length) return;
            for (int i = 0; i < TInfo.Length; i++)
            {
                Color color;
                if (value[i] > TargetT + Math.Abs(OffsetT))
                    color = Color.Red;
                else if (value[i] < TargetT - Math.Abs(OffsetT))
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
                SetTLabelInfo(value);
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
                SetTLabelInfo(value);
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
        /// <summary>
        /// 向所有传感器指定地址写入一个字节
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <param name="speed">速度</param>
        public void Write(byte address, byte value, byte speed = 0x07)
        {
            Connection.WriteRead(WriteAll(address, 1, GetArray(value, SensorCount), speed));
        }
        #endregion

        #region 公共方法
        public abstract void InitializeData();
        public abstract void InitializeACQ();
        public abstract int[] GetSensorsUID();
        public abstract void GetSensorsData(out int[] tRawArray, out int[] pRawArray);
        public abstract void GetSensorsOutput(out decimal[] tArray, out decimal[] pArray, bool isTest = false);
        public abstract decimal[] GetData(decimal setP, decimal setT, out decimal press, bool isTest = false);
        public abstract void Calculate();
        public abstract void Validate(decimal setP = 0, decimal setT = 0, bool isTest = false);
        public abstract byte[] WriteAllFuseData(byte address = 0x34, byte length = 28);
        public abstract byte[] Fuse(byte address = 0x34, byte count = 28);
        public abstract void SelectSensor();
        public abstract bool SaveData(string path = "Data\\SensorData\\");
        public abstract void LoadData(string fileName, string path = "Data\\SensorData\\");
        public abstract Task SaveDatabase();
        public abstract string Show();
        #endregion

        public Group(PressController pace, Database db)
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
            PACE = pace;
            DB = db;
        }
    }

    public class GroupBOE2520 : Group
    {
        public GroupBOE2520(SerialPortTool serialPort, PressController pace, Database db, byte deviceAddress, int sensorCount = 16) : base(pace, db)
        {
            Connection = serialPort;
            DeviceAddress = deviceAddress;
            SensorCount = sensorCount;
            I2CAddress = 0x20;
            I2CSpeed = 0x00;
            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < SensorCount; i++)
            {
                SensorBOE2520 sensor = new(deviceAddress, i);
                sensor.InitializeData([.. TempaturePoints], [.. PressurePoints]);
                Sensors.TryAdd(i, sensor);
                SelectedSensor[i] = false;
            }
        }

        public GroupBOE2520(PressController pace, Database db, byte deviceAddress, int sensorCount = 16) : base(pace, db)
        {
            DeviceAddress = deviceAddress;
            SensorCount = sensorCount;
            I2CAddress = 0x20;
            I2CSpeed = 0x00;
            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < SensorCount; i++)
            {
                SensorBOE2520 sensor = new(deviceAddress, i);
                sensor.InitializeData([.. TempaturePoints], [.. PressurePoints]);
                Sensors.TryAdd(i, sensor);
                SelectedSensor[i] = false;
            }
        }

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

        #region 烧录操作
        public override byte[] WriteAllFuseData(byte address = 0x34, byte length = 28)
        {
            BOE2520UID();
            List<byte> bytes = [];
            for (int i = Sensors.Count - 1; i >= 0; i--)
            {
                if (Sensors[i] is SensorBOE2520 sensor)
                    bytes.AddRange(sensor.CoefficientData.RegisterData);
            }
            return Connection.WriteRead(WriteAll(address, length, [.. bytes], I2CSpeed));
        }
        public byte[] Fuse(bool[] selectedSensor, byte address = 0x34, byte count = 28)
        {
            BOE2520Fuse();
            byte[] sensorAddressBytes = [0x00, 0x00];

            int j = 0;
            for (int i = 0; i < Math.Min(selectedSensor.Length, SensorCount); i++)
            {
                if (i % 8 == 0 && i != 0) j++;//8字节计数
                sensorAddressBytes[j] = BytesTool.SetBit(sensorAddressBytes[j], (ushort)(i - j * 8), selectedSensor[i]);
            }

            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x31, sensorAddressBytes[1], sensorAddressBytes[0], I2CSpeed, I2CAddress, address, count];
            SendData sendData = new(CRC16.CRC16_1(sendBytes), 12);
            return Connection.WriteRead(sendData);
        }
        public override byte[] Fuse(byte address = 0x34, byte count = 28)
        {
            return Fuse(SelectedSensor, address, count);
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
        public override void GetSensorsOutput(out decimal[] tArray, out decimal[] pArray, bool isTest = false)
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
                    Sensors[i].OutputT.Add(tArray[i]);
                    
                    pressResult[i].Data.CopyTo(tempBytes, 0);
                    pArray[i] = BitConverter.ToInt32(tempBytes) / 64m;
                    Sensors[i].OutputP.Add(pArray[i]);

                    Sensors[i].SetSensorInfo(tArray[i], pArray[i]);
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
                    var ori = Sensors[i].RawDataList.Where((arg) => (arg.SetP == setP) && (arg.SetT == setT)).First();
                    Sensors[i].Uid = uidArray[i];
                    ori.Uid = uidArray[i];
                }
                //采集温度压力实时数据
                press = PACE.GetPress(isTest: isTest);
                decimal[] currentTemp = ReadTemperature(isTest);
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
                            PRaw = pressArray[i],
                            TRaw = tempArray[i]
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
                        praw += averageList[i][j].PRaw;
                        traw += averageList[i][j].TRaw;
                    }
                    var ori = Sensors[j].RawDataList.Where((arg) => (arg.SetP == setP) && (arg.SetT == setT)).First();
                    ori.Date = DateTime.Now.ToString("G");
                    ori.PRaw = praw / acquisitionTimes;
                    ori.TRaw = traw / acquisitionTimes;
                    ori.PRef = press;
                    ori.TRef = currentTemp[GetTempIndex(j)];
                }
                return currentTemp;
            }
        }
        #endregion

        /// <summary>
        /// 初始化测试数据容器，清除数据并添加新的测试数据容器
        /// </summary>
        public override void InitializeData()
        {
            //初始化每个传感器数据
            for (int i = 0; i < SensorCount; i++)
                Sensors[i].InitializeData([.. TempaturePoints], [.. PressurePoints]);
        }

        public override void Calculate()
        {
            foreach (SensorBOE2520 sensorData in Sensors.Values.Cast<SensorBOE2520>())
                sensorData.Calculate();
        }

        public override void Validate(decimal setP = 0, decimal setT = 0, bool isTest = false)
        {
            decimal press = PACE.GetPress(isTest: isTest);
            decimal[] currentTemp = ReadTemperature(isTest);
            if (isTest)
            {
                foreach (Sensor sensorData in Sensors.Values)
                    sensorData.Validate(FusePDiff, FuseTDiff);
                return;
            }
            GetSensorsData(out int[] tempArray, out int[] pressArray);
            for (int i = 0; i < SensorCount; i++)
            {
                Validation validationData = new(Sensors[i].Uid, setP, setT, press, currentTemp[GetTempIndex(i)], pressArray[i], tempArray[i])
                {
                    Date = DateTime.Now.ToString("G")
                };
                Sensors[i].ValidationDataList.Add(validationData);
                Sensors[i].Validate(FusePDiff, FuseTDiff);
            }
        }

        public override void SelectSensor()
        {
            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < Sensors.Count; i++)
            {
                if (Sensors[i].Result == "GOOD") SelectedSensor[i] = true;
                if (Sensors[i].IsFused) SelectedSensor[i] = false;
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
                JsonManager.SaveJsonString(dataDirectory, fileName, Sensors);
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
                Sensors.AddOrUpdate(item.Key, item.Value, (key, oldValue) => { return item.Value; });
            }
        }

        public override async Task SaveDatabase()
        {
            for (int i = 0; i < Sensors.Count; i++)
            {
                await DB.AddAllDataAsync<RawData>(Sensors[i].RawDataList);
                await DB.AddDataAsync(((SensorBOE2520)Sensors[i]).CoefficientData);
                await DB.AddAllDataAsync<Validation>(Sensors[i].ValidationDataList);
            }
        }
        /// <summary>
        /// 显示采集组数据
        /// </summary>
        /// <returns></returns>
        public override string Show()
        {
            string message = "";
            for (int i = 0; i < Sensors.Count; i++)
            {
                message += $"[{i}]{Sensors[i].ShowData()}{Environment.NewLine}";
            }
            return $"设备地址：{DeviceAddress}  传感器数量：{SensorCount}{Environment.NewLine}{message}";
        }

        public override void InitializeACQ()
        {
            throw new NotImplementedException();
        }
    }

    public class GroupZXC6862 : Group
    {
        public MinBridgeOffset[] BridgeOffset;
        public TargetGain[] AMPGain;

        public GroupZXC6862(SerialPortTool serialPort, PressController pace, Database db, byte deviceAddress, int sensorCount = 16) : base(pace, db)
        {
            Connection = serialPort;
            DeviceAddress = deviceAddress;
            SensorCount = sensorCount;
            I2CAddress = 0x77;
            BridgeOffset = new MinBridgeOffset[SensorCount];
            AMPGain = new TargetGain[SensorCount];

            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < SensorCount; i++)
            {
                SensorZXC6862 sensor = new(deviceAddress, i);
                sensor.InitializeData(TempaturePoints.ToArray(), PressurePoints.ToArray());
                Sensors.TryAdd(i, sensor);
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

        public override void InitializeACQ()
        {
            InitializeData();
            Thread.Sleep(40);
            CheckOperating();//检测芯片初始化
            GetSensorsUID();//采集芯片ID
            WriteSignature();//签名
            CheckChipRW();//检测芯片读写
            GetMinBridgeOffset();//得到电桥偏移
            GetTargetAMPGain();//得到AMP增益
            RawDataACQ();//采集准备
        }

        #region ZXC6862
        /// <summary>
        /// 签名
        /// </summary>
        public void WriteSignature()
        {
            Connection.WriteRead(WriteAll(0x0E, 2, GetArray([0xA5, 0x96], SensorCount), 0x07));
        }
        /// <summary>
        /// 压力配置设置
        /// </summary>
        public void PressureConfig()
        {
            //压力测量速率、过采样率设置
            Write(0x06, 0x36);
            //测量数据位移，FIFO 使能
            Write(0x09, 0x04);
            //设置压力温度增益
            Write(0x62, 0x02);
        }
        /// <summary>
        /// 检测传感器初始化完成和系数是否可读取
        /// </summary>
        public bool CheckOperating(byte status = 0xC0)
        {
            bool isOK = true;
            for (int n = 0; n < 3; n++)
            {
                isOK = true;
                ReceivedData[] result = ReceivedData.ParseData(ReadAll(0x08, 1, 0x07), SensorCount);
                //遍历读取结果
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i].Data.FirstOrDefault() != status)
                    {
                        if (n >= 2)
                        {
                            Sensors[i].Result = $"0x08不等于{status}|";
                            isOK = false;
                        }
                        else
                        {
                            Thread.Sleep(100);
                            isOK = false;
                            break;
                        }
                    }
                }
                if (isOK) return isOK;
            }
            return isOK;
        }
        /// <summary>
        /// 检查芯片读写
        /// </summary>
        public void CheckChipRW()
        {
            //检测芯片读写
            byte[] testData = Enumerable.Repeat<byte>(0xAA, 18).ToArray();
            Connection.WriteRead(WriteAll(0x40, 18, GetArray(testData, SensorCount), 0x07));
            Thread.Sleep(50);
            ReceivedData[] result;
            result = ReceivedData.ParseData(ReadAll(0x40, 18, 0x07), SensorCount);
            for (int i = 0; i < result.Length; i++)
            {
                if (!BytesTool.CheckEquals(testData, result[i].Data))
                    Sensors[i].Result = "读写测试失败|";
            }

            testData = Enumerable.Repeat<byte>(0x55, 18).ToArray();
            Connection.WriteRead(WriteAll(0x40, 18, GetArray(testData, SensorCount), 0x07));
            Thread.Sleep(50);
            result = ReceivedData.ParseData(ReadAll(0x40, 18, 0x07), SensorCount);
            for (int i = 0; i < result.Length; i++)
            {
                if (!BytesTool.CheckEquals(testData, result[i].Data))
                    Sensors[i].Result = "读写测试失败|";
            }

            testData = Enumerable.Repeat<byte>(0x00, 18).ToArray();
            Connection.WriteRead(WriteAll(0x40, 18, GetArray(testData, SensorCount), 0x07));
            Thread.Sleep(50);
            result = ReceivedData.ParseData(ReadAll(0x40, 18, 0x07), SensorCount);
            for (int i = 0; i < result.Length; i++)
            {
                if (!BytesTool.CheckEquals(testData, result[i].Data))
                    Sensors[i].Result = "读写测试失败|";
            }
        }

        public void SetBridgeOffset(MinBridgeOffset[] minBridgeOffset)
        {
            List<byte> bridgeOffset = [];
            for (int i = 0; i < minBridgeOffset.Length; i++)
            {
                bridgeOffset.Add(minBridgeOffset[i].BridgeOffset);
            }
            //设置电桥偏移
            Connection.WriteRead(WriteAll(0x65, 1, bridgeOffset.ToArray(), 0x07));
        }

        public void SetAMPGain(TargetGain[] gain)
        {
            List<byte> ampGain = [];
            for (int i = 0; i < gain.Length; i++)
            {
                ampGain.Add(gain[i].AMPGain);
            }
            //默认放大器增益和UI微调
            Connection.WriteRead(WriteAll(0x64, 1, ampGain.ToArray(), 0x07));
        }

        public MinBridgeOffset[] GetMinBridgeOffset()
        {
            //压力参数配置
            PressureConfig();
            //默认放大器增益和UI微调
            Connection.WriteRead(WriteAll(0x64, 1, GetArray(0x67, SensorCount), 0x07));
            //压力连续测量
            Connection.WriteRead(WriteAll(0x08, 1, GetArray(0x05, SensorCount), 0x07));
            //默认电桥偏移
            Connection.WriteRead(WriteAll(0x65, 1, GetArray(0x00, SensorCount), 0x07));
            Thread.Sleep(105);
            ReceivedData[] result;
            //read P result (clear P-readybit)
            result = ReceivedData.ParseData(Connection.WriteRead(ReadAll(0x00, 3, 0x07)));

            //创建存储最小压力偏移的数组
            MinBridgeOffset[] offsetPx = new MinBridgeOffset[result.Length];
            for (int i = 0; i < offsetPx.Length; i++)
            {
                offsetPx[i] = new MinBridgeOffset(i, 0x00, int.MaxValue);
            }
            //遍历偏移量，找到最小值
            for (int j = 0; j <= 31; j++)
            {
                Thread.Sleep(110);
                //判断采集的压力是否可用，不可用会标记为NG
                CheckOperating(0xD5);
                //读取压力原值
                result = ReceivedData.ParseData(Connection.WriteRead(ReadAll(0x00, 3, 0x07)));
                //设置电桥偏移
                Write(0x65, (byte)j);
                byte[] data = new byte[4];
                //遍历采集组采集的结果
                for (int i = 0; i < result.Length; i++)
                {
                    Array.Clear(data);
                    result[i].Data.CopyTo(data, 1);
                    Array.Reverse(data);//转换小端
                    int press = BitConverter.ToInt32(data, 0);//转为整数
                    offsetPx[i].SetValue((byte)j, press);
                }
            }
            //设置偏移量
            SetBridgeOffset(offsetPx);
            //停止压力测量
            Write(0x08, 0x00);
            //read P result (clear P-readybit)
            ReceivedData.ParseData(Connection.WriteRead(ReadAll(0x00, 3, 0x07)));
            //检测传感器初始化是否完成
            CheckOperating();
            BridgeOffset = offsetPx;
            return offsetPx;
        }

        public TargetGain[] GetTargetAMPGain()
        {
            //压力参数配置
            PressureConfig();
            //默认放大器增益和UI微调
            Write(0x64, 0x07);
            //设置偏移量
            SetBridgeOffset(BridgeOffset);
            //压力连续测量
            Write(0x08, 0x05);
            Thread.Sleep(105);
            ReceivedData[] result;
            //read P result (clear P-readybit)
            result = ReceivedData.ParseData(Connection.WriteRead(ReadAll(0x00, 3, 0x07)));

            //创建存储amp增益的数组
            TargetGain[] targetGains = new TargetGain[result.Length];
            for (int i = 0; i < targetGains.Length; i++)
            {
                targetGains[i] = new TargetGain(i, 0x07, 0);
            }
            //遍历，找到目标增益
            for (int j = 0; j <= 15; j++)
            {
                byte b1 = (byte)j;
                byte varyGain = (byte)(b1 << 4 | 0x07);
                Thread.Sleep(110);
                //判断采集的压力是否可用，不可用会标记为NG
                CheckOperating(0xD5);
                //读取压力原值
                result = ReceivedData.ParseData(Connection.WriteRead(ReadAll(0x00, 3, 0x07)));
                //设置AMPGain
                Write(0x64, varyGain);
                byte[] data = new byte[4];
                //遍历采集组采集的结果
                for (int i = 0; i < result.Length; i++)
                {
                    Array.Clear(data);
                    result[i].Data.CopyTo(data, 1);
                    Array.Reverse(data);//转换小端
                    int press = BitConverter.ToInt32(data, 0);//转为整数
                    targetGains[i].SetValue(varyGain, press);
                }
            }
            //设置增益
            SetAMPGain(targetGains);
            //停止压力测量
            Write(0x08, 0x00);
            //read P result (clear P-readybit)
            ReceivedData.ParseData(Connection.WriteRead(ReadAll(0x00, 3, 0x07)));
            //检测传感器初始化是否完成
            CheckOperating();
            AMPGain = targetGains;
            return targetGains;
        }

        public void RawDataACQ()
        {
            //设置偏移量
            SetBridgeOffset(BridgeOffset);
            //设置增益
            SetAMPGain(AMPGain);
            //签名
            WriteSignature();
            //压力测量速率、过采样率设置
            Write(0x06, 0x36);
            //测量数据位移，FIFO 使能
            Write(0x09, 0x04);
            //配置温度测量速率和分辨率
            Write(0x07, 0xB0);
            //设置压力温度增益
            Write(0x62, 0x02);
            //开始压力温度测量
            Write(0x08, 0x07);
            Thread.Sleep(200);
        }
        #endregion

        #region 烧录操作
        public override byte[] WriteAllFuseData(byte address = 0x40, byte length = 25)
        {
            List<byte> bytes = [];
            for (int i = Sensors.Count - 1; i >= 0; i--)
            {
                if (Sensors[i] is SensorZXC6862 sensor)
                    bytes.AddRange(sensor.CoefficientData.RegisterData);
            }

            List<byte> bytes2 = [];
            byte[] gainAndOffset = new byte[10];
            for (int i = BridgeOffset.Length - 1; i >= 0; i--)
            {
                gainAndOffset[4] = AMPGain[i].AMPGain;
                gainAndOffset[5] = BridgeOffset[i].BridgeOffset;
                bytes2.AddRange(gainAndOffset);
            }

            byte[] bytes3 = new byte[Sensors.Count];

            var r1 = Connection.WriteRead(WriteAll(address, length, [.. bytes]));
            var r2 = Connection.WriteRead(WriteAll(0x60, 10, [.. bytes2]));
            var r3 = Connection.WriteRead(WriteAll(0x6F, 1, bytes3));
            return r1;
        }
        /// <summary>
        /// 待修改
        /// </summary>
        /// <param name="selectedSensor"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public byte[] Fuse(bool[] selectedSensor, byte address = 0x34, byte count = 28)
        {
            byte[] sensorAddressBytes = [0x00, 0x00];

            int j = 0;
            for (int i = 0; i < Math.Min(selectedSensor.Length, SensorCount); i++)
            {
                if (i % 8 == 0 && i != 0) j++;//8字节计数
                sensorAddressBytes[j] = BytesTool.SetBit(sensorAddressBytes[j], (ushort)(i - j * 8), selectedSensor[i]);
            }

            byte[] sendBytes = [0x24, DeviceAddress, 0x80, 0x31, sensorAddressBytes[1], sensorAddressBytes[0], I2CSpeed, I2CAddress, address, count];
            SendData sendData = new(CRC16.CRC16_1(sendBytes), 12);
            return Connection.WriteRead(sendData);
        }
        public override byte[] Fuse(byte address = 0x34, byte count = 28)
        {
            return Fuse(SelectedSensor, address, count);
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

            CheckOperating(0xC0);

            ReceivedData[] result = ReceivedData.ParseData(ReadAll(0x25, 4, 0x07), SensorCount);
            byte[] uidBytes = new byte[4];
            for (int i = 0; i < result.Length; i++)
            {
                byte v = result[i].Data[3];
                result[i].Data[3] = BytesTool.SetBit(v, 7, false);
                result[i].Data.CopyTo(uidBytes, 0);
                uidArray[i] = BitConverter.ToInt32(uidBytes);
                Sensors[i].Uid = uidArray[i];
                Sensors[i].SetUID();
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
            ReceivedData[] result;
            result = ReceivedData.ParseData(Connection.WriteRead(ReadAll(0x00, 6, 0x07)));
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].IsEffective)
                {
                    byte[] bytes = result[i].Data;
                    pRawArray[i] = BitConverter.ToInt32([bytes[2], bytes[1], bytes[0], 0x00]);
                    tRawArray[i] = BitConverter.ToInt32([bytes[5], bytes[4], bytes[3], 0x00]);
                }
            }
        }
        /// <summary>
        /// 得到采集卡所有芯片输出
        /// </summary>
        /// <param name="tArray">温度值</param>
        /// <param name="pArray">压力值</param>
        public override void GetSensorsOutput(out decimal[] tArray, out decimal[] pArray, bool isTest = false)
        {
            //ReceivedData[] tempResult;
            //ReceivedData[] pressResult;
            tArray = new decimal[SensorCount];
            pArray = new decimal[SensorCount];
            //byte[] tempBytes = new byte[2];
            //byte[] pressBytes = new byte[4];
            if (isTest)
            {
                for (int i = 0; i < SensorCount; i++)
                {
                    tArray[i] = (decimal)random.NextDouble() * 100;
                    pArray[i] = (decimal)random.NextDouble() * 100;
                    Sensors[i].SetSensorInfo(tArray[i], pArray[i]);
                }
                return;
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
            lock (Connection)
            {
                if (isTest)
                {
                    //采集温度压力实时数据
                    press = (decimal)random.NextDouble() * 1000000;
                    return ReadTemperature(isTest);
                }
                //采集温度压力实时数据
                press = PACE.GetPress(isTest: isTest);
                decimal[] currentTemp = ReadTemperature(isTest);
                GetSensorsData(out int[] tempArray, out int[] pressArray);
                for (int i = 0; i < SensorCount; i++)
                {
                    var ori = Sensors[i].RawDataList.Where((arg) => (arg.SetP == setP) && (arg.SetT == setT)).First();
                    ori.Date = DateTime.Now.ToString("G");
                    ori.PRaw = pressArray[i];
                    ori.TRaw = tempArray[i];
                    ori.PRef = press;
                    ori.TRef = currentTemp[GetTempIndex(i)];
                }
                return currentTemp;
            }
        }
        #endregion

        /// <summary>
        /// 初始化测试数据容器，清除数据并添加新的测试数据容器
        /// </summary>
        public override void InitializeData()
        {
            //初始化每个传感器数据
            for (int i = 0; i < SensorCount; i++)
                Sensors[i].InitializeData(TempaturePoints.ToArray(), PressurePoints.ToArray());
        }

        public override void Calculate()
        {
            Write(0x08, 0x00);//停止采集
            foreach (SensorZXC6862 sensorData in Sensors.Values.Cast<SensorZXC6862>())
                sensorData.Calculate();
        }

        public override void Validate(decimal setP = 0, decimal setT = 0, bool isTest = false)
        {
            decimal press = PACE.GetPress(isTest: isTest);
            decimal[] currentTemp = ReadTemperature(isTest);
            if (isTest)
            {
                foreach (Sensor sensorData in Sensors.Values)
                    sensorData.Validate(FusePDiff, FuseTDiff);
                return;
            }
            GetSensorsData(out int[] tempArray, out int[] pressArray);
            for (int i = 0; i < SensorCount; i++)
            {
                Validation validationData = new(Sensors[i].Uid, setP, setT, press, currentTemp[GetTempIndex(i)], pressArray[i], tempArray[i])
                {
                    Date = DateTime.Now.ToString("G")
                };
                Sensors[i].ValidationDataList.Add(validationData);
                Sensors[i].Validate(FusePDiff, FuseTDiff);
            }
        }

        public override void SelectSensor()
        {
            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < Sensors.Count; i++)
            {
                if (Sensors[i].Result == "GOOD") SelectedSensor[i] = true;
                if (Sensors[i].IsFused) SelectedSensor[i] = false;
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
                JsonManager.SaveJsonString(dataDirectory, fileName, Sensors);
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
                Sensors.AddOrUpdate(item.Key, item.Value, (key, oldValue) => { return item.Value; });
            }
        }

        public override async Task SaveDatabase()
        {
            for (int i = 0; i < Sensors.Count; i++)
            {
                await DB.AddAllDataAsync<RawData>(Sensors[i].RawDataList);
                await DB.AddDataAsync(((SensorZXC6862)Sensors[i]).CoefficientData);
                await DB.AddAllDataAsync<Validation>(Sensors[i].ValidationDataList);
            }
        }
        /// <summary>
        /// 显示采集组数据
        /// </summary>
        /// <returns></returns>
        public override string Show()
        {
            string message = "";
            for (int i = 0; i < Sensors.Count; i++)
            {
                message += $"[{i}]{Sensors[i].ShowData()}{Environment.NewLine}";
            }
            return $"设备地址：{DeviceAddress}  传感器数量：{SensorCount}{Environment.NewLine}{message}";
        }

    }

    public class GroupZXW7570 : Group
    {
        public GroupZXW7570(SerialPortTool serialPort, PressController pace, Database db, byte deviceAddress, int sensorCount = 16) : base(pace, db)
        {
            Connection = serialPort;
            DeviceAddress = deviceAddress;
            SensorCount = sensorCount;
            I2CAddress = 0x7F;

            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < SensorCount; i++)
            {
                SensorZXW7570 sensor = new(deviceAddress, i);
                Sensors.TryAdd(i, sensor);
                SelectedSensor[i] = false;
            }
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
            var uidResult = ReceivedData.ParseData(ReadAll(0x02, 1, 0x07), SensorCount);//读取所有传感器的uid数据
            //byte[] uidBytes = new byte[2];
            for (int i = 0; i < SensorCount; i++)
            {
                if (uidResult[i].IsEffective)
                {
                    //uidResult[i].Data.CopyTo(uidBytes, 1);
                    uidArray[i] = (int)uidResult[i].Data[0];
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
        public override void GetSensorsOutput(out decimal[] tArray, out decimal[] pArray, bool isTest = false)
        {
            ReceivedData[] tempResult;
            ReceivedData[] pressResult;
            tArray = new decimal[SensorCount];
            pArray = new decimal[SensorCount];

            if (isTest)
            {
                for (int i = 0; i < SensorCount; i++)
                {
                    tArray[i] = (decimal)random.NextDouble() * 100;
                    pArray[i] = (decimal)random.NextDouble() * 100;
                    Sensors[i].SetSensorInfo(tArray[i], pArray[i]);
                }
                return;
            }

            byte[] tempBytes = new byte[2];
            byte[] pressBytes = new byte[4];

            Connection.WriteRead(WriteAll(0x30, 1, GetArray(0x0A, SensorCount),0x07));
            Thread.Sleep(100);
            pressResult = ReceivedData.ParseData(ReadAll(0x06, 3, 0x07), SensorCount);
            Thread.Sleep(100);
            tempResult = ReceivedData.ParseData(ReadAll(0x09, 2, 0x07), SensorCount);
            for (int i = 0; i < SensorCount; i++)
            {
                if (pressResult[i].IsEffective || tempResult[i].IsEffective)
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

                    Sensors[i].SetSensorInfo(tArray[i], pArray[i]);
                }
            }
        }
        #endregion

        public override void SelectSensor()
        {
            SelectedSensor = new bool[SensorCount];
            for (int i = 0; i < Sensors.Count; i++)
            {
                if (Sensors[i].Result == "GOOD") SelectedSensor[i] = true;
                if (Sensors[i].IsFused) SelectedSensor[i] = false;
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
                JsonManager.SaveJsonString(dataDirectory, fileName, Sensors);
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
                Sensors.AddOrUpdate(item.Key, item.Value, (key, oldValue) => { return item.Value; });
            }
        }

        public override async Task SaveDatabase()
        {
            for (int i = 0; i < Sensors.Count; i++)
            {
                await DB.AddAllDataAsync<RawData>(Sensors[i].RawDataList);
                //await DB.AddDataAsync(Sensors[i].CoefficientData);
                await DB.AddAllDataAsync<Validation>(Sensors[i].ValidationDataList);
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

        public override byte[] Fuse(byte address = 52, byte count = 28)
        {
            throw new NotImplementedException();
        }

        public override decimal[] GetData(decimal setP, decimal setT, out decimal press, bool isTest = false)
        {
            throw new NotImplementedException();
        }

        public override void InitializeData()
        {
            throw new NotImplementedException();
        }

        public override void Calculate()
        {
            throw new NotImplementedException();
        }

        public override void Validate(decimal setP = 0, decimal setT = 0, bool isTest = false)
        {
            throw new NotImplementedException();
        }

        public override void InitializeACQ()
        {
            throw new NotImplementedException();
        }
    }
}
