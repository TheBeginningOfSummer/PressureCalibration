using PressureCalibration.View;
using Module;
using System.ComponentModel;
using WinformKit;
using ReaLTaiizor.Controls;

namespace PressureCalibration
{
    public partial class MainForm : Form
    {
        readonly Acquisition ACQ = Acquisition.Instance;
        readonly Service service = Service.Instance;

        public MainForm()
        {
            InitializeComponent();

            BGWRun.DoWork += BGWRun_DoWork;
            BGWRun.RunWorkerCompleted += BGWRun_RunWorkerCompleted;

            ACQ.WorkProcess += UpdateMessage;
            ACQ.InitializeDevice();

        }

        #region 私有方法
        private void BGWRun_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument is "Run")
            {
                ACQ.Run();
                e.Result = "运行完成";
            }
            if (e.Argument is "excel")
            {
                UpdateMessage("导出Excel");
                string path = $"Data\\Excel\\[{DateTime.Now:yyyy-MM-dd HH_mm_ss}]\\";
                //输出总表
                ACQ.OutputExcel(path);
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
            if (e.Argument is "data")
            {
                UpdateMessage("导出标定数据");
                string path = $"Data\\SensorData\\[{DateTime.Now:yyyy-MM-dd HH_mm_ss}]\\";
                foreach (var item in ACQ.GroupDic.Values)
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

        #endregion

        #region 主界面按钮
        private void TMI窗口_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                switch (menuItem.Tag)
                {
                    case "monitor": new MonitorForm().Show(); break;
                    case "test": new Test().Show(); break;
                    case "chip": new ChipTest().Show(); break;
                    default: break;
                }
            }
            else if (sender is ToolStripButton button)
            {
                switch (button.Tag)
                {
                    case "setting": new Setting().Show(); break;
                    default: break;
                }
            }
        }

        private void TMI导出_Click(object sender, EventArgs e)
        {
            if (IsRunning()) return;
            if (sender is ToolStripMenuItem menuItem)
            {
                BGWRun.RunWorkerAsync(menuItem.Tag);
            }
        }

        private void TMI清除_Click(object sender, EventArgs e)
        {
            HRTB信息.Clear();
        }

        private void LBN运行_Click(object sender, EventArgs e)
        {
            if (IsRunning()) return;
            BGWRun.RunWorkerAsync("Run");
        }

        private void LBN暂停_Click(object sender, EventArgs e)
        {
            LostButton button = (LostButton)sender;
            ACQ.IsSuspend = !ACQ.IsSuspend;
            if (ACQ.IsSuspend)
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

        private void BTN测试_Click(object sender, EventArgs e)
        {
            double[] p = [872639, 43879, -171863, -402066, 874956, 48653, -166100];
            double[] t = [153638, 153704, 153510, 153460, 86508, 86726, 86300];
            double[] pr = [40000, 85000, 95000, 105000, 40000, 85000, 95000];
            double[] tr = [24.632812, 24.640625, 24.687500, 24.703125, 58.007812, 57.906250, 58.140625];
            List<RawData> oriData = [];
            for (int i = 0; i < 7; i++)
            {
                RawData rawData = new() { PRaw = (int)p[i], TRaw = (int)t[i], PRef = (decimal)pr[i], TRef = (decimal)tr[i] };
                oriData.Add(rawData);
            }
            var r = Calculation.StartCalibration(oriData);
            if (r == null) return;
            Calculation.StartValidation(r, 872639, 153638, out double pcal, out double tcal);
            UpdateMessage($"pcal:{pcal}  tcal:{tcal}");
            UpdateMessage(r.Show());
        }
    }
}
