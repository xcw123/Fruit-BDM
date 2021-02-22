using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;
using FruitSortingVtest1;

namespace DrawPieGraph
{
    public class PieGraphClass
    {
        public static String[] XText = new String[16]; //存储数据的名称数组
        public static UInt64[] SzData = new UInt64[16];  //获取每列的和
        public static int ValidArrayLength = 0;        //有效数组长度
        public static int PicWidth = 0;                //图片宽度
        public static int PicHeight = 0;               //图片高度
        public static Color[] WearColor = new Color[] { Color.Red,Color.Gold,Color.Chartreuse,Color.Teal,Color.RoyalBlue,Color.Fuchsia,Color.Firebrick,
            Color.Goldenrod,Color.ForestGreen,Color.Aqua,Color.Blue,Color.PaleVioletRed,Color.Salmon,Color.Yellow,Color.LimeGreen,Color.LightBlue,Color.LightSteelBlue,Color.MediumPurple};
        public static string[] AreaText;//临时存储数据的名称数组
        public static Pen mypen;
        public static float AreaAngle = 0F;
        public static float XSize = 500;//X轴的大小
        public static float YSize = 500;//Y轴的大小
        public static float ASum = 0;//记录饼型的总和
        public static float TemXSize = 0;//X轴的临时大小
        public static float XLeft = 0;//X轴的左端点
        public static float XRight = 0;//X轴的右端点
        public static float YUp = 0;//Y轴的上端点
        public static float YDown = 500;//Y轴的下端点
        public static SolidBrush mybrush = new SolidBrush(Color.Red);
        public static float Aline = 20;//标识文字的前端线长
        public static float Asash = 3;//标识文本名边框的宽度
        public static float temXLeft = 0;//X轴的左端点初始化
        public static float AreaXMaxWidth = 0;//获取字符串的宽度
        public static float AreaXMaxHeight = 0;//获取字符串的高度
        public static ResourceManager m_resourceManager = new ResourceManager(typeof(StatisticsInfoForm3));//创建Mainform资源管理

        //获取饼型图的标识文字
        public static void AreaValue()
        {
            string temTextSize = "";//存储最长的名称
            Font LSfont = new System.Drawing.Font("宋体", 8);//设置说明文字的字体
            AreaText = new string[XText.Length];//实例化一个临时数组
            for (int i = 0; i < AreaText.Length; i++)//获取名称
            {
                AreaText[i] = XText[i];
            }
            float AresF = 0;//记录百分比
            for (int i = 0; i < AreaText.Length; i++)//通过名称及百分比,组合说明文字
            {
                AresF = (SzData[i] / ASum) * 100;//获取当前记录的百分比
                AresF = (float)Math.Round(AresF, 2);//对百分比进行四舍五入
                AreaText[i] = AreaText[i] + " " + AresF.ToString() + "%";//组合说明文字
                if (AreaText[i].Length > temTextSize.Length)//获取最长的说明文件
                    temTextSize = AreaText[i];
            }
            PictureBox picB = new PictureBox();
            Graphics TitG = picB.CreateGraphics();//创建Graphics类对象
            SizeF XMaxSize = TitG.MeasureString(temTextSize + Asash * 2, LSfont);//将绘制的字符串进行格式化
            AreaXMaxWidth = XMaxSize.Width;//获取字符串的宽度
            AreaXMaxHeight = XMaxSize.Height;//获取字符串的高度
        }

        //绘制饼型图标识(Area)
        public static void ProAreaSign(Graphics g)
        {
            AreaValue();//存储最长的名称
            mypen = new Pen(Color.Black, 1);//设置画笔的颜色及大小
            Font LSfont = new System.Drawing.Font("宋体", 8);//设置说明文字的字体样式
            SolidBrush Zbrush = new SolidBrush(Color.Black);//设置存放说明文字边框的画刷
            SolidBrush ATbrush = new SolidBrush(Color.White);//设置存放说明文字方块的背景画刷
            //初始化变量
            float f = 0;//记录百分比
            float TimeNum = 0;//扇形的绘制度数
            float AXLeft = 0;//设置饼型图的X坐标
            float AYUp = 0;//设置饼型图的Y坐标
            float AXSize = 0;//设置饼型图的宽度
            float AYSize = 0;//设置饼型图的高度
            float Atop = 0;//记录饼型的高度为长和宽的最小值
            float Aleft = 0;//记录饼型的高度为长和宽的最小值
            PictureBox picB = new PictureBox();
            Graphics TitG = picB.CreateGraphics();//创建Graphics类对象
            SizeF XMaxSize = TitG.MeasureString("", LSfont);//将绘制的字符串进行格式化
            float SWidth = 0;//获取字符串的宽度
            float SHeight = 0;//获取字符串的高度
            //计算饼型图的初始位置
            XLeft = PicWidth - (PicWidth - 5);//去了边框后饼型图的X位置
            XSize = PicHeight - 10;//设置饼型图的宽度
            temXLeft = AXLeft;//记录饼型图的X坐标
            AXLeft = XLeft;//记录饼型图的X坐标
            AXSize = XSize;//记录饼型图的宽度
            //通过说明文字的大小计算饼型图的位置
            AXLeft = AXLeft + AreaXMaxWidth + Aline;//设置去除说明文字后的饼型图X坐标
            AYUp = AYUp + AreaXMaxHeight;//设置去除说明文字后的饼型图Y坐标
            AXSize = XSize - AreaXMaxWidth * 2 - Aline * 2;//设置去除说明文字后的饼型图宽度
            AYSize = YSize - AreaXMaxHeight * 2;//设置去除说明文字后的饼型图高度
            if (AXSize >= AYSize)//如果饼型图的宽度大于等于高度
            {
                Aleft = AXSize - AYSize;//记录饼型图的X坐标
                AXSize = AYSize;//将高度设为宽度
            }
            else
            {
                Atop = AYSize - AXSize;//记录饼型图的Y坐标
                AYSize = AXSize;//将宽度设为高度
            }
            if (Aleft != 0)//如果宽大于高
            {
                AXLeft = AXLeft + Aleft / 2;//设置饼型图横向局中
            }
            if (Atop != 0)//如果高大于宽
            {
                AYUp = AYUp + Atop / 2;//设置饼型图纵向局中
            }
            temXLeft = XLeft;
            //初始化说明文字前横线的变量
            float X1 = 0;
            float Y1 = 0;
            float X2 = 0;
            float Y2 = 0;
            //初始化说明文字位置的变量
            float TX1 = 0;
            float TY1 = 0;
            float TX2 = 0;
            float TY2 = 0;
            float temf = 0;//记录百分比
            double radians = 0;//记录扇形的角度
            temf = (AreaAngle * (ASum / 360) / ASum);//记录起始位置的度数
            TimeNum = AreaAngle;//记录扇形的起始角度
            //绘制说明文字
            if (AXSize > 0 && AYSize > 0)
            {
                for (int i = 0; i < SzData.Length; i++)//遍历所有说明文字
                {
                    f = SzData[i] / ASum;//获取当前记录的百分比
                    if (f == 0)//如果当前值为0
                        continue;//执行下一次循环
                    radians = ((double)((temf + f / 2) * 360) * Math.PI) / (double)180;
                    X1 = Convert.ToSingle(AXLeft + (AXSize / 2.0 + (int)((float)(AXSize / 2.0) * Math.Cos(radians))));
                    Y1 = Convert.ToSingle(AYUp + (AYSize / 2.0 + (int)((float)(AYSize / 2.0) * Math.Sin(radians))));

                    XMaxSize = TitG.MeasureString(AreaText[i].Trim() + 2.0, LSfont);//将绘制的字符串进行格式化
                    SWidth = XMaxSize.Width;//获取字符串的宽度
                    SHeight = XMaxSize.Height;//获取字符串的高度
                    if ((temf + f / 2) * 360 > 90 && (temf + f / 2) * 360 <= 270)
                    {
                        X2 = X1 - Aline;

                        TX1 = X2 - 1 - SWidth;
                        TY1 = Y1 - SHeight / 2 - Asash;
                        TX2 = SWidth;
                        TY2 = SHeight + Asash * 2;
                        g.FillRectangle(ATbrush, TX1, TY1, TX2, TY2);//绘制内矩形
                        g.DrawRectangle(new Pen(Color.Black, 1), TX1, TY1, TX2, TY2);//绘制矩形
                        g.DrawString(AreaText[i].Trim(), LSfont, Zbrush, new PointF(X2 - SWidth + Asash - 1, Y1 - SHeight / 2));
                    }
                    else
                    {
                        X2 = X1 + Aline;

                        TX1 = X2 + 1;
                        TY1 = Y1 - SHeight / 2 - Asash;
                        TX2 = SWidth;
                        TY2 = SHeight + Asash * 2;
                        g.FillRectangle(ATbrush, TX1, TY1, TX2, TY2);//绘制内矩形
                        g.DrawRectangle(new Pen(Color.Black, 1), TX1, TY1, TX2, TY2);//绘制矩形
                        g.DrawString(AreaText[i].Trim(), LSfont, Zbrush, new PointF(X2 + Asash + 1, Y1 - SHeight / 2));
                    }
                    Y2 = Y1;
                    g.DrawLine(new Pen(new SolidBrush(Color.Black), 1), X1, Y1, X2, Y2);
                    TimeNum += f * 360;  //度数
                    temf = temf + f;     //百分比
                }
                g.DrawString(m_resourceManager.GetString("LblPrintColorReport.Text"), new Font("楷体_GB2312", 15, FontStyle.Bold), new SolidBrush(Color.Black), new PointF(AXLeft + AXSize / 2 - 80, AYUp + AYSize + SHeight + 60 / 2));
            }
            else
                return;
        }

        /// <summary>
        /// 绘制饼型图
        /// </summary>
        /// <param name="xText">存储数据的名称数组</param>
        /// <param name="szData">获取每列的和</param>
        /// <param name="validArrayLength">数组的有效长度（默认数组长度为16）</param>
        /// <param name="picWidth">绘制的面板宽度</param>
        /// <param name="picHeight">绘制的面板高度</param>
        /// <returns></returns>
        public static Bitmap DrawPieImage(string[] xText, UInt64[] szData, int validArrayLength, int picWidth, int picHeight)
        {
            XText = xText;
            SzData = szData;
            ValidArrayLength = validArrayLength;
            PicWidth = picWidth;
            PicHeight = picHeight;
            ASum = 0;
            for(int i=0;i<validArrayLength;i++)
            {
                ASum += SzData[i];
            }
            Bitmap bitM = new Bitmap(PicWidth, PicHeight);
            Graphics g = Graphics.FromImage(bitM);
            g.Clear(Color.White);
            if (ASum == 0)
            {
                g.Clear(Color.Gray);
                return bitM;
            }
            AreaValue();//计算最长说明文字的大小
            //初始化变量
            mypen = new Pen(Color.Black, 1);//设置画笔的颜色及大小
            float f = 0;//记录百分比
            float TimeNum = 0;//扇形的绘制度数
            float AXLeft = 0;//设置饼型图的X坐标
            float AYUp = 0;//设置饼型图的Y坐标
            float AXSize = 0;//设置饼型图的宽度
            float AYSize = 0;//设置饼型图的高度
            float Atop = 0;//记录饼型的高度为长和宽的最小值
            float Aleft = 0;//记录饼型的高度为长和宽的最小值
            TimeNum = AreaAngle;//设置饼型图的起始度数
            //计算饼型图的初始位置
            XLeft = PicWidth - (PicWidth - 5);//去了边框后饼型图的X位置
            XSize = PicHeight - 10;//设置饼型图的宽度
            temXLeft = AXLeft;//记录饼型图的X坐标
            AXLeft = XLeft;//记录饼型图的X坐标
            AXSize = XSize;//记录饼型图的宽度
            //通过说明文字的大小计算饼型图的位置
            AXLeft = AXLeft + AreaXMaxWidth + Aline;//设置去除说明文字后的饼型图X坐标
            AYUp = AYUp + AreaXMaxHeight;//设置去除说明文字后的饼型图Y坐标
            AXSize = XSize - AreaXMaxWidth * 2 - Aline * 2;//设置去除说明文字后的饼型图宽度
            AYSize = YSize - AreaXMaxHeight * 2;//设置去除说明文字后的饼型图高度
            if (AXSize >= AYSize)//如果饼型图的宽度大于等于高度
            {
                Aleft = AXSize - AYSize;//记录饼型图的X坐标
                AXSize = AYSize;//将高度设为宽度
            }
            else
            {
                Atop = AYSize - AXSize;//记录饼型图的Y坐标
                AYSize = AXSize;//将宽度设为高度
            }
            if (Aleft != 0)//如果宽大于高
            {
                AXLeft = AXLeft + Aleft / 2;//设置饼型图横向局中
            }
            if (Atop != 0)//如果高大于宽
            {
                AYUp = AYUp + Atop / 2;//设置饼型图纵向局中
            }
            temXLeft = XLeft;
            //绘制饼型图
            if (AXSize > 0 && AYSize > 0)//如果饼型图的宽和高大于0
            {
                for (int i = 0; i < SzData.Length; i++)//遍历数据
                {
                    f = SzData[i] / ASum;//获取当前数据的百分比
                    //设置当前扇形图的填充颜色
                    if (i >= WearColor.Length)//如果没有超出颜色数组
                        mybrush = new SolidBrush(WearColor[i - WearColor.Length]);
                    else
                        mybrush = new SolidBrush(WearColor[i]);
                    g.FillPie(mybrush, AXLeft, AYUp, AXSize, AYSize, TimeNum, f * 360);//绘制扇形图
                    TimeNum += f * 360;//设置下一个扇形图的度数
                }
                ProAreaSign(g);//绘制饼型图的说明文字
            }
            else
                return null;
            return bitM;
        }

        /// <summary>
        /// 绘制饼型图
        /// </summary>
        /// <param name="xText">存储数据的名称数组</param>
        /// <param name="szData">获取每列的和</param>
        /// <param name="validArrayLength">数组的有效长度（默认数组长度为16）</param>
        /// <param name="picWidth">绘制的面板宽度</param>
        /// <param name="picHeight">绘制的面板高度</param>
        /// <returns></returns>
        public static void DrawPieImage(Graphics g, int zeroWidth, int zeroHeight, string[] xText, UInt64[] szData, int validArrayLength, int picWidth, int picHeight)
        {
            XText = xText;
            SzData = szData;
            ValidArrayLength = validArrayLength;
            PicWidth = picWidth;
            PicHeight = picHeight;
            ASum = 0;
            for (int i = 0; i < validArrayLength; i++)
            {
                ASum += SzData[i];
            }
            //Bitmap bitM = new Bitmap(PicWidth, PicHeight);
            //Graphics g = Graphics.FromImage(bitM);
            //g.Clear(Color.White);
            g.TranslateTransform(zeroWidth,zeroHeight);
            if (ASum == 0)
            {
                //g.Clear(Color.Gray);
                g.ResetTransform();
                return;
            }
            AreaValue();//计算最长说明文字的大小
            //初始化变量
            mypen = new Pen(Color.Black, 1);//设置画笔的颜色及大小
            float f = 0;//记录百分比
            float TimeNum = 0;//扇形的绘制度数
            float AXLeft = 0;//设置饼型图的X坐标
            float AYUp = 0;//设置饼型图的Y坐标
            float AXSize = 0;//设置饼型图的宽度
            float AYSize = 0;//设置饼型图的高度
            float Atop = 0;//记录饼型的高度为长和宽的最小值
            float Aleft = 0;//记录饼型的高度为长和宽的最小值
            TimeNum = AreaAngle;//设置饼型图的起始度数
            //计算饼型图的初始位置
            XLeft = PicWidth - (PicWidth - 5);//去了边框后饼型图的X位置
            XSize = PicHeight - 10;//设置饼型图的宽度
            temXLeft = AXLeft;//记录饼型图的X坐标
            AXLeft = XLeft;//记录饼型图的X坐标
            AXSize = XSize;//记录饼型图的宽度
            //通过说明文字的大小计算饼型图的位置
            AXLeft = AXLeft + AreaXMaxWidth + Aline;//设置去除说明文字后的饼型图X坐标
            AYUp = AYUp + AreaXMaxHeight;//设置去除说明文字后的饼型图Y坐标
            AXSize = XSize - AreaXMaxWidth * 2 - Aline * 2;//设置去除说明文字后的饼型图宽度
            AYSize = YSize - AreaXMaxHeight * 2;//设置去除说明文字后的饼型图高度
            if (AXSize >= AYSize)//如果饼型图的宽度大于等于高度
            {
                Aleft = AXSize - AYSize;//记录饼型图的X坐标
                AXSize = AYSize;//将高度设为宽度
            }
            else
            {
                Atop = AYSize - AXSize;//记录饼型图的Y坐标
                AYSize = AXSize;//将宽度设为高度
            }
            if (Aleft != 0)//如果宽大于高
            {
                AXLeft = AXLeft + Aleft / 2;//设置饼型图横向局中
            }
            if (Atop != 0)//如果高大于宽
            {
                AYUp = AYUp + Atop / 2;//设置饼型图纵向局中
            }
            temXLeft = XLeft;
            //绘制饼型图
            if (AXSize > 0 && AYSize > 0)//如果饼型图的宽和高大于0
            {
                for (int i = 0; i < SzData.Length; i++)//遍历数据
                {
                    f = SzData[i] / ASum;//获取当前数据的百分比
                    //设置当前扇形图的填充颜色
                    if (i >= WearColor.Length)//如果没有超出颜色数组
                        mybrush = new SolidBrush(WearColor[i - WearColor.Length]);
                    else
                        mybrush = new SolidBrush(WearColor[i]);
                    g.FillPie(mybrush, AXLeft, AYUp, AXSize, AYSize, TimeNum, f * 360);//绘制扇形图
                    TimeNum += f * 360;//设置下一个扇形图的度数
                }
                ProAreaSign(g);//绘制饼型图的说明文字
            }
            else
            {
                g.ResetTransform();
                return;
            }
            g.ResetTransform();
            return;
        }
    }
}
