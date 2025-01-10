using Module;
using ScottPlot.WinForms;
using SkiaSharp;

namespace PressureCalibration.View
{
    public partial class MonitorForm : Form
    {
        public Point IniPoint = new(30, 30);
        public Point Interval = new(300, 30);
        public int RowCount = 12;

        readonly ScottPlot.IYAxis[] yAxes = [];
        readonly List<Label> labelList = [];
        readonly Config config = Config.Instance;

        public MonitorForm()
        {
            InitializeComponent();

            yAxes = InitializePlot(FP图表);
            labelList.Add(LB压力数据);
            labelList.Add(LB温度数据);
            T1.Tick += T1_Tick;
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
                    if (data[j].Count <= i) continue;//不显示无数据
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
            FP图表.Plot.Add.Scatter(config.ACQ.DisplayedData["Time"], config.ACQ.DisplayedData["P1"], yAxes[0].Label.ForeColor).Axes.YAxis = yAxes[0];
            FP图表.Plot.Add.Scatter(config.ACQ.DisplayedData["Time"], config.ACQ.DisplayedData["D1T1"], yAxes[1].Label.ForeColor).Axes.YAxis = yAxes[1];
            FP图表.Plot.Axes.AutoScale();
            Invoke(new Action(FP图表.Refresh));
        }
        //数据点查看
        private void FP图表_MouseMove(object? sender, MouseEventArgs e)
        {
            if (config.ACQ.DisplayedData["Time"].Count <= 0) return;
            foreach (var item in labelList) item.Visible = false;
            ShowPointData(FP图表.Plot, yAxes, e.Location, labelList, config.ACQ.DisplayedData["Time"],
                config.ACQ.DisplayedData["P1"], config.ACQ.DisplayedData["D1T1"]);
        }
        #endregion

        #region 按钮
        private void TMI开始_Click(object sender, EventArgs e)
        {
            T1.Start();
        }

        private void TMI停止_Click(object sender, EventArgs e)
        {
            T1.Stop();
        }

        private void TMI清除_Click(object sender, EventArgs e)
        {
            T1.Stop();
            config.ACQ.ClearDisplayedData();
            FP图表.Plot.Clear();
            FP图表.Refresh();
        }
        #endregion
    }
}
