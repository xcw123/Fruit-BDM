using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FruitSortingVtest1.DB;
using Interface;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Drawing;

namespace FruitSortingVtest1
{
    class ExcelReportFunc
    {
        /// <summary>
        /// 导出Excel表格
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreateExcel(string filePath, DataInterface dataInterfaceExcel,string[] resource, bool IsQaulity,bool IsSize)
        {
            try
            {
                string FilePath = filePath;
                FileInfo fi = new FileInfo(FilePath);
                if (fi.Exists)//判断文件是否已经存在，如果存在就删除！
                {
                    fi.Delete();
                }
                //if (sheetNames != null && sheetNames != "")
                //{
                Excel.Application m_Excel = new Excel.Application();//创建一个Excel对象(同时启动EXCEL.EXE进程)
               // m_Excel.Visible = true;
                m_Excel.SheetsInNewWorkbook = 3;//工作表的个数
                Excel._Workbook m_Book = (Excel._Workbook)(m_Excel.Workbooks.Add(Missing.Value));//添加新工作簿
                Excel._Worksheet m_Sheet = m_Book.Worksheets[1];
                CreateTable(m_Sheet, dataInterfaceExcel,resource,IsQaulity);
                FillTableData(m_Sheet, dataInterfaceExcel, 10, IsQaulity, IsSize);

                //自动设置列宽 放到最后
                Excel.Range range = m_Sheet.Columns;
                range.AutoFit();

                //设置列宽
                range = m_Sheet.get_Range("B7", "B7"); //取得单元格范围
                range.ColumnWidth = 15;

                range = m_Sheet.get_Range("G4", "G4"); //取得单元格范围
                range.ColumnWidth = 15;

                //最后载入logo图片
                range = m_Sheet.get_Range("A2", "H2");//取得单元格范围
                Bitmap bitmap = new Bitmap(PrintProtocol.logoPathName);//创建位图对象;   
                float  width = (float)(bitmap.Width * range.Height / bitmap.Height);
                m_Sheet.Shapes.AddPicture(PrintProtocol.logoPathName, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue,
                    range.Left + range.Width / 2 - width / 2, range.Top, width, range.Height);
              

                #region 保存Excel,清除进程
                m_Book.SaveAs(filePath, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);//保存Excel
                m_Book.Close(false, Missing.Value, Missing.Value);
                m_Excel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(m_Sheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(m_Book);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(m_Excel);
                GC.Collect();
                m_Book = null;
                m_Sheet = null;
                m_Excel = null;

                #endregion
                // }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExcelReportFunc中函数备份设置CreateTable出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExcelReportFunc中函数备份设置CreateTable出错" + ex);
#endif
            }

        }
        /// <summary>
        /// 创建表格
        /// </summary>
        /// <param name="m_Sheet"></param>
        public static void CreateTable(Excel._Worksheet m_Sheet, DataInterface dataInterfaceExcel,string[] resource, bool IsQaulity)//string title, Excel._Worksheet m_Sheet, Excel._Workbook m_Book, int startrow
        {
            try
            {
                //打印时间
                Excel.Range range = m_Sheet.get_Range("A1", "H1");//取得单元格范围
                range.MergeCells = true;//合并单元格
                range.Font.Name = "宋体";//设置字体
                range.Font.Size = 12;//字体大小
                //range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                //range.HorizontalAlignment = Excel.XlVAlign.xlVAlignJustify;//单元格纵向靠右
                range.Value2 = resource[0];//当前打印时间

                //logo
                range = m_Sheet.get_Range("A2", "H2");//取得单元格范围
                range.MergeCells = true;//合并单元格
                range.Font.Name = "宋体";//设置字体
                range.Font.Size = 28;//字体大小
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向居中
                

                //表头名称
                range = m_Sheet.get_Range("A3", "H3");//取得单元格范围
                range.MergeCells = true;//合并单元格
                range.Font.Name = "宋体";//设置字体
                range.Font.Size = 24;//字体大小
                range.Font.Bold = true; //字体加粗
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向居中
                range.Value2 = resource[1]; //"加工报表";

                range.EntireRow.AutoFit();

                int startrow;//等级个数
                int qualSum = 1;
                if (dataInterfaceExcel.QualityGradeSum > qualSum) qualSum = dataInterfaceExcel.QualityGradeSum;
                int sizeSum = 1;
                if (dataInterfaceExcel.WeightOrSizeGradeSum > sizeSum) sizeSum = dataInterfaceExcel.WeightOrSizeGradeSum;
                if (IsQaulity)
                    startrow = sizeSum * qualSum;
                else
                    startrow = sizeSum;

                //客户信息表格格式 dataInterface.WeightOrSizeGradeSum
                #region
                range = m_Sheet.get_Range("A4", "H7");
                range.Font.Name = "宋体";//设置字体
                range.Font.Size = 14;//字体大小
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向居中
                range.Borders.LineStyle = Excel.XlLineStyle.xlDot;//设置边框
                range.Borders.Weight = Excel.XlBorderWeight.xlThin;//边框常规粗细
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = Excel.XlLineStyle.xlContinuous;//设置顶部边框
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Excel.XlLineStyle.xlContinuous;//设置底部边框
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Excel.XlLineStyle.xlContinuous;//设置左边边框
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;//设置右边边框
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                range.EntireRow.AutoFit();
                range.EntireColumn.AutoFit();

                range = m_Sheet.get_Range("A4", "A4");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[2];//客户名称

                range = m_Sheet.get_Range("D4", "D4");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[3];//"农场名称";

                range = m_Sheet.get_Range("G4", "G4");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[4];//"水果品种";

                range = m_Sheet.get_Range("A5", "A5");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[5];//"个数汇总";

                range = m_Sheet.get_Range("D5", "D5");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[6];//"重量汇总";

                range = m_Sheet.get_Range("F5", "F5");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.HorizontalAlignment = Excel.XlVAlign.xlVAlignJustify;//单元格横向靠左
                range.Value2 = resource[7];//"吨";

                range = m_Sheet.get_Range("G5", "G5");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[8];//"箱数汇总";

                range = m_Sheet.get_Range("A6", "A6");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[9];//"平均果重";

                range = m_Sheet.get_Range("C6", "C6");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.HorizontalAlignment = Excel.XlVAlign.xlVAlignJustify;//单元格横向靠左
                range.Value2 = resource[10];//"克";

                range = m_Sheet.get_Range("D6", "D6");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[11];//"分选程序";

                range = m_Sheet.get_Range("G6", "G6");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[22];//"序列号";

                range = m_Sheet.get_Range("A7", "A7");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[12];//"开始时间";

                range = m_Sheet.get_Range("D7", "D7");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[13];//"结束时间";
                #endregion

                //等级信息表格格式 dataInterface.WeightOrSizeGradeSum
                #region
                range = m_Sheet.Range[m_Sheet.Cells[9, 1], m_Sheet.Cells[9 + startrow, 8]];//取得单元格范围
                range.Font.Name = "宋体";//设置字体
                range.Font.Size = 14;//字体大小
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向居中
                range.Borders.LineStyle = Excel.XlLineStyle.xlDot;//设置边框
                range.Borders.Weight = Excel.XlBorderWeight.xlThin;//边框常规粗细
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = Excel.XlLineStyle.xlContinuous;//设置顶部边框
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Excel.XlLineStyle.xlContinuous;//设置底部边框
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Excel.XlLineStyle.xlContinuous;//设置左边边框
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;//设置右边边框
                range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                range.EntireRow.AutoFit();
                range.EntireColumn.AutoFit();
                #endregion

                ////列标题
                #region
                range = m_Sheet.get_Range("A9", "A9");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[14];//等级名称
                //range.Cells.Interior.Color = System.Drawing.Color.FromArgb(0, 204, 151).ToArgb();//设置单元格颜色

                range = m_Sheet.get_Range("B9", "B9");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[15];//尺寸/重量
                //range.Cells.Interior.Color = System.Drawing.Color.FromArgb(0, 204, 151).ToArgb();//设置单元格颜色

                range = m_Sheet.get_Range("C9", "C9");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[16];//等级个数
                //range.Cells.Interior.Color = System.Drawing.Color.FromArgb(0, 204, 151).ToArgb();//设置单元格颜色

                range = m_Sheet.get_Range("D9", "D9");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[17];//个数百分比
                //range.Cells.Interior.Color = System.Drawing.Color.FromArgb(0, 204, 151).ToArgb();//设置单元格颜色

                range = m_Sheet.get_Range("E9", "E9");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[18]+"(kg)";//等级重量
                //range.Cells.Interior.Color = System.Drawing.Color.FromArgb(0, 204, 151).ToArgb();//设置单元格颜色

                range = m_Sheet.get_Range("F9", "F9");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[19];//重量百分比
               // range.Cells.Interior.Color = System.Drawing.Color.FromArgb(0, 204, 151).ToArgb();//设置单元格颜色

                range = m_Sheet.get_Range("G9", "G9");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[20];//箱数
               //range.Cells.Interior.Color = System.Drawing.Color.FromArgb(0, 204, 151).ToArgb();//设置单元格颜色

                range = m_Sheet.get_Range("H9", "H9");//取得单元格范围
                range.Font.Bold = true; //字体加粗
                range.Value2 = resource[21];//箱数百分比
                //  range.Cells.Interior.Color = System.Drawing.Color.FromArgb(0, 204, 151).ToArgb();//设置单元格颜色
                #endregion


                //自动设置列宽 放到最后
                range = m_Sheet.Columns;
                range.AutoFit();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExcelReportFunc中函数备份设置CreateTable出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExcelReportFunc中函数备份设置CreateTable出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 填充表格信息
        /// </summary>
        /// <param name="m_Sheet"></param>
        /// <param name="startrow"></param>
        public static void FillTableData(Excel._Worksheet m_Sheet, DataInterface dataInterfaceExcel, int startrow, bool IsQaulity, bool IsSize)
        {
            try
            {

                //客户信息
                #region
                m_Sheet.Cells[4, 2] = dataInterfaceExcel.CustomerName;//客户名称
                m_Sheet.Cells[4, 5] = dataInterfaceExcel.FarmName;//农场名称
                m_Sheet.Cells[4, 8] = dataInterfaceExcel.FruitName;//水果品种
                m_Sheet.Cells[5, 2] = dataInterfaceExcel.IoStStatistics.nTotalCount; //个数汇总
                m_Sheet.Cells[5, 5] = ((double)dataInterfaceExcel.IoStStatistics.nWeightCount / 1000000).ToString(); //重量汇总  //Modify by ChengSk - 20180305
                int sumBoxNum = FunctionInterface.GetSumValue(dataInterfaceExcel.IoStStatistics.nBoxGradeCount);     //总箱数
                m_Sheet.Cells[5, 8] = sumBoxNum; //箱数汇总
                //平均果重
                if (dataInterfaceExcel.IoStStatistics.nTotalCount == 0)
                {
                    m_Sheet.Cells[6, 2] = 0.000; 
                }
                else
                {
                    m_Sheet.Cells[6, 2] = ((double)dataInterfaceExcel.IoStStatistics.nWeightCount / dataInterfaceExcel.IoStStatistics.nTotalCount).ToString("0.000");
                }
                m_Sheet.Cells[6, 5] = dataInterfaceExcel.ProgramName;//分选程序
                m_Sheet.Cells[6, 8] = GlobalDataInterface.SerialNum;//序列号 //add by xcw 20200701

                //开始时间
                Excel.Range range = m_Sheet.Range[m_Sheet.Cells[7, 2], m_Sheet.Cells[7, 3]]; //取得单元格范围
                range.MergeCells = true;//合并单元格
                range.Value2 = dataInterfaceExcel.StartTime;
                //结束时间
                range = m_Sheet.Range[m_Sheet.Cells[7, 5], m_Sheet.Cells[7, 6]]; //取得单元格范围
                range.MergeCells = true;//合并单元格
                range.Value2 = dataInterfaceExcel.EndTime;
                #endregion

                //等级信息
                #region
                int startrowex;//等级个数
                UInt64 uSumValue, uSumWeightValue;
                int uSumBoxValue;
                //if(IsSize)
                uSumValue = FunctionInterface.GetSumValue(dataInterfaceExcel.IoStStatistics.nGradeCount);//获取总个数
                //else
                uSumWeightValue = FunctionInterface.GetSumValue(dataInterfaceExcel.IoStStatistics.nWeightGradeCount);//获取总重量
                uSumBoxValue = FunctionInterface.GetSumValue(dataInterfaceExcel.IoStStatistics.nBoxGradeCount);//获取总箱数
                if (!IsQaulity)
                {
                    startrowex = dataInterfaceExcel.WeightOrSizeGradeSum;
                    for (int j = 0; j < dataInterfaceExcel.WeightOrSizeGradeSum; j++)
                    {
                        if (dataInterfaceExcel.QualityGradeSum == 0)
                        {
                            m_Sheet.Cells[j + startrow, 1] = Encoding.Default.GetString(dataInterfaceExcel.IoStStGradeInfo.strSizeGradeName,
                           j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);//等级名称
                            m_Sheet.Cells[j + startrow, 2] = dataInterfaceExcel.IoStStGradeInfo.grades[j].nMinSize.ToString();//尺寸/重量
                            m_Sheet.Cells[j + startrow, 3] = dataInterfaceExcel.IoStStatistics.nGradeCount[j].ToString();//等级个数
                                                                                                                         //个数百分比
                            if (uSumValue == 0)
                            {
                                m_Sheet.Cells[j + startrow, 4] = "0.00%";
                            }
                            else
                            {
                                m_Sheet.Cells[j + startrow, 4] = ((double)dataInterfaceExcel.IoStStatistics.nGradeCount[j] / uSumValue).ToString("0.00%");
                            }
                            m_Sheet.Cells[j + startrow, 5] = (dataInterfaceExcel.IoStStatistics.nWeightGradeCount[j] / 1000.0).ToString("#0.0");//等级重量
                                                                                                                                                //重量百分比
                            if (uSumWeightValue == 0)
                            {
                                m_Sheet.Cells[j + startrow, 6] = "0.00%";
                            }
                            else
                            {
                                m_Sheet.Cells[j + startrow, 6] = ((double)dataInterfaceExcel.IoStStatistics.nWeightGradeCount[j] / uSumWeightValue).ToString("0.00%");
                            }
                            m_Sheet.Cells[j + startrow, 7] = dataInterfaceExcel.IoStStatistics.nBoxGradeCount[j].ToString();//箱数
                                                                                                                            //箱数百分比
                            if (uSumWeightValue == 0)
                            {
                                m_Sheet.Cells[j + startrow, 8] = "0.00%";
                            }
                            else
                            {
                                m_Sheet.Cells[j + startrow, 8] = ((double)dataInterfaceExcel.IoStStatistics.nBoxGradeCount[j] / uSumBoxValue).ToString("0.00%");
                            }
                        }
                        else
                        {
                            ulong GradeCount = 0;//add by xcw 20201104
                            double CountPer = 0.0;
                            double GradeWeight = 0.0;
                            double WeightPer = 0.0;
                            int BoxSumValue = 0;
                            double BoxSumPer = 0.0;
                            for (int i = 0; i < dataInterfaceExcel.QualityGradeSum; i++)
                            {
                                GradeCount += dataInterfaceExcel.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                                if (uSumValue != 0)
                                {
                                    CountPer += ((double)dataInterfaceExcel.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uSumValue);
                                }
                                GradeWeight += (dataInterfaceExcel.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / 1000.0);
                                if (uSumWeightValue != 0)
                                {
                                    WeightPer += ((double)dataInterfaceExcel.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uSumWeightValue);
                                }
                                BoxSumValue += dataInterfaceExcel.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];

                                if (uSumBoxValue != 0)
                                {
                                    BoxSumPer += ((double)dataInterfaceExcel.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uSumBoxValue);
                                }
                            }
                            m_Sheet.Cells[j + startrow, 1] = Encoding.Default.GetString(dataInterfaceExcel.IoStStGradeInfo.strSizeGradeName,
                                j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);//等级名称
                            m_Sheet.Cells[j + startrow, 2] = dataInterfaceExcel.IoStStGradeInfo.grades[j].nMinSize.ToString();//尺寸/重量
                            m_Sheet.Cells[j + startrow, 3] = GradeCount.ToString();//等级个数
                                                                                   //个数百分比
                            m_Sheet.Cells[j + startrow, 4] = CountPer.ToString("0.00%");
                            m_Sheet.Cells[j + startrow, 5] = GradeWeight.ToString("#0.0");//等级重量
                                                                                          //重量百分比  j
                            m_Sheet.Cells[j + startrow, 6] = WeightPer.ToString("0.00%");
                            m_Sheet.Cells[j + startrow, 7] = BoxSumValue.ToString();//箱数
                                                                                    //箱数百分比
                            m_Sheet.Cells[j + startrow, 8] = BoxSumPer.ToString("0.00%");
                            //m_Sheet.Cells[j + startrow, 8] =((double)BoxSumValue/ uSumBoxValue).ToString("0.00%");
                        }

                    }

                }
                else
                {

                    string strSizeName;
                    string strQualityName;
                    string strMixName;
                    int qualSum = 1;
                    if (dataInterfaceExcel.QualityGradeSum > qualSum) qualSum = dataInterfaceExcel.QualityGradeSum;
                    int sizeSum = 1;
                    if (dataInterfaceExcel.WeightOrSizeGradeSum > sizeSum) sizeSum = dataInterfaceExcel.WeightOrSizeGradeSum;
                    startrowex = qualSum * sizeSum;
                    for (int i = 0; i < qualSum; i++)
                    {
                        for (int j = 0; j < sizeSum; j++)
                        {
                            strSizeName = Encoding.Default.GetString(dataInterfaceExcel.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                            strQualityName = Encoding.Default.GetString(dataInterfaceExcel.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                            int QualityGradeNameLength = strQualityName.IndexOf("\0");  //add by xcw 20201102  
                            if (QualityGradeNameLength == -1)
                            {
                                QualityGradeNameLength = strQualityName.Length;
                            }
                            strMixName = strSizeName.Substring(0, strSizeName.IndexOf("\0")) + "." + strQualityName.Substring(0, QualityGradeNameLength);
                            m_Sheet.Cells[i * dataInterfaceExcel.WeightOrSizeGradeSum + j + startrow, 1] = strMixName;//等级名称
                            m_Sheet.Cells[i * dataInterfaceExcel.WeightOrSizeGradeSum + j + startrow, 2] = dataInterfaceExcel.IoStStGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nMinSize.ToString();//尺寸/重量
                            m_Sheet.Cells[i * dataInterfaceExcel.WeightOrSizeGradeSum + j + startrow, 3] = dataInterfaceExcel.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString();//等级个数
                            //个数百分比
                            if (uSumValue == 0)
                            {
                                m_Sheet.Cells[i * dataInterfaceExcel.WeightOrSizeGradeSum + j + startrow, 4] = "0.00%";
                            }
                            else
                            {
                                m_Sheet.Cells[i * dataInterfaceExcel.WeightOrSizeGradeSum + j + startrow, 4] = ((double)dataInterfaceExcel.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uSumValue).ToString("0.00%");
                            }

                            m_Sheet.Cells[i * dataInterfaceExcel.WeightOrSizeGradeSum + j + startrow, 5] = (dataInterfaceExcel.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j]/ 1000.0).ToString("#0.0").ToString();//等级重量
                            //重量百分比
                            if (uSumWeightValue == 0)
                            {
                                m_Sheet.Cells[i * dataInterfaceExcel.WeightOrSizeGradeSum + j + startrow, 6] = "0.00%";

                            }
                            else
                            {
                                m_Sheet.Cells[i * dataInterfaceExcel.WeightOrSizeGradeSum + j + startrow, 6] = ((double)dataInterfaceExcel.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uSumWeightValue).ToString("0.00%");
                            }
                            m_Sheet.Cells[i * dataInterfaceExcel.WeightOrSizeGradeSum + j + startrow, 7] = dataInterfaceExcel.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString();//箱数
                            //箱数百分比
                            if (uSumBoxValue == 0)
                            {
                                m_Sheet.Cells[i * dataInterfaceExcel.WeightOrSizeGradeSum + j + startrow, 8] = "0.00%";

                            }
                            else
                            {
                                m_Sheet.Cells[i * dataInterfaceExcel.WeightOrSizeGradeSum + j + startrow, 8] = ((double)dataInterfaceExcel.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / uSumBoxValue).ToString("0.00%");
                            }
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExcelReportFunc中函数备份设置FillTableData出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExcelReportFunc中函数备份设置FillTableData出错" + ex);
#endif
            }
        }
       
    }
}
