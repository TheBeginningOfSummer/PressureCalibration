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
            MS菜单 = new MenuStrip();
            TMI设置 = new ToolStripMenuItem();
            TMI窗口 = new ToolStripMenuItem();
            TMI数据监视 = new ToolStripMenuItem();
            TMI测试 = new ToolStripMenuItem();
            TMI导出 = new ToolStripMenuItem();
            TMI导出Excel = new ToolStripMenuItem();
            TMI导出数据 = new ToolStripMenuItem();
            TMI清除 = new ToolStripMenuItem();
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
            MS菜单.SuspendLayout();
            ATP主选项卡.SuspendLayout();
            TP操作.SuspendLayout();
            TP查看.SuspendLayout();
            SuspendLayout();
            // 
            // MS菜单
            // 
            MS菜单.BackColor = Color.DimGray;
            MS菜单.Font = new Font("Microsoft YaHei UI", 11F);
            MS菜单.Items.AddRange(new ToolStripItem[] { TMI设置, TMI窗口, TMI导出, TMI清除 });
            MS菜单.Location = new Point(0, 0);
            MS菜单.Name = "MS菜单";
            MS菜单.Padding = new Padding(4, 2, 0, 2);
            MS菜单.Size = new Size(1184, 28);
            MS菜单.TabIndex = 1;
            MS菜单.Text = "菜单";
            // 
            // TMI设置
            // 
            TMI设置.Alignment = ToolStripItemAlignment.Right;
            TMI设置.Image = Properties.Resources.baseline_build_black_24dp;
            TMI设置.Name = "TMI设置";
            TMI设置.Size = new Size(67, 24);
            TMI设置.Text = "设置";
            TMI设置.Click += TMI设置_Click;
            // 
            // TMI窗口
            // 
            TMI窗口.BackColor = Color.DimGray;
            TMI窗口.DropDownItems.AddRange(new ToolStripItem[] { TMI数据监视, TMI测试 });
            TMI窗口.ForeColor = Color.Black;
            TMI窗口.Image = Properties.Resources.application_16x;
            TMI窗口.Name = "TMI窗口";
            TMI窗口.Size = new Size(67, 24);
            TMI窗口.Text = "窗口";
            // 
            // TMI数据监视
            // 
            TMI数据监视.BackColor = SystemColors.Control;
            TMI数据监视.Name = "TMI数据监视";
            TMI数据监视.Size = new Size(138, 24);
            TMI数据监视.Text = "数据监视";
            TMI数据监视.Click += TMI窗口_Click;
            // 
            // TMI测试
            // 
            TMI测试.Name = "TMI测试";
            TMI测试.Size = new Size(138, 24);
            TMI测试.Text = "测试";
            TMI测试.Click += TMI测试_Click;
            // 
            // TMI导出
            // 
            TMI导出.Alignment = ToolStripItemAlignment.Right;
            TMI导出.DropDownItems.AddRange(new ToolStripItem[] { TMI导出Excel, TMI导出数据 });
            TMI导出.Image = Properties.Resources.Collection_16xLG;
            TMI导出.Name = "TMI导出";
            TMI导出.Size = new Size(67, 24);
            TMI导出.Text = "导出";
            // 
            // TMI导出Excel
            // 
            TMI导出Excel.Name = "TMI导出Excel";
            TMI导出Excel.Size = new Size(145, 24);
            TMI导出Excel.Tag = "Excel";
            TMI导出Excel.Text = "导出Excel";
            TMI导出Excel.Click += TMI导出_Click;
            // 
            // TMI导出数据
            // 
            TMI导出数据.Name = "TMI导出数据";
            TMI导出数据.Size = new Size(145, 24);
            TMI导出数据.Tag = "Data";
            TMI导出数据.Text = "导出数据";
            TMI导出数据.Click += TMI导出_Click;
            // 
            // TMI清除
            // 
            TMI清除.Alignment = ToolStripItemAlignment.Right;
            TMI清除.Image = Properties.Resources.Close_16xLG;
            TMI清除.Name = "TMI清除";
            TMI清除.Size = new Size(67, 24);
            TMI清除.Text = "清除";
            TMI清除.Click += TMI清除_Click;
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
            HRTB信息.Size = new Size(254, 256);
            HRTB信息.TabIndex = 5;
            HRTB信息.TabStop = false;
            HRTB信息.UseSystemPasswordChar = false;
            // 
            // ATP主选项卡
            // 
            ATP主选项卡.Alignment = TabAlignment.Left;
            ATP主选项卡.BaseColor = Color.DimGray;
            ATP主选项卡.Controls.Add(TP操作);
            ATP主选项卡.Controls.Add(TP查看);
            ATP主选项卡.Dock = DockStyle.Fill;
            ATP主选项卡.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold);
            ATP主选项卡.ItemSize = new Size(50, 120);
            ATP主选项卡.Location = new Point(0, 28);
            ATP主选项卡.Multiline = true;
            ATP主选项卡.Name = "ATP主选项卡";
            ATP主选项卡.NormalTextColor = Color.DarkGray;
            ATP主选项卡.Padding = new Point(3, 3);
            ATP主选项卡.SelectedIndex = 0;
            ATP主选项卡.SelectedTabBackColor = Color.White;
            ATP主选项卡.SelectedTextColor = Color.White;
            ATP主选项卡.ShowOuterBorders = false;
            ATP主选项卡.Size = new Size(1184, 733);
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
            TP操作.Size = new Size(1056, 725);
            TP操作.TabIndex = 0;
            TP操作.Text = "操作";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 20F, FontStyle.Bold);
            label1.Location = new Point(433, 24);
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
            LBN断电.Location = new Point(713, 624);
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
            LBN暂停.Location = new Point(433, 624);
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
            LBN切换.Location = new Point(573, 624);
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
            LBN运行.Location = new Point(293, 624);
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
            TP查看.Size = new Size(1056, 725);
            TP查看.TabIndex = 1;
            TP查看.Text = "查看";
            // 
            // GB结果
            // 
            GB结果.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GB结果.Location = new Point(220, 6);
            GB结果.Name = "GB结果";
            GB结果.Size = new Size(460, 398);
            GB结果.TabIndex = 0;
            GB结果.TabStop = false;
            GB结果.Text = "Result";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightGray;
            ClientSize = new Size(1184, 761);
            Controls.Add(ATP主选项卡);
            Controls.Add(MS菜单);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = MS菜单;
            Margin = new Padding(2, 3, 2, 3);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "压力标定";
            MS菜单.ResumeLayout(false);
            MS菜单.PerformLayout();
            ATP主选项卡.ResumeLayout(false);
            TP操作.ResumeLayout(false);
            TP操作.PerformLayout();
            TP查看.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MenuStrip MS菜单;
        private ToolStripMenuItem TMI设置;
        private ToolStripMenuItem TMI窗口;
        private ToolStripMenuItem TMI数据监视;
        private ReaLTaiizor.Controls.HopeRichTextBox HRTB信息;
        private ReaLTaiizor.Controls.AirTabPage ATP主选项卡;
        private TabPage TP操作;
        private TabPage TP查看;
        private ReaLTaiizor.Controls.LostButton LBN运行;
        private System.ComponentModel.BackgroundWorker BGWRun;
        private ReaLTaiizor.Controls.LostButton LBN切换;
        private ToolStripMenuItem TMI导出;
        private ToolStripMenuItem TMI导出Excel;
        private ToolStripMenuItem TMI导出数据;
        private ToolStripMenuItem TMI清除;
        private ReaLTaiizor.Controls.LostButton LBN断电;
        private ReaLTaiizor.Controls.LostButton LBN暂停;
        private ToolStripMenuItem TMI测试;
        private GroupBox GB结果;
        private Label label1;
    }
}
