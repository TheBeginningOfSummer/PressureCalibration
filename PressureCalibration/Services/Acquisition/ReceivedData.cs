using CSharpKit.Communication;
using CSharpKit.DataManagement;

namespace Calibration.Services
{
    public class ReceivedData
    {
        public byte DeviceAddress { get; set; }
        public int SensorIndex { get; set; }
        public bool IsFault { get; set; } = false;
        public int Voltage { get; set; }

        public bool IsEffective { get; set; } = false;
        public byte[] Data { get; set; } = [];
        public ReceivedType DataType { get; set; } = ReceivedType.None;

        //读取数据
        public ReceivedData(byte deviceAddress, int index, bool isEffective, byte[] data, ReceivedType dataType = ReceivedType.Read)
        {
            DeviceAddress = deviceAddress;
            SensorIndex = index;
            IsEffective = isEffective;
            Data = data;
            DataType = dataType;
        }
        //写入数据
        public ReceivedData(byte deviceAddress, int index, bool isEffective, ReceivedType dataType = ReceivedType.Write)
        {
            DeviceAddress = deviceAddress;
            SensorIndex = index;
            IsEffective = isEffective;
            DataType = dataType;
        }
        //电源
        public ReceivedData(byte deviceAddress, bool isFault, ReceivedType dataType = ReceivedType.PowerOff)
        {
            DeviceAddress = deviceAddress;
            IsFault = isFault;
            DataType = dataType;
        }
        //错误数据
        public ReceivedData(byte deviceAddress, ReceivedType dataType = ReceivedType.None)
        {
            DeviceAddress = deviceAddress;
            DataType = dataType;
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
                            if (registerData.Length == receivedDataCount * registerLength)//读全部数据
                            {
                                int j = receivedDataCount;
                                for (int i = 0; i < receivedDataCount; i++)
                                {
                                    j--;
                                    bool isEffective = BytesTool.GetBit(sensorAddress, (ushort)i);
                                    byte[] register = BytesTool.CutBytesByLength(registerData, j * registerLength, registerLength);
                                    recivedList[i] = new ReceivedData(deviceAddress, i, isEffective, register);
                                }
                            }
                            else//读部分数据
                            {
                                int j = registerData.Length / registerLength;
                                for (int i = 0; i < receivedDataCount; i++)
                                {
                                    bool isEffective = BytesTool.GetBit(sensorAddress, (ushort)i);
                                    if (isEffective)
                                    {
                                        j--;
                                        byte[] register = BytesTool.CutBytesByLength(registerData, j * registerLength, registerLength);
                                        recivedList[i] = new ReceivedData(deviceAddress, i, isEffective, register);
                                    }
                                    else
                                    {
                                        recivedList[i] = new ReceivedData(deviceAddress, i, isEffective, []);
                                    }
                                }
                            }
                            break;
                        case 0x30://写入是否成功
                            for (int i = 0; i < receivedDataCount; i++)
                            {
                                bool isEffective = BytesTool.GetBit(sensorAddress, (ushort)i);
                                recivedList[i] = new ReceivedData(deviceAddress, i, isEffective);
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
                                    recivedList[i] = new ReceivedData(deviceAddress, i, isEffective, BytesTool.CutBytesByLength(temperatureData, j * registerLength, registerLength), ReceivedType.ReadTemp);
                                    j--;
                                }
                            }
                            break;
                        case 0x31://烧录是否成功
                            for (int i = 0; i < receivedDataCount; i++)
                            {
                                bool isEffective = BytesTool.GetBit(sensorAddress, (ushort)i);
                                recivedList[i] = new ReceivedData(deviceAddress, i, isEffective, ReceivedType.Fuse);
                            }
                            break;
                        case 0x13://电源置1.8V
                            recivedList[0] = new ReceivedData(deviceAddress, isFault, ReceivedType.Voltage1_8);
                            break;
                        case 0x12://电源置4.1V
                            recivedList[0] = new ReceivedData(deviceAddress, isFault, ReceivedType.Voltage4_1);
                            break;
                        case 0x11://电源置3.3V
                            recivedList[0] = new ReceivedData(deviceAddress, isFault, ReceivedType.Voltage3_3);
                            break;
                        case 0x10://电源关闭
                            recivedList[0] = new ReceivedData(deviceAddress, isFault, ReceivedType.PowerOff);
                            break;
                        case 0x00://板卡信息
                            recivedList[0] = new ReceivedData(deviceAddress, isFault, ReceivedType.Info) { Voltage = bytes[4] };
                            break;
                        default: break;
                    }
                    return recivedList;
                }
                else
                {
                    recivedList[0] = new ReceivedData(deviceAddress);
                    //无效数据
                    return recivedList;
                }
            }
            else
            {
                recivedList[0] = new ReceivedData(0, ReceivedType.Timeout);
                //超时数据
                return recivedList;
            }
        }

        public void SetReadData(byte deviceAddress, int index, bool isEffective, byte[] data, ReceivedType dataType = ReceivedType.Read)
        {
            DeviceAddress = deviceAddress;
            SensorIndex = index;
            IsEffective = isEffective;
            Data = data;
            DataType = dataType;
        }

        public void SetWriteData(byte deviceAddress, int index, bool isEffective, ReceivedType dataType = ReceivedType.Write)
        {
            DeviceAddress = deviceAddress;
            SensorIndex = index;
            IsEffective = isEffective;
            DataType = dataType;
        }

        public string Show()
        {
            if (DataType == ReceivedType.Read || DataType == ReceivedType.ReadTemp)
                return $"设备:{DeviceAddress}  消息类型:{DataType}  传感器地址：{SensorIndex}  是否有效：{IsEffective}{Environment.NewLine}" +
                    $"数据：{DataConverter.BytesToHexString(Data)}";
            else if (DataType == ReceivedType.Timeout)
                return $"超时";
            else if (DataType == ReceivedType.Info)
                return $"设备:{DeviceAddress} 当前电压:{Voltage}";
            else if (DataType == ReceivedType.ReadTemp)
                return $"设备:{DeviceAddress} 读取四路温度";
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

        public decimal GetTemp()
        {
            if (DataType.Equals(ReceivedType.ReadTemp))
            {
                short v = BitConverter.ToInt16(Data);
                return v * 0.0078125M;
            }
            return -1M;
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
