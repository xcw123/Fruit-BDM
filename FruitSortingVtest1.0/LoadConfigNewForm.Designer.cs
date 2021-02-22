namespace FruitSortingVtest1._0
{
    partial class LoadConfigNewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadConfigNewForm));
            this.ConfiglistBox = new System.Windows.Forms.ListBox();
            this.LoadConfigbutton = new System.Windows.Forms.Button();
            this.DeleteConfigbutton = new System.Windows.Forms.Button();
            this.Cancelbutton = new System.Windows.Forms.Button();
            this.LoadConfigprogressBar = new System.Windows.Forms.ProgressBar();
            this.LoadProjectConfiglabel = new System.Windows.Forms.Label();
            this.LoadUserConfiglabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ConfiglistBox
            // 
            this.ConfiglistBox.FormattingEnabled = true;
            resources.ApplyResources(this.ConfiglistBox, "ConfiglistBox");
            this.ConfiglistBox.Name = "ConfiglistBox";
            // 
            // LoadConfigbutton
            // 
            resources.ApplyResources(this.LoadConfigbutton, "LoadConfigbutton");
            this.LoadConfigbutton.Name = "LoadConfigbutton";
            this.LoadConfigbutton.UseVisualStyleBackColor = true;
            this.LoadConfigbutton.Click += new System.EventHandler(this.LoadConfigbutton_Click);
            // 
            // DeleteConfigbutton
            // 
            resources.ApplyResources(this.DeleteConfigbutton, "DeleteConfigbutton");
            this.DeleteConfigbutton.Name = "DeleteConfigbutton";
            this.DeleteConfigbutton.UseVisualStyleBackColor = true;
            this.DeleteConfigbutton.Click += new System.EventHandler(this.DeleteConfigbutton_Click);
            // 
            // Cancelbutton
            // 
            this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.Cancelbutton, "Cancelbutton");
            this.Cancelbutton.Name = "Cancelbutton";
            this.Cancelbutton.UseVisualStyleBackColor = true;
            this.Cancelbutton.Click += new System.EventHandler(this.Cancelbutton_Click);
            // 
            // LoadConfigprogressBar
            // 
            resources.ApplyResources(this.LoadConfigprogressBar, "LoadConfigprogressBar");
            this.LoadConfigprogressBar.Maximum = 200;
            this.LoadConfigprogressBar.Name = "LoadConfigprogressBar";
            this.LoadConfigprogressBar.Step = 5;
            // 
            // LoadProjectConfiglabel
            // 
            resources.ApplyResources(this.LoadProjectConfiglabel, "LoadProjectConfiglabel");
            this.LoadProjectConfiglabel.Name = "LoadProjectConfiglabel";
            // 
            // LoadUserConfiglabel
            // 
            resources.ApplyResources(this.LoadUserConfiglabel, "LoadUserConfiglabel");
            this.LoadUserConfiglabel.Name = "LoadUserConfiglabel";
            // 
            // LoadConfigNewForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LoadUserConfiglabel);
            this.Controls.Add(this.LoadProjectConfiglabel);
            this.Controls.Add(this.LoadConfigprogressBar);
            this.Controls.Add(this.Cancelbutton);
            this.Controls.Add(this.DeleteConfigbutton);
            this.Controls.Add(this.LoadConfigbutton);
            this.Controls.Add(this.ConfiglistBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadConfigNewForm";
            this.Load += new System.EventHandler(this.LoadConfigNewForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ConfiglistBox;
        private System.Windows.Forms.Button LoadConfigbutton;
        private System.Windows.Forms.Button DeleteConfigbutton;
        private System.Windows.Forms.Button Cancelbutton;
        private System.Windows.Forms.ProgressBar LoadConfigprogressBar;
        private System.Windows.Forms.Label LoadProjectConfiglabel;
        private System.Windows.Forms.Label LoadUserConfiglabel;
    }
}