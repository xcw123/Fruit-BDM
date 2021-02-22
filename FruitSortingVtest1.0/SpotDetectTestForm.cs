using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interface;
using Common;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FruitSortingVtest1._0
{
    public partial class SpotDetectTestForm : Form
    {
        Bitmap m_srcBitamp, m_dstBitamp;
        Rectangle m_InitialPictureBoxRectangle,m_InitialDstPictureBoxRectangle;//初始设置图片大小
        stSpotDetectTest m_spotDetectTest;
        stSpliceImageData m_spotimageData;//当前IPM上传图像信息
        byte[] m_SpotDetectRGBImage;//IPM获取的图像数据

        public SpotDetectTestForm()
        {
            InitializeComponent();
            GlobalDataInterface.UpSpotImageDataEvent += new GlobalDataInterface.SpotImageDataEventHandler(OnUpSpotImageData);

        }

        private void SpotDetectTestForm_Load(object sender, EventArgs e)
        {
            m_spotDetectTest = new stSpotDetectTest(true);
            m_InitialPictureBoxRectangle.X = this.SrcImagepictureBox.Location.X;
            m_InitialPictureBoxRectangle.Y = this.SrcImagepictureBox.Location.Y; 
            m_InitialPictureBoxRectangle.Width = this.SrcImagepictureBox.Width;  //3200
            m_InitialPictureBoxRectangle.Height = this.SrcImagepictureBox.Height; //512

            m_InitialDstPictureBoxRectangle.X = this.DstpictureBox.Location.X;
            m_InitialDstPictureBoxRectangle.Y = this.DstpictureBox.Location.Y;
            m_InitialDstPictureBoxRectangle.Width = this.DstpictureBox.Width;  //3200
            m_InitialDstPictureBoxRectangle.Height = this.DstpictureBox.Height; //512
        }
        /// <summary>
        /// 加载图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadImagebutton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有文件|*.bmp;*.pcx;*.png;*.jpg;*.gif;+*.tif;*.ico;*.dcx;*.cgm;*.cdr;*.wmf;*.eps;*.emf;|"
                    + "位图(*.bmp;*.jpg;*.png;...)|*.bmp;*.pcx;*.png;*.jpg;*.gif;*.tif;*.ico;|"
                    + "矢量图(*.wmf;*.eps;*.emf;...)|*.dcx;*.cgm;*.cdr;*.wmf;*.eps;*.emf;";
                openFileDialog.Title = "open Image File";
                openFileDialog.ShowHelp = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = openFileDialog.FileName;
                    this.ImagePathtextBox.Text = openFileDialog.FileName;
                    m_srcBitamp = (Bitmap)Image.FromFile(openFileDialog.FileName);
                    //SetImagepictureBox(m_srcBitamp.Width, m_srcBitamp.Height);
                    //this.ImageWidthnumericUpDown.Text = m_srcBitamp.Width.ToString();
                    //this.ImageHeightnumericUpDown.Text = m_srcBitamp.Height.ToString();
                    //this.CupNumnumericUpDown.Text = GlobalDataInterface.globalOut_Paras[0].nCupNum.ToString();
                    //int nCupW=(GlobalDataInterface.globalOut_Paras[0].cameraParas[0].cup[0].nLeft[1]-GlobalDataInterface.globalOut_Paras[0].cameraParas[0].cup[0].nLeft[0])/GlobalDataInterface.globalOut_Paras[0].nCupNum;
                    //int nNewCupW=(2*nCupW+4)&0xfffe;
                    //this.CupWidthnumericUpDown.Maximum = m_srcBitamp.Width;
                    //this.CupWidthnumericUpDown.Text = nNewCupW.ToString();
                    //this.StartXnumericUpDown.Maximum = m_srcBitamp.Width;
                    //this.StartXnumericUpDown.Text = "20";
                    //this.FruitTypenumericUpDown.Text = GlobalDataInterface.globalOut_GradeInfo.nFruitType.ToString();
                    //获取图像数据
                    this.SrcImagepictureBox.Width = m_srcBitamp.Width;
                    this.SrcImagepictureBox.Height = m_srcBitamp.Height;
                    
                    BitmapData bmpData = new BitmapData();
                    bmpData = m_srcBitamp.LockBits(new Rectangle(0, 0, m_srcBitamp.Width - 1, m_srcBitamp.Height - 1), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                    Marshal.Copy(bmpData.Scan0, m_spotDetectTest.imagedataC, 0, bmpData.Stride * (bmpData.Height + 1));
                    //Marshal.Copy(bmpData.Scan0, m_spotDetectTest.imagedataC, 0, ConstPreDefine.MAX_SPLICE_IMAGE_WIDTH * 100);
                    m_srcBitamp.UnlockBits(bmpData);
                    this.SrcImagepictureBox.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("SpotDetectTestForm中函数 LoadImagebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("SpotDetectTestForm中函数 LoadImagebutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置ImagepictureBox位置和大小
        /// </summary>
        private void SetImagepictureBox(int ImageWidth, int ImageHeight)
        {
            try
            {
                Rect r;
                r.Left = m_InitialPictureBoxRectangle.Left;
                r.Right = m_InitialPictureBoxRectangle.Right;
                r.Top = m_InitialPictureBoxRectangle.Top;
                r.Bottom = m_InitialPictureBoxRectangle.Bottom;

                float fWidth = (float)m_InitialPictureBoxRectangle.Width / ImageWidth;
                float fHeight = (float)m_InitialPictureBoxRectangle.Height / ImageHeight;

                float fTransferRatio = (fWidth > fHeight ? fHeight : fWidth);//全局缩放取比较小的那个

                Rectangle rect = new Rectangle(r.Left, r.Top, (int)(ImageWidth * fTransferRatio), (int)(ImageHeight * fTransferRatio));
                if (fWidth > fHeight)
                {
                    //图像比较高，所以显示框要左右居中
                    // rect.Offset((m_InitialPictureBoxRectangle.Width - rect.Width) / 2, 0);
                    rect.Offset((this.SrcImagepictureBox.Width - rect.Width) / 2, 0);
                }
                else
                {
                    //图像细长，所以显示框要上下居中
                    //rect.Offset(0, (m_InitialPictureBoxRectangle.Height - rect.Height) / 2);
                    rect.Offset(0, (this.SrcImagepictureBox.Height - rect.Height) / 2);
                }

                this.SrcImagepictureBox.Size = rect.Size;
                this.SrcImagepictureBox.Location = rect.Location;

                rect = new Rectangle(m_InitialDstPictureBoxRectangle.Left, m_InitialDstPictureBoxRectangle.Top, (int)(ImageWidth * fTransferRatio), (int)(ImageHeight * fTransferRatio));
                if (fWidth > fHeight)
                {
                    //图像比较高，所以显示框要左右居中
                    // rect.Offset((m_InitialPictureBoxRectangle.Width - rect.Width) / 2, 0);
                    rect.Offset((this.DstpictureBox.Width - rect.Width) / 2, 0);
                }
                else
                {
                    //图像细长，所以显示框要上下居中
                    //rect.Offset(0, (m_InitialPictureBoxRectangle.Height - rect.Height) / 2);
                    rect.Offset(0, (this.DstpictureBox.Height - rect.Height) / 2);
                }
                this.DstpictureBox.Size = rect.Size;
                this.DstpictureBox.Location = rect.Location;

            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数SetImagepictureBox出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数SetImagepictureBox出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 画原图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SrcImagepictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            if (m_srcBitamp != null)
            {
                //graphics.DrawImage(m_srcBitamp, 0, 0, this.SrcImagepictureBox.Width, this.SrcImagepictureBox.Height);
                graphics.DrawImage(m_srcBitamp, 0, 0, m_srcBitamp.Width, m_srcBitamp.Height);
            }
        }

        /// <summary>
        /// 获取图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetImagebutton_Click(object sender, EventArgs e)
        {
            m_spotDetectTest.spotDetectTestInfo.nImgW = m_srcBitamp.Width;
            m_spotDetectTest.spotDetectTestInfo.nImgH = m_srcBitamp.Height;
            //m_spotDetectTest.spotDetectTestInfo.nCupNum = int.Parse(this.CupNumnumericUpDown.Text);
            //m_spotDetectTest.spotDetectTestInfo.nCupW = int.Parse(this.CupWidthnumericUpDown.Text);
            //m_spotDetectTest.spotDetectTestInfo.nStartX = int.Parse(this.StartXnumericUpDown.Text);
            //m_spotDetectTest.spotDetectTestInfo.nFruitType = int.Parse(this.FruitTypenumericUpDown.Text);
            if (GlobalDataInterface.global_IsTestMode)
            {
                GlobalDataInterface.TransmitParam(272, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SPOT_DETECT_TEST, m_spotDetectTest);//只给17发送
            }
        }

        /// <summary>
        /// 上传图像显示刷新
        /// </summary>
        /// <param name="imageInfo"></param>
        public void OnUpSpotImageData(stSpliceImageData spotImageData)
        {
            try
            {
                if (this == Form.ActiveForm)
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new GlobalDataInterface.SpotImageDataEventHandler(OnUpSpotImageData), spotImageData);
                    }
                    else
                    {
                        if (spotImageData.imageInfo.nRouteId != 272 || spotImageData.imageInfo.width != m_srcBitamp.Width)//|| spotImageData.imageInfo.height != 20* m_srcBitamp.Height)
                            return;
                        this.DstpictureBox.Width = m_srcBitamp.Width;
                        this.DstpictureBox.Height = spotImageData.imageInfo.height;
                        m_spotimageData = new stSpliceImageData(spotImageData.imagedataC.Length);
                        m_spotimageData.ToCopy(spotImageData);
                        byte[] tempImagedata = new byte[spotImageData.imageInfo.width * spotImageData.imageInfo.height * 2];
                        m_SpotDetectRGBImage = new byte[spotImageData.imageInfo.width * spotImageData.imageInfo.height * 4];
                        Array.Copy(m_spotimageData.imagedataC, 0, tempImagedata, 0, m_spotimageData.imageInfo.width * m_spotimageData.imageInfo.height * 2);
                        Commonfunction.YUV422ChangeToRGB(tempImagedata, ref m_SpotDetectRGBImage, m_spotimageData.imageInfo.width, m_spotimageData.imageInfo.height);
                        this.FlawAreatextBox.Text = m_spotimageData.imageInfo.unFlawArea.ToString();
                        this.FlawNumtextBox.Text = m_spotimageData.imageInfo.unFlawNum.ToString();
                        this.DstpictureBox.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ColorSetForm中函数OnUpImageInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ColorSetForm中函数OnUpImageInfo出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 画IPM上传图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DstpictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;//创建画板
            GCHandle handle;
            IntPtr scan;
            int width = m_spotimageData.imageInfo.width;
            int height = m_spotimageData.imageInfo.height;
            int stride = width * 4;

            if (m_SpotDetectRGBImage != null)
            {
                handle = GCHandle.Alloc(m_SpotDetectRGBImage, GCHandleType.Pinned);
                scan = handle.AddrOfPinnedObject();
                m_dstBitamp = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppRgb, scan);
                graphics.DrawImage(m_dstBitamp, 0, 0, this.DstpictureBox.Width, this.DstpictureBox.Height);
            }
        }

        private void SpotDetectTestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                GlobalDataInterface.UpSpotImageDataEvent -= new GlobalDataInterface.SpotImageDataEventHandler(OnUpSpotImageData); //Add by ChengSk - 20180830
            }
            catch (Exception ex)
            {
                Trace.WriteLine("SpotDetectTestForm中函数 SpotDetectTestForm_FormClosing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("SpotDetectTestForm中函数 SpotDetectTestForm_FormClosing出错" + ex);
#endif
            }
        }
    }
}
