using PressureCalibration.View;
using Module;
using System.ComponentModel;
using CSharpKit.FileManagement;
using WinformKit;
using ReaLTaiizor.Controls;

namespace PressureCalibration
{
    public partial class MainForm : Form
    {
        readonly Config config = Config.Instance;
        readonly Service service = Service.Instance;

        public MainForm()
        {
            InitializeComponent();

            BGWRun.DoWork += BGWRun_DoWork;
            BGWRun.RunWorkerCompleted += BGWRun_RunWorkerCompleted;

            config.ACQ.WorkProcess += UpdateMessage;
            Initialize();
        }

        #region 私有方法
        private void BGWRun_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument is "Run")
            {
                config.ACQ.Run();
                e.Result = "运行完成";
            }
            if (e.Argument is "Excel")
            {
                UpdateMessage("导出Excel");
                string path = $"Data\\Excel\\[{DateTime.Now:yyyy-MM-dd HH_mm_ss}]\\";
                //输出总表
                config.ACQ.OutputExcel(path);
                //存储每个传感器的表
                //foreach (var item in config.ACQ.GroupDic.Values)
                //{
                //    foreach (var sensor in item.SensorDataGroup.Values)
                //    {
                //        if (sensor.Uid == 0) continue;
                //        sensor.OutputExcel(path);
                //    }
                //}
                e.Result = "导出完成";
            }
            if (e.Argument is "Data")
            {
                UpdateMessage("导出标定数据");
                string path = $"Data\\SensorData\\[{DateTime.Now:yyyy-MM-dd HH_mm_ss}]\\";
                foreach (var item in config.ACQ.GroupDic.Values)
                    item.SaveData(path);
                e.Result = "导出完成";
            }
        }

        private void BGWRun_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is string message) UpdateMessage(message);
        }

        private void UpdateMessage(string message)
        {
            FormKit.UpdateMessage(HRTB信息, message);
        }

        private bool IsRunning()
        {
            if (!BGWRun.IsBusy) return false;
            UpdateMessage("运行中……");
            return true;
        }

        private void Initialize()
        {
            if (config.ACQ.Open())
                UpdateMessage("连接采集卡成功！");
            else
                UpdateMessage("连接采集卡失败！");
            if (config.PACE.Connect())
                UpdateMessage("连接压力控制器成功！");
            else
                UpdateMessage("连接压力控制器失败！");
            if (config.TEC.Open())
                UpdateMessage("连接温度控制器成功！");
            else
                UpdateMessage("连接温度控制器失败！");
        }
        #endregion

        #region 主界面按钮
        private void TMI窗口_Click(object sender, EventArgs e)
        {
            new MonitorForm().Show();
        }

        private void TMI测试_Click(object sender, EventArgs e)
        {
            new Test().Show();
        }

        private void TMI清除_Click(object sender, EventArgs e)
        {
            HRTB信息.Clear();
        }

        private void TMI导出_Click(object sender, EventArgs e)
        {
            if (IsRunning()) return;
            ToolStripMenuItem button = (ToolStripMenuItem)sender;
            if (button.Tag is string tag)
                BGWRun.RunWorkerAsync(tag);
        }

        private void TMI设置_Click(object sender, EventArgs e)
        {
            new Setting().Show();
        }

        private void LBN运行_Click(object sender, EventArgs e)
        {
            if (IsRunning()) return;
            BGWRun.RunWorkerAsync("Run");
        }

        private void LBN暂停_Click(object sender, EventArgs e)
        {
            LostButton button = (LostButton)sender;
            config.ACQ.IsSuspend = !config.ACQ.IsSuspend;
            if (config.ACQ.IsSuspend)
            {
                button.Text = "继续";
            }
            else
            {
                button.Text = "暂停";
            }
        }

        private void LBN其他_Click(object sender, EventArgs e)
        {
            if (IsRunning()) return;
            new SwitchState().ShowDialog();
        }
        #endregion

        private void TMI切换_Click(object sender, EventArgs e)
        {
            //service.Statistic.Switch(RunningState.Warning);
            //Thread.Sleep(10000);
            //service.Statistic.Switch(RunningState.Debug);
            //Thread.Sleep(10000);
            //service.Statistic.Switch(RunningState.Normal);
            //Thread.Sleep(10000);
        }

        
    }
}
