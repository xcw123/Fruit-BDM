namespace FruitSortingVtest1
{
    partial class ElectromagnetictestForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ElectromagnetictestForm));
            this.VolveTestcontextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.单机测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClosed = new System.Windows.Forms.Button();
            this.DistancenumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.Outletlabel = new System.Windows.Forms.Label();
            this.OffsetnumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ExitlistViewEx = new ListViewEx.ListViewEx();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.停止测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitInfoSendbutton = new System.Windows.Forms.Button();
            this.VolveTestcontextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DistancenumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetnumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // VolveTestcontextMenuStrip
            // 
            resources.ApplyResources(this.VolveTestcontextMenuStrip, "VolveTestcontextMenuStrip");
            this.VolveTestcontextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.VolveTestcontextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.单机测试ToolStripMenuItem});
            this.VolveTestcontextMenuStrip.Name = "contextMenuStrip1";
            // 
            // 单机测试ToolStripMenuItem
            // 
            resources.ApplyResources(this.单机测试ToolStripMenuItem, "单机测试ToolStripMenuItem");
            this.单机测试ToolStripMenuItem.Name = "单机测试ToolStripMenuItem";
            this.单机测试ToolStripMenuItem.Click += new System.EventHandler(this.单机测试ToolStripMenuItem_Click);
            // 
            // btnClosed
            // 
            resources.ApplyResources(this.btnClosed, "btnClosed");
            this.btnClosed.Name = "btnClosed";
            this.btnClosed.UseVisualStyleBackColor = true;
            this.btnClosed.Click += new System.EventHandler(this.btnClosed_Click);
            // 
            // DistancenumericUpDown
            // 
            resources.ApplyResources(this.DistancenumericUpDown, "DistancenumericUpDown");
            this.DistancenumericUpDown.DecimalPlaces = 1;
            this.DistancenumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.DistancenumericUpDown.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.DistancenumericUpDown.Name = "DistancenumericUpDown";
            // 
            // Outletlabel
            // 
            resources.ApplyResources(this.Outletlabel, "Outletlabel");
            this.Outletlabel.Name = "Outletlabel";
            // 
            // OffsetnumericUpDown
            // 
            resources.ApplyResources(this.OffsetnumericUpDown, "OffsetnumericUpDown");
            this.OffsetnumericUpDown.DecimalPlaces = 1;
            this.OffsetnumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.OffsetnumericUpDown.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.OffsetnumericUpDown.Name = "OffsetnumericUpDown";
            // 
            // ExitlistViewEx
            // 
            resources.ApplyResources(this.ExitlistViewEx, "ExitlistViewEx");
            this.ExitlistViewEx.AllowColumnReorder = true;
            this.ExitlistViewEx.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.ExitlistViewEx.DoubleClickActivation = false;
            this.ExitlistViewEx.FullRowSelect = true;
            this.ExitlistViewEx.HideSelection = false;
            this.ExitlistViewEx.MultiSelect = false;
            this.ExitlistViewEx.Name = "ExitlistViewEx";
            this.ExitlistViewEx.UseCompatibleStateImageBehavior = false;
            this.ExitlistViewEx.View = System.Windows.Forms.View.Details;
            this.ExitlistViewEx.SubItemClicked += new ListViewEx.SubItemEventHandler(this.ExitlistViewEx_SubItemClicked);
            this.ExitlistViewEx.SubItemEndEditing += new ListViewEx.SubItemEndEditingEventHandler(this.ExitlistViewEx_SubItemEndEditing);
            this.ExitlistViewEx.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ExitlistViewEx_MouseDoubleClick);
            this.ExitlistViewEx.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ExitlistViewEx_MouseDown);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // 停止测试ToolStripMenuItem
            // 
            resources.ApplyResources(this.停止测试ToolStripMenuItem, "停止测试ToolStripMenuItem");
            this.停止测试ToolStripMenuItem.Name = "停止测试ToolStripMenuItem";
            // 
            // ExitInfoSendbutton
            // 
            resources.ApplyResources(this.ExitInfoSendbutton, "ExitInfoSendbutton");
            this.ExitInfoSendbutton.Name = "ExitInfoSendbutton";
            this.ExitInfoSendbutton.UseVisualStyleBackColor = true;
            this.ExitInfoSendbutton.Click += new System.EventHandler(this.ExitInfoSendbutton_Click);
            // 
            // ElectromagnetictestForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ExitInfoSendbutton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.DistancenumericUpDown);
            this.Controls.Add(this.Outletlabel);
            this.Controls.Add(this.OffsetnumericUpDown);
            this.Controls.Add(this.ExitlistViewEx);
            this.Controls.Add(this.btnClosed);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ElectromagnetictestForm";
            this.Load += new System.EventHandler(this.ElectromagnetictestForm_Load);
            this.VolveTestcontextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DistancenumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetnumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip VolveTestcontextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem 单机测试ToolStripMenuItem;
        private System.Windows.Forms.Button btnClosed;
        private System.Windows.Forms.NumericUpDown DistancenumericUpDown;
        private System.Windows.Forms.Label Outletlabel;
        private System.Windows.Forms.NumericUpDown OffsetnumericUpDown;
        private ListViewEx.ListViewEx ExitlistViewEx;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem 停止测试ToolStripMenuItem;
        private System.Windows.Forms.Button ExitInfoSendbutton;
    }
}