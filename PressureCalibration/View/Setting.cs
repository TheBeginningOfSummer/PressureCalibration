using CSharpKit;
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

            settingInfos.Add(new SettingInfo(Loader.Get<CalibrationParameter>()));
            settingInfos.Add(new SettingInfo(Loader.Get<Acquisition>()));
            settingInfos.Add(new SettingInfo(Loader.Get<PressController>()));
            settingInfos.Add(new SettingInfo(Loader.Get<TECController>()));
            settingInfos.Add(new SettingInfo(Loader.Get<ZmotionMotionControl>()));
            settingInfos.Add(new SettingInfo(Loader.Get<MotionParameter>()));
            for (int i = 0; i < settingInfos.Count; i++)
            {
                settingInfos[i].Initialize(HTP设置.TabPages[i]);
                settingInfos[i].CommandAction += SetChildItem;
            }
        }

        private void SetChildItem(object? argument)
        {
            if (argument is ISetting setting)
            {
                new ChildItemSetting(setting).Show();
            }
        }

        private void BTN保存_Click(object sender, EventArgs e)
        {
            settingInfos[HTP设置.SelectedIndex].Save();
        }

    }

}
