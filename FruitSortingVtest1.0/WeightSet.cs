using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Interface;
using Common;
using System.Diagnostics;

namespace FruitSortingVtest1._0
{
    public partial class ProjectSetForm : Form
    {
        private static stWeightBaseInfo[] tempWeightBaseInfo = new stWeightBaseInfo[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM];
        private static stGlobalWeightBaseInfo[] tempGlobalWeightBaseInfo = new stGlobalWeightBaseInfo[ConstPreDefine.MAX_SUBSYS_NUM];
        private static int m_WeightChannelSelectIndex = 0;//当前选择通道
        private static int m_WeightSubsysindex = -1;//当前选择通道所属子系统
        private static int m_WeightSubsysChannelIndex = -1;//当前选择通道所属子系统第几个通道
        private static stWeightResult m_WeightResult = new stWeightResult(true);//当前重量数据
        private static bool m_bDataTracking = false;
        private static byte[] m_WaveInterval = new byte[2];
        private static int m_nCount1 = 0;
        private static int m_nCount2 = 0;
        private static ushort m_nStandardAD0 = 0;//AD0标准值
        private static ushort m_nStandardAD1 = 0;//AD1标准值
        private static int m_nTestTraceWeightCount = 0;//测试追踪重量数据个数
        private static float temperatureparams = 0.0f;
        private DoubleBufferListView m_doubleBufferListView;

        private void WeightSetInitial(int IsSys)
        {
            try
            {
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                {
                    tempWeightBaseInfo[i] = new stWeightBaseInfo(true);
                    tempWeightBaseInfo[i].ToCopy(GlobalDataInterface.globalOut_WeightBaseInfo[i]);

                }
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    tempGlobalWeightBaseInfo[i] = new stGlobalWeightBaseInfo(true);
                    tempGlobalWeightBaseInfo[i].ToCopy(GlobalDataInterface.globalOut_GlobalWeightBaseInfo[i]);
                }


                //通道选择控件
                if (this.WeightChannelcomboBox.Items.Count == 0)
                {
                    for (int i = 1; i <= m_ChanelIDList.Count; i++)
                    {
                        this.WeightChannelcomboBox.Items.Add(m_resourceManager.GetString("Lanelabel.Text")+string.Format(" {0}", i));
                    }
                }
                else
                {
                    int oldItemsCount = this.WeightChannelcomboBox.Items.Count;
                    if (this.WeightChannelcomboBox.Items.Count != m_ChanelIDList.Count)
                    {
                        if (this.WeightChannelcomboBox.Items.Count > m_ChanelIDList.Count)
                        {
                            for (int i = oldItemsCount - 1; i >= m_ChanelIDList.Count; i--)
                                this.WeightChannelcomboBox.Items.RemoveAt(i);

                            if (m_WeightChannelSelectIndex > this.WeightChannelcomboBox.Items.Count - 1)
                                m_WeightChannelSelectIndex = 0;
                        }
                        else
                        {
                            for (int i = oldItemsCount + 1; i <= m_ChanelIDList.Count; i++)
                                this.WeightChannelcomboBox.Items.Add(m_resourceManager.GetString("Lanelabel.Text") + string.Format(" {0}", i));
                        }
                    }
                }
                if (IsSys == 0)
                {
                    m_doubleBufferListView = new DoubleBufferListView();
                    m_doubleBufferListView.Name = "TrackDatalistView";
                    m_doubleBufferListView.Location = new Point(16, 15);
                    m_doubleBufferListView.Size = new Size(523, 610);
                    m_doubleBufferListView.View = View.Details;
                    m_doubleBufferListView.GridLines = true;
                    // m_doubleBufferListView.BackColor = Color.Silver;
                    m_doubleBufferListView.Scrollable = true;
                    m_doubleBufferListView.Visible = true;
                    m_doubleBufferListView.Columns.Add(m_resourceManager.GetString("WagonNumcolumnHeader.Text"));
                    m_doubleBufferListView.Columns[0].Width = 80;
                    m_doubleBufferListView.Columns.Add(m_resourceManager.GetString("FruitWeightcolumnHeader.Text"));
                    m_doubleBufferListView.Columns[1].Width = 110;
                    m_doubleBufferListView.Columns.Add(m_resourceManager.GetString("WagonWeightcolumnHeader.Text"));
                    m_doubleBufferListView.Columns[2].Width = 110;
                    m_doubleBufferListView.Columns.Add("AD0");
                    m_doubleBufferListView.Columns[3].Width = 110;
                    m_doubleBufferListView.Columns.Add("AD1");
                    m_doubleBufferListView.Columns[4].Width = 110;
                    //this.Controls.Add(m_doubleBufferListView);
                    this.groupBox7.Controls.Add(m_doubleBufferListView);
                    if (m_WeightChannelSelectIndex >= 0 && this.WeightChannelcomboBox.Items.Count > 0)
                    {
                        this.WeightChannelcomboBox.SelectedIndex = m_WeightChannelSelectIndex;
                        SetWeightInfo(m_WeightChannelSelectIndex);
                    }

                    //Modify by ChengSk - 20190828
                    this.CurrentWeightnumericUpDown.Text = tempGlobalWeightBaseInfo[m_WeightSubsysindex].RefWeight.ToString();
                    this.ThresholdlnumericUpDown.Text = tempGlobalWeightBaseInfo[m_WeightSubsysindex].WeightTh.ToString();
                    //this.CurrentWeightnumericUpDown.Text = Commonfunction.GetAppSetting("重量设置-当前重量");
                    //this.ThresholdlnumericUpDown.Text = Commonfunction.GetAppSetting("重量设置-阈值");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数WeightSetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数WeightSetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 复位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Resetbutton_Click(object sender, EventArgs e)
        {
            try
            {
                m_mainForm.ResetCupState();
                if (GlobalDataInterface.global_IsTestMode)
                    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHTRESET, null);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数Resetbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数Resetbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 内信号源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SingnalSourcecheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_SIMULATEDPULSE_ON, null);
                }
                else
                {
                    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_SIMULATEDPULSE_OFF, null);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数SingnalSourcecheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数SingnalSourcecheckBox_CheckedChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 测试果杯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestFruitCupcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (GlobalDataInterface.global_IsTestMode)
                {
                    if (checkBox.Checked)
                    {
                        GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_CUP_ON, null);
                    }
                    else
                    {
                        GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_CUP_OFF, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数TestFruitCupcheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数TestFruitCupcheckBox_CheckedChanged出错" + ex);
#endif
            }
        }
        /// <summary>
        /// AD0归零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GAD0Zerobutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalDataInterface.global_IsTestMode)
                {
                    stResetAD ad = new stResetAD(true);
                    ad.value = 0;
                    if (GlobalDataInterface.nVer == 0) //Modify by xcw - 20200619
                    {
                        GlobalDataInterface.TransmitParam(Commonfunction.EncodeChannel(m_WeightSubsysindex, m_WeightSubsysChannelIndex, m_WeightSubsysChannelIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_RESET_AD, ad);
                    }
                    else if (GlobalDataInterface.nVer == 1)
                    {
                        GlobalDataInterface.TransmitParam(Commonfunction.EncodeChannel(m_WeightSubsysindex, Commonfunction.ChannelIndexToIpmIndex(m_WeightSubsysChannelIndex), m_WeightSubsysChannelIndex % ConstPreDefine.CHANNEL_NUM), (int)HC_FSM_COMMAND_TYPE.HC_CMD_RESET_AD, ad);
                    }
                    //GlobalDataInterface.TransmitParam(Commonfunction.EncodeChannel(m_WeightSubsysindex, m_WeightSubsysChannelIndex, m_WeightSubsysChannelIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_RESET_AD, ad);
                }
                m_nStandardAD0 = ushort.Parse(this.GAD0ADtextBox.Text);
                this.GAD0ZeronumericUpDown.Text = "0.000000";  //add by xcw 20200414
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数GAD0Zerobutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数GAD0Zerobutton_Click出错" + ex);
#endif
            }
           // m_nStandardAD1 = m_WeightResult.paras.nStandardAD1;
            //this.GAD0CalibrattextBox.Text = "";
            //this.GAD0WeightnumericUpDown.Text = "0";
           
        }
        
        /// <summary>
        /// AD1归零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GAD1Zerobutton_Click(object sender, EventArgs e)
        {
            try
            {
                //this.GAD1ZeronumericUpDown.Text = "0.100000";
                if (GlobalDataInterface.global_IsTestMode)
                {
                    stResetAD ad = new stResetAD(true);
                    ad.value = 1;
                    if (GlobalDataInterface.nVer == 0) //Modify by xcw - 20200619
                    {
                        GlobalDataInterface.TransmitParam(Commonfunction.EncodeChannel(m_WeightSubsysindex, m_WeightSubsysChannelIndex, m_WeightSubsysChannelIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_RESET_AD, ad);
                    }
                    else if (GlobalDataInterface.nVer == 1)
                    {
                        GlobalDataInterface.TransmitParam(Commonfunction.EncodeChannel(m_WeightSubsysindex, Commonfunction.ChannelIndexToIpmIndex(m_WeightSubsysChannelIndex), m_WeightSubsysChannelIndex % ConstPreDefine.CHANNEL_NUM), (int)HC_FSM_COMMAND_TYPE.HC_CMD_RESET_AD, ad);
                    }
                    //GlobalDataInterface.TransmitParam(Commonfunction.EncodeChannel(m_WeightSubsysindex, m_WeightSubsysChannelIndex, m_WeightSubsysChannelIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_RESET_AD, ad);
                }
                //this.GAD1CalibrattextBox.Text = "";
                //this.GAD1WeightnumericUpDown.Text = "0";
                //this.GAD1ZeronumericUpDown.Text = "0.0";
                m_nStandardAD1 = ushort.Parse(this.GAD1ADtextBox.Text);
                this.GAD1ZeronumericUpDown.Text = "0.000000";  //add by xcw 20200414
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数GAD1Zerobutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数GAD1Zerobutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 通道切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeightChannelcomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;
                if (m_bDataTracking)
                {
                    m_bDataTracking = false;
                    this.TrackDatabutton.Text = m_resourceManager.GetString("TrackDatabutton.Text");
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(m_ChanelIDList[m_WeightChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_DATA_TRACKING_OFF, null);
                    }
                    //this.TrackDatalistView.Items.Clear();
                    m_doubleBufferListView.Items.Clear();
                }
                else
                {
                    m_doubleBufferListView.Items.Clear();
                }

                m_nCount1 = 3;
                GAD0CalibrattextBox.Text = "";
                m_nCount2 = 3;
                GAD1CalibrattextBox.Text = "";

                if (comboBox.SelectedIndex >= 0 && comboBox.SelectedIndex < m_ChanelIDList.Count)
                {
                    int oldSelSysIdx = m_WeightSubsysindex;
                    int selId = m_ChanelIDList[comboBox.SelectedIndex];
                    int nowSelSysIdx = Commonfunction.GetSubsysIndex(selId);
                    if (oldSelSysIdx != nowSelSysIdx)//若切换后与原来通道不属于同一个子系统，需要发送HC_CMD_WEIGHTINFO_OFF命令给上一个子系统
                    {
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            GlobalDataInterface.TransmitParam(m_ChanelIDList[oldSelSysIdx], (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHTINFO_OFF, null);
                        }
                    }
                }
                SetWeightInfo(comboBox.SelectedIndex);
                m_WeightChannelSelectIndex = comboBox.SelectedIndex;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数WeightChannelcomboBox_SelectionChangeCommitted出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数WeightChannelcomboBox_SelectionChangeCommitted出错" + ex);
#endif
            }
        }
      

        /// <summary>
        /// 设置重量界面配置参数
        /// </summary>
        /// <param name="nChannelIdx"></param>
        private void SetWeightInfo(int nChannelIdx)
        {
            try
            {
                if (nChannelIdx >= 0 && nChannelIdx < m_ChanelIDList.Count)
                {
                    int SelId = m_ChanelIDList[nChannelIdx];
                    m_WeightSubsysindex = Commonfunction.GetSubsysIndex(SelId);
                    m_WeightSubsysChannelIndex = Commonfunction.GetChannelIndex(SelId);//Commonfunction.GetIPMIndex(SelId) * ConstPreDefine.CHANNEL_NUM + Commonfunction.GetChannelIndex(SelId);

                    stWeightBaseInfo weight = tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex];

                    this.GAD0ZeronumericUpDown.Text = weight.fGADParam[0].ToString();
                    this.GAD1ZeronumericUpDown.Text = weight.fGADParam[1].ToString();
                    this.TemperatureParamnumericUpDown.Text = weight.fTemperatureParams.ToString();
                    temperatureparams= weight.fTemperatureParams;
                    this.FilterCoeffnumericUpDown.Text = tempGlobalWeightBaseInfo[m_WeightSubsysindex].fFilterParam.ToString();
                    this.MinGradeThresholdnumericUpDown.Text = tempGlobalWeightBaseInfo[m_WeightSubsysindex].nMinGradeThreshold.ToString();
                    this.CupDeviationThresholdnumericUpDown.Text = tempGlobalWeightBaseInfo[m_WeightSubsysindex].nCupDeviationThreshold.ToString();
                    this.CupBreakageThresholdnumericUpDown.Text = tempGlobalWeightBaseInfo[m_WeightSubsysindex].nCupBreakageThreshold.ToString();
                    this.BaseCupNumnumericUpDown.Text = tempGlobalWeightBaseInfo[m_WeightSubsysindex].nBaseCupNum.ToString();
                    this.TotalCupNumnumericUpDown.Text = tempGlobalWeightBaseInfo[m_WeightSubsysindex].nTotalCupNums[m_WeightSubsysindex].ToString();

                    m_WaveInterval[0] = weight.waveinterval[0];
                    m_WaveInterval[1] = weight.waveinterval[1];
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(SelId, (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHTINFO_ON, null);
                    }
                }
                else
                {
                    m_WeightSubsysindex = -1;
                    m_WeightSubsysChannelIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数SetWeightInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数SetWeightInfo出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearDatabutton_Click(object sender, EventArgs e)
        {
           // this.TrackDatalistView.Items.Clear();
            m_doubleBufferListView.Items.Clear();
        }

        /// <summary>
        /// 数据追踪
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackDatabutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_WeightChannelSelectIndex >= 0)
                {
                    m_bDataTracking = !m_bDataTracking;
                    if (m_bDataTracking)
                    {
                        this.TrackDatabutton.Text = m_resourceManager.GetString("StopContinuouslabel.Text");
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            //MessageBox.Show("tracking_on");  //20190827
                            GlobalDataInterface.TransmitParam(m_ChanelIDList[m_WeightChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_DATA_TRACKING_ON, null);
                        }
                    }
                    else
                    {
                        this.TrackDatabutton.Text = m_resourceManager.GetString("TrackDatabutton.Text");
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            //MessageBox.Show("tracking_off"); //20190827
                            GlobalDataInterface.TransmitParam(m_ChanelIDList[m_WeightChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_DATA_TRACKING_OFF, null);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数TrackDatabutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数TrackDatabutton_Click出错" + ex);
#endif
            }
        }

       /// <summary>
       /// AD0标定
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void GAD0Calibratbutton_Click(object sender, EventArgs e)
        {
            try
            {
                int AD = (int)(uint.Parse(this.GAD0ADtextBox.Text) - m_nStandardAD0);
                if (AD == 0)
                    return;
                tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fGADParam[0] = int.Parse(this.GAD0WeightnumericUpDown.Text) * 1.0f / AD;
                this.GAD0ZeronumericUpDown.Text = tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fGADParam[0].ToString("#0.000000");

                GlobalDataInterface.globalOut_WeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fGADParam[0] = tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fGADParam[0];
                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(m_ChanelIDList[m_WeightChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHT_INFO, null);
                }
                //this.GAD0CalibrattextBox.Text = m_WeightResult.fVehicleWeight0.ToString("#0.000000");
                //WeightSaveConfig(false);
                m_nCount1 = 0;
                //this.GAD0CalibrattextBox.Text = "";
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数GAD0Calibratbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数GAD0Calibratbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// AD1标定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GAD1Calibratbutton_Click(object sender, EventArgs e)
        {
            try
            {
                int AD = (int)(uint.Parse(this.GAD1ADtextBox.Text) - m_nStandardAD1);
                if (AD == 0)
                    return;
                tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fGADParam[1] = int.Parse(this.GAD1WeightnumericUpDown.Text) * 1.0f / AD;
                this.GAD1ZeronumericUpDown.Text = tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fGADParam[1].ToString("#0.000000");
                GlobalDataInterface.globalOut_WeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fGADParam[1] = tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fGADParam[1];
                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(m_ChanelIDList[m_WeightChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHT_INFO, null);
                }
                // WeightSaveConfig(false);
                m_nCount2 = 0;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数GAD1Calibratbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数GAD1Calibratbutton_Click出错" + ex);
#endif
            }
           // this.GAD1CalibrattextBox.Text = "";
        }

        /// <summary>
        /// 清零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zerobutton_Click(object sender, EventArgs e)
        {
            try
            {
                //MessageBox.Show("请确认校正系数为1！");
                //MessageBox.Show("0x30001008 Please confirm the corrention coefficient is 1!","Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
                //MessageBox.Show("0x30001008 " + LanguageContainer.WeightSetMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                //    LanguageContainer.WeightSetMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.TestWeight1textBox.Text = "";
                this.TestWeight2textBox.Text = "";
                this.TestWeight3textBox.Text = "";
                this.TestWeight4textBox.Text = "";
                this.TestWeight5textBox.Text = "";
                this.TestWeight6textBox.Text = "";
                this.TestWeight7textBox.Text = "";
                this.TestWeight8textBox.Text = "";
                this.TestWeight9textBox.Text = "";
                this.TestWeight10textBox.Text = "";

                m_nTestTraceWeightCount = 0;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数Zerobutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数Zerobutton_Click出错" + ex);
#endif
            }
        }

        
        /// <summary>
        /// 波形捕捉控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WaveCapturebutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_WeightChannelSelectIndex >= 0)
                {
                    //关闭数据追踪
                    if (m_bDataTracking)
                    {
                        m_bDataTracking = false;
                        this.TrackDatabutton.Text = m_resourceManager.GetString("TrackDatabutton.Text");
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            GlobalDataInterface.TransmitParam(m_ChanelIDList[m_WeightChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_DATA_TRACKING_OFF, null);
                        }
                    }


                }
                GlobalDataInterface.waveForm =  new WaveCaptureForm(m_ChanelIDList[m_WeightChannelSelectIndex], ref m_WaveInterval);
                //  WaveCaptureForm waveForm = new WaveCaptureForm(m_ChanelIDArrayList[m_WeightChannelSelectIndex],ref m_WaveInterval);
                GlobalDataInterface.waveForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数WaveCapturebutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数WaveCapturebutton_Click出错" + ex);
#endif
            }
        }


        /// <summary>
        /// 刷新重量设置界面上行显示数据
        /// </summary>
        /// <param name="sender"></param>
        public void OnUpWeightInfo(stWeightResult weightResult)
        {
            try
            {
                //if (this == Form.ActiveForm)//是否操作当前窗体
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new GlobalDataInterface.WeightInfoEventHandler(OnUpWeightInfo), weightResult);
                    }
                    else
                    {
                        //GlobalDataInterface.WriteErrorInfo("m_WeightSubsysindex = " + m_WeightSubsysindex.ToString() +
                        //    " m_WeightSubsysChannelIndex = " + m_WeightSubsysChannelIndex.ToString() + " weightResult.nChannelId = " +
                        //    weightResult.nChannelId.ToString() + " m_ChanelIDList[m_WeightChannelSelectIndex] = " + m_ChanelIDList[m_WeightChannelSelectIndex].ToString());
                        if (m_WeightSubsysindex > -1 && m_WeightSubsysChannelIndex > -1)
                        {
                            if (weightResult.nChannelId == m_ChanelIDList[m_WeightChannelSelectIndex])
                            {
                                m_WeightResult = weightResult;
                                if (m_nCount1 < 3)
                                {
                                    m_nCount1++;
                                    if (m_nCount1 == 3)
                                    {
                                        this.GAD0CalibrattextBox.Text = m_WeightResult.fVehicleWeight0.ToString("#0.000000");
                                    }
                                }
                                if (m_nCount2 < 3)
                                {
                                    m_nCount2++;
                                    if (m_nCount2 == 3)
                                    {
                                        this.GAD1CalibrattextBox.Text = m_WeightResult.fVehicleWeight1.ToString("#0.000000");
                                    }
                                }

                                if (m_WeightResult.state == 0)
                                {
                                    this.WorkingModeltextBox.Text = m_resourceManager.GetString("CupNormallabel.Text");
                                    this.WorkingModeltextBox.BackColor = Color.Lime;
                                }
                                else
                                {
                                    this.WorkingModeltextBox.Text = m_resourceManager.GetString("CupFaultlabel.Text");
                                    this.WorkingModeltextBox.BackColor = Color.Red;
                                }

                                this.CupAverageWeighttextBox.Text = m_WeightResult.paras.fCupAverageWeight.ToString();
                                this.GAD0ADtextBox.Text = m_WeightResult.paras.nAD0.ToString();
                                this.GAD1ADtextBox.Text = m_WeightResult.paras.nAD1.ToString();

                                string strTrack = m_bDataTracking == true ? "1" : "0";
                                //GlobalDataInterface.WriteErrorInfo("m_bDataTracking=" + strTrack + " m_WeightResult.data.nVehicleI=" + m_WeightResult.data.nVehicleId.ToString() +
                                //    " m_WeightResult.data.nVehicleId=" + m_WeightResult.data.nVehicleId.ToString()); //20190827
                                if (m_bDataTracking /*&& m_WeightResult.data.nVehicleId > 0*/ && m_WeightResult.data.nVehicleId != 0x7f7f7f7f)//modify by xcw 20201021
                                {
                                    //int index = this.TrackDatalistView.Items.Count;
                                    int index = m_doubleBufferListView.Items.Count;
                                     
                                    if (index >= 5000)
                                    {
                                       // this.TrackDatalistView.Clear();
                                        m_doubleBufferListView.Clear();
                                        m_doubleBufferListView.Columns.Add(m_resourceManager.GetString("WagonNumcolumnHeader.Text"));
                                        m_doubleBufferListView.Columns[0].Width = 80;
                                        m_doubleBufferListView.Columns.Add(m_resourceManager.GetString("FruitWeightcolumnHeader.Text"));
                                        m_doubleBufferListView.Columns[1].Width = 110;
                                        m_doubleBufferListView.Columns.Add(m_resourceManager.GetString("WagonWeightcolumnHeader.Text"));
                                        m_doubleBufferListView.Columns[2].Width = 110;
                                        m_doubleBufferListView.Columns.Add("AD0");
                                        m_doubleBufferListView.Columns[3].Width = 110;
                                        m_doubleBufferListView.Columns.Add("AD1");
                                        m_doubleBufferListView.Columns[4].Width = 110;
                                        index = 0;
                                    }

                                    ListViewItem lvi;
                                    lvi = new ListViewItem(string.Format("{0}", m_WeightResult.data.nVehicleId));
                                    lvi.SubItems.Add(m_WeightResult.data.fFruitWeight.ToString("0.00"));
                                    lvi.SubItems.Add(m_WeightResult.data.fVehicleWeight.ToString("0.00"));
                                    lvi.SubItems.Add(string.Format("{0}", m_WeightResult.data.nADVehicle));
                                    lvi.SubItems.Add(string.Format("{0}", m_WeightResult.data.nADFruit));
                                    m_doubleBufferListView.Items.Insert(index, lvi);
                                    m_doubleBufferListView.Items[m_doubleBufferListView.Items.Count - 1].EnsureVisible();
                                    //GlobalDataInterface.WriteErrorInfo("-biao ge load-");  //20190827
                                    //this.TrackDatalistView.Items.Insert(index, lvi);
                                    //this.TrackDatalistView.Items[this.TrackDatalistView.Items.Count - 1].EnsureVisible();
                                    if (m_nTestTraceWeightCount < 10 && Math.Abs(m_WeightResult.data.fFruitWeight - float.Parse(this.CurrentWeightnumericUpDown.Text)) <= int.Parse(this.ThresholdlnumericUpDown.Text))
                                    {
                                        switch (m_nTestTraceWeightCount)
                                        {
                                            case 0:
                                                this.TestWeight1textBox.Text = m_WeightResult.data.fFruitWeight.ToString();
                                                break;
                                            case 1:
                                                this.TestWeight2textBox.Text = m_WeightResult.data.fFruitWeight.ToString();
                                                break;
                                            case 2:
                                                this.TestWeight3textBox.Text = m_WeightResult.data.fFruitWeight.ToString();
                                                break;
                                            case 3:
                                                this.TestWeight4textBox.Text = m_WeightResult.data.fFruitWeight.ToString();
                                                break;
                                            case 4:
                                                this.TestWeight5textBox.Text = m_WeightResult.data.fFruitWeight.ToString();
                                                break;
                                            case 5:
                                                this.TestWeight6textBox.Text = m_WeightResult.data.fFruitWeight.ToString();
                                                break;
                                            case 6:
                                                this.TestWeight7textBox.Text = m_WeightResult.data.fFruitWeight.ToString();
                                                break;
                                            case 7:
                                                this.TestWeight8textBox.Text = m_WeightResult.data.fFruitWeight.ToString();
                                                break;
                                            case 8:
                                                this.TestWeight9textBox.Text = m_WeightResult.data.fFruitWeight.ToString();
                                                break;
                                            case 9:
                                                this.TestWeight10textBox.Text = m_WeightResult.data.fFruitWeight.ToString();
                                                break;
                                            default: break;
                                        }
                                        m_nTestTraceWeightCount++;

                                        if (m_nTestTraceWeightCount == 10)
                                        {
                                            if (this.AverageWeighttextBox.Text == "")
                                                this.AverageWeighttextBox.Text = "0.0";
                                            //if (float.Parse(this.AverageWeighttextBox.Text) != 0.0f)
                                            //{
                                            float fV = (float.Parse(this.TestWeight1textBox.Text) + float.Parse(this.TestWeight2textBox.Text) +
                                                    float.Parse(this.TestWeight3textBox.Text) + float.Parse(this.TestWeight4textBox.Text) + float.Parse(this.TestWeight5textBox.Text) +
                                                    float.Parse(this.TestWeight6textBox.Text) + float.Parse(this.TestWeight7textBox.Text) + float.Parse(this.TestWeight8textBox.Text) +
                                                    float.Parse(this.TestWeight9textBox.Text) + float.Parse(this.TestWeight10textBox.Text)) / 10.0f;
                                            this.AverageWeighttextBox.Text = fV.ToString();
                                            float TemperatureParamnumeric = float.Parse(this.CurrentWeightnumericUpDown.Text) / fV;
                                            float UpDownText = Convert.ToSingle(this.TemperatureParamnumericUpDown.Text);
                                            this.TemperatureParamnumericUpDown.Text = (TemperatureParamnumeric * UpDownText).ToString();//add by xcw 20201207
                                            //this.TemperatureParamnumericUpDown.Text = (float.Parse(this.CurrentWeightnumericUpDown.Text) / fV).ToString();
                                            //}
                                        }
                                    }
                                }
                            }
                        }

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
        /// 重量设置页面被选择
        /// </summary>
        private void WeightSetPageSelected()
        {
            try
            {
                //打开当前通道重量数据上传
                if (m_WeightChannelSelectIndex >= 0)
                {
                    int nDstId = m_ChanelIDList[m_WeightChannelSelectIndex];
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(m_ChanelIDList[m_WeightChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHTINFO_ON, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数WeightSetPageSelected出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数WeightSetPageSelected出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 重量设置页面没有被选择
        /// </summary>
        private void WeightSetPageUnSelected()
        {
            try
            {
                //关闭内信号源
                if (this.SingnalSourcecheckBox.Checked)
                {
                    this.SingnalSourcecheckBox.Checked = false;
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_SIMULATEDPULSE_OFF, null);
                    }
                }
                //关闭测试果杯
                if (this.TestFruitCupcheckBox.Checked)
                {
                    this.TestFruitCupcheckBox.Checked = false;
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_CUP_OFF, null);
                    }
                }
                //关闭当前通道重量数据上传
                if (m_WeightChannelSelectIndex >= 0)
                {
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(m_ChanelIDList[m_WeightChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHTINFO_OFF, null);
                    }
                    //关闭数据追踪
                    if (m_bDataTracking)
                    {
                        m_bDataTracking = false;
                        this.TrackDatabutton.Text = m_resourceManager.GetString("TrackDatabutton.Text");
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            GlobalDataInterface.TransmitParam(m_ChanelIDList[m_WeightChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_DATA_TRACKING_OFF, null);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数WeightSetPageUnSelected出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数WeightSetPageUnSelected出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 存储配置参数
        /// </summary>
        /// <param name="bOnlyWaveInterval"></param>
        /// <returns></returns>
        private bool WeightSaveConfig(bool bOnlyWaveInterval)
        {
            try
            {
                if (m_WeightChannelSelectIndex >= 0)
                {
                    try
                    {
                        //stWeightBaseInfo weight = tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex];

                        if (!bOnlyWaveInterval)
                        {
                            // tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fGADParam[0] = float.Parse(this.GAD0ZeronumericUpDown.Text);
                            // tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fGADParam[1] = float.Parse(this.GAD1ZeronumericUpDown.Text);
                            tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fTemperatureParams = float.Parse(this.TemperatureParamnumericUpDown.Text);

                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].fFilterParam = float.Parse(this.FilterCoeffnumericUpDown.Text);
                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].nMinGradeThreshold = short.Parse(this.MinGradeThresholdnumericUpDown.Text);
                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].nCupDeviationThreshold = short.Parse(this.CupDeviationThresholdnumericUpDown.Text);
                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].nCupBreakageThreshold = short.Parse(this.CupBreakageThresholdnumericUpDown.Text);
                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].nBaseCupNum = short.Parse(this.BaseCupNumnumericUpDown.Text);

                            if (m_WeightSubsysindex == 0)
                            {
                                tempGlobalWeightBaseInfo[0].nTotalCupNums[0] = short.Parse(this.TotalCupNumnumericUpDown.Text);
                                tempGlobalWeightBaseInfo[0].nTotalCupNums[1] = tempGlobalWeightBaseInfo[1].nTotalCupNums[1];
                                tempGlobalWeightBaseInfo[1].nTotalCupNums[0] = short.Parse(this.TotalCupNumnumericUpDown.Text);
                            }
                            else
                            {
                                tempGlobalWeightBaseInfo[1].nTotalCupNums[1] = short.Parse(this.TotalCupNumnumericUpDown.Text);
                                tempGlobalWeightBaseInfo[1].nTotalCupNums[0] = tempGlobalWeightBaseInfo[0].nTotalCupNums[0];
                                tempGlobalWeightBaseInfo[0].nTotalCupNums[1] = short.Parse(this.TotalCupNumnumericUpDown.Text);
                            }
                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].nTotalCupNums[m_WeightSubsysindex] = short.Parse(this.TotalCupNumnumericUpDown.Text);

                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].RefWeight = short.Parse(this.CurrentWeightnumericUpDown.Text);  //Add by ChengSk 20190828
                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].WeightTh = byte.Parse(this.ThresholdlnumericUpDown.Text);       //Add by ChengSk 20190828
                        }
                        tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].waveinterval[0] = m_WaveInterval[0];
                        tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].waveinterval[1] = m_WaveInterval[1];

                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                        {
                            GlobalDataInterface.globalOut_WeightBaseInfo[i].ToCopy(tempWeightBaseInfo[i]);
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                        {
                            GlobalDataInterface.globalOut_GlobalWeightBaseInfo[i].ToCopy(tempGlobalWeightBaseInfo[i]);
                        }
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            GlobalDataInterface.TransmitParam(m_ChanelIDList[m_WeightChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHT_INFO, null);
                            GlobalDataInterface.TransmitParam(Commonfunction.EncodeSubsys(m_WeightSubsysindex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_WEIGHT_INFO, null);
                        }

                        //Commonfunction.SetAppSetting("重量设置-当前重量", this.CurrentWeightnumericUpDown.Text);
                        //Commonfunction.SetAppSetting("重量设置-阈值", this.ThresholdlnumericUpDown.Text);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("重量设置界面保存配置出错:" + ex);
                        //MessageBox.Show("0x10001008 Weight save error:" + ex,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        MessageBox.Show("0x10001008 " + LanguageContainer.WeightSetMessagebox2Text[GlobalDataInterface.selectLanguageIndex] + ex,
                            LanguageContainer.WeightSetMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                //MessageBox.Show("重量设置界面保存配置出错:通道未选择！");
                //MessageBox.Show("0x10001008 Weight save error:Lane is not selected!" , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("0x10001008 " + LanguageContainer.WeightSetMessagebox3Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.WeightSetMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数WeightSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数WeightSaveConfig出错" + ex);
#endif
                return false;
            }
        }

        /// <summary>
        /// 存储配置参数 （另存专用，不给FSM发送指令）    Add by ChengSk - 20190116
        /// </summary>
        /// <param name="bOnlyWaveInterval"></param>
        /// <returns></returns>
        private bool WeightSaveConfig2(bool bOnlyWaveInterval)
        {
            try
            {
                if (m_WeightChannelSelectIndex >= 0)
                {
                    try
                    {
                        //stWeightBaseInfo weight = tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex];

                        if (!bOnlyWaveInterval)
                        {
                            // tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fGADParam[0] = float.Parse(this.GAD0ZeronumericUpDown.Text);
                            // tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fGADParam[1] = float.Parse(this.GAD1ZeronumericUpDown.Text);
                            tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].fTemperatureParams = float.Parse(this.TemperatureParamnumericUpDown.Text);

                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].fFilterParam = float.Parse(this.FilterCoeffnumericUpDown.Text);
                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].nMinGradeThreshold = short.Parse(this.MinGradeThresholdnumericUpDown.Text);
                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].nCupDeviationThreshold = short.Parse(this.CupDeviationThresholdnumericUpDown.Text);
                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].nCupBreakageThreshold = short.Parse(this.CupBreakageThresholdnumericUpDown.Text);
                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].nBaseCupNum = short.Parse(this.BaseCupNumnumericUpDown.Text);

                            if (m_WeightSubsysindex == 0)
                            {
                                tempGlobalWeightBaseInfo[0].nTotalCupNums[0] = short.Parse(this.TotalCupNumnumericUpDown.Text);
                                tempGlobalWeightBaseInfo[0].nTotalCupNums[1] = tempGlobalWeightBaseInfo[1].nTotalCupNums[1];
                                tempGlobalWeightBaseInfo[1].nTotalCupNums[0] = short.Parse(this.TotalCupNumnumericUpDown.Text);
                            }
                            else
                            {
                                tempGlobalWeightBaseInfo[1].nTotalCupNums[1] = short.Parse(this.TotalCupNumnumericUpDown.Text);
                                tempGlobalWeightBaseInfo[1].nTotalCupNums[0] = tempGlobalWeightBaseInfo[0].nTotalCupNums[0];
                                tempGlobalWeightBaseInfo[0].nTotalCupNums[1] = short.Parse(this.TotalCupNumnumericUpDown.Text);
                            }
                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].nTotalCupNums[m_WeightSubsysindex] = short.Parse(this.TotalCupNumnumericUpDown.Text);

                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].RefWeight = short.Parse(this.CurrentWeightnumericUpDown.Text);  //Add by ChengSk 20190828
                            tempGlobalWeightBaseInfo[m_WeightSubsysindex].WeightTh = byte.Parse(this.ThresholdlnumericUpDown.Text);       //Add by ChengSk 20190828
                        }
                        tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].waveinterval[0] = m_WaveInterval[0];
                        tempWeightBaseInfo[m_WeightSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_WeightSubsysChannelIndex].waveinterval[1] = m_WaveInterval[1];

                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                        {
                            GlobalDataInterface.globalOut_WeightBaseInfo[i].ToCopy(tempWeightBaseInfo[i]);
                        }
                        for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                        {
                            GlobalDataInterface.globalOut_GlobalWeightBaseInfo[i].ToCopy(tempGlobalWeightBaseInfo[i]);
                        }
                        //if (GlobalDataInterface.global_IsTestMode)
                        //{
                        //    GlobalDataInterface.TransmitParam(m_ChanelIDList[m_WeightChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHT_INFO, null);
                        //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeSubsys(m_WeightSubsysindex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_WEIGHT_INFO, null);
                        //} //Note by ChengSk - 20190116

                        //Commonfunction.SetAppSetting("重量设置-当前重量", this.CurrentWeightnumericUpDown.Text);
                        //Commonfunction.SetAppSetting("重量设置-阈值", this.ThresholdlnumericUpDown.Text);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("重量设置界面保存配置出错:" + ex);
                        //MessageBox.Show("0x10001008 Weight save error:" + ex,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        MessageBox.Show("0x10001008 " + LanguageContainer.WeightSetMessagebox2Text[GlobalDataInterface.selectLanguageIndex] + ex,
                            LanguageContainer.WeightSetMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                //MessageBox.Show("重量设置界面保存配置出错:通道未选择！");
                //MessageBox.Show("0x10001008 Weight save error:Lane is not selected!" , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("0x10001008 " + LanguageContainer.WeightSetMessagebox3Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.WeightSetMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-WeightSet中函数WeightSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-WeightSet中函数WeightSaveConfig出错" + ex);
#endif
                return false;
            }
        }
        
        //public void SetInterval(byte[] interval)
        //{
        //    m_WaveInterval[0] = interval[0];
        //    m_WaveInterval[1] = interval[1];
        //}
        /// <summary>
        /// 立即生效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeightEffectbutton_Click(object sender, EventArgs e)
        {
            if(!WeightSaveConfig(false))
                return;
            this.WeightEffectbutton.Enabled = false;
            this.EffectButtonDelaytimer4.Enabled = true;
        }

        /// <summary>
        /// 立即生效后延迟1.5秒再启用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffectButtonDelaytimer4_Tick(object sender, EventArgs e)
        {
            this.WeightEffectbutton.Enabled = true;
            this.EffectButtonDelaytimer4.Enabled = false;
        }

    }
}
