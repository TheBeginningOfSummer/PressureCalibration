using CSharpKit.FileManagement;
using System.ComponentModel;

namespace Module
{
    public class CalibrationParameter : Loader
    {
        /// <summary>
        /// 每个数据采集时的重复次数
        /// </summary>
        public int AcquisitionCount { get; set; } = 1;
        /// <summary>
        /// 压力采集时的延时时间
        /// </summary>
        public int PressDelay { get; set; } = 10;
        /// <summary>
        /// 采集的温度点
        /// </summary>
        public BindingList<decimal> TempaturePoints { get; set; } = [15, 30, 50];
        /// <summary>
        /// 采集温度设置点
        /// </summary>
        public BindingList<decimal> SetTPoints { get; set; } = [15, 30, 50];
        /// <summary>
        /// 采集的压力点
        /// </summary>
        public BindingList<decimal> PressurePoints { get; set; } = [55000, 65000, 80000, 90000, 105000];
        /// <summary>
        /// 验证的压力点
        /// </summary>
        public BindingList<decimal> VerifyPressures { get; set; } = [50000, 100000, 110000];
        /// <summary>
        /// 采集数据时稳定的温差
        /// </summary>
        public decimal MaxTemperatureDiff { get; set; } = 1M;
        /// <summary>
        /// 采集温度时稳定的压差
        /// </summary>
        public decimal MaxPressureDiff { get; set; } = 10;
        /// <summary>
        /// 超时时间
        /// </summary>
        public decimal TTimeout { get; set; } = 360;
        /// <summary>
        /// 压力超时
        /// </summary>
        public decimal PTimeout { get; set; } = 120;
        /// <summary>
        /// 烧录的最大温差
        /// </summary>
        public double FuseTDiff { get; set; } = 0.5;
        /// <summary>
        /// 烧录时的最大差值
        /// </summary>
        public double FusePDiff { get; set; } = 30;
        /// <summary>
        /// 是否烧录
        /// </summary>
        public bool IsFuse { get; set; } = false;

        /// <summary>
        /// 检测温度
        /// </summary>
        public BindingList<decimal> CheckTemperatures { get; set; } = [-20, -10, 0, 10, 20, 30, 40, 50, 60, 70, 80];
        /// <summary>
        /// 检测压力
        /// </summary>
        public BindingList<decimal> CheckPressures { get; set; } = [30000, 60000, 90000, 120000, 150000, 180000, 210000];
        /// <summary>
        /// 检测温差
        /// </summary>
        public decimal CheckTemperatureDiff { get; set; } = 0.5m;
        /// <summary>
        /// 检测压差
        /// </summary>
        public decimal CheckPressureDiff { get; set; } = 80;

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

                nameof(CheckTemperatures) => "检测温度",
                nameof(CheckPressures) => "检测压力",
                nameof(CheckPressureDiff) => "检测压力差",
                nameof(CheckTemperatureDiff) => "检测温度差",

                nameof(IsTestVer) => "测试版本",
                nameof(Method) => "计算方法",
                nameof(IsSave) => "数据保存",
                _ => name,
            };
        }


    }

    public class MotionParameter : Loader
    {
        public double Axis1Work { get; set; }
        public double Axis2Work { get; set; }
        public float Axis1Torque { get; set; }
        public float Axis2Torque { get; set; }
        public int Axis1RotateSpeed { get; set; }
        public int Axis2RotateSpeed { get; set; }

        public double Axis1Upload { get; set; }
        public double Axis2Upload { get; set; }

        public MotionParameter() { }

        public override string Translate(string name)
        {
            return name switch
            {
                nameof(Axis1Work) => "轴1工作位置",
                nameof(Axis2Work) => "轴2工作位置",
                nameof(Axis1Torque) => "轴1目标转矩",
                nameof(Axis2Torque) => "轴2目标转矩",
                nameof(Axis1RotateSpeed) => "轴1转速",
                nameof(Axis2RotateSpeed) => "轴2转速",
                nameof(Axis1Upload) => "轴1上料位置",
                nameof(Axis2Upload) => "轴2上料位置",
                _ => name,
            };
        }
    }
}
