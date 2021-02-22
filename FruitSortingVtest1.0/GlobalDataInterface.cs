
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Collections;
using System.Diagnostics;
using Common;
using TcpIP;
using FruitSortingVtest1._0;
using System.Windows.Forms;
using System.IO;
using FruitSortingVtest1.DB;
using System.Data;
using FruitSortingVtest1;
using System.Collections.Concurrent;
using System.Data.SqlClient;

namespace Interface
{
    public enum GRADE_CLASSIFY_TYPE : int
    {
        CLASSIFY_UNKONWN = 0, //未定义
        CLASSIFY_QUAL = 1, //品质
        CLASSIFY_SIZE = 2, //尺寸
        CLASSIFY_WEIGHT = 4, //重量

        CLASSIFY_BIG_MASK = 0x000F,
        CLASSIFY_SIZE_DIAMETER = 0x0010, //尺寸-直径
        CLASSIFY_SIZE_AREA = 0x0020, //尺寸-面积
        CLASSIFY_SIZE_VOLUME = 0x0040, //尺寸-体积

        CLASSIFY_SIZE_MIN_DIAMETER = 0x0080, //尺寸-最小直径
        CLASSIFY_SIZE_MAX_DIAMETER = 0x0100, //尺寸-最大直径
        CLASSIFY_SIZE_VERTICAL = 0x0200, //尺寸-垂直直径

        CLASSIFY_WEIGHT_GRAMS = 0x0400, //重量-克
        CLASSIFY_WEIGHT_NUMBER = 0x0800, //重量-个数
    }

   


    class GlobalDataInterface
    {
        #region 全局FSM上/下行参数
        
        #endregion
        #region 全局FSM上行参数
        public static stGlobal[] globalIn_defaultInis = new stGlobal[ConstPreDefine.MAX_SUBSYS_NUM]; //（I/O）
        public static stStatistics[] globalIn_statistics = new stStatistics[ConstPreDefine.MAX_SUBSYS_NUM];
        public static stFruitGradeInfo globalIn_gradeInfo = new stFruitGradeInfo(true);
        public static stWeightResult globalIn_weightresult = new stWeightResult(true);
        public static List<stWaveInfo> globalIn_wavelist = new List<stWaveInfo>(100);
        public static int nVer = 0;  //modify by xcw 20200604 

        public static int globalIn_nBurningProgress = 0;
        public static int Version = 40201;   //版本号保存加载验证
        public static string VERSION_SHOW = "4.2.1";//保存加载验证

        #endregion

        #region 全局IPM上行参数
        public static stImageData globalIn_image;
        public static stSpliceImageData globalIn_spliceimage;
        public static byte[] globalIn_spliceimgBin;   //仅彩色相机，图像数据后面才会有bin图 Add by ChengSk - 20190827
        public static stSpliceImageData globalIn_spotImage;
        stWhiteBalanceCoefficient globalIn_whiteBalanceCoefficient = new stWhiteBalanceCoefficient(true);
        stShutterAdjust globalIn_ShutterAdjust = new stShutterAdjust(true);
        #endregion

        #region 全局FSM下行参数
        public static stSysConfig globalOut_SysConfig = new stSysConfig(true);
        public static stGradeInfo globalOut_GradeInfo = new stGradeInfo(true);
        public static stExitInfo[] globalOut_ExitInfo = new stExitInfo[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM];
        public static stWeightBaseInfo[] globalOut_WeightBaseInfo = new stWeightBaseInfo[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM];
        public static stParas[] globalOut_Paras = new stParas[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM];
        public static stGlobalExitInfo[] globalOut_GlobalExitInfo = new stGlobalExitInfo[ConstPreDefine.MAX_SUBSYS_NUM];
        public static stGlobalWeightBaseInfo[] globalOut_GlobalWeightBaseInfo = new stGlobalWeightBaseInfo[ConstPreDefine.MAX_SUBSYS_NUM];
        //public static stSpotDetectThresh[] globalOut_SpotDetectThresh = new stSpotDetectThresh[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM];
        public static stMotorInfo[] globalOut_MotorInfo = new stMotorInfo[ConstPreDefine.MAX_EXIT_NUM];
        public static stAnalogDensity globalOut_AnalogDensity = new stAnalogDensity(true);

        public static TSYS_DEV_PARAMETER globalOut_SysDevParaData = new TSYS_DEV_PARAMETER(true); //新增内部品质  Add by ChengSk - 20190114
        public static TSYS_DEV_INFORMATION[] globalOut_SysDevInfoData = new TSYS_DEV_INFORMATION[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM];
        #endregion

        /// <summary>
        /// 全局SIM参数
        /// </summary>
        #region 全局SIM下行参数
        public static bool global_SIMTest = false;  //是否开启SIM测试模式(true 测试模式，false 模拟模式)
        public static bool UpData_gradeinfo = false; //主界面 数据是否刷新(true 刷新，false 不需要刷新)
        public static bool StopCheck = false; //主界面 是否中断抽检(true 刷新，false 不需要刷新)
        public static stGradeInfo tempGradeInfo = new stGradeInfo(true);//全局临时结构，只负责传递nChecknum与临时调用exit值

        #endregion
        #region 广播源下行参数
        public static stBroadcastSysConfig broadcast_SysConfigInfo = new stBroadcastSysConfig(true);   //系统配置信息
        //public static stExitAdditionalTextData broadcast_ExitAdditionalTextInfo = new stExitAdditionalTextData(0); //出口附加信息
        public static stExitAdditionalTextData broadcast_ExitAdditionalTextInfo = new stExitAdditionalTextData(true); //出口附加信息
        public static stGradeInfo broadcast_GradeInfo = new stGradeInfo(true);       //等级信息
        public static stBroadcastStatistics broadcast_StatisticsInfo = new stBroadcastStatistics(true);//基本统计信息（所有子系统合在一起）
        #endregion


        #region 静态构造函数
        static GlobalDataInterface()
        {
            for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
            {
                globalIn_defaultInis[i] = new stGlobal(true);
                globalIn_statistics[i] = new stStatistics(true);
                globalOut_GlobalExitInfo[i] = new stGlobalExitInfo(true);
                globalOut_GlobalWeightBaseInfo[i] = new stGlobalWeightBaseInfo(true);
            }
            for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
            {
                globalOut_ExitInfo[i] = new stExitInfo(true);
                globalOut_WeightBaseInfo[i] = new stWeightBaseInfo(true);
                globalOut_SysDevInfoData[i] = new TSYS_DEV_INFORMATION(true);  //Add by ChengSk - 20190114
            }
            for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM; i++)
            {
                globalOut_Paras[i] = new stParas(true);
                //globalOut_SpotDetectThresh[i] = new stSpotDetectThresh(true);
            }
            for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM; i++)
            {
                globalOut_MotorInfo[i] = new stMotorInfo(true);
            }
        }
        #endregion

        #region 系统全局参数
        public static byte ConnectSystemNum = 0;//初始化时连接的子系统个数
        public static bool global_IsTestMode = false;//是否是测试模式(true 测试模式，false 模拟模式)
        public static bool NetStateSum;//全局网络状态
        public static int[] NetState = new int[ConstPreDefine.MAX_SUBSYS_NUM];//网络状态 
        public static bool CupStateSum = false;
        public static int[] CupState = new int[ConstPreDefine.MAX_SUBSYS_NUM];//果杯状态      
        public static bool CIRAvailable = false;        //CIR视觉
        public static bool UVAvailable = false;         //UV视觉
        public static bool WeightAvailable = false;     //重量
        public static bool InternalAvailable = false;   //内部品质
        public static bool UltrasonicAvailable = false; //超声波
        public static bool MultiFreqAvailable = false;  //倍频功能 - Add by ChengSk 20191018
        public static bool ProjectSetFormIsIntialed = false;//工程设置界面已经初始化
        public static bool SystemStructColor = false;   //颜色
        public static bool SystemStructShape = false;   //形状
        public static bool SystemStructFlaw = false;    //瑕疵
        public static bool SystemStructDensity = false; //密度
        public static bool SystemStructVolume = false;  //体积 //等级没有
        public static bool SystemStructBruise = false;  //擦伤
        public static bool SystemStructRot = false;     //腐烂
        public static bool SystemStructSugar = false;   //糖度
        public static bool SystemStructAcidity = false; //酸度
        public static bool SystemStructHollow = false;  //空心
        public static bool SystemStructSkin = false;    //浮皮
        public static bool SystemStructBrown = false;   //褐变
        public static bool SystemStructTangxin = false; //糖心
        public static bool SystemStructRigidity = false;//硬度
        public static bool SystemStructWater = false;   //含水率
        public static bool SystemStructProjectedArea = false;//投影面积  //等级没有
        //public static int GradeSizeNum = 0;//尺寸等级数量
        //public static int GradeQualityNum = 0;//品质等级数量
        // public static int ColorIntervalNum = 3;//品质-》颜色设置区间数量
        public static int ColorMinGray = 100;     //品质-》颜色对比亮度
        public static int ColorGradeNum = 1;      //品质-》颜色等级数量
        public static int ShapeGradeNum = 1;      //品质-》形状等级数量
        public static int FlawGradeNum = 1;       //品质-》瑕疵等级数量
        public static int DensityGradeNum = 1;    //品质-》密度等级数量
        public static int BruiseGradeNum = 1;     //品质-》擦伤等级数量
        public static int RotGradeNum = 1;        //品质-》腐烂等级数量
        public static int SugarGradeNum = 1;      //品质-》糖度等级数量
        public static int AcidityGradeNum = 1;    //品质-》酸度等级数量
        public static int HollowGradeNum = 1;     //品质-》空心等级数量
        public static int SkinGradeNum = 1;       //品质-》浮皮等级数量
        public static int BrownGradeNum = 1;      //品质-》褐变等级数量
        public static int TangxinGradeNum = 1;    //品质-》糖心等级数量
        public static int RigidityGradeNum = 1;   //品质-》硬度等级数量
        public static int WaterGradeNum = 1;      //品质-》含水率等级数量
        public static QualityGradeInfo Quality_GradeInfo = new QualityGradeInfo(true);//品质等级信息
        public static List<ExitState> ExitList = new List<ExitState>();//出口布局中已选为出口
        public static bool[] SubSystemIsConnected = new bool[ConstPreDefine.MAX_SUBSYS_NUM];//子系统连接标志
        public static GradeSetForm gradeForm;
        //public static int OpenProjectSetFormNumber = 0;  //Add by ChengSk - 20190111
        public static ProjectSetForm projectSet;
        public static QualityParamSetForm qualityParamSetForm;
        public static FruitParamForm fruitParamForm;
        public static WaveCaptureForm waveForm;
        public static MainForm mainform;
        public static FileStream ErrorInfoStream;
        public static BootFlashBurnForm bootFlashBurnForm;
        public static SpotDetectTestForm spotDetectTestForm ;
        public static InnerQualityForm innerQualityForm;
        public static string selectLanguage = "null";
        public static int selectLanguageIndex = 0;      //Add by ChengSk - 20180404
        public static string currentDatabase = "null";  //Add by ChengSk
        public static string dataBaseConn = "";         //Add by ChengSk
        public static int SplitterDistance2 = 450;      //拆分器上部（或左部）距离
        public static int SplitterDistance3 = 560;      //拆分器上部（或左部）距离
        public static int ExitVerticalScroll = 0;       //出口列表垂直滚动条
        public static int sendBroadcastPackageInterval = 2; //发送广播包间隔
        public static bool sendBroadcastPackage = false;    //是否发送广播包
        public static int PadSysConfigUpdate = 0; //平板系统配置更新判断符号,初始化为0
        public static int PadGradeConfigUpdate = 0; //平板等级配置更新判断符号,初始化为0
        public static float gResolutionWidthScale = 0.0f;//屏幕分辨率与原窗口尺寸比
        public static float gResolutionHeightScale = 0.0f;//屏幕分辨率与原窗口尺寸比
        public static int gIPMImageDataLength = 0;//IPM上传图像数据大小
        public static byte gIPMImageNumber = 0;//IPM上传瑕疵图像数据张数，0为彩色，1为彩色与黑白
        public static bool gGradeInterfaceFresh = true;//IPM上传瑕疵图像数据张数，0为彩色，1为彩色与黑白
        public static int nCurrentStatisticsSubsysId = 0; //该子系统向HC发送统计数据，界面刷新 Add by ChengSk - 2017/11/16
        public static float fAlarmRatioThreshold = 0;     //告警比例阈值，从本地配置文件中读取 Add by ChengSk - 20180122
        public static int nSampleOutlet = 0;              //抽检出口 Add by ChengSk - 20180124
        public static int nSampleNumber = 0;              //抽检数量 Add by ChengSk - 20180124
        public static byte nIQSEnable = 0;                //是否存在IQS Add by ChengSk - 20191111
        public static ulong uCurrentSampleExitFruitTotals = 0; //当前抽检出口水果的个数 Add by ChengSk - 20180202
        public static bool usedSeparationLogFlags = true; //是否启用分选日志（记录分选运行时间和分选效率）Add by ChengSk - 20180131
        public static string SelectlogoPathName = "Logo\\MyLogo40201.png";     //根据版本号选择需要加载的Logo图片 Add by xcw - 20200615
        public static bool uAcceptGlobal = false;     //是否接收到FSM发送Global启动命令 Add by xcw - 2020717
        public static bool DatabaseSet = true;     //是否退出数据库 Add by xcw - 20200904
        public static bool qualgradeSet = false;     //是否双击进入



        public static string ServerBindLocalIP = "127.0.0.1"; //（上传下载）绑定服务器IP
        public static string ServerURL = "http://192.168.23.9:8081/rm/DataServer/"; //（上传下载）服务器URL
        public static string UploadStartTime = "1970-01-01 00:00:00";   //上传起始时间
        public static string DownloadStartTime = "1970-01-01 00:00:00"; //下载起始时间
        public static string DeviceNumber = "RM201812041019";           //设备编号（唯一的）
        public static string MacAddress = "";    //MAC地址
        public static string FactoryTime = "";   //出厂时间
        public static string Country = "";       //国家
        public static string Area = "";          //地区
        public static string DetailAddress = ""; //详细地址
        public static string Contactor = "";     //联系人
        public static string PhoneNumber = "";   //联系电话
        public static string TVAccount = "";     //TV账号
        public static string TVPassword = "";    //TV密码
        public static List<string> lstMacAddress = new List<string>();  //本机MAC地址池

        public static bool CommportConnectFlag = false; //内部品质TCP/IP连接标志 Add by ChengSk - 20190116

        public static string SerialNum = "";//序列号 add by xcw  20200701
        //public static bool ChannelDriverReset = false;//判断工程界面通道出口设置引脚参数


        /// <summary>
        /// 内部品质
        /// </summary>
        public static string[] GridAmoTypeNameTAB_New = { "Brix", "Acidity", "Hollow", "Hardness", "Dry matter", "Water core", "Brown" };
        public static string SystemDisplayProductID = "";
        public static string SystemDisplayProductSerial = "";
        public static string SystemDisplaySpectrometerSerial = "";
        public static TSYS_DEV_PARAMETER global_SystemParaData = new TSYS_DEV_PARAMETER(true);
        public static TSYS_AMO_PARAMETER[] global_SystemAmoData = {
        new TSYS_AMO_PARAMETER(true),
        new TSYS_AMO_PARAMETER(true),
        new TSYS_AMO_PARAMETER(true),
        new TSYS_AMO_PARAMETER(true),
        new TSYS_AMO_PARAMETER(true),
        new TSYS_AMO_PARAMETER(true),
        new TSYS_AMO_PARAMETER(true)                                                };
        public static TSYS_DEV_INFORMATION global_SystemInfoData = new TSYS_DEV_INFORMATION(true);
        public static TMsgSingle1024Format global_SystemWLConvTBL = new TMsgSingle1024Format(true);
        public static TMsgWord1024Format[] global_SystemRawData = {
        new TMsgWord1024Format(true),
        new TMsgWord1024Format(true),
        new TMsgWord1024Format(true),
        new TMsgWord1024Format(true),
        new TMsgWord1024Format(true),
        new TMsgWord1024Format(true)
                                                                  };
        public static TMsgSingle151Format global_SystemCalcDxData = new TMsgSingle151Format(true);
        public static TIpcSystemSpecAbsDataFormat global_SystemClacResult = new TIpcSystemSpecAbsDataFormat(true);

        public static bool DeviceFirstConnectFlag = false;
        public static bool DeviceInitCompletedFlag = false;
        //public static bool SerialFirstFlag = false;    //
        public static int SystemFruitMountCount = 0;
        public static TMsgWord1024Format[] SpecSavedRAWValueBuffer = new TMsgWord1024Format[1000];
        public static TMsgSingle151Format[] SpecSavedJDXValueBuffer = new TMsgSingle151Format[1000];


        public static ConcurrentQueue<byte[]> SendMsgData = new ConcurrentQueue<byte[]>();    //内部品质发送队列  Add by ChengSk - 20190116
        #endregion


        #region 局部变量
        private CTcpServer m_TcpSvrImage;
        private CTcpServer m_TcpSvrStat;
        private BroadcastServer m_BroadcastSvr;//广播源
        private static CTcpClient m_TcpClient;
        private Thread m_DPThread;//数据处理线程
        private System.Threading.Timer m_HeartBeatTimer;//FSM心跳查询信息 根据globalIn_statistics上传时间，间隔超过10秒钟判断其有问题
        private System.Threading.Timer m_SendBroadcastTimer; //定时发送广播包
        [StructLayout(LayoutKind.Sequential)]
        struct stDataTime
        {
            public int Hour;
            public int Minute;
            public int Second;
        }//时间结构体
        stDataTime[] m_dataTime = new stDataTime[ConstPreDefine.MAX_SUBSYS_NUM];
        #endregion

        #region 属性
        /*是否正在连接下位机(只读)*/
        private static bool m_bConnecting = false;
        public bool IsConnecting
        {
            get { return m_bConnecting; }
        }

        /*等级分类属性*/
        private int m_nClassifyType = 0;
        public int nClassifyType
        {
            get { return m_nClassifyType; }
        }

        #endregion

        

        #region 方法

        /// <summary>
        /// 获取FSM上行数据
        /// </summary>
        /// <param name="nSrcId">发送源ID</param>
        /// <param name="nCmdId">发送命令ID</param>
        /// <param name="pData">发送数据</param>
        private void SetBuffer(int nSrcId, int nCmdId, IntPtr pData)
        {
            try
            {
                int nSubsysIdx = Commonfunction.GetSubsysIndex(nSrcId);
                nCurrentStatisticsSubsysId = nSubsysIdx; //Add by ChengSk - 2017/11/16
                //if (!GlobalDataInterface.uAcceptGlobal)
                //{
                //    MessageBox.Show("未收到FSM发送Global！");
                //    GlobalDataInterface.WriteErrorInfo("未收到FSM发送Global！");
                //    Application.Exit();
                //}
                switch (nCmdId)
                {
                    case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_CONFIG:
                        globalIn_defaultInis[nSubsysIdx] = (stGlobal)Marshal.PtrToStructure(pData, typeof(stGlobal));
                        VersionJudgment(globalIn_defaultInis[nSubsysIdx]);
                        HandleExceptionBatch(globalIn_defaultInis[nSubsysIdx]);
                        if (UpSetTextCallbackEvent != null)
                        {
                            UpSetTextCallbackEvent();
                        }
                        
                        break;
                    case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_STATISTICS:
                        if (GlobalDataInterface.mainform != null)
                        {
                            globalIn_statistics[nSubsysIdx] = (stStatistics)Marshal.PtrToStructure(pData, typeof(stStatistics));
                        }
                        break;
                    case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_GRADEINFO:
                        globalIn_gradeInfo = (stFruitGradeInfo)Marshal.PtrToStructure(pData, typeof(stFruitGradeInfo));
                        break;
                    case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_WEIGHTINFO:
                        globalIn_weightresult = (stWeightResult)Marshal.PtrToStructure(pData, typeof(stWeightResult));
                        
                        break;
                    case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_WAVEINFO:
                        stWaveInfo waveinfo = new stWaveInfo(true);
                        waveinfo = (stWaveInfo)Marshal.PtrToStructure(pData, typeof(stWaveInfo));
                        lock (this)
                        {
                            if (globalIn_wavelist.Count >= 100)//保存数据不超过100组
                                globalIn_wavelist.Remove(globalIn_wavelist[0]);
                            globalIn_wavelist.Add(waveinfo);
                        }
                        break;
                    case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_VERSIONERROR:
                        nVer = Marshal.ReadInt32(pData);
                        /**/
                        string strFSMVersion = nVer.ToString();
                        string strSubFSMVersion = strFSMVersion.Substring(0, strFSMVersion.Length - 2); //获取主版本号+副版本号
                        string strHCVersion = ConstPreDefine.VERSION.ToString();
                        string strSubHCVersion = strHCVersion.Substring(0, strHCVersion.Length - 2); //获取主版本号+副版本号
                        if (nVer != ConstPreDefine.VERSION)
                        //if (strSubFSMVersion != strSubHCVersion) //Modify by ChengSk - 20180403 //Note by ChengSk - 20180611
                        {
                            int nMainVersionNo = nVer / 10000;
                            int nViceVersionNo = (nVer - nMainVersionNo * 10000) / 100;
                            int nAmendVersionNo = nVer - nMainVersionNo * 10000 - nViceVersionNo * 100;
                            string fsmVersion = string.Format("{0}.{1}.{2}", nMainVersionNo, nViceVersionNo, nAmendVersionNo);
                            //MessageBox.Show(string.Format("版本不同！上位机版本为3.1，下位机为{0}", nVer));
                            //MessageBox.Show(string.Format("0x10002001 Version mismatch! HC is V{0}, LC is V{1}", ConstPreDefine.VERSION_SHOW, nVer), "Error", MessageBoxButtons.OK);//2013-3-26 ivycc  //Modify by ChengSk - 20171023
                            MessageBox.Show(string.Format("0x10002001 Version mismatch! HC is V{0}, LC is V{1}", ConstPreDefine.VERSION_SHOW, fsmVersion), "Error", MessageBoxButtons.OK);//2013-3-26 ivycc  //Modify by ChengSk - 20180611
                            Application.Exit();
                        }
                        break;
                    case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_BURN_FLASH_PROGRESS:
                        globalIn_nBurningProgress = Marshal.ReadInt32(pData);
                        break;
                    case (int)SIM_HMI_COMMAND_TYPE.SIM_HMI_DISPLAY_ON:
                        //globalIn_nBurningProgress = Marshal.ReadInt32(pData);
                        break;
                    case (int)SIM_HMI_COMMAND_TYPE.SIM_HMI_INSPECTION_ON:
                        tempGradeInfo = (stGradeInfo)Marshal.PtrToStructure(pData, typeof(stGradeInfo));
                        globalOut_GradeInfo.nCheckNum = tempGradeInfo.nCheckNum;
                        GlobalDataInterface.global_SIMTest = true;
                        //MessageBox.Show("CheckNum:" + globalOut_GradeInfo.nCheckNum.ToString());
                        GlobalDataInterface.nSampleNumber = int.Parse(GlobalDataInterface.globalOut_GradeInfo.nCheckNum.ToString());//add by xcw - 20200525
                        GlobalDataInterface.UpData_gradeinfo = true;
                        if (UpStatusModifyEvent != null)
                        {
                            UpStatusModifyEvent();
                        }
                        break;
                    case (int)SIM_HMI_COMMAND_TYPE.SIM_HMI_INSPECTION_OFF:           //Add by xcw - 20200520
                        GlobalDataInterface.StopCheck = true;
                        if (UpStopCheckSampleEvent != null)
                        {
                            UpStopCheckSampleEvent();
                        }
                        break;
                    default: break;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("GlobalDataInterface中函数SetBuffer出错"+e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数SetBuffer出错"+e);
#endif
            }
        }

        /// <summary>
        /// 获取IPM上行数据
        /// </summary>
        /// <param name="nSrcId">发送源ID</param>
        /// <param name="nCmdId">发送命令ID</param>
        /// <param name="pData">发送数据</param>
        private void SetBufferForImage(int nSrcId, int nCmdId, IntPtr pData)
        {
            try
            {
               
                //IntPtr pActualData = IntPtr.Zero;
                byte[] tempInfo;
                switch (nCmdId)
                {
                    case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_IMAGE_SPOT:
                        gIPMImageDataLength = Marshal.ReadInt32(pData, 0);
                        gIPMImageNumber = Marshal.ReadByte(pData, 4);
                        globalIn_spotImage = new stSpliceImageData(gIPMImageDataLength);
                        tempInfo = new byte[Marshal.SizeOf(typeof(stSpliceImageInfo))];
                        Marshal.Copy(pData, tempInfo, 0, tempInfo.Length);
                        globalIn_spotImage.imageInfo = (stSpliceImageInfo)(Common.Commonfunction.BytesToStruct(tempInfo, typeof(stSpliceImageInfo)));
                        pData += tempInfo.Length;
                        Marshal.Copy(pData, globalIn_spotImage.imagedataC, 0, gIPMImageDataLength);
                        if (gIPMImageNumber == 2)
                        {
                            pData += gIPMImageDataLength;
                            Marshal.Copy(pData, globalIn_spotImage.imagedataC, 0, gIPMImageDataLength);
                        }
                        break;
                    case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_IMAGE:
                        gIPMImageDataLength = Marshal.ReadInt32(pData, 0);
                        globalIn_image = new stImageData(gIPMImageDataLength);
                        tempInfo = new byte[Marshal.SizeOf(typeof(stImageInfo))];
                        Marshal.Copy(pData, tempInfo, 0, tempInfo.Length);
                        globalIn_image.imageInfo = (stImageInfo)(Common.Commonfunction.BytesToStruct(tempInfo, typeof(stImageInfo)));
                        pData += tempInfo.Length;
                        Marshal.Copy(pData, globalIn_image.imagedata, 0, gIPMImageDataLength);
                        //Marshal.Copy(pData, globalIn_image.imagedata, 0, gIPMImageDataLength);
                        break;
                    case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_IMAGE_SPLICE:
                        gIPMImageDataLength = Marshal.ReadInt32(pData, 0);
                        globalIn_spliceimage = new stSpliceImageData(gIPMImageDataLength);
                        tempInfo = new byte[Marshal.SizeOf(typeof(stSpliceImageInfo))];
                        Marshal.Copy(pData, tempInfo, 0, tempInfo.Length);
                        globalIn_spliceimage.imageInfo = (stSpliceImageInfo)(Common.Commonfunction.BytesToStruct(tempInfo, typeof(stSpliceImageInfo)));
                        pData += tempInfo.Length;
                        Marshal.Copy(pData, globalIn_spliceimage.imagedataC, 0, gIPMImageDataLength);
                        //判断是否有bin图 - Add by ChengSk 20190827
                        if(globalIn_spliceimage.imageInfo.bPixelBit == 2)
                        {
                            globalIn_spliceimgBin = new byte[globalIn_spliceimage.imageInfo.width * globalIn_spliceimage.imageInfo.height];
                            pData += gIPMImageDataLength;
                            Marshal.Copy(pData, globalIn_spliceimgBin, 0, globalIn_spliceimgBin.Length);
                        }
                        break;
                    case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_AUTOBALANCE_COEFFICIENT:
                        globalIn_whiteBalanceCoefficient = (stWhiteBalanceCoefficient)Marshal.PtrToStructure(pData, typeof(stWhiteBalanceCoefficient));
                        break;
                    case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_SHUTTER_ADJUST: //Add by ChengSk - 20190627
                        globalIn_ShutterAdjust = (stShutterAdjust)Marshal.PtrToStructure(pData, typeof(stShutterAdjust));
                        break;
                    default: break;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("GlobalDataInterface中函数SetBufferForImage出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数SetBufferForImage出错" + e);
#endif
            }
        }

        /// <summary>
        /// TCP与处理线程初始化
        /// </summary>
        public void Init()
        {
            try
            {
                m_TcpClient = new CTcpClient(ref SubSystemIsConnected);
                m_TcpSvrImage = new CTcpServer();
                m_TcpSvrStat = new CTcpServer();
                m_BroadcastSvr = new BroadcastServer(); //广播

                m_TcpSvrImage.Start(ConstPreDefine.HC_PORT1_NUM, SetBufferForImage, false);
                m_TcpSvrStat.Start(ConstPreDefine.HC_PORT2_NUM, SetBuffer, false);
                if (m_DPThread == null)
                {
                    m_DPThread = new Thread(DataProcessThread);
                    m_DPThread.Priority = ThreadPriority.Normal;
                    m_DPThread.IsBackground = true;
                    m_DPThread.Start();
                }

                m_HeartBeatTimer = new System.Threading.Timer(InquiryHeartBeatInfo, null, 30000, 10000);//延迟30秒启动，10秒间隔
                m_SendBroadcastTimer = new System.Threading.Timer(SendBroadcastPackage, null, 10000, 5000);//延迟10秒启动，5秒间隔 //由Modify by ChengSk - 20180119
            }
            catch (Exception e)
            {
                Trace.WriteLine("GlobalDataInterface中函数Init出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数Init出错" + e);
#endif
            }
        }

        /// <summary>
        /// 销毁TCPServer绑定的Socket 
        /// </summary>
        public void DestroyTCPServerMarsterSocket() //Add by ChengSk - 20180723
        {
            try
            {
                if (m_TcpSvrImage != null)
                    m_TcpSvrImage.Dispose();
                if (m_TcpSvrStat != null)
                    m_TcpSvrStat.Dispose();
            }
            catch (Exception e)
            {
                Trace.WriteLine("GlobalDataInterface中函数DestroyTCPServerMarsterSocket出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数DestroyTCPServerMarsterSocket出错" + e);
#endif
            }
        }

        //测试使用
        public void InitSendBroadcast()
        {
            try
            {
                m_BroadcastSvr = new BroadcastServer(); //广播
                m_SendBroadcastTimer = new System.Threading.Timer(SendBroadcastPackage, null, 10000, 1000);//延迟10秒启动，1秒间隔
            }
            catch (Exception ex)
            { 
            }
        }

        MessageBoxForm messageBoxForm;
        /// <summary>
        /// 查询心跳信息（根据FSM统计信息上传时间间隔超过10秒，判断连接失败）
        /// </summary>
        /// <param name="state"></param>
        void InquiryHeartBeatInfo(object state)
        {
            try
            {
                if (globalOut_SysConfig.nSubsysNum > 0)
                {
                    int nowSecond = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                    int preSecond = 0;
                    for (int i = 0; i < globalOut_SysConfig.nSubsysNum; i++)
                    {
                        preSecond = m_dataTime[i].Hour * 3600 + m_dataTime[i].Minute * 60 + m_dataTime[i].Second;
                        if ((nowSecond - preSecond) > 10)
                        {
                            if (messageBoxForm == null)
                            {
                                if (GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount >= 100) //已经开始加工，由于FSM网络异常，HC需要自动结束加工
                                {
                                    if (UpAutoEndProcessEvent != null)
                                    {
                                        UpAutoEndProcessEvent();   //执行自动结束加工 Add by ChengSk - 20190513
                                    }
                                }

                                //messageBoxForm = new MessageBoxForm(string.Format("子系统{0}连接失败！", i + 1));
                                //messageBoxForm = new MessageBoxForm(string.Format("0x10000003 Subsystem {0} connection failure,please check the connection!", i + 1));//2013-3-26 ivycc
                                messageBoxForm = new MessageBoxForm(string.Format("0x10000003 心跳 " +
                                    LanguageContainer.GlobalDataInterfaceMessagebox5Sub4Text[GlobalDataInterface.selectLanguageIndex] + "{0}" +
                                    LanguageContainer.GlobalDataInterfaceMessagebox5Sub5Text[GlobalDataInterface.selectLanguageIndex],
                                    i + 1));//2013-3-26 ivycc
                                //MessageBox.Show("初始化");
                                if ( messageBoxForm.ShowDialog() == DialogResult.OK)
                                {
                                    messageBoxForm.Close();
                                    messageBoxForm.Dispose();
                                    messageBoxForm = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("GlobalDataInterface中函数InquiryHeartBeatInfo出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数InquiryHeartBeatInfo出错" + e);
#endif
            }
        }

        /// <summary>
        /// 发送广播包
        /// </summary>
        /// <param name="state"></param>
        void SendBroadcastPackage(object state)
        {
            if (!sendBroadcastPackage)
                return;
            CommandPackage cmdPackage;
            int nlength = 0;
            byte[] cmd = null;
            byte[] data = null;
            //int TmrNum = DateTime.Now.Second % 10; //每当在0，10,20,30,40,50时发送配置信息，其它时间发送统计信息
            int TmrNum = 0;
            if ((sendBroadcastPackageInterval--) % 2 == 0)
            {
                TmrNum = 0;
            }
            else
            {
                TmrNum = 1;
                if (sendBroadcastPackageInterval == 0) sendBroadcastPackageInterval = 2;
            }

            try
            {
                switch (TmrNum)
                {
                    case 0:
                        GetBroadcastSysConfigInfo(); //获取最新配置信息
                        //命令头
                        cmdPackage.nTypeId = (int)BROADCAST_SOURCE_TYPE.BROADCAST_SOURCE_FSM;
                        cmdPackage.nCmdId = (int)BROADCAST_COMMAND_TYPE.BROADCAST_CMD_SYSCONFIG;
                        cmdPackage.nPadConfigUpdate = GlobalDataInterface.PadSysConfigUpdate;
                        nlength = Marshal.SizeOf(typeof(CommandPackage));
                        cmd = new byte[nlength];
                        cmd = Commonfunction.StructToBytes(cmdPackage);
                        //数据
                        nlength = Marshal.SizeOf(typeof(stBroadcastSysConfig));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(broadcast_SysConfigInfo);
                        //发送
                        m_BroadcastSvr.SendBroadcastPackage(cmd, data);

                        GetBroadcastExitAdditionalTextInfo();  //获取最新出口显示信息
                        //命令头
                        cmdPackage.nTypeId = (int)BROADCAST_SOURCE_TYPE.BROADCAST_SOURCE_FSM;
                        cmdPackage.nCmdId = (int)BROADCAST_COMMAND_TYPE.BROADCAST_CMD_EXITDISPLAYINFO;
                        cmdPackage.nPadConfigUpdate = GlobalDataInterface.PadSysConfigUpdate;
                        nlength = Marshal.SizeOf(typeof(CommandPackage));
                        cmd = new byte[nlength];
                        cmd = Commonfunction.StructToBytes(cmdPackage);
                        //数据
                        nlength = Marshal.SizeOf(typeof(stExitAdditionalTextData));
                        //nlength = broadcast_SysConfigInfo.sysConfig.nExitNum * ConstPreDefine.MAX_EXIT_ADDITIONALNAME_LENGTH;
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(broadcast_ExitAdditionalTextInfo);
                        //发送
                        m_BroadcastSvr.SendBroadcastPackage(cmd, data);

                        GetBroadcastGradeInfo();     //获取最新等级信息
                        //命令头
                        cmdPackage.nTypeId = (int)BROADCAST_SOURCE_TYPE.BROADCAST_SOURCE_FSM;
                        cmdPackage.nCmdId = (int)BROADCAST_COMMAND_TYPE.BROADCAST_CMD_GRADEINFO;
                        cmdPackage.nPadConfigUpdate = GlobalDataInterface.PadGradeConfigUpdate;
                        nlength = Marshal.SizeOf(typeof(CommandPackage));
                        cmd = new byte[nlength];
                        cmd = Commonfunction.StructToBytes(cmdPackage);
                        //数据
                        nlength = Marshal.SizeOf(typeof(stGradeInfo));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(broadcast_GradeInfo);
                        //发送
                        m_BroadcastSvr.SendBroadcastPackage(cmd, data);
                        break;

                    default:
                        GetBroadcastStatisticsInfo();//获取最新统计信息
                        //命令头
                        cmdPackage.nTypeId = (int)BROADCAST_SOURCE_TYPE.BROADCAST_SOURCE_FSM;
                        cmdPackage.nCmdId = (int)BROADCAST_DATA_TYPE.BROADCAST_DATA_STATISTICS;
                        cmdPackage.nPadConfigUpdate = -1;
                        nlength = Marshal.SizeOf(typeof(CommandPackage));
                        cmd = new byte[nlength];
                        cmd = Commonfunction.StructToBytes(cmdPackage);
                        //数据
                        nlength = Marshal.SizeOf(typeof(stBroadcastStatistics));
                        data = new byte[nlength];
                        
                        data = Commonfunction.StructToBytes(broadcast_StatisticsInfo);
                        int a = Marshal.SizeOf(broadcast_StatisticsInfo);
                        //发送
                        m_BroadcastSvr.SendBroadcastPackage(cmd, data);
                        break;
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine("GlobalDataInterface中函数SendBroadcastPackage出现错误，错误原因："+ex.ToString());
            }
            
        }
        
        #region 自定义事件

    /// <summary>
    /// 重量设置界面更新
    /// </summary> 
    public delegate void WeightInfoEventHandler(stWeightResult weightResult);
    public static event WeightInfoEventHandler UpWeightInfoEvent;

    /// <summary>
    /// 波形捕捉刷新
    /// </summary>
    public delegate void WaveInfoEventHandler();
    public static event WaveInfoEventHandler UpWaveInfoEvent;

    /// <summary>
    /// 图像上传
    /// </summary>
    public delegate void ImageDataEventHandler(stImageData imageData);
    public static event ImageDataEventHandler UpImageDataEvent;

    /// <summary>
    /// 颜色图像上传
    /// </summary>
    public delegate void SpliceImageDataEventHandler(stSpliceImageData spliceImageData);
    public static event SpliceImageDataEventHandler UpSpliceImageDataEvent;

    public static event SpliceImageDataEventHandler UpSpliceFlawImageDataEvent;//add by xcw 20200909
    /// <summary>
    /// 瑕疵图像上传
    /// </summary>
    public delegate void SpotImageDataEventHandler(stSpliceImageData spotImageData);
    public static event SpotImageDataEventHandler UpSpotImageDataEvent;

    /// <summary>
    /// 擦伤图像上传
    /// </summary>
    public delegate void BruiseImageDataEventHandler(stSpliceImageData spotImageData); //传输图片时与瑕疵结构相同，公用结构体
    public static event BruiseImageDataEventHandler UpBruiseImageDataEvent;

    /// <summary>
    /// 腐烂图像上传
    /// </summary>
    public delegate void RotImageDataEventHandler(stSpliceImageData spotImageData);    //传输图片时与瑕疵结构相同，公用结构体
    public static event RotImageDataEventHandler UpRotImageDataEvent;

    /// <summary>
    /// 自动白平衡系数上传
    /// </summary>
    public delegate void AutoWhiteBalanceInfoEventHandler(int nSrcID,stWhiteBalanceCoefficient WBcoefficient);
    public static event AutoWhiteBalanceInfoEventHandler UpAutoWhiteBalanceInfoEvent;

    /// <summary>
    /// 多相机调节信息上传
    /// </summary>
    public delegate void ShutterAdjustInfoEventHandler(int nSrcID, stShutterAdjust stshutterAdjust);
    public static event ShutterAdjustInfoEventHandler UpShutterAdjustInfoEvent;

    /// <summary>
    /// 水果等级信息上传
    /// </summary>
    public delegate void FruitGradeInfoEventHandler(stFruitGradeInfo fruitGradeInfo);
    public static event FruitGradeInfoEventHandler UpFruitGradeInfoEvent;

    /// <summary>
    /// 水果统计信息上传
    /// </summary>
    public delegate void StatisticInfoEventHandler();
    public static event StatisticInfoEventHandler UpStatisticInfoEvent;

    /// <summary>
    /// FSM程序烧写进度
    /// </summary>
    public delegate void BurnFlashProgressEventHandler(int burnProgress);
    public static event BurnFlashProgressEventHandler UpBurnFlashProgressEvent;

    /// <summary>
    /// 自动“结束加工”
    /// </summary>
    public delegate void AutoEndProcessEventHandler();  //Add by ChengSk - 20190513
    public static event AutoEndProcessEventHandler UpAutoEndProcessEvent;

    /// <summary>
    /// 接收SIM刷新出口
    /// </summary>
    public delegate void StatusModify();  //Add by xcw - 20200524
    public static event StatusModify UpStatusModifyEvent;
    
    /// <summary>
    /// 接收SIM停止抽检命令
    /// </summary>
    public delegate void StopCheckSample();  //Add by xcw - 20200524
    public static event StopCheckSample UpStopCheckSampleEvent;
    /// <summary>
    /// 界面text更新
    /// </summary> 
    public delegate void SetTextCallback();
    public static event SetTextCallback UpSetTextCallbackEvent;//Add by xcw - 20200713
    /// <summary>
    /// 主界面等级刷新
    /// </summary>
    public delegate void GradeInterfaceFreshEvent();
    
        #endregion


        ////测试
        ////ulong count = 0;
        ////ulong weightcount = 0;
        ////测试

        /// <summary>
        /// 数据处理线程
        /// </summary>
        void DataProcessThread()
        {
                bool bCreated;
                EventWaitHandle waitEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "UpStatics", out bCreated);
                while (true)
                {
                    try
                    {
                        if (CTcpServer.CommandHeadQue.Count > 0)
                        {
                            CommandHead cmdHead = CTcpServer.CommandHeadQue.Dequeue();
                            switch (cmdHead.nCmdId)
                            {
                                case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_CONFIG:
                                    RefreshDefaultIni(cmdHead.nSrcId);
                                    break;
                                case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_STATISTICS:
                                    ////测试
                                    //count += 1;
                                    //weightcount += 100;
                                    //GlobalDataInterface.globalIn_statistics[0].nTotalCount += count;
                                    //GlobalDataInterface.globalIn_statistics[0].nWeightCount += weightcount;
                                    //GlobalDataInterface.globalIn_statistics[0].nGradeCount[0] += count;
                                    //GlobalDataInterface.globalIn_statistics[0].nWeightGradeCount[0] += weightcount;
                                    ////测试
                                    int qulNum = 1;
                                   if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//品质与尺寸或者品质与重量
                                    {
                                        qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                                    }
                                    int sizeNum = 1;
                                    if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                                        sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                                    for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                                    {
                                        for (int i = 0; i < qulNum; i++)
                                        {
                                            for (int j = 0; j < sizeNum; j++)
                                            {
                                                if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum != 0)
                                                {
                                                    if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x0001) == 0x0001)//克
                                                    {
                                                        if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum == 0)
                                                            GlobalDataInterface.globalIn_statistics[k].nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] = 0;
                                                        else
                                                            GlobalDataInterface.globalIn_statistics[k].nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] = (int)(GlobalDataInterface.globalIn_statistics[k].nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / ((ulong)GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum));
                                                    }
                                                    else
                                                    {
                                                        if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum == 0)
                                                            GlobalDataInterface.globalIn_statistics[k].nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] = 0;
                                                        else
                                                            GlobalDataInterface.globalIn_statistics[k].nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] = (int)(GlobalDataInterface.globalIn_statistics[k].nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / ((ulong)GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    m_dataTime[Commonfunction.GetSubsysIndex(cmdHead.nSrcId)].Hour = DateTime.Now.Hour;
                                    m_dataTime[Commonfunction.GetSubsysIndex(cmdHead.nSrcId)].Minute = DateTime.Now.Minute;
                                    m_dataTime[Commonfunction.GetSubsysIndex(cmdHead.nSrcId)].Second = DateTime.Now.Second;
                                    
                                    //Add By ChengSk 20131108  动态数据交互入口      
                                    if (UpdateDataInterfaceEvent != null)
                                    {
                                        Struct1ToStruct2();
                                        UpdateDataInterfaceEvent(dataInterface);
                                    }

                                    if (UpStatisticInfoEvent != null)
                                        UpStatisticInfoEvent();

                                    if(mainform != null)
                                    waitEvent.Set();

                                    //Add By ChengSk 20131108  End
                                    break;
                                case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_GRADEINFO:
                                    if (GlobalDataInterface.fruitParamForm == null && GlobalDataInterface.projectSet == null)
                                    {
                                        break;
                                    }
                                    UpFruitGradeInfoEvent(globalIn_gradeInfo);
                                    break;
                                case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_WEIGHTINFO:
                                    if (GlobalDataInterface.projectSet != null)
                                    {
                                        UpWeightInfoEvent(globalIn_weightresult);
                                    }
                                    break;
                                case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_WAVEINFO:
                                    if (GlobalDataInterface.waveForm != null)
                                    {
                                        UpWaveInfoEvent();
                                    }
                                    break;
                                case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_BURN_FLASH_PROGRESS:
                                    if (GlobalDataInterface.bootFlashBurnForm != null)
                                    {
 
                                    }
                                    break;
                                case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_IMAGE_SPOT:
                                    if (GlobalDataInterface.qualityParamSetForm != null)
                                    {
                                        UpSpotImageDataEvent(globalIn_spotImage);
                                        //擦伤、腐烂用同一结构
                                        UpBruiseImageDataEvent(globalIn_spotImage);
                                        UpRotImageDataEvent(globalIn_spotImage);
                                    }
                                    if (GlobalDataInterface.spotDetectTestForm != null)
                                        UpSpotImageDataEvent(globalIn_spotImage);
                                    break;
                                case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_IMAGE:
                                    if (GlobalDataInterface.projectSet == null )
                                    {
                                        UpBurnFlashProgressEvent(globalIn_nBurningProgress);
                                        break;
                                    }
                                    UpImageDataEvent(globalIn_image);
                                    break;
                                case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_IMAGE_SPLICE:
                                    if (GlobalDataInterface.qualityParamSetForm != null)
                                    {
                                        UpSpliceImageDataEvent(globalIn_spliceimage);
                                        UpSpliceFlawImageDataEvent(globalIn_spliceimage);
                                }
                                    break;
                                case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_AUTOBALANCE_COEFFICIENT:
                                    if (GlobalDataInterface.projectSet != null)
                                    {
                                        UpAutoWhiteBalanceInfoEvent(cmdHead.nSrcId, globalIn_whiteBalanceCoefficient);
                                    }
                                    break;
                                case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_SHUTTER_ADJUST:
                                    if (GlobalDataInterface.projectSet != null)
                                    {
                                        UpShutterAdjustInfoEvent(cmdHead.nSrcId, globalIn_ShutterAdjust);
                                    }
                                    break;

                                default: break;
                            } // switch
                        } // if
                        Thread.Sleep(10);//降低CPU消耗
                    }// try
                    catch (Exception e)
                    {
                        Trace.WriteLine("GlobalDataInterface中函数DataProcessThread出错" + e);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数DataProcessThread出错" + e);
#endif
                    }
                } // while
        }

        /// <summary>
        /// 初始化配置参数
        /// </summary>
        /// <param name="nSubsysId">子系统ID号</param>
        private void RefreshDefaultIni(int nSubsysId)
        {
            try
            {
                int nSubsysIdx = Commonfunction.GetSubsysIndex(nSubsysId);
                if (nSubsysIdx < 0 || nSubsysIdx >= ConstPreDefine.MAX_SUBSYS_NUM) return;

                //GradeInfo
                globalOut_GradeInfo.ToCopy(globalIn_defaultInis[nSubsysIdx].grade);
                m_nClassifyType = (int)GRADE_CLASSIFY_TYPE.CLASSIFY_UNKONWN;

                ////标签信息
                //globalOut_GradeInfo.nTagInfo[0] = byte.Parse(Commonfunction.GetAppSetting("标签启用"));
                //globalOut_GradeInfo.nTagInfo[1] = byte.Parse(Commonfunction.GetAppSetting("标签B均值"));
                //globalOut_GradeInfo.nTagInfo[2] = byte.Parse(Commonfunction.GetAppSetting("标签G均值"));
                //globalOut_GradeInfo.nTagInfo[3] = byte.Parse(Commonfunction.GetAppSetting("标签R均值"));
                //globalOut_GradeInfo.nTagInfo[4] = byte.Parse(Commonfunction.GetAppSetting("标签Y值范围"));
                //globalOut_GradeInfo.nTagInfo[5] = byte.Parse(Commonfunction.GetAppSetting("标签H值范围"));  //modify by xcw 20200709

                //sys
                globalOut_SysConfig.nSystemInfo = globalIn_defaultInis[nSubsysIdx].sys.nSystemInfo;
                globalOut_SysConfig.nCameraType = globalIn_defaultInis[nSubsysIdx].sys.nCameraType;
                globalOut_SysConfig.width = globalIn_defaultInis[nSubsysIdx].sys.width;
                globalOut_SysConfig.height = globalIn_defaultInis[nSubsysIdx].sys.height;
                globalOut_SysConfig.packetSize = globalIn_defaultInis[nSubsysIdx].sys.packetSize;
                globalOut_SysConfig.nClassificationInfo = globalIn_defaultInis[nSubsysIdx].sys.nClassificationInfo;
                globalOut_SysConfig.multiFreq = globalIn_defaultInis[nSubsysIdx].sys.multiFreq;

                globalOut_SysConfig.CIRClassifyType = globalIn_defaultInis[nSubsysIdx].sys.CIRClassifyType;
                globalOut_SysConfig.UVClassifyType = globalIn_defaultInis[nSubsysIdx].sys.UVClassifyType;
                globalOut_SysConfig.WeightClassifyTpye = globalIn_defaultInis[nSubsysIdx].sys.WeightClassifyTpye; //Add by ChengSk - 20190828
                globalOut_SysConfig.InternalClassifyType = globalIn_defaultInis[nSubsysIdx].sys.InternalClassifyType;
                globalOut_SysConfig.UltrasonicClassifyType = globalIn_defaultInis[nSubsysIdx].sys.UltrasonicClassifyType;
                globalOut_SysConfig.IfWIFIEnable = globalIn_defaultInis[nSubsysIdx].sys.IfWIFIEnable;  //Add by ChengSk - 20190828
                globalOut_SysConfig.CheckExit = globalIn_defaultInis[nSubsysIdx].sys.CheckExit;        //Add by ChengSk - 20190828
                globalOut_SysConfig.CheckNum = globalIn_defaultInis[nSubsysIdx].sys.CheckNum;          //Add by ChengSk - 20190828
                globalOut_SysConfig.nIQSEnable = globalIn_defaultInis[nSubsysIdx].sys.nIQSEnable;      //Add by ChengSk - 20191111
                if ((globalOut_SysConfig.CIRClassifyType & 0x01) == 1) //颜色
                    SystemStructColor = true;
                else
                    SystemStructColor = false;
                if ((globalOut_SysConfig.CIRClassifyType & 0x02) == 2) //形状
                    SystemStructShape = true;
                else
                    SystemStructShape = false;
                if ((globalOut_SysConfig.CIRClassifyType & 0x04) == 4) //瑕疵
                    SystemStructFlaw = true;
                else
                    SystemStructFlaw = false;
                if ((globalOut_SysConfig.CIRClassifyType & 0x08) == 8) //体积      //Add by ChengSk - 20190708
                    SystemStructVolume = true;
                else
                    SystemStructVolume = false;
                if ((globalOut_SysConfig.CIRClassifyType & 0x10) == 16)//投影面积  //Add by ChengSk - 20190708
                    SystemStructProjectedArea = true;
                else
                    SystemStructProjectedArea = false;
                if ((globalOut_SysConfig.UVClassifyType & 0x01) == 1)  //擦伤
                    SystemStructBruise = true;
                else
                    SystemStructBruise = false;
                if ((globalOut_SysConfig.UVClassifyType & 0x02) == 2)  //腐烂
                    SystemStructRot = true;
                else
                    SystemStructRot = false;
                if ((globalOut_SysConfig.WeightClassifyTpye & 0x01) == 1)    //密度 - Add by ChengSk - 20190828
                    SystemStructDensity = true;
                else
                    SystemStructDensity = false;
                if ((globalOut_SysConfig.InternalClassifyType & 0x01) == 1)  //糖度
                    SystemStructSugar = true;
                else
                    SystemStructSugar = false;
                if ((globalOut_SysConfig.InternalClassifyType & 0x02) == 2)  //酸度
                    SystemStructAcidity = true;
                else
                    SystemStructAcidity = false;
                if ((globalOut_SysConfig.InternalClassifyType & 0x04) == 4)  //空心
                    SystemStructHollow = true;
                else
                    SystemStructHollow = false;
                if ((globalOut_SysConfig.InternalClassifyType & 0x08) == 8)  //浮皮
                    SystemStructSkin = true;
                else
                    SystemStructSkin = false;
                if ((globalOut_SysConfig.InternalClassifyType & 0x10) == 16) //褐变
                    SystemStructBrown = true;
                else
                    SystemStructBrown = false;
                if ((globalOut_SysConfig.InternalClassifyType & 0x20) == 32) //糖心
                    SystemStructTangxin = true;
                else
                    SystemStructTangxin = false;
                if ((globalOut_SysConfig.UltrasonicClassifyType & 0x01) == 1)//硬度
                    SystemStructRigidity = true;
                else
                    SystemStructRigidity = false;
                if ((globalOut_SysConfig.UltrasonicClassifyType & 0x02) == 2)//含水率
                    SystemStructWater = true;
                else
                    SystemStructWater = false;

                //for (int i = 0; i < ConstPreDefine.MAX_CHANNEL_NUM; i++)
                //{
                //    globalOut_SysConfig.nChannelInfo[nSubsysIdx * ConstPreDefine.MAX_CHANNEL_NUM + i] = globalIn_defaultInis[nSubsysIdx].sys.nChannelInfo[nSubsysIdx * ConstPreDefine.MAX_CHANNEL_NUM + i];
                //}
                globalOut_SysConfig.nChannelInfo[nSubsysIdx] = globalIn_defaultInis[nSubsysIdx].sys.nChannelInfo[nSubsysIdx]; //Modify by ChengSk - 20190521
                globalOut_SysConfig.nSubsysNum = 0;
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    {
                        //if (globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                        if (globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                        {
                            ++globalOut_SysConfig.nSubsysNum;
                            break;
                        }
                    }
                }

                if (ConnectSystemNum < globalOut_SysConfig.nSubsysNum && ConnectSystemNum != 0)
                {
                    globalOut_SysConfig.nSubsysNum = ConnectSystemNum;
                }
                globalIn_defaultInis[nSubsysIdx].sys.exitstate.CopyTo(globalOut_SysConfig.exitstate, 0);
                globalOut_SysConfig.nExitNum = globalIn_defaultInis[nSubsysIdx].sys.nExitNum;
                globalOut_SysConfig.nImageUV[nSubsysIdx] = globalIn_defaultInis[nSubsysIdx].sys.nImageUV[nSubsysIdx];
                globalOut_SysConfig.nDataRegistration[nSubsysIdx] = globalIn_defaultInis[nSubsysIdx].sys.nDataRegistration[nSubsysIdx];
                globalOut_SysConfig.nImageSugar[nSubsysIdx] = globalIn_defaultInis[nSubsysIdx].sys.nImageSugar[nSubsysIdx];
                globalOut_SysConfig.nImageUltrasonic[nSubsysIdx] = globalIn_defaultInis[nSubsysIdx].sys.nImageUltrasonic[nSubsysIdx];
                globalIn_defaultInis[nSubsysIdx].sys.nCameraDelay.CopyTo(globalOut_SysConfig.nCameraDelay, 0); //Add by ChengSk - 20190624

                if (GlobalDataInterface.ExitList.Count == 0)
                {
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
                }

                //ExitInfo
                for (int i = 0; i < ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    globalOut_ExitInfo[nSubsysIdx * (int)ConstPreDefine.MAX_CHANNEL_NUM + i].ToCopy(globalIn_defaultInis[nSubsysIdx].exit[i]);
                //Paras,globalOut_SpotDetectThresh
                for (int i = 0; i < ConstPreDefine.MAX_IPM_NUM; i++)//ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM 2013.10.12 ivycc
                {
                    globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM+i].ToCopy(globalIn_defaultInis[nSubsysIdx].paras[i]);
                    //globalOut_SpotDetectThresh[i].ToCopy(globalIn_defaultInis[nSubsysIdx].spotdetectthresh[i]);
                }
                //Weight
                for (int i = 0; i < ConstPreDefine.MAX_CHANNEL_NUM; i++)//ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM 2013.10.12 ivycc
                    globalOut_WeightBaseInfo[nSubsysIdx *(int) ConstPreDefine.MAX_CHANNEL_NUM + i].ToCopy(globalIn_defaultInis[nSubsysIdx].weights[i]);
                //globalExit
                globalOut_GlobalExitInfo[nSubsysIdx] = globalIn_defaultInis[nSubsysIdx].gexit;
                //globalWeight
                globalOut_GlobalWeightBaseInfo[nSubsysIdx].ToCopy(globalIn_defaultInis[nSubsysIdx].gweight);

                if (nSubsysIdx == 1)
                    globalOut_GlobalWeightBaseInfo[0].nTotalCupNums[1] = globalOut_GlobalWeightBaseInfo[1].nTotalCupNums[1];
                else
                    globalOut_GlobalWeightBaseInfo[1].nTotalCupNums[0] = globalOut_GlobalWeightBaseInfo[0].nTotalCupNums[0];
                //MotorInfo
                for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM;i++)
                {
                    globalOut_MotorInfo[i].ToCopy(globalIn_defaultInis[nSubsysIdx].motor[i]);
                }
                    //网络状态
                    NetStateSum &= (globalIn_defaultInis[nSubsysIdx].nNetState == 0);
                NetState[nSubsysIdx] = globalIn_defaultInis[nSubsysIdx].nNetState;

                globalOut_AnalogDensity.ToCopy(globalIn_defaultInis[nSubsysIdx].analogdensity);
                //传感系统类型
                //CIR视觉
                if ((globalOut_SysConfig.nClassificationInfo & 0x01) > 0)
                {
                    GlobalDataInterface.CIRAvailable = true;
                }
                else
                {
                    GlobalDataInterface.CIRAvailable = false;
                }
                //UV视觉
                if ((globalOut_SysConfig.nClassificationInfo & 0x02) > 0)
                {
                    GlobalDataInterface.UVAvailable = true;
                }
                else
                {
                    GlobalDataInterface.UVAvailable = false;
                }
                //重量
                if ((globalOut_SysConfig.nClassificationInfo & 0x04) > 0)
                {
                    GlobalDataInterface.WeightAvailable = true;
                }
                else
                {
                    GlobalDataInterface.WeightAvailable = false;
                }
                //内部品质
                if ((globalOut_SysConfig.nClassificationInfo & 0x08) > 0)
                {
                    GlobalDataInterface.InternalAvailable = true;
                }
                else
                {
                    GlobalDataInterface.InternalAvailable = false;
                }
                //超声波
                if ((globalOut_SysConfig.nClassificationInfo & 0x10) > 0)
                {
                    GlobalDataInterface.UltrasonicAvailable = true;
                }
                else
                {
                    GlobalDataInterface.UltrasonicAvailable = false;
                }
                ////品质等级
                //int cnt = 0;
                //for (int i = 0; i < ConstPreDefine.MAX_QUALITY_GRADE_NUM; i++)
                //{
                //    if (globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM].nFruitNum == 0x7f7f7f7f)
                //        break;
                //    cnt++;
                //}
                //GradeQualityNum = cnt;

                ////尺寸等级
                //cnt = 0;
                //for (int i = 0; i < ConstPreDefine.MAX_SIZE_GRADE_NUM; i++)
                //{
                //    if (globalOut_GradeInfo.grades[i].nFruitNum == 0x7f7f7f7f)
                //        break;
                //    cnt++;
                //}
                //GradeSizeNum = cnt;

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
                        GlobalDataInterface.Quality_GradeInfo.Item[i].sbBruise = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbBruise;
                        GlobalDataInterface.Quality_GradeInfo.Item[i].sbRot = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbRot;
                        GlobalDataInterface.Quality_GradeInfo.Item[i].sbSugar = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbSugar;
                        GlobalDataInterface.Quality_GradeInfo.Item[i].sbAcidity = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbAcidity;
                        GlobalDataInterface.Quality_GradeInfo.Item[i].sbHollow = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbHollow;
                        GlobalDataInterface.Quality_GradeInfo.Item[i].sbSkin = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbSkin;
                        GlobalDataInterface.Quality_GradeInfo.Item[i].sbBrown = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbBrown;
                        GlobalDataInterface.Quality_GradeInfo.Item[i].sbTangxin = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbTangxin;
                        GlobalDataInterface.Quality_GradeInfo.Item[i].sbRigidity = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbRigidity;
                        GlobalDataInterface.Quality_GradeInfo.Item[i].sbWater = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbWater;        
                    }
                }


                //cnt = 0;
                //for (int i = 0; i < ConstPreDefine.MAX_COLOR_INTERVAL_NUM; i++)
                //{
                //    if (globalOut_GradeInfo.intervals[i].nMaxU == 0x7f)
                //        break;
                //    cnt++;
                //}
                //ColorIntervalNum = cnt;

                //颜色等级数量
                //int cnt = 0;    //Note by ChengSk - 20190110
                //for (int i = 0; i < ConstPreDefine.MAX_COLOR_GRADE_NUM; i++)
                //{
                //    if ((globalOut_GradeInfo.ColorType & 0x08) > 0)//百分比
                //    {
                //        if (globalOut_GradeInfo.percent[i * ConstPreDefine.MAX_COLOR_INTERVAL_NUM].nMax == 0x7f)
                //            break;
                //        cnt++;
                //    }
                //    else
                //    {
                //        cnt = 3;
                //    }
                //}
                //ColorGradeNum = cnt;

                if ((globalOut_GradeInfo.ColorType & 0x08) > 0)//百分比
                {
                    for (int i = 0; i < ConstPreDefine.MAX_COLOR_GRADE_NUM; i++)
                    {
                        byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        Array.Copy(globalOut_GradeInfo.strColorGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        string colorName = Encoding.Default.GetString(temp).TrimEnd('\0');
                        if (colorName == "")
                        {
                            ColorGradeNum = i;
                            break;
                        }
                    }
                }
                else
                {
                    ColorGradeNum = 3;
                } //Modify by ChengSk - 20190110  FSM上传颜色数量以名称解析

                //形状等级数量
                for (int i = 0; i < ConstPreDefine.MAX_SHAPE_GRADE_NUM; i++)
                {
                    if (globalOut_GradeInfo.fShapeFactor[i] == 0.000000)
                    {
                        ShapeGradeNum = i + 1;
                        break;
                    }
                }

                //瑕疵等级数量
                for (int i = 0; i < ConstPreDefine.MAX_FlAWAREA_GRADE_NUM; i++)
                {
                    if (globalOut_GradeInfo.unFlawAreaFactor[i * 2] == 0 && globalOut_GradeInfo.unFlawAreaFactor[i * 2 + 1] == 0)
                    {
                        FlawGradeNum = i + 1;
                        break;
                    }
                }

                //密度等级数量
                for (int i = 0; i < ConstPreDefine.MAX_DENSITY_GRADE_NUM; i++)
                {
                    if (globalOut_GradeInfo.fDensityFactor[i] == 0.000000)
                    {
                        DensityGradeNum = i + 1;
                        break;
                    }
                }

                //擦伤等级数量
                for(int i=0;i<ConstPreDefine.MAX_BRUISE_GRADE_NUM;i++)
                {
                    if (globalOut_GradeInfo.unBruiseFactor[i * 2] == 0.000000 && globalOut_GradeInfo.unBruiseFactor[i * 2 + 1] == 0.000000)
                    {
                        BruiseGradeNum = i + 1;
                        break;
                    }
                }
                    
                //腐烂等级数量
                for (int i = 0; i < ConstPreDefine.MAX_ROT_GRADE_NUM; i++)
                {
                    if (globalOut_GradeInfo.unRotFactor[i * 2] == 0.000000 && globalOut_GradeInfo.unRotFactor[i * 2 + 1] == 0.000000)
                    {
                        RotGradeNum = i + 1;
                        break;
                    }
                }

                //糖度等级数量
                for (int i = 0; i < ConstPreDefine.MAX_SUGAR_GRADE_NUM; i++)
                {
                    if (globalOut_GradeInfo.fSugarFactor[i] == 0.000000)
                    {
                        SugarGradeNum = i + 1;
                        break;
                    }
                }

                //酸度等级数量
                for (int i = 0; i < ConstPreDefine.MAX_ACIDITY_GRADE_NUM; i++)
                {
                    if (globalOut_GradeInfo.fAcidityFactor[i] == 0.000000)
                    {
                        AcidityGradeNum = i + 1;
                        break;
                    }
                }

                //空心等级数量
                for (int i = 0; i < ConstPreDefine.MAX_HOLLOW_GRADE_NUM; i++)
                {
                    if (globalOut_GradeInfo.fHollowFactor[i] == 0.000000)
                    {
                        HollowGradeNum = i + 1;
                        break;
                    }
                }

                //浮皮等级数量
                for (int i = 0; i < ConstPreDefine.MAX_SKIN_GRADE_NUM; i++)
                {
                    if (globalOut_GradeInfo.fSkinFactor[i] == 0.000000)
                    {
                        SkinGradeNum = i + 1;
                        break;
                    }
                }

                //褐变等级数量
                for (int i = 0; i < ConstPreDefine.MAX_BROWN_GRADE_NUM; i++)
                {
                    if (globalOut_GradeInfo.fBrownFactor[i] == 0.000000)
                    {
                        BrownGradeNum = i + 1;
                        break;
                    }
                }

                //糖心等级数量
                for (int i = 0; i < ConstPreDefine.MAX_TANGXIN_GRADE_NUM; i++)
                {
                    if (globalOut_GradeInfo.fTangxinFactor[i] == 0.000000)
                    {
                        TangxinGradeNum = i + 1;
                        break;
                    }
                }

                //硬度等级数量
                for (int i = 0; i < ConstPreDefine.MAX_RIGIDITY_GRADE_NUM; i++)
                {
                    if (globalOut_GradeInfo.fRigidityFactor[i] == 0.000000)
                    {
                        RigidityGradeNum = i + 1;
                        break;
                    }
                }

                //含水率等级数量
                for (int i = 0; i < ConstPreDefine.MAX_WATER_GRADE_NUM; i++)
                {
                    if (globalOut_GradeInfo.fWaterFactor[i] == 0.000000)
                    {
                        WaterGradeNum = i + 1;
                        break;
                    }
                }    
            }
            catch (Exception e)
            {
                Trace.WriteLine("GlobalDataInterface中函数RefreshDefaultIni出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数RefreshDefaultIni出错" + e);
#endif
            }    
            
        }

        /// <summary>
        /// 命令/数据发送函数(但目标)
        /// </summary>
        /// <param name="nDestId">发送目标ID</param>
        /// <param name="nCmd">发送命令</param>
        /// <returns></returns>
        public static int TransmitParam(int nDestId, int nCmd, object obj)
        {
            int rc = -1;
            try
            {
                byte[] data = null;
                int nlength = 0;
                bool IsCmd = false;
                int nSubsysIdx = -1;
                int nIpmIdx = -1;
                int nChannelIdx = -1;

                switch (nCmd)
                {
                    //FSM-CMD
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_OFF:
                        bool bSuccess = DisConnect();
                        return (bSuccess ? 0 : rc);  //成功返回0，不成功返回-1  Modify by ChengSk - 20190516
#if DEBUG
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_WAVE_FORM_ON:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_WAVE_FORM_OFF:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_DATA_TRACKING_ON:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_DATA_TRACKING_OFF:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_CLEAR_DATA:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_BACK_LEARN:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_SHUT_DOWN:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHTINFO_ON:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHTINFO_OFF:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_SIMULATEDPULSE_ON:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_SIMULATEDPULSE_OFF:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_PROJ_OPENED:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_PROJ_CLOSED:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_SAVE_PARAS://20200605添加yjj
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHTRESET:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_CUP_ON:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_CUP_OFF:
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_NET://输入sysID，返回-1连接不上，返回0为已连接
                        IsCmd = true;
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_CUPSTATERESET:
                        IsCmd = true;
                        break;
#endif
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_ON:
                        ConnectSystemNum = Connect();
                        return 0;
                    //FSM-PARA
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_SYS_CONFIG:
                        nlength = Marshal.SizeOf(typeof(stSysConfig));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(globalOut_SysConfig);
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO://无需传参数*****
                        nlength = Marshal.SizeOf(typeof(stGradeInfo));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(globalOut_GradeInfo);
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO://无需传参数
                        nSubsysIdx = Commonfunction.GetSubsysIndex(nDestId);
                        nIpmIdx = Commonfunction.GetIPMIndex(nDestId);
                        nChannelIdx = Commonfunction.GetChannelIndex(nDestId);
                        nlength = Marshal.SizeOf(typeof(stExitInfo));
                        data = new byte[nlength];
                        if (GlobalDataInterface.nVer == 0)//Modify by xcw - 20200619
                        {
                            data = Commonfunction.StructToBytes(globalOut_ExitInfo[nSubsysIdx * ConstPreDefine.MAX_CHANNEL_NUM + nChannelIdx]);
                        }
                        else if (GlobalDataInterface.nVer == 1)
                        {
                            data = Commonfunction.StructToBytes(globalOut_ExitInfo[nSubsysIdx * ConstPreDefine.MAX_CHANNEL_NUM + nIpmIdx * ConstPreDefine.CHANNEL_NUM + nChannelIdx]);
                        }
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHT_INFO://无需传参数
                        nSubsysIdx = Commonfunction.GetSubsysIndex(nDestId);
                        nIpmIdx = Commonfunction.GetIPMIndex(nDestId);
                        nChannelIdx = Commonfunction.GetChannelIndex(nDestId);
                        nlength = Marshal.SizeOf(typeof(stWeightBaseInfo));
                        data = new byte[nlength];
                        if (GlobalDataInterface.nVer == 0)//Modify by xcw - 20200619
                        {
                            data = Commonfunction.StructToBytes(globalOut_WeightBaseInfo[nSubsysIdx * ConstPreDefine.MAX_CHANNEL_NUM + nChannelIdx]);
                        }
                        else if (GlobalDataInterface.nVer == 1)
                        {
                            data = Commonfunction.StructToBytes(globalOut_WeightBaseInfo[nSubsysIdx * ConstPreDefine.MAX_CHANNEL_NUM + nIpmIdx * ConstPreDefine.CHANNEL_NUM + nChannelIdx]);
                        }
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_PARAS_INFO://无需传参数
                        nSubsysIdx = Commonfunction.GetSubsysIndex(nDestId);
                        nIpmIdx = Commonfunction.GetIPMIndex(nDestId);
                        nlength = Marshal.SizeOf(typeof(stParas));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(globalOut_Paras[nSubsysIdx * ConstPreDefine.MAX_IPM_NUM + nIpmIdx]);
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE:
                        nlength = Marshal.SizeOf(typeof(stVolveTest));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_ALL_LANE_VOLVE:
                        nlength = Marshal.SizeOf(typeof(stVolveTest));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_RESET_AD:
                        nlength = Marshal.SizeOf(typeof(stResetAD));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_EXIT_INFO://单个子系统发送，无需传参数
                        nSubsysIdx = Commonfunction.GetSubsysIndex(nDestId);
                        nlength = Marshal.SizeOf(typeof(stGlobalExitInfo));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(globalOut_GlobalExitInfo[nSubsysIdx]);
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_WEIGHT_INFO://单个子系统发送，无需传参数
                        nSubsysIdx = Commonfunction.GetSubsysIndex(nDestId);
                        nlength = Marshal.SizeOf(typeof(stGlobalWeightBaseInfo));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(globalOut_GlobalWeightBaseInfo[nSubsysIdx]);
                        break;
                    //case (int)HC_FSM_COMMAND_TYPE.HC_CMD_FlAWAREA_INFO:
                    //    nlength = Marshal.SizeOf(typeof(stSpotDetectThresh));
                    //    data = new byte[nlength];
                    //    data = Commonfunction.StructToBytes(obj);
                    //    break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_INFO:
                        nlength = Marshal.SizeOf(typeof(stGlobal));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_BOOT_FLASH_BURN:
                        nlength = ((byte[])obj).Length;
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_MOTOR_INFO:
                        nlength = Marshal.SizeOf(typeof(stMotorInfo));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_COLOR_GRADE_INFO://无需传参数*****2015-05-29添加
                        nlength = Marshal.SizeOf(typeof(stGradeInfo));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(globalOut_GradeInfo);
                        break;
                    case (int)HC_FSM_COMMAND_TYPE.HC_CMD_DENSITY_INFO:
                        nlength = Marshal.SizeOf(typeof(stAnalogDensity));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(globalOut_AnalogDensity);
                        break;
                    //case (int)HC_FSM_COMMAND_TYPE.HC_CMD_USERSET://****
                    //    break;
                    //IPM
                    case (int)HC_IPM_COMMAND_TYPE.HC_CMD_SINGLE_SAMPLE:
                        IsCmd = true;
                        break;
                    case (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_ON:
                        nlength = Marshal.SizeOf(typeof(stContinousCapture));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_OFF:
                        nlength = 2;
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHOW_BLOB_ON:
                        nlength = 2;
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_IPM_COMMAND_TYPE.HC_CMD_AUTOBALANCE_ON:
                        nlength = Marshal.SizeOf(typeof(stWhiteBalanceParam));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_IPM_COMMAND_TYPE.HC_CMD_AUTOBALANCE_ON_CAMERA:
                        nlength = Marshal.SizeOf(typeof(stWhiteBalanceParam));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    //case (int)HC_IPM_COMMAND_TYPE.HC_CMD_LEVELFEATURE_INFO:
                    //    nlength = Marshal.SizeOf(typeof(stYuvThresh));
                    //    data = new byte[nlength];
                    //    data = Commonfunction.StructToBytes(obj);
                    //    break;
                    case (int)HC_IPM_COMMAND_TYPE.HC_CMD_SINGLE_SAMPLE_SPOT:
                        IsCmd = true;
                        break;
                        //nlength = 2;
                        //data = new byte[nlength];
                        //data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_IPM_COMMAND_TYPE.HC_CMD_TAG_BGR:
                        nlength = Marshal.SizeOf(typeof(stBGR));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_IPM_COMMAND_TYPE.HC_CMD_SPOT_DETECT_TEST:
                        nlength = Marshal.SizeOf(typeof(stSpotDetectTest));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(obj);
                        break;
                    case (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHUTTER_ADJUST_ON:  //Add by ChengSk - 20190627
                        IsCmd = true;
                        break;
                    case (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHUTTER_ADJUST_OFF: //Add by ChengSk - 20190627
                        IsCmd = true;
                        break;
                    case (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHUTDOWN:           //Add by ChengSk - 20191112
                        IsCmd = true;
                        break;
                    case (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_GRADE_INFO: //Add by xcw - 20200520
                        nlength = Marshal.SizeOf(typeof(stGradeInfo));
                        data = new byte[nlength];
                        data = Commonfunction.StructToBytes(globalOut_GradeInfo);
                        break;
                    case (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_INSPECTION_OVER:           //Add by xcw - 20200520
                        IsCmd = true;
                        break;
                   case (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_DISPLAY_OUTLETS:
                        //data= System.BitConverter.GetBytes(s);
                        break;
                    default:
                        IsCmd = true;
                        break;
                }
                if (nDestId == -1)
                {
                    if (IsCmd)
                        rc = m_TcpClient.AllSysSyncRequest(nCmd, null, globalOut_SysConfig.nChannelInfo);
                    else
                        rc = m_TcpClient.AllSysSyncRequest(nCmd, data, globalOut_SysConfig.nChannelInfo);
                }
                else
                {
                    if (IsCmd)
                    {
                        if (m_TcpClient.SyncRequest(nDestId, nCmd, null))
                            rc = 0;
                    }
                    else
                    {
                        if (m_TcpClient.SyncRequest(nDestId, nCmd, data))
                            rc = 0;
                    }
                }
                return rc;
            }
            catch (Exception e)
            {
                Trace.WriteLine("GlobalDataInterface中函数TransmitParam出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数TransmitParam出错" + e);
#endif
                return rc;
            }
        }

        /// <summary>
        /// 命令/数据发送函数(但目标)
        /// </summary>
        /// <param name="nDestId">发送目标ID</param>
        /// <param name="nCmd">发送命令</param>
        /// <param name="data">发送个数</param>
        /// <returns></returns>
        public static int TransmitParamData(int nDestId, int nCmd, int num)
        {
            int rc = -1;
            try
            {
                byte[] data = null;
                //int nlength = 0;
                bool IsCmd = false;
                //int nSubsysIdx = -1;
                //int nIpmIdx = -1;
                //int nChannelIdx = -1;

                switch (nCmd)
                {
                    case (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_DISPLAY_OUTLETS:
                        data= System.BitConverter.GetBytes(num);
                        break;
                    default:
                        IsCmd = true;
                        break;
                }
                if (m_TcpClient.SyncRequest(nDestId, nCmd, data))
                {
                    rc = 0;
                }
                return rc;
            }
            catch (Exception e)
            {
                Trace.WriteLine("GlobalDataInterface中函数TransmitParam出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数TransmitParam出错" + e);
#endif
                return rc;
            }
        }


        /// <summary>
        /// 与每个子系统的FSM握手连接
        /// </summary>
        /// <returns>连接子系统个数</returns>
        private static byte Connect()
        {
            byte nConncetedCount = 0;
            int ver = ConstPreDefine.VERSION;
            List<int> arrayID = new List<int>();
            m_bConnecting = true;
            try
            {


                Commonfunction.GetAllSysID(globalOut_SysConfig.nChannelInfo, ref arrayID);
                if (arrayID.Count <= 0)
                {
                    int nDestId = 0;
                    for (int i = ConstPreDefine.MAX_SUBSYS_NUM - 1; i >= 0; i--)// 2013.10.11 fsm测试ConstPreDefine.MAX_SUBSYS_NUM
                    {
                        nDestId = Commonfunction.EncodeSubsys(i);
                        if (m_TcpClient.SyncRequest(nDestId, (int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_ON, null))
                        {
                            Trace.WriteLine(string.Format("连接下位机：{0}成功", i));
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo(string.Format("连接下位机：{0}成功", i));
#endif
                            nConncetedCount++;
                            SubSystemIsConnected[i] = true;
                        }
                        else
                        {
                            Trace.WriteLine(string.Format("连接下位机：{0}失败", i));
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo(string.Format("连接下位机：{0}失败", i));
#endif
                        }
                    }
                }
                else
                {
                    //已经有系统信息了
                    for (int i = 0; i < arrayID.Count; i++)
                    {
                        if (m_TcpClient.SyncRequest(arrayID[i], (int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_ON,null))
                        {
                            Trace.WriteLine(string.Format("连接下位机：{0}成功", i));
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo(string.Format("连接下位机：{0}成功", i));
#endif
                            nConncetedCount++;
                            SubSystemIsConnected[i] = true;
                        }
                        else
                        {
                            Trace.WriteLine(string.Format("连接下位机：{0}失败", i));
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo(string.Format("连接下位机：{0}失败", i));
#endif
                        }
                    }

                }
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    global_IsTestMode |= SubSystemIsConnected[i];
                }
                m_bConnecting = false;
                return nConncetedCount;
            }
            catch (Exception e)
            {
                Trace.WriteLine("GlobalDataInterface中函数Connect出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数Connect出错" + e);
#endif
                return nConncetedCount;
            }

        }

        /// <summary>
        /// 与所有子系统断开连接
        /// </summary>
        /// <returns></returns>
        public static bool DisConnect()
        {
            try
            {
                m_bConnecting = false;
                bool IsConnected = false;
                /*子系统中是否有已连接的*/
                for (int i = 0; i < SubSystemIsConnected.Count(); i++)
                {
                    IsConnected = SubSystemIsConnected[i] || IsConnected;
                }
                if (IsConnected)
                {
                    int nReturn = m_TcpClient.AllSysSyncRequest((int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_OFF, null, globalOut_SysConfig.nChannelInfo);
                    Array.Clear(SubSystemIsConnected, 0, SubSystemIsConnected.Length);
                    return nReturn == 0;
                }
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("GlobalDataInterface中函数DisConnect出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数DisConnect出错" + e);
#endif
                return false;
            }
        }

        /// <summary>
        /// 创建错误信息文件流
        /// </summary>
        public static void CreatErorrFile()
        {
            if (!Directory.Exists(System.Environment.CurrentDirectory + "\\ErrorLog\\"))
                Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\ErrorLog\\");
            string ErorrName = System.Environment.CurrentDirectory + "\\ErrorLog\\" + string.Format("ErrorLog{0}-{1}-{2}.txt",DateTime.Now.Year.ToString(),DateTime.Now.Month.ToString(),DateTime.Now.Day.ToString());
            ErrorInfoStream = new FileStream(ErorrName, FileMode.Append, FileAccess.Write);
            ErrorInfoStream.Seek(0, SeekOrigin.Current);
            string Info = "************************************************************************\r\n" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "软件启动\r\n";
            byte[] Data = Encoding.Default.GetBytes(Info);
            ErrorInfoStream.Write(Data, 0, Data.Length);
        }

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="ErrorInfo"></param>
        public static void WriteErrorInfo(string ErrorInfo)
        {
            if (ErrorInfoStream != null)
            {
                ErrorInfoStream.Seek(0, SeekOrigin.Current);
                ErrorInfo += "\r\n";
                byte[] Data = Encoding.Default.GetBytes(ErrorInfo);
                ErrorInfoStream.Write(Data, 0, Data.Length);
            }
        }

        /// <summary>
        /// 关闭错误信息文件流
        /// </summary>
        /// <param name="ErrorInfo"></param>
        public static void CloseErorrFile()
        {
            if (ErrorInfoStream != null)
            {
                ErrorInfoStream.Seek(0, SeekOrigin.Current);
                string Info = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "软件关闭\r\n" + "************************************************************************\r\n";
                byte[] Data = Encoding.Default.GetBytes(Info);
                ErrorInfoStream.Write(Data, 0, Data.Length);
                ErrorInfoStream.Close();
            }
        }

        /// <summary>
        /// 获取当前广播源数据：系统配置信息
        /// </summary>
        private void GetBroadcastSysConfigInfo()
        {
            try
            {
                broadcast_SysConfigInfo.sysConfig.ToCopy(globalOut_SysConfig);
                if (GlobalDataInterface.selectLanguage == "")
                {
                    broadcast_SysConfigInfo.nLanguage = 0;
                }
                else if(GlobalDataInterface.selectLanguage == "en")
                {
                     broadcast_SysConfigInfo.nLanguage = 1;
                }
                else
                {
                    broadcast_SysConfigInfo.nLanguage = 0;
                }
                //broadcast_SysConfigInfo.nExitNum = DateTime.Now.Second;//test
                string strexitDisplayTypeConfig = "出口显示名称类型";
                broadcast_SysConfigInfo.exitDisplayType = long.Parse(Commonfunction.GetAppSetting(strexitDisplayTypeConfig));
                for (int i = 0; i < broadcast_SysConfigInfo.sysConfig.nExitNum; i++)
                {
                    byte[] tempByte = new byte[ConstPreDefine.MAX_EXIT_DISPALYNAME_LENGTH];
                    Array.Copy(tempByte, 0, broadcast_SysConfigInfo.strDisplayName, i * ConstPreDefine.MAX_EXIT_DISPALYNAME_LENGTH, tempByte.Length);
                    string strDisplayNameConfig = "出口" + (i + 1).ToString() + "显示名称";
                    tempByte = Encoding.Default.GetBytes(Commonfunction.GetAppSetting(strDisplayNameConfig));
                    Array.Copy(tempByte, 0, broadcast_SysConfigInfo.strDisplayName, i * ConstPreDefine.MAX_EXIT_DISPALYNAME_LENGTH, tempByte.Length);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GlobalDataInterface中函数GetBroadcastSysConfigInfo出现错误，错误原因：" + ex.ToString());
            }
        }

        /// <summary>
        /// 获取出口附加信息
        /// </summary>
        private void GetBroadcastExitAdditionalTextInfo()
        {
            try
            {
                //broadcast_ExitAdditionalTextInfo = new stExitAdditionalTextData(broadcast_SysConfigInfo.sysConfig.nExitNum); //根据出口数目初始化“出口附加信息包”
                broadcast_ExitAdditionalTextInfo = new stExitAdditionalTextData(true); //根据出口数目初始化“出口附加信息包”
                for(int i=0; i<broadcast_SysConfigInfo.sysConfig.nExitNum; i++)
                {
                    byte[] tempByte = new byte[ConstPreDefine.MAX_EXIT_ADDITIONALNAME_LENGTH];
                    Array.Copy(tempByte, 0, broadcast_ExitAdditionalTextInfo.Additionaldata, i * ConstPreDefine.MAX_EXIT_ADDITIONALNAME_LENGTH, tempByte.Length);
                    string strAdditionalTextConfig = "出口" + (i + 1).ToString() + "附加信息";
                    tempByte = Encoding.Default.GetBytes(Commonfunction.GetAppSetting(strAdditionalTextConfig));
                    Array.Copy(tempByte, 0, broadcast_ExitAdditionalTextInfo.Additionaldata, i * ConstPreDefine.MAX_EXIT_ADDITIONALNAME_LENGTH, tempByte.Length);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GlobalDataInterface中函数GetBroadcastExitAdditionalTextInfo出现错误，错误原因：" + ex.ToString());
            }
        }

        /// <summary>
        /// 获取当前广播源数据：等级信息
        /// </summary>
        private void GetBroadcastGradeInfo()
        {
            try
            {
                broadcast_GradeInfo.ToCopy(globalOut_GradeInfo);
                //broadcast_GradeInfo.nQualityGradeNum = (byte)DateTime.Now.Second; //test
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GlobalDataInterface中函数GetBroadcastGradeInfo出现错误，错误原因：" + ex.ToString());
            }
        }

        /// <summary>
        /// 获取当前广播源数据：基本统计信息
        /// </summary>
        private void GetBroadcastStatisticsInfo()
        {
            try
            {
                broadcast_StatisticsInfo.Clear();//清零
                //等级累加
                for (int i = 0; i < ConstPreDefine.MAX_QUALITY_GRADE_NUM; i++)
                {
                    for (int j = 0; j < ConstPreDefine.MAX_SIZE_GRADE_NUM; j++)
                    {
                        for (int k = 0; k < ConstPreDefine.MAX_SUBSYS_NUM; k++)
                        {
                            broadcast_StatisticsInfo.statistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] +=
                                globalIn_statistics[k].nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j]; //（重量分选下）个数
                            broadcast_StatisticsInfo.statistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] +=
                                globalIn_statistics[k].nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];//个数/重量
                            broadcast_StatisticsInfo.statistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] +=
                                globalIn_statistics[k].nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];//箱数
                        }

                        broadcast_StatisticsInfo.statistics.nBoxGradeWeight[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] = 0;//（重量分选下）箱重
                        for (int k = 0; k < ConstPreDefine.MAX_SUBSYS_NUM; k++)
                        {
                            if (globalIn_statistics[k].nBoxGradeWeight[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] != 0)
                            {
                                broadcast_StatisticsInfo.statistics.nBoxGradeWeight[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] =
                                    globalIn_statistics[k].nBoxGradeWeight[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                                break;
                            }
                        }
                    }
                }

                //出口累加
                for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM; i++)
                {
                    for (int k = 0; k < ConstPreDefine.MAX_SUBSYS_NUM; k++)
                    {
                        broadcast_StatisticsInfo.statistics.nExitCount[i] += globalIn_statistics[k].nExitCount[i];//出口个数
                        broadcast_StatisticsInfo.statistics.nExitWeightCount[i] += globalIn_statistics[k].nExitWeightCount[i];//（重量分选下）出口重量
                    }
                }

                //子系统累加
                int num = 0; //正在使用的子系统计数
                int nIntervalSumperminute = 0;
                for (int k = 0; k < ConstPreDefine.MAX_SUBSYS_NUM; k++)
                {
                    broadcast_StatisticsInfo.statistics.nTotalCount += globalIn_statistics[k].nTotalCount;//批个数
                    //broadcast_StatisticsInfo.statistics.nTotalCount = (ulong)DateTime.Now.Second; //test
                    broadcast_StatisticsInfo.statistics.nWeightCount += globalIn_statistics[k].nWeightCount;//（重量分选下）批重量
                    if (globalIn_statistics[k].nIntervalSumperminute != 0)
                    {
                        num++;
                        nIntervalSumperminute += globalIn_statistics[k].nIntervalSumperminute;
                    }
                }

                if (num != 0)
                {
                    broadcast_StatisticsInfo.statistics.nIntervalSumperminute = nIntervalSumperminute / num;//分选速度
                }
                else
                {
                    broadcast_StatisticsInfo.statistics.nIntervalSumperminute = 0;
                }
                byte[] tempName = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                if (mainform != null)
                {
                    //开始时间
                    tempName = Encoding.Default.GetBytes(mainform.GetStartTimerChangelabel());
                    tempName.CopyTo(broadcast_StatisticsInfo.strStartTime,0);
                   
                    broadcast_StatisticsInfo.fSeparationEfficiency = mainform.GetSeparationEfficiency();//分选效率
                    broadcast_StatisticsInfo.fRealWeightCount = mainform.GetRealWeightCount();//实时产量

                    //分选程序
                    tempName = Encoding.Default.GetBytes(mainform.GetProgramName());
                    tempName.CopyTo(broadcast_StatisticsInfo.strProgramName, 0);

                }
                tempName = Encoding.Default.GetBytes(Commonfunction.GetAppSetting("贴标机1"));
                Array.Copy(tempName, 0, broadcast_StatisticsInfo.strLabelName, 0, tempName.Length);
                tempName = Encoding.Default.GetBytes(Commonfunction.GetAppSetting("贴标机2"));
                Array.Copy(tempName, 0, broadcast_StatisticsInfo.strLabelName, ConstPreDefine.MAX_TEXT_LENGTH, tempName.Length);
                tempName = Encoding.Default.GetBytes(Commonfunction.GetAppSetting("贴标机3"));
                Array.Copy(tempName, 0, broadcast_StatisticsInfo.strLabelName, 2 * ConstPreDefine.MAX_TEXT_LENGTH, tempName.Length);
                tempName = Encoding.Default.GetBytes(Commonfunction.GetAppSetting("贴标机4"));
                Array.Copy(tempName, 0, broadcast_StatisticsInfo.strLabelName, 3 * ConstPreDefine.MAX_TEXT_LENGTH, tempName.Length);

            }
            catch (Exception ex)
            {
                Trace.WriteLine("GlobalDataInterface中函数GetBroadcastStatisticsInfo出现错误，错误原因：" + ex.ToString());
            }

        }
        private void VersionJudgment(stGlobal global)
        {
            try
            {
                if (global.nVersion == 40201)
                {
                    GlobalDataInterface.nVer = 0;
                    GlobalDataInterface.SelectlogoPathName = "Logo\\MyLogo40201.png";
                    GlobalDataInterface.Version = 40201;
                    GlobalDataInterface.VERSION_SHOW = "5.0_L"; //add by xcw 20201020
                }
                else if(global.nVersion == 30201)
                {
                    GlobalDataInterface.nVer = 1;
                    GlobalDataInterface.SelectlogoPathName = "Logo\\MyLogo30201.png";
                    GlobalDataInterface.Version = 30201;
                    GlobalDataInterface.VERSION_SHOW = "5.0_S"; //add by xcw 20201020
                }
                else
                {
                    
                    GlobalDataInterface.nVer = global.nVersion;
                    MessageBox.Show(string.Format("版本校验不通过！FSM发送版本号：{0}!" , GlobalDataInterface.nVer), "Error", MessageBoxButtons.OK);
                    GlobalDataInterface.WriteErrorInfo(string.Format("版本校验不通过！FSM发送版本号：{0}!", GlobalDataInterface.nVer));
                    Application.Exit();
                }
                
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GlobalDataInterface中函数VersionJudgment出错" + ex + "\n代码定位：" + ex.StackTrace);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数VersionJudgment出错：" + ex + "\n代码定位：" + ex.StackTrace);
#endif
            }
        }

        private void HandleExceptionBatch(stGlobal global)
        {
            try
            {
                
                int nFsmRestart = global.nFsmRestart;
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数HandleExceptionBatch出错：" + nFsmRestart.ToString() + "\n代码定位：");
                if (nFsmRestart == 1)
                    return;
                //GlobalDataInterface.dataBaseConn = "data source=" + Common.Commonfunction.GetAppSetting("数据源") +
                //        ";database=" + Common.Commonfunction.GetAppSetting("数据库") +
                //        ";user=" + Common.Commonfunction.GetAppSetting("用户名") +
                //        ";pwd=" + Common.Commonfunction.GetAppSetting("密码");
                //bool isConnSuccess = false;
                //SqlConnection conn = new SqlConnection(GlobalDataInterface.dataBaseConn);
                //try
                //{
                //    conn.Open();
                //    isConnSuccess = true;
                //    conn.Close();   //Add 20180919
                //    conn.Dispose(); //Add 20180919
                //}
                //catch (Exception ex)
                //{
                //    isConnSuccess = false;
                //}
                //if (GlobalDataInterface.currentDatabase == "null" || !isConnSuccess)
                //{
                //    DatabaseSetForm databaseSetForm = new DatabaseSetForm();
                //    databaseSetForm.ShowDialog();
                //    if (GlobalDataInterface.DatabaseSet)
                //    {
                //        return;
                //    }
                //}
                //GlobalDataInterface.databaseOperation = new FruitSortingVtest1.DB.DataBaseOperation();
                DataSet dst1 = databaseOperation.GetFruitTopCustomerID();
                
                if (dst1.Tables[0].Rows.Count == 0)
                    return;

                DataSet dst2 = databaseOperation.GetFruitByCustomerID(int.Parse(dst1.Tables[0].Rows[0]["CustomerID"].ToString()));
                string strDatabaseEndTime = dst2.Tables[0].Rows[0]["EndTime"].ToString();
                string strExceptionBatchEndTime = Commonfunction.GetAppSetting("可疑批次结束时间");
                if (strDatabaseEndTime == strExceptionBatchEndTime)
                {
                    databaseOperation.DeleteFruitInfo(int.Parse(dst1.Tables[0].Rows[0]["CustomerID"].ToString()));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GlobalDataInterface中函数HandleExceptionBatch出错" + ex + "\n代码定位：" + ex.StackTrace);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数HandleExceptionBatch出错：" + ex + "\n代码定位：" + ex.StackTrace);
#endif
            }
        }

        

        #endregion



        #region 动态显示功能模块 All Code Add By ChengSk 20131108

        public static DataInterface dataInterface = new DataInterface(true);     //Add By ChengSk 20131108
        public static string lastBatchEndTime = "";                              //最新批次结束加工的时间 Add by ChengSk - 20190516
        public static stStatistics formStatisticsInfo = new stStatistics(true);  //Add By ChengSk 20131108
        public static stGradeInfo formGradeInfo = new stGradeInfo(true);         //Add By ChengSk 20131108
        public static Boolean bIsStart = false;                                  //Add By ChengSk 20131108
        public static Boolean bIsNumberOverHundred = false;                      //Add By ChengSk 20131108
        public static Boolean bIsHaveUnCompleted = false;                        //Add By ChengSk 20131122
        public static Boolean bIsShouldUpdateStartTime = false;                  //Add By ChengSk 20131122
        public static DataBaseOperation databaseOperation = new DataBaseOperation();    //创建数据库操作对象

        public void Struct1ToStruct2()
        {
            try
            {
                //dataInterface = new DataInterface(true);//Add By ChengSk 20180205 //Note by ChengSk - 20191128

                Int32 nBoxGradeCountTemp = 0;       //Add By ChengSk 20131108
                UInt64 nGradeCountTemp = 0;         //Add By ChengSk 20131108
                UInt64 nWeightGradeCountTemp = 0;   //Add By ChengSk 20131108
                UInt64 nExitCountTemp = 0;          //Add By ChengSk 20131108
                UInt64 nExitWeightCountTemp = 0;    //Add By ChengSk 20131108
                UInt64 nTotalCountTemp = 0;         //Add By ChengSk 20131108
                //UInt64 nWeightCountTemp = 0;        //Add By ChengSk 20131108
                UInt64 nWeightTotalCountTemp = 0;        //Add By Wcc 20150129

                int tempQualityGradeNum = 1;
                int tempSizeGradeNum = 1;
                if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)
                    tempQualityGradeNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                    tempSizeGradeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;


                for (int i = 0; i < tempQualityGradeNum; i++)
                {
                    for (int j = 0; j < tempSizeGradeNum; j++)
                    {
                        for (int k = 0; k < globalOut_SysConfig.nSubsysNum; k++)
                        {
                            //各等级箱数
                            nBoxGradeCountTemp += globalIn_statistics[k].nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                            formStatisticsInfo.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] = nBoxGradeCountTemp;
                            //各等级水果个数
                            nGradeCountTemp += globalIn_statistics[k].nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                            formStatisticsInfo.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] = nGradeCountTemp;
                            //各等级水果重量
                            nWeightGradeCountTemp += globalIn_statistics[k].nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                            formStatisticsInfo.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] = nWeightGradeCountTemp;
                        }

                        nWeightTotalCountTemp += formStatisticsInfo.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];//2015-1-16 数据库批重量改为所有等级重量之和

                        nBoxGradeCountTemp = 0;
                        nGradeCountTemp = 0;
                        nWeightGradeCountTemp = 0;
                    }
                }
                formStatisticsInfo.nWeightCount = nWeightTotalCountTemp;//2015-1-16 数据库批重量改为所有等级重量之和

                for (int i = 0; i < globalOut_SysConfig.nExitNum; i++)
                {
                    for (int k = 0; k < globalOut_SysConfig.nSubsysNum; k++)
                    {
                        //各出口水果个数
                        nExitCountTemp += globalIn_statistics[k].nExitCount[i];
                        formStatisticsInfo.nExitCount[i] = nExitCountTemp;
                        //各出口水果重量
                        nExitWeightCountTemp += globalIn_statistics[k].nExitWeightCount[i];
                        formStatisticsInfo.nExitWeightCount[i] = nExitWeightCountTemp;
                    }
                    nExitCountTemp = 0;
                    nExitWeightCountTemp = 0;
                }

                for (int k = 0; k < globalOut_SysConfig.nSubsysNum; k++)
                {
                    //水果批个数
                    nTotalCountTemp += globalIn_statistics[k].nTotalCount;
                    formStatisticsInfo.nTotalCount = nTotalCountTemp;

                    //2015-1-16 数据库批重量改为所有等级重量之和
                    ////水果批重量
                    //nWeightCountTemp += globalIn_statistics[k].nWeightCount;
                    //formStatisticsInfo.nWeightCount = nWeightCountTemp;
                }

                //更新stGradeInfo
                formGradeInfo = globalOut_GradeInfo;

                //更新结构
                dataInterface.BSourceDB = false;                                              //数据结构是否来源于数据库（true），界面（false）
                //Update By ChengSk 20131119
                //dataInterface.CustomerName = "绿萌";                                          //界面获取 
                //dataInterface.FarmName = "绿萌";                                              //界面获取
                //dataInterface.FruitName = "猕猴桃";                                           //界面获取
                //dataInterface.CustomerID = 0;                                                   //默认设置

                //判断数据库中的最新数据是否加工已完成
                if (!bIsHaveUnCompleted)
                {
                    //DataSet getTopDst = BusinessFacade.GetFruitTopCustomerID();
                    DataSet getTopDst = databaseOperation.GetFruitTopCustomerID();
                    if (getTopDst.Tables[0].Rows.Count == 0)
                    {
                        bIsStart = false;
                        bIsHaveUnCompleted = true;
                    }
                    else
                    {
                        int newCustomerID = Convert.ToInt32(getTopDst.Tables[0].Rows[0]["CustomerID"].ToString());
                        //DataSet dst = BusinessFacade.GetFruitByCustomerID(newCustomerID);
                        DataSet dst = databaseOperation.GetFruitByCustomerID(newCustomerID);
                        if (dst.Tables[0].Rows[0]["CompletedState"].ToString().Equals("0"))  //有加工未完成数据，使用原有数据，不用再插入新数据
                        {
                            bIsStart = true;
                        }
                        bIsHaveUnCompleted = true;
                    }
                }

                //if (!bIsStart && dataInterface.IoStStatistics.nTotalCount >= 100)
                if (!bIsStart && formStatisticsInfo.nTotalCount >= 100)  //Modify by ChengSk - 20180307
                {
                    dataInterface.CustomerID = 0;                                             //开始分选时为0，之后由数据库中获取
                    dataInterface.StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");   //开始分选时，系统获取
                    
                    try //添加try-catch Modify by ChengSk - 20191015
                    {
                        #region 获取客户信息
                        string clientInfoFileName = System.AppDomain.CurrentDomain.BaseDirectory + "config\\" + "ClientInfo.txt";
                        string clientInfoContent = FileOperate.ReadFile(1, clientInfoFileName);
                        if (clientInfoContent == null || clientInfoContent == "")
                        {
                            dataInterface.CustomerName = "";
                            dataInterface.FarmName = "";
                            dataInterface.FruitName = "";
                        }
                        else
                        {
                            string[] clientInfoContentItem = clientInfoContent.Split('，');
                            dataInterface.CustomerName = clientInfoContentItem[0].Trim();
                            dataInterface.FarmName = clientInfoContentItem[1].Trim();
                            dataInterface.FruitName = clientInfoContentItem[2].Trim();
                        }
                        #endregion
                    }
                    catch (Exception ee)
                    {
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中操作ClientInfo.txt文件Error，错误原因：" + ee);
#endif
                    }

                    #region 开始分选时插入一条水果信息
                    //bool bInsert = BusinessFacade.InsertFruitInfo(dataInterface.CustomerName.Trim(), dataInterface.FarmName.Trim(), dataInterface.FruitName.Trim(),
                    //    dataInterface.StartTime, "0", "0");
                    bool bInsert = databaseOperation.InsertFruitInfo(dataInterface.CustomerName.Trim(), dataInterface.FarmName.Trim(), dataInterface.FruitName.Trim(),
                        dataInterface.StartTime, "0", "0");
                    if (!bInsert)
                    {
                        //MessageBox.Show("插入水果信息失败！");
                        //MessageBox.Show("0x10003104 Failed to insert fruit information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        MessageBox.Show("0x10003104 " + LanguageContainer.GlobalDataInterfaceMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.GlobalDataInterfaceMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        bIsStart = true;
                    }
                    #endregion
                }

                dataInterface.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");         //每当更新数据时，系统获取                 
                //if ((globalOut_GradeInfo.nClassifyType & 1) == 1) //有品质
                if (globalOut_GradeInfo.nQualityGradeNum > 0) //有品质
                {
                    dataInterface.QualityGradeSum = globalOut_GradeInfo.nQualityGradeNum;     //界面获取
                }
                else
                {
                    dataInterface.QualityGradeSum = 0;                                        //界面获取
                }
                dataInterface.WeightOrSizeGradeSum = globalOut_GradeInfo.nSizeGradeNum;       //界面获取
                dataInterface.ExportSum = globalOut_SysConfig.nExitNum;                       //界面获取   

                dataInterface.IoStStatistics = formStatisticsInfo;                            //界面获取
                dataInterface.IoStStGradeInfo = formGradeInfo;                                //界面获取

                if (!bIsNumberOverHundred && dataInterface.IoStStatistics.nTotalCount >= 100)
                {
                    #region 获取刚插入水果信息的客户编号
                    //int customerID = Convert.ToInt32(BusinessFacade.GetFruitTopCustomerID().Tables[0].Rows[0]["CustomerID"].ToString());
                    int customerID = Convert.ToInt32(databaseOperation.GetFruitTopCustomerID().Tables[0].Rows[0]["CustomerID"].ToString());
                    dataInterface.CustomerID = customerID;                                        //开始分选时为0，之后由数据库中获取
                    #endregion
                    dataInterface.StartedState = "1";                                         //开始分选时为0，当分选水果个数超过100时更改为1
                    dataInterface.CompletedState = "0";                                       //分选过程中为0，分选结束时更改为1（界面过程可始终保持为0）
                    #region 更新水果信息的开始状态
                    if (!bIsNumberOverHundred)
                    {
                        //bool bUpdateFruitStartedState = BusinessFacade.UpdateFruitStartedState(dataInterface.CustomerID, dataInterface.StartedState);
                        bool bUpdateFruitStartedState = databaseOperation.UpdateFruitStartedState(dataInterface.CustomerID, dataInterface.StartedState);
                        if (!bUpdateFruitStartedState)
                        {
                            //MessageBox.Show("更新水果信息开始状态失败！");
                            //MessageBox.Show("0x10003105 Failed to update the start status of fruit information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            MessageBox.Show("0x10003105 " + LanguageContainer.GlobalDataInterfaceMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.GlobalDataInterfaceMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            bIsNumberOverHundred = true;
                        }
                    }
                    #endregion
                }

                if (bIsNumberOverHundred)
                {
                    #region 获取刚插入水果信息的客户编号
                    //int customerID = Convert.ToInt32(BusinessFacade.GetFruitTopCustomerID().Tables[0].Rows[0]["CustomerID"].ToString());
                    int customerID = Convert.ToInt32(databaseOperation.GetFruitTopCustomerID().Tables[0].Rows[0]["CustomerID"].ToString());
                    dataInterface.CustomerID = customerID;                                        //开始分选时为0，之后由数据库中获取
                    #endregion
                    dataInterface.StartedState = "1";
                    dataInterface.CompletedState = "0";
                }
                else
                {
                    dataInterface.StartedState = "0";                                         //开始分选时为0，当分选水果个数超过100时更改为1
                    dataInterface.CompletedState = "0";                                       //分选过程中为0，分选结束时更改为1（界面过程可始终保持为0）
                }

                //判断是否需要更改时间
                if (dataInterface.IoStStatistics.nTotalCount < 100)
                {
                    //DataSet getTopDst = BusinessFacade.GetFruitTopCustomerID();
                    DataSet getTopDst = databaseOperation.GetFruitTopCustomerID();
                    if (getTopDst.Tables[0].Rows.Count == 0)
                    {

                    }
                    else
                    {
                        int newCustomerID = Convert.ToInt32(getTopDst.Tables[0].Rows[0]["CustomerID"].ToString());
                        //DataSet dst = BusinessFacade.GetFruitByCustomerID(newCustomerID);
                        DataSet dst = databaseOperation.GetFruitByCustomerID(newCustomerID);
                        if (dst.Tables[0].Rows[0]["CompletedState"].ToString().Equals("0"))
                        {
                            bIsShouldUpdateStartTime = true;
                        }
                    }
                }
                else
                {
                    if (bIsShouldUpdateStartTime)
                    {
                        dataInterface.StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //bool bUpdateFruitStartTime = BusinessFacade.UpdateFruitStartTime(dataInterface.CustomerID, dataInterface.StartTime);
                        bool bUpdateFruitStartTime = databaseOperation.UpdateFruitStartTime(dataInterface.CustomerID, dataInterface.StartTime);
                        if (!bUpdateFruitStartTime)
                        {
                            //MessageBox.Show("更新水果信息开始时间失败！");
                            //MessageBox.Show("0x10003105 Failed to update the start status of fruit information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            MessageBox.Show("0x10003105 " + LanguageContainer.GlobalDataInterfaceMessagebox3Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.GlobalDataInterfaceMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            bIsShouldUpdateStartTime = false;
                        }
                    }
                }

                #region 获取颜色等级名称
                string strTemp = "";
                string strTempMix = "";
                for (int i = 0; i < ColorGradeNum; i++)
                {
                    strTemp = (Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strColorGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH));
                    //GlobalDataInterface.WriteErrorInfo("@@@@@ 颜色名称：[" + strTemp + "]");
                    //strTemp = strTemp.Substring(0, strTemp.IndexOf("\0"));
                    strTemp = strTemp.TrimEnd('\0'); //Modify by ChengSk - 20190118
                    if (!strTemp.Equals(""))
                    {
                        strTemp = strTemp + "，";
                    }
                    strTempMix += strTemp;
                    strTemp = "";
                }
                if (strTempMix.Length == 0)
                {
                    dataInterface.ColorGradeName = "";
                }
                else
                {
                    dataInterface.ColorGradeName = strTempMix.Substring(0, strTempMix.Length - 1);
                }
                #endregion

                #region 获取形状等级名称
                strTempMix = "";
                for (int i = 0; i < ShapeGradeNum; i++)
                {
                    strTemp = "";
                    strTemp = (Encoding.Default.GetString(dataInterface.IoStStGradeInfo.strShapeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH));
                    //GlobalDataInterface.WriteErrorInfo("@@@@@ 形状名称：[" + strTemp + "]");
                    //strTemp = strTemp.Substring(0, strTemp.IndexOf("\0"));
                    strTemp = strTemp.TrimEnd('\0'); //Modify by ChengSk - 20190118
                    if (!strTemp.Equals(""))
                    {
                        strTemp = strTemp + "，";
                    }
                    strTempMix += strTemp;
                    strTemp = "";
                }
                if (strTempMix.Length == 0)
                {
                    dataInterface.ShapeGradeName = "";
                }
                else
                {
                    dataInterface.ShapeGradeName = strTempMix.Substring(0, strTempMix.Length - 1);
                }
                #endregion

                #region 获取瑕疵等级名称
                strTemp = "";
                strTempMix = "";
                for (int i = 0; i < FlawGradeNum; i++)
                {
                    strTemp = (Encoding.Default.GetString(dataInterface.IoStStGradeInfo.stFlawareaGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH));
                    //GlobalDataInterface.WriteErrorInfo("@@@@@ 瑕疵名称：[" + strTemp + "]");
                    //strTemp = strTemp.Substring(0, strTemp.IndexOf("\0"));
                    strTemp = strTemp.TrimEnd('\0'); //Modify by ChengSk - 20190118
                    if (!strTemp.Equals(""))
                    {
                        strTemp = strTemp + "，";
                    }
                    strTempMix += strTemp;
                    strTemp = "";
                }
                if (strTempMix.Length == 0)
                {
                    dataInterface.FlawGradeName = "";
                }
                else
                {
                    dataInterface.FlawGradeName = strTempMix.Substring(0, strTempMix.Length - 1);
                }
                #endregion

                #region 获取硬度等级名称
                strTemp = "";
                strTempMix = "";
                for (int i = 0; i < RigidityGradeNum; i++)
                {
                    strTemp = (Encoding.Default.GetString(dataInterface.IoStStGradeInfo.stRigidityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH));
                    //GlobalDataInterface.WriteErrorInfo("@@@@@ 硬度名称：[" + strTemp + "]");
                    //strTemp = strTemp.Substring(0, strTemp.IndexOf("\0"));
                    strTemp = strTemp.TrimEnd('\0'); //Modify by ChengSk - 20190118
                    if (!strTemp.Equals(""))
                    {
                        strTemp = strTemp + "，";
                    }
                    strTempMix += strTemp;
                    strTemp = "";
                }
                if (strTempMix.Length == 0)
                {
                    dataInterface.HardGradeName = "";
                }
                else
                {
                    dataInterface.HardGradeName = strTempMix.Substring(0, strTempMix.Length - 1);
                }
                #endregion

                #region 获取密度等级信息
                strTemp = "";
                strTempMix = "";
                for (int i = 0; i < DensityGradeNum; i++)
                {
                    strTemp = (Encoding.Default.GetString(dataInterface.IoStStGradeInfo.stDensityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH));
                    //GlobalDataInterface.WriteErrorInfo("@@@@@ 密度名称：[" + strTemp + "]");
                    //strTemp = strTemp.Substring(0, strTemp.IndexOf("\0"));
                    strTemp = strTemp.TrimEnd('\0'); //Modify by ChengSk - 20190118
                    if (!strTemp.Equals(""))
                    {
                        strTemp = strTemp + "，";
                    }
                    strTempMix += strTemp;
                    strTemp = "";
                }
                if (strTempMix.Length == 0)
                {
                    dataInterface.DensityGradeName = "";
                }
                else
                {
                    dataInterface.DensityGradeName = strTempMix.Substring(0, strTempMix.Length - 1);
                }
                #endregion

                #region 获取含糖量等级信息
                strTemp = "";
                strTempMix = "";
                for (int i = 0; i < SugarGradeNum; i++)
                {
                    strTemp = (Encoding.Default.GetString(dataInterface.IoStStGradeInfo.stSugarGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH));
                    //GlobalDataInterface.WriteErrorInfo("@@@@@ 含糖量名称：[" + strTemp + "]");
                    //strTemp = strTemp.Substring(0, strTemp.IndexOf("\0"));
                    strTemp = strTemp.TrimEnd('\0'); //Modify by ChengSk - 20190118
                    if (!strTemp.Equals(""))
                    {
                        strTemp = strTemp + "，";
                    }
                    strTempMix += strTemp;
                    strTemp = "";
                }
                if (strTempMix.Length == 0)
                {
                    dataInterface.SugarDegreeGradeName = "";
                }
                else
                {
                    dataInterface.SugarDegreeGradeName = strTempMix.Substring(0, strTempMix.Length - 1);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GlobalDataInterface中函数Struct1ToStruct2出现错误，错误原因：" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GlobalDataInterface中函数Struct1ToStruct2出现错误，错误原因：" + ex);
#endif
            }
        }

        /// <summary>
        /// 更新数据接口事件
        /// </summary>
        /// <param name="dataInterface">数据接口参数</param>
        public delegate void DataInterfaceEventHandler(DataInterface dataInterface);
        public static event DataInterfaceEventHandler UpdateDataInterfaceEvent;

        #endregion
    }
}
