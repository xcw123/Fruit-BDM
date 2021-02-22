namespace FruitSortingVtest1
{
    partial class StatisticsInfoForm1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatisticsInfoForm1));
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnPrint = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.PicExport = new System.Windows.Forms.PictureBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.hScrollBar2 = new System.Windows.Forms.HScrollBar();
            this.PicSize = new System.Windows.Forms.PictureBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.hScrollBar3 = new System.Windows.Forms.HScrollBar();
            this.PicBox = new System.Windows.Forms.PictureBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.LvwFruitData = new ListViewEx.ListViewEx();
            this.LblExcelEndTime = new System.Windows.Forms.Label();
            this.LblExcelStartTime = new System.Windows.Forms.Label();
            this.LblPrintProgramName = new System.Windows.Forms.Label();
            this.LblMainReportCartonsPer = new System.Windows.Forms.Label();
            this.LblMainReportWeights = new System.Windows.Forms.Label();
            this.LblMainReportWeightPer = new System.Windows.Forms.Label();
            this.LblPrintSubTotal = new System.Windows.Forms.Label();
            this.LblPrintPages = new System.Windows.Forms.Label();
            this.LblPrintQualityName = new System.Windows.Forms.Label();
            this.LblPrintNumOrPercent = new System.Windows.Forms.Label();
            this.LblPrintGName = new System.Windows.Forms.Label();
            this.LblPrintTName = new System.Windows.Forms.Label();
            this.LblPrintAveFruitWeight = new System.Windows.Forms.Label();
            this.LblPrintTotalWeight = new System.Windows.Forms.Label();
            this.LblPrintTotalCartons = new System.Windows.Forms.Label();
            this.LblPrintTotalPieces = new System.Windows.Forms.Label();
            this.LblPrintFruitVarieties = new System.Windows.Forms.Label();
            this.LblPrintDateTime = new System.Windows.Forms.Label();
            this.LblPrintClassified = new System.Windows.Forms.Label();
            this.LblPrintBatchReport = new System.Windows.Forms.Label();
            this.LblPrintCustomerName = new System.Windows.Forms.Label();
            this.LblPrintFarmName = new System.Windows.Forms.Label();
            this.LblMainReportName = new System.Windows.Forms.Label();
            this.LblMainReportSize = new System.Windows.Forms.Label();
            this.LblMainReportPieces = new System.Windows.Forms.Label();
            this.LblMainReportPiecesPer = new System.Windows.Forms.Label();
            this.LblMainReportCartons = new System.Windows.Forms.Label();
            this.LblCustomerID = new System.Windows.Forms.Label();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.ExportExcelbutton = new System.Windows.Forms.Button();
            this.StatisticModebutton = new System.Windows.Forms.Button();
            this.TimerModebutton = new System.Windows.Forms.Button();
            this.LvwSizeOrWeightData = new ListViewEx.ListViewEx();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicExport)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicSize)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnOK
            // 
            resources.ApplyResources(this.BtnOK, "BtnOK");
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BtnPrint
            // 
            resources.ApplyResources(this.BtnPrint, "BtnPrint");
            this.BtnPrint.Name = "BtnPrint";
            this.BtnPrint.UseVisualStyleBackColor = true;
            this.BtnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.hScrollBar1);
            this.tabPage1.Controls.Add(this.PicExport);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // hScrollBar1
            // 
            resources.ApplyResources(this.hScrollBar1, "hScrollBar1");
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // PicExport
            // 
            this.PicExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.PicExport.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.PicExport, "PicExport");
            this.PicExport.Name = "PicExport";
            this.PicExport.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.hScrollBar2);
            this.tabPage2.Controls.Add(this.PicSize);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // hScrollBar2
            // 
            resources.ApplyResources(this.hScrollBar2, "hScrollBar2");
            this.hScrollBar2.Name = "hScrollBar2";
            this.hScrollBar2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar2_Scroll);
            // 
            // PicSize
            // 
            this.PicSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.PicSize.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.PicSize, "PicSize");
            this.PicSize.Name = "PicSize";
            this.PicSize.TabStop = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.hScrollBar3);
            this.tabPage3.Controls.Add(this.PicBox);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // hScrollBar3
            // 
            resources.ApplyResources(this.hScrollBar3, "hScrollBar3");
            this.hScrollBar3.Name = "hScrollBar3";
            this.hScrollBar3.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar3_Scroll);
            // 
            // PicBox
            // 
            this.PicBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.PicBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.PicBox, "PicBox");
            this.PicBox.Name = "PicBox";
            this.PicBox.TabStop = false;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.LvwSizeOrWeightData);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.LvwFruitData);
            this.tabPage5.Controls.Add(this.LblExcelEndTime);
            this.tabPage5.Controls.Add(this.LblExcelStartTime);
            this.tabPage5.Controls.Add(this.LblPrintProgramName);
            this.tabPage5.Controls.Add(this.LblMainReportCartonsPer);
            this.tabPage5.Controls.Add(this.LblMainReportWeights);
            this.tabPage5.Controls.Add(this.LblMainReportWeightPer);
            this.tabPage5.Controls.Add(this.LblPrintSubTotal);
            this.tabPage5.Controls.Add(this.LblPrintPages);
            this.tabPage5.Controls.Add(this.LblPrintQualityName);
            this.tabPage5.Controls.Add(this.LblPrintNumOrPercent);
            this.tabPage5.Controls.Add(this.LblPrintGName);
            this.tabPage5.Controls.Add(this.LblPrintTName);
            this.tabPage5.Controls.Add(this.LblPrintAveFruitWeight);
            this.tabPage5.Controls.Add(this.LblPrintTotalWeight);
            this.tabPage5.Controls.Add(this.LblPrintTotalCartons);
            this.tabPage5.Controls.Add(this.LblPrintTotalPieces);
            this.tabPage5.Controls.Add(this.LblPrintFruitVarieties);
            this.tabPage5.Controls.Add(this.LblPrintDateTime);
            this.tabPage5.Controls.Add(this.LblPrintClassified);
            this.tabPage5.Controls.Add(this.LblPrintBatchReport);
            this.tabPage5.Controls.Add(this.LblPrintCustomerName);
            this.tabPage5.Controls.Add(this.LblPrintFarmName);
            this.tabPage5.Controls.Add(this.LblMainReportName);
            this.tabPage5.Controls.Add(this.LblMainReportSize);
            this.tabPage5.Controls.Add(this.LblMainReportPieces);
            this.tabPage5.Controls.Add(this.LblMainReportPiecesPer);
            this.tabPage5.Controls.Add(this.LblMainReportCartons);
            this.tabPage5.Controls.Add(this.LblCustomerID);
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // LvwFruitData
            // 
            this.LvwFruitData.AllowColumnReorder = true;
            this.LvwFruitData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LvwFruitData.DoubleClickActivation = false;
            this.LvwFruitData.FullRowSelect = true;
            this.LvwFruitData.HideSelection = false;
            resources.ApplyResources(this.LvwFruitData, "LvwFruitData");
            this.LvwFruitData.Name = "LvwFruitData";
            this.LvwFruitData.UseCompatibleStateImageBehavior = false;
            this.LvwFruitData.View = System.Windows.Forms.View.Details;
            // 
            // LblExcelEndTime
            // 
            resources.ApplyResources(this.LblExcelEndTime, "LblExcelEndTime");
            this.LblExcelEndTime.Name = "LblExcelEndTime";
            // 
            // LblExcelStartTime
            // 
            resources.ApplyResources(this.LblExcelStartTime, "LblExcelStartTime");
            this.LblExcelStartTime.Name = "LblExcelStartTime";
            // 
            // LblPrintProgramName
            // 
            resources.ApplyResources(this.LblPrintProgramName, "LblPrintProgramName");
            this.LblPrintProgramName.Name = "LblPrintProgramName";
            // 
            // LblMainReportCartonsPer
            // 
            resources.ApplyResources(this.LblMainReportCartonsPer, "LblMainReportCartonsPer");
            this.LblMainReportCartonsPer.Name = "LblMainReportCartonsPer";
            // 
            // LblMainReportWeights
            // 
            resources.ApplyResources(this.LblMainReportWeights, "LblMainReportWeights");
            this.LblMainReportWeights.Name = "LblMainReportWeights";
            // 
            // LblMainReportWeightPer
            // 
            resources.ApplyResources(this.LblMainReportWeightPer, "LblMainReportWeightPer");
            this.LblMainReportWeightPer.Name = "LblMainReportWeightPer";
            // 
            // LblPrintSubTotal
            // 
            resources.ApplyResources(this.LblPrintSubTotal, "LblPrintSubTotal");
            this.LblPrintSubTotal.Name = "LblPrintSubTotal";
            // 
            // LblPrintPages
            // 
            resources.ApplyResources(this.LblPrintPages, "LblPrintPages");
            this.LblPrintPages.Name = "LblPrintPages";
            // 
            // LblPrintQualityName
            // 
            resources.ApplyResources(this.LblPrintQualityName, "LblPrintQualityName");
            this.LblPrintQualityName.Name = "LblPrintQualityName";
            // 
            // LblPrintNumOrPercent
            // 
            resources.ApplyResources(this.LblPrintNumOrPercent, "LblPrintNumOrPercent");
            this.LblPrintNumOrPercent.Name = "LblPrintNumOrPercent";
            // 
            // LblPrintGName
            // 
            resources.ApplyResources(this.LblPrintGName, "LblPrintGName");
            this.LblPrintGName.Name = "LblPrintGName";
            // 
            // LblPrintTName
            // 
            resources.ApplyResources(this.LblPrintTName, "LblPrintTName");
            this.LblPrintTName.Name = "LblPrintTName";
            // 
            // LblPrintAveFruitWeight
            // 
            resources.ApplyResources(this.LblPrintAveFruitWeight, "LblPrintAveFruitWeight");
            this.LblPrintAveFruitWeight.Name = "LblPrintAveFruitWeight";
            // 
            // LblPrintTotalWeight
            // 
            resources.ApplyResources(this.LblPrintTotalWeight, "LblPrintTotalWeight");
            this.LblPrintTotalWeight.Name = "LblPrintTotalWeight";
            // 
            // LblPrintTotalCartons
            // 
            resources.ApplyResources(this.LblPrintTotalCartons, "LblPrintTotalCartons");
            this.LblPrintTotalCartons.Name = "LblPrintTotalCartons";
            // 
            // LblPrintTotalPieces
            // 
            resources.ApplyResources(this.LblPrintTotalPieces, "LblPrintTotalPieces");
            this.LblPrintTotalPieces.Name = "LblPrintTotalPieces";
            // 
            // LblPrintFruitVarieties
            // 
            resources.ApplyResources(this.LblPrintFruitVarieties, "LblPrintFruitVarieties");
            this.LblPrintFruitVarieties.Name = "LblPrintFruitVarieties";
            // 
            // LblPrintDateTime
            // 
            resources.ApplyResources(this.LblPrintDateTime, "LblPrintDateTime");
            this.LblPrintDateTime.Name = "LblPrintDateTime";
            // 
            // LblPrintClassified
            // 
            resources.ApplyResources(this.LblPrintClassified, "LblPrintClassified");
            this.LblPrintClassified.Name = "LblPrintClassified";
            // 
            // LblPrintBatchReport
            // 
            resources.ApplyResources(this.LblPrintBatchReport, "LblPrintBatchReport");
            this.LblPrintBatchReport.Name = "LblPrintBatchReport";
            // 
            // LblPrintCustomerName
            // 
            resources.ApplyResources(this.LblPrintCustomerName, "LblPrintCustomerName");
            this.LblPrintCustomerName.Name = "LblPrintCustomerName";
            // 
            // LblPrintFarmName
            // 
            resources.ApplyResources(this.LblPrintFarmName, "LblPrintFarmName");
            this.LblPrintFarmName.Name = "LblPrintFarmName";
            // 
            // LblMainReportName
            // 
            resources.ApplyResources(this.LblMainReportName, "LblMainReportName");
            this.LblMainReportName.Name = "LblMainReportName";
            // 
            // LblMainReportSize
            // 
            resources.ApplyResources(this.LblMainReportSize, "LblMainReportSize");
            this.LblMainReportSize.Name = "LblMainReportSize";
            // 
            // LblMainReportPieces
            // 
            resources.ApplyResources(this.LblMainReportPieces, "LblMainReportPieces");
            this.LblMainReportPieces.Name = "LblMainReportPieces";
            // 
            // LblMainReportPiecesPer
            // 
            resources.ApplyResources(this.LblMainReportPiecesPer, "LblMainReportPiecesPer");
            this.LblMainReportPiecesPer.Name = "LblMainReportPiecesPer";
            // 
            // LblMainReportCartons
            // 
            resources.ApplyResources(this.LblMainReportCartons, "LblMainReportCartons");
            this.LblMainReportCartons.Name = "LblMainReportCartons";
            // 
            // LblCustomerID
            // 
            resources.ApplyResources(this.LblCustomerID, "LblCustomerID");
            this.LblCustomerID.Name = "LblCustomerID";
            // 
            // printDocument1
            // 
            this.printDocument1.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.printDocument1_BeginPrint);
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // printDialog1
            // 
            this.printDialog1.AllowCurrentPage = true;
            this.printDialog1.AllowSelection = true;
            this.printDialog1.AllowSomePages = true;
            this.printDialog1.UseEXDialog = true;
            // 
            // printPreviewDialog1
            // 
            resources.ApplyResources(this.printPreviewDialog1, "printPreviewDialog1");
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            // 
            // ExportExcelbutton
            // 
            resources.ApplyResources(this.ExportExcelbutton, "ExportExcelbutton");
            this.ExportExcelbutton.Name = "ExportExcelbutton";
            this.ExportExcelbutton.UseVisualStyleBackColor = true;
            this.ExportExcelbutton.Click += new System.EventHandler(this.ExportExcelbutton_Click);
            // 
            // StatisticModebutton
            // 
            resources.ApplyResources(this.StatisticModebutton, "StatisticModebutton");
            this.StatisticModebutton.Name = "StatisticModebutton";
            this.StatisticModebutton.UseVisualStyleBackColor = true;
            this.StatisticModebutton.Click += new System.EventHandler(this.StatisticModebutton_Click);
            // 
            // TimerModebutton
            // 
            resources.ApplyResources(this.TimerModebutton, "TimerModebutton");
            this.TimerModebutton.Name = "TimerModebutton";
            this.TimerModebutton.UseVisualStyleBackColor = true;
            this.TimerModebutton.Click += new System.EventHandler(this.TimerModebutton_Click);
            // 
            // LvwSizeOrWeightData
            // 
            this.LvwSizeOrWeightData.AllowColumnReorder = true;
            this.LvwSizeOrWeightData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LvwSizeOrWeightData.DoubleClickActivation = false;
            this.LvwSizeOrWeightData.FullRowSelect = true;
            this.LvwSizeOrWeightData.HideSelection = false;
            resources.ApplyResources(this.LvwSizeOrWeightData, "LvwSizeOrWeightData");
            this.LvwSizeOrWeightData.Name = "LvwSizeOrWeightData";
            this.LvwSizeOrWeightData.UseCompatibleStateImageBehavior = false;
            this.LvwSizeOrWeightData.View = System.Windows.Forms.View.Details;
            // 
            // StatisticsInfoForm1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TimerModebutton);
            this.Controls.Add(this.StatisticModebutton);
            this.Controls.Add(this.ExportExcelbutton);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.BtnPrint);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StatisticsInfoForm1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StatisticsInfoForm1_FormClosing);
            this.Load += new System.EventHandler(this.StatisticsInfoForm1_Load);
            this.SizeChanged += new System.EventHandler(this.StatisticsInfoForm1_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PicExport)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PicSize)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PicBox)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button BtnPrint;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.PictureBox PicExport;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.HScrollBar hScrollBar2;
        private System.Windows.Forms.PictureBox PicSize;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.HScrollBar hScrollBar3;
        private System.Windows.Forms.PictureBox PicBox;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.Label LblMainReportName;
        private System.Windows.Forms.Label LblMainReportSize;
        private System.Windows.Forms.Label LblMainReportPieces;
        private System.Windows.Forms.Label LblMainReportPiecesPer;
        private System.Windows.Forms.Label LblMainReportCartons;
        private System.Windows.Forms.Label LblPrintDateTime;
        private System.Windows.Forms.Label LblPrintClassified;
        private System.Windows.Forms.Label LblPrintBatchReport;
        private System.Windows.Forms.Label LblPrintCustomerName;
        private System.Windows.Forms.Label LblPrintFarmName;
        private System.Windows.Forms.Label LblPrintPages;
        private System.Windows.Forms.Label LblPrintQualityName;
        private System.Windows.Forms.Label LblPrintNumOrPercent;
        private System.Windows.Forms.Label LblPrintGName;
        private System.Windows.Forms.Label LblPrintTName;
        private System.Windows.Forms.Label LblPrintAveFruitWeight;
        private System.Windows.Forms.Label LblPrintTotalWeight;
        private System.Windows.Forms.Label LblPrintTotalCartons;
        private System.Windows.Forms.Label LblPrintTotalPieces;
        private System.Windows.Forms.Label LblPrintFruitVarieties;
        private System.Windows.Forms.Label LblPrintSubTotal;
        private System.Windows.Forms.Button ExportExcelbutton;
        private System.Windows.Forms.Label LblMainReportCartonsPer;
        private System.Windows.Forms.Label LblMainReportWeights;
        private System.Windows.Forms.Label LblMainReportWeightPer;
        private System.Windows.Forms.Label LblPrintProgramName;
        private System.Windows.Forms.Label LblExcelEndTime;
        private System.Windows.Forms.Label LblExcelStartTime;
        private System.Windows.Forms.Button StatisticModebutton;
        private System.Windows.Forms.Button TimerModebutton;
        private System.Windows.Forms.Label LblCustomerID;
        private System.Windows.Forms.TabPage tabPage4;
        private ListViewEx.ListViewEx LvwFruitData;
        private ListViewEx.ListViewEx LvwSizeOrWeightData;
    }
}