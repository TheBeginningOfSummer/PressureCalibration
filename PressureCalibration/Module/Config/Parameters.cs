using CSharpKit.FileManagement;

namespace Module
{
    public class CalibrationParameter : ParameterManager
    {
        /// <summary>
        /// 每个数据采集时的重复次数
        /// </summary>
        public int AcquisitionCount { get; set; } = 1;
        /// <summary>
        /// 采集温度时的延迟时间
        /// </summary>
        public int TempDelay { get; set; } = 60;
        /// <summary>
        /// 压力采集时的延时时间
        /// </summary>
        public int PressDelay { get; set; } = 10;
        /// <summary>
        /// 采集的温度点
        /// </summary>
        public List<decimal> TempaturePoints { get; set; } = [15, 30, 50];
        /// <summary>
        /// 采集温度设置点
        /// </summary>
        public List<decimal> SetTPoints { get; set; } = [15, 30, 50];
        /// <summary>
        /// 采集的压力点
        /// </summary>
        public List<decimal> PressurePoints { get; set; } = [55000, 65000, 80000, 90000, 105000];
        /// <summary>
        /// 验证的压力点
        /// </summary>
        public List<decimal> VerifyPressures { get; set; } = [50000, 100000, 110000];
        /// <summary>
        /// 稳定温差
        /// </summary>
        public decimal MaxTemperatureDiff { get; set; } = 1M;

        public decimal MaxPressureDiff { get; set; } = 10;

        public decimal TTimeout { get; set; } = 360;

        public decimal PTimeout { get; set; } = 120;
        /// <summary>
        /// 烧录时的最大差值
        /// </summary>
        public double FusePDiff { get; set; } = 30;

        public double FuseTDiff { get; set; } = 0.5;

        public bool IsFuse { get; set; } = false;

        public bool IsTestVer { get; set; } = true;

        public bool Method { get; set; } = true;

        public bool IsSave { get; set; } = false;

        public CalibrationParameter()
        {
            
        }

        public override string Translate(string name)
        {
            return name switch
            {
                nameof(AcquisitionCount) => "采集次数",
                nameof(TempDelay) => "温度延时(S)",
                nameof(PressDelay) => "压力延时(S)",
                nameof(TempaturePoints) => "采集温度(℃)",
                nameof(SetTPoints) => "设置温度(℃)",
                nameof(PressurePoints) => "采集压力(Pa)",
                nameof(VerifyPressures) => "验证压力(Pa)",
                nameof(MaxTemperatureDiff) => "采集温差(℃)",
                nameof(MaxPressureDiff) => "采集压差(Pa)",
                nameof(TTimeout) => "温度超时(S)",
                nameof(PTimeout) => "压力超时(S)",
                nameof(FusePDiff) => "烧录压差(Pa)",
                nameof(FuseTDiff) => "烧录温差(℃)",
                nameof(IsFuse) => "是否烧录",
                nameof(IsTestVer) => "测试版本",
                nameof(Method) => "计算方法",
                nameof(IsSave) => "数据保存",
                _ => "",
            };
        }


    }

}
