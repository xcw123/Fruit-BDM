using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Interface;

namespace TcpIP
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CommandHead
    {
        public int nCmdId;
        public int nSrcId;
    }

    class CTcpServer
    {
        #region 字段

        private SetBufferFuncDelegate m_fxnSetBuffer;   //获取数据方法（FSM/IPM）
        private int m_nPort;//端口号（FSM/IPM）
        private bool m_bRunOnce;
        private Thread m_serverThread;
        private Socket m_MasterSocket;
        private Socket m_ClientSocket;
        private bool m_IsRuning;
        private Mutex m_mutex;

        

        public static Queue<CommandHead> CommandHeadQue;//数据接收队列（FSM/IPM）


        #endregion

        #region 构造
        public CTcpServer()
        {
            m_MasterSocket = null;
            m_ClientSocket = null;
            m_IsRuning = true;
            m_serverThread = null;
            m_mutex = new Mutex();
            if (CommandHeadQue == null)
                CommandHeadQue =new Queue<CommandHead>();
        }
        #endregion

        #region 方法
      //  public delegate void NotifyFuncDelegate(int nSrcId, ref IntPtr pData);
        public delegate void SetBufferFuncDelegate(int nSrcId, int nCmdId, IntPtr pData);

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
        /// 创建/启动Tcp服务线程
        /// </summary>
        /// <param name="nPort"></param>
        /// <param name="func"></param>
        /// <param name="bRunOnce"></param>
        /// <returns></returns>
        public bool Start(int nPort, SetBufferFuncDelegate func, bool bRunOnce)
        {
            m_fxnSetBuffer = func;
            m_nPort = nPort;
            m_bRunOnce = bRunOnce;

            if (m_serverThread == null)
            {
                m_serverThread = new Thread(TcpServerThread);
                m_serverThread.Priority = ThreadPriority.Normal;
                m_serverThread.IsBackground = true;
                m_serverThread.Start();
            }
            return true;
        }

        /// <summary>
        /// 接收线程
        /// </summary>
        /// <returns></returns>
        private void TcpServerThread()
        {
            try
            {
                if (m_fxnSetBuffer == null) return;

                // NotifyFuncDelegate fxNotify = null;
                IntPtr pData = IntPtr.Zero;
                CommandHead head;
                head.nCmdId = -1;
                head.nSrcId = -1;
                int nLength = 0;
                // int TotalDataLength = 0;

                bool rc;

                m_MasterSocket = CreateMasterSocket(1);
                if (m_MasterSocket == null)
                {
                    rc = false;
                    goto leave;
                }
                rc = true;
                while (m_IsRuning)
                {
                    m_ClientSocket = CreateClientSocket(m_MasterSocket, 500);  //1->500 Modify by ChengSk - 20180801
                    if (m_ClientSocket == null) goto leave;

                    rc = RecvCommand(m_ClientSocket, ref head.nCmdId, ref head.nSrcId);//接收命令头
                    if(head.nCmdId== 0x1000)
                    {
                        GlobalDataInterface.uAcceptGlobal = true;
                    }
                    if (rc)
                    {
                      
                        switch (head.nCmdId)
                        {
                            case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_CONFIG:
                                nLength = Marshal.SizeOf(typeof(stGlobal));
                                pData = Marshal.AllocHGlobal(nLength);
                                break;
                            case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_STATISTICS:
                                nLength = Marshal.SizeOf(typeof(stStatistics));
                                pData = Marshal.AllocHGlobal(nLength);
                                break;
                            case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_GRADEINFO:
                                nLength = Marshal.SizeOf(typeof(stFruitGradeInfo));
                                pData = Marshal.AllocHGlobal(nLength);
                                break;
                            case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_WEIGHTINFO:
                                nLength = Marshal.SizeOf(typeof(stWeightResult));
                                pData = Marshal.AllocHGlobal(nLength);
                                break;
                            case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_WAVEINFO:
                                nLength = Marshal.SizeOf(typeof(stWaveInfo));
                                pData = Marshal.AllocHGlobal(nLength);
                                break;
                            case (int)FSM_HC_COMMAND_TYPE.FSM_CMD_VERSIONERROR:
                                nLength = sizeof(int);
                                pData = Marshal.AllocHGlobal(nLength);
                                break;
                            case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_IMAGE_SPOT:
                                //读取数据包大小
                                nLength = sizeof(int);
                                pData = Marshal.AllocHGlobal(nLength);
                                rc = RecvData(m_ClientSocket, ref pData, nLength);//接收数据包大小参数
                                // nLength = 0;
                                nLength = Marshal.ReadInt32(pData, 0);//获取数据包大小
                                pData = Marshal.AllocHGlobal(nLength);
                                //rc = RecvData(m_ClientSocket, ref pData, TotalDataLength);//接收实际数据   
                                break;
                            case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_IMAGE:
                                //读取数据包大小
                                nLength = sizeof(int);
                                pData = Marshal.AllocHGlobal(nLength);
                                rc = RecvData(m_ClientSocket, ref pData, nLength);//接收数据包大小参数
                                //nLength = 0;
                                nLength = Marshal.ReadInt32(pData, 0);//获取数据包大小
                                pData = Marshal.AllocHGlobal(nLength);
                                //rc = RecvData(m_ClientSocket,ref pData,TotalDataLength);//接收实际数据                        
                                break;
                            case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_IMAGE_SPLICE:
                                //读取数据包大小
                                nLength = sizeof(int);
                                pData = Marshal.AllocHGlobal(nLength);
                                rc = RecvData(m_ClientSocket, ref pData, nLength);//接收数据包大小参数
                                //nLength = 0;
                                nLength = Marshal.ReadInt32(pData, 0);//获取数据包大小
                                pData = Marshal.AllocHGlobal(nLength);
                                //rc = RecvData(m_ClientSocket,ref pData,TotalDataLength);//接收实际数据                        
                                break;
                            case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_AUTOBALANCE_COEFFICIENT:
                                nLength = Marshal.SizeOf(typeof(stWhiteBalanceCoefficient));
                                pData = Marshal.AllocHGlobal(nLength);
                                break;
                            case (int)IPM_HC_COMMAND_TYPE.IPM_CMD_SHUTTER_ADJUST: //Add by ChengSk - 20190627
                                nLength = Marshal.SizeOf(typeof(stShutterAdjust));
                                pData = Marshal.AllocHGlobal(nLength);
                                break;
                            case (int)SIM_HMI_COMMAND_TYPE.SIM_HMI_DISPLAY_ON: //Add by xcw - 20200520  
                                nLength = 0;
                                pData = Marshal.AllocHGlobal(nLength);
                                GlobalDataInterface.TransmitParam(ConstPreDefine.SIM_ID, (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_GRADE_INFO, null);//更新到SIM
                                break;
                            case (int)SIM_HMI_COMMAND_TYPE.SIM_HMI_INSPECTION_ON:           //Add by xcw - 20200520
                                nLength = Marshal.SizeOf(typeof(stGradeInfo));
                                pData = Marshal.AllocHGlobal(nLength);
                                //System.Windows.Forms.MessageBox.Show("CheckNu:");
                                break;
                            case (int)SIM_HMI_COMMAND_TYPE.SIM_HMI_INSPECTION_OFF:           //Add by xcw - 20200520
                                nLength = 0;
                                pData = Marshal.AllocHGlobal(nLength);

                                //GlobalDataInterface.TransmitParam(ConstPreDefine.SIM_ID, (int)HMI_SIM_COMMAND_TYPE.HMI_SIM_INSPECTION_OVER, null);// HMI给SIM发送抽检完毕命令（无参）

                                break;
                                
                            default: break;
                        }

                        if (pData != IntPtr.Zero)
                        {
                            rc = RecvData(m_ClientSocket, ref pData, nLength);//接收数据
                            m_fxnSetBuffer(head.nSrcId, head.nCmdId, pData);
                            Marshal.FreeHGlobal(pData);
                            if (rc)
                            {
                                lock (CommandHeadQue)
                                {
                                    CommandHeadQue.Enqueue(head);//加入数据处理队列
                                }

                            }
                        }

                    }
                    DestroyClientSocket();
                    Thread.Sleep(10);//释放时间片 ////
                    if (m_bRunOnce) m_IsRuning = false;
                }
            leave:
                DestroyMarsterSocket();
                m_IsRuning = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("TcpServer中函数TcpServerThread错误" + ex);
                #if REALEASE
                                GlobalDataInterface.WriteErrorInfo("TcpServer中函数TcpServerThread错误" + ex);
                #endif
            }
            return;

        }
        /// <summary>
        /// 创建主服务套接字
        /// </summary>
        /// <param name="nTimeout">发送和接收超时时间</param>
        /// <returns>socket</returns>
        private Socket CreateMasterSocket(int nTimeout)
        {

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建Socket 
            IPAddress IP = IPAddress.Parse(ConstPreDefine.HC_IP_ADDR);
            try
            {
                socket.Bind(new IPEndPoint(IP, m_nPort));//绑定IP地址和端口
                socket.SendTimeout = nTimeout;
                socket.ReceiveTimeout = nTimeout;
                socket.Listen(10);
            }
            catch (Exception e)
            {
                Trace.WriteLine("TcpServer中函数CreateMasterSocket错误" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("TcpServer中函数CreateMasterSocket错误" + e);
#endif
                socket.Close();
                return null;
            }
            return socket;
        }

        /// <summary>
        /// 获取客户端套接字
        /// </summary>
        /// <param name="serverSocket">主套接字</param>
        /// <param name="nTimeout">发送/接受时间</param>
        /// <returns>客户端套接字</returns>
        private Socket CreateClientSocket(Socket serverSocket, int nTimeout)
        {
            Socket socket;
            try
            {
                socket = serverSocket.Accept();
                socket.SendTimeout = nTimeout;
                socket.ReceiveTimeout = nTimeout;
                socket.ReceiveBufferSize = 0;//接收缓存置0，提高效率
            }
            catch (Exception e)
            {
                Trace.WriteLine("TcpServer中函数CreateClientSocket错误" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("TcpServer中函数CreateClientSocket错误" + e);
#endif
                return null; 
            }
            return socket;
        }


        /// <summary>
        /// 接受命令头
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="nCmdId">命令ID</param>
        /// <param name="nSrcId">发送源ID</param>
        /// <returns></returns>
        private bool RecvCommand(Socket clientSocket, ref int nCmdId, ref int nSrcId)
        {
            /*接收同步头*/
            if (!RecvSync(clientSocket))
                return false;

            nSrcId = -1;
            int nDstId = -1;
            nCmdId = -1;

            try
            {
                IntPtr pData = Marshal.AllocHGlobal(sizeof(byte) * 4 * 3);
                if(pData==null) 
                {
                    return false;
                }

                /*接收命令头*/
                if (!RecvData(clientSocket, ref pData, 12))
                    return false;
                else
                {
                    nSrcId = Marshal.ReadInt32(pData, 0);//接收发送源Id
                    nDstId = Marshal.ReadInt32(pData, 4);//接收发送目标Id
                    if(nDstId!=ConstPreDefine.HC_ID) return false;
                    nCmdId = Marshal.ReadInt32(pData, 8);//接受命令
                    Marshal.FreeHGlobal(pData);
                }
            }
            catch (AccessViolationException e)
            {
                Trace.WriteLine("TcpServer中RecvCommand函数开辟非托管内存失败：" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("TcpServer中RecvCommand函数开辟非托管内存失败：" + e);
#endif
            }
            catch (Exception e)
            {
                Trace.WriteLine("TcpServer中RecvCommand函数错误：" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("TcpServer中RecvCommand函数错误：" + e);
#endif
            }

            if (nCmdId == -1 || nSrcId == -1)
                return false;

            return true;
        }

        /// <summary>
        /// 接收同步头
        /// </summary>
        /// <param name="clientSocket">客户端socket</param>
        /// <returns>执行成功与否</returns>
        private bool RecvSync(Socket clientSocket)
        {
            byte[] bytes= new byte[4];

            try
            {
                if (clientSocket.Receive(bytes, 4, 0) == 4)
                {
                    string strSync = Encoding.Default.GetString(bytes, 0, 4);
                    if (strSync == "SYNC")
                    {
                        Trace.WriteLine("同步头接收成功");
//#if REALEASE
//                        GlobalDataInterface.WriteErrorInfo("同步头接收成功");
//#endif
                        return true;
                    }
                    else
                    {
                        Trace.WriteLine("同步头接收失败");
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo("同步头接收失败");
#endif
                        return false;
                    }
                }
                else
                {
                    Trace.WriteLine("同步头接收失败");
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("同步头接收失败");
#endif
                    return false;
                }

            }
            catch (Exception e)
            {
                Trace.WriteLine("TcpServer中函数RecvSync错误" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("TcpServer中函数RecvSync错误" + e);
#endif
                return false;
            }
        }

        /// <summary>
        /// 接受数据函数
        /// </summary>
        /// <param name="cliensocket">客户端socket</param>
        /// <param name="bData">接收数据</param>
        /// <param name="nlength">接收数据长度</param>
        /// <returns></returns>
        bool RecvData(Socket cliensocket, ref IntPtr pData,int nlength)
        {
            int nRemain, nRecved;
            int rc;
            byte[] temp = new byte[nlength];

            nRemain = nlength;
            nRecved = 0;

            try
            {
                while (nRemain > 0)
                {
                    rc = cliensocket.Receive(temp, nRecved,nRemain, 0);
                    if (rc > 0)
                    {
                        nRemain -= rc;
                        nRecved += rc;
                        
                    }
                    if (m_IsRuning == false)
                    {
                        return false;
                    }
                }
                Marshal.Copy(temp, 0, pData, nlength);
                if (nRemain == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                Trace.WriteLine("TcpServer中函数RecvData错误" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("TcpServer中函数RecvData错误" + e);
#endif
            }
            return true;
        }

        

        /// <summary>
        /// 注销客户端套接字
        /// </summary>
        void DestroyClientSocket()
        {
            LingerOption linger = new LingerOption(true,0);

            try
            {
                Lock(Timeout.Infinite);
                if (m_ClientSocket != null)
                {
                    m_ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, linger);
                    m_ClientSocket.Shutdown(SocketShutdown.Both);
                    m_ClientSocket.Close();
                    m_ClientSocket = null;
                }
                UnLock();
            }
            catch (Exception e)
            {
                Trace.WriteLine("TcpServer中函数DestroyClientSocket错误" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("TcpServer中函数DestroyClientSocket错误" + e);
#endif
            }
        }

        /// <summary>
        /// 注销主服务套接字
        /// </summary>
        void DestroyMarsterSocket()
        {
            LingerOption linger = new LingerOption(true, 0);

            try
            {
                Lock(Timeout.Infinite);
                if (m_MasterSocket != null)
                {
                    m_MasterSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, linger);
                    m_MasterSocket.Shutdown(SocketShutdown.Both);
                    m_MasterSocket.Close();
                    m_MasterSocket = null;
                }
                UnLock();
            }
            catch (Exception e)
            {
                Trace.WriteLine("TcpServer中函数DestroyMarsterSocket错误" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("TcpServer中函数DestroyMarsterSocket错误" + e);
#endif
            }
        }

        void End()
        {
            if (m_serverThread != null)
            {
                DestroyMarsterSocket();
            }
            m_mutex.Close();
            m_mutex = null;

        }

        public void Dispose()
        {
            try
            {
                Lock(Timeout.Infinite);
                if (m_MasterSocket != null)
                {
                    m_MasterSocket.Close();
                    m_MasterSocket.Dispose();
                    m_MasterSocket = null;
                }
                UnLock();
            }
            catch (Exception e)
            {
                Trace.WriteLine("TcpServer中函数Dispose错误" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("TcpServer中函数Dispose错误" + e);
#endif
            }
        }

        #endregion
    }
}
