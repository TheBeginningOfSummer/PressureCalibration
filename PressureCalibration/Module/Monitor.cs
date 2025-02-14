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
                DisplayedData.TryAdd(keys[i], []);
            Task.Run(RefreshData);
        }

        public static Dictionary<string, double> GetDataContainer(params string[] keys)
        {
            Dictionary<string, double> data = [];
            data.Add("Time", -1);
            for (int i = 0; i < keys.Length; i++)
                data.Add(keys[i], -1);
            return data;
        }
        
        public static void AddData(Dictionary<string, double> data)
        {
            foreach (var item in data)
            {
                DisplayedData.AddOrUpdate(item.Key, [item.Value],
                    (oldKey, oldList) => { oldList.Add(item.Value); return oldList; });
            }
        }

        public static async Task RefreshData()
        {
            while (await Cache.Reader.WaitToReadAsync())
            {
                if (Cache.Reader.TryRead(out Dictionary<string, double>? data))
                {
                    if (data != null)
                    {
                        AddData(data);
                        UpdataData?.Invoke(data);
                    }
                }
            }
        }

        public static void ClearDisplayedData()
        {
            foreach (var item in DisplayedData)
            {
                item.Value.Clear();
            }
        }

    }

    public class IOMonitor
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

        public IOMonitor(ZmotionMotionControl motion)
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
