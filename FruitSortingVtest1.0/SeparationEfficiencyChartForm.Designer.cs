namespace FruitSortingVtest1
{
    partial class SeparationEfficiencyChartForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeparationEfficiencyChartForm));
            this.Chartpanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // Chartpanel
            // 
            resources.ApplyResources(this.Chartpanel, "Chartpanel");
            this.Chartpanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Chartpanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Chartpanel.Name = "Chartpanel";
            // 
            // SeparationEfficiencyChartForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Chartpanel);
            this.Name = "SeparationEfficiencyChartForm";
            this.Load += new System.EventHandler(this.SeparationEfficiencyChartForm_Load);
            this.SizeChanged += new System.EventHandler(this.SeparationEfficiencyChartForm_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Chartpanel;
    }
}