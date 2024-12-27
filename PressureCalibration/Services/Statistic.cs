using CSharpKit;
using CSharpKit.FileManagement;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Services
{

    public class StatisticTime
    {
        public string GUID{ get; set; } 
        public string StartTime { get; set; }
        public string EndTime { get; set; } = "";
        public uint NormalTime { get; set; } = 0;
        public uint WarningTime { get; set; } = 0;
        public uint DebugTime { get; set; } = 0;

        public RunningState State = RunningState.Normal;
        readonly TimerKit kit = new();

        public StatisticTime()
        {
            GUID = Guid.NewGuid().ToString();
            StartTime = DateTime.Now.ToString("G");
            kit.Count += Update;
        }

        public void Initialize()
        {

        }

        public void Start()
        {
            kit.Start();
        }

        public void Stop() {
            kit.Stop();
        }

        public string Show()
        {
            return $"开始时间:{StartTime} 结束时间:{EndTime} 工作时长:{NormalTime}S 报警时长:{WarningTime}S 调试时长:{DebugTime}S";
        }

        public void Save()
        {
            //FileManager.AppendFlieString("Log", $"[{DateTime.Now:yyyy-MM-dd}]{GUID}.log", Show(), FileMode.CreateNew);
            FileManager.AppendCSV("Log", $"[{DateTime.Now:yyyy-MM-dd}]时长统计.csv", ["开始时间", "结束时间", "工作时长(S)", "报警时长(S)", "调试时长(S)"],
                [StartTime, EndTime, NormalTime.ToString(), WarningTime.ToString(), DebugTime.ToString()]);
        }

        public void End()
        {
            Stop();
            EndTime = DateTime.Now.ToString("G");
            Save();
        }

        public void Switch(RunningState state)
        {
            State = state;
        }

        public void Update(int time)
        {
            switch (State)
            {
                case RunningState.Normal:
                    NormalTime++;
                    break;
                case RunningState.Warning:
                    WarningTime++;
                    break;
                case RunningState.Debug:
                    DebugTime++;
                    break;
                default:
                    break;
            }
            Debug.WriteLine($"Nor:{NormalTime} Warning:{WarningTime} Debug:{DebugTime}");
        }
    }

    public enum RunningState
    {
        Normal,
        Warning,
        Debug,
    }

    public class StatisticCounter
    {
        public string? Date { get; set; }
        public string? CurrentDate { get; set; }
        [JsonIgnore]
        public uint CurrentUploadCount { get; set; }
        [JsonIgnore]
        public uint CurrentCalibCount { get; set; }
        [JsonIgnore]
        public uint CurrentLayOffCount { get; set; }
        [JsonIgnore]
        public uint Count1 { get; set; }
        [JsonIgnore]
        public uint Count2 { get; set; }
        public uint Count3 { get; set; }

        public StatisticCounter()
        {

        }

        public void Initialize()
        {
            Date ??= DateTime.Now.ToString();
        }

        public void Save()
        {
            JsonManager.Save("Data", "Counter.json", this);
        }

        public void GetCount()
        {
            CurrentDate = DateTime.Now.ToString();
            Count1 = CurrentUploadCount - CurrentCalibCount;
            Count2 = CurrentCalibCount - CurrentLayOffCount;
            Count3 += (CurrentUploadCount - CurrentLayOffCount);
            Save();
        }

        public void Clear()
        {
            Count1 = 0;
            Count2 = 0;
            Count3 = 0;
            Save();
        }

        
    }
}
