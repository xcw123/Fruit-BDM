namespace FruitSortingVtest1
{
    partial class VolveTestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VolveTestForm));
            this.ChannelcomboBox = new System.Windows.Forms.ComboBox();
            this.StartTestbutton = new System.Windows.Forms.Button();
            this.Cancelbutton = new System.Windows.Forms.Button();
            this.Lanelabel = new System.Windows.Forms.Label();
            this.StopTestlabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkSidebySideTest = new System.Windows.Forms.CheckBox();
            this.DistancenumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.Outletlabel = new System.Windows.Forms.Label();
            this.OffsetnumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ChannelExitEffectbutton = new System.Windows.Forms.Button();
            this.ExitlistViewEx = new ListViewEx.ListViewEx();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ChannelSeleclabel = new System.Windows.Forms.Label();
            this.ChannelSeleccomboBox = new System.Windows.Forms.ComboBox();
            this.VolveTestcontextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Start = new System.Windows.Forms.ToolStripMenuItem();
            this.Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.EffectButtonDelaytimer3 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DistancenumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetnumericUpDown)).BeginInit();
            this.VolveTestcontextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChannelcomboBox
            // 
            resources.ApplyResources(this.ChannelcomboBox, "ChannelcomboBox");
            this.ChannelcomboBox.FormattingEnabled = true;
            this.ChannelcomboBox.Name = "ChannelcomboBox";
            this.ChannelcomboBox.SelectedIndexChanged += new System.EventHandler(this.ChannelcomboBox_SelectedIndexChanged);
            this.ChannelcomboBox.Click += new System.EventHandler(this.ChannelcomboBox_Click);
            // 
            // StartTestbutton
            // 
            resources.ApplyResources(this.StartTestbutton, "StartTestbutton");
            this.StartTestbutton.Name = "StartTestbutton";
            this.StartTestbutton.UseVisualStyleBackColor = true;
            this.StartTestbutton.Click += new System.EventHandler(this.StartTestbutton_Click);
            // 
            // Cancelbutton
            // 
            resources.ApplyResources(this.Cancelbutton, "Cancelbutton");
            this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancelbutton.Name = "Cancelbutton";
            this.Cancelbutton.UseVisualStyleBackColor = true;
            // 
            // Lanelabel
            // 
            resources.ApplyResources(this.Lanelabel, "Lanelabel");
            this.Lanelabel.Name = "Lanelabel";
            // 
            // StopTestlabel
            // 
            resources.ApplyResources(this.StopTestlabel, "StopTestlabel");
            this.StopTestlabel.Name = "StopTestlabel";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.Lanelabel);
            this.groupBox1.Controls.Add(this.StopTestlabel);
            this.groupBox1.Controls.Add(this.ChannelcomboBox);
            this.groupBox1.Controls.Add(this.StartTestbutton);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.chkSidebySideTest);
            this.groupBox2.Controls.Add(this.DistancenumericUpDown);
            this.groupBox2.Controls.Add(this.Outletlabel);
            this.groupBox2.Controls.Add(this.OffsetnumericUpDown);
            this.groupBox2.Controls.Add(this.ChannelExitEffectbutton);
            this.groupBox2.Controls.Add(this.ExitlistViewEx);
            this.groupBox2.Controls.Add(this.ChannelSeleclabel);
            this.groupBox2.Controls.Add(this.ChannelSeleccomboBox);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // chkSidebySideTest
            // 
            resources.ApplyResources(this.chkSidebySideTest, "chkSidebySideTest");
            this.chkSidebySideTest.Name = "chkSidebySideTest";
            this.chkSidebySideTest.UseVisualStyleBackColor = true;
            // 
            // DistancenumericUpDown
            // 
            resources.ApplyResources(this.DistancenumericUpDown, "DistancenumericUpDown");
            this.DistancenumericUpDown.Maximum = new decimal(new int[] {
            10000,
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
            this.OffsetnumericUpDown.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.OffsetnumericUpDown.Name = "OffsetnumericUpDown";
            // 
            // ChannelExitEffectbutton
            // 
            resources.ApplyResources(this.ChannelExitEffectbutton, "ChannelExitEffectbutton");
            this.ChannelExitEffectbutton.Name = "ChannelExitEffectbutton";
            this.ChannelExitEffectbutton.UseVisualStyleBackColor = true;
            this.ChannelExitEffectbutton.Click += new System.EventHandler(this.ChannelExitEffectbutton_Click);
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
            // ChannelSeleclabel
            // 
            resources.ApplyResources(this.ChannelSeleclabel, "ChannelSeleclabel");
            this.ChannelSeleclabel.Name = "ChannelSeleclabel";
            // 
            // ChannelSeleccomboBox
            // 
            resources.ApplyResources(this.ChannelSeleccomboBox, "ChannelSeleccomboBox");
            this.ChannelSeleccomboBox.FormattingEnabled = true;
            this.ChannelSeleccomboBox.Name = "ChannelSeleccomboBox";
            this.ChannelSeleccomboBox.SelectionChangeCommitted += new System.EventHandler(this.ChannelSeleccomboBox_SelectionChangeCommitted);
            // 
            // VolveTestcontextMenuStrip
            // 
            resources.ApplyResources(this.VolveTestcontextMenuStrip, "VolveTestcontextMenuStrip");
            this.VolveTestcontextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.VolveTestcontextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Start,
            this.Stop});
            this.VolveTestcontextMenuStrip.Name = "VolveTestcontextMenuStrip";
            // 
            // Start
            // 
            resources.ApplyResources(this.Start, "Start");
            this.Start.Name = "Start";
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // Stop
            // 
            resources.ApplyResources(this.Stop, "Stop");
            this.Stop.Name = "Stop";
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // EffectButtonDelaytimer3
            // 
            this.EffectButtonDelaytimer3.Interval = 1500;
            this.EffectButtonDelaytimer3.Tick += new System.EventHandler(this.EffectButtonDelaytimer3_Tick);
            // 
            // VolveTestForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Cancelbutton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VolveTestForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.VolveTestForm_FormClosed);
            this.Load += new System.EventHandler(this.VolveTestForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DistancenumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetnumericUpDown)).EndInit();
            this.VolveTestcontextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ChannelcomboBox;
        private System.Windows.Forms.Button StartTestbutton;
        private System.Windows.Forms.Button Cancelbutton;
        private System.Windows.Forms.Label Lanelabel;
        private System.Windows.Forms.Label StopTestlabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label ChannelSeleclabel;
        private System.Windows.Forms.ComboBox ChannelSeleccomboBox;
        private ListViewEx.ListViewEx ExitlistViewEx;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button ChannelExitEffectbutton;
        private System.Windows.Forms.NumericUpDown OffsetnumericUpDown;
        private System.Windows.Forms.Label Outletlabel;
        private System.Windows.Forms.ContextMenuStrip VolveTestcontextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem Start;
        private System.Windows.Forms.ToolStripMenuItem Stop;
        private System.Windows.Forms.Timer EffectButtonDelaytimer3;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.NumericUpDown DistancenumericUpDown;
        private System.Windows.Forms.CheckBox chkSidebySideTest;
    }
}