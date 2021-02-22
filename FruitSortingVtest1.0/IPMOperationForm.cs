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
using System.Net.NetworkInformation;
using Qodex;
using System.Net;
using System.Management;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FruitSortingVtest1
{
    public partial class IPMOperationForm : Form
    {
        Color[] itemColor;
        int ipmNum = 0;
        List<string> lstIPMIP = new List<string>();   //IPM的IP地址列表

        Control[] fMACAddrEditors;
        static int m_ValuelistViewExDownType = 0;

        public IPMOperationForm()
        {
            InitializeComponent();
            ipmNum = 0;
            for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
            {
                for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                {
                    //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                    if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                    {
                        ipmNum++;
                    }
                }
            }
            itemColor = new Color[ipmNum];
        }

        private void IPMOperationForm_Load(object sender, EventArgs e)
        {
            //fMACAddrEditors = new Control[] { txtMACAddr1, txtMACAddr2, txtMACAddr3, txtMACAddr4, txtMACAddr5, txtMACAddr6 };//add by xcw 20200226
            fMACAddrEditors = new Control[] { txtMACAddr };//add by xcw 20200226

            RefreshIPMList();
        }

        /// <summary>
        /// 远程关机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Closebutton_Click(object sender, EventArgs e)
        {
            //Note by ChengSk - 20191112
            //foreach (string item in this.IPMcustomCheckedListBox.CheckedItems)
            //{
            //    string[] sArray = item.Split('-');
            //    string IP = sArray[sArray.Length - 1];
            //    if (GlobalDataInterface.global_IsTestMode)
            //    {
            //        // int nDstId = Commonfunction.EncodeIPM(i, j);
            //        //GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHUTDOWN, null);

            //        //网络关闭IPM机器

            //        ConnectionOptions op = new ConnectionOptions();
            //        op.Username = "Administrator";//账号
            //        op.Password = "";//登录密码
            //        op.Authority = "ntlmdomain:DOMAIN";
            //        ManagementScope scope = new ManagementScope("\\\\"+IP+"\\root\\cimv2", op);
            //        try
            //        {
            //            scope.Connect();
            //            ObjectQuery oq = new ObjectQuery("select * from win32_OperatingSystem");
            //            ManagementObjectSearcher query1 = new ManagementObjectSearcher(scope, oq);//得到WMI控制
            //            ManagementObjectCollection queryCollection1 = query1.Get();
            //            foreach (ManagementObject mobj in queryCollection1)
            //            {
            //                string[] str = { "" };
            //                mobj.InvokeMethod("ShutDown", str);
            //            }
            //        }
            //        catch (Exception ex)
            //        {

            //        }
            //    }
            //}

            if (GlobalDataInterface.global_IsTestMode)  //Modify by ChengSk - 20191112
            {
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    {
                        //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                        if (j < GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i])    //Modify by ChengSk - 20191112
                        {

                            int nDstId = 0;
                            if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                            {
                                nDstId = Commonfunction.EncodeIPMChannel(i, j);
                            }
                            else if (GlobalDataInterface.nVer == 0)
                            {
                                nDstId = Commonfunction.EncodeIPM(i, j);
                            }
                            int nSubsysId = Commonfunction.GetIPMID(nDstId);
                            string strTemp = ConstPreDefine.LC_IP_ADDR_TEMPLATE;
                            string strIP = strTemp + nSubsysId;//得到IPM的IP地址
                            string strIPName = (nSubsysId - 16).ToString();//add by xcw 20201225
                            foreach (string item in this.IPMcustomCheckedListBox.CheckedItems)
                            {
                                string[] sArray = item.Split('-');
                                string IP = sArray[sArray.Length - 1];
                                //if (IP == strIP)
                                if (IP == strIPName)
                                {
                                    nDstId = Commonfunction.EncodeIPM(i, j);
                                    GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHUTDOWN, null);
#if REALEASE
                                    GlobalDataInterface.WriteErrorInfo("=> ShutDown to IPM, IP: " + IP + " !");
#endif
                                    break;
                                }
                            }
                        } //End if
                    } //End for
                } //End for
            } //End if
        }

        private Color IPMcustomCheckedListBox_GetBackColor(CustomCheckedListBox listbox, DrawItemEventArgs e)
        {
            return itemColor[e.Index];
        }

        /// <summary>
        /// 远程开机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenIPMbutton_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string item in this.IPMcustomCheckedListBox.CheckedItems)
                {
                    string[] sArray = item.Split('-');
                    string IP = ConstPreDefine.LC_IP_ADDR_TEMPLATE+ (int.Parse(sArray[sArray.Length - 1])+16).ToString();
                    string IPName = "IPM-" + IP;
                    //string strMAC = getMacAddr_Remote(IP);            //Note by ChengSk - 2017/09/26
                    string strMAC = Commonfunction.GetAppSetting(IPName); //~Note by ChengSk - 2017/09/26
                    if (strMAC != "")
                    {
                        string[] sArray1 = strMAC.Split('-');
                        //if (sArray1.Length != 6)
                        //{
                        //    MessageBox.Show(item + "'s MAC is wrong!");
                        //    continue;
                        //}

                        byte[] mac = new byte[strMAC.Length];
                        for (int i = 0; i < sArray1.Length; i++)
                            mac[i] = (byte)(int.Parse(sArray1[i],System.Globalization.NumberStyles.HexNumber));
                        WakeUp(mac);
                    }
                }
                
            }
            catch (Exception ex)
            { }
        }

        [DllImport("Iphlpapi.dll")]
        static extern int SendARP(Int32 DestIP, Int32 SrcIP, ref Int64 MacAddr, ref Int32 PhyAddrLen);
        [DllImport("Ws2_32.dll")]
        static extern Int32 inet_addr(string ipaddr);
        /// <summary> 
        /// SendArp获取MAC地址 
        /// </summary> 
        /// <param name="RemoteIP">目标机器的IP地址如(192.168.1.1)</param> 
        /// <returns>目标机器的mac 地址</returns> 
        public static string getMacAddr_Remote(string RemoteIP)
        {
            StringBuilder macAddress = new StringBuilder();
            try
            {
                Int32 remote = inet_addr(RemoteIP);
                Int64 macInfo = new Int64();
                Int32 length = 6;
                SendARP(remote, 0, ref macInfo, ref length);
                string temp = Convert.ToString(macInfo, 16).PadLeft(12, '0').ToUpper();
                int x = 12;
                for (int i = 0; i < 6; i++)
                {
                    if (i == 5)
                    {
                        macAddress.Append(temp.Substring(x - 2, 2));
                    }
                    else
                    {
                        macAddress.Append(temp.Substring(x - 2, 2) + "-");
                    }
                    x -= 2;
                }
                return macAddress.ToString();
            }
            catch
            {
                return macAddress.ToString();
            }
        }

        /// <summary>
        /// 远程唤醒
        /// </summary>
        /// <param name="mac"></param>
        private static void WakeUp(byte[] mac)
        {
            try
            {
                UdpClient client = new UdpClient();
                IPAddress ip = new IPAddress(new byte[] { 192, 168, 0, 255 });

                client.Connect(ip, 30000);

                byte[] packet = new byte[17 * 6];

                for (int i = 0; i < 6; i++)
                {
                    packet[i] = 0xFF;
                }

                for (int i = 1; i <= 16; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        packet[i * 6 + j] = mac[j];
                    }
                }

                int result = client.Send(packet, packet.Length);
            }
            catch (Exception ex)
            { }

        }

        /// <summary>
        /// 更新IPM列表状态
        /// </summary>
        private void RefreshIPMList()
        {
            try
            {
                Ping m_ping = new Ping();
                this.IPMcustomCheckedListBox.Items.Clear();
                this.IPMMACAddrlistViewEx.Items.Clear();
                lstIPMIP.Clear();
                //统计每个子系统的通道数
                int k = 0;
                ListViewItem lvi;
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    {
                        //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                        if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                        {
                            //nDstId = Commonfunction.EncodeIPM(i, j);
                            //GlobalDataInterface.TransmitParam(nDstId, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SHUTDOWN, null);
                            int nDstId = Commonfunction.EncodeIPM(i, j);
                            int nSubsysId = Commonfunction.GetIPMID(nDstId);
                            string strTemp = ConstPreDefine.LC_IP_ADDR_TEMPLATE;
                            string strIP = strTemp + nSubsysId;//得到IPM的IP地址
                            string strIPName = (nSubsysId - 16).ToString();
                            this.IPMcustomCheckedListBox.Items.Add("IPM-" + strIPName);
                            //this.IPMcustomCheckedListBox.Items.Add("IPM-" + strIP);

                            lstIPMIP.Add(strIP);
                            lvi = new ListViewItem((k + 1).ToString());
                            string strMACAddr = Commonfunction.GetAppSetting("IPM-" + strIP);
                            lvi.SubItems.Add(strMACAddr);
                            this.IPMMACAddrlistViewEx.Items.Add(lvi);//Add by ChengSk - 20190829

                            PingReply pingReply = m_ping.Send(strIP, 500);
                            if (pingReply.Status == IPStatus.Success)
                            {
                                //lvi.SubItems[k].ForeColor= Color.Green;
                                IPMMACAddrlistViewEx.Items[k].BackColor = System.Drawing.Color.Green;
                                //itemColor[k] = Color.Green;
                            }
                            else
                            {
                                //itemColor[k] = Color.White;
                                IPMMACAddrlistViewEx.Items[k].BackColor = System.Drawing.Color.White;//add by xcw 20200226
                                //lvi.SubItems[k].ForeColor = Color.White;
                            }
                            k++;
                        }
                    }
                }

                this.IPMcustomCheckedListBox.Refresh();

            }
            catch (Exception ex)
            { }
        }

        private void Refreshbutton_Click(object sender, EventArgs e)
        {
            RefreshIPMList();
        }

        private void IPMMACAddrlistViewEx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    m_ValuelistViewExDownType = 1;
            }
            catch(Exception ex)
            {   }
        }

        private void IPMMACAddrlistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (e.SubItem > 0 && m_ValuelistViewExDownType == 1)
                {
                    this.IPMMACAddrlistViewEx.StartEditing(fMACAddrEditors[e.SubItem - 1], e.Item, e.SubItem);
                    m_ValuelistViewExDownType = 0;
                }
            }
            catch(Exception ex)
            {   }
        }

        private void IPMMACAddrlistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                ListViewEx.ListViewEx listviewex = (ListViewEx.ListViewEx)sender;
                switch (e.SubItem)
                {
                    case 1:
                        string strMac = e.DisplayText.Trim();
                        Commonfunction.SetAppSetting("IPM-" + lstIPMIP[e.Item.Index], strMac);//Add by ChengSk - 20190829
                        //MessageBox.Show("行号: " + e.Item.Index + " 列号: " + e.SubItem + " 内容: " + strMac);
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {   }
        }

        //private void txtMACAddr_TextChanged(object sender, EventArgs e)
        //{
        //    if (((TextBox)sender).SelectionLength > 0) return;  //add by xcw 20200226
        //    if (((TextBox)sender).Text.Length >= 2)
        //    {
        //        SelectNextControl((Control)sender, true, true, true, true);

        //        ((TextBox)sender).SelectAll();
        //    }
        //}

        //private void txtMACAddr_Enter(object sender, EventArgs e)
        //{
        //    ((TextBox)sender).SelectAll();
        //}

        private void txtMACAddr_TextChanged(object sender, EventArgs e)
        {
            this.txtMACAddr.TextChanged -= new System.EventHandler(this.txtMACAddr_TextChanged);
            StringBuilder sb = new StringBuilder();
            int Selection = txtMACAddr.SelectionStart;
            char s = '-';//用什么字符隔开

            string str = txtMACAddr.Text.Replace(s.ToString(), "");
            for (int i = 1; i <= str.Length; i++)
            {
                sb.Append(str[i - 1]);
                if ((i != 0 && i % 2 == 0))//每组几个字符就%几
                {
                    if (i == str.Length) continue;
                    sb.Append(s);
                    Selection++;
                }
            }
            Selection = Selection - txtMACAddr.Text.Split(s).Length + 1;
            txtMACAddr.Text = sb.ToString();
            txtMACAddr.SelectionStart = Selection < 0 ? 0 : Selection;
            this.txtMACAddr.TextChanged += new System.EventHandler(this.txtMACAddr_TextChanged);
        }

        private void AllcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkbox = (CheckBox)sender;
                if (checkbox.Checked)
                {
                    for (int i = 0; i < this.IPMcustomCheckedListBox.Items.Count; i++)
                    {
                        this.IPMcustomCheckedListBox.SetItemChecked(i, true);
                    }
                }
                else
                {
                    for (int i = 0; i < this.IPMcustomCheckedListBox.Items.Count; i++)
                    {
                        this.IPMcustomCheckedListBox.SetItemChecked(i, false);
                    }
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
        //获取远程主机MAC
        public string getRemoteMac(string localIP, string remoteIP)
        {
            Int32 ldest = inet_addr(remoteIP); //目的ip 
            Int32 lhost = inet_addr(localIP); //本地ip 

            try
            {
                Int64 macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                return Convert.ToString(macinfo, 16);
            }
            catch (Exception err)
            {
                Console.WriteLine("Error:{0}", err.Message);
            }
            return 0.ToString();
        }
        private void GetMacbutton_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < this.IPMcustomCheckedListBox.Items.Count; i++)
                {
                    if (this.IPMcustomCheckedListBox.GetItemChecked(i))
                    {
                        IPMMACAddrlistViewEx.Items[i].SubItems[1].Text = getMacAddr_Remote(lstIPMIP[i]).ToString();
                        Commonfunction.SetAppSetting("IPM-" + lstIPMIP[i], getMacAddr_Remote(lstIPMIP[i]).ToString());//Add by xcw - 20201104
                    }
                    
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
    }
}
