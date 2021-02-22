namespace FruitSortingVtest1._0
{
    partial class CountDownForm
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
            this.components = new System.ComponentModel.Container();
            this.CountDowTimer = new System.Windows.Forms.Timer(this.components);
            this.lblCountDown = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CountDowTimer
            // 
            this.CountDowTimer.Interval = 1000;
            this.CountDowTimer.Tick += new System.EventHandler(this.CountDowTimer_Tick);
            // 
            // lblCountDown
            // 
            this.lblCountDown.AutoSize = true;
            this.lblCountDown.Font = new System.Drawing.Font("宋体", 42F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCountDown.ForeColor = System.Drawing.Color.Blue;
            this.lblCountDown.Location = new System.Drawing.Point(41, 32);
            this.lblCountDown.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCountDown.Name = "lblCountDown";
            this.lblCountDown.Size = new System.Drawing.Size(52, 56);
            this.lblCountDown.TabIndex = 1;
            this.lblCountDown.Text = "9";
            this.lblCountDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CountDownForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 160);
            this.Controls.Add(this.lblCountDown);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CountDownForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "倒计时";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CountDownForm_FormClosing);
            this.Load += new System.EventHandler(this.CountDownForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer CountDowTimer;
        private System.Windows.Forms.Label lblCountDown;
    }
}