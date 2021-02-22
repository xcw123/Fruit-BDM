namespace FruitSortingVtest1
{
    partial class DataDownloadForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataDownloadForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBoxDataLog = new System.Windows.Forms.ListBox();
            this.btnHttpTest = new System.Windows.Forms.Button();
            this.btnDataRequest = new System.Windows.Forms.Button();
            this.lblDeviceRegister = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.listBoxDataLog);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // listBoxDataLog
            // 
            resources.ApplyResources(this.listBoxDataLog, "listBoxDataLog");
            this.listBoxDataLog.FormattingEnabled = true;
            this.listBoxDataLog.Name = "listBoxDataLog";
            // 
            // btnHttpTest
            // 
            resources.ApplyResources(this.btnHttpTest, "btnHttpTest");
            this.btnHttpTest.Name = "btnHttpTest";
            this.btnHttpTest.UseVisualStyleBackColor = true;
            this.btnHttpTest.Click += new System.EventHandler(this.btnHttpTest_Click);
            // 
            // btnDataRequest
            // 
            resources.ApplyResources(this.btnDataRequest, "btnDataRequest");
            this.btnDataRequest.Name = "btnDataRequest";
            this.btnDataRequest.UseVisualStyleBackColor = true;
            this.btnDataRequest.Click += new System.EventHandler(this.btnDataRequest_Click);
            // 
            // lblDeviceRegister
            // 
            resources.ApplyResources(this.lblDeviceRegister, "lblDeviceRegister");
            this.lblDeviceRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblDeviceRegister.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblDeviceRegister.Name = "lblDeviceRegister";
            this.lblDeviceRegister.DoubleClick += new System.EventHandler(this.lblDeviceRegister_DoubleClick);
            // 
            // DataDownloadForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.Controls.Add(this.lblDeviceRegister);
            this.Controls.Add(this.btnDataRequest);
            this.Controls.Add(this.btnHttpTest);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "DataDownloadForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DataDownloadForm_FormClosing);
            this.Load += new System.EventHandler(this.DataDownloadForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listBoxDataLog;
        private System.Windows.Forms.Button btnHttpTest;
        private System.Windows.Forms.Button btnDataRequest;
        private System.Windows.Forms.Label lblDeviceRegister;
    }
}