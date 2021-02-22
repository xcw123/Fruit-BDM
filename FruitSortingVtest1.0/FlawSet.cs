using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using Interface;
using Common;
using ListViewEx;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FruitSortingVtest1._0
{
    public partial class ProjectSetForm : Form
    {
        private List<int> m_IPMIDList = new List<int>();
        private Control[] FlawLevelThresEditors;
        //private InnerQualityForm innerQualityForm;

        //public ProjectSetForm(InnerQualityForm innerQualityForm)
        //{
        //    this.innerQualityForm = innerQualityForm;
        //}

        //private stSpotDetectThresh m_stSpotDetectThresh = new stSpotDetectThresh(true);
        //private int m_FlawCurrentIPMIndex = 0;
        //private int m_FlawSysIndex = -1;
        //private int m_FlawIPMInSysIndex = -1;


        private void FlawSetIntial()
        {
//            try
//            {
//                this.FlawIPMSeleccomboBox.Items.Clear();
//                int IDnum = 1;
//                FlawLevelThresEditors = new Control[] { null, LevelThreshnumericUpDown };
//                if (m_ChanelIDList.Count > 0)
//                {
//                    for (int i = 0; i < m_ChanelIDList.Count; i++)
//                    {

//                        int Id = m_ChanelIDList[i];
//                        if (Commonfunction.GetChannelIndex(Id) == 0)
//                        {
//                            Id = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(Id), Commonfunction.GetIPMIndex(Id));
//                            this.FlawIPMSeleccomboBox.Items.Add(string.Format("IPM {0}", IDnum));
//                            m_IPMIDList.Add(Id);
//                            IDnum++;
//                            if (m_IPMIDList.Count > 0)
//                            {
//                                if (m_IPMIDList.Contains(Id))
//                                    continue;
//                            }
//                        }


//                    }
//                    if (this.FlawIPMSeleccomboBox.Items.Count > 0)
//                    {
//                        this.FlawIPMSeleccomboBox.SelectedIndex = m_FlawCurrentIPMIndex;
//                        m_FlawSysIndex = Commonfunction.GetSubsysIndex(m_IPMIDList[m_FlawCurrentIPMIndex]);
//                        m_FlawIPMInSysIndex = Commonfunction.GetIPMIndex(m_IPMIDList[m_FlawCurrentIPMIndex]);
//                        SetSpotDetectThreshInfo(m_FlawCurrentIPMIndex);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Trace.WriteLine("ProjectSetForm-FlawSet中函数Cancelbutton_Click出错" + ex);
//#if REALEASE
//                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-FlawSet中函数Cancelbutton_Click出错" + ex);
//#endif
//            }
        }

        /// <summary>
        /// IPM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlawIPMSeleccomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
//            try
//            {
//                ComboBox comboBox = (ComboBox)sender;

//                m_FlawCurrentIPMIndex = comboBox.SelectedIndex;
//                m_FlawSysIndex = Commonfunction.GetSubsysIndex(m_IPMIDList[m_FlawCurrentIPMIndex]);
//                m_FlawIPMInSysIndex = Commonfunction.GetIPMIndex(m_IPMIDList[m_FlawCurrentIPMIndex]);
//                SetSpotDetectThreshInfo(comboBox.SelectedIndex);
//            }
//            catch (Exception ex)
//            {
//                Trace.WriteLine("ProjectSetForm-FlawSet中函数FlawIPMSeleccomboBox_SelectionChangeCommitted出错" + ex);
//#if REALEASE
//                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-FlawSet中函数FlawIPMSeleccomboBox_SelectionChangeCommitted出错" + ex);
//#endif
//            }
        }

        /// <summary>
        /// 设置瑕疵界面配置参数
        /// </summary>
        /// <param name="IPMIndex"></param>
        private void SetSpotDetectThreshInfo(int IPMIndex)
        {
//            try
//            {
//                if (IPMIndex >= 0 && IPMIndex < m_IPMIDList.Count)
//                {
//                    //m_stSpotDetectThresh.ToCopy(GlobalDataInterface.globalOut_SpotDetectThresh[m_FlawSysIndex * ConstPreDefine.MAX_IPM_NUM + m_FlawIPMInSysIndex]);
//                    this.FlawAreaMinnumericUpDown.Text = m_stSpotDetectThresh.nSpotAreaMin.ToString();
//                    this.FlawAreaMaxnumericUpDown.Text = m_stSpotDetectThresh.nSpotAreaMax.ToString();
//                    this.SpotGreyCenterGapnumericUpDown.Text = m_stSpotDetectThresh.nSpotGrayCenterGap.ToString();
//                    this.SpotBlueCenterGapnumericUpDown.Text = m_stSpotDetectThresh.nSpotBlueCenterGap.ToString();
//                    this.LightAreaToZeroThresholdnumericUpDown.Text = m_stSpotDetectThresh.nLightAreaToZeroThresh.ToString();
//                    this.MakeBlueBinThreshnumericUpDown.Text = m_stSpotDetectThresh.nMakeBlueBinThresh.ToString();
//                    this.ErodeTimesForEfctnumericUpDown.Text = m_stSpotDetectThresh.nErodeTimesForEfctOutline.ToString();
//                    this.LayerBumForEdgeSpotnumericUpDown.Text = m_stSpotDetectThresh.nLayerNumForEdgeSpot.ToString();
//                    this.YUVBinDilateTimesnumericUpDown.Text = m_stSpotDetectThresh.nYuvBinDilateTimes.ToString();
//                    this.BlueBinDiilateTimesnumericUpDown.Text = m_stSpotDetectThresh.nBlueBinDilateTimes.ToString();
//                    this.OutlineEfectPointRationumericUpDown.Text = m_stSpotDetectThresh.fOutlineEfectPointRatio.ToString();
//                    this.YUVBinErodeTimesnumericUpDown.Text = m_stSpotDetectThresh.nYuvBinErodeTimes.ToString();

//                    this.HthresholdGnumericUpDown.Text = m_stSpotDetectThresh.nH_threshold_G.ToString();
//                    this.GreenFruitAreaThreshnumericUpDown.Text = m_stSpotDetectThresh.nGreenFruitAreaThresh.ToString();
//                    ////灰度阈值
//                    //this.GreyThreshnumericUpDown1.Text = m_stSpotDetectThresh.stHuaPiGuoThresh.nGrayThresh[0].ToString();
//                    //this.GreyThreshnumericUpDown2.Text = m_stSpotDetectThresh.stHuaPiGuoThresh.nGrayThresh[1].ToString();
//                    //this.GreyThreshnumericUpDown3.Text = m_stSpotDetectThresh.stHuaPiGuoThresh.nGrayThresh[2].ToString();
//                    //this.GreyThreshnumericUpDown4.Text = m_stSpotDetectThresh.stHuaPiGuoThresh.nGrayThresh[3].ToString();

//                    ////比例阈值
//                    //this.AreaRatioThreshnumericUpDown1.Text = m_stSpotDetectThresh.stHuaPiGuoThresh.fAreaRatioThresh[0].ToString();
//                    //this.AreaRatioThreshnumericUpDown2.Text = m_stSpotDetectThresh.stHuaPiGuoThresh.fAreaRatioThresh[1].ToString();
//                    //this.AreaRatioThreshnumericUpDown3.Text = m_stSpotDetectThresh.stHuaPiGuoThresh.fAreaRatioThresh[2].ToString();
//                    //this.AreaRatioThreshnumericUpDown4.Text = m_stSpotDetectThresh.stHuaPiGuoThresh.fAreaRatioThresh[3].ToString();

//                    ////匹配参数
//                    //this.VerOffsetnumericUpDown.Text = m_stSpotDetectThresh.nVertiOffsetRange.ToString();
//                    //this.AngleOffsetnumericUpDown.Text = m_stSpotDetectThresh.nAngleOffsetRange.ToString();
//                    //this.AreaOffsetnumericUpDown.Text = m_stSpotDetectThresh.nAreaOffsetRange.ToString();

//                    //层
//                    this.LevelNumcomboBox.SelectedIndex = m_stSpotDetectThresh.stLevelFeature.nLevelNum;
//                    this.ThresholdlistViewEx.Items.Clear();
//                    ListViewItem lvi;
//                    for (int i = 0; i < m_stSpotDetectThresh.stLevelFeature.nLevelNum; i++)
//                    {
//                        lvi = new ListViewItem(string.Format("层{0}", i + 1));
//                        lvi.SubItems.Add(string.Format("{0}", m_stSpotDetectThresh.stLevelFeature.fThreshold[i]));
//                        this.ThresholdlistViewEx.Items.Add(lvi);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Trace.WriteLine("ProjectSetForm-FlawSet中函数SetSpotDetectThreshInfo出错" + ex);
//#if REALEASE
//                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-FlawSet中函数SetSpotDetectThreshInfo出错" + ex);
//#endif
//            }
        }

        /// <summary>
        /// 层阈值List中SubItemClicked事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThresholdlistViewEx_SubItemClicked(object sender, SubItemEventArgs e)
        {
            if (e.SubItem>0)
                this.ThresholdlistViewEx.StartEditing(FlawLevelThresEditors[e.SubItem], e.Item, e.SubItem);
        }

        /// <summary>
        /// 层阈值List中SubItemEndEditing事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThresholdlistViewEx_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        {
            //ListViewEx.ListViewEx listviewex = (ListViewEx.ListViewEx)sender;
            //if (e.SubItem == 1)
            //    m_stSpotDetectThresh.stLevelFeature.fThreshold[e.Item.Index] = float.Parse(e.DisplayText);
        }

        /// <summary>
        /// 层数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelNumcomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
//            try
//            {
//                ComboBox comboBox = (ComboBox)sender;
//                m_stSpotDetectThresh.stLevelFeature.nLevelNum = comboBox.SelectedIndex;
//                this.ThresholdlistViewEx.Items.Clear();
//                ListViewItem lvi;
//                for (int i = 0; i < m_stSpotDetectThresh.stLevelFeature.nLevelNum; i++)
//                {
//                    lvi = new ListViewItem(string.Format("层{0}", i + 1));
//                    string str = m_stSpotDetectThresh.stLevelFeature.fThreshold[i].ToString();
//                    if (m_stSpotDetectThresh.stLevelFeature.fThreshold[i] == 0.0f)
//                    {
//                        lvi.SubItems.Add("0.00");
//                    }
//                    else
//                        lvi.SubItems.Add(string.Format("{0}", m_stSpotDetectThresh.stLevelFeature.fThreshold[i]));
//                    this.ThresholdlistViewEx.Items.Add(lvi);
//                }
//            }
//            catch (Exception ex)
//            {
//                Trace.WriteLine("ProjectSetForm-FlawSet中函数LevelNumcomboBox_SelectionChangeCommitted出错" + ex);
//#if REALEASE
//                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-FlawSet中函数LevelNumcomboBox_SelectionChangeCommitted出错" + ex);
//#endif
//            }
        }

        /// <summary>
        /// 瑕疵设置界面保存配置
        /// </summary>
        /// <returns></returns>
        private bool FlawSetSaveConfig()
        {
            try
            {
                //m_stSpotDetectThresh.nSpotAreaMin = int.Parse(this.FlawAreaMinnumericUpDown.Text);
                //m_stSpotDetectThresh.nSpotAreaMax = int.Parse(this.FlawAreaMaxnumericUpDown.Text);
                //m_stSpotDetectThresh.nSpotGrayCenterGap = int.Parse(this.SpotGreyCenterGapnumericUpDown.Text);
                //m_stSpotDetectThresh.nSpotBlueCenterGap = int.Parse(this.SpotBlueCenterGapnumericUpDown.Text);
                //m_stSpotDetectThresh.nLightAreaToZeroThresh = int.Parse(this.LightAreaToZeroThresholdnumericUpDown.Text);
                //m_stSpotDetectThresh.nMakeBlueBinThresh = int.Parse(this.MakeBlueBinThreshnumericUpDown.Text);
                //m_stSpotDetectThresh.nErodeTimesForEfctOutline = int.Parse(this.ErodeTimesForEfctnumericUpDown.Text);
                //m_stSpotDetectThresh.nLayerNumForEdgeSpot = int.Parse(this.LayerBumForEdgeSpotnumericUpDown.Text);
                //m_stSpotDetectThresh.nYuvBinDilateTimes = int.Parse(this.YUVBinDilateTimesnumericUpDown.Text);
                //m_stSpotDetectThresh.nBlueBinDilateTimes = int.Parse(this.BlueBinDiilateTimesnumericUpDown.Text);
                //m_stSpotDetectThresh.fOutlineEfectPointRatio = float.Parse(this.OutlineEfectPointRationumericUpDown.Text);
                //m_stSpotDetectThresh.nYuvBinErodeTimes = int.Parse(this.YUVBinErodeTimesnumericUpDown.Text);
                //m_stSpotDetectThresh.nH_threshold_G = int.Parse(this.HthresholdGnumericUpDown.Text);
                //m_stSpotDetectThresh.nGreenFruitAreaThresh = int.Parse(this.GreenFruitAreaThreshnumericUpDown.Text);
                //////灰度阈值
                ////m_stSpotDetectThresh.stHuaPiGuoThresh.nGrayThresh[0] = int.Parse(this.GreyThreshnumericUpDown1.Text);
                ////m_stSpotDetectThresh.stHuaPiGuoThresh.nGrayThresh[1] = int.Parse(this.GreyThreshnumericUpDown2.Text);
                ////m_stSpotDetectThresh.stHuaPiGuoThresh.nGrayThresh[2] = int.Parse(this.GreyThreshnumericUpDown3.Text);
                ////m_stSpotDetectThresh.stHuaPiGuoThresh.nGrayThresh[3] = int.Parse(this.GreyThreshnumericUpDown4.Text);

                //////比例阈值
                ////m_stSpotDetectThresh.stHuaPiGuoThresh.fAreaRatioThresh[0] = float.Parse(this.AreaRatioThreshnumericUpDown1.Text);
                ////m_stSpotDetectThresh.stHuaPiGuoThresh.fAreaRatioThresh[1] = float.Parse(this.AreaRatioThreshnumericUpDown2.Text);
                ////m_stSpotDetectThresh.stHuaPiGuoThresh.fAreaRatioThresh[2] = float.Parse(this.AreaRatioThreshnumericUpDown3.Text);
                ////m_stSpotDetectThresh.stHuaPiGuoThresh.fAreaRatioThresh[3] = float.Parse(this.AreaRatioThreshnumericUpDown4.Text);

                //////匹配参数
                ////m_stSpotDetectThresh.nVertiOffsetRange = int.Parse(this.VerOffsetnumericUpDown.Text);
                ////m_stSpotDetectThresh.nAngleOffsetRange = int.Parse(this.AngleOffsetnumericUpDown.Text);
                ////m_stSpotDetectThresh.nAreaOffsetRange = int.Parse(this.AreaOffsetnumericUpDown.Text);

                ////2014.08,12 chengsk修改，协议中删除ConstPreDefine.MAX_LEVEL项 
                ////for (int i = m_stSpotDetectThresh.stLevelFeature.nLevelNum; i < ConstPreDefine.MAX_LEVEL; i++)
                ////    m_stSpotDetectThresh.stLevelFeature.fThreshold[i] = 0x07f;

                //GlobalDataInterface.globalOut_SpotDetectThresh[m_FlawSysIndex * ConstPreDefine.MAX_IPM_NUM + m_FlawIPMInSysIndex].ToCopy(m_stSpotDetectThresh);

                //if (GlobalDataInterface.global_IsTestMode)
                //{
                //    GlobalDataInterface.TransmitParam(m_IPMIDList[m_FlawCurrentIPMIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_FlAWAREA_INFO, m_stSpotDetectThresh);
                //}

                ////2013.10.18 ivycc修改，给IPM发YuvThreshold.txt取消，由IPM自己读取本地文件 
                ////if (File.Exists(Application.StartupPath + "\\YuvThreshold.txt"))
                ////{
                ////    FileInfo YUVfile = new FileInfo(Application.StartupPath + "\\YuvThreshold.txt");
                ////    FileStream YUVstream = YUVfile.OpenRead();
                ////    int length = Marshal.SizeOf(typeof(stYuvThresh));
                ////    byte[] senddata = new byte[length];
                ////    char[] charData = new char[length];
                ////    YUVstream.Read(senddata, 0, length);
                ////    Decoder d = Encoding.UTF8.GetDecoder();
                ////    d.GetChars(senddata, 0, length, charData, 0);


                ////    stYuvThresh yuvThresh = new stYuvThresh(true);

                ////    string str = charData[0].ToString();
                ////    int index = 0; ;
                ////    for (int i = 1; i < length; i++)
                ////    {
                ////        if (charData[i] != ',')
                ////        {
                ////            str += charData[i].ToString();
                ////        }
                ////        else
                ////        {
                ////            index = i + 1;
                ////            break;
                ////        }
                ////    }
                ////    yuvThresh.nNum = int.Parse(str);

                ////    for (int j = 0; j < yuvThresh.nNum; j++)
                ////    {
                ////        str = "";
                ////        for (int i = index; i < length; i++)
                ////        {
                ////            if (charData[i] != ',')
                ////            {
                ////                str += charData[i].ToString();
                ////            }
                ////            else
                ////            {
                ////                index = i + 1;
                ////                break;
                ////            }
                ////        }
                ////        yuvThresh.stYuv[j].nYmax = int.Parse(str);

                ////        str = "";
                ////        for (int i = index; i < length; i++)
                ////        {
                ////            if (charData[i] != ',')
                ////            {
                ////                str += charData[i].ToString();
                ////            }
                ////            else
                ////            {
                ////                index = i + 1;
                ////                break;
                ////            }
                ////        }
                ////        yuvThresh.stYuv[j].nYmin = int.Parse(str);

                ////        str = "";
                ////        for (int i = index; i < length; i++)
                ////        {
                ////            if (charData[i] != ',')
                ////            {
                ////                str += charData[i].ToString();
                ////            }
                ////            else
                ////            {
                ////                index = i + 1;
                ////                break;
                ////            }
                ////        }
                ////        yuvThresh.stYuv[j].nUmax = int.Parse(str);

                ////        str = "";
                ////        for (int i = index; i < length; i++)
                ////        {
                ////            if (charData[i] != ',')
                ////            {
                ////                str += charData[i].ToString();
                ////            }
                ////            else
                ////            {
                ////                index = i + 1;
                ////                break;
                ////            }
                ////        }
                ////        yuvThresh.stYuv[j].nUmin = int.Parse(str);

                ////        str = "";
                ////        for (int i = index; i < length; i++)
                ////        {
                ////            if (charData[i] != ',')
                ////            {
                ////                str += charData[i].ToString();
                ////            }
                ////            else
                ////            {
                ////                index = i + 1;
                ////                break;
                ////            }
                ////        }
                ////        yuvThresh.stYuv[j].nVmax = int.Parse(str);

                ////        str = "";
                ////        for (int i = index; i < length; i++)
                ////        {
                ////            if (charData[i] != ',')
                ////            {
                ////                str += charData[i].ToString();
                ////            }
                ////            else
                ////            {
                ////                index = i + 1;
                ////                break;
                ////            }
                ////        }
                ////        yuvThresh.stYuv[j].nVmin = int.Parse(str);

                ////    }

                ////    if (GlobalDataInterface.global_IsTestMode)
                ////    {
                ////        GlobalDataInterface.TransmitParam(m_IPMIDList[m_FlawCurrentIPMIndex], (int)HC_IPM_COMMAND_TYPE.HC_CMD_LEVELFEATURE_INFO, yuvThresh);
                ////    }
                ////}
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("瑕疵设置界面保存配置出错：" + ex);
                return false;
            }

        }
        /// <summary>
        /// 立即生效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlawEffectbutton_Click(object sender, EventArgs e)
        {
            //if (!FlawSetSaveConfig())
            //    return;    
        }

    }
}
