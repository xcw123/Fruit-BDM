using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FruitSortingVtest1.DB;
using Interface;
using FruitSortingVtest1._0;
using System.Diagnostics;
using Draw3DBarGraph;
using DrawPieGraph;
using System.Resources;
using System.IO;
using System.Threading;
using Common;

namespace FruitSortingVtest1
{
    public partial class StatisticsInfoForm3 : Form
    {
        private DataInterface dataInterface;
        private Boolean bIsSourceDB;
        private int currentSelectIndex;

        public int intPage = 0;             //打印等级统计表总页数
        public int intPage1 = 0;             //打印重量统计表总页数 add by xcw 20201106
        public int currentPageIndex = 1;    //当前打印页
        public Boolean bIsHaveColorStatis;  //是否有颜色统计
        public Boolean bIsTwoQualityOnePage;//是否每页最多可放两个品质
        public Boolean bLastPageHaveQuality;//最后一页是否还有品质

        private ResourceManager m_resourceManager = new ResourceManager(typeof(StatisticsInfoForm3));//创建Mainform资源管理
        private bool bIsTimerMode = false;         //是否为定时模式
        private stStatistics OneMinuteBeforeStatisticsData;  //1min之前的Statistics的数据

        //AutoSizeFormClass asc = new AutoSizeFormClass();//声明大小自适应类实例 

        private Bitmap bitM1 = null; //将局部变量改为全局变量，便于窗体退出时资源释放 Add by ChengSk - 20180815
        private Graphics g1;
        private Bitmap bitM2 = null;
        private Graphics g2;
        private Bitmap bitM3 = null;
        private Graphics g3;
        private Bitmap bitM4 = null;
        private Graphics g4;
        private Bitmap bitM51 = null;
        private Graphics g51;
        private Bitmap bitM52 = null;
        private Graphics g52;

        public StatisticsInfoForm3()
        {
            InitializeComponent();
        }

        public StatisticsInfoForm3(DataInterface dataInterface1)
        {
            if (dataInterface1.BSourceDB)
            {
                bIsSourceDB = true;
            }
            else
            {
                bIsSourceDB = false;
            }
            this.dataInterface = dataInterface1;
            InitializeComponent();
            GlobalDataInterface.UpdateDataInterfaceEvent += new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent);

            //asc.controllInitializeSize(this);
        }

        private void OnUpdateDataInterfaceEvent(DataInterface dataInterface1)
        {
            try
            {
                if (bIsSourceDB)
                {
                    return;
                }
                if (InvokeRequired)
                {
                    //Thread.Sleep(1000); //Note by ChengSk - 20180702
                    BeginInvoke(new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent), dataInterface1);
                    return;
                }
            }
            catch (Exception ex) { }

            if (!bIsTimerMode) //正常刷新模式
            {
                this.dataInterface = dataInterface1;
            }
            else   //短时刷新模式
            {
                this.dataInterface = Commonfunction.GetDifferDataInterface(OneMinuteBeforeStatisticsData, dataInterface1);  
            }

            if (this.tabControl1.SelectedIndex != 7)
                this.tabControl1_SelectedIndexChanged(this, new EventArgs());
            else
            {
                FillcurrentSelectTabpage6();
                FillcurrentSelectTabpage7();
            }
               
            
        }

        private void StatisticsInfoForm3_Load(object sender, EventArgs e)
        {
            //加载时默认选中TabPage1
            currentSelectTabpage1();
            //
            currentSelectIndex = 0;

            #region ListView标题栏设置
            LvwFruitData.Columns.Add("GradeName", m_resourceManager.GetString("LblMainReportName.Text"));
            LvwFruitData.Columns["GradeName"].Width = 100;
            LvwFruitData.Columns["GradeName"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("SizeLimit", m_resourceManager.GetString("LblMainReportSize.Text"));
            LvwFruitData.Columns["SizeLimit"].Width = 100; //100
            LvwFruitData.Columns["SizeLimit"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("GradeTotalNum", m_resourceManager.GetString("LblMainReportPieces.Text"));
            LvwFruitData.Columns["GradeTotalNum"].Width = 100;
            LvwFruitData.Columns["GradeTotalNum"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("NumPercent", m_resourceManager.GetString("LblMainReportPiecesPer.Text"));
            LvwFruitData.Columns["NumPercent"].Width = 100; //100
            LvwFruitData.Columns["NumPercent"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("GradeTotalWeight", m_resourceManager.GetString("LblMainReportWeights.Text")+"(kg)");
            LvwFruitData.Columns["GradeTotalWeight"].Width = 100;
            LvwFruitData.Columns["GradeTotalWeight"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("WeightPercent", m_resourceManager.GetString("LblMainReportWeightPer.Text"));
            LvwFruitData.Columns["WeightPercent"].Width = 100; //100
            LvwFruitData.Columns["WeightPercent"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("BoxNumber", m_resourceManager.GetString("LblMainReportCartons.Text"));
            LvwFruitData.Columns["BoxNumber"].Width = 100; //100
            LvwFruitData.Columns["BoxNumber"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("BoxPercent", m_resourceManager.GetString("LblMainReportCartonsPer.Text"));
            LvwFruitData.Columns["BoxPercent"].Width = 100; //100
            LvwFruitData.Columns["BoxPercent"].TextAlign = HorizontalAlignment.Center;
            #endregion
            #region ListView标题栏设置
            LvwSizeOrWeightData.Columns.Add("GradeName", m_resourceManager.GetString("LblMainReportName.Text"));
            LvwSizeOrWeightData.Columns["GradeName"].Width = 100;
            LvwSizeOrWeightData.Columns["GradeName"].TextAlign = HorizontalAlignment.Center;
            LvwSizeOrWeightData.Columns.Add("SizeLimit", m_resourceManager.GetString("LblMainReportSize.Text"));
            LvwSizeOrWeightData.Columns["SizeLimit"].Width = 100; //100
            LvwSizeOrWeightData.Columns["SizeLimit"].TextAlign = HorizontalAlignment.Center;
            LvwSizeOrWeightData.Columns.Add("GradeTotalNum", m_resourceManager.GetString("LblMainReportPieces.Text"));
            LvwSizeOrWeightData.Columns["GradeTotalNum"].Width = 100;
            LvwSizeOrWeightData.Columns["GradeTotalNum"].TextAlign = HorizontalAlignment.Center;
            LvwSizeOrWeightData.Columns.Add("NumPercent", m_resourceManager.GetString("LblMainReportPiecesPer.Text"));
            LvwSizeOrWeightData.Columns["NumPercent"].Width = 100; //100
            LvwSizeOrWeightData.Columns["NumPercent"].TextAlign = HorizontalAlignment.Center;
            LvwSizeOrWeightData.Columns.Add("GradeTotalWeight", m_resourceManager.GetString("LblMainReportWeights.Text") + "(kg)");
            LvwSizeOrWeightData.Columns["GradeTotalWeight"].Width = 100;
            LvwSizeOrWeightData.Columns["GradeTotalWeight"].TextAlign = HorizontalAlignment.Center;
            LvwSizeOrWeightData.Columns.Add("WeightPercent", m_resourceManager.GetString("LblMainReportWeightPer.Text"));
            LvwSizeOrWeightData.Columns["WeightPercent"].Width = 100; //100
            LvwSizeOrWeightData.Columns["WeightPercent"].TextAlign = HorizontalAlignment.Center;
            LvwSizeOrWeightData.Columns.Add("BoxNumber", m_resourceManager.GetString("LblMainReportCartons.Text"));
            LvwSizeOrWeightData.Columns["BoxNumber"].Width = 100; //100
            LvwSizeOrWeightData.Columns["BoxNumber"].TextAlign = HorizontalAlignment.Center;
            LvwSizeOrWeightData.Columns.Add("BoxPercent", m_resourceManager.GetString("LblMainReportCartonsPer.Text"));
            LvwSizeOrWeightData.Columns["BoxPercent"].Width = 100; //100
            LvwSizeOrWeightData.Columns["BoxPercent"].TextAlign = HorizontalAlignment.Center;
            #endregion
        }

        private void currentSelectTabpage1()
        {
            #region 变量声明
            int CylinderHeight1;      //柱高
            bool bHaveHscrollBar;     //是否有滚动条
            int leftRightSpace = 0;   //柱体离左右边框距离
            UInt64 uMaxValue = 0;     //各出口"个数"最大值
            UInt64 uSumValue = 0;     //各出口"个数"总值
            #endregion

            #region 界面设定
            if ((dataInterface.ExportSum - 1) * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace) +
                DrawGraphProtocol.CylinderWidth < tabPage1.Width)  //无需使用滚动条
            {
                bHaveHscrollBar = false;
                hScrollBar1.Visible = false;
                PicExport.Dock = DockStyle.Fill;
                CylinderHeight1 = DrawGraphProtocol.CylinderHeight2;
                leftRightSpace = (PicExport.Width - (dataInterface.ExportSum - 1) * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace) -
                    DrawGraphProtocol.CylinderWidth) / 2;
            }
            else  //需要使用滚动条
            {
                bHaveHscrollBar = true;
                PicExport.Width = (dataInterface.ExportSum - 1) * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace) +
                    DrawGraphProtocol.CylinderWidth + 2 * DrawGraphProtocol.LeftRightSpace;
                hScrollBar1.Maximum = PicExport.Width - tabPage1.Width;
                CylinderHeight1 = DrawGraphProtocol.CylinderHeight1;
            }
            #endregion

            //Bitmap bitM = new Bitmap(PicExport.Width, PicExport.Height);
            //Graphics g = Graphics.FromImage(bitM);
            if(bitM1 == null) //Modify by ChengSk - 20180815
            {
                bitM1 = new Bitmap(PicExport.Width, PicExport.Height);
                g1 = Graphics.FromImage(bitM1);
            }
            g1.Clear(DrawGraphProtocol.myBarBackColor);
            //定义画刷
            Brush currentBrush = new SolidBrush(DrawGraphProtocol.myPenColor);
            Brush BarBrush = new SolidBrush(DrawGraphProtocol.myBarBrush1);

            #region 数量/百分比 出口统计信息
            //"数量百分比"字样
            //g.DrawString("数量/百分比", new Font(DrawGraphProtocol.Module1FontStyle, DrawGraphProtocol.Module1FontSize, FontStyle.Regular),
            //    currentBrush, DrawGraphProtocol.Module1LocationX, DrawGraphProtocol.Module1LocationY);
            //"出口统计信息"字样
            g1.DrawString(m_resourceManager.GetString("tabPage1.Text"), new Font(DrawGraphProtocol.Module3FontStyle, DrawGraphProtocol.Module3FontSize, FontStyle.Bold),
                currentBrush, PicExport.Width / 2 - DrawGraphProtocol.Module3MiddleDistance, PicExport.Height - DrawGraphProtocol.Module3BottomDistance);
            #endregion

            #region 出口序号 柱体 个数 百分比
            //"出口序号"字样  "柱体"图形显示  个数  百分比
            uMaxValue = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nExitCount);
            uSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nExitCount);
            for (int i = 0; i < dataInterface.ExportSum; i++)
            {
                if (bHaveHscrollBar)  //有滚动条时
                {
                    //"出口序号"字样
                    g1.DrawString((i + 1).ToString(), new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                    currentBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace), PicExport.Height - DrawGraphProtocol.Module4BottomDistance);

                    if (dataInterface.IoStStatistics.nExitCount[i] == 0)
                    {
                        //"柱体"图形显示
                        g1.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace),
                        PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g1.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace),
                        PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight1 * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight1 * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)));

                        if ((int)(CylinderHeight1 * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g1.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g1.RotateTransform(-90f);
                            g1.DrawString(dataInterface.IoStStatistics.nExitCount[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g1.ResetTransform();
                        }
                        else
                        {
                            g1.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight1 * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)));
                            g1.RotateTransform(-90f);
                            g1.DrawString(dataInterface.IoStStatistics.nExitCount[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g1.ResetTransform();
                        }
                     
                    }

                    if (uSumValue == 0)
                    {
                        //百分比
                        g1.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g1.DrawString(((double)dataInterface.IoStStatistics.nExitCount[i] / uSumValue).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight1 * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                }
                else  //无滚动条时
                {
                    //"出口序号"字样
                    g1.DrawString((i + 1).ToString(), new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                    currentBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace), PicExport.Height - DrawGraphProtocol.Module4BottomDistance);

                    if (dataInterface.IoStStatistics.nExitCount[i] == 0)
                    {
                        //"柱体"图形显示
                        g1.FillRectangle(BarBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace),
                        PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g1.FillRectangle(BarBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace),
                        PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight1 * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight1 * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)));

                        if ((int)(CylinderHeight1 * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g1.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g1.RotateTransform(-90f);
                            g1.DrawString(dataInterface.IoStStatistics.nExitCount[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g1.ResetTransform();
                        }
                        else
                        {
                            g1.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight1 * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)));
                            g1.RotateTransform(-90f);
                            g1.DrawString(dataInterface.IoStStatistics.nExitCount[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g1.ResetTransform();
                        }

                        
                    }

                    if (uSumValue == 0)
                    {
                        //百分比
                        g1.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g1.DrawString(((double)dataInterface.IoStStatistics.nExitCount[i] / uSumValue).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight1 * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                }
            }
            #endregion

            PicExport.BackgroundImage = null;  //无此行代码不会刷新，必须加上 Add by ChengSk - 20180821
            PicExport.BackgroundImage = bitM1;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            PicExport.Left = -hScrollBar1.Value;
        } 

        private void currentSelectTabpage2()
        {
            #region 变量声明
            int CylinderHeight2;          //柱高
            bool bHaveHscrollBar;         //是否有滚动条
            int leftRightSpace = 0;       //柱体离左右边框距离
            UInt64 totalFruitNumber = 0;  //每品质水果的总个数
            UInt64 maxFruitNumber = 0;    //所有品质中水果总个数最大值
            #endregion

            #region 间隔计算
            int CurrentDisplayCylinderSpace = 0; //圆柱间间隔计算 Add by ChengSk - 2017/7/28
            int LongestWeightOrSizeGradeName = 0;
            LongestWeightOrSizeGradeName = DrawGraphProtocol.LongestWeightOrSizeGradeName(dataInterface.QualityGradeSum, dataInterface.IoStStGradeInfo.strQualityGradeName);
            CurrentDisplayCylinderSpace = (LongestWeightOrSizeGradeName > (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace)) ?
                (LongestWeightOrSizeGradeName - DrawGraphProtocol.CylinderWidth) : DrawGraphProtocol.CylinderSpace;//如果(最长等级名称 > 圆柱宽+标准空格)，则空格宽度重新计算
            #endregion

            #region 界面设定
            if ((dataInterface.QualityGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
                DrawGraphProtocol.CylinderWidth < tabPage2.Width)  //无需使用滚动条
            {
                bHaveHscrollBar = false;
                hScrollBar2.Visible = false;
                PicQuality.Dock = DockStyle.Fill;
                CylinderHeight2 = DrawGraphProtocol.CylinderHeight2;
                leftRightSpace = (PicQuality.Width - (dataInterface.QualityGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) -
                    DrawGraphProtocol.CylinderWidth) / 2;
            }
            else  //需要使用滚动条
            {
                bHaveHscrollBar = true;
                PicQuality.Width = (dataInterface.QualityGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
                    DrawGraphProtocol.CylinderWidth + 2 * DrawGraphProtocol.LeftRightSpace;
                hScrollBar2.Maximum = PicQuality.Width - tabPage2.Width;
                CylinderHeight2 = DrawGraphProtocol.CylinderHeight1;
            }
            #endregion

            //Bitmap bitM = new Bitmap(PicQuality.Width, PicQuality.Height);
            //Graphics g = Graphics.FromImage(bitM);
            if(bitM2 == null) //Modify by ChengSk - 20180815
            {
                bitM2 = new Bitmap(PicQuality.Width, PicQuality.Height);
                g2 = Graphics.FromImage(bitM2);
            }
            g2.Clear(DrawGraphProtocol.myBarBackColor);
            //定义画刷
            Brush currentBrush = new SolidBrush(DrawGraphProtocol.myPenColor);
            Brush BarBrush = new SolidBrush(DrawGraphProtocol.myBarBrush1);

            #region 数量/百分比 品质统计信息
            //"数量百分比"字样
            //g.DrawString("数量/百分比", new Font(DrawGraphProtocol.Module1FontStyle, DrawGraphProtocol.Module1FontSize, FontStyle.Regular),
            //    currentBrush, DrawGraphProtocol.Module1LocationX, DrawGraphProtocol.Module1LocationY);
            //"尺寸统计信息"字样
            g2.DrawString(m_resourceManager.GetString("tabPage2.Text"), new Font(DrawGraphProtocol.Module3FontStyle, DrawGraphProtocol.Module3FontSize, FontStyle.Bold),
                currentBrush, PicQuality.Width / 2 - DrawGraphProtocol.Module3MiddleDistance, PicQuality.Height - DrawGraphProtocol.Module3BottomDistance);
            #endregion

            #region 品质名称 柱体 个数 百分比
            //找各品质中的最大值
            for (int i = 0; i < dataInterface.QualityGradeSum; i++)
            {
                UInt64 temp = 0;
                for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                {
                    temp += dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                }
                if (maxFruitNumber < temp)
                {
                    maxFruitNumber = temp;
                }
            }
            for (int i = 0; i < dataInterface.QualityGradeSum; i++)
            {
                if (bHaveHscrollBar)  //有滚动条时
                {
                    //"尺寸名称"字样
                    g2.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH),
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicQuality.Height - DrawGraphProtocol.Module4BottomDistance);

                    for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                    {
                        totalFruitNumber += dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                    }
                   
                    if (totalFruitNumber == 0)
                    {
                        //"柱体"图形显示
                        g2.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicQuality.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g2.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicQuality.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight2 * ((double)totalFruitNumber / maxFruitNumber)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight2 * ((double)totalFruitNumber / maxFruitNumber)));

                        if ((int)(CylinderHeight2 * ((double)totalFruitNumber / maxFruitNumber)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g2.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicQuality.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g2.RotateTransform(-90f);
                            g2.DrawString(totalFruitNumber.ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g2.ResetTransform();
                        }
                        else
                        {
                            g2.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicQuality.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight2 * ((double)totalFruitNumber / maxFruitNumber)));
                            g2.RotateTransform(-90f);
                            g2.DrawString(totalFruitNumber.ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g2.ResetTransform();
                        }

                        
                    }
                    if (dataInterface.IoStStatistics.nTotalCount == 0)
                    {
                        //百分比
                        g2.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicQuality.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g2.DrawString(((double)totalFruitNumber / dataInterface.IoStStatistics.nTotalCount).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicQuality.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight2 * ((double)totalFruitNumber / maxFruitNumber)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                    totalFruitNumber = 0;  //清零
                }
                else  //无滚动条时
                {
                    //"尺寸名称"字样
                    g2.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH),
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicQuality.Height - DrawGraphProtocol.Module4BottomDistance);

                    for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                    {
                        totalFruitNumber += dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                    }

                    if (totalFruitNumber == 0)
                    {
                        //"柱体"图形显示
                        g2.FillRectangle(BarBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicQuality.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g2.FillRectangle(BarBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicQuality.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight2 * ((double)totalFruitNumber / maxFruitNumber)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight2 * ((double)totalFruitNumber / maxFruitNumber)));

                        if ((int)(CylinderHeight2 * ((double)totalFruitNumber / maxFruitNumber)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g2.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicQuality.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g2.RotateTransform(-90f);
                            g2.DrawString(totalFruitNumber.ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g2.ResetTransform();
                        }
                        else
                        {
                            g2.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicQuality.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight2 * ((double)totalFruitNumber / maxFruitNumber)));
                            g2.RotateTransform(-90f);
                            g2.DrawString(totalFruitNumber.ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g2.ResetTransform();
                        }
                        
                    }

                    if (dataInterface.IoStStatistics.nTotalCount == 0)
                    {
                        //百分比
                        g2.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicQuality.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g2.DrawString(((double)totalFruitNumber / dataInterface.IoStStatistics.nTotalCount).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicQuality.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight2 * ((double)totalFruitNumber / maxFruitNumber)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                    totalFruitNumber = 0;  //清零
                }
            }
            #endregion

            PicQuality.BackgroundImage = null;  //无此行代码不会刷新，必须加上 Add by ChengSk - 20180821
            PicQuality.BackgroundImage = bitM2;
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            PicQuality.Left = -hScrollBar2.Value;
        }

        private void currentSelectTabpage3()
        {
            #region 颜色统计
            //统计各颜色水果的个数
            UInt64[] colorFruitNumber = new UInt64[ConstPreDefine.MAX_COLOR_GRADE_NUM];
            string strColorGradeName = dataInterface.ColorGradeName;
            string[] colorGrade;
            if (strColorGradeName == null || strColorGradeName.Equals(""))
            {
                colorGrade = new string[0];
            }
            else
            {
                colorGrade = strColorGradeName.Split('，');
            }     
            #region 统计各颜色水果的个数放到colorFruitNumber中
            for (int i = 0; i < dataInterface.QualityGradeSum; i++)
            {
                for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                {
                    UInt64 tempNum = dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                    switch (Convert.ToInt32(dataInterface.IoStStGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nColorGrade))
                    {
                        case 0:
                            colorFruitNumber[0] += tempNum;
                            break;
                        case 1:
                            colorFruitNumber[1] += tempNum;
                            break;
                        case 2:
                            colorFruitNumber[2] += tempNum;
                            break;
                        case 3:
                            colorFruitNumber[3] += tempNum;
                            break;
                        case 4:
                            colorFruitNumber[4] += tempNum;
                            break;
                        case 5:
                            colorFruitNumber[5] += tempNum;
                            break;
                        case 6:
                            colorFruitNumber[6] += tempNum;
                            break;
                        case 7:
                            colorFruitNumber[7] += tempNum;
                            break;
                        case 8:
                            colorFruitNumber[8] += tempNum;
                            break;
                        case 9:
                            colorFruitNumber[9] += tempNum;
                            break;
                        case 10:
                            colorFruitNumber[10] += tempNum;
                            break;
                        case 11:
                            colorFruitNumber[11] += tempNum;
                            break;
                        case 12:
                            colorFruitNumber[12] += tempNum;
                            break;
                        case 13:
                            colorFruitNumber[13] += tempNum;
                            break;
                        case 14:
                            colorFruitNumber[14] += tempNum;
                            break;
                        case 15:
                            colorFruitNumber[15] += tempNum;
                            break;
                        default:
                            break;   
                    }
                    tempNum = 0;
                }
            }
            #endregion
            UInt64 uMaxValue = FunctionInterface.GetMaxValue(colorFruitNumber);
            UInt64 uSumValue = FunctionInterface.GetSumValue(colorFruitNumber);
            #endregion

            #region 变量声明
            int CylinderHeight3;      //柱高
            bool bHaveHscrollBar;     //是否有滚动条
            int leftRightSpace = 0;   //柱体离左右边框距离
            #endregion

            #region 间隔计算
            int CurrentDisplayCylinderSpace = 0; //圆柱间间隔计算 Add by ChengSk - 2017/7/28
            int LongestWeightOrSizeGradeName = 0;
            LongestWeightOrSizeGradeName = DrawGraphProtocol.LongestWeightOrSizeGradeName(colorGrade.Length, colorGrade);
            CurrentDisplayCylinderSpace = (LongestWeightOrSizeGradeName > (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace)) ?
                (LongestWeightOrSizeGradeName - DrawGraphProtocol.CylinderWidth) : DrawGraphProtocol.CylinderSpace;//如果(最长等级名称 > 圆柱宽+标准空格)，则空格宽度重新计算
            #endregion

            #region 界面设定
            if ((colorGrade.Length-1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
                DrawGraphProtocol.CylinderWidth < tabPage3.Width)  //无需使用滚动条
            {
                bHaveHscrollBar = false;
                hScrollBar3.Visible = false;
                PicColor.Dock = DockStyle.Fill;
                CylinderHeight3 = DrawGraphProtocol.CylinderHeight2;
                leftRightSpace = (PicColor.Width - (colorGrade.Length - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) -
                    DrawGraphProtocol.CylinderWidth) / 2;
            }
            else  //需要使用滚动条
            {
                bHaveHscrollBar = true;
                PicColor.Width = (colorGrade.Length - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
                    DrawGraphProtocol.CylinderWidth + 2 * DrawGraphProtocol.LeftRightSpace;
                hScrollBar3.Maximum = PicColor.Width - tabPage3.Width;
                CylinderHeight3 = DrawGraphProtocol.CylinderHeight1;
            }
            #endregion

            //Bitmap bitM = new Bitmap(PicColor.Width, PicColor.Height);
            //Graphics g = Graphics.FromImage(bitM);
            if(bitM3 == null) //Modify by ChengSk - 20180815
            {
                bitM3 = new Bitmap(PicColor.Width, PicColor.Height);
                g3 = Graphics.FromImage(bitM3);
            }
            g3.Clear(DrawGraphProtocol.myBarBackColor);
            //定义画刷
            Brush currentBrush = new SolidBrush(DrawGraphProtocol.myPenColor);
            Brush BarBrush = new SolidBrush(DrawGraphProtocol.myBarBrush1);

            #region 数量/百分比 颜色统计信息
            //"数量百分比"字样
            //g.DrawString("数量/百分比", new Font(DrawGraphProtocol.Module1FontStyle, DrawGraphProtocol.Module1FontSize, FontStyle.Regular),
            //    currentBrush, DrawGraphProtocol.Module1LocationX, DrawGraphProtocol.Module1LocationY);
            //"尺寸统计信息"字样
            g3.DrawString(m_resourceManager.GetString("tabPage3.Text"), new Font(DrawGraphProtocol.Module3FontStyle, DrawGraphProtocol.Module3FontSize, FontStyle.Bold),
                currentBrush, PicColor.Width / 2 - DrawGraphProtocol.Module3MiddleDistance, PicColor.Height - DrawGraphProtocol.Module3BottomDistance);
            #endregion

            #region 颜色名称 柱体 个数 百分比
            for (int i = 0; i < colorGrade.Length; i++)
            {
                if (bHaveHscrollBar)  //有滚动条时
                {
                    //"颜色名称"字样
                    g3.DrawString(colorGrade[i],
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicColor.Height - DrawGraphProtocol.Module4BottomDistance);

                    if (colorFruitNumber[i] == 0)
                    {
                        //"柱体"图形显示
                        g3.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicColor.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g3.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicColor.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight3 * ((double)colorFruitNumber[i] / uMaxValue)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight3 * ((double)colorFruitNumber[i] / uMaxValue)));

                        if ((int)(CylinderHeight3 * ((double)colorFruitNumber[i] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g3.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicColor.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g3.RotateTransform(-90f);
                            g3.DrawString(colorFruitNumber[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g3.ResetTransform();
                        }
                        else
                        {
                            g3.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicColor.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight3 * ((double)colorFruitNumber[i] / uMaxValue)));
                            g3.RotateTransform(-90f);
                            g3.DrawString(colorFruitNumber[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g3.ResetTransform();
                        }
                        
                    }

                    if (uSumValue == 0)
                    {
                        //百分比
                        g3.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicColor.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g3.DrawString(((double)colorFruitNumber[i] / uSumValue).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicColor.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight3 * ((double)colorFruitNumber[i] / uMaxValue)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                }
                else  //无滚动条时
                {
                    //"颜色名称"字样
                    g3.DrawString(colorGrade[i],
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicColor.Height - DrawGraphProtocol.Module4BottomDistance);

                    if (colorFruitNumber[i] == 0)
                    {
                        //"柱体"图形显示
                        g3.FillRectangle(BarBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicColor.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g3.FillRectangle(BarBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicColor.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight3 * ((double)colorFruitNumber[i] / uMaxValue)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight3 * ((double)colorFruitNumber[i] / uMaxValue)));

                        if ((int)(CylinderHeight3 * ((double)colorFruitNumber[i] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g3.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicColor.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g3.RotateTransform(-90f);
                            g3.DrawString(colorFruitNumber[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g3.ResetTransform();
                        }
                        else
                        {
                            g3.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicColor.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight3 * ((double)colorFruitNumber[i] / uMaxValue)));
                            g3.RotateTransform(-90f);
                            g3.DrawString(colorFruitNumber[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g3.ResetTransform();
                        }
                        
                    }

                    if (uSumValue == 0)
                    {
                        //百分比
                        g3.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicColor.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g3.DrawString(((double)colorFruitNumber[i] / uSumValue).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicColor.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight3 * ((double)colorFruitNumber[i] / uMaxValue)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                }
            }
            #endregion

            PicColor.BackgroundImage = null;  //无此行代码不会刷新，必须加上 Add by ChengSk - 20180821
            PicColor.BackgroundImage = bitM3;
        }

        private void hScrollBar3_Scroll(object sender, ScrollEventArgs e)
        {
            PicColor.Left = -hScrollBar3.Value;
        }

        private void currentSelectTabpage4()
        {
            #region 形状统计
            //统计各形状水果的个数
            UInt64[] shapeFruitNumber = new UInt64[ConstPreDefine.MAX_SHAPE_GRADE_NUM];
            string strShapeGradeName = dataInterface.ShapeGradeName;
            string[] shapeGrade;
            if (strShapeGradeName == null || strShapeGradeName.Equals(""))
            {
                shapeGrade = new string[0];
            }
            else
            {
                shapeGrade = strShapeGradeName.Split('，');
            }         
            #region 统计各颜色水果的个数放到shapeFruitNumber中
            for (int i = 0; i < dataInterface.QualityGradeSum; i++)
            {
                for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                {
                    UInt64 tempNum = dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                    switch (Convert.ToInt32(dataInterface.IoStStGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbShapeSize))
                    {
                        case 0:
                            shapeFruitNumber[0] += tempNum;
                            break;
                        case 1:
                            shapeFruitNumber[1] += tempNum;
                            break;
                        default:
                            break;
                    }
                    tempNum = 0;
                }
            }
            #endregion
            UInt64 uMaxValue = FunctionInterface.GetMaxValue(shapeFruitNumber);
            UInt64 uSumValue = FunctionInterface.GetSumValue(shapeFruitNumber);
            #endregion

            #region 变量声明
            int CylinderHeight4;      //柱高
            bool bHaveHscrollBar;     //是否有滚动条
            int leftRightSpace = 0;   //柱体离左右边框距离
            #endregion

            #region 间隔计算
            int CurrentDisplayCylinderSpace = 0; //圆柱间间隔计算 Add by ChengSk - 2017/7/28
            int LongestWeightOrSizeGradeName = 0;
            LongestWeightOrSizeGradeName = DrawGraphProtocol.LongestWeightOrSizeGradeName(shapeGrade.Length, shapeGrade);
            CurrentDisplayCylinderSpace = (LongestWeightOrSizeGradeName > (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace)) ?
                (LongestWeightOrSizeGradeName - DrawGraphProtocol.CylinderWidth) : DrawGraphProtocol.CylinderSpace;//如果(最长等级名称 > 圆柱宽+标准空格)，则空格宽度重新计算
            #endregion

            #region 界面设定
            if ((shapeGrade.Length - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
                DrawGraphProtocol.CylinderWidth < tabPage4.Width)  //无需使用滚动条
            {
                bHaveHscrollBar = false;
                hScrollBar4.Visible = false;
                PicShape.Dock = DockStyle.Fill;
                CylinderHeight4 = DrawGraphProtocol.CylinderHeight2;
                leftRightSpace = (PicShape.Width - (shapeGrade.Length - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) -
                    DrawGraphProtocol.CylinderWidth) / 2;
            }
            else  //需要使用滚动条
            {
                bHaveHscrollBar = true;
                PicShape.Width = (shapeGrade.Length - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
                    DrawGraphProtocol.CylinderWidth + 2 * DrawGraphProtocol.LeftRightSpace;
                hScrollBar4.Maximum = PicShape.Width - tabPage4.Width;
                CylinderHeight4 = DrawGraphProtocol.CylinderHeight1;
            }
            #endregion

            //Bitmap bitM = new Bitmap(PicShape.Width, PicShape.Height);
            //Graphics g = Graphics.FromImage(bitM);
            if(bitM4 == null) //Modify by ChengSk - 20180815
            {
                bitM4 = new Bitmap(PicShape.Width, PicShape.Height);
                g4 = Graphics.FromImage(bitM4);
            }
            g4.Clear(DrawGraphProtocol.myBarBackColor);
            //定义画刷
            Brush currentBrush = new SolidBrush(DrawGraphProtocol.myPenColor);
            Brush BarBrush = new SolidBrush(DrawGraphProtocol.myBarBrush1);

            #region 数量/百分比 形状统计信息
            //"数量百分比"字样
            //g.DrawString("数量/百分比", new Font(DrawGraphProtocol.Module1FontStyle, DrawGraphProtocol.Module1FontSize, FontStyle.Regular),
            //    currentBrush, DrawGraphProtocol.Module1LocationX, DrawGraphProtocol.Module1LocationY);
            //"尺寸统计信息"字样
            g4.DrawString(m_resourceManager.GetString("tabPage4.Text"), new Font(DrawGraphProtocol.Module3FontStyle, DrawGraphProtocol.Module3FontSize, FontStyle.Bold),
                currentBrush, PicShape.Width / 2 - DrawGraphProtocol.Module3MiddleDistance, PicShape.Height - DrawGraphProtocol.Module3BottomDistance);
            #endregion

            #region 形状名称 柱体 个数 百分比
            for (int i = 0; i < shapeGrade.Length; i++)
            {
                if (bHaveHscrollBar)  //有滚动条时
                {
                    //"形状名称"字样
                    g4.DrawString(shapeGrade[i],
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicShape.Height - DrawGraphProtocol.Module4BottomDistance);

                    if (shapeFruitNumber[i] == 0)
                    {
                        //"柱体"图形显示
                        g4.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicShape.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g4.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicShape.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight4 * ((double)shapeFruitNumber[i] / uMaxValue)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight4 * ((double)shapeFruitNumber[i] / uMaxValue)));

                        if ((int)(CylinderHeight4 * ((double)shapeFruitNumber[i] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g4.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicShape.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g4.RotateTransform(-90f);
                            g4.DrawString(shapeFruitNumber[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g4.ResetTransform();
                        }
                        else
                        {
                            g4.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicShape.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight4 * ((double)shapeFruitNumber[i] / uMaxValue)));
                            g4.RotateTransform(-90f);
                            g4.DrawString(shapeFruitNumber[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g4.ResetTransform();
                        }
                        
                    }

                    if (uSumValue == 0)
                    {
                        //百分比
                        g4.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicShape.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g4.DrawString(((double)shapeFruitNumber[i] / uSumValue).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicShape.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight4 * ((double)shapeFruitNumber[i] / uMaxValue)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                }
                else  //无滚动条时
                {
                    //"颜色名称"字样
                    g4.DrawString(shapeGrade[i],
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicShape.Height - DrawGraphProtocol.Module4BottomDistance);

                    if (shapeFruitNumber[i] == 0)
                    {
                        //"柱体"图形显示
                        g4.FillRectangle(BarBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicShape.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g4.FillRectangle(BarBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicShape.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight4 * ((double)shapeFruitNumber[i] / uMaxValue)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight4 * ((double)shapeFruitNumber[i] / uMaxValue)));

                        if ((int)(CylinderHeight4 * ((double)shapeFruitNumber[i] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g4.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicShape.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g4.RotateTransform(-90f);
                            g4.DrawString(shapeFruitNumber[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g4.ResetTransform();
                        }
                        else
                        {
                            g4.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicShape.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight4 * ((double)shapeFruitNumber[i] / uMaxValue)));
                            g4.RotateTransform(-90f);
                            g4.DrawString(shapeFruitNumber[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g4.ResetTransform();
                        }
               
                    }

                    if (uSumValue == 0)
                    {
                        //百分比
                        g4.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicShape.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g4.DrawString(((double)shapeFruitNumber[i] / uSumValue).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicShape.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight4 * ((double)shapeFruitNumber[i] / uMaxValue)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                }
            }
            #endregion

            PicShape.BackgroundImage = null;  //无此行代码不会刷新，必须加上 Add by ChengSk - 20180821
            PicShape.BackgroundImage = bitM4;
        }

        private void hScrollBar4_Scroll(object sender, ScrollEventArgs e)
        {
            PicShape.Left = -hScrollBar4.Value;
        }

        private void CboSizeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentSelectIndex = CboSizeType.SelectedIndex;
            ReflashTabPage5();
        }

        private void ReflashTabPage5()
        {
            #region 声明变量
            int CylinderHeight5;      //柱高
            bool bHaveHscrollBar;     //是否有滚动条
            int leftRightSpace = 0;   //柱体离左右边框距离
            #endregion

            #region 间隔计算
            int CurrentDisplayCylinderSpace = 0; //圆柱间间隔计算 Add by ChengSk - 2017/7/28
            int LongestWeightOrSizeGradeName = 0;
            LongestWeightOrSizeGradeName = DrawGraphProtocol.LongestWeightOrSizeGradeName(dataInterface.WeightOrSizeGradeSum, dataInterface.IoStStGradeInfo.strSizeGradeName);
            CurrentDisplayCylinderSpace = (LongestWeightOrSizeGradeName > (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace)) ?
                (LongestWeightOrSizeGradeName - DrawGraphProtocol.CylinderWidth) : DrawGraphProtocol.CylinderSpace;//如果(最长等级名称 > 圆柱宽+标准空格)，则空格宽度重新计算
            #endregion

            #region 界面设定
            if ((dataInterface.WeightOrSizeGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
                DrawGraphProtocol.CylinderWidth < tabPage5.Width)  //无需使用滚动条
            {
                bHaveHscrollBar = false;
                //hScrollBar5.Visible = false;
                //PicBoxAll.Dock = DockStyle.Fill;
                hScrollBar5.Maximum = PicSize.Width - tabPage5.Width;
                CylinderHeight5 = DrawGraphProtocol.CylinderHeight3;
                leftRightSpace = (PicSize.Width - (dataInterface.WeightOrSizeGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) -
                    DrawGraphProtocol.CylinderWidth) / 2;
            }
            else  //需要使用滚动条
            {
                bHaveHscrollBar = true;
                PicSize.Width = (dataInterface.WeightOrSizeGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
                    DrawGraphProtocol.CylinderWidth + 2 * DrawGraphProtocol.LeftRightSpace;
                PicBox.Width = (dataInterface.WeightOrSizeGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
                    DrawGraphProtocol.CylinderWidth + 2 * DrawGraphProtocol.LeftRightSpace;
                hScrollBar5.Maximum = PicSize.Width - tabPage5.Width;
                CylinderHeight5 = DrawGraphProtocol.CylinderHeight3;
            }
            #endregion

            #region 绘制PicSize
            //Bitmap bitM1 = new Bitmap(PicSize.Width, PicSize.Height);
            //Graphics g1 = Graphics.FromImage(bitM1);
            if(bitM51 == null) //Modify by ChengSk - 20180815
            {
                bitM51 = new Bitmap(PicSize.Width, PicSize.Height);
                g51 = Graphics.FromImage(bitM51);
            }
            g51.Clear(DrawGraphProtocol.myBarBackColor);
            //定义画刷
            Brush currentBrush1 = new SolidBrush(DrawGraphProtocol.myPenColor);
            Brush BarBrush1 = new SolidBrush(DrawGraphProtocol.myBarBrush1);
            //"数量百分比"字样
            //g1.DrawString("数量/百分比", new Font(DrawGraphProtocol.Module1FontStyle, DrawGraphProtocol.Module1FontSize, FontStyle.Regular),
            //    currentBrush1, DrawGraphProtocol.Module1LocationX, DrawGraphProtocol.Module1LocationY);
            //"尺寸统计信息"字样
            g51.DrawString(m_resourceManager.GetString("LblQualityDimensionLevel.Text"), new Font(DrawGraphProtocol.Module3FontStyle, DrawGraphProtocol.Module3FontSize, FontStyle.Bold),
                currentBrush1, PicSize.Width / 2 - DrawGraphProtocol.Module3MiddleDistance-60, PicSize.Height - DrawGraphProtocol.Module3BottomDistance);
            UInt64 nMaxValue1 = 0;  //尺寸最大值
            UInt64 nSumValue1 = 0;  //尺寸总值
            for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
            {
                UInt64 temp = dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i];
                if (nMaxValue1 < temp)
                {
                    nMaxValue1 = temp;
                }
                nSumValue1 += temp;
            }

            for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
            {
                if (bHaveHscrollBar)  //有滚动条时
                {
                    //"尺寸名称"字样
                    g51.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH),
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush1, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicSize.Height - DrawGraphProtocol.Module4BottomDistance);

                    if (dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] == 0)
                    {
                        //"柱体"图形显示
                        g51.FillRectangle(BarBrush1, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g51.FillRectangle(BarBrush1, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue1)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue1)));

                        if ((int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue1)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g51.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g51.RotateTransform(-90f);
                            g51.DrawString(dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush1, 0, 0);
                            g51.ResetTransform();
                        }
                        else
                        {
                            g51.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue1)));
                            g51.RotateTransform(-90f);
                            g51.DrawString(dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush1, 0, 0);
                            g51.ResetTransform();
                        }

                        
                    }

                    if (nSumValue1 == 0)
                    {
                        //百分比
                        g51.DrawString("0.00",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush1,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g51.DrawString(((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nSumValue1).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush1,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / nMaxValue1)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                }
                else  //无滚动条时
                {
                    //"尺寸名称"字样
                    g51.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH),
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush1, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicSize.Height - DrawGraphProtocol.Module4BottomDistance);


                    if (dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] == 0)
                    {
                        //"柱体"图形显示
                        g51.FillRectangle(BarBrush1, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g51.FillRectangle(BarBrush1, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue1)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue1)));

                        if ((int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue1)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g51.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g51.RotateTransform(-90f);
                            g51.DrawString(dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush1, 0, 0);
                            g51.ResetTransform();
                        }
                        else
                        {
                            g51.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue1)));
                            g51.RotateTransform(-90f);
                            g51.DrawString(dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush1, 0, 0);
                            g51.ResetTransform();
                        }

                        
                    }

                    if (nSumValue1 == 0)
                    {
                        //百分比
                        g51.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush1,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g51.DrawString(((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nSumValue1).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush1,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue1)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                }
            }

            PicSize.BackgroundImage = null;  //无此行代码不会刷新，必须加上 Add by ChengSk - 20180821
            PicSize.BackgroundImage = bitM51;
            #endregion

            #region 绘制PicBox
            //Bitmap bitM2 = new Bitmap(PicBox.Width, PicBox.Height);
            //Graphics g2 = Graphics.FromImage(bitM2);
            if(bitM52 == null) //Modify by ChengSk - 20180815
            {
                bitM52 = new Bitmap(PicBox.Width, PicBox.Height);
                g52 = Graphics.FromImage(bitM52);
            }
            g52.Clear(DrawGraphProtocol.myBarBackColor);
            //定义画刷
            Brush currentBrush2 = new SolidBrush(DrawGraphProtocol.myPenColor);
            Brush BarBrush2 = new SolidBrush(DrawGraphProtocol.myBarBrush1);
            //"数量百分比"字样
            //g2.DrawString("数量/百分比", new Font(DrawGraphProtocol.Module1FontStyle, DrawGraphProtocol.Module1FontSize, FontStyle.Regular),
            //    currentBrush2, DrawGraphProtocol.Module1LocationX, DrawGraphProtocol.Module1LocationY);
            //"尺寸统计信息"字样
            g52.DrawString(m_resourceManager.GetString("LblQualityOfCases.Text"), new Font(DrawGraphProtocol.Module3FontStyle, DrawGraphProtocol.Module3FontSize, FontStyle.Bold),
                currentBrush2, PicBox.Width / 2 - DrawGraphProtocol.Module3MiddleDistance - 30, PicBox.Height - DrawGraphProtocol.Module3BottomDistance);
            Int32 nMaxValue2 = 0;  //尺寸最大值
            Int32 nSumValue2 = 0;  //尺寸总值
            for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
            {
                Int32 temp = dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i];
                if (nMaxValue2 < temp)
                {
                    nMaxValue2 = temp;
                }
                nSumValue2 += temp;
            }

            for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
            {
                if (bHaveHscrollBar)  //有滚动条时
                {
                    //"尺寸名称"字样
                    g52.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH),
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush1, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicBox.Height - DrawGraphProtocol.Module4BottomDistance);

                    if (dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] == 0)
                    {
                        //"柱体"图形显示
                        g52.FillRectangle(BarBrush1, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g52.FillRectangle(BarBrush1, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue2)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue2)));

                        if ((int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue2)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g52.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g52.RotateTransform(-90f);
                            g52.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush1, 0, 0);
                            g52.ResetTransform();
                        }
                        else
                        {
                            g52.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue2)));
                            g52.RotateTransform(-90f);
                            g52.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush1, 0, 0);
                            g52.ResetTransform();
                        }
                        
                    }

                    if (nSumValue2 == 0)
                    {
                        //百分比
                        g52.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush1,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g52.DrawString(((double)dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nSumValue2).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush1,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / nMaxValue2)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                }
                else  //无滚动条时
                {
                    //"尺寸名称"字样
                    g52.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH),
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush1, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicBox.Height - DrawGraphProtocol.Module4BottomDistance);


                    if (dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] == 0)
                    {
                        //"柱体"图形显示
                        g52.FillRectangle(BarBrush1, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g52.FillRectangle(BarBrush1, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue2)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue2)));

                        if ((int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue2)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g52.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g52.RotateTransform(-90f);
                            g52.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush1, 0, 0);
                            g52.ResetTransform();
                        }
                        else
                        {
                            g52.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue2)));
                            g52.RotateTransform(-90f);
                            g52.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush1, 0, 0);
                            g52.ResetTransform();
                        }
                        
                    }

                    if (nSumValue2 == 0)
                    {
                        //百分比
                        g52.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush1,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g52.DrawString(((double)dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nSumValue2).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush1,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight5 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / nMaxValue2)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                }
            }

            PicBox.BackgroundImage = null;  //无此行代码不会刷新，必须加上 Add by ChengSk - 20180821
            PicBox.BackgroundImage = bitM52;
            #endregion

        }

        private void currentSelectTabpage5()
        {
            //CboSizeType清空
            CboSizeType.Items.Clear();

            #region 往CboSizeType中插入数据
            string strQualityName;
            for (int i = 0; i < dataInterface.QualityGradeSum; i++)
            {
                strQualityName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                int QualityGradeNameLength = strQualityName.IndexOf("\0");  //add by xcw 20201102  
                if (QualityGradeNameLength == -1)
                {
                    QualityGradeNameLength = strQualityName.Length;
                }
                strQualityName = strQualityName.Substring(0, QualityGradeNameLength);
                CboSizeType.Items.Add(strQualityName);
            }
            #endregion

            //设定当前选中项
            //CboSizeType.SelectedIndex = 0;
            CboSizeType.SelectedIndex = currentSelectIndex;
        }

        private void hScrollBar5_Scroll(object sender, ScrollEventArgs e)
        {
            PicSize.Left = -hScrollBar5.Value;
            PicBox.Left = -hScrollBar5.Value;
        }

        #region 取消箱数统计信息
        //private void currentSelectTabpage6()
        //{
        //    #region 变量声明
        //    int CylinderHeight6;      //柱高
        //    bool bHaveHscrollBar;     //是否有滚动条
        //    int leftRightSpace = 0;   //柱体离左右边框距离
        //    Int32 uMaxValue = 0;      //各等级"个数"最大值
        //    Int32 uSumValue = 0;      //各等级"个数"总值
        //    string strSizeName;       //尺寸名称
        //    string strQualityName;    //品质名称
        //    string strMixName;        //总名称：尺寸名称.品质名称
        //    #endregion

        //    #region 间隔计算
        //    int CurrentDisplayCylinderSpace = 0; //圆柱间间隔计算 Add by ChengSk - 2017/7/28
        //    int LongestWeightOrSizeGradeName = 0;
        //    LongestWeightOrSizeGradeName = DrawGraphProtocol.LongestWeightOrSizeGradeName(dataInterface.QualityGradeSum, dataInterface.IoStStGradeInfo.strQualityGradeName, 
        //        dataInterface.WeightOrSizeGradeSum, dataInterface.IoStStGradeInfo.strSizeGradeName);
        //    CurrentDisplayCylinderSpace = (LongestWeightOrSizeGradeName > (DrawGraphProtocol.CylinderWidth + DrawGraphProtocol.CylinderSpace)) ?
        //        (LongestWeightOrSizeGradeName - DrawGraphProtocol.CylinderWidth) : DrawGraphProtocol.CylinderSpace;//如果(最长等级名称 > 圆柱宽+标准空格)，则空格宽度重新计算
        //    #endregion

        //    #region 界面设定
        //    if ((dataInterface.QualityGradeSum * dataInterface.WeightOrSizeGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
        //        DrawGraphProtocol.CylinderWidth < tabPage6.Width)  //无需使用滚动条
        //    {
        //        bHaveHscrollBar = false;
        //        hScrollBar6.Visible = false;
        //        PicBoxAll.Dock = DockStyle.Fill;
        //        CylinderHeight6 = DrawGraphProtocol.CylinderHeight2;
        //        leftRightSpace = (PicBoxAll.Width - (dataInterface.QualityGradeSum * dataInterface.WeightOrSizeGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) -
        //            DrawGraphProtocol.CylinderWidth) / 2;
        //    }
        //    else  //需要使用滚动条
        //    {
        //        bHaveHscrollBar = true;
        //        PicBoxAll.Width = (dataInterface.QualityGradeSum * dataInterface.WeightOrSizeGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
        //            DrawGraphProtocol.CylinderWidth + 2 * DrawGraphProtocol.LeftRightSpace;
        //        hScrollBar6.Maximum = PicBoxAll.Width - tabPage6.Width;
        //        CylinderHeight6 = DrawGraphProtocol.CylinderHeight1;
        //    }
        //    #endregion

        //    Bitmap bitM = new Bitmap(PicBoxAll.Width, PicBoxAll.Height);
        //    Graphics g = Graphics.FromImage(bitM);
        //    g.Clear(DrawGraphProtocol.myBarBackColor);
        //    //定义画刷
        //    Brush currentBrush = new SolidBrush(DrawGraphProtocol.myPenColor);
        //    Brush BarBrush = new SolidBrush(DrawGraphProtocol.myBarBrush1);

        //    #region 数量/百分比 箱数统计信息
        //    //"数量百分比"字样
        //    //g.DrawString("数量/百分比", new Font(DrawGraphProtocol.Module1FontStyle, DrawGraphProtocol.Module1FontSize, FontStyle.Regular),
        //    //    currentBrush, DrawGraphProtocol.Module1LocationX, DrawGraphProtocol.Module1LocationY);
        //    //"尺寸统计信息"字样
        //    g.DrawString(m_resourceManager.GetString("tabPage6.Text"), new Font(DrawGraphProtocol.Module3FontStyle, DrawGraphProtocol.Module3FontSize, FontStyle.Bold),
        //        currentBrush, PicBoxAll.Width / 2 - DrawGraphProtocol.Module3MiddleDistance, PicBoxAll.Height - DrawGraphProtocol.Module3BottomDistance);
        //    #endregion

        //    #region 混合等级名称 柱体 个数 百分比
        //    //"尺寸名称"字样  "柱体"图形显示  个数  百分比
        //    uMaxValue = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nBoxGradeCount);
        //    uSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);

        //    for (int i = 0; i < dataInterface.QualityGradeSum; i++)
        //    {
        //        for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
        //        {
        //            if (bHaveHscrollBar)  //有滚动条时
        //            {
        //                //"特征名称"字样
        //                strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
        //                strQualityName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
        //                strMixName = strSizeName.Substring(0, strSizeName.IndexOf("\0")) + "." + strQualityName.Substring(0, strQualityName.IndexOf("\0"));
        //                g.DrawString(strMixName,
        //                new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
        //                currentBrush, DrawGraphProtocol.LeftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
        //                PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance);

        //                if (dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] == 0)
        //                {
        //                    //"柱体"图形显示
        //                    g.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
        //                    PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
        //                    DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
        //                }
        //                else
        //                {
        //                    //"柱体"图形显示  个数
        //                    g.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
        //                    PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight6 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uMaxValue)),
        //                    DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight6 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uMaxValue)));

        //                    if ((int)(CylinderHeight6 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
        //                    {
        //                        g.TranslateTransform(DrawGraphProtocol.LeftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
        //                        PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
        //                        g.RotateTransform(-90f);
        //                        g.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString(),
        //                            new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
        //                        g.ResetTransform();
        //                    }
        //                    else
        //                    {
        //                        g.TranslateTransform(DrawGraphProtocol.LeftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
        //                        PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
        //                        (int)(CylinderHeight6 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uMaxValue)));
        //                        g.RotateTransform(-90f);
        //                        g.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString(),
        //                            new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
        //                        g.ResetTransform();
        //                    }

        //                }

        //                if (uSumValue == 0)
        //                {
        //                    //百分比
        //                    g.DrawString("0.00%",
        //                        new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
        //                        DrawGraphProtocol.LeftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
        //                        PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
        //                }
        //                else
        //                {
        //                    //百分比
        //                    g.DrawString(((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uSumValue).ToString("0.00%"),
        //                        new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
        //                        DrawGraphProtocol.LeftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
        //                        PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight6 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uMaxValue)) - DrawGraphProtocol.Module6CylinderDistance);
        //                }

        //            }
        //            else  //无滚动条时
        //            {
        //                //"特征名称"字样
        //                strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
        //                strQualityName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
        //                strMixName = strSizeName.Substring(0,strSizeName.IndexOf("\0"))+"."+strQualityName.Substring(0,strQualityName.IndexOf("\0"));
        //                g.DrawString(strMixName,
        //                new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
        //                currentBrush, leftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
        //                PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance);

        //                if (dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] == 0)
        //                {
        //                    //"柱体"图形显示
        //                    g.FillRectangle(BarBrush, leftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
        //                    PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
        //                    DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
        //                }
        //                else
        //                {
        //                    //"柱体"图形显示  个数
        //                    g.FillRectangle(BarBrush, leftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
        //                    PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight6 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uMaxValue)),
        //                    DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight6 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uMaxValue)));

        //                    if ((int)(CylinderHeight6 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
        //                    {
        //                        g.TranslateTransform(leftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
        //                        PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
        //                        g.RotateTransform(-90f);
        //                        g.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString(),
        //                            new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
        //                        g.ResetTransform();
        //                    }
        //                    else
        //                    {
        //                        g.TranslateTransform(leftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
        //                        PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
        //                        (int)(CylinderHeight6 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uMaxValue)));
        //                        g.RotateTransform(-90f);
        //                        g.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString(),
        //                            new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
        //                        g.ResetTransform();
        //                    }

        //                }

        //                if (uSumValue == 0)
        //                {
        //                    //百分比
        //                    g.DrawString("0.00%",
        //                        new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
        //                        leftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
        //                        PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
        //                }
        //                else
        //                {
        //                    //百分比
        //                    g.DrawString(((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uSumValue).ToString("0.00%"),
        //                        new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
        //                        leftRightSpace + (i * dataInterface.WeightOrSizeGradeSum + j) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
        //                        PicBoxAll.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight6 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uMaxValue)) - DrawGraphProtocol.Module6CylinderDistance);
        //                }

        //            }
        //        }
        //    }
        //    #endregion

        //    PicBoxAll.BackgroundImage = bitM;
        //}

        //private void hScrollBar6_Scroll(object sender, ScrollEventArgs e)
        //{
        //    PicBoxAll.Left = -hScrollBar6.Value;
        //}
        #endregion

        private void currentSelectTabpage6()
        {
            //LvwSizeData清空
            LvwSizeOrWeightData.Items.Clear();

            #region 变量声明
            UInt64 uMaxValue = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nWeightGradeCount);
            UInt64 uNumSumValue = 0;
            UInt64 uWeightSumValue = 0;
            UInt64 uBoxSumValue = 0;
            uNumSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);
            uWeightSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);
            uBoxSumValue = (ulong)FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);
            string strSizeName;
            string strMixName;


            #endregion
            for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
            {
                ulong GradeCount = 0;
                double CountPer = 0.0;
                double GradeWeight = 0.0;
                double WeightPer = 0.0;
                int BoxSumValue = 0;
                double BoxSumPer = 0.0;
                for (int i = 0; i < dataInterface.QualityGradeSum; i++)
                {
                    GradeCount += dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                    if (uNumSumValue != 0)
                    {
                        CountPer += ((double)dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uNumSumValue);
                    }
                    GradeWeight += (dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / 1000.0);
                    if (uWeightSumValue != 0)
                    {
                        WeightPer += ((double)dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uWeightSumValue);
                    }
                    BoxSumValue += dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];

                    if (uBoxSumValue != 0)
                    {
                        BoxSumPer += ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uBoxSumValue);
                    }
                }
                //获取"特征名称"
                strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                strMixName = strSizeName.Substring(0, strSizeName.IndexOf("\0"))/* + "." + strQualityName.Substring(0, QualityGradeNameLength)*/;
                ListViewItem lv = new ListViewItem(strMixName);
                lv.SubItems.Add(dataInterface.IoStStGradeInfo.grades[j].nMinSize.ToString());
                lv.SubItems.Add(GradeCount.ToString());
                lv.SubItems.Add(CountPer.ToString("0.00%"));
                lv.SubItems.Add(GradeWeight.ToString("0.0"));
                lv.SubItems.Add(WeightPer.ToString("0.00%"));
                lv.SubItems.Add(BoxSumValue.ToString());
                lv.SubItems.Add(BoxSumPer.ToString("0.00%"));
                LvwSizeOrWeightData.Items.Add(lv);
            }
            #region 往LvwSizeData插入数据



            #endregion
            #region ListView单双行换色
            for (int i = 0; i < LvwSizeOrWeightData.Items.Count; i++)
            {
                if (i % 2 == 0)
                {
                    LvwSizeOrWeightData.Items[i].BackColor = DrawGraphProtocol.myBackColor;
                }
            }
            #endregion
        }
        private void currentSelectTabpage7()
        {
           
            //LvwFruitData清空
            LvwFruitData.Items.Clear();

            #region 变量声明
            UInt64 uMaxValue = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nWeightGradeCount);
            UInt64 uNumSumValue = 0;
            UInt64 uWeightSumValue = 0;
            UInt64 uBoxSumValue = 0;
            uNumSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);
            uWeightSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);
            uBoxSumValue = (ulong)FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);
            string strSizeName;
            string strQualityName;
            string strMixName;
            #endregion

            #region 往LvwFruitData插入数据
            for (int i = 0; i < dataInterface.QualityGradeSum; i++)
            {
                for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                {
                    //获取"特征名称"
                    strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                    strQualityName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                    int QualityGradeNameLength = strQualityName.IndexOf("\0");  //add by xcw 20201102  
                    if (QualityGradeNameLength == -1)
                    {
                        QualityGradeNameLength = strQualityName.Length;
                    }
                    strMixName = strSizeName.Substring(0, strSizeName.IndexOf("\0")) + "." + strQualityName.Substring(0, QualityGradeNameLength);
                    ListViewItem lv = new ListViewItem(strMixName);
                    lv.SubItems.Add(dataInterface.IoStStGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nMinSize.ToString());
                    lv.SubItems.Add(dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString());
                    if (uNumSumValue == 0)
                    {
                        lv.SubItems.Add("0.00%");
                    }
                    else
                    {
                        lv.SubItems.Add(((double)dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uNumSumValue).ToString("0.00%"));
                    }
                    lv.SubItems.Add((dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j]/1000.0).ToString("0.0"));
                    if (uWeightSumValue == 0)
                    {
                        lv.SubItems.Add("0.00%");
                    }
                    else
                    {
                        lv.SubItems.Add(((double)dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uWeightSumValue).ToString("0.00%"));
                    }
                    lv.SubItems.Add(dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString());
                    if (uBoxSumValue == 0)
                    {
                        lv.SubItems.Add("0.00%");
                    }
                    else
                    {
                        lv.SubItems.Add(((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uBoxSumValue).ToString("0.00%"));
                    }
                    LvwFruitData.Items.Add(lv);
                }
            }
            #endregion

            #region ListView单双行换色
            for (int i = 0; i < LvwFruitData.Items.Count; i++)
            {
                if (i % 2 == 0)
                {
                    LvwFruitData.Items[i].BackColor = DrawGraphProtocol.myBackColor;
                }
            }
            #endregion
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        private void FillcurrentSelectTabpage7()
        {

            #region 变量声明
            UInt64 uMaxValue = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nWeightGradeCount);
            UInt64 uNumSumValue = 0;
            UInt64 uWeightSumValue = 0;
            UInt64 uBoxSumValue = 0;
            uNumSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);
            uWeightSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);
            uBoxSumValue = (ulong)FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);
            //string strSizeName;
            //string strQualityName;
            //string strMixName;
            #endregion

            if (LvwFruitData.Items.Count != dataInterface.WeightOrSizeGradeSum)
                return;

            #region 往LvwFruitData插入数据
            for (int i = 0; i < dataInterface.QualityGradeSum; i++)
            {
                for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                {
                    //获取"特征名称"
                    //strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                    //strQualityName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                    //strMixName = strSizeName.Substring(0, strSizeName.IndexOf("\0")) + "." + strQualityName.Substring(0, strQualityName.IndexOf("\0"));
                   // LvwFruitData.Items[i * dataInterface.WeightOrSizeGradeSum + j].SubItems[1].Text = dataInterface.IoStStGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nMinSize.ToString();
                    LvwFruitData.Items[i * dataInterface.WeightOrSizeGradeSum + j].SubItems[2].Text = dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString();

                    if (uNumSumValue == 0)
                    {
                        LvwFruitData.Items[i * dataInterface.WeightOrSizeGradeSum + j].SubItems[3].Text = "0.00%";
                    }
                    else
                    {
                        LvwFruitData.Items[i * dataInterface.WeightOrSizeGradeSum + j].SubItems[3].Text = ((double)dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uNumSumValue).ToString("0.00%");
                    }
                    LvwFruitData.Items[i * dataInterface.WeightOrSizeGradeSum + j].SubItems[4].Text = (dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / 1000.0).ToString("0.0");
                    if (uWeightSumValue == 0)
                    {
                        LvwFruitData.Items[i * dataInterface.WeightOrSizeGradeSum + j].SubItems[5].Text = "0.00%";
                    }
                    else
                    {
                        LvwFruitData.Items[i * dataInterface.WeightOrSizeGradeSum + j].SubItems[5].Text = ((double)dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uWeightSumValue).ToString("0.00%");
                    }
                    LvwFruitData.Items[i * dataInterface.WeightOrSizeGradeSum + j].SubItems[6].Text = dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString();
                    if (uBoxSumValue == 0)
                    {
                        LvwFruitData.Items[i * dataInterface.WeightOrSizeGradeSum + j].SubItems[7].Text = "0.00%";
                    }
                    else
                    {
                        LvwFruitData.Items[i * dataInterface.WeightOrSizeGradeSum + j].SubItems[7].Text = ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uBoxSumValue).ToString("0.00%");
                    }
                    //LvwFruitData.Items.Add(lv);
                }
            }
            #endregion

            //#region ListView单双行换色
            //for (int i = 0; i < LvwFruitData.Items.Count; i++)
            //{
            //    if (i % 2 == 0)
            //    {
            //        LvwFruitData.Items[i].BackColor = DrawGraphProtocol.myBackColor;
            //    }
            //}
            //#endregion
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        private void FillcurrentSelectTabpage6()
        {

            #region 变量声明
            UInt64 uMaxValue = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nWeightGradeCount);
            UInt64 uNumSumValue = 0;
            UInt64 uWeightSumValue = 0;
            UInt64 uBoxSumValue = 0;
            uNumSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);
            uWeightSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);
            uBoxSumValue = (ulong)FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);
            //string strSizeName;
            //string strQualityName;
            //string strMixName;
            #endregion
           

            if (LvwSizeOrWeightData.Items.Count != dataInterface.WeightOrSizeGradeSum)
                return;

            #region 往LvwSizeData插入数据
            for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
            {
                ulong GradeCount = 0;//add by xcw 20201104
                double CountPer = 0.0;
                double GradeWeight = 0.0;
                double WeightPer = 0.0;
                int BoxSumValue = 0;
                double BoxSumPer = 0.0;
                for (int i = 0; i < dataInterface.QualityGradeSum; i++)
                {
                    GradeCount += dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                    if (uNumSumValue != 0)
                    {
                        CountPer += ((double)dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uNumSumValue);
                    }
                    GradeWeight += (dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / 1000.0);
                    if (uWeightSumValue != 0)
                    {
                        WeightPer += ((double)dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uWeightSumValue);
                    }
                    BoxSumValue += dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];

                    if (uBoxSumValue != 0)
                    {
                        BoxSumPer += ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uBoxSumValue);
                    }
                }
                LvwSizeOrWeightData.Items[j].SubItems[2].Text = GradeCount.ToString();
                LvwSizeOrWeightData.Items[j].SubItems[3].Text = CountPer.ToString("0.00%");
                LvwSizeOrWeightData.Items[j].SubItems[4].Text = GradeWeight.ToString("0.0");
                LvwSizeOrWeightData.Items[j].SubItems[5].Text = WeightPer.ToString("0.00%");
                LvwSizeOrWeightData.Items[j].SubItems[6].Text = BoxSumValue.ToString();
                LvwSizeOrWeightData.Items[j].SubItems[7].Text = BoxSumPer.ToString("0.00%");

            }

            #endregion


        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    currentSelectTabpage1();
                    //MessageBox.Show("当前选中出口统计信息");                  
                    break;
                case 1:
                    currentSelectTabpage2();
                    //MessageBox.Show("当前选中品质统计信息");
                    break;
                case 2:
                    currentSelectTabpage3();
                    //MessageBox.Show("当前选中颜色统计信息");
                    break;
                case 3:
                    currentSelectTabpage4();
                    //MessageBox.Show("当前选中形状统计信息");
                    break;
                case 4:
                    currentSelectTabpage5();
                    //MessageBox.Show("当前选中数量统计信息");
                    break;
                //case 5:
                //    currentSelectTabpage6();
                //    //MessageBox.Show("当前选中箱数统计信息");
                //    break;
                //case 6:
                //    currentSelectTabpage7();
                //    //MessageBox.Show("当前选中统计表");
                //    break;
                case 5:
                    currentSelectTabpage6(); //Modify by xcw - 20201103
                    //MessageBox.Show("当前选中尺寸统计表");
                    break;
                case 6:
                    currentSelectTabpage7(); //Modify by ChengSk - 20180726
                    //MessageBox.Show("当前选中统计表");
                    break;
                default:
                    break;
            }
        }
        bool showPrintDialog;
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (tabControl1.SelectedIndex >= 0 && tabControl1.SelectedIndex <=4)
                    {
#if AutoSaveImage
                        DialogResult result = MessageBox.Show(LanguageContainer.StatisticsInfoForm3Messagebox3Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.StatisticsInfoForm3MessageboxQuestionCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if(result == DialogResult.Yes)
                        {
                            if (!Directory.Exists("SaveImage"))
                                Directory.CreateDirectory("SaveImage");

                            string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                            string extensionName = "SaveImage\\" + datetime + ".png"; //扩展名称
                            
                            switch (tabControl1.SelectedIndex)
                            {
                                case 0:
                                    PicExport.BackgroundImage.Save(extensionName);
                                    break;
                                case 1:
                                    PicQuality.BackgroundImage.Save(extensionName);
                                    break;
                                case 2:
                                    PicColor.BackgroundImage.Save(extensionName);
                                    break;
                                case 3:
                                    PicShape.BackgroundImage.Save(extensionName);
                                    break;
                                case 4:
                                    PicSize.BackgroundImage.Save("SaveImage\\" + datetime + "_1.png");
                                    PicBox.BackgroundImage.Save("SaveImage\\" + datetime + "_2.png");
                                    break;
                                case 5:
                                    //
                                    break;
                                default:
                                    break;
                            }
                            MessageBox.Show(LanguageContainer.StatisticsInfoForm3Messagebox4Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.StatisticsInfoForm3MessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
#else
                        SaveFileDialog dlg = new SaveFileDialog();
                        dlg.Filter = "JPG格式(*.jpg)|*.jpg|位图(*.bmp)|*.bmp|GIF格式(*.gif)|*.gif|PNG格式(*.png)|*.png";
                        dlg.RestoreDirectory = true;
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            string fullPath = dlg.FileName; //全路径文件名“D://123.jpg”
                            string filename = System.IO.Path.GetFileName(fullPath);  //文件名“123.jpg”
                            string extension = System.IO.Path.GetExtension(fullPath);//扩展名“.jpg”
                            string filenameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fullPath); //没有扩展名的文件名123
                            string filedirectory = System.IO.Path.GetDirectoryName(fullPath);//返回文件所在目录

                            string picsizefilename = filedirectory + "\\" + filenameWithoutExtension + "_1" + extension; //PicSize的图片名
                            string picboxfilename = filedirectory + "\\" + filenameWithoutExtension + "_2" + extension;   //PicBox的图片名

                            switch (tabControl1.SelectedIndex)
                            {
                                case 0:
                                    PicExport.BackgroundImage.Save(dlg.FileName);
                                    break;
                                case 1:
                                    PicQuality.BackgroundImage.Save(dlg.FileName);
                                    break;
                                case 2:
                                    PicColor.BackgroundImage.Save(dlg.FileName);
                                    break;
                                case 3:
                                    PicShape.BackgroundImage.Save(dlg.FileName);
                                    break;
                                case 4:
                                    PicSize.BackgroundImage.Save(picsizefilename);
                                    PicBox.BackgroundImage.Save(picboxfilename);
                                    break;
                                default:
                                    break;
                            }
                            MessageBox.Show(LanguageContainer.StatisticsInfoForm3Messagebox4Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.StatisticsInfoForm3MessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        return;
#endif
                    }
                }
                catch(Exception ex) {   }

                //printDialog1.ShowDialog();//弹出打印选项对话框
                showPrintDialog = false;
                printPreviewDialog1.Document = this.printDocument1;//设置要预览的文档
                printPreviewDialog1.ShowDialog();//弹出打印预览对话框
            }
            catch (Exception ee)
            { 
            }         
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                //统计颜色水果的总个数
                #region
                UInt64[] colorFruitNumber = new UInt64[ConstPreDefine.MAX_COLOR_GRADE_NUM];
                #region 统计各颜色水果的个数放到colorFruitNumber中
                for (int i = 0; i < dataInterface.QualityGradeSum; i++)
                {
                    for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                    {
                        UInt64 tempNum = dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                        switch (Convert.ToInt32(dataInterface.IoStStGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nColorGrade))
                        {
                            case 0:
                                colorFruitNumber[0] += tempNum;
                                break;
                            case 1:
                                colorFruitNumber[1] += tempNum;
                                break;
                            case 2:
                                colorFruitNumber[2] += tempNum;
                                break;
                            case 3:
                                colorFruitNumber[3] += tempNum;
                                break;
                            case 4:
                                colorFruitNumber[4] += tempNum;
                                break;
                            case 5:
                                colorFruitNumber[5] += tempNum;
                                break;
                            case 6:
                                colorFruitNumber[6] += tempNum;
                                break;
                            case 7:
                                colorFruitNumber[7] += tempNum;
                                break;
                            case 8:
                                colorFruitNumber[8] += tempNum;
                                break;
                            case 9:
                                colorFruitNumber[9] += tempNum;
                                break;
                            case 10:
                                colorFruitNumber[10] += tempNum;
                                break;
                            case 11:
                                colorFruitNumber[11] += tempNum;
                                break;
                            case 12:
                                colorFruitNumber[12] += tempNum;
                                break;
                            case 13:
                                colorFruitNumber[13] += tempNum;
                                break;
                            case 14:
                                colorFruitNumber[14] += tempNum;
                                break;
                            case 15:
                                colorFruitNumber[15] += tempNum;
                                break;
                            default:
                                break;
                        }
                        tempNum = 0;
                    }
                }
                #endregion
                UInt64 uColorSumValue = FunctionInterface.GetSumValue(colorFruitNumber);
                bIsHaveColorStatis = (uColorSumValue > 0 ? true : false);  //判断是否需要打印颜色统计信息
                #endregion

                //打印设置(首页一个品质+中间若干品质+最后一个颜色饼图)
                #region
                int gradeNum = dataInterface.WeightOrSizeGradeSum * dataInterface.QualityGradeSum;
                int sizeOrweightNum = dataInterface.WeightOrSizeGradeSum;
                if (sizeOrweightNum > PrintProtocol.GradeNum1) //每页最多可放两个品质
                {
                    if (sizeOrweightNum > (PrintProtocol.GradeNum1 + PrintProtocol.GradeNum2))
                    {
                        if ((sizeOrweightNum - PrintProtocol.GradeNum1) % PrintProtocol.GradeNum2 != 0)
                            intPage1 = (sizeOrweightNum - PrintProtocol.GradeNum1) / PrintProtocol.GradeNum2 + 2;
                        else
                            intPage1 = (sizeOrweightNum - PrintProtocol.GradeNum1) / PrintProtocol.GradeNum2 + 1;
                    }
                    else
                    {
                        intPage1 = 2;
                    }
                }
                else
                {
                    //bIsTwoQualityOnePage = false;
                    intPage1 = 1;
                }
                if (gradeNum > PrintProtocol.GradeNum1) //每页最多可放两个品质
                {
                    if (gradeNum > (PrintProtocol.GradeNum1 + PrintProtocol.GradeNum2))
                    {
                        if ((gradeNum - PrintProtocol.GradeNum1) % PrintProtocol.GradeNum2 != 0)
                            intPage = (gradeNum - PrintProtocol.GradeNum1) / PrintProtocol.GradeNum2 + 2;
                        else
                            intPage = (gradeNum - PrintProtocol.GradeNum1) / PrintProtocol.GradeNum2 + 1;
                    }
                    else
                    {
                        intPage = 2;
                    }
                }
                else
                {
                    //bIsTwoQualityOnePage = false;
                    intPage = 1;
                }
                //}
                //else //不需要打印颜色统计信息
                //{
                //    if (dataInterface.WeightOrSizeGradeSum <= 4) //每页最多可放两个品质
                //    {
                //        bIsTwoQualityOnePage = true; //标志
                //        if (dataInterface.QualityGradeSum % 2 == 0)
                //        {
                //            intPage = dataInterface.QualityGradeSum / 2 + 1;
                //        }
                //        else
                //        {
                //            intPage = dataInterface.QualityGradeSum / 2 + 1;
                //        }
                //    }
                //    else  //每页最多仅可放一个品质
                //    {
                //        bIsTwoQualityOnePage = false;
                //        intPage = dataInterface.QualityGradeSum;
                //    }
                //}          
                #endregion

                //打印定义
                #region
                int currentAvailableHeight = 0;   //当前可用高度

                int cylinderWidth = 0;            //条柱宽度
                int cylinderSpace = 0;            //圆柱间间距
                int cylinderLeftMargin = 0;       //条柱左边距

                int sumTableHeight = 0;      //汇总表格高度
                int sumTalbeWidth = 0;       //汇总表格宽度

                int cylinderDigitLeftDistance = 0;//数字左边框
                #endregion

                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        #region 打印出口统计信息
                        //打印时间
                        #region
                        currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                        Font dateTimeFont0 = new Font("宋体", 15, FontStyle.Regular);
                        Brush dateTimeBrush0 = Brushes.Black;
                        string nowDateTime0 = DateTime.Now.ToString(m_resourceManager.GetString("LblPrintDateTime.Text"));
                        PictureBox picB0 = new PictureBox();
                        Graphics TitG0 = picB0.CreateGraphics();
                        SizeF XMaxSize0 = TitG0.MeasureString(nowDateTime0, dateTimeFont0);
                        e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                            nowDateTime0, dateTimeFont0, dateTimeBrush0, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize0.Width, currentAvailableHeight);
                        #endregion

                        //打印LOGO
                        #region
                        currentAvailableHeight += (int)XMaxSize0.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
                        try
                        {
                            Bitmap bitmap = new Bitmap(PrintProtocol.logoPathName);//创建位图对象        
                            e.Graphics.DrawImage(bitmap,
                                (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - bitmap.Width) / 2 + PrintProtocol.leftMargin, currentAvailableHeight, bitmap.Width, bitmap.Height);
                            currentAvailableHeight += bitmap.Height + PrintProtocol.logoOrTextTitleSpace;    //当前可用高度
                        }
                        catch (Exception ee)//捕获异常
                        {
                            MessageBox.Show(ee.Message);//弹出消息对话框
                        }
                        #endregion

                        //文本标题
                        #region
                        Font textTitleFont0 = new Font("宋体", 20, FontStyle.Bold);
                        Brush textTitleBrush0 = Brushes.Black;
                        string textTitle0 = m_resourceManager.GetString("LblPrintClassified.Text");
                        XMaxSize0 = TitG0.MeasureString(textTitle0, textTitleFont0);
                        e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                            textTitle0, textTitleFont0, textTitleBrush0, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize0.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion

                        //文本内容
                        #region
                        currentAvailableHeight += (int)XMaxSize0.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                        Font textContentFont0 = new Font("宋体", 15, FontStyle.Regular);
                        Brush textContentBrush0 = Brushes.Black;
                        string textContent0 = m_resourceManager.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName +
                            "    " + m_resourceManager.GetString("LblPrintFarmName.Text") + dataInterface.FarmName +
                            "    " + m_resourceManager.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
                        XMaxSize0 = TitG0.MeasureString(textContent0, textContentFont0);
                        e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                            textContent0, textContentFont0, textContentBrush0, PrintProtocol.leftMargin, currentAvailableHeight);
                        #endregion

                        //分割线
                        #region
                        currentAvailableHeight += (int)XMaxSize0.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
                        Pen linePen0 = new Pen(Color.Black, 2);
                        e.Graphics.DrawLine(linePen0, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                        #endregion

                        //条形图头部
                        #region
                        currentAvailableHeight += (int)linePen0.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                        Font barHeadFont0 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                        Brush barHeadBrush0 = Brushes.Black;
                        string barHead0 = m_resourceManager.GetString("LblPrintNumOrPercent.Text");
                        XMaxSize0 = TitG0.MeasureString(barHead0, barHeadFont0);
                        e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                            barHead0, barHeadFont0, barHeadBrush0, PrintProtocol.leftMargin, currentAvailableHeight);
                        #endregion

                        //直方图（或条形图）
                        #region
                        currentAvailableHeight += (int)XMaxSize0.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
                        //Bitmap bitM0 = new Bitmap(PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);
                        //Graphics g0 = Graphics.FromImage(bitM0);
                        //g0.Clear(PrintProtocol.barBackColor);
                        e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                        Brush myCurrentBrush0 = new SolidBrush(PrintProtocol.penColor);
                        Brush myBarBrush = new SolidBrush(PrintProtocol.barBrush);
                        //界面设定
                        if ((dataInterface.ExportSum - 1) * (PrintProtocol.cylinderWidth1 + PrintProtocol.cylinderSpace1) + PrintProtocol.cylinderWidth1 < PrintProtocol.barImageWidht)
                        {
                            cylinderWidth = PrintProtocol.cylinderWidth1;
                            cylinderSpace = PrintProtocol.cylinderSpace1;
                            cylinderLeftMargin = (PrintProtocol.barImageWidht - (dataInterface.ExportSum - 1) * (PrintProtocol.cylinderWidth1 + PrintProtocol.cylinderSpace1) - PrintProtocol.cylinderWidth1) / 2;
                            cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance1;
                        }
                        else
                        {
                            //cylinderWidth = PrintProtocol.cylinderWidth2;
                            //cylinderSpace = PrintProtocol.cylinderSpace2;
                            cylinderWidth = (PrintProtocol.barImageWidht - (dataInterface.ExportSum - 1) * 5) / dataInterface.ExportSum;
                            cylinderSpace = 5;
                            cylinderLeftMargin = (PrintProtocol.barImageWidht - (dataInterface.ExportSum - 1) * (cylinderWidth + cylinderSpace) - cylinderWidth) / 2;
                            cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance2;
                        }
                        UInt64 uMaxValue = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nExitCount);
                        UInt64 uSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nExitCount);
                        for (int i = 0; i < dataInterface.ExportSum; i++)
                        {
                            //柱形图下标字样
                            e.Graphics.DrawString((i + 1).ToString(), new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush0,
                                cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin + PrintProtocol.cylinderOrTextNote);
                            if (dataInterface.IoStStatistics.nExitCount[i] == 0)
                            {
                                //柱形图显示
                                e.Graphics.FillRectangle(myBarBrush, cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight,
                                    cylinderWidth, PrintProtocol.cylinderMinHeight);
                            }
                            else
                            {
                                //柱形图显示
                                e.Graphics.FillRectangle(myBarBrush, cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)),
                                    cylinderWidth, (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)));
                                if ((int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)) > PrintProtocol.cylinderStandardCylinderHeigh)
                                {
                                    e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                        PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - cylinderDigitLeftDistance);
                                    e.Graphics.RotateTransform(-90f);
                                    e.Graphics.DrawString(dataInterface.IoStStatistics.nExitCount[i].ToString(),
                                        new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush0, 0, 0);
                                    e.Graphics.ResetTransform();
                                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                                }
                                else
                                {
                                    e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                        PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)) - PrintProtocol.cylinderDigitBottomDistance);
                                    e.Graphics.RotateTransform(-90f);
                                    e.Graphics.DrawString(dataInterface.IoStStatistics.nExitCount[i].ToString(),
                                        new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush0, 0, 0);
                                    e.Graphics.ResetTransform();
                                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                                }
                            }
                            if (uSumValue == 0)
                            {
                                //百分比
                                e.Graphics.DrawString("0.00%", new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush0,
                                    cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight - PrintProtocol.cylinderPrecentBottomDistance);
                            }
                            else
                            {
                                //百分比
                                e.Graphics.DrawString(((double)dataInterface.IoStStatistics.nExitCount[i] / uSumValue).ToString("0.00%"),
                                    new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush0,
                                    cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)) - PrintProtocol.cylinderPrecentBottomDistance);
                            }
                        }
                        e.Graphics.ResetTransform();
                        //try
                        //{
                        //    e.Graphics.DrawImage(bitM0, PrintProtocol.leftMargin, currentAvailableHeight, PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);

                        //}
                        //catch (Exception ee)//捕获异常
                        //{
                        //    MessageBox.Show(ee.Message);//弹出消息对话框
                        //}
                        #endregion

                        //条形图标题
                        #region
                        //currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + bitM0.Height;   //当前可用高度
                        currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + PrintProtocol.barImageHeight;
                        Font barTitleFont = new Font("楷体_GB2312", 18, FontStyle.Bold);
                        Brush barTitleBrush = Brushes.Black;
                        string barTitle = m_resourceManager.GetString("tabPage1.Text");
                        XMaxSize0 = TitG0.MeasureString(barTitle, barTitleFont);
                        e.Graphics.DrawString(//使用DrawString方法绘制条形图标题字符串
                            barTitle, barTitleFont, barTitleBrush, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize0.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion

                        //打印页数
                        #region
                        currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                        Font currentPagesFont0 = new Font("宋体", 12, FontStyle.Regular);
                        Brush currentPagesBrush0 = Brushes.Black;
                        string currentPages0 = m_resourceManager.GetString("LblPrintPages.Text") + " 1";
                        XMaxSize0 = TitG0.MeasureString(currentPages0, currentPagesFont0);
                        e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                            currentPages0, currentPagesFont0, currentPagesBrush0, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize0.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion
                        #endregion
                        //MessageBox.Show("当前选中出口统计信息");                  
                        break;
                    case 1:
                        #region 打印品质统计信息
                        //打印时间
                        #region
                        currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                        Font dateTimeFont1 = new Font("宋体", 15, FontStyle.Regular);
                        Brush dateTimeBrush1 = Brushes.Black;
                        string nowDateTime1 = DateTime.Now.ToString(m_resourceManager.GetString("LblPrintDateTime.Text"));
                        PictureBox picB1 = new PictureBox();
                        Graphics TitG1 = picB1.CreateGraphics();
                        SizeF XMaxSize1 = TitG1.MeasureString(nowDateTime1, dateTimeFont1);
                        e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                            nowDateTime1, dateTimeFont1, dateTimeBrush1, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize1.Width, currentAvailableHeight);
                        #endregion

                        //打印LOGO
                        #region
                        currentAvailableHeight += (int)XMaxSize1.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
                        try
                        {
                            Bitmap bitmap = new Bitmap(PrintProtocol.logoPathName);//创建位图对象        
                            e.Graphics.DrawImage(bitmap,
                                (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - bitmap.Width) / 2 + PrintProtocol.leftMargin, currentAvailableHeight, bitmap.Width, bitmap.Height);
                            currentAvailableHeight += bitmap.Height + PrintProtocol.logoOrTextTitleSpace;    //当前可用高度
                        }
                        catch (Exception ee)//捕获异常
                        {
                            MessageBox.Show(ee.Message);//弹出消息对话框
                        }
                        #endregion

                        //文本标题
                        #region
                        Font textTitleFont1 = new Font("宋体", 20, FontStyle.Bold);
                        Brush textTitleBrush1 = Brushes.Black;
                        string textTitle1 = m_resourceManager.GetString("LblPrintClassified.Text");
                        XMaxSize1 = TitG1.MeasureString(textTitle1, textTitleFont1);
                        e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                            textTitle1, textTitleFont1, textTitleBrush1, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize1.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion

                        //文本内容
                        #region
                        currentAvailableHeight += (int)XMaxSize1.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                        Font textContentFont1 = new Font("宋体", 15, FontStyle.Regular);
                        Brush textContentBrush1 = Brushes.Black;
                        string textContent1 = m_resourceManager.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName +
                            "    " + m_resourceManager.GetString("LblPrintFarmName.Text") + dataInterface.FarmName +
                            "    " + m_resourceManager.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
                        XMaxSize1 = TitG1.MeasureString(textContent1, textContentFont1);
                        e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                            textContent1, textContentFont1, textContentBrush1, PrintProtocol.leftMargin, currentAvailableHeight);
                        #endregion

                        //分割线
                        #region
                        currentAvailableHeight += (int)XMaxSize1.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
                        Pen linePen1 = new Pen(Color.Black, 2);
                        e.Graphics.DrawLine(linePen1, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                        #endregion

                        //条形图头部
                        #region
                        currentAvailableHeight += (int)linePen1.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                        Font barHeadFont1 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                        Brush barHeadBrush1 = Brushes.Black;
                        string barHead1 = m_resourceManager.GetString("LblPrintNumOrPercent.Text");
                        XMaxSize1 = TitG1.MeasureString(barHead1, barHeadFont1);
                        e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                            barHead1, barHeadFont1, barHeadBrush1, PrintProtocol.leftMargin, currentAvailableHeight);
                        #endregion

                        //直方图（或条形图）
                        #region
                        currentAvailableHeight += (int)XMaxSize1.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
                        //Bitmap bitM1 = new Bitmap(PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);
                        //Graphics g1 = Graphics.FromImage(bitM1);
                        //g1.Clear(PrintProtocol.barBackColor);
                        e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                        Brush myCurrentBrush1 = new SolidBrush(PrintProtocol.penColor);
                        Brush myBarBrush1 = new SolidBrush(PrintProtocol.barBrush);
                        //界面设定
                        if ((dataInterface.QualityGradeSum - 1) * (PrintProtocol.cylinderWidth1 + PrintProtocol.cylinderSpace1) + PrintProtocol.cylinderWidth1 < PrintProtocol.barImageWidht)
                        {
                            cylinderWidth = PrintProtocol.cylinderWidth1;
                            cylinderSpace = PrintProtocol.cylinderSpace1;
                            cylinderLeftMargin = (PrintProtocol.barImageWidht - (dataInterface.QualityGradeSum - 1) * (PrintProtocol.cylinderWidth1 + PrintProtocol.cylinderSpace1) - PrintProtocol.cylinderWidth1) / 2;
                            cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance1;
                        }
                        else
                        {
                            //cylinderWidth = PrintProtocol.cylinderWidth2;
                            //cylinderSpace = PrintProtocol.cylinderSpace2;
                            cylinderWidth = (PrintProtocol.barImageWidht - (dataInterface.QualityGradeSum - 1) * 5) / dataInterface.QualityGradeSum;
                            cylinderSpace = 5;
                            cylinderLeftMargin = (PrintProtocol.barImageWidht - (dataInterface.QualityGradeSum - 1) * (cylinderWidth + cylinderSpace) - cylinderWidth) / 2;
                            cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance2;
                        }
                        UInt64 totalFruitNumber1 = 0;  //每品质水果的总个数
                        UInt64 maxFruitNumber1 = 0;    //所有品质中水果总个数最大值
                        //找各品质中的最大值
                        for (int i = 0; i < dataInterface.QualityGradeSum; i++)
                        {
                            UInt64 temp = 0;
                            for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                            {
                                //temp += dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                                temp += dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j]; //Modify by ChengSk - 20190401
                            }
                            if (maxFruitNumber1 < temp)
                            {
                                maxFruitNumber1 = temp;
                            }
                        }
                        for (int i = 0; i < dataInterface.QualityGradeSum; i++)
                        {
                            //柱形图下标字样
                            e.Graphics.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH),
                                new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush1,
                                cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin + PrintProtocol.cylinderOrTextNote);
                            for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                            {
                                //totalFruitNumber1 += dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                                totalFruitNumber1 += dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];//Modify by ChengSk - 20181205
                            }
                            if (totalFruitNumber1 == 0)
                            {
                                //柱形图显示
                                e.Graphics.FillRectangle(myBarBrush1, cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight,
                                    cylinderWidth, PrintProtocol.cylinderMinHeight);
                            }
                            else
                            {
                                //柱形图显示
                                e.Graphics.FillRectangle(myBarBrush1, cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)totalFruitNumber1 / maxFruitNumber1)),
                                    cylinderWidth, (int)(PrintProtocol.cylinderMaxHeight * ((double)totalFruitNumber1 / maxFruitNumber1)));
                                if ((int)(PrintProtocol.cylinderMaxHeight * ((double)totalFruitNumber1 / maxFruitNumber1)) > PrintProtocol.cylinderStandardCylinderHeigh)
                                {
                                    e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                        PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - cylinderDigitLeftDistance);
                                    e.Graphics.RotateTransform(-90f);
                                    e.Graphics.DrawString(totalFruitNumber1.ToString(),
                                        new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush1, 0, 0);
                                    e.Graphics.ResetTransform();
                                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                                }
                                else
                                {
                                    e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                        PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)totalFruitNumber1 / maxFruitNumber1)) - PrintProtocol.cylinderDigitBottomDistance);
                                    e.Graphics.RotateTransform(-90f);
                                    e.Graphics.DrawString(totalFruitNumber1.ToString(),
                                        new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush1, 0, 0);
                                    e.Graphics.ResetTransform();
                                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                                }
                            }
                            if (dataInterface.IoStStatistics.nTotalCount == 0)
                            {
                                //百分比
                                e.Graphics.DrawString("0.00%", new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush1,
                                    cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight - PrintProtocol.cylinderPrecentBottomDistance);
                            }
                            else
                            {
                                //百分比
                                e.Graphics.DrawString(((double)totalFruitNumber1 / dataInterface.IoStStatistics.nTotalCount).ToString("0.00%"),
                                    new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush1,
                                    cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)totalFruitNumber1 / maxFruitNumber1)) - PrintProtocol.cylinderPrecentBottomDistance);
                            }
                            totalFruitNumber1 = 0;
                        }
                        e.Graphics.ResetTransform();
                        //try
                        //{
                        //    e.Graphics.DrawImage(bitM1, PrintProtocol.leftMargin, currentAvailableHeight, PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);

                        //}
                        //catch (Exception ee)//捕获异常
                        //{
                        //    MessageBox.Show(ee.Message);//弹出消息对话框
                        //}
                        #endregion

                        //条形图标题
                        #region
                        //currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + bitM1.Height;   //当前可用高度
                        currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + PrintProtocol.barImageHeight;
                        Font barTitleFont1 = new Font("楷体_GB2312", 18, FontStyle.Bold);
                        Brush barTitleBrush1 = Brushes.Black;
                        string barTitle1 = m_resourceManager.GetString("tabPage2.Text");
                        XMaxSize1 = TitG1.MeasureString(barTitle1, barTitleFont1);
                        e.Graphics.DrawString(//使用DrawString方法绘制条形图标题字符串
                            barTitle1, barTitleFont1, barTitleBrush1, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize1.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion

                        //打印页数
                        #region
                        currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                        Font currentPagesFont1 = new Font("宋体", 12, FontStyle.Regular);
                        Brush currentPagesBrush1 = Brushes.Black;
                        string currentPages1 = m_resourceManager.GetString("LblPrintPages.Text") + " 1";
                        XMaxSize1 = TitG1.MeasureString(currentPages1, currentPagesFont1);
                        e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                            currentPages1, currentPagesFont1, currentPagesBrush1, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize1.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion
                        #endregion
                        //MessageBox.Show("当前选中品质统计信息");
                        break;
                    case 2:
                        #region 打印颜色统计信息
                        //打印时间
                        #region
                        currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                        Font dateTimeFont2 = new Font("宋体", 15, FontStyle.Regular);
                        Brush dateTimeBrush2 = Brushes.Black;
                        string nowDateTime2 = DateTime.Now.ToString(m_resourceManager.GetString("LblPrintDateTime.Text"));
                        PictureBox picB2 = new PictureBox();
                        Graphics TitG2 = picB2.CreateGraphics();
                        SizeF XMaxSize2 = TitG2.MeasureString(nowDateTime2, dateTimeFont2);
                        e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                            nowDateTime2, dateTimeFont2, dateTimeBrush2, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize2.Width, currentAvailableHeight);
                        #endregion

                        //打印LOGO
                        #region
                        currentAvailableHeight += (int)XMaxSize2.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
                        try
                        {
                            Bitmap bitmap = new Bitmap(PrintProtocol.logoPathName);//创建位图对象        
                            e.Graphics.DrawImage(bitmap,
                                (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - bitmap.Width) / 2 + PrintProtocol.leftMargin, currentAvailableHeight, bitmap.Width, bitmap.Height);
                            currentAvailableHeight += bitmap.Height + PrintProtocol.logoOrTextTitleSpace;    //当前可用高度
                        }
                        catch (Exception ee)//捕获异常
                        {
                            MessageBox.Show(ee.Message);//弹出消息对话框
                        }
                        #endregion

                        //文本标题
                        #region
                        Font textTitleFont2 = new Font("宋体", 20, FontStyle.Bold);
                        Brush textTitleBrush2 = Brushes.Black;
                        string textTitle2 = m_resourceManager.GetString("LblPrintClassified.Text");
                        XMaxSize2 = TitG2.MeasureString(textTitle2, textTitleFont2);
                        e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                            textTitle2, textTitleFont2, textTitleBrush2, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize2.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion

                        //文本内容
                        #region
                        currentAvailableHeight += (int)XMaxSize2.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                        Font textContentFont2 = new Font("宋体", 15, FontStyle.Regular);
                        Brush textContentBrush2 = Brushes.Black;
                        string textContent2 = m_resourceManager.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName +
                            "    " + m_resourceManager.GetString("LblPrintFarmName.Text") + dataInterface.FarmName +
                            "    " + m_resourceManager.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
                        XMaxSize2 = TitG2.MeasureString(textContent2, textContentFont2);
                        e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                            textContent2, textContentFont2, textContentBrush2, PrintProtocol.leftMargin, currentAvailableHeight);
                        #endregion

                        //分割线
                        #region
                        currentAvailableHeight += (int)XMaxSize2.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
                        Pen linePen2 = new Pen(Color.Black, 2);
                        e.Graphics.DrawLine(linePen2, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                        #endregion

                        //条形图头部
                        #region
                        currentAvailableHeight += (int)linePen2.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                        Font barHeadFont2 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                        Brush barHeadBrush2 = Brushes.Black;
                        string barHead2 = m_resourceManager.GetString("LblPrintNumOrPercent.Text");
                        XMaxSize2 = TitG2.MeasureString(barHead2, barHeadFont2);
                        e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                            barHead2, barHeadFont2, barHeadBrush2, PrintProtocol.leftMargin, currentAvailableHeight);
                        #endregion

                        //直方图（或条形图）
                        #region
                        currentAvailableHeight += (int)XMaxSize2.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
                        //Bitmap bitM2 = new Bitmap(PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);
                        //Graphics g2 = Graphics.FromImage(bitM2);
                        //g2.Clear(PrintProtocol.barBackColor);
                        e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                        Brush myCurrentBrush2 = new SolidBrush(PrintProtocol.penColor);
                        Brush myBarBrush2 = new SolidBrush(PrintProtocol.barBrush);
                        //统计各颜色水果的个数
                        UInt64[] colorFruitNumber2 = new UInt64[ConstPreDefine.MAX_COLOR_GRADE_NUM];
                        string strColorGradeName2 = dataInterface.ColorGradeName;
                        string[] colorGrade2;
                        if (strColorGradeName2 == null || strColorGradeName2.Equals(""))
                        {
                            colorGrade2 = new string[0];
                        }
                        else
                        {
                            colorGrade2 = strColorGradeName2.Split('，');
                        }
                        #region 统计各颜色水果的个数放到colorFruitNumber2中
                        for (int i = 0; i < dataInterface.QualityGradeSum; i++)
                        {
                            for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                            {
                                UInt64 tempNum = dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                                switch (Convert.ToInt32(dataInterface.IoStStGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nColorGrade))
                                {
                                    case 0:
                                        colorFruitNumber2[0] += tempNum;
                                        break;
                                    case 1:
                                        colorFruitNumber2[1] += tempNum;
                                        break;
                                    case 2:
                                        colorFruitNumber2[2] += tempNum;
                                        break;
                                    case 3:
                                        colorFruitNumber2[3] += tempNum;
                                        break;
                                    case 4:
                                        colorFruitNumber2[4] += tempNum;
                                        break;
                                    case 5:
                                        colorFruitNumber2[5] += tempNum;
                                        break;
                                    case 6:
                                        colorFruitNumber2[6] += tempNum;
                                        break;
                                    case 7:
                                        colorFruitNumber2[7] += tempNum;
                                        break;
                                    case 8:
                                        colorFruitNumber2[8] += tempNum;
                                        break;
                                    case 9:
                                        colorFruitNumber2[9] += tempNum;
                                        break;
                                    case 10:
                                        colorFruitNumber2[10] += tempNum;
                                        break;
                                    case 11:
                                        colorFruitNumber2[11] += tempNum;
                                        break;
                                    case 12:
                                        colorFruitNumber2[12] += tempNum;
                                        break;
                                    case 13:
                                        colorFruitNumber2[13] += tempNum;
                                        break;
                                    case 14:
                                        colorFruitNumber2[14] += tempNum;
                                        break;
                                    case 15:
                                        colorFruitNumber2[15] += tempNum;
                                        break;
                                    default:
                                        break;
                                }
                                tempNum = 0;
                            }
                        }
                        #endregion
                        UInt64 uMaxValue2 = FunctionInterface.GetMaxValue(colorFruitNumber2);
                        UInt64 uSumValue2 = FunctionInterface.GetSumValue(colorFruitNumber2);
                        //界面设定
                        if ((colorGrade2.Length - 1) * (PrintProtocol.cylinderWidth1 + PrintProtocol.cylinderSpace1) + PrintProtocol.cylinderWidth1 < PrintProtocol.barImageWidht)
                        {
                            cylinderWidth = PrintProtocol.cylinderWidth1;
                            cylinderSpace = PrintProtocol.cylinderSpace1;
                            cylinderLeftMargin = (PrintProtocol.barImageWidht - (colorGrade2.Length - 1) * (PrintProtocol.cylinderWidth1 + PrintProtocol.cylinderSpace1) - PrintProtocol.cylinderWidth1) / 2;
                            cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance1;
                        }
                        else
                        {
                            //cylinderWidth = PrintProtocol.cylinderWidth2;
                            //cylinderSpace = PrintProtocol.cylinderSpace2;
                            cylinderWidth = (PrintProtocol.barImageWidht - (colorGrade2.Length - 1) * 5) / colorGrade2.Length;
                            cylinderSpace = 5;
                            cylinderLeftMargin = (PrintProtocol.barImageWidht - (colorGrade2.Length - 1) * (cylinderWidth + cylinderSpace) - cylinderWidth) / 2;
                            cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance2;
                        }
                        for (int i = 0; i < colorGrade2.Length; i++)
                        {
                            //柱形图下标字样
                            e.Graphics.DrawString(colorGrade2[i],
                                new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush2,
                                cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin + PrintProtocol.cylinderOrTextNote);
                            if (colorFruitNumber2[i] == 0)
                            {
                                //柱形图显示
                                e.Graphics.FillRectangle(myBarBrush2, cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight,
                                    cylinderWidth, PrintProtocol.cylinderMinHeight);
                            }
                            else
                            {
                                //柱形图显示
                                e.Graphics.FillRectangle(myBarBrush2, cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)colorFruitNumber2[i] / uMaxValue2)),
                                    cylinderWidth, (int)(PrintProtocol.cylinderMaxHeight * ((double)(double)colorFruitNumber2[i] / uMaxValue2)));
                                if ((int)(PrintProtocol.cylinderMaxHeight * ((double)(double)colorFruitNumber2[i] / uMaxValue2)) > PrintProtocol.cylinderStandardCylinderHeigh)
                                {
                                    e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                        PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - cylinderDigitLeftDistance);
                                    e.Graphics.RotateTransform(-90f);
                                    e.Graphics.DrawString(colorFruitNumber2[i].ToString(),
                                        new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush2, 0, 0);
                                    e.Graphics.ResetTransform();
                                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                                }
                                else
                                {
                                    e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                        PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)colorFruitNumber2[i] / uMaxValue2)) - PrintProtocol.cylinderDigitBottomDistance);
                                    e.Graphics.RotateTransform(-90f);
                                    e.Graphics.DrawString(colorFruitNumber2[i].ToString(),
                                        new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush2, 0, 0);
                                    e.Graphics.ResetTransform();
                                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                                }
                            }
                            if (uSumValue2 == 0)
                            {
                                //百分比
                                e.Graphics.DrawString("0.00%", new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush2,
                                    cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight - PrintProtocol.cylinderPrecentBottomDistance);
                            }
                            else
                            {
                                //百分比
                                e.Graphics.DrawString(((double)colorFruitNumber2[i] / uSumValue2).ToString("0.00%"),
                                    new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush2,
                                    cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)colorFruitNumber2[i] / uMaxValue2)) - PrintProtocol.cylinderPrecentBottomDistance);
                            }
                        }
                        e.Graphics.ResetTransform();
                        //try
                        //{
                        //    e.Graphics.DrawImage(bitM2, PrintProtocol.leftMargin, currentAvailableHeight, PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);

                        //}
                        //catch (Exception ee)//捕获异常
                        //{
                        //    MessageBox.Show(ee.Message);//弹出消息对话框
                        //}
                        #endregion

                        //条形图标题
                        #region
                        //currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + bitM2.Height;   //当前可用高度
                        currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + PrintProtocol.barImageHeight;
                        Font barTitleFont2 = new Font("楷体_GB2312", 18, FontStyle.Bold);
                        Brush barTitleBrush2 = Brushes.Black;
                        string barTitle2 = m_resourceManager.GetString("tabPage3.Text");
                        XMaxSize2 = TitG2.MeasureString(barTitle2, barTitleFont2);
                        e.Graphics.DrawString(//使用DrawString方法绘制条形图标题字符串
                            barTitle2, barTitleFont2, barTitleBrush2, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize2.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion

                        //打印页数
                        #region
                        currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                        Font currentPagesFont2 = new Font("宋体", 12, FontStyle.Regular);
                        Brush currentPagesBrush2 = Brushes.Black;
                        string currentPages2 = m_resourceManager.GetString("LblPrintPages.Text") + " 1";
                        XMaxSize2 = TitG2.MeasureString(currentPages2, currentPagesFont2);
                        e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                            currentPages2, currentPagesFont2, currentPagesBrush2, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize2.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion
                        #endregion
                        //MessageBox.Show("当前选中颜色统计信息");
                        break;
                    case 3:
                        #region 打印形状统计信息
                        //打印时间
                        #region
                        currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                        Font dateTimeFont3 = new Font("宋体", 15, FontStyle.Regular);
                        Brush dateTimeBrush3 = Brushes.Black;
                        string nowDateTime3 = DateTime.Now.ToString(m_resourceManager.GetString("LblPrintDateTime.Text"));
                        PictureBox picB3 = new PictureBox();
                        Graphics TitG3 = picB3.CreateGraphics();
                        SizeF XMaxSize3 = TitG3.MeasureString(nowDateTime3, dateTimeFont3);
                        e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                            nowDateTime3, dateTimeFont3, dateTimeBrush3, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize3.Width, currentAvailableHeight);
                        #endregion

                        //打印LOGO
                        #region
                        currentAvailableHeight += (int)XMaxSize3.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
                        try
                        {
                            Bitmap bitmap = new Bitmap(PrintProtocol.logoPathName);//创建位图对象        
                            e.Graphics.DrawImage(bitmap,
                                (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - bitmap.Width) / 2 + PrintProtocol.leftMargin, currentAvailableHeight, bitmap.Width, bitmap.Height);
                            currentAvailableHeight += bitmap.Height + PrintProtocol.logoOrTextTitleSpace;    //当前可用高度
                        }
                        catch (Exception ee)//捕获异常
                        {
                            MessageBox.Show(ee.Message);//弹出消息对话框
                        }
                        #endregion

                        //文本标题
                        #region
                        Font textTitleFont3 = new Font("宋体", 20, FontStyle.Bold);
                        Brush textTitleBrush3 = Brushes.Black;
                        string textTitle3 = m_resourceManager.GetString("LblPrintClassified.Text");
                        XMaxSize3 = TitG3.MeasureString(textTitle3, textTitleFont3);
                        e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                            textTitle3, textTitleFont3, textTitleBrush3, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize3.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion

                        //文本内容
                        #region
                        currentAvailableHeight += (int)XMaxSize3.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                        Font textContentFont3 = new Font("宋体", 15, FontStyle.Regular);
                        Brush textContentBrush3 = Brushes.Black;
                        string textContent3 = m_resourceManager.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName +
                            "    " + m_resourceManager.GetString("LblPrintFarmName.Text") + dataInterface.FarmName +
                            "    " + m_resourceManager.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
                        XMaxSize3 = TitG3.MeasureString(textContent3, textContentFont3);
                        e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                            textContent3, textContentFont3, textContentBrush3, PrintProtocol.leftMargin, currentAvailableHeight);
                        #endregion

                        //分割线
                        #region
                        currentAvailableHeight += (int)XMaxSize3.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
                        Pen linePen3 = new Pen(Color.Black, 2);
                        e.Graphics.DrawLine(linePen3, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                        #endregion

                        //条形图头部
                        #region
                        currentAvailableHeight += (int)linePen3.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                        Font barHeadFont3 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                        Brush barHeadBrush3 = Brushes.Black;
                        string barHead3 = m_resourceManager.GetString("LblPrintNumOrPercent.Text");
                        XMaxSize3 = TitG3.MeasureString(barHead3, barHeadFont3);
                        e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                            barHead3, barHeadFont3, barHeadBrush3, PrintProtocol.leftMargin, currentAvailableHeight);
                        #endregion

                        //直方图（或条形图）
                        #region
                        currentAvailableHeight += (int)XMaxSize3.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
                        //Bitmap bitM3 = new Bitmap(PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);
                        //Graphics g3 = Graphics.FromImage(bitM3);
                        //g3.Clear(PrintProtocol.barBackColor);
                        e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                        Brush myCurrentBrush3 = new SolidBrush(PrintProtocol.penColor);
                        Brush myBarBrush3 = new SolidBrush(PrintProtocol.barBrush);
                        //统计各形状水果的个数
                        UInt64[] shapeFruitNumber3 = new UInt64[ConstPreDefine.MAX_SHAPE_GRADE_NUM];
                        string strShapeGradeName3 = dataInterface.ShapeGradeName;
                        string[] shapeGrade3;
                        if (strShapeGradeName3 == null || strShapeGradeName3.Equals(""))
                        {
                            shapeGrade3 = new string[0];
                        }
                        else
                        {
                            shapeGrade3 = strShapeGradeName3.Split('，');
                        }
                        #region 统计各颜色水果的个数放到shapeFruitNumber3中
                        for (int i = 0; i < dataInterface.QualityGradeSum; i++)
                        {
                            for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                            {
                                UInt64 tempNum = dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                                switch (Convert.ToInt32(dataInterface.IoStStGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbShapeSize))
                                {
                                    case 0:
                                        shapeFruitNumber3[0] += tempNum;
                                        break;
                                    case 1:
                                        shapeFruitNumber3[1] += tempNum;
                                        break;
                                    default:
                                        break;
                                }
                                tempNum = 0;
                            }
                        }
                        #endregion
                        UInt64 uMaxValue3 = FunctionInterface.GetMaxValue(shapeFruitNumber3);
                        UInt64 uSumValue3 = FunctionInterface.GetSumValue(shapeFruitNumber3);
                        //界面设定
                        if ((shapeGrade3.Length - 1) * (PrintProtocol.cylinderWidth1 + PrintProtocol.cylinderSpace1) + PrintProtocol.cylinderWidth1 < PrintProtocol.barImageWidht)
                        {
                            cylinderWidth = PrintProtocol.cylinderWidth1;
                            cylinderSpace = PrintProtocol.cylinderSpace1;
                            cylinderLeftMargin = (PrintProtocol.barImageWidht - (shapeGrade3.Length - 1) * (PrintProtocol.cylinderWidth1 + PrintProtocol.cylinderSpace1) - PrintProtocol.cylinderWidth1) / 2;
                            cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance1;
                        }
                        else
                        {
                            //cylinderWidth = PrintProtocol.cylinderWidth2;
                            //cylinderSpace = PrintProtocol.cylinderSpace2;
                            cylinderWidth = (PrintProtocol.barImageWidht - (shapeGrade3.Length - 1) * 5) / shapeGrade3.Length;
                            cylinderSpace = 5;
                            cylinderLeftMargin = (PrintProtocol.barImageWidht - (shapeGrade3.Length - 1) * (cylinderWidth + cylinderSpace) - cylinderWidth) / 2;
                            cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance2;
                        }
                        for (int i = 0; i < shapeGrade3.Length; i++)
                        {
                            //柱形图下标字样
                            e.Graphics.DrawString(shapeGrade3[i],
                                new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush3,
                                cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin + PrintProtocol.cylinderOrTextNote);
                            if (shapeFruitNumber3[i] == 0)
                            {
                                //柱形图显示
                                e.Graphics.FillRectangle(myBarBrush3, cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight,
                                    cylinderWidth, PrintProtocol.cylinderMinHeight);
                            }
                            else
                            {
                                //柱形图显示
                                e.Graphics.FillRectangle(myBarBrush3, cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)shapeFruitNumber3[i] / uMaxValue3)),
                                    cylinderWidth, (int)(PrintProtocol.cylinderMaxHeight * ((double)(double)shapeFruitNumber3[i] / uMaxValue3)));
                                if ((int)(PrintProtocol.cylinderMaxHeight * ((double)(double)shapeFruitNumber3[i] / uMaxValue3)) > PrintProtocol.cylinderStandardCylinderHeigh)
                                {
                                    e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                        PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - cylinderDigitLeftDistance);
                                    e.Graphics.RotateTransform(-90f);
                                    e.Graphics.DrawString(shapeFruitNumber3[i].ToString(),
                                        new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush3, 0, 0);
                                    e.Graphics.ResetTransform();
                                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                                }
                                else
                                {
                                    e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                        PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)shapeFruitNumber3[i] / uMaxValue3)) - PrintProtocol.cylinderDigitBottomDistance);
                                    e.Graphics.RotateTransform(-90f);
                                    e.Graphics.DrawString(shapeFruitNumber3[i].ToString(),
                                        new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush3, 0, 0);
                                    e.Graphics.ResetTransform();
                                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                                }
                            }
                            if (uSumValue3 == 0)
                            {
                                //百分比
                                e.Graphics.DrawString("0.00%", new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush3,
                                    cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight - PrintProtocol.cylinderPrecentBottomDistance);
                            }
                            else
                            {
                                //百分比
                                e.Graphics.DrawString(((double)shapeFruitNumber3[i] / uSumValue3).ToString("0.00%"),
                                    new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush3,
                                    cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)shapeFruitNumber3[i] / uMaxValue3)) - PrintProtocol.cylinderPrecentBottomDistance);
                            }
                        }
                        e.Graphics.ResetTransform();
                        //try
                        //{
                        //    e.Graphics.DrawImage(bitM3, PrintProtocol.leftMargin, currentAvailableHeight, PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);

                        //}
                        //catch (Exception ee)//捕获异常
                        //{
                        //    MessageBox.Show(ee.Message);//弹出消息对话框
                        //}
                        #endregion

                        //条形图标题
                        #region
                        //currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + bitM3.Height;   //当前可用高度
                        currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + PrintProtocol.barImageHeight;
                        Font barTitleFont3 = new Font("楷体_GB2312", 18, FontStyle.Bold);
                        Brush barTitleBrush3 = Brushes.Black;
                        string barTitle3 = m_resourceManager.GetString("tabPage4.Text");
                        XMaxSize3 = TitG3.MeasureString(barTitle3, barTitleFont3);
                        e.Graphics.DrawString(//使用DrawString方法绘制条形图标题字符串
                            barTitle3, barTitleFont3, barTitleBrush3, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize3.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion

                        //打印页数
                        #region
                        currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                        Font currentPagesFont3 = new Font("宋体", 12, FontStyle.Regular);
                        Brush currentPagesBrush3 = Brushes.Black;
                        string currentPages3 = m_resourceManager.GetString("LblPrintPages.Text") + " 1";
                        XMaxSize3 = TitG3.MeasureString(currentPages3, currentPagesFont3);
                        e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                            currentPages3, currentPagesFont3, currentPagesBrush3, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize3.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion
                        #endregion
                        //MessageBox.Show("当前选中形状统计信息");
                        break;
                    case 4:
                        #region 打印数量统计信息
                        //打印时间
                        #region
                        currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                        Font dateTimeFont4 = new Font("宋体", 15, FontStyle.Regular);
                        Brush dateTimeBrush4 = Brushes.Black;
                        string nowDateTime4 = DateTime.Now.ToString(m_resourceManager.GetString("LblPrintDateTime.Text"));
                        PictureBox picB4 = new PictureBox();
                        Graphics TitG4 = picB4.CreateGraphics();
                        SizeF XMaxSize4 = TitG4.MeasureString(nowDateTime4, dateTimeFont4);
                        e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                            nowDateTime4, dateTimeFont4, dateTimeBrush4, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize4.Width, currentAvailableHeight);
                        #endregion

                        //打印LOGO
                        #region
                        currentAvailableHeight += (int)XMaxSize4.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
                        try
                        {
                            Bitmap bitmap = new Bitmap(PrintProtocol.logoPathName);//创建位图对象        
                            e.Graphics.DrawImage(bitmap,
                                (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - bitmap.Width) / 2 + PrintProtocol.leftMargin, currentAvailableHeight, bitmap.Width, bitmap.Height);
                            currentAvailableHeight += bitmap.Height + PrintProtocol.logoOrTextTitleSpace;    //当前可用高度
                        }
                        catch (Exception ee)//捕获异常
                        {
                            MessageBox.Show(ee.Message);//弹出消息对话框
                        }
                        #endregion

                        //文本标题
                        #region
                        Font textTitleFont4 = new Font("宋体", 20, FontStyle.Bold);
                        Brush textTitleBrush4 = Brushes.Black;
                        string textTitle4 = m_resourceManager.GetString("LblPrintClassified.Text");
                        XMaxSize4 = TitG4.MeasureString(textTitle4, textTitleFont4);
                        e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                            textTitle4, textTitleFont4, textTitleBrush4, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize4.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion

                        //文本内容
                        #region
                        currentAvailableHeight += (int)XMaxSize4.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                        Font textContentFont4 = new Font("宋体", 15, FontStyle.Regular);
                        Brush textContentBrush4 = Brushes.Black;
                        string textContent4 = m_resourceManager.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName +
                            "    " + m_resourceManager.GetString("LblPrintFarmName.Text") + dataInterface.FarmName +
                            "    " + m_resourceManager.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
                        XMaxSize4 = TitG4.MeasureString(textContent4, textContentFont4);
                        e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                            textContent4, textContentFont4, textContentBrush4, PrintProtocol.leftMargin, currentAvailableHeight);
                        #endregion

                        //分割线
                        #region
                        currentAvailableHeight += (int)XMaxSize4.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
                        Pen linePen4 = new Pen(Color.Black, 2);
                        e.Graphics.DrawLine(linePen4, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                        #endregion

                        //条形图头部
                        #region
                        currentAvailableHeight += (int)linePen4.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                        Font barHeadFont4 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                        Brush barHeadBrush4 = Brushes.Black;
                        string barHead4 = m_resourceManager.GetString("LblPrintNumOrPercent.Text");
                        XMaxSize4 = TitG4.MeasureString(barHead4, barHeadFont4);
                        e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                            barHead4, barHeadFont4, barHeadBrush4, PrintProtocol.leftMargin, currentAvailableHeight);
                        #endregion

                        //直方图（或条形图）
                        #region
                        currentAvailableHeight += (int)XMaxSize4.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
                        //Bitmap bitM4 = new Bitmap(PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);
                        //Graphics g4 = Graphics.FromImage(bitM4);
                        //g4.Clear(PrintProtocol.barBackColor);
                        e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                        Brush myCurrentBrush4 = new SolidBrush(PrintProtocol.penColor);
                        Brush myBarBrush4 = new SolidBrush(PrintProtocol.barBrush);
                        //界面设定
                        if ((dataInterface.WeightOrSizeGradeSum - 1) * (PrintProtocol.cylinderWidth1 + PrintProtocol.cylinderSpace1) + PrintProtocol.cylinderWidth1 < PrintProtocol.barImageWidht)
                        {
                            cylinderWidth = PrintProtocol.cylinderWidth1;
                            cylinderSpace = PrintProtocol.cylinderSpace1;
                            cylinderLeftMargin = (PrintProtocol.barImageWidht - (dataInterface.WeightOrSizeGradeSum - 1) * (PrintProtocol.cylinderWidth1 + PrintProtocol.cylinderSpace1) - PrintProtocol.cylinderWidth1) / 2;
                            cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance1;
                        }
                        else
                        {
                            //cylinderWidth = PrintProtocol.cylinderWidth2;
                            //cylinderSpace = PrintProtocol.cylinderSpace2;
                            cylinderWidth = (PrintProtocol.barImageWidht - (dataInterface.WeightOrSizeGradeSum - 1) * 5) / dataInterface.WeightOrSizeGradeSum;
                            cylinderSpace = 5;
                            cylinderLeftMargin = (PrintProtocol.barImageWidht - (dataInterface.WeightOrSizeGradeSum - 1) * (cylinderWidth + cylinderSpace) - cylinderWidth) / 2;
                            cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance2;
                        }
                        UInt64 uMaxValue4 = 0;  //尺寸最大值
                        UInt64 uSumValue4 = 0;  //尺寸总值
                        for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
                        {
                            UInt64 temp = dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i];
                            if (uMaxValue4 < temp)
                            {
                                uMaxValue4 = temp;
                            }
                            uSumValue4 += temp;
                        }
                        for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
                        {
                            //柱形图下标字样
                            e.Graphics.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH),
                                new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush4,
                                cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin + PrintProtocol.cylinderOrTextNote);
                            if (dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] == 0)
                            {
                                //柱形图显示
                                e.Graphics.FillRectangle(myBarBrush4, cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight,
                                    cylinderWidth, PrintProtocol.cylinderMinHeight);
                            }
                            else
                            {
                                //柱形图显示
                                e.Graphics.FillRectangle(myBarBrush4, cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / uMaxValue4)),
                                    cylinderWidth, (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / uMaxValue4)));
                                if ((int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / uMaxValue4)) > PrintProtocol.cylinderStandardCylinderHeigh)
                                {
                                    e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                        PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - cylinderDigitLeftDistance);
                                    e.Graphics.RotateTransform(-90f);
                                    e.Graphics.DrawString(dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].ToString(),
                                        new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush4, 0, 0);
                                    e.Graphics.ResetTransform();
                                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                                }
                                else
                                {
                                    e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                        PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / uMaxValue4)) - PrintProtocol.cylinderDigitBottomDistance);
                                    e.Graphics.RotateTransform(-90f);
                                    e.Graphics.DrawString(dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].ToString(),
                                        new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush4, 0, 0);
                                    e.Graphics.ResetTransform();
                                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                                }
                            }
                            if (uSumValue4 == 0)
                            {
                                //百分比
                                e.Graphics.DrawString("0.00%", new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush4,
                                    cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight - PrintProtocol.cylinderPrecentBottomDistance);
                            }
                            else
                            {
                                //百分比
                                e.Graphics.DrawString(((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / uSumValue4).ToString("0.00%"),
                                    new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush4,
                                    cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nWeightGradeCount[CboSizeType.SelectedIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + i] / uMaxValue4)) - PrintProtocol.cylinderPrecentBottomDistance);
                            }
                        }
                        e.Graphics.ResetTransform();
                        //try
                        //{
                        //    e.Graphics.DrawImage(bitM4, PrintProtocol.leftMargin, currentAvailableHeight, PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);

                        //}
                        //catch (Exception ee)//捕获异常
                        //{
                        //    MessageBox.Show(ee.Message);//弹出消息对话框
                        //}
                        #endregion

                        //条形图标题
                        #region
                        //currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + bitM4.Height;   //当前可用高度
                        currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + PrintProtocol.barImageHeight;
                        Font barTitleFont4 = new Font("楷体_GB2312", 18, FontStyle.Bold);
                        Brush barTitleBrush4 = Brushes.Black;
                        string barTitle4 = m_resourceManager.GetString("LblQualityDimensionLevel.Text");
                        XMaxSize4 = TitG4.MeasureString(barTitle4, barTitleFont4);
                        e.Graphics.DrawString(//使用DrawString方法绘制条形图标题字符串
                            barTitle4, barTitleFont4, barTitleBrush4, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize4.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion

                        //打印页数
                        #region
                        currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                        Font currentPagesFont4 = new Font("宋体", 12, FontStyle.Regular);
                        Brush currentPagesBrush4 = Brushes.Black;
                        string currentPages4 = m_resourceManager.GetString("LblPrintPages.Text") + " 1";
                        XMaxSize4 = TitG4.MeasureString(currentPages4, currentPagesFont4);
                        e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                            currentPages4, currentPagesFont4, currentPagesBrush4, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize4.Width) / 2 + PrintProtocol.leftMargin,
                            currentAvailableHeight);
                        #endregion
                        #endregion
                        //MessageBox.Show("当前选中数量统计信息");
                        break;
                        #region 取消箱数统计信息
                        //case 5:
                        //    #region 打印箱数统计信息
                        //    //打印时间
                        //    #region
                        //    currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                        //    Font dateTimeFont5 = new Font("宋体", 15, FontStyle.Regular);
                        //    Brush dateTimeBrush5 = Brushes.Black;
                        //    string nowDateTime5 = DateTime.Now.ToString(m_resourceManager.GetString("LblPrintDateTime.Text"));
                        //    PictureBox picB5 = new PictureBox();
                        //    Graphics TitG5 = picB5.CreateGraphics();
                        //    SizeF XMaxSize5 = TitG5.MeasureString(nowDateTime5, dateTimeFont5);
                        //    e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                        //        nowDateTime5, dateTimeFont5, dateTimeBrush5, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize5.Width, currentAvailableHeight);
                        //    #endregion

                        //    //打印LOGO
                        //    #region
                        //    currentAvailableHeight += (int)XMaxSize5.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
                        //    try
                        //    {
                        //        Bitmap bitmap = new Bitmap(PrintProtocol.logoPathName);//创建位图对象        
                        //        e.Graphics.DrawImage(bitmap,
                        //            (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - bitmap.Width) / 2 + PrintProtocol.leftMargin, currentAvailableHeight, bitmap.Width, bitmap.Height);
                        //        currentAvailableHeight += bitmap.Height + PrintProtocol.logoOrTextTitleSpace;    //当前可用高度
                        //    }
                        //    catch (Exception ee)//捕获异常
                        //    {
                        //        MessageBox.Show(ee.Message);//弹出消息对话框
                        //    }
                        //    #endregion

                        //    //文本标题
                        //    #region
                        //    Font textTitleFont5 = new Font("宋体", 20, FontStyle.Bold);
                        //    Brush textTitleBrush5 = Brushes.Black;
                        //    string textTitle5 = m_resourceManager.GetString("LblPrintClassified.Text");
                        //    XMaxSize5 = TitG5.MeasureString(textTitle5, textTitleFont5);
                        //    e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                        //        textTitle5, textTitleFont5, textTitleBrush5, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize5.Width) / 2 + PrintProtocol.leftMargin,
                        //        currentAvailableHeight);
                        //    #endregion

                        //    //文本内容
                        //    #region
                        //    currentAvailableHeight += (int)XMaxSize5.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                        //    Font textContentFont5 = new Font("宋体", 15, FontStyle.Regular);
                        //    Brush textContentBrush5 = Brushes.Black;
                        //    string textContent5 = m_resourceManager.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName +
                        //        "    " + m_resourceManager.GetString("LblPrintFarmName.Text") + dataInterface.FarmName +
                        //        "    " + m_resourceManager.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
                        //    XMaxSize5 = TitG5.MeasureString(textContent5, textContentFont5);
                        //    e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                        //        textContent5, textContentFont5, textContentBrush5, PrintProtocol.leftMargin, currentAvailableHeight);
                        //    #endregion

                        //    //分割线
                        //    #region
                        //    currentAvailableHeight += (int)XMaxSize5.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
                        //    Pen linePen5 = new Pen(Color.Black, 2);
                        //    e.Graphics.DrawLine(linePen5, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                        //    #endregion

                        //    //条形图头部
                        //    #region
                        //    currentAvailableHeight += (int)linePen5.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                        //    Font barHeadFont5 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                        //    Brush barHeadBrush5 = Brushes.Black;
                        //    string barHead5 = m_resourceManager.GetString("LblPrintNumOrPercent.Text");
                        //    XMaxSize5 = TitG5.MeasureString(barHead5, barHeadFont5);
                        //    e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                        //        barHead5, barHeadFont5, barHeadBrush5, PrintProtocol.leftMargin, currentAvailableHeight);
                        //    #endregion

                        //    //直方图（或条形图）
                        //    #region
                        //    currentAvailableHeight += (int)XMaxSize5.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
                        //    //Bitmap bitM5 = new Bitmap(PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);
                        //    //Graphics g5 = Graphics.FromImage(bitM5);
                        //    //g5.Clear(PrintProtocol.barBackColor);
                        //    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                        //    Brush myCurrentBrush5 = new SolidBrush(PrintProtocol.penColor);
                        //    Brush myBarBrush5 = new SolidBrush(PrintProtocol.barBrush);
                        //    //界面设定
                        //    if ((dataInterface.QualityGradeSum * dataInterface.WeightOrSizeGradeSum - 1) * (PrintProtocol.cylinderWidth1 + PrintProtocol.cylinderSpace1) + PrintProtocol.cylinderWidth1 < PrintProtocol.barImageWidht)
                        //    {
                        //        cylinderWidth = PrintProtocol.cylinderWidth1;
                        //        cylinderSpace = PrintProtocol.cylinderSpace1;
                        //        cylinderLeftMargin = (PrintProtocol.barImageWidht - (dataInterface.QualityGradeSum * dataInterface.WeightOrSizeGradeSum - 1) * (PrintProtocol.cylinderWidth1 + PrintProtocol.cylinderSpace1) - PrintProtocol.cylinderWidth1) / 2;
                        //        cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance1;
                        //    }
                        //    else
                        //    {
                        //        //cylinderWidth = PrintProtocol.cylinderWidth2;
                        //        //cylinderSpace = PrintProtocol.cylinderSpace2;
                        //        cylinderWidth = (PrintProtocol.barImageWidht - (dataInterface.QualityGradeSum * dataInterface.WeightOrSizeGradeSum - 1) * 5) / dataInterface.QualityGradeSum * dataInterface.WeightOrSizeGradeSum;
                        //        cylinderSpace = 5;
                        //        cylinderLeftMargin = (PrintProtocol.barImageWidht - (dataInterface.QualityGradeSum * dataInterface.WeightOrSizeGradeSum - 1) * (cylinderWidth + cylinderSpace) - cylinderWidth) / 2;
                        //        cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance2;
                        //    }
                        //    Int32 maxValue5 = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nBoxGradeCount); //箱数最大值
                        //    Int32 sumValue5 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount); //箱数总值
                        //    string strSizeName5;     //尺寸名称
                        //    string strQualityName5;   //品质名称
                        //    string strMixName5;       //总名称：尺寸名称.品质名称
                        //    for (int i = 0; i < dataInterface.QualityGradeSum; i++)
                        //    {
                        //        for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                        //        {
                        //            //柱形图下标字样
                        //            strSizeName5 = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                        //            strQualityName5 = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                        //            strMixName5 = strSizeName5.Substring(0, strSizeName5.IndexOf("\0")) + "." + strQualityName5.Substring(0, strQualityName5.IndexOf("\0"));
                        //            e.Graphics.DrawString(strMixName5,
                        //                new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush5,
                        //                cylinderLeftMargin + (i * dataInterface.WeightOrSizeGradeSum + j) * (cylinderWidth + cylinderSpace),
                        //                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin + PrintProtocol.cylinderOrTextNote);
                        //            if (dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] == 0)
                        //            {
                        //                //柱形图显示
                        //                e.Graphics.FillRectangle(myBarBrush5, cylinderLeftMargin + (i * dataInterface.WeightOrSizeGradeSum + j) * (cylinderWidth + cylinderSpace),
                        //                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight,
                        //                    cylinderWidth, PrintProtocol.cylinderMinHeight);
                        //            }
                        //            else
                        //            {
                        //                //柱形图显示
                        //                e.Graphics.FillRectangle(myBarBrush5, cylinderLeftMargin + (i * dataInterface.WeightOrSizeGradeSum + j) * (cylinderWidth + cylinderSpace),
                        //                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / maxValue5)),
                        //                    cylinderWidth, (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / maxValue5)));
                        //                if ((int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / maxValue5)) > PrintProtocol.cylinderStandardCylinderHeigh)
                        //                {
                        //                    e.Graphics.TranslateTransform(cylinderLeftMargin + (i * dataInterface.WeightOrSizeGradeSum + j) * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                        //                        PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - cylinderDigitLeftDistance);
                        //                    e.Graphics.RotateTransform(-90f);
                        //                    e.Graphics.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString(),
                        //                        new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush5, 0, 0);
                        //                    e.Graphics.ResetTransform();
                        //                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                        //                }
                        //                else
                        //                {
                        //                    e.Graphics.TranslateTransform(cylinderLeftMargin + (i * dataInterface.WeightOrSizeGradeSum + j) * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                        //                        PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / maxValue5)) - PrintProtocol.cylinderDigitBottomDistance);
                        //                    e.Graphics.RotateTransform(-90f);
                        //                    e.Graphics.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString(),
                        //                        new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush5, 0, 0);
                        //                    e.Graphics.ResetTransform();
                        //                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                        //                }
                        //            }
                        //            if (sumValue5 == 0)
                        //            {
                        //                //百分比
                        //                e.Graphics.DrawString("0.00%", new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush5,
                        //                    cylinderLeftMargin + (i * dataInterface.WeightOrSizeGradeSum + j) * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                        //                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight - PrintProtocol.cylinderPrecentBottomDistance);
                        //            }
                        //            else
                        //            {
                        //                //百分比
                        //                e.Graphics.DrawString(((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / sumValue5).ToString("0.00%"),
                        //                    new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush5,
                        //                    cylinderLeftMargin + (i * dataInterface.WeightOrSizeGradeSum + j) * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                        //                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / maxValue5)) - PrintProtocol.cylinderPrecentBottomDistance);
                        //            }
                        //        }
                        //    }
                        //    e.Graphics.ResetTransform();
                        //    //try
                        //    //{
                        //    //    e.Graphics.DrawImage(bitM5, PrintProtocol.leftMargin, currentAvailableHeight, PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);

                        //    //}
                        //    //catch (Exception ee)//捕获异常
                        //    //{
                        //    //    MessageBox.Show(ee.Message);//弹出消息对话框
                        //    //}
                        //    #endregion

                        //    //条形图标题
                        //    #region
                        //    //currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + bitM5.Height;   //当前可用高度
                        //    currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + PrintProtocol.barImageHeight;
                        //    Font barTitleFont5 = new Font("楷体_GB2312", 18, FontStyle.Bold);
                        //    Brush barTitleBrush5 = Brushes.Black;
                        //    string barTitle5 = m_resourceManager.GetString("tabPage6.Text");
                        //    XMaxSize5 = TitG5.MeasureString(barTitle5, barTitleFont5);
                        //    e.Graphics.DrawString(//使用DrawString方法绘制条形图标题字符串
                        //        barTitle5, barTitleFont5, barTitleBrush5, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize5.Width) / 2 + PrintProtocol.leftMargin,
                        //        currentAvailableHeight);
                        //    #endregion

                        //    //打印页数
                        //    #region
                        //    currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                        //    Font currentPagesFont5 = new Font("宋体", 12, FontStyle.Regular);
                        //    Brush currentPagesBrush5 = Brushes.Black;
                        //    string currentPages5 = m_resourceManager.GetString("LblPrintPages.Text") + " 1";
                        //    XMaxSize5 = TitG5.MeasureString(currentPages5, currentPagesFont5);
                        //    e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                        //        currentPages5, currentPagesFont5, currentPagesBrush5, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize5.Width) / 2 + PrintProtocol.leftMargin,
                        //        currentAvailableHeight);
                        //    #endregion
                        //    #endregion
                        //    //MessageBox.Show("当前选中箱数统计信息");
                        break;
                    #endregion
                    case 5: //add by xcw 20201103 
                        #region 打印      尺寸统计表
                        //if (bIsHaveColorStatis)
                        //{
                        UInt64 SumValue60 = 0; //总个数
                        UInt64 SumWeightValue60 = 0; //总重量
                        Int32 SumBoxValue60 = 0;  //总箱数   
                        if (currentPageIndex == 1)            //第一页
                        {
                            #region 打印第一页
                            //打印时间
                            #region
                            //currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                            currentAvailableHeight = 20;   //50->20 Modify by ChengSk - 20171124
                            Font dateTimeFont60 = new Font("宋体", 15, FontStyle.Regular);
                            Brush dateTimeBrush60 = Brushes.Black;
                            string nowDateTime60 = DateTime.Now.ToString(m_resourceManager.GetString("LblPrintDateTime.Text"));
                            PictureBox picB60 = new PictureBox();
                            Graphics TitG60 = picB60.CreateGraphics();
                            SizeF XMaxSize60 = TitG60.MeasureString(nowDateTime60, dateTimeFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                                nowDateTime60, dateTimeFont60, dateTimeBrush60, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize60.Width, currentAvailableHeight);
                            #endregion

                            //打印LOGO
                            #region
                            //currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
                            currentAvailableHeight += (int)XMaxSize60.Height + 10;  //20->10 Modify by ChengSk - 20171124
                            try
                            {
                                Bitmap bitmap = new Bitmap(PrintProtocol.logoPathName);//创建位图对象        
                                e.Graphics.DrawImage(bitmap,
                                    (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - bitmap.Width) / 2 + PrintProtocol.leftMargin, currentAvailableHeight, bitmap.Width, bitmap.Height);
                                //currentAvailableHeight += bitmap.Height + PrintProtocol.logoOrTextTitleSpace;    //当前可用高度
                                currentAvailableHeight += bitmap.Height + 15;       //30->15 Modify by ChengSk - 20171124
                            }
                            catch (Exception ee)//捕获异常
                            {
                                MessageBox.Show(ee.Message);//弹出消息对话框
                            }
                            #endregion

                            //文本标题
                            #region
                            Font textTitleFont60 = new Font("宋体", 20, FontStyle.Bold);
                            Brush textTitleBrush60 = Brushes.Black;
                            string textTitle60 = m_resourceManager.GetString("LblPrintBatchReport.Text");
                            XMaxSize60 = TitG60.MeasureString(textTitle60, textTitleFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                                textTitle60, textTitleFont60, textTitleBrush60, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin,
                                currentAvailableHeight);
                            #endregion

                            //分割线1
                            #region
                            //currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                            currentAvailableHeight += (int)XMaxSize60.Height + 10;   //20->10 Modify by ChengSk - 20171124
                            Pen linePen61 = new Pen(Color.Black, 2);
                            e.Graphics.DrawLine(linePen61, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                            #endregion

                            //分割线2
                            #region
                            currentAvailableHeight += (int)linePen61.Width + PrintProtocol.lineOrLineSpace; //当前可用高度
                            Pen linePen62 = new Pen(Color.Black, 2);
                            e.Graphics.DrawLine(linePen62, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                            #endregion

                            //文本内容
                            #region
                            //currentAvailableHeight += (int)linePen62.Width + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                            currentAvailableHeight += (int)linePen62.Width + 10;     //20->10 Modify by ChengSk - 20171124
                            Font textContentFont60 = new Font("宋体", 15, FontStyle.Regular);
                            Brush textContentBrush60 = Brushes.Black;
                            int sumBoxNum = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);  //总箱数
                            string textContent31 = m_resourceManager.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName;
                            XMaxSize60 = TitG60.MeasureString(textContent31, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent31, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                            string textContent32 = m_resourceManager.GetString("LblPrintFarmName.Text") + dataInterface.FarmName;
                            XMaxSize60 = TitG60.MeasureString(textContent32, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent32, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                            string textContent33 = m_resourceManager.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
                            XMaxSize60 = TitG60.MeasureString(textContent33, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent33, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                            currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
                            string textContent34 = m_resourceManager.GetString("LblPrintTotalPieces.Text") + dataInterface.IoStStatistics.nTotalCount;
                            XMaxSize60 = TitG60.MeasureString(textContent34, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent34, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                            string textContent35 = m_resourceManager.GetString("LblPrintTotalWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / 1000000).ToString() +
                                " " + m_resourceManager.GetString("LblPrintTName.Text");
                            XMaxSize60 = TitG60.MeasureString(textContent35, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent35, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                            string textContent36 = m_resourceManager.GetString("LblPrintTotalCartons.Text") + sumBoxNum;
                            XMaxSize60 = TitG60.MeasureString(textContent36, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent36, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                            currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
                            string textContent37 = "";
                            if (dataInterface.IoStStatistics.nTotalCount == 0)
                            {
                                textContent37 = m_resourceManager.GetString("LblPrintAveFruitWeight.Text") + "0.000 " + m_resourceManager.GetString("LblPrintGName.Text");
                            }
                            else
                            {
                                textContent37 = m_resourceManager.GetString("LblPrintAveFruitWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / dataInterface.IoStStatistics.nTotalCount).ToString("0.000") +
                                    " " + m_resourceManager.GetString("LblPrintGName.Text");
                            }
                            XMaxSize60 = TitG60.MeasureString(textContent37, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent37, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                            string textContent38 = m_resourceManager.GetString("LblPrintProgramName.Text") + dataInterface.ProgramName;
                            XMaxSize60 = TitG60.MeasureString(textContent38, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                               textContent38, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2, currentAvailableHeight);
                            string textContent41 = m_resourceManager.GetString("LblCustomerID.Text") + GlobalDataInterface.SerialNum;
                            XMaxSize60 = TitG60.MeasureString(textContent41, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                               textContent41, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2 * 2, currentAvailableHeight);//add by xcw 20200701

                            currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度 2015-12-7 ivycc
                            string textContent39 = m_resourceManager.GetString("LblExcelStartTime.Text") + dataInterface.StartTime;
                            XMaxSize60 = TitG60.MeasureString(textContent39, textContentFont60);
                            //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                            //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                               textContent39, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                            string textContent40 = m_resourceManager.GetString("LblExcelEndTime.Text") + dataInterface.EndTime;
                            XMaxSize60 = TitG60.MeasureString(textContent40, textContentFont60);
                            //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                            //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                               textContent40, textContentFont60, textContentBrush60, e.PageBounds.Width / 2/*PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2*/, currentAvailableHeight);
                            #endregion

                            //分割线3
                            #region
                            //currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
                            currentAvailableHeight += (int)XMaxSize60.Height + 10;   //25->10 Modify by ChengSk - 20171124
                            Pen linePen63 = new Pen(Color.Black, 2);
                            e.Graphics.DrawLine(linePen63, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                            #endregion

                            //分割线4
                            #region
                            currentAvailableHeight += (int)linePen63.Width + PrintProtocol.lineOrLineSpace; //当前可用高度
                            Pen linePen64 = new Pen(Color.Black, 2);
                            e.Graphics.DrawLine(linePen64, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                            #endregion
                            //汇总表格 已修改
                            #region
                            bool Isbreak = false;
                            //currentAvailableHeight += (int)linePen64.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                            currentAvailableHeight += (int)linePen64.Width + 10;     //25->10 Modify by ChengSk - 20171124
                            //currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
                            //sumTableHeight = (dataInterface.WeightOrSizeGradeSum <= 10 ? PrintProtocol.sumTableHeight1 : PrintProtocol.sumTableHeight2); //汇总表格高度
                            sumTableHeight = PrintProtocol.sumTableHeight3;
                            sumTalbeWidth = (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin) / 8;
                            Pen linePen60 = new Pen(Color.Black, 1);
                            Font sumTableFont60;
                            if (GlobalDataInterface.selectLanguage == "en")
                                sumTableFont60 = new Font("Times New Roman", 9, FontStyle.Regular);
                            else
                                //sumTableFont60 = (dataInterface.WeightOrSizeGradeSum <= 10 ? (new Font("宋体", 12, FontStyle.Regular)) : (new Font("宋体", 8, FontStyle.Regular)));
                                sumTableFont60 = new Font("宋体", 11, FontStyle.Regular);
                            Brush sumTableBrush60 = Brushes.Black;
                            int gradenum = dataInterface.WeightOrSizeGradeSum ;
                            if (gradenum > PrintProtocol.GradeNum1)
                            {
                                gradenum = PrintProtocol.GradeNum1 - 1;
                            }
                            //if(dataInterface.WeightOrSizeGradeSum*dataInterface.QualityGradeSum)
                            for (int i = 0; i < gradenum + 2; i++) //画横线
                            {
                                e.Graphics.DrawLine(linePen60, PrintProtocol.leftMargin, currentAvailableHeight + i * sumTableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + i * sumTableHeight);
                            }
                            e.Graphics.DrawLine(linePen60, PrintProtocol.leftMargin, currentAvailableHeight + (gradenum + 2) * sumTableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 2) * sumTableHeight);
                            for (int i = 0; i < 8; i++) //画竖线
                            {
                                e.Graphics.DrawLine(linePen60, PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight,
                                    PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight + (gradenum + 2) * sumTableHeight);
                            }
                            e.Graphics.DrawLine(linePen60, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 2) * sumTableHeight);
                            //表格标题行
                            string tablesFirstItems0 = m_resourceManager.GetString("LblMainReportName.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems0, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems0, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems1 = m_resourceManager.GetString("LblMainReportSize.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems1, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems1, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems2 = m_resourceManager.GetString("LblMainReportPieces.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems2, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems2, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems3 = m_resourceManager.GetString("LblMainReportPiecesPer.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems3, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems3, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems4 = m_resourceManager.GetString("LblMainReportWeights.Text") + "(kg)";
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems4, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems4, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems5 = m_resourceManager.GetString("LblMainReportWeightPer.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems5, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems5, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems6 = m_resourceManager.GetString("LblMainReportCartons.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems6, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems6, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems7 = m_resourceManager.GetString("LblMainReportCartonsPer.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems7, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems7, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            //UInt64 uMaxValue60 = 0; //最大个数

                            //for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
                            //{
                            //if (uMaxValue60 < dataInterface.IoStStatistics.nWeightGradeCount[(currentPageIndex - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + i])
                            //{
                            //    uMaxValue60 = dataInterface.IoStStatistics.nWeightGradeCount[(currentPageIndex - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + i];
                            //}
                            //uSumValue60 += dataInterface.IoStStatistics.nGradeCount[(currentPageIndex - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + i];
                            //uSumWeightValue60 += dataInterface.IoStStatistics.nWeightGradeCount[(currentPageIndex - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + i];
                            //uSumBoxValue60 += dataInterface.IoStStatistics.nBoxGradeCount[(currentPageIndex - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + i];
                            SumValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);//获取总个数
                            SumWeightValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);//获取总重量
                            SumBoxValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);//获取总箱数
                            
                            for (int i = 1; i < dataInterface.WeightOrSizeGradeSum + 1; i++)//打印表格数据 add by xcw 20201103
                            {
                                ulong tablesMiddleItems2 = 0;
                                double tablesMiddleItems3 = 0.0;
                                double tablesMiddleItems4 = 0.0;
                                double tablesMiddleItems5 = 0.0;
                                int tablesMiddleItems6 = 0;
                                double tablesMiddleItems7 = 0.0;
                                for (int j = 1; j < dataInterface.QualityGradeSum + 1; j++)
                                {
                                    tablesMiddleItems2 += dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)];

                                    if (SumValue60 != 0)
                                    {
                                        tablesMiddleItems3 += ((double)dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / SumValue60);
                                    }
                                    tablesMiddleItems4 += (dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / 1000.0);
                                    if (SumWeightValue60 != 0)
                                    {
                                        tablesMiddleItems5 += ((double)dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / SumWeightValue60);
                                    }
                                    tablesMiddleItems6 += dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)];
                                    if (SumBoxValue60 != 0)
                                    {
                                        tablesMiddleItems7 += ((double)dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / SumBoxValue60);
                                    }
                                    if (((i - 1)+ j) == PrintProtocol.GradeNum1)
                                    {
                                        Isbreak = true;
                                        break;
                                    }
                                }
                                string strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                                string tablesMiddleItems0 = strSizeName.Substring(0, strSizeName.IndexOf("\0"));
                                XMaxSize60 = TitG60.MeasureString(tablesMiddleItems0, sumTableFont60);
                                e.Graphics.DrawString(tablesMiddleItems0, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesMiddleItems1 = dataInterface.IoStStGradeInfo.grades[(i - 1)].nMinSize.ToString();
                                XMaxSize60 = TitG60.MeasureString(tablesMiddleItems1, sumTableFont60);
                                e.Graphics.DrawString(tablesMiddleItems1, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize60 = TitG60.MeasureString(tablesMiddleItems2.ToString(), sumTableFont60);
                                e.Graphics.DrawString(tablesMiddleItems2.ToString(), sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize60 = TitG60.MeasureString(tablesMiddleItems3.ToString("0.000%"), sumTableFont60);
                                e.Graphics.DrawString(tablesMiddleItems3.ToString("0.000%"), sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize60 = TitG60.MeasureString(tablesMiddleItems4.ToString("0.0"), sumTableFont60);
                                e.Graphics.DrawString(tablesMiddleItems4.ToString("0.0"), sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize60 = TitG60.MeasureString(tablesMiddleItems5.ToString("0.000%"), sumTableFont60);
                                e.Graphics.DrawString(tablesMiddleItems5.ToString("0.000%"), sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize60 = TitG60.MeasureString(tablesMiddleItems6.ToString(), sumTableFont60);
                                e.Graphics.DrawString(tablesMiddleItems6.ToString(), sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);

                                XMaxSize60 = TitG60.MeasureString(tablesMiddleItems7.ToString("0.000%"), sumTableFont60);
                                e.Graphics.DrawString(tablesMiddleItems7.ToString("0.000%"), sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                if (Isbreak)
                                    break;
                            }

                            //表格最后一行
                            if (intPage1 == 1)
                            {
                                string tablesLastItems0 = m_resourceManager.GetString("LblPrintSubTotal.Text");
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems0, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems0, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum  + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems1 = "";
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems1, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems1, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems2 = SumValue60.ToString();
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems2, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems2, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems3 = (SumValue60 == 0 ? "0.000%" : "100.000%");
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems3, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems3, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems4 = (SumWeightValue60 / 1000.0).ToString("0.0");
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems4, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems4, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems5 = (SumWeightValue60 == 0 ? "0.000%" : "100.000%");
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems5, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems5, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum  + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems6 = SumBoxValue60.ToString();
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems6, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems6, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems7 = (SumBoxValue60 == 0 ? "0.000%" : "100.000%");
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems7, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems7, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum /** dataInterface.QualityGradeSum*/ + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            }
                            #endregion
                            //打印页数
                            #region
                            currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                            Font currentPagesFont60 = new Font("宋体", 12, FontStyle.Regular);
                            Brush currentPagesBrush60 = Brushes.Black;
                            string currentPages60 = m_resourceManager.GetString("LblPrintPages.Text") + " " + currentPageIndex.ToString() + "/" + intPage.ToString();
                            XMaxSize60 = TitG60.MeasureString(currentPages60, currentPagesFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                                currentPages60, currentPagesFont60, currentPagesBrush60, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin,
                                currentAvailableHeight);
                            #endregion

                            //是否打印副页
                            #region
                            currentPageIndex++; //下一页页码
                            if (currentPageIndex <= intPage1)  //如果当前页不是最后一页
                            {
                                e.HasMorePages = true;  //打印副页
                            }
                            else
                            {
                                e.HasMorePages = false;  //不打印副页
                                currentPageIndex = 1;
                            }
                            #endregion
                            #endregion
                        }
                        else if (currentPageIndex == intPage1) //最后一页
                        {
                            #region 打印最后一页
                            ////汇总图头部
                            //#region
                            //currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                            //Font sumImageHeadFont65 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                            //Brush sumImagerHeadBrush65 = Brushes.Black;
                            //string tempQualityName65 = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, (dataInterface.QualityGradeSum - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH).ToString();
                            //string sumImageHead65 = m_resourceManager.GetString("LblPrintQualityName.Text") + tempQualityName65.Substring(0, tempQualityName65.IndexOf("\0"));
                            //PictureBox picB65 = new PictureBox();
                            //Graphics TitG65 = picB65.CreateGraphics();
                            //SizeF XMaxSize65 = TitG65.MeasureString(sumImageHead65, sumImageHeadFont65);
                            //e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                            //    sumImageHead65, sumImageHeadFont65, sumImagerHeadBrush65, PrintProtocol.leftMargin, currentAvailableHeight);
                            //#endregion

                            //汇总表格 已修改
                            #region
                            currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                            //currentAvailableHeight += (int)XMaxSize65.Height + PrintProtocol.barHeadOrBarImageSpace1;  //当前可用高度
                            sumTableHeight = PrintProtocol.sumTableHeight3; //汇总表格高度
                            sumTalbeWidth = (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin) / 8;
                            Pen linePen65 = new Pen(Color.Black, 1);
                            Font sumTableFont65;
                            PictureBox picB65 = new PictureBox();
                            Graphics TitG65 = picB65.CreateGraphics();
                            SizeF XMaxSize65;
                            if (GlobalDataInterface.selectLanguage == "en")
                                sumTableFont65 = new Font("Times New Roman", 9, FontStyle.Regular);
                            else
                                sumTableFont65 = new Font("宋体", 11, FontStyle.Regular);
                            Brush sumTableBrush65 = Brushes.Black;
                            int gradenum = dataInterface.WeightOrSizeGradeSum  - (PrintProtocol.GradeNum1 + PrintProtocol.GradeNum2 * (currentPageIndex - 2));
                            for (int i = 0; i < gradenum + 2; i++) //画横线
                            {
                                e.Graphics.DrawLine(linePen65, PrintProtocol.leftMargin, currentAvailableHeight + i * sumTableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + i * sumTableHeight);
                            }
                            e.Graphics.DrawLine(linePen65, PrintProtocol.leftMargin, currentAvailableHeight + (gradenum + 2) * sumTableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 2) * sumTableHeight);
                            for (int i = 0; i < 8; i++) //画竖线
                            {
                                e.Graphics.DrawLine(linePen65, PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight,
                                    PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight + (gradenum + 2) * sumTableHeight);
                            }
                            e.Graphics.DrawLine(linePen65, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 2) * sumTableHeight);
                            //表格标题行
                            string tablesFirstItems0 = m_resourceManager.GetString("LblMainReportName.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems0, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems0, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems1 = m_resourceManager.GetString("LblMainReportSize.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems1, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems1, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems2 = m_resourceManager.GetString("LblMainReportPieces.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems2, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems2, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems3 = m_resourceManager.GetString("LblMainReportPiecesPer.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems3, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems3, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems4 = m_resourceManager.GetString("LblMainReportWeights.Text") + "(kg)";
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems4, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems4, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems5 = m_resourceManager.GetString("LblMainReportWeightPer.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems5, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems5, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems6 = m_resourceManager.GetString("LblMainReportCartons.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems6, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems6, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems7 = m_resourceManager.GetString("LblMainReportCartonsPer.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems7, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems7, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            int index = PrintProtocol.GradeNum1 + PrintProtocol.GradeNum2 * (currentPageIndex - 2) + 1;
                            

                            SumValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);//获取总个数  //Add by ChengSk - 20180422
                            SumWeightValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);//获取总重量  //Add by ChengSk - 20180422
                            SumBoxValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);//获取总箱数  //Add by ChengSk - 20180422
                            for (int i = 1; i < dataInterface.WeightOrSizeGradeSum + 1; i++)
                            {
                                ulong tablesMiddleItems2 = 0;
                                double tablesMiddleItems3 = 0.0;
                                double tablesMiddleItems4 = 0.0;
                                double tablesMiddleItems5 = 0.0;
                                int tablesMiddleItems6 = 0;
                                double tablesMiddleItems7 = 0.0;
                                for (int j = index / dataInterface.WeightOrSizeGradeSum; j < dataInterface.QualityGradeSum + 1; j++)
                                {
                                    tablesMiddleItems2 += dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)];

                                    if (SumValue60 != 0)
                                    {
                                        tablesMiddleItems3 += ((double)dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / SumValue60);
                                    }
                                    tablesMiddleItems4 += (dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / 1000.0);
                                    if (SumWeightValue60 != 0)
                                    {
                                        tablesMiddleItems5 += ((double)dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / SumWeightValue60);
                                    }
                                    tablesMiddleItems6 += dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)];
                                    if (SumBoxValue60 != 0)
                                    {
                                        tablesMiddleItems7 += ((double)dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / SumBoxValue60);
                                    }

                                }
                                string strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                                string tablesMiddleItems0 = strSizeName.Substring(0, strSizeName.IndexOf("\0"));
                                XMaxSize65 = TitG65.MeasureString(tablesMiddleItems0, sumTableFont65);
                                e.Graphics.DrawString(tablesMiddleItems0, sumTableFont65, sumTableBrush65,
                                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                string tablesMiddleItems1 = dataInterface.IoStStGradeInfo.grades[(i - 1)].nMinSize.ToString();
                                XMaxSize65 = TitG65.MeasureString(tablesMiddleItems1, sumTableFont65);
                                e.Graphics.DrawString(tablesMiddleItems1, sumTableFont65, sumTableBrush65,
                                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize65 = TitG65.MeasureString(tablesMiddleItems2.ToString(), sumTableFont65);
                                e.Graphics.DrawString(tablesMiddleItems2.ToString(), sumTableFont65, sumTableBrush65,
                                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize65 = TitG65.MeasureString(tablesMiddleItems3.ToString("0.000%"), sumTableFont65);
                                e.Graphics.DrawString(tablesMiddleItems3.ToString("0.000%"), sumTableFont65, sumTableBrush65,
                                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize65 = TitG65.MeasureString(tablesMiddleItems4.ToString("0.0"), sumTableFont65);
                                e.Graphics.DrawString(tablesMiddleItems4.ToString("0.0"), sumTableFont65, sumTableBrush65,
                                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize65 = TitG65.MeasureString(tablesMiddleItems5.ToString("0.000%"), sumTableFont65);
                                e.Graphics.DrawString(tablesMiddleItems5.ToString("0.000%"), sumTableFont65, sumTableBrush65,
                                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize65 = TitG65.MeasureString(tablesMiddleItems6.ToString(), sumTableFont65);
                                e.Graphics.DrawString(tablesMiddleItems6.ToString(), sumTableFont65, sumTableBrush65,
                                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);

                                XMaxSize65 = TitG65.MeasureString(tablesMiddleItems7.ToString("0.000%"), sumTableFont65);
                                e.Graphics.DrawString(tablesMiddleItems7.ToString("0.000%"), sumTableFont65, sumTableBrush65,
                                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);

                            }
                            ////表格最后一行
                            string tablesLastItems0 = m_resourceManager.GetString("LblPrintSubTotal.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems0, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems0, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems1 = "";
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems1, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems1, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems2 = SumValue60.ToString();
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems2, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems2, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems3 = (SumValue60 == 0 ? "0.000%" : "100.000%");
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems3, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems3, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems4 = (SumWeightValue60 / 1000.0).ToString("0.0");
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems4, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems4, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems5 = (SumWeightValue60 == 0 ? "0.000%" : "100.000%");
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems5, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems5, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems6 = SumBoxValue60.ToString();
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems6, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems6, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems7 = (SumBoxValue60 == 0 ? "0.000%" : "100.000%");
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems7, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems7, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            #endregion
                            //打印页数
                            #region
                            currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                            Font currentPagesFont65 = new Font("宋体", 12, FontStyle.Regular);
                            Brush currentPagesBrush65 = Brushes.Black;
                            string currentPages65 = m_resourceManager.GetString("LblPrintPages.Text") + " " + currentPageIndex.ToString() + "/" + intPage1.ToString();
                            XMaxSize65 = TitG65.MeasureString(currentPages65, currentPagesFont65);
                            e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                                currentPages65, currentPagesFont65, currentPagesBrush65, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin,
                                currentAvailableHeight);
                            #endregion
                            #endregion

                            #region 是否打印副页
                            currentPageIndex++; //下一页页码
                            if (currentPageIndex <= intPage1)  //如果当前页不是最后一页
                            {
                                e.HasMorePages = true;  //打印副页
                            }
                            else
                            {
                                e.HasMorePages = false;  //不打印副页
                                currentPageIndex = 1;
                            }
                            #endregion
                        }
                        else  //中间页
                        {
                            #region 打印中间页
                            ////汇总图头部
                            //#region
                            //currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                            //Font sumImageHeadFont62 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                            //Brush sumImagerHeadBrush62 = Brushes.Black;
                            //string tempQualityName62 = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, ((currentPageIndex - 1) * 2 - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH).ToString();
                            //string sumImageHead62 = m_resourceManager.GetString("LblPrintQualityName.Text") + tempQualityName62.Substring(0, tempQualityName62.IndexOf("\0"));
                            //PictureBox picB62 = new PictureBox();
                            //Graphics TitG62 = picB62.CreateGraphics();
                            //SizeF XMaxSize62 = TitG62.MeasureString(sumImageHead62, sumImageHeadFont62);
                            //e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                            //    sumImageHead62, sumImageHeadFont62, sumImagerHeadBrush62, PrintProtocol.leftMargin, currentAvailableHeight);
                            //#endregion

                            //汇总表格    已修改
                            #region
                            //currentAvailableHeight += (int)XMaxSize62.Height + PrintProtocol.barHeadOrBarImageSpace1;  //当前可用高度
                            bool Isbreak = false;
                            currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                            sumTableHeight = PrintProtocol.sumTableHeight3; //汇总表格高度
                            sumTalbeWidth = (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin) / 8;
                            Pen linePen62 = new Pen(Color.Black, 1);
                            Font sumTableFont62;
                            PictureBox picB62 = new PictureBox();
                            Graphics TitG62 = picB62.CreateGraphics();
                            SizeF XMaxSize62;
                            if (GlobalDataInterface.selectLanguage == "en")
                                sumTableFont62 = new Font("Times New Roman", 9, FontStyle.Regular);
                            else
                                sumTableFont62 = new Font("宋体", 11, FontStyle.Regular);
                            Brush sumTableBrush62 = Brushes.Black;
                            int gradenum = PrintProtocol.GradeNum2;
                            for (int i = 0; i < gradenum + 1; i++) //画横线
                            {
                                e.Graphics.DrawLine(linePen62, PrintProtocol.leftMargin, currentAvailableHeight + i * sumTableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + i * sumTableHeight);
                            }
                            e.Graphics.DrawLine(linePen62, PrintProtocol.leftMargin, currentAvailableHeight + (gradenum + 1) * sumTableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 1) * sumTableHeight);
                            for (int i = 0; i < 8; i++) //画竖线
                            {
                                e.Graphics.DrawLine(linePen62, PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight,
                                    PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight + (gradenum + 1) * sumTableHeight);
                            }
                            e.Graphics.DrawLine(linePen62, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight,
                                     e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 1) * sumTableHeight);
                            //表格标题行
                            string tablesFirstItems0 = m_resourceManager.GetString("LblMainReportName.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems0, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems0, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems1 = m_resourceManager.GetString("LblMainReportSize.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems1, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems1, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems2 = m_resourceManager.GetString("LblMainReportPieces.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems2, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems2, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems3 = m_resourceManager.GetString("LblMainReportPiecesPer.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems3, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems3, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems4 = m_resourceManager.GetString("LblMainReportWeights.Text") + "(kg)";
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems4, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems4, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems5 = m_resourceManager.GetString("LblMainReportWeightPer.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems5, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems5, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems6 = m_resourceManager.GetString("LblMainReportCartons.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems6, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems6, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems7 = m_resourceManager.GetString("LblMainReportCartonsPer.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems7, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems7, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);

                            int index = PrintProtocol.GradeNum1 + PrintProtocol.GradeNum2 * (currentPageIndex - 2) + 1;
                            int heightIndex = 1;

                            SumValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);//获取总个数  //Add by ChengSk - 20180422
                            SumWeightValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);//获取总重量  //Add by ChengSk - 20180422
                            SumBoxValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);//获取总箱数  //Add by ChengSk - 20180422
                            for (int i = 1; i < dataInterface.WeightOrSizeGradeSum + 1; i++)
                            {
                                ulong tablesMiddleItems2 = 0;
                                double tablesMiddleItems3 = 0.0;
                                double tablesMiddleItems4 = 0.0;
                                double tablesMiddleItems5 = 0.0;
                                int tablesMiddleItems6 = 0;
                                double tablesMiddleItems7 = 0.0;
                                for (int j = index / dataInterface.WeightOrSizeGradeSum; j < dataInterface.QualityGradeSum + 1; j++)
                                {
                                    tablesMiddleItems2 += dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)];

                                    if (SumValue60 != 0)
                                    {
                                        tablesMiddleItems3 += ((double)dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / SumValue60);
                                    }
                                    tablesMiddleItems4 += (dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / 1000.0);
                                    if (SumWeightValue60 != 0)
                                    {
                                        tablesMiddleItems5 += ((double)dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / SumWeightValue60);
                                    }
                                    tablesMiddleItems6 += dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)];
                                    if (SumBoxValue60 != 0)
                                    {
                                        tablesMiddleItems7 += ((double)dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / SumBoxValue60);
                                    }
                                    if (heightIndex == PrintProtocol.GradeNum2)
                                    {
                                        Isbreak = true;
                                        break;
                                    }
                                    heightIndex++;
                                }
                                string strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                                string tablesMiddleItems0 = strSizeName.Substring(0, strSizeName.IndexOf("\0"));
                                XMaxSize62 = TitG62.MeasureString(tablesMiddleItems0, sumTableFont62);
                                e.Graphics.DrawString(tablesMiddleItems0, sumTableFont62, sumTableBrush62,
                                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                string tablesMiddleItems1 = dataInterface.IoStStGradeInfo.grades[(i - 1)].nMinSize.ToString();
                                XMaxSize62 = TitG62.MeasureString(tablesMiddleItems1, sumTableFont62);
                                e.Graphics.DrawString(tablesMiddleItems1, sumTableFont62, sumTableBrush62,
                                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize62 = TitG62.MeasureString(tablesMiddleItems2.ToString(), sumTableFont62);
                                e.Graphics.DrawString(tablesMiddleItems2.ToString(), sumTableFont62, sumTableBrush62,
                                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize62 = TitG62.MeasureString(tablesMiddleItems3.ToString("0.000%"), sumTableFont62);
                                e.Graphics.DrawString(tablesMiddleItems3.ToString("0.000%"), sumTableFont62, sumTableBrush62,
                                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize62 = TitG62.MeasureString(tablesMiddleItems4.ToString("0.0"), sumTableFont62);
                                e.Graphics.DrawString(tablesMiddleItems4.ToString("0.0"), sumTableFont62, sumTableBrush62,
                                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize62 = TitG62.MeasureString(tablesMiddleItems5.ToString("0.000%"), sumTableFont62);
                                e.Graphics.DrawString(tablesMiddleItems5.ToString("0.000%"), sumTableFont62, sumTableBrush62,
                                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                XMaxSize62 = TitG62.MeasureString(tablesMiddleItems6.ToString(), sumTableFont62);
                                e.Graphics.DrawString(tablesMiddleItems6.ToString(), sumTableFont62, sumTableBrush62,
                                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);

                                XMaxSize62 = TitG62.MeasureString(tablesMiddleItems7.ToString("0.000%"), sumTableFont62);
                                e.Graphics.DrawString(tablesMiddleItems7.ToString("0.000%"), sumTableFont62, sumTableBrush62,
                                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                    currentAvailableHeight + ((i + 1)) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);


                            }

                            //打印页数
                            #region
                            currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                            Font currentPagesFont62 = new Font("宋体", 12, FontStyle.Regular);
                            Brush currentPagesBrush62 = Brushes.Black;
                            string currentPages62 = m_resourceManager.GetString("LblPrintPages.Text") + " " + currentPageIndex.ToString() + "/" + intPage1.ToString();
                            XMaxSize62 = TitG62.MeasureString(currentPages62, currentPagesFont62);
                            e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                                currentPages62, currentPagesFont62, currentPagesBrush62, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin,
                                currentAvailableHeight);
                            #endregion
                            //#endregion

                            #region 是否打印副页
                            currentPageIndex++; //下一页页码
                            if (currentPageIndex <= intPage1)  //如果当前页不是最后一页
                            {
                                e.HasMorePages = true;  //打印副页
                            }
                            else
                            {
                                e.HasMorePages = false;  //不打印副页
                                currentPageIndex = 1;
                            }
                            #endregion

                            #endregion
                            #endregion
                        }
                        //MessageBox.Show("当前选中统计表");
                        break;
                        #endregion

                    case 6: //6->5 Modify by ChengSk - 20180726 
                        #region 打印      等级统计表
                        //if (bIsHaveColorStatis)
                        //{
                        UInt64 uSumValue60 = 0; //总个数
                        UInt64 uSumWeightValue60 = 0; //总重量
                        Int32 uSumBoxValue60 = 0;  //总箱数   
                        if (currentPageIndex == 1)            //第一页
                        {
                            #region 打印第一页
                            //打印时间
                            #region
                            //currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                            currentAvailableHeight = 20;   //50->20 Modify by ChengSk - 20171124
                            Font dateTimeFont60 = new Font("宋体", 15, FontStyle.Regular);
                            Brush dateTimeBrush60 = Brushes.Black;
                            string nowDateTime60 = DateTime.Now.ToString(m_resourceManager.GetString("LblPrintDateTime.Text"));
                            PictureBox picB60 = new PictureBox();
                            Graphics TitG60 = picB60.CreateGraphics();
                            SizeF XMaxSize60 = TitG60.MeasureString(nowDateTime60, dateTimeFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                                nowDateTime60, dateTimeFont60, dateTimeBrush60, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize60.Width, currentAvailableHeight);
                            #endregion

                            //打印LOGO
                            #region
                            //currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
                            currentAvailableHeight += (int)XMaxSize60.Height + 10;  //20->10 Modify by ChengSk - 20171124
                            try
                            {
                                Bitmap bitmap = new Bitmap(PrintProtocol.logoPathName);//创建位图对象        
                                e.Graphics.DrawImage(bitmap,
                                    (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - bitmap.Width) / 2 + PrintProtocol.leftMargin, currentAvailableHeight, bitmap.Width, bitmap.Height);
                                //currentAvailableHeight += bitmap.Height + PrintProtocol.logoOrTextTitleSpace;    //当前可用高度
                                currentAvailableHeight += bitmap.Height + 15;       //30->15 Modify by ChengSk - 20171124
                            }
                            catch (Exception ee)//捕获异常
                            {
                                MessageBox.Show(ee.Message);//弹出消息对话框
                            }
                            #endregion

                            //文本标题
                            #region
                            Font textTitleFont60 = new Font("宋体", 20, FontStyle.Bold);
                            Brush textTitleBrush60 = Brushes.Black;
                            string textTitle60 = m_resourceManager.GetString("LblPrintBatchReport.Text");
                            XMaxSize60 = TitG60.MeasureString(textTitle60, textTitleFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                                textTitle60, textTitleFont60, textTitleBrush60, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin,
                                currentAvailableHeight);
                            #endregion

                            //分割线1
                            #region
                            //currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                            currentAvailableHeight += (int)XMaxSize60.Height + 10;   //20->10 Modify by ChengSk - 20171124
                            Pen linePen61 = new Pen(Color.Black, 2);
                            e.Graphics.DrawLine(linePen61, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                            #endregion

                            //分割线2
                            #region
                            currentAvailableHeight += (int)linePen61.Width + PrintProtocol.lineOrLineSpace; //当前可用高度
                            Pen linePen62 = new Pen(Color.Black, 2);
                            e.Graphics.DrawLine(linePen62, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                            #endregion

                            //文本内容
                            #region
                            //currentAvailableHeight += (int)linePen62.Width + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                            currentAvailableHeight += (int)linePen62.Width + 10;     //20->10 Modify by ChengSk - 20171124
                            Font textContentFont60 = new Font("宋体", 15, FontStyle.Regular);
                            Brush textContentBrush60 = Brushes.Black;
                            int sumBoxNum = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);  //总箱数
                            string textContent31 = m_resourceManager.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName;
                            XMaxSize60 = TitG60.MeasureString(textContent31, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent31, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                            string textContent32 = m_resourceManager.GetString("LblPrintFarmName.Text") + dataInterface.FarmName;
                            XMaxSize60 = TitG60.MeasureString(textContent32, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent32, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                            string textContent33 = m_resourceManager.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
                            XMaxSize60 = TitG60.MeasureString(textContent33, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent33, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                            currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
                            string textContent34 = m_resourceManager.GetString("LblPrintTotalPieces.Text") + dataInterface.IoStStatistics.nTotalCount;
                            XMaxSize60 = TitG60.MeasureString(textContent34, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent34, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                            string textContent35 = m_resourceManager.GetString("LblPrintTotalWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / 1000000).ToString() +
                                " " + m_resourceManager.GetString("LblPrintTName.Text");
                            XMaxSize60 = TitG60.MeasureString(textContent35, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent35, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                            string textContent36 = m_resourceManager.GetString("LblPrintTotalCartons.Text") + sumBoxNum;
                            XMaxSize60 = TitG60.MeasureString(textContent36, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent36, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                            currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
                            string textContent37 = "";
                            if (dataInterface.IoStStatistics.nTotalCount == 0)
                            {
                                textContent37 = m_resourceManager.GetString("LblPrintAveFruitWeight.Text") + "0.000 " + m_resourceManager.GetString("LblPrintGName.Text");
                            }
                            else
                            {
                                textContent37 = m_resourceManager.GetString("LblPrintAveFruitWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / dataInterface.IoStStatistics.nTotalCount).ToString("0.000") +
                                    " " + m_resourceManager.GetString("LblPrintGName.Text");
                            }
                            XMaxSize60 = TitG60.MeasureString(textContent37, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                                textContent37, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                            string textContent38 = m_resourceManager.GetString("LblPrintProgramName.Text") + dataInterface.ProgramName;
                            XMaxSize60 = TitG60.MeasureString(textContent38, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                               textContent38, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2, currentAvailableHeight);
                            string textContent41 = m_resourceManager.GetString("LblCustomerID.Text") + GlobalDataInterface.SerialNum;
                            XMaxSize60 = TitG60.MeasureString(textContent41, textContentFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                               textContent41, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2 * 2, currentAvailableHeight);//add by xcw 20200701

                            currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度 2015-12-7 ivycc
                            string textContent39 = m_resourceManager.GetString("LblExcelStartTime.Text") + dataInterface.StartTime;
                            XMaxSize60 = TitG60.MeasureString(textContent39, textContentFont60);
                            //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                            //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                               textContent39, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                            string textContent40 = m_resourceManager.GetString("LblExcelEndTime.Text") + dataInterface.EndTime;
                            XMaxSize60 = TitG60.MeasureString(textContent40, textContentFont60);
                            //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                            //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                               textContent40, textContentFont60, textContentBrush60, e.PageBounds.Width / 2/*PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2*/, currentAvailableHeight);
                            #endregion

                            //分割线3
                            #region
                            //currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
                            currentAvailableHeight += (int)XMaxSize60.Height + 10;   //25->10 Modify by ChengSk - 20171124
                            Pen linePen63 = new Pen(Color.Black, 2);
                            e.Graphics.DrawLine(linePen63, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                            #endregion

                            //分割线4
                            #region
                            currentAvailableHeight += (int)linePen63.Width + PrintProtocol.lineOrLineSpace; //当前可用高度
                            Pen linePen64 = new Pen(Color.Black, 2);
                            e.Graphics.DrawLine(linePen64, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                            #endregion
                            //汇总表格 已修改
                            #region
                            bool Isbreak = false;
                            //currentAvailableHeight += (int)linePen64.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                            currentAvailableHeight += (int)linePen64.Width + 10;     //25->10 Modify by ChengSk - 20171124
                            //currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
                            //sumTableHeight = (dataInterface.WeightOrSizeGradeSum <= 10 ? PrintProtocol.sumTableHeight1 : PrintProtocol.sumTableHeight2); //汇总表格高度
                            sumTableHeight = PrintProtocol.sumTableHeight3;
                            sumTalbeWidth = (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin) / 8;
                            Pen linePen60 = new Pen(Color.Black, 1);
                            Font sumTableFont60;
                            if (GlobalDataInterface.selectLanguage == "en")
                                sumTableFont60 = new Font("Times New Roman", 9, FontStyle.Regular);
                            else
                                //sumTableFont60 = (dataInterface.WeightOrSizeGradeSum <= 10 ? (new Font("宋体", 12, FontStyle.Regular)) : (new Font("宋体", 8, FontStyle.Regular)));
                                sumTableFont60 = new Font("宋体", 11, FontStyle.Regular);
                            Brush sumTableBrush60 = Brushes.Black;
                            int gradenum = dataInterface.WeightOrSizeGradeSum * dataInterface.QualityGradeSum;
                            if (gradenum > PrintProtocol.GradeNum1)
                            {
                                gradenum = PrintProtocol.GradeNum1 - 1;
                            }
                            //if(dataInterface.WeightOrSizeGradeSum*dataInterface.QualityGradeSum)
                            for (int i = 0; i < gradenum + 2; i++) //画横线
                            {
                                e.Graphics.DrawLine(linePen60, PrintProtocol.leftMargin, currentAvailableHeight + i * sumTableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + i * sumTableHeight);
                            }
                            e.Graphics.DrawLine(linePen60, PrintProtocol.leftMargin, currentAvailableHeight + (gradenum + 2) * sumTableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 2) * sumTableHeight);
                            for (int i = 0; i < 8; i++) //画竖线
                            {
                                e.Graphics.DrawLine(linePen60, PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight,
                                    PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight + (gradenum + 2) * sumTableHeight);
                            }
                            e.Graphics.DrawLine(linePen60, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 2) * sumTableHeight);
                            //表格标题行
                            string tablesFirstItems0 = m_resourceManager.GetString("LblMainReportName.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems0, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems0, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems1 = m_resourceManager.GetString("LblMainReportSize.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems1, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems1, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems2 = m_resourceManager.GetString("LblMainReportPieces.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems2, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems2, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems3 = m_resourceManager.GetString("LblMainReportPiecesPer.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems3, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems3, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems4 = m_resourceManager.GetString("LblMainReportWeights.Text") + "(kg)";
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems4, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems4, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems5 = m_resourceManager.GetString("LblMainReportWeightPer.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems5, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems5, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems6 = m_resourceManager.GetString("LblMainReportCartons.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems6, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems6, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems7 = m_resourceManager.GetString("LblMainReportCartonsPer.Text");
                            XMaxSize60 = TitG60.MeasureString(tablesFirstItems7, sumTableFont60);
                            e.Graphics.DrawString(tablesFirstItems7, sumTableFont60, sumTableBrush60,
                                (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            //UInt64 uMaxValue60 = 0; //最大个数

                            //for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
                            //{
                            //if (uMaxValue60 < dataInterface.IoStStatistics.nWeightGradeCount[(currentPageIndex - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + i])
                            //{
                            //    uMaxValue60 = dataInterface.IoStStatistics.nWeightGradeCount[(currentPageIndex - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + i];
                            //}
                            //uSumValue60 += dataInterface.IoStStatistics.nGradeCount[(currentPageIndex - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + i];
                            //uSumWeightValue60 += dataInterface.IoStStatistics.nWeightGradeCount[(currentPageIndex - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + i];
                            //uSumBoxValue60 += dataInterface.IoStStatistics.nBoxGradeCount[(currentPageIndex - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + i];
                            uSumValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);//获取总个数
                            uSumWeightValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);//获取总重量
                            uSumBoxValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);//获取总箱数
                            //}
                            for (int j = 1; j < dataInterface.QualityGradeSum + 1; j++)
                            {
                                for (int i = 1; i < dataInterface.WeightOrSizeGradeSum + 1; i++) //中间dataInterface.WeightOrSizeGradeSum行
                                {
                                    string strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                                    string strQualityName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, (j - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                                    int QualityGradeNameLength = strQualityName.IndexOf("\0");
                                    if (QualityGradeNameLength == -1)
                                    {
                                        QualityGradeNameLength = strQualityName.Length;
                                    }
                                    // string strName= strSizeName.Substring(0, strSizeName.IndexOf("\0")) + "." + strQualityName.Substring(0, strQualityName.IndexOf("\0"));
                                    string tablesMiddleItems0 = strSizeName.Substring(0, strSizeName.IndexOf("\0")) + "." + strQualityName.Substring(0, QualityGradeNameLength);
                                    XMaxSize60 = TitG60.MeasureString(tablesMiddleItems0, sumTableFont60);
                                    e.Graphics.DrawString(tablesMiddleItems0, sumTableFont60, sumTableBrush60,
                                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                        currentAvailableHeight + ((j - 1) * dataInterface.WeightOrSizeGradeSum + (i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems1 = dataInterface.IoStStGradeInfo.grades[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].nMinSize.ToString();
                                    XMaxSize60 = TitG60.MeasureString(tablesMiddleItems1, sumTableFont60);
                                    e.Graphics.DrawString(tablesMiddleItems1, sumTableFont60, sumTableBrush60,
                                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                        currentAvailableHeight + ((j - 1) * dataInterface.WeightOrSizeGradeSum + (i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems2 = dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString();
                                    XMaxSize60 = TitG60.MeasureString(tablesMiddleItems2, sumTableFont60);
                                    e.Graphics.DrawString(tablesMiddleItems2, sumTableFont60, sumTableBrush60,
                                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                        currentAvailableHeight + ((j - 1) * dataInterface.WeightOrSizeGradeSum + (i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems3 = "";
                                    if (uSumValue60 == 0)
                                    {
                                        tablesMiddleItems3 = "0.000%";
                                    }
                                    else
                                    {
                                        tablesMiddleItems3 = ((double)dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumValue60).ToString("0.000%");
                                    }
                                    XMaxSize60 = TitG60.MeasureString(tablesMiddleItems3, sumTableFont60);
                                    e.Graphics.DrawString(tablesMiddleItems3, sumTableFont60, sumTableBrush60,
                                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                        currentAvailableHeight + ((j - 1) * dataInterface.WeightOrSizeGradeSum + (i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems4 = (dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / 1000.0).ToString("0.0");
                                    XMaxSize60 = TitG60.MeasureString(tablesMiddleItems4, sumTableFont60);
                                    e.Graphics.DrawString(tablesMiddleItems4, sumTableFont60, sumTableBrush60,
                                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                        currentAvailableHeight + ((j - 1) * dataInterface.WeightOrSizeGradeSum + (i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems5 = "";
                                    if (uSumWeightValue60 == 0)
                                    {
                                        tablesMiddleItems5 = "0.000%";
                                    }
                                    else
                                    {
                                        tablesMiddleItems5 = ((double)dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumWeightValue60).ToString("0.000%");
                                    }
                                    XMaxSize60 = TitG60.MeasureString(tablesMiddleItems5, sumTableFont60);
                                    e.Graphics.DrawString(tablesMiddleItems5, sumTableFont60, sumTableBrush60,
                                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                        currentAvailableHeight + ((j - 1) * dataInterface.WeightOrSizeGradeSum + (i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems6 = dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString(); ;
                                    XMaxSize60 = TitG60.MeasureString(tablesMiddleItems6, sumTableFont60);
                                    e.Graphics.DrawString(tablesMiddleItems6, sumTableFont60, sumTableBrush60,
                                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                        currentAvailableHeight + ((j - 1) * dataInterface.WeightOrSizeGradeSum + (i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems7 = "";
                                    if (uSumBoxValue60 == 0)
                                    {
                                        tablesMiddleItems7 = "0.000%";
                                    }
                                    else
                                    {
                                        tablesMiddleItems7 = ((double)dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumBoxValue60).ToString("0.000%");
                                    }
                                    XMaxSize60 = TitG60.MeasureString(tablesMiddleItems7, sumTableFont60);
                                    e.Graphics.DrawString(tablesMiddleItems7, sumTableFont60, sumTableBrush60,
                                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                        currentAvailableHeight + ((j - 1) * dataInterface.WeightOrSizeGradeSum + (i + 1)) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                    if (((j - 1) * dataInterface.WeightOrSizeGradeSum + i) == PrintProtocol.GradeNum1)
                                    {
                                        Isbreak = true;
                                        break;
                                    }
                                }
                                if (Isbreak)
                                    break;
                            }

                            //表格最后一行
                            if (intPage == 1)
                            {
                                string tablesLastItems0 = m_resourceManager.GetString("LblPrintSubTotal.Text");
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems0, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems0, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum * dataInterface.QualityGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems1 = "";
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems1, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems1, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum * dataInterface.QualityGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems2 = uSumValue60.ToString();
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems2, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems2, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum * dataInterface.QualityGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems3 = (uSumValue60 == 0 ? "0.000%" : "100.000%");
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems3, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems3, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum * dataInterface.QualityGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems4 = (uSumWeightValue60 / 1000.0).ToString("0.0");
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems4, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems4, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum * dataInterface.QualityGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems5 = (uSumWeightValue60 == 0 ? "0.000%" : "100.000%");
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems5, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems5, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum * dataInterface.QualityGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems6 = uSumBoxValue60.ToString();
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems6, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems6, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum * dataInterface.QualityGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                                string tablesLastItems7 = (uSumBoxValue60 == 0 ? "0.000%" : "100.000%");
                                XMaxSize60 = TitG60.MeasureString(tablesLastItems7, sumTableFont60);
                                e.Graphics.DrawString(tablesLastItems7, sumTableFont60, sumTableBrush60,
                                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                    currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum * dataInterface.QualityGradeSum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                            }
                            #endregion
                            //打印页数
                            #region
                            currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                            Font currentPagesFont60 = new Font("宋体", 12, FontStyle.Regular);
                            Brush currentPagesBrush60 = Brushes.Black;
                            string currentPages60 = m_resourceManager.GetString("LblPrintPages.Text") + " " + currentPageIndex.ToString() + "/" + intPage.ToString();
                            XMaxSize60 = TitG60.MeasureString(currentPages60, currentPagesFont60);
                            e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                                currentPages60, currentPagesFont60, currentPagesBrush60, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin,
                                currentAvailableHeight);
                            #endregion

                            //是否打印副页
                            #region
                            currentPageIndex++; //下一页页码
                            if (currentPageIndex <= intPage)  //如果当前页不是最后一页
                            {
                                e.HasMorePages = true;  //打印副页
                            }
                            else
                            {
                                e.HasMorePages = false;  //不打印副页
                                currentPageIndex = 1;
                            }
                            #endregion
                            #endregion
                        }
                        else if (currentPageIndex == intPage) //最后一页
                        {
                            #region 打印最后一页
                            ////汇总图头部
                            //#region
                            //currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                            //Font sumImageHeadFont65 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                            //Brush sumImagerHeadBrush65 = Brushes.Black;
                            //string tempQualityName65 = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, (dataInterface.QualityGradeSum - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH).ToString();
                            //string sumImageHead65 = m_resourceManager.GetString("LblPrintQualityName.Text") + tempQualityName65.Substring(0, tempQualityName65.IndexOf("\0"));
                            //PictureBox picB65 = new PictureBox();
                            //Graphics TitG65 = picB65.CreateGraphics();
                            //SizeF XMaxSize65 = TitG65.MeasureString(sumImageHead65, sumImageHeadFont65);
                            //e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                            //    sumImageHead65, sumImageHeadFont65, sumImagerHeadBrush65, PrintProtocol.leftMargin, currentAvailableHeight);
                            //#endregion

                            //汇总表格 已修改
                            #region
                            currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                            //currentAvailableHeight += (int)XMaxSize65.Height + PrintProtocol.barHeadOrBarImageSpace1;  //当前可用高度
                            sumTableHeight = PrintProtocol.sumTableHeight3; //汇总表格高度
                            sumTalbeWidth = (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin) / 8;
                            Pen linePen65 = new Pen(Color.Black, 1);
                            Font sumTableFont65;
                            PictureBox picB65 = new PictureBox();
                            Graphics TitG65 = picB65.CreateGraphics();
                            SizeF XMaxSize65;
                            if (GlobalDataInterface.selectLanguage == "en")
                                sumTableFont65 = new Font("Times New Roman", 9, FontStyle.Regular);
                            else
                                sumTableFont65 = new Font("宋体", 11, FontStyle.Regular);
                            Brush sumTableBrush65 = Brushes.Black;
                            int gradenum = dataInterface.WeightOrSizeGradeSum * dataInterface.QualityGradeSum - (PrintProtocol.GradeNum1 + PrintProtocol.GradeNum2 * (currentPageIndex - 2));
                            for (int i = 0; i < gradenum + 2; i++) //画横线
                            {
                                e.Graphics.DrawLine(linePen65, PrintProtocol.leftMargin, currentAvailableHeight + i * sumTableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + i * sumTableHeight);
                            }
                            e.Graphics.DrawLine(linePen65, PrintProtocol.leftMargin, currentAvailableHeight + (gradenum + 2) * sumTableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 2) * sumTableHeight);
                            for (int i = 0; i < 8; i++) //画竖线
                            {
                                e.Graphics.DrawLine(linePen65, PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight,
                                    PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight + (gradenum + 2) * sumTableHeight);
                            }
                            e.Graphics.DrawLine(linePen65, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 2) * sumTableHeight);
                            //表格标题行
                            string tablesFirstItems0 = m_resourceManager.GetString("LblMainReportName.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems0, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems0, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems1 = m_resourceManager.GetString("LblMainReportSize.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems1, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems1, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems2 = m_resourceManager.GetString("LblMainReportPieces.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems2, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems2, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems3 = m_resourceManager.GetString("LblMainReportPiecesPer.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems3, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems3, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems4 = m_resourceManager.GetString("LblMainReportWeights.Text") + "(kg)";
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems4, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems4, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems5 = m_resourceManager.GetString("LblMainReportWeightPer.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems5, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems5, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems6 = m_resourceManager.GetString("LblMainReportCartons.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems6, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems6, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems7 = m_resourceManager.GetString("LblMainReportCartonsPer.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesFirstItems7, sumTableFont65);
                            e.Graphics.DrawString(tablesFirstItems7, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            int index = PrintProtocol.GradeNum1 + PrintProtocol.GradeNum2 * (currentPageIndex - 2) + 1;
                            int heightIndex = 1;

                            uSumValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);//获取总个数  //Add by ChengSk - 20180422
                            uSumWeightValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);//获取总重量  //Add by ChengSk - 20180422
                            uSumBoxValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);//获取总箱数  //Add by ChengSk - 20180422

                            for (int j = index / dataInterface.WeightOrSizeGradeSum; j < dataInterface.QualityGradeSum + 1; j++)//ivycc 2016-11-7 j = index/dataInterface.WeightOrSizeGradeSum+1 -》j = index/dataInterface.WeightOrSizeGradeSum
                            {
                                for (int i = 1; i < dataInterface.WeightOrSizeGradeSum + 1; i++) //中间dataInterface.WeightOrSizeGradeSum行
                                {
                                    if (((j - 1) * dataInterface.WeightOrSizeGradeSum + i) < index)
                                        continue;
                                    string strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                                    string strQualityName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, (j - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                                    int QualityGradeNameLength = strQualityName.IndexOf("\0");
                                    if (QualityGradeNameLength == -1)
                                    {
                                        QualityGradeNameLength = strQualityName.Length;
                                    }
                                    string tablesMiddleItems0 = strSizeName.Substring(0, strSizeName.IndexOf("\0")) + "." + strQualityName.Substring(0, QualityGradeNameLength);
                                    XMaxSize65 = TitG65.MeasureString(tablesMiddleItems0, sumTableFont65);
                                    e.Graphics.DrawString(tablesMiddleItems0, sumTableFont65, sumTableBrush65,
                                        (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems1 = dataInterface.IoStStGradeInfo.grades[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].nMinSize.ToString();
                                    XMaxSize65 = TitG65.MeasureString(tablesMiddleItems1, sumTableFont65);
                                    e.Graphics.DrawString(tablesMiddleItems1, sumTableFont65, sumTableBrush65,
                                        (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems2 = dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString();
                                    XMaxSize65 = TitG65.MeasureString(tablesMiddleItems2, sumTableFont65);
                                    e.Graphics.DrawString(tablesMiddleItems2, sumTableFont65, sumTableBrush65,
                                        (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems3 = "";
                                    if (uSumValue60 == 0)
                                    {
                                        tablesMiddleItems3 = "0.000%";
                                    }
                                    else
                                    {
                                        tablesMiddleItems3 = ((double)dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumValue60).ToString("0.000%");
                                    }
                                    XMaxSize65 = TitG65.MeasureString(tablesMiddleItems3, sumTableFont65);
                                    e.Graphics.DrawString(tablesMiddleItems3, sumTableFont65, sumTableBrush65,
                                        (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems4 = (dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / 1000.0).ToString("0.0");
                                    XMaxSize65 = TitG65.MeasureString(tablesMiddleItems4, sumTableFont65);
                                    e.Graphics.DrawString(tablesMiddleItems4, sumTableFont65, sumTableBrush65,
                                        (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems5 = "";
                                    if (uSumWeightValue60 == 0)
                                    {
                                        tablesMiddleItems5 = "0.000%";
                                    }
                                    else
                                    {
                                        tablesMiddleItems5 = ((double)dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumWeightValue60).ToString("0.000%");
                                    }
                                    XMaxSize65 = TitG65.MeasureString(tablesMiddleItems5, sumTableFont65);
                                    e.Graphics.DrawString(tablesMiddleItems5, sumTableFont65, sumTableBrush65,
                                        (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems6 = dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString(); ;
                                    XMaxSize65 = TitG65.MeasureString(tablesMiddleItems6, sumTableFont65);
                                    e.Graphics.DrawString(tablesMiddleItems6, sumTableFont65, sumTableBrush65,
                                        (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems7 = "";
                                    if (uSumBoxValue60 == 0)
                                    {
                                        tablesMiddleItems7 = "0.000%";
                                    }
                                    else
                                    {
                                        tablesMiddleItems7 = ((double)dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumBoxValue60).ToString("0.000%");
                                    }
                                    XMaxSize65 = TitG65.MeasureString(tablesMiddleItems7, sumTableFont65);
                                    e.Graphics.DrawString(tablesMiddleItems7, sumTableFont65, sumTableBrush65,
                                        (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                                    heightIndex++;
                                }
                            }
                            //表格最后一行
                            string tablesLastItems0 = m_resourceManager.GetString("LblPrintSubTotal.Text");
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems0, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems0, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems1 = "";
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems1, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems1, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems2 = uSumValue60.ToString();
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems2, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems2, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems3 = (uSumValue60 == 0 ? "0.000%" : "100.000%");
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems3, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems3, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems4 = (uSumWeightValue60 / 1000.0).ToString("0.0");
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems4, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems4, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems5 = (uSumWeightValue60 == 0 ? "0.000%" : "100.000%");
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems5, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems5, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems6 = uSumBoxValue60.ToString();
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems6, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems6, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            string tablesLastItems7 = (uSumBoxValue60 == 0 ? "0.000%" : "100.000%");
                            XMaxSize65 = TitG65.MeasureString(tablesLastItems7, sumTableFont65);
                            e.Graphics.DrawString(tablesLastItems7, sumTableFont65, sumTableBrush65,
                                (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                            #endregion
                            //打印页数
                            #region
                            currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                            Font currentPagesFont65 = new Font("宋体", 12, FontStyle.Regular);
                            Brush currentPagesBrush65 = Brushes.Black;
                            string currentPages65 = m_resourceManager.GetString("LblPrintPages.Text") + " " + currentPageIndex.ToString() + "/" + intPage.ToString();
                            XMaxSize65 = TitG65.MeasureString(currentPages65, currentPagesFont65);
                            e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                                currentPages65, currentPagesFont65, currentPagesBrush65, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin,
                                currentAvailableHeight);
                            #endregion
                            #endregion

                            #region 是否打印副页
                            currentPageIndex++; //下一页页码
                            if (currentPageIndex <= intPage)  //如果当前页不是最后一页
                            {
                                e.HasMorePages = true;  //打印副页
                            }
                            else
                            {
                                e.HasMorePages = false;  //不打印副页
                                currentPageIndex = 1;
                            }
                            #endregion
                        }
                        else  //中间页
                        {
                            #region 打印中间页
                            ////汇总图头部
                            //#region
                            //currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                            //Font sumImageHeadFont62 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                            //Brush sumImagerHeadBrush62 = Brushes.Black;
                            //string tempQualityName62 = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, ((currentPageIndex - 1) * 2 - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH).ToString();
                            //string sumImageHead62 = m_resourceManager.GetString("LblPrintQualityName.Text") + tempQualityName62.Substring(0, tempQualityName62.IndexOf("\0"));
                            //PictureBox picB62 = new PictureBox();
                            //Graphics TitG62 = picB62.CreateGraphics();
                            //SizeF XMaxSize62 = TitG62.MeasureString(sumImageHead62, sumImageHeadFont62);
                            //e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                            //    sumImageHead62, sumImageHeadFont62, sumImagerHeadBrush62, PrintProtocol.leftMargin, currentAvailableHeight);
                            //#endregion

                            //汇总表格    已修改
                            #region
                            //currentAvailableHeight += (int)XMaxSize62.Height + PrintProtocol.barHeadOrBarImageSpace1;  //当前可用高度
                            bool Isbreak = false;
                            currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                            sumTableHeight = PrintProtocol.sumTableHeight3; //汇总表格高度
                            sumTalbeWidth = (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin) / 8;
                            Pen linePen62 = new Pen(Color.Black, 1);
                            Font sumTableFont62;
                            PictureBox picB62 = new PictureBox();
                            Graphics TitG62 = picB62.CreateGraphics();
                            SizeF XMaxSize62;
                            if (GlobalDataInterface.selectLanguage == "en")
                                sumTableFont62 = new Font("Times New Roman", 9, FontStyle.Regular);
                            else
                                sumTableFont62 = new Font("宋体", 11, FontStyle.Regular);
                            Brush sumTableBrush62 = Brushes.Black;
                            int gradenum = PrintProtocol.GradeNum2;
                            for (int i = 0; i < gradenum + 1; i++) //画横线
                            {
                                e.Graphics.DrawLine(linePen62, PrintProtocol.leftMargin, currentAvailableHeight + i * sumTableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + i * sumTableHeight);
                            }
                            e.Graphics.DrawLine(linePen62, PrintProtocol.leftMargin, currentAvailableHeight + (gradenum + 1) * sumTableHeight,
                                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 1) * sumTableHeight);
                            for (int i = 0; i < 8; i++) //画竖线
                            {
                                e.Graphics.DrawLine(linePen62, PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight,
                                    PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight + (gradenum + 1) * sumTableHeight);
                            }
                            e.Graphics.DrawLine(linePen62, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight,
                                     e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 1) * sumTableHeight);
                            //表格标题行
                            string tablesFirstItems0 = m_resourceManager.GetString("LblMainReportName.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems0, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems0, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems1 = m_resourceManager.GetString("LblMainReportSize.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems1, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems1, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems2 = m_resourceManager.GetString("LblMainReportPieces.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems2, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems2, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems3 = m_resourceManager.GetString("LblMainReportPiecesPer.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems3, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems3, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems4 = m_resourceManager.GetString("LblMainReportWeights.Text") + "(kg)";
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems4, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems4, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems5 = m_resourceManager.GetString("LblMainReportWeightPer.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems5, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems5, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems6 = m_resourceManager.GetString("LblMainReportCartons.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems6, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems6, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                            string tablesFirstItems7 = m_resourceManager.GetString("LblMainReportCartonsPer.Text");
                            XMaxSize62 = TitG62.MeasureString(tablesFirstItems7, sumTableFont62);
                            e.Graphics.DrawString(tablesFirstItems7, sumTableFont62, sumTableBrush62,
                                (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);

                            int index = PrintProtocol.GradeNum1 + PrintProtocol.GradeNum2 * (currentPageIndex - 2) + 1;
                            int heightIndex = 1;

                            uSumValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);//获取总个数  //Add by ChengSk - 20180422
                            uSumWeightValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);//获取总重量  //Add by ChengSk - 20180422
                            uSumBoxValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);//获取总箱数  //Add by ChengSk - 20180422

                            for (int j = index / dataInterface.WeightOrSizeGradeSum + 1; j < dataInterface.QualityGradeSum + 1; j++)
                            {
                                for (int i = 1; i < dataInterface.WeightOrSizeGradeSum + 1; i++) //中间dataInterface.WeightOrSizeGradeSum行
                                {
                                    if (((j - 1) * dataInterface.WeightOrSizeGradeSum + i) < index)
                                        continue;
                                    string strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                                    string strQualityName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, (j - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                                    int QualityGradeNameLength = strQualityName.IndexOf("\0");
                                    if (QualityGradeNameLength == -1)
                                    {
                                        QualityGradeNameLength = strQualityName.Length;
                                    }
                                    string tablesMiddleItems0 = strSizeName.Substring(0, strSizeName.IndexOf("\0")) + "." + strQualityName.Substring(0, QualityGradeNameLength);
                                    XMaxSize62 = TitG62.MeasureString(tablesMiddleItems0, sumTableFont62);
                                    e.Graphics.DrawString(tablesMiddleItems0, sumTableFont62, sumTableBrush62,
                                        (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                    //string tablesMiddleItems1 = dataInterface.IoStStGradeInfo.grades[((j - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].nMinSize.ToString();  //Note by ChengSk - 20180422
                                    string tablesMiddleItems1 = dataInterface.IoStStGradeInfo.grades[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].nMinSize.ToString();
                                    XMaxSize62 = TitG62.MeasureString(tablesMiddleItems1, sumTableFont62);
                                    e.Graphics.DrawString(tablesMiddleItems1, sumTableFont62, sumTableBrush62,
                                        (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                    //string tablesMiddleItems2 = dataInterface.IoStStatistics.nGradeCount[((j - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString();  //Note by ChengSk - 20180422
                                    string tablesMiddleItems2 = dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString();
                                    XMaxSize62 = TitG62.MeasureString(tablesMiddleItems2, sumTableFont62);
                                    e.Graphics.DrawString(tablesMiddleItems2, sumTableFont62, sumTableBrush62,
                                        (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems3 = "";
                                    if (uSumValue60 == 0)
                                    {
                                        tablesMiddleItems3 = "0.000%";
                                    }
                                    else
                                    {
                                        //tablesMiddleItems3 = ((double)dataInterface.IoStStatistics.nGradeCount[((j - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumValue60).ToString("0.000%");  //Note by ChengSk - 20180422
                                        tablesMiddleItems3 = ((double)dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumValue60).ToString("0.000%");
                                    }
                                    XMaxSize62 = TitG62.MeasureString(tablesMiddleItems3, sumTableFont62);
                                    e.Graphics.DrawString(tablesMiddleItems3, sumTableFont62, sumTableBrush62,
                                        (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                    //string tablesMiddleItems4 = (dataInterface.IoStStatistics.nWeightGradeCount[((j - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / 1000.0).ToString("0.0");  //Note by ChengSk - 20180422
                                    string tablesMiddleItems4 = (dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / 1000.0).ToString("0.0");
                                    XMaxSize62 = TitG62.MeasureString(tablesMiddleItems4, sumTableFont62);
                                    e.Graphics.DrawString(tablesMiddleItems4, sumTableFont62, sumTableBrush62,
                                        (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems5 = "";
                                    if (uSumWeightValue60 == 0)
                                    {
                                        tablesMiddleItems5 = "0.000%";
                                    }
                                    else
                                    {
                                        //tablesMiddleItems5 = ((double)dataInterface.IoStStatistics.nWeightGradeCount[((j - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumWeightValue60).ToString("0.000%");   //Note by ChengSk - 20180422
                                        tablesMiddleItems5 = ((double)dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumWeightValue60).ToString("0.000%");
                                    }
                                    XMaxSize62 = TitG62.MeasureString(tablesMiddleItems5, sumTableFont62);
                                    e.Graphics.DrawString(tablesMiddleItems5, sumTableFont62, sumTableBrush62,
                                        (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                    //string tablesMiddleItems6 = dataInterface.IoStStatistics.nBoxGradeCount[((j - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString();   //Note by ChengSk - 20180422
                                    string tablesMiddleItems6 = dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString(); ;
                                    XMaxSize62 = TitG62.MeasureString(tablesMiddleItems6, sumTableFont62);
                                    e.Graphics.DrawString(tablesMiddleItems6, sumTableFont62, sumTableBrush62,
                                        (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                    string tablesMiddleItems7 = "";
                                    if (uSumBoxValue60 == 0)
                                    {
                                        tablesMiddleItems7 = "0.000%";
                                    }
                                    else
                                    {
                                        //tablesMiddleItems7 = ((double)dataInterface.IoStStatistics.nBoxGradeCount[((currentPageIndex - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumBoxValue60).ToString("0.000%");  //Note by ChengSk - 20180422
                                        tablesMiddleItems7 = ((double)dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumBoxValue60).ToString("0.000%");
                                    }
                                    XMaxSize62 = TitG62.MeasureString(tablesMiddleItems7, sumTableFont62);
                                    e.Graphics.DrawString(tablesMiddleItems7, sumTableFont62, sumTableBrush62,
                                        (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                                        currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                                    if (heightIndex == PrintProtocol.GradeNum2)
                                    {
                                        Isbreak = true;
                                        break;
                                    }
                                    heightIndex++;
                                }
                                if (Isbreak)
                                    break;
                            }

                            //打印页数
                            #region
                            currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                            Font currentPagesFont62 = new Font("宋体", 12, FontStyle.Regular);
                            Brush currentPagesBrush62 = Brushes.Black;
                            string currentPages62 = m_resourceManager.GetString("LblPrintPages.Text") + " " + currentPageIndex.ToString() + "/" + intPage.ToString();
                            XMaxSize62 = TitG62.MeasureString(currentPages62, currentPagesFont62);
                            e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                                currentPages62, currentPagesFont62, currentPagesBrush62, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin,
                                currentAvailableHeight);
                            #endregion
                            //#endregion

                            #region 是否打印副页
                            currentPageIndex++; //下一页页码
                            if (currentPageIndex <= intPage)  //如果当前页不是最后一页
                            {
                                e.HasMorePages = true;  //打印副页
                            }
                            else
                            {
                                e.HasMorePages = false;  //不打印副页
                                currentPageIndex = 1;
                            }
                            #endregion

                            #endregion
                            #endregion
                        }
                        //MessageBox.Show("当前选中统计表");
                        break;
                    #endregion
                    default:
                        break;
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Error：" + ex.ToString() + "\nError Trace：" + ex.StackTrace);
            }
        }


        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (showPrintDialog)
            {
                printDialog1.PrinterSettings.Collate = false;
                printDialog1.Document = this.printDocument1;//设置要打印的文档
                if (printDialog1.ShowDialog() == DialogResult.OK)//弹出打印选项对话框
                {
                    e.Cancel = true;
                    showPrintDialog = false;
                    this.printDocument1.Print();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void printDocument1_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            showPrintDialog = true;
        }

        private void StatisticsInfoForm3_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportExcelbutton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "XLS格式(*.xls)|";
            dlg.RestoreDirectory = true;
            string strFileName = dataInterface.EndTime;
            strFileName = strFileName.Replace("-", "");
            strFileName = strFileName.Replace(" ", "");
            strFileName = strFileName.Replace(":", "");
            dlg.FileName = strFileName;//Add by ChengSk - 20181123
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string temp = dlg.FileName + ".xls";
                FileInfo File = new FileInfo(temp);
                if (File.Exists)
                {
                    //DialogResult result = MessageBox.Show("是否覆盖原来的配置信息?", "保存配置", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //DialogResult result = MessageBox.Show("0x30001021 Whether to overwrite the original configuration information?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    DialogResult result = MessageBox.Show("0x30001021 " + LanguageContainer.StatisticsInfoForm3Messagebox1Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.StatisticsInfoForm3MessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }
                string[] resource = { DateTime.Now.ToString(m_resourceManager.GetString("LblPrintDateTime.Text")),m_resourceManager.GetString("LblPrintBatchReport.Text"),m_resourceManager.GetString("LblPrintCustomerName.Text"),
                                     m_resourceManager.GetString("LblPrintFarmName.Text"),m_resourceManager.GetString("LblPrintFruitVarieties.Text"),m_resourceManager.GetString("LblPrintTotalPieces.Text"),
                                     m_resourceManager.GetString("LblPrintTotalWeight.Text"),m_resourceManager.GetString("LblPrintTName.Text"),m_resourceManager.GetString("LblPrintTotalCartons.Text"),
                                     m_resourceManager.GetString("LblPrintAveFruitWeight.Text"),m_resourceManager.GetString("LblPrintGName.Text"),m_resourceManager.GetString("LblPrintProgramName.Text"),
                                     m_resourceManager.GetString("LblExcelStartTime.Text"),m_resourceManager.GetString("LblExcelEndTime.Text"), m_resourceManager.GetString("LblMainReportName.Text"), 
                                     m_resourceManager.GetString("LblMainReportSize.Text"),m_resourceManager.GetString("LblMainReportPieces.Text"), m_resourceManager.GetString("LblMainReportPiecesPer.Text"),
                                     m_resourceManager.GetString("LblMainReportWeights.Text"),m_resourceManager.GetString("LblMainReportWeightPer.Text"), m_resourceManager.GetString("LblMainReportCartons.Text"),
                                     m_resourceManager.GetString("LblMainReportCartonsPer.Text"), m_resourceManager.GetString("LblCustomerID.Text")};//add by xcw 20200701
                if (tabControl1.SelectedIndex == 5)
                {
                    ExcelReportFunc.CreateExcel(dlg.FileName, dataInterface, resource, false, true);
                }
                else
                {
                    ExcelReportFunc.CreateExcel(dlg.FileName, dataInterface, resource, true, true);
                }

                //ExcelReportFunc.CreateExcel(dlg.FileName, dataInterface,true,true);
                //MessageBox.Show("Export excel report successfully!");
                MessageBox.Show(LanguageContainer.StatisticsInfoForm3Messagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.StatisticsInfoForm3MessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 选中总计模式（1s刷新一次）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatisticModebutton_Click(object sender, EventArgs e)
        {
            this.StatisticModebutton.Enabled = false;
            this.TimerModebutton.Enabled = true;
            bIsTimerMode = false;
        }

        /// <summary>
        /// 选中定时模式（1min短时比例刷新）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerModebutton_Click(object sender, EventArgs e)
        {
            this.StatisticModebutton.Enabled = true;
            this.TimerModebutton.Enabled = false;
            OneMinuteBeforeStatisticsData = new stStatistics(true);
            OneMinuteBeforeStatisticsData.ToCopy(this.dataInterface.IoStStatistics);
            bIsTimerMode = true;
        }

        /// <summary>
        /// 窗体退出事件，资源释放  Add by ChengSk - 20180815
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatisticsInfoForm3_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                GlobalDataInterface.UpdateDataInterfaceEvent -= new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent);
                if (bitM1 != null)
                {
                    g1.Dispose();
                    g1 = null;
                    bitM1.Dispose();
                    bitM1 = null;
                }
                if (bitM2 != null)
                {
                    g2.Dispose();
                    g2 = null;
                    bitM2.Dispose();
                    bitM2 = null;
                }
                if (bitM3 != null)
                {
                    g3.Dispose();
                    g3 = null;
                    bitM3.Dispose();
                    bitM3 = null;
                }
                if (bitM4 != null)
                {
                    g4.Dispose();
                    g4 = null;
                    bitM4.Dispose();
                    bitM4 = null;
                }
                if (bitM51 != null)
                {
                    g51.Dispose();
                    g51 = null;
                    bitM51.Dispose();
                    bitM51 = null;
                }
                if (bitM52 != null)
                {
                    g52.Dispose();
                    g52 = null;
                    bitM52.Dispose();
                    bitM52 = null;
                }
            }
            catch (Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("StatisticsInfoForm3_FormClosing出错: " + ex.StackTrace);
#endif
            }
        }      

    }
}
