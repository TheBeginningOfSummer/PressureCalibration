using Calibration.Data;
using CompreDemo.Forms;
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

        readonly Config config = Config.Instance;

        #region 需存导出的数据
        //获取到的tmp117的温度数据
        readonly List<TempTest> temperatureList = [];
        //压力测试数据
        readonly List<PressureTest> pressureList = [];
        #endregion

        #region 控件绑定值
        private int deviceCount = 16;
        public int DeviceCount
        {
            get { return deviceCount; }
            set
            {
                if (value < 0) deviceCount = 0;
                else deviceCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceCount)));
            }
        }

        //温度
        private double temperature = 15;
        public double Temperature
        {
            get => temperature;
            set
            {
                temperature = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Temperature)));
            }
        }

        private int tInterval = 2;
        public int TInterval
        {
            get { return tInterval; }
            set { tInterval = value; }
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

        //压力
        private double pressure = 30000;//压力设置
        public double Pressure
        {
            get { return pressure; }
            set
            {
                pressure = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pressure)));
            }
        }

        private int pInterval = 2;
        public int PInterval
        {
            get { return pInterval; }
            set { pInterval = value; }
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

        readonly BackgroundWorker BGW温度 = new();
        readonly BackgroundWorker BGW压力 = new();
        readonly BackgroundWorker BGW运动 = new();

        public Test()
        {
            InitializeComponent();
            DeviceCount = config.ACQ.CardAmount;
            InitialzeTempPicture(DeviceCount);
            Bindings();
            BGW温度.WorkerSupportsCancellation = true;
            BGW压力.WorkerSupportsCancellation = true;
            BGW运动.WorkerSupportsCancellation = true;
            DataMonitor.UpdataData += UpdateTempPicture;

        }

        private void Bindings()
        {
            BGW温度.DoWork += BGW温度测试_DoWork;
            BGW温度.RunWorkerCompleted += BGW温度测试_RunWorkerCompleted;
            BGW压力.DoWork += BGW压力测试_DoWork;
            BGW压力.RunWorkerCompleted += BGW压力测试_RunWorkerCompleted;
            BGW运动.DoWork += BGW运动_DoWork;
            BGW运动.RunWorkerCompleted += BGW运动_RunWorkerCompleted;
            //温度
            TTB目标温度.DataBindings.Add(new Binding("Text", this, nameof(Temperature)));
            TTB温度采集间隔.DataBindings.Add(new Binding("Text", this, nameof(TInterval)));
            TTB温度测试名称.DataBindings.Add(new Binding("Text", this, nameof(TempTestName)));
            //压力
            TTB目标压力.DataBindings.Add(new Binding("Text", this, nameof(Pressure)));
            TTB压力采集间隔.DataBindings.Add(new Binding("Text", this, nameof(PInterval)));
            TTB压力测试名称.DataBindings.Add(new Binding("Text", this, nameof(PressTestName)));

            CMB轴列表.DataSource = config.Zmotion.AxesName;
        }

        #region 温度测试
        private void BGW温度测试_DoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                if (sender is BackgroundWorker worker)
                {
                    do
                    {
                        AcquisitionT();
                        Thread.Sleep(1000 * TInterval);
                        if (e.Argument is "a1") break;
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }
                    }
                    while (true);
                }
            }
            catch (Exception ex)
            {
                FormKit.UpdateMessage(RTB温度信息, ex.Message);
            }
        }

        private void BGW温度测试_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {

        }

        public void InitialzeTempPicture(int cardCount = 8, int tempCount = 4)
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
                for (int j = 0; j < tempCount; j++)
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
                    if (item.Value > Temperature + double.Parse(TTB偏差温度.Text))
                        color = Color.Red;
                    else if (item.Value < Temperature - double.Parse(TTB偏差温度.Text))
                        color = Color.LightSkyBlue;
                    else
                        color = Color.Orange;

                    if (labelsDic.TryGetValue(item.Key, out var label))
                    {
                        label.Text = $"[{item.Key}]{Environment.NewLine}{item.Value:N2}";
                        label.BackColor = color;
                    }
                });
            }
        }

        public void UpdateTempPicture(TempTest testData)
        {
            //FormKit.UpdateMessage(GB温度分布, $"温度分布（℃）{testData.Date}", false, true);
            for (int i = 0; i < testData.TempList.Count; i++)
            {
                for (int j = 0; j < testData.TempList[i].Length; j++)
                {
                    FormKit.OnThread(this, () =>
                    {
                        Color color;
                        if (testData.TempList[i][j] > (decimal)Temperature + decimal.Parse(TTB偏差温度.Text))
                            color = Color.Red;
                        else if (testData.TempList[i][j] < (decimal)Temperature - decimal.Parse(TTB偏差温度.Text))
                            color = Color.LightSkyBlue;
                        else
                            color = Color.Orange;

                        if (labelsDic.TryGetValue($"D{i + 1}T{j + 1}", out var label))
                        {
                            label.Text = $"[{j + 1}]{Environment.NewLine}{testData.TempList[i][j]:N2}";
                            label.BackColor = color;
                        }
                    });
                }
            }
        }

        public void AcquisitionT()
        {
            TempTest tempTest = config.ACQ.GetTemperatureList();
            UpdateTempPicture(tempTest);//显示数据
            temperatureList.Add(tempTest);//添加数据到收集的数据
        }

        private void TMI采集温度_Click(object sender, EventArgs e)
        {
            if (BGW温度.IsBusy)
            {
                MessageBox.Show("运行中……", "提示");
                return;
            }
            if (sender is ToolStripMenuItem menuItem)
            {
                FormKit.UpdateMessage(RTB温度信息, "开始采集！");
                BGW温度.RunWorkerAsync(menuItem.Tag);
            }
        }

        private void TMI温度测试_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                switch (menuItem.Tag)
                {
                    case "openACQ":
                        config.ACQ.Open();
                        break;
                    case "closeACQ":
                        config.ACQ.Close();
                        break;
                    case "openTEC":
                        config.TEC.Open();
                        break;
                    case "closeTEC":
                        config.TEC.Close();
                        break;
                    case "setT":
                        config.TEC.TECOnOff(true);
                        if (config.TEC.SetTemp((short)(Temperature * 100)))
                            FormKit.UpdateMessage(RTB温度信息, $"设置温度{Temperature}℃.");
                        break;

                    case "cancel":
                        BGW温度.CancelAsync();
                        FormKit.UpdateMessage(RTB温度信息, "停止采集。");
                        break;
                    case "clear":
                        RTB温度信息.Clear();
                        temperatureList.Clear();
                        FormKit.ShowInfoBox("数据清除完成。");
                        break;
                    case "excel":
                        if (ExcelOutput.Output(temperatureList, name: TempTestName))
                            FormKit.UpdateMessage(RTB温度信息, "导出数据完成。");
                        break;
                    case "picture":
                        Bitmap bitmap = new(this.Width, this.Height);
                        Graphics g = Graphics.FromImage(bitmap);
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.CopyFromScreen(Left, Top, 0, 0, new Size(this.Width, this.Height));
                        string path = "Data\\TempTest\\";
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                        path += $"[{DateTime.Now:yyyy-MM-dd HH-mm-ss}]{TempTestName}.png";
                        bitmap.Save(path);
                        FormKit.UpdateMessage(RTB温度信息, "保存完成");
                        break;
                    default: break;
                }
            }
        }
        #endregion

        #region 压力测试
        private void BGW压力测试_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (sender is BackgroundWorker worker)
                while (true)
                {
                    var pData = new PressureTest(DateTime.Now.ToString("HH:mm:ss"), config.PACE.GetPress());
                    FormKit.UpdateMessage(RTB压力信息, pData.Pressure.ToString());
                    pressureList.Add(pData);
                    Thread.Sleep(1000 * PInterval);
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                }
        }

        private void BGW压力测试_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void TMI采集压力_Click(object sender, EventArgs e)
        {
            if (BGW压力.IsBusy)
            {
                MessageBox.Show("运行中……", "提示");
                return;
            }
            FormKit.UpdateMessage(RTB压力信息, "开始采集！");
            BGW压力.RunWorkerAsync();
        }

        private void TMI压力测试_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                switch (menuItem.Tag)
                {
                    case "conPACE":
                        if (config.PACE.Connect())
                            FormKit.UpdateMessage(RTB压力信息, "连接压力控制器成功！");
                        else
                            FormKit.UpdateMessage(RTB压力信息, "连接压力控制器失败！");
                        break;
                    case "disPACE":
                        config.PACE.Disconnect();
                        break;
                    case "gaug":
                        var m1 = config.PACE.SelectMode();
                        FormKit.UpdateMessage(RTB压力信息, m1);
                        break;
                    case "act":
                        var m2 = config.PACE.SelectMode(1);
                        FormKit.UpdateMessage(RTB压力信息, m2);
                        break;
                    case "vent":
                        config.PACE.Vent();
                        break;
                    case "SetP":
                        config.PACE.SetPress((decimal)Pressure);
                        break;

                    case "cancel":
                        BGW压力.CancelAsync();
                        FormKit.UpdateMessage(RTB压力信息, "停止采集。");
                        break;
                    case "clear":
                        RTB压力信息.Clear();
                        pressureList.Clear();
                        FormKit.ShowInfoBox("数据清除完成。");
                        break;
                    case "excel":
                        if (ExcelOutput.Output(pressureList, "Data\\PressureTest\\", PressTestName))
                            FormKit.UpdateMessage(RTB压力信息, "导出数据完成。");
                        break;
                    default: break;
                }
            }
        }
        #endregion

        #region 运动测试
        private void BGW运动_DoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                if (sender is BackgroundWorker worker)
                {
                    switch (e.Argument)
                    {
                        case "1":
                            config.ACQ.ReadyPosition();
                            break;
                        case "2":
                            config.ACQ.WorkingTorque();
                            break;
                        case "3":
                            config.ACQ.UnloadTorque();
                            break;
                        case "4":
                            config.ACQ.UploadPosition();
                            break;
                        default: break;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void BGW运动_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void TMI连接控制卡_Click(object sender, EventArgs e)
        {
            if (config.Zmotion.Connect())
            {
                //轴参数重新初始化
                config.Zmotion.Initialize();
                FormKit.ShowInfoBox("连接成功。");
            }
        }

        private void TMI断开控制卡_Click(object sender, EventArgs e)
        {
            config.Zmotion.Disconnect();
            FormKit.ShowInfoBox("断开连接。");
        }

        private void BTN轴测试_Click(object sender, EventArgs e)
        {
            if (config.Zmotion.Axes.TryGetValue(CMB轴列表.Text, out var result))
            {
                if (result == null) return;
                new AxisTest(result).Show();
            }
        }

        private void BTN轨迹测试_Click(object sender, EventArgs e)
        {
            if (BGW运动.IsBusy)
            {
                MessageBox.Show("运行中……", "提示");
                return;
            }
            if (sender is Button button)
            {
                //FormKit.UpdateMessage(RTB运动信息, "开始采集！");
                BGW运动.RunWorkerAsync(button.Tag);
            }
        }
        #endregion


    }
}
