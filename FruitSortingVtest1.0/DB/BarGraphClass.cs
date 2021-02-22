using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;

namespace Draw3DBarGraph
{
    public class BarGraphClass
    {
        private const int RowsHeight = 26;                 //行高
        private const int Height = RowsHeight * 12;        //总高
        private const int CylinderLong = 28;               //柱长
        private const int CylinderGap = 13;                //柱间距
        private const int CylinderWide = CylinderGap - 5;  //柱宽
        private const int CylinderHeight = RowsHeight * 10;//柱高
        private const int RowsWidth = RowsHeight * 4 + CylinderLong * MaxCylinderSum + CylinderGap * (MaxCylinderSum - 1); //行长
        private const int BottomSpace = 8;                 //距底边距离
        private const int MaxCylinderSum = 16;             //最大柱形数   

        public static Bitmap DrawBarImage(string[] xText,float[] szData,int validArrayLength,int picWidth, int picHeight)
        {
            Bitmap bitM = new Bitmap(picWidth, picHeight);
            Graphics g = Graphics.FromImage(bitM);
            //g.Clear(Color.Turquoise);
            g.Clear(Color.White);
            
            //左侧墙
            Point[] sbxLeft = new Point[]{
                new Point(RowsHeight,RowsHeight),
                new Point(RowsHeight,RowsHeight*11),
                new Point(RowsHeight*2,RowsHeight*10),
                new Point(RowsHeight*2,0)
            };
            g.FillPolygon(new SolidBrush(Color.FromArgb(180, 180, 180)), sbxLeft);
            
            //地下墙
            Point[] sbxBottom = new Point[]{
                new Point(RowsHeight,RowsHeight*11),
                new Point(RowsWidth-RowsHeight,Height-RowsHeight),
                new Point(RowsWidth,RowsHeight*10),
                new Point(RowsHeight*2,RowsHeight*10)
            };
            g.FillPolygon(new SolidBrush(Color.FromArgb(119, 119, 119)), sbxBottom);
            
            //线条绘制
            for (int i = 0; i < 11; i++)
            {
                g.DrawLine(new Pen(Color.Black), RowsHeight * 2, RowsHeight * i, RowsWidth, RowsHeight * i);
                g.DrawLine(new Pen(Color.Black), RowsHeight, RowsHeight * (i + 1), RowsHeight * 2, RowsHeight * i);
                g.DrawString((100 - 10 * i).ToString(), new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.Black), 5, RowsHeight * (float)0.75 + RowsHeight * i);
            }
            g.DrawLine(new Pen(Color.Black), RowsHeight * 2, 0, RowsHeight * 2, RowsHeight * 10);
            g.DrawLine(new Pen(Color.Black), RowsHeight, RowsHeight, RowsHeight, RowsHeight * 11);
            g.DrawLine(new Pen(Color.Black), RowsHeight, RowsHeight * 11, RowsWidth - RowsHeight, Height - RowsHeight);
            g.DrawLine(new Pen(Color.Black), RowsWidth - RowsHeight, Height - RowsHeight, RowsWidth, Height - RowsHeight * 2);
            g.DrawLine(new Pen(Color.Black), RowsWidth, 0, RowsWidth, Height - RowsHeight * 2);

            //求数据的总和
            float szDataSum = 0f;
            for (int i = 0; i < validArrayLength;i++ )
            {
                szDataSum += szData[i];
            }

            //柱形图开始显示位
            int StartShowIndex = (MaxCylinderSum - validArrayLength)/2;

            //绘制柱形图
            for (int i = 0; i < validArrayLength;i++ )
            {
                if(szData[i] == 0)
                {
                    Point[] sbxBar = new Point[] { 
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide,RowsHeight*11-BottomSpace),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide+CylinderLong,RowsHeight*11-BottomSpace),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)+CylinderLong,RowsHeight*11-BottomSpace-CylinderWide),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap),RowsHeight*11-BottomSpace-CylinderWide),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide,RowsHeight*11-BottomSpace)
                    };
                    g.DrawLines(new Pen(Color.Black), sbxBar);
                    g.DrawString(xText[i].ToString(), new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.Black), RowsHeight * 3 + (StartShowIndex + i) * (CylinderLong + CylinderGap) - CylinderWide, Height - RowsHeight);
                }
                else
                {
                    Point[] sbxBar11 = new Point[]{
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide,RowsHeight*11-BottomSpace),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide+CylinderLong,RowsHeight*11-BottomSpace),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide+CylinderLong,RowsHeight*11-BottomSpace-Convert.ToInt32(RowsHeight*10*(szData[i]/szDataSum))),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide,RowsHeight*11-BottomSpace-Convert.ToInt32(RowsHeight*10*(szData[i]/szDataSum))),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide,RowsHeight*11-BottomSpace)
                    };
                    g.FillPolygon(new SolidBrush(Color.FromArgb(199, 199, 199)), sbxBar11);
                    g.DrawLines(new Pen(Color.Black), sbxBar11);
                    Point[] sbxBar12 = new Point[]{
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide+CylinderLong,RowsHeight*11-BottomSpace),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)+CylinderLong,RowsHeight*11-BottomSpace-CylinderWide),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)+CylinderLong,RowsHeight*11-BottomSpace-CylinderWide-Convert.ToInt32(RowsHeight*10*(szData[i]/szDataSum))),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap),RowsHeight*11-BottomSpace-CylinderWide-Convert.ToInt32(RowsHeight*10*(szData[i]/szDataSum))),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide,RowsHeight*11-BottomSpace-Convert.ToInt32(RowsHeight*10*(szData[i]/szDataSum))),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide+CylinderLong,RowsHeight*11-BottomSpace-Convert.ToInt32(RowsHeight*10*(szData[i]/szDataSum))),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide+CylinderLong,RowsHeight*11-BottomSpace)
                    };
                    g.FillPolygon(new SolidBrush(Color.FromArgb(119, 119, 119)), sbxBar12);
                    g.DrawLines(new Pen(Color.Black), sbxBar12);
                    g.DrawLine(new Pen(Color.Black), RowsHeight * 3 + (StartShowIndex + i) * (CylinderLong + CylinderGap) + CylinderLong, RowsHeight * 11 - BottomSpace - CylinderWide - Convert.ToInt32(RowsHeight * 10 * (szData[i] / szDataSum)),
                        RowsHeight * 3 + (StartShowIndex + i) * (CylinderLong + CylinderGap) - CylinderWide + CylinderLong, RowsHeight * 11 - BottomSpace - Convert.ToInt32(RowsHeight * 10 * (szData[i] / szDataSum)));
                    g.DrawString(xText[i].ToString(), new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.Black), RowsHeight * 3 + (StartShowIndex + i) * (CylinderLong + CylinderGap) - CylinderWide, Height - RowsHeight);
                }
            }

            return bitM;
        }

        public static void DrawBarImage(Graphics g, int zeroWidth, int zeroHeight, string[] xText, float[] szData, int validArrayLength, int picWidth, int picHeight)
        {
            //Bitmap bitM = new Bitmap(picWidth, picHeight);
            //Graphics g = Graphics.FromImage(bitM);
            //g.Clear(Color.Turquoise);
            g.TranslateTransform(zeroWidth, zeroHeight);
            //g.Clear(Color.White);

            //左侧墙
            Point[] sbxLeft = new Point[]{
                new Point(RowsHeight,RowsHeight),
                new Point(RowsHeight,RowsHeight*11),
                new Point(RowsHeight*2,RowsHeight*10),
                new Point(RowsHeight*2,0)
            };
            g.FillPolygon(new SolidBrush(Color.FromArgb(180, 180, 180)), sbxLeft);

            //地下墙
            Point[] sbxBottom = new Point[]{
                new Point(RowsHeight,RowsHeight*11),
                new Point(RowsWidth-RowsHeight,Height-RowsHeight),
                new Point(RowsWidth,RowsHeight*10),
                new Point(RowsHeight*2,RowsHeight*10)
            };
            g.FillPolygon(new SolidBrush(Color.FromArgb(119, 119, 119)), sbxBottom);

            //线条绘制
            for (int i = 0; i < 11; i++)
            {
                g.DrawLine(new Pen(Color.Black), RowsHeight * 2, RowsHeight * i, RowsWidth, RowsHeight * i);
                g.DrawLine(new Pen(Color.Black), RowsHeight, RowsHeight * (i + 1), RowsHeight * 2, RowsHeight * i);
                g.DrawString((100 - 10 * i).ToString(), new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.Black), 5, RowsHeight * (float)0.75 + RowsHeight * i);
            }
            g.DrawLine(new Pen(Color.Black), RowsHeight * 2, 0, RowsHeight * 2, RowsHeight * 10);
            g.DrawLine(new Pen(Color.Black), RowsHeight, RowsHeight, RowsHeight, RowsHeight * 11);
            g.DrawLine(new Pen(Color.Black), RowsHeight, RowsHeight * 11, RowsWidth - RowsHeight, Height - RowsHeight);
            g.DrawLine(new Pen(Color.Black), RowsWidth - RowsHeight, Height - RowsHeight, RowsWidth, Height - RowsHeight * 2);
            g.DrawLine(new Pen(Color.Black), RowsWidth, 0, RowsWidth, Height - RowsHeight * 2);

            //求数据的总和
            float szDataSum = 0f;
            for (int i = 0; i < validArrayLength; i++)
            {
                szDataSum += szData[i];
            }

            //柱形图开始显示位
            int StartShowIndex = (MaxCylinderSum - validArrayLength) / 2;

            //绘制柱形图
            for (int i = 0; i < validArrayLength; i++)
            {
                if (szData[i] == 0)
                {
                    Point[] sbxBar = new Point[] { 
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide,RowsHeight*11-BottomSpace),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide+CylinderLong,RowsHeight*11-BottomSpace),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)+CylinderLong,RowsHeight*11-BottomSpace-CylinderWide),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap),RowsHeight*11-BottomSpace-CylinderWide),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide,RowsHeight*11-BottomSpace)
                    };
                    g.DrawLines(new Pen(Color.Black), sbxBar);
                    g.DrawString(xText[i].ToString(), new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.Black), RowsHeight * 3 + (StartShowIndex + i) * (CylinderLong + CylinderGap) - CylinderWide, Height - RowsHeight);
                }
                else
                {
                    Point[] sbxBar11 = new Point[]{
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide,RowsHeight*11-BottomSpace),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide+CylinderLong,RowsHeight*11-BottomSpace),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide+CylinderLong,RowsHeight*11-BottomSpace-Convert.ToInt32(RowsHeight*10*(szData[i]/szDataSum))),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide,RowsHeight*11-BottomSpace-Convert.ToInt32(RowsHeight*10*(szData[i]/szDataSum))),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide,RowsHeight*11-BottomSpace)
                    };
                    g.FillPolygon(new SolidBrush(Color.FromArgb(199, 199, 199)), sbxBar11);
                    g.DrawLines(new Pen(Color.Black), sbxBar11);
                    Point[] sbxBar12 = new Point[]{
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide+CylinderLong,RowsHeight*11-BottomSpace),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)+CylinderLong,RowsHeight*11-BottomSpace-CylinderWide),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)+CylinderLong,RowsHeight*11-BottomSpace-CylinderWide-Convert.ToInt32(RowsHeight*10*(szData[i]/szDataSum))),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap),RowsHeight*11-BottomSpace-CylinderWide-Convert.ToInt32(RowsHeight*10*(szData[i]/szDataSum))),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide,RowsHeight*11-BottomSpace-Convert.ToInt32(RowsHeight*10*(szData[i]/szDataSum))),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide+CylinderLong,RowsHeight*11-BottomSpace-Convert.ToInt32(RowsHeight*10*(szData[i]/szDataSum))),
                        new Point(RowsHeight*3+(StartShowIndex+i)*(CylinderLong+CylinderGap)-CylinderWide+CylinderLong,RowsHeight*11-BottomSpace)
                    };
                    g.FillPolygon(new SolidBrush(Color.FromArgb(119, 119, 119)), sbxBar12);
                    g.DrawLines(new Pen(Color.Black), sbxBar12);
                    g.DrawLine(new Pen(Color.Black), RowsHeight * 3 + (StartShowIndex + i) * (CylinderLong + CylinderGap) + CylinderLong, RowsHeight * 11 - BottomSpace - CylinderWide - Convert.ToInt32(RowsHeight * 10 * (szData[i] / szDataSum)),
                        RowsHeight * 3 + (StartShowIndex + i) * (CylinderLong + CylinderGap) - CylinderWide + CylinderLong, RowsHeight * 11 - BottomSpace - Convert.ToInt32(RowsHeight * 10 * (szData[i] / szDataSum)));
                    g.DrawString(xText[i].ToString(), new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.Black), RowsHeight * 3 + (StartShowIndex + i) * (CylinderLong + CylinderGap) - CylinderWide, Height - RowsHeight);
                }
            }
            g.ResetTransform();

            //return bitM;
        }
    }
}
