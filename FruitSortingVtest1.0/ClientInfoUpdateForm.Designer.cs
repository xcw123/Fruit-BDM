namespace FruitSortingVtest1
{
    partial class ClientInfoUpdateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientInfoUpdateForm));
            this.CboFruitName = new System.Windows.Forms.ComboBox();
            this.CboFarmName = new System.Windows.Forms.ComboBox();
            this.CboClientName = new System.Windows.Forms.ComboBox();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CboFruitName
            // 
            resources.ApplyResources(this.CboFruitName, "CboFruitName");
            this.CboFruitName.FormattingEnabled = true;
            this.CboFruitName.Name = "CboFruitName";
            // 
            // CboFarmName
            // 
            resources.ApplyResources(this.CboFarmName, "CboFarmName");
            this.CboFarmName.FormattingEnabled = true;
            this.CboFarmName.Name = "CboFarmName";
            // 
            // CboClientName
            // 
            resources.ApplyResources(this.CboClientName, "CboClientName");
            this.CboClientName.FormattingEnabled = true;
            this.CboClientName.Name = "CboClientName";
            // 
            // BtnClose
            // 
            resources.ApplyResources(this.BtnClose, "BtnClose");
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnOK
            // 
            resources.ApplyResources(this.BtnOK, "BtnOK");
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ClientInfoUpdateForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CboFruitName);
            this.Controls.Add(this.CboFarmName);
            this.Controls.Add(this.CboClientName);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClientInfoUpdateForm";
            this.Load += new System.EventHandler(this.ClientInfoUpdateForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CboFruitName;
        private System.Windows.Forms.ComboBox CboFarmName;
        private System.Windows.Forms.ComboBox CboClientName;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}