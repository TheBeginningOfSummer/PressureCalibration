namespace PressureCalibration.View
{
    partial class MonitorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            FP图表 = new ScottPlot.WinForms.FormsPlot();
            BGW监测 = new System.ComponentModel.BackgroundWorker();
            LB压力数据 = new Label();
            T1 = new System.Windows.Forms.Timer(components);
            LB温度数据 = new Label();
            menuStrip1 = new MenuStrip();
            TMI停止 = new ToolStripMenuItem();
            TMI开始 = new ToolStripMenuItem();
            TMI清除 = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // FP图表
            // 
            FP图表.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FP图表.DisplayScale = 1F;
            FP图表.Font = new Font("楷体", 9F, FontStyle.Bold);
            FP图表.Location = new Point(0, 26);
            FP图表.Margin = new Padding(1);
            FP图表.Name = "FP图表";
            FP图表.Size = new Size(695, 422);
            FP图表.TabIndex = 0;
            // 
            // BGW监测
            // 
            BGW监测.WorkerSupportsCancellation = true;
            // 
            // LB压力数据
            // 
            LB压力数据.AutoSize = true;
            LB压力数据.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            LB压力数据.ForeColor = Color.LightSkyBlue;
            LB压力数据.Location = new Point(699, 407);
            LB压力数据.Name = "LB压力数据";
            LB压力数据.Size = new Size(56, 17);
            LB压力数据.TabIndex = 3;
            LB压力数据.Tag = "Pa";
            LB压力数据.Text = "数据信息";
            LB压力数据.Visible = false;
            // 
            // T1
            // 
            T1.Interval = 1000;
            // 
            // LB温度数据
            // 
            LB温度数据.AutoSize = true;
            LB温度数据.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            LB温度数据.ForeColor = Color.Orange;
            LB温度数据.Location = new Point(699, 424);
            LB温度数据.Name = "LB温度数据";
            LB温度数据.Size = new Size(56, 17);
            LB温度数据.TabIndex = 4;
            LB温度数据.Tag = "℃";
            LB温度数据.Text = "数据信息";
            LB温度数据.Visible = false;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.Transparent;
            menuStrip1.Items.AddRange(new ToolStripItem[] { TMI停止, TMI开始, TMI清除 });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 25);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // TMI停止
            // 
            TMI停止.Alignment = ToolStripItemAlignment.Right;
            TMI停止.Name = "TMI停止";
            TMI停止.Size = new Size(44, 21);
            TMI停止.Text = "停止";
            TMI停止.Click += TMI停止_Click;
            // 
            // TMI开始
            // 
            TMI开始.Alignment = ToolStripItemAlignment.Right;
            TMI开始.Name = "TMI开始";
            TMI开始.Size = new Size(44, 21);
            TMI开始.Text = "开始";
            TMI开始.Click += TMI开始_Click;
            // 
            // TMI清除
            // 
            TMI清除.Alignment = ToolStripItemAlignment.Right;
            TMI清除.Name = "TMI清除";
            TMI清除.Size = new Size(44, 21);
            TMI清除.Text = "清除";
            TMI清除.Click += TMI清除_Click;
            // 
            // MonitorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(LB温度数据);
            Controls.Add(LB压力数据);
            Controls.Add(FP图表);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MonitorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "数据监视";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ScottPlot.WinForms.FormsPlot FP图表;
        private System.ComponentModel.BackgroundWorker BGW监测;
        private Label LB压力数据;
        private System.Windows.Forms.Timer T1;
        private Label LB温度数据;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem TMI停止;
        private ToolStripMenuItem TMI开始;
        private ToolStripMenuItem TMI清除;
    }
}