namespace FruitSortingVtest1._0
{
    partial class WaveCaptureForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WaveCaptureForm));
            this.WaveCapturehScrollBar = new System.Windows.Forms.HScrollBar();
            this.Intervallabel2 = new System.Windows.Forms.Label();
            this.Intervallabel = new System.Windows.Forms.Label();
            this.Capturedbutton = new System.Windows.Forms.Button();
            this.WaveCaptureOKbutton = new System.Windows.Forms.Button();
            this.WaveCaptureCancelbutton = new System.Windows.Forms.Button();
            this.StartCapturedlabel = new System.Windows.Forms.Label();
            this.WeightWavepictureBox = new System.Windows.Forms.PictureBox();
            this.AD0label = new System.Windows.Forms.Label();
            this.AD0mintextBox = new System.Windows.Forms.TextBox();
            this.AD0maxtextBox = new System.Windows.Forms.TextBox();
            this.AD1maxtextBox = new System.Windows.Forms.TextBox();
            this.AD1mintextBox = new System.Windows.Forms.TextBox();
            this.AD1label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.IntervalnumericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.IntervalnumericUpDown2 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.WeightWavepictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalnumericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalnumericUpDown2)).BeginInit();
            this.SuspendLayout();
            // 
            // WaveCapturehScrollBar
            // 
            resources.ApplyResources(this.WaveCapturehScrollBar, "WaveCapturehScrollBar");
            this.WaveCapturehScrollBar.LargeChange = 1;
            this.WaveCapturehScrollBar.Maximum = 0;
            this.WaveCapturehScrollBar.Name = "WaveCapturehScrollBar";
            this.WaveCapturehScrollBar.TabStop = true;
            this.WaveCapturehScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.WaveCapturehScrollBar_Scroll);
            // 
            // Intervallabel2
            // 
            resources.ApplyResources(this.Intervallabel2, "Intervallabel2");
            this.Intervallabel2.Name = "Intervallabel2";
            // 
            // Intervallabel
            // 
            resources.ApplyResources(this.Intervallabel, "Intervallabel");
            this.Intervallabel.Name = "Intervallabel";
            // 
            // Capturedbutton
            // 
            resources.ApplyResources(this.Capturedbutton, "Capturedbutton");
            this.Capturedbutton.Name = "Capturedbutton";
            this.Capturedbutton.UseVisualStyleBackColor = true;
            this.Capturedbutton.Click += new System.EventHandler(this.Capturedbutton_Click);
            // 
            // WaveCaptureOKbutton
            // 
            this.WaveCaptureOKbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.WaveCaptureOKbutton, "WaveCaptureOKbutton");
            this.WaveCaptureOKbutton.Name = "WaveCaptureOKbutton";
            this.WaveCaptureOKbutton.UseVisualStyleBackColor = true;
            this.WaveCaptureOKbutton.Click += new System.EventHandler(this.WaveCaptureOKbutton_Click);
            // 
            // WaveCaptureCancelbutton
            // 
            this.WaveCaptureCancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.WaveCaptureCancelbutton, "WaveCaptureCancelbutton");
            this.WaveCaptureCancelbutton.Name = "WaveCaptureCancelbutton";
            this.WaveCaptureCancelbutton.UseVisualStyleBackColor = true;
            // 
            // StartCapturedlabel
            // 
            resources.ApplyResources(this.StartCapturedlabel, "StartCapturedlabel");
            this.StartCapturedlabel.Name = "StartCapturedlabel";
            // 
            // WeightWavepictureBox
            // 
            this.WeightWavepictureBox.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.WeightWavepictureBox, "WeightWavepictureBox");
            this.WeightWavepictureBox.Name = "WeightWavepictureBox";
            this.WeightWavepictureBox.TabStop = false;
            this.WeightWavepictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.WeightWavepictureBox_Paint);
            this.WeightWavepictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WeightWavepictureBox_MouseDown);
            this.WeightWavepictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.WeightWavepictureBox_MouseMove);
            this.WeightWavepictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.WeightWavepictureBox_MouseUp);
            // 
            // AD0label
            // 
            resources.ApplyResources(this.AD0label, "AD0label");
            this.AD0label.Name = "AD0label";
            // 
            // AD0mintextBox
            // 
            resources.ApplyResources(this.AD0mintextBox, "AD0mintextBox");
            this.AD0mintextBox.Name = "AD0mintextBox";
            this.AD0mintextBox.ReadOnly = true;
            // 
            // AD0maxtextBox
            // 
            resources.ApplyResources(this.AD0maxtextBox, "AD0maxtextBox");
            this.AD0maxtextBox.Name = "AD0maxtextBox";
            this.AD0maxtextBox.ReadOnly = true;
            // 
            // AD1maxtextBox
            // 
            resources.ApplyResources(this.AD1maxtextBox, "AD1maxtextBox");
            this.AD1maxtextBox.Name = "AD1maxtextBox";
            this.AD1maxtextBox.ReadOnly = true;
            // 
            // AD1mintextBox
            // 
            resources.ApplyResources(this.AD1mintextBox, "AD1mintextBox");
            this.AD1mintextBox.Name = "AD1mintextBox";
            this.AD1mintextBox.ReadOnly = true;
            // 
            // AD1label
            // 
            resources.ApplyResources(this.AD1label, "AD1label");
            this.AD1label.Name = "AD1label";
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
            // IntervalnumericUpDown1
            // 
            resources.ApplyResources(this.IntervalnumericUpDown1, "IntervalnumericUpDown1");
            this.IntervalnumericUpDown1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.IntervalnumericUpDown1.Name = "IntervalnumericUpDown1";
            this.IntervalnumericUpDown1.ValueChanged += new System.EventHandler(this.IntervalnumericUpDown1_ValueChanged);
            this.IntervalnumericUpDown1.Enter += new System.EventHandler(this.IntervalnumericUpDown1_Enter);
            // 
            // IntervalnumericUpDown2
            // 
            resources.ApplyResources(this.IntervalnumericUpDown2, "IntervalnumericUpDown2");
            this.IntervalnumericUpDown2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.IntervalnumericUpDown2.Name = "IntervalnumericUpDown2";
            this.IntervalnumericUpDown2.ValueChanged += new System.EventHandler(this.IntervalnumericUpDown2_ValueChanged);
            this.IntervalnumericUpDown2.Enter += new System.EventHandler(this.IntervalnumericUpDown2_Enter);
            // 
            // WaveCaptureForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.IntervalnumericUpDown2);
            this.Controls.Add(this.IntervalnumericUpDown1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AD1maxtextBox);
            this.Controls.Add(this.AD1mintextBox);
            this.Controls.Add(this.AD1label);
            this.Controls.Add(this.AD0maxtextBox);
            this.Controls.Add(this.AD0mintextBox);
            this.Controls.Add(this.AD0label);
            this.Controls.Add(this.StartCapturedlabel);
            this.Controls.Add(this.WaveCaptureCancelbutton);
            this.Controls.Add(this.WaveCaptureOKbutton);
            this.Controls.Add(this.Capturedbutton);
            this.Controls.Add(this.Intervallabel);
            this.Controls.Add(this.Intervallabel2);
            this.Controls.Add(this.WaveCapturehScrollBar);
            this.Controls.Add(this.WeightWavepictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaveCaptureForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WaveCaptureForm_FormClosing);
            this.Load += new System.EventHandler(this.WaveCaptureForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.WeightWavepictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalnumericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalnumericUpDown2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox WeightWavepictureBox;
        private System.Windows.Forms.HScrollBar WaveCapturehScrollBar;
        private System.Windows.Forms.Label Intervallabel2;
        private System.Windows.Forms.Label Intervallabel;
        private System.Windows.Forms.Button Capturedbutton;
        private System.Windows.Forms.Button WaveCaptureOKbutton;
        private System.Windows.Forms.Button WaveCaptureCancelbutton;
        private System.Windows.Forms.Label StartCapturedlabel;
        private System.Windows.Forms.Label AD0label;
        private System.Windows.Forms.TextBox AD0mintextBox;
        private System.Windows.Forms.TextBox AD0maxtextBox;
        private System.Windows.Forms.TextBox AD1maxtextBox;
        private System.Windows.Forms.TextBox AD1mintextBox;
        private System.Windows.Forms.Label AD1label;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown IntervalnumericUpDown1;
        private System.Windows.Forms.NumericUpDown IntervalnumericUpDown2;
    }
}