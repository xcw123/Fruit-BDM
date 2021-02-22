using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FruitSortingVtest1.DB
{
    public class PrintProtocol
    {
        //public static string logoPathName = "Logo\\myLogo.bmp"; //Logo路径(400*55)
        public static string logoPathName = "Logo\\MyLogo40201.png"; //Logo路径(400*55)
        public const int leftMargin = 40;     //左边距
        public const int rightMargin = 40;    //右边距
        public const int topMargin = 50;      //上边距
        public const int bottomMargin = 59;   //下边距
        public const int textContentItemsWidth = 270;     //文本内容子模块宽度(例如：客户名称：绿盟 的总宽度)
        public const int textContentItemsWidth2 = 270;     //文本内容子模块时间宽度(例如：客户名称：绿盟 的总宽度)
        public const int textContentItemsLeftMargin = 0; //文本内容子模块距标准开始位间距
        public const int dataTimeOrLogoSpace = 20;        //时间与LOGO之间的上下距离
        public const int logoOrTextTitleSpace = 30;      //LOGO与文本标题之间的上下距离
        public const int textTitleOrTextContentSpace = 20;//文本标题与文本内容之间的上下距离
        public const int lineOrLineSpace = 2;            //分割线与分割线之间的上下距离
        public const int textContentOrTextContentSpace = 10;//文本内容与文本内容之间的距离
        public const int textContentOrLineSpace = 25;    //文本标题与分割线之间的距离
        public const int lineOrBarHeadSpace = 25;        //分割线与条形图头部之间的距离
        public const int barHeadOrBarImageSpace = 20;    //条形图头部与条形图之间的距离
        public const int barHeadOrBarImageSpace1 = 10;   //条形图头部与条形图之间的距离1
        public const int BarImageOrBarTitleSpace = 5;    //条形图与条形图标题之间的距离
        public const int TableOr3DImageSpace = 2;        //表格与3D图之间的距离
        public const int QualityOrQualitySpace = 15;     //品质与品质之间的距离

        public const int barImageWidht = 747;     //条形图宽度
        public const int barImageHeight = 500;    //条形图高度
        public static Color barBackColor = Color.White; //条形图背景色
        public static Color penColor = Color.Black;         //笔刷颜色
        public static Color barBrush = Color.Goldenrod;     //柱状图颜色
        public static Color barBrush2 = Color.Teal;         //柱状图颜色2
        public static Color barBrush3 = Color.Gray;         //柱状图颜色3
        public const int cylinderMaxHeight = 400;//条柱的最大高度
        public const int cylinderMinHeight = 1;  //条柱的最小高度
        public const int cylinderStandardCylinderHeigh = 78;//条柱分隔高度
        public const int cylinderWidth1 = 45;    //条柱宽度1
        public const int cylinderSpace1 = 15;    //圆柱间间距1
        public const int cylinderWidth2 = 25;    //条柱宽度2
        public const int cylinderSpace2 = 10;    //圆柱间间距2
        public const int sumTableHeight1 = 28;   //汇总表格高度1(10等级以内使用)
        public const int sumTableHeight2 = 19;   //汇总表格高度2(10-16等级使用)
        public const int sumTableHeight3 = 24;   //汇总表格统一为一个表格
        public const int tableBottomMargin = 0;  //表格字体距下线之间的距离
        public const int cylinderTopMargin = 60; //条柱上边距
        public const int cylinderBottomMargin = 40;//条柱下边距
        public const int cylinderOrTextNote = 5;//条柱距下标的距离
        public const int cylinderDigitLeftDistance1 = 12;  //数字左边框
        public const int cylinderDigitLeftDistance2 = 3;   //数字左边框
        public const int cylinderDigitBottomDistance = 10;//数字下边框
        public const int cylinderPrecentLeftDistance = 0;//百分比左边框
        public const int cylinderPrecentBottomDistance = 12;//百分比下边框
        public const int draw3DImageWidth = 748; //三维立体图宽度
        public const int draw3DImageHeight = 313;//三维立体图高度
        public const int drawPieImageWidth = 500; //饼图宽度
        public const int drawPieImageHeight = 500;//饼图高度


        public const int ModuleRightDistance = 100;          //"颜色框"距右边框距离
        public const int ModuleMiddleDistance = 30;          //"颜色框"距中间距离
        public const int ModuleFrameWidth = 80;              //"颜色框"的宽度 
        public const int ModuleFrameHeight = 50;             //"颜色框"的高度
        public const int ModuleSmallCylinderWidth = 20;      //"颜色框"中小图的宽度
        public const int ModuleSmallCylinderHeight = 23;     //"颜色框"中小图的高度
        public const int ModuleSmallGraphHorizontalSpace = 2;//"颜色框"中小图的水平间距
        public const int ModuleSmallGraphVerticalSpace = 2;  //"颜色框"中小图的垂直间距
        public const int ModuleFontHorizontalSpace = 1;      //"颜色框"中字体的水平间距
        public const int ModuleFontVerticalSpace = 4;        //"颜色框"中字体的垂直间距
        public const string ModuleFontStyle = "楷体_GB2312"; //"颜色框"中的字体类型
        public const float ModuleFontSize = 12;              //"颜色框"中的字体大小
        public const int GradeNum1 = 28;                     //第一页最多等级数量 25->28 Modify by ChengSk - 20171124
        public const int GradeNum2 = 40;                     //中间页最多等级数量
    }
}
