namespace PressureCalibration.View
{
    partial class SwitchState
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
            LBN确定 = new ReaLTaiizor.Controls.LostButton();
            RBN初始化 = new RadioButton();
            RBN温度一 = new RadioButton();
            RBN温度二 = new RadioButton();
            RBN温度三 = new RadioButton();
            SuspendLayout();
            // 
            // LBN确定
            // 
            LBN确定.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            LBN确定.BackColor = Color.Gray;
            LBN确定.Font = new Font("Segoe UI", 9F);
            LBN确定.ForeColor = Color.White;
            LBN确定.HoverColor = Color.DodgerBlue;
            LBN确定.Image = null;
            LBN确定.Location = new Point(708, 408);
            LBN确定.Name = "LBN确定";
            LBN确定.Size = new Size(80, 30);
            LBN确定.TabIndex = 0;
            LBN确定.Text = "确定";
            LBN确定.Click += LBN确定_Click;
            // 
            // RBN初始化
            // 
            RBN初始化.AutoSize = true;
            RBN初始化.Checked = true;
            RBN初始化.Location = new Point(70, 60);
            RBN初始化.Name = "RBN初始化";
            RBN初始化.Size = new Size(62, 21);
            RBN初始化.TabIndex = 1;
            RBN初始化.TabStop = true;
            RBN初始化.Tag = "Init";
            RBN初始化.Text = "初始化";
            RBN初始化.UseVisualStyleBackColor = true;
            RBN初始化.CheckedChanged += RB状态更改_CheckedChanged;
            // 
            // RBN温度一
            // 
            RBN温度一.AutoSize = true;
            RBN温度一.Location = new Point(150, 60);
            RBN温度一.Name = "RBN温度一";
            RBN温度一.Size = new Size(62, 21);
            RBN温度一.TabIndex = 2;
            RBN温度一.Tag = "T1";
            RBN温度一.Text = "温度一";
            RBN温度一.UseVisualStyleBackColor = true;
            RBN温度一.CheckedChanged += RB状态更改_CheckedChanged;
            // 
            // RBN温度二
            // 
            RBN温度二.AutoSize = true;
            RBN温度二.Location = new Point(230, 60);
            RBN温度二.Name = "RBN温度二";
            RBN温度二.Size = new Size(62, 21);
            RBN温度二.TabIndex = 3;
            RBN温度二.Tag = "T2";
            RBN温度二.Text = "温度二";
            RBN温度二.UseVisualStyleBackColor = true;
            RBN温度二.CheckedChanged += RB状态更改_CheckedChanged;
            // 
            // RBN温度三
            // 
            RBN温度三.AutoSize = true;
            RBN温度三.Location = new Point(310, 60);
            RBN温度三.Name = "RBN温度三";
            RBN温度三.Size = new Size(62, 21);
            RBN温度三.TabIndex = 4;
            RBN温度三.Tag = "T3";
            RBN温度三.Text = "温度三";
            RBN温度三.UseVisualStyleBackColor = true;
            RBN温度三.CheckedChanged += RB状态更改_CheckedChanged;
            // 
            // SwitchState
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightGray;
            ClientSize = new Size(800, 450);
            Controls.Add(RBN温度三);
            Controls.Add(RBN温度二);
            Controls.Add(RBN温度一);
            Controls.Add(RBN初始化);
            Controls.Add(LBN确定);
            Name = "SwitchState";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "状态切换";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ReaLTaiizor.Controls.LostButton LBN确定;
        private RadioButton RBN初始化;
        private RadioButton RBN温度一;
        private RadioButton RBN温度二;
        private RadioButton RBN温度三;
    }
}