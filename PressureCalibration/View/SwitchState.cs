using Module;

namespace PressureCalibration.View
{
    public partial class SwitchState : Form
    {
        IAcqState acqState = new Initialize();

        public SwitchState()
        {
            InitializeComponent();
        }

        private void LBN确定_Click(object sender, EventArgs e)
        {
            Acquisition.Instance.SetState(acqState);
            Close();
        }

        private void RB状态更改_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag is string state)
            {
                switch (state)
                {
                    case "Init":
                        acqState = new Initialize();
                        break;
                    case "T1":
                        Acquisition.Instance.CurrentTempIndex = 0;
                        acqState = new WaitT();
                        break;
                    case "T2":
                        Acquisition.Instance.CurrentTempIndex = 1;
                        acqState = new WaitT();
                        break;
                    case "T3":
                        Acquisition.Instance.CurrentTempIndex = 2;
                        acqState = new WaitT();
                        break;
                    case "P":
                        acqState = new WaitP();
                        break;
                }
            }
        }

    }
}
