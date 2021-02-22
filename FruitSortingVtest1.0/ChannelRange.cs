using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using Interface;
using Common;
using System.Diagnostics;

namespace FruitSortingVtest1._0
{
    enum PICTRUE_STATE
    {
        PICTRUE_STATE_NORMAL,
        PICTRUE_STATE_RECTANGLE,//支持拉框矩形
        PICTRUE_STATE_LINE,//支持画线
    };
    public partial class ProjectSetForm : Form
    {
        byte[] m_BottomImageRGB;
        byte[] m_TopImageRGB;
        Bitmap m_BottomImage, m_TopImage;
        private stParas m_paras = new stParas(true);
        private int m_ChannelRangeCurrentChannelIndex = -1;
        private int m_ChannelRangeSubSysIdx = -1;//当前I系统
        private int m_ChannelRangeIPMInSysIndex = -1; //当前IPM
        private int m_ChannelRangeChannelInIPMIndex = -1;//当前属于IPM哪个通道 0 or 1
        private int m_CurrentCameraIndex = 0;//相机类型当前选择 针对控件
        private int m_CurrentCameraLocationIndex = 0;//相机位置当前选择 针对控件
        private int Index = GlobalDataInterface.globalIn_defaultInis.FirstOrDefault().nVersion == 40201 ? 0 : 1;

        private int m_CurrentCameraConfigIndex = 0;//相机类型当前选择 针对配置接口
        private int m_CurrentCameraConfigLocationIndex = 0;//相机位置当前选择 针对配置接口
        public int IDShutterAdjust = 0;//判断通道ID 针对校验开关
        private int m_CameraIndex = 0;//不同系统的相机序号        
        private Rect m_rect = new Rect();


        private Rect m_rectAW;//自动白平衡所选区域
        private bool m_MouseDown = false;

        Point m_ptCheck = new Point();
        int m_ImageWidth = 0;//图像宽
        int m_ImageHeight = 0;//图像高


        private void ChannelRangeInitial()
        {
            try
            {
                if (m_ChanelIDList.Count > 0)
                {
                    this.ChannelRangeChannelcomboBox.Items.Clear();
                    for (int i = 0; i < m_ChanelIDList.Count; i++)
                    {
                        this.ChannelRangeChannelcomboBox.Items.Add(m_resourceManager.GetString("Lanelabel.Text") + string.Format(" {0}", i + 1));
                    }
                    m_ChannelRangeCurrentChannelIndex = 0;
                    this.ChannelRangeChannelcomboBox.SelectedIndex = m_ChannelRangeCurrentChannelIndex;

                    if (GlobalDataInterface.globalOut_SysConfig.multiFreq == 1)//是否启用倍频
                    {
                        this.EvenShowcheckBox.Enabled = true;
                        this.EvenShowcheckBox.Checked = true;
                    }
                    else
                    {
                        this.EvenShowcheckBox.Enabled = false;
                        this.EvenShowcheckBox.Checked = false;
                    }
                    this.CameracomboBox.Items.Clear();
                    this.CameraLocationcomboBox.Items.Clear();
                    switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo)
                    {
                        //case ConstPreDefine.RM_M:
                        case ConstPreDefine.RM2_FM:
                            this.CameracomboBox.Items.Add("Color Camera");
                            this.CameracomboBox.Enabled = false;
                            this.ChannelRangeCupgroupBox.Enabled = true;  //Add by ChengSk - 20190731
                            this.ImageCorrectgroupBox.Enabled = false;    //Add by ChengSk - 20190731
                            this.CameraLocationcomboBox.Items.Add("Middle");
                            this.CameraLocationcomboBox.Enabled = false;
                            this.DetectWhiteThnumericUpDown.Enabled = true;
                            m_ImageHeight = GlobalDataInterface.globalOut_SysConfig.height;
                            m_CurrentCameraIndex = 0;
                            m_CurrentCameraConfigIndex = 0;
                            m_CurrentCameraLocationIndex = 0;  //add by xcw 20200711 针对加载时相机位置不在中间
                            m_CurrentCameraConfigLocationIndex = 0;
                            break;
                        //case ConstPreDefine.RM_LMR:
                        case ConstPreDefine.RM2_FLMR:
                            this.CameracomboBox.Items.Add("Color Camera");
                            this.CameracomboBox.Enabled = false;
                            this.ChannelRangeCupgroupBox.Enabled = true;  //Add by ChengSk - 20190731
                            this.ImageCorrectgroupBox.Enabled = false;    //Add by ChengSk - 20190731
                            this.CameraLocationcomboBox.Items.Add("Middle");
                            this.CameraLocationcomboBox.Items.Add("Left");
                            this.CameraLocationcomboBox.Items.Add("Right");
                            this.CameraLocationcomboBox.Enabled = true;
                            this.DetectWhiteThnumericUpDown.Enabled = true;
                            m_ImageHeight = GlobalDataInterface.globalOut_SysConfig.height * 3;
                            m_CurrentCameraIndex = 0;
                            m_CurrentCameraConfigIndex = 0;
                            m_CurrentCameraLocationIndex = 0;  //add by xcw 20200711 针对加载时相机位置不在中间
                            m_CurrentCameraConfigLocationIndex = 0;

                            break;
                        //case ConstPreDefine.RM_F_M:
                        case ConstPreDefine.RM2_MM_FM:
                            this.CameracomboBox.Items.Add("Color Camera");
                            this.CameracomboBox.Items.Add("NIR-F Camera");
                            this.CameracomboBox.Enabled = true;
                            this.CameraLocationcomboBox.Items.Add("Middle");
                            this.CameraLocationcomboBox.Enabled = false;
                            this.DetectWhiteThnumericUpDown.Enabled = true;
                            m_ImageHeight = GlobalDataInterface.globalOut_SysConfig.height;
                            m_CurrentCameraConfigIndex = (m_CurrentCameraConfigIndex == 0) ? 0 : m_CurrentCameraConfigIndex;
                            m_CurrentCameraIndex = 0;
                            m_CurrentCameraConfigIndex = 0;
                            m_CurrentCameraLocationIndex = 0;  //add by xcw 20200711 针对加载时相机位置不在中间
                            m_CurrentCameraConfigLocationIndex = 0;
                            break;
                        //case ConstPreDefine.RM_LFR_LMR:
                        case ConstPreDefine.RM2_MLMR_FLMR:
                            this.CameracomboBox.Items.Add("Color Camera");
                            this.CameracomboBox.Items.Add("NIR-F Camera");
                            this.CameracomboBox.Enabled = true;
                            this.CameraLocationcomboBox.Items.Add("Middle");
                            this.CameraLocationcomboBox.Items.Add("Left");
                            this.CameraLocationcomboBox.Items.Add("Right");
                            this.CameraLocationcomboBox.Enabled = true;
                            this.DetectWhiteThnumericUpDown.Enabled = true;
                            m_ImageHeight = GlobalDataInterface.globalOut_SysConfig.height * 3;
                            //m_CurrentCameraConfigIndex = 0;
                            m_CurrentCameraIndex = 0;
                            m_CurrentCameraConfigIndex = 0;
                            m_CurrentCameraConfigLocationIndex = 0;
                            m_CurrentCameraConfigIndex = (m_CurrentCameraConfigIndex == 0) ? 0 : m_CurrentCameraConfigIndex;
                            break;
                        //case ConstPreDefine.RM_F_M_B:
                        case ConstPreDefine.RM2_BM_MM_FM:
                            this.CameracomboBox.Items.Add("Color Camera");
                            this.CameracomboBox.Items.Add("NIR-F Camera");
                            this.CameracomboBox.Items.Add("NIR-B Camera");
                            this.CameracomboBox.Enabled = true;
                            this.CameraLocationcomboBox.Items.Add("Middle");
                            this.CameraLocationcomboBox.Enabled = false;
                            m_ImageHeight = GlobalDataInterface.globalOut_SysConfig.height;
                            m_CurrentCameraConfigIndex = (m_CurrentCameraConfigIndex == 0) ? 0 : m_CurrentCameraConfigIndex;
                            break;
                        //case ConstPreDefine.RM_LFR_LMR_LBR:
                        case ConstPreDefine.RM2_BLMR_MLMR_FLMR:
                            this.CameracomboBox.Items.Add("Color Camera");
                            this.CameracomboBox.Items.Add("NIR-F Camera");
                            this.CameracomboBox.Items.Add("NIR-B Camera");
                            this.CameracomboBox.Enabled = true;
                            this.CameraLocationcomboBox.Items.Add("Middle");
                            this.CameraLocationcomboBox.Items.Add("Left");
                            this.CameraLocationcomboBox.Items.Add("Right");
                            this.CameraLocationcomboBox.Enabled = true;
                            m_ImageHeight = GlobalDataInterface.globalOut_SysConfig.height * 3;
                            //m_CurrentCameraConfigIndex = 0;
                            m_CurrentCameraConfigIndex = (m_CurrentCameraConfigIndex == 0) ? 0 : m_CurrentCameraConfigIndex;
                            break;
                        //case ConstPreDefine.RM_LR:
                        case ConstPreDefine.RM2_FLR:
                            this.CameracomboBox.Items.Add("Color Camera");
                            this.CameracomboBox.Enabled = false;
                            this.ChannelRangeCupgroupBox.Enabled = true;  //Add by ChengSk - 20190731
                            this.ImageCorrectgroupBox.Enabled = false;    //Add by ChengSk - 20190731
                            this.CameraLocationcomboBox.Items.Add("Left");
                            this.CameraLocationcomboBox.Items.Add("Right");
                            this.CameraLocationcomboBox.Enabled = true;
                            m_ImageHeight = GlobalDataInterface.globalOut_SysConfig.height;
                            m_CurrentCameraConfigIndex = 0;    //Modify by xcw - 20191129 索引置0
                            break;
                        //case ConstPreDefine.RM_F:
                        case ConstPreDefine.RM2_MM:
                            this.CameracomboBox.Items.Add("NIR-F Camera");
                            this.CameracomboBox.Enabled = false;
                            this.ChannelRangeCupgroupBox.Enabled = true;  //Add by ChengSk - 20190731
                            this.ImageCorrectgroupBox.Enabled = false;    //Add by ChengSk - 20190731
                            this.CameraLocationcomboBox.Items.Add("Middle");
                            this.CameraLocationcomboBox.Enabled = false;
                            m_ImageHeight = GlobalDataInterface.globalOut_SysConfig.height;
                            m_CurrentCameraConfigIndex = 1;
                            break;
                        //case ConstPreDefine.RM_LFR:
                        case ConstPreDefine.RM2_MLMR:
                            this.CameracomboBox.Items.Add("NIR-F Camera");
                            this.CameracomboBox.Enabled = false;
                            this.ChannelRangeCupgroupBox.Enabled = true;  //Add by ChengSk - 20190731
                            this.ImageCorrectgroupBox.Enabled = false;    //Add by ChengSk - 20190731
                            this.CameraLocationcomboBox.Items.Add("Middle");
                            this.CameraLocationcomboBox.Items.Add("Left");
                            this.CameraLocationcomboBox.Items.Add("Right");
                            this.CameraLocationcomboBox.Enabled = true;
                            m_ImageHeight = GlobalDataInterface.globalOut_SysConfig.height * 3;
                            m_CurrentCameraConfigIndex = 1;
                            break;
                        //case ConstPreDefine.RM_F_B:
                        case ConstPreDefine.RM2_BM_MM:
                            this.CameracomboBox.Items.Add("NIR-F Camera");
                            this.CameracomboBox.Items.Add("NIR-B Camera");
                            this.CameracomboBox.Enabled = true;
                            this.CameraLocationcomboBox.Items.Add("Middle");
                            this.CameraLocationcomboBox.Enabled = false;
                            m_ImageHeight = GlobalDataInterface.globalOut_SysConfig.height;
                            m_CurrentCameraConfigIndex = 1;
                            break;
                        //case ConstPreDefine.RM_LFR_LBR:
                        case ConstPreDefine.RM2_BLMR_MLMR:
                            this.CameracomboBox.Items.Add("NIR-F Camera");
                            this.CameracomboBox.Items.Add("NIR-B Camera");
                            this.CameracomboBox.Enabled = true;
                            this.CameraLocationcomboBox.Items.Add("Middle");
                            this.CameraLocationcomboBox.Items.Add("Left");
                            this.CameraLocationcomboBox.Items.Add("Right");
                            this.CameraLocationcomboBox.Enabled = true;
                            m_ImageHeight = GlobalDataInterface.globalOut_SysConfig.height * 3;
                            m_CurrentCameraConfigIndex = 1;
                            break;
                        default: break;
                    }
                    this.CameracomboBox.SelectedIndex = m_CurrentCameraIndex;
                    this.CameraLocationcomboBox.SelectedIndex = m_CurrentCameraLocationIndex;
                    m_CameraIndex = m_CurrentCameraConfigIndex * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex;
                    if (m_ChannelRangeCurrentChannelIndex >= 0)
                        SetParasInfo(m_ChannelRangeCurrentChannelIndex, true);

                    if (this.CameracomboBox.Text.Contains("Color")) //Add by ChengSk - 20191104
                    {
                        this.SemiAutoWhiteBalancecheckBox.Enabled = true;
                        this.AutoWhiteBalancecheckBox.Enabled = true;
                        this.WhiteBalancegroupBox.Enabled = true;
                    }
                    else
                    {
                        this.SemiAutoWhiteBalancecheckBox.Enabled = false;
                        this.AutoWhiteBalancecheckBox.Enabled = false;
                        this.WhiteBalancegroupBox.Enabled = false;
                    }
                }

                m_ImageWidth = GlobalDataInterface.globalOut_SysConfig.width;

                ComputeRECTLimit(m_CurrentCameraLocationIndex, this.CameraLocationcomboBox.Items.Count); //Add by ChengSk - 20190820

                //if (!m_ChannelRangeIntialed)
                //{
                //    this.ContinuousSamplecheckBox.Checked = false;
                //    this.ShowBlobcheckBox.Checked = false;
                //    this.GammacheckBox.Checked = false;
                //    this.GammanumericUpDown.Enabled = false;
                //}
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数ChannelRangeInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ChannelRangeInitial出错" + ex);
#endif
            }



        }

        /// <summary>
        /// 设置下传参数
        /// </summary>
        /// <param name="nCulSel"></param>
        private void SetParasInfo(int nCulSel, bool IsChannelChanged)
        {
            int ChannelInIPMIndex = -1;
            if (GlobalDataInterface.nVer == 0)  //V4.2
            {
                try
                {
                    if (nCulSel >= 0 && nCulSel < m_ChanelIDList.Count)
                    {
                        if (IsChannelChanged)
                        {
                            m_ChannelRangeSubSysIdx = Commonfunction.GetSubsysIndex(m_ChanelIDList[nCulSel]);
                            m_ChannelRangeIPMInSysIndex = Commonfunction.GetIPMIndex(m_ChanelIDList[nCulSel]);
                            ChannelInIPMIndex = Commonfunction.ChanelInIPMIndex(m_ChanelIDList[nCulSel]);
                            m_paras.ToCopy(GlobalDataInterface.globalOut_Paras[m_ChannelRangeSubSysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex]);
                        }
                        int ID = m_ChanelIDList[nCulSel];

                        if (m_CurrentCameraConfigIndex == 0) //彩色相机
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8)
                            //{
                            //    if (m_CurrentCameraIndex == 2)
                            //        index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //    else
                            //        index = m_CurrentCameraIndex + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //}
                            //else
                            //    index = m_CurrentCameraIndex + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;

                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_M || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR)
                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR) //Modify by ChengSk - 20190520                       
                            {
                                this.ImageCorrectioncheckBox.Enabled = false;
                                this.ImageCorrectUpbutton.Enabled = false;
                                this.ImageCorrectDownbutton.Enabled = false;
                                this.ImageCorrectLeftbutton.Enabled = false;
                                this.ImageCorrectRightbutton.Enabled = false;
                                this.ImageCorrectnumericUpDown.Enabled = false;

                                this.CupMoveUpbutton1.Enabled = true;
                                this.CupMoveUpbutton2.Enabled = true;
                                this.CupMoveLeftbutton1.Enabled = true;
                                this.CupMoveLeftbutton2.Enabled = true;
                                this.CupMoveRightbutton1.Enabled = true;
                                this.CupMoveRightbutton2.Enabled = true;
                                this.CupMoveDownbutton1.Enabled = true;
                                this.CupMoveDownbutton2.Enabled = true;
                                this.CupMovenumericUpDown.Enabled = true;
                            }
                            else
                            {
                                this.ImageCorrectioncheckBox.Enabled = true;
                                this.ImageCorrectUpbutton.Enabled = true;
                                this.ImageCorrectDownbutton.Enabled = true;
                                this.ImageCorrectLeftbutton.Enabled = true;
                                this.ImageCorrectRightbutton.Enabled = true;
                                this.ImageCorrectnumericUpDown.Enabled = true;

                                this.CupSeleccomboBox.Enabled = false;
                                this.CupMoveUpbutton1.Enabled = false;
                                this.CupMoveUpbutton2.Enabled = false;
                                this.CupMoveLeftbutton1.Enabled = false;
                                this.CupMoveLeftbutton2.Enabled = false;
                                this.CupMoveRightbutton1.Enabled = false;
                                this.CupMoveRightbutton2.Enabled = false;
                                this.CupMoveDownbutton1.Enabled = false;
                                this.CupMoveDownbutton2.Enabled = false;
                                this.CupMovenumericUpDown.Enabled = false;
                            }

                            this.SemiAutoWhiteBalancecheckBox.Enabled = true;
                            this.AutoWhiteBalancecheckBox.Enabled = true;


                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR) //Modify by ChengSk - 20190520
                            {
                                if (m_CurrentCameraConfigLocationIndex == 0)
                                {
                                    m_rect.Top = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop + GlobalDataInterface.globalOut_SysConfig.height;
                                    m_rect.Bottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom + GlobalDataInterface.globalOut_SysConfig.height;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 2)
                                {
                                    m_rect.Top = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop + GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    m_rect.Bottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom + GlobalDataInterface.globalOut_SysConfig.height * 2;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 1)
                                {
                                    m_rect.Top = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop;
                                    m_rect.Bottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom;
                                }
                                m_rect.Left = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0];
                                m_rect.Right = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1];//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR)
                            {
                                if (m_CurrentCameraConfigLocationIndex == 0)//Modify by xcw - 20200606
                                {
                                    m_rect.Top = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop + GlobalDataInterface.globalOut_SysConfig.height +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY;
                                    m_rect.Bottom = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom + GlobalDataInterface.globalOut_SysConfig.height +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 2)
                                {
                                    m_rect.Top = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop + GlobalDataInterface.globalOut_SysConfig.height * 2 +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY;
                                    m_rect.Bottom = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom + GlobalDataInterface.globalOut_SysConfig.height * 2 +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 1)
                                {
                                    m_rect.Top = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY;
                                    m_rect.Bottom = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY;
                                }
                                m_rect.Left = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] +
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetX;
                                m_rect.Right = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] +
                                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetX;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM)
                            {

                                m_rect.Top = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop;
                                m_rect.Bottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom;
                                m_rect.Left = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0];
                                m_rect.Right = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1];//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM)  //Modify by xcw - 20200828
                            {
                                m_rect.Top = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY;
                                m_rect.Bottom = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY;
                                m_rect.Left = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] +
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetX;
                                m_rect.Right = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] +
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetX;

                            }
                            else
                            {

                                m_rect.Top = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY;
                                m_rect.Bottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY;
                                m_rect.Left = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetX;
                                m_rect.Right = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetX;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            //m_rect.Left = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetX;
                            //m_rect.Right = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetX;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            //m_rect1.Left = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0];
                            //m_rect1.Right = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1];//by 2015-4-17 去掉分隔线，只显示果杯外框
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[Index] = (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[Index] >> 2) << 2;
                            this.OffsettextBox.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[Index].ToString();  //add by xcw 20200918
                            // Modify by xcw 20200918
                            this.TriggerDelaynumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nTriggerDelay.ToString();
                            //this.MaxBrightnumericUpDown.Text = m_paras.nMaxGrayThreshold.ToString();
                            //this.MinBrightnumericUpDown.Text = m_paras.nMinGrayThreshold.ToString();
                            //this.GammanumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fGammaCorrection.ToString();
                            this.WhiteBalanceRnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanR.ToString();
                            this.WhiteBalanceGnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanG.ToString();
                            this.WhiteBalanceBnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanB.ToString();
                            this.ShutternumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter.ToString();
                            if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter < 2)
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter = 2;
                            if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter > 4095)
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter = 4095;
                            this.GammanumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fGammaCorrection.ToString();
                            //this.ShuttertrackBar.Value = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter;
                            this.DetectionThresholdnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nDetectionThreshold[Index].ToString();
                            this.DetectWhiteThnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nDetectWhiteTh[Index].ToString();  //Add by ChengSk - 20190726
                            this.OverlapThresholdnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nXYEdgeBreakTh[Index].ToString();
                            this.OutRangeThresholdnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fFruitCupRangeTh[Index].ToString();
                            this.PixleRatiotextBox.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fPixelRatio[Index].ToString("#0.000");
                        }
                        else//红外相机
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8)
                            //{
                            //    if (m_CurrentCameraIndex == 3)
                            //        index = m_CurrentCameraIndex - 2 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //    else
                            //        index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //}
                            //else
                            //    index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;

                            if (m_CurrentCameraConfigIndex == 1)
                            {
                                this.ImageCorrectioncheckBox.Enabled = false;
                                this.ImageCorrectUpbutton.Enabled = false;
                                this.ImageCorrectDownbutton.Enabled = false;
                                this.ImageCorrectLeftbutton.Enabled = false;
                                this.ImageCorrectRightbutton.Enabled = false;
                                this.ImageCorrectnumericUpDown.Enabled = false;

                                this.CupMoveUpbutton1.Enabled = true;
                                this.CupMoveUpbutton2.Enabled = true;
                                this.CupMoveLeftbutton1.Enabled = true;
                                this.CupMoveLeftbutton2.Enabled = true;
                                this.CupMoveRightbutton1.Enabled = true;
                                this.CupMoveRightbutton2.Enabled = true;
                                this.CupMoveDownbutton1.Enabled = true;
                                this.CupMoveDownbutton2.Enabled = true;
                                this.CupMovenumericUpDown.Enabled = true;
                            }
                            else
                            {
                                this.ImageCorrectioncheckBox.Enabled = true;
                                this.ImageCorrectUpbutton.Enabled = true;
                                this.ImageCorrectDownbutton.Enabled = true;
                                this.ImageCorrectLeftbutton.Enabled = true;
                                this.ImageCorrectRightbutton.Enabled = true;
                                this.ImageCorrectnumericUpDown.Enabled = true;

                                this.CupSeleccomboBox.Enabled = false;
                                this.CupMoveUpbutton1.Enabled = false;
                                this.CupMoveUpbutton2.Enabled = false;
                                this.CupMoveLeftbutton1.Enabled = false;
                                this.CupMoveLeftbutton2.Enabled = false;
                                this.CupMoveRightbutton1.Enabled = false;
                                this.CupMoveRightbutton2.Enabled = false;
                                this.CupMoveDownbutton1.Enabled = false;
                                this.CupMoveDownbutton2.Enabled = false;
                                this.CupMovenumericUpDown.Enabled = false;
                            }

                            this.SemiAutoWhiteBalancecheckBox.Enabled = false;
                            this.AutoWhiteBalancecheckBox.Enabled = false;

                            this.AutoWBStandardRnumericUpDown.Enabled = false;
                            this.AutoWBStandardGnumericUpDown.Enabled = false;
                            this.AutoWBStandardBnumericUpDown.Enabled = false;
                            this.AutoWBGetCoeffbutton.Enabled = false;


                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                            || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                            || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                            || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520
                            {

                                if (m_CurrentCameraConfigLocationIndex == 0)
                                {
                                    m_rect.Top = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop + GlobalDataInterface.globalOut_SysConfig.height;
                                    m_rect.Bottom = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom + GlobalDataInterface.globalOut_SysConfig.height;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 2)
                                {
                                    m_rect.Top = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop + GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    m_rect.Bottom = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom + GlobalDataInterface.globalOut_SysConfig.height * 2;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 1)
                                {
                                    m_rect.Top = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop;
                                    m_rect.Bottom = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom;
                                }
                            }

                            else
                            {
                                m_rect.Top = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop;
                                m_rect.Bottom = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom;
                            }

                            //m_rect.Top = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop;
                            //m_rect.Bottom = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom;
                            m_rect.Left = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0];
                            //m_rect[0].Right = m_paras.cameraParas[m_CameraIndex].cup[0].nLeft[m_paras.nCupNum];
                            m_rect.Right = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1];//by 2015-4-17 去掉分隔线，只显示果杯外框



                            this.TriggerDelaynumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nTriggerDelay.ToString();
                            m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[Index] = (m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[Index] >> 2) << 2;
                            this.OffsettextBox.Text = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[Index].ToString();

                            //this.MaxBrightnumericUpDown.Text = m_paras.nMaxGrayThreshold.ToString();
                            //this.MinBrightnumericUpDown.Text = m_paras.nMinGrayThreshold.ToString();
                            //this.GammanumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fGammaCorrection.ToString();
                            this.ShutternumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter.ToString();
                            if (m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter < 2)
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter = 2;
                            if (m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter > 4095)
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter = 4095;
                            this.GammanumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fGammaCorrection.ToString();

                            //this.ShuttertrackBar.Value = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter;
                            this.DetectionThresholdnumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nIRDetectionThreshold[Index].ToString();
                            this.OverlapThresholdnumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nXYEdgeBreakTh[Index].ToString();
                            this.OutRangeThresholdnumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fFruitCupRangeTh[Index].ToString();
                            this.PixleRatiotextBox.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fPixelRatio[Index].ToString("#0.000");
                        }
                        this.CupSeleccomboBox.SelectedIndex = 0;
                        this.ChannelRangeCupNumnumericUpDown.Text = m_paras.nCupNum.ToString();
                        if (int.Parse(this.ChannelRangeCupNumnumericUpDown.Text) <= 0 || int.Parse(this.ChannelRangeCupNumnumericUpDown.Text) > 10)
                            this.ChannelRangeCupNumnumericUpDown.Text = "10";


                        //this.GainnumericUpDown.Text = m_paras.cameraParas[m_CameraIndex].nGain.ToString();



                        //m_fPixelRatio = m_paras.fPixelRatio;


                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            if (this.ProjecttabControl.SelectedIndex == 1)
                            {
                                GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);
                            }
                        }
                    }
                    else
                    {
                        m_ChannelRangeSubSysIdx = -1;
                        m_ChannelRangeIPMInSysIndex = -1;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数SetParasInfo出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数SetParasInfo出错" + ex);
#endif
                }
            }
            else //V3.2
            {
                try
                {
                    if (nCulSel >= 0 && nCulSel < m_ChanelIDList.Count)
                    {
                        if (IsChannelChanged)
                        {
                            m_ChannelRangeSubSysIdx = Commonfunction.GetSubsysIndex(m_ChanelIDList[nCulSel]);
                            m_ChannelRangeIPMInSysIndex = Commonfunction.GetIPMIndex(m_ChanelIDList[nCulSel]);
                            ChannelInIPMIndex = Commonfunction.ChanelInIPMIndex(m_ChanelIDList[nCulSel]);
                            m_ChannelRangeChannelInIPMIndex = ChannelInIPMIndex % 2;//当前属于IPM哪个通道 0 or 1
                            m_paras.ToCopy(GlobalDataInterface.globalOut_Paras[m_ChannelRangeSubSysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex/*(m_ChannelRangeIPMInSysIndex >> 1)*/]);
                        }
                        int ID = m_ChanelIDList[nCulSel];

                        //this.CameraLocationcomboBox.SelectedIndex = m_CurrentCameraLocationIndex;
                        m_CurrentCameraConfigLocationIndex = m_CurrentCameraLocationIndex;  //Add by ChengSk - 20200709
                        m_CurrentCameraConfigIndex = m_CurrentCameraIndex;//Add by xcw - 20200710
                        if (m_CurrentCameraConfigIndex == 0) //彩色相机
                        {

                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR) //Modify by ChengSk - 20190520                       
                            {
                                this.ImageCorrectioncheckBox.Enabled = false;
                                this.ImageCorrectUpbutton.Enabled = false;
                                this.ImageCorrectDownbutton.Enabled = false;
                                this.ImageCorrectLeftbutton.Enabled = false;
                                this.ImageCorrectRightbutton.Enabled = false;
                                this.ImageCorrectnumericUpDown.Enabled = false;

                                this.CupMoveUpbutton1.Enabled = true;
                                this.CupMoveUpbutton2.Enabled = true;
                                this.CupMoveLeftbutton1.Enabled = true;
                                this.CupMoveLeftbutton2.Enabled = true;
                                this.CupMoveRightbutton1.Enabled = true;
                                this.CupMoveRightbutton2.Enabled = true;
                                this.CupMoveDownbutton1.Enabled = true;
                                this.CupMoveDownbutton2.Enabled = true;
                                this.CupMovenumericUpDown.Enabled = true;
                            }
                            else
                            {
                                this.ImageCorrectioncheckBox.Enabled = true;
                                this.ImageCorrectUpbutton.Enabled = true;
                                this.ImageCorrectDownbutton.Enabled = true;
                                this.ImageCorrectLeftbutton.Enabled = true;
                                this.ImageCorrectRightbutton.Enabled = true;
                                this.ImageCorrectnumericUpDown.Enabled = true;

                                this.CupSeleccomboBox.Enabled = false;
                                this.CupMoveUpbutton1.Enabled = false;
                                this.CupMoveUpbutton2.Enabled = false;
                                this.CupMoveLeftbutton1.Enabled = false;
                                this.CupMoveLeftbutton2.Enabled = false;
                                this.CupMoveRightbutton1.Enabled = false;
                                this.CupMoveRightbutton2.Enabled = false;
                                this.CupMoveDownbutton1.Enabled = false;
                                this.CupMoveDownbutton2.Enabled = false;
                                this.CupMovenumericUpDown.Enabled = false;
                            }

                            this.SemiAutoWhiteBalancecheckBox.Enabled = true;
                            this.AutoWhiteBalancecheckBox.Enabled = true;


                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520
                            {
                                if (m_CurrentCameraConfigLocationIndex == 0)//Modify by xcw - 20200606
                                {
                                    m_rect.Top = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + GlobalDataInterface.globalOut_SysConfig.height +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY;
                                    m_rect.Bottom = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + GlobalDataInterface.globalOut_SysConfig.height +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 2)
                                {
                                    m_rect.Top = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + GlobalDataInterface.globalOut_SysConfig.height * 2 +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY;
                                    m_rect.Bottom = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + GlobalDataInterface.globalOut_SysConfig.height * 2 +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 1)
                                {
                                    m_rect.Top = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY;
                                    m_rect.Bottom = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY;
                                }
                                m_rect.Left = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] +
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX;
                                m_rect.Right = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] +
                                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX;//by 2015-4-17 去掉分隔线，只显示果杯外框

                            }
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR) //Modify by ChengSk - 20190520
                            {
                                if (m_CurrentCameraConfigLocationIndex == 0)//Modify by xcw - 20200606
                                {
                                    m_rect.Top = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + GlobalDataInterface.globalOut_SysConfig.height;
                                    m_rect.Bottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + GlobalDataInterface.globalOut_SysConfig.height;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 2)
                                {
                                    m_rect.Top = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    m_rect.Bottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + GlobalDataInterface.globalOut_SysConfig.height * 2;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 1)
                                {
                                    m_rect.Top = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop;
                                    m_rect.Bottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom;
                                }

                                m_rect.Left = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0];
                                m_rect.Right = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1];//by 2015-4-17 去掉分隔线，只显示果杯外框

                            }
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM)
                            {
                                m_rect.Top = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop;
                                m_rect.Bottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom;
                                m_rect.Left = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0];
                                m_rect.Right = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1];


                            }
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM)
                            {
                                m_rect.Top = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY;
                                m_rect.Bottom = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom +
                                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY;
                                m_rect.Left = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] +
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX;
                                m_rect.Right = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] +
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX;

                            }
                            else
                            {
                                m_rect.Top = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop;
                                m_rect.Bottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom;
                                m_rect.Left = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] +
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX;
                                m_rect.Right = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] +
                                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }



                            this.TriggerDelaynumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nTriggerDelay.ToString();
                            //this.MaxBrightnumericUpDown.Text = m_paras.nMaxGrayThreshold.ToString();
                            //this.MinBrightnumericUpDown.Text = m_paras.nMinGrayThreshold.ToString();
                            //this.GammanumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fGammaCorrection.ToString();
                            //if (m_CurrentCameraConfigLocationIndex == 1)
                            //{
                            //    int ChannelIndex = (ChannelInIPMIndex == 1) ? 0 : 1;
                            //    this.OffsettextBox.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex].ToString();
                            //}
                            //else
                            //{
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex] = (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex] >> 2) << 2;

                            this.OffsettextBox.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex].ToString();
                            //}

                            this.WhiteBalanceRnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanR.ToString();
                            this.WhiteBalanceGnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanG.ToString();
                            this.WhiteBalanceBnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanB.ToString();
                            this.ShutternumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter.ToString();
                            if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter < 2)
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter = 2;
                            if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter > 4095)
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter = 4095;
                            //this.ShuttertrackBar.Value = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter;
                            this.GammanumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fGammaCorrection.ToString();
                            this.DetectionThresholdnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nDetectionThreshold[ChannelInIPMIndex].ToString();
                            this.DetectWhiteThnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nDetectWhiteTh[ChannelInIPMIndex].ToString();  //Add by ChengSk - 20190726
                            this.OverlapThresholdnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nXYEdgeBreakTh[ChannelInIPMIndex].ToString();
                            this.OutRangeThresholdnumericUpDown.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fFruitCupRangeTh[ChannelInIPMIndex].ToString();
                            this.PixleRatiotextBox.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fPixelRatio[ChannelInIPMIndex].ToString("#0.000");
                        }
                        else//红外相机
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8)
                            //{
                            //    if (m_CurrentCameraIndex == 3)
                            //        index = m_CurrentCameraIndex - 2 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //    else
                            //        index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //}
                            //else
                            //    index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;

                            if (m_CurrentCameraConfigIndex == 1)
                            {
                                this.ImageCorrectioncheckBox.Enabled = false;
                                this.ImageCorrectUpbutton.Enabled = false;
                                this.ImageCorrectDownbutton.Enabled = false;
                                this.ImageCorrectLeftbutton.Enabled = false;
                                this.ImageCorrectRightbutton.Enabled = false;
                                this.ImageCorrectnumericUpDown.Enabled = false;

                                this.CupMoveUpbutton1.Enabled = true;
                                this.CupMoveUpbutton2.Enabled = true;
                                this.CupMoveLeftbutton1.Enabled = true;
                                this.CupMoveLeftbutton2.Enabled = true;
                                this.CupMoveRightbutton1.Enabled = true;
                                this.CupMoveRightbutton2.Enabled = true;
                                this.CupMoveDownbutton1.Enabled = true;
                                this.CupMoveDownbutton2.Enabled = true;
                                this.CupMovenumericUpDown.Enabled = true;
                            }
                            else
                            {
                                this.ImageCorrectioncheckBox.Enabled = true;
                                this.ImageCorrectUpbutton.Enabled = true;
                                this.ImageCorrectDownbutton.Enabled = true;
                                this.ImageCorrectLeftbutton.Enabled = true;
                                this.ImageCorrectRightbutton.Enabled = true;
                                this.ImageCorrectnumericUpDown.Enabled = true;

                                this.CupSeleccomboBox.Enabled = false;
                                this.CupMoveUpbutton1.Enabled = false;
                                this.CupMoveUpbutton2.Enabled = false;
                                this.CupMoveLeftbutton1.Enabled = false;
                                this.CupMoveLeftbutton2.Enabled = false;
                                this.CupMoveRightbutton1.Enabled = false;
                                this.CupMoveRightbutton2.Enabled = false;
                                this.CupMoveDownbutton1.Enabled = false;
                                this.CupMoveDownbutton2.Enabled = false;
                                this.CupMovenumericUpDown.Enabled = false;
                            }

                            this.SemiAutoWhiteBalancecheckBox.Enabled = false;
                            this.AutoWhiteBalancecheckBox.Enabled = false;

                            this.AutoWBStandardRnumericUpDown.Enabled = false;
                            this.AutoWBStandardGnumericUpDown.Enabled = false;
                            this.AutoWBStandardBnumericUpDown.Enabled = false;
                            this.AutoWBGetCoeffbutton.Enabled = false;

                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                            || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                            || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                            || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                            || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520
                            {
                                if (m_CurrentCameraConfigLocationIndex == 0)
                                {
                                    m_rect.Top = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + GlobalDataInterface.globalOut_SysConfig.height;
                                    m_rect.Bottom = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + GlobalDataInterface.globalOut_SysConfig.height;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 2)
                                {
                                    m_rect.Top = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    m_rect.Bottom = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + GlobalDataInterface.globalOut_SysConfig.height * 2;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 1)
                                {
                                    m_rect.Top = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop;
                                    m_rect.Bottom = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom;
                                }
                            }
                            else
                            {
                                m_rect.Top = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop;
                                m_rect.Bottom = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom;
                            }

                            //m_rect.Top = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop;
                            //m_rect.Bottom = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom;
                            m_rect.Left = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0];
                            m_rect.Right = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1];//by 2015-4-17 去掉分隔线，只显示果杯外框



                            this.TriggerDelaynumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nTriggerDelay.ToString();
                            //this.MaxBrightnumericUpDown.Text = m_paras.nMaxGrayThreshold.ToString();
                            //this.MinBrightnumericUpDown.Text = m_paras.nMinGrayThreshold.ToString();
                            //this.GammanumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fGammaCorrection.ToString();

                            //if (m_CurrentCameraConfigLocationIndex == 1)
                            //{
                            //    int ChannelIndex = (ChannelInIPMIndex == 1) ? 0 : 1;
                            //    this.OffsettextBox.Text = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex].ToString();
                            //}
                            //else
                            //{
                            m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex] = (m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex] >> 2) << 2;

                            this.OffsettextBox.Text = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex].ToString();
                            //}

                            this.ShutternumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter.ToString();
                            if (m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter < 2)
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter = 2;
                            if (m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter > 4095)
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter = 4095;
                            //this.ShuttertrackBar.Value = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter;
                            this.GammanumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fGammaCorrection.ToString();
                            this.DetectionThresholdnumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nIRDetectionThreshold[ChannelInIPMIndex].ToString();
                            this.OverlapThresholdnumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nXYEdgeBreakTh[ChannelInIPMIndex].ToString();
                            this.OutRangeThresholdnumericUpDown.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fFruitCupRangeTh[ChannelInIPMIndex].ToString();
                            this.PixleRatiotextBox.Text = m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fPixelRatio[ChannelInIPMIndex].ToString("#0.000");
                        }
                        this.CupSeleccomboBox.SelectedIndex = 0;
                        this.ChannelRangeCupNumnumericUpDown.Text = m_paras.nCupNum.ToString();
                        if (int.Parse(this.ChannelRangeCupNumnumericUpDown.Text) <= 0 || int.Parse(this.ChannelRangeCupNumnumericUpDown.Text) > 10)
                            this.ChannelRangeCupNumnumericUpDown.Text = "10";


                        //this.GainnumericUpDown.Text = m_paras.cameraParas[m_CameraIndex].nGain.ToString();





                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            if (this.ProjecttabControl.SelectedIndex == 1)
                            {
                                //if (GlobalDataInterface.nVer == ConstPreDefine.VERSION3)            //版本号判断 add by xcw 20200604
                                //{
                                //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);
                                //}

                                //else if (GlobalDataInterface.nVer == ConstPreDefine.VERSION4)
                                //{
                                //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);
                                //}

                                GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);

                            }
                        }
                    }
                    else
                    {
                        m_ChannelRangeSubSysIdx = -1;
                        m_ChannelRangeIPMInSysIndex = -1;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数SetParasInfo出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数SetParasInfo出错" + ex);
#endif
                }
            }

        }

        /// <summary>
        /// 通道选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelRangeChannelcomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;
                //if (m_CurrentCameraIndex == 0)
                //{
                //    this.ImageCorrectioncheckBox.Enabled = false;
                //    //m_ImageCorrect = false;
                //}
                //else
                //{
                //    this.ImageCorrectioncheckBox.Enabled = true;
                //    // m_ImageCorrect = true;
                //}
                this.ImageCorrectioncheckBox.Checked = false;

                m_ChannelRangeCurrentChannelIndex = comboBox.SelectedIndex;

                this.ImageInfogroupBox.Text = m_resourceManager.GetString("Lanelabel.Text") + string.Format(" {0}", comboBox.SelectedIndex + 1);//2014.3.17 ivycc修改

                CloseTest(); //关闭图像采集与等级数据显示
                CloseWhiteBalance(false);//关闭自动白平衡
                SetParasInfo(comboBox.SelectedIndex, true);
                this.ImagepictureBox.Invalidate();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数ChannelRangeChannelcomboBox_SelectionChangeCommitted出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ChannelRangeChannelcomboBox_SelectionChangeCommitted出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 相机变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameracomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (GlobalDataInterface.nVer == 0)
            {
                try
                {
                    ComboBox comboBox = (ComboBox)sender;
                    this.ImageCorrectioncheckBox.Checked = false;
                    int ChannelInIPMIndex = Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_ChannelRangeCurrentChannelIndex]);
                    // int index = 0;

                    if (m_CurrentCameraConfigIndex == 0) //彩色相机
                    {
                        //WhiteBalanceRnumericUpDown.Enabled = true;
                        //WhiteBalanceGnumericUpDown.Enabled = true;
                        //WhiteBalanceBnumericUpDown.Enabled = true; //Add by xcw - 20191031
                        //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                        //        || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                        //        || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR
                        //        || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR
                        //        || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                        if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                            || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                            || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                            || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520
                        {
                            //if (m_CurrentCameraConfigLocationIndex == 0)
                            //{
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                            //}
                            //if (m_CurrentCameraConfigLocationIndex == 2)
                            //{
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                            //}
                            //if (m_CurrentCameraConfigLocationIndex == 1)
                            //{
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                            //}
                            //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                            //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;//by 2015-4-17 去掉分隔线，只显示果杯外框
                        }
                        else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR)
                        {
                            //if (m_CurrentCameraConfigLocationIndex == 0)
                            //{
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                            //}
                            //if (m_CurrentCameraConfigLocationIndex == 2)
                            //{
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                            //}
                            //if (m_CurrentCameraConfigLocationIndex == 1)
                            //{
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                            //}
                            //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                            //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;//by 2015-4-17 去掉分隔线，只显示果杯外框
                        }
                        else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM)
                        {
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                        }


                        //int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                        //int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                        //m_paras.irCameraParas[index].cup[0].nTop = m_rect[0].Top + y;
                        //m_paras.irCameraParas[index].cup[0].nBottom = m_rect[0].Bottom + y;
                        //m_paras.irCameraParas[index].cup[1].nTop = m_rect[1].Top + y;
                        //m_paras.irCameraParas[index].cup[1].nBottom = m_rect[1].Bottom + y;
                        //m_paras.irCameraParas[index].cup[0].nLeft[0] = m_rect[0].Left + x;
                        //m_paras.irCameraParas[index].cup[0].nLeft[1] = m_rect[0].Right + x;
                        //m_paras.irCameraParas[index].cup[1].nLeft[0] = m_rect[1].Left + x;

                        ////m_paras.irCameraParas[index].cup[1].nLeft[1] = m_rect[1].Right + x;
                        //if (((m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom - m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop) != (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom-m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop))
                        //    || ((m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1] - m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0]) != (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1] - m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0])))
                        //for (int i = 0; i < 2; i++)
                        //{
                        //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.irCameraParas[i* ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop;
                        //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.irCameraParas[i * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom;
                        //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_paras.irCameraParas[i * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0];
                        //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_paras.irCameraParas[i * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1];
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[Index] = int.Parse(this.OffsettextBox.Text);

                        //}
                        //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanR = int.Parse(this.WhiteBalanceRnumericUpDown.Text);
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanG = int.Parse(this.WhiteBalanceGnumericUpDown.Text);
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanB = int.Parse(this.WhiteBalanceBnumericUpDown.Text);
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                    }
                    else//红外相机
                    {
                        //WhiteBalanceRnumericUpDown.Enabled = false; //Add by xcw - 20191031
                        //WhiteBalanceGnumericUpDown.Enabled = false;
                        //WhiteBalanceBnumericUpDown.Enabled = false;

                        //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                        //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                        //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR
                        //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR
                        //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                        //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                        //|| GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                        //|| GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR)
                        if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                           || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                           || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR) //Modify by ChengSk - 20190520
                        {

                            if (m_CurrentCameraConfigLocationIndex == 0)
                            {
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                            }
                            if (m_CurrentCameraConfigLocationIndex == 2)
                            {
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                            }
                            if (m_CurrentCameraConfigLocationIndex == 1)
                            {
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                            }
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;

                        }
                        //else if(GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F||GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F_B )
                        else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BM_MM) //Modify by ChengSk - 20190520                        
                        {
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                        }
                        //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                        else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520                         
                        {
                            if (m_CurrentCameraConfigLocationIndex == 0)
                            {
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                            }
                            if (m_CurrentCameraConfigLocationIndex == 2)
                            {
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                            }
                            if (m_CurrentCameraConfigLocationIndex == 1)
                            {
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                            }
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                        }
                        else
                        {
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                        }

                        if (((m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom - m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop) != (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom - m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop))
                            || ((m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] - m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0]) != (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] - m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0])))
                        {
                            //for (int i = 0; i < 2; i++)
                            //{
                            //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop;
                            //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom;
                            //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0];
                            //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1];

                            // }
                        }
                        if (((m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom - m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop) != (m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom - m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop))
                              || ((m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] - m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0]) != (m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] - m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0])))
                        {
                            //for (int i = 0; i < 2; i++)
                            //{
                            m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop;
                            m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom;
                            m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0];
                            m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1];

                            // }
                        }

                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nROIOffsetY[Index] = int.Parse(this.OffsettextBox.Text);
                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nTriggerDelay = int.Parse(this.TriggerDelaynumericUpDown.Text);
                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                    }
                    switch ((string)comboBox.SelectedItem)
                    {
                        case "Color Camera":
                            m_CurrentCameraConfigIndex = 0;
                            this.ImageCorrectioncheckBox.Enabled = true;
                            this.ImageCorrectgroupBox.Enabled = true;
                            this.ChannelRangeCupgroupBox.Enabled = false;
                            this.DetectWhiteThnumericUpDown.Enabled = true;
                            // this.ImageCorrectgroupBox.Enabled = false;
                            break;
                        case "NIR-F Camera":
                            m_CurrentCameraConfigIndex = 1;
                            this.ImageCorrectioncheckBox.Enabled = false;
                            this.ImageCorrectgroupBox.Enabled = false;
                            this.ChannelRangeCupgroupBox.Enabled = true;
                            // this.ImageCorrectgroupBox.Enabled = true;
                            this.DetectWhiteThnumericUpDown.Enabled = false;
                            break;
                        case "NIR-B Camera":
                            m_CurrentCameraConfigIndex = 2;
                            this.ImageCorrectioncheckBox.Enabled = false;
                            this.ImageCorrectgroupBox.Enabled = false;
                            this.ChannelRangeCupgroupBox.Enabled = true;
                            // this.ImageCorrectgroupBox.Enabled = true;
                            this.DetectWhiteThnumericUpDown.Enabled = false;

                            break;
                        default: break;
                    }
                    m_CameraIndex = m_CurrentCameraConfigIndex * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex;
                    m_CurrentCameraIndex = comboBox.SelectedIndex;
                    CloseTest();
                    CloseWhiteBalance(false);
                    SetParasInfo(m_ChannelRangeCurrentChannelIndex, true);//

                    if (((string)comboBox.SelectedItem).Contains("Color"))  //Add by ChengSk - 20191104
                    {
                        this.SemiAutoWhiteBalancecheckBox.Enabled = true;
                        this.AutoWhiteBalancecheckBox.Enabled = true;
                        this.WhiteBalancegroupBox.Enabled = true;
                    }
                    else
                    {
                        this.SemiAutoWhiteBalancecheckBox.Enabled = false;
                        this.AutoWhiteBalancecheckBox.Enabled = false;
                        this.WhiteBalancegroupBox.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数CameracomboBox_SelectionChangeCommitted出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数CameracomboBox_SelectionChangeCommitted出错" + ex);
#endif
                }
            }
            else
            {
                try
                {
                    ComboBox comboBox = (ComboBox)sender;
                    this.ImageCorrectioncheckBox.Checked = false;
                    int ChannelInIPMIndex = Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_ChannelRangeCurrentChannelIndex]);
                    // int index = 0;

                    if (m_CurrentCameraConfigIndex == 0) //彩色相机
                    {
                        if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                               || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                               || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                               || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520
                        {
                            //if (m_CurrentCameraConfigLocationIndex == 0)
                            //{
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height ;
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height ;
                            //}
                            //if (m_CurrentCameraConfigLocationIndex == 2)
                            //{
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2 ;
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                            //}
                            //if (m_CurrentCameraConfigLocationIndex == 1)
                            //{
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                            //}
                            //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                            //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;//by 2015-4-17 去掉分隔线，只显示果杯外框
                        }


                        else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR) //Modify by ChengSk - 20190520
                        {
                            if (m_CurrentCameraConfigLocationIndex == 0)
                            {
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                            }
                            if (m_CurrentCameraConfigLocationIndex == 2)
                            {
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                            }
                            if (m_CurrentCameraConfigLocationIndex == 1)
                            {
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                            }
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;//by 2015-4-17 去掉分隔线，只显示果杯外框
                        }
                        else if ((GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM))
                        {
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;//by 2015-4-17 去掉分隔线，只显示果杯外框
                        }

                        //if (m_CurrentCameraConfigLocationIndex == 1)
                        //{
                        //    int ChannelIndex = (ChannelInIPMIndex == 1) ? 0 : 1;
                        //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] = int.Parse(this.OffsettextBox.Text);
                        //}
                        //else
                        //{
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex] = int.Parse(this.OffsettextBox.Text);
                        //}

                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanR = int.Parse(this.WhiteBalanceRnumericUpDown.Text);
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanG = int.Parse(this.WhiteBalanceGnumericUpDown.Text);
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanB = int.Parse(this.WhiteBalanceBnumericUpDown.Text);
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                    }
                    else//红外相机
                    {

                        if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                           || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                           || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR) //Modify by ChengSk - 20190520
                        {
                            if (m_CurrentCameraConfigLocationIndex == 0)
                            {
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                            }
                            if (m_CurrentCameraConfigLocationIndex == 2)
                            {
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                            }
                            if (m_CurrentCameraConfigLocationIndex == 1)
                            {
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                            }
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;

                        }
                        //else if(GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F||GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F_B )
                        else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BM_MM) //Modify by ChengSk - 20190520                        
                        {
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                        }
                        //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                        else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520                         
                        {
                            if (m_CurrentCameraConfigLocationIndex == 0)
                            {
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                            }
                            if (m_CurrentCameraConfigLocationIndex == 2)
                            {
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                            }
                            if (m_CurrentCameraConfigLocationIndex == 1)
                            {
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                            }
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                        }
                        else
                        {
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                        }

                        //if (((m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom - m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop) != (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom - m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop))
                        //    || ((m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] - m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0]) != (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] - m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0])))
                        //{
                        //    //for (int i = 0; i < 2; i++)
                        //    //{
                        //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop;
                        //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom;
                        //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0];
                        //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1];

                        //    // }
                        //}
                        //if (((m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom - m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop) != (m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom - m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop))
                        //      || ((m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] - m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0]) != (m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] - m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0])))
                        //{
                        //    //for (int i = 0; i < 2; i++)
                        //    //{
                        //    m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop;
                        //    m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom;
                        //    m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0];
                        //    m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1];

                        //    // }
                        //}

                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex] = int.Parse(this.OffsettextBox.Text);

                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nTriggerDelay = int.Parse(this.TriggerDelaynumericUpDown.Text);
                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                    }
                    switch ((string)comboBox.SelectedItem)
                    {
                        case "Color Camera":
                            m_CurrentCameraConfigIndex = 0;
                            this.ImageCorrectioncheckBox.Enabled = true;
                            this.ImageCorrectgroupBox.Enabled = true;
                            this.ChannelRangeCupgroupBox.Enabled = false;
                            // this.ImageCorrectgroupBox.Enabled = false;
                            this.DetectWhiteThnumericUpDown.Enabled = true;

                            break;
                        case "NIR-F Camera":
                            m_CurrentCameraConfigIndex = 1;
                            this.ImageCorrectioncheckBox.Enabled = false;
                            this.ImageCorrectgroupBox.Enabled = false;
                            this.ChannelRangeCupgroupBox.Enabled = true;
                            // this.ImageCorrectgroupBox.Enabled = true;
                            this.DetectWhiteThnumericUpDown.Enabled = false;

                            break;
                        case "NIR-B Camera":
                            m_CurrentCameraConfigIndex = 2;
                            this.ImageCorrectioncheckBox.Enabled = false;
                            this.ImageCorrectgroupBox.Enabled = false;
                            this.ChannelRangeCupgroupBox.Enabled = true;
                            // this.ImageCorrectgroupBox.Enabled = true;
                            this.DetectWhiteThnumericUpDown.Enabled = false;

                            break;
                        default: break;
                    }
                    m_CameraIndex = m_CurrentCameraConfigIndex * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex;
                    m_CurrentCameraIndex = comboBox.SelectedIndex;
                    CloseTest();
                    CloseWhiteBalance(false);
                    SetParasInfo(m_ChannelRangeCurrentChannelIndex, true);//Add by xcw- 20200628

                    if (((string)comboBox.SelectedItem).Contains("Color"))  //Add by ChengSk - 20191104
                    {
                        this.SemiAutoWhiteBalancecheckBox.Enabled = true;
                        this.AutoWhiteBalancecheckBox.Enabled = true;
                        this.WhiteBalancegroupBox.Enabled = true;
                    }
                    else
                    {
                        this.SemiAutoWhiteBalancecheckBox.Enabled = false;
                        this.AutoWhiteBalancecheckBox.Enabled = false;
                        this.WhiteBalancegroupBox.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数CameracomboBox_SelectionChangeCommitted出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数CameracomboBox_SelectionChangeCommitted出错" + ex);
#endif
                }
            }

        }

        /// <summary>
        /// 相机位置变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraLocationcomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;
                this.ImageCorrectioncheckBox.Checked = false;
                int ChannelInIPMIndex = Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_ChannelRangeCurrentChannelIndex]);
                // int index = 0;

                #region OLD note by ChengSk - 20191129
                //if (m_CurrentCameraConfigIndex == 0) //彩色相机
                //{
                //    //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                //    //         || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                //    //         || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR
                //    //         || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR
                //    //         || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                //    if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                //             || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                //             || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                //             || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                //             || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520
                //    {
                //        if (m_CurrentCameraConfigLocationIndex == 0)
                //        {
                //            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                //            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                //        }
                //        if (m_CurrentCameraConfigLocationIndex == 2)
                //        {
                //            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                //            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                //        }
                //        if (m_CurrentCameraConfigLocationIndex == 1)
                //        {
                //            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top;
                //            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom;
                //        }
                //    }
                //    else
                //    {
                //        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top;
                //        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom;
                //    }
                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_rect.Left;
                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_rect.Right;//by 2015-4-17 去掉分隔线，只显示果杯外框

                //    //int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                //    //int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                //    //m_paras.irCameraParas[index].cup[0].nTop = m_rect[0].Top + y;
                //    //m_paras.irCameraParas[index].cup[0].nBottom = m_rect[0].Bottom + y;
                //    //m_paras.irCameraParas[index].cup[1].nTop = m_rect[1].Top + y;
                //    //m_paras.irCameraParas[index].cup[1].nBottom = m_rect[1].Bottom + y;
                //    //m_paras.irCameraParas[index].cup[0].nLeft[0] = m_rect[0].Left + x;
                //    //m_paras.irCameraParas[index].cup[0].nLeft[1] = m_rect[0].Right + x;
                //    //m_paras.irCameraParas[index].cup[1].nLeft[0] = m_rect[1].Left + x;
                //    //m_paras.irCameraParas[index].cup[1].nLeft[1] = m_rect[1].Right + x;



                //    //for (int i = 0; i < 2; i++)
                //    //{
                //    //    m_paras.irCameraParas[i * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop;
                //    //    m_paras.irCameraParas[i * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom;
                //    //    m_paras.irCameraParas[i * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0];
                //    //    m_paras.irCameraParas[i * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1];
                //    //}

                //    //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanR = int.Parse(this.WhiteBalanceRnumericUpDown.Text);
                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanG = int.Parse(this.WhiteBalanceGnumericUpDown.Text);
                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanB = int.Parse(this.WhiteBalanceBnumericUpDown.Text);
                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                //}
                //else//红外相机
                //{


                //        //int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                //        //int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                //        //m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top + y;
                //        //m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom + y;
                //        //m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_rect.Left + x;
                //        //m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_rect.Right + x;
                //    //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                //    //     || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                //    //     || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR)
                //    if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                //         || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                //         || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR) //Modify by ChengSk - 20190520
                //    {
                //        //if (m_CurrentCameraConfigLocationIndex == 0)
                //        //{
                //        //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                //        //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                //        //}
                //        //if (m_CurrentCameraConfigLocationIndex == 2)
                //        //{
                //        //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                //        //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                //        //}
                //        //if (m_CurrentCameraConfigLocationIndex == 1)
                //        //{
                //        //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y;
                //        //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y;
                //        //}
                //        //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0] + m_ptCheck.X;
                //        //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1] + m_ptCheck.X;
                //         if (m_CurrentCameraConfigLocationIndex == 0)
                //        {
                //            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top- GlobalDataInterface.globalOut_SysConfig.height;
                //            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom  - GlobalDataInterface.globalOut_SysConfig.height;
                //        }
                //        if (m_CurrentCameraConfigLocationIndex == 2)
                //        {
                //            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                //            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom  - GlobalDataInterface.globalOut_SysConfig.height * 2;
                //        }
                //        if (m_CurrentCameraConfigLocationIndex == 1)
                //        {
                //            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top ;
                //            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom ;
                //        }
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_rect.Left;
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_rect.Right;
                //    }
                //    //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F_B)
                //    else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BM_MM) //Modify by ChengSk - 20190520            
                //    {
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top;
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom;
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_rect.Left;
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_rect.Right;
                //    }
                //    //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                //    else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR)  //Modify by ChengSk - 20190520                  
                //    {
                //        if (m_CurrentCameraConfigLocationIndex == 0)
                //        {
                //            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                //            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                //        }
                //        if (m_CurrentCameraConfigLocationIndex == 2)
                //        {
                //            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                //            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                //        }
                //        if (m_CurrentCameraConfigLocationIndex == 1)
                //        {
                //            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top;
                //            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom;
                //        }
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_rect.Left;
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_rect.Right;
                //    }
                //    else
                //    {
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_rect.Top ;
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_rect.Bottom ;
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_rect.Left ;
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_rect.Right;
                //    }

                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nTriggerDelay = int.Parse(this.TriggerDelaynumericUpDown.Text);
                //        //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                //        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);

                //        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop;
                //        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom;
                //        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0];
                //        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1];

                //        m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop;
                //        m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom;
                //        m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0];
                //        m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_paras.irCameraParas[ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1];
                //}
                #endregion

                switch ((string)comboBox.SelectedItem)
                {
                    case "Middle":
                        m_CurrentCameraConfigLocationIndex = 0;
                        break;
                    case "Left":
                        m_CurrentCameraConfigLocationIndex = 1;
                        break;
                    case "Right":
                        m_CurrentCameraConfigLocationIndex = 2;
                        break;
                    default: break;
                }
                m_CameraIndex = m_CurrentCameraConfigIndex * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex;
                m_CurrentCameraLocationIndex = comboBox.SelectedIndex;
                ComputeRECTLimit(m_CurrentCameraLocationIndex, comboBox.Items.Count);  //Add by ChengSk - 20190809
                CloseTest();
                CloseWhiteBalance(false);
                if (GlobalDataInterface.nVer == 0)
                {
                    SetParasInfo(m_ChannelRangeCurrentChannelIndex, false);

                }
                else
                {
                    SetParasInfo(m_ChannelRangeCurrentChannelIndex, true);

                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数CameracomboBox_SelectionChangeCommitted出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数CameracomboBox_SelectionChangeCommitted出错" + ex);
#endif
            }

        }

        int RectLimitTop = 0;    //果杯限制框Top      Add by ChengSk - 20190809
        int RectLimitBottom = 0; //果杯限制框Bottom   Add by ChengSk - 20190809
        private void ComputeRECTLimit(int currentCameraLocationIndex, int comboBoxItemCount) //Add by ChengSk - 20190809
        {
            try
            {
                if (comboBoxItemCount == 1) //相机位置下拉框，仅有Middle
                {
                    RectLimitTop = 1;
                    RectLimitBottom = GlobalDataInterface.globalOut_SysConfig.height;
                    //MessageBox.Show("comboBoxItemCount = 1，进入Middle选项，RectLimitTop=" + RectLimitTop.ToString() + "，RectLimitBottom" + RectLimitBottom.ToString());
                }
                else if (comboBoxItemCount == 2) //相机位置下拉框，有Left、Right
                {
                    if (currentCameraLocationIndex == 0) //Left
                    {
                        RectLimitTop = 1;
                        RectLimitBottom = GlobalDataInterface.globalOut_SysConfig.height;
                        //MessageBox.Show("comboBoxItemCount = 2，进入Middle选项，RectLimitTop=" + RectLimitTop.ToString() + "，RectLimitBottom" + RectLimitBottom.ToString());
                    }
                    else //Right
                    {
                        RectLimitTop = 1 + GlobalDataInterface.globalOut_SysConfig.height;
                        RectLimitBottom = GlobalDataInterface.globalOut_SysConfig.height * 2;
                        //MessageBox.Show("comboBoxItemCount = 2，进入Middle选项，RectLimitTop=" + RectLimitTop.ToString() + "，RectLimitBottom" + RectLimitBottom.ToString());
                    }
                }
                else if (comboBoxItemCount == 3) //相机位置下拉框，有Middle、Left、Right
                {
                    if (currentCameraLocationIndex == 0) //Middle
                    {
                        RectLimitTop = 1 + GlobalDataInterface.globalOut_SysConfig.height;
                        RectLimitBottom = GlobalDataInterface.globalOut_SysConfig.height * 2;
                        //MessageBox.Show("comboBoxItemCount = 3，进入Middle选项，RectLimitTop=" + RectLimitTop.ToString() + "，RectLimitBottom" + RectLimitBottom.ToString());
                    }
                    else if (currentCameraLocationIndex == 1) //Left
                    {
                        RectLimitTop = 1;
                        RectLimitBottom = GlobalDataInterface.globalOut_SysConfig.height;
                        //MessageBox.Show("comboBoxItemCount = 3，进入Left选项，RectLimitTop=" + RectLimitTop.ToString() + "，RectLimitBottom" + RectLimitBottom.ToString());
                    }
                    else //Right
                    {
                        RectLimitTop = 1 + GlobalDataInterface.globalOut_SysConfig.height * 2;
                        RectLimitBottom = GlobalDataInterface.globalOut_SysConfig.height * 3;
                        //MessageBox.Show("comboBoxItemCount = 3，进入Right选项，RectLimitTop=" + RectLimitTop.ToString() + "，RectLimitBottom" + RectLimitBottom.ToString());
                    }
                }
                else //一般不会有这种情况发生
                {
                    MessageBox.Show("进入未知选项，RectLimitTop=" + RectLimitTop.ToString() + "，RectLimitBottom" + RectLimitBottom.ToString());
                }
                RectLimitTop = RectLimitTop - 1;
                RectLimitBottom = RectLimitBottom - 1;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数ComputeRECTLimit出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ComputeRECTLimit出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 关闭图像采集与等级数据显示
        /// </summary>
        void CloseTest()
        {
            if (GlobalDataInterface.nVer == 0)
            {
                try
                {
                    if (m_ChannelRangeSubSysIdx >= 0 && m_ChannelRangeIPMInSysIndex >= 0)
                    {
                        int nDstId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            GlobalDataInterface.TransmitParam(nDstId, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                        }
                        if (this.ContinuousSamplecheckBox.Checked)
                        {
                            if (GlobalDataInterface.global_IsTestMode)
                            {
                                ////修改
                                //int ID = m_ChanelIDList[m_ChannelRangeCurrentChannelIndex]; 
                                //switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo)
                                //{
                                //    case 2://RM100CIR
                                //        m_CameraIndex = m_CurrentCameraIndex;
                                //        break;
                                //    case 4://RM200C
                                //        m_CameraIndex = Commonfunction.ChanelInIPMIndex(ID);
                                //        break;
                                //    case 8://RM200CIR
                                //        m_CameraIndex = Commonfunction.ChanelInIPMIndex(ID) * ConstPreDefine.CHANNEL_NUM + m_CurrentCameraIndex;
                                //        break;
                                //    default:
                                //        m_CameraIndex = 0;
                                //        break;
                                //}

                                //GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_OFF, m_CurrentCameraIndex);
                                GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_OFF, m_CameraIndex);
                            }
                            this.ContinuousSamplecheckBox.Checked = false;
                        }

                        if (this.checkBoxShutterAdjust.Checked)    //Add by ChengSk - 20190708
                        {
                            if (GlobalDataInterface.global_IsTestMode)
                            {
                                GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHUTTER_ADJUST_OFF, null);
                            }
                            this.checkBoxShutterAdjust.Checked = false;
                        }

                        if (this.ShowBlobcheckBox.Checked)
                        {
                            //#if REALEASE

                            //                    GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHOW_BLOB_ON, m_CurrentCameraIndex);
                            //#endif
                            this.ShowBlobcheckBox.Checked = false;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数CloseTest出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数CloseTest出错" + ex);
#endif
                }
            }
            else
            {
                try
                {
                    if (m_ChannelRangeSubSysIdx >= 0 && m_ChannelRangeIPMInSysIndex >= 0)
                    {
                        int nDstId = 0;
                        //if (GlobalDataInterface.globalIn_systemInitConfig.Version == ConstPreDefine.VERSION4)
                        //{
                        //    nDstId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                        //}
                        //else if(GlobalDataInterface.globalIn_systemInitConfig.Version == ConstPreDefine.VERSION3)
                        //{
                        //    nDstId = Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                        //}

                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            GlobalDataInterface.TransmitParam(nDstId, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                        }
                        if (this.ContinuousSamplecheckBox.Checked)
                        {
                            if (GlobalDataInterface.global_IsTestMode)
                            {
                                ////修改
                                //int ID = m_ChanelIDList[m_ChannelRangeCurrentChannelIndex]; 
                                //switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo)
                                //{
                                //    case 2://RM100CIR
                                //        m_CameraIndex = m_CurrentCameraIndex;
                                //        break;
                                //    case 4://RM200C
                                //        m_CameraIndex = Commonfunction.ChanelInIPMIndex(ID);
                                //        break;
                                //    case 8://RM200CIR
                                //        m_CameraIndex = Commonfunction.ChanelInIPMIndex(ID) * ConstPreDefine.CHANNEL_NUM + m_CurrentCameraIndex;
                                //        break;
                                //    default:
                                //        m_CameraIndex = 0;
                                //        break;
                                //}

                                //GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_OFF, m_CurrentCameraIndex);
                                GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_OFF, m_CameraIndex);
                            }
                            this.ContinuousSamplecheckBox.Checked = false;
                        }

                        if (this.checkBoxShutterAdjust.Checked)    //Add by ChengSk - 20190708
                        {
                            if (GlobalDataInterface.global_IsTestMode)
                            {
                                GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHUTTER_ADJUST_OFF, null);
                            }
                            this.checkBoxShutterAdjust.Checked = false;
                        }

                        if (this.ShowBlobcheckBox.Checked)
                        {
                            //#if REALEASE

                            //                    GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHOW_BLOB_ON, m_CurrentCameraIndex);
                            //#endif
                            this.ShowBlobcheckBox.Checked = false;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数CloseTest出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数CloseTest出错" + ex);
#endif
                }
            }

        }

        /// <summary>
        /// 半自动白平衡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SemiAutoWhiteBalancecheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    if (this.AutoWhiteBalancecheckBox.Checked)
                        this.AutoWhiteBalancecheckBox.Checked = false;
                    this.AutoWBStandardRnumericUpDown.Enabled = true;
                    this.AutoWBStandardGnumericUpDown.Enabled = true;
                    this.AutoWBStandardBnumericUpDown.Enabled = true;
                    this.AutoWBGetCoeffbutton.Enabled = true;
                    this.AutoWBCoeffRtextBox.Enabled = true;
                    this.AutoWBCoeffGtextBox.Enabled = true;
                    this.AutoWBCoeffBtextBox.Enabled = true;
                    this.ImagepictureBox.Invalidate();
                    //m_PictrueState = PICTRUE_STATE.PICTRUE_STATE_RECTANGLE;
                }
                else
                {
                    CloseWhiteBalance(true);
                    this.ImagepictureBox.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数SemiAutoWhiteBalancecheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数SemiAutoWhiteBalancecheckBox_CheckedChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 快门调节开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxShutterAdjust_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                int nDstId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                {
                    //nDstId = Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeChannelInIPMIndex);
                    nDstId = Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeChannelInIPMIndex + m_ChannelRangeIPMInSysIndex * ConstPreDefine.CHANNEL_NUM);
                }

                else if (GlobalDataInterface.nVer == 0)
                {
                    nDstId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                }
                //int nDstId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                if (checkBox.Checked)
                {
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHUTTER_ADJUST_ON, null);
                    }
                }
                else
                {
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHUTTER_ADJUST_OFF, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数checkBoxShutterAdjust_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数checkBoxShutterAdjust_CheckedChanged出错" + ex);
#endif
            }




        }

        /// <summary>
        /// 自动白平衡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoWhiteBalancecheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    if (this.SemiAutoWhiteBalancecheckBox.Checked)
                        this.SemiAutoWhiteBalancecheckBox.Checked = false;
                    this.AutoWBStandardRnumericUpDown.Enabled = true;
                    this.AutoWBStandardGnumericUpDown.Enabled = true;
                    this.AutoWBStandardBnumericUpDown.Enabled = true;
                    this.AutoWBGetCoeffbutton.Enabled = true;
                    this.AutoWBCoeffRtextBox.Enabled = true;
                    this.AutoWBCoeffGtextBox.Enabled = true;
                    this.AutoWBCoeffBtextBox.Enabled = true;
                    this.ImagepictureBox.Invalidate();
                    //m_PictrueState = PICTRUE_STATE.PICTRUE_STATE_RECTANGLE;
                }
                else
                {
                    CloseWhiteBalance(true);
                    this.ImagepictureBox.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数AutoWhiteBalancecheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数AutoWhiteBalancecheckBox_CheckedChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 关闭自动白平衡
        /// </summary>
        /// <param name="IsNetSend"></param>
        void CloseWhiteBalance(bool IsNetSend)
        {
            try
            {
                this.AutoWBStandardRnumericUpDown.Enabled = false;
                this.AutoWBStandardGnumericUpDown.Enabled = false;
                this.AutoWBStandardBnumericUpDown.Enabled = false;
                this.AutoWBGetCoeffbutton.Enabled = false;
                this.AutoWBCoeffRtextBox.Enabled = false;
                this.AutoWBCoeffGtextBox.Enabled = false;
                this.AutoWBCoeffBtextBox.Enabled = false;

                //this.SemiAutoWhiteBalancecheckBox.Checked = false;
                //this.AutoWhiteBalancecheckBox.Checked = false;

                if (!IsNetSend)
                {
                    this.SemiAutoWhiteBalancecheckBox.Checked = false;
                    this.AutoWhiteBalancecheckBox.Checked = false;
                }
            }
            //m_PictrueState = PICTRUE_STATE.PICTRUE_STATE_LINE;
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数CloseWhiteBalance出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数CloseWhiteBalance出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 获取自动白平衡系数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoWBGetCoeffbutton_Click(object sender, EventArgs e)
        {
            try
            {
                Rect DestRect;
                // Rectangle rc  =new Rectangle();
                // this.ImagepictureBox.RectangleToClient(rc);
                DestRect.Left = m_rectAW.Left * m_ImageWidth / this.ImagepictureBox.Width;
                DestRect.Right = m_rectAW.Right * m_ImageWidth / this.ImagepictureBox.Width;
                DestRect.Top = m_rectAW.Top * m_ImageHeight / this.ImagepictureBox.Height;
                DestRect.Bottom = m_rectAW.Bottom * m_ImageHeight / this.ImagepictureBox.Height;
                if (m_rectAW.Left == m_rectAW.Right || DestRect.Top == DestRect.Bottom)
                {
                    //MessageBox.Show("宽度或高度为0！");
                    //MessageBox.Show("The height or width of image cannot be zero!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    MessageBox.Show(LanguageContainer.ChannelRangeMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (m_rectAW.Right-m_rectAW.Left<64  ||DestRect.Bottom - DestRect.Top <32)
                {
                    //MessageBox.Show("宽度或高度为0！");
                    //MessageBox.Show("The height or width of image cannot be zero!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    MessageBox.Show(LanguageContainer.ChannelRangeMessagebox6Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }//add by xcw 20201216
                stWhiteBalanceParam wbp = new stWhiteBalanceParam(true);
                wbp.minx = DestRect.Left;
                wbp.maxx = DestRect.Right;
                //wbp.miny = DestRect.Top;
                //wbp.maxy = DestRect.Bottom;
                wbp.miny = DestRect.Top % GlobalDataInterface.globalOut_SysConfig.height;    //Modify by ChengSk - 20190809
                wbp.maxy = DestRect.Bottom % GlobalDataInterface.globalOut_SysConfig.height; //Modify by ChengSk - 20180809
                //MessageBox.Show("DestRect.Left="+ DestRect.Left+ " DestRect.Right="+ DestRect.Right + " DestRect.Top=" + DestRect.Top + " DestRect.Bottom=" + DestRect.Bottom + " GlobalDataInterface.globalOut_SysConfig.height=" +
                //    GlobalDataInterface.globalOut_SysConfig.height);
                wbp.BGR.bR = byte.Parse(this.AutoWBStandardRnumericUpDown.Text);
                wbp.BGR.bG = byte.Parse(this.AutoWBStandardGnumericUpDown.Text);
                wbp.BGR.bB = byte.Parse(this.AutoWBStandardBnumericUpDown.Text);
                wbp.WhiteBalanceCameraId = (byte)m_CameraIndex;  //自动白平衡相机ID号 0~7

                if (GlobalDataInterface.global_IsTestMode)
                {
                    int nDrcId = 0;
                    if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                    {
                        nDrcId = Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex * ConstPreDefine.CHANNEL_NUM);
                    }

                    else if (GlobalDataInterface.nVer == 0)
                    {
                        nDrcId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                    }
                    if (this.SemiAutoWhiteBalancecheckBox.Checked)
                        GlobalDataInterface.TransmitParam(nDrcId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_AUTOBALANCE_ON, wbp);
                    else if (this.AutoWhiteBalancecheckBox.Checked)
                        GlobalDataInterface.TransmitParam(nDrcId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_AUTOBALANCE_ON_CAMERA, wbp);
                    else
                    { }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数AutoWBGetCoeffbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数AutoWBGetCoeffbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// gamma校正
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GammacheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    //this.GammanumericUpDown.Enabled = true;
                }
                else
                {
                    //this.GammanumericUpDown.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数GammacheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数GammacheckBox_CheckedChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 快门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShuttertrackBar_Scroll(object sender, EventArgs e)
        {
            try
            {
                TrackBar trackBar = (TrackBar)sender;
                this.ShutternumericUpDown.Text = trackBar.Value.ToString();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数ShuttertrackBar_Scroll出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ShuttertrackBar_Scroll出错" + ex);
#endif
            }

        }

        /// <summary>
        /// 图像校对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageCorrectioncheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.ImageCorrectioncheckBox.Enabled = true;
                    this.ImageCorrectUpbutton.Enabled = true;
                    this.ImageCorrectDownbutton.Enabled = true;
                    this.ImageCorrectLeftbutton.Enabled = true;
                    this.ImageCorrectRightbutton.Enabled = true;
                    this.ImageCorrectnumericUpDown.Enabled = true;
                }
                else
                {
                    this.ImageCorrectioncheckBox.Enabled = true;
                    this.ImageCorrectUpbutton.Enabled = false;
                    this.ImageCorrectDownbutton.Enabled = false;
                    this.ImageCorrectLeftbutton.Enabled = false;
                    this.ImageCorrectRightbutton.Enabled = false;
                    this.ImageCorrectnumericUpDown.Enabled = false;
                }
                this.ImagepictureBox.Invalidate();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数ImageCorrectioncheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ImageCorrectioncheckBox_CheckedChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 果杯数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelRangeCupNumnumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.ImagepictureBox.Invalidate();
        }

        /// <summary>
        /// 图像重绘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagepictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (GlobalDataInterface.nVer == 0)
            {
                try
                {
                    Graphics graphics = e.Graphics;


                    GCHandle handle;
                    IntPtr scan;
                    // Bitmap image = null;

                    int width = m_ImageWidth;
                    int height = m_ImageHeight;
                    int stride = width * 4;
                    if (m_BottomImageRGB.Length != (width * height * 4))
                        return;//2015-12-4 ivycc修改 解决图像没改变，尺寸变动导致内存崩溃
                    handle = GCHandle.Alloc(m_BottomImageRGB, GCHandleType.Pinned);
                    scan = handle.AddrOfPinnedObject();
                    //scan += (height - 1) * stride;
                    if (width != 0 && height != 0)
                    {
                        m_BottomImage = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppRgb, scan);
                        //this.ImagepictureBox.Image = image;
                        graphics.DrawImage(m_BottomImage, 0, 0, this.ImagepictureBox.Width, this.ImagepictureBox.Height);

                    }
                    handle.Free();
                    //}

                    Pen pen;

                    //if (m_CurrentCameraIndex == 1 || (m_CurrentCameraIndex == 0 &&(GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_M))) //Note by ChengSk - 20190520
                    if (m_CurrentCameraIndex == 1 || (m_CurrentCameraIndex == 0 && (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM)))  //Modify by ChengSk - 20190520
                    {
                        Rectangle rectangle = new Rectangle();
                        pen = new Pen(Color.Lime, 2);
                        rectangle.X = (int)(m_rect.Left * m_fTransferRatio);
                        rectangle.Width = (int)((m_rect.Right - m_rect.Left) * m_fTransferRatio);
                        rectangle.Y = (int)(m_rect.Top * m_fTransferRatio);
                        rectangle.Height = (int)((m_rect.Bottom - m_rect.Top) * m_fTransferRatio);
                        graphics.DrawRectangle(pen, rectangle);
                        //rectangle[1].X = (int)(m_rect[1].Left * m_fTransferRatio);
                        //rectangle[1].Width = (int)((m_rect[1].Right - m_rect[1].Left) * m_fTransferRatio);
                        //rectangle[1].Y = (int)(m_rect[1].Top * m_fTransferRatio);
                        //rectangle[1].Height = (int)((m_rect[1].Bottom - m_rect[1].Top) * m_fTransferRatio);
                        //graphics.DrawRectangle(pen, rectangle[1]);

                        int nDivideCnt = int.Parse(this.ChannelRangeCupNumnumericUpDown.Text);

                        Point pt1;
                        pt1 = new Point();
                        pen = new Pen(Color.Yellow, 1);


                        for (int i = 0; i < nDivideCnt; i++)
                        {
                            pt1.X = (int)(rectangle.X + rectangle.Width / nDivideCnt / 2.0 + rectangle.Width / nDivideCnt * 1.0 * (i));
                            pt1.Y = rectangle.Y + rectangle.Height / 2;
                            graphics.DrawEllipse(pen, pt1.X, pt1.Y, 8, 8);

                            //pt1.X = (int)(rectangle[1].X + rectangle[1].Width / nDivideCnt / 2.0 + rectangle[1].Width / nDivideCnt * 1.0 * (i));
                            //pt1.Y = rectangle[1].Y + rectangle[1].Height / 2;
                            //graphics.DrawEllipse(pen, pt1.X, pt1.Y, 8, 8);
                        }
                    }
                    else
                    {
                        if (this.ImageCorrectioncheckBox.Checked)
                        {
                            Rectangle rectangle = new Rectangle();
                            pen = new Pen(Color.Lime, 2);
                            //rectangle.X = (int)((m_paras.cameraParas[m_CurrentCameraLocationIndex].cup.nLeft[0] + m_ptCheck.X) * m_fTransferRatio);
                            //rectangle.Width = (int)((m_paras.cameraParas[m_CurrentCameraLocationIndex].cup.nLeft[1] - m_paras.cameraParas[m_CurrentCameraLocationIndex].cup.nLeft[0]) * m_fTransferRatio);
                            //rectangle.Y = (int)((m_paras.cameraParas[m_CurrentCameraLocationIndex].cup.nTop + m_ptCheck.Y)* m_fTransferRatio);
                            //rectangle.Height = (int)((m_paras.cameraParas[m_CurrentCameraLocationIndex].cup.nBottom - m_paras.cameraParas[m_CurrentCameraLocationIndex].cup.nTop) * m_fTransferRatio);
                            //rectangle.X = (int)(m_rect.Left * m_fTransferRatio);
                            //rectangle.Width = (int)((m_rect.Right - m_rect.Left) * m_fTransferRatio);
                            //rectangle.Y = (int)(m_rect.Top+ * m_fTransferRatio);
                            //rectangle.Height = (int)((m_rect.Bottom - m_rect.Top) * m_fTransferRatio);
                            m_rect.Left += m_ptCheck.X;
                            m_rect.Right += m_ptCheck.X;
                            m_rect.Top += m_ptCheck.Y;
                            m_rect.Bottom += m_ptCheck.Y;
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetX +=
                                m_ptCheck.X;
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY +=
                                m_ptCheck.Y;
                            m_ptCheck.X = 0;
                            m_ptCheck.Y = 0;
                            rectangle.X = (int)(m_rect.Left * m_fTransferRatio);
                            rectangle.Width = (int)((m_rect.Right - m_rect.Left) * m_fTransferRatio);
                            rectangle.Y = (int)(m_rect.Top * m_fTransferRatio);
                            rectangle.Height = (int)((m_rect.Bottom - m_rect.Top) * m_fTransferRatio);
                            graphics.DrawRectangle(pen, rectangle);



                            int nDivideCnt = int.Parse(this.ChannelRangeCupNumnumericUpDown.Text);

                            Point pt1;
                            pt1 = new Point();
                            pen = new Pen(Color.Yellow, 1);


                            for (int i = 0; i < nDivideCnt; i++)
                            {
                                pt1.X = (int)(rectangle.X + rectangle.Width / nDivideCnt / 2.0 + rectangle.Width / nDivideCnt * 1.0 * (i));
                                pt1.Y = rectangle.Y + rectangle.Height / 2;
                                graphics.DrawEllipse(pen, pt1.X, pt1.Y, 8, 8);

                                //pt1.X = (int)(rectangle[1].X + rectangle[1].Width / nDivideCnt / 2.0 + rectangle[1].Width / nDivideCnt * 1.0 * (i));
                                //pt1.Y = rectangle[1].Y + rectangle[1].Height / 2;
                                //graphics.DrawEllipse(pen, pt1.X, pt1.Y, 8, 8);
                            }
                        }
                    }
                    if (m_rectAW.Left != 0 || m_rectAW.Right != 0 || m_rectAW.Top != 0 || m_rectAW.Bottom != 0)
                    {
                        pen = new Pen(Color.Blue, 2);
                        if (this.SemiAutoWhiteBalancecheckBox.Checked || this.AutoWhiteBalancecheckBox.Checked)
                        {
                            Rectangle rect = new Rectangle(m_rectAW.Left, m_rectAW.Top, m_rectAW.Right - m_rectAW.Left, m_rectAW.Bottom - m_rectAW.Top);
                            graphics.DrawRectangle(pen, rect);
                        }
                        else
                        {
                            graphics.DrawLine(pen, m_rectAW.Left, m_rectAW.Top, m_rectAW.Right, m_rectAW.Bottom);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数ImagepictureBox_Paint出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ImagepictureBox_Paint出错" + ex);
#endif
                }
            }
            else  //3.2
            {
                try
                {
                    Graphics graphics = e.Graphics;
                    GCHandle handle;
                    IntPtr scan;
                    int width = m_ImageWidth;
                    int height = m_ImageHeight;
                    int stride = width * 4;
                    if (m_BottomImageRGB.Length != (width * height * 4))
                        return;//2015-12-4 ivycc修改 解决图像没改变，尺寸变动导致内存崩溃
                    handle = GCHandle.Alloc(m_BottomImageRGB, GCHandleType.Pinned);
                    scan = handle.AddrOfPinnedObject();
                    //scan += (height - 1) * stride;
                    if (width != 0 && height != 0)
                    {
                        //SetImagepictureBox(width, height);
                        m_BottomImage = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppRgb, scan);
                        graphics.DrawImage(m_BottomImage, 0, 0, this.ImagepictureBox.Width, this.ImagepictureBox.Height);

                    }
                    handle.Free();

                    Pen pen;

                    //if (m_CurrentCameraIndex == 1 || (m_CurrentCameraIndex == 0 &&(GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_M))) //Note by ChengSk - 20190520
                    if (m_CurrentCameraIndex == 1 || (m_CurrentCameraIndex == 0 && (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM)))  //Modify by ChengSk - 20190520
                    {
                        Rectangle rectangle = new Rectangle();
                        pen = new Pen(Color.Lime, 2);
                        if ((m_ChannelRangeChannelInIPMIndex & 1) + 1 == 2)//下通道，矩形框位置上移图像分辨率-当前图像位置高度
                        {
                            rectangle.X = (int)(m_rect.Left * m_fTransferRatio);
                            rectangle.Width = (int)((m_rect.Right - m_rect.Left) * m_fTransferRatio);
                            rectangle.Y = (int)((m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height) * m_fTransferRatio);
                            rectangle.Height = (int)((m_rect.Bottom - m_rect.Top) * m_fTransferRatio);
                        }
                        else//上通道
                        {
                            rectangle.X = (int)(m_rect.Left * m_fTransferRatio);
                            rectangle.Width = (int)((m_rect.Right - m_rect.Left) * m_fTransferRatio);
                            rectangle.Y = (int)(m_rect.Top * m_fTransferRatio);
                            rectangle.Height = (int)((m_rect.Bottom - m_rect.Top) * m_fTransferRatio);
                        }
                        graphics.DrawRectangle(pen, rectangle);
                        int nDivideCnt = int.Parse(this.ChannelRangeCupNumnumericUpDown.Text);

                        Point pt1;
                        pt1 = new Point();
                        pen = new Pen(Color.Yellow, 1);


                        for (int i = 0; i < nDivideCnt; i++)
                        {
                            pt1.X = (int)(rectangle.X + rectangle.Width / nDivideCnt / 2.0 + rectangle.Width / nDivideCnt * 1.0 * (i));
                            pt1.Y = rectangle.Y + rectangle.Height / 2;
                            graphics.DrawEllipse(pen, pt1.X, pt1.Y, 8, 8);

                            //pt1.X = (int)(rectangle[1].X + rectangle[1].Width / nDivideCnt / 2.0 + rectangle[1].Width / nDivideCnt * 1.0 * (i));
                            //pt1.Y = rectangle[1].Y + rectangle[1].Height / 2;
                            //graphics.DrawEllipse(pen, pt1.X, pt1.Y, 8, 8);
                        }
                    }
                    else if (m_CurrentCameraIndex == 0 && (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR))
                    {
                        if (this.ImageCorrectioncheckBox.Checked)
                        {
                            Rectangle rectangle = new Rectangle();
                            pen = new Pen(Color.Lime, 2);


                            m_rect.Left += m_ptCheck.X;
                            m_rect.Right += m_ptCheck.X;
                            m_rect.Top += m_ptCheck.Y;
                            m_rect.Bottom += m_ptCheck.Y;



                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[(m_ChannelRangeChannelInIPMIndex)].nOffsetY += m_ptCheck.Y;
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[(m_ChannelRangeChannelInIPMIndex)].nOffsetX += m_ptCheck.X;   //add by xcw 20200602

                            //if (m_rect.nOffsetX+ m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[(m_ChannelRangeIPMInSysIndex & 1)].nLeft[0]<0)
                            //{

                            //}
                            m_ptCheck.X = 0;
                            m_ptCheck.Y = 0;
                            if (m_ChannelRangeChannelInIPMIndex + 1 == 2)//下通道，矩形框位置上移图像分辨率-当前图像位置高度
                            {
                                rectangle.X = (int)(m_rect.Left * m_fTransferRatio);
                                rectangle.Width = (int)((m_rect.Right - m_rect.Left) * m_fTransferRatio);
                                rectangle.Y = (int)((m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height) * m_fTransferRatio);
                                rectangle.Height = (int)((m_rect.Bottom - m_rect.Top) * m_fTransferRatio);
                            }
                            else//上通道
                            {
                                rectangle.X = (int)(m_rect.Left * m_fTransferRatio);
                                rectangle.Width = (int)((m_rect.Right - m_rect.Left) * m_fTransferRatio);
                                rectangle.Y = (int)(m_rect.Top * m_fTransferRatio);
                                rectangle.Height = (int)((m_rect.Bottom - m_rect.Top) * m_fTransferRatio);
                            }
                            //rectangle.X = (int)(m_rect.Left * m_fTransferRatio);
                            //rectangle.Width = (int)((m_rect.Right - m_rect.Left) * m_fTransferRatio);
                            //rectangle.Y = (int)(m_rect.Top * m_fTransferRatio);
                            //rectangle.Height = (int)((m_rect.Bottom - m_rect.Top) * m_fTransferRatio);
                            graphics.DrawRectangle(pen, rectangle);



                            int nDivideCnt = int.Parse(this.ChannelRangeCupNumnumericUpDown.Text);

                            Point pt1;
                            pt1 = new Point();
                            pen = new Pen(Color.Yellow, 1);


                            for (int i = 0; i < nDivideCnt; i++)
                            {
                                pt1.X = (int)(rectangle.X + rectangle.Width / nDivideCnt / 2.0 + rectangle.Width / nDivideCnt * 1.0 * (i));
                                pt1.Y = rectangle.Y + rectangle.Height / 2;
                                graphics.DrawEllipse(pen, pt1.X, pt1.Y, 8, 8);

                                //pt1.X = (int)(rectangle[1].X + rectangle[1].Width / nDivideCnt / 2.0 + rectangle[1].Width / nDivideCnt * 1.0 * (i));
                                //pt1.Y = rectangle[1].Y + rectangle[1].Height / 2;
                                //graphics.DrawEllipse(pen, pt1.X, pt1.Y, 8, 8);
                            }
                        }
                    }
                    //if (m_CurrentCameraIndex == 1 && GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR)
                    //{
                    //    Rectangle rectangle = new Rectangle();
                    //    pen = new Pen(Color.Lime, 2);
                    //    if ((m_ChannelRangeIPMInSysIndex & 1) + 1 == 2)//下通道，矩形框位置上移图像分辨率-当前图像位置高度
                    //    {
                    //        rectangle.X = (int)(m_rect.Left * m_fTransferRatio);
                    //        rectangle.Width = (int)((m_rect.Right - m_rect.Left) * m_fTransferRatio);
                    //        rectangle.Y = (int)((m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height / 2) * m_fTransferRatio);
                    //        rectangle.Height = (int)((m_rect.Bottom - m_rect.Top) * m_fTransferRatio);
                    //    }
                    //    else//上通道
                    //    {
                    //        rectangle.X = (int)(m_rect.Left * m_fTransferRatio);
                    //        rectangle.Width = (int)((m_rect.Right - m_rect.Left) * m_fTransferRatio);
                    //        rectangle.Y = (int)(m_rect.Top * m_fTransferRatio);
                    //        rectangle.Height = (int)((m_rect.Bottom - m_rect.Top) * m_fTransferRatio);
                    //    }
                    //    graphics.DrawRectangle(pen, rectangle);
                    //    int nDivideCnt = int.Parse(this.ChannelRangeCupNumnumericUpDown.Text);

                    //    Point pt1;
                    //    pt1 = new Point();
                    //    pen = new Pen(Color.Yellow, 1);


                    //    for (int i = 0; i < nDivideCnt; i++)
                    //    {
                    //        pt1.X = (int)(rectangle.X + rectangle.Width / nDivideCnt / 2.0 + rectangle.Width / nDivideCnt * 1.0 * (i));
                    //        pt1.Y = rectangle.Y + rectangle.Height / 2;
                    //        graphics.DrawEllipse(pen, pt1.X, pt1.Y, 8, 8);

                    //        //pt1.X = (int)(rectangle[1].X + rectangle[1].Width / nDivideCnt / 2.0 + rectangle[1].Width / nDivideCnt * 1.0 * (i));
                    //        //pt1.Y = rectangle[1].Y + rectangle[1].Height / 2;
                    //        //graphics.DrawEllipse(pen, pt1.X, pt1.Y, 8, 8);
                    //    }
                    //}
                    if (m_rectAW.Left != 0 || m_rectAW.Right != 0 || m_rectAW.Top != 0 || m_rectAW.Bottom != 0)
                    {
                        pen = new Pen(Color.Blue, 2);
                        if (this.SemiAutoWhiteBalancecheckBox.Checked || this.AutoWhiteBalancecheckBox.Checked)
                        {
                            Rectangle rect = new Rectangle(m_rectAW.Left, m_rectAW.Top, m_rectAW.Right - m_rectAW.Left, m_rectAW.Bottom - m_rectAW.Top);
                            graphics.DrawRectangle(pen, rect);
                        }
                        else
                        {
                            graphics.DrawLine(pen, m_rectAW.Left, m_rectAW.Top, m_rectAW.Right, m_rectAW.Bottom);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数ImagepictureBox_Paint出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ImagepictureBox_Paint出错" + ex);
#endif
                }
            }


        }

        /// <summary>
        /// 上传图像显示刷新
        /// </summary>
        /// <param name="imageInfo"></param>
        public void OnUpImageData(stImageData imageData)
        {

            if (GlobalDataInterface.nVer == 0)
            {
                try
                {
                    if (this == Form.ActiveForm)
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new GlobalDataInterface.ImageDataEventHandler(OnUpImageData), imageData);
                            IDShutterAdjust = imageData.imageInfo.nRouteId;
                        }
                        else
                        {
                            if (!this.ContinuousSamplecheckBox.Checked)
                                return;
                            int nCurId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                            IDShutterAdjust = imageData.imageInfo.nRouteId;
                            if (imageData.imageInfo.nRouteId != nCurId)
                                return;
                            int multiple = 1;
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)//高度为原分辨率的3倍 Note by ChengSk - 20190520
                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR)//高度为原分辨率的3倍 Modify by ChengSk - 20190520
                                multiple = 3;
                            if (imageData.imageInfo.width != GlobalDataInterface.globalOut_SysConfig.width || imageData.imageInfo.height != multiple * GlobalDataInterface.globalOut_SysConfig.height)
                                return;
                            if (this.ImageCorrectioncheckBox.Checked)
                            {
                                if (imageData.imageInfo.ucImageFormat == 1)
                                {
                                    m_BottomImageRGB = new byte[imageData.imagedata.Length * 4];
                                    Commonfunction.MONO8ChangeToRGB(imageData.imagedata, ref m_BottomImageRGB, imageData.imageInfo.width, imageData.imageInfo.height);

                                }
                                else
                                {
                                    m_BottomImageRGB = new byte[imageData.imagedata.Length * 2];
                                    Commonfunction.YUV422ChangeToRGB(imageData.imagedata, ref m_BottomImageRGB, imageData.imageInfo.width, imageData.imageInfo.height);
                                }
                                if ((this.ImagepictureBox.Width != imageData.imageInfo.width) || (this.ImagepictureBox.Height != imageData.imageInfo.height))
                                {
                                    SetImagepictureBox(imageData.imageInfo.width, imageData.imageInfo.height);
                                }
                                //m_CheckCamBitmap.Dispose();
                                //IntPtr pData = Marshal.AllocHGlobal(m_BottomImageRGB.Length);//分配结构体大小的内存空间
                                //Marshal.Copy(m_BottomImageRGB, 0, pData, m_BottomImageRGB.Length);
                                //m_CheckCamBitmap = new Bitmap(GlobalDataInterface.globalOut_SysConfig.width, GlobalDataInterface.globalOut_SysConfig.height, 4, PixelFormat.Format32bppArgb, pData);
                            }
                            else
                            {
                                if (imageData.imageInfo.ucImageFormat == 1)
                                {
                                    m_BottomImageRGB = new byte[imageData.imagedata.Length * 4];
                                    Commonfunction.MONO8ChangeToRGB(imageData.imagedata, ref m_BottomImageRGB, imageData.imageInfo.width, imageData.imageInfo.height);

                                }
                                else
                                {
                                    m_BottomImageRGB = new byte[imageData.imagedata.Length * 2];
                                    Commonfunction.YUV422ChangeToRGB(imageData.imagedata, ref m_BottomImageRGB, imageData.imageInfo.width, imageData.imageInfo.height);
                                }
                                if ((this.ImagepictureBox.Width != imageData.imageInfo.width) || (this.ImagepictureBox.Height != imageData.imageInfo.height))
                                {
                                    SetImagepictureBox(imageData.imageInfo.width, imageData.imageInfo.height);
                                }

                            }
                            //m_ImageWidth = imageData.imageInfo.width;//获取图像宽
                            //m_ImageHeight = imageData.imageInfo.height;//获取图像高
                            //float fWidth = (float)m_InitialPictureBoxRectangle.Width / m_ImageWidth;
                            //float fHeight = (float)m_InitialPictureBoxRectangle.Height / m_ImageHeight;

                            //m_fTransferRatio = (fWidth > fHeight ? fHeight : fWidth);//全局缩放取比较小的那个
                            this.ImagepictureBox.Invalidate();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数OnUpImageInfo出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数OnUpImageInfo出错" + ex);
#endif
                }
            }
            else
            {
                try
                {
                    if (this == Form.ActiveForm)
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new GlobalDataInterface.ImageDataEventHandler(OnUpImageData), imageData);
                            IDShutterAdjust = imageData.imageInfo.nRouteId;
                        }
                        else
                        {
                            if (!this.ContinuousSamplecheckBox.Checked)
                                return;
                            int nCurId = 0;
                            if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                            {
                                nCurId = Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeChannelInIPMIndex + m_ChannelRangeIPMInSysIndex * ConstPreDefine.CHANNEL_NUM);

                                //nCurId = Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeChannelInIPMIndex);
                            }

                            else if (GlobalDataInterface.nVer == 0)
                            {
                                nCurId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                            }
                            //int nCurId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                            IDShutterAdjust = imageData.imageInfo.nRouteId;
                            if (imageData.imageInfo.nRouteId != nCurId)
                                return;
                            int multiple = 1;
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)//高度为原分辨率的3倍 Note by ChengSk - 20190520
                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR)//高度为原分辨率的3倍 Modify by ChengSk - 20190520
                                multiple = 3;
                            //if (imageData.imageInfo.width != GlobalDataInterface.globalOut_SysConfig.width || imageData.imageInfo.height != multiple * GlobalDataInterface.globalOut_SysConfig.height)
                            //    return;
                            if (this.ImageCorrectioncheckBox.Checked)
                            {
                                if (imageData.imageInfo.ucImageFormat == 1)
                                {
                                    m_BottomImageRGB = new byte[imageData.imagedata.Length * 4];
                                    Commonfunction.MONO8ChangeToRGB(imageData.imagedata, ref m_BottomImageRGB, imageData.imageInfo.width, imageData.imageInfo.height);

                                }
                                else
                                {
                                    m_BottomImageRGB = new byte[imageData.imagedata.Length * 2];
                                    Commonfunction.YUV422ChangeToRGB(imageData.imagedata, ref m_BottomImageRGB, imageData.imageInfo.width, imageData.imageInfo.height);
                                }
                                if ((this.ImagepictureBox.Width != imageData.imageInfo.width) || (this.ImagepictureBox.Height != GlobalDataInterface.globalOut_SysConfig.height))
                                {
                                    SetImagepictureBox(imageData.imageInfo.width, imageData.imageInfo.height);
                                }
                                //m_CheckCamBitmap.Dispose();
                                //IntPtr pData = Marshal.AllocHGlobal(m_BottomImageRGB.Length);//分配结构体大小的内存空间
                                //Marshal.Copy(m_BottomImageRGB, 0, pData, m_BottomImageRGB.Length);
                                //m_CheckCamBitmap = new Bitmap(GlobalDataInterface.globalOut_SysConfig.width, GlobalDataInterface.globalOut_SysConfig.height, 4, PixelFormat.Format32bppArgb, pData);
                            }
                            else
                            {
                                if (imageData.imageInfo.ucImageFormat == 1)
                                {
                                    m_BottomImageRGB = new byte[imageData.imagedata.Length * 4];
                                    Commonfunction.MONO8ChangeToRGB(imageData.imagedata, ref m_BottomImageRGB, imageData.imageInfo.width, imageData.imageInfo.height);

                                }
                                else
                                {
                                    m_BottomImageRGB = new byte[imageData.imagedata.Length * 2];
                                    Commonfunction.YUV422ChangeToRGB(imageData.imagedata, ref m_BottomImageRGB, imageData.imageInfo.width, imageData.imageInfo.height);
                                }
                                if ((this.ImagepictureBox.Width != imageData.imageInfo.width) || (this.ImagepictureBox.Height != GlobalDataInterface.globalOut_SysConfig.height))
                                {
                                    SetImagepictureBox(imageData.imageInfo.width, imageData.imageInfo.height);
                                }

                            }
                            //m_ImageWidth = imageData.imageInfo.width;//获取图像宽
                            //m_ImageHeight = imageData.imageInfo.height;//获取图像高
                            //float fWidth = (float)m_InitialPictureBoxRectangle.Width / m_ImageWidth;
                            //float fHeight = (float)m_InitialPictureBoxRectangle.Height / m_ImageHeight;

                            //m_fTransferRatio = (fWidth > fHeight ? fHeight : fWidth);//全局缩放取比较小的那个
                            this.ImagepictureBox.Invalidate();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数OnUpImageInfo出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数OnUpImageInfo出错" + ex);
#endif
                }
            }

        }

        /// <summary>
        /// 设置ImagepictureBox位置和大小
        /// </summary>
        private void SetImagepictureBox(int ImageWidth, int ImageHeight)
        {
            //const int MAX_HEIGHT = 384;
            //const int MAX_WIDTH = 512;
            try
            {
                Rect r;
                r.Left = m_InitialPictureBoxRectangle.Left;
                r.Right = m_InitialPictureBoxRectangle.Right;
                r.Top = m_InitialPictureBoxRectangle.Top;
                r.Bottom = m_InitialPictureBoxRectangle.Bottom;

                //float fWidth = (float)m_InitialPictureBoxRectangle.Width / GlobalDataInterface.globalOut_SysConfig.width;
                //float fHeight = (float)m_InitialPictureBoxRectangle.Height / GlobalDataInterface.globalOut_SysConfig.height;
                float fWidth = (float)m_InitialPictureBoxRectangle.Width / m_ImageWidth;
                float fHeight = (float)m_InitialPictureBoxRectangle.Height / m_ImageHeight;

                m_fTransferRatio = (fWidth > fHeight ? fHeight : fWidth);//全局缩放取比较小的那个

                Rectangle rect = new Rectangle(r.Left, r.Top, (int)(m_ImageWidth * m_fTransferRatio), (int)(m_ImageHeight * m_fTransferRatio));
                if (GlobalDataInterface.nVer == 0)
                {
                    fWidth = (float)m_InitialPictureBoxRectangle.Width / m_ImageWidth;
                    fHeight = (float)m_InitialPictureBoxRectangle.Height / m_ImageHeight;

                    m_fTransferRatio = (fWidth > fHeight ? fHeight : fWidth);//全局缩放取比较小的那个

                    rect = new Rectangle(r.Left, r.Top, (int)(m_ImageWidth * m_fTransferRatio), (int)(m_ImageHeight * m_fTransferRatio));
                }
                else
                {
                    fWidth = (float)m_InitialPictureBoxRectangle.Width / ImageWidth;
                    fHeight = (float)m_InitialPictureBoxRectangle.Height / ImageHeight;

                    m_fTransferRatio = (fWidth > fHeight ? fHeight : fWidth);//全局缩放取比较小的那个
                    rect = new Rectangle(r.Left, r.Top, (int)(ImageWidth * m_fTransferRatio), (int)(ImageHeight * m_fTransferRatio));
                }

                if (fWidth > fHeight)
                {
                    //图像比较高，所以显示框要左右居中
                    // rect.Offset((m_InitialPictureBoxRectangle.Width - rect.Width) / 2, 0);
                    rect.Offset((this.ImagepictureBox.Width - rect.Width) / 2, 0);
                }
                else
                {
                    //图像细长，所以显示框要上下居中
                    //rect.Offset(0, (m_InitialPictureBoxRectangle.Height - rect.Height) / 2);
                    rect.Offset(0, (this.ImagepictureBox.Height - rect.Height) / 2);
                }

                this.ImagepictureBox.Size = rect.Size;
                this.ImagepictureBox.Location = rect.Location;

            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数SetImagepictureBox出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数SetImagepictureBox出错" + ex);
#endif
            }
            //MemoryStream ms = new MemoryStream(m_BottomImageRGB);
            //this.ImagepictureBox.Image = Image.FromStream(ms);
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagepictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            m_rectAW.Left = e.X;
            m_rectAW.Top = e.Y;
            m_MouseDown = true;
        }


        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagepictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            //this.Cursor = Cursors.Cross;
            if (m_MouseDown)
            {
                m_rectAW.Right = e.X;
                m_rectAW.Bottom = e.Y;
                this.ImagepictureBox.Invalidate();
            }
        }

        /// <summary>
        /// 鼠标进入图片控件事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagepictureBox_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Cross;
        }


        /// <summary>
        /// 鼠标离开图片控件事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagepictureBox_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 鼠标释放事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagepictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_MouseDown)
            {
                m_MouseDown = false;
                m_rectAW.Right = e.X;
                m_rectAW.Bottom = e.Y;
                this.ImagepictureBox.Invalidate();
            }
        }

        /// <summary>
        /// 连续采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContinuousSamplecheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                stCameraNum cameraNum = new stCameraNum(true);
                // cameraNum.cCameraNum = (byte)this.CameracomboBox.SelectedIndex;
                cameraNum.cCameraNum = (byte)m_CameraIndex;
                if (m_ChannelRangeSubSysIdx >= 0 && m_ChannelRangeIPMInSysIndex >= 0)
                {
                    //int nDrcId = GlobalDataInterface.nVer == 0?Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex):Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                    //MessageBox.Show(m_CurrentCameraConfigIndex.ToString());
                    int nDrcId = 0;

                    if (GlobalDataInterface.nVer == 0)            //版本号判断 add by xcw 20200604
                    {
                        nDrcId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                    }
                    else if (GlobalDataInterface.nVer == 1)
                    {
                        nDrcId = Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeChannelInIPMIndex + m_ChannelRangeIPMInSysIndex * ConstPreDefine.CHANNEL_NUM);
                    }
                    if (checkBox.Checked)
                    {
                        this.ShowBlobcheckBox.Enabled = true;
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            stContinousCapture concapture = new stContinousCapture();
                            concapture.cCameraNum = (byte)m_CameraIndex;
                            if (this.EvenShowcheckBox.Checked)
                                concapture.bEvenShow = 1;
                            else
                                concapture.bEvenShow = 0;
                            GlobalDataInterface.TransmitParam(nDrcId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_ON, concapture);
                        }
                        //if (this.ShowBlobcheckBox.Checked)
                        //{
                        //    if (GlobalDataInterface.global_IsTestMode)
                        //    {
                        //        GlobalDataInterface.TransmitParam(nDrcId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHOW_BLOB_ON, cameraNum);
                        //    }
                        //}
                        //else
                        //{


                        //    if (GlobalDataInterface.global_IsTestMode)
                        //    {
                        //        stContinousCapture concapture = new stContinousCapture();
                        //        concapture.cCameraNum = (byte)m_CameraIndex;
                        //        if (this.EvenShowcheckBox.Checked)
                        //            concapture.bEvenShow = 1;
                        //        else
                        //            concapture.bEvenShow = 0;
                        //        GlobalDataInterface.TransmitParam(nDrcId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_ON, concapture);
                        //    }
                        //}
                    }
                    else
                    {
                        this.ShowBlobcheckBox.Enabled = false;
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            GlobalDataInterface.TransmitParam(nDrcId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_OFF, cameraNum);
                        }
                    }
                    m_rectAW.Left = 0;
                    m_rectAW.Right = 0;
                    m_rectAW.Top = 0;
                    m_rectAW.Bottom = 0;

                    // this.ImagepictureBox.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数ContinuousSamplecheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ContinuousSamplecheckBox_CheckedChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 显示Blob
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowBlobcheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (m_ChannelRangeSubSysIdx >= 0 && m_ChannelRangeIPMInSysIndex >= 0)
                {
                    int nDrcId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                    if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                    {
                        nDrcId = Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeChannelInIPMIndex + m_ChannelRangeIPMInSysIndex * ConstPreDefine.CHANNEL_NUM);

                        //nDrcId = Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeChannelInIPMIndex);
                    }

                    else if (GlobalDataInterface.nVer == 0)
                    {
                        nDrcId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                    }
                    if (checkBox.Checked)
                    {
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            GlobalDataInterface.TransmitParam(nDrcId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHOW_BLOB_ON, m_CameraIndex);
                        }
                    }
                    else
                    {
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            stContinousCapture concapture = new stContinousCapture();
                            concapture.cCameraNum = (byte)m_CameraIndex;
                            if (this.EvenShowcheckBox.Checked)
                                concapture.bEvenShow = 1;
                            else
                                concapture.bEvenShow = 0;
                            GlobalDataInterface.TransmitParam(nDrcId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_ON, concapture);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数ShowBlobcheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ShowBlobcheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Computebutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_rectAW.Left == 0 && m_rectAW.Right == 0 && m_rectAW.Top == 0 && m_rectAW.Bottom == 0)
                    return;
                if (m_rectAW.Right - m_rectAW.Left == 0 && m_rectAW.Bottom - m_rectAW.Top == 0)
                    return;

                float width = (m_rectAW.Right - m_rectAW.Left) / m_fTransferRatio;
                float height = (m_rectAW.Bottom - m_rectAW.Top) / m_fTransferRatio;
                float fPixelRatio = (float)(int.Parse(this.RealLengthnumericUpDown.Text) / Math.Sqrt(width * width + height * height));
                this.PixleRatiotextBox.Text = fPixelRatio.ToString("#0.000");
                if (GlobalDataInterface.nVer == 0)
                {
                    if (m_CurrentCameraConfigIndex == 0) //彩色相机
                    {
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fPixelRatio[Index] = fPixelRatio;
                    }
                    else
                    {
                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fPixelRatio[Index] = fPixelRatio;
                    }
                }
                else if (GlobalDataInterface.nVer == 1)
                {
                    if (m_CurrentCameraConfigIndex == 0) //彩色相机
                    {
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fPixelRatio[m_ChannelRangeChannelInIPMIndex] = fPixelRatio;
                    }
                    else
                    {
                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fPixelRatio[m_ChannelRangeChannelInIPMIndex] = fPixelRatio;
                    }
                }


            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数Computebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数Computebutton_Click出错" + ex);
#endif
            }
        }



        /// <summary>
        /// 自动白平衡系数上传刷新
        /// </summary>
        /// <param name="nSrcID"></param>
        /// <param name="WBcoefficient"></param>
        public void OnUpAutoWhiteBalanceInfo(int nSrcID, stWhiteBalanceCoefficient WBcoefficient)
        {
            if (GlobalDataInterface.nVer == 0)
            {
                try
                {
                    if (this == Form.ActiveForm)//是否操作当前窗体
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new GlobalDataInterface.AutoWhiteBalanceInfoEventHandler(OnUpAutoWhiteBalanceInfo), nSrcID, WBcoefficient);
                        }
                        else
                        {
                            int Id = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                            if (Id != nSrcID)
                                return;
                            this.WhiteBalanceRnumericUpDown.Text = WBcoefficient.MeanValue.MeanR.ToString();
                            this.WhiteBalanceGnumericUpDown.Text = WBcoefficient.MeanValue.MeanG.ToString();
                            this.WhiteBalanceBnumericUpDown.Text = WBcoefficient.MeanValue.MeanB.ToString();
                            this.AutoWBCoeffRtextBox.Text = WBcoefficient.BGR.bR.ToString();
                            this.AutoWBCoeffGtextBox.Text = WBcoefficient.BGR.bG.ToString();
                            this.AutoWBCoeffBtextBox.Text = WBcoefficient.BGR.bB.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数OnUpAutoWhiteBalanceInfo出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数OnUpAutoWhiteBalanceInfo出错" + ex);
#endif
                }
            }
            else
            {
                try
                {
                    if (this == Form.ActiveForm)//是否操作当前窗体
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new GlobalDataInterface.AutoWhiteBalanceInfoEventHandler(OnUpAutoWhiteBalanceInfo), nSrcID, WBcoefficient);
                        }
                        else
                        {
                            int Id = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);

                            //if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                            //{
                            //    Id = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                            //}

                            //else if (GlobalDataInterface.nVer == 0)
                            //{
                            //    Id = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                            //}
                            if (Id != nSrcID)
                                return;
                            this.WhiteBalanceRnumericUpDown.Text = WBcoefficient.MeanValue.MeanR.ToString();
                            this.WhiteBalanceGnumericUpDown.Text = WBcoefficient.MeanValue.MeanG.ToString();
                            this.WhiteBalanceBnumericUpDown.Text = WBcoefficient.MeanValue.MeanB.ToString();
                            this.AutoWBCoeffRtextBox.Text = WBcoefficient.BGR.bR.ToString();
                            this.AutoWBCoeffGtextBox.Text = WBcoefficient.BGR.bG.ToString();
                            this.AutoWBCoeffBtextBox.Text = WBcoefficient.BGR.bB.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数OnUpAutoWhiteBalanceInfo出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数OnUpAutoWhiteBalanceInfo出错" + ex);
#endif
                }
            }

        }

        public void OnShutterAdjustInfo(int nSrcID, stShutterAdjust stshutterAdjust)
        {
            try
            {
                if (this == Form.ActiveForm)//是否操作当前窗体
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new GlobalDataInterface.ShutterAdjustInfoEventHandler(OnShutterAdjustInfo), nSrcID, stshutterAdjust);
                    }
                    else
                    {
                        int Id = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                        if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                        {
                            Id = Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeChannelInIPMIndex + m_ChannelRangeIPMInSysIndex * ConstPreDefine.CHANNEL_NUM);

                        }

                        else if (GlobalDataInterface.nVer == 0)
                        {
                            Id = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                        }

                        if (Id != nSrcID)
                            return;
                        //if (Id != IDShutterAdjust)
                        //    return;
                        this.textBoxLCOLORvalueY.Text = ((stshutterAdjust.colorY[2]) / 2).ToString();
                        this.textBoxMCOLORvalueY.Text = ((stshutterAdjust.colorY[1]) / 2).ToString();
                        this.textBoxRCOLORvalueY.Text = ((stshutterAdjust.colorY[0]) / 2).ToString();

                        this.textBoxLCOLORvalueH.Text = ((stshutterAdjust.colorH[2]) / 2).ToString();
                        this.textBoxMCOLORvalueH.Text = ((stshutterAdjust.colorH[1]) / 2).ToString();
                        this.textBoxRCOLORvalueH.Text = ((stshutterAdjust.colorH[0]) / 2).ToString();

                        this.textBoxLNIR1valueY.Text = ((stshutterAdjust.nir1Y[2]) / 2).ToString();
                        this.textBoxMNIR1valueY.Text = ((stshutterAdjust.nir1Y[1]) / 2).ToString();
                        this.textBoxRNIR1valueY.Text = ((stshutterAdjust.nir1Y[0]) / 2).ToString();

                        this.textBoxLNIR2valueY.Text = ((stshutterAdjust.nir2Y[2]) / 2).ToString();
                        this.textBoxMNIR2valueY.Text = ((stshutterAdjust.nir2Y[1]) / 2).ToString();
                        this.textBoxRNIR2valueY.Text = ((stshutterAdjust.nir2Y[0]) / 2).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数OnShutterAdjustInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数OnShutterAdjustInfo出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 上传水果等级信息刷新
        /// </summary>
        /// <param name="fruitGradeInfo"></param>
        public void OnUpFruitGradeInfo(stFruitGradeInfo fruitGradeInfo)
        {
            if (GlobalDataInterface.nVer == 0)
            {
                try
                {
                    //if (this == Form.ActiveForm)//是否操作当前窗体
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new GlobalDataInterface.FruitGradeInfoEventHandler(OnUpFruitGradeInfo), fruitGradeInfo);
                        }
                        else
                        {
                            //this.DefaultThreshtextBox.Text = fruitGradeInfo.nDefaultDetectionThreshold.ToString();

                            //var ChannelInIPMIndex = GlobalDataInterface.globalIn_defaultInis.FirstOrDefault().nVersion == 40201 ? 0 : Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_ChannelRangeCurrentChannelIndex]);
                            if (fruitGradeInfo.param[Index].unGrade == 0x7f7f7f7f)
                                return;
                            this.MaxDiametertextBox.Text = fruitGradeInfo.param[Index].visionParam.unMaxR.ToString("0.0");
                            this.MinDiametertextBox.Text = fruitGradeInfo.param[Index].visionParam.unMinR.ToString("0.0");
                            this.DAvgtextBox.Text = fruitGradeInfo.param[Index].visionParam.unSelectBasis.ToString("0.0"); //unDAvg->unSelectBasis  //Modify by ChengSk - 20190923
                                                                                                                           //Note by ChengSk - 20190923
                                                                                                                           //this.SymmetryAxistextBox.Text = fruitGradeInfo.param.visionParam.unSymmetryAxisD.ToString("0.0");
                                                                                                                           //this.MaxVerticalSymmetrytextBox.Text = fruitGradeInfo.param.visionParam.unVertiSAMaxL.ToString("0.0");
                                                                                                                           //this.VerticalAxistextBox.Text = fruitGradeInfo.param.visionParam.unVertiSACenterD.ToString("0.000"); //由1位小数改为3位小数 Modify by ChengSk - 20180801
                            this.TransDiaRatiotextBox.Text = fruitGradeInfo.param[Index].visionParam.fDiameterRatio.ToString();
                            this.MinDRatiotextBox.Text = fruitGradeInfo.param[Index].visionParam.fMinDRatio.ToString();
                            this.ColorRatiotextBox1.Text = (fruitGradeInfo.param[Index].visionParam.unColorRate0 / 2).ToString();
                            this.ColorRatiotextBox2.Text = (fruitGradeInfo.param[Index].visionParam.unColorRate1 / 2).ToString();
                            this.ColorRatiotextBox3.Text = (fruitGradeInfo.param[Index].visionParam.unColorRate2 / 2).ToString();
                            this.ProjectAreatextBox.Text = fruitGradeInfo.param[Index].visionParam.unArea.ToString();
                            this.FlawAreatextBox.Text = fruitGradeInfo.param[Index].visionParam.unFlawArea.ToString();
                            this.FlawNumtextBox.Text = fruitGradeInfo.param[Index].visionParam.unFlawNum.ToString();
                            this.VolumtextBox.Text = fruitGradeInfo.param[Index].visionParam.unVolume.ToString();

                            ////等级名称
                            //uint SizeGradeIndex, QualfGradeIndex;
                            //byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                            //string GradeName;
                            ////if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 57) > 0 && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 7) > 0)//品质与尺寸或者品质与重量
                            //if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0)//品质与尺寸或者品质与重量
                            //{
                            //    SizeGradeIndex = fruitGradeInfo.param[ChannelInIPMIndex].unGrade & 15;
                            //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, SizeGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            //    GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');

                            //    QualfGradeIndex = ((fruitGradeInfo.param[ChannelInIPMIndex].unGrade & 240) >> 4);
                            //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, QualfGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            //    GradeName += "." + Encoding.Default.GetString(temp).TrimEnd('\0');
                            //}
                            ////else if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 1)//只有品质
                            //else if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 0)//品质
                            //{
                            //    QualfGradeIndex = fruitGradeInfo.param[ChannelInIPMIndex].unGrade & 240;
                            //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, QualfGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            //    GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                            //}
                            //else//尺寸
                            //{
                            //    SizeGradeIndex = fruitGradeInfo.param[ChannelInIPMIndex].unGrade & 15;
                            //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, SizeGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            //    GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                            //}
                            //this.GradetextBox.Text = GradeName;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数OnUpFruitGradeInfo出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数OnUpFruitGradeInfo出错" + ex);
#endif
                }
            }
            else
            {
                try
                {
                    //if (this == Form.ActiveForm)//是否操作当前窗体
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new GlobalDataInterface.FruitGradeInfoEventHandler(OnUpFruitGradeInfo), fruitGradeInfo);
                        }
                        else
                        {
                            //this.DefaultThreshtextBox.Text = fruitGradeInfo.nDefaultDetectionThreshold.ToString();
                            //m_CurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));
                            //if (GlobalDataInterface.nVer == ConstPreDefine.VERSION3)            //版本号判断 add by xcw 20200604
                            //{
                            //    m_CurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), (Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]) >> 1));
                            //}

                            //else if (GlobalDataInterface.nVer == ConstPreDefine.VERSION4)
                            //{
                            //    m_CurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));
                            //}
                            int ChannelInIPMIndex = Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_ChannelRangeCurrentChannelIndex]);

                            if (fruitGradeInfo.param[ChannelInIPMIndex].unGrade == 0x7f7f7f7f)
                                return;
                            this.MaxDiametertextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unMaxR.ToString("0.0");
                            this.MinDiametertextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unMinR.ToString("0.0");
                            this.DAvgtextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unSelectBasis.ToString("0.0"); //unDAvg->unSelectBasis  //Modify by ChengSk - 20190923
                                                                                                                                       //Note by ChengSk - 20190923
                                                                                                                                       //this.SymmetryAxistextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unSymmetryAxisD.ToString("0.0");
                                                                                                                                       //this.MaxVerticalSymmetrytextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unVertiSAMaxL.ToString("0.0");
                                                                                                                                       //this.VerticalAxistextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unVertiSACenterD.ToString("0.000"); //由1位小数改为3位小数 Modify by ChengSk - 20180801
                            this.TransDiaRatiotextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.fDiameterRatio.ToString();
                            this.MinDRatiotextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.fMinDRatio.ToString();
                            this.ColorRatiotextBox1.Text = (fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unColorRate0 / 2).ToString();
                            this.ColorRatiotextBox2.Text = (fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unColorRate1 / 2).ToString();
                            this.ColorRatiotextBox3.Text = (fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unColorRate2 / 2).ToString();
                            this.ProjectAreatextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unArea.ToString();
                            this.FlawAreatextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unFlawArea.ToString();
                            this.FlawNumtextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unFlawNum.ToString();
                            this.VolumtextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unVolume.ToString();

                            ////等级名称
                            //uint SizeGradeIndex, QualfGradeIndex;
                            //byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                            //string GradeName;
                            ////if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 57) > 0 && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 7) > 0)//品质与尺寸或者品质与重量
                            //if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0)//品质与尺寸或者品质与重量
                            //{
                            //    SizeGradeIndex = fruitGradeInfo.param[ChannelInIPMIndex].unGrade & 15;
                            //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, SizeGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            //    GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');

                            //    QualfGradeIndex = ((fruitGradeInfo.param[ChannelInIPMIndex].unGrade & 240) >> 4);
                            //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, QualfGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            //    GradeName += "." + Encoding.Default.GetString(temp).TrimEnd('\0');
                            //}
                            ////else if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 1)//只有品质
                            //else if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 0)//品质
                            //{
                            //    QualfGradeIndex = fruitGradeInfo.param[ChannelInIPMIndex].unGrade & 240;
                            //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, QualfGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            //    GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                            //}
                            //else//尺寸
                            //{
                            //    SizeGradeIndex = fruitGradeInfo.param[ChannelInIPMIndex].unGrade & 15;
                            //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, SizeGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            //    GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                            //}
                            //this.GradetextBox.Text = GradeName;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数OnUpFruitGradeInfo出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数OnUpFruitGradeInfo出错" + ex);
#endif
                }
            }

        }
        //设置果杯大小
        void SetCupRectValue(bool IsInc, ref int val, int nMaxvalue, int nMinvalue)
        {
            try
            {
                int IncValue = int.Parse(this.CupMovenumericUpDown.Text);

                if (IsInc)
                {
                    val = val + IncValue;
                }
                else
                {
                    val = val - IncValue;
                }
                if (val <= nMinvalue)
                    val = nMinvalue;
                if (val >= nMaxvalue)
                    val = nMaxvalue;
                this.ImagepictureBox.Invalidate();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数SetCupRectValue出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数SetCupRectValue出错" + ex);
#endif
            }
        }

        //设置果杯大小 Add by ChengSk - 20190809
        void SetCupRectValue(bool IsInc, ref int val, int nMaxvalue, int nMinvalue, int rectTopLimitvalue, int rectBottomLimitvalue)
        {
            try
            {
                int IncValue = int.Parse(this.CupMovenumericUpDown.Text);

                if (IsInc)
                {
                    val = val + IncValue;
                }
                else
                {
                    val = val - IncValue;
                }
                if (val <= nMinvalue)
                    val = nMinvalue;
                if (val >= nMaxvalue)
                    val = nMaxvalue;

                if (val < rectTopLimitvalue)    //new Add
                    val = rectTopLimitvalue;
                if (val > rectBottomLimitvalue) //new Add
                    val = rectBottomLimitvalue;
                this.ImagepictureBox.Invalidate();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数SetCupRectValue出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数SetCupRectValue出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 果杯上边沿减小按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CupMoveUpbutton1_Click(object sender, EventArgs e)
        {
            if (GlobalDataInterface.nVer == 0) //V4.2
            {
                SetCupRectValue(false, ref m_rect.Top, m_rect.Bottom, 1, RectLimitTop, RectLimitBottom); //SetCupRectValue(false, ref m_rect.Top, m_rect.Bottom, 1);

            }
            else //V3.2
            {
                if ((m_ChannelRangeChannelInIPMIndex & 1) + 1 == 2)//下通道，矩形框位置上移图像分辨率-当前图像位置高度
                {
                    m_rect.Top = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                    SetCupRectValue(false, ref m_rect.Top, m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height, 1/*(m_rect1.nOffsetY < 0) ? RectLimitTop - m_rect1.nOffsetY : RectLimitTop*/, RectLimitTop, RectLimitBottom); //SetCupRectValue(true, ref m_rect.Top, m_rect.Bottom, 1);
                                                                                                                                                                                                                                         //SetCupRectValue(false, ref m_rect.Top, m_rect.Bottom-GlobalDataInterface.globalOut_SysConfig.height/2, 1, RectLimitTop / 2, RectLimitBottom / 2); //SetCupRectValue(true, ref m_rect.Top, m_rect.Bottom, 1);
                    m_rect.Top = m_rect.Top + GlobalDataInterface.globalOut_SysConfig.height;
                }
                else//上通道
                {
                    SetCupRectValue(false, ref m_rect.Top, m_rect.Bottom, 1/*(m_rect1.nOffsetY < 0) ? RectLimitTop - m_rect1.nOffsetY : RectLimitTop*/, RectLimitTop, RectLimitBottom); //modify by xcw -20200623

                    //SetCupRectValue(false, ref m_rect.Top, m_rect.Bottom, 1, RectLimitTop / 2, RectLimitBottom / 2); //SetCupRectValue(true, ref m_rect.Top, m_rect.Bottom, 1);
                }
            }
        }

        /// <summary>
        /// 果杯上边沿增大按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CupMoveUpbutton2_Click(object sender, EventArgs e)
        {
            if (GlobalDataInterface.nVer == 0)
            {
                SetCupRectValue(true, ref m_rect.Top, m_rect.Bottom, 1, RectLimitTop, RectLimitBottom); //SetCupRectValue(true, ref m_rect.Top, m_rect.Bottom, 1);

            }
            else //V3.2
            {
                if ((m_ChannelRangeChannelInIPMIndex & 1) + 1 == 2)//下通道，矩形框位置上移图像分辨率-当前图像位置高度
                {
                    m_rect.Top = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                    SetCupRectValue(true, ref m_rect.Top, m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height, 1, RectLimitTop, RectLimitBottom); //SetCupRectValue(true, ref m_rect.Top, m_rect.Bottom, 1);
                    m_rect.Top = m_rect.Top + GlobalDataInterface.globalOut_SysConfig.height;
                }
                else//上通道
                {
                    SetCupRectValue(true, ref m_rect.Top, m_rect.Bottom, 1, RectLimitTop, RectLimitBottom); //SetCupRectValue(true, ref m_rect.Top, m_rect.Bottom, 1);
                }
            }
        }

        /// <summary>
        /// 果杯下边沿减小按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CupMoveDownbutton1_Click(object sender, EventArgs e)
        {
            if (GlobalDataInterface.nVer == 0)
            {
                SetCupRectValue(false, ref m_rect.Bottom, m_ImageHeight - 1, m_rect.Top, RectLimitTop, RectLimitBottom); //SetCupRectValue(false, ref m_rect.Bottom, m_ImageHeight - 1, m_rect.Top);

            }
            else
            {
                if ((m_ChannelRangeChannelInIPMIndex & 1) + 1 == 2)//下通道，矩形框位置上移图像分辨率-当前图像位置高度
                {
                    m_rect.Bottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                    SetCupRectValue(false, ref m_rect.Bottom, m_ImageHeight - 1, m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height, RectLimitTop, RectLimitBottom); //SetCupRectValue(false, ref m_rect.Bottom, m_ImageHeight - 1, m_rect.Top);
                    m_rect.Bottom = m_rect.Bottom + GlobalDataInterface.globalOut_SysConfig.height;
                }
                else//上通道
                {
                    SetCupRectValue(false, ref m_rect.Bottom, m_ImageHeight - 1, m_rect.Top, RectLimitTop, RectLimitBottom); //SetCupRectValue(false, ref m_rect.Bottom, m_ImageHeight - 1, m_rect.Top);

                }
            }
            // if (this.CupSeleccomboBox.SelectedIndex == 0)
            //else
            //    SetCupRectValue(false, ref m_rect[1].Bottom, m_ImageHeight - 1, m_rect[1].Top);
        }

        /// <summary>
        /// 果杯下边沿增大按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CupMoveDownbutton2_Click(object sender, EventArgs e)
        {
            if (GlobalDataInterface.nVer == 0)
            {
                SetCupRectValue(true, ref m_rect.Bottom, m_ImageHeight - 1, m_rect.Top, RectLimitTop, RectLimitBottom); //SetCupRectValue(true, ref m_rect.Bottom, m_ImageHeight - 1, m_rect.Top);

            }
            else
            {
                if ((m_ChannelRangeChannelInIPMIndex & 1) + 1 == 2)//下通道，矩形框位置上移图像分辨率-当前图像位置高度
                {
                    m_rect.Bottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                    SetCupRectValue(true, ref m_rect.Bottom, /*(m_rect1.nOffsetY > 0) ? RectLimitBottom - 1 - m_rect1.nOffsetY : RectLimitBottom - 1*/m_ImageHeight - 1, m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height, RectLimitTop, RectLimitBottom); //SetCupRectValue(true, ref m_rect.Bottom, m_ImageHeight - 1, m_rect.Top);

                    m_rect.Bottom = m_rect.Bottom + GlobalDataInterface.globalOut_SysConfig.height;
                }
                else//上通道
                {
                    SetCupRectValue(true, ref m_rect.Bottom, /*(m_rect1.nOffsetY > 0) ? RectLimitBottom - 1 - m_rect1.nOffsetY : RectLimitBottom - 1*/ m_ImageHeight - 1, m_rect.Top, RectLimitTop, RectLimitBottom); //modify by xcw -20200623
                    //SetCupRectValue(true, ref m_rect.Bottom, m_ImageHeight / 2 - 1, m_rect.Top, RectLimitTop / 2, RectLimitBottom / 2); //SetCupRectValue(true, ref m_rect.Bottom, m_ImageHeight - 1, m_rect.Top);
                }
            }
            //if (this.CupSeleccomboBox.SelectedIndex == 0)
            //else
            //    SetCupRectValue(true, ref m_rect[1].Bottom, m_ImageHeight - 1, m_rect[1].Top);

        }

        /// <summary>
        /// 果杯左边沿减小按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CupMoveLeftbutton1_Click(object sender, EventArgs e)
        {
            SetCupRectValue(false, ref m_rect.Left, m_rect.Right, 1);

            //if (GlobalDataInterface.nVer == 0)
            //{
            //    SetCupRectValue(false, ref m_rect.Left, m_rect.Right, 1);
            //}
            //else
            //{
            //    SetCupRectValue(false, ref m_rect.Left, m_rect.Right, (m_rect1.nOffsetX < 0) ? 1 - m_rect1.nOffsetX : 1); //add by xcw -20200623
            //}
        }

        /// <summary>
        /// 果杯左边沿增大按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CupMoveLeftbutton2_Click(object sender, EventArgs e)
        {
            // if (this.CupSeleccomboBox.SelectedIndex == 0)  
            SetCupRectValue(true, ref m_rect.Left, m_rect.Right, 1);
            //else
            //    SetCupRectValue(true, ref m_rect[1].Left, m_rect[1].Right, 1);
        }

        /// <summary>
        /// 果杯右边沿减小按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CupMoveRighttbutton1_Click(object sender, EventArgs e)
        {
            //if (this.CupSeleccomboBox.SelectedIndex == 0)
            SetCupRectValue(false, ref m_rect.Right, m_ImageWidth - 1, m_rect.Left);
            //else
            //    SetCupRectValue(false, ref m_rect[1].Right, m_ImageWidth - 1, m_rect[1].Left);

        }

        /// <summary>
        /// 果杯右边沿增大按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CupMoveRightbutton2_Click(object sender, EventArgs e)
        {
            SetCupRectValue(true, ref m_rect.Right, m_ImageWidth - 1, m_rect.Left);
            //if (GlobalDataInterface.nVer == 0)
            //{
            //    SetCupRectValue(true, ref m_rect.Right, m_ImageWidth - 1, m_rect.Left);
            //}
            //else
            //{
            //    SetCupRectValue(true, ref m_rect.Right, (m_rect1.nOffsetY > 0) ? m_ImageWidth - 1 - m_rect1.nOffsetY : m_ImageWidth - 1, m_rect.Left); //add by xcw -20200623
            //}
        }

        /// <summary>
        /// 图像校对上移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageCorrectUpbutton_Click(object sender, EventArgs e)
        {
            if (GlobalDataInterface.nVer == 0)
            {
                try
                {
                    int add = int.Parse(this.ImageCorrectnumericUpDown.Text);
                    if (m_ptCheck.Y < m_ImageHeight / 2)
                    {
                        m_ptCheck.Y -= add;
                        int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;

                        if (m_rect.Top + y < 0)
                        {
                            m_ptCheck.Y += add;
                            //MessageBox.Show("错误：黑白相机果杯坐标位置超出范围！");
                            //MessageBox.Show("0x10001005 The position of IR camera image area is out of range!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if(GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR|| 
                            GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM)
                        {
                            if (m_rect.Top + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY * m_ImageHeight / this.ImagepictureBox.Height < 0)
                            {
                                m_ptCheck.Y += add;
                                MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox7Text[GlobalDataInterface.selectLanguageIndex],
                                    LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                      
                        this.ImagepictureBox.Invalidate();

                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数ImageCorrectLeftbutton_Click出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ImageCorrectLeftbutton_Click出错" + ex);
#endif
                }
            }
            else
            {
                try
                {
                    int add = int.Parse(this.ImageCorrectnumericUpDown.Text);


                    if (m_ptCheck.Y < m_ImageHeight / 2)
                    {
                        m_ptCheck.Y -= add;
                        int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                        if ((m_ChannelRangeChannelInIPMIndex & 1) + 1 == 2)//下通道，矩形框位置上移图像分辨率-当前图像位置高度
                        {
                            if (y + m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height < 0)
                            {
                                m_ptCheck.Y += add;
                                MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                    LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR ||
                           GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM)
                            {
                                if (m_rect.Top + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[1].nOffsetY * m_ImageHeight / this.ImagepictureBox.Height - GlobalDataInterface.globalOut_SysConfig.height < 0)
                                {
                                    m_ptCheck.Y += add;
                                    MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox7Text[GlobalDataInterface.selectLanguageIndex],
                                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                        else//上通道
                        {
                            if (y + m_rect.Top < 0)
                            {
                                m_ptCheck.Y += add;
                                MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                    LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR ||
                           GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM)
                            {
                                if (m_rect.Top + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[0].nOffsetY * m_ImageHeight / this.ImagepictureBox.Height < 0)
                                {
                                    m_ptCheck.Y += add;
                                    MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox7Text[GlobalDataInterface.selectLanguageIndex],
                                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }

                       
                        this.ImagepictureBox.Invalidate();

                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数ImageCorrectLeftbutton_Click出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ImageCorrectLeftbutton_Click出错" + ex);
#endif
                }
            }


        }
        /// <summary>
        /// 图像校对下移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageCorrectDownbutton_Click(object sender, EventArgs e)
        {
            if (GlobalDataInterface.nVer == 0)
            {
                try
                {
                    int add = int.Parse(this.ImageCorrectnumericUpDown.Text);
                    if (m_ptCheck.Y > -m_ImageHeight / 2)
                    {
                        m_ptCheck.Y += add;
                        int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                        //int index = 0;
                        //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8 && m_CameraIndex > 4)
                        //    index = (m_CameraIndex - 1) / 2;
                        //else
                        //    index = m_CameraIndex - 1;
                        if (m_rect.Bottom + y > (m_ImageHeight - 1))
                        {
                            m_ptCheck.Y -= add;
                            //MessageBox.Show("错误：黑白相机果杯坐标位置超出范围！");
                            //MessageBox.Show("0x10001005 The position of IR camera image area is out of range!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR ||
                           GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM)
                        {
                            if (m_rect.Bottom + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetY * m_ImageHeight / this.ImagepictureBox.Height > (m_ImageHeight - 1))
                            {
                                m_ptCheck.Y -= add;
                                MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox7Text[GlobalDataInterface.selectLanguageIndex],
                                    LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        this.ImagepictureBox.Invalidate();
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数ImageCorrectDownbutton_Click出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ImageCorrectDownbutton_Click出错" + ex);
#endif
                }
            }
            else
            {
                try
                {
                    int add = int.Parse(this.ImageCorrectnumericUpDown.Text);

                    if (m_ptCheck.Y > -m_ImageHeight / 2)
                    {
                        m_ptCheck.Y += add;
                        int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                        if ((m_ChannelRangeChannelInIPMIndex & 1) + 1 == 2)//下通道，矩形框位置上移图像分辨率-当前图像位置高度
                        {
                            if (m_rect.Bottom + y - GlobalDataInterface.globalOut_SysConfig.height > RectLimitBottom/* GlobalDataInterface.globalOut_SysConfig.height*/)
                            {
                                m_ptCheck.Y -= add;
                                MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                    LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR ||
                          GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM)
                            {
                                if (m_rect.Bottom + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[1].nOffsetY * m_ImageHeight / this.ImagepictureBox.Height > RectLimitBottom)
                                {
                                    m_ptCheck.Y -= add;
                                    MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox7Text[GlobalDataInterface.selectLanguageIndex],
                                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                        else//上通道
                        {
                            if (m_rect.Bottom + y > RectLimitBottom/*GlobalDataInterface.globalOut_SysConfig.height*/)
                            {
                                m_ptCheck.Y -= add;
                                MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                    LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR ||
                           GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM)
                            {
                                if (m_rect.Bottom + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[0].nOffsetY * m_ImageHeight / this.ImagepictureBox.Height < 0)
                                {
                                    m_ptCheck.Y -= add;
                                    MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox7Text[GlobalDataInterface.selectLanguageIndex],
                                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }

                        this.ImagepictureBox.Invalidate();
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数ImageCorrectDownbutton_Click出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ImageCorrectDownbutton_Click出错" + ex);
#endif
                }
            }

        }
        /// <summary>
        /// 图像校对左移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageCorrectLeftbutton_Click(object sender, EventArgs e)
        {
            if (GlobalDataInterface.nVer == 0)
            {
                try
                {
                    int add = int.Parse(this.ImageCorrectnumericUpDown.Text);
                    if (m_ptCheck.X < GlobalDataInterface.globalOut_SysConfig.width / 2)
                    {
                        m_ptCheck.X -= add;
                        int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                        //int index = 0;
                        //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8 && m_CameraIndex > 4)
                        //    index = (m_CameraIndex - 1) / 2;
                        //else
                        //    index = m_CameraIndex - 1;
                        if (m_rect.Left + x < 0)
                        {
                            m_ptCheck.X += add;
                            //MessageBox.Show("错误：黑白相机果杯坐标位置超出范围！");
                            //MessageBox.Show("0x10001005 The position of IR camera image area is out of range!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR ||
                         GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM)
                        {
                            if (m_rect.Left + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetX * m_ImageWidth / this.ImagepictureBox.Width <0)
                            {
                                m_ptCheck.X += add;
                                MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox7Text[GlobalDataInterface.selectLanguageIndex],
                                    LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        this.ImagepictureBox.Invalidate();
                    }

                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数ImageCorrectLeftbutton_Click出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ImageCorrectLeftbutton_Click出错" + ex);
#endif
                }
            }
            else
            {
                try
                {
                    int add = int.Parse(this.ImageCorrectnumericUpDown.Text);
                    if (m_ptCheck.X < GlobalDataInterface.globalOut_SysConfig.width / 2)
                    {
                        m_ptCheck.X -= add;
                        int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                        if (m_rect.Left + x < 0)
                        {
                            m_ptCheck.X += add;
                            //MessageBox.Show("错误：黑白相机果杯坐标位置超出范围！");
                            //MessageBox.Show("0x10001005 The position of IR camera image area is out of range!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR ||
                          GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM)
                        {
                            if (m_rect.Left + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[m_ChannelRangeChannelInIPMIndex].nOffsetX * m_ImageWidth / this.ImagepictureBox.Width > RectLimitBottom)
                            {
                                m_ptCheck.X += add;
                                MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox7Text[GlobalDataInterface.selectLanguageIndex],
                                    LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        this.ImagepictureBox.Invalidate();
                    }

                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelRange中函数ImageCorrectLeftbutton_Click出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ImageCorrectLeftbutton_Click出错" + ex);
#endif
                }
            }

        }
        /// <summary>
        /// 图像校对右移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageCorrectRightbutton_Click(object sender, EventArgs e)
        {
            if (GlobalDataInterface.nVer == 0)
            {
                int add = int.Parse(this.ImageCorrectnumericUpDown.Text);
                if (m_ptCheck.X > -m_ImageWidth / 2)
                {
                    m_ptCheck.X += add;
                    int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                    //int index = 0;
                    //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8 && m_CameraIndex > 4)
                    //    index = (m_CameraIndex - 1) / 2;
                    //else
                    //    index = m_CameraIndex - 1;
                    // if ((m_paras.cameraParas[m_CameraIndex - 1].cup[0].nLeft[m_paras.nCupNum] + x > m_ImageWidth - 1) || (m_paras.cameraParas[m_CameraIndex - 1].cup[1].nLeft[m_paras.nCupNum] + x > m_ImageWidth - 1))
                    if (m_rect.Right + x > m_ImageWidth - 1)//by 2015-4-17 去掉分隔线，只显示果杯外框
                    {
                        m_ptCheck.X -= add;
                        //MessageBox.Show("错误：黑白相机果杯坐标位置超出范围！");
                        //MessageBox.Show("0x10001005 The position of IR camera image area is out of range!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR ||
                         GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM)
                    {
                        if (m_rect.Right + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nOffsetX * m_ImageWidth / this.ImagepictureBox.Width > m_ImageWidth - 1)
                        {
                            m_ptCheck.X -= add;
                            MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox7Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    this.ImagepictureBox.Invalidate();
                }
            }
            else
            {
                int add = int.Parse(this.ImageCorrectnumericUpDown.Text);
                if (m_ptCheck.X > -m_ImageWidth / 2)
                {
                    m_ptCheck.X += add;
                    int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                    //int index = 0;
                    //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8 && m_CameraIndex > 4)
                    //    index = (m_CameraIndex - 1) / 2;
                    //else
                    //    index = m_CameraIndex - 1;
                    // if ((m_paras.cameraParas[m_CameraIndex - 1].cup[0].nLeft[m_paras.nCupNum] + x > m_ImageWidth - 1) || (m_paras.cameraParas[m_CameraIndex - 1].cup[1].nLeft[m_paras.nCupNum] + x > m_ImageWidth - 1))
                    if (m_rect.Right + x > m_ImageWidth - 1)//by 2015-4-17 去掉分隔线，只显示果杯外框
                    {
                        m_ptCheck.X -= add;
                        //MessageBox.Show("错误：黑白相机果杯坐标位置超出范围！");
                        //MessageBox.Show("0x10001005 The position of IR camera image area is out of range!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR ||
                         GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM)
                    {
                        if (m_rect.Right + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[m_ChannelRangeChannelInIPMIndex].nOffsetX * m_ImageWidth / this.ImagepictureBox.Width > m_ImageWidth - 1)
                        {
                            m_ptCheck.X -= add;
                            MessageBox.Show("0x10001005 " + LanguageContainer.ChannelRangeMessagebox7Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    this.ImagepictureBox.Invalidate();
                }
            }


        }

        // <summary>
        /// 图像偏移上移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageOffsetUpbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalDataInterface.nVer == 0) //4.2
                {
                    int add = int.Parse(this.ImageOffsetnumericUpDown.Text);
                    int ChannelIndex = 0;

                    if (m_CurrentCameraConfigIndex == 0) //彩色相机
                    {
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] -= add;
                        if (0 <= m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex]
                            && m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] <= this.HeightnumericUpDown.Maximum - GlobalDataInterface.globalOut_SysConfig.height)
                        {

                        }
                        else
                        {
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] += add;
                            MessageBox.Show("0x10001010 " + LanguageContainer.ChannelRangeMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        this.OffsettextBox.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex].ToString();

                    }
                    else if (m_CurrentCameraConfigIndex == 1)//红外1
                    {
                        m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] -= add;

                        if (0 <= m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex]
                            && m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] <= this.HeightnumericUpDown.Maximum - GlobalDataInterface.globalOut_SysConfig.height)
                        {
                        }
                        else
                        {
                            m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] += add;
                            MessageBox.Show("0x10001010 " + LanguageContainer.ChannelRangeMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        this.OffsettextBox.Text = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex].ToString();

                    }
                }
                else //3.2
                {
                    int add = int.Parse(this.ImageOffsetnumericUpDown.Text);
                    int ChannelIndex = 0;
                    bool camaROIOffset = false;
                    bool IrCamaROIOffset = false;

                    ChannelIndex = m_ChannelRangeChannelInIPMIndex;
                    camaROIOffset = (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[1] > m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[0] + GlobalDataInterface.globalOut_SysConfig.height) ? true : false;
                    IrCamaROIOffset = (m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[1] > m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[0] + GlobalDataInterface.globalOut_SysConfig.height) ? true : false;


                    if (m_CurrentCameraConfigIndex == 0) //彩色相机
                    {
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] -= add;
                        if (0 <= m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex]
                            && m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] <= this.HeightnumericUpDown.Maximum
                            && (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[1] >= m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[0] + GlobalDataInterface.globalOut_SysConfig.height))
                        {

                        }
                        else
                        {
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] += add;
                            MessageBox.Show("0x10001010 " + LanguageContainer.ChannelRangeMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        this.OffsettextBox.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex].ToString();

                    }
                    else if (m_CurrentCameraConfigIndex == 1)//红外1
                    {
                        m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] -= add;

                        if (0 <= m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex]
                            && m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] <= this.HeightnumericUpDown.Maximum
                           && (m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[1] >= m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[0] + GlobalDataInterface.globalOut_SysConfig.height))
                        {
                        }
                        else
                        {
                            m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] += add;
                            MessageBox.Show("0x10001010 " + LanguageContainer.ChannelRangeMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        this.OffsettextBox.Text = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex].ToString();

                    }
                }





            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数ImageCorrectLeftbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ImageCorrectLeftbutton_Click出错" + ex);
#endif
            }


        }

        // <summary>
        /// 图像偏移下移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageOffsetDownbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalDataInterface.nVer == 0) //4.2
                {
                    int add = int.Parse(this.ImageOffsetnumericUpDown.Text);
                    int ChannelIndex = 0;
                    //ChannelIndex = m_ChannelRangeChannelInIPMIndex;
                    if (m_CurrentCameraConfigIndex == 0) //彩色相机
                    {
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] += add;

                        if (0 <= m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex]
                            && m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] <= this.HeightnumericUpDown.Maximum - GlobalDataInterface.globalOut_SysConfig.height)
                        {
                        }
                        else
                        {
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] -= add;
                            MessageBox.Show("0x10001010 " + LanguageContainer.ChannelRangeMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        this.OffsettextBox.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex].ToString();
                    }
                    else if (m_CurrentCameraConfigIndex == 1)
                    {
                        m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] += add;

                        if (0 <= m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex]
                            && m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] <= this.HeightnumericUpDown.Maximum - GlobalDataInterface.globalOut_SysConfig.height)
                        {
                        }
                        else
                        {
                            m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] -= add;
                            MessageBox.Show("0x10001010 " + LanguageContainer.ChannelRangeMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        this.OffsettextBox.Text = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex].ToString();

                    }
                }
                else  //3.2
                {
                    int add = int.Parse(this.ImageOffsetnumericUpDown.Text);
                    int ChannelIndex = 0;
                    bool camaROIOffset = false;
                    bool IrCamaROIOffset = false;
                    ChannelIndex = m_ChannelRangeChannelInIPMIndex;
                    camaROIOffset = (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[1] > m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[0] + GlobalDataInterface.globalOut_SysConfig.height) ? true : false;
                    IrCamaROIOffset = (m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[1] > m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[0] + GlobalDataInterface.globalOut_SysConfig.height) ? true : false;
                    //}
                    if (m_CurrentCameraConfigIndex == 0) //彩色相机
                    {
                        m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] += add;

                        if (0 <= m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex]
                            && m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] <= this.HeightnumericUpDown.Maximum
                            && (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[1] >= m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[0] + GlobalDataInterface.globalOut_SysConfig.height))
                        {
                        }
                        else
                        {
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] -= add;
                            MessageBox.Show("0x10001010 " + LanguageContainer.ChannelRangeMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        this.OffsettextBox.Text = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex].ToString();
                    }
                    else if (m_CurrentCameraConfigIndex == 1)
                    {
                        m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] += add;

                        if (0 <= m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex]
                            && m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] <= this.HeightnumericUpDown.Maximum
                            && (m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[1] >= m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[0] + GlobalDataInterface.globalOut_SysConfig.height))
                        {
                        }
                        else
                        {
                            m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] -= add;
                            MessageBox.Show("0x10001010 " + LanguageContainer.ChannelRangeMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        this.OffsettextBox.Text = m_paras.irCameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex].ToString();

                    }
                }





            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数ImageCorrectLeftbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ImageCorrectLeftbutton_Click出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 立即生效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelRangeEffectbutton_Click(object sender, EventArgs e)
        {
            try
            {
                ChannelRangeSaveConfig();
                this.ChannelRangeEffectbutton.Enabled = false;
                this.EffectButtonDelaytimer2.Enabled = true;
            }
            catch (Exception ex)
            {
                this.ChannelRangeEffectbutton.Enabled = true;

                Trace.WriteLine("ProjectSetForm-ChannelRange中函数ChannelRangeEffectbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ChannelRangeEffectbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 立即生效后延迟1.5秒再启用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffectButtonDelaytimer2_Tick(object sender, EventArgs e)
        {
            this.ChannelRangeEffectbutton.Enabled = true;
            this.EffectButtonDelaytimer2.Enabled = false;
        }


        /// <summary>
        /// 保存设置
        /// </summary>
        private bool ChannelRangeSaveConfig()
        {
            if (GlobalDataInterface.nVer == 0)
            {
                try
                {
                    if (m_ChannelRangeSubSysIdx >= 0 && m_ChannelRangeIPMInSysIndex >= 0)
                    {
                        //m_paras.nDefaultDetectionThreshold = int.Parse(this.DetectionThresholdnumericUpDown.Text);

                        //m_paras.fSeedPointRange = int.Parse(this.SeedPointRangenumericUpDown.Text);
                        m_paras.nCupNum = int.Parse(this.ChannelRangeCupNumnumericUpDown.Text);
                        if (m_paras.nCupNum > 10) m_paras.nCupNum = 10;
                        if (m_paras.nCupNum < 1) m_paras.nCupNum = 1;
                        int ChannelInIPMIndex = Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_ChannelRangeCurrentChannelIndex]);
                        // int index = 0;
                        if (m_CurrentCameraConfigIndex == 0) //彩色相机
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8)
                            //{
                            //    if (m_CurrentCameraIndex == 2)
                            //        index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //    else
                            //        index = m_CurrentCameraIndex + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //}
                            //else
                            //    index = m_CurrentCameraIndex + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //if ( GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR) //Note by ChengSk - 20190520
                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM) //Modify by ChengSk - 20190520
                            {
                                //if (m_CurrentCameraConfigLocationIndex == 0)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                //}
                                //if (m_CurrentCameraConfigLocationIndex == 2)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                //}
                                //if (m_CurrentCameraConfigLocationIndex == 1)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                //}
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right + m_ptCheck.X;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_M)
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM) //Modify by ChengSk - 20190520
                            {
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                            }
                            //else if(GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR)
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR) //Modify by ChengSk - 20190520
                            {
                                if (m_CurrentCameraConfigLocationIndex == 0)
                                {
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;

                                }
                                if (m_CurrentCameraConfigLocationIndex == 2)
                                {
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 1)
                                {
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                }
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            else
                            {
                                //if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY < 0)
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY = -m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop;
                                //if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY > GlobalDataInterface.globalOut_SysConfig.height / 2)
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY = GlobalDataInterface.globalOut_SysConfig.height / 2 - m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom;
                                //if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX < 0)
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX = -m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0];
                                //if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX > GlobalDataInterface.globalOut_SysConfig.width)
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX = GlobalDataInterface.globalOut_SysConfig.width - m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1];

                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y;
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right + m_ptCheck.X;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop < 0)
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = 0;
                            if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom < 0)
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = 0;



                            //int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                            //int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                            //m_paras.irCameraParas[index].cup[0].nTop = m_rect[0].Top + y;
                            //m_paras.irCameraParas[index].cup[0].nBottom = m_rect[0].Bottom + y;
                            //m_paras.irCameraParas[index].cup[1].nTop = m_rect[1].Top + y;
                            //m_paras.irCameraParas[index].cup[1].nBottom = m_rect[1].Bottom + y;
                            //m_paras.irCameraParas[index].cup[0].nLeft[0] = m_rect[0].Left + x;
                            //m_paras.irCameraParas[index].cup[0].nLeft[1] = m_rect[0].Right + x;
                            //m_paras.irCameraParas[index].cup[1].nLeft[0] = m_rect[1].Left + x;
                            //m_paras.irCameraParas[index].cup[1].nLeft[1] = m_rect[1].Right + x;
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[Index] = int.Parse(this.OffsettextBox.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanR = int.Parse(this.WhiteBalanceRnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanG = int.Parse(this.WhiteBalanceGnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanB = int.Parse(this.WhiteBalanceBnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nDetectionThreshold[Index] = int.Parse(this.DetectionThresholdnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nDetectWhiteTh[Index] = int.Parse(this.DetectWhiteThnumericUpDown.Text);  //Add by ChengSk - 20190726
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nXYEdgeBreakTh[Index] = byte.Parse(this.OverlapThresholdnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fFruitCupRangeTh[Index] = float.Parse(this.OutRangeThresholdnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fPixelRatio[Index] = float.Parse(this.PixleRatiotextBox.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nTriggerDelay = int.Parse(this.TriggerDelaynumericUpDown.Text);
                        }
                        else//红外相机
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8)
                            //{
                            //    if (m_CurrentCameraIndex == 3)
                            //        index = m_CurrentCameraIndex - 2 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //    else
                            //        index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //}
                            //else
                            //    index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //if (this.ImageCorrectioncheckBox.Checked)
                            //{

                            //int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                            //int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                            if (m_CurrentCameraConfigIndex == 2)
                            {

                                //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                                //  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                                //  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR)
                                if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                                  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR) //Modify by ChengSk - 20190520
                                {
                                    //if (m_CurrentCameraConfigLocationIndex == 0)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 2)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 1)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y;
                                    //}
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0] + m_ptCheck.X;
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1] + m_ptCheck.X;
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right + m_ptCheck.X;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F_B)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BM_MM) //Modify by ChengSk - 20190520                   
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520                          
                                {
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                                }
                                else
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right + m_ptCheck.X;
                                }
                            }
                            else
                            {
                                //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                                //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                                //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR)
                                if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                                    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                                    ) //Modify by ChengSk - 20190520
                                {
                                    //if (m_CurrentCameraConfigLocationIndex == 0)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 2)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 1)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y;
                                    //}
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0] + m_ptCheck.X;
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1] + m_ptCheck.X;
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F_B)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BM_MM) //Modify by ChengSk - 20190520                            
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520                           
                                {
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                                }
                                else
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                                }
                            }
                            //m_paras.irCameraParas[index].cup[1].nTop = m_rect[1].Top + y;
                            //m_paras.irCameraParas[index].cup[1].nBottom = m_rect[1].Bottom + y;

                            //m_paras.irCameraParas[index].cup[1].nLeft[0] = m_rect[1].Left + x;
                            //m_paras.irCameraParas[index].cup[1].nLeft[1] = m_rect[1].Right + x;

                            //}
                            //else
                            //{

                            //    m_paras.irCameraParas[index].cup[0].nTop = m_rect[0].Top;
                            //    m_paras.irCameraParas[index].cup[0].nBottom = m_rect[0].Bottom;
                            //    m_paras.irCameraParas[index].cup[1].nTop = m_rect[1].Top;
                            //    m_paras.irCameraParas[index].cup[1].nBottom = m_rect[1].Bottom;
                            //    m_paras.irCameraParas[index].cup[0].nLeft[0] = m_rect[0].Left;
                            //    m_paras.irCameraParas[index].cup[0].nLeft[1] = m_rect[0].Right;
                            //    m_paras.irCameraParas[index].cup[1].nLeft[0] = m_rect[1].Left;
                            //    m_paras.irCameraParas[index].cup[1].nLeft[1] = m_rect[1].Right;
                            //}

                            if (m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop < 0)
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = 0;
                            if (m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom < 0)
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = 0;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nROIOffsetY[Index] = int.Parse(this.OffsettextBox.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nTriggerDelay = int.Parse(this.TriggerDelaynumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nIRDetectionThreshold[Index] = int.Parse(this.DetectionThresholdnumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nXYEdgeBreakTh[Index] = byte.Parse(this.OverlapThresholdnumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fFruitCupRangeTh[Index] = float.Parse(this.OutRangeThresholdnumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fPixelRatio[Index] = float.Parse(this.PixleRatiotextBox.Text);
                        }

                        GlobalDataInterface.globalOut_Paras[m_ChannelRangeSubSysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex].ToCopy(m_paras);
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            int nDrcId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                            GlobalDataInterface.TransmitParam(nDrcId, (int)HC_FSM_COMMAND_TYPE.HC_CMD_PARAS_INFO, null);
                        }
                        return true;
                    }
                    //MessageBox.Show("通道范围设置界面出错：通道未选择！");
                    //MessageBox.Show("0x10001006 Lane Range save error: lane is not selected!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    MessageBox.Show("0x10001006 " + LanguageContainer.ChannelRangeMessagebox4Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("通道范围设置界面出错：" + ex);
                    //MessageBox.Show("0x10001006 Lane Range save error:" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show("0x10001006 " + LanguageContainer.ChannelRangeMessagebox3Text[GlobalDataInterface.selectLanguageIndex] + ex,
                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                try
                {
                    if (m_ChannelRangeSubSysIdx >= 0 && m_ChannelRangeIPMInSysIndex >= 0)
                    {

                        m_paras.nCupNum = int.Parse(this.ChannelRangeCupNumnumericUpDown.Text);
                        if (m_paras.nCupNum > 10) m_paras.nCupNum = 10;
                        if (m_paras.nCupNum < 1) m_paras.nCupNum = 1;
                        int ChannelInIPMIndex = Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_ChannelRangeCurrentChannelIndex]);
                        // int index = 0;
                        //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY = m_rect.nOffsetY;
                        //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX = m_rect.nOffsetX;   //add by xcw 20200602

                        if (m_CurrentCameraConfigIndex == 0) //彩色相机
                        {

                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM) //Modify by ChengSk - 20190520
                            {
                                //if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY < 0)
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY = -m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop;
                                //if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY > GlobalDataInterface.globalOut_SysConfig.height / 2)
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetY = GlobalDataInterface.globalOut_SysConfig.height / 2 - m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom;
                                //if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX < 0)
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX = -m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0];
                                //if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] + m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX > GlobalDataInterface.globalOut_SysConfig.width)
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nOffsetX = GlobalDataInterface.globalOut_SysConfig.width - m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1];
                                //if (m_CurrentCameraConfigLocationIndex == 0)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height / 2;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height / 2;
                                //}
                                //if (m_CurrentCameraConfigLocationIndex == 2)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2 / 2;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2 / 2;
                                //}
                                //if (m_CurrentCameraConfigLocationIndex == 1)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                //}
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right + m_ptCheck.X;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_M)
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM) //Modify by ChengSk - 20190520
                            {
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                            }
                            //else if(GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR)
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR) //Modify by ChengSk - 20190520
                            {
                                if (m_CurrentCameraConfigLocationIndex == 0)
                                {
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;

                                }
                                if (m_CurrentCameraConfigLocationIndex == 2)
                                {
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 1)
                                {
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                }
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            else
                            {
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right + m_ptCheck.X;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }

                            if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop < 0)
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = 0;
                            if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom < 0)
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = 0;



                            //int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                            //int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][0].nTop = m_rect[0].Top + y;
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][0].nBottom = m_rect[0].Bottom + y;
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nTop = m_rect[1].Top + y;
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nBottom = m_rect[1].Bottom + y;
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][0].nLeft[0] = m_rect[0].Left + x;
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][0].nLeft[1] = m_rect[0].Right + x;
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nLeft[0] = m_rect[1].Left + x;
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nLeft[1] = m_rect[1].Right + x;

                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanR = int.Parse(this.WhiteBalanceRnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanG = int.Parse(this.WhiteBalanceGnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanB = int.Parse(this.WhiteBalanceBnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                            //if (m_CurrentCameraConfigLocationIndex == 1)
                            //{
                            //    int ChannelIndex = (ChannelInIPMIndex == 1) ? 0 : 1;
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] = int.Parse(this.OffsettextBox.Text);
                            //}
                            //else
                            //{
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex] = int.Parse(this.OffsettextBox.Text);
                            //}

                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nDetectionThreshold[ChannelInIPMIndex] = int.Parse(this.DetectionThresholdnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nDetectWhiteTh[ChannelInIPMIndex] = int.Parse(this.DetectWhiteThnumericUpDown.Text);  //Add by ChengSk - 20190726
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nXYEdgeBreakTh[ChannelInIPMIndex] = byte.Parse(this.OverlapThresholdnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fFruitCupRangeTh[ChannelInIPMIndex] = float.Parse(this.OutRangeThresholdnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fPixelRatio[ChannelInIPMIndex] = float.Parse(this.PixleRatiotextBox.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nTriggerDelay = int.Parse(this.TriggerDelaynumericUpDown.Text);
                        }
                        else//红外相机
                        {

                            if (m_CurrentCameraConfigIndex == 2)
                            {

                                if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                                  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR) //Modify by ChengSk - 20190520
                                {
                                    //if (m_CurrentCameraConfigLocationIndex == 0)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 2)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 1)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + m_ptCheck.Y;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + m_ptCheck.Y;
                                    //}
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] + m_ptCheck.X;
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] + m_ptCheck.X;
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right + m_ptCheck.X;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F_B)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BM_MM) //Modify by ChengSk - 20190520                   
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520                          
                                {
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                                }
                                else
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right + m_ptCheck.X;
                                }
                            }
                            else
                            {
                                //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                                //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                                //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR)
                                if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                                    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR) //Modify by ChengSk - 20190520
                                {
                                    //if (m_CurrentCameraConfigLocationIndex == 0)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 2)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 1)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop + m_ptCheck.Y;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom + m_ptCheck.Y;
                                    //}
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] + m_ptCheck.X;
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] + m_ptCheck.X;
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;

                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F_B)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BM_MM) //Modify by ChengSk - 20190520                            
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520                           
                                {
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                                }
                                else
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                                }
                            }
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nTop = m_rect[1].Top + y;
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nBottom = m_rect[1].Bottom + y;

                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nLeft[0] = m_rect[1].Left + x;
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nLeft[1] = m_rect[1].Right + x;

                            //}
                            //else
                            //{

                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][0].nTop = m_rect[0].Top;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][0].nBottom = m_rect[0].Bottom;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nTop = m_rect[1].Top;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nBottom = m_rect[1].Bottom;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][0].nLeft[0] = m_rect[0].Left;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][0].nLeft[1] = m_rect[0].Right;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nLeft[0] = m_rect[1].Left;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nLeft[1] = m_rect[1].Right;
                            //}

                            if (m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop < 0)
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = 0;
                            if (m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom < 0)
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = 0;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nTriggerDelay = int.Parse(this.TriggerDelaynumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                            //if (m_CurrentCameraConfigLocationIndex == 1)
                            //{
                            //    int ChannelIndex = (ChannelInIPMIndex == 1) ? 0 : 1;
                            //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] = int.Parse(this.OffsettextBox.Text);
                            //}
                            //else
                            //{
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex] = int.Parse(this.OffsettextBox.Text);
                            //}

                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nIRDetectionThreshold[ChannelInIPMIndex] = int.Parse(this.DetectionThresholdnumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nXYEdgeBreakTh[ChannelInIPMIndex] = byte.Parse(this.OverlapThresholdnumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fFruitCupRangeTh[ChannelInIPMIndex] = float.Parse(this.OutRangeThresholdnumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fPixelRatio[ChannelInIPMIndex] = float.Parse(this.PixleRatiotextBox.Text);
                        }

                        GlobalDataInterface.globalOut_Paras[m_ChannelRangeSubSysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex/*(m_ChannelRangeIPMInSysIndex >> 1)*/].ToCopy(m_paras);
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            int nDrcId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                            //int nDrcId= Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                            GlobalDataInterface.TransmitParam(nDrcId, (int)HC_FSM_COMMAND_TYPE.HC_CMD_PARAS_INFO, null);
                        }
                        return true;
                    }
                    //MessageBox.Show("通道范围设置界面出错：通道未选择！");
                    //MessageBox.Show("0x10001006 Lane Range save error: lane is not selected!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    MessageBox.Show("0x10001006 " + LanguageContainer.ChannelRangeMessagebox4Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("通道范围设置界面出错：" + ex);
                    //MessageBox.Show("0x10001006 Lane Range save error:" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show("0x10001006 " + LanguageContainer.ChannelRangeMessagebox3Text[GlobalDataInterface.selectLanguageIndex] + ex,
                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

        }

        /// <summary>
        /// 保存设置（另存专用，不给FSM发送指令）    Add by ChengSk - 20190116
        /// </summary>
        /// <returns></returns>
        private bool ChannelRangeSaveConfig2()
        {
            if (GlobalDataInterface.nVer == 0)
            {
                try
                {
                    if (m_ChannelRangeSubSysIdx >= 0 && m_ChannelRangeIPMInSysIndex >= 0)
                    {
                        //m_paras.nDefaultDetectionThreshold = int.Parse(this.DetectionThresholdnumericUpDown.Text);

                        //m_paras.fSeedPointRange = int.Parse(this.SeedPointRangenumericUpDown.Text);
                        m_paras.nCupNum = int.Parse(this.ChannelRangeCupNumnumericUpDown.Text);
                        if (m_paras.nCupNum > 10) m_paras.nCupNum = 10;
                        if (m_paras.nCupNum < 1) m_paras.nCupNum = 1;
                        int ChannelInIPMIndex = Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_ChannelRangeCurrentChannelIndex]);
                        //int index = 0;
                        if (m_CurrentCameraConfigIndex == 0) //彩色相机
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8)
                            //{
                            //    if (m_CurrentCameraIndex == 2)
                            //        index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //    else
                            //        index = m_CurrentCameraIndex + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //}
                            //else
                            //    index = m_CurrentCameraIndex + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM) //Modify by ChengSk - 20190520 
                            {
                                //if (m_CurrentCameraConfigLocationIndex == 0)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;

                                //}
                                //if (m_CurrentCameraConfigLocationIndex == 2)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                //}
                                //if (m_CurrentCameraConfigLocationIndex == 1)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                //}
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right + m_ptCheck.X;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_M)
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM) //Modify by ChengSk - 20190520                       
                            {
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                            }
                            //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR)
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR) //Modify by ChengSk - 20190520                       
                            {
                                if (m_CurrentCameraConfigLocationIndex == 0)
                                {
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;

                                }
                                if (m_CurrentCameraConfigLocationIndex == 2)
                                {
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                }
                                if (m_CurrentCameraConfigLocationIndex == 1)
                                {
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                }
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            else
                            {
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right + m_ptCheck.X;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop < 0)
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nTop = 0;
                            if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom < 0)
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = 0;



                            //int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                            //int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                            //m_paras.irCameraParas[index].cup[0].nTop = m_rect[0].Top + y;
                            //m_paras.irCameraParas[index].cup[0].nBottom = m_rect[0].Bottom + y;
                            //m_paras.irCameraParas[index].cup[1].nTop = m_rect[1].Top + y;
                            //m_paras.irCameraParas[index].cup[1].nBottom = m_rect[1].Bottom + y;
                            //m_paras.irCameraParas[index].cup[0].nLeft[0] = m_rect[0].Left + x;
                            //m_paras.irCameraParas[index].cup[0].nLeft[1] = m_rect[0].Right + x;
                            //m_paras.irCameraParas[index].cup[1].nLeft[0] = m_rect[1].Left + x;
                            //m_paras.irCameraParas[index].cup[1].nLeft[1] = m_rect[1].Right + x;
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[Index] = int.Parse(this.OffsettextBox.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanR = int.Parse(this.WhiteBalanceRnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanG = int.Parse(this.WhiteBalanceGnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanB = int.Parse(this.WhiteBalanceBnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nDetectionThreshold[Index] = int.Parse(this.DetectionThresholdnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nDetectWhiteTh[Index] = int.Parse(this.DetectWhiteThnumericUpDown.Text);  //Add by ChengSk - 20190726
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nXYEdgeBreakTh[Index] = byte.Parse(this.OverlapThresholdnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fFruitCupRangeTh[Index] = float.Parse(this.OutRangeThresholdnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fPixelRatio[Index] = float.Parse(this.PixleRatiotextBox.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nTriggerDelay = int.Parse(this.TriggerDelaynumericUpDown.Text);
                        }
                        else//红外相机
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8)
                            //{
                            //    if (m_CurrentCameraIndex == 3)
                            //        index = m_CurrentCameraIndex - 2 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //    else
                            //        index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //}
                            //else
                            //    index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //if (this.ImageCorrectioncheckBox.Checked)
                            //{

                            //int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                            //int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                            if (m_CurrentCameraConfigIndex == 2)
                            {
                                //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                                //  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                                //  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR)
                                if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                                  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR) //Modify by ChengSk - 20190520
                                {
                                    //if (m_CurrentCameraConfigLocationIndex == 0)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 2)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 1)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y;
                                    //}
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0] + m_ptCheck.X;
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1] + m_ptCheck.X;
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right + m_ptCheck.X;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F_B)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BM_MM) //Modify by ChengSk - 20190520                            
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520                      
                                {
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                                }
                                else
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top + m_ptCheck.Y;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right + m_ptCheck.X;
                                }
                            }
                            else
                            {
                                //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                                //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                                //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR)
                                if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                                    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR) //Modify by ChengSk - 20190520
                                {
                                    //if (m_CurrentCameraConfigLocationIndex == 0)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 2)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 1)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y;
                                    //}
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0] + m_ptCheck.X;
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[1] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[1] + m_ptCheck.X;
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F_B)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BM_MM) //Modify by ChengSk - 20190520                            
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520                       
                                {
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                                }
                                else
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = m_rect.Top;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = m_rect.Bottom;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nLeft[1] = m_rect.Right;
                                }
                            }
                            //m_paras.irCameraParas[index].cup[1].nTop = m_rect[1].Top + y;
                            //m_paras.irCameraParas[index].cup[1].nBottom = m_rect[1].Bottom + y;

                            //m_paras.irCameraParas[index].cup[1].nLeft[0] = m_rect[1].Left + x;
                            //m_paras.irCameraParas[index].cup[1].nLeft[1] = m_rect[1].Right + x;

                            //}
                            //else
                            //{

                            //    m_paras.irCameraParas[index].cup[0].nTop = m_rect[0].Top;
                            //    m_paras.irCameraParas[index].cup[0].nBottom = m_rect[0].Bottom;
                            //    m_paras.irCameraParas[index].cup[1].nTop = m_rect[1].Top;
                            //    m_paras.irCameraParas[index].cup[1].nBottom = m_rect[1].Bottom;
                            //    m_paras.irCameraParas[index].cup[0].nLeft[0] = m_rect[0].Left;
                            //    m_paras.irCameraParas[index].cup[0].nLeft[1] = m_rect[0].Right;
                            //    m_paras.irCameraParas[index].cup[1].nLeft[0] = m_rect[1].Left;
                            //    m_paras.irCameraParas[index].cup[1].nLeft[1] = m_rect[1].Right;
                            //}

                            if (m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop < 0)
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nTop = 0;
                            if (m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom < 0)
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[Index].nBottom = 0;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nROIOffsetY[Index] = int.Parse(this.OffsettextBox.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nTriggerDelay = int.Parse(this.TriggerDelaynumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nIRDetectionThreshold[Index] = int.Parse(this.DetectionThresholdnumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nXYEdgeBreakTh[Index] = byte.Parse(this.OverlapThresholdnumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fFruitCupRangeTh[Index] = float.Parse(this.OutRangeThresholdnumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fPixelRatio[Index] = float.Parse(this.PixleRatiotextBox.Text);
                        }

                        GlobalDataInterface.globalOut_Paras[m_ChannelRangeSubSysIdx * ConstPreDefine.MAX_IPM_NUM + m_ChannelRangeIPMInSysIndex].ToCopy(m_paras);
                        //if (GlobalDataInterface.global_IsTestMode)
                        //{
                        //    int nDrcId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                        //    GlobalDataInterface.TransmitParam(nDrcId, (int)HC_FSM_COMMAND_TYPE.HC_CMD_PARAS_INFO, null);
                        //} //Note by ChengSk - 20190116
                        return true;
                    }
                    //MessageBox.Show("通道范围设置界面出错：通道未选择！");
                    //MessageBox.Show("0x10001006 Lane Range save error: lane is not selected!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    MessageBox.Show("0x10001006 " + LanguageContainer.ChannelRangeMessagebox4Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("通道范围设置界面出错：" + ex);
                    //MessageBox.Show("0x10001006 Lane Range save error:" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show("0x10001006 " + LanguageContainer.ChannelRangeMessagebox3Text[GlobalDataInterface.selectLanguageIndex] + ex,
                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                try
                {
                    if (m_ChannelRangeSubSysIdx >= 0 && m_ChannelRangeIPMInSysIndex >= 0)
                    {
                        //m_paras.nDefaultDetectionThreshold = int.Parse(this.DetectionThresholdnumericUpDown.Text);

                        //m_paras.fSeedPointRange = int.Parse(this.SeedPointRangenumericUpDown.Text);
                        m_paras.nCupNum = int.Parse(this.ChannelRangeCupNumnumericUpDown.Text);
                        if (m_paras.nCupNum > 10) m_paras.nCupNum = 10;
                        if (m_paras.nCupNum < 1) m_paras.nCupNum = 1;
                        int ChannelInIPMIndex = Commonfunction.ChanelInIPMIndex(m_ChanelIDList[m_ChannelRangeCurrentChannelIndex]);
                        //int index = 0;
                        if (m_CurrentCameraConfigIndex == 0) //彩色相机
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8)
                            //{
                            //    if (m_CurrentCameraIndex == 2)
                            //        index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //    else
                            //        index = m_CurrentCameraIndex + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //}
                            //else
                            //    index = m_CurrentCameraIndex + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR
                            //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                            if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR
                                || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM_FM) //Modify by ChengSk - 20190520  //Modify by xcw - 20200828

                            {
                                //if (m_CurrentCameraConfigLocationIndex == 0)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;

                                //}
                                //if (m_CurrentCameraConfigLocationIndex == 2)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                //}
                                //if (m_CurrentCameraConfigLocationIndex == 1)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                //}
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right + m_ptCheck.X;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_M)
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FM) //Modify by ChengSk - 20190520                       
                            {
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                            }
                            //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR)
                            else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR) //Modify by ChengSk - 20190520                       
                            {
                                //if (m_CurrentCameraConfigLocationIndex == 0)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;

                                //}
                                //if (m_CurrentCameraConfigLocationIndex == 2)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                //}
                                //if (m_CurrentCameraConfigLocationIndex == 1)
                                //{
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                //}
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                //m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            else
                            {
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right + m_ptCheck.X;//by 2015-4-17 去掉分隔线，只显示果杯外框
                            }
                            if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop < 0)
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = 0;
                            if (m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom < 0)
                                m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = 0;



                            //int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                            //int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                            //m_paras.irCameraParas[index].cup[0].nTop = m_rect[0].Top + y;
                            //m_paras.irCameraParas[index].cup[0].nBottom = m_rect[0].Bottom + y;
                            //m_paras.irCameraParas[index].cup[1].nTop = m_rect[1].Top + y;
                            //m_paras.irCameraParas[index].cup[1].nBottom = m_rect[1].Bottom + y;
                            //m_paras.irCameraParas[index].cup[0].nLeft[0] = m_rect[0].Left + x;
                            //m_paras.irCameraParas[index].cup[0].nLeft[1] = m_rect[0].Right + x;
                            //m_paras.irCameraParas[index].cup[1].nLeft[0] = m_rect[1].Left + x;
                            //m_paras.irCameraParas[index].cup[1].nLeft[1] = m_rect[1].Right + x;

                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanR = int.Parse(this.WhiteBalanceRnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanG = int.Parse(this.WhiteBalanceGnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].MeanValue.MeanB = int.Parse(this.WhiteBalanceBnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                            //if (m_CurrentCameraConfigLocationIndex == 1)
                            //{
                            //    int ChannelIndex = (ChannelInIPMIndex == 1) ? 0 : 1;
                            //    m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] = int.Parse(this.OffsettextBox.Text);
                            //}
                            //else
                            //{
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex] = int.Parse(this.OffsettextBox.Text);
                            //}


                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nDetectionThreshold[ChannelInIPMIndex] = int.Parse(this.DetectionThresholdnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nDetectWhiteTh[ChannelInIPMIndex] = int.Parse(this.DetectWhiteThnumericUpDown.Text);  //Add by ChengSk - 20190726
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nXYEdgeBreakTh[ChannelInIPMIndex] = byte.Parse(this.OverlapThresholdnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fFruitCupRangeTh[ChannelInIPMIndex] = float.Parse(this.OutRangeThresholdnumericUpDown.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].fPixelRatio[ChannelInIPMIndex] = float.Parse(this.PixleRatiotextBox.Text);
                            m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].nTriggerDelay = int.Parse(this.TriggerDelaynumericUpDown.Text);
                        }
                        else//红外相机
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == 8)
                            //{
                            //    if (m_CurrentCameraIndex == 3)
                            //        index = m_CurrentCameraIndex - 2 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //    else
                            //        index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //}
                            //else
                            //    index = m_CurrentCameraIndex - 1 + ChannelInIPMIndex * ConstPreDefine.CAMERA_TYPE_NUM;
                            //if (this.ImageCorrectioncheckBox.Checked)
                            //{

                            //int x = m_ptCheck.X * m_ImageWidth / this.ImagepictureBox.Width;
                            //int y = m_ptCheck.Y * m_ImageHeight / this.ImagepictureBox.Height;
                            if (m_CurrentCameraConfigIndex == 2)
                            {
                                //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                                //  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                                //  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR)
                                if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                                  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                  || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR) //Modify by ChengSk - 20190520
                                {
                                    //if (m_CurrentCameraConfigLocationIndex == 0)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 2)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    //}
                                    //if (m_CurrentCameraConfigLocationIndex == 1)
                                    //{
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nTop = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nTop + m_ptCheck.Y;
                                    //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nBottom = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nBottom + m_ptCheck.Y;
                                    //}
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup.nLeft[0] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup.nLeft[0] + m_ptCheck.X;
                                    //m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_paras.cameraParas[m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] + m_ptCheck.X;
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right + m_ptCheck.X;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F_B)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BM_MM) //Modify by ChengSk - 20190520                            
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520                      
                                {
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                                }
                                else
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top + m_ptCheck.Y;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom + m_ptCheck.Y;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left + m_ptCheck.X;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right + m_ptCheck.X;
                                }
                            }
                            else
                            {
                                //if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LMR
                                //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR
                                //    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LMR_LBR)
                                if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_FLMR
                                    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR_FLMR
                                    || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR_FLMR) //Modify by ChengSk - 20190520
                                {

                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_F_B)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MM || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BM_MM) //Modify by ChengSk - 20190520                            
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                                }
                                //else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM_LFR_LBR)
                                else if (GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_MLMR || GlobalDataInterface.globalOut_SysConfig.nSystemInfo == ConstPreDefine.RM2_BLMR_MLMR) //Modify by ChengSk - 20190520                       
                                {
                                    if (m_CurrentCameraConfigLocationIndex == 0)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 2)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom - GlobalDataInterface.globalOut_SysConfig.height * 2;
                                    }
                                    if (m_CurrentCameraConfigLocationIndex == 1)
                                    {
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                        m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                    }
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                                }
                                else
                                {
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = m_rect.Top;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = m_rect.Bottom;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[0] = m_rect.Left;
                                    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nLeft[1] = m_rect.Right;
                                }
                            }
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nTop = m_rect[1].Top + y;
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nBottom = m_rect[1].Bottom + y;

                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nLeft[0] = m_rect[1].Left + x;
                            //m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nLeft[1] = m_rect[1].Right + x;

                            //}
                            //else
                            //{

                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][0].nTop = m_rect[0].Top;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][0].nBottom = m_rect[0].Bottom;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nTop = m_rect[1].Top;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nBottom = m_rect[1].Bottom;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][0].nLeft[0] = m_rect[0].Left;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][0].nLeft[1] = m_rect[0].Right;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nLeft[0] = m_rect[1].Left;
                            //    m_paras.irCameraParas[index].cup[ChannelInIPMIndex][1].nLeft[1] = m_rect[1].Right;
                            //}

                            if (m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop < 0)
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nTop = 0;
                            if (m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom < 0)
                                m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].cup[ChannelInIPMIndex].nBottom = 0;
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nTriggerDelay = int.Parse(this.TriggerDelaynumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fGammaCorrection = float.Parse(this.GammanumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nShutter = int.Parse(this.ShutternumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex] = int.Parse(this.OffsettextBox.Text);
                            //if (m_CurrentCameraConfigLocationIndex == 1)
                            //{
                            //    int ChannelIndex = (ChannelInIPMIndex == 1) ? 0 : 1;
                            //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelIndex] = int.Parse(this.OffsettextBox.Text);
                            //}
                            //else
                            //{
                            //    m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nROIOffsetY[ChannelInIPMIndex] = int.Parse(this.OffsettextBox.Text);
                            //}
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nIRDetectionThreshold[ChannelInIPMIndex] = int.Parse(this.DetectionThresholdnumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].nXYEdgeBreakTh[ChannelInIPMIndex] = byte.Parse(this.OverlapThresholdnumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fFruitCupRangeTh[ChannelInIPMIndex] = float.Parse(this.OutRangeThresholdnumericUpDown.Text);
                            m_paras.irCameraParas[(m_CurrentCameraConfigIndex - 1) * ConstPreDefine.MAX_CAMERA_DIRECTION + m_CurrentCameraConfigLocationIndex].fPixelRatio[ChannelInIPMIndex] = float.Parse(this.PixleRatiotextBox.Text);
                        }

                        GlobalDataInterface.globalOut_Paras[m_ChannelRangeSubSysIdx * ConstPreDefine.MAX_IPM_NUM + (m_ChannelRangeIPMInSysIndex)].ToCopy(m_paras);

                        return true;
                    }
                    //MessageBox.Show("通道范围设置界面出错：通道未选择！");
                    //MessageBox.Show("0x10001006 Lane Range save error: lane is not selected!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    MessageBox.Show("0x10001006 " + LanguageContainer.ChannelRangeMessagebox4Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("通道范围设置界面出错：" + ex);
                    //MessageBox.Show("0x10001006 Lane Range save error:" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show("0x10001006 " + LanguageContainer.ChannelRangeMessagebox3Text[GlobalDataInterface.selectLanguageIndex] + ex,
                        LanguageContainer.ChannelRangeMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
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
                    m_BottomImage.Save(dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数SaveImagebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数SaveImagebutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 通道范围设置页面被选择
        /// </summary>
        private void ChannelRangeSetPageSelected()
        {

            try
            {
                //打开当前通道水果检测数据上传
                if (m_ChannelRangeCurrentChannelIndex >= 0)
                {
                    int nDstId = m_ChanelIDList[m_ChannelRangeCurrentChannelIndex];
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelRange中函数ChannelRangeSetPageSelected出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelRange中函数ChannelRangeSetPageSelected出错" + ex);
#endif
            }




        }

    }
}
