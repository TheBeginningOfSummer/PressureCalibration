namespace PressureCalibration.View
{
    partial class Setting
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
            HTP设置 = new ReaLTaiizor.Controls.HopeTabPage();
            TP采集参数 = new TabPage();
            LBN保存 = new ReaLTaiizor.Controls.LostButton();
            TP采集组 = new TabPage();
            lostButton1 = new ReaLTaiizor.Controls.LostButton();
            TP压控 = new TabPage();
            lostButton2 = new ReaLTaiizor.Controls.LostButton();
            TP温控 = new TabPage();
            lostButton3 = new ReaLTaiizor.Controls.LostButton();
            HTP设置.SuspendLayout();
            TP采集参数.SuspendLayout();
            TP采集组.SuspendLayout();
            TP压控.SuspendLayout();
            TP温控.SuspendLayout();
            SuspendLayout();
            // 
            // HTP设置
            // 
            HTP设置.BaseColor = Color.White;
            HTP设置.Controls.Add(TP采集参数);
            HTP设置.Controls.Add(TP采集组);
            HTP设置.Controls.Add(TP压控);
            HTP设置.Controls.Add(TP温控);
            HTP设置.Dock = DockStyle.Fill;
            HTP设置.Font = new Font("Segoe UI", 12F);
            HTP设置.ForeColorA = Color.Black;
            HTP设置.ForeColorB = Color.Gray;
            HTP设置.ForeColorC = Color.DodgerBlue;
            HTP设置.ItemSize = new Size(120, 40);
            HTP设置.Location = new Point(0, 0);
            HTP设置.Name = "HTP设置";
            HTP设置.PixelOffsetType = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            HTP设置.SelectedIndex = 0;
            HTP设置.Size = new Size(804, 461);
            HTP设置.SizeMode = TabSizeMode.Fixed;
            HTP设置.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            HTP设置.TabIndex = 1;
            HTP设置.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            HTP设置.ThemeColorA = Color.FromArgb(64, 158, 255);
            HTP设置.ThemeColorB = Color.FromArgb(150, 64, 158, 255);
            HTP设置.TitleTextState = ReaLTaiizor.Controls.HopeTabPage.TextState.Normal;
            // 
            // TP采集参数
            // 
            TP采集参数.Controls.Add(LBN保存);
            TP采集参数.Font = new Font("Segoe UI", 8F);
            TP采集参数.Location = new Point(0, 40);
            TP采集参数.Name = "TP采集参数";
            TP采集参数.Padding = new Padding(3);
            TP采集参数.Size = new Size(804, 421);
            TP采集参数.TabIndex = 0;
            TP采集参数.Text = "采集参数";
            TP采集参数.UseVisualStyleBackColor = true;
            // 
            // LBN保存
            // 
            LBN保存.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            LBN保存.BackColor = Color.DarkGray;
            LBN保存.Font = new Font("Segoe UI", 9F);
            LBN保存.ForeColor = Color.White;
            LBN保存.HoverColor = Color.DodgerBlue;
            LBN保存.Image = null;
            LBN保存.Location = new Point(712, 379);
            LBN保存.Name = "LBN保存";
            LBN保存.Size = new Size(80, 30);
            LBN保存.TabIndex = 2;
            LBN保存.Text = "保存";
            LBN保存.Click += BTN保存_Click;
            // 
            // TP采集组
            // 
            TP采集组.Controls.Add(lostButton1);
            TP采集组.Font = new Font("Segoe UI", 8F);
            TP采集组.Location = new Point(0, 40);
            TP采集组.Name = "TP采集组";
            TP采集组.Padding = new Padding(3);
            TP采集组.Size = new Size(804, 421);
            TP采集组.TabIndex = 1;
            TP采集组.Text = "采集组";
            TP采集组.UseVisualStyleBackColor = true;
            // 
            // lostButton1
            // 
            lostButton1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lostButton1.BackColor = Color.DarkGray;
            lostButton1.Font = new Font("Segoe UI", 9F);
            lostButton1.ForeColor = Color.White;
            lostButton1.HoverColor = Color.DodgerBlue;
            lostButton1.Image = null;
            lostButton1.Location = new Point(712, 379);
            lostButton1.Name = "lostButton1";
            lostButton1.Size = new Size(80, 30);
            lostButton1.TabIndex = 2;
            lostButton1.Text = "保存";
            lostButton1.Click += BTN保存_Click;
            // 
            // TP压控
            // 
            TP压控.Controls.Add(lostButton2);
            TP压控.Font = new Font("Segoe UI", 8F);
            TP压控.Location = new Point(0, 40);
            TP压控.Name = "TP压控";
            TP压控.Size = new Size(804, 421);
            TP压控.TabIndex = 2;
            TP压控.Text = "压控";
            TP压控.UseVisualStyleBackColor = true;
            // 
            // lostButton2
            // 
            lostButton2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lostButton2.BackColor = Color.DarkGray;
            lostButton2.Font = new Font("Segoe UI", 9F);
            lostButton2.ForeColor = Color.White;
            lostButton2.HoverColor = Color.DodgerBlue;
            lostButton2.Image = null;
            lostButton2.Location = new Point(712, 379);
            lostButton2.Name = "lostButton2";
            lostButton2.Size = new Size(80, 30);
            lostButton2.TabIndex = 2;
            lostButton2.Text = "保存";
            lostButton2.Click += BTN保存_Click;
            // 
            // TP温控
            // 
            TP温控.Controls.Add(lostButton3);
            TP温控.Font = new Font("Segoe UI", 8F);
            TP温控.Location = new Point(0, 40);
            TP温控.Name = "TP温控";
            TP温控.Size = new Size(804, 421);
            TP温控.TabIndex = 3;
            TP温控.Text = "温控";
            TP温控.UseVisualStyleBackColor = true;
            // 
            // lostButton3
            // 
            lostButton3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lostButton3.BackColor = Color.DarkGray;
            lostButton3.Font = new Font("Segoe UI", 9F);
            lostButton3.ForeColor = Color.White;
            lostButton3.HoverColor = Color.DodgerBlue;
            lostButton3.Image = null;
            lostButton3.Location = new Point(712, 379);
            lostButton3.Name = "lostButton3";
            lostButton3.Size = new Size(80, 30);
            lostButton3.TabIndex = 2;
            lostButton3.Text = "保存";
            lostButton3.Click += BTN保存_Click;
            // 
            // Setting
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(804, 461);
            Controls.Add(HTP设置);
            Name = "Setting";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "设置";
            HTP设置.ResumeLayout(false);
            TP采集参数.ResumeLayout(false);
            TP采集组.ResumeLayout(false);
            TP压控.ResumeLayout(false);
            TP温控.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ReaLTaiizor.Controls.HopeTabPage HTP设置;
        private TabPage TP采集参数;
        private TabPage TP采集组;
        private TabPage TP压控;
        private TabPage TP温控;
        private ReaLTaiizor.Controls.LostButton LBN保存;
        private ReaLTaiizor.Controls.LostButton lostButton1;
        private ReaLTaiizor.Controls.LostButton lostButton2;
        private ReaLTaiizor.Controls.LostButton lostButton3;
    }
}