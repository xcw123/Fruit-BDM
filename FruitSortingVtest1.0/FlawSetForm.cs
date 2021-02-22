using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Common;
using Interface;
using System.Drawing.Imaging;
using System.ComponentModel;  
using System.Data;
using System.Threading;

namespace FruitSortingVtest1._0
{
    public partial class QualityParamSetForm : Form
    {
        byte[] m_FlawGradeName = new byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        uint[] m_FlawFactor = new uint[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM * 2];
        int m_FlawCurrentIPM_ID = 0;
        List<int> m_FlawChanelIDList = new List<int>();//所有通道ID
        int m_CurrentFlawChannelIndex = -1;
       // stImageInfo m_flawimageInfo = new stImageInfo(true);
        private Control[] FlawSetEditors;//瑕疵列表点击显示控件
        Image m_FlawImage;
        int m_height,m_width;
        byte[] m_flawimageRGB;

        byte m_cameraIndex = 0;

        /// <summary>
        /// 初始化
        /// </summary>
        private void FlawSetInitial()
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
                            if (GlobalDataInterface.nVer == 0)//Modify by xcw - 20200619
                            {
                                m_FlawChanelIDList.Add(Commonfunction.EncodeChannel(i, j, j));
                            }
                            else if (GlobalDataInterface.nVer == 1)
                            {
                                m_FlawChanelIDList.Add(Commonfunction.EncodeChannel(i, j / 2, j % 2));
                            }
                            num++;
                            this.FlawChannelcontextMenuStrip.Items.Add(m_resourceManager.GetString("Lanelabel.Text") + string.Format(" {0}", num));
                            //this.FlawChannelcontextMenuStrip.Items[num - 1].Click += new EventHandler(FlawChannelcontextMenuStrip_Click); //modify by xcw 20200901

                            switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo)
                            {
                                //case ConstPreDefine.RM_M:
                                case ConstPreDefine.RM2_FM:
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    break;
                                //case ConstPreDefine.RM_LMR:
                                case ConstPreDefine.RM2_FLMR:
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    break;
                                //case ConstPreDefine.RM_F_M:
                                case ConstPreDefine.RM2_MM_FM:
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[1].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    break;
                                //case ConstPreDefine.RM_LFR_LMR:
                                case ConstPreDefine.RM2_MLMR_FLMR:
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[1].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    break;
                                //case ConstPreDefine.RM_F_M_B:
                                case ConstPreDefine.RM2_BM_MM_FM:
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[1].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-B Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[2].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    break;
                                //case ConstPreDefine.RM_LFR_LMR_LBR:
                                case ConstPreDefine.RM2_BLMR_MLMR_FLMR:
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[1].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-B Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[2].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    break;
                                //case ConstPreDefine.RM_LR:
                                case ConstPreDefine.RM2_FLR:
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("Color Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    break;
                                //case ConstPreDefine.RM_F:
                                case ConstPreDefine.RM2_MM:
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    break;
                                //case ConstPreDefine.RM_LFR:
                                case ConstPreDefine.RM2_MLMR:
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    break;
                                //case ConstPreDefine.RM_F_B:
                                case ConstPreDefine.RM2_BM_MM:
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-B Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[1].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    break;
                                //case ConstPreDefine.RM_LFR_LBR:
                                case ConstPreDefine.RM2_BLMR_MLMR:
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-F Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[0].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems.Add("NIR-B Camera");
                                    ((ToolStripDropDownItem)this.FlawChannelcontextMenuStrip.Items[num - 1]).DropDownItems[1].Click += new EventHandler(FlawChannelcontextMenuStrip_Click);
                                    break;
                                default: break;
                            }
                        }
                    }
                }

                if (GlobalDataInterface.FlawGradeNum == 0)
                    GlobalDataInterface.FlawGradeNum = ConstPreDefine.MAX_FlAWAREA_GRADE_NUM;

                this.FlawcomboBox.SelectedIndex = GlobalDataInterface.FlawGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.stFlawareaGradeName.CopyTo(m_FlawGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.unFlawAreaFactor.CopyTo(m_FlawFactor, 0);
                SetFlawListView();
                FlawSetEditors = new Control[] { this.FlawNametextBox, this.FlawAreanumericUpDown, this.FlawNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-FlawSeFormt中函数FlawSetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-FlawSeFormt中函数FlawSetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置瑕疵列表参数
        /// </summary>
        private void SetFlawListView()
        {
            try
            {
                this.FlawlistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.FlawcomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_FlawGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.FlawcomboBox.SelectedIndex)
                    {
                        m_FlawFactor[2 * i] = 0;
                        m_FlawFactor[2 * i + 1] = 0;
                    }
                    item.SubItems.Add(m_FlawFactor[2 * i].ToString());
                    item.SubItems.Add(m_FlawFactor[2 * i + 1].ToString());
                    this.FlawlistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-FlawSeFormt中函数SetFlawListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-FlawSeFormt中函数SetFlawListView出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 瑕疵数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlawcomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetFlawListView();
        }

        /// <summary>
        /// 瑕疵列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlawlistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.FlawlistViewEx.Items.Count - 1) && (e.SubItem == 1 || e.SubItem == 2)))
                    this.FlawlistViewEx.StartEditing(FlawSetEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-FlawSeFormt中函数FlawlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-FlawSeFormt中函数FlawlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 瑕疵列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlawlistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_FlawGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText.Trim()); //去掉后缀空字符串 Modify by ChengSk - 20190118
                        Array.Copy(temp, 0, m_FlawGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_FlawFactor[e.Item.Index * 2] = uint.Parse(e.DisplayText);
                        break;
                    case 2:
                        m_FlawFactor[e.Item.Index * 2 + 1] = uint.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-FlawSeFormt中函数FlawlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-FlawSeFormt中函数FlawlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlawGetImagebutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.FlawChannelcontextMenuStrip.Items.Count > 0)
                {
                    Point point = new Point();

                    point.X = Cursor.Position.X - this.Location.X + GetImagebutton.Location.X - 25; //横坐标-25 Modify by ChengSk - 20190830
                    point.Y = 0 - 40; //纵坐标-40 Modify by ChengSk - 20190830
                    this.FlawChannelcontextMenuStrip.Show(GetImagebutton, point);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-FlawSeFormt中函数FlawGetImagebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-FlawSeFormt中函数FlawGetImagebutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 通道快捷菜单单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlawChannelcontextMenuStrip_Click(object sender, EventArgs e)
        {
            try
            {
                stCameraNum cameraNum = new stCameraNum(true);
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

                switch (menuItem.Text)
                {
                    case "Color Camera":
                        cameraNum.cCameraNum = 0;
                        m_cameraIndex = 0;
                        break;
                    case "NIR-F Camera":
                        cameraNum.cCameraNum = 1;
                        m_cameraIndex = 1;
                        break;
                    case "NIR-B Camera":
                        cameraNum.cCameraNum = 2;
                        m_cameraIndex = 2;
                        break;
                    default: break;
                }
                //string chanel = menuItem.OwnerItem.Text;
                m_CurrentFlawChannelIndex = this.FlawChannelcontextMenuStrip.Items.IndexOf(menuItem.OwnerItem);
                //m_CurrentFlawChannelIndex = this.FlawChannelcontextMenuStrip.Items.IndexOf(menuItem);
                
                if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                {
                    m_FlawCurrentIPM_ID=(m_FlawChanelIDList[m_CurrentFlawChannelIndex]);
                }

                else if (GlobalDataInterface.nVer == 0)
                {
                    m_FlawCurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_FlawChanelIDList[m_CurrentFlawChannelIndex]), Commonfunction.GetIPMIndex(m_FlawChanelIDList[m_CurrentFlawChannelIndex]));
                }
                // stCameraNum cameraNum = new stCameraNum(true);
                //// cameraNum.bChannelIndex = (byte)Commonfunction.ChanelInIPMIndex(m_FlawChanelIDList[m_CurrentFlawChannelIndex]);
                // switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo)
                // {
                //     case 1:
                //         cameraNum.cCameraNum = 0;
                //         break;
                //     case 2:
                //         cameraNum.cCameraNum = 0;
                //         break;
                //     case 4:
                //         cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_FlawChanelIDList[m_CurrentFlawChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                //         break;
                //     case 8:
                //         cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_FlawChanelIDList[m_CurrentFlawChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                //         break;
                // }
                if (GlobalDataInterface.global_IsTestMode)
                {
                    m_CurrentIPM_ID = m_FlawCurrentIPM_ID;
                    //GlobalDataInterface.TransmitParam(m_FlawCurrentIPM_ID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                    GlobalDataInterface.TransmitParam(m_CurrentIPM_ID, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SINGLE_SAMPLE, null); //新增 Add by ChengSk - 20190508
                    Thread.Sleep(500);//add by xcw 20200909
                    GlobalDataInterface.TransmitParam(m_FlawCurrentIPM_ID, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SINGLE_SAMPLE_SPOT, null);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-FlawSeFormt中函数FlawChannelcontextMenuStrip_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-FlawSeFormt中函数FlawChannelcontextMenuStrip_Click出错" + ex);
#endif
            }
        }

        private void FlawChannelcontextMenuStrip_MouseEnter(object sender, EventArgs e) //新增采集IPM原图时使用 Add by ChengSk - 20190508
        {
            try
            {

                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                m_CurrentChannelIndex = this.FlawChannelcontextMenuStrip.Items.IndexOf(menuItem);
                if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                {
                    m_CurrentIPM_ID = (m_FlawChanelIDList[m_CurrentFlawChannelIndex]);
                    //m_CurrentIPM_ID = Commonfunction.EncodeIPMChannel(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));
                }
                else if (GlobalDataInterface.nVer == 0)
                {
                    m_CurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));
                }
                //m_CurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_FlawChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_FlawChanelIDList[m_CurrentChannelIndex]));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-FlawSeFormt中函数FlawChannelcontextMenuStrip_MouseEnter出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-FlawSeFormt中函数FlawChannelcontextMenuStrip_MouseEnter出错" + ex);
#endif
            }
        }

        bool m_FlawDrawImage = false;
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
                        if (spotImageData.imageInfo.nRouteId != m_FlawCurrentIPM_ID)
                            return;
                        byte[] imagedata;

                        if(spotImageData.imageInfo.width==0 &&spotImageData.imageInfo.height==0)    //2015.9.30 ivycc
                            return;
                        //获取图片
                        //if (Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_CurrentFlawChannelIndex]) == 0)
                        //{
                        //    imagedata = new byte[(3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2) * spotImageData.spotImageInfo.width * 2];
                        //    m_flawimageRGB = new byte[(3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2) * spotImageData.spotImageInfo.width * 4];
                        //    Array.Copy(spotImageData.imagedataC, spotImageData.spotImageInfo.nTop[0] * spotImageData.spotImageInfo.width * 2, imagedata, BackgroundLength * spotImageData.spotImageInfo.width * 2, spotImageData.spotImageInfo.width * (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2);
                        //    if (spotImageData.imagedataIR.Length > 0)
                        //    {
                        //        Array.Copy(spotImageData.imagedataIR, spotImageData.spotImageInfo.nTop[0] * spotImageData.spotImageInfo.width * 2, imagedata, (2 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0])) * spotImageData.spotImageInfo.width * 2, spotImageData.spotImageInfo.width * (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2);
                        //    }
                        //    Commonfunction.YUV422ChangeToRGB(imagedata, ref m_flawimageRGB, spotImageData.spotImageInfo.width, 3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2);
                        //    this.FlawAreatextBox.Text = spotImageData.spotImageInfo.unFlawArea[0].ToString();
                        //    this.FlawNumtextBox.Text = spotImageData.spotImageInfo.unFlawNum[0].ToString();
                        //    m_height = 3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[0] - spotImageData.spotImageInfo.nTop[0]) * 2;
                        //}
                        //else
                        //{
                        //    imagedata = new byte[(3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2) * spotImageData.spotImageInfo.width * 2];
                        //    m_flawimageRGB = new byte[(3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2) * spotImageData.spotImageInfo.width * 4];
                        //    Array.Copy(spotImageData.imagedataC, spotImageData.spotImageInfo.nTop[1] * spotImageData.spotImageInfo.width * 2, imagedata, BackgroundLength * spotImageData.spotImageInfo.width * 2, spotImageData.spotImageInfo.width * (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2);
                        //    if (spotImageData.imagedataIR.Length > 0)
                        //    {
                        //        Array.Copy(spotImageData.imagedataIR, spotImageData.spotImageInfo.nTop[1] * spotImageData.spotImageInfo.width * 2, imagedata, (2 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1])) * spotImageData.spotImageInfo.width * 2, spotImageData.spotImageInfo.width * (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2);
                        //    }
                        //    Commonfunction.YUV422ChangeToRGB(imagedata, ref m_flawimageRGB, spotImageData.spotImageInfo.width, 3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2);
                        //    this.FlawAreatextBox.Text = spotImageData.spotImageInfo.unFlawArea[1].ToString();
                        //    this.FlawNumtextBox.Text = spotImageData.spotImageInfo.unFlawNum[1].ToString();
                        //    m_height = 3 * BackgroundLength + (spotImageData.spotImageInfo.nBottom[1] - spotImageData.spotImageInfo.nTop[1]) * 2;
                        //}
                        m_flawimageRGB = new byte[(3 * BackgroundLength + spotImageData.imageInfo.height) * spotImageData.imageInfo.width * 3];
                        imagedata = new byte[(3 * BackgroundLength + spotImageData.imageInfo.height) * spotImageData.imageInfo.width * 2];

                        Array.Copy(spotImageData.imagedataC, 0, imagedata, BackgroundLength * spotImageData.imageInfo.width * 2, spotImageData.imageInfo.width * spotImageData.imageInfo.height * 2);
                        Commonfunction.YUV422ChangeToRGB24(imagedata, ref m_flawimageRGB, spotImageData.imageInfo.width, 3 * BackgroundLength + spotImageData.imageInfo.height);
                        m_height = 3 * BackgroundLength + spotImageData.imageInfo.height;

                        //if (m_cameraIndex == 0)
                        //{
                        //    imagedata = new byte[(3 * BackgroundLength + spotImageData.imageInfo.height) * spotImageData.imageInfo.width * 2];

                        //    Array.Copy(spotImageData.imagedataC, 0, imagedata, BackgroundLength * spotImageData.imageInfo.width * 2, spotImageData.imageInfo.width * spotImageData.imageInfo.height * 2);
                        //    //if (spotImageData.imagedataIR.Length > 0)
                        //    //{
                        //    //    Array.Copy(spotImageData.imagedataIR, 0, imagedata, (2 * BackgroundLength + spotImageData.imageInfo.height) * spotImageData.imageInfo.width * 2, spotImageData.imageInfo.width * spotImageData.imageInfo.height * 2);
                        //    //}
                        //    Commonfunction.YUV422ChangeToRGB24(imagedata, ref m_flawimageRGB, spotImageData.imageInfo.width, 3 * BackgroundLength + spotImageData.imageInfo.height);
                        //    m_height = 3 * BackgroundLength + spotImageData.imageInfo.height;
                        //}
                        //else
                        //{
                        //    imagedata = new byte[(3 * BackgroundLength + spotImageData.imageInfo.height) * spotImageData.imageInfo.width];
                        //    Array.Copy(spotImageData.imagedataC, 0, imagedata, BackgroundLength * spotImageData.imageInfo.width * 2, spotImageData.imageInfo.width * spotImageData.imageInfo.height);
                        //    //if (spotImageData.imagedataIR.Length > 0)
                        //    //{
                        //    //    Array.Copy(spotImageData.imagedataIR, 0, imagedata, (2 * BackgroundLength + spotImageData.imageInfo.height) * spotImageData.imageInfo.width * 2, spotImageData.imageInfo.width * spotImageData.imageInfo.height);
                        //    //}
                        //    Commonfunction.YUV422GrayChangeToRGB24(imagedata, ref m_flawimageRGB, spotImageData.imageInfo.width, 3 * BackgroundLength + spotImageData.imageInfo.height);
                        //    m_height = 3 * BackgroundLength + spotImageData.imageInfo.height;
                        //}
                        this.FlawAreatextBox.Text = spotImageData.imageInfo.unFlawArea.ToString();
                        this.FlawNumtextBox.Text = spotImageData.imageInfo.unFlawNum.ToString();
                        
                        m_width = spotImageData.imageInfo.width;

                        // Graphics graphics = this.FlawpictureBox.CreateGraphics();//创建画板
                        //Image image;
                        m_FlawDrawImage = true;
                        this.FlawpictureBox.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-FlawSeFormt中函数OnUpimageInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-FlawSeFormt中函数OnUpimageInfo出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 上传图像显示刷新
        /// </summary>
        /// <param name="imageInfo"></param>
        public void OnUpFlawSpliceImageData(stSpliceImageData spliceImageData)
        {
            //if (GlobalDataInterface.nVer == 0)
            //{
            try
            {
                //if (this == Form.ActiveForm)
                //{
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new GlobalDataInterface.SpliceImageDataEventHandler(OnUpFlawSpliceImageData), spliceImageData);
                }
                else
                {
                    if (spliceImageData.imageInfo.nRouteId != m_CurrentIPM_ID)//add by xcw 20200909
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
          

        }


        private void FlawpictureBox_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics graphics = e.Graphics;//创建画板
                GCHandle handle;
                IntPtr scan;

                if (m_FlawDrawImage)
                {
                    Graphics graphicsSrc = e.Graphics;//创建画板
                    //Image image;
                    GCHandle handleSrc;
                    IntPtr scanSrc;
                    int width = m_spliceImageData.imageInfo.width;
                    int height;
                    int strideSrc = width * 3;//add by xcw 20200909

                    if (m_spliceImageData.imageInfo.width != 0 && m_spliceImageData.imageInfo.height != 0)
                    {
                        height = 3 * BackgroundLength + m_spliceImageData.imageInfo.height * 2;
                        //2015-8-20 ivycc 按原图大小显示
                        //if (this.ColorImagepictureBox.Width != width || this.ColorImagepictureBox.Height != height)
                        //{
                        //    this.ColorImagepictureBox.Width = width;
                        //    this.ColorImagepictureBox.Height = height;
                        //    this.ColorImagepictureBox.Invalidate();
                        //    return;
                        //}
                        handleSrc = GCHandle.Alloc(m_fruitRGBImage, GCHandleType.Pinned);
                        scanSrc = handleSrc.AddrOfPinnedObject();
                        m_FruitImage = new Bitmap(width, height, strideSrc, System.Drawing.Imaging.PixelFormat.Format24bppRgb, scanSrc);
                        //graphics.DrawImage(m_FruitImage, 0, 0, width, height);//2015-8-20 ivycc 按原图大小显示
                        handleSrc.Free();
                    }




                    //2015-8-20 ivycc 按原图大小显示
                    if (this.FlawpictureBox.Width != m_width || this.FlawpictureBox.Height != m_height)
                        {
                            this.FlawpictureBox.Width = m_width;
                            this.FlawpictureBox.Height = m_height;
                            this.FlawpictureBox.Invalidate();
                            return;
                        }

                        int stride = m_width * 3;
                        // float radio = (float)this.FlawpictureBox.Width / m_width;
                        // this.FlawpictureBox.Height = (int)(radio * m_height);

                        //画图片
                        handle = GCHandle.Alloc(m_flawimageRGB, GCHandleType.Pinned);
                        scan = handle.AddrOfPinnedObject();
                        m_FlawImage = new Bitmap(m_width, m_height, stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, scan);

                    //if (this.FlawpictureBox.Height < 299)
                    //{
                    //    this.Flawpanel.Size = new Size(this.FruitImagepanel.Width, this.FlawpictureBox.Height + 10);
                    //}
                    //else
                    //{
                    //    this.Flawpanel.Size = new Size(this.FruitImagepanel.Width, 309);
                    //}

                    //graphics.DrawImage(m_FlawImage, 0, 0, this.FlawpictureBox.Width, this.FlawpictureBox.Height);
                    //this.FlawpictureBox.Image = m_FlawImage;
                    //Graphics graphics = this.FlawpictureBox.CreateGraphics();

                    //graphics.DrawImage(m_FlawImage, 0, 0, this.FlawpictureBox.Width, radio * m_height);
                    if (m_cameraIndex == 0)
                    {
                        if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR)
                        {
                            graphics.DrawImage(m_FlawImage, 0, 0, new Rectangle(0, 0, m_width, m_height), GraphicsUnit.Pixel);//add by xcw 20200909
                        }
                        //graphics.DrawImage(m_FlawImage, 0, m_height / 2, m_width, m_height);//2015-8-20 ivycc 按原图大小显示
                        else
                        {
                            graphics.DrawImage(m_FlawImage, 0, 0, new Rectangle(0, 0, m_width, m_height / 2), GraphicsUnit.Pixel);//add by xcw 20200909
                        }
                    }
                    else
                    {
                        graphics.DrawImage(m_FlawImage, 0, 0, new Rectangle(0, m_height / 2, m_width, m_height), GraphicsUnit.Pixel);

                    }
                    handle.Free();
                    m_FlawDrawImage = false;
                }


            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-FlawSeFormt中函数FlawpictureBox_Paint出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-FlawSeFormt中函数FlawpictureBox_Paint出错" + ex);
#endif
            }
        }

        private void Flawpanel_Scroll(object sender, ScrollEventArgs e)
        {
            if (m_FlawImage != null)
            {
                m_FlawDrawImage = true;
                this.FlawpictureBox.Invalidate();
            }
        }




        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlawSaveImagebutton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "JPG格式(*.jpg)|*.jpg|位图(*.bmp)|*.bmp|GIF格式(*.gif)|*.gif|PNG格式(*.png)|*.png";
                // dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    m_FlawImage.Save(dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-FlawSeFormt中函数FlawSaveImagebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-FlawSeFormt中函数FlawSaveImagebutton_Click出错" + ex);
#endif
            }
        }

        private void FlawSaveSrcImagebutton_Click(object sender, EventArgs e) //瑕疵界面保存原图 Add by ChengSk - 20190508
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "位图(*.bmp)|*.bmp|JPG格式(*.jpg)|*.jpg|GIF格式(*.gif)|*.gif|PNG格式(*.png)|*.png";
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
                Trace.WriteLine("QualityParamSetForm-FlawSeFormt中函数FlawSaveSrcImagebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-FlawSeFormt中函数FlawSaveSrcImagebutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool FlawSetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.FlawlistViewEx.Items.Count; i++)
                {
                    if (this.FlawlistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("瑕疵等级名称不能为空！");
                        //MessageBox.Show("0x30001017 The blemish name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x30001017 " + LanguageContainer.FlawSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.FlawSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.FlawlistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.FlawlistViewEx.Items[j].SubItems[0].Text, this.FlawlistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("瑕疵等级名称不能重名！");
                            //MessageBox.Show("0x30001018 The blemish names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x30001018 " + LanguageContainer.FlawSetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.FlawSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.FlawlistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.FlawlistViewEx.Items[j].SubItems[1].Text))
                        {
                            //MessageBox.Show(string.Format("瑕疵等级第{0}行的瑕疵面积应大于第{1}行的瑕疵面积", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s blemish area should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.FlawSetFormMessagebox6Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.FlawSetFormMessagebox6Sub2Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.FlawSetFormMessagebox6Sub3Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1, j + 1),
                                LanguageContainer.FlawSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.FlawlistViewEx.Items[i].SubItems[2].Text) < float.Parse(this.FlawlistViewEx.Items[j].SubItems[2].Text))
                        {
                            //MessageBox.Show(string.Format("瑕疵等级第{0}行的瑕疵个数应大于第{1}行的瑕疵个数", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s blemish number should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.FlawSetFormMessagebox6Sub4Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.FlawSetFormMessagebox6Sub5Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.FlawSetFormMessagebox6Sub6Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1, j + 1),
                                LanguageContainer.FlawSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                GlobalDataInterface.FlawGradeNum = this.FlawcomboBox.SelectedIndex + 1;                
                m_FlawGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.stFlawareaGradeName, 0);
                m_FlawFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.unFlawAreaFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-FlawSeFormt中函数FlawSetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-FlawSeFormt中函数FlawSetSaveConfig出错" + ex);
#endif
                return false;
            }
        }


        Form ImageForm;
        /// <summary>
        /// 瑕疵图片双击放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlawpictureBox_DoubleClick(object sender, EventArgs e)
        {
            ImageForm = new Form();
            
          //  int width,height;
            ImageForm.BackColor = Color.Black;
            ImageForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            ImageForm.Height = Screen.AllScreens[0].Bounds.Height;
            ImageForm.Width = Screen.AllScreens[0].Bounds.Width;
            ImageForm.DoubleClick += new System.EventHandler(this.ImageForm_DoubleClick);
            PictureBox picBox = new PictureBox();
            picBox.DoubleClick += new System.EventHandler(this.ImageForm_DoubleClick);

            if (m_FlawImage == null)
                return;
            //if (FlawpictureBox.Height > Screen.AllScreens[0].Bounds.Height)
            //{
                picBox.Width = Screen.AllScreens[0].Bounds.Width;
               // picBox.Width = FlawpictureBox.Width * Screen.AllScreens[0].Bounds.Height / FlawpictureBox.Height;
                picBox.Height = FlawpictureBox.Height * Screen.AllScreens[0].Bounds.Width / FlawpictureBox.Width;
                picBox.SizeMode = PictureBoxSizeMode.CenterImage;
                if (picBox.Height < ImageForm.Height)
                    picBox.Location = new Point(picBox.Location.X,ImageForm.Height / 2 - picBox.Height / 2);
            //}
            //else
            //{
            //    picBox.Width = FlawpictureBox.Width;
            //    picBox.Height = FlawpictureBox.Height;
            //}
            Bitmap img = new Bitmap(m_FlawImage, picBox.Width, picBox.Height);
            ImageForm.AutoScroll = true;
           // ImageForm.MaximizeBox = false;
           // ImageForm.AutoSize = false;
            
           
            picBox.BackgroundImage = img;
           // picBox.Dock = DockStyle.Left;
            ImageForm.Controls.Add(picBox);
            ImageForm.ShowDialog();

            //IntPtr dc1 = CreateDC(Screen.AllScreens[0].DeviceName, null, null, (IntPtr)null);
            //Graphics g1 = Graphics.FromHdc(dc1);//由一个指定设备的句柄创建一个新的Graphics对象 
            //Image MyI = new Bitmap(width, height, g1);//根据屏幕大小创建一个与之相同大小的Bitmap对象 
            //Graphics g2 = Graphics.FromImage(MyI);//获得屏幕的句柄 
            //IntPtr dc3 = g1.GetHdc();//获得位图的句柄 
            //IntPtr dc2 = g2.GetHdc();//把当前屏幕捕获到位图对象中 
            //BitBlt(dc2, 0, 0, width, height, dc3, 0, 0, 13369376);//把当前屏幕拷贝到位图中 
            //g1.ReleaseHdc(dc3);//释放屏幕句柄 
            //g2.ReleaseHdc(dc2);//释放位图句柄 
            //this.BackgroundImage = MyI;
            //this.Visible = true; 
            
        }

       private void Flawpanel_DoubleClick(object sender, EventArgs e)
        {
            ImageForm = new Form();

            //  int width,height;
            ImageForm.BackColor = Color.Black;
            ImageForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            ImageForm.Height = Screen.AllScreens[0].Bounds.Height;
            ImageForm.Width = Screen.AllScreens[0].Bounds.Width;
            ImageForm.DoubleClick += new System.EventHandler(this.ImageForm_DoubleClick);
            PictureBox picBox = new PictureBox();
            picBox.DoubleClick += new System.EventHandler(this.ImageForm_DoubleClick);

            if (m_FlawImage == null)
                return;
            //if (FlawpictureBox.Height > Screen.AllScreens[0].Bounds.Height)
            //{
            picBox.Width = Screen.AllScreens[0].Bounds.Width;
            // picBox.Width = FlawpictureBox.Width * Screen.AllScreens[0].Bounds.Height / FlawpictureBox.Height;
            picBox.Height = FlawpictureBox.Height * Screen.AllScreens[0].Bounds.Width / FlawpictureBox.Width;
            picBox.SizeMode = PictureBoxSizeMode.CenterImage;
            if (picBox.Height < ImageForm.Height)
                picBox.Location = new Point(picBox.Location.X, ImageForm.Height / 2 - picBox.Height / 2);
            //}
            //else
            //{
            //    picBox.Width = FlawpictureBox.Width;
            //    picBox.Height = FlawpictureBox.Height;
            //}
            Bitmap img = new Bitmap(m_FlawImage, picBox.Width, picBox.Height);
            ImageForm.AutoScroll = true;
            // ImageForm.MaximizeBox = false;
            // ImageForm.AutoSize = false;


            picBox.BackgroundImage = img;
            // picBox.Dock = DockStyle.Left;
            ImageForm.Controls.Add(picBox);
            ImageForm.ShowDialog();
        }
        private void ImageForm_DoubleClick(object sender, EventArgs e)
        {
            this.ImageForm.Close();
        }

    }
}
