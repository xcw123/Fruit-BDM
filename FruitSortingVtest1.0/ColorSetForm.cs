using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interface;
using System.Drawing;
using Common;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Resources;
using System.Drawing.Imaging;

namespace FruitSortingVtest1._0
{
    public partial class QualityParamSetForm : Form
    {
        int m_nColorType = 0;
        //int[] m_ColorIntervals = new int[2];
        //stColorIntervalItem[] m_intervals = new stColorIntervalItem[ConstPreDefine.MAX_COLOR_INTERVAL_NUM];
        stPercentInfo[] m_percent = new stPercentInfo[ConstPreDefine.MAX_COLOR_GRADE_NUM * ConstPreDefine.MAX_COLOR_INTERVAL_NUM];
        byte[] m_ColorGradeName = new byte[ConstPreDefine.MAX_COLOR_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        int m_MinGray = 140; //对比亮度
        double m_BrightRatio = 1.00; //Add by ChengSk - 20190830
        byte[] m_ColorRGBImage = new byte[256 * 256 * 4];
        float[] m_slider = new float[2];//滑块位置
        float m_ChangeRadio;//图像转换比例
        bool m_SliderCatched = false;//标指示抓住
        bool m_MouseDown = false;//标指示抓住
        float m_tempSlider = 0.0f;//虚线位置
        int m_SliderCatchedType = 0;//1, min;2，max
        private Control[] ColorSetEditors;//颜色列表点击显示控件
        Color[] m_ColorList = new Color[3] { Color.Red, Color.Lime, Color.Blue };//颜色列表背景颜色
        bool IsMouseRight = false; //鼠标右键点击事件
        Rect[] m_UVRect = new Rect[3];
        int m_UVRectCurrentIndex = 0;//当前UV图方框序列
        //int[] m_ChannelNum = new int[ConstPreDefine.MAX_SUBSYS_NUM];//每个子系统通道数
        List<int> m_ChanelIDList = new List<int>();//所有通道ID
       // List<ToolStripMenuItem> m_toolStripMenuItem = new List<ToolStripMenuItem>();
        int m_CurrentIPM_ID = 0;
        byte[] m_fruitRGBImage;// = new byte[ConstPreDefine.CAPTURE_WIDTH * ConstPreDefine.CAPTURE_HEIGHT * 4];
        byte[] m_srcfruitRGBImage;   //保存原图 Add by ChengSk - 20190902
        int m_CurrentChannelIndex = -1;
        stSpliceImageData m_spliceImageData;//当前IPM上传图像信息
        byte[] m_spliceImageBin = null;     //Add by ChengSk - 20190827
        float m_colorInterval = 0.0f;//获取图像后的到的颜色区间
        Rect[] m_UVColorRect = new Rect[2];//0为水果图片的框，1为点击颜色区间后的所属颜色区间位置
        bool m_fruitImageMouseDown = false;//标指示抓住
        Bitmap m_FruitImage;
        stBGR m_TagBGR = new stBGR(true);
        int ChannelNum = 0;  //点击“获取图像”后出现右键菜单，里面有ChannelNum个通道 Add by ChengSk - 20180601
        int nLengtha = Marshal.SizeOf(typeof(stSpliceImageInfo));
        private void ColorFormInitial()
        {
            try
            {
                int num = 0;
                this.ChannelcontextMenuStrip.Items.Clear();
                //统计每个子系统的通道数
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    {
                        //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                        if (j < GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i])  //Modify by ChengSk - 20190521
                        {
                            //m_ChannelNum[i]++;
                            if (GlobalDataInterface.nVer == 0) //Modify by xcw - 20200619
                            {
                                m_ChanelIDList.Add(Commonfunction.EncodeChannel(i, j, j));
                            }
                            else if (GlobalDataInterface.nVer == 1)
                            {
                                m_ChanelIDList.Add(Commonfunction.EncodeChannel(i, j / 2, j % 2));
                            }
                            num++;
                            this.ChannelcontextMenuStrip.Items.Add(m_resourceManager.GetString("Lanelabel.Text")+string.Format(" {0}", num));
                            this.ChannelcontextMenuStrip.Items[num - 1].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo)
                            //{
                            //    case ConstPreDefine.RM_M:
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num-1]).DropDownItems.Add("Color Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        break;
                            //    case ConstPreDefine.RM_LMR:
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        break;
                            //    case ConstPreDefine.RM_F_M:
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[1].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        break;
                            //    case ConstPreDefine.RM_LFR_LMR:
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[1].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        break;
                            //    case ConstPreDefine.RM_F_M_B:
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[1].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-B Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[2].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        break;
                            //    case ConstPreDefine.RM_LFR_LMR_LBR:
                            //       ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                            //       ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[1].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-B Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[2].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        break;
                            //    case ConstPreDefine.RM_LR:
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        break;
                            //    case ConstPreDefine.RM_F:
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        break;
                            //    case ConstPreDefine.RM_LFR:
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        break;
                            //    case ConstPreDefine.RM_F_B:
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-B Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[1].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        break;
                            //    case ConstPreDefine.RM_LFR_LBR:
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-B Camera");
                            //        ((ToolStripDropDownItem)this.ChannelcontextMenuStrip.Items[num - 1]).DropDownItems[1].Click += new EventHandler(ChannelcontextMenuStrip_Click);
                            //        break;
                            //    default: break;
                            //}
                            
                        }
                    }
                }
                ChannelNum = num; //Add by ChengSk - 20180601

                m_ChangeRadio = this.ColorpictureBox.Height / 100.0f * 1.5f;
                m_slider[0] = (GlobalDataInterface.globalOut_GradeInfo.ColorIntervals[1] + 2) * m_ChangeRadio;
                m_slider[1] = (GlobalDataInterface.globalOut_GradeInfo.ColorIntervals[0] + 2) * m_ChangeRadio ;
                m_nColorType = GlobalDataInterface.globalOut_GradeInfo.ColorType;

                //m_nColorType = 1;//
                //GlobalDataInterface.ColorIntervalNum = 3;
                //GlobalDataInterface.globalOut_GradeInfo.ColorIntervals.CopyTo(m_ColorIntervals, 0);
                float UVChangeRadio = this.ColorpictureBox.Height / 256.0f;
                for (int i = 0; i < ConstPreDefine.MAX_COLOR_INTERVAL_NUM; i++)
                {
                    m_UVRect[i].Left = (int)(GlobalDataInterface.globalOut_GradeInfo.intervals[i].nMinV * UVChangeRadio);
                    m_UVRect[i].Top = (int)(GlobalDataInterface.globalOut_GradeInfo.intervals[i].nMinU * UVChangeRadio);
                    m_UVRect[i].Right = (int)(GlobalDataInterface.globalOut_GradeInfo.intervals[i].nMaxV * UVChangeRadio);
                    if (m_UVRect[i].Right > this.ColorpictureBox.Width)
                        m_UVRect[i].Right = this.ColorpictureBox.Width;
                    m_UVRect[i].Bottom = (int)(GlobalDataInterface.globalOut_GradeInfo.intervals[i].nMaxU * UVChangeRadio);
                    if (m_UVRect[i].Bottom > this.ColorpictureBox.Height)
                        m_UVRect[i].Bottom = this.ColorpictureBox.Height;
                }
                for (int i = 0; i < ConstPreDefine.MAX_COLOR_GRADE_NUM * ConstPreDefine.MAX_COLOR_INTERVAL_NUM; i++)
                {
                    m_percent[i].ToCopy(GlobalDataInterface.globalOut_GradeInfo.percent[i]);

                    // Update By ChengSk 20140626
                    m_percent[i].nMin = (byte)((int)m_percent[i].nMin / 2);
                    m_percent[i].nMax = (byte)((int)m_percent[i].nMax / 2);
                    //End
                }
                GlobalDataInterface.globalOut_GradeInfo.strColorGradeName.CopyTo(m_ColorGradeName, 0);
                if ((m_nColorType & 0x08) <= 0)//平均值
                {
                    switch (m_nColorType & 0x7)
                    {
                        case 1://灰度
                            this.ImageTypecomboBox.SelectedIndex = 2;
                            this.ColorNumnumericUpDown.Maximum = 255;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x01);
                            break;
                        case 2://H
                            this.ImageTypecomboBox.SelectedIndex = 1;
                            this.ColorNumnumericUpDown.Maximum = 360;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x02);
                            break;
                        case 4://UV
                            this.ImageTypecomboBox.SelectedIndex = 0;
                            this.ColorNumnumericUpDown.Maximum = 255;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x04);
                            break;
                        default: break;

                    }
                }
                else//百分比
                {
                    switch (m_nColorType & 0x7)
                    {
                        case 1://灰度
                            this.ImageTypecomboBox.SelectedIndex = 2;
                            this.ColorNumnumericUpDown.Maximum = 100;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x01);
                            break;
                        case 2://H
                            this.ImageTypecomboBox.SelectedIndex = 1;
                            this.ColorNumnumericUpDown.Maximum = 100;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x02);
                            break;
                        case 4://UV
                            this.ImageTypecomboBox.SelectedIndex = 0;
                            this.ColorNumnumericUpDown.Maximum = 100;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x04);
                            break;
                        default: break;

                    }
                }

                string color;
                color = Commonfunction.GetAppSetting("颜色参数-颜色3");  //颜色1->颜色3  Modify by ChengSk - 20181204
                m_ColorList[0] = ColorTranslator.FromHtml(color);
                color = Commonfunction.GetAppSetting("颜色参数-颜色2");
                m_ColorList[1] = ColorTranslator.FromHtml(color);
                color = Commonfunction.GetAppSetting("颜色参数-颜色1");  //颜色3->颜色1  Modify by ChengSk - 20181204
                m_ColorList[2] = ColorTranslator.FromHtml(color);

                ColorlistViewExSetParam();
                m_MinGray = GlobalDataInterface.ColorMinGray;
                //this.MinGraynumericUpDown.Text = m_MinGray.ToString(); //对比度
                m_BrightRatio = double.Parse(Commonfunction.GetAppSetting("对比亮度"));
                this.MinGraynumericUpDown.Text = m_BrightRatio.ToString();   //亮度 Modify by ChengSk - 20190830

                bool bTagDemarcateEnabled = /*Commonfunction.GetAppSetting("标签启用")=="1"*/GlobalDataInterface.globalOut_GradeInfo.nTagInfo[0] == (byte)1 ? true:false;//modify by xcw 20200709
                this.TagDemarcatebutton.Enabled = bTagDemarcateEnabled;
                this.TagDemarcatecheckBox.Checked = bTagDemarcateEnabled;
                
                ColorSetEditors = new Control[] { this.ColorNametextBox, this.ColorNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorFormInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorFormInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 自定义画SubItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorlistViewEx_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawText();
        }

        /// <summary>
        /// 自定义画列表头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorlistViewEx_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawBackground();
            e.DrawText();
             
        }

        /// <summary>
        /// 画颜色图片控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorpictureBox_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics graphics = e.Graphics;//创建画板
                Image image;
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                string name = myAssembly.GetName().Name;
                Stream myStream;
                int width = 256;
                int height = 256;
                int stride = width * 4;
                Pen penSlider;
                PointF[] triangle = new PointF[3];
                Pen pen;
                if ((m_nColorType & 0x08) > 0)//百分比
                {
                    switch (m_nColorType & 0x7)
                    {
                        case 1://灰度
                            myStream = myAssembly.GetManifestResourceStream("FruitSortingVtest1.bmp.Y.bmp");
                            image = new Bitmap(myStream);
                            //ChangeRadio = this.ColorpictureBox.Height/100.0f;
                            this.ColorpictureBox.Width = (int)(262 * m_ChangeRadio);
                            graphics.DrawImage(image, 2 * m_ChangeRadio, 0, this.ColorpictureBox.Width - 6 * m_ChangeRadio, this.ColorpictureBox.Height);
                            PaintScale(graphics, 256, m_ChangeRadio, Color.Goldenrod);
                            penSlider = new Pen(Color.LawnGreen, 1);//画标尺
                            graphics.DrawLine(penSlider, m_slider[0], 0, m_slider[0], this.ColorpictureBox.Height);//画指示线
                            triangle[0] = new PointF(m_slider[0], this.ColorpictureBox.Height / 2 + 1);
                            triangle[1] = new PointF(m_slider[0] - 4, this.ColorpictureBox.Height / 2 + 15);
                            triangle[2] = new PointF(m_slider[0] + 4, this.ColorpictureBox.Height / 2 + 15);
                            graphics.FillPolygon(new SolidBrush(Color.Red), triangle);//画指示标


                            graphics.DrawLine(penSlider, m_slider[1], 0, m_slider[1], this.ColorpictureBox.Height);//画指示线
                            triangle[0] = new PointF(m_slider[1], this.ColorpictureBox.Height / 2 + 1);
                            triangle[1] = new PointF(m_slider[1] - 4, this.ColorpictureBox.Height / 2 + 15);
                            triangle[2] = new PointF(m_slider[1] + 4, this.ColorpictureBox.Height / 2 + 15);
                            graphics.FillPolygon(new SolidBrush(Color.Red), triangle);//画指示标

                            //画临时移动线
                            if (m_SliderCatched)
                            {
                                pen = new Pen(Color.Yellow, 1);
                                graphics.DrawLine(pen, m_tempSlider, 0, m_tempSlider, this.ColorpictureBox.Height);
                            }
                            break;
                        case 2://H
                            myStream = myAssembly.GetManifestResourceStream("FruitSortingVtest1.bmp.H.bmp");
                            image = new Bitmap(myStream);
                            this.ColorpictureBox.Width = (int)(366 * m_ChangeRadio);
                            graphics.DrawImage(image, 2 * m_ChangeRadio, 0, this.ColorpictureBox.Width - 6 * m_ChangeRadio, this.ColorpictureBox.Height);
                            PaintScale(graphics, 360, m_ChangeRadio, Color.Black);
                            penSlider = new Pen(Color.LawnGreen, 1);//画标尺
                            graphics.DrawLine(penSlider, m_slider[0], 0, m_slider[0], this.ColorpictureBox.Height);//画指示线
                            triangle[0] = new PointF(m_slider[0], this.ColorpictureBox.Height / 2 + 1);
                            triangle[1] = new PointF(m_slider[0] - 4, this.ColorpictureBox.Height / 2 + 15);
                            triangle[2] = new PointF(m_slider[0] + 4, this.ColorpictureBox.Height / 2 + 15);
                            graphics.FillPolygon(new SolidBrush(Color.Red), triangle);//画指示标


                            graphics.DrawLine(penSlider, m_slider[1], 0, m_slider[1], this.ColorpictureBox.Height);//画指示线
                            triangle[0] = new PointF(m_slider[1], this.ColorpictureBox.Height / 2 + 1);
                            triangle[1] = new PointF(m_slider[1] - 4, this.ColorpictureBox.Height / 2 + 15);
                            triangle[2] = new PointF(m_slider[1] + 4, this.ColorpictureBox.Height / 2 + 15);
                            graphics.FillPolygon(new SolidBrush(Color.Red), triangle);//画指示标

                            //画临时移动线
                            if (m_SliderCatched)
                            {
                                pen = new Pen(Color.Yellow, 1);
                                graphics.DrawLine(pen, m_tempSlider, 0, m_tempSlider, this.ColorpictureBox.Height);
                            }
                            break;
                        case 4://UV
                            Commonfunction.IniColorChartElem(m_MinGray, ref m_ColorRGBImage);
                            GCHandle handle;
                            IntPtr scan;

                            handle = GCHandle.Alloc(m_ColorRGBImage, GCHandleType.Pinned);
                            scan = handle.AddrOfPinnedObject();
                            image = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppRgb, scan);
                            float UVChangeRadio = this.ColorpictureBox.Height / 256.0f;
                            this.ColorpictureBox.Width = (int)(256 * UVChangeRadio);
                            graphics.DrawImage(image, 0, 0, this.ColorpictureBox.Width, this.ColorpictureBox.Height);
                            handle.Free();

                            //画框
                            Rectangle rect;
                            for (int i = 0; i < 3; i++)
                            {
                                rect = new Rectangle(m_UVRect[i].Left, m_UVRect[i].Top, m_UVRect[i].Right - m_UVRect[i].Left, m_UVRect[i].Bottom - m_UVRect[i].Top);
                                pen = new Pen(m_ColorList[i], 1);
                                graphics.DrawRectangle(pen, rect);
                                if (!(m_UVRect[i].Left == 0 && m_UVRect[i].Top == 0))
                                    graphics.DrawString((i + 1).ToString(), new Font("TimesNewRowman", 8), new SolidBrush(m_ColorList[i]), m_UVRect[i].Left + 2, m_UVRect[i].Top + 2);
                            }

                            //画颜色区间
                            rect = new Rectangle((int)(m_UVColorRect[1].Left * UVChangeRadio), (int)(m_UVColorRect[1].Top * UVChangeRadio), (int)((m_UVColorRect[1].Right - m_UVColorRect[1].Left) * UVChangeRadio), (int)((m_UVColorRect[1].Bottom - m_UVColorRect[1].Top) * UVChangeRadio));
                            pen = new Pen(Color.Black, 2);
                            graphics.DrawRectangle(pen, rect);
                            break;
                        default: break;

                    }

                }
                else //平均值
                {
                    switch (m_nColorType & 0x7)
                    {
                        case 1://灰度
                            myStream = myAssembly.GetManifestResourceStream("FruitSortingVtest1.bmp.Y.bmp");
                            image = new Bitmap(myStream);
                            //ChangeRadio = this.ColorpictureBox.Height/100.0f;
                            this.ColorpictureBox.Width = (int)(262 * m_ChangeRadio);
                            graphics.DrawImage(image, 2 * m_ChangeRadio, 0, this.ColorpictureBox.Width - 6 * m_ChangeRadio, this.ColorpictureBox.Height);
                            PaintScale(graphics, 256, m_ChangeRadio, Color.Goldenrod);
                            break;
                        case 2://H
                            myStream = myAssembly.GetManifestResourceStream("FruitSortingVtest1.bmp.H.bmp");
                            image = new Bitmap(myStream);
                            this.ColorpictureBox.Width = (int)(366 * m_ChangeRadio);
                            graphics.DrawImage(image, 2 * m_ChangeRadio, 0, this.ColorpictureBox.Width - 6 * m_ChangeRadio, this.ColorpictureBox.Height);
                            PaintScale(graphics, 360, m_ChangeRadio, Color.Black);
                            break;
                        case 4://UV
                            Commonfunction.IniColorChartElem(m_MinGray, ref m_ColorRGBImage);
                            GCHandle handle;
                            IntPtr scan;

                            handle = GCHandle.Alloc(m_ColorRGBImage, GCHandleType.Pinned);
                            scan = handle.AddrOfPinnedObject();
                            image = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppRgb, scan);
                            // ChangeRadio = this.ColorpictureBox.Height/100.0f;
                            this.ColorpictureBox.Width = (int)((256 + 6) * m_ChangeRadio);
                            graphics.DrawImage(image, 2 * m_ChangeRadio, 0, this.ColorpictureBox.Width - 6 * m_ChangeRadio, this.ColorpictureBox.Height);
                            handle.Free();
                            PaintScale(graphics, 256, m_ChangeRadio, Color.Black);
                            break;
                        default: break;

                    }
                    penSlider = new Pen(Color.LawnGreen, 1);//画标尺
                    graphics.DrawLine(penSlider, m_slider[0], 0, m_slider[0], this.ColorpictureBox.Height);//画指示线
                    triangle[0] = new PointF(m_slider[0], this.ColorpictureBox.Height / 2 + 1);
                    triangle[1] = new PointF(m_slider[0] - 4, this.ColorpictureBox.Height / 2 + 15);
                    triangle[2] = new PointF(m_slider[0] + 4, this.ColorpictureBox.Height / 2 + 15);
                    graphics.FillPolygon(new SolidBrush(Color.Red), triangle);//画指示标


                    graphics.DrawLine(penSlider, m_slider[1], 0, m_slider[1], this.ColorpictureBox.Height);//画指示线
                    triangle[0] = new PointF(m_slider[1], this.ColorpictureBox.Height / 2 + 1);
                    triangle[1] = new PointF(m_slider[1] - 4, this.ColorpictureBox.Height / 2 + 15);
                    triangle[2] = new PointF(m_slider[1] + 4, this.ColorpictureBox.Height / 2 + 15);
                    graphics.FillPolygon(new SolidBrush(Color.Red), triangle);//画指示标

                    //画颜色区间指示线
                    if (m_colorInterval > 0)
                    {
                        penSlider = new Pen(Color.Black, 1);
                        graphics.DrawLine(penSlider, m_colorInterval, 0, m_colorInterval, this.ColorpictureBox.Height);
                    }

                    //画临时移动线
                    if (m_SliderCatched)
                    {
                        pen = new Pen(Color.Yellow, 1);
                        graphics.DrawLine(pen, m_tempSlider, 0, m_tempSlider, this.ColorpictureBox.Height);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorpictureBox_Paint出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorpictureBox_Paint出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 画图
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="length"></param>
        /// <param name="ChangeRadio"></param>
        /// <param name="color"></param>
        private void PaintScale(Graphics graphics, int length, float ChangeRadio,Color color)
        {
            try
            {
                float interval = 5 * ChangeRadio;
                int num = length / 5;
                Pen pen = new Pen(color, 1);
                for (int i = 1; i <= num + 1; i++)
                {
                    if (i % 2 != 0)
                    {
                        graphics.DrawLine(pen, (i - 1) * interval + 2 * ChangeRadio - 0.25f, this.ColorpictureBox.Height / 2, (i - 1) * interval + 2 * ChangeRadio - 0.25f, this.ColorpictureBox.Height / 2 - 8);
                        if (((i - 1) * 5) < 10)
                            graphics.DrawString(((i - 1) * 5).ToString(), new Font("TimesNewRowman", 6), new SolidBrush(color), (i - 1) * interval + 2 * ChangeRadio - 3, this.ColorpictureBox.Height / 2 - 20);
                        else if (((i - 1) * 5) < 100)
                            graphics.DrawString(((i - 1) * 5).ToString(), new Font("TimesNewRowman", 6), new SolidBrush(color), (i - 1) * interval + 2 * ChangeRadio - 5, this.ColorpictureBox.Height / 2 - 20);
                        else
                            graphics.DrawString(((i - 1) * 5).ToString(), new Font("TimesNewRowman", 6), new SolidBrush(color), (i - 1) * interval + 2 * ChangeRadio - 8, this.ColorpictureBox.Height / 2 - 20);
                    }
                    else
                        graphics.DrawLine(pen, (i - 1) * interval + 2 * ChangeRadio - 0.25f, this.ColorpictureBox.Height / 2, (i - 1) * interval + 2 * ChangeRadio - 0.25f, this.ColorpictureBox.Height / 2 - 4);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数PaintScale出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数PaintScale出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 鼠标放开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorpictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (GlobalDataInterface.qualgradeSet)
                {
                    GlobalDataInterface.qualgradeSet = false;
                    return;
                }
                PictureBox pictureBox = (PictureBox)sender;
                Graphics graphics = pictureBox.CreateGraphics();//创建画板
                if (((m_nColorType & 0x08) > 0) && ((m_nColorType & 0x7) == 4))
                {
                    m_UVRect[m_UVRectCurrentIndex].Right = e.X;
                    if (m_UVRect[m_UVRectCurrentIndex].Right > this.ColorpictureBox.Width)
                        m_UVRect[m_UVRectCurrentIndex].Right = this.ColorpictureBox.Width;
                    m_UVRect[m_UVRectCurrentIndex].Bottom = e.Y;
                    if (m_UVRect[m_UVRectCurrentIndex].Bottom > this.ColorpictureBox.Height)
                        m_UVRect[m_UVRectCurrentIndex].Bottom = this.ColorpictureBox.Height;
                    pictureBox.Invalidate();
                    m_MouseDown = false;
                    ColorPictureChange();
                }
                else
                {
                    if (m_SliderCatched)
                    {
                        if (m_SliderCatchedType == 1)
                        {
                            m_slider[1] = m_tempSlider;
                        }
                        else
                        {
                            m_slider[0] = m_tempSlider;
                        }
                        pictureBox.Invalidate();
                        m_SliderCatched = false;
                        if ((m_nColorType & 0x08) <= 0)
                        {
                            if (m_slider[1] > m_slider[0])
                            {
                                float temp = m_slider[0];
                                m_slider[0] = m_slider[1];
                                m_slider[1] = temp;
                            }

                            this.ColorlistViewEx.Items[0].SubItems[1].Text = ((int)(m_slider[0] / m_ChangeRadio - 2+0.5)).ToString();
                            this.ColorlistViewEx.Items[0].SubItems[1].BackColor = m_ColorList[0];
                            this.ColorlistViewEx.Items[1].SubItems[1].Text = ((int)(m_slider[1]/m_ChangeRadio - 2+0.5)).ToString();
                            this.ColorlistViewEx.Items[1].SubItems[1].BackColor = m_ColorList[1];

                        }
                        ColorPictureChange();
                    }
                }
                m_MouseDown = false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorpictureBox_MouseUp出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorpictureBox_MouseUp出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorpictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (GlobalDataInterface.qualgradeSet)
                {
                    return;
                }
                PictureBox pictureBox = (PictureBox)sender;

                Graphics graphics = pictureBox.CreateGraphics();//创建画板
                if (((m_nColorType & 0x08) > 0) && ((m_nColorType & 0x7) == 4))
                {
                    if (m_MouseDown)
                    {
                        m_UVRect[m_UVRectCurrentIndex].Right = e.X;
                        m_UVRect[m_UVRectCurrentIndex].Bottom = e.Y;
                        pictureBox.Invalidate();
                    }

                }
                else
                {
                    if ((e.X >= m_slider[0] - 2) && (e.X <= m_slider[0] + 2) && (e.Y >= this.ColorpictureBox.Height / 2 + 1) && (e.Y <= this.ColorpictureBox.Height / 2 + 15))
                    {
                        if (m_MouseDown && !m_SliderCatched)
                        {
                            m_tempSlider = m_slider[0];
                            m_SliderCatchedType = 2;
                            m_SliderCatched = true;
                        }
                    }
                    else if ((e.X >= m_slider[1] - 2) && (e.X <= m_slider[1] + 2) && (e.Y >= this.ColorpictureBox.Height / 2 + 1) && (e.Y <= this.ColorpictureBox.Height / 2 + 15))
                    {
                        if (m_MouseDown && !m_SliderCatched)
                        {
                            m_tempSlider = m_slider[1];
                            m_SliderCatchedType = 1;
                            m_SliderCatched = true;
                        }
                    }
                    else if (m_SliderCatched)
                    {
                        // Pen pen;

                        System.Threading.Thread.Sleep(7);
                        //跟随鼠标画线
                        // pen = new Pen(Color.Yellow, 1);
                        // pen.DashPattern = new float[] { 4, 2 };//自定义虚线（短线8，空白2）
                        if (e.X >= m_ChangeRadio * 2 && e.X <= pictureBox.Width - m_ChangeRadio * 4)
                        {
                            m_tempSlider = e.X;
                            //  graphics.DrawLine(pen, m_tempSlider, 0, m_tempSlider, pictureBox.Height);
                        }
                        pictureBox.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorpictureBox_MouseMove出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorpictureBox_MouseMove出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorpictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (GlobalDataInterface.qualgradeSet)
                {
                    return;
                }
                if (((m_nColorType & 0x08) > 0) && ((m_nColorType & 0x7) == 4))
                {
                    m_UVRect[m_UVRectCurrentIndex].Left = e.X;
                    m_UVRect[m_UVRectCurrentIndex].Top = e.Y;
                }
                m_MouseDown = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorpictureBox_MouseDown出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorpictureBox_MouseDown出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 图片选择控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageTypecomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;
                m_nColorType = ((m_nColorType & 0x0F8) | (1 << (2 - comboBox.SelectedIndex)));
                if ((m_nColorType & 0x08) <= 0)//平均值
                {
                    switch (this.ImageTypecomboBox.SelectedIndex)
                    {
                        case 2://灰度
                            if ((m_slider[0] / m_ChangeRadio - 2) > 255)
                            {
                                m_slider[0] = (255 + 2) * m_ChangeRadio;
                                this.ColorImagepictureBox.Invalidate(); 
                            }
                            if ((m_slider[1] / m_ChangeRadio - 2) > 255)
                            {
                                m_slider[1] = (255 + 2) * m_ChangeRadio;
                                this.ColorImagepictureBox.Invalidate(); ;
                            }
                            if (m_slider[0] > m_slider[1])
                            {
                                this.ColorlistViewEx.Items[0].SubItems[1].Text = ((int)(m_slider[0] / m_ChangeRadio - 2 + 0.5)).ToString();
                                this.ColorlistViewEx.Items[1].SubItems[1].Text = ((int)(m_slider[1] / m_ChangeRadio - 2 + 0.5)).ToString();
                            }
                            else
                            {
                                this.ColorlistViewEx.Items[1].SubItems[1].Text = ((int)(m_slider[0] / m_ChangeRadio - 2)).ToString();
                                this.ColorlistViewEx.Items[0].SubItems[1].Text = ((int)(m_slider[1] / m_ChangeRadio - 2 + 0.5)).ToString();
                            }
                            this.ColorNumnumericUpDown.Maximum = 255;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x01);
                            break;
                        case 1://H
                            this.ColorNumnumericUpDown.Maximum = 360;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x02);
                            break;
                        case 0://UV
                            if ((m_slider[0] / m_ChangeRadio - 2) > 255)
                            {
                                m_slider[0] = (255 + 2) * m_ChangeRadio;
                                this.ColorImagepictureBox.Invalidate(); ;
                            }
                            if ((m_slider[1] / m_ChangeRadio - 2) > 255)
                            {
                                m_slider[1] = (255 + 2) * m_ChangeRadio;
                                this.ColorImagepictureBox.Invalidate(); ;
                            }
                            if (m_slider[0] > m_slider[1])
                            {
                                this.ColorlistViewEx.Items[0].SubItems[1].Text = ((int)(m_slider[0] / m_ChangeRadio - 2 + 0.5)).ToString();
                                this.ColorlistViewEx.Items[1].SubItems[1].Text = ((int)(m_slider[1] / m_ChangeRadio - 2 + 0.5)).ToString();
                            }
                            else
                            {
                                this.ColorlistViewEx.Items[1].SubItems[1].Text = ((int)(m_slider[0] / m_ChangeRadio - 2 + 0.5)).ToString();
                                this.ColorlistViewEx.Items[0].SubItems[1].Text = ((int)(m_slider[1] / m_ChangeRadio - 2 + 0.5)).ToString();
                            }
                            this.ColorNumnumericUpDown.Maximum = 255;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x04);
                            break;
                        default: break;

                    }
                }
                else//百分比
                {
                    switch (m_nColorType & 0x7)
                    {
                        case 2://灰度
                            this.ColorNumnumericUpDown.Maximum = 100;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x01);
                            break;
                        case 1://H
                            this.ColorNumnumericUpDown.Maximum = 100;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x02);
                            break;
                        case 0://UV
                            this.ColorNumnumericUpDown.Maximum = 100;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x04);
                            break;
                        default: break;

                    }
                }
                //if ((m_nColorType & 0x08) > 0)
                this.ColorpictureBox.Invalidate();
                ColorPictureChange();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ImageTypecomboBox_SelectionChangeCommitted出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ImageTypecomboBox_SelectionChangeCommitted出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 对比度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinGraynumericUpDown_Validated(object sender, EventArgs e)
        {
            //NumericUpDown numericUpDown = (NumericUpDown)sender;
            //m_MinGray = int.Parse(numericUpDown.Text);
            //this.ColorpictureBox.Invalidate();

            NumericUpDown numericUpDown = (NumericUpDown)sender;
            m_BrightRatio = double.Parse(numericUpDown.Text); //Modify by ChengSk - 20190830
            Commonfunction.SetAppSetting("对比亮度", m_BrightRatio.ToString());
            //ColorImagepictureBoxIncreaseBright();
            //this.ColorImagepictureBox.Invalidate();
        }


        /// <summary>
        /// 对比度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinGraynumericUpDown_Click(object sender, EventArgs e)
        {
            //NumericUpDown numericUpDown = (NumericUpDown)sender;
            //m_MinGray = int.Parse(numericUpDown.Text);
            //this.ColorpictureBox.Invalidate();

            NumericUpDown numericUpDown = (NumericUpDown)sender;
            m_BrightRatio = double.Parse(numericUpDown.Text); //Modify by ChengSk - 20190830
            Commonfunction.SetAppSetting("对比亮度", m_BrightRatio.ToString());
            //ColorImagepictureBoxIncreaseBright();
            //this.ColorImagepictureBox.Invalidate();
        }

        private void MinGraynumericUpDown_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                NumericUpDown numericUpDown = (NumericUpDown)sender;
                m_BrightRatio = double.Parse(numericUpDown.Text); //Modify by ChengSk - 20190830
                Commonfunction.SetAppSetting("对比亮度", m_BrightRatio.ToString());
                ColorImagepictureBoxIncreaseBright();
                m_DrawImage = true;
                this.ColorImagepictureBox.Invalidate();
            }
        }

        private void ColorImagepictureBoxIncreaseBright()     //Add by ChengSk - 20190830
        {
            try
            {
                int rgbValue = 0;
                for(int i=0; i< m_srcfruitRGBImage.Length; i++)
                {
                    rgbValue = (int)(m_srcfruitRGBImage[i] * m_BrightRatio);
                    if (rgbValue > 255)
                        rgbValue = 255;
                    m_fruitRGBImage[i] = (byte)rgbValue;
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorImagepictureBoxIncreaseBright" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorImagepictureBoxIncreaseBright" + ex);
#endif
            }
        }

        /// <summary>
        /// 百分比
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PercentageradioButton_Click(object sender, EventArgs e)
        {
            try
            {
                RadioButton radioButton = (RadioButton)sender;
                if (radioButton.Checked)
                {
                    m_nColorType |= 0x08;


                    switch (m_nColorType & 0x7)
                    {
                        case 1://灰度
                            this.ColorNumnumericUpDown.Maximum = 100;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x01);
                            break;
                        case 2://H
                            this.ColorNumnumericUpDown.Maximum = 100;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x02);
                            break;
                        case 4://UV
                            this.ColorNumnumericUpDown.Maximum = 100;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x04);
                            break;
                        default: break;

                    }
                    if (this.ImageTypecomboBox.SelectedIndex == 0)
                        this.ColorpictureBox.Invalidate();
                    ColorlistViewExSetParam();
                    ColorPictureChange();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数PercentageradioButton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数PercentageradioButton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 平均值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AverageradioButton_Click(object sender, EventArgs e)
        {
            try
            {
                RadioButton radioButton = (RadioButton)sender;
                if (radioButton.Checked)
                {
                    m_nColorType &= 0xF7;

                    switch (m_nColorType & 0x7)
                    {
                        case 1://灰度
                            this.ColorNumnumericUpDown.Maximum = 255;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x01);
                            break;
                        case 2://H
                            this.ColorNumnumericUpDown.Maximum = 360;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x02);
                            break;
                        case 4://UV
                            this.ColorNumnumericUpDown.Maximum = 255;
                            this.ColorNumnumericUpDown.Minimum = 0;
                            this.ColorpictureBox.Invalidate();
                            //m_nColorType = ((m_nColorType) & 0x0F8 | 0x04);
                            break;
                        default: break;

                    }

                    //if (this.ImageTypecomboBox.SelectedIndex == 0)

                    ColorlistViewExSetParam();
                    ColorPictureChange();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数AverageradioButton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数AverageradioButton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置颜色列表
        /// </summary>
        private void ColorlistViewExSetParam()
        {
            try
            {
                this.ColorlistViewEx.Clear();
                this.ColorlistViewEx.OwnerDraw = true;
                if ((m_nColorType & 0x08) > 0)//百分比
                {
                    this.PercentageradioButton.Checked = true;
                    this.AverageradioButton.Checked = false;
                    this.AddColorbutton.Enabled = true;
                    this.DeleteColorbutton.Enabled = true;
                    m_nColorType |= 0x08;

                    this.ColorlistViewEx.Columns.Add(m_resourceManager.GetString("ColorlistViewExName.Text"), 70, HorizontalAlignment.Center);//Modify by xcw - 20191118
                    for (int i = 0; i < 3; i++)
                    {
                        this.ColorlistViewEx.Columns.Add(string.Format("{0}min", i + 1), 70, HorizontalAlignment.Center);
                        this.ColorlistViewEx.Columns.Add(string.Format("{0}max", i + 1), 70, HorizontalAlignment.Center);
                        //this.ColorlistViewEx.Columns.Add(string.Format("{0}min", i + 1), 100, HorizontalAlignment.Center);
                        //this.ColorlistViewEx.Columns.Add(string.Format("{0}max", i + 1), 100, HorizontalAlignment.Center);
                    }
                    ListViewItem item;
                    for (int i = 0; i < GlobalDataInterface.ColorGradeNum; i++)
                    {
                        byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        Array.Copy(m_ColorGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        if (i == GlobalDataInterface.ColorGradeNum - 1)
                        {
                            m_percent[i * 3].nMin = 0;
                            m_percent[i * 3].nMax = 100;
                            m_percent[i * 3 + 1].nMin = 0;
                            m_percent[i * 3 + 1].nMax = 100;
                            m_percent[i * 3 + 2].nMin = 0;
                            m_percent[i * 3 + 2].nMax = 100;
                        }
                        for (int j = 1; j < 3 * 2; j += 2)
                        {
                            if (m_percent[i * 3 + (j - 1) / 2].nMax >= 100)
                                m_percent[i * 3 + (j - 1) / 2].nMax = 100;
                            if (m_percent[i * 3 + (j - 1) / 2].nMin > m_percent[i * 3 + (j - 1) / 2].nMax)
                                m_percent[i * 3 + (j - 1) / 2].nMin = m_percent[i * 3 + (j - 1) / 2].nMax;
                            if (m_percent[i * 3 + (j - 1) / 2].nMin < 0)
                                m_percent[i * 3 + (j - 1) / 2].nMin = 0;
                            item.SubItems.Add(m_percent[i * 3 + (j - 1) / 2].nMin.ToString());
                            item.SubItems.Add(m_percent[i * 3 + (j - 1) / 2].nMax.ToString());
                            switch (j)
                            {
                                case 1:
                                    item.SubItems[j].BackColor = m_ColorList[0];
                                    item.SubItems[j + 1].BackColor = m_ColorList[0];
                                    break;
                                case 3:
                                    item.SubItems[j].BackColor = m_ColorList[1];
                                    item.SubItems[j + 1].BackColor = m_ColorList[1];
                                    break;
                                case 5:
                                    item.SubItems[j].BackColor = m_ColorList[2];
                                    item.SubItems[j + 1].BackColor = m_ColorList[2];
                                    break;
                            }
                        }
                        this.ColorlistViewEx.Items.Add(item);
                    }
                }
                else//平均值
                {
                    this.PercentageradioButton.Checked = false;
                    this.AverageradioButton.Checked = true;
                    this.AddColorbutton.Enabled = false;
                    this.DeleteColorbutton.Enabled = false;
                    m_nColorType &= 0xF7;

                    string str = m_resourceManager.GetString("ColorlistViewExName.Text");
                    this.ColorlistViewEx.Columns.Add(m_resourceManager.GetString("ColorlistViewExName.Text"), 70, HorizontalAlignment.Center);
                    this.ColorlistViewEx.Columns.Add(m_resourceManager.GetString("ColorlistViewExAverage.Text"), 70, HorizontalAlignment.Center);
                    ListViewItem item;

                    for (int i = 0; i < 3; i++)
                    {
                        byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        Array.Copy(m_ColorGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        int colorlist = 0;
                        if (i == 0)
                        {
                            colorlist = (int)(m_slider[0] / m_ChangeRadio + 0.5) - 2;
                            item.SubItems.Add(colorlist.ToString());
                            item.SubItems[1].BackColor = m_ColorList[0];
                        }
                        else if (i == 1)
                        {
                            colorlist = (int)(m_slider[1] / m_ChangeRadio + 0.5) - 2;
                            item.SubItems.Add(colorlist.ToString());
                            item.SubItems[1].BackColor = m_ColorList[1];
                        }
                        else
                        {
                            item.SubItems.Add("0");
                            item.SubItems[1].BackColor = m_ColorList[2];
                        }
                        this.ColorlistViewEx.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorlistViewExSetParam出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorlistViewExSetParam出错" + ex);
#endif
            }
        }
        /// <summary>
        /// /// <summary>
        /// 颜色信息List中SubItemClicked事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorlistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                ListViewEx.ListViewEx listviewex = (ListViewEx.ListViewEx)sender;
                if ((m_nColorType & 0x08) > 0)//百分比
                {
                    if (IsMouseRight)
                    {
                        if (e.SubItem != 0)
                        {
                            this.ColorSetcolorDialog.Color = m_ColorList[(e.SubItem - 1) / 2];
                            if (this.ColorSetcolorDialog.ShowDialog() == DialogResult.OK)
                            {
                                m_ColorList[(e.SubItem - 1) / 2] = this.ColorSetcolorDialog.Color;
                                for (int i = 0; i < listviewex.Items.Count; i++)
                                {
                                    listviewex.Items[i].SubItems[e.SubItem].BackColor = this.ColorSetcolorDialog.Color;
                                    if (e.SubItem % 2 == 0)
                                        listviewex.Items[i].SubItems[e.SubItem - 1].BackColor = this.ColorSetcolorDialog.Color;
                                    else
                                        listviewex.Items[i].SubItems[e.SubItem + 1].BackColor = this.ColorSetcolorDialog.Color;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (e.SubItem == 0)
                            this.ColorlistViewEx.StartEditing(ColorSetEditors[0], e.Item, e.SubItem);
                        else if (!(e.Item.Index == (listviewex.Items.Count - 1)))
                            this.ColorlistViewEx.StartEditing(ColorSetEditors[1], e.Item, e.SubItem);
                    }

                }
                else
                {
                    if (IsMouseRight)
                    {
                        this.ColorSetcolorDialog.Color = m_ColorList[e.Item.Index];
                        if (this.ColorSetcolorDialog.ShowDialog() == DialogResult.OK)
                        {
                            m_ColorList[e.Item.Index] = this.ColorSetcolorDialog.Color;
                            listviewex.Items[e.Item.Index].SubItems[1].BackColor = this.ColorSetcolorDialog.Color;
                            ColorPictureChange();
                        }
                    }
                    else
                    {
                        if (e.SubItem == 0)
                            this.ColorlistViewEx.StartEditing(ColorSetEditors[0], e.Item, e.SubItem);
                        else if (!(e.SubItem == 1 && e.Item.Index == 2))
                            this.ColorlistViewEx.StartEditing(ColorSetEditors[1], e.Item, e.SubItem);

                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 颜色列表鼠标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorlistViewEx_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
                IsMouseRight = false; 
            else
                IsMouseRight = true; 
        }

        /// <summary>
        /// 颜色信息List的结束编辑事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorlistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                ListViewEx.ListViewEx listviewex = (ListViewEx.ListViewEx)sender;

                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                if ((m_nColorType & 0x08) > 0)//百分比
                {
                    switch (e.SubItem)
                    {
                        case 0:
                            Array.Copy(temp, 0, m_ColorGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                            temp = Encoding.Default.GetBytes(e.DisplayText.Trim()); //去掉后缀空字符串 Modify by ChengSk - 20190118
                            Array.Copy(temp, 0, m_ColorGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                            break;
                        case 1:
                            if (byte.Parse(e.DisplayText) > m_percent[e.Item.Index * 3].nMax)
                            {
                                //MessageBox.Show(string.Format("第{0}行第{1}列 参数设置错误！", e.Item.Index + 1, e.SubItem + 1));
                                //MessageBox.Show(string.Format("0x30001010 Row {0} Column {1}'s parameter is invalid!", e.Item.Index + 1, e.SubItem + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.ColorSetFormMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] + "{0}" +
                                    LanguageContainer.ColorSetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] + "{1}" +
                                    LanguageContainer.ColorSetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex],
                                    e.Item.Index + 1, e.SubItem + 1),
                                    LanguageContainer.ColorSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //this.ColorlistViewEx.StartEditing(ColorSetEditors[1], e.Item, e.SubItem);
                                e.Item.SubItems[e.SubItem].BackColor = Color.DeepSkyBlue;
                                return;
                                //m_percent[e.Item.Index * 3].nMin = m_percent[e.Item.Index * 3].nMax;
                            }
                            else
                            {
                                m_percent[e.Item.Index * 3].nMin = byte.Parse(e.DisplayText);
                                e.Item.SubItems[e.SubItem].BackColor = m_ColorList[0];
                            }
                            break;
                        case 2:
                            m_percent[e.Item.Index * 3].nMax = byte.Parse(e.DisplayText);
                            break;
                        case 3:
                            if (byte.Parse(e.DisplayText) > m_percent[e.Item.Index * 3 + 1].nMax)
                            {
                                //MessageBox.Show(string.Format("第{0}行第{1}列 参数设置错误！", e.Item.Index + 1, e.SubItem + 1));
                                //MessageBox.Show(string.Format("0x30001010 Row {0} Column {1}'s parameter is invalid!", e.Item.Index + 1, e.SubItem + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.ColorSetFormMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] + "{0}" +
                                    LanguageContainer.ColorSetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] + "{1}" +
                                    LanguageContainer.ColorSetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex],
                                    e.Item.Index + 1, e.SubItem + 1),
                                    LanguageContainer.ColorSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                e.Item.SubItems[e.SubItem].BackColor = Color.DeepSkyBlue;
                                //this.ColorlistViewEx.StartEditing(ColorSetEditors[1], e.Item, e.SubItem);
                                // m_percent[e.Item.Index * 3 + 1].nMin = m_percent[e.Item.Index * 3 + 1].nMax;
                            }
                            else
                            {
                                m_percent[e.Item.Index * 3 + 1].nMin = byte.Parse(e.DisplayText);
                                e.Item.SubItems[e.SubItem].BackColor = m_ColorList[1];
                            }
                            break;
                        case 4:
                            m_percent[e.Item.Index * 3 + 1].nMax = byte.Parse(e.DisplayText);
                            break;
                        case 5:
                            if (byte.Parse(e.DisplayText) > m_percent[e.Item.Index * 3 + 2].nMax)
                            {
                                //MessageBox.Show(string.Format("第{0}行第{1}列 参数设置错误！", e.Item.Index + 1, e.SubItem + 1));
                                //MessageBox.Show(string.Format("0x30001010 Row {0} Column {1}'s parameter is invalid!", e.Item.Index + 1, e.SubItem + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.ColorSetFormMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] + "{0}" +
                                    LanguageContainer.ColorSetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] + "{1}" +
                                    LanguageContainer.ColorSetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex],
                                    e.Item.Index + 1, e.SubItem + 1),
                                    LanguageContainer.ColorSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                e.Item.SubItems[e.SubItem].BackColor = Color.DeepSkyBlue;
                                // this.ColorlistViewEx.StartEditing(ColorSetEditors[1], e.Item, e.SubItem);
                                //m_percent[e.Item.Index * 3 + 2].nMin = m_percent[e.Item.Index * 3 + 2].nMax;
                            }
                            else
                            {
                                m_percent[e.Item.Index * 3 + 2].nMin = byte.Parse(e.DisplayText);
                                e.Item.SubItems[e.SubItem].BackColor = m_ColorList[2];
                            }
                            break;
                        case 6:
                            m_percent[e.Item.Index * 3 + 2].nMax = byte.Parse(e.DisplayText);
                            break;
                        default: break;
                    }
                }


                else//平均值
                {
                    switch (e.SubItem)
                    {
                        case 0:
                            Array.Copy(temp, 0, m_ColorGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                            temp = Encoding.Default.GetBytes(e.DisplayText);
                            Array.Copy(temp, 0, m_ColorGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                            break;
                        case 1:
                            if (e.Item.Index == 0)
                            {
                                if (int.Parse(e.DisplayText) < int.Parse(listviewex.Items[1].SubItems[1].Text))
                                {
                                    //MessageBox.Show("参数设置无效！(颜色列表第一行值要比第二行值大)");
                                    //MessageBox.Show("0x30001010 Invalid parameter!Row 1's parameter should be larger than Row 2s'", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    MessageBox.Show("0x30001010 " + LanguageContainer.ColorSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                                        LanguageContainer.ColorSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    e.Item.SubItems[e.SubItem].BackColor = Color.DeepSkyBlue;
                                    //listviewex.Items[0].SubItems[1].Text = listviewex.Items[1].SubItems[1].Text;
                                    //m_slider[0] = m_slider[1];
                                }
                                else
                                {
                                    m_slider[0] = (int.Parse(e.DisplayText) + 2) * m_ChangeRadio;
                                    e.Item.SubItems[e.SubItem].BackColor = m_ColorList[e.Item.Index];
                                    listviewex.Items[1].SubItems[e.SubItem].BackColor = m_ColorList[1];
                                    this.ColorpictureBox.Invalidate();
                                }
                            }
                            else if (e.Item.Index == 1)
                            {
                                if (int.Parse(e.DisplayText) > int.Parse(listviewex.Items[0].SubItems[1].Text))
                                {
                                    //MessageBox.Show("参数设置无效！(颜色列表第一行值要比第二行值大)");
                                    //MessageBox.Show("0x30001010 Invalid parameter!Row 1's parameter should be larger than Row 2s'", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    MessageBox.Show("0x30001010 " + LanguageContainer.ColorSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                                        LanguageContainer.ColorSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    e.Item.SubItems[e.SubItem].BackColor = Color.DeepSkyBlue;
                                    //listviewex.Items[1].SubItems[1].Text = listviewex.Items[0].SubItems[1].Text;
                                    //m_slider[1] = m_slider[0];
                                }
                                else
                                {
                                    m_slider[1] = (int.Parse(e.DisplayText) + 2) * m_ChangeRadio;
                                    e.Item.SubItems[e.SubItem].BackColor = m_ColorList[e.Item.Index];
                                    listviewex.Items[0].SubItems[e.SubItem].BackColor = m_ColorList[0];
                                    this.ColorpictureBox.Invalidate();
                                }
                            }
                            break;
                        default: break;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorlistViewEx_SubItemEndEditing出错" + ex);
#endif
            } 
        }

        /// <summary>
        /// 添加颜色等级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddColorbutton_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem item;
                if (this.ColorlistViewEx.Items.Count == 0)
                {
                    item = new ListViewItem("所有颜色");
                    for (int j = 0; j < 3; j++)
                    {
                        item.SubItems.Add("0");
                        item.SubItems[j * 2 + 1].BackColor = m_ColorList[j];
                        item.SubItems.Add("100");
                        item.SubItems[j * 2 + 2].BackColor = m_ColorList[j];
                    }
                    this.ColorlistViewEx.Items.Add(item);

                    //颜色名称及颜色区间赋值 - Add by ChengSk - 20181228
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    temp = Encoding.Default.GetBytes("所有颜色");
                    Array.Copy(temp, 0, m_ColorGradeName, 0 * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                    m_percent[0 * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMin = byte.Parse("0");
                    m_percent[0 * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMax = byte.Parse("100");
                    m_percent[0 * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMin = byte.Parse("0");
                    m_percent[0 * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMax = byte.Parse("100");
                    m_percent[0 * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMin = byte.Parse("0");
                    m_percent[0 * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMax = byte.Parse("100");
                }
                else
                {
                    item = new ListViewItem("新颜色");
                    for (int j = 0; j < 3; j++)
                    {
                        item.SubItems.Add("0");
                        item.SubItems[j * 2 + 1].BackColor = m_ColorList[j];
                        item.SubItems.Add("100");
                        item.SubItems[j * 2 + 2].BackColor = m_ColorList[j];
                    }
                    this.ColorlistViewEx.Items.Insert(0, item);

                    //颜色名称及颜色区间赋值 - Add by ChengSk - 20181228
                    //历史数据向后平移一结构
                    for (int i = ConstPreDefine.MAX_COLOR_GRADE_NUM - 2; i >= 0; i--)
                    {
                        Array.Copy(m_ColorGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, m_ColorGradeName, (i + 1) * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                    }
                    for (int i = ConstPreDefine.MAX_COLOR_GRADE_NUM - 2; i >= 0; i--)
                    {
                        m_percent[(i + 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMin = m_percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMin;
                        m_percent[(i + 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMin = m_percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMin;
                        m_percent[(i + 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMin = m_percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMin;
                        m_percent[(i + 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMax = m_percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMax;
                        m_percent[(i + 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMin = m_percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMin;
                        m_percent[(i + 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMax = m_percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMax;
                    }
                    //新数据赋值
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    temp = Encoding.Default.GetBytes("新颜色");
                    Array.Copy(temp, 0, m_ColorGradeName, 0 * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                    m_percent[0 * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMin = byte.Parse("0");
                    m_percent[0 * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMax = byte.Parse("100");
                    m_percent[0 * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMin = byte.Parse("0");
                    m_percent[0 * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMax = byte.Parse("100");
                    m_percent[0 * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMin = byte.Parse("0");
                    m_percent[0 * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMax = byte.Parse("100");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数AddColorbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数AddColorbutton_Click出错" + ex);
#endif
            } 
        }

        /// <summary>
        /// 减少颜色等级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteColorbutton_Click(object sender, EventArgs e)
        {
            if (this.ColorlistViewEx.Items.Count>1)
            {
                int ColorNumber = this.ColorlistViewEx.Items.Count; //当前颜色等级数量
                this.ColorlistViewEx.Items.RemoveAt(0);

                //历史数据向前平移一结构  Add by ChengSk - 20181228
                for (int i = 0; i < ColorNumber - 1; i++)
                {
                    Array.Copy(m_ColorGradeName, (i + 1) * ConstPreDefine.MAX_TEXT_LENGTH, m_ColorGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);
                }
                for (int i = 0; i < ColorNumber - 1; i++)
                {
                    m_percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMin = m_percent[(i + 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMin;
                    m_percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMin = m_percent[(i + 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMin;
                    m_percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMin = m_percent[(i + 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMin;
                    m_percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMax = m_percent[(i + 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMax;
                    m_percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMin = m_percent[(i + 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMin;
                    m_percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMax = m_percent[(i + 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMax;
                }
                //删除的等级赋值为零
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                //temp = Encoding.Default.GetBytes(""); //Note by ChengSk - 20190109
                Array.Copy(temp, 0, m_ColorGradeName, (ColorNumber - 1) * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                m_percent[(ColorNumber - 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMin = byte.Parse("0");
                m_percent[(ColorNumber - 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 0].nMax = byte.Parse("0");
                m_percent[(ColorNumber - 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMin = byte.Parse("0");
                m_percent[(ColorNumber - 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 1].nMax = byte.Parse("0");
                m_percent[(ColorNumber - 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMin = byte.Parse("0");
                m_percent[(ColorNumber - 1) * ConstPreDefine.MAX_COLOR_INTERVAL_NUM + 2].nMax = byte.Parse("0");
            }  
        }

        /// <summary>
        /// 颜色列表头点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorlistViewEx_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewEx.ListViewEx listviewex = (ListViewEx.ListViewEx)sender;
            Graphics graphics = listviewex.CreateGraphics();
            if ((m_nColorType & 0x08) > 0)//百分比
            {
                m_UVRectCurrentIndex = (e.Column - 1) / 2;
            }
        }

        /// <summary>
        /// 获取图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetImagebutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ChannelcontextMenuStrip.Items.Count > 0)
                {
                    Point point = new Point();

                    point.X = Cursor.Position.X - this.Location.X + GetImagebutton.Location.X;
                    point.Y = 0;
                    this.ChannelcontextMenuStrip.Show(GetImagebutton, point);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数GetImagebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数GetImagebutton_Click出错" + ex);
#endif
            } 
        }

       

        

        /// <summary>
        /// 通道快捷菜单单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelcontextMenuStrip_Click(object sender, EventArgs e)
        {
            try
            {

                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                m_CurrentChannelIndex = this.ChannelcontextMenuStrip.Items.IndexOf(menuItem);
                
                m_CurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));
                if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                {
                    m_CurrentIPM_ID = (m_ChanelIDList[m_CurrentChannelIndex]);
                    
                }

                else if (GlobalDataInterface.nVer == 0)
                {
                    m_CurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));
                }

                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(m_CurrentIPM_ID, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SINGLE_SAMPLE, null);
                }
                
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ChannelcontextMenuStrip_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ChannelcontextMenuStrip_Click出错" + ex);
#endif
            } 
        }

        /// <summary>
        /// 上传图像显示刷新
        /// </summary>
        /// <param name="imageInfo"></param>
        public void OnUpSpliceImageData(stSpliceImageData spliceImageData)
        {
            //if (GlobalDataInterface.nVer == 0)
            //{
                try
                {
                    //if (this == Form.ActiveForm)
                    //{
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new GlobalDataInterface.SpliceImageDataEventHandler(OnUpSpliceImageData), spliceImageData);
                        }
                        else
                        {
                            if (spliceImageData.imageInfo.nRouteId != m_CurrentIPM_ID)
                                return;
                            m_spliceImageData = new stSpliceImageData(spliceImageData.imagedataC.Length);
                            m_spliceImageData.ToCopy(spliceImageData);
                            m_spliceImageBin = new byte[GlobalDataInterface.globalIn_spliceimgBin.Length];
                            Array.Copy(GlobalDataInterface.globalIn_spliceimgBin, m_spliceImageBin, GlobalDataInterface.globalIn_spliceimgBin.Length);
                            ColorPictureChange();
                        }
                    //}
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数OnUpImageInfo出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数OnUpImageInfo出错" + ex);
#endif
                }
            //}
//            else
//            {
//                try
//                {
//                    if (this == Form.ActiveForm)
//                    {
//                        if (this.InvokeRequired)
//                        {
//                            this.BeginInvoke(new GlobalDataInterface.SpliceImageDataEventHandler(OnUpSpliceImageData), spliceImageData);
//                        }
//                        else
//                        {
//                            if (spliceImageData.imageInfo.nRouteId != m_CurrentIPM_ID)
//                                return;
                            
//                            m_spliceImageData = new stSpliceImageData(spliceImageData.imagedataC.Length);
//                            m_spliceImageData.ToCopy(spliceImageData);
//                            m_spliceImageBin = new byte[GlobalDataInterface.globalIn_spliceimgBin.Length];
//                            Array.Copy(GlobalDataInterface.globalIn_spliceimgBin, m_spliceImageBin, GlobalDataInterface.globalIn_spliceimgBin.Length);
//                            ColorPictureChange();
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数OnUpImageInfo出错" + ex);
//#if REALEASE
//                    GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数OnUpImageInfo出错" + ex);
//#endif
//                }
//            }

        }

        bool m_DrawImage = false;
        const int BackgroundLength = 0;//图片背景间隔长度
        /// <summary>
        /// 获取图像后的下半部图片及颜色比例显示
        /// </summary>
        private void ColorPictureChange()
        {
            try
            {
                
                if (m_spliceImageData.imageInfo.nRouteId > 0)
                {

                    ColorRGB[] color = new ColorRGB[3];
                    uint[] unColorIntervals = new uint[3];
                    int ColorType = m_nColorType & 0x0f;  //获取后4位
                    int[] ColorCount = new int[3];
                    if ((ColorType & 0xff8) > 0) //后4位首位为1，代表百分比
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            color[i].ucR = m_ColorList[i].R;
                            color[i].ucG = m_ColorList[i].G;
                            color[i].ucB = m_ColorList[i].B;
                        }
                    }
                    else //首位为0代表平均值
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            color[2 - i].ucR = m_ColorList[i].R;
                            color[2 - i].ucG = m_ColorList[i].G;
                            color[2 - i].ucB = m_ColorList[i].B;
                        }
                    }

                    if ((ColorType & 0x07) == 4)  //UV
                    {
                        if (ColorType == 4)//平均值uv
                        {
                            int[] interval = new int[2];
                            if (m_slider[0] < m_slider[1])
                            {
                                interval[0] = (int)(m_slider[0] / m_ChangeRadio + 0.5) - 2;
                                interval[1] = (int)(m_slider[1] / m_ChangeRadio + 0.5) - 2;
                            }
                            else
                            {
                                interval[0] = (int)(m_slider[1] / m_ChangeRadio + 0.5) - 2;
                                interval[1] = (int)(m_slider[0] / m_ChangeRadio + 0.5) - 2;
                            }

                            unColorIntervals[0] = uint.Parse((0 | (0 << 8) | ((uint)((interval[0] & (uint)0xFF) << 16)) | ((uint)0xFF << 24)).ToString());
                            unColorIntervals[1] = uint.Parse((((uint)(interval[0] & (uint)0xFF)) | (0 << 8) | ((uint)((interval[1] & (uint)0xFF) << 16)) | ((uint)0xFF << 24)).ToString());
                            unColorIntervals[2] = uint.Parse((((uint)(interval[1] & (uint)0xFF)) | (0 << 8) | ((uint)0xFF << 16) | ((uint)0xFF << 24)).ToString());
                        }
                        else//百分比uv
                        {
                            float UVChangeRadio = this.ColorpictureBox.Height / 256.0f;
                            for (int i = 0; i < 3; i++)
                            {
                                unColorIntervals[i] = (uint)(((int)(m_UVRect[i].Left / UVChangeRadio) & 0x0FF) | (((int)(m_UVRect[i].Top / UVChangeRadio) & 0x0FF) << 8) | (((int)(m_UVRect[i].Right / UVChangeRadio) & 0x0FF) << 16) | (((int)(m_UVRect[i].Bottom / UVChangeRadio) & 0x0FF) << 24));
                            }
                        }
                    }
                    else
                    {
                        int[] interval = new int[2];
                        if (m_slider[0] < m_slider[1])
                        {
                            interval[0] = (int)(m_slider[0] / m_ChangeRadio + 0.5) - 2;
                            interval[1] = (int)(m_slider[1] / m_ChangeRadio + 0.5) - 2;
                        }
                        else
                        {
                            interval[0] = (int)(m_slider[1] / m_ChangeRadio + 0.5) - 2;
                            interval[1] = (int)(m_slider[0] / m_ChangeRadio + 0.5) - 2;
                        }
                        unColorIntervals[0] = (uint)interval[0];
                        unColorIntervals[1] = (uint)interval[1];
                        unColorIntervals[2] = 0;
                    }

                    byte[] tempImagedata = new byte[m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height * 3];
                    m_fruitRGBImage = new byte[(3 * BackgroundLength + m_spliceImageData.imageInfo.height * 2) * m_spliceImageData.imageInfo.width * 3];
                    byte[] imagedata;
                    if (m_spliceImageData.imageInfo.bPixelBit == 1)
                    {
                        imagedata = new byte[m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height];//Y值
                        Array.Copy(m_spliceImageData.imagedataC, 0, imagedata, 0, m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height);
                        Commonfunction.YUV422GrayChangeToRGB24(imagedata, ref tempImagedata, m_spliceImageData.imageInfo.width, m_spliceImageData.imageInfo.height);
                    }
                    else
                    {
                        imagedata = new byte[m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height * 2];
                        Array.Copy(m_spliceImageData.imagedataC, 0, imagedata, 0, m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height * 2);
                        Commonfunction.YUV422ChangeToRGB24(imagedata, ref tempImagedata, m_spliceImageData.imageInfo.width, m_spliceImageData.imageInfo.height);
                    }


                    Array.Copy(tempImagedata, 0, m_fruitRGBImage, BackgroundLength * m_spliceImageData.imageInfo.width * 3, m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height * 3);//获取图片上半部分

                    m_srcfruitRGBImage = new byte[m_fruitRGBImage.Length];
                    Array.Copy(m_fruitRGBImage, m_srcfruitRGBImage, m_fruitRGBImage.Length);    //Add by ChengSk - 20190902

                    ////if (GlobalDataInterface.globalOut_SysConfig.multiFreq == 0)
                    ////{
                    ////    Commonfunction.ColorStatistic24(ref tempImagedata, m_spliceImageData.imageInfo.width, m_spliceImageData.imageInfo.nChannelH, GlobalDataInterface.globalOut_Paras[Commonfunction.GetSubsysIndex(m_CurrentIPM_ID) * ConstPreDefine.MAX_IPM_NUM + Commonfunction.GetIPMIndex(m_CurrentIPM_ID)].nCupNum, m_spliceImageData.imageInfo.nLefts1, m_spliceImageData.imageInfo.nLefts0, m_spliceImageData.imageInfo.nLefts2, m_spliceImageData.imageInfo.nColorMidLen, m_spliceImageData.imageInfo.fColorCutY,ColorType,
                    ////                                     color, unColorIntervals, ref ColorCount);//获取转换后的图片下半部分
                    ////}
                    ////else//倍频模式 果杯数为*2-1
                    ////{
                    ////    Commonfunction.ColorStatistic24(ref tempImagedata, m_spliceImageData.imageInfo.width, m_spliceImageData.imageInfo.nChannelH, GlobalDataInterface.globalOut_Paras[Commonfunction.GetSubsysIndex(m_CurrentIPM_ID) * ConstPreDefine.MAX_IPM_NUM + Commonfunction.GetIPMIndex(m_CurrentIPM_ID)].nCupNum * 2 - 1, m_spliceImageData.imageInfo.nLefts1, m_spliceImageData.imageInfo.nLefts0, m_spliceImageData.imageInfo.nLefts2, m_spliceImageData.imageInfo.nColorMidLen, m_spliceImageData.imageInfo.fColorCutY, ColorType,
                    ////                                     color, unColorIntervals, ref ColorCount);//获取转换后的图片下半部分
                    ////}

                    //Modify by ChengSk - 20190827

                    //计算颜色信息
                    if (GlobalDataInterface.globalOut_SysConfig.multiFreq == 0 && m_spliceImageData.imageInfo.bPixelBit == 1)
                    {
                        Commonfunction.ColorStatistic24(ref tempImagedata, m_spliceImageData.imageInfo.width, m_spliceImageData.imageInfo.nChannelH, m_spliceImageData.imageInfo.nValidCupNum,
                             m_spliceImageData.imageInfo.nColorMidLen, m_spliceImageData.imageInfo.fColorCutY, ColorType,
                                                         color, unColorIntervals, ref ColorCount);//获取转换后的图片下半部分
                    }
                    else if (GlobalDataInterface.globalOut_SysConfig.multiFreq == 0 && m_spliceImageData.imageInfo.bPixelBit == 2)
                    {
                        Commonfunction.ColorStatistic24_foreBin(ref tempImagedata, m_spliceImageBin, m_spliceImageData.imageInfo.width, m_spliceImageData.imageInfo.nChannelH, m_spliceImageData.imageInfo.nValidCupNum,
                            m_spliceImageData.imageInfo.nColorMidLen, m_spliceImageData.imageInfo.fColorCutY, ColorType,
                                                         color, unColorIntervals, ref ColorCount);//获取转换后的图片下半部分  
                    }
                    else if (GlobalDataInterface.globalOut_SysConfig.multiFreq == 1 && m_spliceImageData.imageInfo.bPixelBit == 1)
                    {
                        Commonfunction.ColorStatistic24(ref tempImagedata, m_spliceImageData.imageInfo.width, m_spliceImageData.imageInfo.nChannelH, m_spliceImageData.imageInfo.nValidCupNum,
                            m_spliceImageData.imageInfo.nColorMidLen, m_spliceImageData.imageInfo.fColorCutY, ColorType,
                                                         color, unColorIntervals, ref ColorCount);//获取转1图片下半部分

                    }
                    else
                    {
                        Commonfunction.ColorStatistic24_foreBin(ref tempImagedata, m_spliceImageBin, m_spliceImageData.imageInfo.width, m_spliceImageData.imageInfo.nChannelH,
                            m_spliceImageData.imageInfo.nValidCupNum, m_spliceImageData.imageInfo.nColorMidLen, m_spliceImageData.imageInfo.fColorCutY, ColorType,
                                                         color, unColorIntervals, ref ColorCount);//获取转换后的图片下半部分
                        //Modify by xcw - 20191125
                    }


                    Array.Copy(tempImagedata, 0, m_fruitRGBImage, (2 * BackgroundLength + m_spliceImageData.imageInfo.height) * m_spliceImageData.imageInfo.width * 3, m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height * 3);//获取完整图片
                   
                    //if (Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]) == 0)
                    //{
                    //    m_fruitRGBImage = new byte[(3 * BackgroundLength + m_imageData.imageInfo.height * 2) * m_imageData.imageInfo.width * 4];
                    //    Array.Copy(m_imageData.imagedataC, 0, imagedata, 0, m_imageData.imageInfo.width * m_imageData.imageInfo.height * 2);
                    //    Commonfunction.YUV422ChangeToRGB(imagedata, ref tempImagedata, m_imageData.imageInfo.width, m_imageData.imageInfo.height);
                    //    Array.Copy(tempImagedata, 0, m_fruitRGBImage, BackgroundLength * m_imageData.imageInfo.width * 4, m_imageData.imageInfo.width * m_imageData.imageInfo.height * 4);//获取图片上半部分
                    //    Commonfunction.ColorStatistic(ref tempImagedata, m_imageData.imageInfo.width, m_imageData.imageInfo.height, GlobalDataInterface.globalOut_Paras[Commonfunction.GetSubsysIndex(m_CurrentIPM_ID) * ConstPreDefine.MAX_SUBSYS_NUM + Commonfunction.GetIPMIndex(m_CurrentIPM_ID)].nCupNum, ColorType,
                    //                                    color, unColorIntervals, ref ColorCount);//获取转换后的图片下半部分
                    //    Array.Copy(tempImagedata, 0, m_fruitRGBImage, (2 * BackgroundLength + m_imageData.imageInfo.height) * m_imageData.imageInfo.width * 4, m_imageData.imageInfo.width * m_imageData.imageInfo.height * 4);//获取完整图片
                    //}
                    //else
                    //{
                    //    m_fruitRGBImage = new byte[(3 * BackgroundLength + (m_imageData.imageInfo.nBottom[1] - m_imageData.imageInfo.nTop[1]) * 2) * m_imageData.imageInfo.width * 4];
                    //    Array.Copy(m_imageData.imagedataC, m_imageData.imageInfo.width * m_imageData.imageInfo.nTop[1] * 2, imagedata, 0, m_imageData.imageInfo.width * (m_imageData.imageInfo.nBottom[1] - m_imageData.imageInfo.nTop[1]) * 2);
                    //    Commonfunction.YUV422ChangeToRGB(imagedata, ref tempImagedata, m_imageData.imageInfo.width, (m_imageData.imageInfo.nBottom[1] - m_imageData.imageInfo.nTop[1]));
                    //    Array.Copy(tempImagedata, 0, m_fruitRGBImage, BackgroundLength * m_imageData.imageInfo.width * 4, m_imageData.imageInfo.width * (m_imageData.imageInfo.nBottom[1] - m_imageData.imageInfo.nTop[1]) * 4);//获取图片上半部分
                    //    Commonfunction.ColorStatistic(ref tempImagedata, m_imageData.imageInfo.width, (m_imageData.imageInfo.nBottom[1] - m_imageData.imageInfo.nTop[1]), GlobalDataInterface.globalOut_Paras[Commonfunction.GetSubsysIndex(m_CurrentIPM_ID) * ConstPreDefine.MAX_SUBSYS_NUM + Commonfunction.GetIPMIndex(m_CurrentIPM_ID)].nCupNum, ColorType,
                    //                                    color, unColorIntervals, ref ColorCount);//获取转换后的图片下半部分
                    //    Array.Copy(tempImagedata, 0, m_fruitRGBImage, (2 * BackgroundLength + (m_imageData.imageInfo.nBottom[1] - m_imageData.imageInfo.nTop[1])) * m_imageData.imageInfo.width * 4, m_imageData.imageInfo.width * (m_imageData.imageInfo.nBottom[1] - m_imageData.imageInfo.nTop[1]) * 4);//获取完整图片
                    //}
                    int height;
                    height = 3 * BackgroundLength + m_spliceImageData.imageInfo.height * 2;
                    //if (Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]) == 0)
                    //{
                    //    height = 3 * BackgroundLength + (m_imageData.imageInfo.nBottom[0] - m_imageData.imageInfo.nTop[0]) * 2;
                    //}
                    //else
                    //{
                    //    height = 3 * BackgroundLength + (m_imageData.imageInfo.nBottom[1] - m_imageData.imageInfo.nTop[1]) * 2;
                    //}
                    //float radio = (float)this.ColorImagepictureBox.Width / m_imageData.imageInfo.width;
                    //this.ColorImagepictureBox.Height = (int)(radio * height);

                    //Array.Copy(m_fruitRGBImage, 0, imagedata, 0, m_imageInfo.width * (m_imageInfo.height / 2) * 4);
                    //int index = Commonfunction.GetIPMIndex(m_CurrentIPM_ID);

                    if ((m_nColorType & 0x08) > 0)//百分比
                    {
                        //this.ColorPercentlabel.Text = m_resourceManager.GetString("ColorPercentlabel.Text")+m_resourceManager.GetString("ColorIntervallabel.Text")+string.Format(" 1 {0}", ColorCount[0])+m_resourceManager.GetString("ColorIntervallabel.Text")+string.Format(" 2 {0}", ColorCount[1])+m_resourceManager.GetString("ColorIntervallabel.Text")+string.Format(" 3 {0}", ColorCount[2]);
                        this.ColorResultlabel.Text = m_resourceManager.GetString("ColorPercentlabel.Text") + m_resourceManager.GetString("ColorIntervallabel.Text") + string.Format(" 1- {0}  ", ColorCount[0]) + m_resourceManager.GetString("ColorIntervallabel.Text") + string.Format(" 2- {0}  ", ColorCount[1]) + m_resourceManager.GetString("ColorIntervallabel.Text") + string.Format(" 3- {0}", ColorCount[2]);
                        m_colorInterval = 0.0f;
                    }
                    else//平均值
                    {
                        switch (m_nColorType & 0x07)
                        {
                            //case 2://H
                            //    m_colorInterval = (ColorCount[0] + 182) * m_ChangeRadio;
                            //    break;
                            case 4://UV
                                m_colorInterval = (ColorCount[1] + 2) * m_ChangeRadio;//贴图偏移2，为了标尺数字可显示
                                break;
                            default:
                                m_colorInterval = (ColorCount[0] + 2) * m_ChangeRadio;
                                break;
                        }

                        this.ColorResultlabel.Text = m_resourceManager.GetString("ColorAvgThresholdlabel.Text") + string.Format(" {0}", (m_colorInterval / m_ChangeRadio - 2));

                        this.ColorpictureBox.Invalidate();
                    }
                    m_DrawImage = true;
                    this.ColorImagepictureBox.Invalidate(); ;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorPictureChange出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorPictureChange出错" + ex);
#endif
            }
        }
        //        private void ColorPictureChange()
        //        {
        //            try
        //            {
        //                if (m_spliceImageData.imageInfo.nRouteId > 0)
        //                {

        //                    ColorRGB[] color = new ColorRGB[3];
        //                    uint[] unColorIntervals = new uint[3];
        //                    int ColorType = m_nColorType & 0x0f;  //获取后4位
        //                    int[] ColorCount = new int[3];
        //                    if ((ColorType & 0xff8) > 0) //后4位首位为1，代表百分比
        //                    {
        //                        for (int i = 0; i < 3; i++)
        //                        {
        //                            color[i].ucR = m_ColorList[i].R;
        //                            color[i].ucG = m_ColorList[i].G;
        //                            color[i].ucB = m_ColorList[i].B;
        //                        }
        //                    }
        //                    else //首位为0代表平均值
        //                    {
        //                        for (int i = 0; i < 3; i++)
        //                        {
        //                            color[2 - i].ucR = m_ColorList[i].R;
        //                            color[2 - i].ucG = m_ColorList[i].G;
        //                            color[2 - i].ucB = m_ColorList[i].B;
        //                        }
        //                    }

        //                    if ((ColorType & 0x07) == 4)  //UV
        //                    {
        //                        if (ColorType == 4)//平均值uv
        //                        {
        //                            int[] interval = new int[2];
        //                            if (m_slider[0] < m_slider[1])
        //                            {
        //                                interval[0] = (int)(m_slider[0] / m_ChangeRadio + 0.5) - 2;
        //                                interval[1] = (int)(m_slider[1] / m_ChangeRadio + 0.5) - 2;
        //                            }
        //                            else
        //                            {
        //                                interval[0] = (int)(m_slider[1] / m_ChangeRadio + 0.5) - 2;
        //                                interval[1] = (int)(m_slider[0] / m_ChangeRadio + 0.5) - 2;
        //                            }

        //                            unColorIntervals[0] = uint.Parse((0 | (0 << 8) | ((uint)((interval[0] & (uint)0xFF) << 16)) | ((uint)0xFF << 24)).ToString());
        //                            unColorIntervals[1] = uint.Parse((((uint)(interval[0] & (uint)0xFF)) | (0 << 8) | ((uint)((interval[1] & (uint)0xFF) << 16)) | ((uint)0xFF << 24)).ToString());
        //                            unColorIntervals[2] = uint.Parse((((uint)(interval[1] & (uint)0xFF)) | (0 << 8) | ((uint)0xFF << 16) | ((uint)0xFF << 24)).ToString());
        //                        }
        //                        else//百分比uv
        //                        {
        //                            float UVChangeRadio = this.ColorpictureBox.Height / 256.0f;
        //                            for (int i = 0; i < 3; i++)
        //                            {
        //                                unColorIntervals[i] = (uint)(((int)(m_UVRect[i].Left / UVChangeRadio) & 0x0FF) | (((int)(m_UVRect[i].Top / UVChangeRadio) & 0x0FF) << 8) | (((int)(m_UVRect[i].Right / UVChangeRadio) & 0x0FF) << 16) | (((int)(m_UVRect[i].Bottom / UVChangeRadio) & 0x0FF) << 24));
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        int[] interval = new int[2];
        //                        if (m_slider[0] < m_slider[1])
        //                        {
        //                            interval[0] = (int)(m_slider[0] / m_ChangeRadio + 0.5) - 2;
        //                            interval[1] = (int)(m_slider[1] / m_ChangeRadio + 0.5) - 2;
        //                        }
        //                        else
        //                        {
        //                            interval[0] = (int)(m_slider[1] / m_ChangeRadio + 0.5) - 2;
        //                            interval[1] = (int)(m_slider[0] / m_ChangeRadio + 0.5) - 2;
        //                        }
        //                        unColorIntervals[0] = (uint)interval[0];
        //                        unColorIntervals[1] = (uint)interval[1];
        //                        unColorIntervals[2] = 0;
        //                    }

        //                    byte[] tempImagedata = new byte[m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height * 3];
        //                    m_fruitRGBImage = new byte[(3 * BackgroundLength + m_spliceImageData.imageInfo.height * 2) * m_spliceImageData.imageInfo.width * 3];
        //                    byte[] imagedata;
        //                    if (m_spliceImageData.imageInfo.bPixelBit == 1)
        //                    {
        //                        imagedata = new byte[m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height];//Y值
        //                        Array.Copy(m_spliceImageData.imagedataC, 0, imagedata, 0, m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height);
        //                        Commonfunction.YUV422GrayChangeToRGB24(imagedata, ref tempImagedata, m_spliceImageData.imageInfo.width, m_spliceImageData.imageInfo.height);
        //                    }
        //                    else
        //                    {
        //                        imagedata = new byte[m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height * 2];
        //                        Array.Copy(m_spliceImageData.imagedataC, 0, imagedata, 0, m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height * 2);
        //                        Commonfunction.YUV422ChangeToRGB24(imagedata, ref tempImagedata, m_spliceImageData.imageInfo.width, m_spliceImageData.imageInfo.height);
        //                    }
        //                    Array.Copy(tempImagedata, 0, m_fruitRGBImage, BackgroundLength * m_spliceImageData.imageInfo.width * 3, m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height * 3);//获取图片上半部分
        //                    m_srcfruitRGBImage = new byte[m_fruitRGBImage.Length];
        //                    Array.Copy(m_fruitRGBImage, m_srcfruitRGBImage, m_fruitRGBImage.Length);    //Add by ChengSk - 20190902
        //                    Array.Copy(tempImagedata, 0, m_fruitRGBImage, (2 * BackgroundLength + m_spliceImageData.imageInfo.height) * m_spliceImageData.imageInfo.width * 3, m_spliceImageData.imageInfo.width * m_spliceImageData.imageInfo.height * 3);//获取完整图片
        //                    int height;
        //                    height = 3 * BackgroundLength + m_spliceImageData.imageInfo.height * 2;


        //                    if ((m_nColorType & 0x08) > 0)//百分比
        //                    {
        //                        //this.ColorPercentlabel.Text = m_resourceManager.GetString("ColorPercentlabel.Text")+m_resourceManager.GetString("ColorIntervallabel.Text")+string.Format(" 1 {0}", ColorCount[0])+m_resourceManager.GetString("ColorIntervallabel.Text")+string.Format(" 2 {0}", ColorCount[1])+m_resourceManager.GetString("ColorIntervallabel.Text")+string.Format(" 3 {0}", ColorCount[2]);
        //                        this.ColorResultlabel.Text = m_resourceManager.GetString("ColorPercentlabel.Text") + m_resourceManager.GetString("ColorIntervallabel.Text") + string.Format(" 1- {0}  ", ColorCount[0]) + m_resourceManager.GetString("ColorIntervallabel.Text") + string.Format(" 2- {0}  ", ColorCount[1]) + m_resourceManager.GetString("ColorIntervallabel.Text") + string.Format(" 3- {0}", ColorCount[2]);
        //                        m_colorInterval = 0.0f;
        //                    }
        //                    else//平均值
        //                    {
        //                        switch (m_nColorType & 0x07)
        //                        {
        //                            //case 2://H
        //                            //    m_colorInterval = (ColorCount[0] + 182) * m_ChangeRadio;
        //                            //    break;
        //                            case 4://UV
        //                                m_colorInterval = (ColorCount[1] + 2) * m_ChangeRadio;//贴图偏移2，为了标尺数字可显示
        //                                break;
        //                            default:
        //                                m_colorInterval = (ColorCount[0] + 2) * m_ChangeRadio;
        //                                break;
        //                        }

        //                        this.ColorResultlabel.Text = m_resourceManager.GetString("ColorAvgThresholdlabel.Text") + string.Format(" {0}", (m_colorInterval / m_ChangeRadio - 2));
        //                        //MessageBox.Show("输出平均阈值");
        //                        this.ColorpictureBox.Invalidate();
        //                    }
        //                    m_DrawImage = true;
        //                    this.ColorImagepictureBox.Invalidate(); ;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorPictureChange出错" + ex);
        //#if REALEASE
        //                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorPictureChange出错" + ex);
        //#endif
        //            }



        //        }

        /// <summary>
        /// 滚动条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FruitImagepanel_Scroll(object sender, ScrollEventArgs e)
        {
            if (m_FruitImage != null)
            {
                m_DrawImage = true;
                this.ColorImagepictureBox.Invalidate();;
            }
        }

        /// <summary>
        /// 画水果图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorImagepictureBox_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics graphics = e.Graphics;//创建画板
                //Image image;
                GCHandle handle;
                IntPtr scan;
                int width = m_spliceImageData.imageInfo.width;
                int height;
                int stride = width * 3;

                //画图片
                if (m_DrawImage)
                {
                    if (m_spliceImageData.imageInfo.width != 0 && m_spliceImageData.imageInfo.height != 0)
                    {
                        height = 3 * BackgroundLength + m_spliceImageData.imageInfo.height * 2;
                        //2015-8-20 ivycc 按原图大小显示
                        if (this.ColorImagepictureBox.Width != width || this.ColorImagepictureBox.Height != height)
                        {
                            this.ColorImagepictureBox.Width = width;
                            this.ColorImagepictureBox.Height = height;
                            this.ColorImagepictureBox.Invalidate();
                            return;
                        }
                       
                        handle = GCHandle.Alloc(m_fruitRGBImage, GCHandleType.Pinned);
                        scan = handle.AddrOfPinnedObject();
                        m_FruitImage = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, scan);
                        graphics.DrawImage(m_FruitImage, 0, 0, width, height);//2015-8-20 ivycc 按原图大小显示
                    
                        handle.Free();

                        //画矩形框
                        Pen pen = new Pen(Color.White, 2);
                        Rectangle rect = new Rectangle(m_UVColorRect[0].Left, m_UVColorRect[0].Top, m_UVColorRect[0].Right - m_UVColorRect[0].Left, m_UVColorRect[0].Bottom - m_UVColorRect[0].Top);
                        graphics.DrawRectangle(pen, rect);

                        m_DrawImage = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorImagepictureBox_Paint出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorImagepictureBox_Paint出错" + ex);
#endif
            } 
        }

        /// <summary>
        /// 颜色区间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorAreaTestbutton_Click(object sender, EventArgs e)
        {

            try
            {
                if (m_spliceImageData.imageInfo.nRouteId > 0)
                {
                    Rect tempUVColorRect0 = new Rect();
                    float radio = (float)this.ColorImagepictureBox.Width / m_spliceImageData.imageInfo.width;
                    tempUVColorRect0.Bottom = (int)(m_UVColorRect[0].Bottom / radio);
                    tempUVColorRect0.Left = (int)(m_UVColorRect[0].Left / radio);
                    tempUVColorRect0.Right = (int)(m_UVColorRect[0].Right / radio);
                    tempUVColorRect0.Top = (int)(m_UVColorRect[0].Top / radio);
                    m_UVColorRect[1] = Commonfunction.GetRGB24ColorIntervalRect(m_fruitRGBImage, tempUVColorRect0, m_spliceImageData.imageInfo.width, m_spliceImageData.imageInfo.height);
                    //[debug]this.ColorpictureBox.Invalidate();
                    this.ColorpictureBox.Invalidate(); ;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorAreaTestbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorAreaTestbutton_Click出错" + ex);
#endif
            } 
        }

        /// <summary>
        /// 标定标签颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagDemarcatebutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_spliceImageData.imageInfo.nRouteId > 0)
                {
                    Rect tempUVColorRect0 = new Rect();
                    float radio = (float)this.ColorImagepictureBox.Width / m_spliceImageData.imageInfo.width;
                    tempUVColorRect0.Bottom = (int)(m_UVColorRect[0].Bottom / radio);
                    tempUVColorRect0.Left = (int)(m_UVColorRect[0].Left / radio);
                    tempUVColorRect0.Right = (int)(m_UVColorRect[0].Right / radio);
                    tempUVColorRect0.Top = (int)(m_UVColorRect[0].Top / radio);
                    m_TagBGR = Commonfunction.GetAverageColor(m_fruitRGBImage, tempUVColorRect0, m_spliceImageData.imageInfo.width, m_spliceImageData.imageInfo.height);
                    this.ColorResultlabel.Text = m_resourceManager.GetString("TagDemarcatelabel.Text") + string.Format("R {0},G {1},B {2}", m_TagBGR.bR, m_TagBGR.bG, m_TagBGR.bB);

                    //Commonfunction.SetAppSetting("标签B均值", m_TagBGR.bB.ToString());
                    //Commonfunction.SetAppSetting("标签G均值", m_TagBGR.bG.ToString());
                    //Commonfunction.SetAppSetting("标签R均值", m_TagBGR.bR.ToString());//modify by xcw 20200709
                    GlobalDataInterface.globalOut_GradeInfo.nTagInfo[1] = m_TagBGR.bB;
                    GlobalDataInterface.globalOut_GradeInfo.nTagInfo[2] = m_TagBGR.bG;
                    GlobalDataInterface.globalOut_GradeInfo.nTagInfo[3] = m_TagBGR.bR;

                    //if (GlobalDataInterface.global_IsTestMode)
                    //{
                    //    GlobalDataInterface.TransmitParam(m_CurrentIPM_ID, (int)HC_IPM_COMMAND_TYPE.HC_CMD_TAG_BGR, m_TagBGR);
                    //}
                }
                else
                {
                    this.TagDemarcatebutton.Enabled = false;
                    //Commonfunction.SetAppSetting("标签启用", "0");
                    GlobalDataInterface.globalOut_GradeInfo.nTagInfo[0] = (byte)0;

                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorAreaTestbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorAreaTestbutton_Click出错" + ex);
#endif
            } 
        }

        /// <summary>
        /// 水果图片鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorImagepictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_spliceImageData.imageInfo.nRouteId > 0)
            {
                m_UVColorRect[0].Left = e.X;
                m_UVColorRect[0].Top = e.Y;
                m_fruitImageMouseDown = true;
            }
        }

        /// <summary>
        /// 水果图片鼠标放开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorImagepictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_fruitImageMouseDown)
            {
                m_fruitImageMouseDown = false;
                m_UVColorRect[0].Right = e.X;
                m_UVColorRect[0].Bottom = e.Y;
                m_DrawImage = true;
                this.ColorImagepictureBox.Invalidate();;
            }
        }

        /// <summary>
        /// 水果图片鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorImagepictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_fruitImageMouseDown)
            {
                m_UVColorRect[0].Right = e.X;
                m_UVColorRect[0].Bottom = e.Y;
                m_DrawImage = true;
                this.ColorImagepictureBox.Invalidate();
            }
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveImagebutton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "JPG格式(*.jpg)|*.jpg|位图(*.bmp)|*.bmp|GIF格式(*.gif)|*.gif|PNG格式(*.png)|*.png";
                // dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    m_FruitImage.Save(dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数SaveImagebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数SaveImagebutton_Click出错" + ex);
#endif
            } 
        }


        /// <summary>
        /// 保存原图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveSrcImagebutton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "JPG格式(*.jpg)|*.jpg|位图(*.bmp)|*.bmp|GIF格式(*.gif)|*.gif|PNG格式(*.png)|*.png";
                // dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Rectangle rect = new Rectangle(0, 0, m_FruitImage.Width, m_FruitImage.Height / 2);
                    Bitmap SrcImage = m_FruitImage.Clone(rect, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    SrcImage.Save(dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数SaveImagebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数SaveImagebutton_Click出错" + ex);
#endif
            } 
        }

        /// <summary>
        /// 保存颜色设置
        /// </summary>
        /// <returns></returns>
        private bool ColorSetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.ColorlistViewEx.Items.Count; i++)
                {
                    if (this.ColorlistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("颜色等级名称不能为空！");
                        //MessageBox.Show("0x30001011 The color name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x30001011 " + LanguageContainer.ColorSetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.ColorSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.ColorlistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.ColorlistViewEx.Items[j].SubItems[0].Text, this.ColorlistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("颜色等级名称不能重名！");
                            //MessageBox.Show("0x30001012 The color names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x30001012 " + LanguageContainer.ColorSetFormMessagebox3Text[GlobalDataInterface.selectLanguageIndex], 
                                LanguageContainer.ColorSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }

                if (this.ColorlistViewEx.Items.Count <= 1)
                {
                    GlobalDataInterface.ColorGradeNum = 1;
                    for (int k = 0; k < 3; k++)
                    {
                        m_percent[k].nMin = 0;
                        m_percent[k].nMax = 100;
                    }
                }
                GlobalDataInterface.ColorGradeNum = this.ColorlistViewEx.Items.Count;
                m_ColorGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.strColorGradeName, 0);

                for (int i = 0; i < GlobalDataInterface.ColorGradeNum * ConstPreDefine.MAX_COLOR_INTERVAL_NUM; i++)
                {
                    // 数值*2 测试代码  Update By ChengSk 20140626
                    m_percent[i].nMin = (byte)((int)m_percent[i].nMin * 2);
                    m_percent[i].nMax = (byte)((int)m_percent[i].nMax * 2);
                    //End
                    GlobalDataInterface.globalOut_GradeInfo.percent[i].ToCopy(m_percent[i]);
                }

                float UVChangeRadio = this.ColorpictureBox.Height / 256.0f;
                for (int i = 0; i < ConstPreDefine.MAX_COLOR_INTERVAL_NUM; i++)
                {
                    GlobalDataInterface.globalOut_GradeInfo.intervals[i].nMinV = (byte)(m_UVRect[i].Left / UVChangeRadio);
                    GlobalDataInterface.globalOut_GradeInfo.intervals[i].nMinU = (byte)(m_UVRect[i].Top / UVChangeRadio);
                    GlobalDataInterface.globalOut_GradeInfo.intervals[i].nMaxV = (byte)(m_UVRect[i].Right / UVChangeRadio);
                    GlobalDataInterface.globalOut_GradeInfo.intervals[i].nMaxU = (byte)(m_UVRect[i].Bottom / UVChangeRadio);
                }

                if (m_slider[0] < m_slider[1])
                {
                    GlobalDataInterface.globalOut_GradeInfo.ColorIntervals[0] = (int)(m_slider[0] / m_ChangeRadio + 0.5) - 2;
                    GlobalDataInterface.globalOut_GradeInfo.ColorIntervals[1] = (int)(m_slider[1] / m_ChangeRadio + 0.5) - 2;
                }
                else
                {
                    GlobalDataInterface.globalOut_GradeInfo.ColorIntervals[0] = (int)(m_slider[1] / m_ChangeRadio + 0.5) - 2;
                    GlobalDataInterface.globalOut_GradeInfo.ColorIntervals[1] = (int)(m_slider[0] / m_ChangeRadio + 0.5) - 2;
                }
                GlobalDataInterface.globalOut_GradeInfo.ColorType = (byte)m_nColorType;
                GlobalDataInterface.ColorMinGray = m_MinGray;

                Commonfunction.SetAppSetting("颜色参数-颜色3", ColorTranslator.ToHtml(m_ColorList[0]));
                Commonfunction.SetAppSetting("颜色参数-颜色2", ColorTranslator.ToHtml(m_ColorList[1]));
                Commonfunction.SetAppSetting("颜色参数-颜色1", ColorTranslator.ToHtml(m_ColorList[2])); //modify by xcw 20200731
                if ((m_nColorType & 0x08) > 0)//百分比
                {
                    GlobalDataInterface.ColorGradeNum = this.ColorlistViewEx.Items.Count;
                }
                else//平均值
                {
                    GlobalDataInterface.ColorGradeNum = 3;
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数ColorSetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数ColorSetSaveConfig出错" + ex);
#endif
                return false;
            } 
        }

        /// <summary>
        /// 标签启用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagDemarcatecheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if(checkBox.Checked)
                {
                    this.TagDemarcatebutton.Enabled = true;
                    //Commonfunction.SetAppSetting("标签启用", "1");
                    GlobalDataInterface.globalOut_GradeInfo.nTagInfo[0] = (byte)1;
                }
                else
                {
                    this.TagDemarcatebutton.Enabled = false;
                    //Commonfunction.SetAppSetting("标签启用", "0");
                    GlobalDataInterface.globalOut_GradeInfo.nTagInfo[0] = (byte)0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数TagDemarcatecheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数TagDemarcatecheckBox_CheckedChanged出错" + ex);
#endif
            } 
        }
    }
}
