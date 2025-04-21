using CSharpKit.DataManagement;
using Data;
using Module;
using System.Collections.Concurrent;
using System.ComponentModel;
using WinformKit;

namespace PressureCalibration.View
{
    public partial class Test : Form, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Point IniPoint = new(30, 30);
        public Point Interval = new(200, 132);
        public Size LBSize = new(55, 55);
        public int Offset = 56;
        public int RowCount = 2;

        readonly Config config = Config.Instance;

        #region 需存导出的数据
        //获取到的tmp117的温度数据
        readonly List<TempTest> temperatureList = [];
        //压力测试数据
        readonly List<PressureTest> pressureList = [];
        //传感器数据
        readonly List<SensorTest> sensorTestList = [];
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

        private double targetT = 15;
        public double TargetT
        {
            get => targetT;
            set
            {
                targetT = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TargetT)));
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
            DeviceCount = Acquisition.Instance.CardAmount;
            switch (Acquisition.Instance.SensorType)
            {
                case "BOE2520":
                    BOE2520TPicture(DeviceCount);
                    break;
                case "7570":
                    ZXW7570TPicture(Acquisition.Instance.GroupDic);
                    break;
                case "6862":
                    ZXC6862TPicture(Acquisition.Instance.GroupDic);
                    break;
                case "6862T":
                    ZXC6862TPicture2(Acquisition.Instance.GroupDic);
                    break;
                default:break;
            }
            Bindings();
            //DataMonitor.UpdataData += UpdateTempPicture;
        }

        private void Bindings()
        {
            BGW温度.DoWork += BGW温度测试_DoWork;
            BGW温度.RunWorkerCompleted += BGW温度测试_RunWorkerCompleted;
            BGW压力.DoWork += BGW压力测试_DoWork;
            BGW压力.RunWorkerCompleted += BGW压力测试_RunWorkerCompleted;
            BGW运动.DoWork += BGW运动_DoWork;
            BGW运动.RunWorkerCompleted += BGW运动_RunWorkerCompleted;
            BGW温度.WorkerSupportsCancellation = true;
            BGW压力.WorkerSupportsCancellation = true;
            BGW运动.WorkerSupportsCancellation = true;
            //温度
            TTB设置温度.DataBindings.Add(new Binding("Text", this, nameof(Temperature), false, DataSourceUpdateMode.OnPropertyChanged));
            TTB目标温度.DataBindings.Add(new Binding("Text", this, nameof(TargetT), false, DataSourceUpdateMode.OnPropertyChanged));
            TTB温度采集间隔.DataBindings.Add(new Binding("Text", this, nameof(TInterval), false, DataSourceUpdateMode.OnPropertyChanged));
            TTB温度测试名称.DataBindings.Add(new Binding("Text", this, nameof(TempTestName), false, DataSourceUpdateMode.OnPropertyChanged));
            //压力
            TTB目标压力.DataBindings.Add(new Binding("Text", this, nameof(Pressure), false, DataSourceUpdateMode.OnPropertyChanged));
            TTB压力采集间隔.DataBindings.Add(new Binding("Text", this, nameof(PInterval), false, DataSourceUpdateMode.OnPropertyChanged));
            TTB压力测试名称.DataBindings.Add(new Binding("Text", this, nameof(PressTestName), false, DataSourceUpdateMode.OnPropertyChanged));

            CMB轴列表.DataSource = config.Zmotion.AxesName;
        }

        private void Test_FormClosing(object sender, FormClosingEventArgs e)
        {
            PN温度分布.Controls.Clear();//清除控件，防止信息标签被销毁
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
                        TempTest tempTest = Acquisition.Instance.
                            GetTestData(out SensorTest sensorTest, (decimal)TargetT, decimal.Parse(TTB偏差温度.Text));
                        temperatureList.Add(tempTest);//添加数据到收集的数据
                        sensorTestList.Add(sensorTest);
                        Thread.Sleep(1000 * TInterval);
                        if (e.Argument is "a1")
                        {
                            e.Result = "采集完成";
                            break;
                        }
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            e.Result = "采集中止";
                            break;
                        }
                    }
                    while (true);
                }
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
            }
        }

        private void BGW温度测试_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                FormKit.UpdateMessage(RTB温度信息, "运行已中止");
            else if (e.Result is string message)
                FormKit.UpdateMessage(RTB温度信息, message);
        }

        public void BOE2520TPicture(int cardCount = 8, int tempCountPre = 4)
        {
            PN温度分布.Controls.Clear();
            //控件属性
            int offset = 56;
            //变量点
            int x = IniPoint.X;
            int y = IniPoint.Y;
            bool isSwitch = false;
            for (int i = 0; i < cardCount; i++)
            {
                for (int j = 0; j < tempCountPre; j++)
                {
                    Color color = Color.LightGray;

                    int m = Convert.ToInt32(BytesTool.GetBit((byte)j, 0));
                    int n = Convert.ToInt32(BytesTool.GetBit((byte)j, 1));
                    var tLabel = FormKit.ControlFactory<Label>(new Point(x + offset * n, y - offset * m), $"[LB{i}{j}]温度分布", $"[{j + 1}]{Environment.NewLine}0", LBSize, color, Color.White);
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

        public void ZXW7570TPicture(ConcurrentDictionary<int, Group> groupCollection)
        {
            PN温度分布.Controls.Clear();
            //九宫格左上角初始位置
            int x = IniPoint.X;
            int y = IniPoint.Y;

            foreach (GroupZXW7570 group in groupCollection.Values.Cast<GroupZXW7570>())
            {
                //计算前9个传感器位置
                List<Point> points1 = FormKit.GetLBPos9(x, y, Offset, Offset, direction: false);
                //设置前8个传感器的信息显示位置
                for (int i = 0; i < points1.Count; i++)
                {
                    if (i < 4)
                        group.Sensors[i].SetLabelLoc(points1[i]);
                    if (i == 4)
                        group.SetLabelLoc(points1[i], 0);//TMP117[0]位置设置
                    if (i > 4)
                    {
                        group.Sensors[i - 1].SetLabelLoc(points1[i]);
                    }
                }
                x += Interval.X;
                //计算后9个传感器位置
                List<Point> points2 = FormKit.GetLBPos9(x, y, Offset, Offset, direction: false);
                //设置后8个传感器的信息显示位置
                for (int i = 0; i < points2.Count; i++)
                {
                    if (i < 4)
                        group.Sensors[i + 8].SetLabelLoc(points2[i]);
                    if (i == 4)
                        group.SetLabelLoc(points2[i], 1);//TMP117[1]位置设置
                    if (i > 4)
                    {
                        group.Sensors[i + 7].SetLabelLoc(points2[i]);
                    }
                }
                x += Interval.X;
                //将标签添加到界面上
                FormKit.AddControl(PN温度分布, group.TInfo[0]);
                FormKit.AddControl(PN温度分布, group.TInfo[1]);
                foreach (var sensor in group.Sensors.Values)
                    FormKit.AddControl(PN温度分布, sensor.SensorInfo);
            }
        }

        public void Ini6862Pos(GroupZXC6862 group, ref int x, ref int y, int indexOffset = 0, int indexTemp = 0)
        {
            //计算前9个传感器位置
            List<Point> points1 = FormKit.GetLBPos2_4(x, y, Offset, Offset, direction: false);
            //设置前8个传感器的信息显示位置
            for (int i = 0; i < points1.Count; i++)
            {
                if (i < 2)
                    group.Sensors[i + indexOffset].SetLabelLoc(points1[i]);
                else if (i == 2)
                    group.SetLabelLoc(points1[i], indexTemp);//TMP117[0]位置设置
                else if (i > 2 && i < 5)
                    group.Sensors[i - 1 + indexOffset].SetLabelLoc(points1[i]);
                else if (i == 5)
                    group.SetLabelLoc(points1[i], indexTemp + 1);//TMP117[1]位置设置
                else if (i < 8)
                    group.Sensors[i - 2 + indexOffset].SetLabelLoc(points1[i]);
            }
        }

        public void ZXC6862TPicture(ConcurrentDictionary<int, Group> groupCollection)
        {
            PN温度分布.Controls.Clear();
            //九宫格左上角初始位置
            int x = IniPoint.X;
            int y = IniPoint.Y;
            Interval.X = 250;

            foreach (GroupZXC6862 group in groupCollection.Values.Cast<GroupZXC6862>())
            {
                Ini6862Pos(group, ref x, ref y);
                x += Interval.X;
                Ini6862Pos(group, ref x, ref y, 8, 2);
                x += Interval.X;
                //将标签添加到界面上
                //FormKit.AddControl(PN温度分布, group.TInfo[0]);
                //FormKit.AddControl(PN温度分布, group.TInfo[1]);
                foreach (var tLabel in group.TInfo)
                    FormKit.AddControl(PN温度分布, tLabel);
                for (int i = 0; i < 6; i++)
                {
                    FormKit.AddControl(PN温度分布, group.Sensors[i].SensorInfo);
                }
                for (int i = 8; i < 14; i++)
                {
                    FormKit.AddControl(PN温度分布, group.Sensors[i].SensorInfo);
                }
            }
        }

        public void ZXC6862TPicture2(ConcurrentDictionary<int, Group> groupCollection)
        {
            PN温度分布.Controls.Clear();
            //九宫格左上角初始位置
            int x = IniPoint.X;
            int y = IniPoint.Y;
            List<Point> points1 = FormKit.GetLBPos2_4(x, y, Offset, Offset, direction: false);
            Interval.X = 250;
            if (groupCollection.Count >= 2)
            {
                groupCollection[1].SetLabelLoc(points1[2], 0);
                groupCollection[1].SetLabelLoc(points1[5], 1);
                groupCollection[1].SetLabelLoc(points1[3], 2);
                groupCollection[1].SetLabelLoc(points1[4], 3);

                groupCollection[2].SetLabelLoc(points1[0], 0);
                groupCollection[2].SetLabelLoc(points1[1], 1);
                groupCollection[2].SetLabelLoc(points1[6], 2);
                groupCollection[2].SetLabelLoc(points1[7], 3);
            }
            foreach (var tLabel in groupCollection[1].TInfo)
                FormKit.AddControl(PN温度分布, tLabel);
            foreach (var tLabel in groupCollection[2].TInfo)
                FormKit.AddControl(PN温度分布, tLabel);
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
                        Acquisition.Instance.Open();
                        break;
                    case "closeACQ":
                        Acquisition.Instance.Close();
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
                        sensorTestList.Clear();
                        FormKit.ShowInfoBox("数据清除完成。");
                        break;
                    case "excel":
                        if (ExcelOutput.Output(temperatureList, name: $"{TempTestName}T"))
                            FormKit.UpdateMessage(RTB温度信息, "导出温度数据完成。");
                        if (ExcelOutput.Output(sensorTestList, name: $"{TempTestName}S"))
                            FormKit.UpdateMessage(RTB温度信息, "导出传感器数据完成。");
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
                            Acquisition.Instance.ReadyPosition();
                            break;
                        case "2":
                            Acquisition.Instance.WorkingTorque();
                            break;
                        case "3":
                            Acquisition.Instance.UnloadTorque();
                            break;
                        case "4":
                            Acquisition.Instance.UploadPosition();
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
