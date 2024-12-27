namespace PressureCalibration
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            RTB信息 = new RichTextBox();
            MS菜单 = new MenuStrip();
            TMI设置 = new ToolStripMenuItem();
            TMI采集参数设置 = new ToolStripMenuItem();
            TMI采集卡设置 = new ToolStripMenuItem();
            TMI压控设置 = new ToolStripMenuItem();
            TMI温控设置 = new ToolStripMenuItem();
            TMI窗口 = new ToolStripMenuItem();
            TMI监控 = new ToolStripMenuItem();
            TMI测试 = new ToolStripMenuItem();
            TMI切换 = new ToolStripMenuItem();
            MS菜单.SuspendLayout();
            SuspendLayout();
            // 
            // RTB信息
            // 
            RTB信息.Location = new Point(60, 42);
            RTB信息.Name = "RTB信息";
            RTB信息.Size = new Size(654, 377);
            RTB信息.TabIndex = 0;
            RTB信息.Text = "";
            // 
            // MS菜单
            // 
            MS菜单.BackColor = Color.Transparent;
            MS菜单.Items.AddRange(new ToolStripItem[] { TMI设置, TMI窗口, TMI测试, TMI切换 });
            MS菜单.Location = new Point(0, 0);
            MS菜单.Name = "MS菜单";
            MS菜单.Size = new Size(800, 25);
            MS菜单.TabIndex = 1;
            MS菜单.Text = "menuStrip1";
            // 
            // TMI设置
            // 
            TMI设置.Alignment = ToolStripItemAlignment.Right;
            TMI设置.DropDownItems.AddRange(new ToolStripItem[] { TMI采集参数设置, TMI采集卡设置, TMI压控设置, TMI温控设置 });
            TMI设置.Name = "TMI设置";
            TMI设置.Size = new Size(44, 21);
            TMI设置.Text = "设置";
            // 
            // TMI采集参数设置
            // 
            TMI采集参数设置.Name = "TMI采集参数设置";
            TMI采集参数设置.Size = new Size(148, 22);
            TMI采集参数设置.Tag = "APara";
            TMI采集参数设置.Text = "采集参数设置";
            TMI采集参数设置.Click += TMI设置_Click;
            // 
            // TMI采集卡设置
            // 
            TMI采集卡设置.Name = "TMI采集卡设置";
            TMI采集卡设置.Size = new Size(148, 22);
            TMI采集卡设置.Tag = "A";
            TMI采集卡设置.Text = "采集卡设置";
            TMI采集卡设置.Click += TMI设置_Click;
            // 
            // TMI压控设置
            // 
            TMI压控设置.Name = "TMI压控设置";
            TMI压控设置.Size = new Size(148, 22);
            TMI压控设置.Tag = "P";
            TMI压控设置.Text = "压控设置";
            TMI压控设置.Click += TMI设置_Click;
            // 
            // TMI温控设置
            // 
            TMI温控设置.Name = "TMI温控设置";
            TMI温控设置.Size = new Size(148, 22);
            TMI温控设置.Tag = "T";
            TMI温控设置.Text = "温控设置";
            TMI温控设置.Click += TMI设置_Click;
            // 
            // TMI窗口
            // 
            TMI窗口.DropDownItems.AddRange(new ToolStripItem[] { TMI监控 });
            TMI窗口.Name = "TMI窗口";
            TMI窗口.Size = new Size(44, 21);
            TMI窗口.Text = "窗口";
            // 
            // TMI监控
            // 
            TMI监控.Name = "TMI监控";
            TMI监控.Size = new Size(100, 22);
            TMI监控.Text = "监控";
            TMI监控.Click += TMI监控_Click;
            // 
            // TMI测试
            // 
            TMI测试.Name = "TMI测试";
            TMI测试.Size = new Size(44, 21);
            TMI测试.Text = "测试";
            TMI测试.Click += TMI测试_Click;
            // 
            // TMI切换
            // 
            TMI切换.Name = "TMI切换";
            TMI切换.Size = new Size(44, 21);
            TMI切换.Text = "切换";
            TMI切换.Click += TMI切换_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(RTB信息);
            Controls.Add(MS菜单);
            MainMenuStrip = MS菜单;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "压力标定";
            FormClosing += MainForm_FormClosing;
            MS菜单.ResumeLayout(false);
            MS菜单.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox RTB信息;
        private MenuStrip MS菜单;
        private ToolStripMenuItem TMI设置;
        private ToolStripMenuItem TMI采集参数设置;
        private ToolStripMenuItem TMI采集卡设置;
        private ToolStripMenuItem TMI压控设置;
        private ToolStripMenuItem TMI温控设置;
        private ToolStripMenuItem TMI窗口;
        private ToolStripMenuItem TMI监控;
        private ToolStripMenuItem TMI测试;
        private ToolStripMenuItem TMI切换;
    }
}
