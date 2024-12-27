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
            BTN保存 = new Button();
            SuspendLayout();
            // 
            // BTN保存
            // 
            BTN保存.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BTN保存.Location = new Point(717, 426);
            BTN保存.Name = "BTN保存";
            BTN保存.Size = new Size(75, 23);
            BTN保存.TabIndex = 0;
            BTN保存.Text = "保存";
            BTN保存.UseVisualStyleBackColor = true;
            BTN保存.Click += BTN保存_Click;
            // 
            // Setting
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(804, 461);
            Controls.Add(BTN保存);
            Name = "Setting";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "设置";
            ResumeLayout(false);
        }

        #endregion

        private Button BTN保存;
    }
}