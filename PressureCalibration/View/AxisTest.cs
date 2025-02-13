using Services;

namespace CompreDemo.Forms
{
    public partial class AxisTest : Form
    {
        readonly BaseAxis? baseAxis;
        readonly CancellationTokenSource cancellation = new();

        private double tposition = 0;
        public double TargetPosition
        {
            get { return tposition; }
            set { tposition = value; }
        }

        public AxisTest(BaseAxis axis)
        {
            InitializeComponent();

            baseAxis = axis;
            Text = $"{baseAxis.ControllerName} {baseAxis.Name} 轴号 {baseAxis.Number}";
            TB目标位置.DataBindings.Add("Text", this, nameof(TargetPosition));
            Task.Run(UpdateAxisInfo);
        }

        private void UpdateAxisInfo()
        {
            try
            {
                while (true)
                {
                    cancellation.Token.ThrowIfCancellationRequested();
                    Thread.Sleep(100);
                    if (LB轴信息.InvokeRequired)
                        LB轴信息.BeginInvoke(new Action(() => LB轴信息.Text = baseAxis?.GetAxisState()));
                    else
                        LB轴信息.Text = baseAxis?.GetAxisState();
                }
            }
            catch (Exception)
            {
                //任务结束

            }
        }

        private void ManualControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancellation.Cancel();
        }

        private void BTNAxisControl_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string? action = button.Tag?.ToString();
            switch (action)
            {
                case "zero":
                    baseAxis?.DefPos();
                    break;
                case "relative":
                    baseAxis?.SingleRelativeMove(TargetPosition);
                    break;
                case "absolute":
                    baseAxis?.SingleAbsoluteMove(TargetPosition);
                    break;
                case "back":
                    baseAxis?.Reverse();
                    break;
                case "forward":
                    baseAxis?.Forward();
                    break;
                case "home":
                    baseAxis?.Datum(3);
                    break;
                case "stop":
                    baseAxis?.Stop(2);
                    break;
                default: break;
            }
        }

    }
}
