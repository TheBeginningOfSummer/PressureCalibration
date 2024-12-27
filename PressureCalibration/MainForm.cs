using Calibration.Services;
using CSharpKit.FileManagement;
using PressureCalibration.View;
using Services;

namespace PressureCalibration
{
    public partial class MainForm : Form
    {
        Config config = Config.Instance;
        readonly StatisticTime statistic = new();

        public MainForm()
        {
            InitializeComponent();
            statistic.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            statistic.End();
        }

        private static void LoadSettingForm<T>(string name) where T : ParameterManager
        {
            Setting setting = new() { Text = name };
            setting.Initialize(ParameterManager.Get<T>());
            setting.Show();
        }

        private void TMI设置_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem button = (ToolStripMenuItem)sender;
            if (button.Tag is string t)
            {
                switch (t)
                {
                    case "APara":
                        LoadSettingForm<CalibrationParameter>("采集参数设置");
                        break;
                    case "A":
                        LoadSettingForm<Acquisition>("采集卡设置");
                        break;
                    case "P":
                        LoadSettingForm<PressController>("压控设置");
                        break;
                    case "T":
                        LoadSettingForm<TECController>("温控设置");
                        break;
                }
            }
        }

        private void TMI监控_Click(object sender, EventArgs e)
        {
            MonitorForm monitorf = new();
            monitorf.Show();
        }
        
        private void TMI测试_Click(object sender, EventArgs e)
        {
            
        }

        private void TMI切换_Click(object sender, EventArgs e)
        {
            statistic.Switch(RunningState.Warning);
            Thread.Sleep(10000);
            statistic.Switch(RunningState.Debug);
            Thread.Sleep(10000);
            statistic.Switch(RunningState.Normal);
            Thread.Sleep(10000);
        }

        
    }
}
