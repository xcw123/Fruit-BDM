namespace FruitSortingVtest1._0
{
    partial class BootFlashBurnForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BootFlashBurnForm));
            this.BurnprogressBar = new System.Windows.Forms.ProgressBar();
            this.DataLenghthnumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.DataLenghlabel = new System.Windows.Forms.Label();
            this.FileRoottextBox = new System.Windows.Forms.TextBox();
            this.Scanbutton = new System.Windows.Forms.Button();
            this.Startbutton = new System.Windows.Forms.Button();
            this.BurnProgresslabel = new System.Windows.Forms.Label();
            this.FileRoottoolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.DataLenghthnumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // BurnprogressBar
            // 
            resources.ApplyResources(this.BurnprogressBar, "BurnprogressBar");
            this.BurnprogressBar.Name = "BurnprogressBar";
            this.BurnprogressBar.Step = 1;
            // 
            // DataLenghthnumericUpDown
            // 
            resources.ApplyResources(this.DataLenghthnumericUpDown, "DataLenghthnumericUpDown");
            this.DataLenghthnumericUpDown.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.DataLenghthnumericUpDown.Name = "DataLenghthnumericUpDown";
            this.DataLenghthnumericUpDown.ValueChanged += new System.EventHandler(this.DataLenghthnumericUpDown_ValueChanged);
            // 
            // DataLenghlabel
            // 
            resources.ApplyResources(this.DataLenghlabel, "DataLenghlabel");
            this.DataLenghlabel.Name = "DataLenghlabel";
            // 
            // FileRoottextBox
            // 
            resources.ApplyResources(this.FileRoottextBox, "FileRoottextBox");
            this.FileRoottextBox.Name = "FileRoottextBox";
            this.FileRoottextBox.MouseHover += new System.EventHandler(this.FileRoottextBox_MouseHover);
            // 
            // Scanbutton
            // 
            resources.ApplyResources(this.Scanbutton, "Scanbutton");
            this.Scanbutton.Name = "Scanbutton";
            this.Scanbutton.UseVisualStyleBackColor = true;
            this.Scanbutton.Click += new System.EventHandler(this.Scanbutton_Click);
            // 
            // Startbutton
            // 
            resources.ApplyResources(this.Startbutton, "Startbutton");
            this.Startbutton.Name = "Startbutton";
            this.Startbutton.UseVisualStyleBackColor = true;
            this.Startbutton.Click += new System.EventHandler(this.Startbutton_Click);
            // 
            // BurnProgresslabel
            // 
            resources.ApplyResources(this.BurnProgresslabel, "BurnProgresslabel");
            this.BurnProgresslabel.Name = "BurnProgresslabel";
            // 
            // BootFlashBurnForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BurnProgresslabel);
            this.Controls.Add(this.Startbutton);
            this.Controls.Add(this.Scanbutton);
            this.Controls.Add(this.FileRoottextBox);
            this.Controls.Add(this.DataLenghlabel);
            this.Controls.Add(this.DataLenghthnumericUpDown);
            this.Controls.Add(this.BurnprogressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BootFlashBurnForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BootFlashBurnForm_FormClosing);
            this.Load += new System.EventHandler(this.BootFlashBurnForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DataLenghthnumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar BurnprogressBar;
        private System.Windows.Forms.NumericUpDown DataLenghthnumericUpDown;
        private System.Windows.Forms.Label DataLenghlabel;
        private System.Windows.Forms.TextBox FileRoottextBox;
        private System.Windows.Forms.Button Scanbutton;
        private System.Windows.Forms.Button Startbutton;
        private System.Windows.Forms.Label BurnProgresslabel;
        private System.Windows.Forms.ToolTip FileRoottoolTip;
    }
}