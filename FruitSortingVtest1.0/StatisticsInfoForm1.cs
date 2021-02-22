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
using Draw3DBarGraph;
using System.Resources;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using Common;

namespace FruitSortingVtest1
{
    public partial class StatisticsInfoForm1 : Form
    {
        private DataInterface dataInterface;
        private Boolean bIsSourceDB;
        private ResourceManager m_resourceManager = new ResourceManager(typeof(StatisticsInfoForm1));//创建Mainform资源管理
        private bool bIsTimerMode = false;         //是否为定时模式
        private stStatistics OneMinuteBeforeStatisticsData;  //1min之前的Statistics的数据

        //AutoSizeFormClass asc = new AutoSizeFormClass();//声明大小自适应类实例  

        private Bitmap bitM1 = null; //将局部变量改为全局变量，便于窗体退出时资源释放 Add by ChengSk - 20180815
        private Graphics g1;
        private Bitmap bitM2 = null;
        private Graphics g2;
        private Bitmap bitM3 = null;
        private Graphics g3;

        public StatisticsInfoForm1()
        {
            InitializeComponent();
        }

        public StatisticsInfoForm1(DataInterface dataInterface1)
        {
            if (dataInterface1.BSourceDB)
            {
                bIsSourceDB = true;
            }
            else{
                bIsSourceDB = false;
            }

            this.dataInterface = dataInterface1;
            InitializeComponent();
            GlobalDataInterface.UpdateDataInterfaceEvent += new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent);
        }

        private void OnUpdateDataInterfaceEvent(DataInterface dataInterface1)
        {
            if (bIsSourceDB)
            {
                return;
            }
            if (InvokeRequired)
            {
                BeginInvoke(new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent), dataInterface1);
                return;
            }

            if (!bIsTimerMode) //正常刷新模式
            {
                this.dataInterface = dataInterface1;
            }
            else   //短时刷新模式
            {
                this.dataInterface = Commonfunction.GetDifferDataInterface(OneMinuteBeforeStatisticsData, dataInterface1);  
            }

            if (this.tabControl1.SelectedIndex != 6)
                tabControl1_SelectedIndexChanged(null, null);
            else
            {
                FillcurrentSelectTabpage5();
                FillcurrentSelectTabpage4();
            }
                
        }

        private void StatisticsInfoForm1_Load(object sender, EventArgs e)
        {
            //加载时默认选中TabPage1
            currentSelectTabpage1();

            #region ListView标题栏设置
            LvwFruitData.Dock = DockStyle.Fill;
            LvwFruitData.Columns.Add("GradeName", m_resourceManager.GetString("LblMainReportName.Text"));
            LvwFruitData.Columns["GradeName"].Width = 100;
            LvwFruitData.Columns["GradeName"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("SizeLimit", m_resourceManager.GetString("LblMainReportSize.Text"));
            LvwFruitData.Columns["SizeLimit"].Width = 100;  //100
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
            LvwSizeOrWeightData.Dock = DockStyle.Fill;
            LvwSizeOrWeightData.Columns.Add("GradeName", m_resourceManager.GetString("LblMainReportName.Text"));
            LvwSizeOrWeightData.Columns["GradeName"].Width = 100;
            LvwSizeOrWeightData.Columns["GradeName"].TextAlign = HorizontalAlignment.Center;
            LvwSizeOrWeightData.Columns.Add("SizeLimit", m_resourceManager.GetString("LblMainReportSize.Text"));
            LvwSizeOrWeightData.Columns["SizeLimit"].Width = 100;  //100
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

            //asc.controllInitializeSize(this); 
        }

        private void currentSelectTabpage1()
        {
            #region 变量声明
            int CylinderHeight1;       //柱高
            bool bHaveHscrollBar;      //是否有滚动条
            int leftRightSpace = 0;    //柱体离左右边框距离
            UInt64 uMaxValue = 0;      //出口中"个数"最大值
            UInt64 uSumValue = 0;      //所有出口"个数"总值
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
            //"数量/百分比"字样
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
                            PicExport.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance -
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

                        if((int)(CylinderHeight1 * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
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
            int CylinderHeight2;      //柱高
            bool bHaveHscrollBar;     //是否有滚动条
            int leftRightSpace = 0;   //柱体离左右边框距离
            UInt64 uMaxValue = 0;     //各尺寸"个数"最大值
            UInt64 uSumValue = 0;     //各尺寸"个数"总值
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
                DrawGraphProtocol.CylinderWidth < tabPage2.Width)  //无需使用滚动条
            {
                bHaveHscrollBar = false;
                hScrollBar2.Visible = false;
                PicSize.Dock = DockStyle.Fill;
                CylinderHeight2 = DrawGraphProtocol.CylinderHeight2;
                leftRightSpace = (PicSize.Width - (dataInterface.WeightOrSizeGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) -
                    DrawGraphProtocol.CylinderWidth) / 2;
            }
            else  //需要使用滚动条
            {
                bHaveHscrollBar = true;
                PicSize.Width = (dataInterface.WeightOrSizeGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
                    DrawGraphProtocol.CylinderWidth + 2 * DrawGraphProtocol.LeftRightSpace;
                hScrollBar2.Maximum = PicSize.Width - tabPage2.Width;
                CylinderHeight2 = DrawGraphProtocol.CylinderHeight1;
            }
            #endregion

            //Bitmap bitM = new Bitmap(PicSize.Width, PicSize.Height);
            //Graphics g = Graphics.FromImage(bitM);
            if(bitM2 == null) //Modify by ChengSk - 20180815
            {
                bitM2 = new Bitmap(PicSize.Width, PicSize.Height);
                g2 = Graphics.FromImage(bitM2);
            }
            g2.Clear(DrawGraphProtocol.myBarBackColor);
            //定义画刷
            Brush currentBrush = new SolidBrush(DrawGraphProtocol.myPenColor);
            Brush BarBrush = new SolidBrush(DrawGraphProtocol.myBarBrush1);

            #region 数量/百分比 尺寸统计信息
            //"数量/百分比"字样
            //g.DrawString("数量/百分比", new Font(DrawGraphProtocol.Module1FontStyle, DrawGraphProtocol.Module1FontSize, FontStyle.Regular),
            //    currentBrush, DrawGraphProtocol.Module1LocationX, DrawGraphProtocol.Module1LocationY);
            //"尺寸统计信息"字样
            g2.DrawString(m_resourceManager.GetString("tabPage2.Text"), new Font(DrawGraphProtocol.Module3FontStyle, DrawGraphProtocol.Module3FontSize, FontStyle.Bold),
                currentBrush, PicSize.Width / 2 - DrawGraphProtocol.Module3MiddleDistance, PicSize.Height - DrawGraphProtocol.Module3BottomDistance);
            #endregion

            #region 尺寸名称 柱体 个数 百分比
            //"尺寸名称"字样  "柱体"图形显示  个数  百分比
            uMaxValue = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nWeightGradeCount);
            uSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);

            for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
            {
                if (bHaveHscrollBar)  //有滚动条时
                {
                    //"尺寸名称"字样
                    g2.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH), 
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace), 
                        PicSize.Height - DrawGraphProtocol.Module4BottomDistance);

                    if (dataInterface.IoStStatistics.nWeightGradeCount[i] == 0)
                    {
                        //"柱体"图形显示
                        g2.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g2.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight2 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight2 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue)));

                        if ((int)(CylinderHeight2 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g2.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g2.RotateTransform(-90f);
                            g2.DrawString(dataInterface.IoStStatistics.nWeightGradeCount[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g2.ResetTransform();
                        }
                        else
                        {
                            g2.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight2 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue)));
                            g2.RotateTransform(-90f);
                            g2.DrawString(dataInterface.IoStStatistics.nWeightGradeCount[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g2.ResetTransform();
                        }
                        
                    }

                    if (uSumValue == 0)
                    {
                        //百分比
                        g2.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g2.DrawString(((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uSumValue).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight2 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    

                }
                else  //无滚动条时
                {
                    //"尺寸名称"字样
                    g2.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH), 
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace), 
                        PicSize.Height - DrawGraphProtocol.Module4BottomDistance);

                    if (dataInterface.IoStStatistics.nWeightGradeCount[i] == 0)
                    {
                        //"柱体"图形显示
                        g2.FillRectangle(BarBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g2.FillRectangle(BarBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight2 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight2 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue)));

                        if ((int)(CylinderHeight2 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g2.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g2.RotateTransform(-90f);
                            g2.DrawString(dataInterface.IoStStatistics.nWeightGradeCount[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g2.ResetTransform();
                        }
                        else
                        {
                            g2.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight2 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue)));
                            g2.RotateTransform(-90f);
                            g2.DrawString(dataInterface.IoStStatistics.nWeightGradeCount[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g2.ResetTransform();
                        }
                        
                    }

                    if (uSumValue == 0)
                    {
                        //百分比
                        g2.DrawString("0.00%",
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g2.DrawString(((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uSumValue).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicSize.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight2 * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                }
            }
            #endregion

            PicSize.BackgroundImage = null;  //无此行代码不会刷新，必须加上 Add by ChengSk - 20180821
            PicSize.BackgroundImage = bitM2;
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            PicSize.Left = -hScrollBar2.Value;
        }

        private void currentSelectTabpage3()
        {
            #region 变量声明
            int CylinderHeight3;      //柱高
            bool bHaveHscrollBar;     //是否有滚动条
            int leftRightSpace = 0;   //柱体离左右边框距离
            Int32 uMaxValue = 0;      //各尺寸"箱数"最大值
            Int32 uSumValue = 0;      //各尺寸"箱数"总值
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
                DrawGraphProtocol.CylinderWidth < tabPage3.Width)  //无需使用滚动条
            {
                bHaveHscrollBar = false;
                hScrollBar3.Visible = false;
                PicBox.Dock = DockStyle.Fill;
                CylinderHeight3 = DrawGraphProtocol.CylinderHeight2;
                leftRightSpace = (PicBox.Width - (dataInterface.WeightOrSizeGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) -
                    DrawGraphProtocol.CylinderWidth) / 2;
            }
            else  //需要使用滚动条
            {
                bHaveHscrollBar = true;
                PicBox.Width = (dataInterface.WeightOrSizeGradeSum - 1) * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) +
                    DrawGraphProtocol.CylinderWidth + 2 * DrawGraphProtocol.LeftRightSpace;
                hScrollBar3.Maximum = PicBox.Width - tabPage3.Width;
                CylinderHeight3 = DrawGraphProtocol.CylinderHeight1;
            }
            #endregion

            //Bitmap bitM = new Bitmap(PicBox.Width, PicBox.Height);
            //Graphics g = Graphics.FromImage(bitM);
            if(bitM3 == null) //Modify by ChengSk - 20180815
            {
                bitM3 = new Bitmap(PicBox.Width, PicBox.Height);
                g3 = Graphics.FromImage(bitM3);
            }
            g3.Clear(DrawGraphProtocol.myBarBackColor);
            //定义画刷
            Brush currentBrush = new SolidBrush(DrawGraphProtocol.myPenColor);
            Brush BarBrush = new SolidBrush(DrawGraphProtocol.myBarBrush1);

            #region 数量/百分比 箱数统计信息
            //"数量百分比"字样
            //g.DrawString("数量/百分比", new Font(DrawGraphProtocol.Module1FontStyle, DrawGraphProtocol.Module1FontSize, FontStyle.Regular),
            //    currentBrush, DrawGraphProtocol.Module1LocationX, DrawGraphProtocol.Module1LocationY);
            //"尺寸统计信息"字样
            g3.DrawString(m_resourceManager.GetString("tabPage3.Text"), new Font(DrawGraphProtocol.Module3FontStyle, DrawGraphProtocol.Module3FontSize, FontStyle.Bold),
                currentBrush, PicBox.Width / 2 - DrawGraphProtocol.Module3MiddleDistance, PicBox.Height - DrawGraphProtocol.Module3BottomDistance);
            #endregion

            #region 尺寸名称 柱体 个数 百分比
            //"尺寸名称"字样  "柱体"图形显示  个数  百分比
            uMaxValue = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nBoxGradeCount);
            uSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);

            for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
            {
                if (bHaveHscrollBar)  //有滚动条时
                {
                    //"尺寸名称"字样
                    g3.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH),
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicBox.Height - DrawGraphProtocol.Module4BottomDistance);

                    if (dataInterface.IoStStatistics.nBoxGradeCount[i] == 0)
                    {
                        //"柱体"图形显示
                        g3.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g3.FillRectangle(BarBrush, DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight3 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight3 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue)));

                        if ((int)(CylinderHeight3 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g3.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g3.RotateTransform(-90f);
                            g3.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g3.ResetTransform();
                        }
                        else
                        {
                            g3.TranslateTransform(DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight3 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue)));
                            g3.RotateTransform(-90f);
                            g3.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[i].ToString(),
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
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g3.DrawString(((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uSumValue).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            DrawGraphProtocol.LeftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight3 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue)) - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    
                }
                else  //无滚动条时
                {
                    //"尺寸名称"字样
                    g3.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH),
                        new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular),
                        currentBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicBox.Height - DrawGraphProtocol.Module4BottomDistance);

                    if (dataInterface.IoStStatistics.nBoxGradeCount[i] == 0)
                    {
                        //"柱体"图形显示
                        g3.FillRectangle(BarBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.CylinderHeight4,
                        DrawGraphProtocol.CylinderWidth, DrawGraphProtocol.CylinderHeight4);
                    }
                    else
                    {
                        //"柱体"图形显示  个数
                        g3.FillRectangle(BarBrush, leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace),
                        PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight3 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue)),
                        DrawGraphProtocol.CylinderWidth, (int)(CylinderHeight3 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue)));

                        if ((int)(CylinderHeight3 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue)) > DrawGraphProtocol.StandardCylinderHeight)
                        {
                            g3.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance);
                            g3.RotateTransform(-90f);
                            g3.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[i].ToString(),
                                new Font(DrawGraphProtocol.Module7FontStyle, DrawGraphProtocol.Module7FontSize, FontStyle.Bold), currentBrush, 0, 0);
                            g3.ResetTransform();
                        }
                        else
                        {
                            g3.TranslateTransform(leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module7CylinderLeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module7CylinderBottomDistance-
                            (int)(CylinderHeight3 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue)));
                            g3.RotateTransform(-90f);
                            g3.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[i].ToString(),
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
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - DrawGraphProtocol.Module6CylinderDistance);
                    }
                    else
                    {
                        //百分比
                        g3.DrawString(((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uSumValue).ToString("0.00%"),
                            new Font(DrawGraphProtocol.Module6FontStyle, DrawGraphProtocol.Module6FontSize, FontStyle.Regular), currentBrush,
                            leftRightSpace + i * (DrawGraphProtocol.CylinderWidth + CurrentDisplayCylinderSpace) + DrawGraphProtocol.Module6LeftDistance,
                            PicBox.Height - DrawGraphProtocol.Module4BottomDistance - DrawGraphProtocol.Module5FontDistance - (int)(CylinderHeight3 * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue)) - DrawGraphProtocol.Module6CylinderDistance);
                    }                  
                }
            }
            #endregion

            PicBox.BackgroundImage = null;  //无此行代码不会刷新，必须加上 Add by ChengSk - 20180821
            PicBox.BackgroundImage = bitM3;
        }

        private void hScrollBar3_Scroll(object sender, ScrollEventArgs e)
        {
            PicBox.Left = -hScrollBar3.Value;
        }
        private void currentSelectTabpage4()
        {
            //LvwSizeData清空
            LvwSizeOrWeightData.Items.Clear();

            #region 变量声明
            UInt64 uMaxValue = 0;
            UInt64 uNumSumValue = 0;
            UInt64 uWeightSumValue = 0;
            UInt64 uBoxSumValue = 0;
            uMaxValue = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nWeightGradeCount);
            uNumSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);
            uWeightSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);
            uBoxSumValue = (ulong)FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);
            #endregion

            #region 往LvwSizeData插入数据
            for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
            {
                ListViewItem lv = new ListViewItem(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName,
                    i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH));
                lv.SubItems.Add(dataInterface.IoStStGradeInfo.grades[i].nMinSize.ToString());
                lv.SubItems.Add(dataInterface.IoStStatistics.nGradeCount[i].ToString());
                if (uNumSumValue == 0)
                {
                    lv.SubItems.Add("0.00%");
                }
                else
                {
                    lv.SubItems.Add(((double)dataInterface.IoStStatistics.nGradeCount[i] / uNumSumValue).ToString("0.00%"));
                }
                lv.SubItems.Add((dataInterface.IoStStatistics.nWeightGradeCount[i] / 1000.0).ToString("0.0"));
                if (uWeightSumValue == 0)
                {
                    lv.SubItems.Add("0.00%");
                }
                else
                {
                    lv.SubItems.Add(((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uWeightSumValue).ToString("0.00%"));
                }
                lv.SubItems.Add(dataInterface.IoStStatistics.nBoxGradeCount[i].ToString());
                if (uBoxSumValue == 0)
                {
                    lv.SubItems.Add("0.00%");
                }
                else
                {
                    lv.SubItems.Add(((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uBoxSumValue).ToString("0.00%"));
                }
                LvwSizeOrWeightData.Items.Add(lv);
            }
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
        private void currentSelectTabpage5()
        {
            //LvwFruitData清空
            LvwFruitData.Items.Clear();

            #region 变量声明
            UInt64 uMaxValue = 0;
            UInt64 uNumSumValue = 0;
            UInt64 uWeightSumValue = 0;
            UInt64 uBoxSumValue = 0;
            uMaxValue = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nWeightGradeCount);
            uNumSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);
            uWeightSumValue = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);
            uBoxSumValue = (ulong)FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);
            #endregion

            #region 往LvwFruitData插入数据
            for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
            {
                ListViewItem lv = new ListViewItem(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, 
                    i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH));
                lv.SubItems.Add(dataInterface.IoStStGradeInfo.grades[i].nMinSize.ToString());
                lv.SubItems.Add(dataInterface.IoStStatistics.nGradeCount[i].ToString());
                if (uNumSumValue == 0)
                {
                    lv.SubItems.Add("0.00%");
                }
                else
                {
                    lv.SubItems.Add(((double)dataInterface.IoStStatistics.nGradeCount[i] / uNumSumValue).ToString("0.00%"));
                }
                lv.SubItems.Add((dataInterface.IoStStatistics.nWeightGradeCount[i]/1000.0).ToString("0.0"));
                if (uWeightSumValue == 0)
                {
                    lv.SubItems.Add("0.00%");
                }
                else
                {
                    lv.SubItems.Add(((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uWeightSumValue).ToString("0.00%"));
                }
                lv.SubItems.Add(dataInterface.IoStStatistics.nBoxGradeCount[i].ToString());
                if (uBoxSumValue == 0)
                {
                    lv.SubItems.Add("0.00%");
                }
                else
                {
                    lv.SubItems.Add(((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uBoxSumValue).ToString("0.00%"));
                }
                LvwFruitData.Items.Add(lv);
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
        private void FillcurrentSelectTabpage5()
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

            if (LvwFruitData.Items.Count != dataInterface.QualityGradeSum * dataInterface.WeightOrSizeGradeSum)
                return;

            #region 往LvwFruitData插入数据
            
                for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
                {
                    //获取"特征名称"
                    //strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                    //strQualityName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                    //strMixName = strSizeName.Substring(0, strSizeName.IndexOf("\0")) + "." + strQualityName.Substring(0, strQualityName.IndexOf("\0"));
                    // LvwFruitData.Items[i * dataInterface.WeightOrSizeGradeSum + j].SubItems[1].Text = dataInterface.IoStStGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nMinSize.ToString();
                    LvwFruitData.Items[j].SubItems[2].Text = dataInterface.IoStStatistics.nGradeCount[j].ToString();

                    if (uNumSumValue == 0)
                    {
                        LvwFruitData.Items[j].SubItems[3].Text = "0.00%";
                    }
                    else
                    {
                        LvwFruitData.Items[j].SubItems[3].Text = ((double)dataInterface.IoStStatistics.nGradeCount[j] / uNumSumValue).ToString("0.00%");
                    }
                    LvwFruitData.Items[j].SubItems[4].Text = (dataInterface.IoStStatistics.nWeightGradeCount[j] / 1000.0).ToString("0.0");
                    if (uWeightSumValue == 0)
                    {
                        LvwFruitData.Items[j].SubItems[5].Text = "0.00%";
                    }
                    else
                    {
                        LvwFruitData.Items[j].SubItems[5].Text = ((double)dataInterface.IoStStatistics.nWeightGradeCount[j] / uWeightSumValue).ToString("0.00%");
                    }
                    LvwFruitData.Items[j].SubItems[6].Text = dataInterface.IoStStatistics.nBoxGradeCount[j].ToString();
                    if (uBoxSumValue == 0)
                    {
                        LvwFruitData.Items[j].SubItems[7].Text = "0.00%";
                    }
                    else
                    {
                        LvwFruitData.Items[j].SubItems[7].Text = ((double)dataInterface.IoStStatistics.nBoxGradeCount[j] / uBoxSumValue).ToString("0.00%");
                    }
                    //LvwFruitData.Items.Add(lv);
                }
            
            #endregion

        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        private void FillcurrentSelectTabpage4()
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

            if (LvwSizeOrWeightData.Items.Count != dataInterface.QualityGradeSum * dataInterface.WeightOrSizeGradeSum)
                return;

            #region 往LvwSizeData插入数据

            for (int j = 0; j < dataInterface.WeightOrSizeGradeSum; j++)
            {
                //获取"特征名称"
                //strSizeName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                //strQualityName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                //strMixName = strSizeName.Substring(0, strSizeName.IndexOf("\0")) + "." + strQualityName.Substring(0, strQualityName.IndexOf("\0"));
                // LvwSizeData.Items[i * dataInterface.WeightOrSizeGradeSum + j].SubItems[1].Text = dataInterface.IoStStGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nMinSize.ToString();
                LvwSizeOrWeightData.Items[j].SubItems[2].Text = dataInterface.IoStStatistics.nGradeCount[j].ToString();

                if (uNumSumValue == 0)
                {
                    LvwSizeOrWeightData.Items[j].SubItems[3].Text = "0.00%";
                }
                else
                {
                    LvwSizeOrWeightData.Items[j].SubItems[3].Text = ((double)dataInterface.IoStStatistics.nGradeCount[j] / uNumSumValue).ToString("0.00%");
                }
                LvwSizeOrWeightData.Items[j].SubItems[4].Text = (dataInterface.IoStStatistics.nWeightGradeCount[j] / 1000.0).ToString("0.0");
                if (uWeightSumValue == 0)
                {
                    LvwSizeOrWeightData.Items[j].SubItems[5].Text = "0.00%";
                }
                else
                {
                    LvwSizeOrWeightData.Items[j].SubItems[5].Text = ((double)dataInterface.IoStStatistics.nWeightGradeCount[j] / uWeightSumValue).ToString("0.00%");
                }
                LvwSizeOrWeightData.Items[j].SubItems[6].Text = dataInterface.IoStStatistics.nBoxGradeCount[j].ToString();
                if (uBoxSumValue == 0)
                {
                    LvwSizeOrWeightData.Items[j].SubItems[7].Text = "0.00%";
                }
                else
                {
                    LvwSizeOrWeightData.Items[j].SubItems[7].Text = ((double)dataInterface.IoStStatistics.nBoxGradeCount[j] / uBoxSumValue).ToString("0.00%");
                }
                //LvwSizeData.Items.Add(lv);
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
                    //MessageBox.Show("当前选中尺寸统计信息");
                    break;
                case 2:
                    currentSelectTabpage3();
                    //MessageBox.Show("当前选中箱数统计信息");
                    break;
                case 3:
                    currentSelectTabpage4();
                    //MessageBox.Show("当前选中尺寸统计表");
                    break;
                case 4:
                    currentSelectTabpage5();
                    //MessageBox.Show("当前选中等级统计表");
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
                    if (tabControl1.SelectedIndex >= 0 && tabControl1.SelectedIndex <=2)
                    {
                        SaveFileDialog dlg = new SaveFileDialog();
                        dlg.Filter = "JPG格式(*.jpg)|*.jpg|位图(*.bmp)|*.bmp|GIF格式(*.gif)|*.gif|PNG格式(*.png)|*.png";
                        dlg.RestoreDirectory = true;
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            switch (tabControl1.SelectedIndex)
                            {
                                case 0:
                                    PicExport.BackgroundImage.Save(dlg.FileName);
                                    break;
                                case 1:
                                    PicSize.BackgroundImage.Save(dlg.FileName);
                                    break;
                                case 2:
                                    PicBox.BackgroundImage.Save(dlg.FileName);
                                    break;
                                default:
                                    break;
                            }
                            MessageBox.Show(LanguageContainer.StatisticsInfoForm1Messagebox3Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.StatisticsInfoForm1MessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        return;
                    }
                }
                catch (Exception ex) { }

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

            //打印时间
            #region
            //currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
            currentAvailableHeight = 20;   //50->20 Modify by ChengSk - 20171124
            Font dateTimeFont = new Font("宋体", 15, FontStyle.Regular);
            Brush dateTimeBrush = Brushes.Black;
            string nowDateTime = DateTime.Now.ToString(m_resourceManager.GetString("LblPrintDateTime.Text"));
            PictureBox picB = new PictureBox();
            Graphics TitG = picB.CreateGraphics();
            SizeF XMaxSize = TitG.MeasureString(nowDateTime, dateTimeFont);
            e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                nowDateTime, dateTimeFont, dateTimeBrush, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize.Width, currentAvailableHeight);
            #endregion

            //打印LOGO
            #region
            //currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
            currentAvailableHeight += (int)XMaxSize.Height + 10;  //20->10 Modify by ChengSk - 20171124
            try
            {
                Bitmap bitmap = new Bitmap(PrintProtocol.logoPathName);//创建位图对象        
                e.Graphics.DrawImage(bitmap,
                    (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - bitmap.Width) / 2 + PrintProtocol.leftMargin, currentAvailableHeight, bitmap.Width, bitmap.Height);
                //currentAvailableHeight += bitmap.Height + PrintProtocol.logoOrTextTitleSpace;    //当前可用高度
                currentAvailableHeight += bitmap.Height + 15;     //30->15 Modify by ChengSk - 20171124
            }
            catch (Exception ee)//捕获异常
            {
                MessageBox.Show(ee.Message);//弹出消息对话框
            }
            #endregion

            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    #region 打印出口统计信息
                    //文本标题
                    #region
                    Font textTitleFont = new Font("宋体", 20, FontStyle.Bold);
                    Brush textTitleBrush = Brushes.Black;
                    string textTitle = m_resourceManager.GetString("LblPrintClassified.Text");
                    XMaxSize = TitG.MeasureString(textTitle, textTitleFont);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                        textTitle, textTitleFont, textTitleBrush, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize.Width) / 2 + PrintProtocol.leftMargin,
                        currentAvailableHeight);
                    #endregion

                    //文本内容
                    #region
                    currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                    Font textContentFont = new Font("宋体", 15, FontStyle.Regular);
                    Brush textContentBrush = Brushes.Black;
                    string textContent = m_resourceManager.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName +
                        "    " + m_resourceManager.GetString("LblPrintFarmName.Text") + dataInterface.FarmName +
                        "    " + m_resourceManager.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
                    XMaxSize = TitG.MeasureString(textContent, textContentFont);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                        textContent, textContentFont, textContentBrush, PrintProtocol.leftMargin, currentAvailableHeight);
                    #endregion

                    //分割线
                    #region
                    currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
                    Pen linePen = new Pen(Color.Black, 2);
                    e.Graphics.DrawLine(linePen, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                    #endregion

                    //条形图头部
                    #region
                    currentAvailableHeight += (int)linePen.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                    Font barHeadFont = new Font("楷体_GB2312", 15, FontStyle.Regular);
                    Brush barHeadBrush = Brushes.Black;
                    string barHead = m_resourceManager.GetString("LblPrintNumOrPercent.Text");
                    XMaxSize = TitG.MeasureString(barHead, barHeadFont);
                    e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                        barHead, barHeadFont, barHeadBrush, PrintProtocol.leftMargin, currentAvailableHeight);
                    #endregion

                    //直方图（或条形图）
                    #region
                    currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
                    //Bitmap bitM = new Bitmap(PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);
                    //Graphics g = Graphics.FromImage(bitM);
                    //Graphics g = e.Graphics;
                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                    //g.Clear(PrintProtocol.barBackColor);
                    Brush myCurrentBrush = new SolidBrush(PrintProtocol.penColor);
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
                        e.Graphics.DrawString((i + 1).ToString(), new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush,
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
                                    new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush, 0, 0);
                                e.Graphics.ResetTransform();
                                e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                            }
                            else
                            {
                                e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)) - PrintProtocol.cylinderDigitBottomDistance);
                                e.Graphics.RotateTransform(-90f);
                                e.Graphics.DrawString(dataInterface.IoStStatistics.nExitCount[i].ToString(),
                                    new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush, 0, 0);
                                e.Graphics.ResetTransform();
                                e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                            }
                        }
                        if (uSumValue == 0)
                        {
                            //百分比
                            e.Graphics.DrawString("0.00%", new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush,
                                cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight - PrintProtocol.cylinderPrecentBottomDistance);
                        }
                        else
                        {
                            //百分比
                            e.Graphics.DrawString(((double)dataInterface.IoStStatistics.nExitCount[i] / uSumValue).ToString("0.00%"),
                                new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush,
                                cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nExitCount[i] / uMaxValue)) - PrintProtocol.cylinderPrecentBottomDistance);
                        }
                    }
                    e.Graphics.ResetTransform();
                    //try
                    //{
                    //    e.Graphics.DrawImage(bitM, PrintProtocol.leftMargin, currentAvailableHeight, PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);

                    //}
                    //catch (Exception ee)//捕获异常
                    //{
                    //    MessageBox.Show(ee.Message);//弹出消息对话框
                    //}
                    #endregion

                    //条形图标题
                    #region
                    //currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + bitM.Height;   //当前可用高度
                    currentAvailableHeight += PrintProtocol.BarImageOrBarTitleSpace + PrintProtocol.barImageHeight;
                    Font barTitleFont = new Font("楷体_GB2312", 18, FontStyle.Bold);
                    Brush barTitleBrush = Brushes.Black;
                    string barTitle = m_resourceManager.GetString("tabPage1.Text");
                    XMaxSize = TitG.MeasureString(barTitle, barTitleFont);
                    e.Graphics.DrawString(//使用DrawString方法绘制条形图标题字符串
                        barTitle, barTitleFont, barTitleBrush, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize.Width) / 2 + PrintProtocol.leftMargin,
                        currentAvailableHeight);
                    #endregion
                    #endregion                 
                    break;
                case 1:
                    #region 打印尺寸统计信息
                    //文本标题
                    #region
                    Font textTitleFont1 = new Font("宋体", 20, FontStyle.Bold);
                    Brush textTitleBrush1 = Brushes.Black;
                    string textTitle1 = m_resourceManager.GetString("LblPrintClassified.Text");
                    XMaxSize = TitG.MeasureString(textTitle1, textTitleFont1);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                        textTitle1, textTitleFont1, textTitleBrush1, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize.Width) / 2 + PrintProtocol.leftMargin,
                        currentAvailableHeight);
                    #endregion

                    //文本内容
                    #region
                    currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                    Font textContentFont1 = new Font("宋体", 15, FontStyle.Regular);
                    Brush textContentBrush1 = Brushes.Black;
                    string textContent1 = m_resourceManager.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName +
                        "    " + m_resourceManager.GetString("LblPrintFarmName.Text")  + dataInterface.FarmName +
                        "    " + m_resourceManager.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
                    XMaxSize = TitG.MeasureString(textContent1, textContentFont1);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                        textContent1, textContentFont1, textContentBrush1, PrintProtocol.leftMargin, currentAvailableHeight);
                    #endregion

                    //分割线
                    #region
                    currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
                    Pen linePen1 = new Pen(Color.Black, 2);
                    e.Graphics.DrawLine(linePen1, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                    #endregion

                    //条形图头部
                    #region
                    currentAvailableHeight += (int)linePen1.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                    Font barHeadFont1 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                    Brush barHeadBrush1 = Brushes.Black;
                    string barHead1 = m_resourceManager.GetString("LblPrintNumOrPercent.Text");
                    XMaxSize = TitG.MeasureString(barHead1, barHeadFont1);
                    e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                        barHead1, barHeadFont1, barHeadBrush1, PrintProtocol.leftMargin, currentAvailableHeight);
                    #endregion

                    //直方图（或条形图）
                    #region
                    currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
                    //Bitmap bitM1 = new Bitmap(PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);
                    //Graphics g1 = Graphics.FromImage(bitM1);
                    //g1.Clear(PrintProtocol.barBackColor);
                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                    Brush myCurrentBrush1 = new SolidBrush(PrintProtocol.penColor);
                    Brush myBarBrush1 = new SolidBrush(PrintProtocol.barBrush);
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
                        cylinderWidth = PrintProtocol.cylinderWidth2;
                        cylinderSpace = PrintProtocol.cylinderSpace2;
                        cylinderLeftMargin = (PrintProtocol.barImageWidht - (dataInterface.WeightOrSizeGradeSum - 1) * (PrintProtocol.cylinderWidth2 + PrintProtocol.cylinderSpace2) - PrintProtocol.cylinderWidth2) / 2;
                        cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance2;
                    }
                    UInt64 uMaxValue1 = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nWeightGradeCount);
                    UInt64 uSumValue1 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);
                    for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
                    {
                        //柱形图下标字样
                        e.Graphics.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH), 
                            new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush1,
                            cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                            PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin + PrintProtocol.cylinderOrTextNote);
                        if (dataInterface.IoStStatistics.nWeightGradeCount[i] == 0)
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
                                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue1)),
                                cylinderWidth, (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue1)));
                            if ((int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue1)) > PrintProtocol.cylinderStandardCylinderHeigh)
                            {
                                e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderDigitBottomDistance);
                                e.Graphics.RotateTransform(-90f);
                                e.Graphics.DrawString(dataInterface.IoStStatistics.nWeightGradeCount[i].ToString(),
                                    new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush1, 0, 0);
                                e.Graphics.ResetTransform();
                                e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                            }
                            else
                            {
                                e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue1)) - PrintProtocol.cylinderDigitBottomDistance);
                                e.Graphics.RotateTransform(-90f);
                                e.Graphics.DrawString(dataInterface.IoStStatistics.nWeightGradeCount[i].ToString(),
                                    new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush1, 0, 0);
                                e.Graphics.ResetTransform();
                                e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                            }
                        }
                        if (uSumValue1 == 0)
                        {
                            //百分比
                            e.Graphics.DrawString("0.00%", new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush1,
                                cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderMinHeight - PrintProtocol.cylinderPrecentBottomDistance);
                        }
                        else
                        {
                            //百分比
                            e.Graphics.DrawString(((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uSumValue1).ToString("0.00%"),
                                new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush1,
                                cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nWeightGradeCount[i] / uMaxValue1)) - PrintProtocol.cylinderPrecentBottomDistance);
                        }
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
                    XMaxSize = TitG.MeasureString(barTitle1, barTitleFont1);
                    e.Graphics.DrawString(//使用DrawString方法绘制条形图标题字符串
                        barTitle1, barTitleFont1, barTitleBrush1, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize.Width) / 2 + PrintProtocol.leftMargin,
                        currentAvailableHeight);
                    #endregion
                    #endregion
                    break;
                case 2:
                    #region 打印箱数统计信息
                    //文本标题
                    #region
                    Font textTitleFont2 = new Font("宋体", 20, FontStyle.Bold);
                    Brush textTitleBrush2 = Brushes.Black;
                    string textTitle2 = m_resourceManager.GetString("LblPrintClassified.Text");
                    XMaxSize = TitG.MeasureString(textTitle2, textTitleFont2);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                        textTitle2, textTitleFont2, textTitleBrush2, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize.Width) / 2 + PrintProtocol.leftMargin,
                        currentAvailableHeight);
                    #endregion

                    //文本内容
                    #region
                    currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                    Font textContentFont2 = new Font("宋体", 15, FontStyle.Regular);
                    Brush textContentBrush2 = Brushes.Black;
                    string textContent2 = m_resourceManager.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName +
                        "    " + m_resourceManager.GetString("LblPrintFarmName.Text")  + dataInterface.FarmName +
                        "    " + m_resourceManager.GetString("LblPrintFruitVarieties.Text")  + dataInterface.FruitName;
                    XMaxSize = TitG.MeasureString(textContent2, textContentFont2);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                        textContent2, textContentFont2, textContentBrush2, PrintProtocol.leftMargin, currentAvailableHeight);
                    #endregion

                    //分割线
                    #region
                    currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
                    Pen linePen2 = new Pen(Color.Black, 2);
                    e.Graphics.DrawLine(linePen2, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                    #endregion

                    //条形图头部
                    #region
                    currentAvailableHeight += (int)linePen2.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                    Font barHeadFont2 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                    Brush barHeadBrush2 = Brushes.Black;
                    string barHead2 = m_resourceManager.GetString("LblPrintNumOrPercent.Text");
                    XMaxSize = TitG.MeasureString(barHead2, barHeadFont2);
                    e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                        barHead2, barHeadFont2, barHeadBrush2, PrintProtocol.leftMargin, currentAvailableHeight);
                    #endregion

                    //直方图（或条形图）
                    #region
                    currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
                    //Bitmap bitM2 = new Bitmap(PrintProtocol.barImageWidht, PrintProtocol.barImageHeight);
                    //Graphics g2 = Graphics.FromImage(bitM2);
                    //g2.Clear(PrintProtocol.barBackColor);
                    e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                    Brush myCurrentBrush2 = new SolidBrush(PrintProtocol.penColor);
                    Brush myBarBrush2 = new SolidBrush(PrintProtocol.barBrush);
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
                        cylinderWidth = PrintProtocol.cylinderWidth2;
                        cylinderSpace = PrintProtocol.cylinderSpace2;
                        cylinderLeftMargin = (PrintProtocol.barImageWidht - (dataInterface.WeightOrSizeGradeSum - 1) * (PrintProtocol.cylinderWidth2 + PrintProtocol.cylinderSpace2) - PrintProtocol.cylinderWidth2) / 2;
                        cylinderDigitLeftDistance = PrintProtocol.cylinderDigitLeftDistance1;
                    }
                    Int32 uMaxValue2 = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nBoxGradeCount);
                    Int32 uSumValue2 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);
                    for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
                    {
                        //柱形图下标字样
                        e.Graphics.DrawString(Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH), 
                            new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush2,
                            cylinderLeftMargin + i * (cylinderWidth + cylinderSpace),
                            PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin + PrintProtocol.cylinderOrTextNote);
                        if (dataInterface.IoStStatistics.nBoxGradeCount[i] == 0)
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
                                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue2)),
                                cylinderWidth, (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue2)));
                            if ((int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue2)) > PrintProtocol.cylinderStandardCylinderHeigh)
                            {
                                e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - PrintProtocol.cylinderDigitBottomDistance);
                                e.Graphics.RotateTransform(-90f);
                                e.Graphics.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[i].ToString(),
                                    new Font("Times New Roman", 10, FontStyle.Bold), myCurrentBrush2, 0, 0);
                                e.Graphics.ResetTransform();
                                e.Graphics.TranslateTransform(PrintProtocol.leftMargin, currentAvailableHeight);
                            }
                            else
                            {
                                e.Graphics.TranslateTransform(cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + cylinderDigitLeftDistance,
                                    PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue2)) - PrintProtocol.cylinderDigitBottomDistance);
                                e.Graphics.RotateTransform(-90f);
                                e.Graphics.DrawString(dataInterface.IoStStatistics.nBoxGradeCount[i].ToString(),
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
                            e.Graphics.DrawString(((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uSumValue2).ToString("0.00%"),
                                new Font("Times New Roman", 10, FontStyle.Regular), myCurrentBrush2,
                                cylinderLeftMargin + i * (cylinderWidth + cylinderSpace) + PrintProtocol.cylinderPrecentLeftDistance,
                                PrintProtocol.barImageHeight - PrintProtocol.cylinderBottomMargin - (int)(PrintProtocol.cylinderMaxHeight * ((double)dataInterface.IoStStatistics.nBoxGradeCount[i] / uMaxValue2)) - PrintProtocol.cylinderPrecentBottomDistance);
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
                    XMaxSize = TitG.MeasureString(barTitle2, barTitleFont2);
                    e.Graphics.DrawString(//使用DrawString方法绘制条形图标题字符串
                        barTitle2, barTitleFont2, barTitleBrush2, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize.Width) / 2 + PrintProtocol.leftMargin,
                        currentAvailableHeight);
                    #endregion
                    #endregion
                    break;
                case 3:
                case 4:
                    #region 打印      统计表
                    //文本标题
                    #region
                    Font textTitleFont3 = new Font("宋体", 20, FontStyle.Bold);
                    Brush textTitleBrush3 = Brushes.Black;
                    string textTitle3 = m_resourceManager.GetString("LblPrintBatchReport.Text");
                    XMaxSize = TitG.MeasureString(textTitle3, textTitleFont3);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                        textTitle3, textTitleFont3, textTitleBrush3, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize.Width) / 2 + PrintProtocol.leftMargin,
                        currentAvailableHeight);
                    #endregion

                    //分割线1
                    #region
                    //currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                    currentAvailableHeight += (int)XMaxSize.Height + 10;   //20->10 Modify by ChengSk - 20171124
                    Pen linePen31 = new Pen(Color.Black, 2);
                    e.Graphics.DrawLine(linePen31, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                    #endregion

                    //分割线2
                    #region
                    currentAvailableHeight += (int)linePen31.Width+PrintProtocol.lineOrLineSpace; //当前可用高度
                    Pen linePen32 = new Pen(Color.Black, 2);
                    e.Graphics.DrawLine(linePen32, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                    #endregion

                    //文本内容
                    #region
                    //currentAvailableHeight += (int)linePen32.Width + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                    currentAvailableHeight += (int)linePen32.Width + 10;   //20->10 Modify by ChengSk - 20171124
                    Font textContentFont3 = new Font("宋体", 15, FontStyle.Regular);
                    Brush textContentBrush3 = Brushes.Black;
                    int sumBoxNum = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);  //总箱数
                    string textContent31 = m_resourceManager.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName;  
                    XMaxSize = TitG.MeasureString(textContent31, textContentFont3);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                        textContent31, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                    string textContent32 = m_resourceManager.GetString("LblPrintFarmName.Text")  + dataInterface.FarmName;
                    XMaxSize = TitG.MeasureString(textContent32, textContentFont3);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                        textContent32, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                    string textContent33 = m_resourceManager.GetString("LblPrintFruitVarieties.Text")  + dataInterface.FruitName;
                     XMaxSize = TitG.MeasureString(textContent33, textContentFont3);
                     e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                         textContent33, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                    currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
                    string textContent34 = m_resourceManager.GetString("LblPrintTotalPieces.Text") + dataInterface.IoStStatistics.nTotalCount;
                    XMaxSize = TitG.MeasureString(textContent34, textContentFont3);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                        textContent34, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                    string textContent35 = m_resourceManager.GetString("LblPrintTotalWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / 1000000).ToString() + " " +
                        m_resourceManager.GetString("LblPrintTName.Text");
                    XMaxSize = TitG.MeasureString(textContent35, textContentFont3);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                        textContent35, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                   // currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
                    string textContent36 = m_resourceManager.GetString("LblPrintTotalCartons.Text") + sumBoxNum;
                    XMaxSize = TitG.MeasureString(textContent36, textContentFont3);
                    //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                       textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                    currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度 2015-11-5 ivycc
                    string textContent37 = "";
                    if (dataInterface.IoStStatistics.nTotalCount == 0)
                    {
                        textContent37 = m_resourceManager.GetString("LblPrintAveFruitWeight.Text") + "0.000 " + m_resourceManager.GetString("LblPrintGName.Text");
                    }
                    else
                    {
                        textContent37 = m_resourceManager.GetString("LblPrintAveFruitWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / dataInterface.IoStStatistics.nTotalCount).ToString("0.000") +
                            " "+m_resourceManager.GetString("LblPrintGName.Text");
                    }
                    XMaxSize = TitG.MeasureString(textContent37, textContentFont3);
                    //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    //    textContent37, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                        textContent37, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                    string textContent38 = m_resourceManager.GetString("LblPrintProgramName.Text") + dataInterface.ProgramName;
                    XMaxSize = TitG.MeasureString(textContent38, textContentFont3);
                    //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                       textContent38, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin +  PrintProtocol.textContentItemsWidth2, currentAvailableHeight);
                    string textContent41 = m_resourceManager.GetString("LblCustomerID.Text") + GlobalDataInterface.SerialNum;
                    XMaxSize = TitG.MeasureString(textContent41, textContentFont3);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                       textContent41, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2 * 2, currentAvailableHeight);
                    //add by xcw 20200701

                    currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度 2015-12-7 ivycc
                    string textContent39 = m_resourceManager.GetString("LblExcelStartTime.Text") + dataInterface.StartTime;
                    XMaxSize = TitG.MeasureString(textContent39, textContentFont3);
                    //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                       textContent39, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                    string textContent40 = m_resourceManager.GetString("LblExcelEndTime.Text") + dataInterface.EndTime;
                    XMaxSize = TitG.MeasureString(textContent40, textContentFont3);
                    //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                    e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                       textContent40, textContentFont3, textContentBrush3, e.PageBounds.Width / 2/*PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2*/, currentAvailableHeight);//add by xcw 20201020
                    #endregion

                    //分割线3
                    #region
                    //currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
                    currentAvailableHeight += (int)XMaxSize.Height + 10;   //25->10 Modify by ChengSk - 20171124
                    Pen linePen33 = new Pen(Color.Black, 2);
                    e.Graphics.DrawLine(linePen33, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                    #endregion

                    //分割线4
                    #region
                    currentAvailableHeight += (int)linePen31.Width + PrintProtocol.lineOrLineSpace; //当前可用高度
                    Pen linePen34 = new Pen(Color.Black, 2);
                    e.Graphics.DrawLine(linePen34, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
                    #endregion

                    ////汇总图头部
                    //#region
                    //currentAvailableHeight += (int)linePen34.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                    //Font sumImageHeadFont = new Font("楷体_GB2312", 15, FontStyle.Regular);
                    //Brush sumImagerHeadBrush = Brushes.Black;
                    //string sumImageHead = m_resourceManager.GetString("LblPrintQualityName.Text");
                    //XMaxSize = TitG.MeasureString(sumImageHead, sumImageHeadFont);
                    //e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                    //    sumImageHead, sumImageHeadFont, sumImagerHeadBrush, PrintProtocol.leftMargin, currentAvailableHeight);
                    //#endregion

                    //汇总表格
                    #region
                    //currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
                    //currentAvailableHeight += (int)linePen34.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
                    currentAvailableHeight += (int)linePen34.Width + 10;    //25->10 Modify by ChengSk - 20171124
                    //sumTableHeight = (dataInterface.WeightOrSizeGradeSum <= 10 ? PrintProtocol.sumTableHeight1 : PrintProtocol.sumTableHeight2); //汇总表格高度
                    sumTableHeight = PrintProtocol.sumTableHeight3; 
                    sumTalbeWidth = (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin) / 8;
                    Pen linePen35 = new Pen(Color.Black, 1);
                    Font sumTableFont;
                    if (GlobalDataInterface.selectLanguage == "en")
                        sumTableFont =new Font("Times New Roman", 9, FontStyle.Regular);
                    else
                        sumTableFont = new Font("宋体", 11, FontStyle.Regular);
                        //sumTableFont = (dataInterface.WeightOrSizeGradeSum <= 10 ? (new Font("宋体", 12, FontStyle.Regular)) : (new Font("宋体", 8, FontStyle.Regular)));
                    Brush sumTableBrush = Brushes.Black;
                    for (int i = 0; i < dataInterface.WeightOrSizeGradeSum + 2; i++) //画横线
                    {  
                        e.Graphics.DrawLine(linePen35, PrintProtocol.leftMargin, currentAvailableHeight + i * sumTableHeight,
                            e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + i * sumTableHeight);
                    }
                    e.Graphics.DrawLine(linePen35, PrintProtocol.leftMargin, currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight,
                            e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight);
                    for (int i = 0; i < 8; i++) //画竖线
                    {                 
                        e.Graphics.DrawLine(linePen35, PrintProtocol.leftMargin+i*sumTalbeWidth, currentAvailableHeight,
                            PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight);
                    }
                    e.Graphics.DrawLine(linePen35, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight,
                            e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight);
                    //表格标题行
                    string tablesFirstItems0 = m_resourceManager.GetString("LblMainReportName.Text");
                    XMaxSize = TitG.MeasureString(tablesFirstItems0, sumTableFont);
                    e.Graphics.DrawString(tablesFirstItems0, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                        currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesFirstItems1 = m_resourceManager.GetString("LblMainReportSize.Text");
                    XMaxSize = TitG.MeasureString(tablesFirstItems1, sumTableFont);
                    e.Graphics.DrawString(tablesFirstItems1, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                        currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesFirstItems2 = m_resourceManager.GetString("LblMainReportPieces.Text");
                    XMaxSize = TitG.MeasureString(tablesFirstItems2, sumTableFont);
                    e.Graphics.DrawString(tablesFirstItems2, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                        currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesFirstItems3 = m_resourceManager.GetString("LblMainReportPiecesPer.Text");
                    XMaxSize = TitG.MeasureString(tablesFirstItems3, sumTableFont);
                    e.Graphics.DrawString(tablesFirstItems3, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                        currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesFirstItems4 = m_resourceManager.GetString("LblMainReportWeights.Text")+"(kg)";
                    XMaxSize = TitG.MeasureString(tablesFirstItems4, sumTableFont);
                    e.Graphics.DrawString(tablesFirstItems4, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                        currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesFirstItems5 = m_resourceManager.GetString("LblMainReportWeightPer.Text");
                    XMaxSize = TitG.MeasureString(tablesFirstItems5, sumTableFont);
                    e.Graphics.DrawString(tablesFirstItems5, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                        currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesFirstItems6 = m_resourceManager.GetString("LblMainReportCartons.Text");
                    XMaxSize = TitG.MeasureString(tablesFirstItems6, sumTableFont);
                    e.Graphics.DrawString(tablesFirstItems6, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                        currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesFirstItems7 = m_resourceManager.GetString("LblMainReportCartonsPer.Text");
                    XMaxSize = TitG.MeasureString(tablesFirstItems7, sumTableFont);
                    e.Graphics.DrawString(tablesFirstItems7, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                        currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    UInt64 uMaxValue3 = FunctionInterface.GetMaxValue(dataInterface.IoStStatistics.nWeightGradeCount); //最大个数
                    UInt64 uSumValue3 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount); //总个数
                    UInt64 uSumWeightValue3 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount); //总重量
                    Int32 uSumBoxValue3 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);  //总箱数
                    for (int i = 1; i < dataInterface.WeightOrSizeGradeSum + 1; i++) //中间dataInterface.WeightOrSizeGradeSum行
                    {
                        string strName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName,
                            (i-1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH).ToString();
                        string tablesMiddleItems0 = strName.Substring(0, strName.IndexOf("\0"));
                        XMaxSize = TitG.MeasureString(tablesMiddleItems0, sumTableFont);
                        e.Graphics.DrawString(tablesMiddleItems0, sumTableFont, sumTableBrush,
                            (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                            currentAvailableHeight + (i+1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems1 = dataInterface.IoStStGradeInfo.grades[i-1].nMinSize.ToString();
                        XMaxSize = TitG.MeasureString(tablesMiddleItems1, sumTableFont);
                        e.Graphics.DrawString(tablesMiddleItems1, sumTableFont, sumTableBrush,
                            (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                            currentAvailableHeight + (i + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems2 = dataInterface.IoStStatistics.nGradeCount[i-1].ToString();
                        XMaxSize = TitG.MeasureString(tablesMiddleItems2, sumTableFont);
                        e.Graphics.DrawString(tablesMiddleItems2, sumTableFont, sumTableBrush,
                            (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                            currentAvailableHeight + (i + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems3 = "";
                        if (uSumValue3 == 0)
                        {
                            tablesMiddleItems3 = "0.000%";
                        }
                        else
                        {
                            tablesMiddleItems3 = ((double)dataInterface.IoStStatistics.nGradeCount[i-1] / uSumValue3).ToString("0.000%");
                        }
                        XMaxSize = TitG.MeasureString(tablesMiddleItems3, sumTableFont);
                        e.Graphics.DrawString(tablesMiddleItems3, sumTableFont, sumTableBrush,
                            (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                            currentAvailableHeight + (i + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems4 = (dataInterface.IoStStatistics.nWeightGradeCount[i - 1] / 1000.0).ToString("0.0");
                        XMaxSize = TitG.MeasureString(tablesMiddleItems4, sumTableFont);
                        e.Graphics.DrawString(tablesMiddleItems4, sumTableFont, sumTableBrush,
                            (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                            currentAvailableHeight + (i + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems5 = "";
                        if (uSumWeightValue3 == 0)
                        {
                            tablesMiddleItems5 = "0.000%";
                        }
                        else
                        {
                            tablesMiddleItems5 = ((double)dataInterface.IoStStatistics.nWeightGradeCount[i - 1] / uSumWeightValue3).ToString("0.000%");
                        }
                        XMaxSize = TitG.MeasureString(tablesMiddleItems5, sumTableFont);
                        e.Graphics.DrawString(tablesMiddleItems5, sumTableFont, sumTableBrush,
                            (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                            currentAvailableHeight + (i + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems6 = dataInterface.IoStStatistics.nBoxGradeCount[i - 1].ToString(); ;
                        XMaxSize = TitG.MeasureString(tablesMiddleItems6, sumTableFont);
                        e.Graphics.DrawString(tablesMiddleItems6, sumTableFont, sumTableBrush,
                            (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                            currentAvailableHeight + (i + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems7 = "";
                        if (uSumBoxValue3 == 0)
                        {
                            tablesMiddleItems7 = "0.000%";
                        }
                        else
                        {
                            tablesMiddleItems7 = ((double)dataInterface.IoStStatistics.nBoxGradeCount[i - 1] / uSumBoxValue3).ToString("0.000%");
                        }
                        XMaxSize = TitG.MeasureString(tablesMiddleItems7, sumTableFont);
                        e.Graphics.DrawString(tablesMiddleItems7, sumTableFont, sumTableBrush,
                            (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                            currentAvailableHeight + (i + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    }
                    //表格最后一行
                    string tablesLastItems0 = m_resourceManager.GetString("LblPrintSubTotal.Text");
                    XMaxSize = TitG.MeasureString(tablesLastItems0, sumTableFont);
                    e.Graphics.DrawString(tablesLastItems0, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                        currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems1 = "";
                    XMaxSize = TitG.MeasureString(tablesLastItems1, sumTableFont);
                    e.Graphics.DrawString(tablesLastItems1, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                        currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems2 = uSumValue3.ToString();
                     XMaxSize = TitG.MeasureString(tablesLastItems2, sumTableFont);
                    e.Graphics.DrawString(tablesLastItems2, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                        currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems3 = (uSumValue3 == 0 ? "0.000%" : "100.000%");
                    XMaxSize = TitG.MeasureString(tablesLastItems3, sumTableFont);
                    e.Graphics.DrawString(tablesLastItems3, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                        currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems4 = (uSumWeightValue3 / 1000.0).ToString("0.0");
                     XMaxSize = TitG.MeasureString(tablesLastItems4, sumTableFont);
                    e.Graphics.DrawString(tablesLastItems4, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                        currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems5 = (uSumWeightValue3 == 0 ? "0.000%" : "100.000%");
                    XMaxSize = TitG.MeasureString(tablesLastItems5, sumTableFont);
                    e.Graphics.DrawString(tablesLastItems5, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                        currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems6 = uSumBoxValue3.ToString();
                    XMaxSize = TitG.MeasureString(tablesLastItems6, sumTableFont);
                    e.Graphics.DrawString(tablesLastItems6, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                        currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems7 = (uSumBoxValue3 == 0 ? "0.000%" : "100.000%");
                    XMaxSize = TitG.MeasureString(tablesLastItems7, sumTableFont);
                    e.Graphics.DrawString(tablesLastItems7, sumTableFont, sumTableBrush,
                        (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                        currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                    #endregion

                    ////三维直方图
                    //#region
                    //currentAvailableHeight += (dataInterface.WeightOrSizeGradeSum+2)*sumTableHeight+PrintProtocol.TableOr3DImageSpace;  //当前可用高度
                    //String[] sName = new String[dataInterface.WeightOrSizeGradeSum]; //等级名称
                    //float[] fData = new float[dataInterface.WeightOrSizeGradeSum];  //等级对应的数据
                    //string sTempName = "";
                    //for (int i = 0; i < dataInterface.WeightOrSizeGradeSum; i++)
                    //{
                    //    sTempName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName, 
                    //        i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH).ToString();
                    //    sName[i] = sTempName.Substring(0, sTempName.IndexOf("\0"));
                    //    fData[i] = dataInterface.IoStStatistics.nWeightGradeCount[i];
                    //}
                    //BarGraphClass.DrawBarImage(e.Graphics, PrintProtocol.leftMargin - 1, currentAvailableHeight, 
                    //    sName, fData, dataInterface.WeightOrSizeGradeSum, PrintProtocol.draw3DImageWidth, PrintProtocol.draw3DImageHeight);
                    //Bitmap bitM3 = new Bitmap(PrintProtocol.draw3DImageWidth, PrintProtocol.draw3DImageHeight);
                    //bitM3 = BarGraphClass.DrawBarImage(sName, fData, dataInterface.WeightOrSizeGradeSum, PrintProtocol.draw3DImageWidth, PrintProtocol.draw3DImageHeight);
                    //try
                    //{
                    //    e.Graphics.DrawImage(bitM3, PrintProtocol.leftMargin - 1, currentAvailableHeight, PrintProtocol.draw3DImageWidth, PrintProtocol.draw3DImageHeight);

                    //}
                    //catch (Exception ee)//捕获异常
                    //{
                    //    MessageBox.Show(ee.Message);//弹出消息对话框
                    //}
                    //#endregion
                    #endregion
                    break;
                default:
                    break;
            }

            //打印页数
            #region
            currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
            Font currentPagesFont = new Font("宋体", 12, FontStyle.Regular);
            Brush currentPagesBrush = Brushes.Black;
            string currentPages = m_resourceManager.GetString("LblPrintPages.Text") + " 1";
            XMaxSize = TitG.MeasureString(currentPages, currentPagesFont);
            e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                currentPages, currentPagesFont, currentPagesBrush, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize.Width) / 2 + PrintProtocol.leftMargin,
                currentAvailableHeight);
            #endregion
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

        private void StatisticsInfoForm1_SizeChanged(object sender, EventArgs e)
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
                    DialogResult result = MessageBox.Show("0x30001021 " + LanguageContainer.StatisticsInfoForm1Messagebox1Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.StatisticsInfoForm1MessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
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
                ExcelReportFunc.CreateExcel(dlg.FileName, dataInterface, resource,false, true);
                //MessageBox.Show("Export excel report successfully!");
                MessageBox.Show(LanguageContainer.StatisticsInfoForm1Messagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.StatisticsInfoForm1MessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
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

        private void StatisticsInfoForm1_FormClosing(object sender, FormClosingEventArgs e)
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
            }
            catch(Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("StatisticsInfoForm1_FormClosing出错: " + ex.StackTrace);
#endif
            }
        }

    }
}
