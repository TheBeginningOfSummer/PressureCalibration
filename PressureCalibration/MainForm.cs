using PressureCalibration.View;
using Module;
using System.ComponentModel;
using WinformKit;
using ReaLTaiizor.Controls;
using CSharpKit.FileManagement;

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
            ACQ.Initialize();

        }

        #region ˽�з���
        private void BGWRun_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument is "Run")
            {
                ACQ.Run();
                e.Result = "�������";
            }
            if (e.Argument is "excel")
            {
                UpdateMessage("����Excel");
                string path = $"Data\\Excel\\[{DateTime.Now:yyyy-MM-dd HH_mm_ss}]\\";
                //����ܱ�
                ACQ.OutputExcel(path);
                //�洢ÿ���������ı�
                //foreach (var item in config.ACQ.GroupDic.Values)
                //{
                //    foreach (var sensor in item.SensorDataGroup.Values)
                //    {
                //        if (sensor.Uid == 0) continue;
                //        sensor.OutputExcel(path);
                //    }
                //}
                e.Result = "�������";
            }
            if (e.Argument is "data")
            {
                UpdateMessage("�����궨����");
                string path = $"Data\\SensorData\\[{DateTime.Now:yyyy-MM-dd HH_mm_ss}]\\";
                foreach (var item in ACQ.GroupDic.Values)
                    item.SaveData(path);
                e.Result = "�������";
            }
        }

        private void BGWRun_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is string message) UpdateMessage(message);
        }

        private void UpdateMessage(string message)
        {
            FormKit.UpdateMessage(HRTB��Ϣ, message);
        }

        private bool IsRunning()
        {
            if (!BGWRun.IsBusy) return false;
            UpdateMessage("�����С���");
            return true;
        }

        #endregion

        #region �����水ť
        private void TMI����_Click(object sender, EventArgs e)
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

        private void TMI����_Click(object sender, EventArgs e)
        {
            if (IsRunning()) return;
            if (sender is ToolStripMenuItem menuItem)
            {
                BGWRun.RunWorkerAsync(menuItem.Tag);
            }
        }

        private void TMI���_Click(object sender, EventArgs e)
        {
            HRTB��Ϣ.Clear();
        }

        private void LBN����_Click(object sender, EventArgs e)
        {
            if (IsRunning()) return;
            BGWRun.RunWorkerAsync("Run");
        }

        private void LBN��ͣ_Click(object sender, EventArgs e)
        {
            LostButton button = (LostButton)sender;
            ACQ.IsSuspend = !ACQ.IsSuspend;
            if (ACQ.IsSuspend)
            {
                button.Text = "����";
            }
            else
            {
                button.Text = "��ͣ";
            }
        }

        private void LBN����_Click(object sender, EventArgs e)
        {
            if (IsRunning()) return;
            new SwitchState().ShowDialog();
        }
        #endregion

        private void TMI�л�_Click(object sender, EventArgs e)
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
