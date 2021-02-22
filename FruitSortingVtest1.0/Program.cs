
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Interface;
using System.Runtime.InteropServices;
using Common;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Data.SqlClient;
using FruitSortingVtest1.DB;

namespace FruitSortingVtest1._0
{
    static class Program
    {
        /// <summary>
        /// 启动界面
        /// </summary>
        public static SplashForm m_splashForm
        {
            get;
            set;
        }

        static bool IsSystemContinue = true;
        /// <summary>
        /// 系统公共接口类
        /// </summary>
        static GlobalDataInterface global = new GlobalDataInterface();
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                //处理未捕获的异常 Add by ChengSk - 20180809
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理  UI线程异常 Add by ChengSk - 20180809
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //处理非UI线程异常 Add by ChengSk - 20180809
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandleException);

                // nLength = Marshal.SizeOf(typeof(stWeightBaseInfo));
#if REALEASE
                GlobalDataInterface.CreatErorrFile();
#endif

                // int nLength = Marshal.SizeOf(typeof(stGlobal));

                //语言选择功能
                GlobalDataInterface.selectLanguage = Common.Commonfunction.GetAppSetting("选择的语言");

                //string path; //Add by xcw - 20191031        
                //string currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
                //PrintProtocol.logoPathName = currentPath + PrintProtocol.logoPathName;  //更改LOGO标签地址(相对地址转绝对地址)
                //string currentDefaultPath = currentPath + "config\\";
                //path = currentDefaultPath + "languagekind.txt"; //Add by xcw - 20191031
                //if (!Directory.Exists(currentDefaultPath))
                //{
                //    Directory.CreateDirectory(currentDefaultPath);
                //}

                //if (!File.Exists(path))
                //{
                //    FileStream fs = File.Create(path);
                //    fs.Close();
                //}


                //StreamReader sr = new StreamReader(path, Encoding.Default);
                //string m = sr.ReadToEnd();//显示内容
                //m = m.Replace("\r\n", ""); //Add by xcw - 20191031
                //sr.Close();
                //GlobalDataInterface.selectLanguage = m;
                //MessageBox.Show(GlobalDataInterface.selectLanguage);
                if (GlobalDataInterface.selectLanguage == ""|| GlobalDataInterface.selectLanguage == "null")
                //if (true)
                {
                    LanguageSelectForm languageSelectForm = new LanguageSelectForm();
                    languageSelectForm.ShowDialog();
                }
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(GlobalDataInterface.selectLanguage);
                GlobalDataInterface.selectLanguageIndex = LanguageContainer.LanguageVersionIndex(GlobalDataInterface.selectLanguage);

                //信息提示框按钮附名称
                //MessageBoxManager.OK = "OK";
                //MessageBoxManager.No = "No";
                //MessageBoxManager.Yes = "Yes";
                //MessageBoxManager.Cancel = "Cancel";
                //MessageBoxManager.Retry = "Retry";
                //MessageBoxManager.Ignore = "Ignore";
                //MessageBoxManager.Abort = "Abort";
                //MessageBoxManager.Register();
                MessageBoxManager.OK = LanguageContainer.ProgramMessageBoxManagerOK[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.No = LanguageContainer.ProgramMessageBoxManagerNo[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.Yes = LanguageContainer.ProgramMessageBoxManagerYes[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.Cancel = LanguageContainer.ProgramMessageBoxManagerCancel[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.Retry = LanguageContainer.ProgramMessageBoxManagerRetry[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.Ignore = LanguageContainer.ProgramMessageBoxManagerIgnore[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.Abort = LanguageContainer.ProgramMessageBoxManagerAbort[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.Register();

                ////显示启动界面线程
                Thread thSplash = new Thread(new ThreadStart(ShowSplashForm));
                thSplash.Priority = ThreadPriority.Normal;
                thSplash.IsBackground = true;
                thSplash.Start();
                //初始化系统线程
                Thread thInitialSysterm = new Thread(new ThreadStart(InitialSysterm));
                thInitialSysterm.Priority = ThreadPriority.Highest;
                thInitialSysterm.IsBackground = true;
                thInitialSysterm.Start();
                thInitialSysterm.Join();

                if (m_splashForm != null)
                {
                    m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.Close(); }));
                }
                thSplash.Join();

                //模拟测试广播通信代码 正常工作时注释掉
                Thread thSendBroadcast = new Thread(new ThreadStart(sendBroadcast));
                thSendBroadcast.IsBackground = true;
                thSendBroadcast.Start();

                if (IsSystemContinue)
                {
                    ////语言选择功能
                    //GlobalDataInterface.selectLanguage = Common.Commonfunction.GetAppSetting("选择的语言"); //Note by ChengSk - 20180723 代码放到网络连接之前
                    //if (GlobalDataInterface.selectLanguage == "null")
                    //{
                    //    LanguageSelectForm languageSelectForm = new LanguageSelectForm();
                    //    languageSelectForm.ShowDialog();
                    //}
                    //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(GlobalDataInterface.selectLanguage);
                    //GlobalDataInterface.selectLanguageIndex = LanguageContainer.LanguageVersionIndex(GlobalDataInterface.selectLanguage);

                    //数据库配置功能  Add by ChengSk 20140320  ---START---
                    GlobalDataInterface.currentDatabase = Common.Commonfunction.GetAppSetting("当前数据库");
                    GlobalDataInterface.dataBaseConn = "data source=" + Common.Commonfunction.GetAppSetting("数据源") +
                        ";database=" + Common.Commonfunction.GetAppSetting("数据库") +
                        ";user=" + Common.Commonfunction.GetAppSetting("用户名") +
                        ";pwd=" + Common.Commonfunction.GetAppSetting("密码");
                    bool isConnSuccess = false;
                    SqlConnection conn = new SqlConnection(GlobalDataInterface.dataBaseConn);
                    try
                    {
                        conn.Open();
                        isConnSuccess = true;
                        conn.Close();   //Add 20180919
                        conn.Dispose(); //Add 20180919
                    }
                    catch (Exception ex)
                    {
                        isConnSuccess = false;
                    }
                    if (GlobalDataInterface.currentDatabase == "null" || !isConnSuccess)
                    {
                        DatabaseSetForm databaseSetForm = new DatabaseSetForm();
                        databaseSetForm.ShowDialog();
                        if (GlobalDataInterface.DatabaseSet)
                        {
                            return;
                        }
                    }
                    GlobalDataInterface.databaseOperation = new DB.DataBaseOperation();
                    // ---END-- -


                    GlobalDataInterface.mainform = new MainForm();
                    Application.Run(GlobalDataInterface.mainform);
                }

                global.DestroyTCPServerMarsterSocket(); //Add by ChengSk - 20180723
#if REALEASE
                System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
                int handleCount = p.HandleCount;  //程序的句柄数     Add by ChengSk - 20180906
                GlobalDataInterface.WriteErrorInfo("程序即将退出...，当前句柄数量：" + handleCount.ToString());
                GlobalDataInterface.CloseErorrFile();
#endif
                Application.ThreadException -= new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(CurrentDomain_UnhandleException);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Program中函数Main出错：" + ex + "\n代码定位：" + ex.StackTrace);
#if REALEASE
                System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
                int handleCount = p.HandleCount;  //程序的句柄数     Add by ChengSk - 20180906
                GlobalDataInterface.WriteErrorInfo("程序即将退出...，当前句柄数量：" + handleCount.ToString());
                GlobalDataInterface.WriteErrorInfo(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "Program中函数Main出错：" + ex + "\n代码定位：" + ex.StackTrace);
#endif
            }
        }

        /// <summary>
        /// 打印  UI线程异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
#if REALEASE
            GlobalDataInterface.WriteErrorInfo(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " 5555555 Application_ThreadException: " + e.Exception.ToString());
#endif
        }

        /// <summary>
        /// 处理非UI线程异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandleException(object sender, UnhandledExceptionEventArgs e)
        {
#if REALEASE
            GlobalDataInterface.WriteErrorInfo(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " 5555555 CurrentDomain_UnhandleException: " + e.ExceptionObject.ToString());
#endif
        }

        /// <summary>
        /// 显示启动界面线程函数
        /// </summary>
        private static void ShowSplashForm()
        {
            m_splashForm = new SplashForm();
            Application.Run(m_splashForm);
        }

        /// <summary>
        /// 初始化系统线程
        /// </summary>
        private static void InitialSysterm()
        {
            try
            {
                //信息提示框按钮附名称
                //MessageBoxManager.OK = "OK";
                //MessageBoxManager.No = "No";
                //MessageBoxManager.Yes = "Yes";
                //MessageBoxManager.Cancel = "Cancel";
                //MessageBoxManager.Retry = "Retry";
                //MessageBoxManager.Ignore = "Ignore";
                //MessageBoxManager.Abort = "Abort";
                //MessageBoxManager.Register();
                MessageBoxManager.OK = LanguageContainer.ProgramMessageBoxManagerOK[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.No = LanguageContainer.ProgramMessageBoxManagerNo[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.Yes = LanguageContainer.ProgramMessageBoxManagerYes[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.Cancel = LanguageContainer.ProgramMessageBoxManagerCancel[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.Retry = LanguageContainer.ProgramMessageBoxManagerRetry[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.Ignore = LanguageContainer.ProgramMessageBoxManagerIgnore[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.Abort = LanguageContainer.ProgramMessageBoxManagerAbort[GlobalDataInterface.selectLanguageIndex];
                MessageBoxManager.Register();

                Thread.Sleep(1000);
                if (m_splashForm != null)
                {
                   // m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text = "检查本地IP..."; }));
                    //m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text = "Check the local IP..."; }));
                    m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text =
                        LanguageContainer.ProgramMessagebox1Text[GlobalDataInterface.selectLanguageIndex];
                    }));
                }
                //检查IP地址
                string hostName = Dns.GetHostName();
                IPHostEntry IPEntry = Dns.GetHostEntry(hostName);
                IPAddress HCIP = IPAddress.Parse(ConstPreDefine.HC_IP_ADDR);
                string currentIP = "";

                //for (int i = 0; i < IPEntry.AddressList.Length; i++)
                //{
                //    if (IPEntry.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                //    {
                //        currentIP = IPEntry.AddressList[i].ToString();
                //        break;
                //    }
                //}
                //if (!string.Equals(currentIP, ConstPreDefine.HC_IP_ADDR))
                //{
                //    //MessageBox.Show("IP地址错误或没有网络连接！");
                //    MessageBox.Show("0x10000001 IP address error or no network!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                //    IsSystemContinue = false;
                //    return;
                //}

                //解决多网卡的问题
                bool IsExistCorrentIP = false;
                for (int i = 0; i < IPEntry.AddressList.Length; i++)
                {
                    if (IPEntry.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        currentIP = IPEntry.AddressList[i].ToString();
                        if (string.Equals(currentIP, ConstPreDefine.HC_IP_ADDR))
                        {
                            IsExistCorrentIP = true;
                            break;
                        }
                    }
                }
                if (!IsExistCorrentIP)
                {
                    //MessageBox.Show("IP地址错误或没有网络连接！");
                    //MessageBox.Show("0x10000001 IP address error or no network!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show("0x10000001 " +
                        LanguageContainer.ProgramMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ProgramMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    IsSystemContinue = false;
                    return;
                }

                //建立网络连接
                if (m_splashForm != null)
                {
                    //m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text = "正在建立网络连接..."; }));
                    //m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text = "Establishing network..."; }));
                    m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text =
                        LanguageContainer.ProgramMessagebox3Text[GlobalDataInterface.selectLanguageIndex];
                    }));
                }
                global.Init();//网络初始化
                Thread.Sleep(1000);

                //建立数据库连接


                // 查询子系统
                if (m_splashForm != null)
                {
                    //m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text = "正在查询子系统..."; }));
                    //m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text = "Checking the Subsystems..."; }));
                    m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text =
                        LanguageContainer.ProgramMessagebox4Text[GlobalDataInterface.selectLanguageIndex];
                    }));
                }
                string strSubSystemNum = Commonfunction.GetAppSetting("子系统个数");
                GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_ON, null);
                if (GlobalDataInterface.SubSystemIsConnected[0] == false)
                {
                    //MessageBox.Show("子系统1连接失败，请检查连接！");
                    //MessageBox.Show("0x10000003 Subsystem 1 connection failure,please check the connection!", "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
                    MessageBox.Show("0x10000003 初始化" + LanguageContainer.ProgramMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ProgramMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //MessageBox.Show("初始化");
                    IsSystemContinue = false;
                    return;
                }
                if (GlobalDataInterface.ConnectSystemNum == 0)
                {
                    //MessageBox.Show("连接下位机失败，请检查连接！");
                    //MessageBox.Show("0x10000002 FSM connection failure,please check the connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show("0x10000002 " + LanguageContainer.ProgramMessagebox6Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ProgramMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    IsSystemContinue = false;
                    return;
                }

                if (GlobalDataInterface.ConnectSystemNum < byte.Parse(strSubSystemNum))
                {
                    //DialogResult result = MessageBox.Show("子系统与配置数目不符，是否继续运行？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    //DialogResult result = MessageBox.Show("0x20001001 Configuration does not match the number of subsystems,whether to continue to run?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    DialogResult result = MessageBox.Show("0x20001001 " + LanguageContainer.ProgramMessagebox7Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ProgramMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        IsSystemContinue = false;
                        return;
                    }
                }
                if (m_splashForm != null)
                {
                    //m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text = "正在启动..."; }));
                    //m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text = "Starting..."; }));
                    m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text =
                        LanguageContainer.ProgramMessagebox8Text[GlobalDataInterface.selectLanguageIndex];
                    }));
                }
                Thread.Sleep(2000);
                // m_splashForm.Invoke(new MethodInvoker(delegate { m_splashForm.InitialStatuslabel.Text = "完成"; }));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Program中函数InitialSysterm出错" + ex + "\n代码定位：" + ex.StackTrace);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Program中函数InitialSysterm出错：" + ex + "\n代码定位：" + ex.StackTrace);
#endif
            }
        }

        //测试广播
        private static void sendBroadcast()
        {
            global.InitSendBroadcast();
        }

    }
}
