namespace FruitSortingVtest1
{
    partial class LanguageSelectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LanguageSelectForm));
            this.LanguageSelectlabel = new System.Windows.Forms.Label();
            this.LanguageSelectcomboBox = new System.Windows.Forms.ComboBox();
            this.OKbutton = new System.Windows.Forms.Button();
            this.Closedbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LanguageSelectlabel
            // 
            resources.ApplyResources(this.LanguageSelectlabel, "LanguageSelectlabel");
            this.LanguageSelectlabel.Name = "LanguageSelectlabel";
            // 
            // LanguageSelectcomboBox
            // 
            resources.ApplyResources(this.LanguageSelectcomboBox, "LanguageSelectcomboBox");
            this.LanguageSelectcomboBox.FormattingEnabled = true;
            this.LanguageSelectcomboBox.Name = "LanguageSelectcomboBox";
            // 
            // OKbutton
            // 
            resources.ApplyResources(this.OKbutton, "OKbutton");
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // Closedbutton
            // 
            resources.ApplyResources(this.Closedbutton, "Closedbutton");
            this.Closedbutton.Name = "Closedbutton";
            this.Closedbutton.UseVisualStyleBackColor = true;
            this.Closedbutton.Click += new System.EventHandler(this.Closedbutton_Click);
            // 
            // LanguageSelectForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.Closedbutton);
            this.Controls.Add(this.OKbutton);
            this.Controls.Add(this.LanguageSelectcomboBox);
            this.Controls.Add(this.LanguageSelectlabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LanguageSelectForm";
            this.Load += new System.EventHandler(this.LanguageSelectForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LanguageSelectlabel;
        private System.Windows.Forms.ComboBox LanguageSelectcomboBox;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.Button Closedbutton;
    }
}