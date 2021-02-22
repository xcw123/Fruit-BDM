using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Interface;
using Draw3DBarGraph;
using DrawPieGraph;
using System.Resources;

namespace FruitSortingVtest1.DB
{
    public class PrintOperate
    {
        public int intPage = 0;             //打印总页数
        public int currentPageIndex = 1;    //当前打印页
        public Boolean bIsHaveColorStatis;  //是否有颜色统计
        public Boolean bIsTwoQualityOnePage;//是否每页最多可放两个品质
        public Boolean bLastPageHaveQuality;//最后一页是否还有品质

        private ResourceManager m_resourceManager1 = new ResourceManager(typeof(StatisticsInfoForm1));
        private ResourceManager m_resourceManager2 = new ResourceManager(typeof(StatisticsInfoForm2));
        private ResourceManager m_resourceManager3 = new ResourceManager(typeof(StatisticsInfoForm3));
        private ResourceManager m_resourceManager4 = new ResourceManager(typeof(StatisticsInfoForm4));

        public PrintOperate()
        {
            intPage = 0;
            currentPageIndex = 1;
            bIsHaveColorStatis = false;
            bIsTwoQualityOnePage = false;
            bLastPageHaveQuality = false;
        }

        //打印尺寸模块
        public void printSize_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e, DataInterface dataInterface)
        {
            #region
            //打印定义
            #region
            int currentAvailableHeight = 0;   //当前可用高度

            //int cylinderWidth = 0;            //条柱宽度
            //int cylinderSpace = 0;            //圆柱间间距
            //int cylinderLeftMargin = 0;       //条柱左边距

            int sumTableHeight = 0;      //汇总表格高度
            int sumTalbeWidth = 0;       //汇总表格宽度

            //int cylinderDigitLeftDistance = 0;//数字左边框
            #endregion

            //打印时间
            #region
            currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
            Font dateTimeFont = new Font("宋体", 15, FontStyle.Regular);
            Brush dateTimeBrush = Brushes.Black;
            string nowDateTime = DateTime.Now.ToString(m_resourceManager1.GetString("LblPrintDateTime.Text"));
            PictureBox picB = new PictureBox();
            Graphics TitG = picB.CreateGraphics();
            SizeF XMaxSize = TitG.MeasureString(nowDateTime, dateTimeFont);
            e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                nowDateTime, dateTimeFont, dateTimeBrush, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize.Width, currentAvailableHeight);
            #endregion

            //打印LOGO
            #region
            currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
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

            //打印统计表
            #region 打印统计表
            //文本标题
            #region
            Font textTitleFont3 = new Font("宋体", 20, FontStyle.Bold);
            Brush textTitleBrush3 = Brushes.Black;
            string textTitle3 = m_resourceManager1.GetString("LblPrintBatchReport.Text");
            XMaxSize = TitG.MeasureString(textTitle3, textTitleFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                textTitle3, textTitleFont3, textTitleBrush3, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize.Width) / 2 + PrintProtocol.leftMargin,
                currentAvailableHeight);
            #endregion

            //分割线1
            #region
            currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
            Pen linePen31 = new Pen(Color.Black, 2);
            e.Graphics.DrawLine(linePen31, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
            #endregion

            //分割线2
            #region
            currentAvailableHeight += (int)linePen31.Width + PrintProtocol.lineOrLineSpace; //当前可用高度
            Pen linePen32 = new Pen(Color.Black, 2);
            e.Graphics.DrawLine(linePen32, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
            #endregion

            //文本内容
            #region
            currentAvailableHeight += (int)linePen32.Width + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
            Font textContentFont3 = new Font("宋体", 15, FontStyle.Regular);
            Brush textContentBrush3 = Brushes.Black;
            int sumBoxNum = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);  //总箱数
            string textContent31 = m_resourceManager1.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName;
            XMaxSize = TitG.MeasureString(textContent31, textContentFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent31, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            string textContent32 = m_resourceManager1.GetString("LblPrintFarmName.Text") + dataInterface.FarmName;
            XMaxSize = TitG.MeasureString(textContent32, textContentFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent32, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
            string textContent33 = m_resourceManager1.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
            XMaxSize = TitG.MeasureString(textContent33, textContentFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent33, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
            currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
            string textContent34 = m_resourceManager1.GetString("LblPrintTotalPieces.Text") + dataInterface.IoStStatistics.nTotalCount;
            XMaxSize = TitG.MeasureString(textContent34, textContentFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent34, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            string textContent35 = m_resourceManager1.GetString("LblPrintTotalWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / 1000000).ToString() +
                " " + m_resourceManager1.GetString("LblPrintTName.Text");
            XMaxSize = TitG.MeasureString(textContent35, textContentFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent35, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
            // currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
            string textContent36 = m_resourceManager1.GetString("LblPrintTotalCartons.Text") + sumBoxNum;
            XMaxSize = TitG.MeasureString(textContent36, textContentFont3);
            //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
            //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
               textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
            currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度 2015-11-5 ivycc
            string textContent37 = "";
            if (dataInterface.IoStStatistics.nTotalCount == 0)
            {
                textContent37 = m_resourceManager1.GetString("LblPrintAveFruitWeight.Text") + "0.000 " + m_resourceManager1.GetString("LblPrintGName.Text");
            }
            else
            {
                textContent37 = m_resourceManager1.GetString("LblPrintAveFruitWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / dataInterface.IoStStatistics.nTotalCount).ToString("0.000") +
                    " " + m_resourceManager1.GetString("LblPrintGName.Text");
            }
            XMaxSize = TitG.MeasureString(textContent37, textContentFont3);
            //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
            //    textContent37, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent37, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            string textContent38 = m_resourceManager1.GetString("LblPrintProgramName.Text") + dataInterface.ProgramName;
            XMaxSize = TitG.MeasureString(textContent38, textContentFont3);
            //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
            //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
               textContent38, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2, currentAvailableHeight);

            currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度 2015-12-7 ivycc
            string textContent39 = m_resourceManager1.GetString("LblExcelStartTime.Text") + dataInterface.StartTime;
            XMaxSize = TitG.MeasureString(textContent39, textContentFont3);
            //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
            //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
               textContent39, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            string textContent40 = m_resourceManager1.GetString("LblExcelEndTime.Text") + dataInterface.EndTime;
            XMaxSize = TitG.MeasureString(textContent40, textContentFont3);
            //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
            //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
               textContent40, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2, currentAvailableHeight);
            #endregion

            //分割线3
            #region
            currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
            Pen linePen33 = new Pen(Color.Black, 2);
            e.Graphics.DrawLine(linePen33, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
            #endregion

            //分割线4
            #region
            currentAvailableHeight += (int)linePen31.Width + PrintProtocol.lineOrLineSpace; //当前可用高度
            Pen linePen34 = new Pen(Color.Black, 2);
            e.Graphics.DrawLine(linePen34, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
            #endregion

            //汇总表格
            #region
            //currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
            currentAvailableHeight += (int)linePen34.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
            //sumTableHeight = (dataInterface.WeightOrSizeGradeSum <= 10 ? PrintProtocol.sumTableHeight1 : PrintProtocol.sumTableHeight2); //汇总表格高度
            sumTableHeight = PrintProtocol.sumTableHeight3;
            sumTalbeWidth = (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin) / 8;
            Pen linePen35 = new Pen(Color.Black, 1);
            Font sumTableFont;
            if (GlobalDataInterface.selectLanguage == "en")
                sumTableFont = new Font("Times New Roman", 9, FontStyle.Regular);
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
                e.Graphics.DrawLine(linePen35, PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight,
                    PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight);
            }
            e.Graphics.DrawLine(linePen35, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight,
                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight);
            //表格标题行
            string tablesFirstItems0 = m_resourceManager1.GetString("LblMainReportName.Text");
            XMaxSize = TitG.MeasureString(tablesFirstItems0, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems0, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems1 = m_resourceManager1.GetString("LblMainReportSize.Text");
            XMaxSize = TitG.MeasureString(tablesFirstItems1, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems1, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems2 = m_resourceManager1.GetString("LblMainReportPieces.Text");
            XMaxSize = TitG.MeasureString(tablesFirstItems2, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems2, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems3 = m_resourceManager1.GetString("LblMainReportPiecesPer.Text");
            XMaxSize = TitG.MeasureString(tablesFirstItems3, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems3, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems4 = m_resourceManager1.GetString("LblMainReportWeights.Text") + "(kg)";
            XMaxSize = TitG.MeasureString(tablesFirstItems4, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems4, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems5 = m_resourceManager1.GetString("LblMainReportWeightPer.Text");
            XMaxSize = TitG.MeasureString(tablesFirstItems5, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems5, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems6 = m_resourceManager1.GetString("LblMainReportCartons.Text");
            XMaxSize = TitG.MeasureString(tablesFirstItems6, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems6, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems7 = m_resourceManager1.GetString("LblMainReportCartonsPer.Text");
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
                    (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH).ToString();
                string tablesMiddleItems0 = strName.Substring(0, strName.IndexOf("\0"));
                XMaxSize = TitG.MeasureString(tablesMiddleItems0, sumTableFont);
                e.Graphics.DrawString(tablesMiddleItems0, sumTableFont, sumTableBrush,
                    (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                    currentAvailableHeight + (i + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                string tablesMiddleItems1 = dataInterface.IoStStGradeInfo.grades[i - 1].nMinSize.ToString();
                XMaxSize = TitG.MeasureString(tablesMiddleItems1, sumTableFont);
                e.Graphics.DrawString(tablesMiddleItems1, sumTableFont, sumTableBrush,
                    (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                    currentAvailableHeight + (i + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                string tablesMiddleItems2 = dataInterface.IoStStatistics.nGradeCount[i - 1].ToString();
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
                    tablesMiddleItems3 = ((double)dataInterface.IoStStatistics.nGradeCount[i - 1] / uSumValue3).ToString("0.000%");
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
            string tablesLastItems0 = m_resourceManager1.GetString("LblPrintSubTotal.Text");
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

            #endregion

            //打印页数
            #region
            currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
            Font currentPagesFont = new Font("宋体", 12, FontStyle.Regular);
            Brush currentPagesBrush = Brushes.Black;
            string currentPages = m_resourceManager1.GetString("LblPrintPages.Text") + " 1";
            XMaxSize = TitG.MeasureString(currentPages, currentPagesFont);
            e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                currentPages, currentPagesFont, currentPagesBrush, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize.Width) / 2 + PrintProtocol.leftMargin,
                currentAvailableHeight);
            #endregion
            #endregion
        }

        //打印重量模块
        public void printWeight_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e, DataInterface dataInterface)
        {
            #region
            //打印定义
            #region
            int currentAvailableHeight = 0;   //当前可用高度

            //int cylinderWidth = 0;            //条柱宽度
            //int cylinderSpace = 0;            //圆柱间间距
            //int cylinderLeftMargin = 0;       //条柱左边距

            int sumTableHeight = 0;      //汇总表格高度
            int sumTalbeWidth = 0;       //汇总表格宽度

            //int cylinderDigitLeftDistance = 0;//数字左边框
            #endregion

            //打印时间
            #region
            currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
            Font dateTimeFont = new Font("宋体", 15, FontStyle.Regular);
            Brush dateTimeBrush = Brushes.Black;
            string nowDateTime = DateTime.Now.ToString(m_resourceManager2.GetString("LblPrintDateTime.Text"));
            PictureBox picB = new PictureBox();
            Graphics TitG = picB.CreateGraphics();
            SizeF XMaxSize = TitG.MeasureString(nowDateTime, dateTimeFont);
            e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                nowDateTime, dateTimeFont, dateTimeBrush, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize.Width, currentAvailableHeight);
            #endregion

            //打印LOGO
            #region
            currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
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

            //打印统计表
            #region 打印统计表
            //文本标题
            #region
            Font textTitleFont3 = new Font("宋体", 20, FontStyle.Bold);
            Brush textTitleBrush3 = Brushes.Black;
            string textTitle3 = m_resourceManager2.GetString("LblPrintBatchReport.Text");
            XMaxSize = TitG.MeasureString(textTitle3, textTitleFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                textTitle3, textTitleFont3, textTitleBrush3, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize.Width) / 2 + PrintProtocol.leftMargin,
                currentAvailableHeight);
            #endregion

            //分割线1
            #region
            currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
            Pen linePen31 = new Pen(Color.Black, 2);
            e.Graphics.DrawLine(linePen31, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
            #endregion

            //分割线2
            #region
            currentAvailableHeight += (int)linePen31.Width + PrintProtocol.lineOrLineSpace; //当前可用高度
            Pen linePen32 = new Pen(Color.Black, 2);
            e.Graphics.DrawLine(linePen32, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
            #endregion

            //文本内容
            #region
            currentAvailableHeight += (int)linePen32.Width + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
            Font textContentFont3 = new Font("宋体", 15, FontStyle.Regular);
            Brush textContentBrush3 = Brushes.Black;
            int sumBoxNum = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);  //总箱数
            string textContent31 = m_resourceManager2.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName;
            XMaxSize = TitG.MeasureString(textContent31, textContentFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent31, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            string textContent32 = m_resourceManager2.GetString("LblPrintFarmName.Text") + dataInterface.FarmName;
            XMaxSize = TitG.MeasureString(textContent32, textContentFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent32, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
            string textContent33 = m_resourceManager2.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
            XMaxSize = TitG.MeasureString(textContent33, textContentFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent33, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
            currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
            string textContent34 = m_resourceManager2.GetString("LblPrintTotalPieces.Text") + dataInterface.IoStStatistics.nTotalCount;
            XMaxSize = TitG.MeasureString(textContent34, textContentFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent34, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            string textContent35 = m_resourceManager2.GetString("LblPrintTotalWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / 1000000).ToString() + " 吨";
            XMaxSize = TitG.MeasureString(textContent35, textContentFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent35, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
            string textContent36 = m_resourceManager1.GetString("LblPrintTotalCartons.Text") + sumBoxNum;
            XMaxSize = TitG.MeasureString(textContent36, textContentFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
            currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
            string textContent37 = "";
            if (dataInterface.IoStStatistics.nTotalCount == 0)
            {
                textContent37 = m_resourceManager1.GetString("LblPrintAveFruitWeight.Text") + "0.000 " + m_resourceManager1.GetString("LblPrintGName.Text");
            }
            else
            {
                textContent37 = m_resourceManager1.GetString("LblPrintAveFruitWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / dataInterface.IoStStatistics.nTotalCount).ToString("0.000") +
                    " " + m_resourceManager1.GetString("LblPrintGName.Text");
            }
            XMaxSize = TitG.MeasureString(textContent37, textContentFont3);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                textContent37, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            string textContent38 = m_resourceManager1.GetString("LblPrintProgramName.Text") + dataInterface.ProgramName;
            XMaxSize = TitG.MeasureString(textContent38, textContentFont3);
            //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
            //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
               textContent38, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2, currentAvailableHeight);

            currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度 2015-12-7 ivycc
            string textContent39 = m_resourceManager1.GetString("LblExcelStartTime.Text") + dataInterface.StartTime;
            XMaxSize = TitG.MeasureString(textContent39, textContentFont3);
            //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
            //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
               textContent39, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            string textContent40 = m_resourceManager1.GetString("LblExcelEndTime.Text") + dataInterface.EndTime;
            XMaxSize = TitG.MeasureString(textContent40, textContentFont3);
            //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
            //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
            e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
               textContent40, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2, currentAvailableHeight);
            #endregion

            //分割线3
            #region
            currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
            Pen linePen33 = new Pen(Color.Black, 2);
            e.Graphics.DrawLine(linePen33, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
            #endregion

            //分割线4
            #region
            currentAvailableHeight += (int)linePen31.Width + PrintProtocol.lineOrLineSpace; //当前可用高度
            Pen linePen34 = new Pen(Color.Black, 2);
            e.Graphics.DrawLine(linePen34, PrintProtocol.leftMargin, currentAvailableHeight, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight);
            #endregion

            //汇总表格
            #region
            currentAvailableHeight += (int)linePen34.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
            //currentAvailableHeight += (int)XMaxSize.Height + PrintProtocol.barHeadOrBarImageSpace;  //当前可用高度
            //sumTableHeight = (dataInterface.WeightOrSizeGradeSum <= 10 ? PrintProtocol.sumTableHeight1 : PrintProtocol.sumTableHeight2); //汇总表格高度
            sumTableHeight = PrintProtocol.sumTableHeight3;
            sumTalbeWidth = (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin) / 8;
            Pen linePen35 = new Pen(Color.Black, 1);
            Font sumTableFont;
            if (GlobalDataInterface.selectLanguage == "en")
                sumTableFont = new Font("Times New Roman", 9, FontStyle.Regular);
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
                e.Graphics.DrawLine(linePen35, PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight,
                    PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight);
            }
            e.Graphics.DrawLine(linePen35, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight,
                    e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (dataInterface.WeightOrSizeGradeSum + 2) * sumTableHeight);
            //表格标题行
            string tablesFirstItems0 = m_resourceManager2.GetString("LblMainReportName.Text");
            XMaxSize = TitG.MeasureString(tablesFirstItems0, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems0, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems1 = m_resourceManager2.GetString("LblMainReportWeight.Text");
            XMaxSize = TitG.MeasureString(tablesFirstItems1, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems1, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems2 = m_resourceManager2.GetString("LblMainReportPieces.Text");
            XMaxSize = TitG.MeasureString(tablesFirstItems2, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems2, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems3 = m_resourceManager2.GetString("LblMainReportPiecesPer.Text");
            XMaxSize = TitG.MeasureString(tablesFirstItems3, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems3, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems4 = m_resourceManager2.GetString("LblMainReportWeights.Text") + "(kg)";
            XMaxSize = TitG.MeasureString(tablesFirstItems4, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems4, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems5 = m_resourceManager2.GetString("LblMainReportWeightPer.Text");
            XMaxSize = TitG.MeasureString(tablesFirstItems5, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems5, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems6 = m_resourceManager2.GetString("LblMainReportCartons.Text");
            XMaxSize = TitG.MeasureString(tablesFirstItems6, sumTableFont);
            e.Graphics.DrawString(tablesFirstItems6, sumTableFont, sumTableBrush,
                (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
            string tablesFirstItems7 = m_resourceManager2.GetString("LblMainReportCartonsPer.Text");
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
                    (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH).ToString();
                string tablesMiddleItems0 = strName.Substring(0, strName.IndexOf("\0"));
                XMaxSize = TitG.MeasureString(tablesMiddleItems0, sumTableFont);
                e.Graphics.DrawString(tablesMiddleItems0, sumTableFont, sumTableBrush,
                    (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                    currentAvailableHeight + (i + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                string tablesMiddleItems1 = dataInterface.IoStStGradeInfo.grades[i - 1].nMinSize.ToString();
                XMaxSize = TitG.MeasureString(tablesMiddleItems1, sumTableFont);
                e.Graphics.DrawString(tablesMiddleItems1, sumTableFont, sumTableBrush,
                    (sumTalbeWidth - XMaxSize.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                    currentAvailableHeight + (i + 1) * sumTableHeight - XMaxSize.Height - PrintProtocol.tableBottomMargin);
                string tablesMiddleItems2 = dataInterface.IoStStatistics.nGradeCount[i - 1].ToString();
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
                    tablesMiddleItems3 = ((double)dataInterface.IoStStatistics.nGradeCount[i - 1] / uSumValue3).ToString("0.000%");
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
            string tablesLastItems0 = m_resourceManager2.GetString("LblPrintSubTotal.Text");
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

         
            #endregion

            //打印页数
            #region
            currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
            Font currentPagesFont = new Font("宋体", 12, FontStyle.Regular);
            Brush currentPagesBrush = Brushes.Black;
            string currentPages = m_resourceManager2.GetString("LblPrintPages.Text") + " 1";
            XMaxSize = TitG.MeasureString(currentPages, currentPagesFont);
            e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                currentPages, currentPagesFont, currentPagesBrush, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize.Width) / 2 + PrintProtocol.leftMargin,
                currentAvailableHeight);
            #endregion
            #endregion
        }

        //打印品质+尺寸模块
        public void printQualityOrSize_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e, DataInterface dataInterface)
        {
            #region
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
                intPage = 1;
            }
            #endregion

            //打印定义
            #region
            int currentAvailableHeight = 0;   //当前可用高度

            //int cylinderWidth = 0;            //条柱宽度
            //int cylinderSpace = 0;            //圆柱间间距
            //int cylinderLeftMargin = 0;       //条柱左边距

            int sumTableHeight = 0;      //汇总表格高度
            int sumTalbeWidth = 0;       //汇总表格宽度

            //int cylinderDigitLeftDistance = 0;//数字左边框
            #endregion

            //打印统计表
            #region 打印      统计表
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
                currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                Font dateTimeFont60 = new Font("宋体", 15, FontStyle.Regular);
                Brush dateTimeBrush60 = Brushes.Black;
                string nowDateTime60 = DateTime.Now.ToString(m_resourceManager3.GetString("LblPrintDateTime.Text"));
                PictureBox picB60 = new PictureBox();
                Graphics TitG60 = picB60.CreateGraphics();
                SizeF XMaxSize60 = TitG60.MeasureString(nowDateTime60, dateTimeFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                    nowDateTime60, dateTimeFont60, dateTimeBrush60, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize60.Width, currentAvailableHeight);
                #endregion

                //打印LOGO
                #region
                currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
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
                Font textTitleFont60 = new Font("宋体", 20, FontStyle.Bold);
                Brush textTitleBrush60 = Brushes.Black;
                string textTitle60 = m_resourceManager3.GetString("LblPrintBatchReport.Text");
                XMaxSize60 = TitG60.MeasureString(textTitle60, textTitleFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                    textTitle60, textTitleFont60, textTitleBrush60, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin,
                    currentAvailableHeight);
                #endregion

                //分割线1
                #region
                currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
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
                currentAvailableHeight += (int)linePen62.Width + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                Font textContentFont60 = new Font("宋体", 15, FontStyle.Regular);
                Brush textContentBrush60 = Brushes.Black;
                int sumBoxNum = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);  //总箱数
                string textContent31 = m_resourceManager3.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName;
                XMaxSize60 = TitG60.MeasureString(textContent31, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent31, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                string textContent32 = m_resourceManager3.GetString("LblPrintFarmName.Text") + dataInterface.FarmName;
                XMaxSize60 = TitG60.MeasureString(textContent32, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent32, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                string textContent33 = m_resourceManager3.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
                XMaxSize60 = TitG60.MeasureString(textContent33, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent33, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
                string textContent34 = m_resourceManager3.GetString("LblPrintTotalPieces.Text") + dataInterface.IoStStatistics.nTotalCount;
                XMaxSize60 = TitG60.MeasureString(textContent34, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent34, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                string textContent35 = m_resourceManager3.GetString("LblPrintTotalWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / 1000000).ToString() +
                    " " + m_resourceManager3.GetString("LblPrintTName.Text");
                XMaxSize60 = TitG60.MeasureString(textContent35, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent35, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                string textContent36 = m_resourceManager3.GetString("LblPrintTotalCartons.Text") + sumBoxNum;
                XMaxSize60 = TitG60.MeasureString(textContent36, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent36, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
                string textContent37 = "";
                if (dataInterface.IoStStatistics.nTotalCount == 0)
                {
                    textContent37 = m_resourceManager3.GetString("LblPrintAveFruitWeight.Text") + "0.000 " + m_resourceManager3.GetString("LblPrintGName.Text");
                }
                else
                {
                    textContent37 = m_resourceManager3.GetString("LblPrintAveFruitWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / dataInterface.IoStStatistics.nTotalCount).ToString("0.000") +
                        " " + m_resourceManager3.GetString("LblPrintGName.Text");
                }
                XMaxSize60 = TitG60.MeasureString(textContent37, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent37, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                string textContent38 = m_resourceManager3.GetString("LblPrintProgramName.Text") + dataInterface.ProgramName;
                XMaxSize60 = TitG60.MeasureString(textContent38, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                   textContent38, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2, currentAvailableHeight);

                currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度 2015-12-7 ivycc
                string textContent39 = m_resourceManager3.GetString("LblExcelStartTime.Text") + dataInterface.StartTime;
                XMaxSize60 = TitG60.MeasureString(textContent39, textContentFont60);
                //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                   textContent39, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                string textContent40 = m_resourceManager3.GetString("LblExcelEndTime.Text") + dataInterface.EndTime;
                XMaxSize60 = TitG60.MeasureString(textContent40, textContentFont60);
                //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                   textContent40, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2, currentAvailableHeight);
                #endregion

                //分割线3
                #region
                currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
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
                currentAvailableHeight += (int)linePen64.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
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
                string tablesFirstItems0 = m_resourceManager3.GetString("LblMainReportName.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems0, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems0, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems1 = m_resourceManager3.GetString("LblMainReportSize.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems1, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems1, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems2 = m_resourceManager3.GetString("LblMainReportPieces.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems2, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems2, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems3 = m_resourceManager3.GetString("LblMainReportPiecesPer.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems3, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems3, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems4 = m_resourceManager3.GetString("LblMainReportWeights.Text") + "(kg)";
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems4, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems4, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems5 = m_resourceManager3.GetString("LblMainReportWeightPer.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems5, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems5, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems6 = m_resourceManager3.GetString("LblMainReportCartons.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems6, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems6, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems7 = m_resourceManager3.GetString("LblMainReportCartonsPer.Text");
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
                        // string strName= strSizeName.Substring(0, strSizeName.IndexOf("\0")) + "." + strQualityName.Substring(0, strQualityName.IndexOf("\0"));
                        int QualityGradeNameLength = strQualityName.IndexOf("\0");  //add by xcw 20201102  
                        if (QualityGradeNameLength == -1)
                        {
                            QualityGradeNameLength = strQualityName.Length;
                        }
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
                    string tablesLastItems0 = m_resourceManager3.GetString("LblPrintSubTotal.Text");
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
                string currentPages60 = m_resourceManager3.GetString("LblPrintPages.Text") + " " + currentPageIndex.ToString() + "/" + intPage.ToString();
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
                string tablesFirstItems0 = m_resourceManager3.GetString("LblMainReportName.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems0, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems0, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems1 = m_resourceManager3.GetString("LblMainReportSize.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems1, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems1, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems2 = m_resourceManager3.GetString("LblMainReportPieces.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems2, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems2, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems3 = m_resourceManager3.GetString("LblMainReportPiecesPer.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems3, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems3, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems4 = m_resourceManager3.GetString("LblMainReportWeights.Text") + "(kg)";
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems4, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems4, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems5 = m_resourceManager3.GetString("LblMainReportWeightPer.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems5, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems5, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems6 = m_resourceManager3.GetString("LblMainReportCartons.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems6, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems6, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems7 = m_resourceManager3.GetString("LblMainReportCartonsPer.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems7, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems7, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                int index = PrintProtocol.GradeNum1 + PrintProtocol.GradeNum2 * (currentPageIndex - 2) + 1;
                int heightIndex = 1;
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
                string tablesLastItems0 = m_resourceManager3.GetString("LblPrintSubTotal.Text");
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
                string currentPages65 = m_resourceManager3.GetString("LblPrintPages.Text") + " " + currentPageIndex.ToString() + "/" + intPage.ToString();
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
                string tablesFirstItems0 = m_resourceManager3.GetString("LblMainReportName.Text");
                XMaxSize62 = TitG62.MeasureString(tablesFirstItems0, sumTableFont62);
                e.Graphics.DrawString(tablesFirstItems0, sumTableFont62, sumTableBrush62,
                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems1 = m_resourceManager3.GetString("LblMainReportSize.Text");
                XMaxSize62 = TitG62.MeasureString(tablesFirstItems1, sumTableFont62);
                e.Graphics.DrawString(tablesFirstItems1, sumTableFont62, sumTableBrush62,
                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems2 = m_resourceManager3.GetString("LblMainReportPieces.Text");
                XMaxSize62 = TitG62.MeasureString(tablesFirstItems2, sumTableFont62);
                e.Graphics.DrawString(tablesFirstItems2, sumTableFont62, sumTableBrush62,
                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems3 = m_resourceManager3.GetString("LblMainReportPiecesPer.Text");
                XMaxSize62 = TitG62.MeasureString(tablesFirstItems3, sumTableFont62);
                e.Graphics.DrawString(tablesFirstItems3, sumTableFont62, sumTableBrush62,
                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems4 = m_resourceManager3.GetString("LblMainReportWeights.Text") + "(kg)";
                XMaxSize62 = TitG62.MeasureString(tablesFirstItems4, sumTableFont62);
                e.Graphics.DrawString(tablesFirstItems4, sumTableFont62, sumTableBrush62,
                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems5 = m_resourceManager3.GetString("LblMainReportWeightPer.Text");
                XMaxSize62 = TitG62.MeasureString(tablesFirstItems5, sumTableFont62);
                e.Graphics.DrawString(tablesFirstItems5, sumTableFont62, sumTableBrush62,
                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems6 = m_resourceManager3.GetString("LblMainReportCartons.Text");
                XMaxSize62 = TitG62.MeasureString(tablesFirstItems6, sumTableFont62);
                e.Graphics.DrawString(tablesFirstItems6, sumTableFont62, sumTableBrush62,
                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems7 = m_resourceManager3.GetString("LblMainReportCartonsPer.Text");
                XMaxSize62 = TitG62.MeasureString(tablesFirstItems7, sumTableFont62);
                e.Graphics.DrawString(tablesFirstItems7, sumTableFont62, sumTableBrush62,
                    (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);

                int index = PrintProtocol.GradeNum1 + PrintProtocol.GradeNum2 * (currentPageIndex - 2) + 1;
                int heightIndex = 1;
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
                        string tablesMiddleItems1 = dataInterface.IoStStGradeInfo.grades[((j - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].nMinSize.ToString();
                        XMaxSize62 = TitG62.MeasureString(tablesMiddleItems1, sumTableFont62);
                        e.Graphics.DrawString(tablesMiddleItems1, sumTableFont62, sumTableBrush62,
                            (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                            currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems2 = dataInterface.IoStStatistics.nGradeCount[((j - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString();
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
                            tablesMiddleItems3 = ((double)dataInterface.IoStStatistics.nGradeCount[((j - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumValue60).ToString("0.000%");
                        }
                        XMaxSize62 = TitG62.MeasureString(tablesMiddleItems3, sumTableFont62);
                        e.Graphics.DrawString(tablesMiddleItems3, sumTableFont62, sumTableBrush62,
                            (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                            currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems4 = (dataInterface.IoStStatistics.nWeightGradeCount[((j - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / 1000.0).ToString("0.0");
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
                            tablesMiddleItems5 = ((double)dataInterface.IoStStatistics.nWeightGradeCount[((j - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumWeightValue60).ToString("0.000%");
                        }
                        XMaxSize62 = TitG62.MeasureString(tablesMiddleItems5, sumTableFont62);
                        e.Graphics.DrawString(tablesMiddleItems5, sumTableFont62, sumTableBrush62,
                            (sumTalbeWidth - XMaxSize62.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                            currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize62.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems6 = dataInterface.IoStStatistics.nBoxGradeCount[((j - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString(); ;
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
                            tablesMiddleItems7 = ((double)dataInterface.IoStStatistics.nBoxGradeCount[((currentPageIndex - 1) * 2 - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumBoxValue60).ToString("0.000%");
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
                string currentPages62 = m_resourceManager3.GetString("LblPrintPages.Text") + " " + currentPageIndex.ToString() + "/" + intPage.ToString();
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
            #endregion
            #endregion
        }

        //打印品质+重量模块
        public void printQualityOrWeight_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e, DataInterface dataInterface)
        {
            #region
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
                intPage = 1;
            }
            #endregion

            //打印定义
            #region
            int currentAvailableHeight = 0;   //当前可用高度

            //int cylinderWidth = 0;            //条柱宽度
            //int cylinderSpace = 0;            //圆柱间间距
            //int cylinderLeftMargin = 0;       //条柱左边距

            int sumTableHeight = 0;      //汇总表格高度
            int sumTalbeWidth = 0;       //汇总表格宽度

            //int cylinderDigitLeftDistance = 0;//数字左边框
            #endregion

            //打印统计表
            #region 打印统计表
            #region
            UInt64 uSumValue60 = 0; //总个数
            UInt64 uSumWeightValue60 = 0; //总重量
            Int32 uSumBoxValue60 = 0;  //总箱数
            uSumValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nGradeCount);//获取总个数
            uSumWeightValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nWeightGradeCount);//获取总重量
            uSumBoxValue60 = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);//获取总箱数
            if (currentPageIndex == 1)            //第一页
            {
                #region 打印第一页
                //打印时间
                #region
                currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                Font dateTimeFont60 = new Font("宋体", 15, FontStyle.Regular);
                Brush dateTimeBrush60 = Brushes.Black;
                string nowDateTime60 = DateTime.Now.ToString(m_resourceManager4.GetString("LblPrintDateTime.Text"));
                PictureBox picB60 = new PictureBox();
                Graphics TitG60 = picB60.CreateGraphics();
                SizeF XMaxSize60 = TitG60.MeasureString(nowDateTime60, dateTimeFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制时间字符串
                    nowDateTime60, dateTimeFont60, dateTimeBrush60, e.PageBounds.Width - PrintProtocol.rightMargin - XMaxSize60.Width, currentAvailableHeight);
                #endregion

                //打印LOGO
                #region
                currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.dataTimeOrLogoSpace;  //当前可用高度
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
                Font textTitleFont60 = new Font("宋体", 20, FontStyle.Bold);
                Brush textTitleBrush60 = Brushes.Black;
                string textTitle60 = m_resourceManager4.GetString("LblPrintBatchReport.Text");
                XMaxSize60 = TitG60.MeasureString(textTitle60, textTitleFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本标题字符串
                    textTitle60, textTitleFont60, textTitleBrush60, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin,
                    currentAvailableHeight);
                #endregion

                //分割线1
                #region
                currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
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
                currentAvailableHeight += (int)linePen62.Width + PrintProtocol.textTitleOrTextContentSpace;   //当前可用高度
                Font textContentFont60 = new Font("宋体", 15, FontStyle.Regular);
                Brush textContentBrush60 = Brushes.Black;
                int sumBoxNum = FunctionInterface.GetSumValue(dataInterface.IoStStatistics.nBoxGradeCount);  //总箱数
                string textContent31 = m_resourceManager4.GetString("LblPrintCustomerName.Text") + dataInterface.CustomerName;
                XMaxSize60 = TitG60.MeasureString(textContent31, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent31, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                string textContent32 = m_resourceManager4.GetString("LblPrintFarmName.Text") + dataInterface.FarmName;
                XMaxSize60 = TitG60.MeasureString(textContent32, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent32, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                string textContent33 = m_resourceManager4.GetString("LblPrintFruitVarieties.Text") + dataInterface.FruitName;
                XMaxSize60 = TitG60.MeasureString(textContent33, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent33, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
                string textContent34 = m_resourceManager4.GetString("LblPrintTotalPieces.Text") + dataInterface.IoStStatistics.nTotalCount;
                XMaxSize60 = TitG60.MeasureString(textContent34, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent34, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                string textContent35 = m_resourceManager4.GetString("LblPrintTotalWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / 1000000).ToString() +
                    " " + m_resourceManager4.GetString("LblPrintTName.Text");
                XMaxSize60 = TitG60.MeasureString(textContent35, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent35, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                string textContent36 = m_resourceManager4.GetString("LblPrintTotalCartons.Text") + sumBoxNum;
                XMaxSize60 = TitG60.MeasureString(textContent36, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent36, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + 2 * PrintProtocol.textContentItemsWidth, currentAvailableHeight);
                currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度
                string textContent37 = "";
                if (dataInterface.IoStStatistics.nTotalCount == 0)
                {
                    textContent37 = m_resourceManager4.GetString("LblPrintAveFruitWeight.Text") + "0.000 " + m_resourceManager4.GetString("LblPrintGName.Text");
                }
                else
                {
                    textContent37 = m_resourceManager4.GetString("LblPrintAveFruitWeight.Text") + ((double)dataInterface.IoStStatistics.nWeightCount / dataInterface.IoStStatistics.nTotalCount).ToString("0.000") +
                        " " + m_resourceManager4.GetString("LblPrintGName.Text");
                }
                XMaxSize60 = TitG60.MeasureString(textContent37, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                    textContent37, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                string textContent38 = m_resourceManager4.GetString("LblPrintProgramName.Text") + dataInterface.ProgramName;
                XMaxSize60 = TitG60.MeasureString(textContent38, textContentFont60);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                   textContent38, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2, currentAvailableHeight);

                currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrTextContentSpace;  //当前可用高度 2015-12-7 ivycc
                string textContent39 = m_resourceManager4.GetString("LblExcelStartTime.Text") + dataInterface.StartTime;
                XMaxSize60 = TitG60.MeasureString(textContent39, textContentFont60);
                //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                   textContent39, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                string textContent40 = m_resourceManager4.GetString("LblExcelEndTime.Text") + dataInterface.EndTime;
                XMaxSize60 = TitG60.MeasureString(textContent40, textContentFont60);
                //e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                //    textContent36, textContentFont3, textContentBrush3, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin, currentAvailableHeight);
                e.Graphics.DrawString(//使用DrawString方法绘制文本子系统字符串
                   textContent40, textContentFont60, textContentBrush60, PrintProtocol.leftMargin + PrintProtocol.textContentItemsLeftMargin + PrintProtocol.textContentItemsWidth2, currentAvailableHeight);
                #endregion

                //分割线3
                #region
                currentAvailableHeight += (int)XMaxSize60.Height + PrintProtocol.textContentOrLineSpace;   //当前可用高度
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
                currentAvailableHeight += (int)linePen64.Width + PrintProtocol.lineOrBarHeadSpace;    //当前可用高度
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
                string tablesFirstItems0 = m_resourceManager4.GetString("LblMainReportName.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems0, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems0, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems1 = m_resourceManager4.GetString("LblMainReportWeight.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems1, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems1, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems2 = m_resourceManager4.GetString("LblMainReportPieces.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems2, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems2, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems3 = m_resourceManager4.GetString("LblMainReportPiecesPer.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems3, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems3, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems4 = m_resourceManager4.GetString("LblMainReportWeights.Text") + "(kg)";
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems4, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems4, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems5 = m_resourceManager4.GetString("LblMainReportWeightPer.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems5, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems5, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems6 = m_resourceManager4.GetString("LblMainReportCartons.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems6, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems6, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems7 = m_resourceManager4.GetString("LblMainReportCartonsPer.Text");
                XMaxSize60 = TitG60.MeasureString(tablesFirstItems7, sumTableFont60);
                e.Graphics.DrawString(tablesFirstItems7, sumTableFont60, sumTableBrush60,
                    (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                //UInt64 uMaxValue60 = 0; //最大个数

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
                        string tablesMiddleItems6 = dataInterface.IoStStatistics.nBoxGradeCount[(currentPageIndex - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString(); ;
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
                    string tablesLastItems0 = m_resourceManager4.GetString("LblPrintSubTotal.Text");
                    XMaxSize60 = TitG60.MeasureString(tablesLastItems0, sumTableFont60);
                    e.Graphics.DrawString(tablesLastItems0, sumTableFont60, sumTableBrush60,
                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                        currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems1 = "";
                    XMaxSize60 = TitG60.MeasureString(tablesLastItems1, sumTableFont60);
                    e.Graphics.DrawString(tablesLastItems1, sumTableFont60, sumTableBrush60,
                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                        currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems2 = uSumValue60.ToString();
                    XMaxSize60 = TitG60.MeasureString(tablesLastItems2, sumTableFont60);
                    e.Graphics.DrawString(tablesLastItems2, sumTableFont60, sumTableBrush60,
                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                        currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems3 = (uSumValue60 == 0 ? "0.000%" : "100.000%");
                    XMaxSize60 = TitG60.MeasureString(tablesLastItems3, sumTableFont60);
                    e.Graphics.DrawString(tablesLastItems3, sumTableFont60, sumTableBrush60,
                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                        currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems4 = (uSumWeightValue60 / 1000.0).ToString("0.0");
                    XMaxSize60 = TitG60.MeasureString(tablesLastItems4, sumTableFont60);
                    e.Graphics.DrawString(tablesLastItems4, sumTableFont60, sumTableBrush60,
                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                        currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems5 = (uSumWeightValue60 == 0 ? "0.000%" : "100.000%");
                    XMaxSize60 = TitG60.MeasureString(tablesLastItems5, sumTableFont60);
                    e.Graphics.DrawString(tablesLastItems5, sumTableFont60, sumTableBrush60,
                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                        currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
                    string tablesLastItems6 = uSumBoxValue60.ToString();
                    XMaxSize60 = TitG60.MeasureString(tablesLastItems6, sumTableFont60);
                    e.Graphics.DrawString(tablesLastItems6, sumTableFont60, sumTableBrush60,
                        (sumTalbeWidth - XMaxSize60.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                        currentAvailableHeight + (gradenum + 2) * sumTableHeight - XMaxSize60.Height - PrintProtocol.tableBottomMargin);
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
                string currentPages60 = m_resourceManager4.GetString("LblPrintPages.Text") + " " + currentPageIndex.ToString() + "/" + intPage.ToString();
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

                //汇总表格 已修改
                #region
                currentAvailableHeight = PrintProtocol.topMargin; //当前可用高度
                Font sumImageHeadFont65 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                Brush sumImagerHeadBrush65 = Brushes.Black;
                //string sumImageHead65 = m_resourceManager.GetString("LblPrintQualityName.Text") + tempQualityName65.Substring(0, tempQualityName65.IndexOf("\0"));
                PictureBox picB65 = new PictureBox();
                Graphics TitG65 = picB65.CreateGraphics();
                SizeF XMaxSize65;
                sumTableHeight = PrintProtocol.sumTableHeight3; ; //汇总表格高度
                sumTalbeWidth = (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin) / 8;
                Pen linePen65 = new Pen(Color.Black, 1);
                Font sumTableFont65;
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
                string tablesFirstItems0 = m_resourceManager4.GetString("LblMainReportName.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems0, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems0, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems1 = m_resourceManager4.GetString("LblMainReportWeight.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems1, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems1, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems2 = m_resourceManager4.GetString("LblMainReportPieces.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems2, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems2, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems3 = m_resourceManager4.GetString("LblMainReportPiecesPer.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems3, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems3, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems4 = m_resourceManager4.GetString("LblMainReportWeights.Text") + "(kg)";
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems4, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems4, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems5 = m_resourceManager4.GetString("LblMainReportWeightPer.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems5, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems5, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems6 = m_resourceManager4.GetString("LblMainReportCartons.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems6, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems6, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems7 = m_resourceManager4.GetString("LblMainReportCartonsPer.Text");
                XMaxSize65 = TitG65.MeasureString(tablesFirstItems7, sumTableFont65);
                e.Graphics.DrawString(tablesFirstItems7, sumTableFont65, sumTableBrush65,
                    (sumTalbeWidth - XMaxSize65.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize65.Height - PrintProtocol.tableBottomMargin);
                int index = PrintProtocol.GradeNum1 + PrintProtocol.GradeNum2 * (currentPageIndex - 2) + 1;
                int heightIndex = 1;
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
                string tablesLastItems0 = m_resourceManager4.GetString("LblPrintSubTotal.Text");
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

                ////汇总图头部
                //#region
                ////currentAvailableHeight += bitM65.Height + PrintProtocol.QualityOrQualitySpace;   //当前可用高度
                //currentAvailableHeight += PrintProtocol.draw3DImageHeight + PrintProtocol.QualityOrQualitySpace;
                //Font sumImageHeadFont655 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                //Brush sumImagerHeadBrush655 = Brushes.Black;
                //string sumImageHead655 = "";//"颜色/百分比";
                //XMaxSize65 = TitG65.MeasureString(sumImageHead655, sumImageHeadFont655);
                //e.Graphics.DrawString(//使用DrawString方法绘制条形图头部字符串
                //    sumImageHead655, sumImageHeadFont655, sumImagerHeadBrush655, PrintProtocol.leftMargin, currentAvailableHeight);
                //#endregion

                ////绘制饼图
                //#region
                ////currentAvailableHeight += (int)XMaxSize65.Height;   //当前可用高度
                ////PieGraphClass.DrawPieImage(e.Graphics, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - PrintProtocol.drawPieImageWidth) / 2 + PrintProtocol.leftMargin, currentAvailableHeight,
                ////    colorGrade64, colorFruitNumber64, colorGrade64.Length, PrintProtocol.drawPieImageWidth, PrintProtocol.drawPieImageHeight);
                ////Bitmap bitM655 = new Bitmap(PrintProtocol.drawPieImageWidth, PrintProtocol.drawPieImageHeight);
                ////bitM655 = PieGraphClass.DrawPieImage(colorGrade64, colorFruitNumber64, colorGrade64.Length, PrintProtocol.drawPieImageWidth, PrintProtocol.drawPieImageHeight);
                ////try
                ////{
                ////    e.Graphics.DrawImage(bitM655, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - bitM65.Width) / 2 + PrintProtocol.leftMargin,
                ////        currentAvailableHeight, PrintProtocol.drawPieImageWidth, PrintProtocol.drawPieImageHeight);

                ////}
                ////catch (Exception ee)//捕获异常
                ////{
                ////    MessageBox.Show(ee.Message);//弹出消息对话框
                ////}
                //#endregion

                //打印页数
                #region
                currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                Font currentPagesFont65 = new Font("宋体", 12, FontStyle.Regular);
                Brush currentPagesBrush65 = Brushes.Black;
                string currentPages65 = m_resourceManager4.GetString("LblPrintPages.Text") + " " + currentPageIndex.ToString() + "/" + intPage.ToString();
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
                //汇总表格 已修改
                #region
                bool Isbreak = false;
                currentAvailableHeight = PrintProtocol.topMargin;   //当前可用高度
                Font sumImageHeadFont63 = new Font("楷体_GB2312", 15, FontStyle.Regular);
                Brush sumImagerHeadBrush63 = Brushes.Black;
                //string tempQualityName63 = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strQualityGradeName, (currentPageIndex - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH).ToString();
                //string sumImageHead63 = m_resourceManager.GetString("LblPrintQualityName.Text") + tempQualityName63.Substring(0, tempQualityName63.IndexOf("\0"));
                PictureBox picB63 = new PictureBox();
                Graphics TitG63 = picB63.CreateGraphics();
                SizeF XMaxSize63;
                sumTableHeight = PrintProtocol.sumTableHeight3; //汇总表格高度
                sumTalbeWidth = (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin) / 8;
                Pen linePen63 = new Pen(Color.Black, 1);
                Font sumTableFont63;
                if (GlobalDataInterface.selectLanguage == "en")
                    sumTableFont63 = new Font("Times New Roman", 9, FontStyle.Regular);
                else
                    sumTableFont63 = new Font("宋体", 11, FontStyle.Regular);
                Brush sumTableBrush63 = Brushes.Black;
                int gradenum = PrintProtocol.GradeNum2; ;
                for (int i = 0; i < gradenum + 1; i++) //画横线
                {
                    e.Graphics.DrawLine(linePen63, PrintProtocol.leftMargin, currentAvailableHeight + i * sumTableHeight,
                        e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + i * sumTableHeight);
                }
                e.Graphics.DrawLine(linePen63, PrintProtocol.leftMargin, currentAvailableHeight + (gradenum + 1) * sumTableHeight,
                        e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 1) * sumTableHeight);
                for (int i = 0; i < 8; i++) //画竖线
                {
                    e.Graphics.DrawLine(linePen63, PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight,
                        PrintProtocol.leftMargin + i * sumTalbeWidth, currentAvailableHeight + (gradenum + 1) * sumTableHeight);
                }
                e.Graphics.DrawLine(linePen63, e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight,
                        e.PageBounds.Width - PrintProtocol.rightMargin, currentAvailableHeight + (gradenum + 1) * sumTableHeight);
                //表格标题行
                string tablesFirstItems0 = m_resourceManager4.GetString("LblMainReportName.Text");
                XMaxSize63 = TitG63.MeasureString(tablesFirstItems0, sumTableFont63);
                e.Graphics.DrawString(tablesFirstItems0, sumTableFont63, sumTableBrush63,
                    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems1 = m_resourceManager4.GetString("LblMainReportWeight.Text");
                XMaxSize63 = TitG63.MeasureString(tablesFirstItems1, sumTableFont63);
                e.Graphics.DrawString(tablesFirstItems1, sumTableFont63, sumTableBrush63,
                    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems2 = m_resourceManager4.GetString("LblMainReportPieces.Text");
                XMaxSize63 = TitG63.MeasureString(tablesFirstItems2, sumTableFont63);
                e.Graphics.DrawString(tablesFirstItems2, sumTableFont63, sumTableBrush63,
                    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems3 = m_resourceManager4.GetString("LblMainReportPiecesPer.Text");
                XMaxSize63 = TitG63.MeasureString(tablesFirstItems3, sumTableFont63);
                e.Graphics.DrawString(tablesFirstItems3, sumTableFont63, sumTableBrush63,
                    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems4 = m_resourceManager4.GetString("LblMainReportWeights.Text") + "(kg)";
                XMaxSize63 = TitG63.MeasureString(tablesFirstItems4, sumTableFont63);
                e.Graphics.DrawString(tablesFirstItems4, sumTableFont63, sumTableBrush63,
                    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems5 = m_resourceManager4.GetString("LblMainReportWeightPer.Text");
                XMaxSize63 = TitG63.MeasureString(tablesFirstItems5, sumTableFont63);
                e.Graphics.DrawString(tablesFirstItems5, sumTableFont63, sumTableBrush63,
                    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems6 = m_resourceManager4.GetString("LblMainReportCartons.Text");
                XMaxSize63 = TitG63.MeasureString(tablesFirstItems6, sumTableFont63);
                e.Graphics.DrawString(tablesFirstItems6, sumTableFont63, sumTableBrush63,
                    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                string tablesFirstItems7 = m_resourceManager4.GetString("LblMainReportCartonsPer.Text");
                XMaxSize63 = TitG63.MeasureString(tablesFirstItems7, sumTableFont63);
                e.Graphics.DrawString(tablesFirstItems7, sumTableFont63, sumTableBrush63,
                    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                    currentAvailableHeight + (0 + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);

                int index = PrintProtocol.GradeNum1 + PrintProtocol.GradeNum2 * (currentPageIndex - 2) + 1;
                int heightIndex = 1;
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
                        string strName = Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strSizeGradeName,
                            (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH).ToString();
                        XMaxSize63 = TitG63.MeasureString(tablesMiddleItems0, sumTableFont63);
                        e.Graphics.DrawString(tablesMiddleItems0, sumTableFont63, sumTableBrush63,
                            (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                            currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems1 = dataInterface.IoStStGradeInfo.grades[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].nMinSize.ToString();
                        XMaxSize63 = TitG63.MeasureString(tablesMiddleItems1, sumTableFont63);
                        e.Graphics.DrawString(tablesMiddleItems1, sumTableFont63, sumTableBrush63,
                            (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                            currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems2 = dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString();
                        XMaxSize63 = TitG63.MeasureString(tablesMiddleItems2, sumTableFont63);
                        e.Graphics.DrawString(tablesMiddleItems2, sumTableFont63, sumTableBrush63,
                            (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                            currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems3 = "";
                        if (uSumValue60 == 0)
                        {
                            tablesMiddleItems3 = "0.000%";
                        }
                        else
                        {
                            tablesMiddleItems3 = ((double)dataInterface.IoStStatistics.nGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumValue60).ToString("0.000%");
                        }
                        XMaxSize63 = TitG63.MeasureString(tablesMiddleItems3, sumTableFont63);
                        e.Graphics.DrawString(tablesMiddleItems3, sumTableFont63, sumTableBrush63,
                            (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                            currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems4 = (dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / 1000.0).ToString("0.0");
                        XMaxSize63 = TitG63.MeasureString(tablesMiddleItems4, sumTableFont63);
                        e.Graphics.DrawString(tablesMiddleItems4, sumTableFont63, sumTableBrush63,
                            (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                            currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems5 = "";
                        if (uSumWeightValue60 == 0)
                        {
                            tablesMiddleItems5 = "0.000%";
                        }
                        else
                        {
                            tablesMiddleItems5 = ((double)dataInterface.IoStStatistics.nWeightGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumWeightValue60).ToString("0.000%");
                        }
                        XMaxSize63 = TitG63.MeasureString(tablesMiddleItems5, sumTableFont63);
                        e.Graphics.DrawString(tablesMiddleItems5, sumTableFont63, sumTableBrush63,
                            (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                            currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems6 = dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)].ToString(); ;
                        XMaxSize63 = TitG63.MeasureString(tablesMiddleItems6, sumTableFont63);
                        e.Graphics.DrawString(tablesMiddleItems6, sumTableFont63, sumTableBrush63,
                            (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                            currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                        string tablesMiddleItems7 = "";
                        if (uSumBoxValue60 == 0)
                        {
                            tablesMiddleItems7 = "0.000%";
                        }
                        else
                        {
                            tablesMiddleItems7 = ((double)dataInterface.IoStStatistics.nBoxGradeCount[(j - 1) * ConstPreDefine.MAX_SIZE_GRADE_NUM + (i - 1)] / uSumBoxValue60).ToString("0.000%");
                        }
                        XMaxSize63 = TitG63.MeasureString(tablesMiddleItems7, sumTableFont63);
                        e.Graphics.DrawString(tablesMiddleItems7, sumTableFont63, sumTableBrush63,
                            (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                            currentAvailableHeight + (heightIndex + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);

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
                ////表格最后一行
                //string tablesLastItems0 = m_resourceManager.GetString("LblPrintSubTotal.Text");
                //XMaxSize63 = TitG63.MeasureString(tablesLastItems0, sumTableFont63);
                //e.Graphics.DrawString(tablesLastItems0, sumTableFont63, sumTableBrush63,
                //    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 0 * sumTalbeWidth,
                //    currentAvailableHeight + (gradenum + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                //string tablesLastItems1 = "";
                //XMaxSize63 = TitG63.MeasureString(tablesLastItems1, sumTableFont63);
                //e.Graphics.DrawString(tablesLastItems1, sumTableFont63, sumTableBrush63,
                //    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 1 * sumTalbeWidth,
                //    currentAvailableHeight + (gradenum + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                //string tablesLastItems2 = uSumValue60.ToString();
                //XMaxSize63 = TitG63.MeasureString(tablesLastItems2, sumTableFont63);
                //e.Graphics.DrawString(tablesLastItems2, sumTableFont63, sumTableBrush63,
                //    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 2 * sumTalbeWidth,
                //    currentAvailableHeight + (gradenum + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                //string tablesLastItems3 = (uSumValue60 == 0 ? "0.000%" : "100.000%");
                //XMaxSize63 = TitG63.MeasureString(tablesLastItems3, sumTableFont63);
                //e.Graphics.DrawString(tablesLastItems3, sumTableFont63, sumTableBrush63,
                //    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 3 * sumTalbeWidth,
                //    currentAvailableHeight + (gradenum + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                //string tablesLastItems4 = (uSumWeightValue60 / 1000.0).ToString("0.0");
                //XMaxSize63 = TitG63.MeasureString(tablesLastItems4, sumTableFont63);
                //e.Graphics.DrawString(tablesLastItems4, sumTableFont63, sumTableBrush63,
                //    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 4 * sumTalbeWidth,
                //    currentAvailableHeight + (gradenum + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                //string tablesLastItems5 = (uSumWeightValue60 == 0 ? "0.000%" : "100.000%");
                //XMaxSize63 = TitG63.MeasureString(tablesLastItems5, sumTableFont63);
                //e.Graphics.DrawString(tablesLastItems5, sumTableFont63, sumTableBrush63,
                //    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 5 * sumTalbeWidth,
                //    currentAvailableHeight + (gradenum + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                //string tablesLastItems6 = uSumBoxValue60.ToString();
                //XMaxSize63 = TitG63.MeasureString(tablesLastItems6, sumTableFont63);
                //e.Graphics.DrawString(tablesLastItems6, sumTableFont63, sumTableBrush63,
                //    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 6 * sumTalbeWidth,
                //    currentAvailableHeight + (gradenum + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                //string tablesLastItems7 = (uSumBoxValue60 == 0 ? "0.000%" : "100.000%");
                //XMaxSize63 = TitG63.MeasureString(tablesLastItems7, sumTableFont63);
                //e.Graphics.DrawString(tablesLastItems7, sumTableFont63, sumTableBrush63,
                //    (sumTalbeWidth - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin + 7 * sumTalbeWidth,
                //    currentAvailableHeight + (gradenum + 1) * sumTableHeight - XMaxSize63.Height - PrintProtocol.tableBottomMargin);
                #endregion

                //打印页数
                #region
                currentAvailableHeight = e.PageBounds.Height - PrintProtocol.bottomMargin;    //当前可用高度
                Font currentPagesFont63 = new Font("宋体", 12, FontStyle.Regular);
                Brush currentPagesBrush63 = Brushes.Black;
                string currentPages63 = m_resourceManager4.GetString("LblPrintPages.Text") + " " + currentPageIndex.ToString() + "/" + intPage.ToString();
                XMaxSize63 = TitG63.MeasureString(currentPages63, currentPagesFont63);
                e.Graphics.DrawString(//使用DrawString方法绘制页数字符串
                    currentPages63, currentPagesFont63, currentPagesBrush63, (e.PageBounds.Width - PrintProtocol.leftMargin - PrintProtocol.rightMargin - XMaxSize63.Width) / 2 + PrintProtocol.leftMargin,
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
            #endregion
            #endregion
            #endregion
        }
    }
}
