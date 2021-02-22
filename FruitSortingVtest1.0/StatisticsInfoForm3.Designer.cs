namespace FruitSortingVtest1
{
    partial class StatisticsInfoForm3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatisticsInfoForm3));
            this.BtnPrint = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.ExportExcelbutton = new System.Windows.Forms.Button();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.LblExcelEndTime = new System.Windows.Forms.Label();
            this.LblExcelStartTime = new System.Windows.Forms.Label();
            this.LblPrintProgramName = new System.Windows.Forms.Label();
            this.LblMainReportCartonsPer = new System.Windows.Forms.Label();
            this.LblMainReportWeights = new System.Windows.Forms.Label();
            this.LblMainReportWeightPer = new System.Windows.Forms.Label();
            this.LblMainReportPiecesPer = new System.Windows.Forms.Label();
            this.LblPrintColorReport = new System.Windows.Forms.Label();
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
            this.LblMainReportCartons = new System.Windows.Forms.Label();
            this.LblMainReportPieces = new System.Windows.Forms.Label();
            this.LblMainReportSize = new System.Windows.Forms.Label();
            this.LblMainReportName = new System.Windows.Forms.Label();
            this.LblCustomerID = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.LblQualityOfCases = new System.Windows.Forms.Label();
            this.LblQualityDimensionLevel = new System.Windows.Forms.Label();
            this.hScrollBar5 = new System.Windows.Forms.HScrollBar();
            this.CboSizeType = new System.Windows.Forms.ComboBox();
            this.LblQuality = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.hScrollBar4 = new System.Windows.Forms.HScrollBar();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.hScrollBar3 = new System.Windows.Forms.HScrollBar();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.hScrollBar2 = new System.Windows.Forms.HScrollBar();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.TimerModebutton = new System.Windows.Forms.Button();
            this.StatisticModebutton = new System.Windows.Forms.Button();
            this.LvwSizeOrWeightData = new ListViewEx.ListViewEx();
            this.PicExport = new System.Windows.Forms.PictureBox();
            this.PicQuality = new System.Windows.Forms.PictureBox();
            this.PicColor = new System.Windows.Forms.PictureBox();
            this.PicShape = new System.Windows.Forms.PictureBox();
            this.PicBox = new System.Windows.Forms.PictureBox();
            this.PicSize = new System.Windows.Forms.PictureBox();
            this.LvwFruitData = new ListViewEx.ListViewEx();
            this.tabPage7.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicExport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicQuality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicShape)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicSize)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnPrint
            // 
            resources.ApplyResources(this.BtnPrint, "BtnPrint");
            this.BtnPrint.Name = "BtnPrint";
            this.BtnPrint.UseVisualStyleBackColor = true;
            this.BtnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // BtnOK
            // 
            resources.ApplyResources(this.BtnOK, "BtnOK");
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // printDocument1
            // 
            this.printDocument1.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.printDocument1_BeginPrint);
            this.printDocument1.EndPrint += new System.Drawing.Printing.PrintEventHandler(this.printDocument1_EndPrint);
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
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.LvwFruitData);
            this.tabPage7.Controls.Add(this.LblExcelEndTime);
            this.tabPage7.Controls.Add(this.LblExcelStartTime);
            this.tabPage7.Controls.Add(this.LblPrintProgramName);
            this.tabPage7.Controls.Add(this.LblMainReportCartonsPer);
            this.tabPage7.Controls.Add(this.LblMainReportWeights);
            this.tabPage7.Controls.Add(this.LblMainReportWeightPer);
            this.tabPage7.Controls.Add(this.LblMainReportPiecesPer);
            this.tabPage7.Controls.Add(this.LblPrintColorReport);
            this.tabPage7.Controls.Add(this.LblPrintSubTotal);
            this.tabPage7.Controls.Add(this.LblPrintPages);
            this.tabPage7.Controls.Add(this.LblPrintQualityName);
            this.tabPage7.Controls.Add(this.LblPrintNumOrPercent);
            this.tabPage7.Controls.Add(this.LblPrintGName);
            this.tabPage7.Controls.Add(this.LblPrintTName);
            this.tabPage7.Controls.Add(this.LblPrintAveFruitWeight);
            this.tabPage7.Controls.Add(this.LblPrintTotalWeight);
            this.tabPage7.Controls.Add(this.LblPrintTotalCartons);
            this.tabPage7.Controls.Add(this.LblPrintTotalPieces);
            this.tabPage7.Controls.Add(this.LblPrintFruitVarieties);
            this.tabPage7.Controls.Add(this.LblPrintDateTime);
            this.tabPage7.Controls.Add(this.LblPrintClassified);
            this.tabPage7.Controls.Add(this.LblPrintBatchReport);
            this.tabPage7.Controls.Add(this.LblPrintCustomerName);
            this.tabPage7.Controls.Add(this.LblPrintFarmName);
            this.tabPage7.Controls.Add(this.LblMainReportCartons);
            this.tabPage7.Controls.Add(this.LblMainReportPieces);
            this.tabPage7.Controls.Add(this.LblMainReportSize);
            this.tabPage7.Controls.Add(this.LblMainReportName);
            this.tabPage7.Controls.Add(this.LblCustomerID);
            resources.ApplyResources(this.tabPage7, "tabPage7");
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.UseVisualStyleBackColor = true;
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
            // LblMainReportPiecesPer
            // 
            resources.ApplyResources(this.LblMainReportPiecesPer, "LblMainReportPiecesPer");
            this.LblMainReportPiecesPer.Name = "LblMainReportPiecesPer";
            // 
            // LblPrintColorReport
            // 
            resources.ApplyResources(this.LblPrintColorReport, "LblPrintColorReport");
            this.LblPrintColorReport.Name = "LblPrintColorReport";
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
            // LblMainReportCartons
            // 
            resources.ApplyResources(this.LblMainReportCartons, "LblMainReportCartons");
            this.LblMainReportCartons.Name = "LblMainReportCartons";
            // 
            // LblMainReportPieces
            // 
            resources.ApplyResources(this.LblMainReportPieces, "LblMainReportPieces");
            this.LblMainReportPieces.Name = "LblMainReportPieces";
            // 
            // LblMainReportSize
            // 
            resources.ApplyResources(this.LblMainReportSize, "LblMainReportSize");
            this.LblMainReportSize.Name = "LblMainReportSize";
            // 
            // LblMainReportName
            // 
            resources.ApplyResources(this.LblMainReportName, "LblMainReportName");
            this.LblMainReportName.Name = "LblMainReportName";
            // 
            // LblCustomerID
            // 
            resources.ApplyResources(this.LblCustomerID, "LblCustomerID");
            this.LblCustomerID.Name = "LblCustomerID";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.LblQualityOfCases);
            this.tabPage5.Controls.Add(this.LblQualityDimensionLevel);
            this.tabPage5.Controls.Add(this.hScrollBar5);
            this.tabPage5.Controls.Add(this.CboSizeType);
            this.tabPage5.Controls.Add(this.LblQuality);
            this.tabPage5.Controls.Add(this.PicBox);
            this.tabPage5.Controls.Add(this.PicSize);
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // LblQualityOfCases
            // 
            resources.ApplyResources(this.LblQualityOfCases, "LblQualityOfCases");
            this.LblQualityOfCases.Name = "LblQualityOfCases";
            // 
            // LblQualityDimensionLevel
            // 
            resources.ApplyResources(this.LblQualityDimensionLevel, "LblQualityDimensionLevel");
            this.LblQualityDimensionLevel.Name = "LblQualityDimensionLevel";
            // 
            // hScrollBar5
            // 
            resources.ApplyResources(this.hScrollBar5, "hScrollBar5");
            this.hScrollBar5.Name = "hScrollBar5";
            this.hScrollBar5.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar5_Scroll);
            // 
            // CboSizeType
            // 
            this.CboSizeType.FormattingEnabled = true;
            resources.ApplyResources(this.CboSizeType, "CboSizeType");
            this.CboSizeType.Name = "CboSizeType";
            this.CboSizeType.SelectedIndexChanged += new System.EventHandler(this.CboSizeType_SelectedIndexChanged);
            // 
            // LblQuality
            // 
            resources.ApplyResources(this.LblQuality, "LblQuality");
            this.LblQuality.Name = "LblQuality";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.hScrollBar4);
            this.tabPage4.Controls.Add(this.PicShape);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // hScrollBar4
            // 
            resources.ApplyResources(this.hScrollBar4, "hScrollBar4");
            this.hScrollBar4.Name = "hScrollBar4";
            this.hScrollBar4.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar4_Scroll);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.hScrollBar3);
            this.tabPage3.Controls.Add(this.PicColor);
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
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.hScrollBar2);
            this.tabPage2.Controls.Add(this.PicQuality);
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
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.LvwSizeOrWeightData);
            resources.ApplyResources(this.tabPage6, "tabPage6");
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // TimerModebutton
            // 
            resources.ApplyResources(this.TimerModebutton, "TimerModebutton");
            this.TimerModebutton.Name = "TimerModebutton";
            this.TimerModebutton.UseVisualStyleBackColor = true;
            this.TimerModebutton.Click += new System.EventHandler(this.TimerModebutton_Click);
            // 
            // StatisticModebutton
            // 
            resources.ApplyResources(this.StatisticModebutton, "StatisticModebutton");
            this.StatisticModebutton.Name = "StatisticModebutton";
            this.StatisticModebutton.UseVisualStyleBackColor = true;
            this.StatisticModebutton.Click += new System.EventHandler(this.StatisticModebutton_Click);
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
            // PicExport
            // 
            this.PicExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.PicExport.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.PicExport, "PicExport");
            this.PicExport.Name = "PicExport";
            this.PicExport.TabStop = false;
            // 
            // PicQuality
            // 
            this.PicQuality.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.PicQuality.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.PicQuality, "PicQuality");
            this.PicQuality.Name = "PicQuality";
            this.PicQuality.TabStop = false;
            // 
            // PicColor
            // 
            this.PicColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.PicColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.PicColor, "PicColor");
            this.PicColor.Name = "PicColor";
            this.PicColor.TabStop = false;
            // 
            // PicShape
            // 
            this.PicShape.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.PicShape.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.PicShape, "PicShape");
            this.PicShape.Name = "PicShape";
            this.PicShape.TabStop = false;
            // 
            // PicBox
            // 
            this.PicBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.PicBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.PicBox, "PicBox");
            this.PicBox.Name = "PicBox";
            this.PicBox.TabStop = false;
            // 
            // PicSize
            // 
            this.PicSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.PicSize.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.PicSize, "PicSize");
            this.PicSize.Name = "PicSize";
            this.PicSize.TabStop = false;
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
            // StatisticsInfoForm3
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
            this.Name = "StatisticsInfoForm3";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StatisticsInfoForm3_FormClosing);
            this.Load += new System.EventHandler(this.StatisticsInfoForm3_Load);
            this.SizeChanged += new System.EventHandler(this.StatisticsInfoForm3_SizeChanged);
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PicExport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicQuality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicShape)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnPrint;
        private System.Windows.Forms.Button BtnOK;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.Button ExportExcelbutton;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.Label LblExcelEndTime;
        private System.Windows.Forms.Label LblExcelStartTime;
        private System.Windows.Forms.Label LblPrintProgramName;
        private System.Windows.Forms.Label LblMainReportCartonsPer;
        private System.Windows.Forms.Label LblMainReportWeights;
        private System.Windows.Forms.Label LblMainReportWeightPer;
        private System.Windows.Forms.Label LblMainReportPiecesPer;
        private System.Windows.Forms.Label LblPrintColorReport;
        private System.Windows.Forms.Label LblPrintSubTotal;
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
        private System.Windows.Forms.Label LblPrintDateTime;
        private System.Windows.Forms.Label LblPrintClassified;
        private System.Windows.Forms.Label LblPrintBatchReport;
        private System.Windows.Forms.Label LblPrintCustomerName;
        private System.Windows.Forms.Label LblPrintFarmName;
        private System.Windows.Forms.Label LblMainReportCartons;
        private System.Windows.Forms.Label LblMainReportPieces;
        private System.Windows.Forms.Label LblMainReportSize;
        private System.Windows.Forms.Label LblMainReportName;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label LblQualityOfCases;
        private System.Windows.Forms.Label LblQualityDimensionLevel;
        private System.Windows.Forms.HScrollBar hScrollBar5;
        private System.Windows.Forms.ComboBox CboSizeType;
        private System.Windows.Forms.Label LblQuality;
        private System.Windows.Forms.PictureBox PicBox;
        private System.Windows.Forms.PictureBox PicSize;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.HScrollBar hScrollBar4;
        private System.Windows.Forms.PictureBox PicShape;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.HScrollBar hScrollBar3;
        private System.Windows.Forms.PictureBox PicColor;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.HScrollBar hScrollBar2;
        private System.Windows.Forms.PictureBox PicQuality;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.PictureBox PicExport;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button TimerModebutton;
        private System.Windows.Forms.Button StatisticModebutton;
        private System.Windows.Forms.Label LblCustomerID;
        private System.Windows.Forms.TabPage tabPage6;
        private ListViewEx.ListViewEx LvwSizeOrWeightData;
        private ListViewEx.ListViewEx LvwFruitData;
    }
}