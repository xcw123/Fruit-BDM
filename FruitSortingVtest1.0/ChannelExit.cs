using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ListViewEx;
using GlacialComponents.Controls;
using Interface;
using Common;
using System.Diagnostics;
using System.Threading;

namespace FruitSortingVtest1._0
{
    public partial class ProjectSetForm : Form
    {
        private Control[] ExitEditors;
        private static stExitInfo[] tempExitInfo = new stExitInfo[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM];
        private static stGlobalExitInfo[] tempGlobalExitInfo = new stGlobalExitInfo[ConstPreDefine.MAX_SUBSYS_NUM];
        private static int m_ExitChannelSelectIndex = 0;//当前选择通道
        private static int m_ExitSubsysindex = -1;//当前选择通道所属子系统
        private static int m_ExitSubsysChannelIndex = -1;//当前选择通道所属子系统第几个通道
        private static int m_ExitChannelInIPMIndex = -1;//当前选择所属IPM通道
        private static int m_ChannelExitMouseDownType = 0;//出口列表鼠标按下类型，0左键单击；1左键双击；2右键
        private static int m_ChannelExitMotorMouseOnceDownType = 0;//电机列表鼠标按下类型，0左键单击；1右键单击
        private static int m_ChannelExitMotorMouseDownType = 0;//电机列表鼠标按下类型，1左键双击；0为不是左键双击
        private stVolveTest m_VolveTest = new stVolveTest(true);//电磁阀测试参数
        private static int m_CurrentItemIndex = -1;
        private static int m_CurrentMotorItemIndex = -1;
       // private static int m_CurrentExitOrMotorTest = 0; //出口还是电机进行电磁阀测试，0 为出口 1为电机
        private bool m_IsVolveTesting = false;
        private Control[] MotorExitEditors;

        private void ChannelExitIntial()
        {
            try
            {
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    tempGlobalExitInfo[i] = new stGlobalExitInfo(true);

                }
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                {
                    tempExitInfo[i] = new stExitInfo(true);
                }


                //复制参数
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                {
                    tempExitInfo[i].ToCopy(GlobalDataInterface.globalOut_ExitInfo[i]);
                }
                GlobalDataInterface.globalOut_GlobalExitInfo.CopyTo(tempGlobalExitInfo, 0);

                ExitEditors = new Control[] { DistancenumericUpDown, OffsetnumericUpDown, DrivernumericUpDown, InterfacecomboBox, PincomboBox };

                MotorExitEditors = new Control[] { MotorDrivernumericUpDown, MotorInterfacecomboBox, MotorPincomboBox, ExpandTimenumericUpDown, DurationnumericUpDown };


                //通道选择控件
                if (this.ChannelSeleccomboBox.Items.Count == 0)
                {
                    for (int i = 0; i < m_ChanelIDList.Count; i++)
                    {
                        this.ChannelSeleccomboBox.Items.Add(m_resourceManager.GetString("Lanelabel.Text") + string.Format(" {0}", i + 1));
                    }
                }
                else
                {
                    int oldItemsCount = this.ChannelSeleccomboBox.Items.Count;
                    if (this.ChannelSeleccomboBox.Items.Count != m_ChanelIDList.Count)
                    {
                        if (this.ChannelSeleccomboBox.Items.Count > m_ChanelIDList.Count)
                        {
                            
                            for (int i = oldItemsCount - 1; i >= m_ChanelIDList.Count; i--)
                                this.ChannelSeleccomboBox.Items.RemoveAt(i);

                            if (m_ExitChannelSelectIndex > this.ChannelSeleccomboBox.Items.Count - 1)
                                m_ExitChannelSelectIndex = 0;
                        }
                        else
                        {
                            for (int i = oldItemsCount + 1; i <= m_ChanelIDList.Count; i++)
                            {
                                this.ChannelSeleccomboBox.Items.Add(m_resourceManager.GetString("Lanelabel.Text") + string.Format(" {0}", i));
                            }
                        }
                    }

                }
                if (m_ExitChannelSelectIndex >= 0 && this.ChannelSeleccomboBox.Items.Count > 0)
                    this.ChannelSeleccomboBox.SelectedIndex = m_ExitChannelSelectIndex;

                //电磁阀脉宽控件与贴标脉宽控件
                if (m_ExitChannelSelectIndex >= 0)
                {
                    if (m_ChanelIDList.Count > 0)
                    {
                        int SelId = m_ChanelIDList[m_ExitChannelSelectIndex];
                        m_ExitSubsysindex = Commonfunction.GetSubsysIndex(SelId);
                        m_ExitSubsysChannelIndex = Commonfunction.GetChannelIndex(SelId);
                        if (GlobalDataInterface.nVer == 0)  //add by xcw-20200617
                        {
                            m_ExitSubsysChannelIndex = Commonfunction.GetChannelIndex(SelId);

                        }
                        else if (GlobalDataInterface.nVer == 1)
                        {
                            m_ExitSubsysChannelIndex = Commonfunction.GetIPMIndex(SelId) * ConstPreDefine.CHANNEL_NUM + Commonfunction.GetChannelIndex(SelId);
                        }
                        this.ElecPlusenumericUpDown.Text = tempGlobalExitInfo[m_ExitSubsysindex].nPulse.ToString();
                        this.LabelPlusenumericUpDown.Text = tempGlobalExitInfo[m_ExitSubsysindex].nLabelPulse.ToString();
                    }
                }

                ////电机管脚设置只能对存在一个子系统有效
                //IsEnableExitMotorDriverPinlistViewEx();

                SetExitlistViewEx();
                SetExitMotorDriverPinlistViewEx();
            }
            catch (Exception e)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ChannelExitIntial出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ChannelExitIntial出错" + e);
#endif
            }
        }
        /// <summary>
        /// 获取驱动器
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private short GetDriverPinPara1(short x)
        {
            try
            {
                short y = (short)(((x) >> 12) & 0x0f);
                return y;
            }
            catch (Exception e)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数GetDriverPinPara1出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数GetDriverPinPara1出错" + e);
#endif
                return -1;
            }
        }
        /// <summary>
        /// 获取接口
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private short GetDriverPinPara2(short x)
        {
            try
            {
                short y = (short)(((x) >> 9) & 7);
                return y;
            }
            catch (Exception e)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数GetDriverPinPara2出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数GetDriverPinPara2出错" + e);
#endif
                return -1;
            }
        }
        /// <summary>
        /// 获取引脚
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private short GetDriverPinPara3(short x)
        {
            try
            {
                short y = (short)((x) & 0x01ff);
                return y;
            }
            catch (Exception e)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数GetDriverPinPara3出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数GetDriverPinPara3出错" + e);
#endif
                return -1;
            }
        }
        /// <summary>
        /// 设置驱动器引脚参数
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private short SetDriverPin(short x, short y, short z)
        {
            try
            {
                short A = (short)((((x) & 0x0f) << 12) | (((y) & 7) << 9) | ((z) & 0x01ff));
                return A;
            }
            catch (Exception e)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数SetDriverPin出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数SetDriverPin出错" + e);
#endif
                return -1;
            }
        }

        /// <summary>
        /// 设置出口List
        /// </summary>
        private void SetExitlistViewEx()
        {
            try
            {
                if (this.ChannelSeleccomboBox.SelectedIndex >= 0)
                {
                    this.ExitlistViewEx.Items.Clear();
                    int SelId = m_ChanelIDList[this.ChannelSeleccomboBox.SelectedIndex];
                    m_ExitSubsysindex = Commonfunction.GetSubsysIndex(SelId);
                    m_ExitSubsysChannelIndex = Commonfunction.GetChannelIndex(SelId);
                    if (GlobalDataInterface.nVer == 0)  //add by xcw-20200617
                    {
                        m_ExitSubsysChannelIndex = Commonfunction.GetChannelIndex(SelId);

                    }
                    else if (GlobalDataInterface.nVer == 1)
                    {
                        m_ExitSubsysChannelIndex = Commonfunction.GetIPMIndex(SelId) * ConstPreDefine.CHANNEL_NUM + Commonfunction.GetChannelIndex(SelId);
                    }
                    //m_ExitChannelInIPMIndex = Commonfunction.GetIPMIndex(SelId);
                    //贴标
                    //if (this.ExitlistViewEx.Items.Count == 0)
                    //{
                    ListViewItem lvi;
                    for (int i = 0; i < ConstPreDefine.MAX_LABEL_NUM; i++)
                    {
                        lvi = new ListViewItem(m_resourceManager.GetString("Labellabel.Text") + string.Format(" {0}", i + 1));
                        lvi.SubItems.Add(string.Format("{0}", tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[i].nDis));
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add(string.Format("{0}", GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[i].nDriverPin)));
                        switch (GetDriverPinPara2(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[i].nDriverPin))
                        {
                            case 1:
                                lvi.SubItems.Add("A");
                                break;
                            case 2:
                                lvi.SubItems.Add("B");
                                break;
                            case 3:
                                lvi.SubItems.Add("C");
                                break;
                            default:
                                lvi.SubItems.Add("");
                                break;

                        }
                        lvi.SubItems.Add(string.Format("{0}", GetDriverPinPara3(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[i].nDriverPin)));
                        this.ExitlistViewEx.Items.Add(lvi);
                    }
                    //出口
                    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nExitNum; i++)
                    {
                        lvi = new ListViewItem(m_resourceManager.GetString("Outletlabel.Text") + string.Format(" {0}", i + 1));
                        lvi.SubItems.Add(string.Format("{0}", tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDis));
                        lvi.SubItems.Add(string.Format("{0}", tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nOffset));
                        lvi.SubItems.Add(string.Format("{0}", GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDriverPin)));
                        switch (GetDriverPinPara2(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDriverPin))
                        {
                            case 1:
                                lvi.SubItems.Add("A");
                                break;
                            case 2:
                                lvi.SubItems.Add("B");
                                break;
                            case 3:
                                lvi.SubItems.Add("C");
                                break;
                            default:
                                lvi.SubItems.Add("");
                                break;
                        }
                        lvi.SubItems.Add(string.Format("{0}", GetDriverPinPara3(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDriverPin)));
                        this.ExitlistViewEx.Items.Add(lvi);
                    }

                    //  将其它出口的引脚值置为0，防止eeprom里面的管脚被占用 Add by ChengSk - 20190729
                    for(int i= GlobalDataInterface.globalOut_SysConfig.nExitNum; i< ConstPreDefine.MAX_EXIT_NUM; i++)
                    {
                        tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDriverPin = 0;
                    }

                    //}
                    //else
                    //{
                    //    //贴标
                    //    for (int i = 0; i < ConstPreDefine.MAX_LABEL_NUM; i++)
                    //    {
                    //        this.ExitlistViewEx.Items[i].SubItems[1].Text = string.Format("{0}", tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + this.ChannelSeleccomboBox.SelectedIndex].labelexit[i].nDis);
                    //        this.ExitlistViewEx.Items[i].SubItems[3].Text = string.Format("{0}", GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + this.ChannelSeleccomboBox.SelectedIndex].labelexit[i].nDriverPin));
                    //        switch (GetDriverPinPara2(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + this.ChannelSeleccomboBox.SelectedIndex].exits[i].nDriverPin))
                    //        {
                    //            case 1:
                    //                this.ExitlistViewEx.Items[i].SubItems[4].Text = "A";
                    //                break;
                    //            case 2:
                    //                this.ExitlistViewEx.Items[i].SubItems[4].Text = "B";
                    //                break;
                    //            case 3:
                    //                this.ExitlistViewEx.Items[i].SubItems[4].Text = "C";
                    //                break;
                    //            default:
                    //                this.ExitlistViewEx.Items[i].SubItems[4].Text = "";
                    //                break;
                    //        }
                    //        this.ExitlistViewEx.Items[i].SubItems[5].Text = string.Format("{0}", GetDriverPinPara3(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + this.ChannelSeleccomboBox.SelectedIndex].labelexit[i].nDriverPin));

                    //    }
                    //    //出口
                    //    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nExitNum; i++)
                    //    {
                    //        this.ExitlistViewEx.Items[i + ConstPreDefine.MAX_LABEL_NUM].SubItems[1].Text = string.Format("{0}", tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + this.ChannelSeleccomboBox.SelectedIndex].exits[i].nDis);
                    //        this.ExitlistViewEx.Items[i + ConstPreDefine.MAX_LABEL_NUM].SubItems[2].Text = string.Format("{0}", tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + this.ChannelSeleccomboBox.SelectedIndex].exits[i].nOffset);
                    //        this.ExitlistViewEx.Items[i + ConstPreDefine.MAX_LABEL_NUM].SubItems[3].Text = string.Format("{0}", GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + this.ChannelSeleccomboBox.SelectedIndex].exits[i].nDriverPin));
                    //        switch (GetDriverPinPara2(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + this.ChannelSeleccomboBox.SelectedIndex].exits[i].nDriverPin))
                    //        {
                    //            case 1:
                    //                this.ExitlistViewEx.Items[i + ConstPreDefine.MAX_LABEL_NUM].SubItems[4].Text = "A";
                    //                break;
                    //            case 2:
                    //                this.ExitlistViewEx.Items[i + ConstPreDefine.MAX_LABEL_NUM].SubItems[4].Text = "B";
                    //                break;
                    //            case 3:
                    //                this.ExitlistViewEx.Items[i + ConstPreDefine.MAX_LABEL_NUM].SubItems[4].Text = "C";
                    //                break;
                    //            default:
                    //                this.ExitlistViewEx.Items[i + ConstPreDefine.MAX_LABEL_NUM].SubItems[4].Text = "";
                    //                break;
                    //        }
                    //        this.ExitlistViewEx.Items[i + ConstPreDefine.MAX_LABEL_NUM].SubItems[5].Text = string.Format("{0}", GetDriverPinPara3(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + this.ChannelSeleccomboBox.SelectedIndex].exits[i].nDriverPin));
                    //    }

                    //}
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数SetExitlistViewEx出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数SetExitlistViewEx出错" + e);
#endif
            }
        }

        /// <summary>
        /// 设置电机出口List
        /// </summary>
        private void SetExitMotorDriverPinlistViewEx()
        {
            try
            {
                this.ExitMotorDriverPinlistViewEx.Items.Clear();

                ListViewItem lvi;
                //出口
                for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nExitNum; i++)
                {
                    lvi = new ListViewItem(m_resourceManager.GetString("Outletlabel.Text") + string.Format(" {0}", i + 1));
                    lvi.SubItems.Add(string.Format("{0}", GetDriverPinPara1(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i])));
                    switch (GetDriverPinPara2(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]))
                    {
                        case 1:
                            lvi.SubItems.Add("A");
                            break;
                        case 2:
                            lvi.SubItems.Add("B");
                            break;
                        case 3:
                            lvi.SubItems.Add("C");
                            break;
                        default:
                            lvi.SubItems.Add("");
                            break;
                    }
                    lvi.SubItems.Add(string.Format("{0}", GetDriverPinPara3(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i])));
                    lvi.SubItems.Add(tempGlobalExitInfo[m_ExitSubsysindex].Delay_time[i].ToString()); //add by xcw - 20191209
                    lvi.SubItems.Add(tempGlobalExitInfo[m_ExitSubsysindex].Hold_time[i].ToString());

                    this.ExitMotorDriverPinlistViewEx.Items.Add(lvi);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数SetExitMotorDriverPinlistViewEx()出错" + e);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数SetExitMotorDriverPinlistViewEx()出错" + e);
#endif
            }
        }

        /// <summary>
        /// 通道选择控件的SelectedIndexChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelSeleccomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                ComboBox combobox = (ComboBox)sender;
                if (m_ExitChannelSelectIndex != combobox.SelectedIndex)
                {
                    if (m_VolveTest.ExitId != 255)
                    {
                        if (m_VolveTest.ExitId <= 100)
                            this.ExitlistViewEx.Items[m_VolveTest.ExitId + 4].BackColor = Color.White;
                        else if (m_VolveTest.ExitId > 100 && m_VolveTest.ExitId < 105)
                            this.ExitlistViewEx.Items[m_VolveTest.ExitId - 101].BackColor = Color.White;

                        m_VolveTest.ExitId = 255;
                        if (GlobalDataInterface.global_IsTestMode)
                            GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ExitChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, m_VolveTest);
                        m_IsVolveTesting = false;

                    }
                    SetExitlistViewEx();
                    m_ExitChannelSelectIndex = combobox.SelectedIndex;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ChannelSeleccomboBox_SelectionChangeCommitted出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ChannelSeleccomboBox_SelectionChangeCommitted出错" + ex);
#endif
            }
        }

       

       

        /// <summary>
        /// 出口列表鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitlistViewEx_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    //m_ChannelExitMouseDownType = 1;
                    m_ChannelExitMouseDownType = 0;
                else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    m_ChannelExitMouseDownType = 2;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ExitlistViewEx_MouseDown出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ExitlistViewEx_MouseDown出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 出口列表鼠标双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitlistViewEx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    m_ChannelExitMouseDownType = 1;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ExitlistViewEx_MouseDoubleClick出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ExitlistViewEx_MouseDoubleClick出错" + ex);
#endif
            }
        }

        

        /// <summary>
        /// 电磁阀测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_VolveTest.ExitId != 255)
                {
                    if (m_VolveTest.ExitId <= 100)
                    {
                        //if (m_CurrentExitOrMotorTest == 0 )
                        //{
                            this.ExitlistViewEx.Items[m_VolveTest.ExitId + 4].BackColor = Color.White;
                            this.ExitlistViewEx.Items[m_VolveTest.ExitId + 4].Font = new Font("宋体", 9, FontStyle.Regular);//ChengSk
                        //}
                        //else
                        //{
                        //    this.ExitMotorDriverPinlistViewEx.Items[m_VolveTest.ExitId - 48].BackColor = Color.White;
                        //    this.ExitMotorDriverPinlistViewEx.Items[m_VolveTest.ExitId - 48].Font = new Font("宋体", 9, FontStyle.Regular);
                        //}

                    }
                    else if (m_VolveTest.ExitId > 100 && m_VolveTest.ExitId < 105)
                    {
                        this.ExitlistViewEx.Items[m_VolveTest.ExitId - 101].BackColor = Color.White;
                        this.ExitlistViewEx.Items[m_VolveTest.ExitId - 101].Font = new Font("宋体", 9, FontStyle.Regular);//ChengSk
                    }              
                }
                //if (m_CurrentExitOrMotorTest == 0)
                //{
                    if (m_CurrentItemIndex < 4)
                        m_VolveTest.ExitId = (byte)(m_CurrentItemIndex + 101);
                    else
                        m_VolveTest.ExitId = (byte)(m_CurrentItemIndex - 4);
                    this.ExitlistViewEx.Items[m_CurrentItemIndex].BackColor = Color.Red;
                    this.ExitlistViewEx.Items[m_CurrentItemIndex].Font = new Font("黑体", 9, FontStyle.Italic | FontStyle.Bold);//ChengSk
                //}
                //else
                //{
                //    m_VolveTest.ExitId = (byte)(m_CurrentMotorItemIndex + 48);
                //    this.ExitMotorDriverPinlistViewEx.Items[m_CurrentMotorItemIndex].BackColor = Color.Red;
                //    this.ExitMotorDriverPinlistViewEx.Items[m_CurrentMotorItemIndex].Font = new Font("黑体", 9, FontStyle.Italic | FontStyle.Bold);//ChengSk
                //}

                
                if (GlobalDataInterface.global_IsTestMode)
                    GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ExitChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, m_VolveTest);
                m_IsVolveTesting = true;
                m_CurrentItemIndex = -1;
                m_CurrentMotorItemIndex = -1;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数Start_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数Start_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 停止电磁阀测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Click(object sender, EventArgs e)
        {
            try
            {
                //if (m_CurrentExitOrMotorTest == 0)
                //{
                    this.ExitlistViewEx.Items[m_CurrentItemIndex].BackColor = Color.White;
                    this.ExitlistViewEx.Items[m_CurrentItemIndex].Font = new Font("宋体", 9, FontStyle.Regular);//ChengSk
                //}
                //else
                //{
                //    this.ExitMotorDriverPinlistViewEx.Items[m_CurrentMotorItemIndex].BackColor = Color.White;
                //    this.ExitMotorDriverPinlistViewEx.Items[m_CurrentMotorItemIndex].Font = new Font("宋体", 9, FontStyle.Regular);
                //}

                if (GlobalDataInterface.global_IsTestMode)
                {
                    stVolveTest stop_VolveTest = new stVolveTest(true);//停止电磁阀测试
                    GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ExitChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, stop_VolveTest);
                }
                m_CurrentItemIndex = -1;
                m_CurrentMotorItemIndex = -1;
                m_IsVolveTesting = false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数Stop_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数Stop_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 出口List中SubItemClicked事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitlistViewEx_SubItemClicked(object sender, SubItemEventArgs e)
        {
            try
            {
                if (m_ChannelExitMouseDownType == 1)//左键
                {
                    if (e.SubItem > 0 && (e.Item.Index > 3 || e.SubItem != 2))
                        this.ExitlistViewEx.StartEditing(ExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                }
                else if (m_ChannelExitMouseDownType == 2)//右键
                {
                    m_CurrentItemIndex = e.Item.Index;
                    if (e.Item.BackColor == Color.Red)//测试中
                    {
                        this.VolveTestcontextMenuStrip.Items[1].Enabled = true;
                        this.VolveTestcontextMenuStrip.Items[0].Enabled = false;
                      //  m_VolveTest.ExitId = 255;
                    }
                    else//未测试
                    {
                        this.VolveTestcontextMenuStrip.Items[0].Enabled = true;
                        this.VolveTestcontextMenuStrip.Items[1].Enabled = false;
                        //if (m_CurrentItemIndex < 4)
                        //    m_VolveTest.ExitId = (byte)(m_CurrentItemIndex + 101);
                        //else
                        //    m_VolveTest.ExitId = (byte)(m_CurrentItemIndex - 4);

                    }
                    //Point point = new Point();
                    //point.X = Cursor.Position.X - this.Location.X + this.ExitlistViewEx.Location.X;
                    //point.Y = Cursor.Position.Y - this.Location.Y - 3 * this.ExitlistViewEx.Location.Y - 30;
                    Point point = this.ExitlistViewEx.PointToClient(Control.MousePosition);
                   // m_CurrentExitOrMotorTest = 0;
                    this.VolveTestcontextMenuStrip.Show(this.ExitlistViewEx, point);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ExitlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ExitlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 出口List中SubItemEndEditing事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitlistViewEx_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        {
            try
            {
                ListViewEx.ListViewEx listviewex = (ListViewEx.ListViewEx)sender;

                short p1, p2, p3, nDriverPin;
                p1 = p2 = p3 = nDriverPin = 0;

                switch (e.SubItem)
                {
                    case 1:
                        if (e.Item.Index < 4)
                            tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDis = (short)int.Parse(e.DisplayText);
                        else
                            tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDis = (short)int.Parse(e.DisplayText);
                        break;
                    case 2:
                        if (e.Item.Index >= 4)
                            tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nOffset = (short)int.Parse(e.DisplayText);
                        break;
                    case 3:
                        if (e.Item.Index < 4)
                        {
                            p2 = GetDriverPinPara2(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDriverPin);
                            p3 = GetDriverPinPara3(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDriverPin);
                            nDriverPin = SetDriverPin(short.Parse(e.DisplayText), p2, p3);
                            if (IsDriverPinExits(nDriverPin,0,e.Item.Index))
                            {
                                //MessageBox.Show(string.Format("贴标{0} 驱动器管脚值设置重复！", e.Item.Index + 1));
                                //MessageBox.Show(string.Format("0x30001005 Label{0}'s driver pin setting repeat!", e.Item.Index + 1),"Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
                                MessageBox.Show(string.Format("0x30001005 " + LanguageContainer.ChannelExitMessagebox4Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                    LanguageContainer.ChannelExitMessagebox4Sub2Text[GlobalDataInterface.selectLanguageIndex], 
                                    e.Item.Index + 1),
                                    LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.ExitlistViewEx.StartEditing(ExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                                break;
                                //listviewex.Items[e.Item.Index].SubItems[3].Text = GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDriverPin).ToString();
                            }
                            else
                            {
                                tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDriverPin = nDriverPin;
                            }
                        }
                        else
                        {
                            p2 = GetDriverPinPara2(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin);
                            p3 = GetDriverPinPara3(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin);
                            nDriverPin = SetDriverPin(short.Parse(e.DisplayText), p2, p3);
                            if (IsDriverPinExits(nDriverPin,0,e.Item.Index))
                            {
                                //MessageBox.Show(string.Format("出口{0} 驱动器管脚值设置重复！", e.Item.Index - 3));
                                //MessageBox.Show(string.Format("0x30001005 Outlet{0}'s driver pin setting repeat!", e.Item.Index - 3), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                MessageBox.Show(string.Format("0x30001005 " + LanguageContainer.ChannelExitMessagebox4Sub3Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                    LanguageContainer.ChannelExitMessagebox4Sub4Text[GlobalDataInterface.selectLanguageIndex],
                                    e.Item.Index - 3),
                                    LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.ExitlistViewEx.StartEditing(ExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                                break;
                            }
                            else
                            {

                                tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin = nDriverPin;
                            }
                        }

                        break;
                    case 4:
                        switch (e.DisplayText)
                        {
                            case "A":
                                p2 = 1;
                                break;
                            case "B":
                                p2 = 2;
                                break;
                            case "C":
                                p2 = 3;
                                break;
                            default:
                                //MessageBox.Show("只能输入A，B，C！");
                                //MessageBox.Show("0x30001006 You can only enter A/B/C!","Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
                                MessageBox.Show("0x30001006 " + LanguageContainer.ChannelExitMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                                    LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                e.DisplayText = "";
                                return;
                            //break;

                        }
                        if (e.Item.Index < 4)
                        {
                            p1 = GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDriverPin);
                            p3 = GetDriverPinPara3(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDriverPin);
                            nDriverPin = SetDriverPin(p1, p2, p3);
                            if (IsDriverPinExits(nDriverPin,0, e.Item.Index))
                            {
                                //MessageBox.Show(string.Format("贴标{0} 驱动器管脚值设置重复！", e.Item.Index + 1));
                                //MessageBox.Show(string.Format("0x30001005 Label{0}'s driver pin setting repeat!", e.Item.Index + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                MessageBox.Show(string.Format("0x30001005 " + LanguageContainer.ChannelExitMessagebox4Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                    LanguageContainer.ChannelExitMessagebox4Sub2Text[GlobalDataInterface.selectLanguageIndex],
                                    e.Item.Index + 1),
                                    LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.ExitlistViewEx.StartEditing(ExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                                break;
                                //switch (GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDriverPin))
                                //{
                                //    case 1:
                                //        listviewex.Items[e.Item.Index].SubItems[4].Text = "A";
                                //        break;
                                //    case 2:
                                //        listviewex.Items[e.Item.Index].SubItems[4].Text = "B";
                                //        break;
                                //    case 3:
                                //        listviewex.Items[e.Item.Index].SubItems[4].Text = "C";
                                //        break;
                                //}
                            }
                            else
                            {
                                tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDriverPin = nDriverPin;
                            }
                        }
                        else
                        {
                            p1 = GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin);
                            p3 = GetDriverPinPara3(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin);
                            nDriverPin = SetDriverPin(p1, p2, p3);
                            if (IsDriverPinExits(nDriverPin,0, e.Item.Index))
                            {
                                //MessageBox.Show(string.Format("出口{0} 驱动器管脚值设置重复！", e.Item.Index - 3));
                                //MessageBox.Show(string.Format("0x30001005 Outlet{0}'s driver pin setting repeat!", e.Item.Index - 3), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                MessageBox.Show(string.Format("0x30001005 " + LanguageContainer.ChannelExitMessagebox4Sub3Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                    LanguageContainer.ChannelExitMessagebox4Sub4Text[GlobalDataInterface.selectLanguageIndex],
                                    e.Item.Index - 3),
                                    LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.ExitlistViewEx.StartEditing(ExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                                break;
                                //switch (GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin))
                                //{
                                //    case 1:
                                //        listviewex.Items[e.Item.Index].SubItems[4].Text = "A";
                                //        break;
                                //    case 2:
                                //        listviewex.Items[e.Item.Index].SubItems[4].Text = "B";
                                //        break;
                                //    case 3:
                                //        listviewex.Items[e.Item.Index].SubItems[4].Text = "C";
                                //        break;
                                //}
                            }
                            else
                            {
                                tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin = nDriverPin;
                            }
                        }

                        break;
                    case 5:
                        if (int.Parse(e.DisplayText) < 0 || int.Parse(e.DisplayText) > 18)
                        {
                            //MessageBox.Show("只能输入0~18!");
                            //MessageBox.Show("0x30001007 You can only enter 0~18!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x30001007 " + LanguageContainer.ChannelExitMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //this.ExitlistViewEx.StartEditing(ExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                            e.DisplayText = "0";
                            return;
                        }
                        if (e.Item.Index < 4)
                        {
                            p1 = GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDriverPin);
                            p2 = GetDriverPinPara2(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDriverPin);
                            //  p3 = short.Parse(e.DisplayText);
                            nDriverPin = SetDriverPin(p1, p2, short.Parse(e.DisplayText));
                            if (IsDriverPinExits(nDriverPin,0, e.Item.Index))
                            {
                                //this.ExitlistViewEx.StartEditing(ExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                                //MessageBox.Show(string.Format("贴标{0} 驱动器管脚值设置重复！", e.Item.Index + 1));
                                //MessageBox.Show(string.Format("0x30001005 Label{0}'s driver pin setting repeat!", e.Item.Index + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                MessageBox.Show(string.Format("0x30001005 " + LanguageContainer.ChannelExitMessagebox4Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                    LanguageContainer.ChannelExitMessagebox4Sub2Text[GlobalDataInterface.selectLanguageIndex],
                                    e.Item.Index + 1),
                                    LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.ExitlistViewEx.StartEditing(ExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                                break;
                                //this.ExitlistViewEx.Focus();
                                //e.Item.Selected = true;
                                //e.Item.Focused = true;
                                //e.Item.EnsureVisible() = true;
                                //listviewex.Items[e.Item.Index].SubItems[5].Text = GetDriverPinPara3(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDriverPin).ToString();
                            }
                            else
                            {
                                tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].labelexit[e.Item.Index].nDriverPin = nDriverPin;
                            }
                        }
                        else
                        {
                            p1 = GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin);
                            p2 = GetDriverPinPara2(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin);
                            // p3 = short.Parse(e.DisplayText);
                            nDriverPin = SetDriverPin(p1, p2, short.Parse(e.DisplayText));
                            if (IsDriverPinExits(nDriverPin,0, e.Item.Index))
                            {
                                //MessageBox.Show(string.Format("出口{0} 驱动器管脚值设置重复！", e.Item.Index - 3));
                                //MessageBox.Show(string.Format("0x30001005 Outlet{0}'s driver pin setting repeat!", e.Item.Index - 3), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                MessageBox.Show(string.Format("0x30001005 " + LanguageContainer.ChannelExitMessagebox4Sub3Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                    LanguageContainer.ChannelExitMessagebox4Sub4Text[GlobalDataInterface.selectLanguageIndex],
                                    e.Item.Index - 3),
                                    LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.ExitlistViewEx.StartEditing(ExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                                break;
                                // listviewex.Items[e.Item.Index].SubItems[5].Text = GetDriverPinPara3(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin).ToString();
                            }
                            else
                            {
                                tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin = nDriverPin;
                            }
                        }

                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ExitlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ExitlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }


        /// <summary>
        /// 电机列表双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMotorDriverPinlistViewEx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    m_ChannelExitMotorMouseDownType = 1;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ExitMotorDriverPinlistViewEx_MouseDoubleClick出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ExitMotorDriverPinlistViewEx_MouseDoubleClick出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 电机列表鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMotorDriverPinlistViewEx_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    //m_ChannelExitMouseDownType = 1;
                    m_ChannelExitMotorMouseOnceDownType = 0;//左键单击
                else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    m_ChannelExitMotorMouseOnceDownType = 1;//右键单击
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ExitlistViewEx_MouseDown出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ExitlistViewEx_MouseDown出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 电机出口List中SubItemClicked事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMotorDriverPinlistViewEx_SubItemClicked(object sender, SubItemEventArgs e)
        {
            try
            {
                if (e.SubItem > 0 && m_ChannelExitMotorMouseDownType == 1)
                {
                    this.ExitMotorDriverPinlistViewEx.StartEditing(MotorExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                    m_ChannelExitMotorMouseDownType = 0;
                }
                if (m_ChannelExitMotorMouseOnceDownType == 1)//右键单击
                {
                    m_CurrentMotorItemIndex = e.Item.Index;
                    //if (e.Item.BackColor == Color.Red)//测试中
                    //{
                    //    this.VolveTestcontextMenuStrip.Items[1].Enabled = true;
                    //    this.VolveTestcontextMenuStrip.Items[0].Enabled = false;
                    //    m_VolveTest.ExitId = 255;

                    //}
                    //else//未测试
                    //{
                    //    this.VolveTestcontextMenuStrip.Items[0].Enabled = true;
                    //    this.VolveTestcontextMenuStrip.Items[1].Enabled = false;

                    //}
                    //Point point = new Point();
                    //point.X = Cursor.Position.X - this.Location.X + this.ExitlistViewEx.Location.X;
                    //point.Y = Cursor.Position.Y - this.Location.Y - 3 * this.ExitlistViewEx.Location.Y - 30;
                    Point point = this.ExitMotorDriverPinlistViewEx.PointToClient(Control.MousePosition);
                   // m_CurrentExitOrMotorTest = 1;
                    this.MotoTestcontextMenuStrip.Show(this.ExitMotorDriverPinlistViewEx, point);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ExitMotorDriverPinlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ExitMotorDriverPinlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 电机列表右键测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TesttoolStripMenuItem_Click(object sender, EventArgs e)
        {
            stVolveTest volveTest = new stVolveTest(true);
            volveTest.ExitId = (byte)(m_CurrentMotorItemIndex + 48);
            if (GlobalDataInterface.global_IsTestMode)
                GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ExitChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, volveTest);
        }

        /// <summary>
        /// 判断重复引脚设置
        /// </summary>
        /// <param name="nDriverPin"></param>
        /// <param name="ExitorMotor"></param>出口列表还是电机列表，0为出口，1为电机
        /// <param name="ItemIndex"></param>
        /// <returns></returns>
        private bool IsDriverPinExits(short nDriverPin,int ExitorMotor,int ItemIndex)
        {
            try
            {
                stExitInfo exit;
                if ((nDriverPin & 511) > 0 && (nDriverPin & 3854) > 0 && (nDriverPin & 61440) > 0)
                {
                    if (ExitorMotor == 1)//当前设置电机列表管脚
                    {
                        for (int j = 0; j < GlobalDataInterface.globalOut_SysConfig.nExitNum; j++)
                        {
                            if ((nDriverPin == tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[j]) && !(ItemIndex == j))//不与自己比较
                                return true;
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_CHANNEL_NUM; i++)
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + i] == 1)
                            if (i < GlobalDataInterface.globalOut_SysConfig.nChannelInfo[m_ExitSubsysindex])  //Modify by ChengSk - 20190521
                            {                  
                                exit = tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + i];

                                for (int j = 0; j < ConstPreDefine.MAX_LABEL_NUM; j++)
                                {
                                    if (nDriverPin == exit.labelexit[j].nDriverPin)
                                        return true;
                                }
                                for (int j = 0; j < GlobalDataInterface.globalOut_SysConfig.nExitNum; j++)
                                {
                                    if (nDriverPin == exit.exits[j].nDriverPin)
                                        return true;
                                }
                            }
                        }
                    }
                    else//设置出口列表管脚
                    {
                        if (GlobalDataInterface.globalOut_SysConfig.nSubsysNum == 1)
                        {
                            for (int j = 0; j < GlobalDataInterface.globalOut_SysConfig.nExitNum; j++)
                            {
                                if (nDriverPin == tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[j])
                                    return true;
                            }
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_CHANNEL_NUM; i++)
                        {
                            //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + i] == 1)
                            if (i < GlobalDataInterface.globalOut_SysConfig.nChannelInfo[m_ExitSubsysindex])  //Modify by ChengSk - 20190521
                            {
                                exit = tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + i];

                                if (ItemIndex < ConstPreDefine.MAX_LABEL_NUM)//贴标
                                {
                                    for (int j = 0; j < ConstPreDefine.MAX_LABEL_NUM; j++)
                                    {
                                        if (nDriverPin == exit.labelexit[j].nDriverPin && !(ItemIndex == j && m_ExitSubsysChannelIndex == i))//不与自己比较
                                            return true;
                                    }
                                    for (int j = 0; j < GlobalDataInterface.globalOut_SysConfig.nExitNum; j++)
                                    {
                                        if (nDriverPin == exit.exits[j].nDriverPin)
                                            return true;
                                    }
                                }
                                else//出口
                                {
                                    for (int j = 0; j < ConstPreDefine.MAX_LABEL_NUM; j++)
                                    {
                                        if (nDriverPin == exit.labelexit[j].nDriverPin)
                                            return true;
                                    }
                                    for (int j = 0; j < GlobalDataInterface.globalOut_SysConfig.nExitNum; j++)
                                    {
                                        if (nDriverPin == exit.exits[j].nDriverPin && !((ItemIndex - ConstPreDefine.MAX_LABEL_NUM) == j && m_ExitSubsysChannelIndex == i))//不与自己比较
                                            return true;

                                    }
                                }   
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ExitlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ExitlistViewEx_SubItemEndEditing出错" + ex);
#endif
                return false;
            }
        }


        /// <summary>
        /// 电机出口List中SubItemEndEditing事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMotorDriverPinlistViewEx_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        {
            try
            {
                ListViewEx.ListViewEx listviewex = (ListViewEx.ListViewEx)sender;

                short p1, p2, p3, nDriverPin;
                p1 = p2 = p3 = nDriverPin = 0;

                switch (e.SubItem)
                {
                    case 1:

                        p2 = GetDriverPinPara2(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[e.Item.Index]);
                        p3 = GetDriverPinPara3(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[e.Item.Index]);
                        nDriverPin = SetDriverPin(short.Parse(e.DisplayText), p2, p3);
                        if (IsDriverPinExits(nDriverPin, 1, e.Item.Index))
                        {
                            //MessageBox.Show(string.Format("出口{0} 驱动器管脚值设置重复！", e.Item.Index - 3));
                            //MessageBox.Show(string.Format("0x30001005 MotorOutlet{0}'s driver pin setting repeat!", e.Item.Index +1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001005 " + LanguageContainer.ChannelExitMessagebox4Sub3Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                    LanguageContainer.ChannelExitMessagebox4Sub4Text[GlobalDataInterface.selectLanguageIndex],
                                    e.Item.Index + 1),
                                    LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.ExitMotorDriverPinlistViewEx.StartEditing(MotorExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                            //listviewex.Items[e.Item.Index].SubItems[3].Text = GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin).ToString();
                        }
                        else
                        {

                            tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[e.Item.Index] = nDriverPin;
                        }
                        break;
                    case 2:
                        switch (e.DisplayText)
                        {
                            case "A":
                                p2 = 1;
                                break;
                            case "B":
                                p2 = 2;
                                break;
                            case "C":
                                p2 = 3;
                                break;
                            default:
                                //MessageBox.Show("只能输入A，B，C！");
                                //MessageBox.Show("0x30001006 You can only enter A/B/C!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                MessageBox.Show("0x30001006 " + LanguageContainer.ChannelExitMessagebox1Text[GlobalDataInterface.selectLanguageIndex], 
                                    LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                e.DisplayText = "";
                                return;
                            //break;

                        }

                        p1 = GetDriverPinPara1(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[e.Item.Index]);
                        p3 = GetDriverPinPara3(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[e.Item.Index]);
                        nDriverPin = SetDriverPin(p1, p2, p3);
                        if (IsDriverPinExits(nDriverPin, 1, e.Item.Index))
                        {
                            //MessageBox.Show(string.Format("出口{0} 驱动器管脚值设置重复！", e.Item.Index - 3));
                            //MessageBox.Show(string.Format("0x30001005 MotorOutlet{0}'s driver pin setting repeat!", e.Item.Index + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001005 " + LanguageContainer.ChannelExitMessagebox4Sub3Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                    LanguageContainer.ChannelExitMessagebox4Sub4Text[GlobalDataInterface.selectLanguageIndex],
                                    e.Item.Index + 1),
                                    LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.ExitMotorDriverPinlistViewEx.StartEditing(MotorExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                            //switch (GetDriverPinPara1(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin))
                            //{
                            //    case 1:
                            //        listviewex.Items[e.Item.Index].SubItems[4].Text = "A";
                            //        break;
                            //    case 2:
                            //        listviewex.Items[e.Item.Index].SubItems[4].Text = "B";
                            //        break;
                            //    case 3:
                            //        listviewex.Items[e.Item.Index].SubItems[4].Text = "C";
                            //        break;
                            //}
                        }
                        else
                        {
                            tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[e.Item.Index] = nDriverPin;
                        }

                        break;
                    case 3:
                        if (int.Parse(e.DisplayText) < 0 || int.Parse(e.DisplayText) > 18)
                        {
                            //MessageBox.Show("只能输入0~18!");
                            //MessageBox.Show("0x30001007 You can only enter 0~18!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x30001007 " + LanguageContainer.ChannelExitMessagebox2Text[GlobalDataInterface.selectLanguageIndex], 
                                LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //this.ExitlistViewEx.StartEditing(ExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                            e.DisplayText = "0";
                            return;
                        }

                        p1 = GetDriverPinPara1(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[e.Item.Index]);
                        p2 = GetDriverPinPara2(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[e.Item.Index]);
                        // p3 = short.Parse(e.DisplayText);
                        nDriverPin = SetDriverPin(p1, p2, short.Parse(e.DisplayText));
                        if (IsDriverPinExits(nDriverPin, 1, e.Item.Index))
                        {
                            //MessageBox.Show(string.Format("出口{0} 驱动器管脚值设置重复！", e.Item.Index - 3));
                            //MessageBox.Show(string.Format("0x30001005 MotorOutlet{0}'s driver pin setting repeat!", e.Item.Index + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001005 " + LanguageContainer.ChannelExitMessagebox4Sub3Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                    LanguageContainer.ChannelExitMessagebox4Sub4Text[GlobalDataInterface.selectLanguageIndex],
                                    e.Item.Index + 1),
                                    LanguageContainer.ChannelExitMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.ExitMotorDriverPinlistViewEx.StartEditing(MotorExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                            // listviewex.Items[e.Item.Index].SubItems[5].Text = GetDriverPinPara3(tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index - ConstPreDefine.MAX_LABEL_NUM].nDriverPin).ToString();
                        }

                        else
                        {
                            tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[e.Item.Index] = nDriverPin;
                        }
                        break;
                    case 4:
                        if (e.Item.Index >= 0)
                            tempGlobalExitInfo[m_ExitSubsysindex ].Delay_time[e.Item.Index] = float.Parse(e.DisplayText);//add by xcw - 20191209
                        break;
                    case 5:
                        if (e.Item.Index >= 0)
                            tempGlobalExitInfo[m_ExitSubsysindex].Hold_time[e.Item.Index] = float.Parse(e.DisplayText);//add by xcw - 20191209
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ExitMotorDriverPinlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ExitMotorDriverPinlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }




//        /// <summary>
//        /// 判断电机重复引脚设置
//        /// </summary>
//        /// <param name="nDriverPin"></param>
//        /// <param name="ItemIndex"></param>
//        /// <returns></returns>
//        private bool IsMotorDriverPinExits(short nDriverPin, int ItemIndex)
//        {
//            try
//            {
//                if ((nDriverPin & 511) > 0 && (nDriverPin & 3854) > 0 && (nDriverPin & 61440) > 0)
//                {

//                    for (int j = 0; j < GlobalDataInterface.globalOut_SysConfig.nExitNum; j++)
//                    {
//                        if ((nDriverPin == tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[j]) && !(ItemIndex == j))
//                            return true;
//                    }


//                }
//                return false;
//            }
//            catch (Exception ex)
//            {
//                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ExitlistViewEx_SubItemEndEditing出错" + ex);
//#if REALEASE
//                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ExitlistViewEx_SubItemEndEditing出错" + ex);
//#endif
//                return false;
//            }
//        }

        /// <summary>
        /// 通道出口保存配置
        /// </summary>
        /// <returns></returns>
        private bool ChannelExitSaveConfig()
        {
            try
            {
                if (m_ExitSubsysindex >= 0 && m_ExitSubsysChannelIndex >= 0 && m_ExitChannelSelectIndex >= 0)
                {
                    //tempGlobalExitInfo[m_ExitSubsysindex].nPulse = int.Parse(this.ElecPlusenumericUpDown.Text);
                    //tempGlobalExitInfo[m_ExitSubsysindex].nLabelPulse = int.Parse(this.LabelPlusenumericUpDown.Text);

                    //设置无效驱动器管脚
                    short nInvalidDriverPin = SetDriverPin(0, 0, 0);
                    short p1, p2, p3;
                    stExitInfo exit = tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex];
                    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nExitNum; i++)
                    {
                        p1 = GetDriverPinPara1(exit.exits[i].nDriverPin);
                        p2 = GetDriverPinPara2(exit.exits[i].nDriverPin);
                        p3 = GetDriverPinPara3(exit.exits[i].nDriverPin);
                        if (p1 == 0 || p2 == 0 || p3 == 0)
                        {
                            exit.exits[i].nDriverPin = nInvalidDriverPin;
                            this.ExitlistViewEx.Items[i + 4].SubItems[3].Text = "0";
                            this.ExitlistViewEx.Items[i + 4].SubItems[4].Text = "";
                            this.ExitlistViewEx.Items[i + 4].SubItems[5].Text = "0";
                        }

                        //p1 = GetDriverPinPara1(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                        //p2 = GetDriverPinPara2(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                        //p3 = GetDriverPinPara3(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                        //if (p1 == 0 || p2 == 0 || p3 == 0)
                        //{
                        //    tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i] = nInvalidDriverPin;
                        //    this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[1].Text = "0";
                        //    this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[2].Text = "";
                        //    this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[3].Text = "0";
                        //}
                    }
                    for (int i = 0; i < ConstPreDefine.MAX_LABEL_NUM; i++)
                    {
                        p1 = GetDriverPinPara1(exit.labelexit[i].nDriverPin);
                        p2 = GetDriverPinPara2(exit.labelexit[i].nDriverPin);
                        p3 = GetDriverPinPara3(exit.labelexit[i].nDriverPin);
                        if (p1 == 0 || p2 == 0 || p3 == 0)
                        {
                            exit.labelexit[i].nDriverPin = nInvalidDriverPin;
                            this.ExitlistViewEx.Items[i].SubItems[3].Text = "0";
                            this.ExitlistViewEx.Items[i].SubItems[4].Text = "";
                            this.ExitlistViewEx.Items[i].SubItems[5].Text = "0";
                        }
                    }

                    ////非有效出口变量（距离、偏移、驱动器管脚）设为零 Add by ChengSk - 20190228
                    //for(int i = GlobalDataInterface.globalOut_SysConfig.nExitNum; i < ConstPreDefine.MAX_EXIT_NUM; i++)
                    //{
                    //    tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDis = 0;
                    //    tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nOffset = 0;
                    //    tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDriverPin = 0;
                    //} 
                    //delete by xcw 20200902

                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    {
                        GlobalDataInterface.globalOut_ExitInfo[i].ToCopy(tempExitInfo[i]);
                    }
                    //tempGlobalExitInfo.CopyTo(GlobalDataInterface.globalOut_GlobalExitInfo, 0);
                    if(GlobalDataInterface.global_IsTestMode)
                    {
                        //int DrcId = Commonfunction.EncodeChannel(m_ExitSubsysindex, m_ExitChannelInIPMIndex, m_ExitChannelInIPMIndex);
                        GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ExitChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null);

                        //for(int i =0;i<GlobalDataInterface.globalOut_SysConfig.nSubsysNum;i++)
                        //{
                        //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeSubsys(i), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_EXIT_INFO, null);
                        //}
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("通道出口设置界面保存配置出错：" + ex);
                //MessageBox.Show("0x10001007 Lane Outlet save error: " + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("0x10001007 " + LanguageContainer.ChannelExitMessagebox3Text[GlobalDataInterface.selectLanguageIndex] + ex,
                    LanguageContainer.ChannelExitMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        /// <summary>
        /// 通道出口所有通道数据保存保存配置
        /// </summary>
        /// <returns></returns>
        private bool ChannelExitSaveConfig1()
        {
            try
            {
                if (m_ExitSubsysindex >= 0 && m_ExitSubsysChannelIndex >= 0 && m_ExitChannelSelectIndex >= 0)
                {
                    //tempGlobalExitInfo[m_ExitSubsysindex].nPulse = int.Parse(this.ElecPlusenumericUpDown.Text);
                    //tempGlobalExitInfo[m_ExitSubsysindex].nLabelPulse = int.Parse(this.LabelPlusenumericUpDown.Text);

                    //设置无效驱动器管脚
                    short nInvalidDriverPin = SetDriverPin(0, 0, 0);
                    short p1, p2, p3;
                    stExitInfo exit = tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex];
                    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nExitNum; i++)
                    {
                        p1 = GetDriverPinPara1(exit.exits[i].nDriverPin);
                        p2 = GetDriverPinPara2(exit.exits[i].nDriverPin);
                        p3 = GetDriverPinPara3(exit.exits[i].nDriverPin);
                        if (p1 == 0 || p2 == 0 || p3 == 0)
                        {
                            exit.exits[i].nDriverPin = nInvalidDriverPin;
                            this.ExitlistViewEx.Items[i + 4].SubItems[3].Text = "0";
                            this.ExitlistViewEx.Items[i + 4].SubItems[4].Text = "";
                            this.ExitlistViewEx.Items[i + 4].SubItems[5].Text = "0";
                        }

                        //p1 = GetDriverPinPara1(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                        //p2 = GetDriverPinPara2(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                        //p3 = GetDriverPinPara3(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                        //if (p1 == 0 || p2 == 0 || p3 == 0)
                        //{
                        //    tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i] = nInvalidDriverPin;
                        //    this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[1].Text = "0";
                        //    this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[2].Text = "";
                        //    this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[3].Text = "0";
                        //}
                    }
                    for (int i = 0; i < ConstPreDefine.MAX_LABEL_NUM; i++)
                    {
                        p1 = GetDriverPinPara1(exit.labelexit[i].nDriverPin);
                        p2 = GetDriverPinPara2(exit.labelexit[i].nDriverPin);
                        p3 = GetDriverPinPara3(exit.labelexit[i].nDriverPin);
                        if (p1 == 0 || p2 == 0 || p3 == 0)
                        {
                            exit.labelexit[i].nDriverPin = nInvalidDriverPin;
                            this.ExitlistViewEx.Items[i].SubItems[3].Text = "0";
                            this.ExitlistViewEx.Items[i].SubItems[4].Text = "";
                            this.ExitlistViewEx.Items[i].SubItems[5].Text = "0";
                        }
                    }

                    ////非有效出口变量（距离、偏移、驱动器管脚）设为零 Add by ChengSk - 20190228
                    //for(int i = GlobalDataInterface.globalOut_SysConfig.nExitNum; i < ConstPreDefine.MAX_EXIT_NUM; i++)
                    //{
                    //    tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDis = 0;
                    //    tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nOffset = 0;
                    //    tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDriverPin = 0;
                    //} 
                    //delete by xcw 20200902
                    //非有效通道变量（驱动器管脚）设为零 Add by xcw - 20201204
                    for (int j = m_ChanelIDList.Count; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    {
                        for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM; i++)
                        {
                            //tempExitInfo[0 * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[i].nDis = 0;
                            //tempExitInfo[0 * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[i].nOffset = 0;
                            tempExitInfo[0 * ConstPreDefine.MAX_CHANNEL_NUM + j].exits[i].nDriverPin = 0;
                        }
                    }

                      

                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    {
                        GlobalDataInterface.globalOut_ExitInfo[i].ToCopy(tempExitInfo[i]);
                    }
                    //tempGlobalExitInfo.CopyTo(GlobalDataInterface.globalOut_GlobalExitInfo, 0);
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        for(int i = 0; i < m_ChanelExitList.Count; i++)
                        {
                            GlobalDataInterface.TransmitParam(m_ChanelExitList[i], (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null);
                            Thread.Sleep(1000);//add by xcw 20201204
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("通道出口设置界面保存配置出错：" + ex);
                //MessageBox.Show("0x10001007 Lane Outlet save error: " + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("0x10001007 " + LanguageContainer.ChannelExitMessagebox3Text[GlobalDataInterface.selectLanguageIndex] + ex,
                    LanguageContainer.ChannelExitMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }
        /// <summary>
        /// 通道出口保存配置 （另存专用，不给FSM发送指令）    Add by ChengSk - 20190116
        /// </summary>
        /// <returns></returns>
        private bool ChannelExitSaveConfig2()
        {
            try
            {
                if (m_ExitSubsysindex >= 0 && m_ExitSubsysChannelIndex >= 0 && m_ExitChannelSelectIndex >= 0)
                {
                    //tempGlobalExitInfo[m_ExitSubsysindex].nPulse = int.Parse(this.ElecPlusenumericUpDown.Text);
                    //tempGlobalExitInfo[m_ExitSubsysindex].nLabelPulse = int.Parse(this.LabelPlusenumericUpDown.Text);

                    //设置无效驱动器管脚
                    short nInvalidDriverPin = SetDriverPin(0, 0, 0);
                    short p1, p2, p3;
                    stExitInfo exit = tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex];
                    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nExitNum; i++)
                    {
                        p1 = GetDriverPinPara1(exit.exits[i].nDriverPin);
                        p2 = GetDriverPinPara2(exit.exits[i].nDriverPin);
                        p3 = GetDriverPinPara3(exit.exits[i].nDriverPin);
                        if (p1 == 0 || p2 == 0 || p3 == 0)
                        {
                            exit.exits[i].nDriverPin = nInvalidDriverPin;
                            this.ExitlistViewEx.Items[i + 4].SubItems[3].Text = "0";
                            this.ExitlistViewEx.Items[i + 4].SubItems[4].Text = "";
                            this.ExitlistViewEx.Items[i + 4].SubItems[5].Text = "0";
                        }

                        //p1 = GetDriverPinPara1(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                        //p2 = GetDriverPinPara2(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                        //p3 = GetDriverPinPara3(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                        //if (p1 == 0 || p2 == 0 || p3 == 0)
                        //{
                        //    tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i] = nInvalidDriverPin;
                        //    this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[1].Text = "0";
                        //    this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[2].Text = "";
                        //    this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[3].Text = "0";
                        //}
                    }
                    for (int i = 0; i < ConstPreDefine.MAX_LABEL_NUM; i++)
                    {
                        p1 = GetDriverPinPara1(exit.labelexit[i].nDriverPin);
                        p2 = GetDriverPinPara2(exit.labelexit[i].nDriverPin);
                        p3 = GetDriverPinPara3(exit.labelexit[i].nDriverPin);
                        if (p1 == 0 || p2 == 0 || p3 == 0)
                        {
                            exit.labelexit[i].nDriverPin = nInvalidDriverPin;
                            this.ExitlistViewEx.Items[i].SubItems[3].Text = "0";
                            this.ExitlistViewEx.Items[i].SubItems[4].Text = "";
                            this.ExitlistViewEx.Items[i].SubItems[5].Text = "0";
                        }
                    }


                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    {
                        GlobalDataInterface.globalOut_ExitInfo[i].ToCopy(tempExitInfo[i]);
                    }
                    //tempGlobalExitInfo.CopyTo(GlobalDataInterface.globalOut_GlobalExitInfo, 0);
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        //int DrcId = Commonfunction.EncodeChannel(m_ExitSubsysindex, m_ExitChannelInIPMIndex, m_ExitChannelInIPMIndex);
                        GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ExitChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null); //Note by ChengSk - 20190116

                        //for(int i =0;i<GlobalDataInterface.globalOut_SysConfig.nSubsysNum;i++)
                        //{
                        //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeSubsys(i), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_EXIT_INFO, null);
                        //}
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("通道出口设置界面保存配置出错：" + ex);
                //MessageBox.Show("0x10001007 Lane Outlet save error: " + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("0x10001007 " + LanguageContainer.ChannelExitMessagebox3Text[GlobalDataInterface.selectLanguageIndex] + ex,
                    LanguageContainer.ChannelExitMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        /// <summary>
        /// 通道出口保存配置
        /// </summary>
        /// <returns></returns>
        private bool ExitMotorDriverPinSaveConfig()
        {
            try
            {
                //if (m_ExitSubsysindex >= 0 && m_ExitSubsysChannelIndex >= 0 && m_ExitChannelSelectIndex >= 0)
                //{
                tempGlobalExitInfo[m_ExitSubsysindex].nPulse = (short)int.Parse(this.ElecPlusenumericUpDown.Text);
                tempGlobalExitInfo[m_ExitSubsysindex].nLabelPulse = (short)int.Parse(this.LabelPlusenumericUpDown.Text);

                    //设置无效驱动器管脚
                    short nInvalidDriverPin = SetDriverPin(0, 0, 0);
                    short p1, p2, p3;
                    stExitInfo exit = tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex];
                    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nExitNum; i++)
                    {
                        //p1 = GetDriverPinPara1(exit.exits[i].nDriverPin);
                        //p2 = GetDriverPinPara2(exit.exits[i].nDriverPin);
                        //p3 = GetDriverPinPara3(exit.exits[i].nDriverPin);
                        //if (p1 == 0 || p2 == 0 || p3 == 0)
                        //{
                        //    exit.exits[i].nDriverPin = nInvalidDriverPin;
                        //    this.ExitlistViewEx.Items[i + 4].SubItems[3].Text = "0";
                        //    this.ExitlistViewEx.Items[i + 4].SubItems[4].Text = "";
                        //    this.ExitlistViewEx.Items[i + 4].SubItems[5].Text = "0";
                        //}

                        p1 = GetDriverPinPara1(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                        p2 = GetDriverPinPara2(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                        p3 = GetDriverPinPara3(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                        if (p1 == 0 || p2 == 0 || p3 == 0)
                        {
                            tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i] = nInvalidDriverPin;
                            this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[1].Text = "0";
                            this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[2].Text = "";
                            this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[3].Text = "0";
                        }
                    }
                    //for (int i = 0; i < ConstPreDefine.MAX_LABEL_NUM; i++)
                    //{
                    //    p1 = GetDriverPinPara1(exit.labelexit[i].nDriverPin);
                    //    p2 = GetDriverPinPara2(exit.labelexit[i].nDriverPin);
                    //    p3 = GetDriverPinPara3(exit.labelexit[i].nDriverPin);
                    //    if (p1 == 0 || p2 == 0 || p3 == 0)
                    //    {
                    //        exit.labelexit[i].nDriverPin = nInvalidDriverPin;
                    //        this.ExitlistViewEx.Items[i].SubItems[3].Text = "0";
                    //        this.ExitlistViewEx.Items[i].SubItems[4].Text = "";
                    //        this.ExitlistViewEx.Items[i].SubItems[5].Text = "0";
                    //    }
                    //}

                    //非有效出口变量（电机输出管脚）设为零 Add by ChengSk - 20190228
                    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++)
                    {
                        for (int j = GlobalDataInterface.globalOut_SysConfig.nExitNum; j < ConstPreDefine.MAX_EXIT_NUM; j++)
                        {
                            tempGlobalExitInfo[i].nDriverPin[j] = 0; 
                        }
                    }

                    //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    //{
                    //    GlobalDataInterface.globalOut_ExitInfo[i].ToCopy(tempExitInfo[i]);
                    //}
                    tempGlobalExitInfo.CopyTo(GlobalDataInterface.globalOut_GlobalExitInfo, 0);
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        //int DrcId = Commonfunction.EncodeChannel(m_ExitSubsysindex, m_ExitChannelInIPMIndex, m_ExitChannelInIPMIndex);
                        //GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ExitChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null);

                        for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++)
                        {
                            GlobalDataInterface.TransmitParam(Commonfunction.EncodeSubsys(i), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_EXIT_INFO, null);
                        }
                    }
               // }
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("通道出口设置界面保存配置出错：" + ex);
                //MessageBox.Show("0x10001007 Lane Outlet save error: " + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("0x10001007 " + LanguageContainer.ChannelExitMessagebox3Text[GlobalDataInterface.selectLanguageIndex] + ex,
                    LanguageContainer.ChannelExitMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        /// <summary>
        /// 通道出口保存配置 （另存专用，不给FSM发送指令）    Add by ChengSk - 20190116
        /// </summary>
        /// <returns></returns>
        private bool ExitMotorDriverPinSaveConfig2()
        {
            try
            {
                //if (m_ExitSubsysindex >= 0 && m_ExitSubsysChannelIndex >= 0 && m_ExitChannelSelectIndex >= 0)
                //{
                tempGlobalExitInfo[m_ExitSubsysindex].nPulse = (short)int.Parse(this.ElecPlusenumericUpDown.Text);
                tempGlobalExitInfo[m_ExitSubsysindex].nLabelPulse = (short)int.Parse(this.LabelPlusenumericUpDown.Text);

                //设置无效驱动器管脚
                short nInvalidDriverPin = SetDriverPin(0, 0, 0);
                short p1, p2, p3;
                stExitInfo exit = tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex];
                for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nExitNum; i++)
                {
                    //p1 = GetDriverPinPara1(exit.exits[i].nDriverPin);
                    //p2 = GetDriverPinPara2(exit.exits[i].nDriverPin);
                    //p3 = GetDriverPinPara3(exit.exits[i].nDriverPin);
                    //if (p1 == 0 || p2 == 0 || p3 == 0)
                    //{
                    //    exit.exits[i].nDriverPin = nInvalidDriverPin;
                    //    this.ExitlistViewEx.Items[i + 4].SubItems[3].Text = "0";
                    //    this.ExitlistViewEx.Items[i + 4].SubItems[4].Text = "";
                    //    this.ExitlistViewEx.Items[i + 4].SubItems[5].Text = "0";
                    //}

                    p1 = GetDriverPinPara1(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                    p2 = GetDriverPinPara2(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                    p3 = GetDriverPinPara3(tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i]);
                    if (p1 == 0 || p2 == 0 || p3 == 0)
                    {
                        tempGlobalExitInfo[m_ExitSubsysindex].nDriverPin[i] = nInvalidDriverPin;
                        this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[1].Text = "0";
                        this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[2].Text = "";
                        this.ExitMotorDriverPinlistViewEx.Items[i].SubItems[3].Text = "0";
                    }
                }
                //for (int i = 0; i < ConstPreDefine.MAX_LABEL_NUM; i++)
                //{
                //    p1 = GetDriverPinPara1(exit.labelexit[i].nDriverPin);
                //    p2 = GetDriverPinPara2(exit.labelexit[i].nDriverPin);
                //    p3 = GetDriverPinPara3(exit.labelexit[i].nDriverPin);
                //    if (p1 == 0 || p2 == 0 || p3 == 0)
                //    {
                //        exit.labelexit[i].nDriverPin = nInvalidDriverPin;
                //        this.ExitlistViewEx.Items[i].SubItems[3].Text = "0";
                //        this.ExitlistViewEx.Items[i].SubItems[4].Text = "";
                //        this.ExitlistViewEx.Items[i].SubItems[5].Text = "0";
                //    }
                //}


                //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                //{
                //    GlobalDataInterface.globalOut_ExitInfo[i].ToCopy(tempExitInfo[i]);
                //}
                tempGlobalExitInfo.CopyTo(GlobalDataInterface.globalOut_GlobalExitInfo, 0);
                //if (GlobalDataInterface.global_IsTestMode)
                //{
                //    //int DrcId = Commonfunction.EncodeChannel(m_ExitSubsysindex, m_ExitChannelInIPMIndex, m_ExitChannelInIPMIndex);
                //    //GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ExitChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null);

                //    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++)
                //    {
                //        GlobalDataInterface.TransmitParam(Commonfunction.EncodeSubsys(i), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_EXIT_INFO, null);
                //    }
                //} //Note by ChengSk - 20190116
                // }
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("通道出口设置界面保存配置出错：" + ex);
                //MessageBox.Show("0x10001007 Lane Outlet save error: " + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("0x10001007 " + LanguageContainer.ChannelExitMessagebox3Text[GlobalDataInterface.selectLanguageIndex] + ex,
                    LanguageContainer.ChannelExitMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        /// <summary>
        /// 电机设置立即生效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMotorDriverPinEffectbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ExitMotorDriverPinSaveConfig())
                    return;
                this.ExitMotorDriverPinEffectbutton.Enabled = false;
                this.EffectButtonDelaytimer3.Enabled = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ChannelExitEffectbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ChannelExitEffectbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 立即生效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelExitEffectbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ChannelExitSaveConfig())
                    return;
                this.ChannelExitEffectbutton.Enabled = false;
                this.EffectButtonDelaytimer3.Enabled = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数ChannelExitEffectbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数ChannelExitEffectbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 立即生效后延迟1.5秒再启用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffectButtonDelaytimer3_Tick(object sender, EventArgs e)
        {
            this.ChannelExitEffectbutton.Enabled = true;
            this.ExitMotorDriverPinEffectbutton.Enabled = true;
            this.DriverPinResetbutton.Enabled = true;
            this.EffectButtonDelaytimer3.Enabled = false;
        }

        /// <summary>
        /// 停止电磁阀测试
        /// </summary>
        private void StopVolveTest()
        {
            if (m_IsVolveTesting)
            {
                try
                {
                    if (m_VolveTest.ExitId != 255)
                    {
                        if (m_VolveTest.ExitId <= 100)
                        {
                            this.ExitlistViewEx.Items[m_VolveTest.ExitId + 4].BackColor = Color.White;
                            this.ExitlistViewEx.Items[m_VolveTest.ExitId + 4].Font = new Font("宋体", 9, FontStyle.Regular);//ChengSk
                        }
                        else if (m_VolveTest.ExitId > 100 && m_VolveTest.ExitId < 105)
                        {
                            this.ExitlistViewEx.Items[m_VolveTest.ExitId - 101].BackColor = Color.White;
                            this.ExitlistViewEx.Items[m_VolveTest.ExitId - 101].Font = new Font("宋体", 9, FontStyle.Regular);//ChengSk
                        }
                            

                        m_VolveTest.ExitId = 255;
                        if (GlobalDataInterface.global_IsTestMode)
                            GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ExitChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, m_VolveTest);

                    }
                    m_IsVolveTesting = false;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProjectSetForm-ChannelExit中函数Stop_Click出错" + ex);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数Stop_Click出错" + ex);
#endif
                }
            }
        }

        /// <summary>
        /// 设置电机管脚列表启用
        /// </summary>
        public void IsEnableExitMotorDriverPinlistViewEx()
        {
            if (GlobalDataInterface.globalOut_SysConfig.nSubsysNum > 1)
            {
                this.ExitMotorDriverPinlistViewEx.Enabled = false;
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM; i++)
                    {
                        GlobalDataInterface.globalOut_GlobalExitInfo[j].nDriverPin[i] = 0;
                    }
                }
            }
            else
            {
                this.ExitMotorDriverPinlistViewEx.Enabled = true;
            }
        }
        private void DriverPinResetbutton_Click(object sender, EventArgs e)
        {
            try
            {
                ChannelExitDriverPinReset channelDriverReset = new ChannelExitDriverPinReset();
                if (channelDriverReset.ShowDialog() == DialogResult.OK)
                {
                    this.DriverPinResetbutton.Enabled = false;
                    if (!ChannelExitSaveConfig1())
                        return;
                  
                    this.EffectButtonDelaytimer3.Enabled = true;
                }
              
               
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数DriverPinResetbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数DriverPinResetbutton_Click出错" + ex);
#endif
            }
        }
      
    }
}
