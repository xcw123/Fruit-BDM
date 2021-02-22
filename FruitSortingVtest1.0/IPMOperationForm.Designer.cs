namespace FruitSortingVtest1
{
    partial class IPMOperationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IPMOperationForm));
            this.Closebutton = new System.Windows.Forms.Button();
            this.OpenIPMbutton = new System.Windows.Forms.Button();
            this.Refreshbutton = new System.Windows.Forms.Button();
            this.IPMMACAddrlistViewEx = new ListViewEx.ListViewEx();
            this.ValueNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ValueValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AllcheckBox = new System.Windows.Forms.CheckBox();
            this.IPMcustomCheckedListBox = new Qodex.CustomCheckedListBox();
            this.txtMACAddr = new System.Windows.Forms.TextBox();
            this.GetMacbutton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Closebutton
            // 
            resources.ApplyResources(this.Closebutton, "Closebutton");
            this.Closebutton.Name = "Closebutton";
            this.Closebutton.UseVisualStyleBackColor = true;
            this.Closebutton.Click += new System.EventHandler(this.Closebutton_Click);
            // 
            // OpenIPMbutton
            // 
            resources.ApplyResources(this.OpenIPMbutton, "OpenIPMbutton");
            this.OpenIPMbutton.Name = "OpenIPMbutton";
            this.OpenIPMbutton.UseVisualStyleBackColor = true;
            this.OpenIPMbutton.Click += new System.EventHandler(this.OpenIPMbutton_Click);
            // 
            // Refreshbutton
            // 
            resources.ApplyResources(this.Refreshbutton, "Refreshbutton");
            this.Refreshbutton.Name = "Refreshbutton";
            this.Refreshbutton.UseVisualStyleBackColor = true;
            this.Refreshbutton.Click += new System.EventHandler(this.Refreshbutton_Click);
            // 
            // IPMMACAddrlistViewEx
            // 
            resources.ApplyResources(this.IPMMACAddrlistViewEx, "IPMMACAddrlistViewEx");
            this.IPMMACAddrlistViewEx.AllowColumnReorder = true;
            this.IPMMACAddrlistViewEx.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ValueNum,
            this.ValueValue});
            this.IPMMACAddrlistViewEx.DoubleClickActivation = false;
            this.IPMMACAddrlistViewEx.FullRowSelect = true;
            this.IPMMACAddrlistViewEx.HideSelection = false;
            this.IPMMACAddrlistViewEx.Name = "IPMMACAddrlistViewEx";
            this.IPMMACAddrlistViewEx.UseCompatibleStateImageBehavior = false;
            this.IPMMACAddrlistViewEx.View = System.Windows.Forms.View.Details;
            this.IPMMACAddrlistViewEx.SubItemClicked += new ListViewEx.SubItemEventHandler(this.IPMMACAddrlistViewEx_SubItemClicked);
            this.IPMMACAddrlistViewEx.SubItemEndEditing += new ListViewEx.SubItemEndEditingEventHandler(this.IPMMACAddrlistViewEx_SubItemEndEditing);
            this.IPMMACAddrlistViewEx.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.IPMMACAddrlistViewEx_MouseDoubleClick);
            // 
            // ValueNum
            // 
            resources.ApplyResources(this.ValueNum, "ValueNum");
            // 
            // ValueValue
            // 
            resources.ApplyResources(this.ValueValue, "ValueValue");
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.AllcheckBox);
            this.groupBox1.Controls.Add(this.IPMcustomCheckedListBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // AllcheckBox
            // 
            resources.ApplyResources(this.AllcheckBox, "AllcheckBox");
            this.AllcheckBox.Name = "AllcheckBox";
            this.AllcheckBox.UseVisualStyleBackColor = true;
            this.AllcheckBox.CheckedChanged += new System.EventHandler(this.AllcheckBox_CheckedChanged);
            // 
            // IPMcustomCheckedListBox
            // 
            resources.ApplyResources(this.IPMcustomCheckedListBox, "IPMcustomCheckedListBox");
            this.IPMcustomCheckedListBox.DrawFocusedIndicator = false;
            this.IPMcustomCheckedListBox.FormattingEnabled = true;
            this.IPMcustomCheckedListBox.Name = "IPMcustomCheckedListBox";
            this.IPMcustomCheckedListBox.GetBackColor += new Qodex.CustomCheckedListBox.GetColorDelegate(this.IPMcustomCheckedListBox_GetBackColor);
            // 
            // txtMACAddr
            // 
            resources.ApplyResources(this.txtMACAddr, "txtMACAddr");
            this.txtMACAddr.Name = "txtMACAddr";
            this.txtMACAddr.TextChanged += new System.EventHandler(this.txtMACAddr_TextChanged);
            // 
            // GetMacbutton
            // 
            resources.ApplyResources(this.GetMacbutton, "GetMacbutton");
            this.GetMacbutton.Name = "GetMacbutton";
            this.GetMacbutton.UseVisualStyleBackColor = true;
            this.GetMacbutton.Click += new System.EventHandler(this.GetMacbutton_Click);
            // 
            // IPMOperationForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GetMacbutton);
            this.Controls.Add(this.txtMACAddr);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.IPMMACAddrlistViewEx);
            this.Controls.Add(this.Refreshbutton);
            this.Controls.Add(this.OpenIPMbutton);
            this.Controls.Add(this.Closebutton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IPMOperationForm";
            this.Load += new System.EventHandler(this.IPMOperationForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Closebutton;
        private System.Windows.Forms.Button OpenIPMbutton;
        private Qodex.CustomCheckedListBox IPMcustomCheckedListBox;
        private System.Windows.Forms.Button Refreshbutton;
        private ListViewEx.ListViewEx IPMMACAddrlistViewEx;
        private System.Windows.Forms.ColumnHeader ValueNum;
        private System.Windows.Forms.ColumnHeader ValueValue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtMACAddr;
        private System.Windows.Forms.CheckBox AllcheckBox;
        private System.Windows.Forms.Button GetMacbutton;
    }
}