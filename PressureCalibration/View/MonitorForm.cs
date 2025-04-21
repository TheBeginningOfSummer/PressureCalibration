using CSharpKit.FileManagement;
using Module;
using ScottPlot.WinForms;
using SkiaSharp;
using System.Collections.Concurrent;

namespace PressureCalibration.View
{
    public partial class MonitorForm : Form
    {
        public Point IniPoint = new(30, 30);
        public Point Interval = new(300, 30);
        public int RowCount = 12;

        readonly ScottPlot.IYAxis[] yAxes = [];//Y轴
        readonly ConcurrentDictionary<string, AxisData> pDisPoints = [];//压力数据
        readonly ConcurrentDictionary<string, AxisData> tDisPoints = [];//温度数据
        bool isUpdate = true;
        readonly OpenFileDialog OFDFile = new();

        public MonitorForm()
        {
            InitializeComponent();

            yAxes = InitializePlot(FP图表);
            DataMonitor.UpdataData += UpdateData;

            #region 数据初始化
            //压力数据初始化
            AxisData p1AxisData = new(this, $"P1", FP图表.Plot, Color.LightSkyBlue, CKBP1, DataMonitor.DisplayedData["Time"], DataMonitor.DisplayedData[$"P1"]);
            //AxisData p2AxisData = new(this, $"P2", FP图表.Plot, Color.Black, CKBP2, DataMonitor.DisplayedData["Time"], DataMonitor.DisplayedData[$"P2"]);
            //压力加入数据集
            pDisPoints.TryAdd(p1AxisData.Name, p1AxisData);
            //pDisPoints.TryAdd(p2AxisData.Name, p2AxisData);
            //温度数据初始化
            int x = 870;
            int y = 80;
            Color color = Color.White;
            for (int i = 1; i <= Acquisition.Instance.CardAmount; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    //切换是否显示的控件
                    CheckBox checkBox = new();
                    checkBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    checkBox.Location = new Point(x + (j - 1) * 65, y);
                    checkBox.Name = $"CKBD{i}T{j}";
                    checkBox.AutoSize = false;
                    checkBox.Size = new Size(64, 20);
                    checkBox.Text = $"D{i}T{j}";

                    switch (j)
                    {
                        case 1: color = Color.Orange; break;
                        case 2: color = Color.Blue; break;
                        case 3: color = Color.DarkRed; break;
                        case 4: color = Color.YellowGreen; break;
                    };
                    //温度数据
                    AxisData tAxisData = new(this, $"D{i}T{j}", FP图表.Plot, color, checkBox, DataMonitor.DisplayedData["Time"], DataMonitor.DisplayedData[$"D{i}T{j}"]);
                    //温度数据集加入数据集
                    tDisPoints.TryAdd(tAxisData.Name!, tAxisData);
                }
                y += 20;
            }
            #endregion
        }

        #region 图表
        /// <summary>
        /// 初始化设置图表和轴
        /// </summary>
        /// <param name="formsPlot">需设置的图表</param>
        /// <returns></returns>
        public ScottPlot.IYAxis[] InitializePlot(FormsPlot formsPlot)
        {
            formsPlot.Plot.Font.Set(SKFontManager.Default.MatchCharacter('汉').FamilyName);
            formsPlot.Plot.XLabel("time");
            formsPlot.Plot.Title("P-T Curve");
            formsPlot.Plot.Axes.DateTimeTicksBottom();
            formsPlot.MouseMove += FP图表_MouseMove;
            //设置Y轴
            ScottPlot.IYAxis y1 = formsPlot.Plot.Axes.Left;
            ScottPlot.IYAxis y2 = formsPlot.Plot.Axes.Right;
            y1.Label.Text = "P(Pa)";
            y1.Label.ForeColor = ScottPlot.Color.FromColor(Color.LightSkyBlue);
            y2.Label.Text = "T(℃)";
            y2.Label.ForeColor = ScottPlot.Color.FromColor(Color.Orange);
            return [y1, y2];
        }
        /// <summary>
        /// 显示每个轴数据点的数据
        /// </summary>
        /// <param name="plot">数据表</param>
        /// <param name="yAxis">显示轴</param>
        /// <param name="mouseLocation">鼠标坐标</param>
        /// <param name="label">显示的标签</param>
        /// <param name="timeLine">时间轴</param>
        /// <param name="data">数据轴</param>
        public void ShowPointData(Point mouseLocation, int minDistance)
        {
            //计算最小距离
            foreach (var item in pDisPoints)
                item.Value.GetMinDistance(yAxes[0], mouseLocation);
            //显示符合条件的坐标
            int i = 0;
            foreach (var item in pDisPoints)
                item.Value.ShowData(mouseLocation, minDistance, ref i);

            //计算最小距离
            foreach (var item in tDisPoints)
                item.Value.GetMinDistance(yAxes[1], mouseLocation);
            //显示符合条件的坐标
            int j = 0;
            foreach (var item in tDisPoints)
                item.Value.ShowData(mouseLocation, minDistance, ref j);
        }
        /// <summary>
        /// 图表数据刷新
        /// </summary>
        private void UpdateData(Dictionary<string, double> data)
        {
            if (!isUpdate) return;
            //更新数据轴上的数据
            foreach (var item in pDisPoints)
                item.Value.UpdateData(yAxes[0]);
            foreach (var item in tDisPoints)
                item.Value.UpdateData(yAxes[1]);
            //刷新图表
            FP图表.Plot.Axes.AutoScale();
            Invoke(new Action(FP图表.Refresh));
        }
        /// <summary>
        /// 鼠标图表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FP图表_MouseMove(object? sender, MouseEventArgs e)
        {
            if (DataMonitor.DisplayedData["Time"].Count <= 0) return;
            ShowPointData(e.Location, 10);
        }
        #endregion

        #region 按钮
        private void TMI清除_Click(object sender, EventArgs e)
        {
            DataMonitor.ClearDisplayedData();
            FP图表.Plot.Clear();
            FP图表.Refresh();
        }

        private void TMI刷新_Click(object sender, EventArgs e)
        {
            if (isUpdate)
            {
                UpdateData([]);
            }
            else
            {
                isUpdate = true;
                UpdateData([]);
                isUpdate = false;
            }
        }

        private void TMI停止_Click(object sender, EventArgs e)
        {
            var button = (ToolStripMenuItem)sender;
            isUpdate = !isUpdate;
            if (isUpdate)
                button.Text = "停止";
            else
                button.Text = "继续";
        }
        #endregion

        private void TMI打开_Click(object sender, EventArgs e)
        {
            if (OFDFile.ShowDialog() == DialogResult.OK)
            {
                string path = OFDFile.FileName.Replace($"\\{OFDFile.FileName.Split('\\').Last()}", "");
                string name = OFDFile.FileName.Split('\\').Last();
                var data = JsonManager.Load<ConcurrentDictionary<string, List<double>>>(path, name);
                if (data != null)
                {
                    DataMonitor.ClearDisplayedData();
                    foreach (var item in data)
                        DataMonitor.DisplayedData.AddOrUpdate(item.Key, item.Value, (k, v) => { v.AddRange(item.Value); return v; });
                }
                UpdateData([]);
            }
        }

        private void TMI保存_Click(object sender, EventArgs e)
        {
            if (JsonManager.Save("Data", $"{DateTime.Now:yyyyMMdd_HHmmss}.json", DataMonitor.DisplayedData))
                MessageBox.Show("保存完成", "提示");
        }
    }
}
