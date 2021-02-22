namespace FruitSortingVtest1
{
    partial class SeparationLogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeparationLogForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelMinute = new System.Windows.Forms.Label();
            this.labelTotal = new System.Windows.Forms.Label();
            this.labelAdd = new System.Windows.Forms.Label();
            this.SelectdateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.SelectdateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.Displaypanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.Panel1.Controls.Add(this.labelMinute);
            this.splitContainer1.Panel1.Controls.Add(this.labelTotal);
            this.splitContainer1.Panel1.Controls.Add(this.labelAdd);
            this.splitContainer1.Panel1.Controls.Add(this.SelectdateTimePicker2);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.btnSelect);
            this.splitContainer1.Panel1.Controls.Add(this.SelectdateTimePicker1);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.Panel2.Controls.Add(this.vScrollBar1);
            this.splitContainer1.Panel2.Controls.Add(this.Displaypanel);
            // 
            // labelMinute
            // 
            resources.ApplyResources(this.labelMinute, "labelMinute");
            this.labelMinute.Name = "labelMinute";
            // 
            // labelTotal
            // 
            resources.ApplyResources(this.labelTotal, "labelTotal");
            this.labelTotal.Name = "labelTotal";
            // 
            // labelAdd
            // 
            resources.ApplyResources(this.labelAdd, "labelAdd");
            this.labelAdd.Name = "labelAdd";
            // 
            // SelectdateTimePicker2
            // 
            resources.ApplyResources(this.SelectdateTimePicker2, "SelectdateTimePicker2");
            this.SelectdateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.SelectdateTimePicker2.Name = "SelectdateTimePicker2";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btnSelect
            // 
            resources.ApplyResources(this.btnSelect, "btnSelect");
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // SelectdateTimePicker1
            // 
            resources.ApplyResources(this.SelectdateTimePicker1, "SelectdateTimePicker1");
            this.SelectdateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.SelectdateTimePicker1.Name = "SelectdateTimePicker1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // vScrollBar1
            // 
            resources.ApplyResources(this.vScrollBar1, "vScrollBar1");
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // Displaypanel
            // 
            resources.ApplyResources(this.Displaypanel, "Displaypanel");
            this.Displaypanel.BackColor = System.Drawing.Color.White;
            this.Displaypanel.Name = "Displaypanel";
            // 
            // SeparationLogForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.Controls.Add(this.splitContainer1);
            this.Name = "SeparationLogForm";
            this.Load += new System.EventHandler(this.SeparationLogForm_Load);
            this.SizeChanged += new System.EventHandler(this.SeparationLogForm_SizeChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.DateTimePicker SelectdateTimePicker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.Panel Displaypanel;
        private System.Windows.Forms.DateTimePicker SelectdateTimePicker2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelMinute;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.Label labelAdd;
    }
}