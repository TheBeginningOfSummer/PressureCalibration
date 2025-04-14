using CSharpKit.Communication;
using Module;
using System.ComponentModel;
using WinformKit;

namespace PressureCalibration.View
{
    public partial class ChipTest : Form
    {
        readonly BackgroundWorker BGW采集卡 = new();
        //选中的采集组
        Group group;
        //选中的传感器
        Sensor sensor;

        #region 绑定属性
        private int deviceAddress = 1;
        public int DeviceAddress
        {
            get { return deviceAddress; }
            set
            {
                if (value > 0)
                {
                    deviceAddress = value;
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceAddress)));

                    group = Acquisition.Instance.GroupDic[DeviceAddress];
                    if (SensorIndex >= 0)
                        sensor = group.GetSensor(SensorIndex);
                }
            }
        }

        private int sensorIndex = -1;
        public int SensorIndex
        {
            get { return sensorIndex; }
            set
            {
                sensorIndex = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SensorIndex)));
                if (group != null)
                {
                    if (SensorIndex >= 0)
                        sensor = group.GetSensor(SensorIndex);
                }
            }
        }

        private byte registerAddress = 52;
        public byte RegisterAddress
        {
            get { return registerAddress; }
            set
            {
                registerAddress = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegisterAddress)));
            }
        }

        private byte registerCount = 28;
        public byte RegisterCount
        {
            get { return registerCount; }
            set
            {
                registerCount = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegisterCount)));
            }
        }
        #endregion

        public ChipTest()
        {
            InitializeComponent();

            Bindings();
            group = Acquisition.Instance.GroupDic[DeviceAddress];
            sensor = group.GetSensor(0);
        }

        public void Bindings()
        {
            BGW采集卡.DoWork += BGW采集卡_DoWork;
            BGW采集卡.RunWorkerCompleted += BGW采集卡_RunWorkerCompleted;

            CB设备地址.Items.Clear();
            for (int i = 1; i <= Acquisition.Instance.CardAmount; i++)
            {
                CB设备地址.Items.Add(i.ToString());
            }
            CB设备地址.DataBindings.Add(new Binding("Text", this, nameof(DeviceAddress)));
            CB传感器地址.DataBindings.Add(new Binding("Text", this, nameof(SensorIndex)));
            CB寄存器地址.DataBindings.Add(new Binding("Text", this, nameof(RegisterAddress)));
            TB寄存器个数.DataBindings.Add(new Binding("Text", this, nameof(RegisterCount)));
        }

        private void BGW采集卡_DoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                switch (e.Argument)
                {
                    case "read":
                        if (SensorIndex < 0)
                        {
                            e.Result = ReceivedData.ParseData(group.ReadAll(RegisterAddress, RegisterCount));
                        }
                        else
                        {
                            SendData sendData = sensor.Read(RegisterAddress, RegisterCount);
                            e.Result = ReceivedData.ParseData(group.Connection.WriteRead(sendData));
                        }
                        break;
                    case "readT":
                        decimal[] resultT = group.ReadTemperature();
                        for (int i = 0; i < resultT.Length; i++)
                            e.Result += $"{Environment.NewLine}第{i}路温度：{resultT[i]}";
                        break;
                    case "readUID":
                        int[] resultUID = group.GetSensorsUID();
                        for (int i = 0; i < resultUID.Length; i++)
                            e.Result += $"{Environment.NewLine}第{i}路芯片UID：{resultUID[i]}";
                        break;
                    case "readOutput":
                        group.GetSensorsOutput(out decimal[] tempArray, out decimal[] pressArray);
                        for (int i = 0; i < group.SensorCount; i++)
                            e.Result += $"{Environment.NewLine}芯片{i.ToString().PadLeft(2, '0')}  温度值：{tempArray[i]:F2}℃  压力值：{pressArray[i]:N6}MPa";
                        break;
                    case "readData":
                        e.Result = group.Show();
                        break;

                    case "off":
                        e.Result = ReceivedData.ParseData(group.PowerOff());
                        break;
                    case "1.8":
                        e.Result = ReceivedData.ParseData(group.PowerOn());
                        break;
                    case "3.3":
                        e.Result = ReceivedData.ParseData(group.PowerOn(3.3f));
                        break;
                    case "4.1":
                        e.Result = ReceivedData.ParseData(group.PowerOn(4.1f));
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                FormKit.UpdateMessage(RTB信息, ex.Message);
            }
        }

        private void BGW采集卡_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is string s)
            {
                FormKit.UpdateMessage(RTB信息, s);
            }
            else if (e.Result is ReceivedData[] receivedData)
            {
                for (int i = 0; i < receivedData.Length; i++)
                {
                    if (receivedData[i] == null) continue;
                    FormKit.UpdateMessage(RTB信息, $"[{i:D2}]  {receivedData[i]?.Show()}");
                }
            }
        }

        private void BTN采集卡_Click(object sender, EventArgs e)
        {
            if (BGW采集卡.IsBusy)
            {
                FormKit.UpdateMessage(RTB信息, "运行中……");
                return;
            }
            if (sender is Button button)
                BGW采集卡.RunWorkerAsync(button.Tag);
        }

        private void TMI清除_Click(object sender, EventArgs e)
        {
            RTB信息.Clear();
        }
    }
}
