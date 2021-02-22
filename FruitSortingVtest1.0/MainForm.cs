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
using TransParentListBox;
using System.Threading;
using System.Diagnostics;
using FruitSortingVtest1.DB;
using System.IO;
using System.Resources;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace FruitSortingVtest1._0
{
    public partial class MainForm : Form
    {
        //Add by ChengSk 20131119
        private string currentPath;                                               //获取当前路径
        private string currentDefaultPath;                                        //配置文件的保存路径
        public string clientInfoFileName;                                         //客户信息文件路径名
        public string clientInfoContent;                                          //客户信息内容
        private string[] clientInfoContentItem;                                   //客户子项目内容
        private Boolean bIsHaveCompleted;                                         //是否已完成过加工
        private DataInterface printDataInterface;
        private PrintOperate printOperate;                                        //打印类定义
        private DataBaseOperation databaseOperation = new DataBaseOperation();    //创建数据库操作对象
        private Boolean bIsOnceMinimumSized = false;                              //是否已最小化一次
        private int currentExitVerticalScroll = 0;                                //当前出口滚动条位置
        private Boolean EndProcessEnabled = false;                                //结束加工启用标志 Add by ChengSk - 20180305
        private SqlLog log = new SqlLog();                                        //写日志类 Add by ChengSk - 20181214
        public string EndSaveMode = "1";                                          //保存模式（1-清空模式，2-累加模式）
        public Boolean bIsEndProcess = false;                                     //是否结束加工
        public Boolean bIsEndSendData = false;                                     //是否结束加工

        //Add by ChengSk 20131119 End

        //public delegate void UpLoadDataEventHandler();
        //public static event UpLoadDataEventHandler UpLoadDataEvent;     //数据上传事件  Add by ChengSk - 20181206
        //public delegate void DownLoadDataEventHandler();
        //public static event DownLoadDataEventHandler DownLoadDataEvent; //数据下载事件  Add by ChengSk - 20181206

        //出口组合控件(自定义)
        struct ExitControl
        {
            public List<Label> labelList;//标签控件显示数字
            public List<TransParentListBox.TransParentListBox> listBoxList;//显示出口等级
            public List<bool> listBoxEnabledList;//记录出口启用状态
            public ExitControl(bool IsOK)
            {
                labelList = new List<Label>();
                listBoxList = new List<TransParentListBox.TransParentListBox>();
                listBoxEnabledList = new List<bool>();
            }

        };

        //被选择等级信息记录
        struct GradedataGridViewSelectedCell
        {
            public int rowIndex;
            public int colIndex;
        };

        List<GradedataGridViewSelectedCell> m_GradedataGridViewSelectedCellList = new List<GradedataGridViewSelectedCell>();
        private ExitControl m_ExitControl = new ExitControl(true);//出口布局中的出口控件列表
        public stGlobal tempGlobal = new stGlobal(true);
        public stStatistics tempStatistics = new stStatistics(true);

        //int nLengtha = Marshal.SizeOf(typeof(stGlobal));
        //int nLengthb = Marshal.SizeOf(typeof(stStatistics));
        int nLength1= Marshal.SizeOf(typeof(stSysConfig)); //test
        //int nLength2= Marshal.SizeOf(typeof(stGradeInfo));
        //int nLength3= Marshal.SizeOf(typeof(stGlobalExitInfo));
        //int nLength4= Marshal.SizeOf(typeof(stGlobalWeightBaseInfo));
        //int nLength5= Marshal.SizeOf(typeof(stAnalogDensity));
        //int nLength6= Marshal.SizeOf(typeof(stExitInfo));
        //int nLength7= Marshal.SizeOf(typeof(stParas));
        //int nLength8= Marshal.SizeOf(typeof(stWeightBaseInfo));
        // int nLength9 = Marshal.SizeOf(typeof(stMotorInfo));
        //nLength =1 Marshal.SizeOf(typeof(stGlobal));
        private bool[] m_bExitEnable = new bool[48];//出口启用标志
        public bool m_IsGradedataGridViewMouseDown = false;
        public bool m_IsExitlistBoxMouseDown = false;
        int m_CurrentMouseDownlistBoxIndex = -1;
        ulong m_preTotalCupNum, m_preTotalCount;
        float m_preWeightCount = 0.0f;
        bool m_GradeSizelistViewExRightMouseDown = false;//主界面水果等级实时参数列表右击
        int m_GradeSizelistViewExRightMouseDownItemIndex = -1;//主界面水果等级实时参数列表右击事件点击的项目序号
        int m_GradeSizelistViewExRightMouseDownSubItemIndex = -1;//主界面水果等级实时参数列表右击事件点击的子项目序号 
        bool m_MainFormIsInitial = false;
        //bool m_IsGradedataGridViewMouseDownDone = false; //主界面等级表格MouseDown事件是否执行完
        bool m_ClearZero = false;
        private ResourceManager m_resourceManager = new ResourceManager(typeof(MainForm));//创建Mainform资源管理
        //Size LvMoonLogoOldSize = new Size();//绿萌图标原始大小
        Size MainFormOldSize = new Size();//主界面原始大小
        //Size ClientInfoLabelOldSize = new Size();//主界面原始大小
        AutoSizeFormClass asc = new AutoSizeFormClass();//声明大小自适应类实例  
        int StatisticInfotableLayoutPanelOldWidth = 0;//原始统计信息显示控件宽
        public int staticCount = 0;//统计数据刷新 分批显示 Modify by ChengSk - 20171220
        private Dictionary<int, Queue<string>> m_ExitSortingStatisticDic = new Dictionary<int, Queue<string>>(); //出口分选量统计(出口索引号-分选量<1分钟之内>) Add by ChengSk - 20180122
        private Queue<string> m_AllSortingStatisticQueue = new Queue<string>();  //总的分选量<1分钟之内>统计 Add by ChengSk - 20180122
        private bool RunningTimeInfoStopTimeIsEmptyFlags = false;//运行时间信息表(停止时间)是否为空标志 false-没有为空的数据 true-有为空的数据

        public MainForm()
        {

            InitializeComponent();

            if (GlobalDataInterface.usedSeparationLogFlags)
            {
                CheckRunningTimeInfoDataTable(); //检测分选运行时间表，如果数据表中有停止时间为空的情况，则将停止时间赋值开始时间
            }

            Thread m_UpStaticsThread = new Thread(UpStatisticInfoThread);//显示数据刷新线程
            m_UpStaticsThread.Priority = ThreadPriority.Normal;
            m_UpStaticsThread.IsBackground = true;
            m_UpStaticsThread.Start();

            //GlobalDataInterface.UpStatisticInfoEvent += new GlobalDataInterface.StatisticInfoEventHandler(OnUpStatisticInfoEvent);
            //Add By ChengSk 20131108
            GlobalDataInterface.UpdateDataInterfaceEvent += new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent);
            GlobalDataInterface.UpAutoEndProcessEvent += new GlobalDataInterface.AutoEndProcessEventHandler(OnAutoEndProcess);   //Add by ChengSk - 20190513
            GlobalDataInterface.UpStatusModifyEvent += new GlobalDataInterface.StatusModify(OnUpStatusModify);   //Add by xcw - 20200527
            GlobalDataInterface.UpStopCheckSampleEvent += new GlobalDataInterface.StopCheckSample(OnUpStopCheck);   //Add by xcw - 20200527
            
            GlobalDataInterface.UpSetTextCallbackEvent += new GlobalDataInterface.SetTextCallback(SetTextCallback);   //Add by xcw - 20200527

            // this.StatisticInfotableLayoutPanel.Location = new Point(3, this.splitContainer2.Panel1.Height / 2 - StatisticInfotableLayoutPanel.Height / 2);
            //this.StatisticInfotableLayoutPanel.Size = new System.Drawing.Size(this.Width, this.StatisticInfotableLayoutPanel.Height);
            ////获取原始绿萌图标大小
            //LvMoonLogoOldSize.Width = this.LvMoonLogotoolStripButton.Width;
            //LvMoonLogoOldSize.Height = this.LvMoonLogotoolStripButton.Height;
            //获取原始界面大小
            MainFormOldSize.Width = this.Width;
            MainFormOldSize.Height = this.Height;
            StatisticInfotableLayoutPanelOldWidth = this.StatisticInfotableLayoutPanel.Width;
            ////获取原始客户信息界面大小
            //ClientInfoLabelOldSize.Width = this.ClientInfoLabel.Width;
            //ClientInfoLabelOldSize.Height = this.ClientInfoLabel.Height;

            //asc.controllInitializeSize(this);
            EndProcessEnabled = EndProcesstoolStripButton.Enabled;  //Add by ChengSk - 20180305
        }


        /// <summary>
        /// 主界面初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                //工程设置显示
                string str;

                if (GlobalDataInterface.nVer == 0)            //版本号判断 add by xcw 20200604
                {
                    this.LvMoonLogotoolStripButton.BackgroundImage = global::FruitSortingVtest1.Properties.Resources.MyLogo40201;
                    //GlobalDataInterface.mainform.Text = "FruitSorting4.2.1";

                }
                else if (GlobalDataInterface.nVer == 1)
                {
                    this.LvMoonLogotoolStripButton.BackgroundImage = global::FruitSortingVtest1.Properties.Resources.MyLogo30201;
                    //GlobalDataInterface.mainform.Text = "FruitSorting3.2.1";
                }

                //不写本地啦   Modify by ChengSk - 20190708
                //str = Commonfunction.GetAppSetting("颜色");
                //GlobalDataInterface.SystemStructColor = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("形状");
                //GlobalDataInterface.SystemStructShape = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("瑕疵");
                //GlobalDataInterface.SystemStructFlaw = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("体积");
                //GlobalDataInterface.SystemStructVolume = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("投影面积");
                //GlobalDataInterface.SystemStructProjectedArea = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("擦伤");
                //GlobalDataInterface.SystemStructBruise = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("腐烂");
                //GlobalDataInterface.SystemStructRot = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("密度");
                //GlobalDataInterface.SystemStructDensity = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("含糖量");
                //GlobalDataInterface.SystemStructSugar = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("酸度");
                //GlobalDataInterface.SystemStructAcidity = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("空心");
                //GlobalDataInterface.SystemStructHollow = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("浮皮");
                //GlobalDataInterface.SystemStructSkin = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("褐变");
                //GlobalDataInterface.SystemStructBrown = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("糖心");
                //GlobalDataInterface.SystemStructTangxin = Convert.ToBoolean(str);  
                //str = Commonfunction.GetAppSetting("硬度");
                //GlobalDataInterface.SystemStructRigidity = Convert.ToBoolean(str);
                //str = Commonfunction.GetAppSetting("含水率");
                //GlobalDataInterface.SystemStructWater = Convert.ToBoolean(str);
                int nLength = Marshal.SizeOf(typeof(stParas));
                int nLength1 = Marshal.SizeOf(typeof(stCameraParas));
                int nLength2 = Marshal.SizeOf(typeof(stIRCameraParas));
                str = Commonfunction.GetAppSetting("拆分器距离");
                GlobalDataInterface.SplitterDistance2 = Convert.ToInt32(str);
                str = Commonfunction.GetAppSetting("拆分器距离2");
                GlobalDataInterface.SplitterDistance3 = Convert.ToInt32(str);
                str = Commonfunction.GetAppSetting("出口垂直滚动条");
                GlobalDataInterface.ExitVerticalScroll = Convert.ToInt32(str);
                currentExitVerticalScroll = GlobalDataInterface.ExitVerticalScroll; //当前滚动条位置
                //str = Commonfunction.GetAppSetting("Wifi功能");
                //Modify by ChengSk - 20190828
                GlobalDataInterface.sendBroadcastPackage = GlobalDataInterface.globalOut_SysConfig.IfWIFIEnable == 0x01 ? true : false;

                //GlobalDataInterface.nSampleOutlet = int.Parse(Commonfunction.GetAppSetting("抽检出口")); //Add by ChengSk - 20180124
                GlobalDataInterface.nSampleOutlet = int.Parse(GlobalDataInterface.globalOut_SysConfig.CheckExit.ToString());  //Modify by ChengSk - 20190828
                //GlobalDataInterface.nSampleNumber = int.Parse(Commonfunction.GetAppSetting("抽检数量")); //Add by ChengSk - 20180124
                GlobalDataInterface.nSampleNumber = int.Parse(GlobalDataInterface.globalOut_SysConfig.CheckNum.ToString());   //Modify by ChengSk - 20190828
                GlobalDataInterface.nSampleNumber = int.Parse(GlobalDataInterface.globalOut_GradeInfo.nCheckNum.ToString());//add by xcw - 20200525
                GlobalDataInterface.nIQSEnable = GlobalDataInterface.globalOut_SysConfig.nIQSEnable; //Add by ChengSk - 20191111
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

                this.splitContainer2.SplitterDistance = GlobalDataInterface.SplitterDistance2;    //设置拆分器上边距距离
                this.splitContainer3.SplitterDistance = GlobalDataInterface.SplitterDistance3;    //设置拆分器上边距距离

                this.StatisticInfotableLayoutPanel.Location = new Point(3, this.splitContainer2.Panel1.Height / 2 - StatisticInfotableLayoutPanel.Height / 2);

                this.StatisticInfotableLayoutPanel.Size = new System.Drawing.Size((this.Width > this.StatisticInfotableLayoutPanelOldWidth ? this.Width : this.StatisticInfotableLayoutPanelOldWidth), this.StatisticInfotableLayoutPanel.Height);

                //if (DateTime.Now.Second<10)
                this.TimetoolStripLabel.Text = string.Format("{0:T}", DateTime.Now);
                // else
                //this.TimetoolStripLabel.Text = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString()+ ":" +DateTime.Now.Second.ToString();

                SetMenuMotorEnable();
                /**************************************************************Start*******************************************************************/
                //Add by Chengsk 20131129
                //if (File.Exists("..//..//Logo//MyLogo.bmp")) //生成Logo文件
                //{
                //    if (!Directory.Exists("Logo"))
                //    {
                //        Directory.CreateDirectory("Logo");
                //    }
                //    if (!File.Exists("Logo//MyLogo.bmp"))
                //    {
                //        File.Create("Logo//MyLogo.bmp").Close();
                //        FileStream ofs = File.OpenRead("..//..//Logo//MyLogo.bmp");
                //        FileStream ifs = File.OpenWrite("Logo//MyLogo.bmp");
                //        byte[] b = new byte[1024];
                //        int k = 0;
                //        while ((k = ofs.Read(b, 0, b.Length)) > 0)
                //        {
                //            ifs.Write(b, 0, k);
                //        }
                //        ofs.Close();
                //        ifs.Close();
                //    }                  
                //}

                Image logoImage = null;
                try
                {
                    //Image logoImage = Image.FromFile(System.Environment.CurrentDirectory + "//Logo//MyLogo.png");
                    logoImage = Image.FromFile(System.Environment.CurrentDirectory + "//" + GlobalDataInterface.SelectlogoPathName);

                }
                catch(Exception eee) { }

                this.LvMoonLogotoolStripButton.BackgroundImage = logoImage;
                if (File.Exists("..//..//" + GlobalDataInterface.SelectlogoPathName)) //生成Logo文件
                {
                    if (!Directory.Exists("Logo"))
                    {
                        Directory.CreateDirectory("Logo");
                    }
                    if (!File.Exists(GlobalDataInterface.SelectlogoPathName))
                    {
                        File.Create(GlobalDataInterface.SelectlogoPathName).Close();
                        FileStream ofs = File.OpenRead("..//..//" + GlobalDataInterface.SelectlogoPathName);
                        FileStream ifs = File.OpenWrite(GlobalDataInterface.SelectlogoPathName);
                        byte[] b = new byte[1024];
                        int k = 0;
                        while ((k = ofs.Read(b, 0, b.Length)) > 0)
                        {
                            ifs.Write(b, 0, k);
                        }
                        ofs.Close();
                        ifs.Close();
                    }
                }
                //if (File.Exists("..//..//Logo//MyLogo.png")) //生成Logo文件
                //{
                //    if (!Directory.Exists("Logo"))
                //    {
                //        Directory.CreateDirectory("Logo");
                //    }
                //    if (!File.Exists("Logo//MyLogo.png"))
                //    {
                //        File.Create("Logo//MyLogo.png").Close();
                //        FileStream ofs = File.OpenRead("..//..//Logo//MyLogo.png");
                //        FileStream ifs = File.OpenWrite("Logo//MyLogo.png");
                //        byte[] b = new byte[1024];
                //        int k = 0;
                //        while ((k = ofs.Read(b, 0, b.Length)) > 0)
                //        {
                //            ifs.Write(b, 0, k);
                //        }
                //        ofs.Close();
                //        ifs.Close();
                //    }
                //}

                //Add by Chengsk 20131119
                currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
                PrintProtocol.logoPathName = currentPath + GlobalDataInterface.SelectlogoPathName;  //更改LOGO标签地址(相对地址转绝对地址)//Add by xcw - 20200615
                currentDefaultPath = currentPath + "config\\";
                clientInfoFileName = currentDefaultPath + "ClientInfo.txt";

                try //添加try-catch Modify by ChengSk - 20191015
                {
                    if (Directory.Exists(currentDefaultPath))
                    {

                    }
                    else //创建目录
                    {
                        Directory.CreateDirectory(currentDefaultPath);
                    }
                    if (File.Exists(clientInfoFileName))
                    {
                        clientInfoContent = FileOperate.ReadFile(1, clientInfoFileName);
                    }
                    else //创建文件
                    {
                        File.Create(clientInfoFileName).Close();
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Reemoon，Reemoon，Orange");
                        sb.Append("\r\n" + "Reemoon");
                        sb.Append("\r\n" + "Reemoon");
                        sb.Append("\r\n" + "Orange");
                        FileOperate.WriteFile(sb, clientInfoFileName);
                        clientInfoContent = FileOperate.ReadFile(1, clientInfoFileName);
                    }
                }
                catch (Exception ee)
                {
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("MainForm中操作ClientInfo.txt文件Error，错误原因" + ee);
#endif
                }

                if (clientInfoContent == null)
                {
                    GlobalDataInterface.dataInterface.CustomerName = "";
                    GlobalDataInterface.dataInterface.FarmName = "";
                    GlobalDataInterface.dataInterface.FruitName = "";
                }
                else
                {
                    clientInfoContentItem = clientInfoContent.Split('，');
                    GlobalDataInterface.dataInterface.CustomerName = clientInfoContentItem[0].Trim();
                    GlobalDataInterface.dataInterface.FarmName = clientInfoContentItem[1].Trim();
                    GlobalDataInterface.dataInterface.FruitName = clientInfoContentItem[2].Trim();
                }
                //Add by Chengsk 20131119 end

                //Add by Chengsk 20131120
                bIsHaveCompleted = false;
                //Add by Chengsk 20131120 end

                //ivycc 2013.11.1       ---Update By ChengSk 20131119
                //string CilentInfo = "客户名称：绿萌" + "\n" + "农场名称：绿萌" + "\n" + "水果品种：脐橙";
                string CilentInfo;
                if (clientInfoContent == null)
                {
                    CilentInfo = m_resourceManager.GetString("ClientInfoLabelCustomer.Text") + "\n" + m_resourceManager.GetString("ClientInfoLabelGrower.Text") + "\n" + m_resourceManager.GetString("ClientInfoLabelVariety.Text");
                }
                else
                {
                    CilentInfo = m_resourceManager.GetString("ClientInfoLabelCustomer.Text") + GlobalDataInterface.dataInterface.CustomerName + "\n"
                    + m_resourceManager.GetString("ClientInfoLabelGrower.Text") + GlobalDataInterface.dataInterface.FarmName + "\n"
                    + m_resourceManager.GetString("ClientInfoLabelVariety.Text") + GlobalDataInterface.dataInterface.FruitName;
                }
                Font CilentInfoFont = new Font("宋体", 15, FontStyle.Bold);
                PictureBox picB = new PictureBox();
                Graphics TitG = picB.CreateGraphics();
                SizeF XMaxSize0 = TitG.MeasureString(m_resourceManager.GetString("ClientInfoLabelCustomer.Text") + GlobalDataInterface.dataInterface.CustomerName, CilentInfoFont);
                float tempWidth = XMaxSize0.Width;
                XMaxSize0 = TitG.MeasureString(m_resourceManager.GetString("ClientInfoLabelGrower.Text") + GlobalDataInterface.dataInterface.FarmName, CilentInfoFont);
                tempWidth = (tempWidth < XMaxSize0.Width ? XMaxSize0.Width : tempWidth);
                XMaxSize0 = TitG.MeasureString(m_resourceManager.GetString("ClientInfoLabelVariety.Text") + GlobalDataInterface.dataInterface.FruitName, CilentInfoFont);
                tempWidth = (tempWidth < XMaxSize0.Width ? XMaxSize0.Width : tempWidth);
                this.ClientInfoLabel.Width = (int)tempWidth;
                //float FontWidth = this.ClientInfoLabel.Width / this.ClientInfoLabel.Text.Length;
                //this.ClientInfoLabel.Width = (int)(FontWidth * 10);
                this.ClientInfoLabel.Text = CilentInfo;
                TitG.Dispose(); //Add 20180919
                TitG = null;    //Add 20180919
                                //ivycc 2013.11.1 end   ---Update By ChengSk 20131119 End
                                /**************************************************************End*******************************************************************/

                //asc.controlAutoSize(this);

                string FileName = Commonfunction.GetAppSetting("用户配置参数");
                if (FileName == "(未保存)" || FileName == "(no guardado)")     //Modify by xcw - 20200214    
                {
                    SetSeparationProgrameChangelabel(false, null);
                }
                else
                {
                    SetSeparationProgrameChangelabel(true, FileName);
                }


                SetQaulitytoolStripButtonEnabled();//初始化出口中关于检测信息的分隔条
                SetGradedataGridViewInfo();//初始化等级表格
                SetGradeSizelistViewEx();//初始化等级列表 
                SetMainstatusStrip();//设置状态栏



                if (GlobalDataInterface.ExitList.Count > 0)
                {
                    InitExitListBox(true);//初始化出口
                    SetAllExitListBox();//初始化出口中等级

                    if (GlobalDataInterface.ExitVerticalScroll != this.splitContainer2.Panel1.VerticalScroll.Value &&
                        this.splitContainer2.Panel1.VerticalScroll.Value == 0)
                    {
                        bIsOnceMinimumSized = true;
                    }
                }
                //asc.controllInitializeSize(this);

                ////改变绿萌logo大小
                //if (this.Width != MainFormOldSize.Width)
                //{
                //    //int tempWidth = this.Width - this.QaulitytoolStripButton.Width - this.GradetoolStripButton.Width- this.FruitInfotoolStripButton.Width
                //    //    - this.SavetoolStripButton.Width - this.LoadtoolStripButton.Width -this.ProcessInfotoolStripButton.Width-this.StatisticInfotoolStripButton.Width - this.PrinttoolStripButton.Width
                //    //    - this.EndProcesstoolStripButton.Width - this.ClientInfoLabel.Width - this.TimetoolStripLabel.Width-80;
                //    int tempWidth1 = LvMoonLogoOldSize.Width + (this.Width - MainFormOldSize.Width - (this.ClientInfoLabel.Width - ClientInfoLabelOldSize.Width));
                //    if (tempWidth1 * LvMoonLogoOldSize.Height / LvMoonLogoOldSize.Width > this.MaintoolStrip.Height)
                //    {
                //        this.LvMoonLogotoolStripButton.Height = this.MaintoolStrip.Height;
                //        this.LvMoonLogotoolStripButton.Width = this.LvMoonLogotoolStripButton.Height * LvMoonLogoOldSize.Width / LvMoonLogoOldSize.Height;
                //    }
                //    else
                //    {
                //        this.LvMoonLogotoolStripButton.Width = tempWidth1;
                //        this.LvMoonLogotoolStripButton.Height = tempWidth1 * LvMoonLogoOldSize.Height / LvMoonLogoOldSize.Width;
                //    }
                //    //获取原始绿萌图标大小
                //    LvMoonLogoOldSize.Width = this.LvMoonLogotoolStripButton.Width;
                //    LvMoonLogoOldSize.Height = this.LvMoonLogotoolStripButton.Height;
                //    //获取原始界面大小
                //    MainFormOldSize.Width = this.Width;
                //    MainFormOldSize.Height = this.Height;
                //    ////获取原始客户信息界面大小
                //    ClientInfoLabelOldSize.Width = this.ClientInfoLabel.Width;
                //    ClientInfoLabelOldSize.Height = this.ClientInfoLabel.Height;
                //}
                //获取窗口尺寸修改比例
                GlobalDataInterface.gResolutionWidthScale = this.Width * 1.0f / MainFormOldSize.Width;
                GlobalDataInterface.gResolutionHeightScale = this.Height * 1.0f / MainFormOldSize.Height;

                string strAlarmRatioThreshold = Commonfunction.GetAppSetting("出口报警阈值");//Add by ChengSk - 20180122
                if (strAlarmRatioThreshold == "")
                    GlobalDataInterface.fAlarmRatioThreshold = 0;
                else
                {
                    GlobalDataInterface.fAlarmRatioThreshold = ((float)int.Parse(strAlarmRatioThreshold)) / 100;
                }

                //新增数据上传下载功能 Add by ChengSk - 20181206
                GlobalDataInterface.ServerBindLocalIP = Commonfunction.GetAppSetting("绑定地址");
                GlobalDataInterface.ServerURL = Commonfunction.GetAppSetting("访问地址");
                GlobalDataInterface.UploadStartTime = Commonfunction.GetAppSetting("上传起始时间");
                GlobalDataInterface.DownloadStartTime = Commonfunction.GetAppSetting("下载起始时间");
                GlobalDataInterface.DeviceNumber = Commonfunction.GetAppSetting("设备编号");
                GlobalDataInterface.MacAddress = Commonfunction.GetAppSetting("Mac地址");
                GlobalDataInterface.FactoryTime = Commonfunction.GetAppSetting("出厂日期");
                GlobalDataInterface.Country = Commonfunction.GetAppSetting("国家");
                GlobalDataInterface.Area = Commonfunction.GetAppSetting("地区");
                GlobalDataInterface.DetailAddress = Commonfunction.GetAppSetting("详细地址");
                GlobalDataInterface.Contactor = Commonfunction.GetAppSetting("负责人");
                GlobalDataInterface.PhoneNumber = Commonfunction.GetAppSetting("联系电话");
                GlobalDataInterface.TVAccount = Commonfunction.GetAppSetting("TV账号");
                GlobalDataInterface.TVPassword = Commonfunction.GetAppSetting("TV密码");
                GlobalDataInterface.lstMacAddress = Commonfunction.GetMacAddress();
                Thread m_UpLoadThread = new Thread(UpLoadDataThread);  //上传数据线程
                m_UpLoadThread.Priority = ThreadPriority.Normal;
                m_UpLoadThread.IsBackground = true;
                m_UpLoadThread.Start();

                m_MainFormIsInitial = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数MainForm_Load出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数MainForm_Load出错" + ex);
#endif
            }
        }

        //出口间距离
        const int Distance1 = 4;//横向1
        const int Distance2 = 30;//横向2
        const int Distance3 = 12;//纵向
        const int ExitLabelHeight = 14;
        const int ExitLabelWidth = 60;//40
        const int ExitListHeight = 90;//60
        /// <summary>
        /// 初始化出口布局
        /// </summary>
        /// <param name="bReInit"></param>
        /// <returns></returns>
        public bool InitExitListBox(bool bReInit)
        {
            try
            {
                if (!bReInit && GlobalDataInterface.ExitList.Count == m_ExitControl.listBoxList.Count)
                    return false;
                if (GlobalDataInterface.ExitList.Count == 0)
                    return false;
                if (m_ExitControl.listBoxList.Count > 0)
                    DestroyAllExits();

                m_ExitSortingStatisticDic = new Dictionary<int, Queue<string>>(); //初始化
                m_AllSortingStatisticQueue = new Queue<string>(); //初始化
                for (int i = 0; i < GlobalDataInterface.ExitList.Count; i++)    //Add by ChengSk - 20180122
                {
                    Queue<string> exitSortSumQueue = new Queue<string>();
                    m_ExitSortingStatisticDic.Add(i, exitSortSumQueue);   //初始化时出口的统计量为空
                }

                //出口中间的通道实体
                this.splitContainer2.Panel1.VerticalScroll.Value = 0;//ivycc 2013.11.25              
                this.StatisticInfotableLayoutPanel.Location = new Point(3, 20 + 2 * Distance1 + 2 * (ExitLabelHeight + ExitListHeight));
                Rect baseRect;
                baseRect.Left = this.StatisticInfotableLayoutPanel.Location.X;
                baseRect.Right = this.StatisticInfotableLayoutPanel.Location.X + this.StatisticInfotableLayoutPanel.Width;
                baseRect.Top = this.StatisticInfotableLayoutPanel.Location.Y;
                baseRect.Bottom = this.StatisticInfotableLayoutPanel.Location.Y + this.StatisticInfotableLayoutPanel.Height;
                // List<ExitState> tempExitList = new List<ExitState>(GlobalDataInterface.ExitList.ToArray());
                //tempExitList.Add(GlobalDataInterface.ExitList[0]);
                Int64 exitEnabled = GlobalDataInterface.globalOut_GradeInfo.ExitEnabled[1];
                Int64 temp64 = GlobalDataInterface.globalOut_GradeInfo.ExitEnabled[0];
                exitEnabled = exitEnabled << 32;
                exitEnabled += (uint)temp64;
                for (int i = 0; i < GlobalDataInterface.ExitList.Count; i++)
                {

                    for (int j = i + 1; j < GlobalDataInterface.ExitList.Count; j++)
                    {
                        if (GlobalDataInterface.ExitList[i].ColumnIndex > GlobalDataInterface.ExitList[j].ColumnIndex)
                        {
                            //    if (tempExitList.IndexOf(tempExitList[i]) >= 0)
                            //    {
                            //        tempExitList.Remove(tempExitList[i]);
                            //    }
                            ExitState temp = GlobalDataInterface.ExitList[i];
                            GlobalDataInterface.ExitList[i] = GlobalDataInterface.ExitList[j];
                            GlobalDataInterface.ExitList[j] = temp;
                        }
                        else if (GlobalDataInterface.ExitList[i].ColumnIndex == GlobalDataInterface.ExitList[j].ColumnIndex)
                        {
                            if (GlobalDataInterface.ExitList[i].ItemIndex > GlobalDataInterface.ExitList[j].ItemIndex)
                            {
                                ExitState temp = GlobalDataInterface.ExitList[i];
                                GlobalDataInterface.ExitList[i] = GlobalDataInterface.ExitList[j];
                                GlobalDataInterface.ExitList[j] = temp;
                            }
                        }
                        //else
                        //{
                        //    if (tempExitList.IndexOf(tempExitList[i]) >= 0)
                        //    {
                        //        tempExitList.Remove(tempExitList[i]);
                        //    }
                        //    tempExitList.Add(tempExitList[i]);
                        //}
                    }
                    if ((exitEnabled & ((Int64)1 << i)) >> i == 1)
                        m_bExitEnable[i] = true;
                    else
                        m_bExitEnable[i] = false;
                }

                //int[] exitLocationPrePoint = new int[4];
                //int[] exitLocationPreColumnIndex = new int[4];
                //计算分隔条长度
                int MaxStatisticInfoPanelWidth = 0;

                for (int i = 0; i < GlobalDataInterface.ExitList.Count; i++)
                {
                    Label label = new Label();
                    label.Font = new Font("宋体", 9);
                    label.ForeColor = System.Drawing.Color.Black;
                    label.Size = new System.Drawing.Size(ExitLabelWidth, ExitLabelHeight);
                    label.Text = "  " + GlobalDataInterface.ExitList[i].Index.ToString() + "  ";
                    label.Name = "ExitLabel" + GlobalDataInterface.ExitList[i].Index.ToString();
                    if (m_bExitEnable[GlobalDataInterface.ExitList[i].Index - 1])
                        label.BackColor = Color.Gray;
                    else
                        label.BackColor = Color.HotPink;

                    //if (GlobalDataInterface.nSampleOutlet == (i + 1))
                    //{ 
                    //    label.Text = "  " + "抽检口" + "  "; 
                    //}
                    //else
                    //{
                    //    label.Text = "  " + GlobalDataInterface.ExitList[i].Index.ToString() + "  ";
                    //}   

                    //Label fruitlabel = new Label();
                    //fruitlabel.Font = new Font("宋体", 9);
                    //fruitlabel.ForeColor = System.Drawing.Color.Black;
                    //fruitlabel.Size = new System.Drawing.Size(ExitLabelWidth, ExitLabelHeight);
                    //fruitlabel.Text = "  0%  ";
                    //fruitlabel.Name = "ExitFruitLabel" + tempExitList[i].Index.ToString();
                    //if (m_bExitEnable[i])
                    //    fruitlabel.BackColor = Color.Gray;
                    //else
                    //    fruitlabel.BackColor = Color.White;


                    TransParentListBox.TransParentListBox listBox = new TransParentListBox.TransParentListBox();
                    listBox.Font = new Font("宋体", 9);
                    listBox.ForeColor = System.Drawing.Color.Black;
                    listBox.Name = "ExitlistBox" + GlobalDataInterface.ExitList[i].Index.ToString();
                    listBox.Size = new System.Drawing.Size(label.Size.Width, ExitListHeight);
                    listBox.DoubleClick += new System.EventHandler(this.ExitlistBox_DoubleClick);
                    listBox.SelectionMode = SelectionMode.MultiExtended;
                    //listBox.BackColor = Color.Transparent;

                    bool listBoxEnabled;
                    if (m_bExitEnable[GlobalDataInterface.ExitList[i].Index - 1])
                    {
                        listBoxEnabled = true;
                        listBox.BackColor = Color.White;
                        listBox.AllowDrop = true;
                    }
                    else
                    {
                        listBoxEnabled = false;
                        listBox.BackColor = Color.LightGray;
                        listBox.AllowDrop = false;
                    }

                    if (GlobalDataInterface.nSampleOutlet == (GlobalDataInterface.ExitList[i].Index))
                    {
                        //listBox.Items.Add("抽检口");
                        listBox.Items.Add(m_resourceManager.GetString("SampleExitLabel.Text"));

                        if (m_bExitEnable[GlobalDataInterface.ExitList[i].Index - 1])
                        {
                            label.BackColor = Color.DarkOrange;
                            listBox.BackColor = Color.DarkOrange;
                        }
                    }

                    if (GlobalDataInterface.ExitList[i].ItemIndex < 2)//上面两排
                    {
                        if (i == 0)//第一列
                        {
                            if (GlobalDataInterface.ExitList[i].ItemIndex == 0)
                                listBox.Location = new Point(baseRect.Left + 2, baseRect.Top - (Distance1 + ExitListHeight) * 2 - ExitLabelHeight);
                            if (GlobalDataInterface.ExitList[i].ItemIndex == 1)
                                listBox.Location = new Point(baseRect.Left + 2, baseRect.Top - (Distance1 + ExitListHeight));
                        }
                        else
                        {
                            if (GlobalDataInterface.ExitList[i].ColumnIndex - GlobalDataInterface.ExitList[i - 1].ColumnIndex == 0)
                            {
                                if (GlobalDataInterface.ExitList[i].ItemIndex == 0)
                                    listBox.Location = new Point(m_ExitControl.listBoxList[i - 1].Location.X, baseRect.Top - (Distance1 + ExitListHeight) * 2 - ExitLabelHeight);
                                if (GlobalDataInterface.ExitList[i].ItemIndex == 1)
                                    listBox.Location = new Point(m_ExitControl.listBoxList[i - 1].Location.X, baseRect.Top - (Distance1 + ExitListHeight));
                            }
                            else if (GlobalDataInterface.ExitList[i].ColumnIndex - GlobalDataInterface.ExitList[i - 1].ColumnIndex == 1)
                            {
                                if (GlobalDataInterface.ExitList[i].ItemIndex == 0)
                                    listBox.Location = new Point(m_ExitControl.listBoxList[i - 1].Location.X + m_ExitControl.listBoxList[i - 1].Size.Width + Distance1, baseRect.Top - (Distance1 + ExitListHeight) * 2 - ExitLabelHeight);
                                if (GlobalDataInterface.ExitList[i].ItemIndex == 1)
                                    listBox.Location = new Point(m_ExitControl.listBoxList[i - 1].Location.X + m_ExitControl.listBoxList[i - 1].Size.Width + Distance1, baseRect.Top - (Distance1 + ExitListHeight));
                            }
                            else
                            {
                                if (GlobalDataInterface.ExitList[i].ItemIndex == 0)
                                    listBox.Location = new Point(m_ExitControl.listBoxList[i - 1].Location.X + m_ExitControl.listBoxList[i - 1].Size.Width + Distance2, baseRect.Top - (Distance1 + ExitListHeight) * 2 - ExitLabelHeight);
                                if (GlobalDataInterface.ExitList[i].ItemIndex == 1)
                                    listBox.Location = new Point(m_ExitControl.listBoxList[i - 1].Location.X + m_ExitControl.listBoxList[i - 1].Size.Width + Distance2, baseRect.Top - (Distance1 + ExitListHeight));
                            }
                        }
                    }
                    else//下面两排
                    {
                        if (i == 0)//第一列
                        {
                            if (GlobalDataInterface.ExitList[i].ItemIndex == 2)
                                listBox.Location = new Point(baseRect.Left + 2, baseRect.Bottom + Distance3 + ExitLabelHeight);
                            if (GlobalDataInterface.ExitList[i].ItemIndex == 3)
                                listBox.Location = new Point(baseRect.Left + 2, baseRect.Bottom + Distance1 + Distance3 + ExitListHeight + 2 * ExitLabelHeight);
                        }
                        else
                        {
                            if (GlobalDataInterface.ExitList[i].ColumnIndex - GlobalDataInterface.ExitList[i - 1].ColumnIndex == 0)
                            {
                                if (GlobalDataInterface.ExitList[i].ItemIndex == 2)
                                    listBox.Location = new Point(m_ExitControl.listBoxList[i - 1].Location.X, baseRect.Bottom + Distance3 + ExitLabelHeight);
                                if (GlobalDataInterface.ExitList[i].ItemIndex == 3)
                                    listBox.Location = new Point(m_ExitControl.listBoxList[i - 1].Location.X, baseRect.Bottom + Distance1 + Distance3 + ExitListHeight + 2 * ExitLabelHeight);
                            }
                            else if (GlobalDataInterface.ExitList[i].ColumnIndex - GlobalDataInterface.ExitList[i - 1].ColumnIndex == 1)
                            {
                                if (GlobalDataInterface.ExitList[i].ItemIndex == 2)
                                    listBox.Location = new Point(m_ExitControl.listBoxList[i - 1].Location.X + m_ExitControl.listBoxList[i - 1].Size.Width + Distance1, baseRect.Bottom + Distance3 + ExitLabelHeight);
                                if (GlobalDataInterface.ExitList[i].ItemIndex == 3)
                                    listBox.Location = new Point(m_ExitControl.listBoxList[i - 1].Location.X + m_ExitControl.listBoxList[i - 1].Size.Width + Distance1, baseRect.Bottom + Distance1 + Distance3 + ExitListHeight + 2 * ExitLabelHeight);
                            }
                            else
                            {
                                if (GlobalDataInterface.ExitList[i].ItemIndex == 2)
                                    listBox.Location = new Point(m_ExitControl.listBoxList[i - 1].Location.X + m_ExitControl.listBoxList[i - 1].Size.Width + Distance2, baseRect.Bottom + Distance3 + ExitLabelHeight);
                                if (GlobalDataInterface.ExitList[i].ItemIndex == 3)
                                    listBox.Location = new Point(m_ExitControl.listBoxList[i - 1].Location.X + m_ExitControl.listBoxList[i - 1].Size.Width + Distance2, baseRect.Bottom + Distance1 + Distance3 + ExitListHeight + 2 * ExitLabelHeight);
                            }
                        }
                    }

                    //if (tempExitList[i].ColumnIndex ==0 )//第一列
                    //{
                    //    switch(tempExitList[i].ItemIndex)
                    //    {
                    //        case 0:
                    //            listBox.Location = new Point(baseRect.Left + 2, baseRect.Top - (Distance1 + ExitListHeight) * 2 - ExitLabelHeight);
                    //            break;
                    //        case 1:
                    //            listBox.Location = new Point(baseRect.Left + 2, baseRect.Top - (Distance1 + ExitListHeight));
                    //            break;
                    //        case 2:
                    //            listBox.Location = new Point(baseRect.Left + 2, baseRect.Bottom + Distance1 + ExitLabelHeight);
                    //            break;
                    //        case 3:
                    //            listBox.Location = new Point(baseRect.Left + 2, baseRect.Bottom + 2 * Distance1 + ExitListHeight + 2 * ExitLabelHeight);
                    //            break;
                    //        default:break;
                    //    }
                    //    exitLocationPrePoint[tempExitList[i].ItemIndex] = baseRect.Left + 2;
                    //    exitLocationPreColumnIndex[tempExitList[i].ItemIndex] = 0;
                    //}
                    //else//其它列
                    //{
                    //    if (tempExitList[i].ItemIndex == 0)//第一行
                    //    {
                    //        switch(Math.Abs(tempExitList[i].ColumnIndex - exitLocationPreColumnIndex[0]))
                    //        {
                    //            case 1:
                    //                listBox.Location = new Point(exitLocationPrePoint[0] + ExitLabelWidth + Distance1, baseRect.Top - (Distance1 + ExitListHeight) * 2 - ExitLabelHeight);
                    //                exitLocationPrePoint[0] = exitLocationPrePoint[0] + ExitLabelWidth + Distance1;
                    //                break;
                    //            case 2:
                    //                listBox.Location = new Point(exitLocationPrePoint[0] + ExitLabelWidth + Distance2, baseRect.Top - (Distance1 + ExitListHeight) * 2 - ExitLabelHeight);
                    //                exitLocationPrePoint[0] = exitLocationPrePoint[0] + ExitLabelWidth + Distance2;
                    //                break;
                    //            default:break;

                    //        }
                    //        exitLocationPreColumnIndex[0] = tempExitList[i].ColumnIndex;
                    //    }
                    //    else if(tempExitList[i].ItemIndex == 1)//第二行
                    //    {
                    //        switch(Math.Abs(tempExitList[i].ColumnIndex - exitLocationPreColumnIndex[1]))
                    //        {
                    //            case 1:
                    //                listBox.Location = new Point(exitLocationPrePoint[1] + ExitLabelWidth + Distance1, baseRect.Top - (Distance1 + ExitListHeight));
                    //                exitLocationPrePoint[1] = exitLocationPrePoint[1] + ExitLabelWidth + Distance1;
                    //                break;
                    //            case 2:
                    //                listBox.Location = new Point(exitLocationPrePoint[1] + ExitLabelWidth + Distance2, baseRect.Top - (Distance1 + ExitListHeight));
                    //                exitLocationPrePoint[1] = exitLocationPrePoint[1] + ExitLabelWidth + Distance2;
                    //                break;
                    //            default:break;
                    //        }
                    //        exitLocationPreColumnIndex[1] = tempExitList[i].ColumnIndex;
                    //    }
                    //    else if (tempExitList[i].ItemIndex == 2)//第三行
                    //    {
                    //        switch (Math.Abs(tempExitList[i].ColumnIndex - exitLocationPreColumnIndex[2]))
                    //        {
                    //            case 1:
                    //                listBox.Location = new Point(exitLocationPrePoint[2] + ExitLabelWidth + Distance1, baseRect.Bottom + Distance1 + ExitLabelHeight);
                    //                exitLocationPrePoint[2] = exitLocationPrePoint[2] + ExitLabelWidth + Distance1;
                    //                break;
                    //            case 2:
                    //                listBox.Location = new Point(exitLocationPrePoint[2] + ExitLabelWidth + Distance2, baseRect.Bottom + Distance1 + ExitLabelHeight);
                    //                exitLocationPrePoint[2] = exitLocationPrePoint[2] + ExitLabelWidth + Distance2;
                    //                break;
                    //            default: break;
                    //        }
                    //        exitLocationPreColumnIndex[2] = tempExitList[i].ColumnIndex;
                    //    }
                    //    else if (tempExitList[i].ItemIndex == 3)//第四行
                    //    {
                    //        switch (Math.Abs(tempExitList[i].ColumnIndex - exitLocationPreColumnIndex[3]))
                    //        {
                    //            case 1:
                    //                listBox.Location = new Point(exitLocationPrePoint[3] + ExitLabelWidth + Distance1, baseRect.Bottom + 2 * Distance1 + ExitListHeight + 2 * ExitLabelHeight);
                    //                exitLocationPrePoint[3] = exitLocationPrePoint[3] + ExitLabelWidth + Distance1;
                    //                break;
                    //            case 2:
                    //                listBox.Location = new Point(exitLocationPrePoint[3] + ExitLabelWidth + Distance2, baseRect.Bottom + 2 * Distance1 + ExitListHeight + 2 * ExitLabelHeight);
                    //                exitLocationPrePoint[3] = exitLocationPrePoint[3] + ExitLabelWidth + Distance2;
                    //                break;
                    //            default: break;
                    //        }
                    //        exitLocationPreColumnIndex[3] = tempExitList[i].ColumnIndex;
                    //    }

                    //}

                    listBox.FruitRadio = 0.0f;
                    listBox.FruitRadioColor = Color.MediumSpringGreen;
                    label.Location = new Point(listBox.Location.X, listBox.Location.Y - ExitLabelHeight);


                    listBox.DragEnter += new DragEventHandler(ExitlistBox_DragEnter);
                    listBox.DragDrop += new DragEventHandler(ExitlistBox_DragDrop);
                    listBox.MouseDown += new MouseEventHandler(ExitlistBox_MouseDown);
                    listBox.MouseHover += new EventHandler(ExitlistBox_MouseHover);

                    // listBox.DragLeave += new EventHandler(ExitlistBox_DragLeave);
                    this.splitContainer2.Panel1.Controls.Add(label);

                    this.splitContainer2.Panel1.Controls.Add(listBox);
                    m_ExitControl.listBoxEnabledList.Add(listBoxEnabled);
                    m_ExitControl.listBoxList.Add(listBox);
                    m_ExitControl.labelList.Add(label);
                    //m_ExitControlList[m_ExitControlList.Count - 1].listBox.DoubleClick += new System.EventHandler(this.ExitlistBox_DoubleClick);

                    //计算分隔条长度
                    if (listBox.Location.X + ExitLabelWidth > MaxStatisticInfoPanelWidth)
                        MaxStatisticInfoPanelWidth = listBox.Location.X + ExitLabelWidth;

                    this.StatisticInfotableLayoutPanel.Location = new Point(3, 20 + 2 * Distance1 + 2 * (ExitLabelHeight + ExitListHeight));
                }
                //m_ExitControl.listBoxList[0].FruitRadio = 0.5f;
                //m_ExitControl.listBoxList[0].FruitRadioColor = Color.Green;
                if (MaxStatisticInfoPanelWidth + Distance2 > this.StatisticInfotableLayoutPanel.Width)
                    this.StatisticInfotableLayoutPanel.Width = MaxStatisticInfoPanelWidth + Distance2;
                this.splitContainer2.SplitterDistance = GlobalDataInterface.SplitterDistance2;
                this.splitContainer2.Panel1.VerticalScroll.Value = GlobalDataInterface.ExitVerticalScroll;

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数InitExitListBox出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数InitExitListBox出错" + ex);
#endif
                return false;
            }
        }

        /// <summary>
        /// 设置单个出口List内容
        /// </summary>
        public void SetExitListBox(int ContorlIndex, int ExitIndex)
        {
            try
            {
                if (m_ExitControl.listBoxList.Count > 0 && GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0 && m_ExitControl.listBoxList[ContorlIndex] != null)
                {
                    m_ExitControl.listBoxList[ContorlIndex].Items.Clear();
                    //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 1) > 0)
                    int qualNum = 1;
                    if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//品质与尺寸或者品质与重量
                    {
                        qualNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                    }
                    int sizeNum = 1;
                    if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                        sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    string GradeName;
                    if (GlobalDataInterface.nSampleOutlet == int.Parse(m_ExitControl.labelList[ContorlIndex].Text.Trim()))
                    {
                        //m_ExitControl.listBoxList[ContorlIndex].Items.Add("抽检口");
                        m_ExitControl.listBoxList[ContorlIndex].Items.Add(m_resourceManager.GetString("SampleExitLabel.Text"));
                    }
                    for (int i = 0; i < qualNum; i++)
                    {
                        for (int j = 0; j < sizeNum; j++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit > 0)
                            {
                                //GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit >> (int.Parse(m_ExitControl.labelList[k].Text) - 1) & 1) > 0
                                //if ((GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit >> ExitIndex & 1) > 0)
                                //{
                                //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                //    GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                                //    //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 1) > 0)
                                //    if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)
                                //    {
                                //        GradeName += ".";
                                //        Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                //        GradeName += Encoding.Default.GetString(temp).TrimEnd('\0');
                                //    }
                                //    GradeName += "    ";
                                //    m_ExitControl.listBoxList[ContorlIndex].Items.Add(GradeName);
                                //}
                                if (GlobalDataInterface.UpData_gradeinfo)
                                {
                                    if ((GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit >> ExitIndex & 1) > 0)
                                    {
                                        Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                        GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                                        //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 1) > 0)
                                        if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)
                                        {
                                            GradeName += ".";
                                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                            GradeName += Encoding.Default.GetString(temp).TrimEnd('\0');
                                        }
                                        GradeName += "    ";
                                        m_ExitControl.listBoxList[ContorlIndex].Items.Add(GradeName);
                                    }
                                        
                                }
                                this.GradedataGridView[j, i].Style.BackColor = Color.LightGray;
                            }
                            else
                            {
                                this.GradedataGridView[j, i].Style.BackColor = Color.Pink;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数SetExitListBox出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数SetExitListBox出错" + ex);
#endif
            }

        }

        /// <summary>
        /// 设置出口List内容
        /// </summary>
        public void SetAllExitListBox()
        {
            try
            {
                if (m_ExitControl.listBoxList.Count > 0 && GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                {
                    for (int k = 0; k < m_ExitControl.listBoxList.Count; k++)
                    {
                        m_ExitControl.listBoxList[k].Items.Clear();
                        if (GlobalDataInterface.nSampleOutlet == int.Parse(m_ExitControl.labelList[k].Text.Trim())) m_ExitControl.listBoxList[k].Items.Add(m_resourceManager.GetString("SampleExitLabel.Text"));
                    }

                    int sizeNum = 1;
                    int qulNum = 1;
                    if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//品质与尺寸或者品质与重量
                    {
                        qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                    }
                    if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                        sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    string GradeName;
                    for (int i = 0; i < qulNum; i++)
                    {
                        for (int j = 0; j < sizeNum; j++)
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit > 0)
                            {
                                for (int k = 0; k < m_ExitControl.listBoxList.Count; k++)
                                {
                                    if ((GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit >> (int.Parse(m_ExitControl.labelList[k].Text) - 1) & 1) > 0)
                                    {
                                        Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                        GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                                        if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)
                                        {
                                            GradeName += ".";
                                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                            GradeName += Encoding.Default.GetString(temp).TrimEnd('\0');
                                        }
                                        GradeName += "    ";
                                        m_ExitControl.listBoxList[k].Items.Add(GradeName);
                                    }
                                }
                                this.GradedataGridView[j, i].Style.BackColor = Color.LightGray;
                            }
                            else
                            {
                                this.GradedataGridView[j, i].Style.BackColor = Color.Pink;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数SetAllExitListBox出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数SetAllExitListBox出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 获取匹配的出口
        /// </summary>
        /// <param name="ItemIndex"></param>
        /// <param name="ColumnIndex"></param>
        /// <returns></returns>
        private int FindMatchExit(List<ExitState> list, int ItemIndex, int ColumnIndex)
        {
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if ((list[i].ItemIndex == ItemIndex) && (list[i].ColumnIndex == ColumnIndex))
                        return i;
                }
                return -1;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数FindMatchExit出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数FindMatchExit出错" + ex);
#endif
                return -1;
            }
        }

        //        /// <summary>
        //        /// 获取纵队相同的匹配数
        //        /// </summary>
        //        /// <param name="ColumnIndex"></param>
        //        /// <returns></returns>
        //        private int FindMatchColumnIndexExit(List<ExitState> list,int ColumnIndex)
        //        {
        //            try
        //            {
        //                int MatchNum = 0;
        //                for (int i = 0; i < list.Count; i++)
        //                {
        //                    if (list[i].ColumnIndex == ColumnIndex)
        //                    {
        //                        MatchNum++;
        //                    }
        //                }
        //                return MatchNum;
        //            }
        //            catch (Exception ex)
        //            {
        //                Trace.WriteLine("MainForm中函数FindMatchColumnIndexExit出错" + ex);
        //#if REALEASE
        //                GlobalDataInterface.WriteErrorInfo("MainForm中函数FindMatchColumnIndexExit出错" + ex);
        //#endif
        //                return 0;
        //            }
        //        }

        /// <summary>
        /// 销毁所有出口控件
        /// </summary>
        private void DestroyAllExits()
        {
            try
            {
                for (int i = 0; i < m_ExitControl.listBoxList.Count; i++)
                {
                    this.splitContainer2.Panel1.Controls.Remove(m_ExitControl.listBoxList[i]);
                    this.splitContainer2.Panel1.Controls.Remove(m_ExitControl.labelList[i]);
                    m_ExitControl.labelList[i].Dispose();
                    m_ExitControl.listBoxList[i].Dispose();
                }
                m_ExitControl.listBoxEnabledList.Clear();
                m_ExitControl.labelList.Clear();
                m_ExitControl.listBoxList.Clear();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数DestroyAllExits出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数DestroyAllExits出错" + ex);
#endif
            }
        }

        bool splitContainer2SplitterMoved = false;

        private void splitContainer2_MouseDown(object sender, MouseEventArgs e)
        {
            splitContainer2SplitterMoved = true;
        }
        /// <summary>
        /// 分隔条移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            try
            {
                if (GlobalDataInterface.ExitList.Count > 0 && splitContainer2SplitterMoved)
                {
                    //InitExitListBox(true);
                    //SetAllExitListBox();

                    this.StatisticInfotableLayoutPanel.Location = new Point(this.StatisticInfotableLayoutPanel.Location.X, 20 + 2 * Distance1 + 2 * (ExitLabelHeight + ExitListHeight) - this.splitContainer2.Panel1.VerticalScroll.Value);//ivycc 2013.11.25
                }
                splitContainer2SplitterMoved = false;

                if (!m_MainFormIsInitial)
                {
                    this.splitContainer2.SplitterDistance = Convert.ToInt32(Commonfunction.GetAppSetting("拆分器距离"));
                }

                //GlobalDataInterface.SplitterDistance2 = splitContainer2.SplitterDistance;
                //Commonfunction.SetAppSetting("拆分器距离", splitContainer2.SplitterDistance.ToString());
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数splitContainer2_SplitterMoved出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数splitContainer2_SplitterMoved出错" + ex);
#endif
            }
        }


        /// <summary>
        /// 出口悬停事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitlistBox_MouseHover(object sender, EventArgs e)
        {
            try
            {
                TransParentListBox.TransParentListBox listBox = (TransParentListBox.TransParentListBox)sender;
                int Index = m_ExitControl.listBoxList.IndexOf(listBox);

                this.ExitGradetoolTip.AutoPopDelay = 5000;
                this.ExitGradetoolTip.InitialDelay = 500;
                this.ExitGradetoolTip.ReshowDelay = 200;
                this.ExitGradetoolTip.ShowAlways = true;

                string str = "";
                for (int i = 0; i < listBox.Items.Count; i++)
                {
                    if (i != 0)
                        str += Environment.NewLine;
                    str += (string)listBox.Items[i];
                }
                this.ExitGradetoolTip.SetToolTip(listBox, str);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数ExitlistBox_MouseHover出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数ExitlistBox_MouseHover出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 出口双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitlistBox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                TransParentListBox.TransParentListBox listBox = (TransParentListBox.TransParentListBox)sender;
                string Name = listBox.Name;
                int Index = m_ExitControl.listBoxList.IndexOf(listBox);
                if (!m_ExitControl.listBoxEnabledList[Index])
                    return;
                //Graphics graphics = m_ExitControl.listBoxList[Index].CreateGraphics();
                //Rectangle rect = new Rectangle(1, m_ExitControl.listBoxList[Index].Height - 11, m_ExitControl.listBoxList[Index].Width - 2, 10);

                //SolidBrush brush = new SolidBrush(Color.FromArgb(100, Color.Black.R, Color.Black.G, Color.Black.B));
                //graphics.FillRectangle(brush, rect);
                ExitSwitchForm exitSwitchForm = new ExitSwitchForm(this, Index, int.Parse(m_ExitControl.labelList[Index].Text) - 1);
                exitSwitchForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数ExitlistBox_DoubleClick出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数ExitlistBox_DoubleClick出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 出口鼠标右击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitlistBox_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                TransParentListBox.TransParentListBox listBox = (TransParentListBox.TransParentListBox)sender;
                int Index = m_ExitControl.listBoxList.IndexOf(listBox);
                if (e.Button == MouseButtons.Right)
                {
                    if (m_ExitControl.listBoxEnabledList[Index])//启用
                    {
                        this.启动ToolStripMenuItem.Enabled = false;
                        this.启动ToolStripMenuItem.Checked = true;
                        this.暂停ToolStripMenuItem.Checked = false;
                        this.暂停ToolStripMenuItem.Enabled = true;
                    }
                    else//暂停
                    {
                        this.启动ToolStripMenuItem.Enabled = true;
                        this.启动ToolStripMenuItem.Checked = false;
                        this.暂停ToolStripMenuItem.Checked = true;
                        this.暂停ToolStripMenuItem.Enabled = false;
                    }
                    // this.ExitcontextMenuStrip.sou
                    // Rectangle rect=new Rectangle();

                    // this.splitContainer2.Panel1.loc (rect);
                    this.ExitcontextMenuStrip.Show(listBox, e.X, e.Y);
                }
                else
                {
                    if (!m_ExitControl.listBoxEnabledList[Index])
                        return;
                    if (e.Clicks == 1 && m_ExitControl.listBoxList[Index].SelectedIndex >= 0)
                    {
                        m_IsExitlistBoxMouseDown = true;
                        m_IsGradedataGridViewMouseDown = false;
                        m_CurrentMouseDownlistBoxIndex = Index;
                        m_ExitControl.listBoxList[Index].DoDragDrop(m_ExitControl.listBoxList[Index].SelectedItem, DragDropEffects.Move);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数ExitlistBox_MouseDown出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数ExitlistBox_MouseDown出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 出口右键菜单启动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 启动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                ContextMenuStrip menu = (ContextMenuStrip)menuItem.Owner;
                TransParentListBox.TransParentListBox listBox = (TransParentListBox.TransParentListBox)menu.SourceControl;
                int Index = m_ExitControl.listBoxList.IndexOf(listBox);
                //m_ExitControl.listBoxEnabledList[Index] = true;
                //m_ExitControl.labelList[Index].BackColor = Color.Gray;
                //listBox.BackColor = Color.White;
                //menuItem.Checked = true;
                //menuItem.Enabled = false;
                //暂停ToolStripMenuItem.Checked = false;
                //暂停ToolStripMenuItem.Enabled = true;
                //m_ExitControl.listBoxList[Index].AllowDrop = true;

                if (GlobalDataInterface.nSampleOutlet.ToString() == m_ExitControl.labelList[Index].Text.Trim()) //抽检口，颜色特殊
                {
                    m_ExitControl.labelList[Index].BackColor = Color.DarkOrange;
                    listBox.BackColor = Color.DarkOrange;
                }

                if (int.Parse(m_ExitControl.labelList[Index].Text) < 33)
                {
                    GlobalDataInterface.globalOut_GradeInfo.ExitEnabled[0] |= (1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 1));
                }
                else
                {
                    GlobalDataInterface.globalOut_GradeInfo.ExitEnabled[1] |= (1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 33));

                }

                if (GlobalDataInterface.global_IsTestMode)
                {
                    //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                    int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                    if (global_IsTest != 0) //add by xcw 20201211
                    {
                        MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                        LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                }
                m_ExitControl.listBoxEnabledList[Index] = true;
                m_ExitControl.labelList[Index].BackColor = Color.Gray;
                listBox.BackColor = Color.White;
                menuItem.Checked = true;
                menuItem.Enabled = false;
                暂停ToolStripMenuItem.Checked = false;
                暂停ToolStripMenuItem.Enabled = true;
                m_ExitControl.listBoxList[Index].AllowDrop = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数启动ToolStripMenuItem_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数启动ToolStripMenuItem_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 出口右键菜单暂停事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 暂停ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                ContextMenuStrip menu = (ContextMenuStrip)menuItem.Owner;
                TransParentListBox.TransParentListBox listBox = (TransParentListBox.TransParentListBox)menu.SourceControl;
                int Index = m_ExitControl.listBoxList.IndexOf(listBox);

                /*检查出口内有没有唯一等级项*/
                long exitGrade = ((long)1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 1));
                int sizeNum = 1;
                int qulNum = 1;
                if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//品质与尺寸或者品质与重量
                {
                    qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                }
                if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                    sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                for (int i = 0; i < qulNum; i++)
                {
                    for (int j = 0; j < sizeNum; j++)
                    {
                        if ((GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit & exitGrade) > 0)//该出口存在此等级
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit == exitGrade) //此等级只存在该出口
                            {
                                //MessageBox.Show("The grade is only in this exit,don't be disabled!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                MessageBox.Show(LanguageContainer.MainFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                                    LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            else
                            {
                                long tempexit = 1;
                                bool HasExit = false;
                                for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nExitNum; k++)
                                {
                                    if (k != Index)
                                    {
                                        if (((tempexit << k) & GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit) > 0)
                                        {
                                            if (k < 33)
                                            {
                                                if ((GlobalDataInterface.globalOut_GradeInfo.ExitEnabled[0] & ((Int64)1 << k)) > 0)
                                                {
                                                    HasExit = true;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if ((GlobalDataInterface.globalOut_GradeInfo.ExitEnabled[1] & ((Int64)1 << (k - 33))) > 0)
                                                {
                                                    HasExit = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!HasExit)
                                {
                                    //MessageBox.Show("The grade is only in this exit,don't be disabled!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    MessageBox.Show(LanguageContainer.MainFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                                        LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                            }
                        }
                    }
                }
                /*检查出口内有没有唯一等级项 end*/

                //m_ExitControl.listBoxEnabledList[Index] = false;
                //m_ExitControl.labelList[Index].BackColor = Color.HotPink;
                //listBox.BackColor = Color.LightGray;
                //menuItem.Checked = true;
                //menuItem.Enabled = false;
                //启动ToolStripMenuItem.Checked = false;
                //启动ToolStripMenuItem.Enabled = true;
                //m_ExitControl.listBoxList[Index].AllowDrop = false;

                if (int.Parse(m_ExitControl.labelList[Index].Text) < 33)
                    GlobalDataInterface.globalOut_GradeInfo.ExitEnabled[0] ^= ((Int32)1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 1));
                else
                    GlobalDataInterface.globalOut_GradeInfo.ExitEnabled[1] ^= ((Int32)1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 33));
                //int global_IsTest = 0;
                if (GlobalDataInterface.global_IsTestMode)
                {
                    int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                    if (global_IsTest != 0)
                    {
                        MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                        LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }              
                    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                }
               
                    m_ExitControl.listBoxEnabledList[Index] = false;
                    m_ExitControl.labelList[Index].BackColor = Color.HotPink;
                    listBox.BackColor = Color.LightGray;
                    menuItem.Checked = true;
                    menuItem.Enabled = false;
                    启动ToolStripMenuItem.Checked = false;
                    启动ToolStripMenuItem.Enabled = true;
                    m_ExitControl.listBoxList[Index].AllowDrop = false;
                
               

                //if (int.Parse(m_ExitControl.labelList[Index].Text) < 33)
                //    GlobalDataInterface.globalOut_GradeInfo.ExitEnabled[0] ^= ((Int32)1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 1));
                //else
                //    GlobalDataInterface.globalOut_GradeInfo.ExitEnabled[1] ^= ((Int32)1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 33));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数暂停ToolStripMenuItem_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数暂停ToolStripMenuItem_Click出错" + ex);
#endif
            }

        }


        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                ContextMenuStrip menu = (ContextMenuStrip)menuItem.Owner;
                TransParentListBox.TransParentListBox listBox = (TransParentListBox.TransParentListBox)menu.SourceControl;
                int Index = m_ExitControl.listBoxList.IndexOf(listBox);
                if (!m_ExitControl.listBoxEnabledList[Index])
                    return;
                //Graphics graphics = m_ExitControl.listBoxList[Index].CreateGraphics();
                //Rectangle rect = new Rectangle(1, m_ExitControl.listBoxList[Index].Height - 11, m_ExitControl.listBoxList[Index].Width - 2, 10);

                //SolidBrush brush = new SolidBrush(Color.FromArgb(100, Color.Black.R, Color.Black.G, Color.Black.B));
                //graphics.FillRectangle(brush, rect);
                ExitSwitchForm exitSwitchForm = new ExitSwitchForm(this, Index, int.Parse(m_ExitControl.labelList[Index].Text) - 1);
                exitSwitchForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数打开ToolStripMenuItem_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数打开ToolStripMenuItem_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 清空出口中的所有等级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //DialogResult result = MessageBox.Show("Do you want to clear all of grade about exit?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                DialogResult result = MessageBox.Show(LanguageContainer.MainFormMessagebox15Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                    ContextMenuStrip menu = (ContextMenuStrip)menuItem.Owner;
                    TransParentListBox.TransParentListBox listBox = (TransParentListBox.TransParentListBox)menu.SourceControl;
                    int Index = m_ExitControl.listBoxList.IndexOf(listBox);
                    if (!m_ExitControl.listBoxEnabledList[Index])
                        return;
                    if (GlobalDataInterface.nSampleOutlet.ToString() == m_ExitControl.labelList[Index].Text.Trim())  //抽检口，返回
                        return;

                    if (m_ExitControl.listBoxList[Index].Items.Count > 0)
                    {
                        for (int i = 0; i < m_ExitControl.listBoxList[Index].Items.Count; i++)
                        {
                            for (int p = 0; p < GradedataGridView.RowCount; p++)
                            {
                                for (int q = 0; q < GradedataGridView.ColumnCount; q++)
                                {
                                    if ((string)m_ExitControl.listBoxList[Index].Items[i] == (string)this.GradedataGridView[q, p].Value)
                                    {
                                        GlobalDataInterface.globalOut_GradeInfo.grades[p * ConstPreDefine.MAX_SIZE_GRADE_NUM + q].exit &= ~((long)1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 1));

                                        //if (GlobalDataInterface.globalOut_GradeInfo.grades[p * ConstPreDefine.MAX_SIZE_GRADE_NUM + q].exit == 0) //Add by ChengSk - 20180408
                                        //{
                                        //    this.GradedataGridView[q, p].Style.BackColor = Color.Pink;
                                        //    this.GradedataGridView[q, p].Style.SelectionBackColor = Color.FromArgb(36, 155, 255);
                                        //    this.GradedataGridView[q, p].Selected = false;
                                        //}
                                    }
                                }
                            }
                        }
                        //m_ExitControl.listBoxList[Index].Items.Clear();

                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                            int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                            if (global_IsTest != 0) //add by xcw 20201211
                            {
                                MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                                LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                        }
                        if (m_ExitControl.listBoxList[Index].Items.Count > 0)
                        {
                            for (int i = 0; i < m_ExitControl.listBoxList[Index].Items.Count; i++)
                            {
                                for (int p = 0; p < GradedataGridView.RowCount; p++)
                                {
                                    for (int q = 0; q < GradedataGridView.ColumnCount; q++)
                                    {
                                        if ((string)m_ExitControl.listBoxList[Index].Items[i] == (string)this.GradedataGridView[q, p].Value)
                                        {

                                            if (GlobalDataInterface.globalOut_GradeInfo.grades[p * ConstPreDefine.MAX_SIZE_GRADE_NUM + q].exit == 0) //Add by ChengSk - 20180408
                                            {
                                                this.GradedataGridView[q, p].Style.BackColor = Color.Pink;
                                                this.GradedataGridView[q, p].Style.SelectionBackColor = Color.FromArgb(36, 155, 255);
                                                this.GradedataGridView[q, p].Selected = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        m_ExitControl.listBoxList[Index].Items.Clear();

                    } //End inner if   

                } //End outer if
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数清空ToolStripMenuItem_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数清空ToolStripMenuItem_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置主界面等级表格
        /// </summary>
        public void SetGradedataGridViewInfo()
        {
            try
            {
                this.GradedataGridView.Font = new Font("微软雅黑", 9, FontStyle.Regular);
                this.GradedataGridView.AllowUserToAddRows = true;
                this.GradedataGridView.Columns.Clear();
                this.GradedataGridView.Rows.Clear();
                this.GradedataGridView.ReadOnly = true;
                int rowNum = 1;
                int colNum = 1;
                DataGridViewTextBoxColumn column;
                DataGridViewRow row;
                //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 57) > 0 && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 7) > 0)//品质与尺寸或者品质与重量
                if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0)//品质与尺寸或者品质与重量
                {
                    if (GlobalDataInterface.Quality_GradeInfo.GradeCnt != 0)
                        rowNum = GlobalDataInterface.Quality_GradeInfo.GradeCnt;
                    if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum != 0)
                        colNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                    for (int i = 1; i <= colNum; i++)
                    {
                        column = new DataGridViewTextBoxColumn();
                        byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, tempByte, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        column.Width = 120;
                        //  column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                        column.HeaderText = Encoding.Default.GetString(tempByte).TrimEnd('\0');
                        this.GradedataGridView.Columns.Add(column);
                    }
                    for (int i = 0; i < rowNum; i++)
                    {
                        //if (i == 0)
                        //{
                        row = new DataGridViewRow();
                        //  byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        //  Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, i  * ConstPreDefine.MAX_TEXT_LENGTH, tempByte, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                        this.GradedataGridView.Rows.Add(row);
                        this.GradedataGridView.Rows[i].HeaderCell.Value = Encoding.Default.GetString(GlobalDataInterface.Quality_GradeInfo.Item[i].GradeName).TrimEnd('\0');
                        //}
                        //else
                        //{
                        //    row = new DataGridViewRow();
                        //    //byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        //    //Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, tempByte, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        //    row.HeaderCell.Value = Encoding.Default.GetString(GlobalDataInterface.Quality_GradeInfo.Item[i].GradeName).TrimEnd('\0');
                        //    // row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                        //    this.GradedataGridView.Rows.Add(row);
                        //}
                    }
                }
                //else if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 1)//品质
                else if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//品质
                {
                    rowNum = GlobalDataInterface.Quality_GradeInfo.GradeCnt;
                    for (int i = 0; i < rowNum; i++)
                    {
                        //if (i == 0)
                        //{
                        row = new DataGridViewRow();
                        //byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        //Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, tempByte, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        // row.HeaderCell.Value = Encoding.Default.GetString(GlobalDataInterface.Quality_GradeInfo.Item[i].GradeName);
                        // row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                        this.GradedataGridView.Rows.Add(row);
                        this.GradedataGridView.Rows[i].HeaderCell.Value = Encoding.Default.GetString(GlobalDataInterface.Quality_GradeInfo.Item[i].GradeName).TrimEnd('\0');
                        //}
                        //else
                        //{
                        //    row = new DataGridViewRow();
                        //    //byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        //    //Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, tempByte, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        //    row.HeaderCell.Value = Encoding.Default.GetString(GlobalDataInterface.Quality_GradeInfo.Item[i].GradeName);
                        //    // row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                        //    this.GradedataGridView.Rows.Add(row);
                        //}
                        this.GradedataGridView[0, i].Value = Encoding.Default.GetString(GlobalDataInterface.Quality_GradeInfo.Item[i].GradeName).TrimEnd('\0') + "    ";
                    }
                    //column = new DataGridViewImageColumn();
                    //this.GradedataGridView.Columns.Add(column);
                }
                else//尺寸
                {
                    colNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;

                    for (int i = 1; i <= colNum; i++)
                    {
                        column = new DataGridViewTextBoxColumn();
                        byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, tempByte, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        column.Width = 120;
                        // column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                        column.HeaderText = Encoding.Default.GetString(tempByte).TrimEnd('\0');
                        this.GradedataGridView.Columns.Add(column);
                        if (this.GradedataGridView.RowCount == 1)
                        {
                            row = new DataGridViewRow();
                            this.GradedataGridView.Rows.Add(row);
                        }
                        this.GradedataGridView[i - 1, 0].Value = Encoding.Default.GetString(tempByte).TrimEnd('\0') + "    ";
                    }

                }

                //   DateTime before = DateTime.Now;
                //设置表格内属性
                for (int i = 0; i < rowNum; i++)
                {
                    for (int j = 0; j < colNum; j++)
                    {
                        // this.GradedataGridView[j, i].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        //if (changeColor)
                        //{
                        if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit > 0)
                            this.GradedataGridView[j, i].Style.BackColor = Color.LightGray;
                        else
                            this.GradedataGridView[j, i].Style.BackColor = Color.Pink;
                        //}

                        //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 57) > 0 && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 7) > 0)//品质与尺寸或者品质与重量
                        if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0)//品质与尺寸或者品质与重量
                            this.GradedataGridView[j, i].Value = this.GradedataGridView.Columns[j].HeaderText.TrimEnd('\0') + "." + (string)this.GradedataGridView.Rows[i].HeaderCell.Value + "    ";
                    }
                }
                //  DateTime after = DateTime.Now;

                //   TimeSpan time = after - before;

                this.GradedataGridView.RowHeadersWidth = 120;
                //this.GradedataGridView.TopLeftHeaderCell.Size = new Size(this.GradedataGridView.RowHeadersWidth, this.GradedataGridView.ColumnHeadersHeight);
                this.GradedataGridView.TopLeftHeaderCell.Value = m_resourceManager.GetString("GradedataGridViewTopLeftHeadCelllabel.Text");
                this.GradedataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                this.GradedataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                this.GradedataGridView.AllowUserToAddRows = false;
                if (GlobalDataInterface.global_SIMTest)
                {
                    GlobalDataInterface.TransmitParam(ConstPreDefine.SIM_ID, (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_GRADE_INFO, null);//更新到SIM
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数SetGradedataGridViewInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数SetGradedataGridViewInfo出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 检测分选运行时间数据表
        /// </summary>
        private void CheckRunningTimeInfoDataTable()
        {
            try
            {
                DataSet dst = databaseOperation.GetRunningTimeInfoByStopTime("");
                //DataSet dst1 = databaseOperation.GetFruitTopCustomerID(); //test by xcw 20200415
                //if (dst1.Tables[0].Rows.Count == 0)
                //    return;
                //DataSet dstTemp = databaseOperation.GetFruitByCompletedState(); //Modify by ChengSk - 20181212
                //if (dstTemp.Tables[0].Rows.Count > 0)
                //{
                //    string[] startTimes = dstTemp.Tables[0].Rows[0]["StartTime"].ToString().Split(' ');
                //    this.StartTimeChangelabel.Text = startTimes[1];
                //}
                //else
                //{
                //    this.StartTimeChangelabel.Text = DateTime.Now.ToLongTimeString().ToString();
                //}
                if (dst.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {
                        databaseOperation.UpdateRunningStopTime(int.Parse(dst.Tables[0].Rows[i]["ID"].ToString()), dst.Tables[0].Rows[i]["StartTime"].ToString());
                    }
                }
                RunningTimeInfoStopTimeIsEmptyFlags = false;
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// 出口列表拖入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitlistBox_DragEnter(object sender, DragEventArgs e)
        {
            if (m_IsGradedataGridViewMouseDown || m_IsExitlistBoxMouseDown)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// 出口列表拖曳事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitlistBox_DragDrop(object sender, DragEventArgs e)
        {
            //#if DEBUG
            //            while (!m_IsGradedataGridViewMouseDownDone);
            //#endif
            try
            {
                TransParentListBox.TransParentListBox listBox = (TransParentListBox.TransParentListBox)sender;
                string Name = listBox.Name;
                int Index = m_ExitControl.listBoxList.IndexOf(listBox);
                if (!m_ExitControl.listBoxEnabledList[Index])
                    return;
                //从表格拖曳
                if (m_IsGradedataGridViewMouseDown)
                {
                    m_IsGradedataGridViewMouseDown = false;
                    m_IsExitlistBoxMouseDown = false;
                    bool IsSendFSM = false;
                    if (GlobalDataInterface.nSampleOutlet == int.Parse(m_ExitControl.labelList[Index].Text.Trim())) //往抽样出口拖动等级
                    {
                        if (m_ExitControl.listBoxList[Index].Items.Count == 2 || m_GradedataGridViewSelectedCellList.Count > 1)
                            return;
                    }
                    for (int i = 0; i < m_GradedataGridViewSelectedCellList.Count; i++)
                    {
                        if (m_ExitControl.listBoxList[Index].Items.Count > 0)
                        {
                            bool IsSame = false;
                            for (int j = 0; j < m_ExitControl.listBoxList[Index].Items.Count; j++)
                            {
                                if ((string)m_ExitControl.listBoxList[Index].Items[j] == (string)this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Value)
                                {
                                    //this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.SelectionBackColor = Color.LightGray;
                                    //this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.SelectionForeColor = Color.Black;
                                    IsSame = true;
                                    break;
                                }
                            }
                            if (!IsSame)
                            {
                                //m_ExitControl.listBoxList[Index].Items.Add((string)this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Value);
                                //this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.BackColor = Color.LightGray;
                                //this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.SelectionBackColor = Color.LightGray;
                                //this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.SelectionForeColor = Color.Black;
                                GlobalDataInterface.globalOut_GradeInfo.grades[m_GradedataGridViewSelectedCellList[i].rowIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + m_GradedataGridViewSelectedCellList[i].colIndex].exit |= ((long)1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 1));
                                IsSendFSM = true;
                                //if (GlobalDataInterface.global_IsTestMode)
                                //    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                            }
                        }
                        else
                        {
                            //m_ExitControl.listBoxList[Index].Items.Add((string)this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Value);
                            //this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.BackColor = Color.LightGray;
                            //this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.SelectionBackColor = Color.LightGray;
                            //this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.SelectionForeColor = Color.Black;
                            GlobalDataInterface.globalOut_GradeInfo.grades[m_GradedataGridViewSelectedCellList[i].rowIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + m_GradedataGridViewSelectedCellList[i].colIndex].exit |= ((long)1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 1));
                            IsSendFSM = true;
                            //if (GlobalDataInterface.global_IsTestMode)
                            //    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        }
                    }
                    if (GlobalDataInterface.global_IsTestMode && IsSendFSM == true)
                    {
                        //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        if (global_IsTest != 0) //add by xcw 20201211
                        {
                            MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                            LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        //GlobalDataInterface.TransmitParam(ConstPreDefine.SIM_ID, (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_GRADE_INFO, null);//更新到SIM
                        if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                    }
                    for (int i = 0; i < m_GradedataGridViewSelectedCellList.Count; i++)
                    {
                        if (m_ExitControl.listBoxList[Index].Items.Count > 0)
                        {
                            bool IsSame = false;
                            for (int j = 0; j < m_ExitControl.listBoxList[Index].Items.Count; j++)
                            {
                                if ((string)m_ExitControl.listBoxList[Index].Items[j] == (string)this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Value)
                                {
                                    this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.SelectionBackColor = Color.LightGray;
                                    this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.SelectionForeColor = Color.Black;
                                    IsSame = true;
                                    break;
                                }
                            }
                            if (!IsSame)
                            {
                                m_ExitControl.listBoxList[Index].Items.Add((string)this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Value);
                                this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.BackColor = Color.LightGray;
                                this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.SelectionBackColor = Color.LightGray;
                                this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.SelectionForeColor = Color.Black;
                                //GlobalDataInterface.globalOut_GradeInfo.grades[m_GradedataGridViewSelectedCellList[i].rowIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + m_GradedataGridViewSelectedCellList[i].colIndex].exit |= ((long)1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 1));
                                //IsSendFSM = true;
                                //if (GlobalDataInterface.global_IsTestMode)
                                //    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                            }
                        }
                        else
                        {
                            m_ExitControl.listBoxList[Index].Items.Add((string)this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Value);
                            this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.BackColor = Color.LightGray;
                            this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.SelectionBackColor = Color.LightGray;
                            this.GradedataGridView[m_GradedataGridViewSelectedCellList[i].colIndex, m_GradedataGridViewSelectedCellList[i].rowIndex].Style.SelectionForeColor = Color.Black;
                            //GlobalDataInterface.globalOut_GradeInfo.grades[m_GradedataGridViewSelectedCellList[i].rowIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + m_GradedataGridViewSelectedCellList[i].colIndex].exit |= ((long)1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 1));
                            //IsSendFSM = true;
                            //if (GlobalDataInterface.global_IsTestMode)
                            //    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        }
                    }
                    m_IsGradedataGridViewMouseDown = false;

                }
                //从其它列表拖曳
                if (m_IsExitlistBoxMouseDown)
                {
                    m_IsGradedataGridViewMouseDown = false;
                    m_IsExitlistBoxMouseDown = false;
                    bool IsSame = false;
                    if (GlobalDataInterface.nSampleOutlet == (Index + 1))      //往抽样出口拖动等级
                    {
                        if (m_ExitControl.listBoxList[Index].Items.Count == 2)
                            return;
                    }
                    string CurrentSelectGradeData = (string)e.Data.GetData(DataFormats.Text);
                    //if ((string)e.Data.GetData(DataFormats.Text) == "抽检口")  //“抽检口”往外拖动
                    if ((string)e.Data.GetData(DataFormats.Text) == m_resourceManager.GetString("SampleExitLabel.Text"))  //“抽检口”往外拖动
                    {
                        return;
                    }
                    if (m_ExitControl.listBoxList[Index].Items.Count > 0)
                    {
                        for (int i = 0; i < m_ExitControl.listBoxList[Index].Items.Count; i++)
                        {
                            if ((string)m_ExitControl.listBoxList[Index].Items[i] == (string)e.Data.GetData(DataFormats.Text))  //拖动相关两个出口等级相同
                            {
                                if (m_CurrentMouseDownlistBoxIndex != Index)  //一个出口拖到另一个出口
                                {
                                    IsSame = true;
                                    //return;
                                    break;
                                }
                                else //同一个出口拖动
                                {
                                    return;
                                }
                            }
                        }
                    }
                    if (IsSame)
                    {
                        //m_ExitControl.listBoxList[m_CurrentMouseDownlistBoxIndex].Items.Remove((string)e.Data.GetData(DataFormats.Text));
                        //m_IsExitlistBoxMouseDown = false;
                        for (int p = 0; p < GradedataGridView.RowCount; p++)
                        {
                            for (int q = 0; q < GradedataGridView.ColumnCount; q++)
                            {
                                if ((string)e.Data.GetData(DataFormats.Text) == (string)this.GradedataGridView[q, p].Value)
                                {
                                    GlobalDataInterface.globalOut_GradeInfo.grades[p * ConstPreDefine.MAX_SIZE_GRADE_NUM + q].exit |= ((long)1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 1));
                                    GlobalDataInterface.globalOut_GradeInfo.grades[p * ConstPreDefine.MAX_SIZE_GRADE_NUM + q].exit &= ~((long)1 << (int.Parse(m_ExitControl.labelList[m_CurrentMouseDownlistBoxIndex].Text) - 1));
                                    if (GlobalDataInterface.global_IsTestMode)
                                    {
                                        //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                                        int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                                        if (global_IsTest != 0) //add by xcw 20201211
                                        {
                                            MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                                            LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            return;
                                        }
                                        //GlobalDataInterface.TransmitParam(ConstPreDefine.SIM_ID, (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_GRADE_INFO, null);//更新到SIM
                                        if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                                    }
                                    m_ExitControl.listBoxList[m_CurrentMouseDownlistBoxIndex].Items.Remove((string)e.Data.GetData(DataFormats.Text));
                                    m_IsExitlistBoxMouseDown = false;
                                    return;
                                }
                            }
                        }
                       
                    }
                    else
                    {
                        //m_ExitControl.listBoxList[Index].Items.Add((string)e.Data.GetData(DataFormats.Text));
                        //m_ExitControl.listBoxList[m_CurrentMouseDownlistBoxIndex].Items.Remove((string)e.Data.GetData(DataFormats.Text));
                        //m_IsExitlistBoxMouseDown = false;
                        for (int p = 0; p < GradedataGridView.RowCount; p++)
                        {
                            for (int q = 0; q < GradedataGridView.ColumnCount; q++)
                            {
                                if ((string)e.Data.GetData(DataFormats.Text) == (string)this.GradedataGridView[q, p].Value)
                                {
                                    GlobalDataInterface.globalOut_GradeInfo.grades[p * ConstPreDefine.MAX_SIZE_GRADE_NUM + q].exit |= ((long)1 << (int.Parse(m_ExitControl.labelList[Index].Text) - 1));
                                    GlobalDataInterface.globalOut_GradeInfo.grades[p * ConstPreDefine.MAX_SIZE_GRADE_NUM + q].exit &= ~((long)1 << (int.Parse(m_ExitControl.labelList[m_CurrentMouseDownlistBoxIndex].Text) - 1));
                                    if (GlobalDataInterface.global_IsTestMode)
                                    {
                                        //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                                        int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                                        if (global_IsTest != 0) //add by xcw 20201211
                                        {
                                            MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                                            LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            return;
                                        }
                                        if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                                    }
                                    m_ExitControl.listBoxList[Index].Items.Add((string)e.Data.GetData(DataFormats.Text));
                                    m_ExitControl.listBoxList[m_CurrentMouseDownlistBoxIndex].Items.Remove((string)e.Data.GetData(DataFormats.Text));
                                    m_IsExitlistBoxMouseDown = false;
                                    return;
                                }
                            }
                        }
                       
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数ExitlistBox_DragDrop出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数ExitlistBox_DragDrop出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 出口列表拖出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void ExitlistBox_DragLeave(object sender, EventArgs e)
        //{
        //    ListBox listBox = (ListBox)sender;
        //    string Name = listBox.Name;
        //    int Index = m_ExitControl.listBoxList.IndexOf(listBox);
        //    if (!m_ExitControl.listBoxEnabledList[Index])
        //        return;
        //    if (m_ExitControl.listBoxList[Index].SelectedIndex >= 0)
        //    {
        //        m_ExitControl.listBoxList[Index].Items.RemoveAt(m_ExitControl.listBoxList[Index].SelectedIndex);
        //        for (int i = 0; i < this.GradedataGridView.RowCount; i++)
        //        {
        //            for (int j = 0; j < this.GradedataGridView.ColumnCount; j++)
        //            {
        //                if (m_ExitControl.listBoxList[Index].SelectedItem == this.GradedataGridView[j, i].Value)
        //                {
        //                    GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit &= ~((long)1 << Index);
        //                    return;
        //                }
        //            }
        //        }

        //    }
        //}
        /// <summary>
        /// 出口列表拖出等级条目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitContainer2_Panel1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                //for (int k = m_ExitControl.listBoxList[m_CurrentMouseDownlistBoxIndex].SelectedItems.Count - 1; k > -1; k--)
                //{
                //    bool jump = false;
                //    for (int i = 0; i < this.GradedataGridView.RowCount; i++)
                //    {
                //        for (int j = 0; j < this.GradedataGridView.ColumnCount; j++)
                //        {
                //            if ((string)m_ExitControl.listBoxList[m_CurrentMouseDownlistBoxIndex].SelectedItems[k] == (string)this.GradedataGridView[j, i].Value)
                //            {
                //                m_ExitControl.listBoxList[m_CurrentMouseDownlistBoxIndex].Items.Remove(m_ExitControl.listBoxList[m_CurrentMouseDownlistBoxIndex].SelectedItems[k]);
                //                GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit ^= ((long)1 << (int.Parse(m_ExitControl.labelList[m_CurrentMouseDownlistBoxIndex].Text) - 1));
                //                if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit == 0)
                //                {
                //                    this.GradedataGridView[j, i].Style.BackColor = Color.Pink;
                //                    this.GradedataGridView[j, i].Style.SelectionBackColor = Color.FromArgb(36, 155, 255);
                //                    this.GradedataGridView[j, i].Selected = false;
                //                }
                //                if (GlobalDataInterface.global_IsTestMode)
                //                    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                //                jump = true;
                //                break;
                //            }
                //            if (jump == true)
                //                break;
                //        }
                //    }

                //}
                //m_IsExitlistBoxMouseDown = false;
                if (m_ExitControl.listBoxList[m_CurrentMouseDownlistBoxIndex].SelectedIndex >= 0)
                {

                    for (int i = 0; i < this.GradedataGridView.RowCount; i++)
                    {
                        for (int j = 0; j < this.GradedataGridView.ColumnCount; j++)
                        {
                            if ((string)m_ExitControl.listBoxList[m_CurrentMouseDownlistBoxIndex].SelectedItem == (string)this.GradedataGridView[j, i].Value)
                            {
                                m_ExitControl.listBoxList[m_CurrentMouseDownlistBoxIndex].Items.RemoveAt(m_ExitControl.listBoxList[m_CurrentMouseDownlistBoxIndex].SelectedIndex);
                                GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit ^= ((long)1 << (int.Parse(m_ExitControl.labelList[m_CurrentMouseDownlistBoxIndex].Text) - 1));
                                //if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit == 0)
                                //{
                                //    this.GradedataGridView[j, i].Style.BackColor = Color.Pink;
                                //    this.GradedataGridView[j, i].Style.SelectionBackColor = Color.FromArgb(36, 155, 255);
                                //    this.GradedataGridView[j, i].Selected = false;
                                //}
                                if (GlobalDataInterface.global_IsTestMode)
                                {
                                    //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                                    int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                                    if (global_IsTest != 0) //add by xcw 20201211
                                    {
                                        MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                                        LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }
                                    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                                }
                                if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit == 0)
                                {
                                    this.GradedataGridView[j, i].Style.BackColor = Color.Pink;
                                    this.GradedataGridView[j, i].Style.SelectionBackColor = Color.FromArgb(36, 155, 255);
                                    this.GradedataGridView[j, i].Selected = false;
                                }
                                return;
                            }
                        }
                    }
                    m_IsExitlistBoxMouseDown = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数splitContainer2_Panel1_DragDrop出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数splitContainer2_Panel1_DragDrop出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 出口列表拖出等级条目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitContainer2_Panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (m_IsExitlistBoxMouseDown)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// 等级表格鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void GradedataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //#if DEBUG
            //            m_IsGradedataGridViewMouseDownDone = false;
            //#endif
            try
            {
                m_IsGradedataGridViewMouseDown = true;
                m_IsExitlistBoxMouseDown = false;


                DataGridView dataGridView = (DataGridView)sender;
                if (e.RowIndex == -1)
                {
                    if (e.ColumnIndex == -1) //表格全选
                    {
                        m_GradedataGridViewSelectedCellList.Clear();
                        for (int i = 0; i < this.GradedataGridView.RowCount; i++)
                        {
                            for (int j = 0; j < this.GradedataGridView.ColumnCount; j++)
                            {
                                GradedataGridViewSelectedCell cell;
                                cell.rowIndex = i;
                                cell.colIndex = j;
                                this.GradedataGridView[cell.colIndex, cell.rowIndex].Style.SelectionBackColor = Color.FromArgb(36, 155, 255);
                                this.GradedataGridView[cell.colIndex, cell.rowIndex].Style.SelectionForeColor = Color.White;
                                this.GradedataGridView[cell.colIndex, cell.rowIndex].Selected = true;
                                m_GradedataGridViewSelectedCellList.Add(cell);
                            }
                        }
                        this.GradedataGridView.DoDragDrop(m_GradedataGridViewSelectedCellList, DragDropEffects.Move);
                    }
                    else//整列全选
                    {
                        m_GradedataGridViewSelectedCellList.Clear();
                        int selectedCellCount = dataGridView.GetCellCount(DataGridViewElementStates.Selected);
                        if (selectedCellCount > 0)
                        {

                            for (int i = 0; i < selectedCellCount; i++)
                            {
                                GradedataGridViewSelectedCell cell;
                                cell.rowIndex = this.GradedataGridView.SelectedCells[0].RowIndex;
                                cell.colIndex = this.GradedataGridView.SelectedCells[0].ColumnIndex;
                                this.GradedataGridView[cell.colIndex, cell.rowIndex].Selected = false;
                            }
                        }
                        for (int i = 0; i < this.GradedataGridView.RowCount; i++)
                        {
                            GradedataGridViewSelectedCell cell;
                            cell.rowIndex = i;
                            cell.colIndex = e.ColumnIndex;
                            this.GradedataGridView[cell.colIndex, cell.rowIndex].Style.SelectionBackColor = Color.FromArgb(36, 155, 255);
                            this.GradedataGridView[cell.colIndex, cell.rowIndex].Style.SelectionForeColor = Color.White;
                            this.GradedataGridView[cell.colIndex, cell.rowIndex].Selected = true;
                            m_GradedataGridViewSelectedCellList.Add(cell);
                        }
                        this.GradedataGridView.DoDragDrop(m_GradedataGridViewSelectedCellList, DragDropEffects.Move);
                    }
                }
                else
                {
                    int selectedCellCount = dataGridView.GetCellCount(DataGridViewElementStates.Selected);
                    if (selectedCellCount > 0)
                    {
                        m_GradedataGridViewSelectedCellList.Clear();
                        for (int i = 0; i < selectedCellCount; i++)
                        {

                            GradedataGridViewSelectedCell cell;
                            cell.rowIndex = this.GradedataGridView.SelectedCells[i].RowIndex;
                            cell.colIndex = this.GradedataGridView.SelectedCells[i].ColumnIndex;
                            this.GradedataGridView[cell.colIndex, cell.rowIndex].Style.SelectionBackColor = Color.FromArgb(36, 155, 255);
                            this.GradedataGridView[cell.colIndex, cell.rowIndex].Style.SelectionForeColor = Color.White;
                            m_GradedataGridViewSelectedCellList.Add(cell);
                        }
                        this.GradedataGridView.DoDragDrop(m_GradedataGridViewSelectedCellList, DragDropEffects.Move);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数GradedataGridView_CellMouseDown出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数GradedataGridView_CellMouseDown出错" + ex);
#endif
            }
            //#if DEBUG
            //            m_IsGradedataGridViewMouseDownDone = true;
            //#endif
        }


        /// <summary>
        /// 设置主界面水果等级实时参数列表
        /// </summary>
        public void SetGradeSizelistViewEx()
        {
            try
            {
                //初始化列表头
                this.GradeSizelistViewEx.Clear();
                this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewName.Text"), 120, HorizontalAlignment.Center);
                //if (!(GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 1))//除了只有品质
                if (!(GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 0))//除了只有品质
                {
                    if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x03) > 0)//重量
                        this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewWeight.Text"), 60, HorizontalAlignment.Center);
                    else
                        this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewSize.Text"), 60, HorizontalAlignment.Center);
                }
                this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewNum.Text"), 60, HorizontalAlignment.Center);//等级个数
                this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewNumPercent.Text"), 80, HorizontalAlignment.Center);//个数百分比
                this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewWeightSum.Text"), 60, HorizontalAlignment.Center);//等级重量
                this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewWeightPercent.Text"), 80, HorizontalAlignment.Center);//重量百分比
                this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewBoxNumber.Text"), 60, HorizontalAlignment.Center);//箱数
                this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewBoxPercent.Text"), 80, HorizontalAlignment.Center);//箱数百分比
                if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x03) > 0)//重量
                {
                    if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x0001) > 0)
                        this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewTargetWeight.Text"), 100, HorizontalAlignment.Center);
                    else
                        this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewTargetNumber.Text"), 100, HorizontalAlignment.Center);
                }
                else if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x1C) > 0)
                {
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewTargetNumber.Text"), 100, HorizontalAlignment.Center);
                }
                if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x03) > 0)//重量
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewWeightPerBox.Text"), 100, HorizontalAlignment.Center);
                if (GlobalDataInterface.globalOut_GradeInfo.nLabelType == 1)//按等级贴标
                {
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewLabel.Text"), 60, HorizontalAlignment.Center);
                }

                //具体等级条目
                ListViewItem item;
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                string GradeName;
                ulong totalNumSum = 0;//所有等级的生产个数
                ulong totalWeightSum = 0;//所有等级的生产重量
                ulong totalBoxSum = 0;//所有等级的生产箱数
                int qulNum = 1;

                if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0)//品质与尺寸或者品质与重量
                {
                    qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                }

                for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                {
                    for (int i = 0; i < qulNum; i++)
                    {
                        for (int j = 0; j < GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum; j++)
                        {
                            totalNumSum += GlobalDataInterface.globalIn_statistics[k].nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                            totalWeightSum += GlobalDataInterface.globalIn_statistics[k].nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                            totalBoxSum += (ulong)GlobalDataInterface.globalIn_statistics[k].nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                        }
                    }
                }
                this.GradeSizelistViewEx.Items.Clear();
                qulNum = 1;
                //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 57) > 0 && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 7) > 0)//品质与尺寸或者品质与重量
                if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0)//品质与尺寸或者品质与重量
                {
                    qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                }
                int size = 1;
                if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                    size = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                for (int i = 0; i < qulNum; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        //等级名称
                        Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                        //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 57) > 0 && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 7) > 0)//品质与尺寸或者品质与重量
                        if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0)//品质与尺寸或者品质与重量
                        {
                            GradeName += "." + Encoding.Default.GetString(GlobalDataInterface.Quality_GradeInfo.Item[i].GradeName).TrimEnd('\0');
                        }
                        item = new ListViewItem(GradeName);
                        //最小重量或最小尺寸
                        if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nMinSize == 0x7f7f7f7f)
                            GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nMinSize = 0;
                        item.SubItems.Add(GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nMinSize.ToString());




                        //等级个数
                        ulong GradeCountSum = 0;
                        //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x03) > 0)//重量
                        //{
                        for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                        {
                            if (GlobalDataInterface.globalIn_statistics[k].nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] == 0x7f7f7f7f)
                                GlobalDataInterface.globalIn_statistics[k].nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] = 0;
                            GradeCountSum += GlobalDataInterface.globalIn_statistics[k].nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                        }
                        item.SubItems.Add(GradeCountSum.ToString());
                        //}

                        //等级个数百分比
                        float percent;
                        if (totalNumSum == 0)
                            percent = 0.00f;
                        else
                        {
                            percent = GradeCountSum / totalNumSum * 100.00f;
                        }
                        item.SubItems.Add(string.Format("{0:F}", percent));//保留两位小数

                        //总重量 需要将现有全部子系统同一等级的生产总数累加
                        ulong GradeWeightSum = 0;
                        for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                        {
                            if (GlobalDataInterface.globalIn_statistics[k].nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] == 0x7f7f7f7f)
                                GlobalDataInterface.globalIn_statistics[k].nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] = 0;
                            GradeWeightSum += GlobalDataInterface.globalIn_statistics[k].nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                        }
                        item.SubItems.Add(GradeWeightSum.ToString());

                        //等级重量百分比
                        if (totalWeightSum == 0)
                            percent = 0.00f;
                        else
                        {
                            percent = GradeWeightSum / totalWeightSum * 100.00f;
                        }
                        item.SubItems.Add(string.Format("{0:F}", percent));//保留两位小数


                        //箱数
                        ulong Sum = 0;
                        for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                        {
                            if (GlobalDataInterface.globalIn_statistics[k].nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] == 0x7f7f7f7f)
                                GlobalDataInterface.globalIn_statistics[k].nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] = 0;
                            Sum += (ulong)GlobalDataInterface.globalIn_statistics[k].nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                        }
                        item.SubItems.Add(Sum.ToString());

                        //等级箱数百分比
                        if (totalBoxSum == 0)
                            percent = 0.00f;
                        else
                        {
                            percent = Sum / totalBoxSum * 100.00f;
                        }
                        item.SubItems.Add(string.Format("{0:F}", percent));//保留两位小数

                        //装箱量
                        if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum == 0x7f7f7f7f)
                            GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum = 0;
                        item.SubItems.Add(GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum.ToString());

                        //每箱重
                        if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x03) > 0)//重量
                        {
                            for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                            {
                                Sum += (ulong)GlobalDataInterface.globalIn_statistics[k].nBoxGradeWeight[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                            }
                            item.SubItems.Add(Sum.ToString());
                        }
                        this.GradeSizelistViewEx.Items.Add(item);

                        //贴标
                        string str;
                        if (GlobalDataInterface.globalOut_GradeInfo.nLabelType == 1)//按等级贴标
                        {
                            if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbLabelbyGrade == 0x7f)
                                GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbLabelbyGrade = 0;
                            if (GlobalDataInterface.selectLanguage == "zh") //modify by xcw - 20200408
                            {
                                switch (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbLabelbyGrade)
                                {
                                    case 0:
                                        item.SubItems.Add("无");
                                        break;
                                    case 1:
                                        str = Commonfunction.GetAppSetting("贴标机1");
                                        if (str != "")
                                            item.SubItems.Add(str);
                                        else
                                            item.SubItems.Add("贴标1");
                                        break;
                                    case 2:
                                        str = Commonfunction.GetAppSetting("贴标机2");
                                        if (str != "")
                                            item.SubItems.Add(str);
                                        else
                                            item.SubItems.Add("贴标2");
                                        break;
                                    case 3:
                                        str = Commonfunction.GetAppSetting("贴标机3");
                                        if (str != "")
                                            item.SubItems.Add(str);
                                        else
                                            item.SubItems.Add("贴标3");
                                        break;
                                    case 4:
                                        str = Commonfunction.GetAppSetting("贴标机4");
                                        if (str != "")
                                            item.SubItems.Add(str);
                                        else
                                            item.SubItems.Add("贴标4");
                                        break;
                                    default: break;
                                }
                            }
                            else if (GlobalDataInterface.selectLanguage == "en")
                            {
                                switch (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbLabelbyGrade)
                                {
                                    case 0:
                                        item.SubItems.Add("None");
                                        break;
                                    case 1:
                                        str = Commonfunction.GetAppSetting("Labeling machine 1");
                                        if (str != "")
                                            item.SubItems.Add(str);
                                        else
                                            item.SubItems.Add("Label1");
                                        break;
                                    case 2:
                                        str = Commonfunction.GetAppSetting("Labeling machine 2");
                                        if (str != "")
                                            item.SubItems.Add(str);
                                        else
                                            item.SubItems.Add("Label2");
                                        break;
                                    case 3:
                                        str = Commonfunction.GetAppSetting("Labeling machine 3");
                                        if (str != "")
                                            item.SubItems.Add(str);
                                        else
                                            item.SubItems.Add("Label3");
                                        break;
                                    case 4:
                                        str = Commonfunction.GetAppSetting("Labeling machine 4");
                                        if (str != "")
                                            item.SubItems.Add(str);
                                        else
                                            item.SubItems.Add("Label4");
                                        break;
                                    default: break;
                                }
                            }
                            else if (GlobalDataInterface.selectLanguage == "es")
                            {
                                switch (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbLabelbyGrade)
                                {
                                    case 0:
                                        item.SubItems.Add("Ninguna");
                                        break;
                                    case 1:
                                        str = Commonfunction.GetAppSetting("Calcomanías1");
                                        if (str != "")
                                            item.SubItems.Add(str);
                                        else
                                            item.SubItems.Add("Etiquetado1");
                                        break;
                                    case 2:
                                        str = Commonfunction.GetAppSetting("Calcomanías2");
                                        if (str != "")
                                            item.SubItems.Add(str);
                                        else
                                            item.SubItems.Add("Etiquetado2");
                                        break;
                                    case 3:
                                        str = Commonfunction.GetAppSetting("Calcomanías3");
                                        if (str != "")
                                            item.SubItems.Add(str);
                                        else
                                            item.SubItems.Add("Etiquetado3");
                                        break;
                                    case 4:
                                        str = Commonfunction.GetAppSetting("Calcomanías4");
                                        if (str != "")
                                            item.SubItems.Add(str);
                                        else
                                            item.SubItems.Add("Etiquetado4");
                                        break;
                                    default: break;
                                }
                            }
                            //switch (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbLabelbyGrade)
                            //{
                            //    case 0:
                            //        item.SubItems.Add("无");
                            //        break;
                            //    case 1:
                            //        str = Commonfunction.GetAppSetting("贴标机1");
                            //        if (str != "")
                            //            item.SubItems.Add(str);
                            //        else
                            //            item.SubItems.Add("贴标1");
                            //        break;
                            //    case 2:
                            //        str = Commonfunction.GetAppSetting("贴标机2");
                            //        if (str != "")
                            //            item.SubItems.Add(str);
                            //        else
                            //            item.SubItems.Add("贴标2");
                            //        break;
                            //    case 3:
                            //        str = Commonfunction.GetAppSetting("贴标机3");
                            //        if (str != "")
                            //            item.SubItems.Add(str);
                            //        else
                            //            item.SubItems.Add("贴标3");
                            //        break;
                            //    case 4:
                            //        str = Commonfunction.GetAppSetting("贴标机4");
                            //        if (str != "")
                            //            item.SubItems.Add(str);
                            //        else
                            //            item.SubItems.Add("贴标4");
                            //        break;
                            //    default: break;
                            //}
                            ////item.SubItems[this.GradeSizelistViewEx.Columns.Count - 1].BackColor = Color.Pink;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数SetGradeSizelistViewEx出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数SetGradeSizelistViewEx出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 主界面水果等级实时参数列表右击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GradeSizelistViewEx_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                m_GradeSizelistViewExRightMouseDown = true;
        }
        /// <summary>
        /// 主界面水果等级实时参数列表右击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GradeSizelistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (m_GradeSizelistViewExRightMouseDown && GradeSizelistViewEx.Items.Count > 0)  //modify by ChengSk - 20171023 解决英文贴标“右键不显示”Bug modify by xcw - 20200408 解决西班牙语贴标“右键不显示”Bug
                {
                    this.LabelcontextMenuStrip.Items.Clear();
                    string str;
                    if (this.GradeSizelistViewEx.Columns[e.SubItem].Text == "贴标")//modify by ChengSk - 20171023 解决英文贴标“右键不显示”Bug   modify by xcw - 20200408 解决西班牙语贴标“右键不显示”Bug
                    {
                        str = Commonfunction.GetAppSetting("贴标机1");
                        if (str != "")
                            this.LabelcontextMenuStrip.Items.Add(str);
                        else
                            this.LabelcontextMenuStrip.Items.Add("贴标1");
                        str = Commonfunction.GetAppSetting("贴标机2");
                        if (str != "")
                            this.LabelcontextMenuStrip.Items.Add(str);
                        else
                            this.LabelcontextMenuStrip.Items.Add("贴标2");
                        str = Commonfunction.GetAppSetting("贴标机3");
                        if (str != "")
                            this.LabelcontextMenuStrip.Items.Add(str);
                        else
                            this.LabelcontextMenuStrip.Items.Add("贴标3");
                        str = Commonfunction.GetAppSetting("贴标机4");
                        if (str != "")
                            this.LabelcontextMenuStrip.Items.Add(str);
                        else
                            this.LabelcontextMenuStrip.Items.Add("贴标4");
                    }
                    else if (this.GradeSizelistViewEx.Columns[e.SubItem].Text == "Label")
                    {
                        str = Commonfunction.GetAppSetting("Labeling machine 1");
                        if (str != "")
                            this.LabelcontextMenuStrip.Items.Add(str);
                        else
                            this.LabelcontextMenuStrip.Items.Add("Label1");
                        str = Commonfunction.GetAppSetting("Labeling machine 2");
                        if (str != "")
                            this.LabelcontextMenuStrip.Items.Add(str);
                        else
                            this.LabelcontextMenuStrip.Items.Add("Label2");
                        str = Commonfunction.GetAppSetting("Labeling machine 3");
                        if (str != "")
                            this.LabelcontextMenuStrip.Items.Add(str);
                        else
                            this.LabelcontextMenuStrip.Items.Add("Label3");
                        str = Commonfunction.GetAppSetting("Labeling machine 4");
                        if (str != "")
                            this.LabelcontextMenuStrip.Items.Add(str);
                        else
                            this.LabelcontextMenuStrip.Items.Add("Label4");
                    }
                    else if (this.GradeSizelistViewEx.Columns[e.SubItem].Text == "Etiquetado")
                    {
                        str = Commonfunction.GetAppSetting("Calcomanías1");
                        if (str != "")
                            this.LabelcontextMenuStrip.Items.Add(str);
                        else
                            this.LabelcontextMenuStrip.Items.Add("Etiquetado1");
                        str = Commonfunction.GetAppSetting("Calcomanías2");
                        if (str != "")
                            this.LabelcontextMenuStrip.Items.Add(str);
                        else
                            this.LabelcontextMenuStrip.Items.Add("Etiquetado2");
                        str = Commonfunction.GetAppSetting("Calcomanías3");
                        if (str != "")
                            this.LabelcontextMenuStrip.Items.Add(str);
                        else
                            this.LabelcontextMenuStrip.Items.Add("Etiquetado3");
                        str = Commonfunction.GetAppSetting("Calcomanías4");
                        if (str != "")
                            this.LabelcontextMenuStrip.Items.Add(str);
                        else
                            this.LabelcontextMenuStrip.Items.Add("Etiquetado4");
                    }

                    //str = Commonfunction.GetAppSetting("贴标机1");
                    //if (str != "")
                    //    this.LabelcontextMenuStrip.Items.Add(str);
                    //else
                    //    this.LabelcontextMenuStrip.Items.Add("贴标1");
                    //str = Commonfunction.GetAppSetting("贴标机2");
                    //if (str != "")
                    //    this.LabelcontextMenuStrip.Items.Add(str);
                    //else
                    //    this.LabelcontextMenuStrip.Items.Add("贴标2");
                    //str = Commonfunction.GetAppSetting("贴标机3");
                    //if (str != "")
                    //    this.LabelcontextMenuStrip.Items.Add(str);
                    //else
                    //    this.LabelcontextMenuStrip.Items.Add("贴标3");
                    //str = Commonfunction.GetAppSetting("贴标机4");
                    //if (str != "")
                    //    this.LabelcontextMenuStrip.Items.Add(str);
                    //else
                    //    this.LabelcontextMenuStrip.Items.Add("贴标4");


                    ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
                    this.LabelcontextMenuStrip.Items.Add(toolStripSeparator);
                    if (this.GradeSizelistViewEx.Columns[e.SubItem].Text == "贴标")
                    {
                        this.LabelcontextMenuStrip.Items.Add("无");
                    }
                    else if (this.GradeSizelistViewEx.Columns[e.SubItem].Text == "Label")
                    {
                        this.LabelcontextMenuStrip.Items.Add("None");
                    }
                    else if (this.GradeSizelistViewEx.Columns[e.SubItem].Text == "Etiquetado")
                    {
                        this.LabelcontextMenuStrip.Items.Add("Ninguna");
                    }//modify by xcw - 20200408

                    for (int i = 0; i < this.LabelcontextMenuStrip.Items.Count; i++)
                        this.LabelcontextMenuStrip.Items[i].Click += new EventHandler(LabelcontextMenuStrip_Click);
                    Point point = new Point();
                    point.X = Cursor.Position.X;
                    point.Y = Cursor.Position.Y;
                    this.LabelcontextMenuStrip.Show(point);
                    m_GradeSizelistViewExRightMouseDownItemIndex = e.Item.Index;
                    m_GradeSizelistViewExRightMouseDownSubItemIndex = e.SubItem;
                    m_GradeSizelistViewExRightMouseDown = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数GradeSizelistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数GradeSizelistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 贴标右击菜单单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelcontextMenuStrip_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                {
                    int qualNum = 1;
                    if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//品质与尺寸或者品质与重量
                    {
                        qualNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                    }

                    ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                    if (m_GradeSizelistViewExRightMouseDownItemIndex >= 0 && m_GradeSizelistViewExRightMouseDownSubItemIndex >= 0)
                    {
                        //grades计算索引
                        int gradesIndex = 0;
                        gradesIndex = (m_GradeSizelistViewExRightMouseDownItemIndex / GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum) * ConstPreDefine.MAX_SIZE_GRADE_NUM +
                            (m_GradeSizelistViewExRightMouseDownItemIndex % GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum);

                        if (this.LabelcontextMenuStrip.Items.IndexOf(menuItem) == 5)
                            GlobalDataInterface.globalOut_GradeInfo.grades[gradesIndex].sbLabelbyGrade = 0;
                        else
                            GlobalDataInterface.globalOut_GradeInfo.grades[gradesIndex].sbLabelbyGrade = (sbyte)(this.LabelcontextMenuStrip.Items.IndexOf(menuItem) + 1);
                        //this.GradeSizelistViewEx.Items[m_GradeSizelistViewExRightMouseDownItemIndex].SubItems[m_GradeSizelistViewExRightMouseDownSubItemIndex].Text = menuItem.Text;
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                            int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                            if (global_IsTest != 0) //add by xcw 20201211
                            {
                                MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                                LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                        }
                        this.GradeSizelistViewEx.Items[m_GradeSizelistViewExRightMouseDownItemIndex].SubItems[m_GradeSizelistViewExRightMouseDownSubItemIndex].Text = menuItem.Text;

                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数LabelcontextMenuStrip_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数LabelcontextMenuStrip_Click出错" + ex);
#endif
            }
        }

        private void UpStatisticInfoThread()
        {
            bool bCreated;
            EventWaitHandle waitEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "UpStatics", out bCreated);
            while (true)
            {
                waitEvent.WaitOne();
                this.Invoke(new GlobalDataInterface.StatisticInfoEventHandler(OnUpStatisticInfoEvent));
                Thread.Sleep(1000);
            }
        }

        private void UpLoadDataThread()
        {
            bool bUpLoaded;
            EventWaitHandle waitEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "UpLoad", out bUpLoaded);
            while (true)
            {
                waitEvent.WaitOne();
                //this.Invoke(new MainForm.UpLoadDataEventHandler(OnUpLoadDataEvent));
                bool bContinue = true;
                int ErrorNum = 0;
                while (bContinue)
                {
                    int bFlag = OnUpLoadDataEvent();
                    switch (bFlag)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            bContinue = false;
                            break;
                        case 3:
                            ErrorNum++;
                            break;
                        default:
                            break;
                    }
                    if (ErrorNum >= 5)
                    {
                        bContinue = false;
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 执行上传事件
        /// </summary>
        /// <returns>0-失败，1-成功，2-停止，3-异常</returns>
        private int OnUpLoadDataEvent()
        {
            try
            {
                #region 获取服务器端加工时间

                if (!GlobalDataInterface.lstMacAddress.Contains(GlobalDataInterface.MacAddress.Replace("-", ":"))) //本机不包含配置文件中的MAC地址
                {
                    for (int i = 0; i < GlobalDataInterface.lstMacAddress.Count; i++)
                    {
                        log.WriteHttpLog("本地MAC列表：" + GlobalDataInterface.lstMacAddress[i]);
                    }
                    log.WriteHttpLog("文件配置MAC：" + GlobalDataInterface.MacAddress.Replace("-", ":"));

                    log.WriteHttpLog("==>设备" + GlobalDataInterface.DeviceNumber + "准备上传数据，配置文件中MAC地址无效，停止上传！");
                    return 2;
                }

                string strUrl = GlobalDataInterface.ServerURL + "RequestProcessingEndTime?DeviceNumber=" + GlobalDataInterface.DeviceNumber +
                    "&MacAddress=" + GlobalDataInterface.MacAddress;
                TimeoutWebClient _dbMyClient = new TimeoutWebClient(10000, new IPEndPoint(IPAddress.Parse(GlobalDataInterface.ServerBindLocalIP), 0));
                _dbMyClient.Encoding = System.Text.Encoding.UTF8;
                string result = _dbMyClient.DownloadString(strUrl);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                string reMessage = jo["message"].ToString();
                string reResult = jo["result"].ToString();
                string reStatus = jo["status"].ToString();
                string reStatusCode = jo["statusCode"].ToString();
                if (!(reMessage.Contains("查询成功") && reMessage.Contains(GlobalDataInterface.DeviceNumber)))
                {
                    log.WriteHttpLog("==>设备" + GlobalDataInterface.DeviceNumber + "查询服务器最新加工时间失败，[Message]" + reMessage + "；");
                    return 2;   //未正常返回信息，停止下列操作
                }
                log.WriteHttpLog("==>设备" + GlobalDataInterface.DeviceNumber + "查询到服务器最新加工时间：" + reResult + "；");
                #endregion

                #region 上传数据

                string UploadStartTime = GlobalDataInterface.UploadStartTime;
                if (UploadStartTime.CompareTo(reResult) == -1)   //更新为最近时间
                {
                    UploadStartTime = reResult;
                    GlobalDataInterface.UploadStartTime = reResult;
                }
                log.WriteHttpLog("上传起始时间设定：" + UploadStartTime + "；");
                DataSet dst1 = databaseOperation.GetFruitTop1ByEndTime(UploadStartTime);
                if (dst1.Tables[0].Rows.Count == 0)
                {
                    return 2;   //没有最新的加工数据，停止以下操作 
                }
                int CustomerID = int.Parse(dst1.Tables[0].Rows[0]["CustomerID"].ToString()); //客户ID号
                int ExportNum = int.Parse(dst1.Tables[0].Rows[0]["ExportSum"].ToString());   //出口数量
                int GradeNum = 0;
                if (int.Parse(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString()) == 0)
                {
                    GradeNum = int.Parse(dst1.Tables[0].Rows[0]["WeightOrSizeGradeSum"].ToString());
                }
                else
                {
                    GradeNum = int.Parse(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString()) * int.Parse(dst1.Tables[0].Rows[0]["WeightOrSizeGradeSum"].ToString()); //等级数量
                }

                DataSet dst2 = databaseOperation.GetGradeByCustomerID(CustomerID);
                if (GradeNum != dst2.Tables[0].Rows.Count)
                {
                    GlobalDataInterface.UploadStartTime = dst1.Tables[0].Rows[0]["EndTime"].ToString();
                    //GlobalDataInterface.WriteErrorInfo("**上传时等级数量不符**，CusteomerID：" + CustomerID.ToString() +
                    //    "，应有等级：" + GradeNum.ToString() + "，实际等级：" + dst2.Tables[0].Rows.Count.ToString());
                    log.WriteHttpLog("MM上传时等级数量不符，CusteomerID：" + CustomerID.ToString() +
                        "，应有等级：" + GradeNum.ToString() + "，实际等级：" + dst2.Tables[0].Rows.Count.ToString() + "；");
                    return 0;   //当前批次的等级数量与数据库不一致，停止以下操作
                }
                DataSet dst3 = databaseOperation.GetExportByCustomerID(CustomerID);
                if (ExportNum != dst3.Tables[0].Rows.Count)
                {
                    GlobalDataInterface.UploadStartTime = dst1.Tables[0].Rows[0]["EndTime"].ToString();
                    //GlobalDataInterface.WriteErrorInfo("**上传时出口数量不符**，CusteomerID：" + CustomerID.ToString() +
                    //    "，应有出口：" + ExportNum.ToString() + "，实际出口：" + dst3.Tables[0].Rows.Count.ToString());
                    log.WriteHttpLog("MM上传时出口数量不符，CusteomerID：" + CustomerID.ToString() +
                        "，应有出口：" + ExportNum.ToString() + "，实际出口：" + dst3.Tables[0].Rows.Count.ToString() + "；");
                    return 0;   //当前批次的出口数量与数据库不一致，停止以下操作
                }

                FruitProcessingInfo fpInfo = new FruitProcessingInfo();
                fpInfo.DeviceNumber = GlobalDataInterface.DeviceNumber;
                fpInfo.t1_CustomerID = CustomerID.ToString();
                fpInfo.t1_CustomerName = dst1.Tables[0].Rows[0]["CustomerName"].ToString();
                fpInfo.t1_FarmName = dst1.Tables[0].Rows[0]["FarmName"].ToString();
                fpInfo.t1_FruitName = dst1.Tables[0].Rows[0]["FruitName"].ToString();
                fpInfo.t1_StartTime = dst1.Tables[0].Rows[0]["StartTime"].ToString();
                fpInfo.t1_EndTime = dst1.Tables[0].Rows[0]["EndTime"].ToString();
                fpInfo.t1_StartedState = dst1.Tables[0].Rows[0]["StartedState"].ToString();
                fpInfo.t1_CompletedState = dst1.Tables[0].Rows[0]["CompletedState"].ToString();
                fpInfo.t1_BatchWeight = dst1.Tables[0].Rows[0]["BatchWeight"].ToString();
                fpInfo.t1_BatchNumber = dst1.Tables[0].Rows[0]["BatchNumber"].ToString();
                fpInfo.t1_QualityGradeSum = dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString();
                fpInfo.t1_WeightOrSizeGradeNum = dst1.Tables[0].Rows[0]["WeightOrSizeGradeSum"].ToString();
                fpInfo.t1_ExportSum = dst1.Tables[0].Rows[0]["ExportSum"].ToString();
                fpInfo.t1_ColorGradeName = dst1.Tables[0].Rows[0]["ColorGradeName"].ToString();
                fpInfo.t1_ShapeGradeName = dst1.Tables[0].Rows[0]["ShapeGradeName"].ToString();
                fpInfo.t1_FlawGradeName = dst1.Tables[0].Rows[0]["FlawGradeName"].ToString();
                fpInfo.t1_HardGradeName = dst1.Tables[0].Rows[0]["HardGradeName"].ToString();
                fpInfo.t1_DensityGradeName = dst1.Tables[0].Rows[0]["DensityGradeName"].ToString();
                fpInfo.t1_SugarDegreeGradeName = dst1.Tables[0].Rows[0]["SugarDegreeGradeName"].ToString();
                fpInfo.t1_ProgramName = dst1.Tables[0].Rows[0]["ProgramName"].ToString();
                for (int i = 0; i < GradeNum; i++)
                {
                    fpInfo.t2_GradeID.Add(dst2.Tables[0].Rows[i]["GradeID"].ToString());
                    fpInfo.t2_BoxNumber.Add(dst2.Tables[0].Rows[i]["BoxNumber"].ToString());
                    fpInfo.t2_FruitNumber.Add(dst2.Tables[0].Rows[i]["FruitNumber"].ToString());
                    fpInfo.t2_FruitWeight.Add(dst2.Tables[0].Rows[i]["FruitWeight"].ToString());
                    fpInfo.t2_QualityName.Add(dst2.Tables[0].Rows[i]["QualityName"].ToString());
                    fpInfo.t2_WeightOrSizeName.Add(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString());
                    fpInfo.t2_WeightOrSizeLimit.Add(dst2.Tables[0].Rows[i]["WeightOrSizeLimit"].ToString());
                    fpInfo.t2_SelectWeightOrSize.Add(dst2.Tables[0].Rows[i]["SelectWeightOrSize"].ToString());
                    fpInfo.t2_TraitWeightOrSize.Add(dst2.Tables[0].Rows[i]["TraitWeightOrSize"].ToString());
                    fpInfo.t2_TraitColor.Add(dst2.Tables[0].Rows[i]["TraitColor"].ToString());
                    fpInfo.t2_TraitShape.Add(dst2.Tables[0].Rows[i]["TraitShape"].ToString());
                    fpInfo.t2_TraitFlaw.Add(dst2.Tables[0].Rows[i]["TraitFlaw"].ToString());
                    fpInfo.t2_TraitHard.Add(dst2.Tables[0].Rows[i]["TraitHard"].ToString());
                    fpInfo.t2_TraitDensity.Add(dst2.Tables[0].Rows[i]["TraitDensity"].ToString());
                    fpInfo.t2_TraitSugarDegree.Add(dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString());
                }
                for (int i = 0; i < ExportNum; i++)
                {
                    fpInfo.t3_ExportID.Add(dst3.Tables[0].Rows[i]["ExportID"].ToString());
                    fpInfo.t3_FruitNumber.Add(dst3.Tables[0].Rows[i]["FruitNumber"].ToString());
                    fpInfo.t3_FruitWeight.Add(dst3.Tables[0].Rows[i]["FruitWeight"].ToString());
                }

                string strData = Commonfunction.getJsonByObject(fpInfo);

                //strUrl = GlobalDataInterface.ServerURL + "UpLoadFruitProcessingInfo?data=" + strData;
                //_dbMyClient = new TimeoutWebClient(10000, new IPEndPoint(IPAddress.Parse(GlobalDataInterface.ServerBindLocalIP), 0));
                //_dbMyClient.Encoding = System.Text.Encoding.UTF8;
                //result = _dbMyClient.DownloadString(strUrl);

                strUrl = GlobalDataInterface.ServerURL + "UpLoadFruitProcessingInfo?data=";
                result = HttpHelper.OpenReadWithHttps(strUrl, strData, 10000, new IPEndPoint(IPAddress.Parse(GlobalDataInterface.ServerBindLocalIP), 0));
                jo = (JObject)JsonConvert.DeserializeObject(result);
                reMessage = jo["message"].ToString();
                reResult = jo["result"].ToString();
                reStatus = jo["status"].ToString();
                reStatusCode = jo["statusCode"].ToString();
                if (reMessage.Contains("插入成功") && reMessage.Contains(GlobalDataInterface.DeviceNumber))
                {
                    GlobalDataInterface.UploadStartTime = dst1.Tables[0].Rows[0]["EndTime"].ToString();
                    log.WriteHttpLog("OO上传数据成功，等级个数：" + GradeNum.ToString() + "，出口个数：" + ExportNum.ToString() + "，当前批次加工时间：" +
                        dst1.Tables[0].Rows[0]["EndTime"].ToString() + "；");
                    return 1;
                }
                else
                {
                    log.WriteHttpLog("II上传数据失败，等级个数：" + GradeNum.ToString() + "，出口个数：" + ExportNum.ToString() + "，当前批次加工时间：" +
                        dst1.Tables[0].Rows[0]["EndTime"].ToString() + "；");
                    return 2;
                }
                #endregion
            }
            catch (Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数OnUpLoadDataEvent出错" + ex);
#endif
                //log.WriteHttpLog("XX上传数据出现异常！！！");
                return 3;
            }
        }

        int secondSum = 1;  //计时，分选效率表要求300s计时一次

        /// <summary>
        /// 上行水果统计信息刷新
        /// </summary>
        /// <param name="statistic"></param>
        private void OnUpStatisticInfoEvent()
        {
            int q = -1;//获取出口等级拖动异常时的出口Index
            try
            {
                this.TimetoolStripLabel.Text = string.Format("{0:T}", DateTime.Now);

                //   if (this == Form.ActiveForm)//是否操作当前窗体
                //if (true)//是否操作当前窗体
                {
                    //if (this.InvokeRequired)
                    //{
                    //    this.BeginInvoke(new GlobalDataInterface.StatisticInfoEventHandler(OnUpStatisticInfoEvent));
                    //}
                    //else
                    //{

                    if (GlobalDataInterface.gGradeInterfaceFresh)
                    {
                        // this.SystemtoolStripStatusLabel.Refresh();
                        //刷新主界面水果等级实时参数列表
                        //if (((this.GradeSizelistViewEx.Items.Count == GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum) && ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 1) == 0))
                        //    || ((this.GradeSizelistViewEx.Items.Count == GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum * GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum) && ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 1) > 0)))//判断当前数据对应
                        if (((this.GradeSizelistViewEx.Items.Count == GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum) && (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum == 0))   //只有尺寸或重量，没有品质
                            || ((this.GradeSizelistViewEx.Items.Count == GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum * GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum) && (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)))//判断当前数据对应  （重量+品质）或（尺寸+品质）
                        {

                            //byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                            //string GradeName;

                            int tempQualityGradeNum = 1;
                            int tempSizeGradeNum = 1;
                            if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)
                                tempQualityGradeNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;   //品质个数
                            if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                                tempSizeGradeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;         //尺寸个数（尺寸或重量）
                            //ulong totalNumSum = 0;//所有等级的生产个数
                            //ulong totalWeightSum = 0;//所有等级的生产重量
                            //ulong totalBoxSum = 0;//所有等级的生产箱数

                            ulong uNumSumValue = FunctionInterface.GetSumValue(GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount);//所有等级的生产个数
                            ulong uWeightSumValue = FunctionInterface.GetSumValue(GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount);//所有等级的生产重量
                                                                                                                                                      // ulong uBoxSumValue = (ulong)FunctionInterface.GetSumValue(GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount);//所有等级的生产箱数


                            //for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                            //{
                            //    for (int i = 0; i < tempQualityGradeNum; i++)
                            //    {
                            //        for (int j = 0; j < tempSizeGradeNum; j++)
                            //        {
                            //            totalNumSum += GlobalDataInterface.globalIn_statistics[k].nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                            //            totalWeightSum += GlobalDataInterface.globalIn_statistics[k].nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                            //            //totalBoxSum += (ulong)GlobalDataInterface.globalIn_statistics[k].nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];

                            //        }
                            //    }
                            //}



                            int qulNum = 1;
                            //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 57) > 0 && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 7) > 0)//品质与尺寸或者品质与重量
                            if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//（品质与尺寸）或者（品质与重量）
                            {
                                qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                            }
                            int sizeNum = 1;
                            if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                                sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;

                            //计算总箱数
                            ulong uSum = 0;
                            ulong uBoxSumValue = 0;
                            for (int i = 0; i < qulNum; i++)
                            {
                                for (int j = 0; j < sizeNum; j++)
                                {
                                    uSum = 0;
                                    for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                                    {
                                        if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x01) > 0)//重量
                                        {
                                            uSum += GlobalDataInterface.globalIn_statistics[k].nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                                        }
                                        else
                                        {
                                            uSum += GlobalDataInterface.globalIn_statistics[k].nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                                        }

                                    }
                                    if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum <= 0)
                                        continue;
                                    uBoxSumValue += uSum / ((ulong)GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum);
                                }
                            }

                            //for (int i = 0; i < qulNum; i++)
                            //{
                            for (int j = 0; j < sizeNum; j++)
                            {
                                //等级名称
                                //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 57) > 0 && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 7) > 0)//品质与尺寸或者品质与重量
                                //if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//品质与尺寸或者品质与重量
                                //{
                                //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                //    GradeName = Encoding.Default.GetString(temp).TrimEnd('\0') + "." + Encoding.Default.GetString(GlobalDataInterface.Quality_GradeInfo.Item[i].GradeName).TrimEnd('\0');
                                //}
                                //else
                                //{
                                //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                //    GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                                //}
                                //this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[0].Text = GradeName;
                                ////最小重量或最小尺寸
                                //this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[1].Text = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nMinSize.ToString();

                                //等级个数
                                // ulong GradeCountSum = 0;
                                //for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                                //{
                                //    GradeCountSum += GlobalDataInterface.globalIn_statistics[k].nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                                //}
                                this.GradeSizelistViewEx.Items[staticCount * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[2].Text = GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[staticCount * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString();
                                //}

                                //等级个数百分比
                                float percent;
                                if (uNumSumValue == 0)
                                    percent = 0.00f;
                                else
                                {
                                    percent = GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[staticCount * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] * 1.0f / uNumSumValue * 100.00f;
                                }
                                this.GradeSizelistViewEx.Items[staticCount * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[3].Text = string.Format("{0:F}", percent);//保留两位小数

                                //总重量 需要将现有全部子系统同一等级的生产总数累加
                                //ulong WeightGradeCountSum = 0;
                                //for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                                //{
                                //    WeightGradeCountSum += GlobalDataInterface.globalIn_statistics[k].nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                                //}
                                this.GradeSizelistViewEx.Items[staticCount * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[4].Text = GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[staticCount * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString();

                                //等级重量百分比
                                if (uWeightSumValue == 0)
                                    percent = 0.00f;
                                else
                                {
                                    percent = GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[staticCount * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] * 1.0f / uWeightSumValue * 100.00f;
                                }
                                this.GradeSizelistViewEx.Items[staticCount * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[5].Text = string.Format("{0:F}", percent);//保留两位小数


                                //箱数
                                ulong Sum = 0;
                                if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x01) > 0)//重量(克)
                                {
                                    if (GlobalDataInterface.globalOut_GradeInfo.grades[staticCount * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum > 0)
                                        Sum = GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[staticCount * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / ((ulong)GlobalDataInterface.globalOut_GradeInfo.grades[staticCount * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum);
                                }
                                else
                                {
                                    if (GlobalDataInterface.globalOut_GradeInfo.grades[staticCount * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum > 0)
                                        Sum = GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[staticCount * ConstPreDefine.MAX_SIZE_GRADE_NUM + j] / ((ulong)GlobalDataInterface.globalOut_GradeInfo.grades[staticCount * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum);
                                }

                                //for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                                //{
                                //    Sum += (ulong)GlobalDataInterface.globalIn_statistics[k].nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                                //}
                                this.GradeSizelistViewEx.Items[staticCount * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[6].Text = Sum.ToString();
                                //等级箱数百分比
                                if (uBoxSumValue == 0)
                                    percent = 0.00f;
                                else
                                {
                                    percent = Sum * 1.0f / uBoxSumValue * 100.00f;
                                }
                                this.GradeSizelistViewEx.Items[staticCount * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[7].Text = string.Format("{0:F}", percent);//保留两位小数
                                                                                                                                                                                           ////装箱量
                                                                                                                                                                                           //this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[8].Text = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum.ToString();

                                //}
                                //else
                                //{
                                //    this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[4].Text = Sum.ToString();
                                //    //装箱量
                                //    this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[5].Text = GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum.ToString();
                                //}
                                //每箱重
                                //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x06) > 0)//重量
                                Sum = 0;
                                if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x03) > 0)//重量
                                {
                                    //Note by ChengSk - 2017/11/16
                                    //for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                                    //{
                                    //    Sum += (ulong)GlobalDataInterface.globalIn_statistics[k].nBoxGradeWeight[staticCount * ConstPreDefine.MAX_SIZE_GRADE_NUM + j];
                                    //}
                                    //this.GradeSizelistViewEx.Items[staticCount * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[9].Text = Sum.ToString();
                                    //Modify by ChengSk - 2017/11/16
                                    this.GradeSizelistViewEx.Items[staticCount * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[9].Text =
                                        GlobalDataInterface.globalIn_statistics[GlobalDataInterface.nCurrentStatisticsSubsysId].nBoxGradeWeight[staticCount * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].ToString();
                                }
                                int SubItemsCount = this.GradeSizelistViewEx.Items[staticCount * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems.Count;
                                ////贴标
                                //string str;
                                //if (GlobalDataInterface.globalOut_GradeInfo.nLabelType == 1)//按等级贴标
                                //{
                                //    switch (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbLabelbyGrade)
                                //    {
                                //        case 0:
                                //            this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[SubItemsCount - 1].Text = "无";
                                //            break;
                                //        case 1:
                                //            str = Commonfunction.GetAppSetting("贴标机1");
                                //            if (str != "")
                                //                this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[SubItemsCount - 1].Text = str;
                                //            else
                                //                this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[SubItemsCount - 1].Text = "贴标1";
                                //            break;
                                //        case 2:
                                //            str = Commonfunction.GetAppSetting("贴标机2");
                                //            if (str != "")
                                //                this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[SubItemsCount - 1].Text = str;
                                //            else
                                //                this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[SubItemsCount - 1].Text = "贴标2";
                                //            break;
                                //        case 3:
                                //            str = Commonfunction.GetAppSetting("贴标机3");
                                //            if (str != "")
                                //                this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[SubItemsCount - 1].Text = str;
                                //            else
                                //                this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[SubItemsCount - 1].Text = "贴标3";
                                //            break;
                                //        case 4:
                                //            str = Commonfunction.GetAppSetting("贴标机4");
                                //            if (str != "")
                                //                this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[SubItemsCount - 1].Text = str;
                                //            else
                                //                this.GradeSizelistViewEx.Items[i * GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum + j].SubItems[SubItemsCount - 1].Text = "贴标4";
                                //            break;
                                //        default: break;
                                //    }
                                //}


                            }
                            staticCount++;
                            if (staticCount >= qulNum) //将小于号改成大于号 Update by ChengSk - 2017/07/26
                                staticCount = 0;

                            // }
                        }
                    }

                    //if (m_GradeNum > 0)
                    //{
                    //    if ((tempGradeInfo.nClassifyType & 0x06) > 0)//重量
                    //    {
                    //        for (int i = 0; i < m_GradeNum; i++)
                    //        {
                    //            int Sum = 0;
                    //            for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                    //            {
                    //                Sum += GlobalDataInterface.globalIn_statistics[k].nBoxGradeWeight[i];
                    //            }
                    //            this.GradeSizelistViewEx.Items[i].SubItems[3].Text = Sum.ToString();//有疑问
                    //        }
                    //    }
                    //}

                    //刷新状态栏
                    //检测子系统重量整定 2015-6-25 ivycc
                    bool bNormalSortingFlags = false;    //正常分选标志
                    //int a = (GlobalDataInterface.globalOut_SysConfig.nClassificationInfo >> 2) % 2;
                    //int b = GlobalDataInterface.globalOut_SysConfig.nClassificationInfo >> 2;
                    switch (GlobalDataInterface.globalOut_SysConfig.nSubsysNum)
                    {
                        case 1:
                            if (GlobalDataInterface.globalIn_statistics[0].nWeightSetting == 1)
                            {
                                //if(GlobalDataInterface.globalIn_statistics[0].nIntervalSumperminute == 0)
                                //{
                                //    this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("StopSortinglabel.Text");
                                //    this.MainstatusStrip.Items[0].BackColor = Color.Yellow;
                                //}
                                //else
                                //{
                                //    this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("NoramlSortinglabel.Text");
                                //    this.MainstatusStrip.Items[0].BackColor = Color.Green;
                                //}
                                bNormalSortingFlags = true;
                                this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("NoramlSortinglabel.Text");
                                this.MainstatusStrip.Items[0].BackColor = Color.Green;
                            }
                            else
                            {
                                bNormalSortingFlags = false;
                                this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("StandardSettinglabel.Text");
                                this.MainstatusStrip.Items[0].BackColor = Color.Red;
                            }
                            break;
                        case 2:
                            if (GlobalDataInterface.globalIn_statistics[0].nWeightSetting == 1 && GlobalDataInterface.globalIn_statistics[1].nWeightSetting == 1)
                            {
                                bNormalSortingFlags = true;
                                this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("NoramlSortinglabel.Text");
                                this.MainstatusStrip.Items[0].BackColor = Color.Green;
                            }
                            else
                            {
                                this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("StandardSettinglabel.Text");
                                this.MainstatusStrip.Items[0].BackColor = Color.Red;
                            }
                            break;
                        case 3:
                            if (GlobalDataInterface.globalIn_statistics[0].nWeightSetting == 1 && GlobalDataInterface.globalIn_statistics[1].nWeightSetting == 1
                                && GlobalDataInterface.globalIn_statistics[2].nWeightSetting == 1)
                            {
                                bNormalSortingFlags = true;
                                this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("NoramlSortinglabel.Text");
                                this.MainstatusStrip.Items[0].BackColor = Color.Green;
                            }
                            else
                            {
                                this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("StandardSettinglabel.Text");
                                this.MainstatusStrip.Items[0].BackColor = Color.Red;
                            }
                            break;
                        case 4:
                            if (GlobalDataInterface.globalIn_statistics[0].nWeightSetting == 1 && GlobalDataInterface.globalIn_statistics[1].nWeightSetting == 1
                               && GlobalDataInterface.globalIn_statistics[2].nWeightSetting == 1 && GlobalDataInterface.globalIn_statistics[3].nWeightSetting == 1)
                            {
                                bNormalSortingFlags = true;
                                this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("NoramlSortinglabel.Text");
                                this.MainstatusStrip.Items[0].BackColor = Color.Green;
                            }
                            else
                            {
                                this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("StandardSettinglabel.Text");
                                this.MainstatusStrip.Items[0].BackColor = Color.Red;
                            }
                            break;
                        default: break;
                    }
                    ////标定完毕后正常分选，而速度为零，此时为设备停机状态 //Add by ChengSk - 20180728
                    //if (bNormalSortingFlags && (GlobalDataInterface.globalIn_statistics[0].nIntervalSumperminute == 0))
                    //{
                    //    this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("StopSortinglabel.Text");
                    //    this.MainstatusStrip.Items[0].BackColor = Color.Yellow;
                    //}
                    //标定完毕后正常分选，而速度为零，此时为设备停机状态 //Add by ChengSk - 20180728
                    if (bNormalSortingFlags)//true代表nWeightSetting=1
                    {
                        if (GlobalDataInterface.globalIn_statistics[0].nIntervalSumperminute == 0)
                        {
                            this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("StopSortinglabel.Text");
                            this.MainstatusStrip.Items[0].BackColor = Color.Yellow;
                        }
                        else
                        {
                            this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("NoramlSortinglabel.Text");
                            this.MainstatusStrip.Items[0].BackColor = Color.Green;
                        }
                    }
                    else
                    {
                        this.MainstatusStrip.Items[0].Text = m_resourceManager.GetString("StandardSettinglabel.Text");
                        this.MainstatusStrip.Items[0].BackColor = Color.Red;
                    }

                    int preCupNum = 0;
                    int preIPMNum = 0;
                    int preIQSNum = 0; //Add by ChengSk - 20181126
                    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++)
                    {
                        if (GlobalDataInterface.globalIn_statistics[i].nSCMState == 1)
                        {
                            this.MainstatusStrip.Items[2 + i].BackColor = Color.Red;
                        }
                        else
                        {
                            this.MainstatusStrip.Items[2 + i].BackColor = Color.Green;
                        }
                    }
                    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++)
                    {
                        for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                            if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                            {
                                //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 6) != 0)
                                if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x03) > 0)
                                {
                                    //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                                    if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                                    {
                                        if (((GlobalDataInterface.globalIn_statistics[i].nCupState >> j) & 1) == 1)
                                        {
                                            this.MainstatusStrip.Items[GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 3 + preCupNum].BackColor = Color.Red;
                                            this.复位果杯toolStripMenuItem.Enabled = true;
                                        }
                                        else
                                        {
                                            this.MainstatusStrip.Items[GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 3 + preCupNum].BackColor = Color.Green;
                                        }
                                        preCupNum++;
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++) //Add by ChengSk - 20181126
                    {
                        for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                            if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                            {
                                if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) > 0) //有内部品质
                                {
                                    if (((GlobalDataInterface.globalIn_statistics[i].nIQSNetState >> j) & 1) == 1)
                                    {
                                        this.MainstatusStrip.Items[GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 4 + preCupNum + preIQSNum].BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        this.MainstatusStrip.Items[GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 4 + preCupNum + preIQSNum].BackColor = Color.Green;
                                    }
                                    preIQSNum++;
                                }
                            }
                        }
                    } //Add End

                    if (GlobalDataInterface.nVer == 0)
                    {
                        if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 1) > 0)
                        {
                            for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++)
                            {
                                for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                                {

                                    //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                                    if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                                    {
                                        //if (j % 2 == 0)
                                        //{
                                        if (((GlobalDataInterface.globalIn_statistics[i].nNetState >> j) & 1) == 1)
                                        {
                                            this.MainstatusStrip.Items[GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 5 + preCupNum + preIQSNum + preIPMNum].BackColor = Color.Red;
                                        }
                                        else
                                        {
                                            this.MainstatusStrip.Items[GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 5 + preCupNum + preIQSNum + preIPMNum].BackColor = Color.Green;
                                        }
                                        preIPMNum++;
                                        //}
                                    }

                                }
                            }
                        }
                    }
                    else if (GlobalDataInterface.nVer == 1)
                    {
                        if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 1) > 0)
                        {
                            for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++)
                            {

                                int ChannalNum = GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i];
                                int IPMNum = (ChannalNum + 1) / 2;
                                for (int k = 0; k < IPMNum; k++)
                                {
                                    //if ((GlobalDataInterface.globalIn_statistics[i].nNetState & (0x01 << k)) == 1)
                                    if (((GlobalDataInterface.globalIn_statistics[i].nNetState >>k )&1) == 1)
                                    {
                                        this.MainstatusStrip.Items[GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 5 + preCupNum + preIQSNum + preIPMNum].BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        this.MainstatusStrip.Items[GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 5 + preCupNum + preIQSNum + preIPMNum].BackColor = Color.Green;
                                    }
                                    preIPMNum++;
                                }


                                //for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                                //{
                                //    if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //add by xcw - 20200616
                                //    {
                                //        if (j % 2 == 0)
                                //        {
                                //            if (((GlobalDataInterface.globalIn_statistics[i].nNetState >> (j / 2)) & 1) == 1)
                                //            {
                                //                this.MainstatusStrip.Items[GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 5 + preCupNum + preIQSNum + preIPMNum].BackColor = Color.Red;
                                //            }
                                //            else
                                //            {
                                //                this.MainstatusStrip.Items[GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 5 + preCupNum + preIQSNum + preIPMNum].BackColor = Color.Green;
                                //            }
                                //            preIPMNum++;
                                //        }
                                //    }
                                //}



                            }
                        }
                        
                    }
                    //if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 1) > 0)
                    //{
                    //    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++)
                    //    {
                    //        for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    //        {
                    //            //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                    //            if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                    //            {
                    //                //if (j % 2 == 0)
                    //                //{
                    //                if (((GlobalDataInterface.globalIn_statistics[i].nNetState >> j) & 1) == 1)
                    //                {
                    //                    this.MainstatusStrip.Items[GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 5 + preCupNum + preIQSNum + preIPMNum].BackColor = Color.Red;
                    //                }
                    //                else
                    //                {
                    //                    this.MainstatusStrip.Items[GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 5 + preCupNum + preIQSNum + preIPMNum].BackColor = Color.Green;
                    //                }
                    //                preIPMNum++;
                    //                //}
                    //            }

                    //        }
                    //    }
                    //}

                    //刷新出口面板的数据显示
                    ulong lSum = 0;
                    uint ValidSubsysNum = 0;
                    ulong lVelChangelabelValue = 0;
                    //分选速度
                    for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                    {
                        if (GlobalDataInterface.globalIn_statistics[k].nIntervalSumperminute > 0)
                        {
                            lSum += (ulong)GlobalDataInterface.globalIn_statistics[k].nIntervalSumperminute;
                            ValidSubsysNum++;
                        }
                    }
                    if (ValidSubsysNum != 0)
                        lVelChangelabelValue = lSum / ValidSubsysNum;
                    this.VelChangelabel.Text = lVelChangelabelValue.ToString();

                    int IntervalSum = 0;  //Add by ChengSk - 20181130
                    uint IntervalValidSubsysNum = 0;
                    int IntervalChangelabelValue = 0;
                    //光电速度
                    for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                    {
                        if (GlobalDataInterface.globalIn_statistics[k].nInterval > 0)
                        {
                            IntervalSum += GlobalDataInterface.globalIn_statistics[k].nPulseInterval;
                            IntervalValidSubsysNum++;
                        }
                    }
                    if (IntervalValidSubsysNum != 0)
                        IntervalChangelabelValue = (int)(IntervalSum / IntervalValidSubsysNum);
                    this.IntervalChangelabel.Text = IntervalChangelabelValue.ToString();//End Add

                    try
                    {
                        #region 分选日志&分选效率
                        if (GlobalDataInterface.usedSeparationLogFlags)
                        {
                            DateTime dt = DateTime.Now;

                            if (dt.ToString("HH:mm:ss") == "23:59:59" && lVelChangelabelValue > 0 && RunningTimeInfoStopTimeIsEmptyFlags)
                            {
                                DataSet dst = databaseOperation.GetRunningTimeInfoByStopTime("");
                                databaseOperation.UpdateRunningStopTime(int.Parse(dst.Tables[0].Rows[0]["ID"].ToString()), "23:59:59");
                                RunningTimeInfoStopTimeIsEmptyFlags = false;
                            }
                            else
                            {
                                if (lVelChangelabelValue > 0) //大于零
                                {
                                    if (RunningTimeInfoStopTimeIsEmptyFlags == false)
                                    {
                                        databaseOperation.InsertRunningTimeInfo(dt.ToString("yyyy-MM-dd"), dt.ToString("HH:mm:ss"), "");
                                        RunningTimeInfoStopTimeIsEmptyFlags = true;

                                        databaseOperation.InsertSeparationEfficiencyInfo("0.00", dt.ToString("yyyy-MM-dd"), dt.ToString("HH:mm:ss"));
                                    }
                                }
                                else //等于零
                                {
                                    if (RunningTimeInfoStopTimeIsEmptyFlags == true)
                                    {
                                        DataSet dst = databaseOperation.GetRunningTimeInfoByStopTime("");
                                        databaseOperation.UpdateRunningStopTime(int.Parse(dst.Tables[0].Rows[0]["ID"].ToString()), dt.ToString("HH:mm:ss"));
                                        RunningTimeInfoStopTimeIsEmptyFlags = false;
                                    }
                                }
                            }

                            if ((secondSum++) % 300 == 0 && lVelChangelabelValue > 0)
                            {
                                secondSum = 1;
                                float SeparationEfficiencyLabelValue = GetSeparationEfficiency();
                                double SeparationEfficiencyValue = (double)SeparationEfficiencyLabelValue / 100;
                                databaseOperation.InsertSeparationEfficiencyInfo(SeparationEfficiencyValue.ToString(), dt.ToString("yyyy-MM-dd"), dt.ToString("HH:mm:ss"));
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)  //Add by ChengSk - 20180702
                    {
                        Trace.WriteLine("MainForm中函数块&分选日志&分选效率&出错" + ex + "\n" + ex.StackTrace);
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo("MainForm中函数块&分选日志&分选效率&出错" + ex + "\n" + ex.StackTrace);
#endif
                    }

                    //本批个数
                    ulong TotalCount = 0;
                    ulong AllExitCount = 0;        //计算出口柱状图 Add by ChengSk - 20190702
                    ulong AllExitWeightCount = 0;  //计算出口柱状图 Add by ChengSk - 20190702
                    for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                    {
                        TotalCount += GlobalDataInterface.globalIn_statistics[k].nTotalCount;

                        for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM; i++)
                        {
                            AllExitCount += GlobalDataInterface.globalIn_statistics[k].nExitCount[i];
                            AllExitWeightCount += GlobalDataInterface.globalIn_statistics[k].nExitWeightCount[i];
                        }
                    }
                    this.BatchNumChangelabel.Text = TotalCount.ToString();
                    if (m_preTotalCount == 0)
                        m_preTotalCount = TotalCount;
                    //开始时间
                    if (this.StartTimeChangelabel.Text == "" || m_ClearZero)
                    {
                        if (TotalCount >= 100)
                        {
                            if (this.StartTimeChangelabel.Text == "")
                            {
                                this.Staticstimer.Enabled = true;
                                DataSet dstTemp = databaseOperation.GetFruitByCompletedState(); //Modify by ChengSk - 20181212
                                if (dstTemp != null && dstTemp.Tables[0].Rows.Count > 0)
                                {
                                    string[] startTimes = dstTemp.Tables[0].Rows[0]["StartTime"].ToString().Split(' ');
                                    this.StartTimeChangelabel.Text = startTimes[1];
                                }
                                else
                                {
                                    this.StartTimeChangelabel.Text = DateTime.Now.ToLongTimeString().ToString();
                                }
                            }
                            m_ClearZero = false;
                        }
                    }

                    //本批重量
                    ulong WeightCount = 0;
                    if (this.WeightCountlabelChangelabel.Visible == true)
                    {
                        for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                        {
                            WeightCount += GlobalDataInterface.globalIn_statistics[k].nWeightCount;
                        }
                        this.WeightCountlabelChangelabel.Text = (WeightCount * 1.0f / 1000000).ToString("0.000");
                        if (m_preWeightCount == 0)
                            m_preWeightCount = WeightCount * 1.0f / 1000000;
                    }


                    //平均重量
                    if (this.AverWeightCountChangelabel.Visible == true)
                    {
                        lSum = 0;
                        if (ulong.Parse(this.BatchNumChangelabel.Text) != 0)
                            this.AverWeightCountChangelabel.Text = (WeightCount * 1.0f / (ulong.Parse(this.BatchNumChangelabel.Text))).ToString("0.00");
                    }
                    lSum = 0;
                    //果杯数
                    if (m_preTotalCupNum == 0)
                    {
                        for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                        {
                            int ChannelNum = 0;
                            for (int i = 0; i < ConstPreDefine.MAX_CHANNEL_NUM; i++)
                            {
                                //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[k * ConstPreDefine.MAX_CHANNEL_NUM + i] == 1)
                                if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[k] > i)  //Modify by ChengSk - 20190521
                                    ChannelNum++;
                            }
                            if (GlobalDataInterface.globalIn_statistics[k].nTotalCount != 0)
                            {
                                lSum += (ulong)(GlobalDataInterface.globalIn_statistics[k].nTotalCupNum * ChannelNum);
                            }
                        }
                        m_preTotalCupNum = lSum;
                    }

                    //刷新出口
                    if (GlobalDataInterface.gGradeInterfaceFresh)
                    {
                        for (q = 0; q < m_ExitControl.listBoxList.Count; q++)
                        {
                            lSum = 0;
                            ulong lExitSum = 0; //Add by ChengSk - 20180202
                            for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                            {
                                //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 6) == 0)//没有重量
                                if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x03) == 0)//没有重量
                                {
                                    lSum += GlobalDataInterface.globalIn_statistics[k].nExitCount[int.Parse(m_ExitControl.labelList[q].Text) - 1];
                                    lExitSum += GlobalDataInterface.globalIn_statistics[k].nExitCount[int.Parse(m_ExitControl.labelList[q].Text) - 1];
                                }
                                else
                                {
                                    lSum += GlobalDataInterface.globalIn_statistics[k].nExitWeightCount[int.Parse(m_ExitControl.labelList[q].Text) - 1];
                                    lExitSum += GlobalDataInterface.globalIn_statistics[k].nExitCount[int.Parse(m_ExitControl.labelList[q].Text) - 1];
                                }
                            }
                            //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 6) == 0)
                            if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x03) == 0)
                            {
                                if (TotalCount != 0)
                                {
                                    //m_ExitControl.listBoxList[q].FruitRadio = lSum * 1.0f / TotalCount;
                                    m_ExitControl.listBoxList[q].FruitRadio = lSum * 1.0f / AllExitCount; //Modify by ChengSk - 20190702 
                                    if (m_ExitControl.listBoxList[q].FruitRadio > 0.0f && m_ExitControl.listBoxList[q].FruitRadio < 0.1f)
                                        m_ExitControl.listBoxList[q].FruitRadio = 0.1f;
                                }
                                else
                                    m_ExitControl.listBoxList[q].FruitRadio = 0.0f;

                                if (m_ExitSortingStatisticDic[q].Count < 60) //小于1min的时候不刷新
                                {
                                    m_ExitSortingStatisticDic[q].Enqueue(lSum.ToString());
                                    m_AllSortingStatisticQueue.Enqueue(TotalCount.ToString());
                                }
                                else
                                {
                                    m_ExitSortingStatisticDic[q].Enqueue(lSum.ToString());
                                    m_AllSortingStatisticQueue.Enqueue(TotalCount.ToString());
                                    ulong oldlSum = uint.Parse(m_ExitSortingStatisticDic[q].Dequeue());
                                    ulong oldTotalCount = uint.Parse(m_AllSortingStatisticQueue.Dequeue());
                                    if (TotalCount - oldTotalCount > 0) //除数不为零
                                    {
                                        float alarmRadio = ((float)(lSum - oldlSum)) / (TotalCount - oldTotalCount);
                                        if (alarmRadio > GlobalDataInterface.fAlarmRatioThreshold)
                                        {
                                            if (m_ExitControl.listBoxList[q].FruitRadioColor != Color.HotPink)
                                                m_ExitControl.listBoxList[q].FruitRadioColor = Color.HotPink;
                                        }
                                        else
                                        {
                                            if (m_ExitControl.listBoxList[q].FruitRadioColor != Color.MediumSpringGreen)
                                                m_ExitControl.listBoxList[q].FruitRadioColor = Color.MediumSpringGreen;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (WeightCount != 0)
                                {
                                    //m_ExitControl.listBoxList[q].FruitRadio = lSum * 1.0f / WeightCount;
                                    m_ExitControl.listBoxList[q].FruitRadio = lSum * 1.0f / AllExitWeightCount;  //Modify by ChengSk - 20190702
                                    if (m_ExitControl.listBoxList[q].FruitRadio > 0.0f && m_ExitControl.listBoxList[q].FruitRadio < 0.1f)
                                        m_ExitControl.listBoxList[q].FruitRadio = 0.1f;
                                }
                                else
                                    m_ExitControl.listBoxList[q].FruitRadio = 0.0f;

                                if (m_ExitSortingStatisticDic[q].Count < 60) //小于1min的时候不刷新
                                {
                                    m_ExitSortingStatisticDic[q].Enqueue(lSum.ToString());
                                    m_AllSortingStatisticQueue.Enqueue(WeightCount.ToString());
                                }
                                else
                                {
                                    m_ExitSortingStatisticDic[q].Enqueue(lSum.ToString());
                                    m_AllSortingStatisticQueue.Enqueue(WeightCount.ToString());
                                    ulong oldlSum = uint.Parse(m_ExitSortingStatisticDic[q].Dequeue());
                                    ulong oldWeightCount = uint.Parse(m_AllSortingStatisticQueue.Dequeue());
                                    if (WeightCount - oldWeightCount > 0) //除数不为零
                                    {
                                        float alarmRadio = ((float)(lSum - oldlSum)) / (WeightCount - oldWeightCount);
                                        if (alarmRadio > GlobalDataInterface.fAlarmRatioThreshold)
                                        {
                                            if (m_ExitControl.listBoxList[q].FruitRadioColor != Color.HotPink)
                                                m_ExitControl.listBoxList[q].FruitRadioColor = Color.HotPink;
                                        }
                                        else
                                        {
                                            if (m_ExitControl.listBoxList[q].FruitRadioColor != Color.MediumSpringGreen)
                                                m_ExitControl.listBoxList[q].FruitRadioColor = Color.MediumSpringGreen;
                                        }
                                    }
                                }
                            } //End If
                            int immidiatenum;
                            if (GlobalDataInterface.nSampleOutlet == int.Parse(m_ExitControl.labelList[q].Text.Trim()))
                            {
                                if ((lExitSum - GlobalDataInterface.uCurrentSampleExitFruitTotals) >= (ulong)GlobalDataInterface.nSampleNumber)
                                {
                                    for (int num = m_ExitControl.listBoxList[q].Items.Count - 1; num >= 1; num--)
                                    {
                                        for (int i = 0; i < GradedataGridView.RowCount; i++)
                                        {
                                            for (int j = 0; j < GradedataGridView.ColumnCount; j++)
                                            {
                                                if (m_ExitControl.listBoxList[q].Items[num].ToString() == (string)this.GradedataGridView[j, i].Value)
                                                {
                                                    GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit &= ~((long)1 << (GlobalDataInterface.nSampleOutlet - 1));
                                                    if (GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit == 0)
                                                    {
                                                        this.GradedataGridView[j, i].Style.BackColor = Color.Pink;
                                                        this.GradedataGridView[j, i].Style.SelectionBackColor = Color.FromArgb(36, 155, 255);
                                                        this.GradedataGridView[j, i].Selected = false;
                                                    }
                                                }
                                            }
                                        }
                                        m_ExitControl.listBoxList[q].Items.RemoveAt(num);  //add by xcw 20200530
                                        //GlobalDataInterface.TransmitParam(ConstPreDefine.SIM_ID, (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_GRADE_INFO, null);//更新到SIM
                                    }
                                    //m_ExitControl.listBoxList[q].Items.RemoveAt(1);
                                    if (GlobalDataInterface.global_IsTestMode)
                                    {
                                        //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                                        int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                                        if (global_IsTest != 0) //add by xcw 20201211
                                        {
                                            MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                                            LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            return;
                                        }
                                        if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                                    }
                                    immidiatenum = (int)(lExitSum - GlobalDataInterface.uCurrentSampleExitFruitTotals);
                                    if (immidiatenum != 0 && GlobalDataInterface.global_SIMTest)
                                    {
                                        GlobalDataInterface.TransmitParamData(ConstPreDefine.SIM_ID, (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_DISPLAY_OUTLETS, immidiatenum);// HMI给SIM发送抽检完毕命令（无参）
                                    }
                                    bIsEndSendData = false;
                                    GlobalDataInterface.uCurrentSampleExitFruitTotals = lExitSum;
                                    if (immidiatenum>=GlobalDataInterface.nSampleNumber) //add by xcw 20200928
                                    {
                                        GlobalDataInterface.TransmitParam(ConstPreDefine.SIM_ID, (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_INSPECTION_OVER, null);// HMI给SIM发送抽检完毕命令（无参）
                                    }
                                }
                                else
                                {
                                    immidiatenum = (int)(lExitSum - GlobalDataInterface.uCurrentSampleExitFruitTotals);
                                    if (immidiatenum != 0 && bIsEndSendData && GlobalDataInterface.global_SIMTest)
                                    {
                                        GlobalDataInterface.TransmitParamData(ConstPreDefine.SIM_ID, (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_DISPLAY_OUTLETS, immidiatenum);// HMI给SIM发送抽检完毕命令（无参）
                                        if (immidiatenum >= GlobalDataInterface.nSampleNumber) //add by xcw 20200928
                                        {
                                            GlobalDataInterface.TransmitParam(ConstPreDefine.SIM_ID, (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_INSPECTION_OVER, null);// HMI给SIM发送抽检完毕命令（无参）
                                        }
                                    }
                                }

                            } //抽检任务已完成
                            //GlobalDataInterface.uCurrentSampleExitFruitTotals = lExitSum;

                        }
                        //m_ExitControl.listBoxList[0].FruitRadio = 0.5f;
                        //m_ExitControl.listBoxList[0].FruitRadioColor = Color.Green;
                    }
                }

            }

            catch (IndexOutOfRangeException ex)
            {
                //MessageBox.Show("Drag quickly throw an exception!");
                //MessageBox.Show(LanguageContainer.MainFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex] + ex + "\n" + ex.StackTrace,
                //    LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex], 
                //    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Trace.WriteLine("MainForm中函数OnUpStatisticInfoEvent出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数OnUpStatisticInfoEvent出错" + ex.StackTrace + "\n" + ex.ToString());
#endif

                if (q > -1)
                {
                    m_ExitControl.listBoxList[q].Dispose();
                    TransParentListBox.TransParentListBox listBox = new TransParentListBox.TransParentListBox();
                    listBox.Font = new Font("宋体", 9);
                    listBox.ForeColor = System.Drawing.Color.Black;
                    listBox.Name = "ExitlistBox" + GlobalDataInterface.ExitList[q].Index.ToString();
                    listBox.Size = new System.Drawing.Size(m_ExitControl.labelList[q].Size.Width, ExitListHeight);
                    listBox.DoubleClick += new System.EventHandler(this.ExitlistBox_DoubleClick);
                    listBox.SelectionMode = SelectionMode.MultiExtended;
                    //listBox.BackColor = Color.Transparent;


                    if (m_bExitEnable[GlobalDataInterface.ExitList[q].Index - 1])
                    {
                        listBox.BackColor = Color.White;
                        listBox.AllowDrop = true;
                    }
                    else
                    {
                        listBox.BackColor = Color.LightGray;
                        listBox.AllowDrop = false;
                    }
                    listBox.Location = new Point(m_ExitControl.labelList[q].Location.X, m_ExitControl.labelList[q].Location.Y + m_ExitControl.labelList[q].Height);
                    listBox.DragEnter += new DragEventHandler(ExitlistBox_DragEnter);
                    listBox.DragDrop += new DragEventHandler(ExitlistBox_DragDrop);
                    listBox.MouseDown += new MouseEventHandler(ExitlistBox_MouseDown);
                    listBox.MouseHover += new EventHandler(ExitlistBox_MouseHover);

                    this.splitContainer2.Panel1.Controls.Add(listBox);
                    m_ExitControl.listBoxList.Insert(q, listBox);

                    SetExitListBox(q, int.Parse(m_ExitControl.labelList[q].Text) - 1);
                    //InitExitListBox(true);//初始化出口
                    //SetAllExitListBox();//初始化出口中等级
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数OnUpStatisticInfoEvent出错" + ex);
                //int i = ex.StackTrace.IndexOf("line");
                //string s = ex.StackTrace.Substring(i + 3);
                //i = s.IndexOf(' ');
                //if(i!=-1)
                //{
                //    s = s.Substring(0, i);
                //}
                //if (Convert.ToInt32(s) == 2546)//
                //{
                //    int A = 1;
                //}
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数OnUpStatisticInfoEvent出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 物理时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCurrenttimer_Tick(object sender, EventArgs e)
        {
            this.TimetoolStripLabel.Text = string.Format("{0:T}", DateTime.Now);
            //if (DateTime.Now.Second < 10)
            //    this.TimetoolStripLabel.Text = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":0" + DateTime.Now.Second.ToString();
            //else
            //    this.TimetoolStripLabel.Text = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
        }

        /// <summary>
        /// 设置主界面状态栏
        /// </summary>
        public void SetMainstatusStrip()
        {
            try
            {
                this.MainstatusStrip.Items.Clear();

                int totalChannelNum = 0;
                int interval = 0;
                int[] IPMNum = new int[4];
                int[] IQSNum = new int[4];  //Add 20181123

                ToolStripStatusLabel label;
                label = new ToolStripStatusLabel();
                label.AutoSize = false;
                label.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
                label.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
                label.DoubleClickEnabled = true;
                label.Name = "SystemtoolStripStatusLabel";
                label.Size = new System.Drawing.Size(100 * MainstatusStrip.Width / 1360, 17);
                label.Text = m_resourceManager.GetString("SystemtoolStripStatusLabel.Text");
                label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                this.MainstatusStrip.Items.Insert(0, label);

                label = new ToolStripStatusLabel();
                label.AutoSize = false;
                label.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
                label.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
                label.DoubleClickEnabled = true;
                label.Name = "SCMtoolStripStatusLabel";
                label.Size = new System.Drawing.Size(35 * MainstatusStrip.Width / 1360, 17);
                label.Text = "SCM:";
                label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                this.MainstatusStrip.Items.Insert(1, label);

                for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++)
                {
                    label = new ToolStripStatusLabel();
                    label.AutoSize = false;
                    label.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
                    label.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
                    label.DoubleClickEnabled = true;
                    //label.Name = "SCMtoolStripStatusLabel";
                    label.Size = new System.Drawing.Size(15 * MainstatusStrip.Width / 1360, 17);
                    label.Text = String.Format("{0}", i + 1);
                    label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    this.MainstatusStrip.Items.Insert(i + 2, label);

                    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    {
                        //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                        if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                        {
                            totalChannelNum++;
                            IPMNum[i]++;
                            IQSNum[i]++; //Add 20181123
                        }
                    }
                }



                if (totalChannelNum > 0)
                {
                    //试验果杯状态显示栏
                    //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 6) != 0)
                    if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x03) != 0) //有重量时
                    {
                        label = new ToolStripStatusLabel();
                        label.AutoSize = false;
                        label.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
                        label.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
                        label.Name = "CuptoolStripStatusLabel";
                        label.Size = new System.Drawing.Size(35 * MainstatusStrip.Width / 1360, 17);
                        label.Text = m_resourceManager.GetString("CuptoolStripStatusLabel.Text");
                        label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                        this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 2, label);
                        this.CuptoolStripStatusLabel.BorderSides = ToolStripStatusLabelBorderSides.None;
                        interval = (275 * MainstatusStrip.Width / 1360) / totalChannelNum;   //720->275
                        for (int i = 0; i < totalChannelNum; i++)
                        {
                            label = new ToolStripStatusLabel();
                            label.AutoSize = false;
                            label.Size = new Size(interval, this.CuptoolStripStatusLabel.Height);
                            label.Text = string.Format("{0}", i + 1);
                            label.TextAlign = ContentAlignment.MiddleCenter;
                            label.BackColor = Color.Green;
                            label.BorderSides = ToolStripStatusLabelBorderSides.Right;
                            label.BorderStyle = Border3DStyle.RaisedInner;
                            label.Visible = true;
                            label.DoubleClickEnabled = true;
                            this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 3 + i, label);
                            // this.ToolStripStatusLabel.a
                            label.DoubleClick += new EventHandler(MainstatusStripCupLabel_DoubleClick);
                        }

                        if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) > 0) //有内部品质
                        {
                            label = new ToolStripStatusLabel();
                            label.Size = new System.Drawing.Size(35, 17);
                            label.AutoSize = false;
                            label.Name = "IQStoolStripStatusLabel";
                            label.Text = "IQS:";
                            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + totalChannelNum + 3, label);

                            int IQSTotalNum = 0;
                            for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                                IQSTotalNum += IQSNum[i];
                            interval = (410 * MainstatusStrip.Width / 1360) / IQSTotalNum;
                            for (int i = 0; i < IQSTotalNum; i++)
                            {
                                label = new ToolStripStatusLabel();
                                label.AutoSize = false;
                                label.Size = new Size(interval, this.CuptoolStripStatusLabel.Height);
                                label.Text = string.Format("{0}", i + 1);
                                label.TextAlign = ContentAlignment.MiddleCenter;
                                label.BackColor = Color.Green;
                                label.BorderSides = ToolStripStatusLabelBorderSides.Right;
                                label.BorderStyle = Border3DStyle.RaisedInner;
                                label.Visible = true;
                                this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + totalChannelNum + 4 + i, label);
                            }

                            label = new ToolStripStatusLabel();
                            label.Size = new System.Drawing.Size(35, 17);
                            label.AutoSize = false;
                            label.Name = "IPMtoolStripStatusLabel";
                            label.Text = "IPM:";
                            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + totalChannelNum + IQSTotalNum + 4, label);
                        }
                        else //无内部品质
                        {
                            label = new ToolStripStatusLabel();
                            label.Size = new System.Drawing.Size(445, 17);
                            label.AutoSize = false;
                            label.Name = "IQStoolStripStatusLabel";
                            label.Text = "IQS:";
                            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + totalChannelNum + 3, label);

                            label = new ToolStripStatusLabel();
                            label.Size = new System.Drawing.Size(35, 17);
                            label.AutoSize = false;
                            label.Name = "IPMtoolStripStatusLabel";
                            label.Text = "IPM:";
                            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + totalChannelNum + 4, label);
                        }

                        //if (totalChannelNum > 0)
                        //{
                        //    label = new ToolStripStatusLabel();
                        //    label.Size = new System.Drawing.Size(35, 17);
                        //}
                        //else
                        //{
                        //    //label = new ToolStripStatusLabel();
                        //    //label.Size = new System.Drawing.Size(460, 17); //冗余代码，肯定不会进来
                        //}
                        //label.AutoSize = false;
                        //label.Name = "IPMtoolStripStatusLabel";

                        //label.Text = "IPM:";
                        //label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                        //this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + totalChannelNum + 3, label);
                    }
                    else
                    {
                        label = new ToolStripStatusLabel();
                        label.AutoSize = false;
                        label.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
                        label.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
                        label.Name = "CuptoolStripStatusLabel";
                        label.Size = new System.Drawing.Size(310 * MainstatusStrip.Width / 1360, 17); //760->310
                        label.Text = m_resourceManager.GetString("CuptoolStripStatusLabel.Text"); ;
                        label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                        this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 2, label);

                        if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) > 0) //有内部品质
                        {
                            label = new ToolStripStatusLabel();
                            label.Size = new System.Drawing.Size(35, 17);
                            label.AutoSize = false;
                            label.Name = "IQStoolStripStatusLabel";
                            label.Text = "IQS:";
                            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 3, label);

                            int IQSTotalNum = 0;
                            for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                                IQSTotalNum += IQSNum[i];
                            interval = (410 * MainstatusStrip.Width / 1360) / IQSTotalNum;
                            for (int i = 0; i < IQSTotalNum; i++)
                            {
                                label = new ToolStripStatusLabel();
                                label.AutoSize = false;
                                label.Size = new Size(interval, this.CuptoolStripStatusLabel.Height);
                                label.Text = string.Format("{0}", i + 1);
                                label.TextAlign = ContentAlignment.MiddleCenter;
                                label.BackColor = Color.Green;
                                label.BorderSides = ToolStripStatusLabelBorderSides.Right;
                                label.BorderStyle = Border3DStyle.RaisedInner;
                                label.Visible = true;
                                this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 4 + i, label);
                            }

                            label = new ToolStripStatusLabel();
                            label.Size = new System.Drawing.Size(35, 17);
                            label.AutoSize = false;
                            label.Name = "IPMtoolStripStatusLabel";
                            label.Text = "IPM:";
                            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + IQSTotalNum + 4, label);
                        }
                        else //无内部品质
                        {
                            label = new ToolStripStatusLabel();
                            label.Size = new System.Drawing.Size(445, 17);
                            label.AutoSize = false;
                            label.Name = "IQStoolStripStatusLabel";
                            label.Text = "IQS:";
                            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 3, label);

                            label = new ToolStripStatusLabel();
                            label.Size = new System.Drawing.Size(35, 17);
                            label.AutoSize = false;
                            label.Name = "IPMtoolStripStatusLabel";
                            label.Text = "IPM:";
                            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 4, label);
                        }

                        //if (totalChannelNum > 0)
                        //{
                        //    label = new ToolStripStatusLabel();
                        //    label.Size = new System.Drawing.Size(35 * MainstatusStrip.Width / 1360, 17);
                        //}
                        //else
                        //{
                        //    //label = new ToolStripStatusLabel();
                        //    //label.Size = new System.Drawing.Size(400 * MainstatusStrip.Width / 1360, 17); //冗余代码，肯定不会进来
                        //}
                        //label.AutoSize = false;
                        //label.Name = "IPMtoolStripStatusLabel";

                        //label.Text = "IPM:";
                        //label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                        //this.MainstatusStrip.Items.Insert(GlobalDataInterface.globalOut_SysConfig.nSubsysNum + 3, label);
                    }

                    //if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 1) > 0)//视觉情况下有IPM
                    int IPMTotalNum = 0;
                    //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                    //    IPMTotalNum += IPMNum[i];
                    if (GlobalDataInterface.nVer == 0)
                    {
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                            IPMTotalNum += IPMNum[i];
                    }
                    else if (GlobalDataInterface.nVer == 1)
                    {
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                            IPMTotalNum += IPMNum[i];
                        IPMTotalNum = (IPMTotalNum + 1) / 2;
                    }
                    if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x01) == 1)//CIV视觉下有IPM
                    {
                        interval = (int)((410 * MainstatusStrip.Width / 1360) / (IPMTotalNum + 1));
                        for (int i = 0; i < IPMTotalNum; i++)
                        {
                            label = new ToolStripStatusLabel();
                            label.AutoSize = false;
                            label.Size = new Size(interval, this.CuptoolStripStatusLabel.Height);
                            label.Text = string.Format("{0}", i + 1);
                            label.TextAlign = ContentAlignment.MiddleCenter;
                            label.BackColor = Color.Green;
                            label.Visible = true;
                            label.BorderSides = ToolStripStatusLabelBorderSides.Right;
                            label.BorderStyle = Border3DStyle.RaisedInner;
                            this.MainstatusStrip.Items.Add(label);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数SetMainstatusStrip出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数SetMainstatusStrip出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置主界面出口面板的数据显示
        /// </summary>
        public void SetQaulitytoolStripButtonEnabled()
        {
            try
            {
                //if (((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 2) > 0) && ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x06) > 0))
                //if (((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x04) > 0) && ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x03) > 0))
                //{

                //    this.WeightCountlabel.Visible = true;
                //    this.WeightCountlabelChangelabel.Visible = true;
                //    this.WeightCountUnitlabel.Visible = true;
                //    this.AverWeightCountlabel.Visible = true;
                //    this.AverWeightCountChangelabel.Visible = true;
                //    this.AverWeightCountUnitlabel.Visible = true;
                //    this.RealWeightCountlabel.Visible = true;
                //    this.RealWeightCountChangelabel.Visible = true;
                //    this.RealWeightCountUnitlabel.Visible = true;
                //}
                //else
                //{
                //    this.WeightCountlabel.Visible = false;
                //    this.WeightCountlabelChangelabel.Visible = false;
                //    this.WeightCountUnitlabel.Visible = false;
                //    this.AverWeightCountlabel.Visible = false;
                //    this.AverWeightCountChangelabel.Visible = false;
                //    this.AverWeightCountUnitlabel.Visible = false;
                //    this.RealWeightCountlabel.Visible = false;
                //    this.RealWeightCountChangelabel.Visible = false;
                //    this.RealWeightCountUnitlabel.Visible = false;
                //}

                //if (((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x01) == 1 
                //    //&&(GlobalDataInterface.SystemStructColor || GlobalDataInterface.SystemStructShape || GlobalDataInterface.SystemStructFlaw
                //    //|| GlobalDataInterface.SystemStructVolume || GlobalDataInterface.SystemStructProjectedArea)
                //    )//Add by ChengSk - 20181206
                //    || ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x02) == 2 &&
                //    (GlobalDataInterface.SystemStructBruise || GlobalDataInterface.SystemStructRot))
                //    || ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x04) == 4 &&
                //    GlobalDataInterface.SystemStructDensity)
                //    || ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8 &&
                //    (GlobalDataInterface.SystemStructSugar || GlobalDataInterface.SystemStructAcidity ||
                //    GlobalDataInterface.SystemStructHollow || GlobalDataInterface.SystemStructSkin ||
                //    GlobalDataInterface.SystemStructBrown || GlobalDataInterface.SystemStructTangxin))
                //    || ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x10) == 16 &&
                //    (GlobalDataInterface.SystemStructRigidity || GlobalDataInterface.SystemStructWater))
                //     )
                if (((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x01) == 1)//Add by ChengSk - 20181206
                    || ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x02) == 2 )
                    || ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8)
                    || ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x10) == 16)
                     )
                {
                    this.QaulitytoolStripButton.Enabled = true;
                }
                else
                {
                    this.QaulitytoolStripButton.Enabled = false;
                }

                //if (((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x02) == 1) || ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x04) == 1) || (GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 0))//只选重量时
                //{
                //    this.QaulitytoolStripButton.Enabled = false;
                //}
                //else
                //{
                //    this.QaulitytoolStripButton.Enabled = true;
                //}
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数SetStatisticInfotableLayoutPanel出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数SetStatisticInfotableLayoutPanel出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 计算统计信息(20秒/每次)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Staticstimer_Tick(object sender, EventArgs e)
        {
            try
            {
                ulong nowTotalCupNum = 0, nowTotalCount = 0;
                float nowWeightCount = 0;
                float X = 0.0f;
                //分选效率

                for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                {
                    int ChannelNum = 0;
                    for (int i = 0; i < ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    {
                        //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[k * ConstPreDefine.MAX_CHANNEL_NUM + i] == 1)
                        if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[k] > i)  //Modify by ChengSk - 20190521
                            ChannelNum++;
                    }
                    if (GlobalDataInterface.globalIn_statistics[k].nTotalCount != 0)
                    {
                        nowTotalCupNum += (ulong)(GlobalDataInterface.globalIn_statistics[k].nTotalCupNum * ChannelNum);
                    }
                }
                nowTotalCount = ulong.Parse(this.BatchNumChangelabel.Text);
                nowWeightCount = float.Parse(this.WeightCountlabelChangelabel.Text);

                //if (this == Form.ActiveForm)//是否操作当前窗体//Note by ChengSk - 20180727
                {


                    if (nowTotalCount < m_preTotalCount || nowTotalCupNum < m_preTotalCupNum)
                    {
                        this.SeparationEfficiencyChangelabel.Text = "0.0%";
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo("Go into SeparationEfficiencyChangelabel Calculation: nowTotalCount < m_preTotalCount || nowTotalCupNum < m_preTotalCupNum");
#endif
                    }
                    else
                    {
                        if (nowTotalCupNum - m_preTotalCupNum != 0)
                        {
                            float efficincy = (float)((nowTotalCount - m_preTotalCount) * 100.0f / (nowTotalCupNum - m_preTotalCupNum));
                            this.SeparationEfficiencyChangelabel.Text = efficincy.ToString("0.0") + "%";
#if REALEASE
                            //GlobalDataInterface.WriteErrorInfo("分选效率：nowTotalCount=" + nowTotalCount.ToString() + ", m_preTotalCount=" + m_preTotalCount.ToString() +
                            //    ", nowTotalCupNum=" + nowTotalCupNum.ToString() + ", m_preTotalCupNum=" + m_preTotalCupNum.ToString());
#endif
                        }
                        else
                        {
                            this.SeparationEfficiencyChangelabel.Text = "0.0%";
#if REALEASE
                            //GlobalDataInterface.WriteErrorInfo("Go into SeparationEfficiencyChangelabel Calculation: nowTotalCupNum = m_preTotalCupNum");
#endif
                        }
                    }

                    //实时产量
                    if (this.WeightCountlabelChangelabel.Visible == true)
                    {

                        if (nowWeightCount > m_preWeightCount)
                            X = (nowWeightCount - m_preWeightCount) * 180.0f;
                        else
                            X = 0.0f;
                        this.RealWeightCountChangelabel.Text = string.Format("{0:F}", X);
                    }
                }
//#if REALEASE
//                GlobalDataInterface.WriteErrorInfo(String.Format("**测试**实时产量{0},当前批重{1}，之前批重{2}，当前时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), X, nowWeightCount, m_preWeightCount));
//#endif   //delete by xcw 20201207
                m_preTotalCount = nowTotalCount;
                m_preTotalCupNum = nowTotalCupNum;
                m_preWeightCount = nowWeightCount;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数Staticstimer_Tick出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数Staticstimer_Tick出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 主界面关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                List<int> ChanelIDList = new List<int>();
                Commonfunction.GetAllChannelID(GlobalDataInterface.globalOut_SysConfig.nChannelInfo, ref ChanelIDList);
                if (ChanelIDList.Count > 0)
                {
                    // DialogResult result = MessageBox.Show("是否保存配置信息", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    //DialogResult result = MessageBox.Show("0x30001001 Whether to preseve the configuration information?", "Information", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    DialogResult result = MessageBox.Show("0x30001001 " + LanguageContainer.MainFormMessagebox3Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.MainFormMessageboxQuestionCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else if (result == DialogResult.Yes)
                    {
                        if (GlobalDataInterface.global_IsTestMode)
                        {
#if REALEASE
                            GlobalDataInterface.WriteErrorInfo(" **^V^** 窗体关闭，往FSM发送SHUTDOWN指令 **^V^**");
#endif
                            GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_SHUT_DOWN, null);
                            int res = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_OFF, null);//必须最后发送
                            if (res == -1) //FSM通信网络异常，需要写数据库、写标志位 Add by ChengSk - 20190516
                            {
                                AutoEndProcessEvent();
                                Commonfunction.SetAppSetting("可疑批次结束时间", GlobalDataInterface.lastBatchEndTime);
                            }
                        }
                        //Application.Exit();
                    }
                    else if (result == DialogResult.No)
                    {
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            int res = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_OFF, null);
                            if (res == -1) //FSM通信网络异常，需要写数据库、写标志位 Add by ChengSk - 20190516
                            {
                                AutoEndProcessEvent();
                                Commonfunction.SetAppSetting("可疑批次结束时间", GlobalDataInterface.lastBatchEndTime);
                            }
                        }
                    }
                    try
                    {
                        if (GlobalDataInterface.usedSeparationLogFlags)
                        {
                            DataSet dst = databaseOperation.GetRunningTimeInfoByStopTime("");
                            if (dst.Tables[0].Rows.Count > 0)
                            {
                                DateTime dt = DateTime.Now;
                                databaseOperation.UpdateRunningStopTime(int.Parse(dst.Tables[0].Rows[0]["ID"].ToString()), dt.ToString("HH:mm:ss"));
                            }
                            RunningTimeInfoStopTimeIsEmptyFlags = false;
                        }
                    }
                    catch (Exception ex) { }
                }

                GlobalDataInterface.UpStatusModifyEvent -= new GlobalDataInterface.StatusModify(OnUpStatusModify);   //Add by xcw - 20200527

                GlobalDataInterface.UpdateDataInterfaceEvent -= new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent); //Add by ChengSk - 20180830
                GlobalDataInterface.UpStopCheckSampleEvent -= new GlobalDataInterface.StopCheckSample(OnUpStopCheck);   //Add by xcw - 20200527
                GlobalDataInterface.UpSetTextCallbackEvent -= new GlobalDataInterface.SetTextCallback(SetTextCallback);   //Add by xcw - 20200713

            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数MainForm_FormClosing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数MainForm_FormClosing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 重置果杯状态
        /// </summary>
        public void ResetCupState()
        {
            try
            {
                //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 6) != 0)
                if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x03) != 0)
                {
                    List<int> ChanelIDList = new List<int>();
                    Commonfunction.GetAllChannelID(GlobalDataInterface.globalOut_SysConfig.nChannelInfo, ref ChanelIDList);
                    for (int i = 0; i < ChanelIDList.Count; i++)
                    {
                        //if (i % 2 == 0)///add by xcw 0617
                        //{
                        this.MainstatusStrip.Items[i + 4].BackColor = Color.Green;  //2->4 Modify by ChengSk - 20180723
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数ResetCupState出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数ResetCupState出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 双击果杯状态栏事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainstatusStripCupLabel_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ResetCupState();
                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_CUPSTATERESET, null);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数MainstatusStripCupLabel_DoubleClick出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数MainstatusStripCupLabel_DoubleClick出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 主界面大小改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            try
            {

                //if (m_MainFormIsInitial)
                //{
                //    //改变绿萌logo大小
                //    if (this.Width != MainFormOldSize.Width && this.Width >=1300)
                //    {
                //        //int tempWidth = this.Width - this.QaulitytoolStripButton.Width - this.GradetoolStripButton.Width- this.FruitInfotoolStripButton.Width
                //        //    - this.SavetoolStripButton.Width - this.LoadtoolStripButton.Width -this.ProcessInfotoolStripButton.Width-this.StatisticInfotoolStripButton.Width - this.PrinttoolStripButton.Width
                //        //    - this.EndProcesstoolStripButton.Width - this.ClientInfoLabel.Width - this.TimetoolStripLabel.Width-80;
                //        int tempWidth = LvMoonLogoOldSize.Width + (this.Width - MainFormOldSize.Width - (this.ClientInfoLabel.Width - ClientInfoLabelOldSize.Width));
                //        if (tempWidth * LvMoonLogoOldSize.Height / LvMoonLogoOldSize.Width > this.MaintoolStrip.Height)
                //        {
                //            this.LvMoonLogotoolStripButton.Height = this.MaintoolStrip.Height;
                //            this.LvMoonLogotoolStripButton.Width = this.LvMoonLogotoolStripButton.Height * LvMoonLogoOldSize.Width / LvMoonLogoOldSize.Height;
                //        }
                //        else
                //        {
                //            this.LvMoonLogotoolStripButton.Width = tempWidth;
                //            this.LvMoonLogotoolStripButton.Height = (int)(tempWidth * 1.0 * LvMoonLogoOldSize.Height / LvMoonLogoOldSize.Width);
                //        }
                //        ////获取原始绿萌图标大小
                //        LvMoonLogoOldSize.Width = this.LvMoonLogotoolStripButton.Width;
                //        LvMoonLogoOldSize.Height = this.LvMoonLogotoolStripButton.Height;
                //        ////获取原始界面大小
                //        MainFormOldSize.Width = this.Width;
                //        MainFormOldSize.Height = this.Height;
                //        //////获取原始客户信息界面大小
                //        ClientInfoLabelOldSize.Width = this.ClientInfoLabel.Width;
                //        ClientInfoLabelOldSize.Height = this.ClientInfoLabel.Height;
                //    }
                //}
                if (GlobalDataInterface.ExitList.Count > 0 && m_MainFormIsInitial)
                {
                    if (this.WindowState == FormWindowState.Minimized || this.WindowState == FormWindowState.Normal)
                    {
                        if (this.splitContainer2.Panel1.VerticalScroll.Value == 0 && bIsOnceMinimumSized == false)
                        {
                            bIsOnceMinimumSized = true;
                            currentExitVerticalScroll = this.splitContainer2.Panel1.VerticalScroll.Value;
                        }
                        else if (this.splitContainer2.Panel1.VerticalScroll.Value == 0 && bIsOnceMinimumSized == true) //说明已最小化一次
                        {
                            currentExitVerticalScroll = GlobalDataInterface.ExitVerticalScroll;
                        }
                        else if (this.splitContainer2.Panel1.VerticalScroll.Value != 0 && bIsOnceMinimumSized == false) //第一次最小化
                        {
                            bIsOnceMinimumSized = true;
                            currentExitVerticalScroll = this.splitContainer2.Panel1.VerticalScroll.Value;
                        }
                        else
                        {
                            bIsOnceMinimumSized = false;
                            currentExitVerticalScroll = this.splitContainer2.Panel1.VerticalScroll.Value;
                        }
                    }
                    else
                        this.splitContainer2.Panel1.VerticalScroll.Value = GlobalDataInterface.ExitVerticalScroll;

                    this.StatisticInfotableLayoutPanel.Size = new System.Drawing.Size((this.Width > this.StatisticInfotableLayoutPanelOldWidth ? this.Width : this.StatisticInfotableLayoutPanelOldWidth), this.StatisticInfotableLayoutPanel.Height);
                    //InitExitListBox(true);//初始化出口
                    //SetAllExitListBox();
                    //SetAllExitListBox();//初始化出口中等级
                    SetMainstatusStrip();

                }



                //asc.controlAutoSize(this);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数MainForm_SizeChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数MainForm_SizeChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// splitContainer2_Panel1水平滚动条初始化为零 //Add by ChengSk - 20180408
        /// </summary>
        public void splitContainer2_Panel1_HorizontalScroll_init()
        {
            try
            {
                if (this.splitContainer2.Panel1.HorizontalScroll.Value != 0)
                {
                    this.splitContainer2.Panel1.HorizontalScroll.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数splitContainer2_Panel1_HorizontalScroll_init出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数splitContainer2_Panel1_HorizontalScroll_init出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 出口列表面板滚动条拖动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitContainer2_Panel1_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                //if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                //    this.StatisticInfotableLayoutPanel.Location = new Point(3, this.StatisticInfotableLayoutPanel.Location.Y);
                //else
                //{
                //    bIsOnceMinimumSized = false;
                //}
                if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
                    bIsOnceMinimumSized = false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数splitContainer2_Panel1_Scroll出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数splitContainer2_Panel1_Scroll出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置显示程序保存与否
        /// </summary>
        /// <param name="IsSave"></param>
        public void SetSeparationProgrameChangelabel(bool IsSave, string FileName)//ivycc 2013.11.26
        {
            if (IsSave)
            {
                this.SeparationProgrameChangelabel.Text = FileName;//ivycc 2013.11.26
            }
            else
            {
                this.SeparationProgrameChangelabel.Text = m_resourceManager.GetString("SeparationProgrameChangelabel.Text");
            }
            Commonfunction.SetAppSetting("用户配置参数", this.SeparationProgrameChangelabel.Text);
        }

        /// <summary>
        /// 设置菜单电机使能按钮Enable
        /// </summary>
        public void SetMenuMotorEnable()
        {
            if (GlobalDataInterface.globalOut_SysConfig.nSubsysNum == 1)
                this.电机使能ToolStripMenuItem.Enabled = true;
            else
                this.电机使能ToolStripMenuItem.Enabled = false;
            //this.电机使能ToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// 获取分选开始时间
        /// </summary>
        /// <returns></returns>
        public string GetStartTimerChangelabel()
        {
            string time = this.StartTimeChangelabel.Text;
            return time;
        }


        /// <summary>
        /// 获取分选效率
        /// </summary>
        /// <returns></returns>
        public float GetSeparationEfficiency()
        {
            string[] sArray = this.SeparationEfficiencyChangelabel.Text.Split('%');
            float efficiency = float.Parse(sArray[0]);
            return efficiency;
        }

        /// <summary>
        /// 获取实时产量
        /// </summary>
        /// <returns></returns>
        public float GetRealWeightCount()
        {
            float RealWeightCount = float.Parse(this.RealWeightCountChangelabel.Text);
            return RealWeightCount;
        }

        /// <summary>
        /// 获取分选程序名称
        /// </summary>
        /// <returns></returns>
        public string GetProgramName()
        {
            string name = this.SeparationProgrameChangelabel.Text;
            return name;
        }

        //Add By ChengSk 20131108
        /// <summary>
        /// 数据接口更新时事件消息
        /// </summary>
        private void OnUpdateDataInterfaceEvent(DataInterface dataInterface)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent), dataInterface);
                return;
            }
            try
            {
                if (this == Form.ActiveForm)  //Modify by ChengSk - 20180305
                {
                    if (dataInterface.IoStStatistics.nTotalCount < 100)
                    {
                        if (EndProcessEnabled)
                        {
                            EndProcesstoolStripButton.Enabled = false;
                            结束加工toolStripMenuItem.Enabled = false;
                            EndProcessEnabled = false;
                        }
                    }
                    else
                    {
                        if (!EndProcessEnabled)
                        {
                            EndProcesstoolStripButton.Enabled = true;
                            结束加工toolStripMenuItem.Enabled = true;
                            EndProcessEnabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数OnUpdateDataInterfaceEvent出错" + ex + "\n" + ex.StackTrace);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数OnUpdateDataInterfaceEvent出错" + ex + "\n" + ex.StackTrace);
#endif
            }
        }

        //Add by ChengSk 20180130 分选日志
        private void SeparationLogtoolStripButton_Click(object sender, EventArgs e)
        {
            SeparationLogForm separationLogForm = new SeparationLogForm();
            separationLogForm.Show();
        }

        //Add By ChengSk 20131108 加工信息
        private void ProcessInfotoolStripButton_Click(object sender, EventArgs e)
        {
            ProcessInfoForm processInfoFrom = new ProcessInfoForm();
            processInfoFrom.ShowDialog(this);
        }

        //Add By ChengSk 20131108
        private void StatisticInfotoolStripButton_Click(object sender, EventArgs e)
        {
            int index = 0;

            #region 尺寸 重量 尺寸品质 重量品质 空 等情况判断
            if (GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType > 0)  //有尺寸（或重量）
            {
                if (GlobalDataInterface.dataInterface.IoStStGradeInfo.nQualityGradeNum > 0) //有品质
                {
                    if ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType & 0x03) > 0) //重量
                        index = 4;
                    else //尺寸
                        index = 3;
                }
                else //无品质
                {
                    if ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType & 0x03) > 0) //重量
                        index = 2;
                    else //尺寸
                        index = 1;
                }
            }
            else //无尺寸（或重量），认为没有连接FSM
            {
                index = 0;
            }

            //int selectType = GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType;
            //if ((selectType & 1) == 1) //有品质
            //{
            //    if ((selectType >> 3 & 1) == 1 || (selectType >> 4 & 1) == 1 || (selectType >> 5 & 1) == 1) //有尺寸
            //    {
            //        index = 3;
            //    }
            //    else                   //有重量
            //    {
            //        index = 4;
            //    }
            //}
            //else                       //无品质
            //{
            //    if ((selectType >> 3 & 1) == 1 || (selectType >> 4 & 1) == 1 || (selectType >> 5 & 1) == 1) //有尺寸
            //    {
            //        index = 1;
            //    }
            //    else                   //有重量
            //    {
            //        index = 2;
            //    }
            //}
            //if (selectType == 0)
            //{
            //    index = 0;
            //}
            #endregion

            switch (index)
            {
                case 0:
                    StatisticsInfoForm statisticsInfoForm = new StatisticsInfoForm();
                    statisticsInfoForm.ShowDialog(this);
                    break;
                case 1:
                    StatisticsInfoForm1 statisticsInfoForm1 = new StatisticsInfoForm1(GlobalDataInterface.dataInterface);
                    statisticsInfoForm1.ShowDialog(this);
                    break;
                case 2:
                    StatisticsInfoForm2 statisticsInfoForm2 = new StatisticsInfoForm2(GlobalDataInterface.dataInterface);
                    statisticsInfoForm2.ShowDialog(this);
                    break;
                case 3:
                    StatisticsInfoForm3 statisticsInfoForm3 = new StatisticsInfoForm3(GlobalDataInterface.dataInterface);
                    statisticsInfoForm3.ShowDialog(this);
                    break;
                case 4:
                    StatisticsInfoForm4 statisticsInfoForm4 = new StatisticsInfoForm4(GlobalDataInterface.dataInterface);
                    statisticsInfoForm4.ShowDialog(this);
                    break;
                default:
                    break;
            }
        }

        //Add By ChengSk 20131108 //Modify by ChengSk 20180727
        private void EndProcesstoolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount < 100)
                {
                    //MessageBox.Show("当前状态无法结束加工！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show("0x20001102 The current state cannot stop processing!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    MessageBox.Show("0x20001102 " + LanguageContainer.MainFormMessagebox4Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                GlobalDataInterface.WriteErrorInfo("***** 操作员点击了结束加工，即将征求意见，是否确定结束加工 *****");//Add by ChengSk - 20181228
                //if (MessageBox.Show("是否结束本批次加工，确认后本批次数据将清零，自动生成表格，不能再进行修改", "温馨提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                //if (MessageBox.Show("0x30001106 Whether to end the batch processing,confirmed the data will be zero,generated form automatically and can not modify?",
                //    "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)

                EndProcessingForm endProcessingForm = new EndProcessingForm(this);
                endProcessingForm.ShowDialog();

                if (bIsEndProcess == true)     //Modify by ChengSk - 20190628
                //if (MessageBox.Show("0x30001106 " + LanguageContainer.MainFormMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                //    LanguageContainer.MainFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                //    MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    GlobalDataInterface.WriteErrorInfo("***** 操作员确定了要结束加工 *****");//Add by ChengSk - 20181228
                    int QualityGradeNum = GlobalDataInterface.dataInterface.QualityGradeSum; //品质数量
                    int WeightOrSizeGradeNum = GlobalDataInterface.dataInterface.WeightOrSizeGradeSum; //尺寸或重量数量
                    int CustomerID = GlobalDataInterface.dataInterface.CustomerID; //客户ID
                    int ExportSum = GlobalDataInterface.dataInterface.ExportSum;   //出口数量

                    #region 插入等级信息表
                    bool bInsertGrade;
                    bool bUpdateGrade;
                    if (QualityGradeNum == 0) //无品质
                    {
                        for (int i = 0; i < WeightOrSizeGradeNum; i++)
                        {
                            DataSet dstGrade = databaseOperation.GetGradeByCustomerIDAndGradeID(CustomerID, i);
                            if (dstGrade.Tables[0].Rows.Count > 0) //已有数据，仅需修改
                            {
                                #region 修改等级
                                bUpdateGrade = databaseOperation.UpdateGradeInfo(CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1)
                                    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "", "", "", "", "", "", "");
                                if (!bUpdateGrade)
                                {
                                    //MessageBox.Show("0x10003102 Failed to update grade information " + i.ToString() + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    MessageBox.Show("0x10003102 " + LanguageContainer.MainFormMessagebox16Text[GlobalDataInterface.selectLanguageIndex] + " [" + i.ToString() + "]",
                                        LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                    GlobalDataInterface.WriteErrorInfo(string.Format("***打印***Failed to update Grade information! customerID:{0},gradeID:{1},boxNumber:{2},fruitNumber:{3},fruitWeight:{4},qualityName:{5},weightOrSizeName:{6},weightOrSizeLimit:{7}, selectWeightOrSize:{8},traitWeightOrSize:{9},traitColor:{10},traitShape:{11},traitFlaw:{12},traitHard:{13}, traitDensity:{14}, traitSugarDegree:{15}",
                                        CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "", "", "", "", "", "", ""));
#endif
                                    return;
                                }
                                #endregion
                            }
                            else //无数据，需要插入
                            {
                                #region 插入等级
                                //bInsertGrade = BusinessFacade.InsertGradeInfo(GlobalDataInterface.dataInterface.CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                //    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                //    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                //    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                //    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                //    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)
                                //    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 5 & 1) == 1)) ? 0 : 1).ToString(),
                                //    "", "", "", "", "", "", "");

                                bInsertGrade = databaseOperation.InsertGradeInfo(CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1)
                                    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "", "", "", "", "", "", "");
                                if (!bInsertGrade)
                                {
                                    //MessageBox.Show("插入等级信息" + i + "时失败！");
                                    //MessageBox.Show("0x10003102 Failed to insert Grade information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    MessageBox.Show("0x10003102 " + LanguageContainer.MainFormMessagebox7Text[GlobalDataInterface.selectLanguageIndex] + " [" + i.ToString() + "]",
                                        LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                    GlobalDataInterface.WriteErrorInfo(string.Format("**Print**Failed to insert Grade information! customerID:{0},gradeID:{1},boxNumber:{2},fruitNumber:{3},fruitWeight:{4},qualityName:{5},weightOrSizeName:{6},weightOrSizeLimit:{7}, selectWeightOrSize:{8},traitWeightOrSize:{9},traitColor:{10},traitShape:{11},traitFlaw:{12},traitHard:{13}, traitDensity:{14}, traitSugarDegree:{15}",
                                        CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "", "", "", "", "", "", ""));
#endif
                                    return;
                                }
                                #endregion
                            }
                        }
                    }
                    else //有品质
                    {
                        for (int i = 0; i < QualityGradeNum; i++)
                        {
                            for (int j = 0; j < WeightOrSizeGradeNum; j++)
                            {
                                DataSet dstGrade = databaseOperation.GetGradeByCustomerIDAndGradeID(CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j);
                                if (dstGrade.Tables[0].Rows.Count > 0) //已有数据，仅需修改
                                {
                                    #region 修改等级
                                    int QualityGradeNameLength = (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0");
                                    if (QualityGradeNameLength == -1)
                                    {
                                        QualityGradeNameLength = (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Length;
                                    }

                                    bUpdateGrade = databaseOperation.UpdateGradeInfo(CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    QualityGradeNameLength),
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1)
                                    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "",
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString());
                                    if (!bUpdateGrade)
                                    {
                                        //MessageBox.Show("0x10003102 Failed to update grade information " + (i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j).ToString() + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        MessageBox.Show("0x10003102 " + LanguageContainer.MainFormMessagebox16Text[GlobalDataInterface.selectLanguageIndex] + " [" + (i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j).ToString() + "]",
                                            LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                        GlobalDataInterface.WriteErrorInfo(string.Format("***打印***Failed to update Grade information! customerID:{0},gradeID:{1},boxNumber:{2},fruitNumber:{3},fruitWeight:{4},qualityName:{5},weightOrSizeName:{6},weightOrSizeLimit:{7}, selectWeightOrSize:{8},traitWeightOrSize:{9},traitColor:{10},traitShape:{11},traitFlaw:{12},traitHard:{13}, traitDensity:{14}, traitSugarDegree:{15}",
                                                CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                            (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                            (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                            (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                            GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                            ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                            "",
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString()));
#endif
                                        return;
                                    }
                                    #endregion
                                }
                                else //无数据，需要插入
                                {
                                    #region 插入等级
                                    //bInsertGrade = BusinessFacade.InsertGradeInfo(GlobalDataInterface.dataInterface.CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    //(Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    //(Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    //(Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    //(Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    //(Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    //GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                    //((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)
                                    //|| ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 5 & 1) == 1)) ? 0 : 1).ToString(),
                                    //"",
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString());
                                    int QualityGradeNameLength = (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0");
                                    if (QualityGradeNameLength == -1)
                                    {
                                        QualityGradeNameLength = (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Length;
                                    }
                                    bInsertGrade = databaseOperation.InsertGradeInfo(CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    QualityGradeNameLength),
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1)
                                    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "",
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString());
                                    if (!bInsertGrade)
                                    {
                                        //MessageBox.Show("插入等级信息" + (i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j) + "时失败！");
                                        MessageBox.Show("0x10003102 " + LanguageContainer.MainFormMessagebox7Text[GlobalDataInterface.selectLanguageIndex] + " [" + (i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j).ToString() + "]",
                                            LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                        GlobalDataInterface.WriteErrorInfo(string.Format("**Print**Failed to insert Grade information! customerID:{0},gradeID:{1},boxNumber:{2},fruitNumber:{3},fruitWeight:{4},qualityName:{5},weightOrSizeName:{6},weightOrSizeLimit:{7}, selectWeightOrSize:{8},traitWeightOrSize:{9},traitColor:{10},traitShape:{11},traitFlaw:{12},traitHard:{13}, traitDensity:{14}, traitSugarDegree:{15}",
                                            CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                            (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                            (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, QualityGradeNameLength),
                                            (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                            GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                            ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                            "",
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString()));
#endif
                                        return;
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion

                    #region 插入出口信息表
                    bool bInsertExport;
                    bool bUpdateExport;
                    for (int i = 0; i < ExportSum; i++)
                    {
                        DataSet dstExport = databaseOperation.GetExportByCustomerIDAndExportID(CustomerID, i);
                        if (dstExport.Tables[0].Rows.Count > 0) //已有数据，仅需修改
                        {
                            #region 修改出口
                            bUpdateExport = databaseOperation.UpdateExportInfo(CustomerID, i, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitCount[i],
                                (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitWeightCount[i]);
                            if (!bUpdateExport)
                            {
                                //MessageBox.Show("修改出口信息" + i + "时失败！");
                                //MessageBox.Show("0x10003103 Failed to update Outlets information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                MessageBox.Show("0x10003103 " + LanguageContainer.MainFormMessagebox17Text[GlobalDataInterface.selectLanguageIndex] + " [" + i.ToString() + "]",
                                    LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            #endregion
                        }
                        else //无数据，需要插入
                        {
                            #region 插入出口
                            //bInsertExport = BusinessFacade.InsertExportInfo(GlobalDataInterface.dataInterface.CustomerID, i, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitCount[i],
                            //    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitWeightCount[i]);
                            bInsertExport = databaseOperation.InsertExportInfo(GlobalDataInterface.dataInterface.CustomerID, i, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitCount[i],
                                (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitWeightCount[i]);
                            if (!bInsertExport)
                            {
                                //MessageBox.Show("插入出口信息" + i + "时失败！");
                                //MessageBox.Show("0x10003103 Failed to insert Outlets information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                MessageBox.Show("0x10003103 " + LanguageContainer.MainFormMessagebox8Text[GlobalDataInterface.selectLanguageIndex] + " [" + i.ToString() + "]",
                                    LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                GlobalDataInterface.WriteErrorInfo(string.Format("**Print**Failed to insert Outlets information! customerID:{0}, exportID:{1}, fruitNumber:{2}, fruitWeight:{3}",
                                    GlobalDataInterface.dataInterface.CustomerID, i, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitWeightCount[i]));
#endif
                                return;
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region 更新水果信息的完成状态
                    //bool bUpdateFruitCompletedState = BusinessFacade.UpdateFruitCompletedState(GlobalDataInterface.dataInterface.CustomerID, GlobalDataInterface.dataInterface.EndTime,
                    //    "1", (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightCount, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount,
                    //    GlobalDataInterface.dataInterface.QualityGradeSum, GlobalDataInterface.dataInterface.WeightOrSizeGradeSum, GlobalDataInterface.dataInterface.ExportSum, GlobalDataInterface.dataInterface.ColorGradeName,
                    //    GlobalDataInterface.dataInterface.ShapeGradeName, GlobalDataInterface.dataInterface.FlawGradeName, GlobalDataInterface.dataInterface.HardGradeName, GlobalDataInterface.dataInterface.DensityGradeName,
                    //    GlobalDataInterface.dataInterface.SugarDegreeGradeName);
                    //GlobalDataInterface.dataInterface.CustomerID -> CustomerID Modify by ChengSk - 20191128
                    bool bUpdateFruitCompletedState = databaseOperation.UpdateFruitCompletedState(CustomerID, GlobalDataInterface.dataInterface.EndTime,
                        "1", (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightCount, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount,
                        GlobalDataInterface.dataInterface.QualityGradeSum, GlobalDataInterface.dataInterface.WeightOrSizeGradeSum, GlobalDataInterface.dataInterface.ExportSum, GlobalDataInterface.dataInterface.ColorGradeName,
                        GlobalDataInterface.dataInterface.ShapeGradeName, GlobalDataInterface.dataInterface.FlawGradeName, GlobalDataInterface.dataInterface.HardGradeName, GlobalDataInterface.dataInterface.DensityGradeName,
                        GlobalDataInterface.dataInterface.SugarDegreeGradeName, this.SeparationProgrameChangelabel.Text);//2015-11-4 ivycc
                    if (!bUpdateFruitCompletedState)
                    {
                        //MessageBox.Show("更新水果信息完成状态失败！");
                        //MessageBox.Show("0x10003101 Failed to update the completion status of fruit information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        MessageBox.Show("0x10003101 " + LanguageContainer.MainFormMessagebox6Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    #endregion

                    GlobalDataInterface.bIsStart = false;                  //Add By ChengSk 20131108
                    GlobalDataInterface.bIsNumberOverHundred = false;      //Add By ChengSk 20131108
                    GlobalDataInterface.dataInterface = new DataInterface(true);
                    GlobalDataInterface.formStatisticsInfo = new stStatistics(true);
                    GlobalDataInterface.formGradeInfo = new stGradeInfo(true);
                    //重新加载客户信息
                    clientInfoContent = FileOperate.ReadFile(1, clientInfoFileName);
                    if (clientInfoContent == null)
                    {
                        GlobalDataInterface.dataInterface.CustomerName = "";
                        GlobalDataInterface.dataInterface.FarmName = "";
                        GlobalDataInterface.dataInterface.FruitName = "";
                    }
                    else
                    {
                        clientInfoContentItem = clientInfoContent.Split('，');
                        GlobalDataInterface.dataInterface.CustomerName = clientInfoContentItem[0];
                        GlobalDataInterface.dataInterface.FarmName = clientInfoContentItem[1];
                        GlobalDataInterface.dataInterface.FruitName = clientInfoContentItem[2];
                    }
                    //消息通知，加工清零
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        if (EndSaveMode == "1")
                        {
                            GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_CLEAR_DATA, null);
                            GlobalDataInterface.WriteErrorInfo("***** 结束加工指令HC_CMD_CLEAR_DATA已发出，完成结束加工过程 *****"); //Add by ChengSk - 20181228
                        }
                        else
                        {
                            GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_SAVE_CURRENT_DATA, null);
                            GlobalDataInterface.WriteErrorInfo("***** 结束加工指令HC_CMD_SAVE_CURRENT_DATA已发出，完成结束加工过程 *****");
                        }
                    }
                    this.Staticstimer.Enabled = false;
                    this.StartTimeChangelabel.Text = "";
                    m_preWeightCount = 0.0f;
                    m_preTotalCupNum = 0;
                    m_preTotalCount = 0;
                    m_ClearZero = true; //ivycc 2013.11.28
                    bIsHaveCompleted = true;                               //Add By ChengSk 20131120
                    //MessageBox.Show("本次加工结束！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show("0x30001107 End of the processing!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("0x30001107 " + LanguageContainer.MainFormMessagebox9Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.MainFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    bool bUpLoaded;
                    EventWaitHandle waitEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "UpLoad", out bUpLoaded);
                    waitEvent.Set(); //数据上传事件 Add by ChengSk - 20181210
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数EndProcesstoolStripButton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数EndProcesstoolStripButton_Click出错" + ex);
#endif
            }
            
        }

        /// <summary>
        /// 自动结束加工函数
        /// </summary>
        private void OnAutoEndProcess() //Add by ChengSk - 20190513
        {
            try
            {
                //if (MessageBox.Show("0x30001106 " + LanguageContainer.MainFormMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                //    LanguageContainer.MainFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                //    MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    GlobalDataInterface.WriteErrorInfo("***** 由于FSM网络异常，且当前加工开始，HC将自动结束加工 *****");
                    int QualityGradeNum = GlobalDataInterface.dataInterface.QualityGradeSum; //品质数量
                    int WeightOrSizeGradeNum = GlobalDataInterface.dataInterface.WeightOrSizeGradeSum; //尺寸或重量数量
                    int CustomerID = GlobalDataInterface.dataInterface.CustomerID; //客户ID
                    int ExportSum = GlobalDataInterface.dataInterface.ExportSum;   //出口数量

                    #region 插入等级信息表
                    bool bInsertGrade;
                    bool bUpdateGrade;
                    if (QualityGradeNum == 0) //无品质
                    {
                        for (int i = 0; i < WeightOrSizeGradeNum; i++)
                        {
                            DataSet dstGrade = databaseOperation.GetGradeByCustomerIDAndGradeID(CustomerID, i);
                            if (dstGrade.Tables[0].Rows.Count > 0) //已有数据，仅需修改
                            {
                                #region 修改等级
                                bUpdateGrade = databaseOperation.UpdateGradeInfo(CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1)
                                    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "", "", "", "", "", "", "");
                                if (!bUpdateGrade)
                                {
                                    //MessageBox.Show("0x10003102 Failed to update grade information " + i.ToString() + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    MessageBox.Show("0x10003102 " + LanguageContainer.MainFormMessagebox16Text[GlobalDataInterface.selectLanguageIndex] + " [" + i.ToString() + "]",
                                        LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                    GlobalDataInterface.WriteErrorInfo(string.Format("***打印***Failed to update Grade information! customerID:{0},gradeID:{1},boxNumber:{2},fruitNumber:{3},fruitWeight:{4},qualityName:{5},weightOrSizeName:{6},weightOrSizeLimit:{7}, selectWeightOrSize:{8},traitWeightOrSize:{9},traitColor:{10},traitShape:{11},traitFlaw:{12},traitHard:{13}, traitDensity:{14}, traitSugarDegree:{15}",
                                        CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "", "", "", "", "", "", ""));
#endif
                                    return;
                                }
                                #endregion
                            }
                            else //无数据，需要插入
                            {
                                #region 插入等级
                                //bInsertGrade = BusinessFacade.InsertGradeInfo(GlobalDataInterface.dataInterface.CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                //    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                //    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                //    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                //    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                //    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)
                                //    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 5 & 1) == 1)) ? 0 : 1).ToString(),
                                //    "", "", "", "", "", "", "");
                                bInsertGrade = databaseOperation.InsertGradeInfo(CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1)
                                    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "", "", "", "", "", "", "");
                                if (!bInsertGrade)
                                {
                                    //MessageBox.Show("插入等级信息" + i + "时失败！");
                                    //MessageBox.Show("0x10003102 Failed to insert Grade information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    MessageBox.Show("0x10003102 " + LanguageContainer.MainFormMessagebox7Text[GlobalDataInterface.selectLanguageIndex] + " [" + i.ToString() + "]",
                                        LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                    GlobalDataInterface.WriteErrorInfo(string.Format("**Print**Failed to insert Grade information! customerID:{0},gradeID:{1},boxNumber:{2},fruitNumber:{3},fruitWeight:{4},qualityName:{5},weightOrSizeName:{6},weightOrSizeLimit:{7}, selectWeightOrSize:{8},traitWeightOrSize:{9},traitColor:{10},traitShape:{11},traitFlaw:{12},traitHard:{13}, traitDensity:{14}, traitSugarDegree:{15}",
                                        CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "", "", "", "", "", "", ""));
#endif
                                    return;
                                }
                                #endregion
                            }
                        }
                    }
                    else //有品质
                    {
                        for (int i = 0; i < QualityGradeNum; i++)
                        {
                            for (int j = 0; j < WeightOrSizeGradeNum; j++)
                            {
                                DataSet dstGrade = databaseOperation.GetGradeByCustomerIDAndGradeID(CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j);
                                if (dstGrade.Tables[0].Rows.Count > 0) //已有数据，仅需修改
                                {
                                    #region 修改等级
                                    int QualityGradeNameLength = (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0");
                                    if (QualityGradeNameLength == -1)
                                    {
                                        QualityGradeNameLength = (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Length;
                                    }
                                    bUpdateGrade = databaseOperation.UpdateGradeInfo(CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    QualityGradeNameLength),
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1)
                                    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "",
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString());
                                    if (!bUpdateGrade)
                                    {
                                        //MessageBox.Show("0x10003102 Failed to update grade information " + (i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j).ToString() + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        MessageBox.Show("0x10003102 " + LanguageContainer.MainFormMessagebox16Text[GlobalDataInterface.selectLanguageIndex] + " [" + (i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j).ToString() + "]",
                                            LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                        GlobalDataInterface.WriteErrorInfo(string.Format("***打印***Failed to update Grade information! customerID:{0},gradeID:{1},boxNumber:{2},fruitNumber:{3},fruitWeight:{4},qualityName:{5},weightOrSizeName:{6},weightOrSizeLimit:{7}, selectWeightOrSize:{8},traitWeightOrSize:{9},traitColor:{10},traitShape:{11},traitFlaw:{12},traitHard:{13}, traitDensity:{14}, traitSugarDegree:{15}",
                                                CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                            (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                            (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                            (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                            GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                            ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                            "",
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString()));
#endif
                                        return;
                                    }
                                    #endregion
                                }
                                else //无数据，需要插入
                                {
                                    #region 插入等级
                                    //bInsertGrade = BusinessFacade.InsertGradeInfo(GlobalDataInterface.dataInterface.CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    //(Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    //(Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    //(Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    //(Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    //(Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    //GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                    //((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)
                                    //|| ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 5 & 1) == 1)) ? 0 : 1).ToString(),
                                    //"",
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString());
                                    int QualityGradeNameLength = (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0");
                                    if (QualityGradeNameLength == -1)
                                    {
                                        QualityGradeNameLength = (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Length;
                                    }
                                    bInsertGrade = databaseOperation.InsertGradeInfo(CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    QualityGradeNameLength),
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1)
                                    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "",
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString());
                                    if (!bInsertGrade)
                                    {
                                        //MessageBox.Show("插入等级信息" + (i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j) + "时失败！");
                                        MessageBox.Show("0x10003102 " + LanguageContainer.MainFormMessagebox7Text[GlobalDataInterface.selectLanguageIndex] + " [" + (i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j).ToString() + "]",
                                            LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                        GlobalDataInterface.WriteErrorInfo(string.Format("**Print**Failed to insert Grade information! customerID:{0},gradeID:{1},boxNumber:{2},fruitNumber:{3},fruitWeight:{4},qualityName:{5},weightOrSizeName:{6},weightOrSizeLimit:{7}, selectWeightOrSize:{8},traitWeightOrSize:{9},traitColor:{10},traitShape:{11},traitFlaw:{12},traitHard:{13}, traitDensity:{14}, traitSugarDegree:{15}",
                                            CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                            (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                            (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, QualityGradeNameLength),
                                            (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                            GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                            ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                            "",
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString()));
#endif
                                        return;
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion

                    #region 插入出口信息表
                    bool bInsertExport;
                    bool bUpdateExport;
                    for (int i = 0; i < ExportSum; i++)
                    {
                        DataSet dstExport = databaseOperation.GetExportByCustomerIDAndExportID(CustomerID, i);
                        if (dstExport.Tables[0].Rows.Count > 0) //已有数据，仅需修改
                        {
                            #region 修改出口
                            bUpdateExport = databaseOperation.UpdateExportInfo(CustomerID, i, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitCount[i],
                                (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitWeightCount[i]);
                            if (!bUpdateExport)
                            {
                                //MessageBox.Show("修改出口信息" + i + "时失败！");
                                //MessageBox.Show("0x10003103 Failed to update Outlets information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                MessageBox.Show("0x10003103 " + LanguageContainer.MainFormMessagebox17Text[GlobalDataInterface.selectLanguageIndex] + " [" + i.ToString() + "]",
                                    LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            #endregion
                        }
                        else //无数据，需要插入
                        {
                            #region 插入出口
                            //bInsertExport = BusinessFacade.InsertExportInfo(GlobalDataInterface.dataInterface.CustomerID, i, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitCount[i],
                            //    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitWeightCount[i]);
                            bInsertExport = databaseOperation.InsertExportInfo(GlobalDataInterface.dataInterface.CustomerID, i, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitCount[i],
                                (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitWeightCount[i]);
                            if (!bInsertExport)
                            {
                                //MessageBox.Show("插入出口信息" + i + "时失败！");
                                //MessageBox.Show("0x10003103 Failed to insert Outlets information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                MessageBox.Show("0x10003103 " + LanguageContainer.MainFormMessagebox8Text[GlobalDataInterface.selectLanguageIndex] + " [" + i.ToString() + "]",
                                    LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                GlobalDataInterface.WriteErrorInfo(string.Format("**Print**Failed to insert Outlets information! customerID:{0}, exportID:{1}, fruitNumber:{2}, fruitWeight:{3}",
                                    GlobalDataInterface.dataInterface.CustomerID, i, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitWeightCount[i]));
#endif
                                return;
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region 更新水果信息的完成状态
                    //bool bUpdateFruitCompletedState = BusinessFacade.UpdateFruitCompletedState(GlobalDataInterface.dataInterface.CustomerID, GlobalDataInterface.dataInterface.EndTime,
                    //    "1", (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightCount, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount,
                    //    GlobalDataInterface.dataInterface.QualityGradeSum, GlobalDataInterface.dataInterface.WeightOrSizeGradeSum, GlobalDataInterface.dataInterface.ExportSum, GlobalDataInterface.dataInterface.ColorGradeName,
                    //    GlobalDataInterface.dataInterface.ShapeGradeName, GlobalDataInterface.dataInterface.FlawGradeName, GlobalDataInterface.dataInterface.HardGradeName, GlobalDataInterface.dataInterface.DensityGradeName,
                    //    GlobalDataInterface.dataInterface.SugarDegreeGradeName);
                    bool bUpdateFruitCompletedState = databaseOperation.UpdateFruitCompletedState(GlobalDataInterface.dataInterface.CustomerID, GlobalDataInterface.dataInterface.EndTime,
                        "1", (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightCount, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount,
                        GlobalDataInterface.dataInterface.QualityGradeSum, GlobalDataInterface.dataInterface.WeightOrSizeGradeSum, GlobalDataInterface.dataInterface.ExportSum, GlobalDataInterface.dataInterface.ColorGradeName,
                        GlobalDataInterface.dataInterface.ShapeGradeName, GlobalDataInterface.dataInterface.FlawGradeName, GlobalDataInterface.dataInterface.HardGradeName, GlobalDataInterface.dataInterface.DensityGradeName,
                        GlobalDataInterface.dataInterface.SugarDegreeGradeName, this.SeparationProgrameChangelabel.Text);//2015-11-4 ivycc
                    if (!bUpdateFruitCompletedState)
                    {
                        //MessageBox.Show("更新水果信息完成状态失败！");
                        //MessageBox.Show("0x10003101 Failed to update the completion status of fruit information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        MessageBox.Show("0x10003101 " + LanguageContainer.MainFormMessagebox6Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    #endregion

                    GlobalDataInterface.bIsStart = false;                  //Add By ChengSk 20131108
                    GlobalDataInterface.bIsNumberOverHundred = false;      //Add By ChengSk 20131108
                    GlobalDataInterface.dataInterface = new DataInterface(true);
                    GlobalDataInterface.formStatisticsInfo = new stStatistics(true);
                    GlobalDataInterface.formGradeInfo = new stGradeInfo(true);
                    //重新加载客户信息
                    clientInfoContent = FileOperate.ReadFile(1, clientInfoFileName);
                    if (clientInfoContent == null)
                    {
                        GlobalDataInterface.dataInterface.CustomerName = "";
                        GlobalDataInterface.dataInterface.FarmName = "";
                        GlobalDataInterface.dataInterface.FruitName = "";
                    }
                    else
                    {
                        clientInfoContentItem = clientInfoContent.Split('，');
                        GlobalDataInterface.dataInterface.CustomerName = clientInfoContentItem[0];
                        GlobalDataInterface.dataInterface.FarmName = clientInfoContentItem[1];
                        GlobalDataInterface.dataInterface.FruitName = clientInfoContentItem[2];
                    }
                    //消息通知，加工清零
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        //if (EndSaveMode == "1")
                        if (CustomerID == 0)
                        {
                            GlobalDataInterface.WriteErrorInfo("***** @@@结束加工时，CustomerID=0，不进行清零操作 *****"); //Add by ChengSk - 20191128
                        }
                        else
                        {
                            GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_CLEAR_DATA, null);
                            GlobalDataInterface.WriteErrorInfo("***** 结束加工指令已发出，完成结束加工过程(自动) *****");
                        }
                        //else
                        //{
                        //    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_SAVE_CURRENT_DATA, null);
                        //    GlobalDataInterface.WriteErrorInfo("***** 结束加工指令HC_CMD_SAVE_CURRENT_DATA已发出，完成结束加工过程 *****");
                        //}
                    }
                    this.Staticstimer.Enabled = false;
                    this.StartTimeChangelabel.Text = "";
                    m_preWeightCount = 0.0f;
                    m_preTotalCupNum = 0;
                    m_preTotalCount = 0;
                    m_ClearZero = true; //ivycc 2013.11.28
                    bIsHaveCompleted = true;                               //Add By ChengSk 20131120
                    //MessageBox.Show("本次加工结束！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show("0x30001107 End of the processing!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show("0x30001107 " + LanguageContainer.MainFormMessagebox9Text[GlobalDataInterface.selectLanguageIndex],
                    //    LanguageContainer.MainFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                    //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("MainForm中函数AutoEndProcess出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数AutoEndProcess出错" + e);
#endif
            }
        }

        /// <summary>
        /// 刷新主界面上显示数据
        /// </summary>
        /// <param name="sender"></param>
        public void OnUpStatusModify()
        {
            try
            {

                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new GlobalDataInterface.StatusModify(OnUpStatusModify));
                }
                else
                {


                    if (GlobalDataInterface.UpData_gradeinfo)
                    {
                        if (GlobalDataInterface.nSampleOutlet != 0)
                        {
                            bIsEndSendData = true;
                            //InitExitListBox(true);//初始化出口
                            SetExitListBox(GlobalDataInterface.nSampleOutlet - 1, GlobalDataInterface.nSampleOutlet);
                            //SetAllExitListBox();
                            GlobalDataInterface.globalOut_GradeInfo.nExitSwitchNum[GlobalDataInterface.nSampleOutlet - 1] = GlobalDataInterface.globalOut_GradeInfo.nCheckNum;//
                            GlobalDataInterface.globalOut_GradeInfo.nSwitchLabel[GlobalDataInterface.nSampleOutlet - 1] = 0;
                            int sizeNum = 1;
                            int qulNum = 1;
                            if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//品质与尺寸或者品质与重量
                            {
                                qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                            }
                            sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                            for (int i = 0; i < qulNum; i++)
                            {
                                for (int j = 0; j < sizeNum; j++)
                                {
                                    if (GlobalDataInterface.tempGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit > 0)
                                    {

                                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit |= ((long)1 << (int.Parse(GlobalDataInterface.nSampleOutlet.ToString()/*m_ExitControl.labelList[Index].Text*/) - 1));
                                    }

                                }
                            }
                            //GlobalDataInterface.globalOut_GradeInfo.grades[m_GradedataGridViewSelectedCellList[0].rowIndex * ConstPreDefine.MAX_SIZE_GRADE_NUM + m_GradedataGridViewSelectedCellList[0].colIndex].exit |= ((long)1 << (int.Parse(GlobalDataInterface.nSampleOutlet.ToString()/*m_ExitControl.labelList[Index].Text*/) - 1));
                            //GlobalDataInterface.globalOut_GradeInfo.grades[0 * ConstPreDefine.MAX_SIZE_GRADE_NUM + 4].exit = ((long)1 << (int.Parse(GlobalDataInterface.nSampleOutlet.ToString()/*m_ExitControl.labelList[Index].Text*/) - 1)); ;

                            GlobalDataInterface.UpData_gradeinfo = false;
                        }


                        //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        if (global_IsTest != 0) //add by xcw 20201211
                        {
                            MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                            LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }


                    }
                    else
                    {
                        return;
                    }


                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数OnUpWeightInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数OnUpWeightInfo出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 刷新主界面上显示数据
        /// </summary>
        /// <param name="sender"></param>
        public void OnUpStopCheck()
        {
            try
            {
                //if (this == Form.ActiveForm)//是否操作当前窗体
                //{
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new GlobalDataInterface.StatusModify(OnUpStopCheck));
                }
                else
                {


                    if (GlobalDataInterface.StopCheck)
                    {
                        SetGradedataGridViewInfo();//初始化等级表格
                        for (int p = 0; p < GradedataGridView.RowCount; p++)
                        {
                            for (int q = 0; q < GradedataGridView.ColumnCount; q++)
                            {
                                GlobalDataInterface.globalOut_GradeInfo.grades[p * ConstPreDefine.MAX_SIZE_GRADE_NUM + q].exit &= ~((long)1 << (int.Parse(GlobalDataInterface.nSampleOutlet.ToString()) - 1));
                                if (GlobalDataInterface.globalOut_GradeInfo.grades[p * ConstPreDefine.MAX_SIZE_GRADE_NUM + q].exit == 0) //Add by ChengSk - 20180408
                                {
                                    this.GradedataGridView[q, p].Style.BackColor = Color.Pink;
                                    this.GradedataGridView[q, p].Style.SelectionBackColor = Color.FromArgb(36, 155, 255);
                                }
                            }
                        }
                        //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                        if (global_IsTest != 0) //add by xcw 20201211
                        {
                            MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                            LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (GlobalDataInterface.nSampleOutlet != 0)
                        {
                            //InitExitListBox(true);//初始化出口
                            SetExitListBox(GlobalDataInterface.nSampleOutlet - 1, GlobalDataInterface.nSampleOutlet);
                        }

                        GlobalDataInterface.StopCheck = true;
                    }
                    else
                    {
                        return;
                    }


                }
                //}
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数OnUpWeightInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数OnUpWeightInfo出错" + ex);
#endif
            }
        }


        /// <summary>
        /// 刷新主界面上显示数据
        /// </summary>
        /// <param name="sender"></param>
        public void SetTextCallback()
        {
            try
            {
                //if (this == Form.ActiveForm)//是否操作当前窗体
                //{
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new GlobalDataInterface.SetTextCallback(SetTextCallback));
                    }
                    else
                    {
                        if (GlobalDataInterface.nVer == 0)
                        {
                            GlobalDataInterface.mainform.Text = "FruitSorting5.1.1";
                        }
                        else if (GlobalDataInterface.nVer == 1)
                        {
                            GlobalDataInterface.mainform.Text = "FruitSorting3.2.1";
                        }                    
                    }
                //}
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数OnUpWeightInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数OnUpWeightInfo出错" + ex);
#endif
            }
        }
        private void AutoEndProcessEvent()
        {
            try
            {
                if (GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount >= 100)
                {
                    GlobalDataInterface.WriteErrorInfo("***** 由于FSM网络异常，自动结束加工 *****");
                    int QualityGradeNum = GlobalDataInterface.dataInterface.QualityGradeSum; //品质数量
                    int WeightOrSizeGradeNum = GlobalDataInterface.dataInterface.WeightOrSizeGradeSum; //尺寸或重量数量
                    int CustomerID = GlobalDataInterface.dataInterface.CustomerID; //客户ID
                    int ExportSum = GlobalDataInterface.dataInterface.ExportSum;   //出口数量

                    #region 插入等级信息表
                    bool bInsertGrade;
                    bool bUpdateGrade;
                    if (QualityGradeNum == 0) //无品质
                    {
                        for (int i = 0; i < WeightOrSizeGradeNum; i++)
                        {
                            DataSet dstGrade = databaseOperation.GetGradeByCustomerIDAndGradeID(CustomerID, i);
                            if (dstGrade.Tables[0].Rows.Count > 0) //已有数据，仅需修改
                            {
                                #region 修改等级
                                bUpdateGrade = databaseOperation.UpdateGradeInfo(CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1)
                                    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "", "", "", "", "", "", "");
                                if (!bUpdateGrade)
                                {
                                    //MessageBox.Show("0x10003102 Failed to update grade information " + i.ToString() + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    MessageBox.Show("0x10003102 " + LanguageContainer.MainFormMessagebox16Text[GlobalDataInterface.selectLanguageIndex] + " [" + i.ToString() + "]",
                                        LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                    GlobalDataInterface.WriteErrorInfo(string.Format("***打印***Failed to update Grade information! customerID:{0},gradeID:{1},boxNumber:{2},fruitNumber:{3},fruitWeight:{4},qualityName:{5},weightOrSizeName:{6},weightOrSizeLimit:{7}, selectWeightOrSize:{8},traitWeightOrSize:{9},traitColor:{10},traitShape:{11},traitFlaw:{12},traitHard:{13}, traitDensity:{14}, traitSugarDegree:{15}",
                                        CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "", "", "", "", "", "", ""));
#endif
                                    return;
                                }
                                #endregion
                            }
                            else //无数据，需要插入
                            {
                                #region 插入等级
                                //bInsertGrade = BusinessFacade.InsertGradeInfo(GlobalDataInterface.dataInterface.CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                //    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                //    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                //    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                //    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                //    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)
                                //    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 5 & 1) == 1)) ? 0 : 1).ToString(),
                                //    "", "", "", "", "", "", "");
                                bInsertGrade = databaseOperation.InsertGradeInfo(CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1)
                                    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "", "", "", "", "", "", "");
                                if (!bInsertGrade)
                                {
                                    //MessageBox.Show("插入等级信息" + i + "时失败！");
                                    //MessageBox.Show("0x10003102 Failed to insert Grade information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    MessageBox.Show("0x10003102 " + LanguageContainer.MainFormMessagebox7Text[GlobalDataInterface.selectLanguageIndex] + " [" + i.ToString() + "]",
                                        LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                    GlobalDataInterface.WriteErrorInfo(string.Format("**Print**Failed to insert Grade information! customerID:{0},gradeID:{1},boxNumber:{2},fruitNumber:{3},fruitWeight:{4},qualityName:{5},weightOrSizeName:{6},weightOrSizeLimit:{7}, selectWeightOrSize:{8},traitWeightOrSize:{9},traitColor:{10},traitShape:{11},traitFlaw:{12},traitHard:{13}, traitDensity:{14}, traitSugarDegree:{15}",
                                        CustomerID, i, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i], "",
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[i].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "", "", "", "", "", "", ""));
#endif
                                    return;
                                }
                                #endregion
                            }
                        }
                    }
                    else //有品质
                    {
                        for (int i = 0; i < QualityGradeNum; i++)
                        {
                            for (int j = 0; j < WeightOrSizeGradeNum; j++)
                            {
                                DataSet dstGrade = databaseOperation.GetGradeByCustomerIDAndGradeID(CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j);
                                if (dstGrade.Tables[0].Rows.Count > 0) //已有数据，仅需修改
                                {
                                    #region 修改等级
                                    int QualityGradeNameLength = (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0");
                                    if (QualityGradeNameLength == -1)
                                    {
                                        QualityGradeNameLength = (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Length;
                                    }
                                    bUpdateGrade = databaseOperation.UpdateGradeInfo(CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    QualityGradeNameLength),
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1)
                                    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "",
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString());
                                    if (!bUpdateGrade)
                                    {
                                        //MessageBox.Show("0x10003102 Failed to update grade information " + (i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j).ToString() + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        MessageBox.Show("0x10003102 " + LanguageContainer.MainFormMessagebox16Text[GlobalDataInterface.selectLanguageIndex] + " [" + (i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j).ToString() + "]",
                                            LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                        GlobalDataInterface.WriteErrorInfo(string.Format("***打印***Failed to update Grade information! customerID:{0},gradeID:{1},boxNumber:{2},fruitNumber:{3},fruitWeight:{4},qualityName:{5},weightOrSizeName:{6},weightOrSizeLimit:{7}, selectWeightOrSize:{8},traitWeightOrSize:{9},traitColor:{10},traitShape:{11},traitFlaw:{12},traitHard:{13}, traitDensity:{14}, traitSugarDegree:{15}",
                                                CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                            (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                            (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                            (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                            GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                            ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                            "",
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString()));
#endif
                                        return;
                                    }
                                    #endregion
                                }
                                else //无数据，需要插入
                                {
                                    #region 插入等级
                                    //bInsertGrade = BusinessFacade.InsertGradeInfo(GlobalDataInterface.dataInterface.CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    //(Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    //(Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    //(Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    //(Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    //(Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    //GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                    //((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)
                                    //|| ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 5 & 1) == 1)) ? 0 : 1).ToString(),
                                    //"",
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                    //Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString());
                                    int QualityGradeNameLength = (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0");
                                    if (QualityGradeNameLength == -1)
                                    {
                                        QualityGradeNameLength = (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Length;
                                    }
                                    bInsertGrade = databaseOperation.InsertGradeInfo(CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    QualityGradeNameLength),
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0,
                                    (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                    GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                    ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1)
                                    || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                    "",
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                    Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString());
                                    if (!bInsertGrade)
                                    {
                                        //MessageBox.Show("插入等级信息" + (i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j) + "时失败！");
                                        MessageBox.Show("0x10003102 " + LanguageContainer.MainFormMessagebox7Text[GlobalDataInterface.selectLanguageIndex] + " [" + (i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j).ToString() + "]",
                                            LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                        GlobalDataInterface.WriteErrorInfo(string.Format("**Print**Failed to insert Grade information! customerID:{0},gradeID:{1},boxNumber:{2},fruitNumber:{3},fruitWeight:{4},qualityName:{5},weightOrSizeName:{6},weightOrSizeLimit:{7}, selectWeightOrSize:{8},traitWeightOrSize:{9},traitColor:{10},traitShape:{11},traitFlaw:{12},traitHard:{13}, traitDensity:{14}, traitSugarDegree:{15}",
                                            CustomerID, i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j, GlobalDataInterface.dataInterface.IoStStatistics.nBoxGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                            (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightGradeCount[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j],
                                            (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                            (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).Substring(0, (Encoding.Default.GetString(GlobalDataInterface.dataInterface.IoStStGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH)).IndexOf("\0")),
                                            GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nMinSize,
                                            ((((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 2 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 3 & 1) == 1) || ((GlobalDataInterface.dataInterface.IoStStGradeInfo.nClassifyType >> 4 & 1) == 1)) ? 0 : 1).ToString(),
                                            "",
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].nColorGrade).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbShapeSize).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbFlawArea).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbRigidity).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbDensity).ToString(),
                                            Convert.ToInt32(GlobalDataInterface.dataInterface.IoStStGradeInfo.grades[(i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j)].sbSugar).ToString()));
#endif
                                        return;
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion

                    #region 插入出口信息表
                    bool bInsertExport;
                    bool bUpdateExport;
                    for (int i = 0; i < ExportSum; i++)
                    {
                        DataSet dstExport = databaseOperation.GetExportByCustomerIDAndExportID(CustomerID, i);
                        if (dstExport.Tables[0].Rows.Count > 0) //已有数据，仅需修改
                        {
                            #region 修改出口
                            bUpdateExport = databaseOperation.UpdateExportInfo(CustomerID, i, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitCount[i],
                                (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitWeightCount[i]);
                            if (!bUpdateExport)
                            {
                                //MessageBox.Show("修改出口信息" + i + "时失败！");
                                //MessageBox.Show("0x10003103 Failed to update Outlets information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                MessageBox.Show("0x10003103 " + LanguageContainer.MainFormMessagebox17Text[GlobalDataInterface.selectLanguageIndex] + " [" + i.ToString() + "]",
                                    LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            #endregion
                        }
                        else //无数据，需要插入
                        {
                            #region 插入出口
                            //bInsertExport = BusinessFacade.InsertExportInfo(GlobalDataInterface.dataInterface.CustomerID, i, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitCount[i],
                            //    (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitWeightCount[i]);
                            bInsertExport = databaseOperation.InsertExportInfo(GlobalDataInterface.dataInterface.CustomerID, i, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitCount[i],
                                (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitWeightCount[i]);
                            if (!bInsertExport)
                            {
                                //MessageBox.Show("插入出口信息" + i + "时失败！");
                                //MessageBox.Show("0x10003103 Failed to insert Outlets information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                MessageBox.Show("0x10003103 " + LanguageContainer.MainFormMessagebox8Text[GlobalDataInterface.selectLanguageIndex] + " [" + i.ToString() + "]",
                                    LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
#if REALEASE
                                GlobalDataInterface.WriteErrorInfo(string.Format("**Print**Failed to insert Outlets information! customerID:{0}, exportID:{1}, fruitNumber:{2}, fruitWeight:{3}",
                                    GlobalDataInterface.dataInterface.CustomerID, i, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitCount[i], (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nExitWeightCount[i]));
#endif
                                return;
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region 更新水果信息的完成状态
                    //bool bUpdateFruitCompletedState = BusinessFacade.UpdateFruitCompletedState(GlobalDataInterface.dataInterface.CustomerID, GlobalDataInterface.dataInterface.EndTime,
                    //    "1", (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightCount, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount,
                    //    GlobalDataInterface.dataInterface.QualityGradeSum, GlobalDataInterface.dataInterface.WeightOrSizeGradeSum, GlobalDataInterface.dataInterface.ExportSum, GlobalDataInterface.dataInterface.ColorGradeName,
                    //    GlobalDataInterface.dataInterface.ShapeGradeName, GlobalDataInterface.dataInterface.FlawGradeName, GlobalDataInterface.dataInterface.HardGradeName, GlobalDataInterface.dataInterface.DensityGradeName,
                    //    GlobalDataInterface.dataInterface.SugarDegreeGradeName);
                    bool bUpdateFruitCompletedState = databaseOperation.UpdateFruitCompletedState(GlobalDataInterface.dataInterface.CustomerID, GlobalDataInterface.dataInterface.EndTime,
                        "1", (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nWeightCount, (Int32)GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount,
                        GlobalDataInterface.dataInterface.QualityGradeSum, GlobalDataInterface.dataInterface.WeightOrSizeGradeSum, GlobalDataInterface.dataInterface.ExportSum, GlobalDataInterface.dataInterface.ColorGradeName,
                        GlobalDataInterface.dataInterface.ShapeGradeName, GlobalDataInterface.dataInterface.FlawGradeName, GlobalDataInterface.dataInterface.HardGradeName, GlobalDataInterface.dataInterface.DensityGradeName,
                        GlobalDataInterface.dataInterface.SugarDegreeGradeName, this.SeparationProgrameChangelabel.Text);//2015-11-4 ivycc
                    if (!bUpdateFruitCompletedState)
                    {
                        //MessageBox.Show("更新水果信息完成状态失败！");
                        //MessageBox.Show("0x10003101 Failed to update the completion status of fruit information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        MessageBox.Show("0x10003101 " + LanguageContainer.MainFormMessagebox6Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.MainFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    #endregion

                    GlobalDataInterface.lastBatchEndTime = GlobalDataInterface.dataInterface.EndTime;
                    GlobalDataInterface.bIsStart = false;                  //Add By ChengSk 20131108
                    GlobalDataInterface.bIsNumberOverHundred = false;      //Add By ChengSk 20131108
                    GlobalDataInterface.dataInterface = new DataInterface(true);
                    GlobalDataInterface.formStatisticsInfo = new stStatistics(true);
                    GlobalDataInterface.formGradeInfo = new stGradeInfo(true);
                    //重新加载客户信息
                    clientInfoContent = FileOperate.ReadFile(1, clientInfoFileName);
                    if (clientInfoContent == null)
                    {
                        GlobalDataInterface.dataInterface.CustomerName = "";
                        GlobalDataInterface.dataInterface.FarmName = "";
                        GlobalDataInterface.dataInterface.FruitName = "";
                    }
                    else
                    {
                        clientInfoContentItem = clientInfoContent.Split('，');
                        GlobalDataInterface.dataInterface.CustomerName = clientInfoContentItem[0];
                        GlobalDataInterface.dataInterface.FarmName = clientInfoContentItem[1];
                        GlobalDataInterface.dataInterface.FruitName = clientInfoContentItem[2];
                    }
                    //消息通知，加工清零
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_CLEAR_DATA, null);
                        GlobalDataInterface.WriteErrorInfo("***** 结束加工指令已发出，完成结束加工过程 *****"); //Add by ChengSk - 20181228
                    }
                    this.Staticstimer.Enabled = false;
                    this.StartTimeChangelabel.Text = "";
                    m_preWeightCount = 0.0f;
                    m_preTotalCupNum = 0;
                    m_preTotalCount = 0;
                    m_ClearZero = true; //ivycc 2013.11.28
                    bIsHaveCompleted = true;                               //Add By ChengSk 20131120
                    //MessageBox.Show("本次加工结束！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show("0x30001107 End of the processing!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show("0x30001107 " + LanguageContainer.MainFormMessagebox9Text[GlobalDataInterface.selectLanguageIndex],
                    //    LanguageContainer.MainFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                    //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GlobalDataInterface.WriteErrorInfo("***** 由于FSM网络异常，自动结束加工SUCCESS *****");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数AutoEndProcessEvent出错" + ex + "\n代码定位：" + ex.StackTrace);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数AutoEndProcessEvent出错：" + ex + "\n代码定位：" + ex.StackTrace);
#endif
            }
        }

        //Add By ChengSk 20131108
        private void PrinttoolStripButton_Click(object sender, EventArgs e)
        {
            //Point frmP = new Point(this.MaintoolStrip.Left + this.PrinttoolStripButton.Width * 9 + 5, this.MaintoolStrip.Top + PrinttoolStripButton.Height);
            Point frmP = new Point(this.MaintoolStrip.Left + this.QaulitytoolStripButton.Width + this.GradetoolStripButton.Width + this.FruitInfotoolStripButton.Width +
                this.LoadtoolStripButton.Width + this.SavetoolStripButton.Width + this.SeparationLogtoolStripButton.Width +
                this.ProcessInfotoolStripButton.Width + this.StatisticInfotoolStripButton.Width + this.EndProcesstoolStripButton.Width + 5,
                this.MaintoolStrip.Top + PrinttoolStripButton.Height);
            Point startPosition = this.PointToScreen(frmP);

            //if (BusinessFacade.GetFruitTopCustomerID().Tables[0].Rows.Count == 0)
            if (databaseOperation.GetFruitTopCustomerID().Tables[0].Rows.Count == 0)
            {
                //MessageBox.Show("初始状态下数据为空，不能打印！");
                //MessageBox.Show("0x20001103 The initial state data is empty,and cannot print!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MessageBox.Show("0x20001103 " + LanguageContainer.MainFormMessagebox10Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //int newCustomerID = Convert.ToInt32(BusinessFacade.GetFruitTopCustomerID().Tables[0].Rows[0]["CustomerID"].ToString());
            int newCustomerID = Convert.ToInt32(databaseOperation.GetFruitTopCustomerID().Tables[0].Rows[0]["CustomerID"].ToString());
            //DataSet dst1 = BusinessFacade.GetFruitByCustomerID(newCustomerID);
            DataSet dst1 = databaseOperation.GetFruitByCustomerID(newCustomerID);

            if (GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount < 100 && bIsHaveCompleted && dst1.Tables[0].Rows[0]["CompletedState"].ToString().Equals("1"))
            {
                //this.ContextMenuPrint.Text = "加工已完成可以打印";
                this.ContextMenuPrint.Text = m_resourceManager.GetString("LblProOffAndPrint.Text");
                this.ContextMenuPrint.Enabled = true;
            }
            else
            {
                //this.ContextMenuPrint.Text = "加工未完成不能打印";
                this.ContextMenuPrint.Text = m_resourceManager.GetString("LblProOffAndCannotPrint.Text");
                this.ContextMenuPrint.Enabled = false;
            }
            contextMenuStrip1.Show(startPosition);
        }

        //Add By xcw 20191202
        //private void QualitytoolStripButton_Click(object sender, EventArgs e)
        //{
        //    QualGradeSetForm qualGradeSetForm = new QualGradeSetForm(this);
        //    qualGradeSetForm.ShowDialog();
        //}

        bool[] showPrintDialog = new bool[4];//打印设置页面是否显示
        //Add By ChengSk 20131120
        private void ContextMenuPrint_Click(object sender, EventArgs e)
        {

            try
            {
                //获取最新客户信息的ID号
                //int newCustomerID = Convert.ToInt32(BusinessFacade.GetFruitTopCustomerID().Tables[0].Rows[0]["CustomerID"].ToString());
                int newCustomerID = Convert.ToInt32(databaseOperation.GetFruitTopCustomerID().Tables[0].Rows[0]["CustomerID"].ToString());

                DataSet dst1;                   //统计信息时获取tb_FruitInfo
                DataSet dst2;                   //统计信息时获取tb_GradeInfo
                DataSet dst3;                   //统计信息时获取tb_ExportInfo
                printDataInterface = new DataInterface(true);
                stStatistics statisticsInfo = new stStatistics(true);
                stGradeInfo gradeInfo = new stGradeInfo(true);

                printDataInterface.BSourceDB = true;

                #region 从数据库中取相应条目信息放到DataSet中
                //获取tb_FruitInfo
                //dst1 = BusinessFacade.GetFruitByCustomerID(newCustomerID);
                dst1 = databaseOperation.GetFruitByCustomerID(newCustomerID);
                if (dst1.Tables[0].Rows[0]["CompletedState"].ToString().Equals("0"))
                {
                    //MessageBox.Show("加工进行中 不能进行统计！");
                    //MessageBox.Show("20001101 In the processing, can not be statistics!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    MessageBox.Show("20001101 " + LanguageContainer.MainFormMessagebox11Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //获取tb_GradeInfo
                //dst2 = BusinessFacade.GetGradeByCustomerID(newCustomerID);
                dst2 = databaseOperation.GetGradeByCustomerID(newCustomerID);
                //获取tb_ExportInfo
                //dst3 = BusinessFacade.GetExportByCustomerID(newCustomerID);
                dst3 = databaseOperation.GetExportByCustomerID(newCustomerID);
                #endregion

                #region 往printDataInterface中插入水果信息
                //往printDataInterface中插入水果信息
                if (dst1.Tables[0].Rows.Count <= 0)
                {
                    //MessageBox.Show("选择条目的水果信息为空！");
                    //MessageBox.Show("30001103 The currently selected fruit information is empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("30001103 " + LanguageContainer.MainFormMessagebox12Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.MainFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                printDataInterface.BSourceDB = true;
                printDataInterface.CustomerID = Convert.ToInt32(dst1.Tables[0].Rows[0]["CustomerID"].ToString());
                printDataInterface.CustomerName = dst1.Tables[0].Rows[0]["CustomerName"].ToString();
                printDataInterface.FarmName = dst1.Tables[0].Rows[0]["FarmName"].ToString();
                printDataInterface.FruitName = dst1.Tables[0].Rows[0]["FruitName"].ToString();
                printDataInterface.StartTime = dst1.Tables[0].Rows[0]["StartTime"].ToString();
                printDataInterface.EndTime = dst1.Tables[0].Rows[0]["EndTime"].ToString();
                printDataInterface.StartedState = dst1.Tables[0].Rows[0]["StartedState"].ToString();
                printDataInterface.CompletedState = dst1.Tables[0].Rows[0]["CompletedState"].ToString();
                printDataInterface.QualityGradeSum = Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString());
                if (dst1.Tables[0].Rows[0]["WeightOrSizeGradeSum"].ToString().Equals(""))
                {
                    printDataInterface.WeightOrSizeGradeSum = 0;
                }
                else
                {
                    printDataInterface.WeightOrSizeGradeSum = Convert.ToInt32(dst1.Tables[0].Rows[0]["WeightOrSizeGradeSum"].ToString());
                }
                printDataInterface.ExportSum = Convert.ToInt32(dst1.Tables[0].Rows[0]["ExportSum"].ToString());
                printDataInterface.ColorGradeName = dst1.Tables[0].Rows[0]["ColorGradeName"].ToString();
                printDataInterface.ShapeGradeName = dst1.Tables[0].Rows[0]["ShapeGradeName"].ToString();
                printDataInterface.FlawGradeName = dst1.Tables[0].Rows[0]["FlawGradeName"].ToString();
                printDataInterface.HardGradeName = dst1.Tables[0].Rows[0]["HardGradeName"].ToString();
                printDataInterface.DensityGradeName = dst1.Tables[0].Rows[0]["DensityGradeName"].ToString();
                printDataInterface.SugarDegreeGradeName = dst1.Tables[0].Rows[0]["SugarDegreeGradeName"].ToString();
                printDataInterface.ProgramName = dst1.Tables[0].Rows[0]["ProgramName"].ToString();
                statisticsInfo.nWeightCount = Convert.ToUInt32(dst1.Tables[0].Rows[0]["BatchWeight"].ToString());
                statisticsInfo.nTotalCount = Convert.ToUInt32(dst1.Tables[0].Rows[0]["BatchNumber"].ToString());
                #endregion

                #region 往printDataInterface中插入等级信息
                //往printDataInterface中插入等级信息
                if (dst2.Tables[0].Rows.Count <= 0)
                {
                    //MessageBox.Show("选择条目的等级信息为空！");
                    //MessageBox.Show("30001104 The currently selected Grade information is empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("30001104 " + LanguageContainer.MainFormMessagebox13Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.MainFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                for (int i = 0; i < dst2.Tables[0].Rows.Count; i++)
                {
                    //存水果箱数
                    statisticsInfo.nBoxGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] = Convert.ToInt32(dst2.Tables[0].Rows[i]["BoxNumber"].ToString());
                    //存水果个数
                    statisticsInfo.nGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] = Convert.ToUInt32(dst2.Tables[0].Rows[i]["FruitNumber"].ToString());
                    //存水果重量
                    statisticsInfo.nWeightGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] = Convert.ToUInt32(dst2.Tables[0].Rows[i]["FruitWeight"].ToString());
                    //存品质名称
                    if (Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString()) == 0)
                    {
                        Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()),
                        0,
                        FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()).Length,
                        gradeInfo.strQualityGradeName,
                        i * ConstPreDefine.MAX_TEXT_LENGTH);
                        //取品质名称
                        //MessageBox.Show(Encoding.Default.GetString(gradeInfo.strQualityGradeName,
                        //    Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) * ConstPreDefine.MAX_TEXT_LENGTH,
                        //    ConstPreDefine.MAX_TEXT_LENGTH));

                        //存重量/尺寸名称
                        Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()),
                            0,
                            FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()).ToString().Length,
                            gradeInfo.strSizeGradeName,
                            i * ConstPreDefine.MAX_TEXT_LENGTH);
                    }
                    else  //有品质特征时：品质+尺寸  品质+重量
                    {
                        Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()),
                        0,
                        FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()).Length,
                        gradeInfo.strQualityGradeName,
                        (Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) / ConstPreDefine.MAX_SIZE_GRADE_NUM) * ConstPreDefine.MAX_TEXT_LENGTH);
                        //取品质名称
                        //MessageBox.Show(Encoding.Default.GetString(gradeInfo.strQualityGradeName,
                        //    Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) * ConstPreDefine.MAX_TEXT_LENGTH,
                        //    ConstPreDefine.MAX_TEXT_LENGTH));
                        //存重量/尺寸名称

                        Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()),
                            0,
                            FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()).ToString().Length,
                            gradeInfo.strSizeGradeName,
                            (Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) % ConstPreDefine.MAX_SIZE_GRADE_NUM) * ConstPreDefine.MAX_TEXT_LENGTH);
                    }
                    //存重量/尺寸限制
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nMinSize = (float)(Convert.ToDouble(Convert.ToDouble(dst2.Tables[0].Rows[i]["WeightOrSizeLimit"].ToString())));
                    //存重量/尺寸选择
                    if (Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString()) > 0)  //有品质
                    {
                        if (dst2.Tables[0].Rows[i]["SelectWeightOrSize"].ToString().Equals("0"))   //选尺寸
                        {
                            gradeInfo.nClassifyType = 4;
                        }
                        else  //选重量
                        {
                            gradeInfo.nClassifyType = 2;
                        }
                    }
                    else  //无品质
                    {
                        if (dst2.Tables[0].Rows[i]["SelectWeightOrSize"].ToString().Equals("0"))   //选尺寸
                        {
                            gradeInfo.nClassifyType = 4;
                        }
                        else  //选重量
                        {
                            gradeInfo.nClassifyType = 2;
                        }
                    }
                    //存重量/尺寸特征

                    //存颜色特征
                    if (dst2.Tables[0].Rows[i]["TraitColor"].ToString() == null || dst2.Tables[0].Rows[i]["TraitColor"].ToString().Equals(""))
                    {
                        gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nColorGrade = 0x7F;
                    }
                    else
                    {
                        gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nColorGrade = Convert.ToSByte(dst2.Tables[0].Rows[i]["TraitColor"].ToString());
                    }
                    //存形状特征
                    if (dst2.Tables[0].Rows[i]["TraitShape"].ToString() == null || dst2.Tables[0].Rows[i]["TraitShape"].ToString().Equals(""))
                    {
                        gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbShapeSize = 0x7F;
                    }
                    else
                    {
                        gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbShapeSize = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitShape"].ToString());
                    }
                    //存瑕疵特征
                    if (dst2.Tables[0].Rows[i]["TraitFlaw"].ToString() == null || dst2.Tables[0].Rows[i]["TraitFlaw"].ToString().Equals(""))
                    {
                        gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbFlawArea = 0x7F;
                    }
                    else
                    {
                        gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbFlawArea = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitFlaw"].ToString());
                    }
                    //存硬度特征
                    if (dst2.Tables[0].Rows[i]["TraitHard"].ToString() == null || dst2.Tables[0].Rows[i]["TraitHard"].ToString().Equals(""))
                    {
                        gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbRigidity = 0x7F;
                    }
                    else
                    {
                        gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbRigidity = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitHard"].ToString());
                    }
                    //存密度特征
                    if (dst2.Tables[0].Rows[i]["TraitDensity"].ToString() == null || dst2.Tables[0].Rows[i]["TraitDensity"].ToString().Equals(""))
                    {
                        gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbDensity = 0x7F;
                    }
                    else
                    {
                        gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbDensity = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitDensity"].ToString());
                    }
                    //存含糖量特征
                    if (dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString() == null || dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString().Equals(""))
                    {
                        gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbSugar = 0x7F;
                    }
                    else
                    {
                        gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbSugar = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString());
                    }

                }
                #endregion

                #region 往printDataInterface中插入出口信息
                //往printDataInterface中插入出口信息
                if (dst3.Tables[0].Rows.Count <= 0)
                {
                    //MessageBox.Show("选择条目的出口信息为空！");
                    //MessageBox.Show("30001105 The currently selected Outlets information is empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("30001105 " + LanguageContainer.MainFormMessagebox14Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.MainFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                for (int i = 0; i < dst3.Tables[0].Rows.Count; i++)
                {
                    statisticsInfo.nExitCount[Convert.ToInt32(dst3.Tables[0].Rows[i]["ExportID"].ToString())] = Convert.ToUInt32(dst3.Tables[0].Rows[i]["FruitNumber"].ToString());
                    statisticsInfo.nExitWeightCount[Convert.ToInt32(dst3.Tables[0].Rows[i]["ExportID"].ToString())] = Convert.ToUInt32(dst3.Tables[0].Rows[i]["FruitWeight"].ToString());
                }
                #endregion

                #region 往printDataInterface类中汇总结构体数据
                //往printDataInterface类中汇总数据
                printDataInterface.IoStStatistics = statisticsInfo;
                printDataInterface.IoStStGradeInfo = gradeInfo;
                #endregion

                #region 判断当前加工状态能否进行统计
                if (printDataInterface.StartedState.Equals("1") && printDataInterface.CompletedState.Equals("0"))
                {
                    //MessageBox.Show("加工进行中 不能进行统计！");
                    //MessageBox.Show("20001101 In the processing, can not be statistics!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    MessageBox.Show("20001101 " + LanguageContainer.MainFormMessagebox11Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                #endregion


                int index = 0;

                #region 尺寸 重量 尺寸品质 重量品质 空 等情况判断
                int selectType = printDataInterface.IoStStGradeInfo.nClassifyType;
                //if ((selectType & 1) == 1) //有品质
                if (printDataInterface.QualityGradeSum > 0)//if (printDataInterface.IoStStGradeInfo.nQualityGradeNum > 0) //有品质
                {
                    //if ((selectType >> 3 & 1) == 1 || (selectType >> 4 & 1) == 1 || (selectType >> 5 & 1) == 1) //有尺寸
                    if ((selectType & 0x1C) > 0)  //有尺寸
                    {
                        index = 3;
                    }
                    else                   //有重量
                    {
                        index = 4;
                    }
                }
                else                       //无品质
                {
                    //if ((selectType >> 3 & 1) == 1 || (selectType >> 4 & 1) == 1 || (selectType >> 5 & 1) == 1) //有尺寸
                    if ((selectType & 0x1C) > 0)  //有尺寸
                    {
                        index = 1;
                    }
                    else                   //有重量
                    {
                        index = 2;
                    }
                }
                if (selectType == 0)
                {
                    index = 0;
                }
                #endregion

                printOperate = new PrintOperate();

                switch (index)
                {
                    case 0:
                        break;
                    case 1:
                        try
                        {
                            showPrintDialog[0] = false;
                            printPreviewDialog1.Document = this.printDocument1;//设置要预览的文档
                            printPreviewDialog1.ShowDialog();//弹出打印预览对话框
                        }
                        catch (Exception ee)
                        {
                        }
                        break;
                    case 2:
                        try
                        {
                            showPrintDialog[1] = false;
                            printPreviewDialog1.Document = this.printDocument2;//设置要预览的文档
                            printPreviewDialog1.ShowDialog();//弹出打印预览对话框
                        }
                        catch (Exception ee)
                        {
                        }
                        break;
                    case 3:
                        try
                        {

                            showPrintDialog[2] = false;
                            printPreviewDialog1.Document = this.printDocument3;//设置要预览的文档
                            printPreviewDialog1.ShowDialog();//弹出打印预览对话框

                        }
                        catch (Exception ee)
                        {
                        }
                        break;
                    case 4:
                        try
                        {
                            showPrintDialog[3] = false;
                            printPreviewDialog1.Document = this.printDocument4;//设置要预览的文档
                            printPreviewDialog1.ShowDialog();//弹出打印预览对话框
                        }
                        catch (Exception ee)
                        {
                        }
                        break;
                    default:
                        break;


                }
            }
            catch (Exception ex)
            {

            }
        }

        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (showPrintDialog[0])
            {
                printDialog1.PrinterSettings.Collate = false;
                printDialog1.Document = this.printDocument1;//设置要打印的文档
                if (printDialog1.ShowDialog() == DialogResult.OK)//弹出打印选项对话框
                {
                    e.Cancel = true;
                    showPrintDialog[0] = false;
                    this.printDocument1.Print();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void printDocument1_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            showPrintDialog[0] = true;
        }

        //Add By ChengSk 20131120
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //#if REALEASE
            //            GlobalDataInterface.WriteErrorInfo(string.Format("printDocument1中最大打印分数为{0}，设置的打印分数为{1}", printDocument1.PrinterSettings.MaximumCopies, printDocument1.PrinterSettings.Copies));
            //#endif
            printOperate.printSize_PrintPage(sender, e, printDataInterface);
        }

        private void printDocument2_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (showPrintDialog[1])
            {
                printDialog1.PrinterSettings.Collate = false;
                printDialog1.Document = this.printDocument2;//设置要打印的文档
                if (printDialog1.ShowDialog() == DialogResult.OK)//弹出打印选项对话框
                {
                    e.Cancel = true;
                    showPrintDialog[1] = false;
                    this.printDocument2.Print();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void printDocument2_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            showPrintDialog[1] = true;
        }

        //Add By ChengSk 20131120
        private void printDocument2_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //#if REALEASE
            //            GlobalDataInterface.WriteErrorInfo(string.Format("printDocument2中最大打印分数为{0}，设置的打印分数为{1}", printDocument2.PrinterSettings.MaximumCopies, printDocument2.PrinterSettings.Copies));
            //#endif
            printOperate.printWeight_PrintPage(sender, e, printDataInterface);
        }

        private void printDocument3_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

            if (showPrintDialog[2])
            {
                printDialog1.PrinterSettings.Collate = false;
                printDialog1.Document = this.printDocument3;//设置要打印的文档
                if (printDialog1.ShowDialog() == DialogResult.OK)//弹出打印选项对话框
                {
                    e.Cancel = true;
                    showPrintDialog[2] = false;
                    this.printDocument3.Print();
                }
                else
                {
                    e.Cancel = true;
                }
            }

        }
        private void printDocument3_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            showPrintDialog[2] = true;
        }

        //Add By ChengSk 20131120
        private void printDocument3_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // int a = printDocument3.DefaultPageSettings.PrinterSettings.MaximumCopies;
            //#if REALEASE
            //            GlobalDataInterface.WriteErrorInfo(string.Format("printDocument3中最大打印分数为{0}，设置的打印分数为{1}", printDocument3.PrinterSettings.MaximumCopies, printDocument3.PrinterSettings.Copies));
            //#endif
            printOperate.printQualityOrSize_PrintPage(sender, e, printDataInterface);
        }


        private void printDocument4_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (showPrintDialog[3])
            {
                printDialog1.PrinterSettings.Collate = false;
                printDialog1.Document = this.printDocument4;//设置要打印的文档
                if (printDialog1.ShowDialog() == DialogResult.OK)//弹出打印选项对话框
                {
                    e.Cancel = true;
                    showPrintDialog[3] = false;
                    this.printDocument4.Print();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void printDocument4_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            showPrintDialog[3] = true;
        }

        //Add By ChengSk 20131120
        private void printDocument4_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //#if REALEASE
            //            GlobalDataInterface.WriteErrorInfo(string.Format("printDocument4中最大打印分数为{0}，设置的打印分数为{1}", printDocument4.PrinterSettings.MaximumCopies, printDocument4.PrinterSettings.Copies));
            //#endif
            printOperate.printQualityOrWeight_PrintPage(sender, e, printDataInterface);
        }

        //Add By ChengSk 20131119
        private void ClientInfoLabel_Click(object sender, EventArgs e)
        {
            ClientInfoForm clientInfoForm = new ClientInfoForm();
            clientInfoForm.ShowDialog(this);
        }

        //Add By ChengSk 20131119
        public void UpdateClientInfoState()
        {
            string CilentInfo = m_resourceManager.GetString("ClientInfoLabelCustomer.Text") + GlobalDataInterface.dataInterface.CustomerName + "\n"
                    + m_resourceManager.GetString("ClientInfoLabelGrower.Text") + GlobalDataInterface.dataInterface.FarmName + "\n"
                    + m_resourceManager.GetString("ClientInfoLabelVariety.Text") + GlobalDataInterface.dataInterface.FruitName;
            Font CilentInfoFont = new Font("宋体", 15, FontStyle.Bold);
            PictureBox picB = new PictureBox();
            Graphics TitG = picB.CreateGraphics();
            SizeF XMaxSize0 = TitG.MeasureString(m_resourceManager.GetString("ClientInfoLabelCustomer.Text") + GlobalDataInterface.dataInterface.CustomerName, CilentInfoFont);
            float tempWidth = XMaxSize0.Width;
            XMaxSize0 = TitG.MeasureString(m_resourceManager.GetString("ClientInfoLabelGrower.Text") + GlobalDataInterface.dataInterface.FarmName, CilentInfoFont);
            tempWidth = (tempWidth < XMaxSize0.Width ? XMaxSize0.Width : tempWidth);
            XMaxSize0 = TitG.MeasureString(m_resourceManager.GetString("ClientInfoLabelVariety.Text") + GlobalDataInterface.dataInterface.FruitName, CilentInfoFont);
            tempWidth = (tempWidth < XMaxSize0.Width ? XMaxSize0.Width : tempWidth);
            //float FontWidth = this.ClientInfoLabel.Width / this.ClientInfoLabel.Text.Length;
            //this.ClientInfoLabel.Width = (int)(FontWidth * 10);
            this.ClientInfoLabel.Width = (int)tempWidth;
            this.ClientInfoLabel.Text = CilentInfo;
            TitG.Dispose(); //Add 20180919
            TitG = null;    //Add 20180919
        }

        //Add By ChengSk 20131128
        private void 统计信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ProcessInfotoolStripButton_Click(sender, e);
            StatisticInfotoolStripButton_Click(sender, e);
        }

        private void 结束加工toolStripMenuItem_Click(object sender, EventArgs e)
        {
            EndProcesstoolStripButton_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool bUpLoaded;
            EventWaitHandle waitEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "UpLoad", out bUpLoaded);
            waitEvent.Set();
        }

        

        //private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //}


        //“设置”-“工程设置”是否可用 Add by ChengSk - 20190121
        public void SetProjectEnabledtoolStripMenuItem(bool bEnabled)
        {
            this.工程设置toolStripMenuItem.Enabled = bEnabled;
            this.MinimizeBox = bEnabled; //Add by ChengSk - 20190226
        }
    }
}
