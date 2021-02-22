using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using Interface;
using Common;
using System.Net.NetworkInformation;

namespace TcpIP
{
    class CTcpClient
    {
        
        #region 字段

        private bool[] m_IsConnected;
        private Mutex m_mutex;
        /// <summary>
        /// 客户端连接尝试次数 
        /// </summary>
        private int nTryConnectCount = 1;

        ///// <summary>
        ///// 客户端初始连接服务器超时时间（毫秒）
        ///// </summary>
        //private int nTryInitialConnectTime = 1000;
        ///// <summary>
        ///// 客户端初始连接服务器超时时间（毫秒）
        ///// </summary>
        //private int nTryConnectTime = 50;
        /// <summary>
        /// 客户端发送数据超时时间（毫秒）
        /// </summary>
        private int nTrySendTime = 1000;  //30->1000 Modify by ChengSk - 20180608
        /// <summary>
        /// 客户端是否已连接服务器
        /// </summary>
       // private bool IsConnected = false;

        /// <summary>
        /// socket实例化队列
        /// </summary>
        private List<Socket> SocketList;

        /// <summary>
        /// 等待服务器断开连接后关闭本地socket线程
        /// </summary>
        private Thread threadWaitSocketClose;//系统退出时需要关闭此线程

        /// <summary>
        /// HC发送命令/数据结构体
        /// </summary>
        private struct SendCMD
        {
            public int SYNC;
            public int nSrcId;
            public int nDestId;
            public int nCmd;
        }

        #endregion

        #region 构造
        public CTcpClient(ref bool[] bIsConnected)
        {
            m_IsConnected = bIsConnected;
            m_mutex = new Mutex();
        }
        #endregion

        #region 方法

        /// <summary>
        /// 线程锁锁住资源
        /// </summary>
        /// <param name="ulTimeout"></param>
        /// <returns></returns>
        private bool Lock(int nTimeout)
        {
            if (m_mutex == null) return false;
            else
            {
                try
                {
                    m_mutex.WaitOne(nTimeout);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 线程锁释放资源
        /// </summary>
        /// <returns></returns>
        private bool UnLock()
        {
            if (m_mutex == null) return false;
            else
            {
                m_mutex.ReleaseMutex();
                return true;
            }
        }

        /// <summary>
        /// 创建Socket
        /// </summary>
        /// <returns></returns>
        private Socket CreateSocket()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建Socket
            ////当一台电脑上有多个网卡是，TCP/IP通信绑定一个固定的IP地址
            //IPAddress localAddress = IPAddress.Parse(ConstPreDefine.HC_IP_ADDR);
            //IPEndPoint localIpPoint = new IPEndPoint(localAddress, 0);
            //Socket socket = new Socket(localIpPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);//创建Socket
            return socket;
        }


        /// <summary>
        /// 出口布局列表结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class ConnectServerParam
        {
            public Socket socket;
            public string IPaddress;
            public int PortNum;
            public bool IsConnected;

            public ConnectServerParam(Socket socketex,string iPaddress, int portNum)
            {
                this.socket = socketex;
                this.IPaddress = iPaddress;
                this.PortNum = portNum;
                this.IsConnected = false;
            }
        }


        Ping m_ping = new Ping();
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="socket"></param>socket对象
        /// <param name="IPaddress"></param>服务器IP地址
        /// <param name="PortNum"></param>服务器端口号
        /// <returns></returns>连接成功/失败
        private bool ConnectoServer(Socket socket, string IPaddress, int PortNum,bool IsInitialSystem)
        {
            

            //当一台电脑上有多个网卡是，TCP/IP通信绑定一个固定的IP地址
            IPAddress localAddress = IPAddress.Parse(ConstPreDefine.HC_IP_ADDR);
            IPEndPoint localIpPoint = new IPEndPoint(localAddress, 0);
            socket.Bind(localIpPoint);

           // IsConnected = false;
            //socket.NoDelay = true;
           // ConnectServerParam param = new ConnectServerParam(socket, IPaddress, PortNum);
            for(int i = 0; i < nTryConnectCount; i++)
            {
                try
                {
                    if (IsInitialSystem)
                    {
                        PingReply pingReply = m_ping.Send(IPaddress, 500);
                        if (pingReply.Status != IPStatus.Success)
                        {
                            Trace.WriteLine(string.Format("网络连接出错:无法ping通下位机{0}:{1}！", IPaddress, PortNum));
#if REALEASE
                            GlobalDataInterface.WriteErrorInfo(string.Format("网络连接出错:无法ping通下位机{0}:{1}！", IPaddress, PortNum));
#endif
                            return false;
                        }
                        else
                        {
                            Trace.WriteLine(string.Format("ping通下位机{0}:{1}！", IPaddress, PortNum));
#if REALEASE
                            GlobalDataInterface.WriteErrorInfo(string.Format("ping通下位机{0}:{1}！", IPaddress, PortNum));
#endif
                            
                        }
                    }
                    socket.Connect(IPaddress, PortNum);//连接服务器
                    return true;
                    //Thread thConnecttoServer = new Thread(new ParameterizedThreadStart(ConnecttoServerTimeOut));
                    //thConnecttoServer.Priority = ThreadPriority.Normal;
                    //thConnecttoServer.IsBackground = true;
                    //thConnecttoServer.Start((object)param);

                    //if (IsInitialSystem)
                    //    Thread.Sleep(nTryInitialConnectTime);
                    //else
                    //    Thread.Sleep(nTryConnectTime);

                    //if (param.IsConnected == true)
                    //{
                    //   // thConnecttoServer.Abort();
                    //    return true;
                    //}
                    //else
                    //{
                    //   // thConnecttoServer.Abort();
                    //    continue;
                    //}
                    
                }
                catch (Exception e)
                {
                    Trace.WriteLine("网络连接出错" + e);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("网络连接出错" + e);
#endif
                    continue;
                }
            }
            return false;
        }

//        private void ConnecttoServerTimeOut(object sender)
//        {
            
//            ConnectServerParam param = (ConnectServerParam)sender;

//            try
//            {
//                //lock (this)
//                //{

//                    if (!param.socket.Connected)
//                    {
//                        param.socket.Connect(param.IPaddress, param.PortNum);//连接服务器
//                        //IsConnected = true;
//                        param.IsConnected = true;
//                    }
//                //}
//            }
//            catch (Exception e)
//            {
//                Trace.WriteLine(string.Format("网络连接出错,IP{0},PortNum{1}", param.IPaddress, param.PortNum) + e);
//#if REALEASE
//                GlobalDataInterface.WriteErrorInfo(string.Format("网络连接出错,IP{0},PortNum{1}", param.IPaddress, param.PortNum) + e);
//#endif                
//            }

//        }
        /// <summary>
        /// 断开服务器
        /// </summary>
        private void DestroySocket(Socket socket, bool sign)
        {
            if (socket != null)
            {
                //lock (this)
                //{
                    if (sign)
                    {
                        socket.Shutdown(SocketShutdown.Send);
                        WaitSocketClose(socket);
                        Trace.WriteLine("Socket正常等待服务器通知关闭");
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo("Socket正常等待服务器通知关闭");
#endif
                    }
                    else
                    {
                        socket.Close();
                        Trace.WriteLine("Socket不正常关闭");
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo("Socket不正常关闭");
#endif
                    }
                //}
            }
        }
        /// <summary>
        /// 等待服务器断开连接
        /// </summary>
        /// <param name="socket"></param>
        private void WaitSocketClose(Socket socket)
        {
            if (SocketList == null)
            {
                SocketList = new List<Socket>();
            }
            SocketList.Add(socket);
            if (threadWaitSocketClose == null)
            {
                threadWaitSocketClose = new Thread(WaitSocketCloseThread);
                threadWaitSocketClose.IsBackground = true;//设置为后台线程，当所有前台线程停止运行时，会强制终止后台线程
                threadWaitSocketClose.Start();
            }
        }

        /// <summary>
        /// 关闭Socket线程
        /// </summary>
        private void WaitSocketCloseThread()
        {
            try
            {
                while (SocketList.Count > 0)
                {
                    byte[] temp = new byte[1];

                    Socket iter = (Socket)SocketList[SocketList.Count - 1];
                    int iResult = iter.Receive(temp);
                    while (iResult > 0) iResult = iter.Receive(temp);
                    Thread.Sleep(1);
                    iter.Close();
                    SocketList.Remove(iter);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("CTcpClient类中函数WaitSocketCloseThread出错"+e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("CTcpClient类中函数WaitSocketCloseThread出错"+e);
#endif
            }
        }

        ////测试获取图像发送命令时间
        //[System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        //static extern bool QueryPerformanceCounter(ref long count);
        //[System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        //static extern bool QueryPerformanceFrequency(ref long count);
        //long count = 0;
        //long count1 = 0;
        //long freq = 0;
        //double result = 0;


        /// <summary>
        /// 网络发送数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="bytes"></param>发送数据
        /// <returns></returns>
        private bool Send(Socket socket, byte[] bytes)
        {
            
            if (bytes.Length == 0)
            {
                return false;
            }

            int nRemain = bytes.Length;
            int nSentByte;
            int nOffset = 0;

            while (nRemain > 0)
            {
                try
                {
                    //QueryPerformanceFrequency(ref freq);
                    //QueryPerformanceCounter(ref count);

                   // nSentByte = socket.Send(bytes, nOffset, bytes.Length, 0);//发送数据
                    nSentByte = socket.Send(bytes, nOffset, bytes.Length, 0);//发送数据
                    //测试获取图像命令发送时间
                    //QueryPerformanceCounter(ref count1);
                    //count = count1 - count;
                    //result = (double)(count) / (double)freq;
                    //Trace.WriteLine("发送时间：" + result);

                    if (nSentByte > 0)
                    {
                        nRemain -= nSentByte;
                        nOffset += nSentByte;
                    }
                }
                catch(Exception e)
                {
                    Trace.WriteLine("网络发送失败"+e);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("网络发送失败"+e);
#endif
                    break;
                }
            }
            if (nRemain == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        

        /// <summary>
        /// 一对一发送命令/数据
        /// </summary>
        /// <param name="nDestId"></param>命令/数据目的ID
        /// <param name="nCmd"></param>命令
        /// <param name="rData"></param>数据
        /// <returns></returns>
        public bool SyncRequest(int nDestId, int nCmd, byte[] Data)
        {
            bool boRC = false;//运行正常与否标志位
            Socket socket;

            if (nCmd != (int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_OFF
                && nCmd != (int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_ON
               && !SubsysIsConnected(Commonfunction.GetSubsysIndex(nDestId)))
            {
                Trace.WriteLine(string.Format("当前子系统不可用，函数SyncRequest nDestId:{0},nCmd={1}", nDestId, nCmd));
#if REALEASE
                GlobalDataInterface.WriteErrorInfo(string.Format("当前子系统不可用，函数SyncRequest nDestId:{0},nCmd={1}", nDestId, nCmd));
#endif
                return false;
            }
            //C++: AfxGetApp()->BeginWaitCursor();

            string strIP = "";
            int nSubsysId = Commonfunction.GetSubSysID(nDestId);
            int nPortNum = ConstPreDefine.FSM_PORT_NUM;
            
            if (nCmd >= 0x2000 && nCmd < 0x3000) //判断为HC->IPM的发送命令
            {
                nPortNum = ConstPreDefine.IPM_PORT_NUM;
                nSubsysId = Commonfunction.GetIPMID(nDestId);
				
            }           
			string strTemp = ConstPreDefine.LC_IP_ADDR_TEMPLATE;
           	strIP = strTemp + nSubsysId;//得到发送的IP地址
            //if (nCmd >= 0x7100 && nCmd <= 0x7101) //判断为HC->SIM的发送命令
            if (nCmd >= 0x7000 && nCmd <= 0x7102) //判断为HC->SIM的发送命令
            {
                nPortNum = ConstPreDefine.SIM_PORT_NUM;
                strIP = ConstPreDefine.SIM_IP_ADDR;//得到发送的IP地址
            }

            try
            {
                Lock(1000);//资源加锁
                

                socket = CreateSocket();//创建socket
                socket.SendBufferSize = 0;//网络发送缓冲区为0
                socket.SendTimeout = nTrySendTime;//设置发送超时时间
                socket.NoDelay = false;

                //if (nCmd == (int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_ON)
                //    boRC = ConnectoServer(socket, strIP, nPortNum, true);//初始连接
                //else
                    //boRC = ConnectoServer(socket, strIP, nPortNum, false);//运行状态下连接
                boRC = ConnectoServer(socket, strIP, nPortNum, true);//2015-5-14 运行过程中会存在网线断了的情况


                if (!boRC)
                {
                    Trace.WriteLine("网络连接失败！");
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo(string.Format("网络连接失败！"));
#endif
                    UnLock();

                    socket.Close();
                    socket = null;
                    Trace.WriteLine("Socket关闭");
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("Socket关闭");
#endif
                    return boRC;
                }
                //byte[] ABC = new byte[1];
                //ABC[0] = 100;
                //if (nCmd == (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_ON)
                //{
                //    QueryPerformanceFrequency(ref freq);
                //    QueryPerformanceCounter(ref count);
                //    Trace.WriteLine("发送连续采集前同步信息开始");
                //    boRC = Send(socket, ABC);//发送同步信息
                //    Trace.WriteLine("发送连续采集前同步信息结束");
                //}
                SendCMD cmd = new SendCMD();
               
                cmd.SYNC = 0x434e5953;
                cmd.nSrcId = ConstPreDefine.HC_ID;//发送源ID
                cmd.nDestId = nDestId;//发送目的ID
                cmd.nCmd = nCmd;

                int nStructLen = 4 * sizeof(int);
                byte[] bytes = new byte[nStructLen];
                bytes = Commonfunction.StructToBytes(cmd);//将结构体转化为byte数组
                //if (nCmd == (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_ON)
                //    Trace.WriteLine("发送连续采集命令头开始");
                boRC = Send(socket, bytes);//发送命令头
                //if (nCmd == (int)HC_IPM_COMMAND_TYPE.HC_CMD_CONTINUOUS_SAMPLE_ON)
                //{
                //    //socket.RE
                //    Trace.WriteLine("发送连续采集命令头结束");
                //}
                ////测试获取图像命令发送时间
                //QueryPerformanceCounter(ref count1);
                //count = count1 - count;
                //result = (double)(count) / (double)freq;
                //Trace.WriteLine("发送连续采集同步信息+命令头时间：" + result);
                if (!boRC)
                {
                    Trace.WriteLine("网络发送命令头错误！");
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo(string.Format("网络发送命令头错误！"));
#endif
                    goto leave;
                }

                int A = BitConverter.ToInt32(bytes, 12);

                if (Data != null)
                {
                    
                    boRC = Send(socket, Data);//发送数据
                    
                    if (!boRC)
                    {
                        Trace.WriteLine("网络发送数据错误！");
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo(string.Format("网络发送数据错误！"));
#endif
                        goto leave;
                    }
                }
                
               
            leave:
                DestroySocket(socket, boRC);
                UnLock();
                //AfxGetApp()->EndWaitCursor();
                if (boRC)
                {
                    if (Data != null)
                    {
                        Trace.WriteLine(string.Format("成功：函数SyncRequest nDestId:{0},nCmd={1},数据长度={2}", nDestId, nCmd, Data.Length));
//#if REALEASE
//                        GlobalDataInterface.WriteErrorInfo(string.Format("成功：函数SyncRequest nDestId:{0},nCmd={1},数据长度={2}", nDestId, nCmd, Data.Length));
//#endif
                    }
                    else
                    {
                        Trace.WriteLine(string.Format("成功：函数SyncRequest nDestId:{0},nCmd={1}", nDestId, nCmd));
//#if REALEASE
//                        GlobalDataInterface.WriteErrorInfo(string.Format("成功：函数SyncRequest nDestId:{0},nCmd={1}", nDestId, nCmd));
//#endif   //delete by xcw 20201207
                    }
                }
                else
                {
                    if(nCmd == (int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_ON)
                    {
                        m_IsConnected[Commonfunction.GetSubsysIndex(nDestId)] = false;
                    }
                    if (Data != null)
                    {
                        Trace.WriteLine(string.Format("失败：函数SyncRequest nDestId:{0},nCmd={1},数据长度={2},当前时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), nDestId, nCmd, Data.Length));
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo(string.Format("失败：函数SyncRequest nDestId:{0},nCmd={1},数据长度={2},当前时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), nDestId, nCmd, Data.Length));
#endif
                    }
                    else
                    {
                        Trace.WriteLine(string.Format("失败：函数SyncRequest nDestId:{0},nCmd={1},当前时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), nDestId, nCmd));
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo(string.Format("失败：函数SyncRequest nDestId:{0},nCmd={1},当前时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), nDestId, nCmd));
#endif   //add by xcw 20201207
                    }
                }
            }
            catch(Exception e)
            {
                Trace.WriteLine("失败：函数SyncRequest" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("失败：函数SyncRequest" + e);
#endif
                return false;
            }

            return boRC;
        }

        /// <summary>
        /// 所有下位机命令/数据发送
        /// </summary>
        /// <param name="nCmd"></param> 命令
        /// <param name="Data"></param> 数据
        /// <param name="bChannelInfo"></param> 子系统通道是否有效 stSysConfig.nChannelInfo
        /// <returns></returns>是否执行成功 0成功 不成功返回相应子系统ID 
        public int AllSysSyncRequest(int nCmd, byte[] Data,byte[] bChannelInfo)
        {
            List<int> arrayID = new List<int>();
            int result = 0;//成功

            if (nCmd != (int)HC_FSM_COMMAND_TYPE.HC_CMD_SYS_CONFIG)
            {
                Commonfunction.GetAllSysID(bChannelInfo, ref arrayID);
            }
            else
            {
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    {
                        //if (bChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                        if (bChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                        {
                            int id = Commonfunction.EncodeSubsys(i);
                            arrayID.Add(id);
                            break;
                        }
                        stSysConfig sys = new stSysConfig(true);
                        sys=(stSysConfig)Commonfunction.BytesToStruct(Data,typeof(stSysConfig));
                        //if (sys.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                        if (sys.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                        {
                            int id = Commonfunction.EncodeSubsys(i);
                            arrayID.Add(id);
                            break;
                        }
                    }
                }
            }
            Trace.WriteLine(string.Format("函数 AllSysSyncRequest arrayID.Count:{0}", arrayID.Count));
#if REALEASE
            GlobalDataInterface.WriteErrorInfo(string.Format("函数 AllSysSyncRequest arrayID.Count:{0}", arrayID.Count));
#endif
            for (int i = 0; i < arrayID.Count; i++)
            {
                int nDestId = arrayID[i];
                if (!SubsysIsConnected(Commonfunction.GetSubsysIndex(nDestId)))
                {
                    Trace.WriteLine(string.Format("当前子系统不可用，函数SyncRequest nDestId:{0},nCmd={1}", nDestId, nCmd));
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo(string.Format("当前子系统不可用，函数SyncRequest nDestId:{0},nCmd={1}", nDestId, nCmd));
#endif
                    continue;
                }
                if (!SyncRequest(nDestId, nCmd, Data)) 
                    result=nDestId; 
            }
            return result;
        }

        /// <summary>
        /// 判断子系统是否连接
        /// </summary>
        /// <param name="nSubsysIdx"></param>子系统序列
        /// <returns></returns>
        private bool SubsysIsConnected(int nSubsysIdx)
        {
            if (nSubsysIdx == -1)
            {
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    if (m_IsConnected[i]) return true;
                }
                return false;
            }
            else
                return m_IsConnected[nSubsysIdx];
        }
        #endregion

    }
}
