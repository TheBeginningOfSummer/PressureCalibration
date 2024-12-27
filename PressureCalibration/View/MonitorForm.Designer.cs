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
            BTN测试 = new Button();
            BTN停止 = new Button();
            BGW监测 = new System.ComponentModel.BackgroundWorker();
            LB压力数据1 = new Label();
            T1 = new System.Windows.Forms.Timer(components);
            LB温度数据 = new Label();
            BTN清除 = new Button();
            CKBD1T1 = new CheckBox();
            SuspendLayout();
            // 
            // FP图表
            // 
            FP图表.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FP图表.DisplayScale = 1F;
            FP图表.Font = new Font("楷体", 9F, FontStyle.Bold);
            FP图表.Location = new Point(12, 12);
            FP图表.Name = "FP图表";
            FP图表.Size = new Size(695, 426);
            FP图表.TabIndex = 0;
            // 
            // BTN测试
            // 
            BTN测试.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BTN测试.Location = new Point(713, 12);
            BTN测试.Name = "BTN测试";
            BTN测试.Size = new Size(75, 23);
            BTN测试.TabIndex = 1;
            BTN测试.Text = "测试";
            BTN测试.UseVisualStyleBackColor = true;
            BTN测试.Click += BTN测试_Click;
            // 
            // BTN停止
            // 
            BTN停止.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BTN停止.Location = new Point(713, 41);
            BTN停止.Name = "BTN停止";
            BTN停止.Size = new Size(75, 23);
            BTN停止.TabIndex = 2;
            BTN停止.Text = "停止";
            BTN停止.UseVisualStyleBackColor = true;
            BTN停止.Click += BTN停止_Click;
            // 
            // BGW监测
            // 
            BGW监测.WorkerSupportsCancellation = true;
            // 
            // LB压力数据1
            // 
            LB压力数据1.AutoSize = true;
            LB压力数据1.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            LB压力数据1.ForeColor = Color.LightSkyBlue;
            LB压力数据1.Location = new Point(46, 9);
            LB压力数据1.Name = "LB压力数据1";
            LB压力数据1.Size = new Size(56, 17);
            LB压力数据1.TabIndex = 3;
            LB压力数据1.Tag = "Pa";
            LB压力数据1.Text = "数据信息";
            LB压力数据1.Visible = false;
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
            LB温度数据.Location = new Point(108, 9);
            LB温度数据.Name = "LB温度数据";
            LB温度数据.Size = new Size(56, 17);
            LB温度数据.TabIndex = 4;
            LB温度数据.Tag = "℃";
            LB温度数据.Text = "数据信息";
            LB温度数据.Visible = false;
            // 
            // BTN清除
            // 
            BTN清除.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BTN清除.Location = new Point(713, 70);
            BTN清除.Name = "BTN清除";
            BTN清除.Size = new Size(75, 23);
            BTN清除.TabIndex = 5;
            BTN清除.Text = "清除";
            BTN清除.UseVisualStyleBackColor = true;
            BTN清除.Click += BTN清除_Click;
            // 
            // CKBD1T1
            // 
            CKBD1T1.AutoSize = true;
            CKBD1T1.Location = new Point(699, 99);
            CKBD1T1.Name = "CKBD1T1";
            CKBD1T1.Size = new Size(57, 21);
            CKBD1T1.TabIndex = 6;
            CKBD1T1.Text = "D1T1";
            CKBD1T1.UseVisualStyleBackColor = true;
            // 
            // MonitorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(CKBD1T1);
            Controls.Add(BTN清除);
            Controls.Add(LB温度数据);
            Controls.Add(LB压力数据1);
            Controls.Add(BTN停止);
            Controls.Add(BTN测试);
            Controls.Add(FP图表);
            Name = "MonitorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "数据监视";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ScottPlot.WinForms.FormsPlot FP图表;
        private Button BTN测试;
        private Button BTN停止;
        private System.ComponentModel.BackgroundWorker BGW监测;
        private Label LB压力数据1;
        private System.Windows.Forms.Timer T1;
        private Label LB温度数据;
        private Button BTN清除;
        private CheckBox CKBD1T1;
    }
}