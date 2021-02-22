namespace FruitSortingVtest1
{
    partial class DatabaseSetForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseSetForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TxtDataSource = new System.Windows.Forms.TextBox();
            this.TxtDataBase = new System.Windows.Forms.TextBox();
            this.TxtUserName = new System.Windows.Forms.TextBox();
            this.TxtPassword = new System.Windows.Forms.TextBox();
            this.BtnConnectTest = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // TxtDataSource
            // 
            resources.ApplyResources(this.TxtDataSource, "TxtDataSource");
            this.TxtDataSource.Name = "TxtDataSource";
            // 
            // TxtDataBase
            // 
            resources.ApplyResources(this.TxtDataBase, "TxtDataBase");
            this.TxtDataBase.Name = "TxtDataBase";
            // 
            // TxtUserName
            // 
            resources.ApplyResources(this.TxtUserName, "TxtUserName");
            this.TxtUserName.Name = "TxtUserName";
            // 
            // TxtPassword
            // 
            resources.ApplyResources(this.TxtPassword, "TxtPassword");
            this.TxtPassword.Name = "TxtPassword";
            this.TxtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtPassword_KeyPress);
            // 
            // BtnConnectTest
            // 
            resources.ApplyResources(this.BtnConnectTest, "BtnConnectTest");
            this.BtnConnectTest.Name = "BtnConnectTest";
            this.BtnConnectTest.UseVisualStyleBackColor = true;
            this.BtnConnectTest.Click += new System.EventHandler(this.BtnConnectTest_Click);
            // 
            // BtnOK
            // 
            resources.ApplyResources(this.BtnOK, "BtnOK");
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // DatabaseSetForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.BtnConnectTest);
            this.Controls.Add(this.TxtPassword);
            this.Controls.Add(this.TxtUserName);
            this.Controls.Add(this.TxtDataBase);
            this.Controls.Add(this.TxtDataSource);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DatabaseSetForm";
            this.Load += new System.EventHandler(this.DatabaseSetForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TxtDataSource;
        private System.Windows.Forms.TextBox TxtDataBase;
        private System.Windows.Forms.TextBox TxtUserName;
        private System.Windows.Forms.TextBox TxtPassword;
        private System.Windows.Forms.Button BtnConnectTest;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Label label5;
    }
}