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
            this.IntervaltextBox1 = new System.Windows.Forms.TextBox();
            this.IntervaltextBox2 = new System.Windows.Forms.TextBox();
            this.Intervallabel2 = new System.Windows.Forms.Label();
            this.Intervallabel = new System.Windows.Forms.Label();
            this.Capturedbutton = new System.Windows.Forms.Button();
            this.WaveCaptureOKbutton = new System.Windows.Forms.Button();
            this.WaveCaptureCancelbutton = new System.Windows.Forms.Button();
            this.StartCapturedlabel = new System.Windows.Forms.Label();
            this.WeightWavepictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.WeightWavepictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // WaveCapturehScrollBar
            // 
            this.WaveCapturehScrollBar.LargeChange = 1;
            resources.ApplyResources(this.WaveCapturehScrollBar, "WaveCapturehScrollBar");
            this.WaveCapturehScrollBar.Maximum = 0;
            this.WaveCapturehScrollBar.Name = "WaveCapturehScrollBar";
            this.WaveCapturehScrollBar.TabStop = true;
            this.WaveCapturehScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.WaveCapturehScrollBar_Scroll);
            // 
            // IntervaltextBox1
            // 
            resources.ApplyResources(this.IntervaltextBox1, "IntervaltextBox1");
            this.IntervaltextBox1.Name = "IntervaltextBox1";
            this.IntervaltextBox1.ReadOnly = true;
            // 
            // IntervaltextBox2
            // 
            resources.ApplyResources(this.IntervaltextBox2, "IntervaltextBox2");
            this.IntervaltextBox2.Name = "IntervaltextBox2";
            this.IntervaltextBox2.ReadOnly = true;
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
            // WaveCaptureForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StartCapturedlabel);
            this.Controls.Add(this.WaveCaptureCancelbutton);
            this.Controls.Add(this.WaveCaptureOKbutton);
            this.Controls.Add(this.Capturedbutton);
            this.Controls.Add(this.Intervallabel);
            this.Controls.Add(this.Intervallabel2);
            this.Controls.Add(this.IntervaltextBox2);
            this.Controls.Add(this.IntervaltextBox1);
            this.Controls.Add(this.WaveCapturehScrollBar);
            this.Controls.Add(this.WeightWavepictureBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaveCaptureForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WaveCaptureForm_FormClosing);
            this.Load += new System.EventHandler(this.WaveCaptureForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.WeightWavepictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox WeightWavepictureBox;
        private System.Windows.Forms.HScrollBar WaveCapturehScrollBar;
        private System.Windows.Forms.TextBox IntervaltextBox1;
        private System.Windows.Forms.TextBox IntervaltextBox2;
        private System.Windows.Forms.Label Intervallabel2;
        private System.Windows.Forms.Label Intervallabel;
        private System.Windows.Forms.Button Capturedbutton;
        private System.Windows.Forms.Button WaveCaptureOKbutton;
        private System.Windows.Forms.Button WaveCaptureCancelbutton;
        private System.Windows.Forms.Label StartCapturedlabel;
    }
}