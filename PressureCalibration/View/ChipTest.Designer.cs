namespace PressureCalibration.View
{
    partial class ChipTest
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
            RTB信息 = new RichTextBox();
            CMS信息 = new ContextMenuStrip(components);
            TMI清除 = new ToolStripMenuItem();
            CB设备地址 = new ComboBox();
            label1 = new Label();
            BTN读取UID = new Button();
            BTN读取温度 = new Button();
            CB寄存器地址 = new ComboBox();
            CB传感器地址 = new ComboBox();
            TB寄存器个数 = new TextBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            GB电源控制 = new GroupBox();
            BTN41V = new Button();
            BTN33V = new Button();
            BTN18V = new Button();
            BTN关闭电源 = new Button();
            BTN读取芯片数据 = new Button();
            BTN读取 = new Button();
            BTN查看数据 = new Button();
            CMS信息.SuspendLayout();
            GB电源控制.SuspendLayout();
            SuspendLayout();
            // 
            // RTB信息
            // 
            RTB信息.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RTB信息.BorderStyle = BorderStyle.None;
            RTB信息.ContextMenuStrip = CMS信息;
            RTB信息.Location = new Point(10, 10);
            RTB信息.Margin = new Padding(1);
            RTB信息.Name = "RTB信息";
            RTB信息.ReadOnly = true;
            RTB信息.Size = new Size(547, 430);
            RTB信息.TabIndex = 0;
            RTB信息.Text = "";
            // 
            // CMS信息
            // 
            CMS信息.Items.AddRange(new ToolStripItem[] { TMI清除 });
            CMS信息.Name = "CMS信息";
            CMS信息.Size = new Size(101, 26);
            // 
            // TMI清除
            // 
            TMI清除.Name = "TMI清除";
            TMI清除.Size = new Size(100, 22);
            TMI清除.Text = "清除";
            TMI清除.Click += TMI清除_Click;
            // 
            // CB设备地址
            // 
            CB设备地址.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CB设备地址.FormattingEnabled = true;
            CB设备地址.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            CB设备地址.Location = new Point(627, 21);
            CB设备地址.Name = "CB设备地址";
            CB设备地址.Size = new Size(83, 25);
            CB设备地址.TabIndex = 22;
            CB设备地址.Text = "1";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(565, 26);
            label1.Name = "label1";
            label1.Size = new Size(56, 17);
            label1.TabIndex = 21;
            label1.Text = "设备地址";
            // 
            // BTN读取UID
            // 
            BTN读取UID.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BTN读取UID.Font = new Font("Microsoft YaHei UI", 9F);
            BTN读取UID.Location = new Point(642, 59);
            BTN读取UID.Name = "BTN读取UID";
            BTN读取UID.Size = new Size(70, 23);
            BTN读取UID.TabIndex = 24;
            BTN读取UID.Tag = "readUID";
            BTN读取UID.Text = "读取UID";
            BTN读取UID.UseVisualStyleBackColor = true;
            BTN读取UID.Click += BTN采集卡_Click;
            // 
            // BTN读取温度
            // 
            BTN读取温度.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BTN读取温度.Font = new Font("Microsoft YaHei UI", 9F);
            BTN读取温度.Location = new Point(565, 59);
            BTN读取温度.Name = "BTN读取温度";
            BTN读取温度.Size = new Size(70, 23);
            BTN读取温度.TabIndex = 23;
            BTN读取温度.Tag = "readT";
            BTN读取温度.Text = "读取温度";
            BTN读取温度.UseVisualStyleBackColor = true;
            BTN读取温度.Click += BTN采集卡_Click;
            // 
            // CB寄存器地址
            // 
            CB寄存器地址.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CB寄存器地址.FormattingEnabled = true;
            CB寄存器地址.Items.AddRange(new object[] { "2", "23", "26", "52" });
            CB寄存器地址.Location = new Point(639, 274);
            CB寄存器地址.Name = "CB寄存器地址";
            CB寄存器地址.Size = new Size(73, 25);
            CB寄存器地址.TabIndex = 30;
            CB寄存器地址.Text = "2";
            // 
            // CB传感器地址
            // 
            CB传感器地址.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CB传感器地址.FormattingEnabled = true;
            CB传感器地址.Items.AddRange(new object[] { "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" });
            CB传感器地址.Location = new Point(639, 246);
            CB传感器地址.Name = "CB传感器地址";
            CB传感器地址.Size = new Size(73, 25);
            CB传感器地址.TabIndex = 29;
            CB传感器地址.Text = "-1";
            // 
            // TB寄存器个数
            // 
            TB寄存器个数.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            TB寄存器个数.Location = new Point(639, 306);
            TB寄存器个数.Name = "TB寄存器个数";
            TB寄存器个数.Size = new Size(73, 23);
            TB寄存器个数.TabIndex = 28;
            TB寄存器个数.Text = "4";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(565, 251);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 25;
            label2.Text = "传感器地址";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(565, 280);
            label3.Name = "label3";
            label3.Size = new Size(68, 17);
            label3.TabIndex = 26;
            label3.Text = "寄存器地址";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(565, 309);
            label4.Name = "label4";
            label4.Size = new Size(68, 17);
            label4.TabIndex = 27;
            label4.Text = "寄存器个数";
            // 
            // GB电源控制
            // 
            GB电源控制.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            GB电源控制.Controls.Add(BTN41V);
            GB电源控制.Controls.Add(BTN33V);
            GB电源控制.Controls.Add(BTN18V);
            GB电源控制.Controls.Add(BTN关闭电源);
            GB电源控制.Location = new Point(566, 345);
            GB电源控制.Name = "GB电源控制";
            GB电源控制.Size = new Size(222, 93);
            GB电源控制.TabIndex = 31;
            GB电源控制.TabStop = false;
            GB电源控制.Text = "电源控制";
            // 
            // BTN41V
            // 
            BTN41V.Location = new Point(83, 51);
            BTN41V.Name = "BTN41V";
            BTN41V.Size = new Size(70, 23);
            BTN41V.TabIndex = 12;
            BTN41V.Tag = "4.1";
            BTN41V.Text = "4.1V";
            BTN41V.UseVisualStyleBackColor = true;
            BTN41V.Click += BTN采集卡_Click;
            // 
            // BTN33V
            // 
            BTN33V.Location = new Point(7, 51);
            BTN33V.Name = "BTN33V";
            BTN33V.Size = new Size(70, 23);
            BTN33V.TabIndex = 11;
            BTN33V.Tag = "3.3";
            BTN33V.Text = "3.3V";
            BTN33V.UseVisualStyleBackColor = true;
            BTN33V.Click += BTN采集卡_Click;
            // 
            // BTN18V
            // 
            BTN18V.Location = new Point(83, 22);
            BTN18V.Name = "BTN18V";
            BTN18V.Size = new Size(70, 23);
            BTN18V.TabIndex = 10;
            BTN18V.Tag = "1.8";
            BTN18V.Text = "1.8V";
            BTN18V.UseVisualStyleBackColor = true;
            BTN18V.Click += BTN采集卡_Click;
            // 
            // BTN关闭电源
            // 
            BTN关闭电源.Location = new Point(7, 22);
            BTN关闭电源.Name = "BTN关闭电源";
            BTN关闭电源.Size = new Size(70, 23);
            BTN关闭电源.TabIndex = 9;
            BTN关闭电源.Tag = "off";
            BTN关闭电源.Text = "关闭电源";
            BTN关闭电源.UseVisualStyleBackColor = true;
            BTN关闭电源.Click += BTN采集卡_Click;
            // 
            // BTN读取芯片数据
            // 
            BTN读取芯片数据.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BTN读取芯片数据.Location = new Point(718, 59);
            BTN读取芯片数据.Name = "BTN读取芯片数据";
            BTN读取芯片数据.Size = new Size(70, 23);
            BTN读取芯片数据.TabIndex = 32;
            BTN读取芯片数据.Tag = "readOutput";
            BTN读取芯片数据.Text = "读取芯片";
            BTN读取芯片数据.UseVisualStyleBackColor = true;
            BTN读取芯片数据.Click += BTN采集卡_Click;
            // 
            // BTN读取
            // 
            BTN读取.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BTN读取.Location = new Point(718, 246);
            BTN读取.Name = "BTN读取";
            BTN读取.Size = new Size(70, 23);
            BTN读取.TabIndex = 33;
            BTN读取.Tag = "read";
            BTN读取.Text = "读取";
            BTN读取.UseVisualStyleBackColor = true;
            BTN读取.Click += BTN采集卡_Click;
            // 
            // BTN查看数据
            // 
            BTN查看数据.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BTN查看数据.Font = new Font("Microsoft YaHei UI", 9F);
            BTN查看数据.Location = new Point(566, 88);
            BTN查看数据.Name = "BTN查看数据";
            BTN查看数据.Size = new Size(70, 23);
            BTN查看数据.TabIndex = 34;
            BTN查看数据.Tag = "readData";
            BTN查看数据.Text = "查看数据";
            BTN查看数据.UseVisualStyleBackColor = true;
            BTN查看数据.Click += BTN采集卡_Click;
            // 
            // ChipTest
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(BTN查看数据);
            Controls.Add(BTN读取);
            Controls.Add(BTN读取芯片数据);
            Controls.Add(GB电源控制);
            Controls.Add(CB寄存器地址);
            Controls.Add(CB传感器地址);
            Controls.Add(TB寄存器个数);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(BTN读取UID);
            Controls.Add(BTN读取温度);
            Controls.Add(CB设备地址);
            Controls.Add(label1);
            Controls.Add(RTB信息);
            Name = "ChipTest";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "芯片测试";
            CMS信息.ResumeLayout(false);
            GB电源控制.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox RTB信息;
        private ComboBox CB设备地址;
        private Label label1;
        private Button BTN读取UID;
        private Button BTN读取温度;
        private ComboBox CB寄存器地址;
        private ComboBox CB传感器地址;
        private TextBox TB寄存器个数;
        private Label label2;
        private Label label3;
        private Label label4;
        private GroupBox GB电源控制;
        private Button BTN41V;
        private Button BTN33V;
        private Button BTN18V;
        private Button BTN关闭电源;
        private Button BTN读取芯片数据;
        private Button BTN读取;
        private ContextMenuStrip CMS信息;
        private ToolStripMenuItem TMI清除;
        private Button BTN查看数据;
    }
}