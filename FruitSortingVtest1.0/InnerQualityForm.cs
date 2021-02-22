using Common;
using Interface;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Interface.TIpcSystemSpecAbsDataFormat;

namespace FruitSortingVtest1._0
{
    public partial class InnerQualityForm : Form
    {
        MainForm m_mainForm;
        private AsyncTcpSession tcpClient = new AsyncTcpSession();
        private Thread m_SendDataThread;
        private int preComboBoxSbcSelectSelectedIndex = -1;
        private static List<int> m_ChanelIDList = new List<int>();
        private TSYS_DEV_INFORMATION[] temp_SysDevInfoData = new TSYS_DEV_INFORMATION[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM];   //光谱仪信息

        #region 控件事件
        public InnerQualityForm(MainForm mainForm)
        {
            InitializeComponent();
            m_mainForm = mainForm;
            if (m_SendDataThread == null)
            {
                m_SendDataThread = new Thread(SendDataThread);
                m_SendDataThread.Priority = ThreadPriority.Normal;
                m_SendDataThread.IsBackground = true;
                m_SendDataThread.Start();
            }
            //GlobalDataInterface.UpStatisticInfoEvent += new GlobalDataInterface.StatisticInfoEventHandler(OnUpStatisticInfoEvent);
        }
       
        private void InnerQualityForm_Load(object sender, EventArgs e)
        {
            try
            {
                tcpClient.Connected += TcpClient_Connected;
                tcpClient.DataReceived += TcpClient_DataReceived;
                tcpClient.Error += TcpClient_Error;
                GlobalDataInterface.CommportConnectFlag = false;
                this.ShapeConn.BackColor = Color.Silver;
                GlobalDataInterface.DeviceFirstConnectFlag = true;
                GlobalDataInterface.DeviceInitCompletedFlag = false;
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                {
                    temp_SysDevInfoData[i] = new TSYS_DEV_INFORMATION(true);
                    temp_SysDevInfoData[i].ToCopy(GlobalDataInterface.globalOut_SysDevInfoData[i]);
                }
                SystemInformationDisplay(false);
                SystemDataInitial_Processing();
                CheckParameterProcess();
                this.LabelGenMountA.Text = GlobalDataInterface.SystemFruitMountCount.ToString();
                ProgressBarClearProcess();
                MainFormResultDiaplayClear();
                this.ComboBoxSbcSelect.Enabled = true;

                PlotDataDisplayClearProcess();
                ButtonEnableProcess(false);

                this.ComboBoxSbcSelect.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数MainForm_Load出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数MainForm_Load出错:" + ex
);
#endif
            }
        }

        private void InnerQualityForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (GlobalDataInterface.DeviceInitCompletedFlag)
                {


                    if (tcpClient.IsConnected)
                    {
                        //tcpClient.Close();
                        ProgressBarClearProcess();
                        DisconnectCommonRoutine();
                    }

                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainForm中函数MainForm_FormClosing出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数MainForm_Load出错:" + ex
);
#endif
            }
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;
            if (chkBox == null)
            {
                return;
            }
            try
            {
                this.iPlotMainSam.BeginUpdate();
                switch (chkBox.Name)
                {
                    case "CheckBoxDarkR":
                        if (chkBox.Checked)
                        {
                            PlotSamPlotDisplay(0);
                            this.iPlotMainSam.Channels[0].Visible = true;
                            PlotSamPlotDisplay(1);
                            this.iPlotMainSam.Channels[1].Visible = true;
                            PlotSamPlotDisplay(2);
                            this.iPlotMainSam.Channels[2].Visible = true;
                            PlotSamPlotDisplay(3);
                            this.iPlotMainSam.Channels[3].Visible = true;
                        }
                        else
                        {

                            this.iPlotMainSam.Channels[0].Visible = false;
                            this.iPlotMainSam.Channels[0].Clear();
                            this.iPlotMainSam.Channels[1].Visible = false;
                            this.iPlotMainSam.Channels[1].Clear();
                            this.iPlotMainSam.Channels[2].Visible = false;
                            this.iPlotMainSam.Channels[2].Clear();
                            this.iPlotMainSam.Channels[3].Visible = false;
                            this.iPlotMainSam.Channels[3].Clear();
                        }
                        break;
                    case "CheckBoxRef":
                        if (chkBox.Checked)
                        {
                            PlotSamPlotDisplay(4);
                            this.iPlotMainSam.Channels[4].Visible = true;
                        }
                        else
                        {
                            this.iPlotMainSam.Channels[4].Visible = false;
                            this.iPlotMainSam.Channels[4].Clear();
                        }
                        break;
                    case "CheckBoxSample":
                        if (chkBox.Checked)
                        {
                            PlotSamPlotDisplay(5);
                            this.iPlotMainSam.Channels[5].Visible = true;
                        }
                        else
                        {
                            this.iPlotMainSam.Channels[5].Visible = false;
                            this.iPlotMainSam.Channels[5].Clear();
                        }
                        break;
                    default:
                        break;
                }
                this.iPlotMainSam.XAxes[0].Tracking.ZoomToFitAll();
                this.iPlotMainSam.EndUpdate();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("InnerQualityForm中函数{0}出错:" + ex, chkBox.Name));
#if REALEASE
                GlobalDataInterface.WriteErrorInfo(string.Format("InnerQualityForm中函数{0}出错:" + ex, chkBox.Name)
);
#endif              
            }
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalDataInterface.SystemFruitMountCount == 0)
                {
                    return;
                }
                if (MessageBox.Show("删除最后一次扫描数据."
                    + System.Environment.NewLine + " 最新的扫描数据将在此程序中删除."
                    + System.Environment.NewLine + "存储在设备中的数据保持不变."
                    , "警告信息", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
                GlobalDataInterface.SystemFruitMountCount--;
                this.LabelGenMountA.Text = GlobalDataInterface.SystemFruitMountCount.ToString();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ButtonBack_Click出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ButtonBack_Click出错:" + ex
);
#endif
            }
        }

        private void ButtonZeroM_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalDataInterface.SystemFruitMountCount == 0)
                {
                    return;
                }
                if (MessageBox.Show("删除所有扫描数据.     "
                    + System.Environment.NewLine + "所有扫描数据将仅在此管理器程序中删除."
                    + System.Environment.NewLine + "存储在设备中的数据保持不变."
                    , "警告信息", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
                GlobalDataInterface.SystemFruitMountCount = 0;
                this.LabelGenMountA.Text = GlobalDataInterface.SystemFruitMountCount.ToString();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ButtonZeroM_Click出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ButtonZeroM_Click出错:" + ex
);
#endif
            }
        }

        /// <summary>
        ///分选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStateGoAuto_Click(object sender, EventArgs e)
        {
            if (!GlobalDataInterface.CommportConnectFlag)
            {
                return;
            }
            List<SendCommand> list_SendCommand = new List<SendCommand>();
            //停止
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_STATUS_SET,
                MsgData = new byte[1] { 2 },
            });
            //光源开
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_LAMP_SET,
                MsgData = new byte[2] {
                     ConstPreDefine.DEV_CONT_ON,
                     ConstPreDefine.DEV_CONT_NOT
                },
            });
            //基准快门OFF
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_ALARM_SET,
                MsgData = new byte[4] {
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_OFF,
                },
            });
            //遮光板OFF
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_ALARM_SET,
                MsgData = new byte[4] {
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_OFF,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_NOT,
                },
            });
            //分选
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_STATUS_SET,
                MsgData = new byte[1] { 3 },
            });
            CountDownForm countDownForm = new CountDownForm();
            countDownForm.list_SendCommand = list_SendCommand;
            countDownForm.ShowDialog(this);
        }
        /// <summary>
        /// 建模
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStateGoCali_Click(object sender, EventArgs e)
        {
            if (!GlobalDataInterface.CommportConnectFlag)
            {
                return;
            }
            List<SendCommand> list_SendCommand = new List<SendCommand>();
            //停止
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_STATUS_SET,
                MsgData = new byte[1] { 2 },
            });
            //光源开
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_LAMP_SET,
                MsgData = new byte[2] {
                     ConstPreDefine.DEV_CONT_ON,
                     ConstPreDefine.DEV_CONT_NOT
                },
            });
            //基准快门OFF
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_ALARM_SET,
                MsgData = new byte[4] {
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_OFF,
                },
            });
            //遮光板OFF
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_ALARM_SET,
                MsgData = new byte[4] {
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_OFF,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_NOT,
                },
            });
            //建模
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_STATUS_SET,
                MsgData = new byte[1] { 4 },
            });
            CountDownForm countDownForm = new CountDownForm();
            countDownForm.list_SendCommand = list_SendCommand;
            countDownForm.ShowDialog(this);
        }

        /// <summary>
        /// 手动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStateGoManual_Click(object sender, EventArgs e)
        {
            if (!GlobalDataInterface.CommportConnectFlag)
            {
                return;
            }
            List<SendCommand> list_SendCommand = new List<SendCommand>();
            //停止
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_STATUS_SET,
                MsgData = new byte[1] { 2 },
            });
            //手动
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_STATUS_SET,
                MsgData = new byte[1] { 5 },
            });
            CountDownForm countDownForm = new CountDownForm();
            countDownForm.list_SendCommand = list_SendCommand;
            countDownForm.ShowDialog(this);
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonJDXSave_Click(object sender, EventArgs e)
        {
            try
            {
                //if (GlobalDataInterface.SystemFruitMountCount == 0)
                //{
                //    return;
                //}
                SaveFileDialog SaveDialogJdxFile = new SaveFileDialog();
                SaveDialogJdxFile.Filter = "RMJ files (*.rmj)|*.rmj|All files (*.*)|*.*";
                SaveDialogJdxFile.FilterIndex = 0;
                SaveDialogJdxFile.RestoreDirectory = true;
                SaveDialogJdxFile.CreatePrompt = false;
                SaveDialogJdxFile.Title = "JAR File Save (.rmj)";
                if (SaveDialogJdxFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string SaveDialogJDxFileName = SaveDialogJdxFile.FileName;
                    string SaveDialogDirectoryName = System.IO.Path.GetDirectoryName(SaveDialogJDxFileName);
                    string SaveDialogRawFileName = System.IO.Path.Combine(SaveDialogDirectoryName, System.IO.Path.GetFileNameWithoutExtension(SaveDialogJDxFileName) + "." + ConstPreDefine.FILE_NAME_RAWRA);
                    float mathMin, mathMax;
                    string SaveDialogDxFileName;
                    int k;
                    string strData;
                    string SaveDialogExtractJDxFileName = Commonfunction.ExtractFileNames(System.IO.Path.GetFileNameWithoutExtension(SaveDialogJDxFileName));
                    StringBuilder sb = new StringBuilder();
                    if (GlobalDataInterface.SystemFruitMountCount > 1)
                    {
                        sb.AppendLine("##TITLE=" + SaveDialogExtractJDxFileName);
                        sb.AppendLine("##JCAMP-DX=4.24");
                        sb.AppendLine("##DATA TYPE=LINK");
                        sb.AppendLine("##ORIGIN=170808");
                        sb.AppendLine("##OWNER=Reemoon");
                        sb.AppendLine("##BLOCKS=" + GlobalDataInterface.SystemFruitMountCount);
                        for (int i = 0; i < GlobalDataInterface.SystemFruitMountCount; i++)
                        {
                            mathMin = GlobalDataInterface.SpecSavedJDXValueBuffer[i].buf.Min();
                            mathMax = GlobalDataInterface.SpecSavedJDXValueBuffer[i].buf.Max();
                            SaveDialogDxFileName = ConstPreDefine.DXRAW_FILENAME + (i + 1).ToString().PadLeft(4, '0');
                            sb.AppendLine("##TITLE=" + SaveDialogExtractJDxFileName);
                            sb.AppendLine("##JCAMP-DX=4.24");
                            sb.AppendLine("##DATA TYPE=INFRARED SPECTRUM");
                            sb.AppendLine("##ORIGIN=170808");
                            sb.AppendLine("##OWNER=Reemoon");
                            sb.AppendLine("##SAMPLE DESCRIPTION=" + SaveDialogDxFileName);
                            sb.AppendLine("##SAMPLING PROCEDURE=");
                            sb.AppendLine("##DATA PROCESSING=");
                            sb.AppendLine("##XUNITS=NANOMETERS");
                            sb.AppendLine("##YUNITS=ABSORBANCE");
                            sb.AppendLine("##XFACTOR=1.0");
                            sb.AppendLine("##YFACTOR=1.0");
                            sb.AppendLine("##FIRSTX=" + ConstPreDefine.NO_WAVELENGTH_FIRST);
                            sb.AppendLine("##LASTX=" + ConstPreDefine.NO_WAVELENGTH_LAST);
                            sb.AppendLine("##FIRSTY=" + string.Format("{0:0.000000}", GlobalDataInterface.SpecSavedJDXValueBuffer[i].buf[0]));
                            sb.AppendLine("##MINY=" + string.Format("{0:0.000000}", mathMin));
                            sb.AppendLine("##MAXY=" + string.Format("{0:0.000000}", mathMax));
                            sb.AppendLine("##NPOINTS=" + ConstPreDefine.MAX_SPLINE_NO);
                            sb.AppendLine("##XYPOINTS=(XY..XY)");
                            k = 0;
                            for (int j = 0; j < ConstPreDefine.MAX_PIXEL_NO; j++)
                            {
                                strData = "";
                                for (int l = 0; l < 4; l++)
                                {
                                    strData += "  " + string.Format("{0:0}", ConstPreDefine.NO_WAVELENGTH_FIRST + k) +
                                        ", " + string.Format("{0:0.000000E+000}", GlobalDataInterface.SpecSavedJDXValueBuffer[i].buf[k]) + "; ";
                                    k++;
                                    if (k >= ConstPreDefine.MAX_SPLINE_NO)
                                    {
                                        break;
                                    }
                                }
                                sb.AppendLine(strData);
                                if (k >= ConstPreDefine.MAX_SPLINE_NO)
                                {
                                    break;
                                }
                            }
                            sb.AppendLine("##END=");
                        }
                        if (GlobalDataInterface.SystemFruitMountCount > 1)
                        {
                            sb.AppendLine("##END=");
                        }
                    }
                    string strTemp = "";
                    using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(SaveDialogJDxFileName, false, Encoding.Default))
                    {
                        if (GlobalDataInterface.SystemFruitMountCount > 1)
                        {
                            strTemp = sb.ToString();
                            strTemp = DESEncrypt.Encrypt(strTemp);
                            streamWriter.Write(strTemp);
                        }
                    }

                    sb.Clear();
                    strData = "Pixel,WaveL,DarkR,DarkS_S,DarkS_M,DarkS_L,REF";
                    for (int i = 0; i < GlobalDataInterface.SystemFruitMountCount; i++)
                    {
                        strData += ",Sample(" + (i + 1) + ")";
                    }
                    sb.AppendLine(strData);
                    for (int i = 0; i < ConstPreDefine.MAX_PIXEL_NO; i++)
                    {
                        strData = (i + 1) + "," + string.Format("{0:0.00}", GlobalDataInterface.global_SystemWLConvTBL.buf[i]) + "," +
                            GlobalDataInterface.global_SystemRawData[ConstPreDefine.NO_RAW_DARKR].buf[i] + "," +
                            GlobalDataInterface.global_SystemRawData[ConstPreDefine.NO_RAW_DARKS_S].buf[i] + "," +
                            GlobalDataInterface.global_SystemRawData[ConstPreDefine.NO_RAW_DARKS_M].buf[i] + "," +
                            GlobalDataInterface.global_SystemRawData[ConstPreDefine.NO_RAW_DARKS_L].buf[i] + "," +
                            GlobalDataInterface.global_SystemRawData[ConstPreDefine.NO_RAW_REF].buf[i];
                        for (int j = 0; j < GlobalDataInterface.SystemFruitMountCount; j++)
                        {
                            strData += GlobalDataInterface.SpecSavedRAWValueBuffer[j].buf[i];
                        }
                        sb.AppendLine(strData);
                    }
                    using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(SaveDialogRawFileName, false, Encoding.Default))
                    {
                        strTemp = sb.ToString();
                        strTemp = DESEncrypt.Encrypt(strTemp);
                        streamWriter.Write(strTemp);
                    }
                    MessageBox.Show("Save the measured data.     " + System.Environment.NewLine
                        + System.Environment.NewLine + "Saved Data Count : " + GlobalDataInterface.SystemFruitMountCount,
                        "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ButtonJDXSave_Click出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ButtonJDXSave_Click出错:" + ex
);
#endif
            }
        }

        

        private void ButtonCoverDark_Click(object sender, EventArgs e)
        {
            List<SendCommand> list_SendCommand = new List<SendCommand>();
            //光源开
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_LAMP_SET,
                MsgData = new byte[2] {
                     ConstPreDefine.DEV_CONT_ON,
                     ConstPreDefine.DEV_CONT_NOT
                },
            });
            //基准快门ON
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_ALARM_SET,
                MsgData = new byte[4] {
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_ON,
                },
            });
            //遮光板ON
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_ALARM_SET,
                MsgData = new byte[4] {
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_ON,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_NOT,
                },
            });
            //背景状态
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_ALARM_SET,
                MsgData = new byte[4] {
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_ON,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_ON,
                },
            });
            //背景S
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_DARKS_S_START_REQ
            });
            //背景M
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_DARKS_M_START_REQ
            });
            //背景L
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_DARKS_L_START_REQ,
                Interval = 2,
            });
            //背景R
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_DARKR_START_REQ
            });
            //分选状态
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_ALARM_SET,
                MsgData = new byte[4] {
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_OFF,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_OFF,
                },
            });
            CountDownForm countDownForm = new CountDownForm();
            countDownForm.list_SendCommand = list_SendCommand;
            countDownForm.ShowDialog(this);
        }

        private void ButtonCoverRef_Click(object sender, EventArgs e)
        {
            List<SendCommand> list_SendCommand = new List<SendCommand>();
            //光源开
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_LAMP_SET,
                MsgData = new byte[2] {
                     ConstPreDefine.DEV_CONT_ON,
                     ConstPreDefine.DEV_CONT_NOT
                },
            });
            //基准快门ON
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_ALARM_SET,
                MsgData = new byte[4] {
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_ON,
                },
            });
            //遮光板OFF
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_ALARM_SET,
                MsgData = new byte[4] {
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_OFF,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_NOT,
                },
            });
            //基准状态
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_ALARM_SET,
                MsgData = new byte[4] {
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_OFF,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_ON,
                },
            });
            //REF
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_REF_START_REQ
            });
            //分选状态
            list_SendCommand.Add(new SendCommand()
            {
                MsgType = ConstPreDefine.SBC_MCU_ALARM_SET,
                MsgData = new byte[4] {
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_OFF,
                     ConstPreDefine.DEV_CONT_NOT,
                     ConstPreDefine.DEV_CONT_OFF,
                },
            });
            CountDownForm countDownForm = new CountDownForm();
            countDownForm.list_SendCommand = list_SendCommand;
            countDownForm.ShowDialog(this);
        }

        private void ButtonMeasSampleL_Click(object sender, EventArgs e)
        {
            MessageDataSend(ConstPreDefine.SBC_MEAS_L_START_REQ);
        }

        private void ButtonMeasSampleM_Click(object sender, EventArgs e)
        {
            MessageDataSend(ConstPreDefine.SBC_MEAS_M_START_REQ);
        }

        private void ButtonLampOn_Click(object sender, EventArgs e)
        {
            byte[] bytes = new byte[2];
            bytes[0] = ConstPreDefine.DEV_CONT_ON;
            bytes[1] = ConstPreDefine.DEV_CONT_NOT;
            MessageDataSend(ConstPreDefine.SBC_MCU_LAMP_SET, bytes);
        }
       
        private void ButtonLampOff_Click(object sender, EventArgs e)
        {
            byte[] bytes = new byte[2];
            bytes[0] = ConstPreDefine.DEV_CONT_OFF;
            bytes[1] = ConstPreDefine.DEV_CONT_NOT;
            MessageDataSend(ConstPreDefine.SBC_MCU_LAMP_SET, bytes);
        }

        private void ButtonMeasSampleS_Click(object sender, EventArgs e)
        {
            MessageDataSend(ConstPreDefine.SBC_MEAS_S_START_REQ);
        }

        private void ButtonRef_Click(object sender, EventArgs e)
        {
            try
            {
                MessageDataSend(ConstPreDefine.SBC_REF_START_REQ);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ButtonJDXSave_Click出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ButtonJDXSave_Click出错:" + ex
);
#endif
            }
        }
        #endregion

        #region 定时器事件
        private void TimerTx_Tick(object sender, EventArgs e)
        {
            try
            {
                this.TimerTx.Enabled = false;
                this.ShapeTx.BackColor = Color.White;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数TimerTx_Tick出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数TimerTx_Tick出错:" + ex
);
#endif
            }
        }

        private void TimerRx_Tick(object sender, EventArgs e)
        {
            try
            {
                this.TimerRx.Enabled = false;
                this.ShapeRx.BackColor = Color.White;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数TimerRx_Tick出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数TimerRx_Tick出错:" + ex
);
#endif
            }
        }

        private void TimerConnect_Tick(object sender, EventArgs e)
        {
            try
            {
                this.TimerConnect.Enabled = false;
                ProgressBarClearProcess();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数TimerConnect_Tick出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数TimerConnect_Tick出错:" + ex
);
#endif
            }
        }

        private void TimerGetState_Tick(object sender, EventArgs e)
        {
            try
            {
                MessageDataSend(ConstPreDefine.SBC_STATUS_REQ);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数TimerGetState_Tick出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数TimerGetState_Tick出错:" + ex
);
#endif
            }
        }
        #endregion

        #region tcp/ip 连接
        private void TcpClient_Connected(object sender, EventArgs e)
        {
            ConnectGoodCommonRoutine();
        }

        private void TcpClient_DataReceived(object sender, DataEventArgs e)
        {
            //this.BeginInvoke(new MethodInvoker(delegate
            //{
            //    if (e.Length < 1)
            //    {
            //        return;
            //    }
            //    Array.Copy(e.Data, GlobalDataInterface.buffRx, e.Length);
            //    ComSocketRxCommon(e);
            //}));
            if (e.Length < 1)
            {
                return;
            }
            //Array.Copy(e.Data, GlobalDataInterface.buffRx, e.Length);
            ComSocketRxCommon(e);

        }

        private void TcpClient_Error(object sender, ErrorEventArgs e)
        {
            ConnectErrorCommonRoutine();
        }
        private void TcpClient_Closed(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                ProgressBarClearProcess();
                DisconnectCommonRoutine();
            }));
        }
        #endregion

        #region 自定义函数
        private void PlotDataDisplayClearProcess()
        {
            try
            {
                this.iPlotMainSam.BeginUpdate();
                for (int i = 0; i < this.iPlotMainSam.Channels.Count; i++)
                {
                    this.iPlotMainSam.Channels[i].Clear();
                }
                this.iPlotMainSam.XAxes[0].Min = 0;
                this.iPlotMainSam.XAxes[0].Span = ConstPreDefine.NO_WAVELENGTH_LAST - ConstPreDefine.NO_WAVELENGTH_FIRST + 15;
                this.iPlotMainSam.XAxes[0].Tracking.ZoomToFitAll();
                this.iPlotMainSam.EndUpdate();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数PlotDataDisplayClearProcess出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数PlotDataDisplayClearProcess出错:" + ex
);
#endif
            }
        }

        private void PlotSamPlotDisplay(int ChannelLevel)
        {
            try
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    this.iPlotMainSam.BeginUpdate();
                    this.iPlotMainSam.Channels[ChannelLevel].Clear();
                    double dMinimum = GlobalDataInterface.global_SystemWLConvTBL.buf[0];
                    double dMaximum = GlobalDataInterface.global_SystemWLConvTBL.buf[0];
                    for (int i = 1; i < GlobalDataInterface.global_SystemWLConvTBL.buf.Length; i++)
                    {
                        if (GlobalDataInterface.global_SystemWLConvTBL.buf[i] > dMaximum)
                        {
                            dMaximum = GlobalDataInterface.global_SystemWLConvTBL.buf[i];
                        }
                        if (GlobalDataInterface.global_SystemWLConvTBL.buf[i] < dMinimum)
                        {
                            dMinimum = GlobalDataInterface.global_SystemWLConvTBL.buf[i];
                        }
                    }
                    dMaximum = dMaximum - dMinimum;
                    if (this.iPlotMainSam.XAxes[0].Span < dMaximum)
                    {
                        this.iPlotMainSam.XAxes[0].Span = Math.Ceiling(dMaximum / 10.0) * 10.0;
                    }
                    double xData, yData;
                    switch (ChannelLevel)
                    {
                        case ConstPreDefine.NO_RAW_DARKR:
                            if (this.CheckBoxDarkR.Checked)
                            {
                                for (int i = 1; i < GlobalDataInterface.global_SystemWLConvTBL.buf.Length; i++)
                                {
                                    xData = GlobalDataInterface.global_SystemWLConvTBL.buf[i] - dMinimum;
                                    yData = GlobalDataInterface.global_SystemRawData[ChannelLevel].buf[i];
                                    this.iPlotMainSam.Channels[ChannelLevel].AddXY(xData, yData);
                                }
                                //this.iPlotMainSam.Series[ChannelLevel].Points.DataBindXY(GlobalDataInterface.global_SystemWLConvTBL.buf,
                                //                                                    GlobalDataInterface.global_SystemRawData[ChannelLevel].buf);
                            }
                            break;
                        case ConstPreDefine.NO_RAW_DARKS_S:
                            if (this.CheckBoxDarkR.Checked)
                            {
                                for (int i = 1; i < GlobalDataInterface.global_SystemWLConvTBL.buf.Length; i++)
                                {
                                    xData = GlobalDataInterface.global_SystemWLConvTBL.buf[i] - dMinimum;
                                    yData = GlobalDataInterface.global_SystemRawData[ChannelLevel].buf[i];
                                    this.iPlotMainSam.Channels[ChannelLevel].AddXY(xData, yData);
                                }
                                //this.iPlotMainSam.Series[ChannelLevel].Points.DataBindXY(GlobalDataInterface.global_SystemWLConvTBL.buf,
                                //                                                    GlobalDataInterface.global_SystemRawData[ChannelLevel].buf);
                            }
                            break;
                        case ConstPreDefine.NO_RAW_DARKS_M:
                            if (this.CheckBoxDarkR.Checked)
                            {
                                for (int i = 1; i < GlobalDataInterface.global_SystemWLConvTBL.buf.Length; i++)
                                {
                                    xData = GlobalDataInterface.global_SystemWLConvTBL.buf[i] - dMinimum;
                                    yData = GlobalDataInterface.global_SystemRawData[ChannelLevel].buf[i];
                                    this.iPlotMainSam.Channels[ChannelLevel].AddXY(xData, yData);
                                }
                                //this.iPlotMainSam.Series[ChannelLevel].Points.DataBindXY(GlobalDataInterface.global_SystemWLConvTBL.buf,
                                //                                                    GlobalDataInterface.global_SystemRawData[ChannelLevel].buf);
                            }
                            break;
                        case ConstPreDefine.NO_RAW_DARKS_L:
                            if (this.CheckBoxDarkR.Checked)
                            {
                                for (int i = 1; i < GlobalDataInterface.global_SystemWLConvTBL.buf.Length; i++)
                                {
                                    xData = GlobalDataInterface.global_SystemWLConvTBL.buf[i] - dMinimum;
                                    yData = GlobalDataInterface.global_SystemRawData[ChannelLevel].buf[i];
                                    this.iPlotMainSam.Channels[ChannelLevel].AddXY(xData, yData);
                                }
                                //this.iPlotMainSam.Series[ChannelLevel].Points.DataBindXY(GlobalDataInterface.global_SystemWLConvTBL.buf,
                                //                                                    GlobalDataInterface.global_SystemRawData[ChannelLevel].buf);
                            }
                            break;
                        case ConstPreDefine.NO_RAW_REF:
                            if (this.CheckBoxRef.Checked)
                            {
                                for (int i = 1; i < GlobalDataInterface.global_SystemWLConvTBL.buf.Length; i++)
                                {
                                    xData = GlobalDataInterface.global_SystemWLConvTBL.buf[i] - dMinimum;
                                    yData = GlobalDataInterface.global_SystemRawData[ChannelLevel].buf[i];
                                    this.iPlotMainSam.Channels[ChannelLevel].AddXY(xData, yData);
                                }
                                //this.iPlotMainSam.Series[ChannelLevel].Points.DataBindXY(GlobalDataInterface.global_SystemWLConvTBL.buf,
                                //                                                    GlobalDataInterface.global_SystemRawData[ChannelLevel].buf);
                            }
                            break;
                        case ConstPreDefine.NO_RAW_SAMPLE:
                            if (this.CheckBoxSample.Checked)
                            {
                                for (int i = 1; i < GlobalDataInterface.global_SystemWLConvTBL.buf.Length; i++)
                                {
                                    xData = GlobalDataInterface.global_SystemWLConvTBL.buf[i] - dMinimum;
                                    yData = GlobalDataInterface.global_SystemRawData[ChannelLevel].buf[i];
                                    this.iPlotMainSam.Channels[ChannelLevel].AddXY(xData, yData);
                                }
                                //this.iPlotMainSam.Series[ChannelLevel].Points.DataBindXY(GlobalDataInterface.global_SystemWLConvTBL.buf,
                                //                                                    GlobalDataInterface.global_SystemRawData[ChannelLevel].buf);
                            }
                            break;
                    }
                    this.iPlotMainSam.XAxes[0].Tracking.ZoomToFitAll();
                    this.iPlotMainSam.EndUpdate();
                }));

                //this.BeginInvoke(new MethodInvoker(delegate
                //{
                //   Common.FileHelper.logQueue.Enqueue(string.Format("PlotSamPlotDisplay函数{0}开始时间{1}", ChannelLevel, System.DateTime.Now));
                //    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                //    stopwatch.Start();
                //    //this.iPlotMainSam.BeginInit();
                //    float[] temp = GlobalDataInterface.global_SystemWLConvTBL.buf;
                //    Array.Sort(temp);
                //    double dMinimum = Math.Floor(temp[0] / 10.0) * 10.0;
                //    double dMaximum = Math.Ceiling(temp[temp.Length - 1] / 10.0) * 10.0;
                //    if (this.iPlotMainSam.ChartAreas[0].AxisX.Minimum > dMinimum)
                //    {
                //        this.iPlotMainSam.ChartAreas[0].AxisX.Minimum = dMinimum;
                //    }
                //    if (this.iPlotMainSam.ChartAreas[0].AxisX.Maximum < dMaximum)
                //    {
                //        this.iPlotMainSam.ChartAreas[0].AxisX.Maximum = dMaximum;
                //    }
                //    //if (this.iPlotMainSam.ChartAreas[0].AxisX.Minimum == 0 && this.iPlotMainSam.ChartAreas[0].AxisX.Maximum == 0)
                //    //{
                //    //    this.iPlotMainSam.ChartAreas[0].AxisX.Minimum = ConstPreDefine.NO_WAVELENGTH_FIRST - 5;
                //    //    this.iPlotMainSam.ChartAreas[0].AxisX.Maximum = ConstPreDefine.NO_WAVELENGTH_LAST + 10;
                //    //}
                //    this.iPlotMainSam.Series[ChannelLevel].Points.SuspendUpdates();
                //    this.iPlotMainSam.Series[ChannelLevel].Points.Clear();
                //    float XData, YData;
                //    CheckBox checkBox;
                //    if (dicCheckBox.TryGetValue(ChannelLevel, out checkBox))
                //    {
                //        if (checkBox.Checked)
                //        {
                //            this.iPlotMainSam.Series[ChannelLevel].Points.DataBindXY(GlobalDataInterface.global_SystemWLConvTBL.buf,
                //                                                                    GlobalDataInterface.global_SystemRawData[ChannelLevel].buf);
                //            //for (int i = 0; i < GlobalDataInterface.global_SystemWLConvTBL.buf.Length; i++)
                //            //{
                //            //    XData = GlobalDataInterface.global_SystemWLConvTBL.buf[i];
                //            //    YData = GlobalDataInterface.global_SystemRawData[ChannelLevel].buf[i];
                //            //    this.iPlotMainSam.Series[ChannelLevel].Points.AddXY(XData, YData);

                //            //}
                //        }
                //    }
                //    this.iPlotMainSam.Series[ChannelLevel].Points.ResumeUpdates();
                //    this.iPlotMainSam.ChartAreas[0].AxisX.ScaleView.ZoomReset(100);
                //    //this.iPlotMainSam.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                //    //this.iPlotMainSam.EndInit();
                //    stopwatch.Stop();
                //   Common.FileHelper.logQueue.Enqueue(string.Format("PlotSamPlotDisplay函数{0}结束时间{1}；耗时：{2}", ChannelLevel, System.DateTime.Now, stopwatch.Elapsed.TotalMilliseconds));
                //}));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数PlotSamPlotDisplay出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数PlotSamPlotDisplay出错:" + ex
);
#endif
            }
        }

        private void SystemInformationDisplay(bool displayFlag)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        GlobalDataInterface.SystemDisplayProductID = "Undefined";
                        GlobalDataInterface.SystemDisplayProductSerial = "Undefined";
                        GlobalDataInterface.SystemDisplaySpectrometerSerial = "Undefined";
                        if (displayFlag)
                        {
                            GlobalDataInterface.SystemDisplayProductID =
                             Commonfunction.ByteArrayToString(temp_SysDevInfoData[0].ProductID.buf);
                            GlobalDataInterface.SystemDisplayProductSerial =
                                Commonfunction.ByteArrayToString(temp_SysDevInfoData[0].ProductSerial.buf);
                            GlobalDataInterface.SystemDisplaySpectrometerSerial =
                                System.Text.Encoding.Default.GetString(temp_SysDevInfoData[0].SpectraSerial.buf);
                            if (string.IsNullOrWhiteSpace(GlobalDataInterface.SystemDisplayProductID))
                            {
                                GlobalDataInterface.SystemDisplayProductID = "Undefined";
                            }
                            if (string.IsNullOrWhiteSpace(GlobalDataInterface.SystemDisplayProductSerial))
                            {
                                GlobalDataInterface.SystemDisplayProductSerial = "Undefined";
                            }
                            if (string.IsNullOrWhiteSpace(GlobalDataInterface.SystemDisplaySpectrometerSerial))
                            {
                                GlobalDataInterface.SystemDisplaySpectrometerSerial = "Undefined";
                            }
                        }
                        this.EditInfoBuffer.Text = GlobalDataInterface.SystemDisplayProductID;
                        this.EditInfoBuffer.Text = GlobalDataInterface.SystemDisplayProductSerial;
                        this.EditInfoBuffer.Text = GlobalDataInterface.SystemDisplaySpectrometerSerial;
                    }));

                }
                else
                {
                    GlobalDataInterface.SystemDisplayProductID = "Undefined";
                    GlobalDataInterface.SystemDisplayProductSerial = "Undefined";
                    GlobalDataInterface.SystemDisplaySpectrometerSerial = "Undefined";
                    if (displayFlag)
                    {
                        GlobalDataInterface.SystemDisplayProductID =
                             Commonfunction.ByteArrayToString(temp_SysDevInfoData[0].ProductID.buf);
                        GlobalDataInterface.SystemDisplayProductSerial =
                            Commonfunction.ByteArrayToString(temp_SysDevInfoData[0].ProductSerial.buf);
                        GlobalDataInterface.SystemDisplaySpectrometerSerial =
                            System.Text.Encoding.Default.GetString(temp_SysDevInfoData[0].SpectraSerial.buf);
                        if (string.IsNullOrWhiteSpace(GlobalDataInterface.SystemDisplayProductID))
                        {
                            GlobalDataInterface.SystemDisplayProductID = "Undefined";
                        }
                        if (string.IsNullOrWhiteSpace(GlobalDataInterface.SystemDisplayProductSerial))
                        {
                            GlobalDataInterface.SystemDisplayProductSerial = "Undefined";
                        }
                        if (string.IsNullOrWhiteSpace(GlobalDataInterface.SystemDisplaySpectrometerSerial))
                        {
                            GlobalDataInterface.SystemDisplaySpectrometerSerial = "Undefined";
                        }
                    }
                    this.EditInfoBuffer.Text = GlobalDataInterface.SystemDisplayProductID;
                    this.EditInfoBuffer.Text = GlobalDataInterface.SystemDisplayProductSerial;
                    this.EditInfoBuffer.Text = GlobalDataInterface.SystemDisplaySpectrometerSerial;
                }
            }
            catch (Exception ex)
            {

                Trace.WriteLine("InnerQualityForm中函数SystemInformationDisplay出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数SystemInformationDisplay出错:" + ex
);
#endif
            }
        }

        private void Stm32_WavelengthCalculation_Processing()
        {
            try
            {
                float FL;
                for (int i = 0; i < ConstPreDefine.MAX_PIXEL_NO; i++)
                {
                    FL = GlobalDataInterface.global_SystemInfoData.unitDblAr.DblArray[0];
                    for (int j = 1; j <= 5; j++)
                    {
                        FL += Convert.ToSingle(GlobalDataInterface.global_SystemInfoData.unitDblAr.DblArray[j] * Math.Pow(i + 1, j));
                    }
                    GlobalDataInterface.global_SystemWLConvTBL.buf[i] = FL;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数Stm32_WavelengthCalculation_Processing出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数Stm32_WavelengthCalculation_Processing出错:" + ex
);
#endif
            }
        }

        private void SystemDataInitial_Processing()
        {
            try
            {
                for (int i = 0; i < temp_SysDevInfoData[0].unitDblAr.DblArray.Length; i++)
                {
                    temp_SysDevInfoData[0].unitDblAr.DblArray[i] = 0;
                }
                for (int i = 0; i < GlobalDataInterface.global_SystemAmoData.Length; i++)
                {
                    GlobalDataInterface.global_SystemAmoData[i].B0ArrayData = 0;
                    for (int j = 0; j < GlobalDataInterface.global_SystemAmoData[i].BArrayBuffer.buf.Length; j++)
                    {
                        GlobalDataInterface.global_SystemAmoData[i].BArrayBuffer.buf[j] = 0;
                    }
                }
                for (int i = 0; i < GlobalDataInterface.global_SystemWLConvTBL.buf.Length; i++)
                {
                    GlobalDataInterface.global_SystemWLConvTBL.buf[i] = 0;
                }
                for (int i = 0; i < GlobalDataInterface.global_SystemRawData.Length; i++)
                {
                    for (int j = 0; j < GlobalDataInterface.global_SystemRawData[i].buf.Length; j++)
                    {
                        GlobalDataInterface.global_SystemRawData[i].buf[j] = 0;
                    }
                }
                for (int i = 0; i < GlobalDataInterface.global_SystemCalcDxData.buf.Length; i++)
                {
                    GlobalDataInterface.global_SystemCalcDxData.buf[i] = 0;
                }
                for (int i = 0; i < GlobalDataInterface.global_SystemClacResult.abs.buf.Length; i++)
                {
                    GlobalDataInterface.global_SystemClacResult.abs.buf[i] = 0;
                }
                for (int i = 0; i < GlobalDataInterface.global_SystemClacResult.cal.Length; i++)
                {
                    GlobalDataInterface.global_SystemClacResult.cal[i] = 0;
                }
                GlobalDataInterface.DeviceFirstConnectFlag = true;
                GlobalDataInterface.DeviceInitCompletedFlag = false;
                GlobalDataInterface.SystemFruitMountCount = 0;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数SystemDataInitial_Processing出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数SystemDataInitial_Processing:" + ex
);
#endif
            }
        }

        public void CheckParameterProcess()
        {
            try
            {

                if (GlobalDataInterface.globalOut_SysDevParaData.IntgTimeR > 200 || GlobalDataInterface.globalOut_SysDevParaData.IntgTimeR < 5)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.IntgTimeR = ConstPreDefine.VAL_DEF_INTG_TIME;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.IntgTimeSL > 200 || GlobalDataInterface.globalOut_SysDevParaData.IntgTimeSL < 5)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.IntgTimeSL = ConstPreDefine.VAL_DEF_INTG_TIME;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.IntgTimeSM > 200 || GlobalDataInterface.globalOut_SysDevParaData.IntgTimeSM < 5)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.IntgTimeSM = ConstPreDefine.VAL_DEF_INTG_TIME - 5;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.IntgTimeSS > 200 || GlobalDataInterface.globalOut_SysDevParaData.IntgTimeSS < 5)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.IntgTimeSS = ConstPreDefine.VAL_DEF_INTG_TIME - 10;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.CupPitchSize > 500 || GlobalDataInterface.globalOut_SysDevParaData.CupPitchSize < 40)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.CupPitchSize = 95;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.FruitSizeLFlag != 1)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.FruitSizeLFlag = 0;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.FruitSizeSFlag != 1)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.FruitSizeSFlag = 0;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.FruitSizeL > GlobalDataInterface.globalOut_SysDevParaData.CupPitchSize
                    || GlobalDataInterface.globalOut_SysDevParaData.FruitSizeL < ConstPreDefine.VAL_DEF_FRUIT_SIZE_MIN)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.FruitSizeL = 90;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.FruitSizeS > GlobalDataInterface.globalOut_SysDevParaData.CupPitchSize
                    || GlobalDataInterface.globalOut_SysDevParaData.FruitSizeS < ConstPreDefine.VAL_DEF_FRUIT_SIZE_MIN)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.FruitSizeS = 70;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.FruitSizeS >= GlobalDataInterface.globalOut_SysDevParaData.FruitSizeL)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.FruitSizeL = 90;
                    GlobalDataInterface.globalOut_SysDevParaData.FruitSizeS = 70;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.SmoothUsedFlag != 1)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.SmoothUsedFlag = 0;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.SmoothingPoint > 13 || GlobalDataInterface.globalOut_SysDevParaData.SmoothingPoint < 7)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.SmoothingPoint = 9;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.SmoothingPoint % 2 == 0)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.SmoothingPoint = 9;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.FirFilterUsedFlag != 1)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.FirFilterUsedFlag = 0;
                }
                if (Single.IsNaN(GlobalDataInterface.globalOut_SysDevParaData.FirFilterRatio))
                {
                    GlobalDataInterface.globalOut_SysDevParaData.FirFilterRatio = 0.05f;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.FirFilterRatio != 0)
                {
                    if (GlobalDataInterface.globalOut_SysDevParaData.FirFilterRatio > 1 || GlobalDataInterface.globalOut_SysDevParaData.FirFilterRatio < 0.0009f)
                    {
                        GlobalDataInterface.globalOut_SysDevParaData.FirFilterRatio = 0.05f;
                    }
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.DerivUsedFlag != 1)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.DerivUsedFlag = 0;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.DerivOrder > 2 || GlobalDataInterface.globalOut_SysDevParaData.DerivOrder < 1)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.DerivOrder = 1;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.DerivSegSize > 9 || GlobalDataInterface.globalOut_SysDevParaData.DerivSegSize < 3)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.DerivSegSize = 5;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.DerivSegSize % 2 == 0)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.DerivSegSize = 5;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.BaseLUsedFlag != 1)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.BaseLUsedFlag = 0;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.BaseLOffsetPoint > (ConstPreDefine.NO_WAVELENGTH_LAST - 50)
                    || GlobalDataInterface.globalOut_SysDevParaData.BaseLOffsetPoint < (ConstPreDefine.NO_WAVELENGTH_FIRST + 50))
                {
                    GlobalDataInterface.globalOut_SysDevParaData.BaseLOffsetPoint = 820;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.BaseLOffsetPoint % 2 == 1)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.BaseLOffsetPoint = 820;
                }
                if (Single.IsNaN(GlobalDataInterface.globalOut_SysDevParaData.BaseLOffsetMin))
                {
                    GlobalDataInterface.globalOut_SysDevParaData.BaseLOffsetMin = 0;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.BaseLOffsetMin != 0)
                {
                    if (GlobalDataInterface.globalOut_SysDevParaData.BaseLOffsetMin > 3 || GlobalDataInterface.globalOut_SysDevParaData.BaseLOffsetMin < 0.0009f)
                    {
                        GlobalDataInterface.globalOut_SysDevParaData.BaseLOffsetMin = 0;
                    }
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.ScanCountVal > 50 || GlobalDataInterface.globalOut_SysDevParaData.ScanCountVal < 1)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.ScanCountVal = 50;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.Decision950nmMax > 40000 || GlobalDataInterface.globalOut_SysDevParaData.Decision950nmMax < 50)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.Decision950nmMax = 10000;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.ProcessingMethod != 1)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.ProcessingMethod = 0;
                }
                if (GlobalDataInterface.globalOut_SysDevParaData.WarmupTime > 30)
                {
                    GlobalDataInterface.globalOut_SysDevParaData.WarmupTime = ConstPreDefine.VAL_DEF_WARMUP_TIME;
                }
                for (int i = 0; i < GlobalDataInterface.globalOut_SysDevParaData.SlopOffset.Length; i++)
                {
                    if (Single.IsNaN(GlobalDataInterface.globalOut_SysDevParaData.SlopOffset[i].slope))
                    {
                        GlobalDataInterface.globalOut_SysDevParaData.SlopOffset[i].slope = 1;
                    }
                    if (Single.IsNaN(GlobalDataInterface.globalOut_SysDevParaData.SlopOffset[i].offset))
                    {
                        GlobalDataInterface.globalOut_SysDevParaData.SlopOffset[i].offset = 0;
                    }
                    if (GlobalDataInterface.globalOut_SysDevParaData.SlopOffset[i].slope > 2
                        || GlobalDataInterface.globalOut_SysDevParaData.SlopOffset[i].slope < 0
                        || GlobalDataInterface.globalOut_SysDevParaData.SlopOffset[i].slope == 0)
                    {
                        GlobalDataInterface.globalOut_SysDevParaData.SlopOffset[i].slope = 1;
                    }
                    if (GlobalDataInterface.globalOut_SysDevParaData.SlopOffset[i].offset > 20
                        || GlobalDataInterface.globalOut_SysDevParaData.SlopOffset[i].offset < -20)
                    {
                        GlobalDataInterface.globalOut_SysDevParaData.SlopOffset[i].offset = 0;
                    }
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数CheckParameterProcess出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数CheckParameterProcess出错:" + ex
);
#endif
            }
        }

        private void ParameterDisplay()
        {
            try
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    for (int i = 0; i < ConstPreDefine.MAX_PARA_AMO_NO; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                this.CheckBoxCalUsed0.Checked = GlobalDataInterface.global_SystemParaData.CalUsedFlag[i] == 1;
                                this.EditSlope0.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].slope);
                                this.EditOffset0.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].offset);
                                break;
                            case 1:
                                this.CheckBoxCalUsed1.Checked = GlobalDataInterface.global_SystemParaData.CalUsedFlag[i] == 1;
                                this.EditSlope1.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].slope);
                                this.EditOffset1.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].offset);
                                break;
                            case 2:
                                this.CheckBoxCalUsed2.Checked = GlobalDataInterface.global_SystemParaData.CalUsedFlag[i] == 1;
                                this.EditSlope2.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].slope);
                                this.EditOffset2.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].offset);
                                break;
                            case 3:
                                this.CheckBoxCalUsed3.Checked = GlobalDataInterface.global_SystemParaData.CalUsedFlag[i] == 1;
                                this.EditSlope3.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].slope);
                                this.EditOffset3.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].offset);
                                break;
                            case 4:
                                this.CheckBoxCalUsed4.Checked = GlobalDataInterface.global_SystemParaData.CalUsedFlag[i] == 1;
                                this.EditSlope4.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].slope);
                                this.EditOffset4.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].offset);
                                break;
                            case 5:
                                this.CheckBoxCalUsed5.Checked = GlobalDataInterface.global_SystemParaData.CalUsedFlag[i] == 1;
                                this.EditSlope5.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].slope);
                                this.EditOffset5.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].offset);
                                break;
                            case 6:
                                this.CheckBoxCalUsed6.Checked = GlobalDataInterface.global_SystemParaData.CalUsedFlag[i] == 1;
                                this.EditSlope6.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].slope);
                                this.EditOffset6.Text = string.Format("{0:0.0000}", GlobalDataInterface.global_SystemParaData.SlopOffset[i].offset);
                                break;
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ProgressBarClearProcess出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ProgressBarClearProcess出错:" + ex
);
#endif
            }
        }

        private void AmoDisplay(int AmoNoNew)
        {
            try
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    switch (AmoNoNew)
                    {
                        case 0:
                            this.EditSetB0Amo0.Text = string.Format("{0:0.0000000}", GlobalDataInterface.global_SystemAmoData[AmoNoNew].B0ArrayData);
                            break;
                        case 1:
                            this.EditSetB0Amo1.Text = string.Format("{0:0.0000000}", GlobalDataInterface.global_SystemAmoData[AmoNoNew].B0ArrayData);
                            break;
                        case 2:
                            this.EditSetB0Amo2.Text = string.Format("{0:0.0000000}", GlobalDataInterface.global_SystemAmoData[AmoNoNew].B0ArrayData);
                            break;
                        case 3:
                            this.EditSetB0Amo3.Text = string.Format("{0:0.0000000}", GlobalDataInterface.global_SystemAmoData[AmoNoNew].B0ArrayData);
                            break;
                        case 4:
                            this.EditSetB0Amo4.Text = string.Format("{0:0.0000000}", GlobalDataInterface.global_SystemAmoData[AmoNoNew].B0ArrayData);
                            break;
                        case 5:
                            this.EditSetB0Amo5.Text = string.Format("{0:0.0000000}", GlobalDataInterface.global_SystemAmoData[AmoNoNew].B0ArrayData);
                            break;
                        case 6:
                            this.EditSetB0Amo6.Text = string.Format("{0:0.0000000}", GlobalDataInterface.global_SystemAmoData[AmoNoNew].B0ArrayData);
                            break;
                    }
                }));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ProgressBarClearProcess出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ProgressBarClearProcess出错:" + ex
);
#endif
            }
        }

        private void MainFormResultDiaplayClear()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        this.LabelMeasRes0Value.Text = "0.0";
                        this.LabelMeasRes1Value.Text = "0.00";
                        this.LabelMeasRes2Value.Text = "0.0";
                        this.LabelMeasRes3Value.Text = "0.0";
                        this.LabelMeasRes4Value.Text = "0.0";
                        this.LabelMeasRes5Value.Text = "0.0";
                        this.LabelMeasRes6Value.Text = "0.0";
                    }));
                }
                else
                {
                    this.LabelMeasRes0Value.Text = "0.0";
                    this.LabelMeasRes1Value.Text = "0.00";
                    this.LabelMeasRes2Value.Text = "0.0";
                    this.LabelMeasRes3Value.Text = "0.0";
                    this.LabelMeasRes4Value.Text = "0.0";
                    this.LabelMeasRes5Value.Text = "0.0";
                    this.LabelMeasRes6Value.Text = "0.0";
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数MainFormResultDiaplayClea出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数MainFormResultDiaplayClea出错:" + ex
);
#endif
            }
        }

        private void ProgressBarClearProcess()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        this.ProgressBarConnect.Visible = false;
                        this.ProgressBarConnect.Style = ProgressBarStyle.Marquee;
                        this.ProgressBarConnect.Minimum = 0;
                        this.ProgressBarConnect.Maximum = 0;
                        this.PanelControlMenu.Enabled = true;
                        this.PanelWave.Enabled = true;
                    }));
                }
                else
                {
                    this.ProgressBarConnect.Visible = false;
                    this.ProgressBarConnect.Style = ProgressBarStyle.Marquee;
                    this.ProgressBarConnect.Minimum = 0;
                    this.ProgressBarConnect.Maximum = 0;
                    this.PanelControlMenu.Enabled = true;
                    this.PanelWave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ProgressBarClearProcess出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ProgressBarClearProcess出错:" + ex
);
#endif
            }
        }

        private void ProgressBarRunProcess()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        this.ProgressBarConnect.Visible = true;
                        this.ProgressBarConnect.Style = ProgressBarStyle.Marquee;
                        this.ProgressBarConnect.Minimum = 0;
                        this.ProgressBarConnect.Maximum = 100;
                        this.PanelControlMenu.Enabled = false;
                        this.PanelWave.Enabled = false;
                    }));
                }
                else
                {
                    this.ProgressBarConnect.Visible = true;
                    this.ProgressBarConnect.Style = ProgressBarStyle.Marquee;
                    this.ProgressBarConnect.Minimum = 0;
                    this.ProgressBarConnect.Maximum = 100;
                    this.PanelControlMenu.Enabled = false;
                    this.PanelWave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ProgressBarRunProcess出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ProgressBarRunProcess出错:" + ex
);
#endif
            }
        }

        private void MenuEnableProcess(bool enableFlag)
        {
            try
            {
                this.PanelControlMenu.Enabled = enableFlag;
                this.PanelWave.Enabled = enableFlag;
                this.CheckBoxDarkR.Enabled = enableFlag;
                this.CheckBoxRef.Enabled = enableFlag;
                this.CheckBoxSample.Enabled = enableFlag;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数MenuEnableProcess出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数MenuEnableProcess出错:" + ex
);
#endif
            }
        }

        private void ButtonEnableProcess(bool enableFlag)
        {
            try
            {
                MenuEnableProcess(enableFlag);
                Font font = new System.Drawing.Font("宋体", 9F, enableFlag ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular,
                    System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                this.LabelGenMountA.Font = font;
                //this.LabelMeasRes0String.Font = font;
                this.LabelMeasRes0Value.Font = font;
                this.ButtonMeasSampleM.Font = font;
                if (GlobalDataInterface.CommportConnectFlag)
                {
                    SystemDataInitial_Processing();
                    CheckParameterProcess();
                    MainFormResultDiaplayClear();
                    this.LabelGenMountA.Text = GlobalDataInterface.SystemFruitMountCount.ToString();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ButtonEnableProcess出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ButtonEnableProcess出错:" + ex
);
#endif
            }
        }

        private void ComSocketRxCommon(DataEventArgs e)
        {
            try
            {
                //Common.FileHelper.logQueue.Enqueue(string.Format("ComSocketRxCommon函数开始时间{0}", System.DateTime.Now));
                //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                //stopwatch.Start();
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    this.TimerRx.Enabled = true;
                    this.TimerRx.Interval = 100;
                    this.ShapeRx.BackColor = Color.Blue;
                }));

                

                byte[] buff = new byte[e.Length];
                Array.Copy(e.Data, buff, e.Length);
                bool SerialFirstFlag = true;
                int ReceiveBufferPoint = 0;
                int msgLen = 0;
                for (int i = 0; i < e.Length; i++)
                {
                    if (SerialFirstFlag)
                    {
                        if (buff[i] == ConstPreDefine.STX)
                        {
                            SerialFirstFlag = false;
                            ReceiveBufferPoint = 0;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    ReceiveBufferPoint++;
                    if (ReceiveBufferPoint < 4)
                    {
                        continue;
                    }
                    msgLen = (buff[2] & 0xff)
                            + ((buff[3] << 8) & 0xff00);
                    if (msgLen > 0)
                    {
                        if (ReceiveBufferPoint < (msgLen + 4))
                        {
                            continue;
                        }
                    }
                    byte[] ReceiveMessageData = new byte[ConstPreDefine.MAX_BFR_SIZE];
                    Array.Copy(buff, ReceiveMessageData, ReceiveBufferPoint);
                    SerialFirstFlag = true;
                    ReceiveBufferPoint = 0;
                    ReceivedMessageProcess(ReceiveMessageData);
                }
                //stopwatch.Stop();
                //Common.FileHelper.logQueue.Enqueue(string.Format("ComSocketRxCommon函数结束时间{0}，耗时:{1}", System.DateTime.Now, stopwatch.Elapsed.TotalMilliseconds));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ComSocketRxCommon出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ComSocketRxCommon出错:" + ex
);
#endif
            }
        }

        private void ReceivedMessageProcess(byte[] receiveMessageData)
        {
            try
            {
                //Common.FileHelper.logQueue.Enqueue(string.Format("ReceivedMessageProcess函数开始时间{0}", System.DateTime.Now));
                //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                //stopwatch.Start();
                int msgHead = receiveMessageData[0];
                int msgType = receiveMessageData[1];
                if (msgHead != ConstPreDefine.STX)
                {
                    return;
                }
                IntPtr pData = IntPtr.Zero;
                int nLength = 0;
                switch (msgType)
                {
                    case ConstPreDefine.SBC_INFO_REP:
                        nLength = Marshal.SizeOf(typeof(TSYS_DEV_INFORMATION));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            GlobalDataInterface.global_SystemInfoData = (TSYS_DEV_INFORMATION)Marshal.PtrToStructure(pData, typeof(TSYS_DEV_INFORMATION));
                            Marshal.FreeHGlobal(pData);
                        }
                        for (int i = 0; i < GlobalDataInterface.global_SystemInfoData.unitDblAr.DblArray.Length; i++)
                        {
                            if (Single.IsNaN(GlobalDataInterface.global_SystemInfoData.unitDblAr.DblArray[i]))
                            {
                                GlobalDataInterface.global_SystemInfoData.unitDblAr.DblArray[i] = 0;
                            }
                        }
                        SystemInformationDisplay(true);
                        Stm32_WavelengthCalculation_Processing();
                        MainFormResultDiaplayClear();
                        if (GlobalDataInterface.DeviceFirstConnectFlag)
                        {
                            MessageDataSend(ConstPreDefine.SBC_PARA_REQ);
                        }
                        break;
                    case ConstPreDefine.SBC_PARA_REP:
                        nLength = Marshal.SizeOf(typeof(TSYS_DEV_PARAMETER));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            GlobalDataInterface.global_SystemParaData = (TSYS_DEV_PARAMETER)Marshal.PtrToStructure(pData, typeof(TSYS_DEV_PARAMETER));
                            Marshal.FreeHGlobal(pData);
                        }
                        CheckParameterProcess();
                        ParameterDisplay();
                        if (GlobalDataInterface.DeviceFirstConnectFlag)
                        {
                            MessageDataSend(ConstPreDefine.SBC_AMO_REQ);
                        }
                        break;
                    case ConstPreDefine.SBC_AMO_REP:
                    case ConstPreDefine.SBC_AMO_REP + 1:
                    case ConstPreDefine.SBC_AMO_REP + 2:
                    case ConstPreDefine.SBC_AMO_REP + 3:
                    case ConstPreDefine.SBC_AMO_REP + 4:
                    case ConstPreDefine.SBC_AMO_REP + 5:
                    case ConstPreDefine.SBC_AMO_REP + 6:
                        int AmoNoNew = msgType - ConstPreDefine.SBC_AMO_REP;
                        nLength = Marshal.SizeOf(typeof(TSYS_AMO_PARAMETER));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            GlobalDataInterface.global_SystemAmoData[AmoNoNew] = (TSYS_AMO_PARAMETER)Marshal.PtrToStructure(pData, typeof(TSYS_AMO_PARAMETER));
                            Marshal.FreeHGlobal(pData);
                        }
                        if (Single.IsNaN(GlobalDataInterface.global_SystemAmoData[AmoNoNew].B0ArrayData))
                        {
                            GlobalDataInterface.global_SystemAmoData[AmoNoNew].B0ArrayData = 0;
                        }
                        for (int i = 0; i < GlobalDataInterface.global_SystemAmoData[AmoNoNew].BArrayBuffer.buf.Length; i++)
                        {
                            if (Single.IsNaN(GlobalDataInterface.global_SystemAmoData[AmoNoNew].BArrayBuffer.buf[i]))
                            {
                                GlobalDataInterface.global_SystemAmoData[AmoNoNew].BArrayBuffer.buf[i] = 0;
                            }
                        }
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            this.PanelControlMenu.Enabled = true;
                            this.PanelWave.Enabled = true;
                        }));
                        AmoDisplay(AmoNoNew);
                        if (GlobalDataInterface.DeviceFirstConnectFlag)
                        {
                            if (AmoNoNew < ConstPreDefine.MAX_PARA_AMO_NO - 1)
                            {
                                MessageDataSend(Convert.ToByte(ConstPreDefine.SBC_AMO_REQ + AmoNoNew + 1));
                            }
                            else
                            {
                                MessageDataSend(ConstPreDefine.SBC_DARKR_DATA_REQ);
                            }
                        }
                        break;
                    case ConstPreDefine.SBC_DARKR_DATA_REP:
                        nLength = Marshal.SizeOf(typeof(TMsgWord1024Format));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            GlobalDataInterface.global_SystemRawData[ConstPreDefine.NO_RAW_DARKR] = (TMsgWord1024Format)Marshal.PtrToStructure(pData, typeof(TMsgWord1024Format));
                            Marshal.FreeHGlobal(pData);
                        }
                        PlotSamPlotDisplay(ConstPreDefine.NO_RAW_DARKR);
                        if (GlobalDataInterface.DeviceFirstConnectFlag)
                        {
                            MessageDataSend(ConstPreDefine.SBC_DARKS_S_DATA_REQ);
                        }
                        break;
                    case ConstPreDefine.SBC_DARKS_S_DATA_REP:
                        nLength = Marshal.SizeOf(typeof(TMsgWord1024Format));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            GlobalDataInterface.global_SystemRawData[ConstPreDefine.NO_RAW_DARKS_S] = (TMsgWord1024Format)Marshal.PtrToStructure(pData, typeof(TMsgWord1024Format));
                            Marshal.FreeHGlobal(pData);
                        }
                        PlotSamPlotDisplay(ConstPreDefine.NO_RAW_DARKS_S);
                        if (GlobalDataInterface.DeviceFirstConnectFlag)
                        {
                            MessageDataSend(ConstPreDefine.SBC_DARKS_M_DATA_REQ);
                        }
                        break;
                    case ConstPreDefine.SBC_DARKS_M_DATA_REP:
                        nLength = Marshal.SizeOf(typeof(TMsgWord1024Format));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            GlobalDataInterface.global_SystemRawData[ConstPreDefine.NO_RAW_DARKS_M] = (TMsgWord1024Format)Marshal.PtrToStructure(pData, typeof(TMsgWord1024Format));
                            Marshal.FreeHGlobal(pData);
                        }
                        PlotSamPlotDisplay(ConstPreDefine.NO_RAW_DARKS_M);
                        if (GlobalDataInterface.DeviceFirstConnectFlag)
                        {
                            MessageDataSend(ConstPreDefine.SBC_DARKS_L_DATA_REQ);
                        }
                        break;
                    case ConstPreDefine.SBC_DARKS_L_DATA_REP:
                        nLength = Marshal.SizeOf(typeof(TMsgWord1024Format));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            GlobalDataInterface.global_SystemRawData[ConstPreDefine.NO_RAW_DARKS_L] = (TMsgWord1024Format)Marshal.PtrToStructure(pData, typeof(TMsgWord1024Format));
                            Marshal.FreeHGlobal(pData);
                        }
                        PlotSamPlotDisplay(ConstPreDefine.NO_RAW_DARKS_L);
                        if (GlobalDataInterface.DeviceFirstConnectFlag)
                        {
                            MessageDataSend(ConstPreDefine.SBC_REF_DATA_REQ);
                        }
                        break;
                    case ConstPreDefine.SBC_REF_DATA_REP:
                        nLength = Marshal.SizeOf(typeof(TMsgWord1024Format));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            GlobalDataInterface.global_SystemRawData[ConstPreDefine.NO_RAW_REF] = (TMsgWord1024Format)Marshal.PtrToStructure(pData, typeof(TMsgWord1024Format));
                            Marshal.FreeHGlobal(pData);
                        }
                        PlotSamPlotDisplay(ConstPreDefine.NO_RAW_REF);
                        if (GlobalDataInterface.DeviceFirstConnectFlag)
                        {
                            MessageDataSend(ConstPreDefine.SBC_STATUS_REQ);
                        }
                        break;
                    case ConstPreDefine.SBC_MEAS_DATA_REP:
                        nLength = Marshal.SizeOf(typeof(TMsgWord1024Format));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            GlobalDataInterface.global_SystemRawData[ConstPreDefine.NO_RAW_SAMPLE] = (TMsgWord1024Format)Marshal.PtrToStructure(pData, typeof(TMsgWord1024Format));
                            Marshal.FreeHGlobal(pData);
                        }
                        PlotSamPlotDisplay(ConstPreDefine.NO_RAW_SAMPLE);
                        GlobalDataInterface.SpecSavedRAWValueBuffer[GlobalDataInterface.SystemFruitMountCount]
                            = GlobalDataInterface.global_SystemRawData[ConstPreDefine.NO_RAW_SAMPLE];
                        MessageDataSend(ConstPreDefine.SBC_MAKE_DATA_REQ);
                        break;
                    case ConstPreDefine.SBC_MAKE_DATA_REP:
                        nLength = Marshal.SizeOf(typeof(TIpcSystemSpecAbsDataFormat));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            GlobalDataInterface.global_SystemClacResult = (TIpcSystemSpecAbsDataFormat)Marshal.PtrToStructure(pData, typeof(TIpcSystemSpecAbsDataFormat));
                            Marshal.FreeHGlobal(pData);
                        }
                        for (int i = 0; i < GlobalDataInterface.global_SystemClacResult.cal.Length; i++)
                        {
                            if (Single.IsNaN(GlobalDataInterface.global_SystemClacResult.cal[i]))
                            {
                                GlobalDataInterface.global_SystemClacResult.cal[i] = 0;
                            }
                        }
                        for (int i = 0; i < GlobalDataInterface.global_SystemClacResult.abs.buf.Length; i++)
                        {
                            if (Single.IsNaN(GlobalDataInterface.global_SystemClacResult.abs.buf[i]))
                            {
                                GlobalDataInterface.global_SystemClacResult.abs.buf[i] = 0;
                            }
                        }
                        GlobalDataInterface.global_SystemCalcDxData = GlobalDataInterface.global_SystemClacResult.abs;
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            this.LabelCalCounter.Text = GlobalDataInterface.global_SystemClacResult.vCnt.ToString();
                            this.EditFruitSize.Text = GlobalDataInterface.global_SystemClacResult.FruitSize.ToString();
                        }));
                        GlobalDataInterface.SpecSavedJDXValueBuffer[GlobalDataInterface.SystemFruitMountCount] = GlobalDataInterface.global_SystemClacResult.abs;
                        if (GlobalDataInterface.SystemFruitMountCount < 999)
                        {
                            GlobalDataInterface.SystemFruitMountCount++;
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                this.LabelGenMountA.Text = GlobalDataInterface.SystemFruitMountCount.ToString();
                                this.LabelMeasRes0Value.Text = string.Format("{0:0.0}", GlobalDataInterface.global_SystemClacResult.cal[0]);
                                this.LabelMeasRes1Value.Text = string.Format("{0:0.00}", GlobalDataInterface.global_SystemClacResult.cal[1]);
                                this.LabelMeasRes2Value.Text = string.Format("{0:0.0}", GlobalDataInterface.global_SystemClacResult.cal[2]);
                                this.LabelMeasRes3Value.Text = string.Format("{0:0.0}", GlobalDataInterface.global_SystemClacResult.cal[3]);
                                this.LabelMeasRes4Value.Text = string.Format("{0:0.0}", GlobalDataInterface.global_SystemClacResult.cal[4]);
                                this.LabelMeasRes5Value.Text = string.Format("{0:0.0}", GlobalDataInterface.global_SystemClacResult.cal[5]);
                                this.LabelMeasRes6Value.Text = string.Format("{0:0.0}", GlobalDataInterface.global_SystemClacResult.cal[6]);
                            }));
                        }
                        break;
                    case ConstPreDefine.SBC_STATUS_REP:
                        TIpcSystemStatusDataFormat msgDevStatusRep = new TIpcSystemStatusDataFormat(true);
                        nLength = Marshal.SizeOf(typeof(TIpcSystemStatusDataFormat));
                        pData = Marshal.AllocHGlobal(nLength);
                        if (pData != IntPtr.Zero)
                        {
                            Marshal.Copy(receiveMessageData, 4, pData, nLength);
                            msgDevStatusRep = (TIpcSystemStatusDataFormat)Marshal.PtrToStructure(pData, typeof(TIpcSystemStatusDataFormat));
                            Marshal.FreeHGlobal(pData);
                        }
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            switch (msgDevStatusRep.mstate)
                            {
                                case 0:
                                    this.StateState_Label.Text = "开始";
                                    this.StateState_Label.BackColor = Color.Black;
                                    break;
                                case 1:
                                    this.StateState_Label.Text = "执行";
                                    this.StateState_Label.BackColor = Color.LightGray;
                                    break;
                                case 2:
                                    this.StateState_Label.Text = "准备";
                                    this.StateState_Label.BackColor = Color.LightCoral;
                                    break;
                                case 3:
                                    this.StateState_Label.Text = "分选";
                                    this.StateState_Label.BackColor = Color.LightGreen;
                                    this.ManualGroupBox.Visible = false;
                                    break;
                                case 4:
                                    this.StateState_Label.Text = "建模";
                                    this.StateState_Label.BackColor = Color.LightYellow;
                                    this.ManualGroupBox.Visible = false;
                                    break;
                                case 5:
                                    this.StateState_Label.Text = "手动";
                                    this.StateState_Label.BackColor = Color.LightBlue;
                                    this.ManualGroupBox.Visible = true;
                                    break;
                                default:
                                    break;
                            }
                            this.EditStateSpeed.Text = msgDevStatusRep.CupInterval.ToString();
                            if (msgDevStatusRep.mDarkSts == 0)
                            {
                                this.LabelErrStateDark.ForeColor = Color.Blue;
                            }
                            else
                            {
                                this.LabelErrStateDark.ForeColor = Color.Red;
                            }
                            if (msgDevStatusRep.mRefSts == 0)
                            {
                                this.LabelErrStateRef.ForeColor = Color.Blue;
                            }
                            else
                            {
                                this.LabelErrStateRef.ForeColor = Color.Red;
                            }
                        }));
                        if (GlobalDataInterface.DeviceFirstConnectFlag)
                        {
                            GlobalDataInterface.DeviceFirstConnectFlag = false;
                            GlobalDataInterface.DeviceInitCompletedFlag = true;
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                ProgressBarClearProcess();
                                this.TimerGetState.Enabled = true;
                            }));
                        }
                        break;
                    default:
                        break;
                }
                //stopwatch.Stop();
                //Common.FileHelper.logQueue.Enqueue(string.Format("ReceivedMessageProcess函数结束时间{0}，耗时:{1}", System.DateTime.Now, stopwatch.Elapsed.TotalMilliseconds));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ReceivedMessageProcess出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ReceivedMessageProcess出错:" + ex
);
#endif
            }
        }

        private void ConnectGoodCommonRoutine()
        {
            try
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    GlobalDataInterface.CommportConnectFlag = true;
                    this.ShapeConn.BackColor = Color.Lime;
                    ButtonEnableProcess(true);
                    this.ComboBoxSbcSelect.Enabled = true;
                    ConnectCommonRoutine();
                }));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ConnectGoodCommonRoutine出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ConnectGoodCommonRoutine出错:" + ex
);
#endif
            }
        }

        private void ConnectErrorCommonRoutine()
        {
            try
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    GlobalDataInterface.CommportConnectFlag = false;
                    this.ShapeConn.BackColor = Color.Red;
                    this.ComboBoxSbcSelect.Enabled = true;
                    ProgressBarClearProcess();
                    ButtonEnableProcess(false);
                    ConnectCommonRoutine();
                }));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ConnectErrorCommonRoutine出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ConnectErrorCommonRoutine出错:" + ex
);
#endif
            }
        }

        private void ConnectCommonRoutine()
        {
            try
            {
                //GlobalDataInterface.SerialFirstFlag = true;
                this.TimerConnect.Enabled = false;
                SystemInformationDisplay(false);
                Application.DoEvents();
                if (GlobalDataInterface.CommportConnectFlag)
                {
                    MessageDataSend(ConstPreDefine.SBC_INFO_REQ);
                    GlobalDataInterface.DeviceFirstConnectFlag = true;
                    GlobalDataInterface.DeviceInitCompletedFlag = false;
                }
                this.StateState_Label.Text = "分选";
                this.StateState_Label.BackColor = Color.LightYellow;/*SystemColors.Control;*/
                this.EditStateSpeed.Text = "0";
                this.EditFruitSize.Text = "0";
                this.LabelCalCounter.Text = "0";
                this.LabelErrStateDark.ForeColor = Color.Black;
                this.LabelErrStateRef.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ConnectCommonRoutine出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ConnectCommonRoutine出错:" + ex
);
#endif
            }
        }

        private void DisconnectCommonRoutine()
        {
            try
            {
                GlobalDataInterface.CommportConnectFlag = false;
                //this.ShapeConn.BackColor = Color.Silver;
                ButtonEnableProcess(false);
                this.ComboBoxSbcSelect.Enabled = true;
                //GlobalDataInterface.SerialFirstFlag = true;
                PlotDataDisplayClearProcess();
                GlobalDataInterface.CommportConnectFlag = false;

                this.StateState_Label.Text = "";
                this.StateState_Label.BackColor = SystemColors.Control;
                this.EditStateSpeed.Text = "0";
                this.EditFruitSize.Text = "0";
                this.LabelCalCounter.Text = "0";
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数DisconnectCommonRoutine出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数DisconnectCommonRoutine出错:" + ex
);
#endif
            }
        }

        public void MessageDataSend(byte MsgType, byte[] MsgData = null)
        {
            try
            {
                if (!GlobalDataInterface.CommportConnectFlag)
                {
                    return;
                }
                int MsgLength = 0;
                if (MsgData != null)
                {
                    MsgLength = MsgData.Length;
                }
                byte[] buff = new byte[(4 + MsgLength)];
                buff[0] = ConstPreDefine.STX;
                buff[1] = MsgType;
                buff[2] = Convert.ToByte(MsgLength & 0xff);
                buff[3] = Convert.ToByte((MsgLength >> 8) & 0xff);
                if (MsgData != null && MsgLength > 0)
                {
                    Array.Copy(MsgData, 0, buff, 4, MsgLength);
                }
                //tcpClient.Send(buff, 0, buff.Length);
                GlobalDataInterface.SendMsgData.Enqueue(buff);
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        this.TimerTx.Enabled = true;
                        this.TimerTx.Interval = 100;
                        this.ShapeTx.BackColor = Color.Blue;
                    }));
                }
                else
                {
                    this.TimerTx.Enabled = true;
                    this.TimerTx.Interval = 100;
                    this.ShapeTx.BackColor = Color.Blue;
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数MessageDataSend出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数MessageDataSend出错:" + ex
);
#endif
            }
        }

        private void SendDataThread()
        {
            try
            {
                while (true)
                {
                    if (!GlobalDataInterface.CommportConnectFlag)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    if (GlobalDataInterface.SendMsgData.Count == 0)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    byte[] bytes;
                    if (GlobalDataInterface.SendMsgData.TryDequeue(out bytes))
                    {
                        tcpClient.Send(bytes, 0, bytes.Length);
                    }
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数SendDataThread出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数SendDataThread出错:" + ex
);
#endif
            }
        }


        #endregion

        private void ComboBoxSbcSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (preComboBoxSbcSelectSelectedIndex == this.ComboBoxSbcSelect.SelectedIndex)
                {
                    return;
                }
                if (GlobalDataInterface.CommportConnectFlag)
                {
                    if (MessageBox.Show("您确定要关闭当前连接吗？" + System.Environment.NewLine
                    + System.Environment.NewLine + "如果是，请在关闭之前之前保存数据。"
                    , "警告信息", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
                    {
                        this.ComboBoxSbcSelect.SelectedIndex = preComboBoxSbcSelectSelectedIndex;
                        return;
                    }
                }
                ProgressBarClearProcess();
                this.TimerGetState.Enabled = false;
                if (tcpClient.IsConnected)
                {
                    tcpClient.Close();
                    ProgressBarClearProcess();
                    DisconnectCommonRoutine();
                }
                else
                {
                    DisconnectCommonRoutine();
                }
                this.ComboBoxSbcSelect.Enabled = false;
                this.ShapeConn.BackColor = Color.White;
                ProgressBarRunProcess();
                string ip = ConstPreDefine.LC_IP_ADDR_TEMPLATE + (this.ComboBoxSbcSelect.SelectedIndex + 101);
                tcpClient.Connect(new IPEndPoint(IPAddress.Parse(ip), ConstPreDefine.LC_PORT_NUM));
                this.ShapeConn.BackColor = Color.SkyBlue;
                preComboBoxSbcSelectSelectedIndex = this.ComboBoxSbcSelect.SelectedIndex;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ComboBoxSbcSelect_SelectedIndexChanged出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ComboBoxSbcSelect_SelectedIndexChanged出错:" + ex
);
#endif
            }
        }

        private void ButtonAmoRead_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                switch (btn.Name)
                {
                    case "ButtonAmoRead0":
                        ReadAmoCommonProcess(0);
                        break;
                    case "ButtonAmoRead1":
                        ReadAmoCommonProcess(1);
                        break;
                    case "ButtonAmoRead2":
                        ReadAmoCommonProcess(2);
                        break;
                    case "ButtonAmoRead3":
                        ReadAmoCommonProcess(3);
                        break;
                    case "ButtonAmoRead4":
                        ReadAmoCommonProcess(4);
                        break;
                    case "ButtonAmoRead5":
                        ReadAmoCommonProcess(5);
                        break;
                    case "ButtonAmoRead6":
                        ReadAmoCommonProcess(6);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数ButtonAmoRead_Click出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ButtonAmoRead_Click出错:" + ex
);
#endif
            }
        }

        private bool ReadAmoCommonProcess(int AmoNoNew)
        {
            try
            {
                OpenFileDialog OpenDialogAmoFile = new OpenFileDialog();
                OpenDialogAmoFile.Title = GlobalDataInterface.GridAmoTypeNameTAB_New[AmoNoNew] + "打开模型 (.rma)";
                OpenDialogAmoFile.Filter = "RMA Files (*.rma)|*.rma|All Files (*.*)|*.*";
                if (OpenDialogAmoFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string AmoFileName = OpenDialogAmoFile.FileName;
                    if (!System.IO.File.Exists(AmoFileName))
                    {
                        return false;
                    }
                    if (MessageBox.Show("是否更新至Reemoon设备？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    {
                        return false;
                    }
                    string[] arrayTemp;
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(AmoFileName, Encoding.Default))
                    {
                        string strTemp = streamReader.ReadToEnd();
                        strTemp = DESEncrypt.Decrypt(strTemp);
                        arrayTemp = System.Text.RegularExpressions.Regex.Split(strTemp, "\r\n");
                    }
                    if (arrayTemp == null || arrayTemp.Length < 3)
                    {
                        return false;
                    }
                    string getReadLineString = arrayTemp[0];
                    string MessageStr1 = "";
                    Commonfunction.GetCsvField(getReadLineString, 0, ref MessageStr1, '=');
                    if (MessageStr1 != "TYPE")
                    {
                        return false;
                    }
                    getReadLineString = arrayTemp[1];
                    Commonfunction.GetCsvField(getReadLineString, 0, ref MessageStr1, '=');
                    if (MessageStr1 != "VERSION")
                    {
                        return false;
                    }
                    getReadLineString = arrayTemp[2];
                    Commonfunction.GetCsvField(getReadLineString, 0, ref MessageStr1, '=');
                    if (MessageStr1 != "MODELNAME")
                    {
                        return false;
                    }
                    bool xArrFlag = false;
                    bool yArrFlag = false;
                    string MessageStr2 = "";
                    string MessageStr3 = "";
                    string MessageStr4 = "";
                    float FL = 0;
                    int[] data_amo_read_xVar = new int[ConstPreDefine.MAX_PIXEL_NO];
                    float[] data_amo_read_yVar = new float[ConstPreDefine.MAX_PIXEL_NO];
                    int[] data_amo_make_xVar = new int[ConstPreDefine.MAX_SPLINE_NO];
                    float[] data_amo_make_yVar = new float[ConstPreDefine.MAX_SPLINE_NO];
                    for (int i = 0; i < ConstPreDefine.MAX_SPLINE_NO; i++)
                    {
                        data_amo_make_xVar[i] = ConstPreDefine.NO_WAVELENGTH_FIRST + i;
                        data_amo_make_yVar[i] = 0;
                    }
                    int data_amo_read_x_cnt = 0;
                    int data_amo_read_y_cnt = 0;
                    int data_amo_read_org_cnt = 0;
                    int amo_read_save_yPointer = 0;
                    int suggestCnt = 1;
                    float data_amo_read_B0 = 0;
                    for (int i = 0; i < arrayTemp.Length; i++)
                    {
                        getReadLineString = arrayTemp[i].Trim();
                        if (string.IsNullOrWhiteSpace(getReadLineString))
                        {
                            continue;
                        }
                        if (xArrFlag)
                        {
                            MessageStr1 = getReadLineString.Substring(0, 1);
                            if (MessageStr1 == "%")
                            {
                                xArrFlag = false;
                            }
                            else
                            {
                                MessageStr2 = getReadLineString.Replace(" ", "");
                                MessageStr2 = MessageStr2.Replace("\t", "");
                                MessageStr2 = MessageStr2.Replace("\"\"", "\"");
                                for (int j = 1; j <= 20; j++)
                                {
                                    Commonfunction.GetCsvField(MessageStr2, (ushort)j, ref MessageStr3, '\"');
                                    MessageStr3 = MessageStr3.Trim();
                                    if (string.IsNullOrWhiteSpace(MessageStr3))
                                    {
                                        break;
                                    }
                                    float.TryParse(MessageStr3, out FL);
                                    data_amo_read_xVar[data_amo_read_x_cnt] = Convert.ToInt32(Math.Truncate(FL));
                                    if (data_amo_read_x_cnt < ConstPreDefine.MAX_PIXEL_NO - 1)
                                    {
                                        data_amo_read_x_cnt++;
                                    }
                                }
                                continue;
                            }
                        }

                        if (yArrFlag)
                        {
                            MessageStr1 = getReadLineString.Substring(0, 1);
                            if (MessageStr1 == "%")
                            {
                                yArrFlag = false;
                            }
                            else
                            {
                                MessageStr2 = getReadLineString.Replace("  ", " ");
                                MessageStr2 = MessageStr2.Replace("\t", " ");
                                for (int j = 0; j <= 20; j++)
                                {
                                    Commonfunction.GetCsvField(MessageStr2, (ushort)j, ref MessageStr3, ' ');
                                    MessageStr3 = MessageStr3.Trim();
                                    if (string.IsNullOrWhiteSpace(MessageStr3))
                                    {
                                        break;
                                    }
                                    if (amo_read_save_yPointer > 0)
                                    {
                                        amo_read_save_yPointer--;
                                    }
                                    else
                                    {
                                        if (data_amo_read_y_cnt >= data_amo_read_org_cnt)
                                        {
                                            continue;
                                        }
                                        float.TryParse(MessageStr3, out FL);
                                        data_amo_read_yVar[data_amo_read_y_cnt] = FL;
                                        if (data_amo_read_y_cnt < ConstPreDefine.MAX_PIXEL_NO - 1)
                                        {
                                            data_amo_read_y_cnt++;
                                        }
                                    }
                                }
                                continue;
                            }
                        }
                        if (getReadLineString == "%XvarNames")
                        {
                            amo_read_save_yPointer = data_amo_read_org_cnt * (suggestCnt - 1);
                            xArrFlag = true;
                            continue;
                        }
                        Commonfunction.GetCsvField(getReadLineString, 0, ref MessageStr1, '=');
                        if (MessageStr1 == "SUGGESTED")
                        {
                            Commonfunction.GetCsvField(getReadLineString, 1, ref MessageStr2, '=');
                            int.TryParse(MessageStr2, out suggestCnt);
                            continue;
                        }
                        if (MessageStr1 == "XVARS")
                        {
                            Commonfunction.GetCsvField(getReadLineString, 1, ref MessageStr2, '=');
                            int.TryParse(MessageStr2, out data_amo_read_org_cnt);
                            continue;
                        }
                        Commonfunction.GetCsvField(getReadLineString, 0, ref MessageStr1, ' ');
                        if (MessageStr1 == "%B")
                        {
                            MessageStr2 = getReadLineString.Replace(MessageStr1, "").Trim();
                            Commonfunction.GetCsvField(MessageStr2, 0, ref MessageStr3, ' ');
                            MessageStr4 = MessageStr2.Replace(MessageStr3, "").Trim();
                            yArrFlag = true;
                            continue;
                        }
                        if (MessageStr1 == "%B0")
                        {
                            MessageStr2 = getReadLineString.Replace(MessageStr1, "").Trim();
                            Commonfunction.GetCsvField(MessageStr2, 0, ref MessageStr3, ' ');
                            MessageStr4 = MessageStr2.Replace(MessageStr3, "").Trim();
                            if (i + suggestCnt < arrayTemp.Length)
                            {
                                float.TryParse(arrayTemp[i + suggestCnt], out data_amo_read_B0);
                            }
                            else
                            {
                                data_amo_read_B0 = 0;
                            }
                            continue;
                        }
                    }

                    int k = 0;
                    for (int i = 0; i < ConstPreDefine.MAX_PIXEL_NO; i++)
                    {
                        k = data_amo_read_xVar[i];
                        if (k != 0)
                        {
                            for (int j = 0; j < ConstPreDefine.MAX_SPLINE_NO; j++)
                            {
                                if (data_amo_make_xVar[j] == k)
                                {
                                    data_amo_make_yVar[j] = data_amo_read_yVar[i];
                                    break;
                                }
                            }
                        }
                    }
                    GlobalDataInterface.global_SystemAmoData[AmoNoNew].B0ArrayData = data_amo_read_B0;
                    for (int i = 0; i < ConstPreDefine.MAX_SPLINE_NO; i++)
                    {
                        GlobalDataInterface.global_SystemAmoData[AmoNoNew].BArrayBuffer.buf[i] = data_amo_make_yVar[i];
                    }
                    byte[] bytes = Commonfunction.StructToBytes(GlobalDataInterface.global_SystemAmoData[AmoNoNew]);
                    List<SendCommand> list_SendCommand = new List<SendCommand>();
                    list_SendCommand.Add(new SendCommand()
                    {
                        MsgType = Convert.ToByte(ConstPreDefine.SBC_AMO_SET + AmoNoNew),
                        MsgData = bytes
                    });
                    CountDownForm countDownForm = new CountDownForm();
                    countDownForm.list_SendCommand = list_SendCommand;
                    countDownForm.ShowDialog(this);
                    switch (AmoNoNew)
                    {
                        case 0:
                            this.EditSetB0Amo0.Text = string.Format("{0:0.0000000}", data_amo_read_B0);
                            break;
                        case 1:
                            this.EditSetB0Amo1.Text = string.Format("{0:0.0000000}", data_amo_read_B0);
                            break;
                        case 2:
                            this.EditSetB0Amo2.Text = string.Format("{0:0.0000000}", data_amo_read_B0);
                            break;
                        case 3:
                            this.EditSetB0Amo3.Text = string.Format("{0:0.0000000}", data_amo_read_B0);
                            break;
                        case 4:
                            this.EditSetB0Amo4.Text = string.Format("{0:0.0000000}", data_amo_read_B0);
                            break;
                        case 5:
                            this.EditSetB0Amo5.Text = string.Format("{0:0.0000000}", data_amo_read_B0);
                            break;
                        case 6:
                            this.EditSetB0Amo6.Text = string.Format("{0:0.0000000}", data_amo_read_B0);
                            break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("BO生效出错:" + ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Trace.WriteLine("InnerQualityForm中函数ReadAmoCommonProcess出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数ReadAmoCommonProcess出错:" + ex
);
#endif
                return false;
            }

        }

        private void SetAmoBlackDisplay()
        {
            this.EditSlope0.ForeColor = Color.Black;
            this.EditOffset0.ForeColor = Color.Black;
            this.EditSlope1.ForeColor = Color.Black;
            this.EditOffset1.ForeColor = Color.Black;
            this.EditSlope2.ForeColor = Color.Black;
            this.EditOffset2.ForeColor = Color.Black;
            this.EditSlope3.ForeColor = Color.Black;
            this.EditOffset3.ForeColor = Color.Black;
            this.EditSlope4.ForeColor = Color.Black;
            this.EditOffset4.ForeColor = Color.Black;
            this.EditSlope5.ForeColor = Color.Black;
            this.EditOffset5.ForeColor = Color.Black;
            this.EditSlope6.ForeColor = Color.Black;
            this.EditOffset6.ForeColor = Color.Black;
        }

        private void CheckBoxCalUsed_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox chk = sender as CheckBox;
                if (chk == null)
                {
                    return;
                }
                byte temp = Convert.ToByte(chk.Checked ? 1 : 0);
                int index = 0;
                int.TryParse(chk.Name.Substring(chk.Name.Length - 1), out index);
                if (GlobalDataInterface.global_SystemParaData.CalUsedFlag[index] == temp)
                {
                    return;
                }
                if (MessageBox.Show("是否生效?", "询问提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                {
                    chk.Checked = GlobalDataInterface.global_SystemParaData.CalUsedFlag[index] == 1;
                    return;
                }
                GlobalDataInterface.global_SystemParaData.CalUsedFlag[index] = temp;
                List<SendCommand> list_SendCommand = new List<SendCommand>();
                list_SendCommand.Add(new SendCommand()
                {
                    MsgType = ConstPreDefine.SBC_PARA_SET,
                    MsgData = Commonfunction.StructToBytes(GlobalDataInterface.global_SystemParaData),
                });
                CountDownForm countDownForm = new CountDownForm();
                countDownForm.list_SendCommand = list_SendCommand;
                countDownForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("生效出错:" + ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Trace.WriteLine("InnerQualityForm中函数CheckBoxCalUsed_CheckedChanged出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数CheckBoxCalUsed_CheckedChanged出错:" + ex
);
#endif
            }
        }

        private void AmoPanel_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                if (!AmoIsChanged())
                {
                    return;
                }
                SetAmoBlackDisplay();
                if (!AmoParameterCheckProcess())
                {
                    return;
                }
                if (MessageBox.Show("是否生效?", "询问提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                {
                    ParameterDisplay();
                    return;
                }
                GlobalDataInterface.global_SystemParaData.SlopOffset[0].slope = Convert.ToSingle(this.EditSlope0.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[0].offset = Convert.ToSingle(this.EditOffset0.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[1].slope = Convert.ToSingle(this.EditSlope1.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[1].offset = Convert.ToSingle(this.EditOffset1.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[2].slope = Convert.ToSingle(this.EditSlope2.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[2].offset = Convert.ToSingle(this.EditOffset2.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[3].slope = Convert.ToSingle(this.EditSlope3.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[3].offset = Convert.ToSingle(this.EditOffset3.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[4].slope = Convert.ToSingle(this.EditSlope4.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[4].offset = Convert.ToSingle(this.EditOffset4.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[5].slope = Convert.ToSingle(this.EditSlope5.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[5].offset = Convert.ToSingle(this.EditOffset5.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[6].slope = Convert.ToSingle(this.EditSlope6.Text);
                GlobalDataInterface.global_SystemParaData.SlopOffset[6].offset = Convert.ToSingle(this.EditOffset6.Text);
                List<SendCommand> list_SendCommand = new List<SendCommand>();
                list_SendCommand.Add(new SendCommand()
                {
                    MsgType = ConstPreDefine.SBC_PARA_SET,
                    MsgData = Commonfunction.StructToBytes(GlobalDataInterface.global_SystemParaData),
                });
                CountDownForm countDownForm = new CountDownForm();
                countDownForm.list_SendCommand = list_SendCommand;
                countDownForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InnerQualityForm中函数AmoPanel_MouseLeave出错:" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InnerQualityForm中函数AmoPanel_MouseLeave出错:" + ex
);
#endif
            }
        }

        private bool AmoIsChanged()
        {
            float ks = 0;
            float.TryParse(this.EditSlope0.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[0].slope != ks)
            {
                return true;
            }
            float.TryParse(this.EditSlope1.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[1].slope != ks)
            {
                return true;
            }
            float.TryParse(this.EditSlope2.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[2].slope != ks)
            {
                return true;
            }
            float.TryParse(this.EditSlope3.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[3].slope != ks)
            {
                return true;
            }
            float.TryParse(this.EditSlope4.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[4].slope != ks)
            {
                return true;
            }
            float.TryParse(this.EditSlope5.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[5].slope != ks)
            {
                return true;
            }
            float.TryParse(this.EditSlope6.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[6].slope != ks)
            {
                return true;
            }
            float.TryParse(this.EditOffset0.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[0].offset != ks)
            {
                return true;
            }
            float.TryParse(this.EditOffset1.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[1].offset != ks)
            {
                return true;
            }
            float.TryParse(this.EditOffset2.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[2].offset != ks)
            {
                return true;
            }
            float.TryParse(this.EditOffset3.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[3].offset != ks)
            {
                return true;
            }
            float.TryParse(this.EditOffset4.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[4].offset != ks)
            {
                return true;
            }
            float.TryParse(this.EditOffset5.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[5].offset != ks)
            {
                return true;
            }
            float.TryParse(this.EditOffset6.Text.Trim(), out ks);
            if (GlobalDataInterface.global_SystemParaData.SlopOffset[6].offset != ks)
            {
                return true;
            }
            return false;
        }

        private bool AmoParameterCheckProcess()
        {
            float ks = 0;
            bool flagconv = float.TryParse(this.EditSlope0.Text.Trim(), out ks);
            if (!flagconv || (ks > 2) || (ks < 0) || (ks == 0))
            {
                this.EditSlope0.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditSlope0.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditOffset0.Text.Trim(), out ks);
            if (!flagconv || (ks > 20) || (ks < -20))
            {
                this.EditOffset0.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditOffset0.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditSlope1.Text.Trim(), out ks);
            if (!flagconv || (ks > 2) || (ks < 0) || (ks == 0))
            {
                this.EditSlope1.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditSlope1.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditOffset1.Text.Trim(), out ks);
            if (!flagconv || (ks > 20) || (ks < -20))
            {
                this.EditOffset1.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditOffset1.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditSlope2.Text.Trim(), out ks);
            if (!flagconv || (ks > 2) || (ks < 0) || (ks == 0))
            {
                this.EditSlope2.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditSlope2.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditOffset2.Text.Trim(), out ks);
            if (!flagconv || (ks > 20) || (ks < -20))
            {
                this.EditOffset2.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditOffset2.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditSlope3.Text.Trim(), out ks);
            if (!flagconv || (ks > 2) || (ks < 0) || (ks == 0))
            {
                this.EditSlope3.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditSlope3.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditOffset3.Text.Trim(), out ks);
            if (!flagconv || (ks > 20) || (ks < -20))
            {
                this.EditOffset3.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditOffset3.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditSlope4.Text.Trim(), out ks);
            if (!flagconv || (ks > 2) || (ks < 0) || (ks == 0))
            {
                this.EditSlope4.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditSlope4.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditOffset4.Text.Trim(), out ks);
            if (!flagconv || (ks > 20) || (ks < -20))
            {
                this.EditOffset4.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditOffset4.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditSlope5.Text.Trim(), out ks);
            if (!flagconv || (ks > 2) || (ks < 0) || (ks == 0))
            {
                this.EditSlope5.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditSlope5.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditOffset5.Text.Trim(), out ks);
            if (!flagconv || (ks > 20) || (ks < -20))
            {
                this.EditOffset5.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditOffset5.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditSlope6.Text.Trim(), out ks);
            if (!flagconv || (ks > 2) || (ks < 0) || (ks == 0))
            {
                this.EditSlope6.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditSlope6.ForeColor = Color.Black;
            }
            flagconv = float.TryParse(this.EditOffset6.Text.Trim(), out ks);
            if (!flagconv || (ks > 20) || (ks < -20))
            {
                this.EditOffset6.ForeColor = Color.Red;
                return false;
            }
            else
            {
                this.EditOffset6.ForeColor = Color.Black;
            }
            return true;
        }

        private void Closedbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
    }
}
