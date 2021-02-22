using Common;
using Interface;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FruitSortingVtest1._0
{
    public partial class ProjectSetForm : Form
    {
        private static int m_InnerQualitySelectIndex = 0;//当前选择通道
        private TSYS_DEV_PARAMETER temp_SysDevParaData = new TSYS_DEV_PARAMETER(true); //内部品质参数信息
        private TSYS_DEV_INFORMATION[] temp_SysDevInfoData = new TSYS_DEV_INFORMATION[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM];   //光谱仪信息

        private void InnerQualityIntial()
        {
            try
            {
                temp_SysDevParaData.ToCopy(GlobalDataInterface.globalOut_SysDevParaData);
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                {
                    temp_SysDevInfoData[i] = new TSYS_DEV_INFORMATION(true);
                    temp_SysDevInfoData[i].ToCopy(GlobalDataInterface.globalOut_SysDevInfoData[i]);
                }

                #region 光谱仪信息 通道选择控件
                //  通道选择控件
                if (this.ComboBoxChannel.Items.Count == 0)
                {
                    for (int i = 0; i < m_ChanelIDList.Count; i++)
                    {
                        this.ComboBoxChannel.Items.Add("通道" + string.Format(" {0}", i + 1));
                    }
                }
                else
                {
                    int oldItemsCount = this.ComboBoxChannel.Items.Count;
                    if (this.ComboBoxChannel.Items.Count != m_ChanelIDList.Count)
                    {
                        if (this.ComboBoxChannel.Items.Count > m_ChanelIDList.Count)
                        {

                            for (int i = oldItemsCount - 1; i >= m_ChanelIDList.Count; i--)
                                this.ComboBoxChannel.Items.RemoveAt(i);

                            if (m_InnerQualitySelectIndex > this.ComboBoxChannel.Items.Count - 1)
                                m_InnerQualitySelectIndex = 0;
                        }
                        else
                        {
                            for (int i = oldItemsCount + 1; i <= m_ChanelIDList.Count; i++)
                            {
                                this.ComboBoxChannel.Items.Add("通道" + string.Format(" {0}", i));
                            }
                        }
                    }

                }
                if (m_InnerQualitySelectIndex >= 0 && this.ComboBoxChannel.Items.Count > 0)
                {
                    this.ComboBoxChannel.SelectedIndex = m_InnerQualitySelectIndex;
                }
                #endregion

                #region 初始化界面显示
                this.ComboBoxValidMethod.SelectedIndex = 0;
                this.ComboBoxSmoothPoint.SelectedIndex = 1;
                this.ComboBoxDeriOrder.SelectedIndex = 0;
                this.ComboBoxDeriSeg.SelectedIndex = 1;
                if (this.ComboBoxChannel.Items.Count > 0)
                    this.ComboBoxChannel.SelectedIndex = 0;
                SetFormParaDisplay();
                SetFormInfoDisplay();
                #endregion
            }
            catch(Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-InnerQuality中函数InnerQualityIntial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-InnerQuality中函数InnerQualityIntial出错" + ex);
#endif
            }
        }

        //内部品质参数信息
        private void SetFormParaDisplay()
        {
            try
            {            
                this.EditIntgTimeR.Text = temp_SysDevParaData.IntgTimeR.ToString();
                this.EditIntgTimeSL.Text = temp_SysDevParaData.IntgTimeSL.ToString();
                this.EditIntgTimeSM.Text = temp_SysDevParaData.IntgTimeSM.ToString();
                this.EditIntgTimeSS.Text = temp_SysDevParaData.IntgTimeSS.ToString();

                this.EditSizeFS.Text = temp_SysDevParaData.FruitSizeS.ToString();
                this.EditSizeFL.Text = temp_SysDevParaData.FruitSizeL.ToString();
                this.CheckBoxFruitS.Checked = temp_SysDevParaData.FruitSizeSFlag == 1;
                this.CheckBoxFruitL.Checked = temp_SysDevParaData.FruitSizeLFlag == 1;

                this.CheckBoxSmmothUsed.Checked = temp_SysDevParaData.SmoothUsedFlag == 1;
                this.ComboBoxSmoothPoint.SelectedIndex = (((temp_SysDevParaData.SmoothingPoint - 1) / 2) - 3) < 0 ? 0 : ((((temp_SysDevParaData.SmoothingPoint - 1) / 2) - 3));

                this.CheckBoxFilterRatio.Checked = temp_SysDevParaData.FirFilterUsedFlag == 1;
                this.EditFilterRatio.Text = string.Format("{0:0.000}", temp_SysDevParaData.FirFilterRatio);

                this.CheckBoxDerivativeUsed.Checked = temp_SysDevParaData.DerivUsedFlag == 1;
                this.ComboBoxDeriOrder.SelectedIndex = (temp_SysDevParaData.DerivOrder - 1) < 0 ? 0 : (temp_SysDevParaData.DerivOrder - 1);
                this.ComboBoxDeriSeg.SelectedIndex = (((temp_SysDevParaData.DerivSegSize - 1) / 2) - 1) < 0 ? 0 : (((temp_SysDevParaData.DerivSegSize - 1) / 2) - 1);

                this.CheckBoxBaselineUsed.Checked = temp_SysDevParaData.BaseLUsedFlag == 1;
                this.EditBaselineOffsetPoint.Text = temp_SysDevParaData.BaseLOffsetPoint.ToString();
                this.EditBaselineOffsetValue.Text = string.Format("{0:0.000}", temp_SysDevParaData.BaseLOffsetMin);

                this.EditScanCount.Text = temp_SysDevParaData.ScanCountVal.ToString();
                this.EditDecisionVal950nm.Text = temp_SysDevParaData.Decision950nmMax.ToString();
                this.ComboBoxValidMethod.SelectedIndex = temp_SysDevParaData.ProcessingMethod < 0 ? 0 : temp_SysDevParaData.ProcessingMethod;

                this.EditCupPitchSize.Text = temp_SysDevParaData.CupPitchSize.ToString();
                this.EditWarmupTime.Text = temp_SysDevParaData.WarmupTime.ToString();
            }
            catch(Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-InnerQuality中函数SetFormParaDisplay出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-InnerQuality中函数SetFormParaDisplay出错" + ex);
#endif
            }
        }

        //光谱仪信息
        private void SetFormInfoDisplay()
        {
            try
            {
                //加载通道零
                if (this.ComboBoxChannel.Items.Count == 0)
                    return;
                int SelId = m_ChanelIDList[m_InnerQualitySelectIndex];
                int m_InnerQualitySubsysindex = Commonfunction.GetSubsysIndex(SelId);  //子系统索引
                int m_InnerQualityChannelIndex = Commonfunction.GetChannelIndex(SelId);//子系统通道
                int dataIndex = m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex;
                this.EditInfoProductID.Text = Commonfunction.ByteArrayToString(temp_SysDevInfoData[dataIndex].ProductID.buf);
                this.EditInfoProductSerial.Text = Commonfunction.ByteArrayToString(temp_SysDevInfoData[dataIndex].ProductSerial.buf);
                this.EditInfoSpectoroSerial.Text = System.Text.Encoding.Default.GetString(temp_SysDevInfoData[dataIndex].SpectraSerial.buf);
                this.EditCcdInfoCoeffA0.Text = string.Format("{0:0.0000000E+00}", temp_SysDevInfoData[dataIndex].unitDblAr.DblArray[0]);
                this.EditCcdInfoCoeffA1.Text = string.Format("{0:0.0000000E+00}", temp_SysDevInfoData[dataIndex].unitDblAr.DblArray[1]);
                this.EditCcdInfoCoeffA2.Text = string.Format("{0:0.0000000E+00}", temp_SysDevInfoData[dataIndex].unitDblAr.DblArray[2]);
                this.EditCcdInfoCoeffA3.Text = string.Format("{0:0.0000000E+00}", temp_SysDevInfoData[dataIndex].unitDblAr.DblArray[3]);
                this.EditCcdInfoCoeffA4.Text = string.Format("{0:0.0000000E+00}", temp_SysDevInfoData[dataIndex].unitDblAr.DblArray[4]);
                this.EditCcdInfoCoeffA5.Text = string.Format("{0:0.0000000E+00}", temp_SysDevInfoData[dataIndex].unitDblAr.DblArray[5]);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-InnerQuality中函数SetFormInfoDisplay出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-InnerQuality中函数SetFormInfoDisplay出错" + ex);
#endif
            }
        }

        //光谱仪信息 - 生效（仅当前通道的光谱仪信息生效，其它通道不变）
        private void ButtonSetSpectrum_Click(object sender, EventArgs e)
        {
            try
            {
                bool resFlag = SetupParameterCheckProcess2();
                if (!resFlag)
                {
                    return;
                }

                if (MessageBox.Show("将光谱信息参数更新到通道" + (m_InnerQualitySelectIndex + 1).ToString() + "设备." +
                    System.Environment.NewLine + System.Environment.NewLine + "是否继续?"
                    , "警告信息", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }

                this.LabelWaitCount.Text = "光谱仪信息发送中...";
                //将“当前通道”的光谱信息参数下发到设备
                SendSpectrumInfoData(m_InnerQualitySelectIndex);

                int SelId = m_ChanelIDList[m_InnerQualitySelectIndex];
                int m_InnerQualitySubsysindex = Commonfunction.GetSubsysIndex(SelId);  //子系统索引
                int m_InnerQualityChannelIndex = Commonfunction.GetChannelIndex(SelId);//子系统通道
                int dataIndex = m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex;
                GlobalDataInterface.globalOut_SysDevInfoData[dataIndex].ToCopy(temp_SysDevInfoData[dataIndex]); //局部变量赋值到全局变量
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-InnerQuality中函数ButtonSetSpectrum_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-InnerQuality中函数ButtonSetSpectrum_Click出错" + ex);
#endif
            }
        }

        private void SendSpectrumInfoData(int channelSelectIndex)
        {
            try
            {
                bool bflags = ThreadPool.QueueUserWorkItem(new WaitCallback(SendSpectrumInfoThread), channelSelectIndex);
                if (!bflags)
                {
                    this.LabelWaitCount.Text = "通道" + (channelSelectIndex + 1).ToString() + "光谱仪信息发送失败";
                }
            }
            catch (Exception ex)
            {
                this.LabelWaitCount.Text = "通道" + (channelSelectIndex + 1).ToString() + "光谱仪信息发送失败";
                Trace.WriteLine("ProjectSetForm中函数StartReceivedInnerQualityInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数StartReceivedInnerQualityInfo出错" + ex);
#endif
            }
        }

        private void SendSpectrumInfoThread(object arg)
        {
            try
            {
                int index = (int)arg;
                if (tcpClient.IsConnected) //首先关闭当前的连接
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
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "光谱仪信息发送失败";
                        }));
                    }
                    else{
                        this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "光谱仪信息发送失败";
                    }
                    return;
                } //网络不通
                #endregion

                tcpClient.Connect(new IPEndPoint(IPAddress.Parse(ip), ConstPreDefine.LC_PORT_NUM)); //建立index+1的通道连接
                
                Thread.Sleep(1000);
                if (GlobalDataInterface.CommportConnectFlag)
                {
                    GlobalDataInterface.globalOut_SysDevInfoData[dataIndex].ToCopy(temp_SysDevInfoData[dataIndex]);
                    byte[] bytes = Commonfunction.StructToBytes(GlobalDataInterface.globalOut_SysDevInfoData[dataIndex]);
                    MessageDataSend(7, bytes);  //设置光谱仪信息

                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "光谱仪信息发送完成";
                        }));
                    }
                    else
                        this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "光谱仪信息发送完成";
                }
                else
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "光谱仪信息发送失败";
                        }));
                    }
                    else
                        this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "光谱仪信息发送失败";
                }

                Thread.Sleep(500);
                if (tcpClient.IsConnected) //最后关闭当前的连接（仅在有需要时才打开连接）
                {
                    tcpClient.Close();
                    GlobalDataInterface.CommportConnectFlag = false;
                }
                else{
                    GlobalDataInterface.CommportConnectFlag = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数SendSpectrumInfoThread出错，arg = " + arg.ToString() + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数SendSpectrumInfoThread出错，arg = " + arg.ToString() + ex);
#endif
            }
        }

        //光谱仪信息 - 默认
        private void ButtonSetSpectrumDefault_Click(object sender, EventArgs e)
        {
            try
            {
                this.EditInfoProductID.Text = "100001";
                this.EditInfoProductSerial.Text = "200001";
                this.EditInfoSpectoroSerial.Text = "300001";
                this.EditCcdInfoCoeffA0.Text = "6.268700224E+002";
                this.EditCcdInfoCoeffA1.Text = "1.968955472E+000";
                this.EditCcdInfoCoeffA2.Text = "-7.581370116E-004";
                this.EditCcdInfoCoeffA3.Text = "-1.322739500E-006";
                this.EditCcdInfoCoeffA4.Text = "1.224827478E-009";
                this.EditCcdInfoCoeffA5.Text = "-2.040336664E-012";
            }
            catch(Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-InnerQuality中函数ButtonSetSpectrumDefault_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-InnerQuality中函数ButtonSetSpectrumDefault_Click出错" + ex);
#endif
            }
        }

        //参数信息 - 生效（所有通道均生效）
        private void ButtonSetupSetAll_Click(object sender, EventArgs e)
        {
            try
            {
                bool resFlag = SetupParameterCheckProcess1();
                if (!resFlag)
                {
                    return;
                }

                if (MessageBox.Show("将内部品质信息参数更新到所有通道设备." +
                    System.Environment.NewLine + System.Environment.NewLine + "是否继续?"
                    , "警告信息", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }

                //将“所有通道”的内部品质参数下发到设备
                if (m_ChanelIDList.Count > 0 && GlobalDataInterface.InternalAvailable == true)
                {
                    SendDevParaData(m_ChanelIDList.Count);  //仅在通道不为零的情况下才发送内部品质基础信息
                }

                GlobalDataInterface.globalOut_SysDevParaData.ToCopy(temp_SysDevParaData); //局部变量赋值到全局变量
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-InnerQuality中函数ButtonSetupSetAll_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-InnerQuality中函数ButtonSetupSetAll_Click出错" + ex);
#endif
            }
        }

        private void SendDevParaData(int channelNumber)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(SendDevParaThread), channelNumber);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm中函数SendDevParaData出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数SendDevParaData出错" + ex);
#endif
            }
        }

        private void SendDevParaThread(object arg)
        {
            try
            {
                int channelNumber = (int)arg;
                int index = 0;
                while (index < channelNumber)
                {
                    try
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "基础信息发送中...";
                            }));
                        }
                        else
                            this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "基础信息发送中...";

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
                        if (pingReply.Status != IPStatus.Success) //网络不通
                        {
                            Thread.Sleep(1000);
                            if (this.InvokeRequired)
                            {
                                this.BeginInvoke(new MethodInvoker(delegate
                                {
                                    this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "基础信息发送失败";
                                }));
                            }
                            else
                                this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "基础信息发送失败";
                            Thread.Sleep(1000);
                            index++;
                            continue;
                        } 
                        #endregion

                        tcpClient.Connect(new IPEndPoint(IPAddress.Parse(ip), ConstPreDefine.LC_PORT_NUM)); //建立index+1的通道连接
                        
                        Thread.Sleep(1000);
                        if (GlobalDataInterface.CommportConnectFlag)
                        {
                            GlobalDataInterface.globalOut_SysDevParaData.ToCopy(temp_SysDevParaData);
                            byte[] bytes = Commonfunction.StructToBytes(GlobalDataInterface.globalOut_SysDevParaData);
                            MessageDataSend(ConstPreDefine.SBC_PARA_SET, bytes);  //设置基础信息

                            if (this.InvokeRequired)
                            {
                                this.BeginInvoke(new MethodInvoker(delegate
                                {
                                    this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "基础信息发送成功";
                                }));
                            }
                            else
                                this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "基础信息发送成功";
                        }
                        else
                        {
                            if (this.InvokeRequired)
                            {
                                this.BeginInvoke(new MethodInvoker(delegate
                                {
                                    this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "基础信息发送失败";
                                }));
                            }
                            else
                                this.LabelWaitCount.Text = "通道" + (index + 1).ToString() + "基础信息发送失败";
                        }

                        Thread.Sleep(500);
                        if (tcpClient.IsConnected)  //最后关闭当前的连接（仅在有需要时才打开连接）
                        {
                            tcpClient.Close();
                            GlobalDataInterface.CommportConnectFlag = false;
                        }
                        else{
                            GlobalDataInterface.CommportConnectFlag = false;
                        }
                    }
                    catch (Exception ex)
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
                Trace.WriteLine("ProjectSetForm中函数SendDevParaThread出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm中函数SendDevParaThread出错" + ex);
#endif
            }
        }

        //参数信息 - 加载
        private void ButtonSParaOpen_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog OpenDialogFile = new OpenFileDialog();
                OpenDialogFile.DefaultExt = "*." + ConstPreDefine.FILE_NAME_PARA;
                OpenDialogFile.Filter = ConstPreDefine.FILE_NAME_PARA + " Files (*." + ConstPreDefine.FILE_NAME_PARA + ")|*."
                    + ConstPreDefine.FILE_NAME_PARA + "|All Files (*.*)|*.*";
                OpenDialogFile.Title = "内部品质参数文件打开";
                if (OpenDialogFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                if (!System.IO.File.Exists(OpenDialogFile.FileName))
                {
                    return;
                }
                if (System.IO.Path.GetExtension(OpenDialogFile.FileName) != "." + ConstPreDefine.FILE_NAME_PARA)
                {
                    return;
                }

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(OpenDialogFile.FileName);
                int readFileSize = Convert.ToInt32(fileInfo.Length);
                int sumStructLen = 0;
                int structLen = Marshal.SizeOf(typeof(TSYS_DEV_PARAMETER));
                sumStructLen += structLen;
                if (structLen < Marshal.SizeOf(typeof(TSYS_DEV_INFORMATION)))
                {
                    structLen = Marshal.SizeOf(typeof(TSYS_DEV_INFORMATION));
                }
                sumStructLen += Marshal.SizeOf(typeof(TSYS_DEV_INFORMATION)) * (ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM);
                if (sumStructLen != readFileSize)
                {
                    MessageBox.Show("文件读取错误(" + structLen + "," + readFileSize + ")", "错误提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                using (System.IO.FileStream fileStream = System.IO.File.OpenRead(OpenDialogFile.FileName))
                {
                    byte[] FileData = new byte[structLen];
                    fileStream.Seek(0, SeekOrigin.Begin);
                    fileStream.Read(FileData, 0, Marshal.SizeOf(typeof(TSYS_DEV_PARAMETER)));
                    temp_SysDevParaData = (TSYS_DEV_PARAMETER)Commonfunction.BytesToStruct(FileData, typeof(TSYS_DEV_PARAMETER));
                    
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    {
                        fileStream.Read(FileData, 0, Marshal.SizeOf(typeof(TSYS_DEV_INFORMATION)));
                        temp_SysDevInfoData[i] = (TSYS_DEV_INFORMATION)Commonfunction.BytesToStruct(FileData, typeof(TSYS_DEV_INFORMATION));
                    }
                }
                SetFormParaDisplay();
                SetFormInfoDisplay();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-InnerQuality中函数ButtonSParaOpen_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-InnerQuality中函数ButtonSParaOpen_Click出错" + ex);
#endif
            }
        }

        //参数信息 - 保存
        private void ButtonSParaSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool resFlag = false;
                resFlag = SetupParameterCheckProcess1();
                if (!resFlag)
                {
                    return;
                }
                resFlag = SetupParameterCheckProcess2();
                if (!resFlag)
                {
                    return;
                }
                SaveFileDialog SaveDialogFile = new SaveFileDialog();
                SaveDialogFile.DefaultExt = "*." + ConstPreDefine.FILE_NAME_PARA;
                SaveDialogFile.Filter = ConstPreDefine.FILE_NAME_PARA + " Files (*." + ConstPreDefine.FILE_NAME_PARA + ")|*." + ConstPreDefine.FILE_NAME_PARA + "|All Files (*.*)|*.*";
                SaveDialogFile.Title = "绿萌参数文件保存";
                if (SaveDialogFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                string SaveFileName = SaveDialogFile.FileName;
                if (System.IO.File.Exists(SaveFileName))
                {
                    if (MessageBox.Show("文件已存在. 是否覆盖?"
                    , "警告信息", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }
                    System.IO.File.Delete(SaveFileName);
                }
                using (System.IO.FileStream fileStream = new System.IO.FileStream(SaveFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    int MaxLenth = Marshal.SizeOf(typeof(TSYS_DEV_PARAMETER));
                    if (MaxLenth < Marshal.SizeOf(typeof(TSYS_DEV_INFORMATION)))
                    {
                        MaxLenth = Marshal.SizeOf(typeof(TSYS_DEV_INFORMATION));
                    }
                    byte[] FileData = new byte[MaxLenth];
                    FileData = Commonfunction.StructToBytes(temp_SysDevParaData);
                    fileStream.Write(FileData, 0, Marshal.SizeOf(typeof(TSYS_DEV_PARAMETER)));  //写参数信息

                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    {
                        FileData = Commonfunction.StructToBytes(temp_SysDevInfoData[i]);
                        fileStream.Write(FileData, 0, Marshal.SizeOf(typeof(TSYS_DEV_INFORMATION)));//写光谱信息
                    }
                }
                MessageBox.Show("保存成功: " + System.IO.Path.GetFileNameWithoutExtension(SaveFileName), "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-InnerQuality中函数ButtonSParaSave_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-InnerQuality中函数ButtonSParaSave_Click出错" + ex);
#endif
            }
        }

        private void ComboBoxChannel_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                ComboBox combobox = (ComboBox)sender;
                m_InnerQualitySelectIndex = combobox.SelectedIndex;  //获取当前索引

                SetFormInfoDisplay();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-InnerQuality中函数ComboBoxChannel_SelectionChangeCommitted出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-InnerQuality中函数ComboBoxChannel_SelectionChangeCommitted出错" + ex);
#endif
            }
        }

        private void SetBlackDisplay()
        {
            this.EditIntgTimeR.ForeColor = Color.Black;
            this.EditIntgTimeSS.ForeColor = Color.Black;
            this.EditIntgTimeSM.ForeColor = Color.Black;
            this.EditIntgTimeSL.ForeColor = Color.Black;
            this.EditSizeFS.ForeColor = Color.Black;
            this.EditSizeFL.ForeColor = Color.Black;

            this.EditFilterRatio.ForeColor = Color.Black;
            this.EditBaselineOffsetPoint.ForeColor = Color.Black;
            this.EditBaselineOffsetValue.ForeColor = Color.Black;

            this.EditScanCount.ForeColor = Color.Black;
            this.EditDecisionVal950nm.ForeColor = Color.Black;
            this.EditWarmupTime.ForeColor = Color.Black;

            this.EditInfoProductID.ForeColor = Color.Black;
            this.EditInfoProductSerial.ForeColor = Color.Black;
            this.EditInfoSpectoroSerial.ForeColor = Color.Black;
            this.EditCcdInfoCoeffA0.ForeColor = Color.Black;
            this.EditCcdInfoCoeffA1.ForeColor = Color.Black;
            this.EditCcdInfoCoeffA2.ForeColor = Color.Black;
            this.EditCcdInfoCoeffA3.ForeColor = Color.Black;
            this.EditCcdInfoCoeffA4.ForeColor = Color.Black;
            this.EditCcdInfoCoeffA5.ForeColor = Color.Black;
        }

        private bool SetupParameterCheckProcess1()
        {
            SetBlackDisplay();

            #region 验证参数信息是否合法
            ushort ki = 0;
            ushort.TryParse(this.EditIntgTimeR.Text.Trim(), out ki);
            if ((ki > 200) || (ki < 5))
            {
                this.EditIntgTimeR.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditIntgTimeR.ForeColor = Color.Black;
                temp_SysDevParaData.IntgTimeR = ki;
            }
            ushort.TryParse(this.EditCupPitchSize.Text.Trim(), out ki);
            if ((ki > 500) || (ki < 40))
            {
                this.EditCupPitchSize.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditCupPitchSize.ForeColor = Color.Black;
                temp_SysDevParaData.CupPitchSize = ki;
            }
            ushort.TryParse(this.EditSizeFL.Text.Trim(), out ki);
            if ((ki > temp_SysDevParaData.CupPitchSize) || (ki < ConstPreDefine.VAL_DEF_FRUIT_SIZE_MIN))
            {
                this.EditSizeFL.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditSizeFL.ForeColor = Color.Black;
                temp_SysDevParaData.FruitSizeL = ki;
            }
            ushort.TryParse(this.EditSizeFS.Text.Trim(), out ki);
            if ((ki > temp_SysDevParaData.CupPitchSize) || (ki < ConstPreDefine.VAL_DEF_FRUIT_SIZE_MIN))
            {
                this.EditSizeFS.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditSizeFS.ForeColor = Color.Black;
                temp_SysDevParaData.FruitSizeS = ki;
            }
            if (temp_SysDevParaData.FruitSizeS >= temp_SysDevParaData.FruitSizeL)
            {
                this.EditSizeFS.ForeColor = Color.Red;
                this.EditSizeFL.ForeColor = Color.Red;
                return false;
            }
            temp_SysDevParaData.FruitSizeLFlag = Convert.ToByte(this.CheckBoxFruitL.Checked ? 1 : 0);
            temp_SysDevParaData.FruitSizeSFlag = Convert.ToByte(this.CheckBoxFruitS.Checked ? 1 : 0);
            ushort.TryParse(this.EditIntgTimeSL.Text.Trim(), out ki);
            if (((ki > 200) || (ki < 5)))
            {
                this.EditIntgTimeSL.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditIntgTimeSL.ForeColor = Color.Black;
                temp_SysDevParaData.IntgTimeSL = ki;
            }
            ushort.TryParse(this.EditIntgTimeSM.Text.Trim(), out ki);
            if ((ki > 200) || (ki < 5))
            {
                this.EditIntgTimeSM.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditIntgTimeSM.ForeColor = Color.Black;
                temp_SysDevParaData.IntgTimeSM = ki;
            }
            ushort.TryParse(this.EditIntgTimeSS.Text.Trim(), out ki);
            if ((ki > 200) || (ki < 5))
            {
                this.EditIntgTimeSS.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditIntgTimeSS.ForeColor = Color.Black;
                temp_SysDevParaData.IntgTimeSS = ki;
            }
            temp_SysDevParaData.SmoothUsedFlag = Convert.ToByte(this.CheckBoxSmmothUsed.Checked ? 1 : 0);
            byte bTemp = 0;
            if (!byte.TryParse(this.ComboBoxSmoothPoint.Text, out bTemp))
            {
                bTemp = 9;
            }
            temp_SysDevParaData.SmoothingPoint = bTemp;
            temp_SysDevParaData.FirFilterUsedFlag = Convert.ToByte(this.CheckBoxFilterRatio.Checked ? 1 : 0);
            float ks = 0;
            bool flagconv = float.TryParse(this.EditFilterRatio.Text, out ks);
            if (!flagconv || (ks > 1) || (ks < 0))
            {
                this.EditFilterRatio.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditFilterRatio.ForeColor = Color.Black;
                temp_SysDevParaData.FirFilterRatio = ks;
            }
            temp_SysDevParaData.DerivUsedFlag = Convert.ToByte(this.CheckBoxDerivativeUsed.Checked ? 1 : 0);
            if (!byte.TryParse(this.ComboBoxDeriOrder.Text, out bTemp))
            {
                bTemp = 1;
            }
            temp_SysDevParaData.DerivOrder = bTemp;
            if (!byte.TryParse(this.ComboBoxDeriSeg.Text, out bTemp))
            {
                bTemp = 5;
            }
            temp_SysDevParaData.DerivSegSize = bTemp;
            temp_SysDevParaData.BaseLUsedFlag = Convert.ToByte(this.CheckBoxBaselineUsed.Checked ? 1 : 0);
            ushort.TryParse(this.EditBaselineOffsetPoint.Text.Trim(), out ki);
            int kj = ki % 2;
            if ((ki > (ConstPreDefine.NO_WAVELENGTH_LAST - 50)) || (ki < (ConstPreDefine.NO_WAVELENGTH_FIRST + 50)) || (kj == 1))
            {
                this.EditBaselineOffsetPoint.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditBaselineOffsetPoint.ForeColor = Color.Black;
                temp_SysDevParaData.BaseLOffsetPoint = ki;
            }
            flagconv = float.TryParse(this.EditBaselineOffsetValue.Text.Trim(), out ks);
            if (!flagconv || (ks > 3) || (ks < 0))
            {
                this.EditBaselineOffsetValue.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditBaselineOffsetValue.ForeColor = Color.Black;
                temp_SysDevParaData.BaseLOffsetMin = ks;
            }
            if (!ushort.TryParse(this.EditScanCount.Text.Trim(), out ki))
            {
                ki = 50;
            }
            if ((ki > 50) || (ki < 1))
            {
                this.EditScanCount.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditScanCount.ForeColor = Color.Black;
                temp_SysDevParaData.ScanCountVal = ki;
            }
            ushort.TryParse(this.EditDecisionVal950nm.Text.Trim(), out ki);
            if ((ki > 40000) || (ki < 50))
            {
                this.EditDecisionVal950nm.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditDecisionVal950nm.ForeColor = Color.Black;
                temp_SysDevParaData.Decision950nmMax = ki;
            }
            temp_SysDevParaData.ProcessingMethod = Convert.ToByte(this.ComboBoxValidMethod.SelectedIndex);
            flagconv = ushort.TryParse(this.EditWarmupTime.Text.Trim(), out ki);
            if (!flagconv || (ki > 30) || (ki < 0))
            {
                this.EditWarmupTime.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditWarmupTime.ForeColor = Color.Black;
                temp_SysDevParaData.WarmupTime = ki;
            }
            #endregion
            return true;
        }

        private bool SetupParameterCheckProcess2()
        {
            SetBlackDisplay();

            #region 验证光谱信息是否合法
            int SelId = m_ChanelIDList[m_InnerQualitySelectIndex];
            int m_InnerQualitySubsysindex = Commonfunction.GetSubsysIndex(SelId);  //子系统索引
            int m_InnerQualityChannelIndex = Commonfunction.GetChannelIndex(SelId);//子系统通道
            string strData = this.EditInfoProductID.Text.Trim();
            int len = strData.Length;
            string qProductID = "";
            string qProductSerial = "";
            string qSpectrometerSerial = "";
            float FL = 0;
            if ((len < 5) || (len > 23) || string.IsNullOrWhiteSpace(strData))
            {
                this.EditInfoProductID.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditInfoProductID.ForeColor = Color.Black;
                qProductID = strData;
            }
            strData = this.EditInfoProductSerial.Text.Trim();
            len = strData.Length;
            if ((len < 5) || (len > 23) || string.IsNullOrWhiteSpace(strData))
            {
                this.EditInfoProductSerial.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditInfoProductSerial.ForeColor = Color.Black;
                qProductSerial = strData;
            }
            strData = this.EditInfoSpectoroSerial.Text.Trim();
            len = strData.Length;
            if ((len < 5) || (len > 23) || string.IsNullOrWhiteSpace(strData))
            {
                this.EditInfoSpectoroSerial.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditInfoSpectoroSerial.ForeColor = Color.Black;
                qSpectrometerSerial = strData;
            }
            TMsgByte24Format buffProductID = Commonfunction.StringToByteArrayConversion(qProductID);
            TMsgByte24Format buffProductSerial = Commonfunction.StringToByteArrayConversion(qProductSerial);
            TMsgByte24Format buffSpectrometerSerial = Commonfunction.StringToByteArrayConversion(qSpectrometerSerial);
            for (int i = 0; i < 23; i++)
            {
                temp_SysDevInfoData[m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex].ProductID.buf[i] = buffProductID.buf[i];
                temp_SysDevInfoData[m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex].ProductSerial.buf[i] = buffProductSerial.buf[i];
                temp_SysDevInfoData[m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex].SpectraSerial.buf[i] = buffSpectrometerSerial.buf[i];
            }
            temp_SysDevInfoData[m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex].ProductID.buf[23] = 0;
            temp_SysDevInfoData[m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex].ProductSerial.buf[23] = 0;
            temp_SysDevInfoData[m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex].SpectraSerial.buf[23] = 0;
            if (float.TryParse(this.EditCcdInfoCoeffA0.Text.Trim(), out FL))
            {
                this.EditCcdInfoCoeffA0.ForeColor = Color.Black;
                temp_SysDevInfoData[m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex].unitDblAr.DblArray[0] = FL;
            }
            else
            {
                this.EditCcdInfoCoeffA0.ForeColor = Color.Red;
                return false;
            }
            if (float.TryParse(this.EditCcdInfoCoeffA1.Text.Trim(), out FL))
            {
                this.EditCcdInfoCoeffA1.ForeColor = Color.Black;
                temp_SysDevInfoData[m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex].unitDblAr.DblArray[1] = FL;
            }
            else
            {
                this.EditCcdInfoCoeffA1.ForeColor = Color.Red;
                return false;
            }
            if (float.TryParse(this.EditCcdInfoCoeffA2.Text.Trim(), out FL))
            {
                this.EditCcdInfoCoeffA2.ForeColor = Color.Black;
                temp_SysDevInfoData[m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex].unitDblAr.DblArray[2] = FL;
            }
            else
            {
                this.EditCcdInfoCoeffA2.ForeColor = Color.Red;
                return false;
            }
            if (float.TryParse(this.EditCcdInfoCoeffA3.Text.Trim(), out FL))
            {
                this.EditCcdInfoCoeffA3.ForeColor = Color.Black;
                temp_SysDevInfoData[m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex].unitDblAr.DblArray[3] = FL;
            }
            else
            {
                this.EditCcdInfoCoeffA3.ForeColor = Color.Red;
                return false;
            }
            if (float.TryParse(this.EditCcdInfoCoeffA4.Text.Trim(), out FL))
            {
                this.EditCcdInfoCoeffA4.ForeColor = Color.Black;
                temp_SysDevInfoData[m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex].unitDblAr.DblArray[4] = FL;
            }
            else
            {
                this.EditCcdInfoCoeffA4.ForeColor = Color.Red;
                return false;
            }
            if (float.TryParse(this.EditCcdInfoCoeffA5.Text.Trim(), out FL))
            {
                this.EditCcdInfoCoeffA5.ForeColor = Color.Black;
                temp_SysDevInfoData[m_InnerQualitySubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_InnerQualityChannelIndex].unitDblAr.DblArray[5] = FL;
            }
            else
            {
                this.EditCcdInfoCoeffA5.ForeColor = Color.Red;
                return false;
            }
            #endregion
            return true;
        }
    }
}
