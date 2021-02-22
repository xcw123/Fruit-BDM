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
        byte[] m_BruiseGradeName = new byte[ConstPreDefine.MAX_BRUISE_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        uint[] m_BruiseFactor = new uint[ConstPreDefine.MAX_BRUISE_GRADE_NUM * 2];
        int m_BruiseCurrentIPM_ID = 0;
        List<int> m_BruiseChanelIDList = new List<int>();//所有通道ID
        int m_CurrentBruiseChannelIndex = -1;
        //stImageInfo m_bruiseimageInfo;
        private Control[] BruiseSetEditors;//擦伤列表点击显示控件
        Image m_BruiseImage;
        int m_bruiseHeight, m_bruiseWidth;
        byte[] m_bruiseimageRGB;

        /// <summary>
        /// 初始化
        /// </summary>
        private void BruiseSetInitial()
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
                        if (j < GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i])  //Modify  by ChengSk - 20190521
                        {
                            if (GlobalDataInterface.nVer == 0) //Modify by xcw - 20200619
                            {
                                m_BruiseChanelIDList.Add(Commonfunction.EncodeChannel(i, j, j));
                            }
                            else if (GlobalDataInterface.nVer == 1)
                            {
                                m_BruiseChanelIDList.Add(Commonfunction.EncodeChannel(i, j / 2, j % 2));
                            }
                            num++;
                            this.BruiseChannelcontextMenuStrip.Items.Add(m_resourceManager.GetString("Lanelabel.Text") + string.Format(" {0}", num));
                            this.BruiseChannelcontextMenuStrip.Items[num - 1].Click += new EventHandler(BruiseChannelcontextMenuStrip_Click);
                        }
                    }
                }

                if (GlobalDataInterface.BruiseGradeNum == 0)
                    GlobalDataInterface.BruiseGradeNum = ConstPreDefine.MAX_BRUISE_GRADE_NUM;

                this.BruisecomboBox.SelectedIndex = GlobalDataInterface.BruiseGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.stBruiseGradeName.CopyTo(m_BruiseGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.unBruiseFactor.CopyTo(m_BruiseFactor, 0);
                SetBruiseListView();
                BruiseSetEditors = new Control[] { this.BruiseNametextBox, this.BruiseAreanumericUpDown, this.BruiseNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BruiseSetFormt中函数BruiseSetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BruiseSetFormt中函数BruiseSetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置擦伤列表参数
        /// </summary>
        private void SetBruiseListView()
        {
            try
            {
                this.BruiselistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.BruisecomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_BruiseGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.BruisecomboBox.SelectedIndex)
                    {
                        m_BruiseFactor[2 * i] = 0;
                        m_BruiseFactor[2 * i + 1] = 0;
                    }
                    item.SubItems.Add(m_BruiseFactor[2 * i].ToString());
                    item.SubItems.Add(m_BruiseFactor[2 * i + 1].ToString());
                    this.BruiselistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BruiseSetFormt中函数SetBruiseListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BruiseSetFormt中函数SetBruiseListView出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 擦伤数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BruisecomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetBruiseListView();
        }

        /// <summary>
        /// 擦伤列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BruiselistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.BruiselistViewEx.Items.Count - 1) && (e.SubItem == 1 || e.SubItem == 2)))
                    this.BruiselistViewEx.StartEditing(BruiseSetEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BruiseSetFormt中函数BruiselistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BruiseSetFormt中函数BruiselistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 擦伤列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BruiselistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_BruiseGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText);
                        Array.Copy(temp, 0, m_BruiseGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_BruiseFactor[e.Item.Index * 2] = uint.Parse(e.DisplayText);
                        break;
                    case 2:
                        m_BruiseFactor[e.Item.Index * 2 + 1] = uint.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BruiseSetFormt中函数BruiselistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BruiseSetFormt中函数BruiselistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BruiseGetImagebutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.BruiseChannelcontextMenuStrip.Items.Count > 0)
                {
                    Point point = new Point();

                    point.X = Cursor.Position.X - this.Location.X + BruiseGetImagebutton.Location.X;
                    point.Y = 0;
                    this.BruiseChannelcontextMenuStrip.Show(BruiseGetImagebutton, point);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BruiseSetFormt中函数BruiseGetImagebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BruiseSetFormt中函数BruiseGetImagebutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 通道快捷菜单单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BruiseChannelcontextMenuStrip_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                m_CurrentBruiseChannelIndex = this.BruiseChannelcontextMenuStrip.Items.IndexOf(menuItem);
                //m_BruiseCurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]), Commonfunction.GetIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]));
                if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                {
                    //m_BruiseCurrentIPM_ID = Commonfunction.EncodeIPMChannel(Commonfunction.GetSubsysIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]), Commonfunction.GetIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]));
                    m_BruiseCurrentIPM_ID = m_BruiseChanelIDList[m_CurrentBruiseChannelIndex];

                }

                else if (GlobalDataInterface.nVer == 0)
                {
                    m_BruiseCurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]), Commonfunction.GetIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]));
                }
                stCameraNum cameraNum = new stCameraNum(true);
               
                switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo) //Modify by ChengSk - 20190520
                {
                    case 64:  //NIR2-右
                        cameraNum.cCameraNum = 0;
                        break;
                    case 128: //NIR2-中
                        cameraNum.cCameraNum = 0;
                        break;
                    case 256: //NIR2-左
                        cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                        break;
                    case 1:   //彩色-右
                        cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                        break;
                }
                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(m_BruiseCurrentIPM_ID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                    GlobalDataInterface.TransmitParam(m_BruiseCurrentIPM_ID, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SINGLE_SAMPLE_SPOT, cameraNum);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BruiseSetFormt中函数BruiseChannelcontextMenuStrip_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BruiseSetFormt中函数BruiseChannelcontextMenuStrip_Click出错" + ex);
#endif
            }
        }

        bool m_BruiseDrawImage = false;

        /// <summary>
        /// 上传图像显示刷新
        /// </summary>
        /// <param name="imageInfo"></param>
        public void OnUpBruiseImageData(stSpliceImageData spotImageData)
        {
            try
            {
                if (this == Form.ActiveForm)
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new GlobalDataInterface.BruiseImageDataEventHandler(OnUpBruiseImageData), spotImageData);
                    }
                    else
                    {
                        if (spotImageData.imageInfo.nRouteId != m_BruiseCurrentIPM_ID)
                            return;
                        byte[] imagedata;


                        //获取图片
                        //if (Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_CurrentBruiseChannelIndex]) == 0)
                        //{
                        //    imagedata = new byte[(3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2) * spotImageData.spotImageInfo.width * 2];
                        //    m_bruiseimageRGB = new byte[(3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2) * spotImageData.spotImageInfo.width * 4];
                        //    Array.Copy(spotImageData.imagedataC, spotImageData.spotImageInfo.nTop[0] * spotImageData.spotImageInfo.width * 2, imagedata, BackgroundLength * spotImageData.spotImageInfo.width * 2, spotImageData.spotImageInfo.width * (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2);
                        //    if (spotImageData.imagedataIR.Length > 0)
                        //    {
                        //        Array.Copy(spotImageData.imagedataIR, spotImageData.spotImageInfo.nTop[0] * spotImageData.spotImageInfo.width * 2, imagedata, (2 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0])) * spotImageData.spotImageInfo.width * 2, spotImageData.spotImageInfo.width * (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2);

                        //    } 
                        //    Commonfunction.YUV422ChangeToRGB(imagedata, ref m_bruiseimageRGB, spotImageData.spotImageInfo.width, 3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2);
                        //    this.BruiseAreatextBox.Text = spotImageData.spotImageInfo.unFlawArea[0].ToString();
                        //    this.BruiseNumtextBox.Text = spotImageData.spotImageInfo.unFlawNum[0].ToString();
                        //    m_bruiseHeight = 3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2;
                        //}
                        //else
                        //{
                        //    imagedata = new byte[(3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2) * spotImageData.spotImageInfo.width * 2];
                        //    m_bruiseimageRGB = new byte[(3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2) * spotImageData.spotImageInfo.width * 4];
                        //    Array.Copy(spotImageData.imagedataC, spotImageData.spotImageInfo.nTop[1] * spotImageData.spotImageInfo.width * 2, imagedata, BackgroundLength * spotImageData.spotImageInfo.width * 2, spotImageData.spotImageInfo.width * (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2);
                        //    if (spotImageData.imagedataIR.Length > 0)
                        //    {
                        //        Array.Copy(spotImageData.imagedataIR, spotImageData.spotImageInfo.nTop[1] * spotImageData.spotImageInfo.width * 2, imagedata, (2 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1])) * spotImageData.spotImageInfo.width * 2, spotImageData.spotImageInfo.width * (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2);
                        //    }
                        //    Commonfunction.YUV422ChangeToRGB(imagedata, ref m_bruiseimageRGB, spotImageData.spotImageInfo.width, 3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2);
                        //    this.BruiseAreatextBox.Text = spotImageData.spotImageInfo.unFlawArea[1].ToString();
                        //    this.BruiseNumtextBox.Text = spotImageData.spotImageInfo.unFlawNum[1].ToString();
                        //    m_bruiseHeight = 3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2;
                        //}
                        imagedata = new byte[(3 * BackgroundLength + spotImageData.imageInfo.height * 2) * spotImageData.imageInfo.width * 2];
                        m_bruiseimageRGB = new byte[(3 * BackgroundLength + spotImageData.imageInfo.height * 2) * spotImageData.imageInfo.width * 4];
                        Array.Copy(spotImageData.imagedataC, 0, imagedata, BackgroundLength * spotImageData.imageInfo.width * 2, spotImageData.imageInfo.width * spotImageData.imageInfo.height * 2);
                        if (spotImageData.imagedataC.Length > 0)
                        {
                            Array.Copy(spotImageData.imagedataC, 0, imagedata, (2 * BackgroundLength + spotImageData.imageInfo.height) * spotImageData.imageInfo.width * 2, spotImageData.imageInfo.width * spotImageData.imageInfo.height * 2);
                        }
                        Commonfunction.YUV422ChangeToRGB(imagedata, ref m_bruiseimageRGB, spotImageData.imageInfo.width, 3 * BackgroundLength + spotImageData.imageInfo.height * 2);
                        this.BruiseAreatextBox.Text = spotImageData.imageInfo.unFlawArea.ToString();
                        this.BruiseNumtextBox.Text = spotImageData.imageInfo.unFlawNum.ToString();
                        m_bruiseHeight = 3 * BackgroundLength + spotImageData.imageInfo.height * 2;
                        m_bruiseWidth = spotImageData.imageInfo.width;

                        m_BruiseDrawImage = true;
                        this.BruisepictureBox.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BruiseSetFormt中函数OnUpBruiseImageInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BruiseSetFormt中函数OnUpBruiseImageInfo出错" + ex);
#endif
            }
        }

        private void BruisepictureBox_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics graphics = e.Graphics;//创建画板
                GCHandle handle;
                IntPtr scan;

                if (m_BruiseDrawImage)
                {
                    int stride = m_bruiseWidth * 4;
                    float radio = (float)this.BruisepictureBox.Width / m_bruiseWidth;

                    //画图片
                    handle = GCHandle.Alloc(m_bruiseimageRGB, GCHandleType.Pinned);
                    scan = handle.AddrOfPinnedObject();
                    m_BruiseImage = new Bitmap(m_bruiseWidth, m_bruiseHeight, stride, System.Drawing.Imaging.PixelFormat.Format32bppRgb, scan);

                    graphics.DrawImage(m_BruiseImage, 0, 0, this.BruisepictureBox.Width, radio * m_bruiseHeight);
                    handle.Free();

                    m_BruiseDrawImage = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BruiseSetFormt中函数BruisepictureBox_Paint出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BruiseSetFormt中函数BruisepictureBox_Paint出错" + ex);
#endif
            }
        }

        private void Bruisepanel_Scroll(object sender, ScrollEventArgs e)
        {
            if (m_BruiseImage != null)
            {
                m_BruiseDrawImage = true;
                this.BruisepictureBox.Invalidate();
            }
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BruiseSaveImagebutton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "JPG格式(*.jpg)|*.jpg|位图(*.bmp)|*.bmp|GIF格式(*.gif)|*.gif|PNG格式(*.png)|*.png";
                // dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    m_BruiseImage.Save(dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BruiseSetFormt中函数BruiseSaveImagebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BruiseSetFormt中函数BruiseSaveImagebutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool BruiseSetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.BruiselistViewEx.Items.Count; i++)
                {
                    if (this.BruiselistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("擦伤等级名称不能为空！");
                        //MessageBox.Show("0x3000102E The bruise name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x3000102E " + LanguageContainer.BruiseSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.BruiseSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.BruiselistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.BruiselistViewEx.Items[j].SubItems[0].Text, this.BruiselistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("擦伤等级名称不能重名！");
                            //MessageBox.Show("0x3000102F The bruise names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3000102F " + LanguageContainer.BruiseSetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex], 
                            LanguageContainer.BruiseSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.BruiselistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.BruiselistViewEx.Items[j].SubItems[1].Text))
                        {
                            //MessageBox.Show(string.Format("擦伤等级第{0}行的擦伤面积应大于第{1}行的擦伤面积", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s bruise area should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.BruiseSetFormMessagebox6Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.BruiseSetFormMessagebox6Sub2Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.BruiseSetFormMessagebox6Sub3Text[GlobalDataInterface.selectLanguageIndex], i + 1, j + 1),
                                LanguageContainer.BruiseSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.BruiselistViewEx.Items[i].SubItems[2].Text) < float.Parse(this.BruiselistViewEx.Items[j].SubItems[2].Text))
                        {
                            //MessageBox.Show(string.Format("擦伤等级第{0}行的擦伤个数应大于第{1}行的擦伤个数", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s bruise number should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.BruiseSetFormMessagebox6Sub4Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.BruiseSetFormMessagebox6Sub5Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.BruiseSetFormMessagebox6Sub6Text[GlobalDataInterface.selectLanguageIndex], i + 1, j + 1),
                                LanguageContainer.BruiseSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                GlobalDataInterface.BruiseGradeNum = this.BruisecomboBox.SelectedIndex + 1;
                m_BruiseGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.stBruiseGradeName, 0);
                m_BruiseFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.unBruiseFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BruiseSetFormt中函数BruiseSetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BruiseSetFormt中函数BruiseSetSaveConfig出错" + ex);
#endif
                return false;
            }
        }

    }
}
