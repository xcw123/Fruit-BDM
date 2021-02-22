using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Interface;
using System.Windows.Forms;

namespace FruitSortingVtest1.DB
{
    public class DrawGraphProtocol
    {
        //柱状图背景颜色
        public static Color myBarBackColor = Color.Turquoise;
        //笔刷颜色
        public static Color myPenColor = Color.Black;
        //柱状颜色一
        public static Color myBarBrush1 = Color.Goldenrod;
        //柱状颜色二
        public static Color myBarBrush2 = Color.Teal;
        //柱状颜色三
        public static Color myBarBrush3 = Color.Gray;
        //表格各行换背景色
        public static Color myBackColor = Color.LightGray;

        //宏定义标准高度：当低于此高度时，百分比在下面，个数在上面
        public const int StandardCylinderHeight = 78;

        //圆柱宽
        public const int CylinderWidth = 45;
        //圆柱间间距
        public const int CylinderSpace = 15;
        //圆柱距左右边框间距
        public const int LeftRightSpace = 20;

        //型号1：圆柱高
        public const int CylinderHeight1 = 400;  //有滚动条   300
        //型号2：圆柱高
        public const int CylinderHeight2 = 425;  //无滚动条   325
        //型号3：圆柱高
        public const int CylinderHeight3 = 150;  //数量统计信息   95
        //型号4：圆柱高
        public const int CylinderHeight4 = 1;    //百分比为0时默认高度

        //"数量/百分比"绘图X位置
        public const int Module1LocationX = 10;
        //"数量/百分比"绘图Y位置
        public const int Module1LocationY = 10;
        //"数量/百分比"字体类型
        public const string Module1FontStyle = "Times New Roman";
        //"数量/百分比"字体大小
        public const float Module1FontSize = 12;
 
        //"颜色框"距右边框距离
        public const int Module2RightDistance = 100;
        //"颜色框"距中间距离
        public const int Module2MiddleDistance = 30;
        //"颜色框"的宽度
        public const int Module2FrameWidth = 80;
        //"颜色框"的高度
        public const int Module2FrameHeight = 50;
        //"颜色框"中小图的宽度
        public const int Module2SmallCylinderWidth = 20;
        //"颜色框"中小图的高度
        public const int Module2SmallCylinderHeight = 23;
        //"颜色框"中小图的水平间距
        public const int Module2SmallGraphHorizontalSpace = 2;
        //"颜色框"中小图的垂直间距
        public const int Module2SmallGraphVerticalSpace = 2;
        //"颜色框"中字体的水平间距
        public const int Module2FontHorizontalSpace = 1;
        //"颜色框"中字体的垂直间距
        public const int Module2FontVerticalSpace = 4;
        //"颜色框"中的字体类型
        public const string Module2FontStyle = "Times New Roman";
        //"颜色框"中的字体大小
        public const float Module2FontSize = 12;

        //"出口统计信息"距中间距离
        public const int Module3MiddleDistance = 40;
        //"出口统计信息"距下边框距离
        public const int Module3BottomDistance = 50;  //如果设置太小，有滚动条的情况下会覆盖字体，因此将25改为50 modify by ChengSk - 2017/08/02
        //"出口统计信息"字体类型
        public const string Module3FontStyle = "Times New Roman";
        //"出口统计信息"字体大小
        public const float Module3FontSize = 15;

        //"新品质1"距下边框距离
        public const int Module4BottomDistance = 70;  //考虑到Module3BottomDistance值的改变，布局需要，将45改为70 modify by ChengSk - 2017/08/02
        //"新品质1"字体类型
        public const string Module4FontStyle = "Times New Roman";
        //"新品质1"字体大小
        public const float Module4FontSize = 10;

        //"圆柱体"距"新品质1"的距离
        public const int Module5FontDistance = 2;

        //"100%"距"圆柱体"的距离
        public const int Module6CylinderDistance = 12;
        //"100%"字体类型
        public const string Module6FontStyle = "Times New Roman";
        //"100%"字体大小
        public const float Module6FontSize = 10;
        //"100%"距左、上的距离
        public const int Module6LeftDistance = 0;

        //"234"距"圆柱体"底端的距离
        public const int Module7CylinderBottomDistance = 12;
        //"234"字体类型
        public const string Module7FontStyle = "Times New Roman";
        //"234"字体大小
        public const float Module7FontSize = 10;
        //"234"距"圆柱体"左端的距离
        public const int Module7CylinderLeftDistance = 12;

        //最小公约数 Add by ChengSk - 2017/7/28
        public const int MINIMALCOVENANT = 5;  //计算“圆柱间间距”时满足最小公约数规则，例：间距为62时 ((int)(Width - 1) / MINIMALCOVENANT + 1) * MINIMALCOVENANT = 65

        /// <summary>
        /// 最长等级名称长度（柱状图绘制中的长度） Add by ChengSk - 2017/7/28
        /// </summary>
        /// <param name="WeightOrSizeGradeSum">重量或尺寸等级数量</param>
        /// <param name="bWeightOrSizeGradeName">重量或尺寸等级名称</param>
        /// <returns></returns>
        public static int LongestWeightOrSizeGradeName(int WeightOrSizeGradeSum, byte[] bWeightOrSizeGradeName)
        {
            string strTempName = "";        //等级名称
            int tempNameLength = 0;         //临时名字长度
            int longestNameLength = 0;      //最长名字长度
            int indexLongestNameLength = 0; //最长名字长度  的索引号
            int returnLongestNameLength = 0;//返回  最长名字长度

            try
            {
                for (int i = 0; i < WeightOrSizeGradeSum; i++)  //查找最长等级名称及索引号
                {
                    strTempName = Encoding.Default.GetString(bWeightOrSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                    tempNameLength = strTempName.Length;
                    longestNameLength = (tempNameLength > longestNameLength) ? tempNameLength : longestNameLength;
                    indexLongestNameLength = (tempNameLength > longestNameLength) ? i : indexLongestNameLength;
                }

                {
                    Font ft = new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular); ;
                    Control ctrl = new Control();
                    Graphics vGraphics = ctrl.CreateGraphics();
                    SizeF vSizeF = vGraphics.MeasureString(Encoding.Default.GetString(bWeightOrSizeGradeName, indexLongestNameLength * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH), ft);

                    returnLongestNameLength = ((int)(vSizeF.Width - 1) / MINIMALCOVENANT + 1) * MINIMALCOVENANT;   //返回最长名字长度
                }

                return returnLongestNameLength;
            }
            catch (Exception ex)
            {
                Console.WriteLine("LongestWeightOrSizeGradeName Error: " + ex.ToString());
            }

            return returnLongestNameLength;          
        }

        /// <summary>
        /// 最长等级名称长度（柱状图绘制中的长度） Add by ChengSk - 2017/7/28
        /// </summary>
        /// <param name="WeightOrSizeGradeSum">重量或尺寸等级数量</param>
        /// <param name="strWeightOrSizeGradeName">重量或尺寸等级名称</param>
        /// <returns></returns>
        public static int LongestWeightOrSizeGradeName(int WeightOrSizeGradeSum, string[] strWeightOrSizeGradeName)
        {
            string strTempName = "";        //等级名称
            int tempNameLength = 0;         //临时名字长度
            int longestNameLength = 0;      //最长名字长度
            int indexLongestNameLength = 0; //最长名字长度  的索引号
            int returnLongestNameLength = 0;//返回  最长名字长度

            try
            {
                for (int i = 0; i < WeightOrSizeGradeSum; i++)  //查找最长等级名称及索引号
                {
                    strTempName = strWeightOrSizeGradeName[i];
                    tempNameLength = strTempName.Length;
                    longestNameLength = (tempNameLength > longestNameLength) ? tempNameLength : longestNameLength;
                    indexLongestNameLength = (tempNameLength > longestNameLength) ? i : indexLongestNameLength;
                }

                {
                    Font ft = new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular); ;
                    Control ctrl = new Control();
                    Graphics vGraphics = ctrl.CreateGraphics();
                    SizeF vSizeF = vGraphics.MeasureString(strWeightOrSizeGradeName[indexLongestNameLength], ft);

                    returnLongestNameLength = ((int)(vSizeF.Width - 1) / MINIMALCOVENANT + 1) * MINIMALCOVENANT;   //返回最长名字长度
                }

                return returnLongestNameLength;
            }
            catch (Exception ex)
            {
                Console.WriteLine("LongestWeightOrSizeGradeName Error: " + ex.ToString());
            }

            return returnLongestNameLength;
        }

        /// <summary>
        /// 最长等级名称长度（柱状图绘制中的长度） Add by ChengSk - 2017/7/28
        /// </summary>
        /// <param name="QualityGradeSum">品质等级数量</param>
        /// <param name="bQualityGradeName">品质等级名称</param>
        /// <param name="WeightOrSizeGradeSum">重量或尺寸等级数量</param>
        /// <param name="bWeightOrSizeGradeName">重量或尺寸等级名称</param>
        /// <returns></returns>
        public static int LongestWeightOrSizeGradeName(int QualityGradeSum, byte[] bQualityGradeName, int WeightOrSizeGradeSum, byte[] bWeightOrSizeGradeName)
        {
            string strTempName = "";        //等级名称（品质+重量或尺寸名称）
            string strQualityName = "";     //品质名称
            string strWeightOrSizeName = "";//重量或尺寸名称        
            int tempNameLength = 0;         //临时名字长度
            int longestNameLength = 0;      //最长名字长度
            int indexLongestNameLength = 0; //最长名字长度  的索引号
            int returnLongestNameLength = 0;//返回  最长名字长度

            try
            {
                for (int i = 0; i < QualityGradeSum; i++)
                {
                    for (int j = 0; j < WeightOrSizeGradeSum; j++)
                    {
                        strQualityName = Encoding.Default.GetString(bQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                        strWeightOrSizeName = Encoding.Default.GetString(bWeightOrSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                        int QualityGradeNameLength = strQualityName.IndexOf("\0");  //add by xcw 20201102  
                        if (QualityGradeNameLength == -1)
                        {
                            QualityGradeNameLength = strQualityName.Length;
                        }
                        strTempName = strWeightOrSizeName.Substring(0, strWeightOrSizeName.IndexOf("\0")) + "." + strQualityName.Substring(0, QualityGradeNameLength);
                        longestNameLength = (tempNameLength > longestNameLength) ? tempNameLength : longestNameLength;
                        indexLongestNameLength = (tempNameLength > longestNameLength) ? i : indexLongestNameLength;
                    }
                }

                {
                    Font ft = new Font(DrawGraphProtocol.Module4FontStyle, DrawGraphProtocol.Module4FontSize, FontStyle.Regular); ;
                    Control ctrl = new Control();
                    Graphics vGraphics = ctrl.CreateGraphics();
                    SizeF vSizeF = vGraphics.MeasureString(Encoding.Default.GetString(bWeightOrSizeGradeName, indexLongestNameLength * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH), ft);

                    returnLongestNameLength = ((int)(vSizeF.Width - 1) / MINIMALCOVENANT + 1) * MINIMALCOVENANT;   //返回最长名字长度
                }

                return returnLongestNameLength;
            }
            catch (Exception ex)
            {
                Console.WriteLine("LongestWeightOrSizeGradeName Error: " + ex.ToString());
            }

            return returnLongestNameLength;
        }

    }
}
