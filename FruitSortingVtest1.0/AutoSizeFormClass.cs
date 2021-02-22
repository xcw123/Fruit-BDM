using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using Interface;

namespace FruitSortingVtest1._0
{
    class AutoSizeFormClass
    {
        //(1).声明结构,只记录窗体和其控件的初始位置和大小。
        public struct controlRect
        {
            public int Left;
            public int Top;
            public int Width;
            public int Height;
            public float FontSize;
        }
        //(2).声明 1个对象
        //注意这里不能使用控件列表记录 List<Control> nCtrl;，因为控件的关联性，记录的始终是当前的大小。
        public List<controlRect> oldCtrl;
        //int ctrl_first = 0;
        //(3). 创建两个函数
        //(3.1)记录窗体和其控件的初始位置和大小,
        public void controllInitializeSize(Form mForm)
        {
            // if (ctrl_first == 0)
            //{
            //  ctrl_first = 1;
            try
            {
                oldCtrl = new List<controlRect>();
                controlRect cR;
                cR.Left = mForm.Left; cR.Top = mForm.Top; cR.Width = mForm.Width; cR.Height = mForm.Height; cR.FontSize = mForm.Font.Size;
                oldCtrl.Add(cR);
                //GetControlSize(mForm);
                foreach (Control c in mForm.Controls)
                {
                    controlRect objCtrl, SubobjCtrl, SubobjCtrl2, SubobjCtrl3, SubobjCtrl4, SubobjCtrl5, SubobjCtrl6, SubobjCtrl7, SubobjCtrl8;
                    objCtrl.Left = c.Left; objCtrl.Top = c.Top; objCtrl.Width = c.Width; objCtrl.Height = c.Height; objCtrl.FontSize = c.Font.Size;
                    oldCtrl.Add(objCtrl);
                    if (c.Controls.Count > 0)
                    {
                        List<Control> result = FindInSubControl(c);
                        foreach (Control Subitem in result)
                        {
                            SubobjCtrl.Left = Subitem.Left; SubobjCtrl.Top = Subitem.Top; SubobjCtrl.Width = Subitem.Width; SubobjCtrl.Height = Subitem.Height; SubobjCtrl.FontSize = Subitem.Font.Size;
                            oldCtrl.Add(SubobjCtrl);
                            if (Subitem.Controls.Count > 0)
                            {
                                List<Control> result2 = FindInSubControl(Subitem);
                                foreach (Control Subitem2 in result2)
                                {
                                    SubobjCtrl2.Left = Subitem2.Left; SubobjCtrl2.Top = Subitem2.Top; SubobjCtrl2.Width = Subitem2.Width; SubobjCtrl2.Height = Subitem2.Height; SubobjCtrl2.FontSize = Subitem2.Font.Size;
                                    oldCtrl.Add(SubobjCtrl2);
                                    if (Subitem2.Controls.Count > 0)
                                    {
                                        List<Control> result3 = FindInSubControl(Subitem2);
                                        foreach (Control Subitem3 in result3)
                                        {
                                            SubobjCtrl3.Left = Subitem3.Left; SubobjCtrl3.Top = Subitem3.Top; SubobjCtrl3.Width = Subitem3.Width; SubobjCtrl3.Height = Subitem3.Height; SubobjCtrl3.FontSize = Subitem3.Font.Size;
                                            oldCtrl.Add(SubobjCtrl3);
                                            if (Subitem3.Controls.Count > 0)
                                            {
                                                List<Control> result4 = FindInSubControl(Subitem3);
                                                foreach (Control Subitem4 in result4)
                                                {
                                                    SubobjCtrl4.Left = Subitem4.Left; SubobjCtrl4.Top = Subitem4.Top; SubobjCtrl4.Width = Subitem4.Width; SubobjCtrl4.Height = Subitem4.Height; SubobjCtrl4.FontSize = Subitem4.Font.Size;
                                                    oldCtrl.Add(SubobjCtrl4);
                                                    if (Subitem4.Controls.Count > 0)
                                                    {
                                                        List<Control> result5 = FindInSubControl(Subitem4);
                                                        foreach (Control Subitem5 in result5)
                                                        {
                                                            SubobjCtrl5.Left = Subitem5.Left; SubobjCtrl5.Top = Subitem5.Top; SubobjCtrl5.Width = Subitem5.Width; SubobjCtrl5.Height = Subitem5.Height; SubobjCtrl5.FontSize = Subitem5.Font.Size;
                                                            oldCtrl.Add(SubobjCtrl5);
                                                            if (Subitem5.Controls.Count > 0)
                                                            {
                                                                List<Control> result6 = FindInSubControl(Subitem5);
                                                                foreach (Control Subitem6 in result6)
                                                                {
                                                                    SubobjCtrl6.Left = Subitem6.Left; SubobjCtrl6.Top = Subitem6.Top; SubobjCtrl6.Width = Subitem6.Width; SubobjCtrl6.Height = Subitem6.Height; SubobjCtrl6.FontSize = Subitem6.Font.Size;
                                                                    oldCtrl.Add(SubobjCtrl6);
                                                                    if (Subitem6.Controls.Count > 0)
                                                                    {
                                                                        List<Control> result7 = FindInSubControl(Subitem6);
                                                                        foreach (Control Subitem7 in result7)
                                                                        {
                                                                            SubobjCtrl7.Left = Subitem7.Left; SubobjCtrl7.Top = Subitem7.Top; SubobjCtrl7.Width = Subitem7.Width; SubobjCtrl7.Height = Subitem7.Height; SubobjCtrl7.FontSize = Subitem7.Font.Size;
                                                                            oldCtrl.Add(SubobjCtrl7);
                                                                            if (Subitem7.Controls.Count > 0)
                                                                            {
                                                                                List<Control> result8 = FindInSubControl(Subitem7);
                                                                                foreach (Control Subitem8 in result8)
                                                                                {
                                                                                    SubobjCtrl8.Left = Subitem8.Left; SubobjCtrl8.Top = Subitem8.Top; SubobjCtrl8.Width = Subitem8.Width; SubobjCtrl8.Height = Subitem8.Height; SubobjCtrl8.FontSize = Subitem8.Font.Size;
                                                                                    oldCtrl.Add(SubobjCtrl8);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("AutoSizeFormClass中函数controllInitializeSize出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("AutoSizeFormClass中函数controllInitializeSize出错" + ex);
#endif
            }
            //}
            // this.WindowState = (System.Windows.Forms.FormWindowState)(2);//记录完控件的初始位置和大小后，再最大化
            //0 - Normalize , 1 - Minimize,2- Maximize
        }

        ////记录控件容器中各个控件的位置与大小
        //private void GetControlSize(Control con)
        //{
        //    int s = con.Controls.Count;
        //    string name = con.Name;
        //    //记录控件的位置大小与字体大小
        //    foreach (Control c in con.Controls)
        //    {
        //        controlRect objCtrl;
        //        objCtrl.Left = c.Left; objCtrl.Top = c.Top; objCtrl.Width = c.Width; objCtrl.Height = c.Height; objCtrl.FontSize = c.Font.Size;
        //        oldCtrl.Add(objCtrl);
        //        if (c.GetType().ToString() == "System.Windows.Forms.Panel")
        //        {
        //            GetControlSize(c);
        //        }

        //    }

        //}

        private List<Control> FindInSubControl(Control parent)
        {

            List<Control> result = new List<Control>();
            try
            {
                foreach (Control c in parent.Controls)
                {
                    result.Add(c);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("AutoSizeFormClass中函数FindInSubControl出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("AutoSizeFormClass中函数FindInSubControl出错" + ex);
#endif
            }
            return result;


        }

        //(3.2)控件自适应大小,
        public void controlAutoSize(Form mForm)
        {
            try
            {
                //int wLeft0 = oldCtrl[0].Left; ;//窗体最初的位置
                //int wTop0 = oldCtrl[0].Top;
                ////int wLeft1 = this.Left;//窗体当前的位置
                //int wTop1 = this.Top;
                float wScale = (float)mForm.Width / (float)oldCtrl[0].Width;//新旧窗体之间的比例，与最早的旧窗体
                float hScale = (float)mForm.Height / (float)oldCtrl[0].Height;//.Height;
                int ctrLeft0, ctrTop0, ctrWidth0, ctrHeight0;
                float ctrFontSize0;
                int ctrlNo = 1;//第1个是窗体自身的 Left,Top,Width,Height，所以窗体控件从ctrlNo=1开始
                //SetSize(mForm, ctrlNo, wScale, hScale);
                foreach (Control c in mForm.Controls)
                {
                    ctrLeft0 = oldCtrl[ctrlNo].Left;
                    ctrTop0 = oldCtrl[ctrlNo].Top;
                    ctrWidth0 = oldCtrl[ctrlNo].Width;
                    ctrHeight0 = oldCtrl[ctrlNo].Height;
                    ctrFontSize0 = oldCtrl[ctrlNo].FontSize;
                    //c.Left = (int)((ctrLeft0 - wLeft0) * wScale) + wLeft1;//新旧控件之间的线性比例
                    //c.Top = (int)((ctrTop0 - wTop0) * h) + wTop1;
                    c.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体，所以不能加 + wLeft1
                    c.Top = (int)((ctrTop0) * hScale);//
                    c.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 (int)(c.Width * w);
                    c.Height = (int)(ctrHeight0 * hScale);//
                    c.Font = new Font(c.Font.Name, (float)(ctrFontSize0 * hScale), c.Font.Style);
                    ctrlNo += 1;
                    if (c.Controls.Count > 0)
                    {
                        List<Control> result = FindInSubControl(c);
                        foreach (Control Subitem in result)
                        {
                            ctrLeft0 = oldCtrl[ctrlNo].Left;
                            ctrTop0 = oldCtrl[ctrlNo].Top;
                            ctrWidth0 = oldCtrl[ctrlNo].Width;
                            ctrHeight0 = oldCtrl[ctrlNo].Height;
                            ctrFontSize0 = oldCtrl[ctrlNo].FontSize;

                            //c.Left = (int)((ctrLeft0 - wLeft0) * wScale) + wLeft1;//新旧控件之间的线性比例
                            //c.Top = (int)((ctrTop0 - wTop0) * h) + wTop1;
                            Subitem.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体，所以不能加 + wLeft1
                            Subitem.Top = (int)((ctrTop0) * hScale);//
                            Subitem.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 (int)(c.Width * w);
                            Subitem.Height = (int)(ctrHeight0 * hScale);//
                            Subitem.Font = new Font(Subitem.Font.Name, (float)(ctrFontSize0 * hScale), Subitem.Font.Style);
                            ctrlNo += 1;
                            if (Subitem.Controls.Count > 0)
                            {
                                List<Control> result2 = FindInSubControl(Subitem);
                                foreach (Control Subitem2 in result2)
                                {
                                    ctrLeft0 = oldCtrl[ctrlNo].Left;
                                    ctrTop0 = oldCtrl[ctrlNo].Top;
                                    ctrWidth0 = oldCtrl[ctrlNo].Width;
                                    ctrHeight0 = oldCtrl[ctrlNo].Height;
                                    ctrFontSize0 = oldCtrl[ctrlNo].FontSize;
                                    //c.Left = (int)((ctrLeft0 - wLeft0) * wScale) + wLeft1;//新旧控件之间的线性比例
                                    //c.Top = (int)((ctrTop0 - wTop0) * h) + wTop1;
                                    Subitem2.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体，所以不能加 + wLeft1
                                    Subitem2.Top = (int)((ctrTop0) * hScale);//
                                    Subitem2.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 (int)(c.Width * w);
                                    Subitem2.Height = (int)(ctrHeight0 * hScale);//
                                    Subitem2.Font = new Font(Subitem2.Font.Name, (float)(ctrFontSize0 * hScale), Subitem2.Font.Style);
                                    ctrlNo += 1;
                                    if (Subitem2.Controls.Count > 0)
                                    {
                                        List<Control> result3 = FindInSubControl(Subitem2);
                                        foreach (Control Subitem3 in result3)
                                        {
                                            ctrLeft0 = oldCtrl[ctrlNo].Left;
                                            ctrTop0 = oldCtrl[ctrlNo].Top;
                                            ctrWidth0 = oldCtrl[ctrlNo].Width;
                                            ctrHeight0 = oldCtrl[ctrlNo].Height;
                                            ctrFontSize0 = oldCtrl[ctrlNo].FontSize;
                                            //c.Left = (int)((ctrLeft0 - wLeft0) * wScale) + wLeft1;//新旧控件之间的线性比例
                                            //c.Top = (int)((ctrTop0 - wTop0) * h) + wTop1;
                                            Subitem3.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体，所以不能加 + wLeft1
                                            Subitem3.Top = (int)((ctrTop0) * hScale);//
                                            Subitem3.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 (int)(c.Width * w);
                                            Subitem3.Height = (int)(ctrHeight0 * hScale);//
                                            Subitem3.Font = new Font(Subitem3.Font.Name, (float)(ctrFontSize0 * hScale), Subitem3.Font.Style);
                                            ctrlNo += 1;
                                            if (Subitem3.Controls.Count > 0)
                                            {
                                                List<Control> result4 = FindInSubControl(Subitem3);
                                                foreach (Control Subitem4 in result4)
                                                {
                                                    ctrLeft0 = oldCtrl[ctrlNo].Left;
                                                    ctrTop0 = oldCtrl[ctrlNo].Top;
                                                    ctrWidth0 = oldCtrl[ctrlNo].Width;
                                                    ctrHeight0 = oldCtrl[ctrlNo].Height;
                                                    ctrFontSize0 = oldCtrl[ctrlNo].FontSize;
                                                    //c.Left = (int)((ctrLeft0 - wLeft0) * wScale) + wLeft1;//新旧控件之间的线性比例
                                                    //c.Top = (int)((ctrTop0 - wTop0) * h) + wTop1;
                                                    Subitem4.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体，所以不能加 + wLeft1
                                                    Subitem4.Top = (int)((ctrTop0) * hScale);//
                                                    Subitem4.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 (int)(c.Width * w);
                                                    Subitem4.Height = (int)(ctrHeight0 * hScale);//
                                                    Subitem4.Font = new Font(Subitem4.Font.Name, (float)(ctrFontSize0 * hScale), Subitem4.Font.Style);
                                                    ctrlNo += 1;
                                                    if (Subitem4.Controls.Count > 0)
                                                    {
                                                        List<Control> result5 = FindInSubControl(Subitem4);
                                                        foreach (Control Subitem5 in result5)
                                                        {
                                                            ctrLeft0 = oldCtrl[ctrlNo].Left;
                                                            ctrTop0 = oldCtrl[ctrlNo].Top;
                                                            ctrWidth0 = oldCtrl[ctrlNo].Width;
                                                            ctrHeight0 = oldCtrl[ctrlNo].Height;
                                                            ctrFontSize0 = oldCtrl[ctrlNo].FontSize;
                                                            //c.Left = (int)((ctrLeft0 - wLeft0) * wScale) + wLeft1;//新旧控件之间的线性比例
                                                            //c.Top = (int)((ctrTop0 - wTop0) * h) + wTop1;
                                                            Subitem5.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体，所以不能加 + wLeft1
                                                            Subitem5.Top = (int)((ctrTop0) * hScale);//
                                                            Subitem5.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 (int)(c.Width * w);
                                                            Subitem5.Height = (int)(ctrHeight0 * hScale);//
                                                            Subitem5.Font = new Font(Subitem5.Font.Name, (float)(ctrFontSize0 * hScale), Subitem5.Font.Style);
                                                            ctrlNo += 1;
                                                            if (Subitem5.Controls.Count > 0)
                                                            {
                                                                List<Control> result6 = FindInSubControl(Subitem5);
                                                                foreach (Control Subitem6 in result6)
                                                                {
                                                                    ctrLeft0 = oldCtrl[ctrlNo].Left;
                                                                    ctrTop0 = oldCtrl[ctrlNo].Top;
                                                                    ctrWidth0 = oldCtrl[ctrlNo].Width;
                                                                    ctrHeight0 = oldCtrl[ctrlNo].Height;
                                                                    ctrFontSize0 = oldCtrl[ctrlNo].FontSize;
                                                                    //c.Left = (int)((ctrLeft0 - wLeft0) * wScale) + wLeft1;//新旧控件之间的线性比例
                                                                    //c.Top = (int)((ctrTop0 - wTop0) * h) + wTop1;
                                                                    Subitem6.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体，所以不能加 + wLeft1
                                                                    Subitem6.Top = (int)((ctrTop0) * hScale);//
                                                                    Subitem6.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 (int)(c.Width * w);
                                                                    Subitem6.Height = (int)(ctrHeight0 * hScale);//
                                                                    Subitem6.Font = new Font(Subitem6.Font.Name, (float)(ctrFontSize0 * hScale), Subitem6.Font.Style);
                                                                    ctrlNo += 1;
                                                                    if (Subitem6.Controls.Count > 0)
                                                                    {
                                                                        List<Control> result7 = FindInSubControl(Subitem6);
                                                                        foreach (Control Subitem7 in result7)
                                                                        {
                                                                            ctrLeft0 = oldCtrl[ctrlNo].Left;
                                                                            ctrTop0 = oldCtrl[ctrlNo].Top;
                                                                            ctrWidth0 = oldCtrl[ctrlNo].Width;
                                                                            ctrHeight0 = oldCtrl[ctrlNo].Height;
                                                                            ctrFontSize0 = oldCtrl[ctrlNo].FontSize;
                                                                            //c.Left = (int)((ctrLeft0 - wLeft0) * wScale) + wLeft1;//新旧控件之间的线性比例
                                                                            //c.Top = (int)((ctrTop0 - wTop0) * h) + wTop1;
                                                                            Subitem7.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体，所以不能加 + wLeft1
                                                                            Subitem7.Top = (int)((ctrTop0) * hScale);//
                                                                            Subitem7.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 (int)(c.Width * w);
                                                                            Subitem7.Height = (int)(ctrHeight0 * hScale);//
                                                                            Subitem7.Font = new Font(Subitem7.Font.Name, (float)(ctrFontSize0 * hScale), Subitem7.Font.Style);
                                                                            ctrlNo += 1;
                                                                            if (Subitem7.Controls.Count > 0)
                                                                            {
                                                                                List<Control> result8 = FindInSubControl(Subitem7);
                                                                                foreach (Control Subitem8 in result8)
                                                                                {
                                                                                    ctrLeft0 = oldCtrl[ctrlNo].Left;
                                                                                    ctrTop0 = oldCtrl[ctrlNo].Top;
                                                                                    ctrWidth0 = oldCtrl[ctrlNo].Width;
                                                                                    ctrHeight0 = oldCtrl[ctrlNo].Height;
                                                                                    ctrFontSize0 = oldCtrl[ctrlNo].FontSize;
                                                                                    //c.Left = (int)((ctrLeft0 - wLeft0) * wScale) + wLeft1;//新旧控件之间的线性比例
                                                                                    //c.Top = (int)((ctrTop0 - wTop0) * h) + wTop1;
                                                                                    Subitem8.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体，所以不能加 + wLeft1
                                                                                    Subitem8.Top = (int)((ctrTop0) * hScale);//
                                                                                    Subitem8.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 (int)(c.Width * w);
                                                                                    Subitem8.Height = (int)(ctrHeight0 * hScale);//
                                                                                    Subitem8.Font = new Font(Subitem8.Font.Name, (float)(ctrFontSize0 * hScale), Subitem8.Font.Style);
                                                                                    ctrlNo += 1;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("AutoSizeFormClass中函数controlAutoSize出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("AutoSizeFormClass中函数controlAutoSize出错" + ex);
#endif
            }

            //private void SetSize(Control con, int ctrlNo, float wScale, float hScale)
            //{
            //    int ctrLeft0, ctrTop0, ctrWidth0, ctrHeight0;
            //    float ctrFontSize0;
            //    int s = con.Controls.Count;
            //    string name = con.Name;
            //    foreach (Control c in con.Controls)
            //    {
            //        ctrLeft0 = oldCtrl[ctrlNo].Left;
            //        ctrTop0 = oldCtrl[ctrlNo].Top;
            //        ctrWidth0 = oldCtrl[ctrlNo].Width;
            //        ctrHeight0 = oldCtrl[ctrlNo].Height;
            //        ctrFontSize0 = oldCtrl[ctrlNo].FontSize;

            //        c.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体，所以不能加 + wLeft1
            //        c.Top = (int)((ctrTop0) * hScale);//
            //        c.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 (int)(c.Width * w);
            //        c.Height = (int)(ctrHeight0 * hScale);//
            //        c.Font = new Font(c.Font.Name,(float)(ctrFontSize0 * hScale));
            //        ctrlNo += 1;
            //        if (ctrlNo >= oldCtrl.Count) return;
            //        if (c.GetType().ToString() == "System.Windows.Forms.Panel")
            //        {
            //            SetSize(c, ctrlNo, wScale, hScale);
            //        }

            //    }
            //}
        }
    }

}
