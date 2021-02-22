namespace FruitSortingVtest1
{
    partial class ChannelExitDriverPinReset
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChannelExitDriverPinReset));
            this.Cancelbutton = new System.Windows.Forms.Button();
            this.OKbutton = new System.Windows.Forms.Button();
            this.PasswordtextBox = new System.Windows.Forms.TextBox();
            this.Passwordlabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Cancelbutton
            // 
            this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.Cancelbutton, "Cancelbutton");
            this.Cancelbutton.Name = "Cancelbutton";
            this.Cancelbutton.UseVisualStyleBackColor = true;
            // 
            // OKbutton
            // 
            resources.ApplyResources(this.OKbutton, "OKbutton");
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // PasswordtextBox
            // 
            resources.ApplyResources(this.PasswordtextBox, "PasswordtextBox");
            this.PasswordtextBox.Name = "PasswordtextBox";
            this.PasswordtextBox.UseSystemPasswordChar = true;
            this.PasswordtextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PasswordtextBox_KeyPress);
            // 
            // Passwordlabel
            // 
            resources.ApplyResources(this.Passwordlabel, "Passwordlabel");
            this.Passwordlabel.Name = "Passwordlabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ChannelExitDriverPinReset
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cancelbutton);
            this.Controls.Add(this.OKbutton);
            this.Controls.Add(this.PasswordtextBox);
            this.Controls.Add(this.Passwordlabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChannelExitDriverPinReset";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancelbutton;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.TextBox PasswordtextBox;
        private System.Windows.Forms.Label Passwordlabel;
        private System.Windows.Forms.Label label1;
    }
}