using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ListViewEx;
using GlacialComponents.Controls;
using Interface;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Resources;
using SuperSocket.ClientEngine;
using System.Threading;
using System.Net;
using Common;
using System.Net.NetworkInformation;

namespace FruitSortingVtest1._0
{
    public partial class ProjectSetForm : Form
    {
        private static MainForm m_mainForm;
        private static int[] m_ChannelNum = new int[ConstPreDefine.MAX_SUBSYS_NUM];//每个子系统通道数
        private static List<int> m_ChanelIDList = new List<int>();
        private static List<int> m_ChanelExitList = new List<int>(); //add by xcw 20201204
        private ResourceManager m_resourceManager = new ResourceManager(typeof(ProjectSetForm));//创建ProjectSetForm资源管理
        [DllImport("user32.dll", EntryPoint = "EnableWindow")]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        AutoSizeFormClass asc = new AutoSizeFormClass();//声明大小自适应类实例  
        Rectangle m_InitialPictureBoxRectangle;//通道范围初始设置图片大小
        private float m_fTransferRatio;//通道范围图像缩放比
        //bool IsChannelRangePageSwitch = false;//通道范围防止刚切换页面刷新图像
        //bool m_tabselectInvalid = true;

        private AsyncTcpSession tcpClient = new AsyncTcpSession();  //Add by ChengSk - 20190116
        private Thread m_SendDataThread;   //Add by ChengSk - 20190117
        private int m_TcpChannelIndex = 0; //Add by ChengSk - 20190117
        private static int m_CameraDelaylistViewExDownType = 0;//“相机延时参数列表”鼠标按下类型，1左键双击；0为不是左键双击
        private Control[] CameraDelayEditors;
        public static int sign = GlobalDataInterface.globalIn_defaultInis.FirstOrDefault().nFsmModule;
       public ProjectSetForm(MainForm mainForm)
        {
            InitializeComponent();
            m_mainForm = mainForm;
            m_mainForm.SetProjectEnabledtoolStripMenuItem(false);//Add by ChengSk - 20190121 
            //GlobalDataInterface.OpenProjectSetFormNumber++; //Add by ChengSk - 20190111
            GlobalDataInterface.UpWeightInfoEvent += new GlobalDataInterface.WeightInfoEventHandler(OnUpWeightInfo);
            GlobalDataInterface.UpImageDataEvent += new GlobalDataInterface.ImageDataEventHandler(OnUpImageData);
            GlobalDataInterface.UpAutoWhiteBalanceInfoEvent += new GlobalDataInterface.AutoWhiteBalanceInfoEventHandler(OnUpAutoWhiteBalanceInfo);
            GlobalDataInterface.UpShutterAdjustInfoEvent += new GlobalDataInterface.ShutterAdjustInfoEventHandler(OnShutterAdjustInfo);
            GlobalDataInterface.UpFruitGradeInfoEvent += new GlobalDataInterface.FruitGradeInfoEventHandler(OnUpFruitGradeInfo);
            asc.controllInitializeSize(this);
            sign = GlobalDataInterface.globalIn_defaultInis.FirstOrDefault().nFsmModule;
            button1.Enabled = sign != 1;
            //HeightnumericUpDown.Maximum = GlobalDataInterface.nVer == 0 ? 400 : 1024;
        }

        private void ProjectSetForm_Load(object sender, EventArgs e)
        {
            CameraDelayEditors = new Control[] { CameraDelaynumericUpDown };

            m_InitialPictureBoxRectangle.X = this.ImagepictureBox.Location.X; //10
            m_InitialPictureBoxRectangle.Y = this.ImagepictureBox.Location.Y; //34
            m_InitialPictureBoxRectangle.Width = this.ImagepictureBox.Width;  //512
            m_InitialPictureBoxRectangle.Height = this.ImagepictureBox.Height; //384
            float fWidth = (float)this.ImagepictureBox.Width / GlobalDataInterface.globalOut_SysConfig.width;
            float fHeight = (float)this.ImagepictureBox.Height / GlobalDataInterface.globalOut_SysConfig.height;
            //if (GlobalDataInterface.nVer == 0)
            //{
            //    //HeightnumericUpDown.Maximum = 400;
            //    this.ImageOffsetgroupBox.Enabled = false;
            //}
            //else if(GlobalDataInterface.nVer == 1)
            //{
            //    //HeightnumericUpDown.Maximum = 1024;
            //    this.ImageOffsetgroupBox.Enabled = true;
            //}
            m_fTransferRatio = (fWidth > fHeight ? fHeight : fWidth);//全局缩放取比较小的那个
            Reload(0);
            GlobalDataInterface.ProjectSetFormIsIntialed = true;
            sign = GlobalDataInterface.globalIn_defaultInis.FirstOrDefault().nFsmModule;
            button1.Enabled = sign != 1;

            try
            {
                tcpClient.Connected += TcpClient_Connected;
                tcpClient.DataReceived += TcpClient_DataReceived;
                tcpClient.Error += TcpClient_Error;

                if (m_SendDataThread == null)
                {
                    m_SendDataThread = new Thread(SendDataThread);
                    m_SendDataThread.Priority = ThreadPriority.Normal;
                    m_SendDataThread.IsBackground = true;
                    m_SendDataThread.Start();
                } //Add by ChengSk - 20190117

                //可以在此处添加内部品质访问的代码
                //逐通道向后获取光谱仪信息（通道1还要获取基础信息）
                //收到通道1的完整信息之后再次刷新界面
                //光谱仪信息通道选择以获取其它通道的信息（若无消息，则显示默认值）
                if (m_ChanelIDList.Count > 0 && GlobalDataInterface.InternalAvailable == true)
                {
                    StartReceivedInnerQualityInfo(m_ChanelIDList.Count);  //仅在通道不为零的情况下才接收内部品质信息
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中内部品质事件出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中内部品质事件出错" + ex);
#endif
            }
        }

        public void Reload(int IsSys)
        {
            try
            {
                if (IsSys == 0)
                    SystemStructInitial();
                ChannelExitIntial();
                WeightSetInitial(IsSys);          
                ChannelRangeInitial();
                FruitSetInitial();
                InnerQualityIntial(); //Add by ChengSk - 20190114
                
                //FlawSetIntial();    //瑕疵功能待确定
                ProjecttabControl.TabPages.Remove(this.FlawSettabPage);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数Reload出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数Reload出错" + ex);
#endif
            }
        }

        public void Reload2(int IsSys)  //仅“工程配置”加载后刷新调用 Add by ChengSk - 20190827
        {
            try
            {
                //if (IsSys == 0)
                SystemStructInitial();
                ChannelExitIntial();
                WeightSetInitial(IsSys);
                ChannelRangeInitial();
                if (GlobalDataInterface.global_IsTestMode)
                {
                    stCameraNum cameraNum = new stCameraNum(true);
                    cameraNum.cCameraNum = (byte)m_CameraIndex;
                    int nDrcId = 0;
                    if (m_ChannelRangeSubSysIdx >= 0 && m_ChannelRangeIPMInSysIndex >= 0)
                    {
                        if (GlobalDataInterface.nVer == 0)            //版本号判断 add by xcw 20200604
                        {
                            nDrcId = Commonfunction.EncodeIPM(m_ChannelRangeSubSysIdx, m_ChannelRangeIPMInSysIndex);
                        }
                        else if (GlobalDataInterface.nVer == 1)
                        {
                            nDrcId = Commonfunction.EncodeIPMChannel(m_ChannelRangeSubSysIdx, m_ChannelRangeChannelInIPMIndex + m_ChannelRangeIPMInSysIndex * ConstPreDefine.CHANNEL_NUM);
                        }
                    }  // add by xcw 20200715
                    cameraNum.cCameraNum = (byte)m_CameraIndex;
                    ContinuousSamplecheckBox.Checked = false;
                    ShowBlobcheckBox.Checked = false;
                    ImageCorrectioncheckBox.Checked = false;
                    GlobalDataInterface.TransmitParam(nDrcId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_OFF, cameraNum);
                }
                FruitSetInitial();
                InnerQualityIntial(); //Add by ChengSk - 20190114

                //FlawSetIntial();    //瑕疵功能待确定
                ProjecttabControl.TabPages.Remove(this.FlawSettabPage);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数Reload出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数Reload出错" + ex);
#endif
            }
        }

        int m_preTabSelectedIndex = 0;
        /// <summary>
        /// Tab页面切换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjecttabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                TabControl tab = (TabControl)sender;
                switch (tab.SelectedIndex)
                {
                    //系统结构设置
                    case 0:
                        if (m_preTabSelectedIndex == 3)
                            WeightSetPageUnSelected();
                        if (m_preTabSelectedIndex == 1)
                            CloseTest();
                        if (m_preTabSelectedIndex == 2)
                            StopVolveTest();
                        m_preTabSelectedIndex = 0;
                        EnableWindow(m_mainForm.Handle, false);
                        break;
                    //通道范围设置
                    case 1:
                        if (m_preTabSelectedIndex == 3)
                            WeightSetPageUnSelected();
                        if (m_preTabSelectedIndex == 2)
                            StopVolveTest();
                        ChannelRangeSetPageSelected();
                        m_preTabSelectedIndex = 1;
                        EnableWindow(m_mainForm.Handle, false);
                       // IsChannelRangePageSwitch = true;
                        break;
                    //通道出口设置
                    case 2:
                        if (m_preTabSelectedIndex == 3)
                            WeightSetPageUnSelected();
                        if (m_preTabSelectedIndex == 1)
                            CloseTest();
                        //系统结构设置界面子系统数目设置影响通道出口界面电机管角设置
                        if (m_preTabSelectedIndex == 0)
                            IsEnableExitMotorDriverPinlistViewEx();
                        m_preTabSelectedIndex = 2;
                        EnableWindow(m_mainForm.Handle, true);
                        //EnableWindow(m_mainForm.Handle, false); //Modify by ChengSk - 20190111
                        break;
                    //重量设置
                    case 3:
                        if (m_preTabSelectedIndex == 1)
                            CloseTest();
                        if (m_preTabSelectedIndex == 2)
                            StopVolveTest();
                        WeightSetPageSelected();
                        m_preTabSelectedIndex = 3;
                        EnableWindow(m_mainForm.Handle, false);
                        break;
                    case 4:
                        if (m_preTabSelectedIndex == 3)
                            WeightSetPageUnSelected();
                        if (m_preTabSelectedIndex == 1)
                            CloseTest();
                        if (m_preTabSelectedIndex == 2)
                            StopVolveTest();
                        m_preTabSelectedIndex = 4;
                        EnableWindow(m_mainForm.Handle, false);
                        break;
                    //内部品质设置
                    case 5:
                        if (m_preTabSelectedIndex == 3)
                            WeightSetPageUnSelected();
                        if (m_preTabSelectedIndex == 1)
                            CloseTest();
                        if (m_preTabSelectedIndex == 2)
                            StopVolveTest();
                        m_preTabSelectedIndex = 5;
                        EnableWindow(m_mainForm.Handle, false);
                        break;
                    default: break;

                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数ProjecttabControl_SelectedIndexChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数ProjecttabControl_SelectedIndexChanged出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 加载控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadConfigbutton_Click(object sender, EventArgs e)
        {
            LoadConfigNewForm lcForm = new LoadConfigNewForm(m_mainForm, this, true);
            lcForm.ShowDialog();

        }
        /// <summary>
        /// 另存控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveConfigbutton_Click(object sender, EventArgs e)
        {
            try
            {
                //Note by ChengSk - 20190116
                //if (!SystemStructSaveConfig())
                //    return;
                //if (!ChannelRangeSaveConfig())
                //    return;
                //if (!ChannelExitSaveConfig())
                //    return;
                //if (!ExitMotorDriverPinSaveConfig())
                //    return;
                //if (!WeightSaveConfig(false))
                //    return;
                ////if (!FlawSetSaveConfig())
                ////    return;

                //Modify by ChengSk - 20190116
                if (!SystemStructSaveConfig2())
                    return;
                if (!ChannelRangeSaveConfig2())
                    return;
                if (!ChannelExitSaveConfig2())
                    return;
                if (!ExitMotorDriverPinSaveConfig2())
                    return;
                if (!WeightSaveConfig2(false))
                    return;

                SaveConfigForm scForm = new SaveConfigForm(null, true);
                scForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数SaveConfigbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数SaveConfigbutton_Click出错" + ex);
#endif
            }
        }

        private void ProjectSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //EnableWindow(m_mainForm.Handle, false);//ivycc 2013.11.29
                //DialogResult result = MessageBox.Show("是否保存配置信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                //DialogResult result = MessageBox.Show("0x30001001 Whether to preseve the configuration information?", "Information", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                DialogResult result = MessageBox.Show("0x30001001 " + LanguageContainer.ProjectSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.ProjectSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)//cancel
                {
                    e.Cancel = true;
                    return;
                }
                else if (result == DialogResult.No)//no
                {
                    GlobalDataInterface.UpWeightInfoEvent -= new GlobalDataInterface.WeightInfoEventHandler(OnUpWeightInfo); //Add by ChengSk - 20180830
                    GlobalDataInterface.UpImageDataEvent -= new GlobalDataInterface.ImageDataEventHandler(OnUpImageData);    //Add by ChengSk - 20180830
                    GlobalDataInterface.UpAutoWhiteBalanceInfoEvent -= new GlobalDataInterface.AutoWhiteBalanceInfoEventHandler(OnUpAutoWhiteBalanceInfo); //Add by ChengSk - 20180830
                    GlobalDataInterface.UpShutterAdjustInfoEvent -= new GlobalDataInterface.ShutterAdjustInfoEventHandler(OnShutterAdjustInfo);
                    GlobalDataInterface.UpFruitGradeInfoEvent -= new GlobalDataInterface.FruitGradeInfoEventHandler(OnUpFruitGradeInfo);//Add by ChengSk - 20180830
                    //add by xcw 20201207
                    m_mainForm.SetProjectEnabledtoolStripMenuItem(true); //Add by ChengSk - 20190121
                    //GlobalDataInterface.OpenProjectSetFormNumber--; //Add by ChengSk - 20190111
                    //if (!SystemStructSaveConfig())
                    //    return;
                    //if (!ChannelRangeSaveConfig())
                    //    return;
                    //if (!ChannelExitSaveConfig())
                    //    return;
                    //if (!WeightSaveConfig(false))
                    //    return;
                    //if (!FlawSetSaveConfig())
                    //    return;
                    WeightSetPageUnSelected();
                    CloseTest();
                    StopVolveTest();
                    //水平滚动条初始化 //Add by ChengSk - 20180408
                    m_mainForm.splitContainer2_Panel1_HorizontalScroll_init(); 
                    //出口列表操作
                    m_mainForm.InitExitListBox(true);
                    m_mainForm.SetAllExitListBox();
                    if (GlobalDataInterface.global_IsTestMode)
                        GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_PROJ_CLOSED, null);
                    return;
                }
                else if(result == DialogResult.Yes)//yes
                {
                    m_mainForm.SetProjectEnabledtoolStripMenuItem(true); //Add by ChengSk - 20190121
                    //GlobalDataInterface.OpenProjectSetFormNumber--; //Add by ChengSk - 20190111
                    
                    //Note by ChengSk - 20190116
                    //if (!SystemStructSaveConfig())
                    //    return;
                    //if (!ChannelRangeSaveConfig())
                    //    return;
                    //if (!ChannelExitSaveConfig())
                    //    return;
                    //if (!ExitMotorDriverPinSaveConfig())
                    //    return;
                    //if (!WeightSaveConfig(false))
                    //    return;
                    ////if (!FlawSetSaveConfig())
                    ////    return;

                    //Modify by ChengSk - 20190116
                    if (!SystemStructSaveConfig2())
                        return;
                    if (!ChannelRangeSaveConfig2())
                        return;
                    if (!ChannelExitSaveConfig2())
                        return;
                    if (!ExitMotorDriverPinSaveConfig2())
                        return;
                    if (!WeightSaveConfig2(false))
                        return;

                    SaveConfigForm scForm = new SaveConfigForm(null, true);
                    scForm.ShowDialog();
                }
                WeightSetPageUnSelected();
                CloseTest();
                StopVolveTest();
                if (GlobalDataInterface.global_IsTestMode)
                    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_PROJ_CLOSED, null);

                //水平滚动条初始化 //Add by ChengSk - 20180408
                m_mainForm.splitContainer2_Panel1_HorizontalScroll_init(); 

                //出口列表操作
                m_mainForm.InitExitListBox(true);
                m_mainForm.SetAllExitListBox();

                //主界面菜单电机使能按钮
                m_mainForm.SetMenuMotorEnable();

                GlobalDataInterface.UpWeightInfoEvent -= new GlobalDataInterface.WeightInfoEventHandler(OnUpWeightInfo); //Add by ChengSk - 20180830
                GlobalDataInterface.UpImageDataEvent -= new GlobalDataInterface.ImageDataEventHandler(OnUpImageData);    //Add by ChengSk - 20180830
                GlobalDataInterface.UpAutoWhiteBalanceInfoEvent -= new GlobalDataInterface.AutoWhiteBalanceInfoEventHandler(OnUpAutoWhiteBalanceInfo); //Add by ChengSk - 20180830
                GlobalDataInterface.UpShutterAdjustInfoEvent -= new GlobalDataInterface.ShutterAdjustInfoEventHandler(OnShutterAdjustInfo);
                GlobalDataInterface.UpFruitGradeInfoEvent -= new GlobalDataInterface.FruitGradeInfoEventHandler(OnUpFruitGradeInfo);//Add by ChengSk - 20180830
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数ProjectSetForm_FormClosing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数ProjectSetForm_FormClosing出错" + ex);
#endif
            }
        }

        private void ProjectSetForm_SizeChanged(object sender, EventArgs e)
        {
            if (GlobalDataInterface.ProjectSetFormIsIntialed)
                asc.controlAutoSize(this);
        }

        private void ProjecttabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //if(!m_tabselectInvalid)
            //    e.Cancel = true;   
            if(!string.IsNullOrEmpty(e.TabPage.Tag.ToString())&&!Convert.ToBoolean(e.TabPage.Tag))
            {
                e.Cancel = true;
            }
        }

        #region 内部品质TCP/IP连接

        private void TcpClient_Connected(object sender, EventArgs e)
        {
            try
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    //连接成功之后可以做一些事情啦
                    GlobalDataInterface.CommportConnectFlag = true;
                    //MessageDataSend(ConstPreDefine.SBC_INFO_REQ); //请求光谱仪指令
                }));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数TcpClient_Connected出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数TcpClient_Connected出错" + ex);
#endif
            }
        }

        private void TcpClient_DataReceived(object sender, DataEventArgs e)
        {
            if (e.Length < 1)
            {
                return;
            }
            ComSocketRxCommon(e);
        }

        private void ComSocketRxCommon(DataEventArgs e)
        {
            try
            {
                byte[] buff = new byte[e.Length];
                Array.Copy(e.Data, buff, e.Length);
                bool SerialFirstFlag = true;
                int ReceiveBufferPoint = 0;
                int msgLen = 0;
                for (int i = 0; i < e.Length; i++)
                {
                    if (SerialFirstFlag)
                    {
                        if (buff[i] == ConstPreDefine.STX)
                        {
                            SerialFirstFlag = false;
                            ReceiveBufferPoint = 0;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    ReceiveBufferPoint++;
                    if (ReceiveBufferPoint < 4)
                    {
                        continue;
                    }
                    msgLen = (buff[2] & 0xff)
                            + ((buff[3] << 8) & 0xff00);
                    if (msgLen > 0)
                    {
                        if (ReceiveBufferPoint < (msgLen + 4))
                        {
                            continue;
                        }
                    }
                    byte[] ReceiveMessageData = new byte[ConstPreDefine.MAX_BFR_SIZE];
                    Array.Copy(buff, ReceiveMessageData, ReceiveBufferPoint);
                    SerialFirstFlag = true;
                    ReceiveBufferPoint = 0;
                    ReceivedMessageProcess(ReceiveMessageData); //收到数据，开始处理
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数ComSocketRxCommon出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数ComSocketRxCommon出错" + ex);
#endif
            }
        }

        private void ReceivedMessageProcess(byte[] receiveMessageData)
        {
            try
            {
                int msgHead = receiveMessageData[0];
                int msgType = receiveMessageData[1];
                if (msgHead != ConstPreDefine.STX)
                {
                    return;
                }
                IntPtr pData = IntPtr.Zero;
                int nLength = 0;
                switch (msgType)
                {
                    case ConstPreDefine.SBC_INFO_REP:
                        nLength = Marshal.SizeOf(typeof(TSYS_DEV_INFORMATION));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            int SelId = m_ChanelIDList[m_InnerQualitySelectIndex];
                            int m_InnerQualitySubsysindex = Commonfunction.GetSubsysIndex(SelId);  //子系统索引
                            int m_InnerQualityChannelIndex = Commonfunction.GetChannelIndex(SelId);//子系统通道
                            int dataIndex = m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex;
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            GlobalDataInterface.globalOut_SysDevInfoData[dataIndex] = (TSYS_DEV_INFORMATION)Marshal.PtrToStructure(pData, typeof(TSYS_DEV_INFORMATION));
                            Marshal.FreeHGlobal(pData);
                            temp_SysDevInfoData[dataIndex].ToCopy(GlobalDataInterface.globalOut_SysDevInfoData[dataIndex]);
                            if (m_TcpChannelIndex == 0)
                            {
                                if (this.InvokeRequired)
                                {
                                    this.BeginInvoke(new MethodInvoker(delegate
                                    {
                                        m_InnerQualitySelectIndex = 0;
                                        SetFormInfoDisplay();//刷新光谱仪的信息
                                    }));
                                }
                            }
                        }
                        if (m_TcpChannelIndex == 0)
                        {
                            MessageDataSend(ConstPreDefine.SBC_PARA_REQ);
                        } //仅获取通道1的基础信息
                        break;
                    case ConstPreDefine.SBC_PARA_REP:
                        nLength = Marshal.SizeOf(typeof(TSYS_DEV_PARAMETER));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            GlobalDataInterface.globalOut_SysDevParaData = (TSYS_DEV_PARAMETER)Marshal.PtrToStructure(pData, typeof(TSYS_DEV_PARAMETER));
                            Marshal.FreeHGlobal(pData);
                            temp_SysDevParaData.ToCopy(GlobalDataInterface.globalOut_SysDevParaData);
                            if(m_TcpChannelIndex == 0)
                            {
                                if (this.InvokeRequired)
                                {
                                    this.BeginInvoke(new MethodInvoker(delegate
                                    {
                                        SetFormParaDisplay();//刷新内部品质参数
                                    }));
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
                //stopwatch.Stop();
                //Common.FileHelper.logQueue.Enqueue(string.Format("ReceivedMessageProcess函数结束时间{0}，耗时:{1}", System.DateTime.Now, stopwatch.Elapsed.TotalMilliseconds));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数ReceivedMessageProcess出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数ReceivedMessageProcess出错" + ex);
#endif
            }
        }

        public void MessageDataSend(byte MsgType, byte[] MsgData = null)
        {
            try
            {
                if (!GlobalDataInterface.CommportConnectFlag)
                {
                    return;
                }
                int MsgLength = 0;
                if (MsgData != null)
                {
                    MsgLength = MsgData.Length;
                }
                byte[] buff = new byte[(4 + MsgLength)];
                buff[0] = ConstPreDefine.STX;
                buff[1] = MsgType;
                buff[2] = Convert.ToByte(MsgLength & 0xff);
                buff[3] = Convert.ToByte((MsgLength >> 8) & 0xff);
                if (MsgData != null && MsgLength > 0)
                {
                    Array.Copy(MsgData, 0, buff, 4, MsgLength);
                }
                GlobalDataInterface.SendMsgData.Enqueue(buff);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数MessageDataSend出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数MessageDataSend出错" + ex);
#endif
            }
        }

        private void SendDataThread()
        {
            try
            {
                while (true)
                {
                    if (!GlobalDataInterface.CommportConnectFlag)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    if (GlobalDataInterface.SendMsgData.Count == 0)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    byte[] bytes;
                    if (GlobalDataInterface.SendMsgData.TryDequeue(out bytes))
                    {
                        tcpClient.Send(bytes, 0, bytes.Length);
                    }
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数SendDataThread出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数SendDataThread出错" + ex);
#endif
            }
        }

        private void TcpClient_Error(object sender, ErrorEventArgs e)
        {
            try
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    //连接失败之后可以做一些事情啦
                    GlobalDataInterface.CommportConnectFlag = false;
                }));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数TcpClient_Error出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数TcpClient_Error出错" + ex);
#endif
            }
        }

        private void StartReceivedInnerQualityInfo(int channelNumber)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(OpenTcpThread), channelNumber);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数StartReceivedInnerQualityInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数StartReceivedInnerQualityInfo出错" + ex);
#endif
            }
        }

        private void OpenTcpThread(object arg)
        {
            try
            {
                int channelNumber = (int)arg;
                int index = 0;
                while(index < channelNumber)
                {
                    try
                    {
                        //首先关闭当前的连接
                        if (tcpClient.IsConnected)
                        {
                            tcpClient.Close();
                            GlobalDataInterface.CommportConnectFlag = false;
                        }
                        else{
                            GlobalDataInterface.CommportConnectFlag = false;
                        }

                        int SelId = m_ChanelIDList[index];
                        int m_InnerQualitySubsysindex = Commonfunction.GetSubsysIndex(SelId);  //子系统索引
                        int m_InnerQualityChannelIndex = Commonfunction.GetChannelIndex(SelId);//子系统通道
                        int dataIndex = m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex;
                        m_TcpChannelIndex = dataIndex;     //当前选择的TCP通道索引
                        string ip = ConstPreDefine.LC_IP_ADDR_TEMPLATE + (dataIndex + 101);

                        #region 网络Ping验证
                        Ping m_ping = new Ping();
                        PingReply pingReply = m_ping.Send(ip, 500);
                        if (pingReply.Status != IPStatus.Success)
                        {
                            index++;
                            continue;
                        } //网络不通
                        #endregion

                        tcpClient.Connect(new IPEndPoint(IPAddress.Parse(ip), ConstPreDefine.LC_PORT_NUM)); //建立index+1的通道连接
                        
                        Thread.Sleep(500);
                        if (GlobalDataInterface.CommportConnectFlag)
                        {
                            MessageDataSend(ConstPreDefine.SBC_INFO_REQ); //请求光谱仪指令
                        }
                        
                        Thread.Sleep(1000);
                        //最后关闭当前的连接（仅在有需要时才打开连接）
                        if (tcpClient.IsConnected)
                        {
                            tcpClient.Close();
                            GlobalDataInterface.CommportConnectFlag = false;
                        }
                        else{
                            GlobalDataInterface.CommportConnectFlag = false;
                        }
                    }
                    catch(Exception ex)
                    {
                        Trace.WriteLine("ProjectSetForm中函数OpenTcpThread的while循环出错，index=" + index.ToString() + ex);
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数OpenTcpThread的while循环出错，index=" + index.ToString() + ex);
#endif
                    }
                    index++;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数OpenTcpThread出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数OpenTcpThread出错" + ex);
#endif
            }
        }

        #endregion

        //private void SubSysInfolistViewEx_SubItemClicked(object sender, SubItemEventArgs e)
        //{

        //}

        //private void SubSysInfolistViewEx_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        //{

        //}

        private void CameraDelaylistViewEx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    m_CameraDelaylistViewExDownType = 1;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数CameraDelaylistViewEx_MouseDoubleClick出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数CameraDelaylistViewEx_MouseDoubleClick出错" + ex);
#endif
            }
        }

        private void CameraDelaylistViewEx_SubItemClicked(object sender, SubItemEventArgs e)
        {
            try
            {
                if (e.SubItem > 0 && m_CameraDelaylistViewExDownType == 1)
                {
                    this.CameraDelaylistViewEx.StartEditing(CameraDelayEditors[0], e.Item, e.SubItem);
                    m_CameraDelaylistViewExDownType = 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数CameraDelaylistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数CameraDelaylistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        private void CameraDelaylistViewEx_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        {
            try
            {
                ListViewEx.ListViewEx listviewex = (ListViewEx.ListViewEx)sender;
                switch (e.SubItem)
                {
                    case 1:
                        if(e.Item.Index == 0)
                        {
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1)] = int.Parse(e.DisplayText);
                        }
                        else{
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1) + 1] = int.Parse(e.DisplayText);
                        }
                        break;
                    case 2:
                        if (e.Item.Index == 0)
                        {
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1)] = int.Parse(e.DisplayText);
                        }
                        else{
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1) + 1] = int.Parse(e.DisplayText);
                        }
                        break;
                    case 3:
                        if (e.Item.Index == 0)
                        {
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1)] = int.Parse(e.DisplayText);
                        }
                        else{
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1) + 1] = int.Parse(e.DisplayText);
                        }
                        break;
                    case 4:
                        if (e.Item.Index == 0)
                        {
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1)] = int.Parse(e.DisplayText);
                        }
                        else{
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1) + 1] = int.Parse(e.DisplayText);
                        }
                        break;
                    case 5:
                        if (e.Item.Index == 0)
                        {
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1)] = int.Parse(e.DisplayText);
                        }
                        else{
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1) + 1] = int.Parse(e.DisplayText);
                        }
                        break;
                    case 6:
                        if (e.Item.Index == 0)
                        {
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1)] = int.Parse(e.DisplayText);
                        }
                        else{
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1) + 1] = int.Parse(e.DisplayText);
                        }
                        break;
                    case 7:
                        if (e.Item.Index == 0)
                        {
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1)] = int.Parse(e.DisplayText);
                        }
                        else{
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1) + 1] = int.Parse(e.DisplayText);
                        }
                        break;
                    case 8:
                        if (e.Item.Index == 0)
                        {
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1)] = int.Parse(e.DisplayText);
                        }
                        else{
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1) + 1] = int.Parse(e.DisplayText);
                        }
                        break;
                    case 9:
                        if (e.Item.Index == 0)
                        {
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1)] = int.Parse(e.DisplayText);
                        }
                        else{
                            tempSysConfig.nCameraDelay[2 * (e.SubItem - 1) + 1] = int.Parse(e.DisplayText);
                        }
                        break;
                    default:
                        //MessageBox.Show("行号: " + e.Item.Index + " 列号: " + e.SubItem + " 内容: " + e.DisplayText);
                        break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数ParaslistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数ParaslistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        private void OutRangeThresholdlabel_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_SAVE_PARAS, null);
        }

      

       
    }
}
