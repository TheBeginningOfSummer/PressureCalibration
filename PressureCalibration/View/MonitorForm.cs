using CSharpKit.DataManagement;
using ScottPlot.WinForms;
using Services;
using SkiaSharp;
using System.ComponentModel;

namespace PressureCalibration.View
{
    public partial class MonitorForm : Form
    {
        public Point IniPoint = new(30, 30);
        public Point Interval = new(300, 30);
        public int RowCount = 12;

        readonly Dictionary<string, Label> labelDic = [];
        List<double> dateTime = [];
        List<double> pressure = [];
        List<double> temperature = [];
        Random random = new();

        readonly ScottPlot.IYAxis[] yAxes = [];
        readonly List<Label> labelList = [];

        public MonitorForm()
        {
            InitializeComponent();

            yAxes = InitializePlot(FP图表);
            labelList.Add(LB压力数据1);
            labelList.Add(LB温度数据);
            T1.Tick += T1_Tick;
            BGW监测.WorkerSupportsCancellation = true;
            BGW监测.DoWork += BGW监测_DoWork;
        }

        #region 图表
        //初始化设置图表和轴
        public ScottPlot.IYAxis[] InitializePlot(FormsPlot formsPlot)
        {
            formsPlot.Plot.Font.Set(SKFontManager.Default.MatchCharacter('汉').FamilyName);
            formsPlot.Plot.XLabel("time");
            formsPlot.Plot.Title("P-T Curve");
            formsPlot.Plot.Axes.DateTimeTicksBottom();
            formsPlot.MouseMove += FP图表_MouseMove;

            ScottPlot.IYAxis y1 = formsPlot.Plot.Axes.Left;
            ScottPlot.IYAxis y2 = formsPlot.Plot.Axes.Right;
            y1.Label.Text = "P(Pa)";
            y1.Label.ForeColor = ScottPlot.Color.FromColor(Color.LightSkyBlue);
            y2.Label.Text = "T(℃)";
            y2.Label.ForeColor = ScottPlot.Color.FromColor(Color.Orange);
            return [y1, y2];
        }
        //显示每个轴的数据
        public static void ShowPointData(ScottPlot.Plot plot, ScottPlot.IYAxis[] yAxes, Point mouseLocation, List<Label> labels, List<double> timeLine, params List<double>[] data)
        {
            int[] index = new int[data.Length];
            double[] distance = Enumerable.Repeat(999.9, data.Length).ToArray();
            for (int i = 0; i < timeLine.Count; i++)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    var valuePoint = plot.GetPixel(new ScottPlot.Coordinates(timeLine[i], data[j][i]), plot.Axes.Bottom, yAxes[j]);
                    double temp = Math.Pow(valuePoint.X - mouseLocation.X, 2) + Math.Pow(valuePoint.Y - mouseLocation.Y, 2);
                    if (temp <= distance[j])
                    {
                        distance[j] = Math.Sqrt(temp);
                        index[j] = i;
                    }
                }
            }
            for (int j = 0; j < data.Length; j++)
            {
                if (distance[j] < 10)
                {
                    labels[j].Text = $"[{index[j]}] [{DateTime.FromOADate(timeLine[index[j]]):HH:mm:ss}] {data[j][index[j]]:N3}{labels[j].Tag}";
                    labels[j].Location = new Point(mouseLocation.X + 30, mouseLocation.Y + labels[j].Height * j);
                    labels[j].Visible = true;
                }
            }
        }
        //图表刷新
        private void T1_Tick(object? sender, EventArgs e)
        {
            FP图表.Plot.Add.Scatter(dateTime, pressure, yAxes[0].Label.ForeColor).Axes.YAxis = yAxes[0];
            FP图表.Plot.Add.Scatter(dateTime, temperature, yAxes[1].Label.ForeColor).Axes.YAxis = yAxes[1];
            FP图表.Plot.Axes.AutoScale();
            Invoke(new Action(FP图表.Refresh));
        }
        //数据采集
        private void BGW监测_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (sender is BackgroundWorker worker)
                while (!worker.CancellationPending)
                {
                    lock (dateTime)
                        dateTime.Add(DateTime.Now.ToOADate());
                    lock (pressure)
                        pressure.Add(random.NextDouble() * 100000);
                    lock (temperature)
                        temperature.Add(random.NextDouble() * 100);
                    Thread.Sleep(1000);
                }
        }
        //数据点查看
        private void FP图表_MouseMove(object? sender, MouseEventArgs e)
        {
            if (BGW监测.IsBusy) return;
            foreach (var item in labelList) item.Visible = false;
            ShowPointData(FP图表.Plot, yAxes, e.Location, labelList, dateTime, pressure, temperature);
        }
        #endregion

        public void InitialzeTempPicture(Control control, List<decimal[]> tempList)
        {
            control.Controls.Clear();
            //控件属性
            int offset = 56;
            Size size = new(55, 55);
            //变量点
            int x = IniPoint.X;
            int y = IniPoint.Y;
            bool isSwitch = false;
            for (int i = 0; i < tempList.Count; i++)
            {
                for (int j = 0; j < tempList[i].Length; j++)
                {
                    Color color = Color.LightSkyBlue;
                    //if (tempList[i][j] > decimal.Parse(TTB目标温度.Text) + decimal.Parse(TSTB偏差值.Text))
                    //    color = Color.Red;
                    //else if (tempList[i][j] < decimal.Parse(TTB目标温度.Text) - decimal.Parse(TSTB偏差值.Text))
                    //    color = Color.LightSkyBlue;
                    //else
                    //    color = Color.Orange;

                    int m = Convert.ToInt32(BytesTool.GetBit((byte)j, 0));
                    int n = Convert.ToInt32(BytesTool.GetBit((byte)j, 1));
                    var tLabel = FormKit.ControlFactory<Label>(new Point(x + offset * n, y - offset * m), $"[LB{i}{j}]温度分布", $"[{j + 1}]{Environment.NewLine}{tempList[i][j]:N2}", size, color, Color.White);
                    FormKit.AddControl(control, tLabel);
                    labelDic.Add($"{i}{j}", tLabel);
                }
                //Socket位置
                var socketLabel = FormKit.ControlFactory<Label>(new Point(x, y - 75), $"[LB{i}]Socket座", $"[{i + 1}]");
                FormKit.AddControl(control, socketLabel);
                if ((i + 1) % 8 == 0)
                {
                    isSwitch = !isSwitch;
                    IniPoint.Y = 2 * Interval.Y + y;
                    x += Interval.X;
                }
                FormKit.GetRowPosition2(ref x, ref y, IniPoint.Y, Interval.X, Interval.Y, i, RowCount, isSwitch);
            }
        }

        private void BTN测试_Click(object sender, EventArgs e)
        {
            if (BGW监测.IsBusy)
            {
                FormKit.ShowInfoBox("运行中");
                return;
            }
            T1.Start();
            BGW监测.RunWorkerAsync();
        }

        private void BTN停止_Click(object sender, EventArgs e)
        {
            BGW监测.CancelAsync();
            T1.Stop();
        }

        private void BTN清除_Click(object sender, EventArgs e)
        {
            BGW监测.CancelAsync();
            T1.Stop();
            dateTime.Clear();
            pressure.Clear();
            temperature.Clear();
            FP图表.Plot.Clear();
            FP图表.Refresh();
        }
    }
}
