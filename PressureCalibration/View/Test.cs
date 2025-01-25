using Calibration.Data;
using CSharpKit.DataManagement;
using Module;
using System.ComponentModel;
using WinformKit;

namespace PressureCalibration.View
{
    public partial class Test : Form, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Point IniPoint = new(30, 212);
        public Point Interval = new(120, 132);
        public int RowCount = 2;

        #region 控件绑定值
        private decimal deviceCount = 16;
        public decimal DeviceCount
        {
            get { return deviceCount; }
            set
            {
                if (value < 0) deviceCount = 0;
                else deviceCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceCount)));
            }
        }


        private decimal temperature;
        public decimal Temperature
        {
            get => temperature;
            set
            {
                temperature = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Temperature)));
            }
        }

        private decimal pressure = 30000;//压力设置
        public decimal Pressure
        {
            get { return pressure; }
            set
            {
                pressure = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pressure)));
            }
        }

        private string tempTestName = "";
        public string TempTestName
        {
            get { return tempTestName; }
            set
            {
                tempTestName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TempTestName)));
            }
        }

        private string pressTestName = "";
        public string PressTestName
        {
            get { return pressTestName; }
            set
            {
                pressTestName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pressTestName)));
            }
        }
        #endregion

        readonly Dictionary<string, Label> labelsDic = [];

        public Test()
        {
            InitializeComponent();
            InitialzeTempPicture();
            Bindings();
            DataMonitor.UpdataData += UpdateTempPicture;
        }

        private void Bindings()
        {
            //BGW温度测试.DoWork += BGW温度测试_DoWork;
            //BGW温度测试.RunWorkerCompleted += BGW温度测试_RunWorkerCompleted;

            //BGW压力测试.DoWork += BGW压力测试_DoWork;
            //BGW压力测试.RunWorkerCompleted += BGW压力测试_RunWorkerCompleted;

            TTB目标温度.DataBindings.Add(new Binding("Text", this, nameof(Temperature)));
            TTB目标压力.DataBindings.Add(new Binding("Text", this, nameof(Pressure)));
            TTB温度测试名称.DataBindings.Add(new Binding("Text", this, nameof(TempTestName)));
            TTB压力测试名称.DataBindings.Add(new Binding("Text", this, nameof(PressTestName)));
        }

        public void InitialzeTempPicture(int cardCount = 16)
        {
            PN温度分布.Controls.Clear();
            //控件属性
            int offset = 56;
            Size size = new(55, 55);
            //变量点
            int x = IniPoint.X;
            int y = IniPoint.Y;
            bool isSwitch = false;
            for (int i = 0; i < cardCount; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Color color = Color.LightGray;

                    int m = Convert.ToInt32(BytesTool.GetBit((byte)j, 0));
                    int n = Convert.ToInt32(BytesTool.GetBit((byte)j, 1));
                    var tLabel = FormKit.ControlFactory<Label>(new Point(x + offset * n, y - offset * m), $"[LB{i}{j}]温度分布", $"[{j + 1}]{Environment.NewLine}0", size, color, Color.White);
                    FormKit.AddControl(PN温度分布, tLabel);
                    labelsDic.Add($"D{i + 1}T{j + 1}", tLabel);
                }
                //Socket位置
                var socketLabel = FormKit.ControlFactory<Label>(new Point(x, y - 75), $"[LB{i}]Socket座", $"[{i + 1}]");
                FormKit.AddControl(PN温度分布, socketLabel);
                if ((i + 1) % 8 == 0)
                {
                    isSwitch = !isSwitch;
                    IniPoint.Y = 2 * Interval.Y + y;
                    x += Interval.X;
                }
                FormKit.GetRowPosition2(ref x, ref y, IniPoint.Y, Interval.X, Interval.Y, i, RowCount, isSwitch);
            }
        }

        public void UpdateTempPicture(Dictionary<string, double> data)
        {
            //FormKit.UpdateMessage(GPB温度分布, $"温度分布（℃）{testData.Date}", false, true);
            foreach (var item in data)
            {
                FormKit.OnThread(this, () =>
                {
                    Color color;
                    if (item.Value > double.Parse(TTB目标温度.Text) + double.Parse(TTB偏差温度.Text))
                        color = Color.Red;
                    else if (item.Value < double.Parse(TTB目标温度.Text) - double.Parse(TTB偏差温度.Text))
                        color = Color.LightSkyBlue;
                    else
                        color = Color.Orange;

                    labelsDic[item.Key].Text = $"[{item.Key}]{Environment.NewLine}{item.Value:N2}";
                    labelsDic[item.Key].BackColor = color;
                });
            }
        }

    }
}
