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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            HRTB信息 = new ReaLTaiizor.Controls.HopeRichTextBox();
            ATP主选项卡 = new ReaLTaiizor.Controls.AirTabPage();
            TP操作 = new TabPage();
            label1 = new Label();
            LBN断电 = new ReaLTaiizor.Controls.LostButton();
            LBN暂停 = new ReaLTaiizor.Controls.LostButton();
            LBN切换 = new ReaLTaiizor.Controls.LostButton();
            LBN运行 = new ReaLTaiizor.Controls.LostButton();
            TP查看 = new TabPage();
            GB结果 = new GroupBox();
            BGWRun = new System.ComponentModel.BackgroundWorker();
            HF压力标定 = new ReaLTaiizor.Forms.HopeForm();
            TS设置 = new ToolStrip();
            TSB设置 = new ToolStripButton();
            TDB窗口 = new ToolStripDropDownButton();
            TMI测试 = new ToolStripMenuItem();
            TMI监视 = new ToolStripMenuItem();
            TMI芯片 = new ToolStripMenuItem();
            TDB导出 = new ToolStripDropDownButton();
            TMI导出Excel = new ToolStripMenuItem();
            TMI导出数据 = new ToolStripMenuItem();
            TSB清除 = new ToolStripButton();
            ATP主选项卡.SuspendLayout();
            TP操作.SuspendLayout();
            TP查看.SuspendLayout();
            TS设置.SuspendLayout();
            SuspendLayout();
            // 
            // HRTB信息
            // 
            HRTB信息.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            HRTB信息.BorderColor = Color.FromArgb(220, 223, 230);
            HRTB信息.Font = new Font("Segoe UI", 8F);
            HRTB信息.ForeColor = Color.FromArgb(48, 49, 51);
            HRTB信息.Hint = "";
            HRTB信息.HoverBorderColor = Color.FromArgb(64, 158, 255);
            HRTB信息.Location = new Point(5, 88);
            HRTB信息.Margin = new Padding(2, 3, 2, 3);
            HRTB信息.MaxLength = 32767;
            HRTB信息.Multiline = true;
            HRTB信息.Name = "HRTB信息";
            HRTB信息.PasswordChar = '\0';
            HRTB信息.ScrollBars = ScrollBars.Vertical;
            HRTB信息.SelectedText = "";
            HRTB信息.SelectionLength = 0;
            HRTB信息.SelectionStart = 0;
            HRTB信息.Size = new Size(224, 244);
            HRTB信息.TabIndex = 5;
            HRTB信息.TabStop = false;
            HRTB信息.UseSystemPasswordChar = false;
            // 
            // ATP主选项卡
            // 
            ATP主选项卡.Alignment = TabAlignment.Left;
            ATP主选项卡.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ATP主选项卡.BaseColor = Color.DimGray;
            ATP主选项卡.Controls.Add(TP操作);
            ATP主选项卡.Controls.Add(TP查看);
            ATP主选项卡.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold);
            ATP主选项卡.ItemSize = new Size(50, 120);
            ATP主选项卡.Location = new Point(0, 40);
            ATP主选项卡.Margin = new Padding(0);
            ATP主选项卡.Multiline = true;
            ATP主选项卡.Name = "ATP主选项卡";
            ATP主选项卡.NormalTextColor = Color.DarkGray;
            ATP主选项卡.Padding = new Point(3, 3);
            ATP主选项卡.SelectedIndex = 0;
            ATP主选项卡.SelectedTabBackColor = Color.White;
            ATP主选项卡.SelectedTextColor = Color.White;
            ATP主选项卡.ShowOuterBorders = false;
            ATP主选项卡.Size = new Size(1154, 721);
            ATP主选项卡.SizeMode = TabSizeMode.Fixed;
            ATP主选项卡.SquareColor = Color.DodgerBlue;
            ATP主选项卡.TabCursor = Cursors.Hand;
            ATP主选项卡.TabIndex = 7;
            // 
            // TP操作
            // 
            TP操作.BackColor = Color.White;
            TP操作.Controls.Add(label1);
            TP操作.Controls.Add(LBN断电);
            TP操作.Controls.Add(LBN暂停);
            TP操作.Controls.Add(LBN切换);
            TP操作.Controls.Add(LBN运行);
            TP操作.Controls.Add(HRTB信息);
            TP操作.Location = new Point(124, 4);
            TP操作.Name = "TP操作";
            TP操作.Padding = new Padding(3);
            TP操作.Size = new Size(1026, 713);
            TP操作.TabIndex = 0;
            TP操作.Text = "操作";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 20F, FontStyle.Bold);
            label1.Location = new Point(418, 24);
            label1.Name = "label1";
            label1.Size = new Size(231, 36);
            label1.TabIndex = 10;
            label1.Text = "压力标定测试软件";
            // 
            // LBN断电
            // 
            LBN断电.Anchor = AnchorStyles.Bottom;
            LBN断电.BackColor = Color.Gray;
            LBN断电.Font = new Font("Segoe UI", 9F);
            LBN断电.ForeColor = Color.White;
            LBN断电.HoverColor = Color.DodgerBlue;
            LBN断电.Image = null;
            LBN断电.Location = new Point(698, 612);
            LBN断电.Name = "LBN断电";
            LBN断电.Size = new Size(90, 40);
            LBN断电.TabIndex = 9;
            LBN断电.Text = "断电";
            // 
            // LBN暂停
            // 
            LBN暂停.Anchor = AnchorStyles.Bottom;
            LBN暂停.BackColor = Color.Gray;
            LBN暂停.Font = new Font("Segoe UI", 9F);
            LBN暂停.ForeColor = Color.White;
            LBN暂停.HoverColor = Color.DodgerBlue;
            LBN暂停.Image = null;
            LBN暂停.Location = new Point(418, 612);
            LBN暂停.Name = "LBN暂停";
            LBN暂停.Size = new Size(90, 40);
            LBN暂停.TabIndex = 8;
            LBN暂停.Text = "暂停";
            LBN暂停.Click += LBN暂停_Click;
            // 
            // LBN切换
            // 
            LBN切换.Anchor = AnchorStyles.Bottom;
            LBN切换.BackColor = Color.Gray;
            LBN切换.Font = new Font("Segoe UI", 9F);
            LBN切换.ForeColor = Color.White;
            LBN切换.HoverColor = Color.DodgerBlue;
            LBN切换.Image = null;
            LBN切换.Location = new Point(558, 612);
            LBN切换.Name = "LBN切换";
            LBN切换.Size = new Size(90, 40);
            LBN切换.TabIndex = 7;
            LBN切换.Text = "切换";
            LBN切换.Click += LBN其他_Click;
            // 
            // LBN运行
            // 
            LBN运行.Anchor = AnchorStyles.Bottom;
            LBN运行.BackColor = Color.Gray;
            LBN运行.Font = new Font("Segoe UI", 9F);
            LBN运行.ForeColor = Color.White;
            LBN运行.HoverColor = Color.DodgerBlue;
            LBN运行.Image = null;
            LBN运行.Location = new Point(278, 612);
            LBN运行.Name = "LBN运行";
            LBN运行.Size = new Size(90, 40);
            LBN运行.TabIndex = 6;
            LBN运行.Text = "运行";
            LBN运行.Click += LBN运行_Click;
            // 
            // TP查看
            // 
            TP查看.BackColor = Color.White;
            TP查看.Controls.Add(GB结果);
            TP查看.Location = new Point(124, 4);
            TP查看.Name = "TP查看";
            TP查看.Padding = new Padding(3);
            TP查看.Size = new Size(1026, 713);
            TP查看.TabIndex = 1;
            TP查看.Text = "查看";
            // 
            // GB结果
            // 
            GB结果.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GB结果.Location = new Point(220, 6);
            GB结果.Name = "GB结果";
            GB结果.Size = new Size(430, 386);
            GB结果.TabIndex = 0;
            GB结果.TabStop = false;
            GB结果.Text = "Result";
            // 
            // HF压力标定
            // 
            HF压力标定.ControlBoxColorH = Color.FromArgb(228, 231, 237);
            HF压力标定.ControlBoxColorHC = Color.FromArgb(245, 108, 108);
            HF压力标定.ControlBoxColorN = Color.White;
            HF压力标定.Dock = DockStyle.Top;
            HF压力标定.Font = new Font("Segoe UI", 12F);
            HF压力标定.ForeColor = Color.FromArgb(242, 246, 252);
            HF压力标定.Image = Properties.Resources.压力标校;
            HF压力标定.Location = new Point(0, 0);
            HF压力标定.Margin = new Padding(0);
            HF压力标定.Name = "HF压力标定";
            HF压力标定.Size = new Size(1184, 40);
            HF压力标定.TabIndex = 1;
            HF压力标定.Text = "压力标定";
            HF压力标定.ThemeColor = Color.DimGray;
            // 
            // TS设置
            // 
            TS设置.Dock = DockStyle.Right;
            TS设置.GripStyle = ToolStripGripStyle.Hidden;
            TS设置.Items.AddRange(new ToolStripItem[] { TSB设置, TDB窗口, TDB导出, TSB清除 });
            TS设置.Location = new Point(1154, 40);
            TS设置.Name = "TS设置";
            TS设置.Size = new Size(30, 721);
            TS设置.TabIndex = 8;
            TS设置.Text = "toolStrip1";
            // 
            // TSB设置
            // 
            TSB设置.Alignment = ToolStripItemAlignment.Right;
            TSB设置.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TSB设置.Image = Properties.Resources.baseline_build_black_24dp;
            TSB设置.ImageTransparentColor = Color.Magenta;
            TSB设置.Name = "TSB设置";
            TSB设置.Size = new Size(29, 20);
            TSB设置.Tag = "setting";
            TSB设置.Text = "toolStripButton1";
            TSB设置.Click += TMI窗口_Click;
            // 
            // TDB窗口
            // 
            TDB窗口.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TDB窗口.DropDownItems.AddRange(new ToolStripItem[] { TMI测试, TMI监视, TMI芯片 });
            TDB窗口.Image = Properties.Resources.application_16x;
            TDB窗口.ImageTransparentColor = Color.Magenta;
            TDB窗口.Name = "TDB窗口";
            TDB窗口.Size = new Size(29, 20);
            TDB窗口.Text = "窗口";
            // 
            // TMI测试
            // 
            TMI测试.Name = "TMI测试";
            TMI测试.Size = new Size(100, 22);
            TMI测试.Tag = "test";
            TMI测试.Text = "测试";
            TMI测试.Click += TMI窗口_Click;
            // 
            // TMI监视
            // 
            TMI监视.Name = "TMI监视";
            TMI监视.Size = new Size(100, 22);
            TMI监视.Tag = "monitor";
            TMI监视.Text = "监视";
            TMI监视.Click += TMI窗口_Click;
            // 
            // TMI芯片
            // 
            TMI芯片.Name = "TMI芯片";
            TMI芯片.Size = new Size(100, 22);
            TMI芯片.Tag = "chip";
            TMI芯片.Text = "芯片";
            TMI芯片.Click += TMI窗口_Click;
            // 
            // TDB导出
            // 
            TDB导出.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TDB导出.DropDownItems.AddRange(new ToolStripItem[] { TMI导出Excel, TMI导出数据 });
            TDB导出.Image = (Image)resources.GetObject("TDB导出.Image");
            TDB导出.ImageTransparentColor = Color.Magenta;
            TDB导出.Name = "TDB导出";
            TDB导出.Size = new Size(27, 20);
            TDB导出.Text = "导出";
            // 
            // TMI导出Excel
            // 
            TMI导出Excel.Name = "TMI导出Excel";
            TMI导出Excel.Size = new Size(129, 22);
            TMI导出Excel.Tag = "excel";
            TMI导出Excel.Text = "导出Excel";
            TMI导出Excel.Click += TMI导出_Click;
            // 
            // TMI导出数据
            // 
            TMI导出数据.Name = "TMI导出数据";
            TMI导出数据.Size = new Size(129, 22);
            TMI导出数据.Tag = "data";
            TMI导出数据.Text = "导出数据";
            TMI导出数据.Click += TMI导出_Click;
            // 
            // TSB清除
            // 
            TSB清除.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TSB清除.Image = (Image)resources.GetObject("TSB清除.Image");
            TSB清除.ImageTransparentColor = Color.Magenta;
            TSB清除.Name = "TSB清除";
            TSB清除.Size = new Size(29, 20);
            TSB清除.Text = "toolStripButton1";
            TSB清除.Click += TMI清除_Click;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.LightGray;
            ClientSize = new Size(1184, 761);
            Controls.Add(TS设置);
            Controls.Add(HF压力标定);
            Controls.Add(ATP主选项卡);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2, 3, 2, 3);
            MaximumSize = new Size(1280, 984);
            MinimumSize = new Size(190, 40);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "压力标定";
            ATP主选项卡.ResumeLayout(false);
            TP操作.ResumeLayout(false);
            TP操作.PerformLayout();
            TP查看.ResumeLayout(false);
            TS设置.ResumeLayout(false);
            TS设置.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ReaLTaiizor.Controls.HopeRichTextBox HRTB信息;
        private ReaLTaiizor.Controls.AirTabPage ATP主选项卡;
        private TabPage TP操作;
        private TabPage TP查看;
        private ReaLTaiizor.Controls.LostButton LBN运行;
        private System.ComponentModel.BackgroundWorker BGWRun;
        private ReaLTaiizor.Controls.LostButton LBN切换;
        private ReaLTaiizor.Controls.LostButton LBN断电;
        private ReaLTaiizor.Controls.LostButton LBN暂停;
        private GroupBox GB结果;
        private Label label1;
        private ReaLTaiizor.Forms.HopeForm HF压力标定;
        private ToolStrip TS设置;
        private ToolStripButton TSB设置;
        private ToolStripDropDownButton TDB窗口;
        private ToolStripMenuItem TMI测试;
        private ToolStripMenuItem TMI监视;
        private ToolStripDropDownButton TDB导出;
        private ToolStripMenuItem TMI导出Excel;
        private ToolStripMenuItem TMI导出数据;
        private ToolStripMenuItem TMI芯片;
        private ToolStripButton TSB清除;
    }
}
