using CSharpKit.FileManagement;
using Services;

namespace Module
{
    public class Config
    {
        #region 单例模式
        private static Config? _instance;
        private static readonly object _instanceLock = new();
        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                        _instance = new Config();
                }
                return _instance;
            }
        }
        #endregion

        public string ConfigPath { get; private set; } = "Config";

        #region 配置文件名定义
        public string DatabaseName { get; private set; } = "[Config]Database.json";
        public string CalibName { get; private set; } = "[Parameter]CalibPara.json";
        public string MotionName { get; private set; } = "[Parameter]MotionPara.json";
        public string ACQCardName { get; private set; } = "[Device]ACQCard.json";
        public string PressureName { get; private set; } = "[Device]Pressure.json";
        public string TECName { get; private set; } = "[Device]Temperature.json";
        public string ZmotionName { get; private set; } = "[Device]Zmotion.json";
        #endregion

        #region 加载的实例
        public Database DB { get; private set; }
        public CalibrationParameter CP { get; private set; }
        public MotionParameter MP { get; private set; }
        public PressController PACE { get; private set; }
        public TECController TEC { get; private set; }
        public Acquisition ACQ { get; private set; }
        public ZmotionMotionControl Zmotion { get; private set; }
        #endregion

        public Config()
        {
            //注意加载顺序
            DB = Loader.Load<Database>(ConfigPath, DatabaseName, nameof(Database));//加载数据库
            CP = Loader.Load<CalibrationParameter>(ConfigPath, CalibName, nameof(CalibrationParameter));//加载参数
            MP = Loader.Load<MotionParameter>(ConfigPath, CalibName, nameof(MotionParameter));
            PACE = Loader.Load<PressController>(ConfigPath, PressureName, nameof(PressController));//加载设备
            TEC = Loader.Load<TECController>(ConfigPath, TECName, nameof(TECController));//加载设备
            Zmotion = Loader.Load<ZmotionMotionControl>(ConfigPath, ZmotionName, nameof(ZmotionMotionControl));

            ACQ = Loader.Load<Acquisition>(ConfigPath, ACQCardName, nameof(Acquisition));//采集总实例，需最后加载
            
        }
    }
}
