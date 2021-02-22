using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Configuration;
using ListViewEx;
using GlacialComponents.Controls;
using Interface;
using Common;
using System.Diagnostics;

namespace FruitSortingVtest1._0
{

    public partial class ProjectSetForm : Form
    {

        private Control[] SystemEditors;
        private static stSysConfig tempSysConfig = new stSysConfig(true);//临时参数
        private static stGradeInfo tempGradeInfo = new stGradeInfo(true);//临时参数
        private bool m_bAlert = false;//是否提示“注意修改等级设置-分选标准设置页面的相应内容”
        private List<ExitState> m_ExitList;

        private void SystemStructInitial()
        {

            try
            {
                //GlobalDataInterface.globalOut_SysConfig.nExitNum = 4;
                tempGradeInfo.ToCopy(GlobalDataInterface.globalOut_GradeInfo);
                tempSysConfig.ToCopy(GlobalDataInterface.globalOut_SysConfig);
                if (!GlobalDataInterface.ProjectSetFormIsIntialed)
                {
                    m_ChanelIDList.Clear();
                    m_ChanelExitList.Clear();
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                    {
                        m_ChannelNum[i] = 0;
                    }
                    if (GlobalDataInterface.nVer == 0)  //add by xcw -20200618
                    {
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                        {
                            for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                            {
                                //if (tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                                if (tempSysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                                {
                                    m_ChannelNum[i]++;
                                    m_ChanelIDList.Add(Commonfunction.EncodeChannel(i, j, j));
                                }
                                if (i == 0)
                                {
                                    m_ChanelExitList.Add(Commonfunction.EncodeChannel(i, j, j));
                                }

                            }
                        }
                    }
                    else if (GlobalDataInterface.nVer == 1)
                    {
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                        {
                            for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                            {
                                //if (tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                                if (tempSysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                                {
                                    m_ChannelNum[i]++;
                                    m_ChanelIDList.Add(Commonfunction.EncodeChannel(i, j / 2, j % 2));
                                }
                                if (i == 0)
                                {
                                    m_ChanelExitList.Add(Commonfunction.EncodeChannel(i, j / 2, j % 2));
                                }
                            }
                        }

                    }
                    //统计每个子系统的通道数
                    //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                    //{
                    //    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    //    {
                    //        //if (tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                    //        if (tempSysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                    //        {
                    //            m_ChannelNum[i]++;
                    //            m_ChanelIDList.Add(Commonfunction.EncodeChannel(i, j, j));
                    //        }
                    //    }
                    //}
                }

                ListViewItem lvi;
                //系统结构设置
                //if (tempSysConfig.nSystemInfo > 0)
                {
                    #region 旧模式系统类型
                    //switch (tempSysConfig.nSystemInfo) //中间新增类型 Modify by ChengSk - 20190226
                    //{
                    //    case ConstPreDefine.RM_M:
                    //        this.SysTypecomboBox.SelectedIndex = 0;
                    //        break;
                    //    case ConstPreDefine.RM_LMR:
                    //        this.SysTypecomboBox.SelectedIndex = 1;
                    //        break;
                    //    case ConstPreDefine.RM_F_M:
                    //        this.SysTypecomboBox.SelectedIndex = 2;
                    //        break;
                    //    case ConstPreDefine.RM_LFR_LMR:
                    //        this.SysTypecomboBox.SelectedIndex = 3;
                    //        break;
                    //    case ConstPreDefine.RM_F_M_B:
                    //        this.SysTypecomboBox.SelectedIndex = 4;
                    //        break;
                    //    case ConstPreDefine.RM_LFR_LMR_LBR:
                    //        this.SysTypecomboBox.SelectedIndex = 5;
                    //        break;
                    //    case ConstPreDefine.RM_LR:
                    //        this.SysTypecomboBox.SelectedIndex = 6;
                    //        break;
                    //    case ConstPreDefine.RM_LR_LR:
                    //        this.SysTypecomboBox.SelectedIndex = 7;
                    //        break;
                    //    case ConstPreDefine.RM_LR_LR_LR:
                    //        this.SysTypecomboBox.SelectedIndex = 8;
                    //        break;
                    //    case ConstPreDefine.RM_F:
                    //        this.SysTypecomboBox.SelectedIndex = 9;
                    //        break;
                    //    case ConstPreDefine.RM_LRF:
                    //        this.SysTypecomboBox.SelectedIndex = 10;
                    //        break;
                    //    case ConstPreDefine.RM_LFR:
                    //        this.SysTypecomboBox.SelectedIndex = 11;
                    //        break;
                    //    case ConstPreDefine.RM_F_B:
                    //        this.SysTypecomboBox.SelectedIndex = 12;
                    //        break;
                    //    case ConstPreDefine.RM_LFR_LBR:
                    //        this.SysTypecomboBox.SelectedIndex = 13;
                    //        break;
                    //    default: break;
                    //}
                    #endregion

                    #region 新模式系统类型
                    if (((tempSysConfig.nSystemInfo >> 8) & 1) == 1)
                        this.lblLampLNIR2.ForeColor = Color.Green;
                    else
                        this.lblLampLNIR2.ForeColor = Color.Red;

                    if (((tempSysConfig.nSystemInfo >> 7) & 1) == 1)
                        this.lblLampMNIR2.ForeColor = Color.Green;
                    else
                        this.lblLampMNIR2.ForeColor = Color.Red;

                    if (((tempSysConfig.nSystemInfo >> 6) & 1) == 1)
                        this.lblLampRNIR2.ForeColor = Color.Green;
                    else
                        this.lblLampRNIR2.ForeColor = Color.Red;

                    if (((tempSysConfig.nSystemInfo >> 5) & 1) == 1)
                        this.lblLampLNIR1.ForeColor = Color.Green;
                    else
                        this.lblLampLNIR1.ForeColor = Color.Red;

                    if (((tempSysConfig.nSystemInfo >> 4) & 1) == 1)
                        this.lblLampMNIR1.ForeColor = Color.Green;
                    else
                        this.lblLampMNIR1.ForeColor = Color.Red;

                    if (((tempSysConfig.nSystemInfo >> 3) & 1) == 1)
                        this.lblLampRNIR1.ForeColor = Color.Green;
                    else
                        this.lblLampRNIR1.ForeColor = Color.Red;

                    if (((tempSysConfig.nSystemInfo >> 2) & 1) == 1)
                        this.lblLampLColor.ForeColor = Color.Green;
                    else
                        this.lblLampLColor.ForeColor = Color.Red;

                    if (((tempSysConfig.nSystemInfo >> 1) & 1) == 1)
                        this.lblLampMColor.ForeColor = Color.Green;
                    else
                        this.lblLampMColor.ForeColor = Color.Red;

                    if (((tempSysConfig.nSystemInfo >> 0) & 1) == 1)
                        this.lblLampRColor.ForeColor = Color.Green;
                    else
                        this.lblLampRColor.ForeColor = Color.Red;
                    #endregion
                }

                //相机延时
                this.CameraDelaylistViewEx.Items.Clear();

                lvi = new ListViewItem("PacketDelay");
                for (int i = 0; i < ConstPreDefine.MAX_CAMERA_NUM; i++)
                {
                    lvi.SubItems.Add(tempSysConfig.nCameraDelay[2 * i].ToString());
                }
                this.CameraDelaylistViewEx.Items.Add(lvi);
                lvi = new ListViewItem("TransDelay");
                for (int i = 0; i < ConstPreDefine.MAX_CAMERA_NUM; i++)
                {
                    lvi.SubItems.Add(tempSysConfig.nCameraDelay[2 * i + 1].ToString());
                }
                this.CameraDelaylistViewEx.Items.Add(lvi);

                //子系统数
                this.SubSysNumcomboBox.SelectedIndex = tempSysConfig.nSubsysNum - 1;
                //子系统信息
                this.SubSysInfolistViewEx.Items.Clear();
                for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++)
                {
                    lvi = new ListViewItem(string.Format("{0}", i + 1));
                    lvi.SubItems.Add(string.Format("{0}", m_ChannelNum[i]));
                    lvi.SubItems.Add(string.Format("{0}", tempSysConfig.nImageUV[i]));
                    lvi.SubItems.Add(string.Format("{0}", tempSysConfig.nDataRegistration[i]));
                    lvi.SubItems.Add(string.Format("{0}", tempSysConfig.nImageSugar[i]));
                    lvi.SubItems.Add(string.Format("{0}", tempSysConfig.nImageUltrasonic[i]));
                    lvi.SubItems.Add(string.Format("{0}", tempSysConfig.nIQSEnable)); //Add by ChengSk - 20191111
                    this.SubSysInfolistViewEx.Items.Add(lvi);
                }
                this.ChannelnumericUpDown.Maximum = ConstPreDefine.MAX_CHANNEL_NUM;
                if ((tempSysConfig.nClassificationInfo & 0x02) == 2)
                    this.ImgAndUVnumericUpDown.Enabled = true;
                else
                    this.ImgAndUVnumericUpDown.Enabled = false;
                if ((tempSysConfig.nClassificationInfo & 0x04) == 4)
                    this.ImgAndWeighnumericUpDown.Enabled = true;
                else
                    this.ImgAndWeighnumericUpDown.Enabled = false;
                if ((tempSysConfig.nClassificationInfo & 0x08) == 8)
                {
                    this.ImgAndSugarnumericUpDown.Enabled = true;
                    this.IQSEnableFlagsnumericUpDown.Enabled = true;  //Add by ChengSk - 20191111
                }
                else
                {
                    this.ImgAndSugarnumericUpDown.Enabled = false;
                    this.IQSEnableFlagsnumericUpDown.Enabled = false; //Add by ChengSk - 20191111
                }
                if ((tempSysConfig.nClassificationInfo & 0x10) == 16)
                    this.ImgAndUltrasonicnumericUpDown.Enabled = true;
                else
                    this.ImgAndUltrasonicnumericUpDown.Enabled = false;
                //新增IQSEnableFlagsnumericUpDown - Add by ChengSk 20191111
                SystemEditors = new Control[] { ChannelnumericUpDown,ImgAndUVnumericUpDown,
                    ImgAndWeighnumericUpDown, ImgAndSugarnumericUpDown,ImgAndUltrasonicnumericUpDown,IQSEnableFlagsnumericUpDown };

                //传感系统类型
                //CIR视觉
                if ((tempSysConfig.nClassificationInfo & 0x01) == 1)
                {
                    GlobalDataInterface.CIRAvailable = true;
                    this.CIRVisioncheckBox.Checked = true;

                    this.ColorcheckBox.Enabled = true;
                    this.ShapecheckBox.Enabled = true;
                    this.FlawcheckBox.Enabled = true;
                    this.VolumecheckBox.Enabled = true;
                    this.ProjectedAreacheckBox.Enabled = true;

                    this.ChannelRangePage.Tag = "True";//启用通道范围设置界面
                    //this.ChannelRangePage.Show();
                }
                else
                {
                    GlobalDataInterface.CIRAvailable = false;
                    this.CIRVisioncheckBox.Checked = false;

                    this.ColorcheckBox.Enabled = false;
                    this.ShapecheckBox.Enabled = false;
                    this.FlawcheckBox.Enabled = false;
                    this.VolumecheckBox.Enabled = false;
                    this.ProjectedAreacheckBox.Enabled = false;

                    this.ChannelRangePage.Tag = "False";//不启用通道范围设置界面
                    //this.ChannelRangePage.Hide();
                }
                //UV视觉
                if ((tempSysConfig.nClassificationInfo & 0x02) == 2)
                {
                    GlobalDataInterface.UVAvailable = true;
                    this.UVVisioncheckBox.Checked = true;

                    this.BruisecheckBox.Enabled = true;
                    this.RotcheckBox.Enabled = true;
                }
                else
                {
                    GlobalDataInterface.UVAvailable = false;
                    this.UVVisioncheckBox.Checked = false;

                    this.BruisecheckBox.Enabled = false;
                    this.RotcheckBox.Enabled = false;
                }
                //重量系统
                if ((tempSysConfig.nClassificationInfo & 0x04) == 4)
                {
                    GlobalDataInterface.WeightAvailable = true;
                    this.WeighcheckBox.Checked = true;
                    this.WeightSettabPage.Tag = "True";//启用重量设置界面

                    this.DensitycheckBox.Enabled = true;
                }
                else
                {
                    GlobalDataInterface.WeightAvailable = false;
                    this.WeighcheckBox.Checked = false;
                    this.WeightSettabPage.Tag = "False";//不启用重量设置界面
                    this.DensitycheckBox.Enabled = false;
                }
                //内部品质
                if ((tempSysConfig.nClassificationInfo & 0x08) == 8)
                {
                    GlobalDataInterface.InternalAvailable = true;
                    this.InternalcheckBox.Checked = true;

                    this.SugarcheckBox.Enabled = true;
                    this.AciditycheckBox.Enabled = true;
                    this.HollowcheckBox.Enabled = true;
                    this.SkincheckBox.Enabled = true;
                    this.BrowncheckBox.Enabled = true;
                    this.TangxincheckBox.Enabled = true;
                    this.InnerQualityPage.Tag = "true"; //启用内部品质设置界面  Add by ChengSk - 20190114
                }
                else
                {
                    GlobalDataInterface.InternalAvailable = false;
                    this.InternalcheckBox.Checked = false;

                    this.SugarcheckBox.Enabled = false;
                    this.AciditycheckBox.Enabled = false;
                    this.HollowcheckBox.Enabled = false;
                    this.SkincheckBox.Enabled = false;
                    this.BrowncheckBox.Enabled = false;
                    this.TangxincheckBox.Enabled = false;
                    this.InnerQualityPage.Tag = "false";//不启用内部品质设置界面
                }
                //超声波
                if ((tempSysConfig.nClassificationInfo & 0x10) == 16)
                {
                    GlobalDataInterface.UltrasonicAvailable = true;
                    this.UltrasoniccheckBox.Checked = true;

                    this.HardnesscheckBox.Enabled = true;
                    this.WatercheckBox.Enabled = true;
                }
                else
                {
                    GlobalDataInterface.UltrasonicAvailable = false;
                    this.UltrasoniccheckBox.Checked = false;

                    this.HardnesscheckBox.Enabled = false;
                    this.WatercheckBox.Enabled = false;
                }

                //分辨率
                int maxWidth = 0;    //最大宽度
                int maxHeight = 0;   //最大高度
                byte cameraType = 0; //相机类型
                cameraType = tempSysConfig.nCameraType;
                ////switch (cameraType)
                ////{
                ////    case 0x00:
                ////        maxWidth = 1024;
                ////        maxHeight = 400;
                ////        break;
                ////    case 0x01:
                ////        maxWidth = 1280;
                ////        maxHeight = 400;
                ////        break;
                ////    case 0x02:
                ////        maxWidth = 1600;
                ////        maxHeight = 400;
                ////        break;
                ////    case 0x03:
                ////        maxWidth = 2046;
                ////        maxHeight = 400;
                ////        break;
                ////    default:
                ////        maxWidth = 1024;
                ////        maxHeight = 400;
                ////        break;
                ////}
                //switch (cameraType) //Modify by ChengSk - 20190520
                //{
                //    case 0x00:
                //        maxWidth = 1280;
                //        //maxHeight = GlobalDataInterface.nVer == 0?400:1024;
                //        maxHeight = 1024;
                //        break;
                //    default:
                //        maxWidth = 1280;
                //        //maxHeight = GlobalDataInterface.nVer == 0?400:1024;//4.2固定图像高度（3.2已修改）
                //        maxHeight = 1024;//4.2固定图像高度（3.2已修改）
                //        break;
                //}
                maxWidth = 2560;
                maxHeight = 1920;
                CameraTypecomboBox.SelectedIndex = (int)cameraType;

                //宽
                if (tempSysConfig.width > maxWidth || tempSysConfig.width == 0) tempSysConfig.width = maxWidth;
                if (tempSysConfig.width < 2) tempSysConfig.width = 2;
                this.WidthnumericUpDown.Maximum = maxWidth;
                this.WidthnumericUpDown.Text = tempSysConfig.width.ToString();
                //高
                if (tempSysConfig.height > maxHeight || tempSysConfig.height == 0) tempSysConfig.height = maxHeight;
                if (tempSysConfig.height < 2) tempSysConfig.height = 2;
                this.HeightnumericUpDown.Maximum = maxHeight;
                this.HeightnumericUpDown.Text = tempSysConfig.height.ToString();

                //包大小
                this.PacketSizenumericUpDown.Text = tempSysConfig.packetSize.ToString();

                //出口数量
                this.ExitNumcomboBox.SelectedIndex = tempSysConfig.nExitNum - 1;

                //工作台结构
                this.TableSetglacialList.Columns.Clear();
                for (int i = 0; i < (this.ExitNumcomboBox.SelectedIndex + 1) * 2; i++)
                {
                    this.TableSetglacialList.Columns.Add((i + 1).ToString(), 40);
                    this.TableSetglacialList.Columns[i].Text = (i + 1).ToString();
                    this.TableSetglacialList.Columns[i].CheckBoxes = true;
                    this.TableSetglacialList.Columns[i].TextAlignment = ContentAlignment.MiddleCenter;

                }

                for (int j = 0; j < 5; j++)
                {
                    for (int i = 0; i < (this.ExitNumcomboBox.SelectedIndex + 1) * 2; i++)
                    {
                        this.TableSetglacialList.Items[j].SubItems[i].Checked = false;
                        this.TableSetglacialList.Items[j].SubItems[i].Text = "0";
                    }
                }

                for (int i = 0; i < GlobalDataInterface.ExitList.Count; i++)
                {
                    if (GlobalDataInterface.ExitList[i].ItemIndex < 2)
                    {
                        this.TableSetglacialList.Items[GlobalDataInterface.ExitList[i].ItemIndex].SubItems[GlobalDataInterface.ExitList[i].ColumnIndex].Checked = true;
                        this.TableSetglacialList.Items[GlobalDataInterface.ExitList[i].ItemIndex].SubItems[GlobalDataInterface.ExitList[i].ColumnIndex].Text = GlobalDataInterface.ExitList[i].Index.ToString();
                    }
                    else
                    {
                        this.TableSetglacialList.Items[GlobalDataInterface.ExitList[i].ItemIndex + 1].SubItems[GlobalDataInterface.ExitList[i].ColumnIndex].Checked = true;
                        this.TableSetglacialList.Items[GlobalDataInterface.ExitList[i].ItemIndex + 1].SubItems[GlobalDataInterface.ExitList[i].ColumnIndex].Text = GlobalDataInterface.ExitList[i].Index.ToString();
                    }
                }


                m_ExitList = new List<ExitState>(GlobalDataInterface.ExitList.ToArray());//出口布局中已选为出口
                //m_ExitList以Index有小到大排序 2015-5-14
                for (int i = 0; i < m_ExitList.Count; i++)
                {
                    for (int j = i; j < m_ExitList.Count; j++)
                    {
                        if (m_ExitList[i].Index > m_ExitList[j].Index)
                        {
                            ExitState temp = m_ExitList[i];
                            m_ExitList[i] = m_ExitList[j];
                            m_ExitList[j] = temp;
                        }
                    }
                }

                //显示
                this.ColorcheckBox.Checked = GlobalDataInterface.SystemStructColor;
                this.ShapecheckBox.Checked = GlobalDataInterface.SystemStructShape;
                this.FlawcheckBox.Checked = GlobalDataInterface.SystemStructFlaw;
                this.VolumecheckBox.Checked = GlobalDataInterface.SystemStructVolume;
                this.ProjectedAreacheckBox.Checked = GlobalDataInterface.SystemStructProjectedArea;//以上为CIR视觉
                this.BruisecheckBox.Checked = GlobalDataInterface.SystemStructBruise;
                this.RotcheckBox.Checked = GlobalDataInterface.SystemStructRot;        //以上为UV视觉
                this.DensitycheckBox.Checked = GlobalDataInterface.SystemStructDensity;//以上为重量系统
                this.SugarcheckBox.Checked = GlobalDataInterface.SystemStructSugar;
                this.AciditycheckBox.Checked = GlobalDataInterface.SystemStructAcidity;
                this.HollowcheckBox.Checked = GlobalDataInterface.SystemStructHollow;
                this.SkincheckBox.Checked = GlobalDataInterface.SystemStructSkin;
                this.BrowncheckBox.Checked = GlobalDataInterface.SystemStructBrown;
                this.TangxincheckBox.Checked = GlobalDataInterface.SystemStructTangxin;//以上为内部品质
                this.HardnesscheckBox.Checked = GlobalDataInterface.SystemStructRigidity;
                this.WatercheckBox.Checked = GlobalDataInterface.SystemStructWater;    //以上为超声波
                this.WifiBroadcastcheckBox.Checked = GlobalDataInterface.sendBroadcastPackage; //以上为WIFI功能

                //倍频功能 2016-8-19
                if (tempSysConfig.multiFreq == 0)
                    this.MultiFreqcheckBox.Checked = false;
                else
                    this.MultiFreqcheckBox.Checked = true;

                this.SampleOutletcomboBox.Items.Clear();
                this.SampleOutletcomboBox.Items.Add("0");
                for (int i = 0; i < tempSysConfig.nExitNum; i++)
                {
                    this.SampleOutletcomboBox.Items.Add((i + 1).ToString());
                }
                if (tempSysConfig.nExitNum == 0)
                {
                    this.SampleOutletcomboBox.Text = "0";
                    this.SampleNumbertextBox.Text = "0";
                }
                else
                {
                    this.SampleOutletcomboBox.Text = GlobalDataInterface.nSampleOutlet.ToString();
                    this.SampleNumbertextBox.Text = GlobalDataInterface.nSampleNumber.ToString();
                }

                GlobalDataInterface.ProjectSetFormIsIntialed = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数SystemStructInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数SystemStructInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 系统类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SysTypecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region 旧模式系统类型

            //ComboBox comboBox = (ComboBox)sender;
            //switch (SysTypecomboBox.SelectedIndex) //中间新增类型 Modify by ChengSk - 20190226
            //{
            //    case 0:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_M;
            //        break;
            //    case 1:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_LMR;
            //        break;
            //    case 2:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_F_M;
            //        break;
            //    case 3:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_LFR_LMR;
            //        break;
            //    case 4:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_F_M_B;
            //        break;
            //    case 5:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_LFR_LMR_LBR;
            //        break;
            //    case 6:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_LR;
            //        break;
            //    case 7:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_LR_LR;
            //        break;
            //    case 8:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_LR_LR_LR;
            //        break;
            //    case 9:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_F;
            //        break;
            //    case 10:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_LRF;
            //        break;
            //    case 11:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_LFR;
            //        break;
            //    case 12:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_F_B;
            //        break;
            //    case 13:
            //        tempSysConfig.nSystemInfo = ConstPreDefine.RM_LFR_LBR;
            //        break;
            //    default: break;
            //}
            #endregion
        }

        private void btnLampLNIR2_Click(object sender, EventArgs e)
        {
            if (this.lblLampLNIR2.ForeColor == Color.Green) //启用->未启用
            {
                this.lblLampLNIR2.ForeColor = Color.Red;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo & 0xFEFF);
            }
            else //未启用->启用
            {
                this.lblLampLNIR2.ForeColor = Color.Green;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo | 0x0100);
            }
        }

        private void btnLampMNIR2_Click(object sender, EventArgs e)
        {
            if (this.lblLampMNIR2.ForeColor == Color.Green) //启用->未启用
            {
                this.lblLampMNIR2.ForeColor = Color.Red;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo & 0xFF7F);
            }
            else //未启用->启用
            {
                this.lblLampMNIR2.ForeColor = Color.Green;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo | 0x0080);
            }
        }

        private void btnLampRNIR2_Click(object sender, EventArgs e)
        {
            if (this.lblLampRNIR2.ForeColor == Color.Green) //启用->未启用
            {
                this.lblLampRNIR2.ForeColor = Color.Red;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo & 0xFFBF);
            }
            else //未启用->启用
            {
                this.lblLampRNIR2.ForeColor = Color.Green;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo | 0x0040);
            }
        }

        private void btnLampLNIR1_Click(object sender, EventArgs e)
        {
            if (this.lblLampLNIR1.ForeColor == Color.Green) //启用->未启用
            {
                this.lblLampLNIR1.ForeColor = Color.Red;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo & 0xFFDF);
            }
            else //未启用->启用
            {
                this.lblLampLNIR1.ForeColor = Color.Green;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo | 0x0020);
            }
        }

        private void btnLampMNIR1_Click(object sender, EventArgs e)
        {
            if (this.lblLampMNIR1.ForeColor == Color.Green) //启用->未启用
            {
                this.lblLampMNIR1.ForeColor = Color.Red;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo & 0xFFEF);
            }
            else //未启用->启用
            {
                this.lblLampMNIR1.ForeColor = Color.Green;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo | 0x0010);
            }
        }

        private void btnLampRNIR1_Click(object sender, EventArgs e)
        {
            if (this.lblLampRNIR1.ForeColor == Color.Green) //启用->未启用
            {
                this.lblLampRNIR1.ForeColor = Color.Red;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo & 0xFFF7);
            }
            else //未启用->启用
            {
                this.lblLampRNIR1.ForeColor = Color.Green;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo | 0x0008);
            }
        }

        private void btnLampLColor_Click(object sender, EventArgs e)
        {
            if (this.lblLampLColor.ForeColor == Color.Green) //启用->未启用
            {
                this.lblLampLColor.ForeColor = Color.Red;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo & 0xFFFB);
            }
            else //未启用->启用
            {
                this.lblLampLColor.ForeColor = Color.Green;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo | 0x0004);
            }
        }

        private void btnLampMColor_Click(object sender, EventArgs e)
        {
            if (this.lblLampMColor.ForeColor == Color.Green) //启用->未启用
            {
                this.lblLampMColor.ForeColor = Color.Red;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo & 0xFFFD);
            }
            else //未启用->启用
            {
                this.lblLampMColor.ForeColor = Color.Green;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo | 0x0002);
            }
        }

        private void btnLampRColor_Click(object sender, EventArgs e)
        {
            if (this.lblLampRColor.ForeColor == Color.Green) //启用->未启用
            {
                this.lblLampRColor.ForeColor = Color.Red;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo & 0xFFFE);
            }
            else //未启用->启用
            {
                this.lblLampRColor.ForeColor = Color.Green;
                tempSysConfig.nSystemInfo = (UInt16)(tempSysConfig.nSystemInfo | 0x0001);
            }
        }

        /// <summary>
        /// 子系统信息List中SubItemClicked事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubSysInfolistViewEx_SubItemClicked(object sender, SubItemEventArgs e)
        {
            try
            {
                if (e.SubItem > 0)
                {
                    switch (e.SubItem)
                    {
                        case 1:
                            this.SubSysInfolistViewEx.StartEditing(SystemEditors[e.SubItem - 1], e.Item, e.SubItem);
                            break;
                        case 2:
                            if ((tempSysConfig.nClassificationInfo & 0x02) == 2)
                                this.SubSysInfolistViewEx.StartEditing(SystemEditors[e.SubItem - 1], e.Item, e.SubItem);
                            break;
                        case 3:
                            if ((tempSysConfig.nClassificationInfo & 0x04) == 4)
                                this.SubSysInfolistViewEx.StartEditing(SystemEditors[e.SubItem - 1], e.Item, e.SubItem);
                            break;
                        case 4:
                            if ((tempSysConfig.nClassificationInfo & 0x08) == 8)
                                this.SubSysInfolistViewEx.StartEditing(SystemEditors[e.SubItem - 1], e.Item, e.SubItem);
                            break;
                        case 5:
                            if ((tempSysConfig.nClassificationInfo & 0x10) == 16)
                                this.SubSysInfolistViewEx.StartEditing(SystemEditors[e.SubItem - 1], e.Item, e.SubItem);
                            break;
                        case 6:  //Add by ChengSk - 20191111
                            if ((tempSysConfig.nClassificationInfo & 0x08) == 8)
                                this.SubSysInfolistViewEx.StartEditing(SystemEditors[e.SubItem - 1], e.Item, e.SubItem);
                            break;
                    }
                }

                //if ((tempSysConfig.nClassificationInfo & 3) > 0)
                //{
                //    if (e.SubItem > 0)
                //        this.SubSysInfolistViewEx.StartEditing(SystemEditors[e.SubItem - 1], e.Item, e.SubItem);
                //}
                //else
                //{
                //    if (e.SubItem > 0 && e.SubItem != 2)
                //        this.SubSysInfolistViewEx.StartEditing(SystemEditors[e.SubItem - 1], e.Item, e.SubItem);
                //}
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数SubSysInfolistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数SubSysInfolistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 子系统个数comboBox的SelectedIndexChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubSysNumcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;

                if (comboBox.SelectedIndex != tempSysConfig.nSubsysNum - 1)
                {
                    if (comboBox.SelectedIndex < tempSysConfig.nSubsysNum - 1)
                    {
                        for (int i = comboBox.SelectedIndex + 1; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                        {
                            m_ChannelNum[i] = 0;
                            tempSysConfig.nChannelInfo[i] = 0; //Modify by ChengSk - 20190521
                            for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                            {
                                //tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] = 0;
                                //if (m_ChanelIDList.IndexOf(Commonfunction.EncodeChannel(i, j, j)) >= 0)
                                //{
                                //    m_ChanelIDList.Remove(Commonfunction.EncodeChannel(i, j, j));
                                //}
                                if (GlobalDataInterface.nVer == 0) //Modify by xcw - 20200619
                                {
                                    if (m_ChanelIDList.IndexOf(Commonfunction.EncodeChannel(i, j, j)) >= 0)
                                    {
                                        m_ChanelIDList.Remove(Commonfunction.EncodeChannel(i, j, j));
                                    }
                                }
                                else if (GlobalDataInterface.nVer == 1)
                                {
                                    if (m_ChanelIDList.IndexOf(Commonfunction.EncodeChannel(i, j / 2, j % 2)) >= 0)
                                    {
                                        m_ChanelIDList.Remove(Commonfunction.EncodeChannel(i, j / 2, j % 2));
                                    }
                                }
                            }
                        }
                    }

                    SetSubSysInfolistViewEx(comboBox.SelectedIndex);
                    tempSysConfig.nSubsysNum = (byte)(comboBox.SelectedIndex + 1);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数SubSysNumcomboBox_SelectedIndexChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数SubSysNumcomboBox_SelectedIndexChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置子系统信息list的Item
        /// </summary>
        /// <param name="SelectedIndex"></param>
        private void SetSubSysInfolistViewEx(int SelectedIndex)
        {
            try
            {
                ListViewItem lvi;

                if (SelectedIndex > tempSysConfig.nSubsysNum - 1)
                {
                    int startIndex = 0;
                    if (tempSysConfig.nSubsysNum > 0) startIndex = tempSysConfig.nSubsysNum;
                    for (int i = startIndex; i < SelectedIndex + 1; i++)
                    {
                        lvi = new ListViewItem(string.Format("{0}", i + 1));
                        lvi.SubItems.Add(string.Format("{0}", m_ChannelNum[i]));
                        lvi.SubItems.Add(string.Format("{0}", tempSysConfig.nImageUV[i]));
                        lvi.SubItems.Add(string.Format("{0}", tempSysConfig.nDataRegistration[i]));
                        lvi.SubItems.Add(string.Format("{0}", tempSysConfig.nImageSugar[i]));
                        lvi.SubItems.Add(string.Format("{0}", tempSysConfig.nImageUltrasonic[i]));
                        lvi.SubItems.Add(string.Format("{0}", GlobalDataInterface.globalIn_defaultInis[i].sys.nIQSEnable)); //Add by ChengSk - 20191111
                        this.SubSysInfolistViewEx.Items.Add(lvi);
                    }
                }
                else
                {
                    for (int i = tempSysConfig.nSubsysNum - 1; i > SelectedIndex; i--)
                    {
                        SubSysInfolistViewEx.Items.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数SetSubSysInfolistViewEx出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数SetSubSysInfolistViewEx出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 子系统信息List的结束编辑事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubSysInfolistViewEx_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        {
            try
            {
                ListViewEx.ListViewEx listviewex = (ListViewEx.ListViewEx)sender;

                switch (e.SubItem)
                {
                    case 1:
                        m_ChannelNum[e.Item.Index] = int.Parse(e.DisplayText);
                        //for (int i = 0; i < m_ChannelNum[e.Item.Index]; i++)
                        //{
                        //    tempSysConfig.nChannelInfo[e.Item.Index * ConstPreDefine.MAX_CHANNEL_NUM + i] = 1;
                        //}
                        //for (int i = m_ChannelNum[e.Item.Index]; i < ConstPreDefine.MAX_CHANNEL_NUM; i++)
                        //{
                        //    tempSysConfig.nChannelInfo[e.Item.Index * ConstPreDefine.MAX_CHANNEL_NUM + i] = 0;
                        //}
                        tempSysConfig.nChannelInfo[e.Item.Index] = (byte)m_ChannelNum[e.Item.Index]; //Modify by ChengSk - 20190521
                        break;
                    case 2:
                        tempSysConfig.nImageUV[e.Item.Index] = byte.Parse(e.DisplayText);
                        break;
                    case 3:
                        tempSysConfig.nDataRegistration[e.Item.Index] = byte.Parse(e.DisplayText);
                        break;
                    case 4:
                        tempSysConfig.nImageSugar[e.Item.Index] = byte.Parse(e.DisplayText);
                        break;
                    case 5:
                        tempSysConfig.nImageUltrasonic[e.Item.Index] = byte.Parse(e.DisplayText);
                        break;
                    case 6:  //Add by ChengSk - 20191111
                        tempSysConfig.nIQSEnable = byte.Parse(e.DisplayText);
                        //MessageBox.Show(((int)tempSysConfig.nIQSEnable).ToString());
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数SubSysInfolistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数SubSysInfolistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 相机类型的SelectedIndexChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraTypecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;

                int maxWidth = 0;    //最大宽度
                int maxHeight = 0;   //最大高度
                byte cameraType = 0; //相机类型
                cameraType = (byte)comboBox.SelectedIndex;
                tempSysConfig.nCameraType = cameraType;
                //switch (cameraType)
                //{
                //    case 0x00:
                //        maxWidth = 1024;
                //        maxHeight = 400;
                //        this.WhiteBalanceRnumericUpDown.Maximum = 255;
                //        this.WhiteBalanceGnumericUpDown.Maximum = 255;
                //        this.WhiteBalanceBnumericUpDown.Maximum = 255;
                //        break;
                //    case 0x01:
                //        maxWidth = 1280;
                //        maxHeight = 400;
                //        this.WhiteBalanceRnumericUpDown.Maximum = 1024;
                //        this.WhiteBalanceGnumericUpDown.Maximum = 1024;
                //        this.WhiteBalanceBnumericUpDown.Maximum = 1024;
                //        break;
                //    case 0x02:
                //        maxWidth = 1600;
                //        maxHeight = 400;
                //        this.WhiteBalanceRnumericUpDown.Maximum = 255;
                //        this.WhiteBalanceGnumericUpDown.Maximum = 255;
                //        this.WhiteBalanceBnumericUpDown.Maximum = 255;
                //        break;
                //    case 0x03:
                //        maxWidth = 2046;
                //        maxHeight = 400;
                //        this.WhiteBalanceRnumericUpDown.Maximum = 255;
                //        this.WhiteBalanceGnumericUpDown.Maximum = 255;
                //        this.WhiteBalanceBnumericUpDown.Maximum = 255;
                //        break;
                //    default:
                //        maxWidth = 1024;
                //        maxHeight = 400;
                //        this.WhiteBalanceRnumericUpDown.Maximum = 255;
                //        this.WhiteBalanceGnumericUpDown.Maximum = 255;
                //        this.WhiteBalanceBnumericUpDown.Maximum = 255;
                //        break;
                //}
                switch (cameraType)  //Modify by ChengSk - 20190520
                {
                    case 0x00:
                        maxWidth = 1280;
                        //maxHeight = GlobalDataInterface.nVer == 0?400:1024;
                        maxHeight = 1024;

                        this.WhiteBalanceRnumericUpDown.Maximum = 1023;
                        this.WhiteBalanceGnumericUpDown.Maximum = 1023;
                        this.WhiteBalanceBnumericUpDown.Maximum = 1023;//modify by xcw 20210112--->255改成1023
                        break;
                    default:
                        maxWidth = 1280;
                        maxHeight = 1024;
                        this.WhiteBalanceRnumericUpDown.Maximum = 1023;
                        this.WhiteBalanceGnumericUpDown.Maximum = 1023;
                        this.WhiteBalanceBnumericUpDown.Maximum = 1023;//4.2固定图像高度（3.2已修改）
                        break;
                }
                maxWidth = 2560;
                maxHeight = 1920;
                //宽度
                if (tempSysConfig.width > maxWidth || tempSysConfig.width == 0) tempSysConfig.width = maxWidth;
                if (tempSysConfig.width < 2) tempSysConfig.width = 2;
                this.WidthnumericUpDown.Maximum = maxWidth;
                this.WidthnumericUpDown.Text = tempSysConfig.width.ToString();
                //高度
                if (tempSysConfig.height > maxHeight || tempSysConfig.height == 0) tempSysConfig.height = maxHeight;
                if (tempSysConfig.height < 2) tempSysConfig.height = 2;
                this.HeightnumericUpDown.Maximum = maxHeight;
                this.HeightnumericUpDown.Text = tempSysConfig.height.ToString();
                //相机界面白平衡参数
                if (int.Parse(this.WhiteBalanceRnumericUpDown.Text) > this.WhiteBalanceRnumericUpDown.Maximum)
                    this.WhiteBalanceRnumericUpDown.Text = this.WhiteBalanceRnumericUpDown.Maximum.ToString();

            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数CameraTypecomboBox_SelectedIndexChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数CameraTypecomboBox_SelectedIndexChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// CIR视觉系统CheckBox的checked事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisioncheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkbox = (CheckBox)sender;
                //uint mark = 0;

                if (checkbox.Checked)
                {
                    tempSysConfig.nClassificationInfo |= 0x01;//第一位置1

                    this.ColorcheckBox.Enabled = true;
                    this.ShapecheckBox.Enabled = true;
                    this.FlawcheckBox.Enabled = true;
                    this.VolumecheckBox.Enabled = true;
                    this.ProjectedAreacheckBox.Enabled = true;
                    //if (tempSysConfig.nClassificationInfo == 3)
                    //    this.ImgAndWeighnumericUpDown.Enabled = true;
                }
                else
                {
                    tempSysConfig.nClassificationInfo &= 0xFE;//第一位置0

                    this.ColorcheckBox.Enabled = false;
                    this.ShapecheckBox.Enabled = false;
                    this.FlawcheckBox.Enabled = false;
                    this.VolumecheckBox.Enabled = false;
                    this.ProjectedAreacheckBox.Enabled = false;

                    //this.WeighcheckBox.Checked = true;  //CIR视觉系统取消时，重量系统必须被选择（二者对立关系）

                    //mark = (~mark) ^ 1;
                    //tempSysConfig.nClassificationInfo &= (byte)mark;//第一位置0
                    //this.ImgAndWeighnumericUpDown.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数VisioncheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数VisioncheckBox_CheckedChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// UV视觉系统CheckBox的checked事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UVVisioncheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkbox = (CheckBox)sender;

                if (checkbox.Checked)
                {
                    tempSysConfig.nClassificationInfo |= 0x02;//第二位置1

                    this.BruisecheckBox.Enabled = true;
                    this.RotcheckBox.Enabled = true;
                    this.ImgAndUVnumericUpDown.Enabled = true;
                }
                else
                {
                    tempSysConfig.nClassificationInfo &= 0xFD;//第二位置0

                    this.BruisecheckBox.Enabled = false;
                    this.RotcheckBox.Enabled = false;
                    this.ImgAndUVnumericUpDown.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数UVVisioncheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数UVVisioncheckBox_CheckedChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 重量系统CheckBox的checked事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeighcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkbox = (CheckBox)sender;

                if (checkbox.Checked)
                {
                    tempSysConfig.nClassificationInfo |= 0x04;//第三位置1

                    this.DensitycheckBox.Enabled = true;
                    this.ImgAndWeighnumericUpDown.Enabled = true;
                }
                else
                {
                    tempSysConfig.nClassificationInfo &= 0xFB;//第三位置0

                    this.DensitycheckBox.Enabled = false;
                    this.ImgAndWeighnumericUpDown.Enabled = false;

                    //this.CIRVisioncheckBox.Checked = true; //重量系统被取消时，CIR系统必须选中（二者对立关系）
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数WeighcheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数WeighcheckBox_CheckedChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 内部品质CheckBox的checked事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QualitycheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkbox = (CheckBox)sender;

                if (checkbox.Checked)
                {
                    tempSysConfig.nClassificationInfo |= 0x08;//第四位置1

                    this.SugarcheckBox.Enabled = true;
                    this.AciditycheckBox.Enabled = true;
                    this.HollowcheckBox.Enabled = true;
                    this.SkincheckBox.Enabled = true;
                    this.BrowncheckBox.Enabled = true;
                    this.TangxincheckBox.Enabled = true;
                    this.ImgAndSugarnumericUpDown.Enabled = true;
                    this.IQSEnableFlagsnumericUpDown.Enabled = true;  //Add by ChengSk - 20191111
                }
                else
                {
                    tempSysConfig.nClassificationInfo &= 0xF7;//第四位置0

                    this.SugarcheckBox.Enabled = false;
                    this.AciditycheckBox.Enabled = false;
                    this.HollowcheckBox.Enabled = false;
                    this.SkincheckBox.Enabled = false;
                    this.BrowncheckBox.Enabled = false;
                    this.TangxincheckBox.Enabled = false;
                    this.ImgAndSugarnumericUpDown.Enabled = false;
                    this.IQSEnableFlagsnumericUpDown.Enabled = false;  //Add by ChengSk - 20191111
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数QualitycheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数QualitycheckBox_CheckedChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 超声波CheckBox的checked事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UltrasoniccheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkbox = (CheckBox)sender;

                if (checkbox.Checked)
                {
                    tempSysConfig.nClassificationInfo |= 0x10;//第五位置1

                    this.HardnesscheckBox.Enabled = true;
                    this.WatercheckBox.Enabled = true;
                    this.ImgAndUltrasonicnumericUpDown.Enabled = true;
                }
                else
                {
                    tempSysConfig.nClassificationInfo &= 0xEF;//第五位置0

                    this.HardnesscheckBox.Enabled = false;
                    this.WatercheckBox.Enabled = false;
                    this.ImgAndUltrasonicnumericUpDown.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数UltrasoniccheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数UltrasoniccheckBox_CheckedChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 分辨率中宽NumericUpDown的ValueChanged事件
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WidthnumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                NumericUpDown numeric = (NumericUpDown)sender;

                tempSysConfig.width = int.Parse(numeric.Value.ToString());
                if (tempSysConfig.width % 2 != 0)
                {
                    tempSysConfig.width += 1;
                    numeric.Text = tempSysConfig.width.ToString();
                }

                //int maxWidth = 0;
                //byte cameraType = 0; //相机类型
                //cameraType = tempSysConfig.nCameraType;
                ////switch (cameraType)
                ////{
                ////    case 0x00:
                ////        maxWidth = 1024;
                ////        break;
                ////    case 0x01:
                ////        maxWidth = 1280;
                ////        break;
                ////    case 0x02:
                ////        maxWidth = 1600;
                ////        break;
                ////    case 0x03:
                ////        maxWidth = 2046;
                ////        break;
                ////    default:
                ////        maxWidth = 1024;
                ////        break;
                ////}
                //switch (cameraType) //Modify by ChengSk - 20190520
                //{
                //    case 0x00:
                //        maxWidth = 1280;
                //        break;
                //    default:
                //        maxWidth = 1280;
                //        break;
                //}

                //if (tempSysConfig.width > maxWidth)
                //{
                //    tempSysConfig.width = maxWidth;
                //}
                if (tempSysConfig.width > ConstPreDefine.CAPTURE_WIDTH)
                {
                    tempSysConfig.width = ConstPreDefine.CAPTURE_WIDTH;
                }
                if (tempSysConfig.width < 2)
                {
                    tempSysConfig.width = 2;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数WidthnumericUpDown_ValueChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数WidthnumericUpDown_ValueChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 分辨率中高NumericUpDown的ValueChanged事件
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeightnumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                NumericUpDown numeric = (NumericUpDown)sender;
                tempSysConfig.height = int.Parse(numeric.Value.ToString());
                if (tempSysConfig.height % 2 != 0)
                {
                    tempSysConfig.height += 1;
                    numeric.Text = tempSysConfig.width.ToString();
                }

                //int maxHeight = 0;
                //byte cameraType = 0; //相机类型
                //cameraType = tempSysConfig.nCameraType;
                ////switch (cameraType)
                ////{
                ////    case 0x00:
                ////        maxHeight = 768;
                ////        break;
                ////    case 0x01:
                ////        maxHeight = 1024;
                ////        break;
                ////    case 0x02:
                ////        maxHeight = 1200;
                ////        break;
                ////    case 0x03:
                ////        maxHeight = 1086;
                ////        break;
                ////    default:
                ////        maxHeight = 768;
                ////        break;
                ////}
                //switch (cameraType)  //Modify by ChengSk - 20190520
                //{
                //    case 0x00:
                //        maxHeight = 1024;
                //        break;
                //    default:
                //        maxHeight = 1024;
                //        break;
                //}// Modify by xcw 20200918

                //if (tempSysConfig.height > maxHeight)
                //{
                //    tempSysConfig.height = maxHeight;
                //}
                if (tempSysConfig.height > ConstPreDefine.CAPTURE_HEIGHT)
                {
                    tempSysConfig.height = ConstPreDefine.CAPTURE_HEIGHT;
                }
                if (tempSysConfig.height < 2)
                {
                    tempSysConfig.height = 2;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数HeightnumericUpDown_ValueChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数HeightnumericUpDown_ValueChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 包大小中NumericUpDown的ValueChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PacketSizenumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                NumericUpDown numeric = (NumericUpDown)sender;
                int size = int.Parse(numeric.Text);
                if (size % 100 != 0)
                {
                    numeric.Text = (size / 100 * 100).ToString();//只能输入100的整数倍
                }
                //tempSysConfig.packetSize = int.Parse(numeric.Text);
                tempSysConfig.packetSize = int.Parse(numeric.Value.ToString());
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数PacketSizenumericUpDown_ValueChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数PacketSizenumericUpDown_ValueChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 出口数量控件SelectedIndexChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitNumcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;
                if (tempSysConfig.nExitNum != comboBox.SelectedIndex + 1)
                {

                    tempSysConfig.exitstate = new byte[4 * ConstPreDefine.MAX_EXIT_NUM * 2];
                    this.TableSetglacialList.Columns.Clear();
                    //int startIndex = 0;
                    //if (comboBox.SelectedIndex + 1 > tempSysConfig.nExitNum)
                    //{
                    //    if (tempSysConfig.nExitNum > 0) startIndex = tempSysConfig.nExitNum * 2;
                    for (int i = 0; i < (comboBox.SelectedIndex + 1) * 2; i++)
                    {
                        this.TableSetglacialList.Columns.Add((i + 1).ToString(), 40);
                        this.TableSetglacialList.Columns[i].Text = (i + 1).ToString();
                        this.TableSetglacialList.Columns[i].CheckBoxes = true;
                        this.TableSetglacialList.Columns[i].TextAlignment = ContentAlignment.MiddleCenter;
                        for (int j = 0; j < 5; j++)
                        {
                            if (j != 2)
                            {
                                this.TableSetglacialList.Items[j].SubItems[i].Text = "0";
                                this.TableSetglacialList.Items[j].SubItems[i].Checked = false;
                            }
                        }
                    }
                    //}
                    //else
                    //{
                    //    for (int i = tempSysConfig.nExitNum * 2 - 1; i > (comboBox.SelectedIndex + 1) * 2 - 1; i--)
                    //    {
                    //        this.TableSetglacialList.Columns.RemoveAt(i);
                    //    }
                    //}
                    tempSysConfig.nExitNum = (byte)(comboBox.SelectedIndex + 1);
                    m_ExitList.Clear();

                }

                this.SampleOutletcomboBox.Items.Clear();
                this.SampleOutletcomboBox.Items.Add("0");
                for (int i = 0; i < comboBox.SelectedIndex + 1; i++)
                    this.SampleOutletcomboBox.Items.Add((i + 1).ToString());
                this.SampleOutletcomboBox.Text = "0";
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数ExitNumcomboBox_SelectedIndexChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数ExitNumcomboBox_SelectedIndexChanged出错" + ex);
#endif
            }
        }


        /// <summary>
        /// 出口布局出口改变事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void TableSetglacialList_SelectedIndexChanged(object source, ClickEventArgs e)
        {
            try
            {
                GlacialList list = (GlacialList)source;
                ExitState exitState = new ExitState();

                if (list.Items[e.ItemIndex].SubItems[e.ColumnIndex].Checked)
                {
                    if (e.ItemIndex != 2)
                    {
                        if (tempSysConfig.nExitNum == m_ExitList.Count)
                            list.Items[e.ItemIndex].SubItems[e.ColumnIndex].Checked = false;
                        else
                        {
                            exitState.ColumnIndex = e.ColumnIndex;
                            if (e.ItemIndex > 2)
                            {
                                tempSysConfig.exitstate[2 * (e.ItemIndex - 1) * ConstPreDefine.MAX_EXIT_NUM + e.ColumnIndex] = (byte)(m_ExitList.Count + 1);
                                exitState.ItemIndex = e.ItemIndex - 1;
                            }
                            else
                            {
                                tempSysConfig.exitstate[2 * e.ItemIndex * ConstPreDefine.MAX_EXIT_NUM + e.ColumnIndex] = (byte)(m_ExitList.Count + 1);
                                exitState.ItemIndex = e.ItemIndex;
                            }
                            exitState.Index = m_ExitList.Count + 1;
                            m_ExitList.Add(exitState);
                            list.Items[e.ItemIndex].SubItems[e.ColumnIndex].Text = m_ExitList.Count.ToString();
                        }
                    }
                }
                else
                {

                    exitState.ColumnIndex = e.ColumnIndex;

                    if (e.ItemIndex > 2)
                    {
                        exitState.ItemIndex = e.ItemIndex - 1;
                        tempSysConfig.exitstate[2 * (e.ItemIndex - 1) * ConstPreDefine.MAX_EXIT_NUM + e.ColumnIndex] = 0;
                    }
                    else
                    {
                        exitState.ItemIndex = e.ItemIndex;
                        tempSysConfig.exitstate[2 * e.ItemIndex * ConstPreDefine.MAX_EXIT_NUM + e.ColumnIndex] = 0;
                    }
                    exitState.Index = int.Parse(this.TableSetglacialList.Items[e.ItemIndex].SubItems[exitState.ColumnIndex].Text);
                    int index = m_ExitList.IndexOf(exitState);
                    for (int i = index; i < m_ExitList.Count; i++)
                    {

                        if (m_ExitList[i].ItemIndex > 1)
                        {
                            list.Items[m_ExitList[i].ItemIndex + 1].SubItems[m_ExitList[i].ColumnIndex].Text = "0";
                            list.Items[m_ExitList[i].ItemIndex + 1].SubItems[m_ExitList[i].ColumnIndex].Checked = false;
                        }
                        else
                        {
                            list.Items[m_ExitList[i].ItemIndex].SubItems[m_ExitList[i].ColumnIndex].Text = "0";
                            list.Items[m_ExitList[i].ItemIndex].SubItems[m_ExitList[i].ColumnIndex].Checked = false;
                        }
                        tempSysConfig.exitstate[2 * m_ExitList[i].ItemIndex * ConstPreDefine.MAX_EXIT_NUM + m_ExitList[i].ColumnIndex] = 0;
                    }
                    m_ExitList.RemoveRange(index, m_ExitList.Count - index);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数TableSetglacialList_SelectedIndexChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数TableSetglacialList_SelectedIndexChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 重排出口布局
        /// </summary>
        private void ResetExitState()
        {
            try
            {
                //第一列必须有出口，否则全部偏移
                for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM * 2; i++)
                {
                    if (tempSysConfig.exitstate[i] > 0 || tempSysConfig.exitstate[2 * ConstPreDefine.MAX_EXIT_NUM + i] > 0
                        || tempSysConfig.exitstate[4 * ConstPreDefine.MAX_EXIT_NUM + i] > 0 || tempSysConfig.exitstate[6 * ConstPreDefine.MAX_EXIT_NUM + i] > 0)
                    {
                        if (i > 0)
                        {
                            m_ExitList.Clear();
                            ExitState exitState = new ExitState();
                            for (int j = i; j < ConstPreDefine.MAX_EXIT_NUM * 2; j++)
                            {
                                tempSysConfig.exitstate[j - i] = tempSysConfig.exitstate[j]; tempSysConfig.exitstate[j] = 0;
                                tempSysConfig.exitstate[2 * ConstPreDefine.MAX_EXIT_NUM + j - i] = tempSysConfig.exitstate[2 * ConstPreDefine.MAX_EXIT_NUM + j]; tempSysConfig.exitstate[2 * ConstPreDefine.MAX_EXIT_NUM + j] = 0;
                                tempSysConfig.exitstate[4 * ConstPreDefine.MAX_EXIT_NUM + j - i] = tempSysConfig.exitstate[4 * ConstPreDefine.MAX_EXIT_NUM + j]; tempSysConfig.exitstate[4 * ConstPreDefine.MAX_EXIT_NUM + j] = 0;
                                tempSysConfig.exitstate[6 * ConstPreDefine.MAX_EXIT_NUM + j - i] = tempSysConfig.exitstate[6 * ConstPreDefine.MAX_EXIT_NUM + j]; tempSysConfig.exitstate[6 * ConstPreDefine.MAX_EXIT_NUM + j] = 0;
                                if (tempSysConfig.exitstate[j - i] > 0)
                                {
                                    exitState.ColumnIndex = j - i;
                                    exitState.ItemIndex = 0;
                                    exitState.Index = int.Parse(this.TableSetglacialList.Items[0].SubItems[j].Text);
                                    m_ExitList.Add(exitState);
                                    this.TableSetglacialList.Items[exitState.ItemIndex].SubItems[exitState.ColumnIndex].Text = exitState.Index.ToString();
                                }
                                if (tempSysConfig.exitstate[2 * ConstPreDefine.MAX_EXIT_NUM + j - i] > 0)
                                {
                                    exitState.ColumnIndex = j - i;
                                    exitState.ItemIndex = 1;
                                    exitState.Index = int.Parse(this.TableSetglacialList.Items[1].SubItems[j].Text);
                                    this.TableSetglacialList.Items[exitState.ItemIndex].SubItems[exitState.ColumnIndex].Text = exitState.Index.ToString();
                                    m_ExitList.Add(exitState);
                                }
                                if (tempSysConfig.exitstate[4 * ConstPreDefine.MAX_EXIT_NUM + j - i] > 0)
                                {
                                    exitState.ColumnIndex = j - i;
                                    exitState.ItemIndex = 2;
                                    exitState.Index = int.Parse(this.TableSetglacialList.Items[3].SubItems[j].Text);
                                    m_ExitList.Add(exitState);
                                    this.TableSetglacialList.Items[exitState.ItemIndex + 1].SubItems[exitState.ColumnIndex].Text = exitState.Index.ToString();
                                }
                                if (tempSysConfig.exitstate[6 * ConstPreDefine.MAX_EXIT_NUM + j - i] > 0)
                                {
                                    exitState.ColumnIndex = j - i;
                                    exitState.ItemIndex = 3;
                                    exitState.Index = int.Parse(this.TableSetglacialList.Items[4].SubItems[j].Text);
                                    m_ExitList.Add(exitState);
                                    this.TableSetglacialList.Items[exitState.ItemIndex + 1].SubItems[exitState.ColumnIndex].Text = exitState.Index.ToString();
                                }
                            }
                        }
                        break;
                    }
                }

                //如果相邻两列都没有数据则后面的往前挪，直到相差一列
                int nLastEmptyColumn = -1;
                int nEmpthColumnNum = 0;
                for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM * 2; i++)
                {
                    if (!(tempSysConfig.exitstate[i] > 0 || tempSysConfig.exitstate[2 * ConstPreDefine.MAX_EXIT_NUM + i] > 0
                        || tempSysConfig.exitstate[4 * ConstPreDefine.MAX_EXIT_NUM + i] > 0 || tempSysConfig.exitstate[6 * ConstPreDefine.MAX_EXIT_NUM + i] > 0))
                    {
                        if (nLastEmptyColumn == -1)
                        {
                            nLastEmptyColumn = i;
                        }
                        nEmpthColumnNum++;
                    }
                    else
                    {
                        if (nEmpthColumnNum > 1 && nLastEmptyColumn != -1)
                        {
                            //前面有多个空列，将后面的数据往前挪i-nLastEmptyColumn个位置
                            int t = i - (nLastEmptyColumn + 1);
                            ExitState exitState = new ExitState();
                            for (int j = i; j < ConstPreDefine.MAX_EXIT_NUM * 2; j++)
                            {
                                tempSysConfig.exitstate[j - t] = tempSysConfig.exitstate[j]; tempSysConfig.exitstate[j] = 0;
                                if (tempSysConfig.exitstate[j - t] > 0)
                                {
                                    exitState.ColumnIndex = j;
                                    exitState.ItemIndex = 0;
                                    exitState.Index = int.Parse(this.TableSetglacialList.Items[exitState.ItemIndex].SubItems[exitState.ColumnIndex].Text);
                                    m_ExitList.Remove(exitState);
                                    exitState.ColumnIndex = j - t;
                                    exitState.ItemIndex = 0;
                                    // exitState.Index = GlobalDataInterface.ExitList.Count+1;
                                    m_ExitList.Add(exitState);
                                }

                                tempSysConfig.exitstate[2 * ConstPreDefine.MAX_EXIT_NUM + j - t] = tempSysConfig.exitstate[2 * ConstPreDefine.MAX_EXIT_NUM + j]; tempSysConfig.exitstate[2 * ConstPreDefine.MAX_EXIT_NUM + j] = 0;
                                if (tempSysConfig.exitstate[2 * ConstPreDefine.MAX_EXIT_NUM + j - t] > 0)
                                {
                                    exitState.ColumnIndex = j;
                                    exitState.ItemIndex = 1;
                                    exitState.Index = int.Parse(this.TableSetglacialList.Items[exitState.ItemIndex].SubItems[exitState.ColumnIndex].Text);
                                    m_ExitList.Remove(exitState);
                                    exitState.ColumnIndex = j - t;
                                    exitState.ItemIndex = 1;
                                    // exitState.Index = GlobalDataInterface.ExitList.Count + 1;
                                    m_ExitList.Add(exitState);
                                }

                                tempSysConfig.exitstate[4 * ConstPreDefine.MAX_EXIT_NUM + j - t] = tempSysConfig.exitstate[4 * ConstPreDefine.MAX_EXIT_NUM + j]; tempSysConfig.exitstate[4 * ConstPreDefine.MAX_EXIT_NUM + j] = 0;
                                if (tempSysConfig.exitstate[4 * ConstPreDefine.MAX_EXIT_NUM + j - t] > 0)
                                {
                                    exitState.ColumnIndex = j;
                                    exitState.ItemIndex = 2;
                                    exitState.Index = int.Parse(this.TableSetglacialList.Items[exitState.ItemIndex + 1].SubItems[exitState.ColumnIndex].Text);
                                    m_ExitList.Remove(exitState);
                                    exitState.ColumnIndex = j - t;
                                    exitState.ItemIndex = 2;
                                    // exitState.Index = GlobalDataInterface.ExitList.Count + 1;
                                    m_ExitList.Add(exitState);
                                }

                                tempSysConfig.exitstate[6 * ConstPreDefine.MAX_EXIT_NUM + j - t] = tempSysConfig.exitstate[6 * ConstPreDefine.MAX_EXIT_NUM + j]; tempSysConfig.exitstate[6 * ConstPreDefine.MAX_EXIT_NUM + j] = 0;
                                if (tempSysConfig.exitstate[6 * ConstPreDefine.MAX_EXIT_NUM + j - t] > 0)
                                {
                                    exitState.ColumnIndex = j;
                                    exitState.ItemIndex = 3;
                                    exitState.Index = int.Parse(this.TableSetglacialList.Items[exitState.ItemIndex + 1].SubItems[exitState.ColumnIndex].Text);
                                    m_ExitList.Remove(exitState);
                                    exitState.ColumnIndex = j - t;
                                    exitState.ItemIndex = 3;
                                    //  exitState.Index = GlobalDataInterface.ExitList.Count + 1;
                                    m_ExitList.Add(exitState);
                                }
                            }
                            i = nLastEmptyColumn + 1;
                        }
                        nLastEmptyColumn = -1;
                        nEmpthColumnNum = 0;
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < tempSysConfig.nExitNum * 2; j++)
                    {
                        if (i < 2)
                        {
                            this.TableSetglacialList.Items[i].SubItems[j].Text = "0";
                            this.TableSetglacialList.Items[i].SubItems[j].Checked = false;
                        }
                        else
                        {
                            this.TableSetglacialList.Items[i + 1].SubItems[j].Text = "0";
                            this.TableSetglacialList.Items[i + 1].SubItems[j].Checked = false;
                        }
                    }
                }
                for (int i = 0; i < m_ExitList.Count; i++)
                {
                    if (m_ExitList[i].ItemIndex < 2)
                    {
                        this.TableSetglacialList.Items[m_ExitList[i].ItemIndex].SubItems[m_ExitList[i].ColumnIndex].Text = m_ExitList[i].Index.ToString();
                        this.TableSetglacialList.Items[m_ExitList[i].ItemIndex].SubItems[m_ExitList[i].ColumnIndex].Checked = true;
                    }
                    else
                    {
                        this.TableSetglacialList.Items[m_ExitList[i].ItemIndex + 1].SubItems[m_ExitList[i].ColumnIndex].Text = m_ExitList[i].Index.ToString();
                        this.TableSetglacialList.Items[m_ExitList[i].ItemIndex + 1].SubItems[m_ExitList[i].ColumnIndex].Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-SystemSruct中函数ResetExitState出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-SystemSruct中函数ResetExitState出错" + ex);
#endif
            }

        }


        /// <summary>
        /// 保存系统结构配置参数
        /// </summary>
        private bool SystemStructSaveConfig()
        {
            if (tempSysConfig.nExitNum != m_ExitList.Count)
            {
                //MessageBox.Show("当前工作台出口数设置没有达到设定量！");
                //MessageBox.Show("0x10001002 The current number of layout settings exports did not reach the set amount!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("0x10001002 " + LanguageContainer.SystemStructMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.SystemStructMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            try
            {
                m_ChanelIDList.Clear();
                Commonfunction.GetAllChannelID(tempSysConfig.nChannelInfo, ref m_ChanelIDList);
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    {
                        //if (tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] !=
                        //    GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j])
                        if (tempSysConfig.nChannelInfo[i] != GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i])  //Modify by ChengSk - 20190521
                        {
                            //if (tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 0)
                            if (tempSysConfig.nChannelInfo[i] <= j) //Modify by ChengSk - 20190521
                            {
                                //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].Clear();
                                if (GlobalDataInterface.global_IsTestMode)
                                {
                                    if (GlobalDataInterface.nVer == 0)//Modify by xcw - 20200619
                                    {
                                        GlobalDataInterface.TransmitParam(Commonfunction.EncodeChannel(i, j, j), (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null);
                                    }
                                    else if (GlobalDataInterface.nVer == 1)
                                    {
                                        GlobalDataInterface.TransmitParam(Commonfunction.EncodeChannel(i, j / 2, j % 2), (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null);
                                    }
                                    //GlobalDataInterface.TransmitParam(Commonfunction.EncodeChannel(i, j, j), (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null);
                                }
                            }
                            else
                            {

                                m_bAlert = true;
                                if (j >= GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i])
                                {
                                    for (int n = 0; n < ConstPreDefine.MAX_LABEL_NUM; n++)
                                    {
                                        //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].labelexit[n].nDis = (short)(tempSysConfig.nDataRegistration[i] + 2);
                                        GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].labelexit[n].nDriverPin = 0;
                                    }
                                    for (int n = 0; n < tempSysConfig.nExitNum; n++)
                                    {
                                        //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDis = (short)(tempSysConfig.nDataRegistration[i] + 2);
                                        //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nOffset = 0;
                                        GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDriverPin = 0;
                                    }
                                }
                                for (int n = 0; n < ConstPreDefine.MAX_LABEL_NUM; n++)
                                {
                                    //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].labelexit[n].nDis = (short)(tempSysConfig.nDataRegistration[i] + 2);
                                    //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].labelexit[n].nDriverPin = 0;
                                }
                                for (int n = 0; n < tempSysConfig.nExitNum; n++)
                                {
                                    //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDis = (short)(tempSysConfig.nDataRegistration[i] + 2);
                                    //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nOffset = 0;
                                    //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDriverPin = 0;
                                }

                            }
                        }
                    }

                }
                if (this.CIRVisioncheckBox.Checked != GlobalDataInterface.CIRAvailable || this.UVVisioncheckBox.Checked != GlobalDataInterface.UVAvailable ||
                    this.WeighcheckBox.Checked != GlobalDataInterface.WeightAvailable || this.InternalcheckBox.Checked != GlobalDataInterface.InternalAvailable ||
                    this.UltrasoniccheckBox.Checked != GlobalDataInterface.UltrasonicAvailable)
                {
                    if (m_bAlert)
                    {
                        //MessageBox.Show("请到通道出口页面对该通道进行相应设置，并注意修改等级设置--分选标准页面的相应内容", "通道和传感器类型改变", MessageBoxButtons.OK);
                        //MessageBox.Show("0x3001004 Please go to the page of Channel Exit to set the channel settings,and modify the Size Settings,especially the Sorting Standard!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x3001004 " + LanguageContainer.SystemStructMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.SystemStructMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        m_bAlert = false;
                    }
                    else
                    {
                        //MessageBox.Show("注意修改等级设置--分选标准页面的相应内容!", "传感器类型改变", MessageBoxButtons.OK);
                        //MessageBox.Show("0x3001002 Please modify the Size Settings,especially the Sorting Standard!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x3001002 " + LanguageContainer.SystemStructMessagebox3Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.SystemStructMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (m_bAlert)
                    {
                        //MessageBox.Show("0x3001003 Please go to the page of Channel Exit to set the channel settings!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x3001003 " + LanguageContainer.SystemStructMessagebox4Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.SystemStructMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                //保存计算机视觉和重量感应功能设置
                GlobalDataInterface.CIRAvailable = this.CIRVisioncheckBox.Checked;
                GlobalDataInterface.UVAvailable = this.UVVisioncheckBox.Checked;
                GlobalDataInterface.WeightAvailable = this.WeighcheckBox.Checked;
                GlobalDataInterface.InternalAvailable = this.InternalcheckBox.Checked;
                GlobalDataInterface.UltrasonicAvailable = this.UltrasoniccheckBox.Checked;

                if (this.WeighcheckBox.Checked)
                    this.WeightSettabPage.Tag = "True";//启用重量设置界面
                else
                    this.WeightSettabPage.Tag = "False";//不启用重量设置界面

                //if (this.InternalcheckBox.Checked)      //Add by ChengSk - 20190111
                //    this.InnerQualityPage.Tag = "True"; //启用内部品质设置界面
                //else
                //    this.InnerQualityPage.Tag = "false";//不启用内部品质设置界面

                //CIR视觉
                if (this.CIRVisioncheckBox.Checked)
                {
                    if (this.ColorcheckBox.Checked)
                        tempSysConfig.CIRClassifyType |= 0x01;
                    else
                        tempSysConfig.CIRClassifyType &= 0xFE;
                    if (this.ShapecheckBox.Checked)
                        tempSysConfig.CIRClassifyType |= 0x02;
                    else
                        tempSysConfig.CIRClassifyType &= 0xFD;
                    if (this.FlawcheckBox.Checked)
                        tempSysConfig.CIRClassifyType |= 0x04;
                    else
                        tempSysConfig.CIRClassifyType &= 0xFB;
                    if (this.VolumecheckBox.Checked)          //Add by ChengSk - 20181206
                        tempSysConfig.CIRClassifyType |= 0x08;
                    else
                        tempSysConfig.CIRClassifyType &= 0xF7;
                    if (this.ProjectedAreacheckBox.Checked)
                        tempSysConfig.CIRClassifyType |= 0x10;
                    else
                        tempSysConfig.CIRClassifyType &= 0xEF;//Add End
                    this.ChannelRangePage.Tag = "True";//启用通道范围设置界面


                }
                else
                {
                    tempSysConfig.CIRClassifyType = 0;
                    this.ChannelRangePage.Tag = "False";//不启用通道范围设置界面


                }
                //UV视觉
                if (this.UVVisioncheckBox.Checked)
                {
                    if (this.BruisecheckBox.Checked)
                        tempSysConfig.UVClassifyType |= 0x01;
                    else
                        tempSysConfig.UVClassifyType &= 0xFE;
                    if (this.RotcheckBox.Checked)
                        tempSysConfig.UVClassifyType |= 0x02;
                    else
                        tempSysConfig.UVClassifyType &= 0xFD;
                }
                else
                {
                    tempSysConfig.UVClassifyType = 0;
                }
                //重量系统 - Add by ChengSk - 20190828
                if (this.WeighcheckBox.Checked)
                {
                    if (this.DensitycheckBox.Checked)
                        tempSysConfig.WeightClassifyTpye |= 0x01;
                    else
                        tempSysConfig.WeightClassifyTpye &= 0xFE;
                }
                else
                {
                    tempSysConfig.WeightClassifyTpye = 0;
                }
                //含糖量
                if (this.InternalcheckBox.Checked)
                {
                    if (this.SugarcheckBox.Checked)
                        tempSysConfig.InternalClassifyType |= 0x01;
                    else
                        tempSysConfig.InternalClassifyType &= 0xFE;
                    if (this.AciditycheckBox.Checked)
                        tempSysConfig.InternalClassifyType |= 0x02;
                    else
                        tempSysConfig.InternalClassifyType &= 0xFD;
                    if (this.HollowcheckBox.Checked)
                        tempSysConfig.InternalClassifyType |= 0x04;
                    else
                        tempSysConfig.InternalClassifyType &= 0xFB;
                    if (this.SkincheckBox.Checked)
                        tempSysConfig.InternalClassifyType |= 0x08;
                    else
                        tempSysConfig.InternalClassifyType &= 0xF7;
                    if (this.BrowncheckBox.Checked)
                        tempSysConfig.InternalClassifyType |= 0x10;
                    else
                        tempSysConfig.InternalClassifyType &= 0xEF;
                    if (this.TangxincheckBox.Checked)
                        tempSysConfig.InternalClassifyType |= 0x20;
                    else
                        tempSysConfig.InternalClassifyType &= 0xDF;
                    //this.InnerQualityPage.Tag = "True"; //启用内部品质设置界面  Add by ChengSk - 20190114
                    this.InnerQualityPage.Tag = "false";   //屏蔽内部品质设置界面  Modify by xcw - 20200214
                }
                else
                {
                    tempSysConfig.InternalClassifyType = 0;
                    this.InnerQualityPage.Tag = "false";//不启用内部品质设置界面
                }
                //超声波
                if (this.UltrasoniccheckBox.Checked)
                {
                    if (this.HardnesscheckBox.Checked)
                        tempSysConfig.UltrasonicClassifyType |= 0x01;
                    else
                        tempSysConfig.UltrasonicClassifyType &= 0xFE;
                    if (this.WatercheckBox.Checked)
                        tempSysConfig.UltrasonicClassifyType |= 0x02;
                    else
                        tempSysConfig.UltrasonicClassifyType &= 0xFD;
                }
                else
                {
                    tempSysConfig.UltrasonicClassifyType = 0;
                }

                if (tempSysConfig.nExitNum != GlobalDataInterface.globalOut_SysConfig.nExitNum)
                {
                    //出口设置修改后清空出口与等级关系
                    for (int i = 0; i < ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM; i++)
                        GlobalDataInterface.globalOut_GradeInfo.grades[i].exit = (long)0;
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        if (global_IsTest != 0) //add by xcw 20201211
                        {
                            MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                            LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                    }
                    if (tempSysConfig.nExitNum < GlobalDataInterface.globalOut_SysConfig.nExitNum)//出口数减少
                    {
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                        {
                            for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                            {
                                //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                                if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                                {
                                    for (int k = tempSysConfig.nExitNum; k < ConstPreDefine.MAX_EXIT_NUM; k++) //2015-10-08 ivycc
                                    {
                                        //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[k] = new stExitItemInfo(true);
                                        GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[k].nDriverPin = 0;

                                    }
                                }
                            }
                        }
                    }
                    else//出口数增加
                    {
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                        {
                            for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                            {
                                //if (tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                                if (tempSysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                                {
                                    for (int n = GlobalDataInterface.globalOut_SysConfig.nExitNum; n < tempSysConfig.nExitNum; n++)
                                    {
                                        if (n < 0) n = 0;
                                        //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDis = (short)(tempSysConfig.nDataRegistration[i] + 2);
                                        //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nOffset = 0;
                                        GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDriverPin = 0;
                                    }
                                }
                            }
                        }
                    }
                }

                //如果分辨率发生了变化，重置果杯位置，并且重置图像显示框的缓冲区
                if (tempSysConfig.height != GlobalDataInterface.globalOut_SysConfig.height || tempSysConfig.width != GlobalDataInterface.globalOut_SysConfig.width)
                {
                    //对每一个IPM要重新设置
                    List<int> IdArray = new List<int>();
                    Commonfunction.GetAllChannelID(tempSysConfig.nChannelInfo, ref IdArray);
                    int cupHeight = tempSysConfig.height / 10;
                    int offsetUpHeight = tempSysConfig.height / 2 - 10;
                    int offsetDownHeight = tempSysConfig.height / 2 + 10;
                    int cupWidth = tempSysConfig.width / 2;
                    int cupLeft = tempSysConfig.width / 8;
                    int cupNum = 1;
                    for (int i = 0; i < IdArray.Count; i++)
                    {
                        int nSubsysIdx = Commonfunction.GetSubsysIndex(IdArray[i]);
                        int nIPMIdx = Commonfunction.GetIPMIndex(IdArray[i]);
                        //int nChannelIdx = nIPMIdx & 1;
                        //int m_ChannelRangeIPMInSysIndex = (nIPMIdx >> 1);

                        if (GlobalDataInterface.nVer == 0)            //V4.2版本号判断 add by xcw 20200604
                        {
                            for (int j = 0; j < ConstPreDefine.MAX_COLOR_CAMERA_NUM; j++)
                            {
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[Index].nTop = 1;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[Index].nBottom = 1 + cupHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[Index].nLeft[0] = cupLeft;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[Index].nLeft[1] = cupLeft + cupWidth;
                            }
                            for (int j = 0; j < ConstPreDefine.MAX_NIR_CAMERA_NUM; j++)
                            {
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[Index].nTop = 1;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[Index].nBottom = 1 + cupHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[Index].nLeft[0] = cupLeft;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[Index].nLeft[1] = cupLeft + cupWidth;
                            }
                            GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].nCupNum = cupNum;
                        }
                        else if (GlobalDataInterface.nVer == 1)       //V3.2版本
                        {
                            for (int j = 0; j < ConstPreDefine.MAX_COLOR_CAMERA_NUM; j++)
                            {
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[0].nTop = offsetUpHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[0].nBottom = offsetDownHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[0].nLeft[0] = cupLeft;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[0].nLeft[1] = cupLeft + cupWidth;

                                //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[1].nBottom = tempSysConfig.height - 1;
                                //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[1].nTop =
                                //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[1].nBottom - cupHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[1].nTop = tempSysConfig.height + offsetUpHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[1].nBottom = tempSysConfig.height + offsetDownHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[1].nLeft[0] = cupLeft;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[1].nLeft[1] = cupLeft + cupWidth;

                            }
                            for (int j = 0; j < ConstPreDefine.MAX_NIR_CAMERA_NUM; j++)
                            {
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[0].nTop = offsetUpHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[0].nBottom = offsetDownHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[0].nLeft[0] = cupLeft;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[0].nLeft[1] = cupLeft + cupWidth;

                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[1].nTop = tempSysConfig.height + offsetUpHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[1].nBottom = tempSysConfig.height + offsetDownHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[1].nLeft[0] = cupLeft;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[1].nLeft[1] = cupLeft + cupWidth; //add by xcw -20200622
                            }
                            GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].nCupNum = cupNum;
                        }
                        //for (int j = 0; j < ConstPreDefine.MAX_COLOR_CAMERA_NUM; j++)
                        //{
                        //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[Index].nTop = 1;
                        //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[Index].nBottom = 1 + cupHeight;
                        //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[Index].nLeft[0] = cupLeft;
                        //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[Index].nLeft[1] = cupLeft + cupWidth;
                        //}
                        //for (int j = 0; j < ConstPreDefine.MAX_NIR_CAMERA_NUM; j++)
                        //{
                        //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[Index].nTop = 1;
                        //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[Index].nBottom = 1 + cupHeight;
                        //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[Index].nLeft[0] = cupLeft;
                        //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[Index].nLeft[1] = cupLeft + cupWidth;
                        //}
                        //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].nCupNum = cupNum;


                        //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nBottom = tempSysConfig.height-1;
                        //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nTop =
                        //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nBottom - cupHeight;
                        //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nLeft[0] = cupLeft;
                        //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nLeft[1] = cupLeft + cupWidth;

                        //if (GlobalDataInterface.global_IsTestMode)
                        //{
                        //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(nSubsysIdx, nIPMIdx), (int)HC_FSM_COMMAND_TYPE.HC_CMD_PARAS_INFO, null);
                        //}
                    }
                }
                List<int> IdArray1 = new List<int>();
                Commonfunction.GetAllChannelID(tempSysConfig.nChannelInfo, ref IdArray1);
                for (int i = 0; i < IdArray1.Count; i++)
                {
                    int nSubsysIdx = Commonfunction.GetSubsysIndex(IdArray1[i]);
                    int nIPMIdx = Commonfunction.GetIPMIndex(IdArray1[i]);
                    int nChannelIdx = nIPMIdx & 1;
                    int m_ChannelRangeIPMInSysIndex = (nIPMIdx >> 1);
                }
                //出口布局
                if (m_ExitList.Count > 1)
                {
                    ResetExitState();
                }

                GlobalDataInterface.ExitList = new List<ExitState>(m_ExitList.ToArray());
                //显示
                GlobalDataInterface.SystemStructColor = this.ColorcheckBox.Checked;
                GlobalDataInterface.SystemStructShape = this.ShapecheckBox.Checked;
                GlobalDataInterface.SystemStructFlaw = this.FlawcheckBox.Checked;
                GlobalDataInterface.SystemStructVolume = this.VolumecheckBox.Checked;
                GlobalDataInterface.SystemStructProjectedArea = this.ProjectedAreacheckBox.Checked;//
                GlobalDataInterface.SystemStructBruise = this.BruisecheckBox.Checked;
                GlobalDataInterface.SystemStructRot = this.RotcheckBox.Checked;//
                GlobalDataInterface.SystemStructDensity = this.DensitycheckBox.Checked;//
                GlobalDataInterface.SystemStructSugar = this.SugarcheckBox.Checked;
                GlobalDataInterface.SystemStructAcidity = this.AciditycheckBox.Checked;
                GlobalDataInterface.SystemStructHollow = this.HollowcheckBox.Checked;
                GlobalDataInterface.SystemStructSkin = this.SkincheckBox.Checked;
                GlobalDataInterface.SystemStructBrown = this.BrowncheckBox.Checked;
                GlobalDataInterface.SystemStructTangxin = this.TangxincheckBox.Checked;
                GlobalDataInterface.SystemStructRigidity = this.HardnesscheckBox.Checked;
                GlobalDataInterface.SystemStructWater = this.WatercheckBox.Checked;
                GlobalDataInterface.sendBroadcastPackage = this.WifiBroadcastcheckBox.Checked;

                //Commonfunction.SetAppSetting("颜色", GlobalDataInterface.SystemStructColor.ToString());
                //Commonfunction.SetAppSetting("形状", GlobalDataInterface.SystemStructShape.ToString());
                //Commonfunction.SetAppSetting("瑕疵", GlobalDataInterface.SystemStructFlaw.ToString());
                //Commonfunction.SetAppSetting("体积", GlobalDataInterface.SystemStructVolume.ToString());
                //Commonfunction.SetAppSetting("投影面积", GlobalDataInterface.SystemStructProjectedArea.ToString());
                //Commonfunction.SetAppSetting("擦伤", GlobalDataInterface.SystemStructBruise.ToString());
                //Commonfunction.SetAppSetting("腐烂", GlobalDataInterface.SystemStructRot.ToString());
                //Commonfunction.SetAppSetting("密度", GlobalDataInterface.SystemStructDensity.ToString());
                //Commonfunction.SetAppSetting("含糖量", GlobalDataInterface.SystemStructSugar.ToString());
                //Commonfunction.SetAppSetting("酸度", GlobalDataInterface.SystemStructAcidity.ToString());
                //Commonfunction.SetAppSetting("空心", GlobalDataInterface.SystemStructHollow.ToString());
                //Commonfunction.SetAppSetting("浮皮", GlobalDataInterface.SystemStructSkin.ToString());
                //Commonfunction.SetAppSetting("褐变", GlobalDataInterface.SystemStructBrown.ToString());
                //Commonfunction.SetAppSetting("糖心", GlobalDataInterface.SystemStructTangxin.ToString());
                //Commonfunction.SetAppSetting("硬度",GlobalDataInterface.SystemStructRigidity.ToString());
                //Commonfunction.SetAppSetting("含水率", GlobalDataInterface.SystemStructWater.ToString());
                //Commonfunction.SetAppSetting("Wifi功能", GlobalDataInterface.sendBroadcastPackage.ToString());

                if (GlobalDataInterface.sendBroadcastPackage)  //Add by ChengSk - 20190828
                {
                    tempSysConfig.IfWIFIEnable |= 0x01;
                }
                else
                {
                    tempSysConfig.IfWIFIEnable &= 0xFE;
                }

                if (this.SampleOutletcomboBox.Text.Trim() == "")
                    GlobalDataInterface.nSampleOutlet = 0;
                else
                    GlobalDataInterface.nSampleOutlet = int.Parse(this.SampleOutletcomboBox.Text.Trim());
                if (this.SampleNumbertextBox.Text.Trim() == "")
                    GlobalDataInterface.nSampleNumber = 0;
                else
                    //GlobalDataInterface.nSampleNumber = int.Parse(this.SampleNumbertextBox.Text.Trim());
                    GlobalDataInterface.nSampleNumber = tempGradeInfo.nCheckNum;//Add by xcw - 20200525
                tempSysConfig.CheckExit = (byte)GlobalDataInterface.nSampleOutlet;  //Add by ChengSk - 20190828
                tempSysConfig.CheckNum = (byte)GlobalDataInterface.nSampleNumber;   //Add by ChengSk - 20190828
                                                                                    //tempSysConfig.nIQSEnable = GlobalDataInterface.nIQSEnable;          //Add by ChengSk - 20191111

                //倍频功能 2016-8-19
                if (this.MultiFreqcheckBox.Checked)
                {
                    tempSysConfig.multiFreq = 1;
                    //this.EvenShowcheckBox.Enabled = true;
                }
                else
                {
                    tempSysConfig.multiFreq = 0;
                    //this.EvenShowcheckBox.Enabled = false;
                    //this.EvenShowcheckBox.Checked = false;
                }

                bool IsChanged = false;
                //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++) //Modify by ChengSk - 20190521
                {
                    if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] == tempSysConfig.nChannelInfo[i])
                    {
                        IsChanged = false;
                    }
                    else
                    {
                        IsChanged = true;
                        break;
                    }
                }
                if (GlobalDataInterface.globalOut_SysConfig.nClassificationInfo != tempSysConfig.nClassificationInfo)
                    IsChanged = true;
                if (tempSysConfig.nSystemInfo != GlobalDataInterface.globalOut_SysConfig.nSystemInfo)//系统类型改变
                {
                    m_CurrentCameraIndex = 0;//通道选择中相机选择默认值为0
                    m_CurrentCameraLocationIndex = 0;
                }

                //MessageBox.Show("1--" + tempSysConfig.nIQSEnable.ToString());
                GlobalDataInterface.globalOut_SysConfig.ToCopy(tempSysConfig);

                if (IsChanged)
                {
                    tempGradeInfo.ToCopy(GlobalDataInterface.globalOut_GradeInfo);
                    if (!(this.CIRVisioncheckBox.Checked && this.WeighcheckBox.Checked)) //CIR和重量系统只有一个选中时
                    {
                        if (this.CIRVisioncheckBox.Checked) //CIR选中
                        {
                            if ((tempGradeInfo.nClassifyType & 0x03) > 0) //重量
                            {
                                tempGradeInfo.nClassifyType = 0x04;  //重量换成大小（直径）
                            }
                        }
                        else //重量被选中
                        {
                            if ((tempGradeInfo.nClassifyType & 0x1C) > 0)  //大小
                            {
                                tempGradeInfo.nClassifyType = 0x01;  //大小换成重量（克）
                            }
                        }
                    }
                    GlobalDataInterface.globalOut_GradeInfo.ToCopy(tempGradeInfo);
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        if (global_IsTest != 0) //add by xcw 20201211
                        {
                            MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                            LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                    }

                    m_mainForm.SetMainstatusStrip();
                    m_mainForm.SetQaulitytoolStripButtonEnabled();
                    m_mainForm.SetGradeSizelistViewEx();
                    IsChanged = false;
                }
                //MessageBox.Show("2--" + GlobalDataInterface.globalOut_SysConfig.nIQSEnable);
                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_SYS_CONFIG, null);
                    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadSysConfigUpdate++;//系统配置信息更改，平板需更新
                }
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("系统结构设置界面保存配置错误：" + ex);
                //MessageBox.Show("0x10001003 System Struct save error:" + ex,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                MessageBox.Show("0x10001003 " + LanguageContainer.SystemStructMessagebox5Text[GlobalDataInterface.selectLanguageIndex] + ex,
                    LanguageContainer.SystemStructMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        /// <summary>
        /// 保存系统结构配置参数（另存专用，不给FSM发送指令）    Add by ChengSk - 20190116
        /// </summary>
        /// <returns></returns>
        private bool SystemStructSaveConfig2()
        {
            if (tempSysConfig.nExitNum != m_ExitList.Count)
            {
                //MessageBox.Show("当前工作台出口数设置没有达到设定量！");
                //MessageBox.Show("0x10001002 The current number of layout settings exports did not reach the set amount!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("0x10001002 " + LanguageContainer.SystemStructMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.SystemStructMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (GlobalDataInterface.nVer == 0)
            {
                try
                {
                    m_ChanelIDList.Clear();
                    Commonfunction.GetAllChannelID(tempSysConfig.nChannelInfo, ref m_ChanelIDList);
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                    {
                        for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                        {
                            //if (tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] !=
                            //    GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j])
                            if (tempSysConfig.nChannelInfo[i] != GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i])   //Modify by ChengSk - 20190521
                            {
                                //if (tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 0)
                                if (tempSysConfig.nChannelInfo[i] <= j) //Modify by ChengSk - 20190521
                                {
                                    //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].Clear();
                                    //if (GlobalDataInterface.global_IsTestMode)
                                    //{
                                    //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeChannel(i, j, j), (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null);
                                    //}   //Note by ChengSk - 20190116
                                }
                                else
                                {
                                    m_bAlert = true;
                                    for (int n = 0; n < ConstPreDefine.MAX_LABEL_NUM; n++)
                                    {
                                        //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].labelexit[n].nDis = (short)(tempSysConfig.nDataRegistration[i] + 2);
                                        GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].labelexit[n].nDriverPin = 0;
                                    }
                                    for (int n = 0; n < tempSysConfig.nExitNum; n++)
                                    {
                                        //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDis = (short)(tempSysConfig.nDataRegistration[i] + 2);
                                        //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nOffset = 0;
                                        GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDriverPin = 0;
                                    }

                                }
                            }
                        }

                    }
                    if (this.CIRVisioncheckBox.Checked != GlobalDataInterface.CIRAvailable || this.UVVisioncheckBox.Checked != GlobalDataInterface.UVAvailable ||
                        this.WeighcheckBox.Checked != GlobalDataInterface.WeightAvailable || this.InternalcheckBox.Checked != GlobalDataInterface.InternalAvailable ||
                        this.UltrasoniccheckBox.Checked != GlobalDataInterface.UltrasonicAvailable)
                    {
                        if (m_bAlert)
                        {
                            //MessageBox.Show("请到通道出口页面对该通道进行相应设置，并注意修改等级设置--分选标准页面的相应内容", "通道和传感器类型改变", MessageBoxButtons.OK);
                            //MessageBox.Show("0x3001004 Please go to the page of Channel Exit to set the channel settings,and modify the Size Settings,especially the Sorting Standard!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3001004 " + LanguageContainer.SystemStructMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.SystemStructMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            //MessageBox.Show("注意修改等级设置--分选标准页面的相应内容!", "传感器类型改变", MessageBoxButtons.OK);
                            //MessageBox.Show("0x3001002 Please modify the Size Settings,especially the Sorting Standard!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3001002 " + LanguageContainer.SystemStructMessagebox3Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.SystemStructMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        if (m_bAlert)
                        {
                            //MessageBox.Show("0x3001003 Please go to the page of Channel Exit to set the channel settings!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3001003 " + LanguageContainer.SystemStructMessagebox4Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.SystemStructMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    //保存计算机视觉和重量感应功能设置
                    GlobalDataInterface.CIRAvailable = this.CIRVisioncheckBox.Checked;
                    GlobalDataInterface.UVAvailable = this.UVVisioncheckBox.Checked;
                    GlobalDataInterface.WeightAvailable = this.WeighcheckBox.Checked;
                    GlobalDataInterface.InternalAvailable = this.InternalcheckBox.Checked;
                    GlobalDataInterface.UltrasonicAvailable = this.UltrasoniccheckBox.Checked;

                    if (this.WeighcheckBox.Checked)
                        this.WeightSettabPage.Tag = "True";//启用重量设置界面
                    else
                        this.WeightSettabPage.Tag = "False";//不启用重量设置界面

                    //if (this.InternalcheckBox.Checked)      //Add by ChengSk - 20190111
                    //    this.InnerQualityPage.Tag = "True"; //启用内部品质设置界面
                    //else
                    //    this.InnerQualityPage.Tag = "false";//不启用内部品质设置界面

                    //CIR视觉
                    if (this.CIRVisioncheckBox.Checked)
                    {
                        if (this.ColorcheckBox.Checked)
                            tempSysConfig.CIRClassifyType |= 0x01;
                        else
                            tempSysConfig.CIRClassifyType &= 0xFE;
                        if (this.ShapecheckBox.Checked)
                            tempSysConfig.CIRClassifyType |= 0x02;
                        else
                            tempSysConfig.CIRClassifyType &= 0xFD;
                        if (this.FlawcheckBox.Checked)
                            tempSysConfig.CIRClassifyType |= 0x04;
                        else
                            tempSysConfig.CIRClassifyType &= 0xFB;
                        if (this.VolumecheckBox.Checked)          //Add by ChengSk - 20181206
                            tempSysConfig.CIRClassifyType |= 0x08;
                        else
                            tempSysConfig.CIRClassifyType &= 0xF7;
                        if (this.ProjectedAreacheckBox.Checked)
                            tempSysConfig.CIRClassifyType |= 0x10;
                        else
                            tempSysConfig.CIRClassifyType &= 0xEF;//Add End
                        this.ChannelRangePage.Tag = "True";//启用通道范围设置界面


                    }
                    else
                    {
                        tempSysConfig.CIRClassifyType = 0;
                        this.ChannelRangePage.Tag = "False";//不启用通道范围设置界面


                    }
                    //UV视觉
                    if (this.UVVisioncheckBox.Checked)
                    {
                        if (this.BruisecheckBox.Checked)
                            tempSysConfig.UVClassifyType |= 0x01;
                        else
                            tempSysConfig.UVClassifyType &= 0xFE;
                        if (this.RotcheckBox.Checked)
                            tempSysConfig.UVClassifyType |= 0x02;
                        else
                            tempSysConfig.UVClassifyType &= 0xFD;
                    }
                    else
                    {
                        tempSysConfig.UVClassifyType = 0;
                    }

                    //重量系统 - Add by ChengSk 20190828
                    if (this.WeighcheckBox.Checked)
                    {
                        if (this.DensitycheckBox.Checked)
                            tempSysConfig.WeightClassifyTpye |= 0x01;
                        else
                            tempSysConfig.WeightClassifyTpye &= 0xFE;
                    }
                    else
                    {
                        tempSysConfig.WeightClassifyTpye = 0;
                    }

                    //含糖量
                    if (this.InternalcheckBox.Checked)
                    {
                        if (this.SugarcheckBox.Checked)
                            tempSysConfig.InternalClassifyType |= 0x01;
                        else
                            tempSysConfig.InternalClassifyType &= 0xFE;
                        if (this.AciditycheckBox.Checked)
                            tempSysConfig.InternalClassifyType |= 0x02;
                        else
                            tempSysConfig.InternalClassifyType &= 0xFD;
                        if (this.HollowcheckBox.Checked)
                            tempSysConfig.InternalClassifyType |= 0x04;
                        else
                            tempSysConfig.InternalClassifyType &= 0xFB;
                        if (this.SkincheckBox.Checked)
                            tempSysConfig.InternalClassifyType |= 0x08;
                        else
                            tempSysConfig.InternalClassifyType &= 0xF7;
                        if (this.BrowncheckBox.Checked)
                            tempSysConfig.InternalClassifyType |= 0x10;
                        else
                            tempSysConfig.InternalClassifyType &= 0xEF;
                        if (this.TangxincheckBox.Checked)
                            tempSysConfig.InternalClassifyType |= 0x20;
                        else
                            tempSysConfig.InternalClassifyType &= 0xDF;
                        this.InnerQualityPage.Tag = "True"; //启用内部品质设置界面  Add by ChengSk - 20190114
                    }
                    else
                    {
                        tempSysConfig.InternalClassifyType = 0;
                        this.InnerQualityPage.Tag = "false";//不启用内部品质设置界面
                    }
                    //超声波
                    if (this.UltrasoniccheckBox.Checked)
                    {
                        if (this.HardnesscheckBox.Checked)
                            tempSysConfig.UltrasonicClassifyType |= 0x01;
                        else
                            tempSysConfig.UltrasonicClassifyType &= 0xFE;
                        if (this.WatercheckBox.Checked)
                            tempSysConfig.UltrasonicClassifyType |= 0x02;
                        else
                            tempSysConfig.UltrasonicClassifyType &= 0xFD;
                    }
                    else
                    {
                        tempSysConfig.UltrasonicClassifyType = 0;
                    }

                    if (tempSysConfig.nExitNum != GlobalDataInterface.globalOut_SysConfig.nExitNum)
                    {
                        //出口设置修改后清空出口与等级关系
                        for (int i = 0; i < ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM; i++)
                            GlobalDataInterface.globalOut_GradeInfo.grades[i].exit = (long)0;
                        //if (GlobalDataInterface.global_IsTestMode)
                        //{
                        //    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        //    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                        //} //Note by ChengSk - 20190116

                        if (tempSysConfig.nExitNum < GlobalDataInterface.globalOut_SysConfig.nExitNum)//出口数减少
                        {
                            for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                            {
                                for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                                {
                                    //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                                    if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                                    {
                                        for (int k = tempSysConfig.nExitNum; k < ConstPreDefine.MAX_EXIT_NUM; k++) //2015-10-08 ivycc
                                        {
                                            //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[k] = new stExitItemInfo(true);
                                            GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[k].nDriverPin = 0;  //add by xcw 20200902


                                        }
                                    }
                                }
                            }
                        }
                        else//出口数增加
                        {
                            for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                            {
                                for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                                {
                                    //if (tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                                    if (tempSysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                                    {
                                        for (int n = GlobalDataInterface.globalOut_SysConfig.nExitNum; n < tempSysConfig.nExitNum; n++)
                                        {
                                            if (n < 0) n = 0;
                                            //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDis = (short)(tempSysConfig.nDataRegistration[i] + 2);
                                            //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nOffset = 0;
                                            GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDriverPin = 0;
                                        }
                                    }
                                }
                            }
                        }

                    }

                    //如果分辨率发生了变化，重置果杯位置，并且重置图像显示框的缓冲区
                    if (tempSysConfig.height != GlobalDataInterface.globalOut_SysConfig.height || tempSysConfig.width != GlobalDataInterface.globalOut_SysConfig.width)
                    {
                        //对每一个IPM要重新设置
                        List<int> IdArray = new List<int>();
                        Commonfunction.GetAllChannelID(tempSysConfig.nChannelInfo, ref IdArray);
                        int cupHeight = tempSysConfig.height / 10;
                        int offsetUpHeight = tempSysConfig.height / 2 - 10;
                        int offsetDownHeight = tempSysConfig.height / 2 + 10;
                        tempSysConfig.height = int.Parse(this.HeightnumericUpDown.Text);
                        int cupWidth = tempSysConfig.width / 2;
                        int cupLeft = tempSysConfig.width / 8;
                        int cupNum = 1;
                        for (int i = 0; i < IdArray.Count; i++)
                        {
                            int nSubsysIdx = Commonfunction.GetSubsysIndex(IdArray[i]);
                            int nIPMIdx = Commonfunction.GetIPMIndex(IdArray[i]);
                            for (int j = 0; j < ConstPreDefine.MAX_COLOR_CAMERA_NUM; j++)
                            {
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[Index].nTop = 1;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[Index].nBottom = 1 + cupHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[Index].nLeft[0] = cupLeft;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[Index].nLeft[1] = cupLeft + cupWidth;
                            }
                            for (int j = 0; j < ConstPreDefine.MAX_NIR_CAMERA_NUM; j++)
                            {
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[Index].nTop = 1;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[Index].nBottom = 1 + cupHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[Index].nLeft[0] = cupLeft;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[Index].nLeft[1] = cupLeft + cupWidth;
                            }
                            GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].nCupNum = cupNum;


                            //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nBottom = tempSysConfig.height-1;
                            //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nTop =
                            //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nBottom - cupHeight;
                            //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nLeft[0] = cupLeft;
                            //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nLeft[1] = cupLeft + cupWidth;

                            //if (GlobalDataInterface.global_IsTestMode)
                            //{
                            //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(nSubsysIdx, nIPMIdx), (int)HC_FSM_COMMAND_TYPE.HC_CMD_PARAS_INFO, null);
                            //}
                        }
                    }

                    //for (int j = m_ChanelIDList.Count; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    //        {
                    //            for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM; i++)
                    //            {
                    //            //tempExitInfo[0 * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[i].nDis = 0;
                    //            //tempExitInfo[0 * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[i].nOffset = 0;
                    //            GlobalDataInterface.globalOut_ExitInfo[0 * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[i].nDriverPin = 0;
                    //        }
                    //    }
                    //出口布局
                    if (m_ExitList.Count > 1)
                    {
                        ResetExitState();
                    }

                    GlobalDataInterface.ExitList = new List<ExitState>(m_ExitList.ToArray());
                    //显示
                    GlobalDataInterface.SystemStructColor = this.ColorcheckBox.Checked;
                    GlobalDataInterface.SystemStructShape = this.ShapecheckBox.Checked;
                    GlobalDataInterface.SystemStructFlaw = this.FlawcheckBox.Checked;
                    GlobalDataInterface.SystemStructVolume = this.VolumecheckBox.Checked;
                    GlobalDataInterface.SystemStructProjectedArea = this.ProjectedAreacheckBox.Checked;//
                    GlobalDataInterface.SystemStructBruise = this.BruisecheckBox.Checked;
                    GlobalDataInterface.SystemStructRot = this.RotcheckBox.Checked;//
                    GlobalDataInterface.SystemStructDensity = this.DensitycheckBox.Checked;//
                    GlobalDataInterface.SystemStructSugar = this.SugarcheckBox.Checked;
                    GlobalDataInterface.SystemStructAcidity = this.AciditycheckBox.Checked;
                    GlobalDataInterface.SystemStructHollow = this.HollowcheckBox.Checked;
                    GlobalDataInterface.SystemStructSkin = this.SkincheckBox.Checked;
                    GlobalDataInterface.SystemStructBrown = this.BrowncheckBox.Checked;
                    GlobalDataInterface.SystemStructTangxin = this.TangxincheckBox.Checked;
                    GlobalDataInterface.SystemStructRigidity = this.HardnesscheckBox.Checked;
                    GlobalDataInterface.SystemStructWater = this.WatercheckBox.Checked;
                    GlobalDataInterface.sendBroadcastPackage = this.WifiBroadcastcheckBox.Checked;

                    //Commonfunction.SetAppSetting("颜色", GlobalDataInterface.SystemStructColor.ToString());
                    //Commonfunction.SetAppSetting("形状", GlobalDataInterface.SystemStructShape.ToString());
                    //Commonfunction.SetAppSetting("瑕疵", GlobalDataInterface.SystemStructFlaw.ToString());
                    //Commonfunction.SetAppSetting("体积", GlobalDataInterface.SystemStructVolume.ToString());
                    //Commonfunction.SetAppSetting("投影面积", GlobalDataInterface.SystemStructProjectedArea.ToString());
                    //Commonfunction.SetAppSetting("擦伤", GlobalDataInterface.SystemStructBruise.ToString());
                    //Commonfunction.SetAppSetting("腐烂", GlobalDataInterface.SystemStructRot.ToString());
                    //Commonfunction.SetAppSetting("密度", GlobalDataInterface.SystemStructDensity.ToString());
                    //Commonfunction.SetAppSetting("含糖量", GlobalDataInterface.SystemStructSugar.ToString());
                    //Commonfunction.SetAppSetting("酸度", GlobalDataInterface.SystemStructAcidity.ToString());
                    //Commonfunction.SetAppSetting("空心", GlobalDataInterface.SystemStructHollow.ToString());
                    //Commonfunction.SetAppSetting("浮皮", GlobalDataInterface.SystemStructSkin.ToString());
                    //Commonfunction.SetAppSetting("褐变", GlobalDataInterface.SystemStructBrown.ToString());
                    //Commonfunction.SetAppSetting("糖心", GlobalDataInterface.SystemStructTangxin.ToString());
                    //Commonfunction.SetAppSetting("硬度", GlobalDataInterface.SystemStructRigidity.ToString());
                    //Commonfunction.SetAppSetting("含水率", GlobalDataInterface.SystemStructWater.ToString());
                    //Commonfunction.SetAppSetting("Wifi功能", GlobalDataInterface.sendBroadcastPackage.ToString());

                    if (GlobalDataInterface.sendBroadcastPackage)  //Add by ChengSk - 20190828
                    {
                        tempSysConfig.IfWIFIEnable |= 0x01;
                    }
                    else
                    {
                        tempSysConfig.IfWIFIEnable &= 0xFE;
                    }
                    tempSysConfig.CheckExit = (byte)GlobalDataInterface.nSampleOutlet;  //Add by ChengSk - 20190828
                    tempSysConfig.CheckNum = (byte)GlobalDataInterface.nSampleNumber;   //Add by ChengSk - 20190828
                                                                                        //tempSysConfig.nIQSEnable = GlobalDataInterface.nIQSEnable;          //Add by ChengSk - 20191111

                    //倍频功能 2016-8-19
                    if (this.MultiFreqcheckBox.Checked)
                    {
                        tempSysConfig.multiFreq = 1;
                        //this.EvenShowcheckBox.Enabled = true;
                    }
                    else
                    {
                        tempSysConfig.multiFreq = 0;
                        //this.EvenShowcheckBox.Enabled = false;
                        //this.EvenShowcheckBox.Checked = false;
                    }

                    bool IsChanged = false;
                    //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)  //Modify by ChengSk - 20190521
                    {
                        if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] == tempSysConfig.nChannelInfo[i])
                        {
                            IsChanged = false;
                        }
                        else
                        {
                            IsChanged = true;
                            break;
                        }
                    }
                    if (GlobalDataInterface.globalOut_SysConfig.nClassificationInfo != tempSysConfig.nClassificationInfo)
                        IsChanged = true;
                    if (tempSysConfig.nSystemInfo != GlobalDataInterface.globalOut_SysConfig.nSystemInfo)//系统类型改变
                    {
                        m_CurrentCameraIndex = 0;//通道选择中相机选择默认值为0
                        m_CurrentCameraLocationIndex = 0;
                    }
                    GlobalDataInterface.globalOut_SysConfig.ToCopy(tempSysConfig);

                    if (IsChanged)
                    {
                        tempGradeInfo.ToCopy(GlobalDataInterface.globalOut_GradeInfo);
                        if (!(this.CIRVisioncheckBox.Checked && this.WeighcheckBox.Checked)) //CIR和重量系统只有一个选中时
                        {
                            if (this.CIRVisioncheckBox.Checked) //CIR选中
                            {
                                if ((tempGradeInfo.nClassifyType & 0x03) > 0) //重量
                                {
                                    tempGradeInfo.nClassifyType = 0x04;  //重量换成大小（直径）
                                }
                            }
                            else //重量被选中
                            {
                                if ((tempGradeInfo.nClassifyType & 0x1C) > 0)  //大小
                                {
                                    tempGradeInfo.nClassifyType = 0x01;  //大小换成重量（克）
                                }
                            }
                        }
                        GlobalDataInterface.globalOut_GradeInfo.ToCopy(tempGradeInfo);
                        //if (GlobalDataInterface.global_IsTestMode)
                        //{
                        //    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        //    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                        //} //Note by ChengSk - 20190116

                        m_mainForm.SetMainstatusStrip();
                        m_mainForm.SetQaulitytoolStripButtonEnabled();
                        m_mainForm.SetGradeSizelistViewEx();
                        IsChanged = false;
                    }
                    //if (GlobalDataInterface.global_IsTestMode)
                    //{
                    //    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_SYS_CONFIG, null);
                    //    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadSysConfigUpdate++;//系统配置信息更改，平板需更新
                    //} //Note by ChengSk - 20190116
                    return true;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("系统结构设置界面保存配置错误：" + ex);
                    //MessageBox.Show("0x10001003 System Struct save error:" + ex,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    MessageBox.Show("0x10001003 " + LanguageContainer.SystemStructMessagebox5Text[GlobalDataInterface.selectLanguageIndex] + ex,
                        LanguageContainer.SystemStructMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                try
                {
                    m_ChanelIDList.Clear();
                    Commonfunction.GetAllChannelID(tempSysConfig.nChannelInfo, ref m_ChanelIDList);
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                    {
                        for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                        {
                            //if (tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] !=
                            //    GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j])
                            if (tempSysConfig.nChannelInfo[i] != GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i])   //Modify by ChengSk - 20190521
                            {
                                //if (tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 0)
                                if (tempSysConfig.nChannelInfo[i] <= j) //Modify by ChengSk - 20190521
                                {
                                    //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].Clear();
                                    //if (GlobalDataInterface.global_IsTestMode)
                                    //{
                                    //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeChannel(i, j, j), (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null);
                                    //}   //Note by ChengSk - 20190116
                                }
                                else
                                {
                                    m_bAlert = true;
                                    for (int n = 0; n < ConstPreDefine.MAX_LABEL_NUM; n++)
                                    {
                                        //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].labelexit[n].nDis = (short)(tempSysConfig.nDataRegistration[i] + 2);
                                        GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].labelexit[n].nDriverPin = 0;
                                    }
                                    for (int n = 0; n < tempSysConfig.nExitNum; n++)
                                    {
                                        //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDis = (short)(tempSysConfig.nDataRegistration[i] + 2);
                                        //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nOffset = 0;
                                        GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDriverPin = 0;
                                    }

                                }
                            }
                        }

                    }
                    if (this.CIRVisioncheckBox.Checked != GlobalDataInterface.CIRAvailable || this.UVVisioncheckBox.Checked != GlobalDataInterface.UVAvailable ||
                        this.WeighcheckBox.Checked != GlobalDataInterface.WeightAvailable || this.InternalcheckBox.Checked != GlobalDataInterface.InternalAvailable ||
                        this.UltrasoniccheckBox.Checked != GlobalDataInterface.UltrasonicAvailable)
                    {
                        if (m_bAlert)
                        {
                            //MessageBox.Show("请到通道出口页面对该通道进行相应设置，并注意修改等级设置--分选标准页面的相应内容", "通道和传感器类型改变", MessageBoxButtons.OK);
                            //MessageBox.Show("0x3001004 Please go to the page of Channel Exit to set the channel settings,and modify the Size Settings,especially the Sorting Standard!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3001004 " + LanguageContainer.SystemStructMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.SystemStructMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            //MessageBox.Show("注意修改等级设置--分选标准页面的相应内容!", "传感器类型改变", MessageBoxButtons.OK);
                            //MessageBox.Show("0x3001002 Please modify the Size Settings,especially the Sorting Standard!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3001002 " + LanguageContainer.SystemStructMessagebox3Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.SystemStructMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        if (m_bAlert)
                        {
                            //MessageBox.Show("0x3001003 Please go to the page of Channel Exit to set the channel settings!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3001003 " + LanguageContainer.SystemStructMessagebox4Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.SystemStructMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    //保存计算机视觉和重量感应功能设置
                    GlobalDataInterface.CIRAvailable = this.CIRVisioncheckBox.Checked;
                    GlobalDataInterface.UVAvailable = this.UVVisioncheckBox.Checked;
                    GlobalDataInterface.WeightAvailable = this.WeighcheckBox.Checked;
                    GlobalDataInterface.InternalAvailable = this.InternalcheckBox.Checked;
                    GlobalDataInterface.UltrasonicAvailable = this.UltrasoniccheckBox.Checked;

                    if (this.WeighcheckBox.Checked)
                        this.WeightSettabPage.Tag = "True";//启用重量设置界面
                    else
                        this.WeightSettabPage.Tag = "False";//不启用重量设置界面

                    //if (this.InternalcheckBox.Checked)      //Add by ChengSk - 20190111
                    //    this.InnerQualityPage.Tag = "True"; //启用内部品质设置界面
                    //else
                    //    this.InnerQualityPage.Tag = "false";//不启用内部品质设置界面

                    //CIR视觉
                    if (this.CIRVisioncheckBox.Checked)
                    {
                        if (this.ColorcheckBox.Checked)
                            tempSysConfig.CIRClassifyType |= 0x01;
                        else
                            tempSysConfig.CIRClassifyType &= 0xFE;
                        if (this.ShapecheckBox.Checked)
                            tempSysConfig.CIRClassifyType |= 0x02;
                        else
                            tempSysConfig.CIRClassifyType &= 0xFD;
                        if (this.FlawcheckBox.Checked)
                            tempSysConfig.CIRClassifyType |= 0x04;
                        else
                            tempSysConfig.CIRClassifyType &= 0xFB;
                        if (this.VolumecheckBox.Checked)          //Add by ChengSk - 20181206
                            tempSysConfig.CIRClassifyType |= 0x08;
                        else
                            tempSysConfig.CIRClassifyType &= 0xF7;
                        if (this.ProjectedAreacheckBox.Checked)
                            tempSysConfig.CIRClassifyType |= 0x10;
                        else
                            tempSysConfig.CIRClassifyType &= 0xEF;//Add End
                        this.ChannelRangePage.Tag = "True";//启用通道范围设置界面


                    }
                    else
                    {
                        tempSysConfig.CIRClassifyType = 0;
                        this.ChannelRangePage.Tag = "False";//不启用通道范围设置界面


                    }
                    //UV视觉
                    if (this.UVVisioncheckBox.Checked)
                    {
                        if (this.BruisecheckBox.Checked)
                            tempSysConfig.UVClassifyType |= 0x01;
                        else
                            tempSysConfig.UVClassifyType &= 0xFE;
                        if (this.RotcheckBox.Checked)
                            tempSysConfig.UVClassifyType |= 0x02;
                        else
                            tempSysConfig.UVClassifyType &= 0xFD;
                    }
                    else
                    {
                        tempSysConfig.UVClassifyType = 0;
                    }

                    //重量系统 - Add by ChengSk 20190828
                    if (this.WeighcheckBox.Checked)
                    {
                        if (this.DensitycheckBox.Checked)
                            tempSysConfig.WeightClassifyTpye |= 0x01;
                        else
                            tempSysConfig.WeightClassifyTpye &= 0xFE;
                    }
                    else
                    {
                        tempSysConfig.WeightClassifyTpye = 0;
                    }

                    //含糖量
                    if (this.InternalcheckBox.Checked)
                    {
                        if (this.SugarcheckBox.Checked)
                            tempSysConfig.InternalClassifyType |= 0x01;
                        else
                            tempSysConfig.InternalClassifyType &= 0xFE;
                        if (this.AciditycheckBox.Checked)
                            tempSysConfig.InternalClassifyType |= 0x02;
                        else
                            tempSysConfig.InternalClassifyType &= 0xFD;
                        if (this.HollowcheckBox.Checked)
                            tempSysConfig.InternalClassifyType |= 0x04;
                        else
                            tempSysConfig.InternalClassifyType &= 0xFB;
                        if (this.SkincheckBox.Checked)
                            tempSysConfig.InternalClassifyType |= 0x08;
                        else
                            tempSysConfig.InternalClassifyType &= 0xF7;
                        if (this.BrowncheckBox.Checked)
                            tempSysConfig.InternalClassifyType |= 0x10;
                        else
                            tempSysConfig.InternalClassifyType &= 0xEF;
                        if (this.TangxincheckBox.Checked)
                            tempSysConfig.InternalClassifyType |= 0x20;
                        else
                            tempSysConfig.InternalClassifyType &= 0xDF;
                        this.InnerQualityPage.Tag = "True"; //启用内部品质设置界面  Add by ChengSk - 20190114
                    }
                    else
                    {
                        tempSysConfig.InternalClassifyType = 0;
                        this.InnerQualityPage.Tag = "false";//不启用内部品质设置界面
                    }
                    //超声波
                    if (this.UltrasoniccheckBox.Checked)
                    {
                        if (this.HardnesscheckBox.Checked)
                            tempSysConfig.UltrasonicClassifyType |= 0x01;
                        else
                            tempSysConfig.UltrasonicClassifyType &= 0xFE;
                        if (this.WatercheckBox.Checked)
                            tempSysConfig.UltrasonicClassifyType |= 0x02;
                        else
                            tempSysConfig.UltrasonicClassifyType &= 0xFD;
                    }
                    else
                    {
                        tempSysConfig.UltrasonicClassifyType = 0;
                    }

                    if (tempSysConfig.nExitNum != GlobalDataInterface.globalOut_SysConfig.nExitNum)
                    {
                        //出口设置修改后清空出口与等级关系
                        for (int i = 0; i < ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM; i++)
                            GlobalDataInterface.globalOut_GradeInfo.grades[i].exit = (long)0;
                        //if (GlobalDataInterface.global_IsTestMode)
                        //{
                        //    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        //    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                        //} //Note by ChengSk - 20190116
                        if (tempSysConfig.nExitNum < GlobalDataInterface.globalOut_SysConfig.nExitNum)//出口数减少
                        {
                            for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                            {
                                for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                                {
                                    //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                                    if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                                    {
                                        for (int k = tempSysConfig.nExitNum; k < ConstPreDefine.MAX_EXIT_NUM; k++) //2015-10-08 ivycc
                                        {
                                            //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[k] = new stExitItemInfo(true);
                                            GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[k].nDriverPin = 0;  //add by xcw 20200902

                                        }
                                    }
                                }
                            }
                        }
                        else//出口数增加
                        {
                            for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                            {
                                for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                                {
                                    //if (tempSysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                                    if (tempSysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                                    {
                                        for (int n = GlobalDataInterface.globalOut_SysConfig.nExitNum; n < tempSysConfig.nExitNum; n++)
                                        {
                                            if (n < 0) n = 0;
                                            //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDis = (short)(tempSysConfig.nDataRegistration[i] + 2);
                                            //GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nOffset = 0;
                                            GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[n].nDriverPin = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //如果分辨率发生了变化，重置果杯位置，并且重置图像显示框的缓冲区
                    if (tempSysConfig.height != GlobalDataInterface.globalOut_SysConfig.height || tempSysConfig.width != GlobalDataInterface.globalOut_SysConfig.width)
                    {
                        //对每一个IPM要重新设置
                        List<int> IdArray = new List<int>();
                        Commonfunction.GetAllChannelID(tempSysConfig.nChannelInfo, ref IdArray);
                        int cupHeight = tempSysConfig.height / 10;
                        int cupWidth = tempSysConfig.width / 2;
                        int cupLeft = tempSysConfig.width / 8;
                        int cupNum = 1;
                        for (int i = 0; i < IdArray.Count; i++)
                        {
                            int nSubsysIdx = Commonfunction.GetSubsysIndex(IdArray[i]);
                            int nIPMIdx = Commonfunction.GetIPMIndex(IdArray[i]);
                            //int nChannelIdx = nIPMIdx & 1;
                            //int m_ChannelRangeIPMInSysIndex = (nIPMIdx >> 1);
                            for (int j = 0; j < ConstPreDefine.MAX_COLOR_CAMERA_NUM; j++)
                            {
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[0].nTop = 1;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[0].nBottom = 1 + cupHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[0].nLeft[0] = cupLeft;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[0].nLeft[1] = cupLeft + cupWidth;

                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[1].nBottom = tempSysConfig.height - 1;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[1].nTop =
                                    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[1].nBottom - cupHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[1].nLeft[0] = cupLeft;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[j].cup[1].nLeft[1] = cupLeft + cupWidth;

                            }
                            for (int j = 0; j < ConstPreDefine.MAX_NIR_CAMERA_NUM; j++)
                            {
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[0].nTop = 1;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[0].nBottom = 1 + cupHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[0].nLeft[0] = cupLeft;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[0].nLeft[1] = cupLeft + cupWidth;

                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[1].nBottom = tempSysConfig.height - 1;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[1].nTop =
                                    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[1].nBottom - cupHeight;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[1].nLeft[0] = cupLeft;
                                GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].irCameraParas[j].cup[1].nLeft[1] = cupLeft + cupWidth; //add by xcw -20200622
                            }
                            GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].nCupNum = cupNum;
                            //    for (int j = 0; j < ConstPreDefine.MAX_COLOR_CAMERA_NUM; j++)
                            //{
                            //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex].cameraParas[j].cup[nChannelIdx].nTop = 1;
                            //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex].cameraParas[j].cup[nChannelIdx].nBottom = 1 + cupHeight;
                            //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex].cameraParas[j].cup[nChannelIdx].nLeft[0] = cupLeft;
                            //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex].cameraParas[j].cup[nChannelIdx].nLeft[1] = cupLeft + cupWidth;
                            //}
                            //for (int j = 0; j < ConstPreDefine.MAX_NIR_CAMERA_NUM; j++)
                            //{
                            //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex].irCameraParas[j].cup[nChannelIdx].nTop = 1;
                            //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex].irCameraParas[j].cup[nChannelIdx].nBottom = 1 + cupHeight;
                            //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex].irCameraParas[j].cup[nChannelIdx].nLeft[0] = cupLeft;
                            //    GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex].irCameraParas[j].cup[nChannelIdx].nLeft[1] = cupLeft + cupWidth;
                            //}
                            //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex].nCupNum = cupNum;


                            //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nBottom = tempSysConfig.height-1;
                            //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nTop =
                            //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nBottom - cupHeight;
                            //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nLeft[0] = cupLeft;
                            //GlobalDataInterface.globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIPMIdx].cameraParas[nCamIdx].cup[1].nLeft[1] = cupLeft + cupWidth;

                            //if (GlobalDataInterface.global_IsTestMode)
                            //{
                            //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(nSubsysIdx, nIPMIdx), (int)HC_FSM_COMMAND_TYPE.HC_CMD_PARAS_INFO, null);
                            //}
                        }
                    }

                    //出口布局
                    if (m_ExitList.Count > 1)
                    {
                        ResetExitState();
                    }

                    GlobalDataInterface.ExitList = new List<ExitState>(m_ExitList.ToArray());
                    //显示
                    GlobalDataInterface.SystemStructColor = this.ColorcheckBox.Checked;
                    GlobalDataInterface.SystemStructShape = this.ShapecheckBox.Checked;
                    GlobalDataInterface.SystemStructFlaw = this.FlawcheckBox.Checked;
                    GlobalDataInterface.SystemStructVolume = this.VolumecheckBox.Checked;
                    GlobalDataInterface.SystemStructProjectedArea = this.ProjectedAreacheckBox.Checked;//
                    GlobalDataInterface.SystemStructBruise = this.BruisecheckBox.Checked;
                    GlobalDataInterface.SystemStructRot = this.RotcheckBox.Checked;//
                    GlobalDataInterface.SystemStructDensity = this.DensitycheckBox.Checked;//
                    GlobalDataInterface.SystemStructSugar = this.SugarcheckBox.Checked;
                    GlobalDataInterface.SystemStructAcidity = this.AciditycheckBox.Checked;
                    GlobalDataInterface.SystemStructHollow = this.HollowcheckBox.Checked;
                    GlobalDataInterface.SystemStructSkin = this.SkincheckBox.Checked;
                    GlobalDataInterface.SystemStructBrown = this.BrowncheckBox.Checked;
                    GlobalDataInterface.SystemStructTangxin = this.TangxincheckBox.Checked;
                    GlobalDataInterface.SystemStructRigidity = this.HardnesscheckBox.Checked;
                    GlobalDataInterface.SystemStructWater = this.WatercheckBox.Checked;
                    GlobalDataInterface.sendBroadcastPackage = this.WifiBroadcastcheckBox.Checked;

                    //Commonfunction.SetAppSetting("颜色", GlobalDataInterface.SystemStructColor.ToString());
                    //Commonfunction.SetAppSetting("形状", GlobalDataInterface.SystemStructShape.ToString());
                    //Commonfunction.SetAppSetting("瑕疵", GlobalDataInterface.SystemStructFlaw.ToString());
                    //Commonfunction.SetAppSetting("体积", GlobalDataInterface.SystemStructVolume.ToString());
                    //Commonfunction.SetAppSetting("投影面积", GlobalDataInterface.SystemStructProjectedArea.ToString());
                    //Commonfunction.SetAppSetting("擦伤", GlobalDataInterface.SystemStructBruise.ToString());
                    //Commonfunction.SetAppSetting("腐烂", GlobalDataInterface.SystemStructRot.ToString());
                    //Commonfunction.SetAppSetting("密度", GlobalDataInterface.SystemStructDensity.ToString());
                    //Commonfunction.SetAppSetting("含糖量", GlobalDataInterface.SystemStructSugar.ToString());
                    //Commonfunction.SetAppSetting("酸度", GlobalDataInterface.SystemStructAcidity.ToString());
                    //Commonfunction.SetAppSetting("空心", GlobalDataInterface.SystemStructHollow.ToString());
                    //Commonfunction.SetAppSetting("浮皮", GlobalDataInterface.SystemStructSkin.ToString());
                    //Commonfunction.SetAppSetting("褐变", GlobalDataInterface.SystemStructBrown.ToString());
                    //Commonfunction.SetAppSetting("糖心", GlobalDataInterface.SystemStructTangxin.ToString());
                    //Commonfunction.SetAppSetting("硬度", GlobalDataInterface.SystemStructRigidity.ToString());
                    //Commonfunction.SetAppSetting("含水率", GlobalDataInterface.SystemStructWater.ToString());
                    //Commonfunction.SetAppSetting("Wifi功能", GlobalDataInterface.sendBroadcastPackage.ToString());

                    if (GlobalDataInterface.sendBroadcastPackage)  //Add by ChengSk - 20190828
                    {
                        tempSysConfig.IfWIFIEnable |= 0x01;
                    }
                    else
                    {
                        tempSysConfig.IfWIFIEnable &= 0xFE;
                    }
                    tempSysConfig.CheckExit = (byte)GlobalDataInterface.nSampleOutlet;  //Add by ChengSk - 20190828
                    tempSysConfig.CheckNum = (byte)GlobalDataInterface.nSampleNumber;   //Add by ChengSk - 20190828
                                                                                        //tempSysConfig.nIQSEnable = GlobalDataInterface.nIQSEnable;          //Add by ChengSk - 20191111

                    //倍频功能 2016-8-19
                    if (this.MultiFreqcheckBox.Checked)
                    {
                        tempSysConfig.multiFreq = 1;
                        //this.EvenShowcheckBox.Enabled = true;
                    }
                    else
                    {
                        tempSysConfig.multiFreq = 0;
                        //this.EvenShowcheckBox.Enabled = false;
                        //this.EvenShowcheckBox.Checked = false;
                    }

                    bool IsChanged = false;
                    //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)  //Modify by ChengSk - 20190521
                    {
                        if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] == tempSysConfig.nChannelInfo[i])
                        {
                            IsChanged = false;
                        }
                        else
                        {
                            IsChanged = true;
                            break;
                        }
                    }
                    if (GlobalDataInterface.globalOut_SysConfig.nClassificationInfo != tempSysConfig.nClassificationInfo)
                        IsChanged = true;
                    if (tempSysConfig.nSystemInfo != GlobalDataInterface.globalOut_SysConfig.nSystemInfo)//系统类型改变
                    {
                        m_CurrentCameraIndex = 0;//通道选择中相机选择默认值为0
                        m_CurrentCameraLocationIndex = 0;
                    }
                    GlobalDataInterface.globalOut_SysConfig.ToCopy(tempSysConfig);

                    if (IsChanged)
                    {
                        tempGradeInfo.ToCopy(GlobalDataInterface.globalOut_GradeInfo);
                        if (!(this.CIRVisioncheckBox.Checked && this.WeighcheckBox.Checked)) //CIR和重量系统只有一个选中时
                        {
                            if (this.CIRVisioncheckBox.Checked) //CIR选中
                            {
                                if ((tempGradeInfo.nClassifyType & 0x03) > 0) //重量
                                {
                                    tempGradeInfo.nClassifyType = 0x04;  //重量换成大小（直径）
                                }
                            }
                            else //重量被选中
                            {
                                if ((tempGradeInfo.nClassifyType & 0x1C) > 0)  //大小
                                {
                                    tempGradeInfo.nClassifyType = 0x01;  //大小换成重量（克）
                                }
                            }
                        }
                        GlobalDataInterface.globalOut_GradeInfo.ToCopy(tempGradeInfo);
                        //if (GlobalDataInterface.global_IsTestMode)
                        //{
                        //    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        //    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                        //} //Note by ChengSk - 20190116

                        m_mainForm.SetMainstatusStrip();
                        m_mainForm.SetQaulitytoolStripButtonEnabled();
                        m_mainForm.SetGradeSizelistViewEx();
                        IsChanged = false;
                    }
                    //if (GlobalDataInterface.global_IsTestMode)
                    //{
                    //    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_SYS_CONFIG, null);
                    //    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadSysConfigUpdate++;//系统配置信息更改，平板需更新
                    //} //Note by ChengSk - 20190116
                    return true;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("系统结构设置界面保存配置错误：" + ex);
                    //MessageBox.Show("0x10001003 System Struct save error:" + ex,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    MessageBox.Show("0x10001003 " + LanguageContainer.SystemStructMessagebox5Text[GlobalDataInterface.selectLanguageIndex] + ex,
                        LanguageContainer.SystemStructMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }


        }

        /// <summary>
        /// 立即生效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemStructEffectbutton_Click(object sender, EventArgs e)
        {
            if (!SystemStructSaveConfig())
                return;

            //if (this.SampleOutletcomboBox.Text.Trim() == "")
            //    GlobalDataInterface.nSampleOutlet = 0;
            //else
            //    GlobalDataInterface.nSampleOutlet = int.Parse(this.SampleOutletcomboBox.Text.Trim());

            //if (this.SampleNumbertextBox.Text.Trim() == "")
            //    GlobalDataInterface.nSampleNumber = 0;
            //else
            //    GlobalDataInterface.nSampleNumber = int.Parse(this.SampleNumbertextBox.Text.Trim());

            //Commonfunction.SetAppSetting("抽检出口", GlobalDataInterface.nSampleOutlet.ToString());
            //Commonfunction.SetAppSetting("抽检数量", GlobalDataInterface.nSampleNumber.ToString());

            ulong uCurrentSampleExitFruitTotals = 0;  //Add by ChengSk - 20180202
            if (GlobalDataInterface.nSampleOutlet == 0)
            {
                GlobalDataInterface.uCurrentSampleExitFruitTotals = 0;
            }
            else
            {
                for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                {
                    uCurrentSampleExitFruitTotals += GlobalDataInterface.globalIn_statistics[k].nExitCount[GlobalDataInterface.nSampleOutlet - 1];
                }
                GlobalDataInterface.uCurrentSampleExitFruitTotals = uCurrentSampleExitFruitTotals;
            }

            this.SystemStructEffectbutton.Enabled = false;
            //m_tabselectInvalid = false;
            this.EffectButtonDelaytimer1.Enabled = true;

            Reload(1);

            //可以在此处添加内部品质访问的代码
            //逐通道向后获取光谱仪信息（通道1还要获取基础信息）
            //收到通道1的完整信息之后再次刷新界面
            //光谱仪信息通道选择以获取其它通道的信息（若无消息，则显示默认值）
            if (m_ChanelIDList.Count > 0 && GlobalDataInterface.InternalAvailable == true)
            {
                StartReceivedInnerQualityInfo(m_ChanelIDList.Count);  //仅在通道不为零的情况下才接收内部品质信息
            }
        }


        /// <summary>
        /// 立即生效后延迟1.5秒再启用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffectButtonDelaytimer1_Tick(object sender, EventArgs e)
        {
            this.SystemStructEffectbutton.Enabled = true;
            this.EffectButtonDelaytimer1.Enabled = false;
            //m_tabselectInvalid = true;
        }


    }
}
