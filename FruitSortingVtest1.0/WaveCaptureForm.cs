using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interface;
using Common;
using System.Diagnostics;
using System.Resources;

namespace FruitSortingVtest1._0
{
    public partial class WaveCaptureForm : Form
    {
        private static int rectangleWidth = 10;
        private static int _parameters = 4096;
        public static int sign = GlobalDataInterface.globalIn_defaultInis.FirstOrDefault().nFsmModule;
        int m_nChannelID;
        byte[] m_waveInterval;
        float m_fTransRatio = 0.0f;
        Rectangle[] m_Intervalrect = new Rectangle[2];
        bool m_MouseDown = false;
        int m_tempInterval = 0;
        bool m_IntervalCatched = false;
        int m_IntervalCatchedType = 0; //1, min;2，max
        PointF[] m_wave1Point = new PointF[768];
        PointF[] m_wave2Point = new PointF[768];  //modify by xcw 20201128
        bool m_IsWaveCapturing = true; //是否在进行波形捕捉
        byte[] m_EndInterval = new byte[2];
        private ResourceManager m_resourceManager = new ResourceManager(typeof(WaveCaptureForm));//创建WaveCaptureForm资源管理
        bool m_IntervalValueChanged = false;
        bool m_IntervalValueChanged1 = false;
        bool m_IntervalValueChanged2 = false;

        public WaveCaptureForm(int nChannelID, ref byte[] interval)
        {
            m_nChannelID = nChannelID;
            m_waveInterval = interval;
            m_EndInterval = m_waveInterval;
            GlobalDataInterface.UpWaveInfoEvent += new GlobalDataInterface.WaveInfoEventHandler(OnUpWaveInfo);
            InitializeComponent();
            sign = GlobalDataInterface.globalIn_defaultInis.FirstOrDefault().nFsmModule;
        }


        private void WaveCaptureForm_Load(object sender, EventArgs e)
        {
            try
            {
                sign = GlobalDataInterface.globalIn_defaultInis.FirstOrDefault().nFsmModule;
                this.WaveCapturehScrollBar.Enabled = false;//波形捕捉时不许查看之前数据
                if (sign == 1)
                {
                    _parameters = 4096;
                    m_fTransRatio = (float)this.WeightWavepictureBox.Height / (4096.0f * 2.0f);
                }
                else
                {
                    _parameters = 65535;
                    m_fTransRatio = (float)this.WeightWavepictureBox.Height / (65535.0f * 2.0f);
                }


                this.WaveCapturehScrollBar.Value = this.WaveCapturehScrollBar.Maximum;
                lock (this)
                {
                    GlobalDataInterface.globalIn_wavelist.Clear();
                }

                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(m_nChannelID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_WAVE_FORM_ON, null);//开启波形捕捉
                }
                this.IntervalnumericUpDown1.Text = m_waveInterval[0].ToString();
                this.IntervalnumericUpDown2.Text = m_waveInterval[1].ToString();
                m_IntervalValueChanged1 = false;
                m_IntervalValueChanged2 = false;
                this.WeightWavepictureBox.Invalidate();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WaveCaptureForm中函数WaveCaptureForm_Load出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("WaveCaptureForm中函数WaveCaptureForm_Load出错" + ex);
#endif
            }
        }


        private void WeightWavepictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;//创建画板

            DrawPictureBox(graphics);
        }

        private void DrawPictureBox(Graphics graphics)
        {
            try
            {
                //画框架
                Pen pen = new Pen(Color.Black, 2);
                graphics.DrawLine(pen, 0, 0, 0, this.WeightWavepictureBox.Height);
                pen = new Pen(Color.Black, 3);
                graphics.DrawLine(pen, 0, this.WeightWavepictureBox.Height, this.WeightWavepictureBox.Width, this.WeightWavepictureBox.Height);
                pen = new Pen(Color.Black, 1);
                graphics.DrawLine(pen, 0, this.WeightWavepictureBox.Height / 2, this.WeightWavepictureBox.Width, this.WeightWavepictureBox.Height / 2);

                pen = new Pen(Color.Blue, 2);
                pen.DashPattern = new float[] { 8, 2 };//自定义虚线（短线8，空白2）
                graphics.DrawLine(pen, this.WeightWavepictureBox.Width / 3, 0, this.WeightWavepictureBox.Width / 3, this.WeightWavepictureBox.Height);
                graphics.DrawLine(pen, this.WeightWavepictureBox.Width * 2 / 3, 0, this.WeightWavepictureBox.Width * 2 / 3, this.WeightWavepictureBox.Height);

                //画区间
                pen = new Pen(Color.Yellow, 2);
                graphics.DrawLine(pen, this.WeightWavepictureBox.Width / 3 + m_waveInterval[0] * this.WeightWavepictureBox.Width / 768, 0, this.WeightWavepictureBox.Width / 3 + m_waveInterval[0] * this.WeightWavepictureBox.Width / 768, this.WeightWavepictureBox.Height);
                graphics.DrawLine(pen, this.WeightWavepictureBox.Width / 3 + m_waveInterval[1] * this.WeightWavepictureBox.Width / 768, 0, this.WeightWavepictureBox.Width / 3 + m_waveInterval[1] * this.WeightWavepictureBox.Width / 768, this.WeightWavepictureBox.Height);

                m_Intervalrect[0] = new Rectangle(this.WeightWavepictureBox.Width / 3 + m_waveInterval[0] * this.WeightWavepictureBox.Width / 768 - rectangleWidth / 2, this.WeightWavepictureBox.Height / 2 - rectangleWidth / 2, rectangleWidth, rectangleWidth);
                SolidBrush brush = new SolidBrush(Color.Yellow);//定义画刷
                graphics.FillRectangle(brush, m_Intervalrect[0]);//填充矩形

                m_Intervalrect[1] = new Rectangle(this.WeightWavepictureBox.Width / 3 + m_waveInterval[1] * this.WeightWavepictureBox.Width / 768 - rectangleWidth / 2, this.WeightWavepictureBox.Height / 2 - rectangleWidth / 2, rectangleWidth, rectangleWidth);
                graphics.FillRectangle(brush, m_Intervalrect[1]);//填充矩形
                if (m_waveInterval[0] > m_waveInterval[1])
                {
                    byte tempInterval = m_waveInterval[0];
                    m_waveInterval[0] = m_waveInterval[1];
                    m_waveInterval[1] = tempInterval;

                    Rectangle tempRect = m_Intervalrect[0];
                    m_Intervalrect[0] = m_Intervalrect[1];
                    m_Intervalrect[1] = tempRect;
                }
                if (!m_IntervalValueChanged)
                {
                    this.IntervalnumericUpDown1.Text = m_waveInterval[0].ToString();
                    this.IntervalnumericUpDown2.Text = m_waveInterval[1].ToString();
                    m_IntervalValueChanged1 = false;
                    m_IntervalValueChanged2 = false;
                }
                else
                {
                    m_IntervalValueChanged = false;
                }
                //画波形
                pen = new Pen(Color.Lime, 2);
                graphics.DrawLines(pen, m_wave1Point);
                graphics.DrawLines(pen, m_wave2Point);
                _parameters = sign == 2 ? 65535 : 4096;
                this.AD0mintextBox.Text = ((int)(_parameters - m_wave1Point[256 + m_waveInterval[0]].Y / m_fTransRatio)).ToString();
                this.AD0maxtextBox.Text = ((int)(_parameters - m_wave1Point[256 + m_waveInterval[1]].Y / m_fTransRatio)).ToString();
                this.AD1mintextBox.Text = ((int)(_parameters - (m_wave2Point[256 + m_waveInterval[0]].Y - this.WeightWavepictureBox.Height / 2) / m_fTransRatio)).ToString();
                this.AD1maxtextBox.Text = ((int)(_parameters - (m_wave2Point[256 + m_waveInterval[1]].Y - this.WeightWavepictureBox.Height / 2) / m_fTransRatio)).ToString();

                if (m_IntervalCatched)
                {
                    //Pen pen;

                    //this.Cursor = Cursors.SizeWE;
                    //pictureBox.Invalidate();


                    //跟随鼠标画虚线
                    pen = new Pen(Color.Black, 1);
                    pen.DashPattern = new float[] { 4, 2 };//自定义虚线（短线8，空白2）


                    graphics.DrawLine(pen, m_tempInterval, 0, m_tempInterval, this.WeightWavepictureBox.Height);
                    pen = new Pen(Color.Black, 3);
                    graphics.DrawLine(pen, 0, this.WeightWavepictureBox.Height, this.WeightWavepictureBox.Width, this.WeightWavepictureBox.Height);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WaveCaptureForm中函数DrawPictureBox出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("WaveCaptureForm中函数DrawPictureBox出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeightWavepictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                PictureBox pictureBox = (PictureBox)sender;

                Graphics graphics = pictureBox.CreateGraphics();//创建画板


                if ((e.X >= m_Intervalrect[0].X) && (e.X <= m_Intervalrect[0].X + m_Intervalrect[0].Width) && (e.Y >= m_Intervalrect[0].Y) && (e.Y <= m_Intervalrect[0].Y + m_Intervalrect[0].Height))
                {
                    this.Cursor = Cursors.SizeWE;

                    if (m_MouseDown && !m_IntervalCatched)
                    {
                        m_tempInterval = (m_waveInterval[0] + 256) * 768 / this.WeightWavepictureBox.Width;
                        m_IntervalCatchedType = 1;
                        m_IntervalCatched = true;
                    }
                }
                else if ((e.X >= m_Intervalrect[1].X) && (e.X <= m_Intervalrect[1].X + m_Intervalrect[1].Width) && (e.Y >= m_Intervalrect[1].Y) && (e.Y <= m_Intervalrect[1].Y + m_Intervalrect[1].Height))
                {
                    this.Cursor = Cursors.SizeWE;

                    if (m_MouseDown && !m_IntervalCatched)
                    {
                        m_tempInterval = (m_waveInterval[1] + 256) * 768 / this.WeightWavepictureBox.Width;
                        m_IntervalCatchedType = 2;
                        m_IntervalCatched = true;
                    }
                }
                else if (m_IntervalCatched)
                {
                    // Pen pen;

                    this.Cursor = Cursors.SizeWE;

                    //跟随鼠标画虚线
                    //  pen = new Pen(Color.Black, 1);
                    // pen.DashPattern = new float[] { 4, 2 };//自定义虚线（短线8，空白2）
                    if (e.X >= pictureBox.Width / 3 && e.X <= pictureBox.Width * 2 / 3)
                    {
                        m_tempInterval = e.X;
                        // graphics.DrawLine(pen, m_tempInterval, 0, m_tempInterval, this.WeightWavepictureBox.Height);
                        //pen = new Pen(Color.Black, 3);
                        //graphics.DrawLine(pen, 0, pictureBox.Height, pictureBox.Width, pictureBox.Height);
                    }
                    pictureBox.Invalidate();
                }

                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WaveCaptureForm中函数WeightWavepictureBox_MouseMove出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("WaveCaptureForm中函数WeightWavepictureBox_MouseMove出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeightWavepictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            m_MouseDown = true;
        }
        /// <summary>
        /// 鼠标放开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeightWavepictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                PictureBox pictureBox = (PictureBox)sender;
                Graphics graphics = pictureBox.CreateGraphics();//创建画板

                if (m_IntervalCatched)
                {
                    if (m_IntervalCatchedType == 1)
                    {
                        m_waveInterval[0] = (byte)(m_tempInterval * 768 / this.WeightWavepictureBox.Width - 256);
                    }
                    else
                    {
                        m_waveInterval[1] = (byte)(m_tempInterval * 768 / this.WeightWavepictureBox.Width - 256);
                    }
                    pictureBox.Invalidate();
                    m_IntervalCatched = false;
                }
                m_MouseDown = false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WaveCaptureForm中函数WeightWavepictureBox_MouseUp出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("WaveCaptureForm中函数WeightWavepictureBox_MouseUp出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 上行波形数据刷新
        /// </summary>
        public void OnUpWaveInfo()
        {
            try
            {
                if (this == Form.ActiveForm)//是否操作当前窗体
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new GlobalDataInterface.WaveInfoEventHandler(OnUpWaveInfo));
                    }
                    else
                    {
                        if (!m_IsWaveCapturing)//停止波形捕捉，不在刷新波形数据
                            return;
                        if (GlobalDataInterface.globalIn_wavelist.Count > 0)
                        {
                            if (GlobalDataInterface.globalIn_wavelist.Count > 3)
                                this.WaveCapturehScrollBar.Maximum = GlobalDataInterface.globalIn_wavelist.Count - 3;
                            this.WaveCapturehScrollBar.Value = this.WaveCapturehScrollBar.Maximum;

                            _parameters = sign == 2 ? 65535 : 4096;
                            int k = 0;
                            for (int j = GlobalDataInterface.globalIn_wavelist.Count - 1; j > (GlobalDataInterface.globalIn_wavelist.Count - 4 > 0 ? GlobalDataInterface.globalIn_wavelist.Count - 4 : 0); j--)
                            //for (int j = 0; j < 3; j++)
                            {
                                for (int i = 0; i < 256; i++)
                                {
                                    if (GlobalDataInterface.globalIn_wavelist.Count < 4)
                                    {
                                        if (GlobalDataInterface.globalIn_wavelist.Count == j)
                                            break;
                                        m_wave1Point[(2 - k) * 256 + i].X = 256 * (2 - k) * this.WeightWavepictureBox.Width / 768 + i * this.WeightWavepictureBox.Width / 768;
                                        if (_parameters == 65535)
                                        {
                                            if (GlobalDataInterface.globalIn_wavelist[j].waveform0[i] < 0)
                                                GlobalDataInterface.globalIn_wavelist[j].waveform0[i] = 0;
                                            if (GlobalDataInterface.globalIn_wavelist[j].waveform0[i] > 65535)
                                                GlobalDataInterface.globalIn_wavelist[j].waveform0[i] = 65535;
                                        }
                                        else
                                        {
                                            if (GlobalDataInterface.globalIn_wavelist[j].waveform0[i] < 0)
                                                GlobalDataInterface.globalIn_wavelist[j].waveform0[i] = 0;
                                            if (GlobalDataInterface.globalIn_wavelist[j].waveform0[i] > 4096)
                                                GlobalDataInterface.globalIn_wavelist[j].waveform0[i] = 4096;
                                        }

                                        m_wave1Point[(2 - k) * 256 + i].Y = (float)((_parameters - GlobalDataInterface.globalIn_wavelist[j].waveform0[i]) * m_fTransRatio);

                                        m_wave2Point[(2 - k) * 256 + i].X = 256 * (2 - k) * this.WeightWavepictureBox.Width / 768 + i * this.WeightWavepictureBox.Width / 768;
                                        if (_parameters == 65535)
                                        {
                                            if (GlobalDataInterface.globalIn_wavelist[j].waveform1[i] < 0)
                                                GlobalDataInterface.globalIn_wavelist[j].waveform1[i] = 0;
                                            if (GlobalDataInterface.globalIn_wavelist[j].waveform1[i] > 65535)
                                                GlobalDataInterface.globalIn_wavelist[j].waveform1[i] = 65535;
                                        }
                                        else
                                        {
                                            if (GlobalDataInterface.globalIn_wavelist[j].waveform1[i] < 0)
                                                GlobalDataInterface.globalIn_wavelist[j].waveform1[i] = 0;
                                            if (GlobalDataInterface.globalIn_wavelist[j].waveform1[i] > 4096)
                                                GlobalDataInterface.globalIn_wavelist[j].waveform1[i] = 4096;
                                        }

                                        m_wave2Point[(2 - k) * 256 + i].Y = (float)((_parameters - GlobalDataInterface.globalIn_wavelist[j].waveform1[i]) * m_fTransRatio + this.WeightWavepictureBox.Height / 2 -1);
                                    }
                                    else
                                    {
                                        m_wave1Point[(2 - k) * 256 + i].X = 256 * (2 - k) * this.WeightWavepictureBox.Width / 768 + i * this.WeightWavepictureBox.Width / 768;
                                        if (_parameters == 65535)
                                        {
                                            if (GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform0[i] < 0)
                                                GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform0[i] = 0;
                                            if (GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform0[i] > 65535)
                                                GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform0[i] = 65535;
                                        }
                                        else
                                        {
                                            if (GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform0[i] < 0)
                                                GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform0[i] = 0;
                                            if (GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform0[i] > 4096)
                                                GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform0[i] = 4096;
                                        }

                                        m_wave1Point[(2 - k) * 256 + i].Y = (float)((_parameters - GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform0[i]) * m_fTransRatio);

                                        m_wave2Point[(2 - k) * 256 + i].X = 256 * (2 - k) * this.WeightWavepictureBox.Width / 768 + i * this.WeightWavepictureBox.Width / 768;
                                        if (_parameters == 65535)
                                        {
                                            if (GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform1[i] < 0)
                                                GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform1[i] = 0;
                                            if (GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform1[i] > 65535)
                                                GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform1[i] = 65535;
                                        }
                                        else
                                        {
                                            if (GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform1[i] < 0)
                                                GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform1[i] = 0;
                                            if (GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform1[i] > 4096)
                                                GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform1[i] = 4096;
                                        }


                                        m_wave2Point[(2 - k) * 256 + i].Y = (float)((_parameters - GlobalDataInterface.globalIn_wavelist[this.WaveCapturehScrollBar.Value - k + 2].waveform1[i]) * m_fTransRatio + this.WeightWavepictureBox.Height / 2 - 1);
                                    }

                                }
                                k++;
                            }
                            this.WeightWavepictureBox.Invalidate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WaveCaptureForm中函数OnUpWaveInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("WaveCaptureForm中函数OnUpWaveInfo出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 开始/停止捕捉按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Capturedbutton_Click(object sender, EventArgs e)
        {
            try
            {
                m_IsWaveCapturing = !m_IsWaveCapturing;
                if (m_IsWaveCapturing)
                {
                    this.Capturedbutton.Text = m_resourceManager.GetString("Capturedbutton.Text");
                    this.WaveCapturehScrollBar.Enabled = false;
                    this.WaveCapturehScrollBar.Value = this.WaveCapturehScrollBar.Maximum;
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(m_nChannelID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_WAVE_FORM_ON, null);//关闭波形捕捉
                    }
                }
                else
                {
                    this.Capturedbutton.Text = m_resourceManager.GetString("StartCapturedlabel.Text");
                    this.WaveCapturehScrollBar.Enabled = true;
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(m_nChannelID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_WAVE_FORM_OFF, null);//关闭波形捕捉
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WaveCaptureForm中函数Capturedbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("WaveCaptureForm中函数Capturedbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 滚动条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WaveCapturehScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                ScrollBar scrollBar = (ScrollBar)sender;
                _parameters = sign == 2 ? 65535 : 4096;
                if (scrollBar.Value > 0)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int i = 0; i < 256; i++)
                        {
                            m_wave1Point[(2 - j) * 256 + i].X = 256 * (2 - j) * this.WeightWavepictureBox.Width / 768 + i * this.WeightWavepictureBox.Width / 768;
                            if (_parameters == 65535)
                            {
                                if (GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform0[i] < 0)
                                    GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform0[i] = 0;
                                if (GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform0[i] > 65535)
                                    GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform0[i] = 65535;
                            }
                            else
                            {
                                if (GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform0[i] < 0)
                                    GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform0[i] = 0;
                                if (GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform0[i] > 4096)
                                    GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform0[i] = 4096;
                            }


                            m_wave1Point[(2 - j) * 256 + i].Y = (int)((_parameters - GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform0[i]) * m_fTransRatio);

                            m_wave2Point[(2 - j) * 256 + i].X = 256 * (2 - j) * this.WeightWavepictureBox.Width / 768 + i * this.WeightWavepictureBox.Width / 768;
                            if (_parameters == 65535)
                            {
                                if (GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform1[i] < 0)
                                    GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform1[i] = 0;
                                if (GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform1[i] > 65535)
                                    GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform1[i] = 65535;
                            }
                            else
                            {
                                if (GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform1[i] < 0)
                                    GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform1[i] = 0;
                                if (GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform1[i] > 4096)
                                    GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform1[i] = 4096;
                            }


                            m_wave2Point[(2 - j) * 256 + i].Y = (int)((_parameters - GlobalDataInterface.globalIn_wavelist[scrollBar.Value - j + 2].waveform1[i]) * m_fTransRatio + this.WeightWavepictureBox.Height / 2 - 1);

                        }
                    }
                    this.WeightWavepictureBox.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WaveCaptureForm中函数WaveCapturehScrollBar_Scroll出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("WaveCaptureForm中函数WaveCapturehScrollBar_Scroll出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WaveCaptureOKbutton_Click(object sender, EventArgs e)
        {
            try
            {
                m_EndInterval.CopyTo(m_waveInterval, 0);
                if (m_waveInterval[0] > m_waveInterval[1])
                {
                    byte temp = m_waveInterval[0];
                    m_waveInterval[0] = m_waveInterval[1];
                    m_waveInterval[1] = temp;
                }
                int WeightSubsysindex = Commonfunction.GetSubsysIndex(m_nChannelID);
                int WeightSubsysChannelIndex = Commonfunction.GetChannelIndex(m_nChannelID);

                GlobalDataInterface.globalOut_WeightBaseInfo[WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + WeightSubsysChannelIndex].waveinterval[0] = m_waveInterval[0];
                GlobalDataInterface.globalOut_WeightBaseInfo[WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + WeightSubsysChannelIndex].waveinterval[1] = m_waveInterval[1];
                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(m_nChannelID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHT_INFO, null);
                    GlobalDataInterface.TransmitParam(m_nChannelID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_WAVE_FORM_OFF, null);//开启波形捕捉
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WaveCaptureForm中函数WaveCaptureOKbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("WaveCaptureForm中函数WaveCaptureOKbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 波形捕捉窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WaveCaptureForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GlobalDataInterface.global_IsTestMode)
            {
                GlobalDataInterface.TransmitParam(m_nChannelID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_WAVE_FORM_OFF, null);//关闭波形捕捉
            }
            GlobalDataInterface.UpWaveInfoEvent -= new GlobalDataInterface.WaveInfoEventHandler(OnUpWaveInfo); //Add by ChengSk - 20180830
        }

        /// <summary>
        /// 区间控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntervalnumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try

            {
                if (m_IntervalValueChanged1)
                {
                    if (int.Parse(this.IntervalnumericUpDown1.Text) > 255 || int.Parse(this.IntervalnumericUpDown1.Text) < 0)
                    {
                        this.IntervalnumericUpDown1.Text = m_waveInterval[0].ToString();
                        return;
                    }
                    if (int.Parse(this.IntervalnumericUpDown1.Text) > int.Parse(this.IntervalnumericUpDown2.Text))
                    {
                        string temp = this.IntervalnumericUpDown1.Text;
                        this.IntervalnumericUpDown1.Text = this.IntervalnumericUpDown2.Text;
                        this.IntervalnumericUpDown2.Text = temp;
                    }
                    m_waveInterval[0] = byte.Parse(this.IntervalnumericUpDown1.Text);
                    m_waveInterval[1] = byte.Parse(this.IntervalnumericUpDown2.Text);
                    m_IntervalValueChanged = true;
                    this.WeightWavepictureBox.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WaveCaptureForm中函数IntervalnumericUpDown1_ValueChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("WaveCaptureForm中函数IntervalnumericUpDown1_ValueChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 区间控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntervalnumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_IntervalValueChanged2)
                {
                    if (int.Parse(this.IntervalnumericUpDown2.Text) > 255 || int.Parse(this.IntervalnumericUpDown2.Text) < 0)
                    {
                        this.IntervalnumericUpDown2.Text = m_waveInterval[1].ToString();
                        return;
                    }
                    if (int.Parse(this.IntervalnumericUpDown1.Text) > int.Parse(this.IntervalnumericUpDown2.Text))
                    {
                        string temp = this.IntervalnumericUpDown1.Text;
                        this.IntervalnumericUpDown1.Text = this.IntervalnumericUpDown2.Text;
                        this.IntervalnumericUpDown2.Text = temp;
                    }
                    m_waveInterval[0] = byte.Parse(this.IntervalnumericUpDown1.Text);
                    m_waveInterval[1] = byte.Parse(this.IntervalnumericUpDown2.Text);
                    m_IntervalValueChanged = true;
                    this.WeightWavepictureBox.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WaveCaptureForm中函数IntervalnumericUpDown2_ValueChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("WaveCaptureForm中函数IntervalnumericUpDown2_ValueChanged出错" + ex);
#endif
            }
        }

        private void IntervalnumericUpDown1_Enter(object sender, EventArgs e)
        {
            m_IntervalValueChanged1 = true;
        }

        private void IntervalnumericUpDown2_Enter(object sender, EventArgs e)
        {
            m_IntervalValueChanged2 = true;
        }
    }
}
