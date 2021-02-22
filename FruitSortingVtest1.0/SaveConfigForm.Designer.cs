namespace FruitSortingVtest1._0
{
    partial class SaveConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveConfigForm));
            this.EditConfigtextBox = new System.Windows.Forms.TextBox();
            this.SaveConfigbutton = new System.Windows.Forms.Button();
            this.ConfiglistBox = new System.Windows.Forms.ListBox();
            this.SaveUserConfiglabel = new System.Windows.Forms.Label();
            this.SaveProjectConfiglabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // EditConfigtextBox
            // 
            resources.ApplyResources(this.EditConfigtextBox, "EditConfigtextBox");
            this.EditConfigtextBox.Name = "EditConfigtextBox";
            // 
            // SaveConfigbutton
            // 
            resources.ApplyResources(this.SaveConfigbutton, "SaveConfigbutton");
            this.SaveConfigbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SaveConfigbutton.Name = "SaveConfigbutton";
            this.SaveConfigbutton.UseVisualStyleBackColor = true;
            this.SaveConfigbutton.Click += new System.EventHandler(this.SaveConfigbutton_Click);
            // 
            // ConfiglistBox
            // 
            resources.ApplyResources(this.ConfiglistBox, "ConfiglistBox");
            this.ConfiglistBox.FormattingEnabled = true;
            this.ConfiglistBox.Name = "ConfiglistBox";
            this.ConfiglistBox.SelectedIndexChanged += new System.EventHandler(this.ConfiglistBox_SelectedIndexChanged);
            // 
            // SaveUserConfiglabel
            // 
            resources.ApplyResources(this.SaveUserConfiglabel, "SaveUserConfiglabel");
            this.SaveUserConfiglabel.Name = "SaveUserConfiglabel";
            // 
            // SaveProjectConfiglabel
            // 
            resources.ApplyResources(this.SaveProjectConfiglabel, "SaveProjectConfiglabel");
            this.SaveProjectConfiglabel.Name = "SaveProjectConfiglabel";
            // 
            // SaveConfigForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SaveProjectConfiglabel);
            this.Controls.Add(this.SaveUserConfiglabel);
            this.Controls.Add(this.ConfiglistBox);
            this.Controls.Add(this.SaveConfigbutton);
            this.Controls.Add(this.EditConfigtextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SaveConfigForm";
            this.Load += new System.EventHandler(this.SaveConfigForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox EditConfigtextBox;
        private System.Windows.Forms.Button SaveConfigbutton;
        private System.Windows.Forms.ListBox ConfiglistBox;
        private System.Windows.Forms.Label SaveUserConfiglabel;
        private System.Windows.Forms.Label SaveProjectConfiglabel;
    }
}