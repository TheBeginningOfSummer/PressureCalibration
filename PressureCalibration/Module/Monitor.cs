using Services;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Module
{
    public class DataMonitor
    {
        public static Channel<Dictionary<string, double>> Cache { get; set; } = Channel.CreateUnbounded<Dictionary<string, double>>();
        public static ConcurrentDictionary<string, List<double>> DisplayedData { get; set; } = [];

        public static Action<Dictionary<string, double>>? UpdataData;
        
        public DataMonitor()
        {
            
        }

        public static void Initialize(params string[] keys)
        {
            DisplayedData.TryAdd("Time", []);
            //初始化采集压力数据
            for (int i = 0; i < keys.Length; i++)
            {
                DisplayedData.TryAdd(keys[i], []);
            }
            Task.Run(RefreshData);
        }

        public static ConcurrentDictionary<string, double> GetDataContainer(double defaultData = double.NaN)
        {
            ConcurrentDictionary<string, double> data = [];
            string[] keys = [.. DisplayedData.Keys];
            for (int i = 0; i < keys.Length; i++)
                data.TryAdd(keys[i], defaultData);
            return data;
        }
        /// <summary>
        /// 更新字典数据
        /// </summary>
        /// <param name="data">数据</param>
        public static void UpdateData(Dictionary<string, double> data)
        {
            foreach (var item in data)
            {
                DisplayedData.AddOrUpdate(item.Key, [item.Value],
                    (oldKey, oldList) => { oldList.Add(item.Value); return oldList; });
            }
        }

        public static bool IsFilled(ConcurrentDictionary<string, double> data)
        {
            bool isFilled = true;
            foreach (var value in data.Values)
            {
                if (double.IsNaN(value)) isFilled = false;
            }
            return isFilled;
        }
        /// <summary>
        /// 从缓存中更新数据到字典
        /// </summary>
        /// <returns></returns>
        public static async Task RefreshData()
        {
            while (await Cache.Reader.WaitToReadAsync())
            {
                if (Cache.Reader.TryRead(out Dictionary<string, double>? data))
                {
                    if (data != null)
                    {
                        UpdateData(data);
                        UpdataData?.Invoke(data);
                    }
                }
            }
        }
        /// <summary>
        /// 清除字典中的数据
        /// </summary>
        public static void ClearDisplayedData()
        {
            foreach (var item in DisplayedData)
            {
                item.Value.Clear();
            }
        }

    }

    public class AxisData
    {
        public string Name { get; set; }
        public int TimeIndex { get; set; }
        public double Distance { get; set; } = 999.9;
        public ScottPlot.Plot Plot { get; set; }
        public Color ControlColor { get; set; }
        public CheckBox SelectBox { get; set; } = new();
        public Label InfoLabel { get; set; } = new();
        public List<double> Time { get; set; } = [];
        public List<double> Data { get; set; } = [];

        public AxisData(Control control, string name, ScottPlot.Plot plot, Color color, CheckBox selectBox, List<double> time, List<double> data)
        {
            Name = name;
            Plot = plot;
            ControlColor = color;
            SelectBox = selectBox;
            Time = time;
            Data = data;

            SelectBox.ForeColor = ControlColor;
            InfoLabel.ForeColor = ControlColor;
            InfoLabel.Name = $"LB{name}";
            InfoLabel.AutoSize = true;
            InfoLabel.Visible = false;

            control.Controls.Add(SelectBox);
            control.Controls.Add(InfoLabel);
            InfoLabel.BringToFront();

            SelectBox.CheckedChanged += SelectBox_CheckedChanged;
        }

        private void SelectBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (!checkBox.Checked) Plot.Clear();
            }
        }
        /// <summary>
        /// 更新数据显示
        /// </summary>
        /// <param name="yAxis">数据的纵坐标轴</param>
        public void UpdateData(ScottPlot.IYAxis yAxis)
        {
            if (SelectBox.Checked)
                Plot.Add.Scatter(Time, Data, ScottPlot.Color.FromColor(ControlColor)).Axes.YAxis = yAxis;
        }
        /// <summary>
        /// 计算鼠标位置与数据点的最小距离
        /// </summary>
        /// <param name="yAxis">纵坐标轴</param>
        /// <param name="mouseLocation">光标位置</param>
        public void GetMinDistance(ScottPlot.IYAxis yAxis, Point mouseLocation)
        {
            InfoLabel.Visible = false;
            Distance = 999.9;
            for (int i = 0; i < Time.Count; i++)
            {
                var valuePoint = Plot.GetPixel(new ScottPlot.Coordinates(Time[i], Data[i]), Plot.Axes.Bottom, yAxis);
                double dis = Math.Sqrt(Math.Pow(valuePoint.X - mouseLocation.X, 2) + Math.Pow(valuePoint.Y - mouseLocation.Y, 2));
                if (dis <= Distance)
                {
                    Distance = dis;
                    TimeIndex = i;
                }
            }
        }
        /// <summary>
        /// 展示鼠标指向的数据点的数据
        /// </summary>
        /// <param name="mouseLocation">鼠标位置</param>
        /// <param name="minDistance">最小距离</param>
        /// <param name="height">数据标签显示高度</param>
        public void ShowData(Point mouseLocation, int minDistance, ref int height)
        {
            if (SelectBox.Checked)
            {
                if (Distance < minDistance)
                {
                    InfoLabel.Text = $"[{Name}] [{DateTime.FromOADate(Time[TimeIndex]):HH:mm:ss}] {Data[TimeIndex]:N3}";
                    InfoLabel.Location = new Point(mouseLocation.X + 30, mouseLocation.Y + InfoLabel.Height * height);
                    InfoLabel.Visible = true;
                    height++;
                }
            }
        }

    }

    public class InputMonitor
    {
        private int run = 0;
        public int Run
        {
            get { return run; }
            set
            {
                if (run == 0 && value == 1)
                    RunChanged?.Invoke(this, EventArgs.Empty);
                run = value;
            }
        }

        private int reset = 0;
        public int Reset
        {
            get { return reset; }
            set
            {
                if (reset == 0 && value == 1)
                    ResetChanged?.Invoke(this, EventArgs.Empty);
                reset = value;
            }
        }

        private int stop = 0;
        public int Stop
        {
            get { return stop; }
            set
            {
                if (stop == 0 && value == 1)
                    StopChanged?.Invoke(this, EventArgs.Empty);
                stop = value;
            }
        }

        //信号触发事件
        public event EventHandler? RunChanged;
        public event EventHandler? ResetChanged;
        public event EventHandler? StopChanged;

        public ZmotionMotionControl Motion;

        public InputMonitor(ZmotionMotionControl motion)
        {
            Motion = motion;
            Task.Run(UpdateInput);
        }

        public void UpdateInput()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(100);
                    double[] input = Motion.GetInputs(16);
                    Run = (int)input[0];
                    Reset = (int)input[1];
                    Stop = (int)input[2];
                }
            }
            catch (Exception)
            {

            }
        }
    }

}
