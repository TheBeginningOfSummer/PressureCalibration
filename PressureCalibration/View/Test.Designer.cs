namespace PressureCalibration.View
{
    partial class Test
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
            HTP测试 = new ReaLTaiizor.Controls.HopeTabPage();
            TP温度测试 = new TabPage();
            GPB温度分布 = new GroupBox();
            PN温度分布 = new Panel();
            richTextBox1 = new RichTextBox();
            MS温度测试 = new MenuStrip();
            设备ToolStripMenuItem = new ToolStripMenuItem();
            TMI连接采集卡 = new ToolStripMenuItem();
            TMI断开采集卡 = new ToolStripMenuItem();
            TMI连接温控 = new ToolStripMenuItem();
            TMI断开温控 = new ToolStripMenuItem();
            采集间隔ToolStripMenuItem = new ToolStripMenuItem();
            TTB温度采集间隔 = new ToolStripTextBox();
            sToolStripMenuItem = new ToolStripMenuItem();
            偏差温度ToolStripMenuItem = new ToolStripMenuItem();
            TTB偏差温度 = new ToolStripTextBox();
            toolStripMenuItem1 = new ToolStripMenuItem();
            目标温度ToolStripMenuItem = new ToolStripMenuItem();
            TTB目标温度 = new ToolStripTextBox();
            设置温度ToolStripMenuItem = new ToolStripMenuItem();
            TTB设置温度 = new ToolStripTextBox();
            TMI温度设置 = new ToolStripMenuItem();
            TMI温度采集 = new ToolStripMenuItem();
            采集一次ToolStripMenuItem = new ToolStripMenuItem();
            开始采集ToolStripMenuItem = new ToolStripMenuItem();
            清除数据ToolStripMenuItem = new ToolStripMenuItem();
            停止采集ToolStripMenuItem = new ToolStripMenuItem();
            导出表格ToolStripMenuItem = new ToolStripMenuItem();
            保存图片ToolStripMenuItem = new ToolStripMenuItem();
            TTB温度测试名称 = new ToolStripTextBox();
            温度测试名称 = new ToolStripMenuItem();
            TP压力测试 = new TabPage();
            MS压力测试 = new MenuStrip();
            设备ToolStripMenuItem1 = new ToolStripMenuItem();
            采集间隔ToolStripMenuItem1 = new ToolStripMenuItem();
            TTB压力采集间隔 = new ToolStripTextBox();
            sToolStripMenuItem1 = new ToolStripMenuItem();
            目标压力ToolStripMenuItem = new ToolStripMenuItem();
            TTB目标压力 = new ToolStripTextBox();
            paToolStripMenuItem = new ToolStripMenuItem();
            TMI压力设置 = new ToolStripMenuItem();
            TMI压力采集 = new ToolStripMenuItem();
            TTB压力测试名称 = new ToolStripTextBox();
            测试名称ToolStripMenuItem = new ToolStripMenuItem();
            HTP测试.SuspendLayout();
            TP温度测试.SuspendLayout();
            GPB温度分布.SuspendLayout();
            MS温度测试.SuspendLayout();
            TP压力测试.SuspendLayout();
            MS压力测试.SuspendLayout();
            SuspendLayout();
            // 
            // HTP测试
            // 
            HTP测试.BaseColor = Color.White;
            HTP测试.Controls.Add(TP温度测试);
            HTP测试.Controls.Add(TP压力测试);
            HTP测试.Dock = DockStyle.Fill;
            HTP测试.Font = new Font("Segoe UI", 12F);
            HTP测试.ForeColorA = Color.Silver;
            HTP测试.ForeColorB = Color.Gray;
            HTP测试.ForeColorC = Color.DodgerBlue;
            HTP测试.ItemSize = new Size(120, 40);
            HTP测试.Location = new Point(0, 0);
            HTP测试.Name = "HTP测试";
            HTP测试.PixelOffsetType = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            HTP测试.SelectedIndex = 0;
            HTP测试.Size = new Size(984, 641);
            HTP测试.SizeMode = TabSizeMode.Fixed;
            HTP测试.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            HTP测试.TabIndex = 0;
            HTP测试.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            HTP测试.ThemeColorA = Color.FromArgb(64, 158, 255);
            HTP测试.ThemeColorB = Color.FromArgb(150, 64, 158, 255);
            HTP测试.TitleTextState = ReaLTaiizor.Controls.HopeTabPage.TextState.Normal;
            // 
            // TP温度测试
            // 
            TP温度测试.Controls.Add(GPB温度分布);
            TP温度测试.Controls.Add(richTextBox1);
            TP温度测试.Controls.Add(MS温度测试);
            TP温度测试.Location = new Point(0, 40);
            TP温度测试.Name = "TP温度测试";
            TP温度测试.Padding = new Padding(3);
            TP温度测试.Size = new Size(984, 601);
            TP温度测试.TabIndex = 0;
            TP温度测试.Text = "温度测试";
            TP温度测试.UseVisualStyleBackColor = true;
            // 
            // GPB温度分布
            // 
            GPB温度分布.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            GPB温度分布.Controls.Add(PN温度分布);
            GPB温度分布.Location = new Point(421, 33);
            GPB温度分布.Name = "GPB温度分布";
            GPB温度分布.Size = new Size(557, 564);
            GPB温度分布.TabIndex = 2;
            GPB温度分布.TabStop = false;
            GPB温度分布.Text = "温度分布（℃）";
            // 
            // PN温度分布
            // 
            PN温度分布.Dock = DockStyle.Fill;
            PN温度分布.Location = new Point(3, 25);
            PN温度分布.Name = "PN温度分布";
            PN温度分布.Size = new Size(551, 536);
            PN温度分布.TabIndex = 0;
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBox1.BackColor = Color.Silver;
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.Location = new Point(4, 31);
            richTextBox1.Margin = new Padding(1);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(413, 566);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            // 
            // MS温度测试
            // 
            MS温度测试.Items.AddRange(new ToolStripItem[] { 设备ToolStripMenuItem, 采集间隔ToolStripMenuItem, TTB温度采集间隔, sToolStripMenuItem, 偏差温度ToolStripMenuItem, TTB偏差温度, toolStripMenuItem1, 目标温度ToolStripMenuItem, TTB目标温度, 设置温度ToolStripMenuItem, TTB设置温度, TMI温度设置, TMI温度采集, TTB温度测试名称, 温度测试名称 });
            MS温度测试.Location = new Point(3, 3);
            MS温度测试.Name = "MS温度测试";
            MS温度测试.Size = new Size(978, 27);
            MS温度测试.TabIndex = 0;
            MS温度测试.Text = "menuStrip1";
            // 
            // 设备ToolStripMenuItem
            // 
            设备ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { TMI连接采集卡, TMI断开采集卡, TMI连接温控, TMI断开温控 });
            设备ToolStripMenuItem.Name = "设备ToolStripMenuItem";
            设备ToolStripMenuItem.Size = new Size(44, 23);
            设备ToolStripMenuItem.Text = "设备";
            // 
            // TMI连接采集卡
            // 
            TMI连接采集卡.Name = "TMI连接采集卡";
            TMI连接采集卡.Size = new Size(136, 22);
            TMI连接采集卡.Text = "连接采集卡";
            // 
            // TMI断开采集卡
            // 
            TMI断开采集卡.Name = "TMI断开采集卡";
            TMI断开采集卡.Size = new Size(136, 22);
            TMI断开采集卡.Text = "断开采集卡";
            // 
            // TMI连接温控
            // 
            TMI连接温控.Name = "TMI连接温控";
            TMI连接温控.Size = new Size(136, 22);
            TMI连接温控.Text = "连接温控";
            // 
            // TMI断开温控
            // 
            TMI断开温控.Name = "TMI断开温控";
            TMI断开温控.Size = new Size(136, 22);
            TMI断开温控.Text = "断开温控";
            // 
            // 采集间隔ToolStripMenuItem
            // 
            采集间隔ToolStripMenuItem.Name = "采集间隔ToolStripMenuItem";
            采集间隔ToolStripMenuItem.Size = new Size(68, 23);
            采集间隔ToolStripMenuItem.Text = "采集间隔";
            // 
            // TTB温度采集间隔
            // 
            TTB温度采集间隔.Name = "TTB温度采集间隔";
            TTB温度采集间隔.Size = new Size(30, 23);
            TTB温度采集间隔.Text = "5";
            // 
            // sToolStripMenuItem
            // 
            sToolStripMenuItem.Name = "sToolStripMenuItem";
            sToolStripMenuItem.Size = new Size(27, 23);
            sToolStripMenuItem.Text = "S";
            // 
            // 偏差温度ToolStripMenuItem
            // 
            偏差温度ToolStripMenuItem.Name = "偏差温度ToolStripMenuItem";
            偏差温度ToolStripMenuItem.Size = new Size(68, 23);
            偏差温度ToolStripMenuItem.Text = "偏差温度";
            // 
            // TTB偏差温度
            // 
            TTB偏差温度.Name = "TTB偏差温度";
            TTB偏差温度.Size = new Size(30, 23);
            TTB偏差温度.Text = "1";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(32, 23);
            toolStripMenuItem1.Text = "℃";
            // 
            // 目标温度ToolStripMenuItem
            // 
            目标温度ToolStripMenuItem.Name = "目标温度ToolStripMenuItem";
            目标温度ToolStripMenuItem.Size = new Size(68, 23);
            目标温度ToolStripMenuItem.Text = "目标温度";
            // 
            // TTB目标温度
            // 
            TTB目标温度.Name = "TTB目标温度";
            TTB目标温度.Size = new Size(30, 23);
            TTB目标温度.Text = "15";
            // 
            // 设置温度ToolStripMenuItem
            // 
            设置温度ToolStripMenuItem.Name = "设置温度ToolStripMenuItem";
            设置温度ToolStripMenuItem.Size = new Size(68, 23);
            设置温度ToolStripMenuItem.Text = "设置温度";
            // 
            // TTB设置温度
            // 
            TTB设置温度.Name = "TTB设置温度";
            TTB设置温度.Size = new Size(30, 23);
            TTB设置温度.Text = "15";
            // 
            // TMI温度设置
            // 
            TMI温度设置.Name = "TMI温度设置";
            TMI温度设置.Size = new Size(44, 23);
            TMI温度设置.Text = "设置";
            // 
            // TMI温度采集
            // 
            TMI温度采集.Alignment = ToolStripItemAlignment.Right;
            TMI温度采集.DropDownItems.AddRange(new ToolStripItem[] { 采集一次ToolStripMenuItem, 开始采集ToolStripMenuItem, 清除数据ToolStripMenuItem, 停止采集ToolStripMenuItem, 导出表格ToolStripMenuItem, 保存图片ToolStripMenuItem });
            TMI温度采集.Name = "TMI温度采集";
            TMI温度采集.Size = new Size(44, 23);
            TMI温度采集.Text = "采集";
            // 
            // 采集一次ToolStripMenuItem
            // 
            采集一次ToolStripMenuItem.Name = "采集一次ToolStripMenuItem";
            采集一次ToolStripMenuItem.Size = new Size(124, 22);
            采集一次ToolStripMenuItem.Text = "采集一次";
            // 
            // 开始采集ToolStripMenuItem
            // 
            开始采集ToolStripMenuItem.Name = "开始采集ToolStripMenuItem";
            开始采集ToolStripMenuItem.Size = new Size(124, 22);
            开始采集ToolStripMenuItem.Text = "开始采集";
            // 
            // 清除数据ToolStripMenuItem
            // 
            清除数据ToolStripMenuItem.Name = "清除数据ToolStripMenuItem";
            清除数据ToolStripMenuItem.Size = new Size(124, 22);
            清除数据ToolStripMenuItem.Text = "清除数据";
            // 
            // 停止采集ToolStripMenuItem
            // 
            停止采集ToolStripMenuItem.Name = "停止采集ToolStripMenuItem";
            停止采集ToolStripMenuItem.Size = new Size(124, 22);
            停止采集ToolStripMenuItem.Text = "停止采集";
            // 
            // 导出表格ToolStripMenuItem
            // 
            导出表格ToolStripMenuItem.Name = "导出表格ToolStripMenuItem";
            导出表格ToolStripMenuItem.Size = new Size(124, 22);
            导出表格ToolStripMenuItem.Text = "导出表格";
            // 
            // 保存图片ToolStripMenuItem
            // 
            保存图片ToolStripMenuItem.Name = "保存图片ToolStripMenuItem";
            保存图片ToolStripMenuItem.Size = new Size(124, 22);
            保存图片ToolStripMenuItem.Text = "保存图片";
            // 
            // TTB温度测试名称
            // 
            TTB温度测试名称.Alignment = ToolStripItemAlignment.Right;
            TTB温度测试名称.Name = "TTB温度测试名称";
            TTB温度测试名称.Size = new Size(100, 23);
            // 
            // 温度测试名称
            // 
            温度测试名称.Alignment = ToolStripItemAlignment.Right;
            温度测试名称.Name = "温度测试名称";
            温度测试名称.Size = new Size(68, 23);
            温度测试名称.Text = "测试名称";
            // 
            // TP压力测试
            // 
            TP压力测试.Controls.Add(MS压力测试);
            TP压力测试.Location = new Point(0, 40);
            TP压力测试.Name = "TP压力测试";
            TP压力测试.Padding = new Padding(3);
            TP压力测试.Size = new Size(984, 601);
            TP压力测试.TabIndex = 1;
            TP压力测试.Text = "压力测试";
            TP压力测试.UseVisualStyleBackColor = true;
            // 
            // MS压力测试
            // 
            MS压力测试.Items.AddRange(new ToolStripItem[] { 设备ToolStripMenuItem1, 采集间隔ToolStripMenuItem1, TTB压力采集间隔, sToolStripMenuItem1, 目标压力ToolStripMenuItem, TTB目标压力, paToolStripMenuItem, TMI压力设置, TMI压力采集, TTB压力测试名称, 测试名称ToolStripMenuItem });
            MS压力测试.Location = new Point(3, 3);
            MS压力测试.Name = "MS压力测试";
            MS压力测试.Size = new Size(978, 27);
            MS压力测试.TabIndex = 0;
            MS压力测试.Text = "menuStrip2";
            // 
            // 设备ToolStripMenuItem1
            // 
            设备ToolStripMenuItem1.Name = "设备ToolStripMenuItem1";
            设备ToolStripMenuItem1.Size = new Size(44, 23);
            设备ToolStripMenuItem1.Text = "设备";
            // 
            // 采集间隔ToolStripMenuItem1
            // 
            采集间隔ToolStripMenuItem1.Name = "采集间隔ToolStripMenuItem1";
            采集间隔ToolStripMenuItem1.Size = new Size(68, 23);
            采集间隔ToolStripMenuItem1.Text = "采集间隔";
            // 
            // TTB压力采集间隔
            // 
            TTB压力采集间隔.Name = "TTB压力采集间隔";
            TTB压力采集间隔.Size = new Size(30, 23);
            TTB压力采集间隔.Text = "2";
            // 
            // sToolStripMenuItem1
            // 
            sToolStripMenuItem1.Name = "sToolStripMenuItem1";
            sToolStripMenuItem1.Size = new Size(27, 23);
            sToolStripMenuItem1.Text = "S";
            // 
            // 目标压力ToolStripMenuItem
            // 
            目标压力ToolStripMenuItem.Name = "目标压力ToolStripMenuItem";
            目标压力ToolStripMenuItem.Size = new Size(68, 23);
            目标压力ToolStripMenuItem.Text = "目标压力";
            // 
            // TTB目标压力
            // 
            TTB目标压力.Name = "TTB目标压力";
            TTB目标压力.Size = new Size(50, 23);
            TTB目标压力.Text = "500000";
            // 
            // paToolStripMenuItem
            // 
            paToolStripMenuItem.Name = "paToolStripMenuItem";
            paToolStripMenuItem.Size = new Size(34, 23);
            paToolStripMenuItem.Text = "Pa";
            // 
            // TMI压力设置
            // 
            TMI压力设置.Name = "TMI压力设置";
            TMI压力设置.Size = new Size(44, 23);
            TMI压力设置.Text = "设置";
            // 
            // TMI压力采集
            // 
            TMI压力采集.Alignment = ToolStripItemAlignment.Right;
            TMI压力采集.Name = "TMI压力采集";
            TMI压力采集.Size = new Size(44, 23);
            TMI压力采集.Text = "采集";
            // 
            // TTB压力测试名称
            // 
            TTB压力测试名称.Alignment = ToolStripItemAlignment.Right;
            TTB压力测试名称.Name = "TTB压力测试名称";
            TTB压力测试名称.Size = new Size(100, 23);
            // 
            // 测试名称ToolStripMenuItem
            // 
            测试名称ToolStripMenuItem.Alignment = ToolStripItemAlignment.Right;
            测试名称ToolStripMenuItem.Name = "测试名称ToolStripMenuItem";
            测试名称ToolStripMenuItem.Size = new Size(68, 23);
            测试名称ToolStripMenuItem.Text = "测试名称";
            // 
            // Test
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 641);
            Controls.Add(HTP测试);
            MainMenuStrip = MS温度测试;
            Name = "Test";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Test";
            HTP测试.ResumeLayout(false);
            TP温度测试.ResumeLayout(false);
            TP温度测试.PerformLayout();
            GPB温度分布.ResumeLayout(false);
            MS温度测试.ResumeLayout(false);
            MS温度测试.PerformLayout();
            TP压力测试.ResumeLayout(false);
            TP压力测试.PerformLayout();
            MS压力测试.ResumeLayout(false);
            MS压力测试.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.HopeTabPage HTP测试;
        private TabPage TP温度测试;
        private TabPage TP压力测试;
        private MenuStrip MS温度测试;
        private MenuStrip MS压力测试;
        private ToolStripMenuItem 设备ToolStripMenuItem;
        private ToolStripMenuItem 设备ToolStripMenuItem1;
        private ToolStripMenuItem TMI连接采集卡;
        private ToolStripMenuItem TMI断开采集卡;
        private ToolStripMenuItem TMI连接温控;
        private ToolStripMenuItem TMI断开温控;
        private ToolStripMenuItem 采集间隔ToolStripMenuItem;
        private ToolStripTextBox TTB温度采集间隔;
        private ToolStripMenuItem sToolStripMenuItem;
        private ToolStripMenuItem 偏差温度ToolStripMenuItem;
        private ToolStripTextBox TTB偏差温度;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem 目标温度ToolStripMenuItem;
        private ToolStripTextBox TTB目标温度;
        private ToolStripMenuItem 设置温度ToolStripMenuItem;
        private ToolStripTextBox TTB设置温度;
        private ToolStripMenuItem TMI温度设置;
        private ToolStripMenuItem TMI温度采集;
        private ToolStripTextBox TTB温度测试名称;
        private ToolStripMenuItem 温度测试名称;
        private ToolStripMenuItem 采集一次ToolStripMenuItem;
        private ToolStripMenuItem 开始采集ToolStripMenuItem;
        private ToolStripMenuItem 清除数据ToolStripMenuItem;
        private ToolStripMenuItem 停止采集ToolStripMenuItem;
        private ToolStripMenuItem 导出表格ToolStripMenuItem;
        private ToolStripMenuItem 保存图片ToolStripMenuItem;
        private ToolStripMenuItem 采集间隔ToolStripMenuItem1;
        private ToolStripTextBox TTB压力采集间隔;
        private ToolStripMenuItem sToolStripMenuItem1;
        private ToolStripMenuItem 目标压力ToolStripMenuItem;
        private ToolStripTextBox TTB目标压力;
        private ToolStripMenuItem paToolStripMenuItem;
        private ToolStripMenuItem TMI压力设置;
        private ToolStripMenuItem TMI压力采集;
        private ToolStripTextBox TTB压力测试名称;
        private ToolStripMenuItem 测试名称ToolStripMenuItem;
        private RichTextBox richTextBox1;
        private GroupBox GPB温度分布;
        private Panel PN温度分布;
    }
}