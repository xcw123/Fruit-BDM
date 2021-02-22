using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Interface;
using System.Diagnostics;

namespace FruitSortingVtest1
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CommandPackage
    {
        public int nTypeId; //类型Id
        public int nCmdId;  //命令Id
        public int nPadConfigUpdate;//平板配置更新判断符号（原数据<新数据就更新系统配置），-1为无效值,0为上位机启动初始值，平板也应更新为0。
    }

    public class BroadcastServer
    {
        private Socket m_ServerSocket;  //广播源Socket套接字
        private IPEndPoint ipEndPoint;  //广播网络端点
        private Mutex m_mutex;          //资源互斥变量

        public BroadcastServer()
        {
            m_ServerSocket = null;
            ipEndPoint = null;
            m_mutex = new Mutex();

            m_ServerSocket = CreateServerSocket(1);
        }

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
        /// 获取服务器套接字
        /// </summary>
        /// <param name="nTimeout">发送时间</param>
        /// <returns>服务器端套接字</returns>
        private Socket CreateServerSocket(int nTimeout)
        {
            Socket ss = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ss.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            //绑定固定IP
            //IPAddress localAddress = IPAddress.Parse(ConstPreDefine.BROADCAST_LOCAL_IP_ADDR);
            //IPEndPoint localIpPoint = new IPEndPoint(localAddress, 0);
            //ss.Bind(localIpPoint);

            IPAddress broadcastIP = IPAddress.Parse(ConstPreDefine.BROADCAST_IP_ADDR);
            ipEndPoint = new IPEndPoint(broadcastIP, ConstPreDefine.BROADCAST_PORT_NUM);

            try
            {
                ss.SendTimeout = nTimeout;
                //ss.SendBufferSize = 0; //发送缓存置0，提高效率
            }
            catch (Exception ex)
            {
                Trace.WriteLine("CreateServerSocket()中出现错误，错误原因：" + ex.ToString());
                return null;
            }
            return ss;
        }

        public bool SendBroadcastPackage(byte[] Cmd, byte[] Data)
        {
            try
            {
                //m_ServerSocket = CreateServerSocket(1);
                if (m_ServerSocket == null)
                {
                    return false;
                }

                byte[] bytes = Encoding.Default.GetBytes("SYNC");
                if (!Send(bytes)) //发送同步头
                {
                    Trace.WriteLine("SendBroadcastPackage中发送同步头失败！");
                    //DestroyServerSocket();
                    return false;
                }

                if (!Send(Cmd))   //发送命令包
                {
                    Trace.WriteLine("SendBroadcastPackage中发送命令包失败！");
                    //DestroyServerSocket();
                    return false;
                }

                if (!Send(Data))  //发送数据包
                {
                    Trace.WriteLine("SendBroadcastPackage中发送数据包失败！");
                    //DestroyServerSocket();
                    return false;
                }

                int CheckCode = 0;         //求和校验
                for (int i = 0; i < Data.Length; i++)
                {
                    CheckCode += Data[i];  
                }  

                byte[] bCheckCode = new byte[4];  
                ConvertIntToByteArray(CheckCode, ref bCheckCode);

                if (!Send(bCheckCode))     //发送求和校验 Add by ChengSk - 20180119
                {
                    Trace.WriteLine("SendBroadcastPackage中发送求和校验失败！");
                    return false;
                }

                //DestroyServerSocket();   //销毁套接字
            }
            catch (Exception ex)
            {
                Trace.WriteLine("BroadcastServer中函数SendBroadcastPackage出错，错误原因：" + ex.ToString());
                GlobalDataInterface.WriteErrorInfo("BroadcastServer中函数SendBroadcastPackage出错，错误原因：" + ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>  
        /// 把int32类型的数据转存到4个字节的byte数组中  
        /// </summary>  
        /// <param name="m">int32类型的数据</param>  
        /// <param name="arry">4个字节大小的byte数组</param>  
        /// <returns></returns>  
        static bool ConvertIntToByteArray(Int32 m, ref byte[] arry)
        {
            if (arry == null) return false;
            if (arry.Length < 4) return false;

            arry[0] = (byte)(m & 0xFF);
            arry[1] = (byte)((m & 0xFF00) >> 8);
            arry[2] = (byte)((m & 0xFF0000) >> 16);
            arry[3] = (byte)((m >> 24) & 0xFF);

            return true;
        } 

        /// <summary>
        /// 广播发送数据
        /// </summary>
        /// <param name="bytes">发送数据</param>
        /// <returns>发送是否成功</returns>
        private bool Send(byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                return false;
            }

            int nRemain = bytes.Length;
            int nSendByte;
            int nOffset = 0;

            while (nRemain > 0)
            {
                try
                {
                    if (nRemain >= 1024)
                    {
                        nSendByte = m_ServerSocket.SendTo(bytes, nOffset, 1024, 0, ipEndPoint);
                    }
                    else
                    {
                        nSendByte = m_ServerSocket.SendTo(bytes, nOffset, nRemain, 0, ipEndPoint);
                    }    
                    if (nSendByte > 0)
                    {
                        nRemain -= nSendByte;
                        nOffset += nSendByte;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("网络发送数据失败，失败原因：" + ex.ToString());
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
        /// 注销服务器端套接字
        /// </summary>
        void DestroyServerSocket()
        {
            LingerOption linger = new LingerOption(true, 0);

            try
            {
                Lock(Timeout.Infinite);
                if (m_ServerSocket != null)
                {
                    m_ServerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, linger);
                    m_ServerSocket.Shutdown(SocketShutdown.Send);
                    m_ServerSocket.Close();
                    m_ServerSocket = null;
                }
                UnLock();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("BroadcastServer中函数DestoryServerSocket错误，错误原因：" + ex.ToString());
            }
        }

    }
}
