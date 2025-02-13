using CSharpKit.FileManagement;
using Module;
using Services;
using UIKit;

namespace PressureCalibration.View
{
    public partial class Setting : Form
    {
        readonly List<SettingInfo> settingInfos = [];

        public Setting()
        {
            InitializeComponent();

            settingInfos.Add(new SettingInfo(ParameterManager.Get<CalibrationParameter>()));
            settingInfos.Add(new SettingInfo(ParameterManager.Get<Acquisition>()));
            settingInfos.Add(new SettingInfo(ParameterManager.Get<PressController>()));
            settingInfos.Add(new SettingInfo(ParameterManager.Get<TECController>()));
            settingInfos.Add(new SettingInfo(ParameterManager.Get<ZmotionMotionControl>()));
            for (int i = 0; i < settingInfos.Count; i++)
            {
                settingInfos[i].Initialize(HTP设置.TabPages[i]);
                settingInfos[i].CommandAction += SetAxis;
            }
        }

        private void SetAxis(object? argument)
        {
            if (argument is ZmotionAxis axis)
            {
                AxisSetting axisSetting = new(axis);
                axisSetting.Show();
            }
        }

        private void BTN保存_Click(object sender, EventArgs e)
        {
            settingInfos[HTP设置.SelectedIndex].Save();
        }

    }

}
