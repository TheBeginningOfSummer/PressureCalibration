using CSharpKit.Communication;
using Services;
using UIKit;

namespace PressureCalibration.View
{
    public partial class ChildItemSetting : Form
    {
        readonly SettingInfo settingInfo = new();

        public ChildItemSetting(BaseAxis currentAxis)
        {
            InitializeComponent();
            Text = $"{currentAxis.ControllerName} {currentAxis.Name} 轴号 {currentAxis.Number}";

            //变量点
            int x = settingInfo.IniPoint.X;
            int y = settingInfo.IniPoint.Y;
            int rowCount = 0;
            settingInfo.SetChildParameter(this, currentAxis, ref x, ref y, ref rowCount,
                "State", "IsMoving", "TargetPosition", "CurrentPosition", "CurrentSpeed");
        }

        public ChildItemSetting(SerialPortTool currentCom)
        {
            InitializeComponent();
            Text = $"{currentCom.PortName}";

            //变量点
            int x = settingInfo.IniPoint.X;
            int y = settingInfo.IniPoint.Y;
            int rowCount = 0;
            settingInfo.SetChildParameter(this, currentCom, ref x, ref y, ref rowCount, 
                "Parity", "DataBits", "StopBits");
        }

    }
}
