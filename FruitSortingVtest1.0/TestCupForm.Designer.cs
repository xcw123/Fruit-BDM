namespace FruitSortingVtest1
{
    partial class TestCupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestCupForm));
            this.PrepareInfolabel = new System.Windows.Forms.Label();
            this.Testbutton = new System.Windows.Forms.Button();
            this.Quitbutton = new System.Windows.Forms.Button();
            this.TestInfolabel = new System.Windows.Forms.Label();
            this.Infolabel = new System.Windows.Forms.Label();
            this.StopTestlabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PrepareInfolabel
            // 
            resources.ApplyResources(this.PrepareInfolabel, "PrepareInfolabel");
            this.PrepareInfolabel.Name = "PrepareInfolabel";
            // 
            // Testbutton
            // 
            resources.ApplyResources(this.Testbutton, "Testbutton");
            this.Testbutton.Name = "Testbutton";
            this.Testbutton.UseVisualStyleBackColor = true;
            this.Testbutton.Click += new System.EventHandler(this.Testbutton_Click);
            // 
            // Quitbutton
            // 
            resources.ApplyResources(this.Quitbutton, "Quitbutton");
            this.Quitbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Quitbutton.Name = "Quitbutton";
            this.Quitbutton.UseVisualStyleBackColor = true;
            // 
            // TestInfolabel
            // 
            resources.ApplyResources(this.TestInfolabel, "TestInfolabel");
            this.TestInfolabel.Name = "TestInfolabel";
            // 
            // Infolabel
            // 
            resources.ApplyResources(this.Infolabel, "Infolabel");
            this.Infolabel.Name = "Infolabel";
            // 
            // StopTestlabel
            // 
            resources.ApplyResources(this.StopTestlabel, "StopTestlabel");
            this.StopTestlabel.Name = "StopTestlabel";
            // 
            // TestCupForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StopTestlabel);
            this.Controls.Add(this.Infolabel);
            this.Controls.Add(this.TestInfolabel);
            this.Controls.Add(this.Quitbutton);
            this.Controls.Add(this.Testbutton);
            this.Controls.Add(this.PrepareInfolabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestCupForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TestCupForm_FormClosed);
            this.Load += new System.EventHandler(this.TestCupForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label PrepareInfolabel;
        private System.Windows.Forms.Button Testbutton;
        private System.Windows.Forms.Button Quitbutton;
        private System.Windows.Forms.Label TestInfolabel;
        private System.Windows.Forms.Label Infolabel;
        private System.Windows.Forms.Label StopTestlabel;
    }
}