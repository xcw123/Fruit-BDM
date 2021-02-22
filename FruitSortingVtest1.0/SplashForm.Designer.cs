namespace FruitSortingVtest1
{
    partial class SplashForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
            this.InitialStatuslabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // InitialStatuslabel
            // 
            resources.ApplyResources(this.InitialStatuslabel, "InitialStatuslabel");
            this.InitialStatuslabel.BackColor = System.Drawing.Color.Transparent;
            this.InitialStatuslabel.ForeColor = System.Drawing.Color.Black;
            this.InitialStatuslabel.Name = "InitialStatuslabel";
            // 
            // SplashForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::FruitSortingVtest1.Properties.Resources.启动界面3;
            this.ControlBox = false;
            this.Controls.Add(this.InitialStatuslabel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashForm";
            this.Load += new System.EventHandler(this.SplashForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label InitialStatuslabel;
    }
}