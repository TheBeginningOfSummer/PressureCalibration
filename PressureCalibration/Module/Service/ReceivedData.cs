using CSharpKit.Communication;
using CSharpKit.DataManagement;

namespace Module
{
    public class ReceivedData
    {
        /// <summary>
        /// 设备地址
        /// </summary>
        public byte DeviceAddress { get; set; }
        /// <summary>
        /// 传感器索引地址
        /// </summary>
        public int SensorIndex { get; set; } = -1;
        /// <summary>
        /// 是否短路
        /// </summary>
        public bool IsFault { get; set; } = false;
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsEffective { get; set; } = false;
        /// <summary>
        /// 消息类型
        /// </summary>
        public ReceivedType DataType { get; set; } = ReceivedType.None;
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; set; } = [];

        /// <summary>
        /// 电压
        /// </summary>
        public double Voltage { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public decimal Temperature { get; set; }

        //读取数据
        public ReceivedData(byte deviceAddress, int sensorIndex, bool isFault, bool isEffective, ReceivedType dataType, byte[]? data)
        {
            DeviceAddress = deviceAddress;
            SensorIndex = sensorIndex;
            IsFault = isFault;
            IsEffective = isEffective;
            DataType = dataType;
            if (data != null) Data = data;
        }
        
        /// <summary>
        /// 解析与分割数据
        /// </summary>
        /// <param name="bytes">要解析的字节</param>
        /// <param name="receivedDataCount">读取或写入通路个数</param>
        /// <param name="tempCount">温度数据通路个数</param>
        /// <returns></returns>
        public static ReceivedData[] ParseData(byte[] bytes, int receivedDataCount = 16, int tempCount = 4)
        {
            if (receivedDataCount < 1) receivedDataCount = 1;
            ReceivedData[] recivedList = new ReceivedData[receivedDataCount];
            if (bytes[0] == 0x24)
            {
                byte[] data = bytes.Take(bytes.Length - 2).ToArray();
                byte deviceAddress = bytes[1];
                bool isFault = bytes[2] == 0x01;
                byte[] sensorAddress = [];
                int registerLength = 0;
                if (bytes.Length >= 12)
                {
                    sensorAddress = [bytes[4], bytes[5]];
                    registerLength = bytes[9];
                }

                if (BytesTool.CheckEquals(bytes, CRC16.CRC16_1(data)))
                {
                    switch (bytes[3])
                    {
                        case 0x20://读取数据
                            byte[] registerData = data.Skip(10).ToArray();
                            int count = registerData.Length / registerLength;
                            for (int i = 0; i < receivedDataCount; i++)
                            {
                                bool isEffective = BytesTool.GetBit(sensorAddress, (ushort)i);
                                if (isEffective)
                                {
                                    count--;
                                    byte[] register = BytesTool.CutBytesByLength(registerData, count * registerLength, registerLength);
                                    recivedList[i] = new ReceivedData(deviceAddress, i, isFault, isEffective, ReceivedType.Read, register);
                                }
                                else
                                {
                                    recivedList[i] = new ReceivedData(deviceAddress, i, isFault, isEffective, ReceivedType.Read, []);
                                }
                            }
                            break;
                        case 0x30://写入是否成功
                            for (int i = 0; i < receivedDataCount; i++)
                            {
                                bool isEffective = BytesTool.GetBit(sensorAddress, (ushort)i);
                                recivedList[i] = new ReceivedData(deviceAddress, i, isFault, isEffective, ReceivedType.Write, null);
                            }
                            break;
                        case 0x21://读取温度
                            byte[] temperatureData = data.Skip(10).ToArray();
                            if (temperatureData.Length == tempCount * registerLength)//读取温度
                            {
                                int j = tempCount - 1;
                                for (int i = 0; i < tempCount; i++)
                                {
                                    bool isEffective = BytesTool.GetBit(sensorAddress, (ushort)i);
                                    recivedList[i] = new ReceivedData(deviceAddress, i, isFault, isEffective, ReceivedType.ReadTemp,
                                        BytesTool.CutBytesByLength(temperatureData, j * registerLength, registerLength));
                                    recivedList[i].GetData();
                                    j--;
                                }
                            }
                            break;
                        case 0x31://烧录是否成功
                            for (int i = 0; i < receivedDataCount; i++)
                            {
                                bool isEffective = BytesTool.GetBit(sensorAddress, (ushort)i);
                                recivedList[i] = new ReceivedData(deviceAddress, i, isFault, isEffective, ReceivedType.Fuse, null);
                            }
                            break;
                        case 0x13://电源置1.8V
                            recivedList[0] = new ReceivedData(deviceAddress, -1, isFault, false, ReceivedType.Voltage1_8, [bytes[4]]);
                            recivedList[0].GetData();
                            break;
                        case 0x12://电源置4.1V
                            recivedList[0] = new ReceivedData(deviceAddress, -1, isFault, false, ReceivedType.Voltage4_1, [bytes[4]]);
                            recivedList[0].GetData();
                            break;
                        case 0x11://电源置3.3V
                            recivedList[0] = new ReceivedData(deviceAddress, -1, isFault, false, ReceivedType.Voltage3_3, [bytes[4]]);
                            recivedList[0].GetData();
                            break;
                        case 0x10://电源关闭
                            recivedList[0] = new ReceivedData(deviceAddress, -1, isFault, false, ReceivedType.PowerOff, [bytes[4]]);
                            recivedList[0].GetData();
                            break;
                        case 0x00://板卡信息
                            recivedList[0] = new ReceivedData(deviceAddress, -1, isFault, false, ReceivedType.Info, [bytes[4]]);
                            recivedList[0].GetData();
                            break;
                        default: break;
                    }
                    return recivedList;
                }
                else
                {
                    recivedList[0] = new ReceivedData(deviceAddress, -1, false, false, ReceivedType.None, null);
                    //无效数据
                    return recivedList;
                }
            }
            else
            {
                recivedList[0] = new ReceivedData(0, -1, false, false, ReceivedType.Timeout, null);
                //超时数据
                return recivedList;
            }
        }

        public void SetData(byte deviceAddress, int sensorIndex, bool isFault, bool isEffective, ReceivedType dataType, byte[]? data = null)
        {
            DeviceAddress = deviceAddress;
            SensorIndex = sensorIndex;
            IsFault = isFault;
            IsEffective = isEffective;
            DataType = dataType;
            if (data != null) Data = data;
        }

        public void GetData()
        {
            if (DataType.Equals(ReceivedType.Info) || DataType.Equals(ReceivedType.Voltage3_3) || DataType.Equals(ReceivedType.Voltage4_1) || DataType.Equals(ReceivedType.Voltage1_8))
            {
                switch (Data[0])
                {
                    case 0x01: Voltage = 3.3; break;
                    case 0x02: Voltage = 4.1; break;
                    case 0x03: Voltage = 1.8; break;
                    default: Voltage = Data[0]; break;
                }
            }
            if (DataType.Equals(ReceivedType.ReadTemp))
            {
                Temperature = BitConverter.ToInt16(Data) * 0.0078125M;
            }
        }

        public string Show()
        {
            if (DataType == ReceivedType.Read)
                return $"设备:{DeviceAddress}  消息类型:{DataType}  传感器地址：{SensorIndex}  是否有效：{IsEffective}{Environment.NewLine}" +
                    $"数据：{DataConverter.BytesToHexString(Data)}";
            else if (DataType == ReceivedType.Timeout)
                return $"数据无法解析或超时";
            else if (DataType == ReceivedType.Info)
                return $"设备:{DeviceAddress} 当前电压:{Voltage}";
            else if (DataType == ReceivedType.ReadTemp)
                return $"设备:{DeviceAddress} 读取温度，当前温度:{Temperature}";
            else if (DataType == ReceivedType.PowerOff)
                return $"设备:{DeviceAddress} 断电";
            else if (DataType == ReceivedType.Voltage1_8)
                if (IsFault)
                    return $"设备:{DeviceAddress} 短路(1.8V)";
                else
                    return $"设备:{DeviceAddress} 设为1.8V";
            else if (DataType == ReceivedType.Voltage3_3)
                if (IsFault)
                    return $"设备:{DeviceAddress} 短路(3.3V)";
                else
                    return $"设备:{DeviceAddress} 设为3.3V";
            else if (DataType == ReceivedType.Voltage4_1)
                if (IsFault)
                    return $"设备:{DeviceAddress} 短路(4.1V)";
                else
                    return $"设备:{DeviceAddress} 设为4.1V";
            else
                return $"设备:{DeviceAddress}  消息类型:{DataType}  传感器地址：{SensorIndex}  是否有效：{IsEffective}{Environment.NewLine}";
        }
    }

    public enum ReceivedType
    {
        None,
        Timeout,
        Info,
        Read,
        Write,
        Fuse,
        ReadTemp,
        PowerOff,
        Voltage1_8,
        Voltage3_3,
        Voltage4_1
    }
}
