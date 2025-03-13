namespace PressureCalibration.View
{
    partial class AxisTest
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
            LB目标位置 = new Label();
            TB目标位置 = new TextBox();
            BTN相对移动 = new Button();
            BTN绝对移动 = new Button();
            BTN后 = new Button();
            BTN前 = new Button();
            BTN回原点 = new Button();
            BTN停止 = new Button();
            BTN位置清零 = new Button();
            LB轴信息 = new Label();
            SuspendLayout();
            // 
            // LB目标位置
            // 
            LB目标位置.AutoSize = true;
            LB目标位置.Location = new Point(12, 9);
            LB目标位置.Name = "LB目标位置";
            LB目标位置.Size = new Size(56, 17);
            LB目标位置.TabIndex = 0;
            LB目标位置.Text = "目标位置";
            // 
            // TB目标位置
            // 
            TB目标位置.Location = new Point(74, 6);
            TB目标位置.Name = "TB目标位置";
            TB目标位置.Size = new Size(93, 23);
            TB目标位置.TabIndex = 1;
            // 
            // BTN相对移动
            // 
            BTN相对移动.Location = new Point(12, 35);
            BTN相对移动.Name = "BTN相对移动";
            BTN相对移动.Size = new Size(75, 23);
            BTN相对移动.TabIndex = 3;
            BTN相对移动.Tag = "relative";
            BTN相对移动.Text = "相对移动";
            BTN相对移动.UseVisualStyleBackColor = true;
            BTN相对移动.Click += BTNAxisControl_Click;
            // 
            // BTN绝对移动
            // 
            BTN绝对移动.Location = new Point(92, 35);
            BTN绝对移动.Name = "BTN绝对移动";
            BTN绝对移动.Size = new Size(75, 23);
            BTN绝对移动.TabIndex = 4;
            BTN绝对移动.Tag = "absolute";
            BTN绝对移动.Text = "绝对移动";
            BTN绝对移动.UseVisualStyleBackColor = true;
            BTN绝对移动.Click += BTNAxisControl_Click;
            // 
            // BTN后
            // 
            BTN后.Location = new Point(12, 64);
            BTN后.Name = "BTN后";
            BTN后.Size = new Size(75, 23);
            BTN后.TabIndex = 5;
            BTN后.Tag = "back";
            BTN后.Text = "←";
            BTN后.UseVisualStyleBackColor = true;
            BTN后.Click += BTNAxisControl_Click;
            // 
            // BTN前
            // 
            BTN前.Location = new Point(92, 64);
            BTN前.Name = "BTN前";
            BTN前.Size = new Size(75, 23);
            BTN前.TabIndex = 6;
            BTN前.Tag = "forward";
            BTN前.Text = "→";
            BTN前.UseVisualStyleBackColor = true;
            BTN前.Click += BTNAxisControl_Click;
            // 
            // BTN回原点
            // 
            BTN回原点.Location = new Point(12, 93);
            BTN回原点.Name = "BTN回原点";
            BTN回原点.Size = new Size(75, 23);
            BTN回原点.TabIndex = 7;
            BTN回原点.Tag = "home";
            BTN回原点.Text = "回原点";
            BTN回原点.UseVisualStyleBackColor = true;
            BTN回原点.Click += BTNAxisControl_Click;
            // 
            // BTN停止
            // 
            BTN停止.Location = new Point(92, 93);
            BTN停止.Name = "BTN停止";
            BTN停止.Size = new Size(75, 23);
            BTN停止.TabIndex = 8;
            BTN停止.Tag = "stop";
            BTN停止.Text = "停止";
            BTN停止.UseVisualStyleBackColor = true;
            BTN停止.Click += BTNAxisControl_Click;
            // 
            // BTN位置清零
            // 
            BTN位置清零.Location = new Point(172, 6);
            BTN位置清零.Name = "BTN位置清零";
            BTN位置清零.Size = new Size(75, 23);
            BTN位置清零.TabIndex = 9;
            BTN位置清零.Tag = "zero";
            BTN位置清零.Text = "位置清零";
            BTN位置清零.UseVisualStyleBackColor = true;
            BTN位置清零.Click += BTNAxisControl_Click;
            // 
            // LB轴信息
            // 
            LB轴信息.AutoSize = true;
            LB轴信息.Location = new Point(173, 41);
            LB轴信息.Name = "LB轴信息";
            LB轴信息.Size = new Size(44, 17);
            LB轴信息.TabIndex = 10;
            LB轴信息.Text = "轴信息";
            // 
            // AxisTest
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(320, 124);
            Controls.Add(LB轴信息);
            Controls.Add(BTN位置清零);
            Controls.Add(BTN停止);
            Controls.Add(BTN回原点);
            Controls.Add(BTN前);
            Controls.Add(BTN后);
            Controls.Add(BTN绝对移动);
            Controls.Add(BTN相对移动);
            Controls.Add(TB目标位置);
            Controls.Add(LB目标位置);
            Name = "AxisTest";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ManualControl";
            FormClosing += ManualControl_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LB目标位置;
        private TextBox TB目标位置;
        private Button BTN相对移动;
        private Button BTN绝对移动;
        private Button BTN后;
        private Button BTN前;
        private Button BTN回原点;
        private Button BTN停止;
        private Button BTN位置清零;
        private Label LB轴信息;
    }
}