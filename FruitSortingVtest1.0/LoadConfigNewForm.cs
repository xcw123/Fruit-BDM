using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Interface;
using Common;
using System.Diagnostics;
using System.Threading;
using System.Resources;
using FruitSortingVtest1.DB;

namespace FruitSortingVtest1._0
{
    ///// <summary>
    ///// 工程设置界面下行配置参数
    ///// </summary>
    //[StructLayout(LayoutKind.Sequential)]
    //public struct stProjectSetting
    //{
    //    public stSysConfig globalOut_SysConfig;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM)]
    //    public stExitInfo[] globalOut_ExitInfo;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM)]
    //    public stWeightBaseInfo[] globalOut_WeightBaseInfo;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM)]
    //    public stParas[] globalOut_Paras;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
    //    public stGlobalExitInfo[] globalOut_GlobalExitInfo;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
    //    public stGlobalWeightBaseInfo[] globalOut_GlobalWeightBaseInfo;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM)]
    //    public stSpotDetectThresh[] globalOut_SpotDetectThresh;
    //    public bool CIRAvailable;//视觉系统
    //    public bool WeightAvailable;//重量系统
    //    public bool InternalAvailable;//内部品质
    //    public bool NetStateSum;//全局网络状态
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
    //    public int[] NetState;//网络状态 
    //    public bool CupStateSum;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
    //    public int[] CupState;//果杯状态   

    //    public stProjectSetting(bool IsOK)
    //    {
    //        globalOut_SysConfig = new stSysConfig(true);
    //        globalOut_ExitInfo = new stExitInfo[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM];
    //        globalOut_WeightBaseInfo = new stWeightBaseInfo[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM];
    //        globalOut_Paras = new stParas[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM];
    //        globalOut_GlobalExitInfo = new stGlobalExitInfo[ConstPreDefine.MAX_SUBSYS_NUM];
    //        globalOut_GlobalWeightBaseInfo = new stGlobalWeightBaseInfo[ConstPreDefine.MAX_SUBSYS_NUM];
    //        globalOut_SpotDetectThresh = new stSpotDetectThresh[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM];
    //        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
    //        {
    //            globalOut_GlobalExitInfo[i] = new stGlobalExitInfo(true);
    //            globalOut_GlobalWeightBaseInfo[i] = new stGlobalWeightBaseInfo(true);
    //        }
    //        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
    //        {
    //            globalOut_ExitInfo[i] = new stExitInfo(true);
    //            globalOut_WeightBaseInfo[i] = new stWeightBaseInfo(true);
    //        }
    //        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM; i++)
    //        {
    //            globalOut_Paras[i] = new stParas(true);
    //            globalOut_SpotDetectThresh[i] = new stSpotDetectThresh(true);
    //        }
    //        NetStateSum = false;//全局网络状态
    //        NetState = new int[ConstPreDefine.MAX_SUBSYS_NUM];//网络状态 
    //        CupStateSum = false;
    //        CupState = new int[ConstPreDefine.MAX_SUBSYS_NUM];//果杯状态      
    //        CIRAvailable = false;//视觉系统
    //        WeightAvailable = false;//重量系统
    //        InternalAvailable = false;//内部品质
    //    }
    //}

    public partial class LoadConfigNewForm : Form
    {
        private bool m_IsProjectConfig = false;
        MainForm m_mainForm;
        ProjectSetForm m_projectSetForm;
        private ResourceManager m_resourceManager = new ResourceManager(typeof(LoadConfigNewForm));//创建LoadConfigNewForm资源管理
        public LoadConfigNewForm(MainForm mainForm, ProjectSetForm projectSetForm,bool IsProjectConfig)
        {
            m_IsProjectConfig = IsProjectConfig;
            m_mainForm = mainForm;
            m_projectSetForm = projectSetForm;
            InitializeComponent();
        }

        /// <summary>
        /// 窗口加载初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadConfigNewForm_Load(object sender, EventArgs e)
        {
            try
            {
                //this.Progresslabel.Text = "";
                //this.Progresslabel.Visible = false;
                this.LoadConfigprogressBar.Visible = false;
                List<string> configFileNameList = new List<string>();
                if (m_IsProjectConfig)
                {
                    this.Text = m_resourceManager.GetString("LoadProjectConfiglabel.Text");
                    Commonfunction.GetAllProjectSettingFileName(ref configFileNameList);

                    
                }
                else
                {
                    this.Text = m_resourceManager.GetString("LoadUserConfiglabel.Text");
                    Commonfunction.GetAllCommonSettingFileName(ref configFileNameList);
                    
                }
                for (int i = 0; i < configFileNameList.Count; i++)
                {
                    this.ConfiglistBox.Items.Add(configFileNameList[i]);
                }

               
                if (this.ConfiglistBox.Items.Count > 0)
                {
                    this.ConfiglistBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("LoadConfigNewForm中函数LoadConfigNewForm_Load出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("LoadConfigNewForm中函数LoadConfigNewForm_Load出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 加载配置控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadConfigbutton_Click(object sender, EventArgs e)
        {            
           // this.Progresslabel.Text = "正在加载配置...";

            int SelIndex = this.ConfiglistBox.SelectedIndex;
            if (SelIndex < 0)
            {
                //MessageBox.Show("请选择配置文件！");
                //MessageBox.Show("0x3000101D Please select the configuration file!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("0x3000101D " + LanguageContainer.LoadConfigNewFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.LoadConfigNewFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.ConfiglistBox.Focus();
                return;
            }
            if (!Directory.Exists(System.Environment.CurrentDirectory + "\\config\\" + GlobalDataInterface.VERSION_SHOW + "\\"))
                Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\config\\"+ GlobalDataInterface.VERSION_SHOW+ "\\");
            string FileName = System.Environment.CurrentDirectory; 
            
            FileName += "\\config\\" + GlobalDataInterface.VERSION_SHOW + "\\";
            FileName += (string)this.ConfiglistBox.Items[this.ConfiglistBox.SelectedIndex];
            if (m_IsProjectConfig)
            {
                FileName += ".exp";
            }
            else
            {
                FileName += ".cmc";
            }
            FileInfo configFile = new FileInfo(FileName);
            byte[] FileData;
            FileStream configStream;
            try
            {

                if (configFile.Exists)
                {
                    this.LoadConfigprogressBar.Visible = true;
                    configStream = File.OpenRead(FileName);

                    if (m_IsProjectConfig)//工程配置
                    {
                        //寻找最长结构体长度
                        int MaxLenth = Marshal.SizeOf(typeof(stSysConfig));
                        if (MaxLenth < Marshal.SizeOf(typeof(stExitInfo)))
                            MaxLenth = Marshal.SizeOf(typeof(stExitInfo));
                        if (MaxLenth < Marshal.SizeOf(typeof(stWeightBaseInfo)))
                            MaxLenth = Marshal.SizeOf(typeof(stWeightBaseInfo));
                        if (MaxLenth < Marshal.SizeOf(typeof(stParas)))
                            MaxLenth = Marshal.SizeOf(typeof(stParas));
                        if (MaxLenth < Marshal.SizeOf(typeof(stGlobalExitInfo)))
                            MaxLenth = Marshal.SizeOf(typeof(stGlobalExitInfo));
                        if (MaxLenth < Marshal.SizeOf(typeof(stGlobalWeightBaseInfo)))
                            MaxLenth = Marshal.SizeOf(typeof(stGlobalWeightBaseInfo));
                        //if (MaxLenth < Marshal.SizeOf(typeof(stSpotDetectThresh)))
                        //    MaxLenth = Marshal.SizeOf(typeof(stSpotDetectThresh));

                        FileData = new byte[MaxLenth];
                        configStream.Seek(0, SeekOrigin.Begin);

                        configStream.Read(FileData, 0, sizeof(Int32));//2016-12-6加版本号校验
                        int version = BitConverter.ToInt32(FileData, 0);

                        string strFSMVersion = version.ToString();
                        string strSubFSMVersion = strFSMVersion.Substring(0, strFSMVersion.Length - 2); //获取主版本号+副版本号
                        string strHCVersion = GlobalDataInterface.Version.ToString();
                        string strSubHCVersion = strHCVersion.Substring(0, strHCVersion.Length - 2); //获取主版本号+副版本号
                        //if (ConstPreDefine.VERSION != version)
                        if (strSubFSMVersion != strSubHCVersion) //Modify by ChengSk - 20180403
                        {
                            int nMainVersionNo = version / 10000;
                            int nViceVersionNo = (version - nMainVersionNo * 10000) / 100;
                            int nAmendVersionNo = version - nMainVersionNo * 10000 - nViceVersionNo * 100;
                            string fsmVersion = string.Format("{0}.{1}.{2}", nMainVersionNo, nViceVersionNo, nAmendVersionNo);
                            //MessageBox.Show(string.Format("0x10002001 Version mismatch! HC is V{0},File is V{1}",ConstPreDefine.VERSION_SHOW, version), "Error", MessageBoxButtons.OK);
                            MessageBox.Show(string.Format("0x10002001 " + LanguageContainer.LoadConfigNewFormMessagebox6Sub1Text[GlobalDataInterface.selectLanguageIndex] + "{0}" +
                                LanguageContainer.LoadConfigNewFormMessagebox6Sub2Text[GlobalDataInterface.selectLanguageIndex] + "{1}" +
                                LanguageContainer.LoadConfigNewFormMessagebox6Sub3Text[GlobalDataInterface.selectLanguageIndex],
                                GlobalDataInterface.VERSION_SHOW, fsmVersion),
                                LanguageContainer.LoadConfigNewFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK); //Modify by ChengSk - 20180611
                            configStream.Close();
                            return;
                        }
                        configStream.Read(FileData, 0, Marshal.SizeOf(typeof(stSysConfig)));
                        GlobalDataInterface.globalOut_SysConfig = (stSysConfig)Commonfunction.BytesToStruct(FileData, typeof(stSysConfig));
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                        {
                            configStream.Read(FileData, 0, Marshal.SizeOf(typeof(stExitInfo)));
                            GlobalDataInterface.globalOut_ExitInfo[i] = (stExitInfo)Commonfunction.BytesToStruct(FileData, typeof(stExitInfo));
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                        {
                            configStream.Read(FileData, 0, Marshal.SizeOf(typeof(stWeightBaseInfo)));
                            GlobalDataInterface.globalOut_WeightBaseInfo[i] = (stWeightBaseInfo)Commonfunction.BytesToStruct(FileData, typeof(stWeightBaseInfo));
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM; i++)
                        {
                            configStream.Read(FileData, 0, Marshal.SizeOf(typeof(stParas)));
                            GlobalDataInterface.globalOut_Paras[i] = (stParas)Commonfunction.BytesToStruct(FileData, typeof(stParas));
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                        {
                            configStream.Read(FileData, 0, Marshal.SizeOf(typeof(stGlobalExitInfo)));
                            GlobalDataInterface.globalOut_GlobalExitInfo[i] = (stGlobalExitInfo)Commonfunction.BytesToStruct(FileData, typeof(stGlobalExitInfo));
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                        {
                            configStream.Read(FileData, 0, Marshal.SizeOf(typeof(stGlobalWeightBaseInfo)));
                            GlobalDataInterface.globalOut_GlobalWeightBaseInfo[i] = (stGlobalWeightBaseInfo)Commonfunction.BytesToStruct(FileData, typeof(stGlobalWeightBaseInfo));
                        }
                        //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM; i++)
                        //{
                        //    configStream.Read(FileData, 0, Marshal.SizeOf(typeof(stSpotDetectThresh)));
                        //    GlobalDataInterface.globalOut_SpotDetectThresh[i] = (stSpotDetectThresh)Commonfunction.BytesToStruct(FileData, typeof(stSpotDetectThresh));
                        //}

                        configStream.Read(FileData, 0, sizeof(bool));
                        GlobalDataInterface.CIRAvailable = BitConverter.ToBoolean(FileData, 0);
                        configStream.Read(FileData, 0, sizeof(bool));
                        GlobalDataInterface.WeightAvailable = BitConverter.ToBoolean(FileData, 0);
                        configStream.Read(FileData, 0, sizeof(bool));
                        GlobalDataInterface.NetStateSum = BitConverter.ToBoolean(FileData, 0);
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                        {
                            configStream.Read(FileData, 0, sizeof(int));
                            GlobalDataInterface.NetState[i] = BitConverter.ToInt32(FileData, 0);
                        }
                        configStream.Read(FileData, 0, sizeof(bool));
                        GlobalDataInterface.CupStateSum = BitConverter.ToBoolean(FileData, 0);
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                        {
                            configStream.Read(FileData, 0, sizeof(int));
                            GlobalDataInterface.CupState[i] = BitConverter.ToInt32(FileData, 0);
                        }
                        configStream.Close();
                        GlobalDataInterface.ProjectSetFormIsIntialed = false;
                        GlobalDataInterface.ExitList.Clear();
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < ConstPreDefine.MAX_EXIT_NUM * 2; j++)
                            {
                                if (GlobalDataInterface.globalOut_SysConfig.exitstate[i * ConstPreDefine.MAX_EXIT_NUM * 2 + j] > 0)
                                {
                                    ExitState exitState;
                                    exitState.Index = GlobalDataInterface.globalOut_SysConfig.exitstate[i * ConstPreDefine.MAX_EXIT_NUM * 2 + j];
                                    exitState.ItemIndex = i;
                                    exitState.ColumnIndex = j;
                                    GlobalDataInterface.ExitList.Add(exitState);
                                }
                            }
                        }
                        //if (GlobalDataInterface.ExitList.Count > 0)
                        //{
                           
                        //    m_mainForm.InitExitListBox(true); //初始化出口
                        //    m_mainForm.SetAllExitListBox(); //初始化出口中等级
                        //}
                        //传感系统类型

                        ////视觉
                        //if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 1) > 0)
                        //{
                        //    GlobalDataInterface.CIRAvailable = true;
                        //}
                        //else
                        //{
                        //    GlobalDataInterface.CIRAvailable = false;
                        //}
                        ////重量
                        //if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 2) > 0)
                        //{
                        //    GlobalDataInterface.WeightAvailable = true;
                        //}
                        //else
                        //{
                        //    GlobalDataInterface.WeightAvailable = false;
                        //}
                        ////内部品质
                        //if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 4) > 0)
                        //{
                        //    GlobalDataInterface.InternalAvailable = true;
                        //}
                        //else
                        //{
                        //    GlobalDataInterface.InternalAvailable = false;
                        //}

                        #region 硬件设置加载 Modify by ChengSk - 20191018
                        //视觉
                        if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 1) > 0)
                        {
                            GlobalDataInterface.CIRAvailable = true;

                            #region 颜色、形状、瑕疵、体积、投影面积
                            if ((GlobalDataInterface.globalOut_SysConfig.CIRClassifyType & 0x01) == 1) //颜色
                                GlobalDataInterface.SystemStructColor = true;
                            else
                                GlobalDataInterface.SystemStructColor = false;
                            if ((GlobalDataInterface.globalOut_SysConfig.CIRClassifyType & 0x02) == 2) //形状
                                GlobalDataInterface.SystemStructShape = true;
                            else
                                GlobalDataInterface.SystemStructShape = false;
                            if ((GlobalDataInterface.globalOut_SysConfig.CIRClassifyType & 0x04) == 4) //瑕疵
                                GlobalDataInterface.SystemStructFlaw = true;
                            else
                                GlobalDataInterface.SystemStructFlaw = false;
                            if ((GlobalDataInterface.globalOut_SysConfig.CIRClassifyType & 0x08) == 8) //体积
                                GlobalDataInterface.SystemStructVolume = true;
                            else
                                GlobalDataInterface.SystemStructVolume = false;
                            if ((GlobalDataInterface.globalOut_SysConfig.CIRClassifyType & 0x10) == 16)//投影面积
                                GlobalDataInterface.SystemStructProjectedArea = true;
                            else
                                GlobalDataInterface.SystemStructProjectedArea = false;
                            #endregion
                        }
                        else
                        {
                            GlobalDataInterface.CIRAvailable = false;

                            #region 颜色、形状、瑕疵、体积、投影面积
                            GlobalDataInterface.SystemStructColor = false; //颜色
                            GlobalDataInterface.SystemStructShape = false; //形状
                            GlobalDataInterface.SystemStructFlaw = false;  //瑕疵
                            GlobalDataInterface.SystemStructVolume = false; //体积
                            GlobalDataInterface.SystemStructProjectedArea = false;//投影面积
                            #endregion
                        }
                        //UV视觉
                        if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 2) > 0)
                        {
                            GlobalDataInterface.UVAvailable = true;

                            #region 擦伤、腐烂
                            if ((GlobalDataInterface.globalOut_SysConfig.UVClassifyType & 0x01) == 1)  //擦伤
                                GlobalDataInterface.SystemStructBruise = true;
                            else
                                GlobalDataInterface.SystemStructBruise = false;
                            if ((GlobalDataInterface.globalOut_SysConfig.UVClassifyType & 0x02) == 2)  //腐烂
                                GlobalDataInterface.SystemStructRot = true;
                            else
                                GlobalDataInterface.SystemStructRot = false;
                            #endregion
                        }
                        else
                        {
                            GlobalDataInterface.UVAvailable = false;

                            #region 擦伤、腐烂
                            GlobalDataInterface.SystemStructBruise = false;  //擦伤
                            GlobalDataInterface.SystemStructRot = false;     //腐烂
                            #endregion
                        }
                        //重量
                        if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 4) > 0)
                        {
                            GlobalDataInterface.WeightAvailable = true;

                            #region 密度
                            if ((GlobalDataInterface.globalOut_SysConfig.WeightClassifyTpye & 0x01) == 1)  //密度
                                GlobalDataInterface.SystemStructDensity = true;
                            else
                                GlobalDataInterface.SystemStructDensity = false;
                            #endregion
                        }
                        else
                        {
                            GlobalDataInterface.WeightAvailable = false;

                            #region 密度
                            GlobalDataInterface.SystemStructDensity = false;  //密度
                            #endregion
                        }
                        //内部品质
                        if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 8) > 0)
                        {
                            GlobalDataInterface.InternalAvailable = true;

                            #region 糖度、酸度、空心、浮皮、褐变、糖心
                            if ((GlobalDataInterface.globalOut_SysConfig.InternalClassifyType & 0x01) == 1)  //糖度
                                GlobalDataInterface.SystemStructSugar = true;
                            else
                                GlobalDataInterface.SystemStructSugar = false;
                            if ((GlobalDataInterface.globalOut_SysConfig.InternalClassifyType & 0x02) == 2)  //酸度
                                GlobalDataInterface.SystemStructAcidity = true;
                            else
                                GlobalDataInterface.SystemStructAcidity = false;
                            if ((GlobalDataInterface.globalOut_SysConfig.InternalClassifyType & 0x04) == 4)  //空心
                                GlobalDataInterface.SystemStructHollow = true;
                            else
                                GlobalDataInterface.SystemStructHollow = false;
                            if ((GlobalDataInterface.globalOut_SysConfig.InternalClassifyType & 0x08) == 8)  //浮皮
                                GlobalDataInterface.SystemStructSkin = true;
                            else
                                GlobalDataInterface.SystemStructSkin = false;
                            if ((GlobalDataInterface.globalOut_SysConfig.InternalClassifyType & 0x10) == 16) //褐变
                                GlobalDataInterface.SystemStructBrown = true;
                            else
                                GlobalDataInterface.SystemStructBrown = false;
                            if ((GlobalDataInterface.globalOut_SysConfig.InternalClassifyType & 0x20) == 32) //糖心
                                GlobalDataInterface.SystemStructTangxin = true;
                            else
                                GlobalDataInterface.SystemStructTangxin = false;
                            #endregion
                        }
                        else
                        {
                            GlobalDataInterface.InternalAvailable = false;

                            #region 糖度、酸度、空心、浮皮、褐变、糖心
                            GlobalDataInterface.SystemStructSugar = false;   //糖度
                            GlobalDataInterface.SystemStructAcidity = false; //酸度
                            GlobalDataInterface.SystemStructHollow = false;  //空心
                            GlobalDataInterface.SystemStructSkin = false;    //浮皮
                            GlobalDataInterface.SystemStructBrown = false;   //褐变
                            GlobalDataInterface.SystemStructTangxin = false; //糖心
                            #endregion
                        }
                        //超声波
                        if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 16) > 0)
                        {
                            GlobalDataInterface.UltrasonicAvailable = true;

                            #region 硬度、含水率
                            if ((GlobalDataInterface.globalOut_SysConfig.UltrasonicClassifyType & 0x01) == 1)//硬度
                                GlobalDataInterface.SystemStructRigidity = true;
                            else
                                GlobalDataInterface.SystemStructRigidity = false;
                            if ((GlobalDataInterface.globalOut_SysConfig.UltrasonicClassifyType & 0x02) == 2)//含水率
                                GlobalDataInterface.SystemStructWater = true;
                            else
                                GlobalDataInterface.SystemStructWater = false;
                            #endregion
                        }
                        else
                        {
                            GlobalDataInterface.UltrasonicAvailable = false;

                            #region 硬度、含水率
                            GlobalDataInterface.SystemStructRigidity = false;  //硬度
                            GlobalDataInterface.SystemStructWater = false;     //含水率
                            #endregion
                        }
                        //WIFI功能
                        if ((GlobalDataInterface.globalOut_SysConfig.IfWIFIEnable & 1) > 0)
                        {
                            GlobalDataInterface.sendBroadcastPackage = true;
                        }
                        else
                        {
                            GlobalDataInterface.sendBroadcastPackage = false;
                        }
                        //倍频功能
                        if ((GlobalDataInterface.globalOut_SysConfig.multiFreq & 1) > 0)
                        {
                            GlobalDataInterface.MultiFreqAvailable = true;
                        }
                        else
                        {
                            GlobalDataInterface.MultiFreqAvailable = false;
                        }
                        #endregion

                        // GlobalDataInterface.ProjectSetFormIsIntialed = false;
                        this.LoadConfigprogressBar.Value = 10;

                        //m_projectSetForm.Reload(0); //0->1 Modify by ChengSk - 20180919  //1->0 Modify by ChengSk - 20190827
                        m_projectSetForm.Reload2(1);  //Modify by ChengSk - 20190827

                        this.LoadConfigprogressBar.Value += 10;
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_SYS_CONFIG, null);

                            if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadSysConfigUpdate++;//系统配置信息更改，平板需更新
                            Thread.Sleep(800);
                            this.LoadConfigprogressBar.Value += 10;
                            //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null); //Note by ChengSk - 20190116
                            //if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                            Thread.Sleep(800);
                            this.LoadConfigprogressBar.Value += 10;
                            for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                            {
                                for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                                {
                                    //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                                    if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                                    {
                                        int ChannelID = Commonfunction.EncodeChannel(i, j, j);
                                        if (GlobalDataInterface.nVer == 0) //Modify by xcw - 20200619
                                        {
                                            ChannelID = Commonfunction.EncodeChannel(i, j, j);
                                        }
                                        else if (GlobalDataInterface.nVer == 1)
                                        {
                                            ChannelID = Commonfunction.EncodeChannel(i, j / 2, j % 2);
                                        }
                                        GlobalDataInterface.TransmitParam(ChannelID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null);
                                        Thread.Sleep(800);  //modify by xcw 20200903
                                        GlobalDataInterface.TransmitParam(ChannelID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHT_INFO, null);
                                        Thread.Sleep(800);
                                        if (GlobalDataInterface.nVer == 0) //Modify by xcw - 20201230
                                        {
                                            GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(i, j), (int)HC_FSM_COMMAND_TYPE.HC_CMD_PARAS_INFO, null);
                                            Thread.Sleep(800);
                                        }
                                        else if (GlobalDataInterface.nVer == 1)
                                        {
                                            if (j % 2 == 0)
                                            {
                                                GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(i, j / 2), (int)HC_FSM_COMMAND_TYPE.HC_CMD_PARAS_INFO, null);
                                                Thread.Sleep(800);
                                            }
                                        }
                                        //if (j % 2 == 0)
                                        //{
                                        //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(i, j / 2), (int)HC_FSM_COMMAND_TYPE.HC_CMD_PARAS_INFO, null);
                                        //    Thread.Sleep(800);
                                        //    //GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(i, j / 2), (int)HC_FSM_COMMAND_TYPE.HC_CMD_FlAWAREA_INFO, GlobalDataInterface.globalOut_SpotDetectThresh[i * ConstPreDefine.MAX_IPM_NUM + j / 2]);
                                        //    //Thread.Sleep(800);
                                        //}
                                    }
                                }
                                this.LoadConfigprogressBar.Value += 20;
                                GlobalDataInterface.TransmitParam(Commonfunction.EncodeSubsys(i), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_EXIT_INFO, null);
                                Thread.Sleep(800);
                                this.LoadConfigprogressBar.Value += 10;
                                GlobalDataInterface.TransmitParam(Commonfunction.EncodeSubsys(i), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_WEIGHT_INFO, null);
                                Thread.Sleep(800);
                                this.LoadConfigprogressBar.Value += 10;
                            }
                        }

                        m_mainForm.SetMainstatusStrip();
                        m_mainForm.SetQaulitytoolStripButtonEnabled();
                        //this.Progresslabel.Text = "完成加载";
                       // GlobalDataInterface.ProjectSetFormIsIntialed = true;
                        //if (GlobalDataInterface.global_IsTestMode)
                        //{
                        //    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++)
                        //    {
                        //        stGlobal global = new stGlobal(true);
                        //        global.sys.ToCopy(GlobalDataInterface.globalOut_SysConfig);
                        //        global.grade.ToCopy(GlobalDataInterface.globalOut_GradeInfo);
                        //        global.gexit.ToCopy(GlobalDataInterface.globalOut_GlobalExitInfo[i]);
                        //        global.gweight.ToCopy(GlobalDataInterface.globalOut_GlobalWeightBaseInfo[i]);
                        //        for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                        //        {
                        //            if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                        //            {
                        //                global.exit[j].ToCopy(GlobalDataInterface.globalOut_ExitInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j]);
                        //                global.weights[j].ToCopy(GlobalDataInterface.globalOut_WeightBaseInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j]);
                        //                if (j % 2 == 0)
                        //                {
                        //                    global.paras[j / 2].ToCopy(GlobalDataInterface.globalOut_Paras[i * ConstPreDefine.MAX_IPM_NUM + j / 2]);
                        //                    global.spotdetectthresh[j / 2].ToCopy(GlobalDataInterface.globalOut_SpotDetectThresh[i * ConstPreDefine.MAX_IPM_NUM + j / 2]);
                        //                }
                        //            }
                        //        }
                        //        global.nSubsysId = Commonfunction.EncodeSubsys(i);
                        //        GlobalDataInterface.TransmitParam(global.nSubsysId, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_INFO, global);
                        //    }
                        //}
                    }
                    else//用户配置
                    {
                        FileData = new byte[Marshal.SizeOf(typeof(stGradeInfo))];
                        configStream.Seek(0, SeekOrigin.Begin);

                        configStream.Read(FileData, 0, sizeof(Int32));//2016-12-6加版本号校验
                        int version = BitConverter.ToInt32(FileData, 0);

                        string strFSMVersion = version.ToString();
                        string strSubFSMVersion = strFSMVersion.Substring(0, strFSMVersion.Length - 2); //获取主版本号+副版本号
                        string strHCVersion = GlobalDataInterface.Version.ToString();
                        string strSubHCVersion = strHCVersion.Substring(0, strHCVersion.Length - 2); //获取主版本号+副版本号
                        //if (ConstPreDefine.VERSION != version)
                        if (strSubFSMVersion != strSubHCVersion) //Modify by ChengSk - 20180403
                        {
                            int nMainVersionNo = version / 10000;
                            int nViceVersionNo = (version - nMainVersionNo * 10000) / 100;
                            int nAmendVersionNo = version - nMainVersionNo * 10000 - nViceVersionNo * 100;
                            string fsmVersion = string.Format("{0}.{1}.{2}", nMainVersionNo, nViceVersionNo, nAmendVersionNo);
                            //MessageBox.Show(string.Format("0x10002001 Version mismatch! HC is V{0},File is V{1}", ConstPreDefine.VERSION_SHOW, version), "Error", MessageBoxButtons.OK);
                            MessageBox.Show(string.Format("0x10002001 " + LanguageContainer.LoadConfigNewFormMessagebox6Sub4Text[GlobalDataInterface.selectLanguageIndex] + "{0}" +
                                LanguageContainer.LoadConfigNewFormMessagebox6Sub5Text[GlobalDataInterface.selectLanguageIndex] + "{1}" +
                                LanguageContainer.LoadConfigNewFormMessagebox6Sub6Text[GlobalDataInterface.selectLanguageIndex],
                                GlobalDataInterface.VERSION_SHOW, fsmVersion),
                                LanguageContainer.LoadConfigNewFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK); //Modify by ChengSk - 20180611
                            configStream.Close();
                            return;
                        }

                        configStream.Read(FileData, 0, Marshal.SizeOf(typeof(stGradeInfo)));
                        GlobalDataInterface.globalOut_GradeInfo = (stGradeInfo)Commonfunction.BytesToStruct(FileData, typeof(stGradeInfo));
                        /*用户配置增加颜色界面-》颜色列表背景颜色保存*/
                        string color;
                        stColorList colorlist = new stColorList(true);
                        configStream.Read(FileData, 0, Marshal.SizeOf(typeof(stColorList)));
                        colorlist = (stColorList)Commonfunction.BytesToStruct(FileData, typeof(stColorList));
                        color = Encoding.Default.GetString(colorlist.color1).TrimEnd('\0');
                        Commonfunction.SetAppSetting("颜色参数-颜色1", color);
                        color = Encoding.Default.GetString(colorlist.color2).TrimEnd('\0');
                        Commonfunction.SetAppSetting("颜色参数-颜色2", color);
                        color = Encoding.Default.GetString(colorlist.color3).TrimEnd('\0');
                        Commonfunction.SetAppSetting("颜色参数-颜色3", color);

                        //Add by ChengSk - 20190929
                        string clientinfo;
                        stClientInfo ClientInfo = new stClientInfo(true);
                        configStream.Read(FileData, 0, Marshal.SizeOf(typeof(stClientInfo)));
                        ClientInfo = (stClientInfo)Commonfunction.BytesToStruct(FileData, typeof(stClientInfo));
                        clientinfo = Encoding.Default.GetString(ClientInfo.customerName).TrimEnd('\0');
                        GlobalDataInterface.dataInterface.CustomerName = clientinfo;   //客户名称
                        clientinfo = Encoding.Default.GetString(ClientInfo.farmName).TrimEnd('\0');
                        GlobalDataInterface.dataInterface.FarmName = clientinfo;       //农场名称
                        clientinfo = Encoding.Default.GetString(ClientInfo.fruitName).TrimEnd('\0');
                        GlobalDataInterface.dataInterface.FruitName = clientinfo;      //水果名称
                        m_mainForm.clientInfoContent = GlobalDataInterface.dataInterface.CustomerName + "，" +
                            GlobalDataInterface.dataInterface.FarmName + "，" + GlobalDataInterface.dataInterface.FruitName;
                        FileOperate.EditFile(1, m_mainForm.clientInfoContent, m_mainForm.clientInfoFileName);//修改文件

                        configStream.Close();
                        int size = 1;
                        if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                            size = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                        GlobalDataInterface.Quality_GradeInfo.GradeCnt = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                        for (int i = 0; i < GlobalDataInterface.Quality_GradeInfo.GradeCnt; i++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, GlobalDataInterface.Quality_GradeInfo.Item[i].GradeName, 0, GlobalDataInterface.Quality_GradeInfo.Item[i].GradeName.Length);
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbShapeGrade = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbShapeSize;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].ColorGrade = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nColorGrade;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbDensity = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbDensity;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbFlaw = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbFlawArea;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbRigidity = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbRigidity;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbSugar = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbSugar;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbAcidity = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbAcidity;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbBrown = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbBrown;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbBruise = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbBruise;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbHollow = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbHollow;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbRot = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbRot;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbSkin = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbSkin;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbTangxin = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbTangxin;
                                GlobalDataInterface.Quality_GradeInfo.Item[i].sbWater = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbWater;
                            }
                        }

                        //int cnt = 0;  //Note by ChengSk - 20181128
                        //for (int i = 0; i < ConstPreDefine.MAX_COLOR_GRADE_NUM; i++)
                        //{
                        //    if ((GlobalDataInterface.globalOut_GradeInfo.ColorType & 0x08) > 0)//百分比
                        //    {
                        //        if (GlobalDataInterface.globalOut_GradeInfo.percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM].nMax == 0x7f)
                        //            break;
                        //        cnt++;
                        //    }
                        //    else
                        //    {
                        //        cnt = 3;
                        //    }
                        //}
                        //GlobalDataInterface.ColorGradeNum = cnt;

                        if ((GlobalDataInterface.globalOut_GradeInfo.ColorType & 0x08) > 0)//百分比
                        {
                            for (int i = 0; i < ConstPreDefine.MAX_COLOR_GRADE_NUM; i++)
                            {
                                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strColorGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                string colorName = Encoding.Default.GetString(temp).TrimEnd('\0');
                                if (colorName == "")
                                {
                                    GlobalDataInterface.ColorGradeNum = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            GlobalDataInterface.ColorGradeNum = 3;
                        } //Modify by ChengSk - 20181228 加载时颜色数量不正确

                        //形状等级数量
                        for (int i = 0; i < ConstPreDefine.MAX_SHAPE_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.fShapeFactor[i] == 0.000000)
                            {
                                GlobalDataInterface.ShapeGradeNum = i + 1;
                                break;
                            }
                        }

                        for (int i = 0; i < ConstPreDefine.MAX_DENSITY_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.fDensityFactor[i] == 0.000000)
                            {
                                GlobalDataInterface.DensityGradeNum = i + 1;
                                break;
                            }
                        }

                        for (int i = 0; i < ConstPreDefine.MAX_RIGIDITY_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.fRigidityFactor[i] == 0.000000)
                            {
                                GlobalDataInterface.RigidityGradeNum = i + 1;
                                break;
                            }
                        }

                        for (int i = 0; i < ConstPreDefine.MAX_SUGAR_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.fSugarFactor[i] == 0.000000)
                            {
                                GlobalDataInterface.SugarGradeNum = i + 1;
                                break;
                            }
                        }

                        for (int i = 0; i < ConstPreDefine.MAX_FlAWAREA_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.unFlawAreaFactor[i * 2] == 0 && GlobalDataInterface.globalOut_GradeInfo.unFlawAreaFactor[i * 2 + 1] == 0)
                            {
                                GlobalDataInterface.FlawGradeNum = i + 1;
                                break;
                            }
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_ACIDITY_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.fAcidityFactor[i] == 0.000000)
                            {
                                GlobalDataInterface.AcidityGradeNum = i + 1;
                                break;
                            }
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_BROWN_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.fBrownFactor[i] == 0.000000)
                            {
                                GlobalDataInterface.BrownGradeNum = i + 1;
                                break;
                            }
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_BRUISE_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.unBruiseFactor[i] == 0)
                            {
                                GlobalDataInterface.BruiseGradeNum = i + 1;
                                break;
                            }
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_HOLLOW_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.fHollowFactor[i] == 0.000000)
                            {
                                GlobalDataInterface.HollowGradeNum = i + 1;
                                break;
                            }
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_ROT_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.unRotFactor[i] == 0)
                            {
                                GlobalDataInterface.RotGradeNum = i + 1;
                                break;
                            }
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_SKIN_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.fSkinFactor[i] == 0.000000)
                            {
                                GlobalDataInterface.SkinGradeNum = i + 1;
                                break;
                            }
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_TANGXIN_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.fTangxinFactor[i] == 0.000000)
                            {
                                GlobalDataInterface.TangxinGradeNum = i + 1;
                                break;
                            }
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_WATER_GRADE_NUM; i++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.fWaterFactor[i] == 0.000000)
                            {
                                GlobalDataInterface.WaterGradeNum = i + 1;
                                break;
                            }
                        }
                       
                        m_mainForm.SetGradedataGridViewInfo();
                        m_mainForm.SetGradeSizelistViewEx();
                        m_mainForm.SetQaulitytoolStripButtonEnabled();
                        m_mainForm.InitExitListBox(true); //初始化出口
                        m_mainForm.SetAllExitListBox(); //add by xcw 20201215
                        m_mainForm.SetSeparationProgrameChangelabel(true, (string)this.ConfiglistBox.Items[this.ConfiglistBox.SelectedIndex]);
                        m_mainForm.SetMainstatusStrip();
                        m_mainForm.UpdateClientInfoState();//Add by ChengSk - 20190929

                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);      //Update by ChengSk - 2017/07/25
                            int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                            if (global_IsTest != 0) //add by xcw 20201211
                            {
                                MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                                LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_COLOR_GRADE_INFO, null);//调换发送命令次序

                            if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                        }

                        m_mainForm.staticCount = 0;  //切换用户后，分批显示初始化 Add by ChengSk - 20171220
                    }
                    this.Close();
                }
                else
                {
                    //MessageBox.Show("请选择正确的配置文件！");
                    //MessageBox.Show("0x3000101E Please select the correct configuration file!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("0x3000101E " + LanguageContainer.LoadConfigNewFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.LoadConfigNewFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("载入配置失败:"+ex);
                //MessageBox.Show("0x1000100B Load configuration failed:"+ex,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                MessageBox.Show("0x1000100B " + LanguageContainer.LoadConfigNewFormMessagebox3Text[GlobalDataInterface.selectLanguageIndex] + ex,
                    LanguageContainer.LoadConfigNewFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ConfiglistBox.Focus();
                return;
            }


        }

        /// <summary>
        /// 删除配置控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteConfigbutton_Click(object sender, EventArgs e)
        {
            try
            {
                int nSelIndex = this.ConfiglistBox.SelectedIndex;
                if (nSelIndex < 0)
                    return;
                //DialogResult result = MessageBox.Show("确定要删除选中的配置文件", "删除配置文件", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                //DialogResult result = MessageBox.Show("0x3000101F Do you want to delete the selected configuration file", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                DialogResult result = MessageBox.Show("0x3000101F " + LanguageContainer.LoadConfigNewFormMessagebox4Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.LoadConfigNewFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    string FileName = System.Environment.CurrentDirectory;
                    FileName += "\\config\\" + GlobalDataInterface.VERSION_SHOW + "\\";
                    FileName += (string)this.ConfiglistBox.Items[this.ConfiglistBox.SelectedIndex];
                    if (m_IsProjectConfig)
                        FileName += ".exp";
                    else
                        FileName += ".cmc";
                    FileInfo configFile = new FileInfo(FileName);
                    this.ConfiglistBox.Items.Remove(this.ConfiglistBox.Items[nSelIndex]);
                    configFile.Delete();
                    if (this.ConfiglistBox.Items.Count > 0)
                        this.ConfiglistBox.SelectedIndex = 0;
                }
                else
                {
                    this.ConfiglistBox.Focus();
                    return;

                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("LoadConfigNewForm中函数DeleteConfigbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("LoadConfigNewForm中函数DeleteConfigbutton_Click出错" + ex);
#endif
            }
        }

        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }


}
