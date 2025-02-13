using Services;
using UIKit;

namespace PressureCalibration.View
{
    public partial class AxisSetting : Form
    {
        readonly BaseAxis axis;
        readonly SettingInfo settingInfo;

        public AxisSetting(BaseAxis currentAxis)
        {
            InitializeComponent();
            Text = $"{currentAxis.ControllerName} {currentAxis.Name} 轴号 {currentAxis.Number}";
            axis = currentAxis;
            settingInfo = new SettingInfo(axis);
            //变量点
            int x = settingInfo.IniPoint.X;
            int y = settingInfo.IniPoint.Y;
            int rowCount = 0;
            settingInfo.SetParameter(this, axis, ref x, ref y, ref rowCount,
                "State", "IsMoving", "TargetPosition", "CurrentPosition", "CurrentSpeed");
        }

    }
}
