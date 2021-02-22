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
using System.Runtime.InteropServices;

namespace FruitSortingVtest1._0
{
    public partial class QualityParamSetForm : Form
    {
        byte[] m_RotGradeName = new byte[ConstPreDefine.MAX_ROT_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        uint[] m_RotFactor = new uint[ConstPreDefine.MAX_ROT_GRADE_NUM * 2];
        int m_RotCurrentIPM_ID = 0;
        List<int> m_RotChanelIDList = new List<int>();//所有通道ID
        int m_CurrentRotChannelIndex = -1;
        //stImageInfo m_RotimageInfo = new stImageInfo(GlobalDataInterface.gIPMImageDataLength);
        private Control[] RotSetEditors;//腐烂列表点击显示控件
        Image m_RotImage;
        int m_rotHeight, m_rotWidth;
        byte[] m_rotimageRGB;

        /// <summary>
        /// 初始化
        /// </summary>
        private void RotSetInitial()
        {
            try
            {
                int num = 0;
                //统计每个子系统的通道数
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    {
                        //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                        if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                        {
                            if (GlobalDataInterface.nVer == 0) //Modify by xcw - 20200619
                            {
                                m_RotChanelIDList.Add(Commonfunction.EncodeChannel(i, j, j));
                            }
                            else if (GlobalDataInterface.nVer == 1)
                            {
                                m_RotChanelIDList.Add(Commonfunction.EncodeChannel(i, j / 2, j % 2));
                            }
                            num++;
                            this.RotChannelcontextMenuStrip.Items.Add(m_resourceManager.GetString("Lanelabel.Text") + string.Format(" {0}", num));
                            this.RotChannelcontextMenuStrip.Items[num - 1].Click += new EventHandler(RotChannelcontextMenuStrip_Click);
                        }
                    }
                }

                if (GlobalDataInterface.RotGradeNum == 0)
                    GlobalDataInterface.RotGradeNum = ConstPreDefine.MAX_ROT_GRADE_NUM;

                this.RotcomboBox.SelectedIndex = GlobalDataInterface.RotGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.stRotGradeName.CopyTo(m_RotGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.unRotFactor.CopyTo(m_RotFactor, 0);
                SetRotListView();
                RotSetEditors = new Control[] { this.RotNametextBox, this.RotAreanumericUpDown, this.RotNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RotSetFormt中函数RotSetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RotSetFormt中函数RotSetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置腐烂列表参数
        /// </summary>
        private void SetRotListView()
        {
            try
            {
                this.RotlistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.RotcomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_RotGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.RotcomboBox.SelectedIndex)
                    {
                        m_RotFactor[2 * i] = 0;
                        m_RotFactor[2 * i + 1] = 0;
                    }
                    item.SubItems.Add(m_RotFactor[2 * i].ToString());
                    item.SubItems.Add(m_RotFactor[2 * i + 1].ToString());
                    this.RotlistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RotSetFormt中函数SetRotListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RotSetFormt中函数SetRotListView出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 腐烂数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotcomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetRotListView();
        }

        /// <summary>
        /// 腐烂列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotlistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.RotlistViewEx.Items.Count - 1) && (e.SubItem == 1 || e.SubItem == 2)))
                    this.RotlistViewEx.StartEditing(RotSetEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RotSetFormt中函数RotlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RotSetFormt中函数RotlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 腐烂列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotlistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_RotGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText);
                        Array.Copy(temp, 0, m_RotGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_RotFactor[e.Item.Index * 2] = uint.Parse(e.DisplayText);
                        break;
                    case 2:
                        m_RotFactor[e.Item.Index * 2 + 1] = uint.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RotSetFormt中函数RotlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RotSetFormt中函数RotlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 获取图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotGetImagebutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.RotChannelcontextMenuStrip.Items.Count > 0)
                {
                    Point point = new Point();

                    point.X = Cursor.Position.X - this.Location.X + RotGetImagebutton.Location.X;
                    point.Y = 0;
                    this.RotChannelcontextMenuStrip.Show(RotGetImagebutton, point);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RotSetFormt中函数RotGetImagebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RotSetFormt中函数RotGetImagebutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 通道快捷菜单单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotChannelcontextMenuStrip_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                m_CurrentRotChannelIndex = this.RotChannelcontextMenuStrip.Items.IndexOf(menuItem);
                //m_RotCurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]), Commonfunction.GetIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]));
                if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                {
                    //m_RotCurrentIPM_ID = Commonfunction.EncodeIPMChannel(Commonfunction.GetSubsysIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]), Commonfunction.GetIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]));
                    m_RotCurrentIPM_ID= (m_RotChanelIDList[m_CurrentRotChannelIndex]);
                }

                else if (GlobalDataInterface.nVer == 0)
                {
                    m_RotCurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]), Commonfunction.GetIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]));
                }
                stCameraNum cameraNum = new stCameraNum(true);
               // cameraNum.bChannelIndex = (byte)Commonfunction.ChanelInIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]);
                //switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo)
                //{
                //    case 1:
                //        cameraNum.cCameraNum = 0;
                //        break;
                //    case 2:
                //        cameraNum.cCameraNum = 0;
                //        break;
                //    case 4:
                //        cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                //        break;
                //    case 8:
                //        cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                //        break;
                //}
                switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo) //Modify by ChengSk - 20190520
                {
                    case 64:  //NIR2-右
                        cameraNum.cCameraNum = 0;
                        break;
                    case 128: //NIR2-中
                        cameraNum.cCameraNum = 0;
                        break;
                    case 256: //NIR2-左
                        cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                        break;
                    case 1:   //彩色-右
                        cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                        break;
                }
                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(m_RotCurrentIPM_ID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                    GlobalDataInterface.TransmitParam(m_RotCurrentIPM_ID, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SINGLE_SAMPLE_SPOT, cameraNum);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RotSetFormt中函数RotChannelcontextMenuStrip_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RotSetFormt中函数RotChannelcontextMenuStrip_Click出错" + ex);
#endif
            }
        }

        bool m_RotDrawImage = false;

        /// <summary>
        /// 上传图像显示刷新
        /// </summary>
        /// <param name="imageInfo"></param>
        public void OnUpRotImageData(stSpliceImageData spotImageData)
        {
            try
            {
                if (this == Form.ActiveForm)
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new GlobalDataInterface.RotImageDataEventHandler(OnUpRotImageData), spotImageData);
                    }
                    else
                    {
                        if (spotImageData.imageInfo.nRouteId != m_RotCurrentIPM_ID)
                            return;
                        byte[] imagedata;


                        //获取图片
                        //if (Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_CurrentRotChannelIndex]) == 0)
                        //{
                        //    imagedata = new byte[(3 * BackgroundLength + (spotImageData.imageInfo.nBottom[0] - spotImageData.imageInfo.nTop[0]) * 2) * spotImageData.imageInfo.width * 2];
                        //    m_rotimageRGB = new byte[(3 * BackgroundLength + (spotImageData.imageInfo.nBottom[0] - spotImageData.imageInfo.nTop[0]) * 2) * spotImageData.imageInfo.width * 4];
                        //    Array.Copy(spotImageData.imagedataC, spotImageData.imageInfo.nTop[0] * spotImageData.imageInfo.width * 2, imagedata, BackgroundLength * spotImageData.imageInfo.width * 2, spotImageData.imageInfo.width * (spotImageData.imageInfo.nBottom[0] - spotImageData.imageInfo.nTop[0]) * 2);
                        //    if (spotImageData.imagedataIR.Length > 0)
                        //    {
                        //        Array.Copy(spotImageData.imagedataIR, spotImageData.spotImageInfo.nTop[0] * spotImageData.spotImageInfo.width * 2, imagedata, (2 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0])) * spotImageData.spotImageInfo.width * 2, spotImageData.spotImageInfo.width * (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2);
                        //    }
                        //    Commonfunction.YUV422ChangeToRGB(imagedata, ref m_rotimageRGB, spotImageData.spotImageInfo.width, 3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2);
                        //    this.RotAreatextBox.Text = spotImageData.spotImageInfo.unFlawArea[0].ToString();
                        //    this.RotNumtextBox.Text = spotImageData.spotImageInfo.unFlawNum[0].ToString();
                        //    m_rotHeight = 3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2;
                        //}
                        //else
                        //{
                        //    imagedata = new byte[(3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2) * spotImageData.spotImageInfo.width * 2];
                        //    m_rotimageRGB = new byte[(3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2) * spotImageData.spotImageInfo.width * 4];
                        //    if (spotImageData.imagedataIR.Length > 0)
                        //    {
                        //        Array.Copy(spotImageData.imagedataC, spotImageData.spotImageInfo.nTop[1] * spotImageData.spotImageInfo.width * 2, imagedata, BackgroundLength * spotImageData.spotImageInfo.width * 2, spotImageData.spotImageInfo.width * (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2);
                        //    }
                        //    Array.Copy(spotImageData.imagedataIR, spotImageData.spotImageInfo.nTop[1] * spotImageData.spotImageInfo.width * 2, imagedata, (2 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1])) * spotImageData.spotImageInfo.width * 2, spotImageData.spotImageInfo.width * (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2);
                        //    Commonfunction.YUV422ChangeToRGB(imagedata, ref m_rotimageRGB, spotImageData.spotImageInfo.width, 3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2);
                        //    this.RotAreatextBox.Text = spotImageData.spotImageInfo.unFlawArea[1].ToString();
                        //    this.RotNumtextBox.Text = spotImageData.spotImageInfo.unFlawNum[1].ToString();
                        //    m_rotHeight = 3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2;
                        //}
                        imagedata = new byte[(3 * BackgroundLength + spotImageData.imageInfo.height * 2) * spotImageData.imageInfo.width * 2];
                        m_rotimageRGB = new byte[(3 * BackgroundLength + spotImageData.imageInfo.height * 2) * spotImageData.imageInfo.width * 4];
                        Array.Copy(spotImageData.imagedataC, 0, imagedata, BackgroundLength * spotImageData.imageInfo.width * 2, spotImageData.imageInfo.width * spotImageData.imageInfo.height * 2);
                        if (spotImageData.imagedataC.Length > 0)
                        {
                            Array.Copy(spotImageData.imagedataC, 0, imagedata, (2 * BackgroundLength + spotImageData.imageInfo.height) * spotImageData.imageInfo.width * 2, spotImageData.imageInfo.width * spotImageData.imageInfo.height * 2);
                        }
                        Commonfunction.YUV422ChangeToRGB(imagedata, ref m_rotimageRGB, spotImageData.imageInfo.width, 3 * BackgroundLength + spotImageData.imageInfo.height * 2);
                        this.RotAreatextBox.Text = spotImageData.imageInfo.unFlawArea.ToString();
                        this.RotNumtextBox.Text = spotImageData.imageInfo.unFlawNum.ToString();
                        m_rotHeight = 3 * BackgroundLength + spotImageData.imageInfo.height * 2;
                        m_rotWidth = spotImageData.imageInfo.width;

                        m_RotDrawImage = true;
                        this.RotpictureBox.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RotSetFormt中函数OnUpRotImageInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RotSetFormt中函数OnUpRotImageInfo出错" + ex);
#endif
            }
        }

        private void RotpictureBox_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics graphics = e.Graphics;//创建画板
                GCHandle handle;
                IntPtr scan;

                if (m_RotDrawImage)
                {
                    int stride = m_rotWidth * 4;
                    float radio = (float)this.RotpictureBox.Width / m_rotWidth;

                    //画图片
                    handle = GCHandle.Alloc(m_rotimageRGB, GCHandleType.Pinned);
                    scan = handle.AddrOfPinnedObject();
                    m_RotImage = new Bitmap(m_rotWidth, m_rotHeight, stride, System.Drawing.Imaging.PixelFormat.Format32bppRgb, scan);

                    graphics.DrawImage(m_RotImage, 0, 0, this.RotpictureBox.Width, radio * m_rotHeight);
                    handle.Free();

                    m_RotDrawImage = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RotSetFormt中函数RotpictureBox_Paint出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RotSetFormt中函数RotpictureBox_Paint出错" + ex);
#endif
            }
        }

        private void Rotpanel_Scroll(object sender, ScrollEventArgs e)
        {
            if (m_RotImage != null)
            {
                m_RotDrawImage = true;
                this.RotpictureBox.Invalidate();
            }
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotSaveImagebutton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "JPG格式(*.jpg)|*.jpg|位图(*.bmp)|*.bmp|GIF格式(*.gif)|*.gif|PNG格式(*.png)|*.png";
                // dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    m_RotImage.Save(dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RotSetFormt中函数RotSaveImagebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RotSetFormt中函数RotSaveImagebutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool RotSetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.RotlistViewEx.Items.Count; i++)
                {
                    if (this.RotlistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("腐烂等级名称不能为空！");
                        //MessageBox.Show("0x30001030 The rot name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x30001030 " + LanguageContainer.RotSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.RotSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.RotlistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.RotlistViewEx.Items[j].SubItems[0].Text, this.RotlistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("腐烂等级名称不能重名！");
                            //MessageBox.Show("0x30001031 The rot names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x30001031 " + LanguageContainer.RotSetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.RotSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.RotlistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.RotlistViewEx.Items[j].SubItems[1].Text))
                        {
                            //MessageBox.Show(string.Format("腐烂等级第{0}行的腐烂面积应大于第{1}行的腐烂面积", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s rot area should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.RotSetFormMessagebox6Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.RotSetFormMessagebox6Sub2Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.RotSetFormMessagebox6Sub3Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1, j + 1),
                                LanguageContainer.RotSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.RotlistViewEx.Items[i].SubItems[2].Text) < float.Parse(this.RotlistViewEx.Items[j].SubItems[2].Text))
                        {
                            //MessageBox.Show(string.Format("腐烂等级第{0}行的腐烂个数应大于第{1}行的腐烂个数", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s rot number should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.RotSetFormMessagebox6Sub4Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.RotSetFormMessagebox6Sub5Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.RotSetFormMessagebox6Sub6Text[GlobalDataInterface.selectLanguageIndex], 
                                i + 1, j + 1),
                                LanguageContainer.RotSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                GlobalDataInterface.RotGradeNum = this.RotcomboBox.SelectedIndex + 1;
                m_RotGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.stRotGradeName, 0);
                m_RotFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.unRotFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RotSetFormt中函数RotSetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RotSetFormt中函数RotSetSaveConfig出错" + ex);
#endif
                return false;
            }
        }

    }
}
