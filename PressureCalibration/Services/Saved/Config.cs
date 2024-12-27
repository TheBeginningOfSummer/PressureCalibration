using CSharpKit.FileManagement;

namespace Calibration.Services
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
        public string ACQCardName { get; private set; } = "[Device]ACQCard.json";
        public string PressureName { get; private set; } = "[Device]Pressure.json";
        public string TECName { get; private set; } = "[Device]Temperature.json";
        #endregion

        public Config()
        {
            //注意加载顺序
            ParameterManager.Load<Database>(ConfigPath, DatabaseName, nameof(Database));//加载数据库
            ParameterManager.Load<CalibrationParameter>(ConfigPath, CalibName, nameof(CalibrationParameter));//加载参数
            //ParameterManager.Load<Workflow>(ConfigPath, ACQCardName, nameof(Workflow));//加载参数
            ParameterManager.Load<PressController>(ConfigPath, PressureName, nameof(PressController));//加载设备
            ParameterManager.Load<TECController>(ConfigPath, TECName, nameof(TECController));//加载设备
        }
    }
}
