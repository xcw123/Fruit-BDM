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
using System.Diagnostics;

namespace FruitSortingVtest1._0
{
    public partial class FruitParamForm : Form
    {
        private static List<int> m_IPMIDList = new List<int>();
        int m_CurrentIPMIndex = 0;
        private int Index = GlobalDataInterface.globalIn_defaultInis.FirstOrDefault().nVersion == 40201 ? 0 : 1;
        private static List<int> m_ChannelIDList = new List<int>();
        int m_CurrentChannelIndex = 0;
        int m_CurrentSysIndex = 0;

        public FruitParamForm()
        {
            InitializeComponent();
            GlobalDataInterface.UpFruitGradeInfoEvent += new GlobalDataInterface.FruitGradeInfoEventHandler(OnUpFruitGradeInfo);
        }

        private void FruitParamForm_Load(object sender, EventArgs e)
        {
            try
            {
                ////统计每个子系统的IPM数
                //int IPMid = 0, IDnum = 1;
                //this.IPMcomboBox.Items.Clear();
                //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                //{
                //    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                //    {
                //        if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                //        {
                //            if (j % 2 == 0)
                //            {
                //                IPMid = Commonfunction.EncodeIPM(i, j / 2);
                //                this.IPMcomboBox.Items.Add(string.Format("IPM {0}", IDnum));
                //                IDnum++;

                //                if (m_IPMIDList.Count > 0)
                //                {
                //                    if (m_IPMIDList.Contains(IPMid))
                //                        continue;
                //                }
                //                m_IPMIDList.Add(IPMid);
                //            }

                //        }
                //    }
                //}
                //if (m_IPMIDList.Count > 0)
                //{
                //    this.IPMcomboBox.SelectedIndex = 0;
                //    if (GlobalDataInterface.global_IsTestMode)
                //        GlobalDataInterface.TransmitParam(m_IPMIDList[0], (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);
                //}

                m_IPMIDList.Clear();
                m_ChannelIDList.Clear();
                this.ChannelcomboBox.Items.Clear();
                //统计每个子系统的通道数和IPM数
                int IPMid = 0;
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    {
                        //if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                        if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i] > j)  //Modify by ChengSk - 20190521
                        {
                            if (GlobalDataInterface.nVer == 0)            //版本号判断 add by xcw 20200619
                            {
                                m_ChannelIDList.Add(Commonfunction.EncodeChannel(i, j, j));
                            }
                            else if (GlobalDataInterface.nVer == 1)
                            {
                                m_ChannelIDList.Add(Commonfunction.EncodeChannel(i, j / 2, j % 2));
                            }

                            if (j % 2 == 0)
                            {
                                IPMid = Commonfunction.EncodeIPM(i, j / 2);
                                if (m_IPMIDList.Count > 0)
                                {
                                    if (m_IPMIDList.Contains(IPMid))
                                        continue;
                                }
                                m_IPMIDList.Add(IPMid);
                            }
                        }
                    }
                }
                if (m_ChannelIDList.Count > 0)
                {
                    for (int i = 0; i < m_ChannelIDList.Count; i++)
                    {
                        this.ChannelcomboBox.Items.Add(string.Format("lane-{0}", i + 1));
                    }
                    this.ChannelcomboBox.SelectedIndex = 0;
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(m_IPMIDList[0], (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);
                    }
                }  
          
                //控件属性
                //CIR视觉系统
                if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x01) == 1)
                {
                    this.Diameterlabel1.Enabled = true;       //直径
                    this.DiametertextBox1.Enabled = true;
                    if (GlobalDataInterface.SystemStructProjectedArea)
                    {
                        this.Arealabel1.Enabled = true;       //投影面积
                        this.AreatextBox1.Enabled = true;
                    }
                    else
                    {
                        this.Arealabel1.Enabled = false;       //投影面积
                        this.AreatextBox1.Enabled = false;
                    }
                    if (GlobalDataInterface.SystemStructVolume)
                    {
                        this.Volumelabel1.Enabled = true;     //体积
                        this.VolumetextBox1.Enabled = true;
                    }
                    else
                    {
                        this.Volumelabel1.Enabled = false;     //体积
                        this.VolumetextBox1.Enabled = false;
                    }
                    if (GlobalDataInterface.SystemStructColor)
                    {
                        this.ColorRate0label1.Enabled = true; //颜色1比例
                        this.ColorRate0textBox1.Enabled = true;
                        this.ColorRate1label1.Enabled = true; //颜色2比例
                        this.ColorRate1textBox1.Enabled = true;
                        this.ColorRate2label1.Enabled = true; //颜色3比例
                        this.ColorRate2textBox1.Enabled = true;
                    }
                    else
                    {
                        this.ColorRate0label1.Enabled = false; //颜色1比例
                        this.ColorRate0textBox1.Enabled = false;
                        this.ColorRate1label1.Enabled = false; //颜色2比例
                        this.ColorRate1textBox1.Enabled = false;
                        this.ColorRate2label1.Enabled = false; //颜色3比例
                        this.ColorRate2textBox1.Enabled = false;
                    }
                    if (GlobalDataInterface.SystemStructFlaw)
                    {
                        this.FlawArealabel1.Enabled = true;   //瑕疵面积
                        this.FlawAreatextBox1.Enabled = true;
                        this.FlawNumlabel1.Enabled = true;    //瑕疵个数
                        this.FlawNumtextBox1.Enabled = true;
                    }
                    else
                    {
                        this.FlawArealabel1.Enabled = false;   //瑕疵面积
                        this.FlawAreatextBox1.Enabled = false;
                        this.FlawNumlabel1.Enabled = false;    //瑕疵个数
                        this.FlawNumtextBox1.Enabled = false;
                    }
                    if (GlobalDataInterface.SystemStructShape)
                    {
                        this.VerticalAxislabel1.Enabled = true; //垂轴径
                        this.VerticalAxistextBox1.Enabled = true;
                        this.DiameterRatiolabel1.Enabled = true;//横径比
                        this.DiameterRatiotextBox1.Enabled = true;
                        this.MinDRatiotextBox.Enabled = true;//扁椭型横径比
                        this.MinDRatiolabel.Enabled = true;
                    }
                    else
                    {
                        this.VerticalAxislabel1.Enabled = false; //垂轴径
                        this.VerticalAxistextBox1.Enabled = false;
                        this.DiameterRatiolabel1.Enabled = false;//横径比
                        this.DiameterRatiotextBox1.Enabled = false;
                        this.MinDRatiotextBox.Enabled = false;
                        this.MinDRatiolabel.Enabled = false;
                    }
                }
                else
                {
                    this.Diameterlabel1.Enabled = false;   //直径
                    this.DiametertextBox1.Enabled = false;
                    this.Arealabel1.Enabled = false;       //投影面积
                    this.AreatextBox1.Enabled = false;
                    this.Volumelabel1.Enabled = false;     //体积
                    this.VolumetextBox1.Enabled = false;
                    this.ColorRate0label1.Enabled = false; //颜色1比例
                    this.ColorRate0textBox1.Enabled = false;
                    this.ColorRate1label1.Enabled = false; //颜色2比例
                    this.ColorRate1textBox1.Enabled = false;
                    this.ColorRate2label1.Enabled = false; //颜色3比例
                    this.ColorRate2textBox1.Enabled = false;
                    this.FlawArealabel1.Enabled = false;   //瑕疵面积
                    this.FlawAreatextBox1.Enabled = false;
                    this.FlawNumlabel1.Enabled = false;    //瑕疵个数
                    this.FlawNumtextBox1.Enabled = false;
                    this.VerticalAxislabel1.Enabled = false; //垂轴径
                    this.VerticalAxistextBox1.Enabled = false;
                    this.DiameterRatiolabel1.Enabled = false;//横径比
                    this.DiameterRatiotextBox1.Enabled = false;
                    this.MinDRatiotextBox.Enabled = false;//扁椭型横径比
                    this.MinDRatiolabel.Enabled = false;
                }
                //UV视觉系统
                if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x02) == 2)
                {
                    if (GlobalDataInterface.SystemStructBruise)
                    {
                        this.BruiseArealabel1.Enabled = true;  //擦伤面积
                        this.BruiseAreatextBox1.Enabled = true;
                        this.BruiseNumlabel1.Enabled = true;    //擦伤个数
                        this.BruiseNumtextBox1.Enabled = true;
                    }
                    else
                    {
                        this.BruiseArealabel1.Enabled = false;  //擦伤面积
                        this.BruiseAreatextBox1.Enabled = false;
                        this.BruiseNumlabel1.Enabled = false;    //擦伤个数
                        this.BruiseNumtextBox1.Enabled = false;
                    }
                    if (GlobalDataInterface.SystemStructRot)
                    {
                        this.RotArealabel1.Enabled = true;     //腐蚀面积
                        this.RotAreatextBox1.Enabled = true;
                        this.RotNumlabel1.Enabled = true;      //腐蚀个数
                        this.RotNumtextBox1.Enabled = true;
                    }
                    else
                    {
                        this.RotArealabel1.Enabled = false;     //腐蚀面积
                        this.RotAreatextBox1.Enabled = false;
                        this.RotNumlabel1.Enabled = false;      //腐蚀个数
                        this.RotNumtextBox1.Enabled = false;
                    }
                }
                else
                {
                    this.BruiseArealabel1.Enabled = false;  //擦伤面积
                    this.BruiseAreatextBox1.Enabled = false;
                    this.BruiseNumlabel1.Enabled = false;   //擦伤个数
                    this.BruiseNumtextBox1.Enabled = false;
                    this.RotArealabel1.Enabled = false;     //腐蚀面积
                    this.RotAreatextBox1.Enabled = false;
                    this.RotNumlabel1.Enabled = false;      //腐蚀个数
                    this.RotNumtextBox1.Enabled = false;
                }
                //重量系统
                if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x04) == 4)
                {
                    this.Weightlabel1.Enabled = true;          //重量
                    this.WeighttextBox1.Enabled = true;
                    if (GlobalDataInterface.SystemStructDensity)
                    {
                        this.Densitylabel1.Enabled = true;     //密度
                        this.DensitytextBox1.Enabled = true;
                    }
                    else
                    {
                        this.Densitylabel1.Enabled = false;     //密度
                        this.DensitytextBox1.Enabled = false;
                    }
                }
                else
                {
                    this.Weightlabel1.Enabled = false;      //重量
                    this.WeighttextBox1.Enabled = false;
                    this.Densitylabel1.Enabled = false;     //密度
                    this.DensitytextBox1.Enabled = false;
                }
                //内部品质
                if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8)
                {
                    if (GlobalDataInterface.SystemStructSugar)
                    {
                        this.Sugarlabel1.Enabled = true;       //糖度
                        this.SugartextBox1.Enabled = true;
                    }
                    else
                    {
                        this.Sugarlabel1.Enabled = false;       //糖度
                        this.SugartextBox1.Enabled = false;
                    }
                    if (GlobalDataInterface.SystemStructAcidity)
                    {
                        this.Aciditylabel1.Enabled = true;     //酸度
                        this.AciditytextBox1.Enabled = true;
                    }
                    else
                    {
                        this.Aciditylabel1.Enabled = false;     //酸度
                        this.AciditytextBox1.Enabled = false;
                    }
                    if (GlobalDataInterface.SystemStructHollow)
                    {
                        this.Hollowlabel1.Enabled = true;      //空心
                        this.HollowtextBox1.Enabled = true;
                    }
                    else
                    {
                        this.Hollowlabel1.Enabled = false;      //空心
                        this.HollowtextBox1.Enabled = false;
                    }
                    if (GlobalDataInterface.SystemStructSkin)
                    {
                        this.Skinlabel1.Enabled = true;        //浮皮
                        this.SkintextBox1.Enabled = true;
                    }
                    else
                    {
                        this.Skinlabel1.Enabled = false;        //浮皮
                        this.SkintextBox1.Enabled = false;
                    }
                    if (GlobalDataInterface.SystemStructBrown)
                    {
                        this.Brownlabel1.Enabled = true;       //褐变
                        this.BrowntextBox1.Enabled = true;
                    }
                    else
                    {
                        this.Brownlabel1.Enabled = false;       //褐变
                        this.BrowntextBox1.Enabled = false;
                    }
                    if (GlobalDataInterface.SystemStructTangxin)
                    {
                        this.Tangxinlabel1.Enabled = true;     //糖心
                        this.TangxintextBox1.Enabled = true;
                    }
                    else
                    {
                        this.Tangxinlabel1.Enabled = false;     //糖心
                        this.TangxintextBox1.Enabled = false;
                    }
                }
                else
                {
                    this.Sugarlabel1.Enabled = false;       //糖度
                    this.SugartextBox1.Enabled = false;
                    this.Aciditylabel1.Enabled = false;     //酸度
                    this.AciditytextBox1.Enabled = false;
                    this.Hollowlabel1.Enabled = false;      //空心
                    this.HollowtextBox1.Enabled = false;
                    this.Skinlabel1.Enabled = false;        //浮皮
                    this.SkintextBox1.Enabled = false;
                    this.Brownlabel1.Enabled = false;       //褐变
                    this.BrowntextBox1.Enabled = false;
                    this.Tangxinlabel1.Enabled = false;     //糖心
                    this.TangxintextBox1.Enabled = false;
                }
                //超声波系统
                if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x10) == 16)
                {
                    if (GlobalDataInterface.SystemStructRigidity)
                    {
                        this.Rigiditylabel1.Enabled = true;     //硬度
                        this.RigiditytextBox1.Enabled = true;
                    }
                    else
                    {
                        this.Rigiditylabel1.Enabled = false;     //硬度
                        this.RigiditytextBox1.Enabled = false;
                    }
                    if (GlobalDataInterface.SystemStructWater)
                    {
                        this.Waterlabel1.Enabled = true;        //含水率
                        this.WatertextBox1.Enabled = true;
                    }
                    else
                    {
                        this.Waterlabel1.Enabled = false;        //含水率
                        this.WatertextBox1.Enabled = false;
                    }
                }
                else
                {
                    this.Rigiditylabel1.Enabled = false;     //硬度
                    this.RigiditytextBox1.Enabled = false;
                    this.Waterlabel1.Enabled = false;        //含水率
                    this.WatertextBox1.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("FruitParamForm中函数FruitParamForm_Load出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("FruitParamForm中函数FruitParamForm_Load出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 上传水果等级信息刷新
        /// </summary>
        /// <param name="fruitGradeInfo"></param>
        public void OnUpFruitGradeInfo(stFruitGradeInfo fruitGradeInfo)
        {
		if (GlobalDataInterface.nVer == 0)
        {
            try
            {
                if (this == Form.ActiveForm)//是否操作当前窗体
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new GlobalDataInterface.FruitGradeInfoEventHandler(OnUpFruitGradeInfo), fruitGradeInfo);
                    }
                    else
                    {
                        int ChannelInIPMIndex = Commonfunction.ChanelInIPMIndex(m_ChannelIDList[m_CurrentChannelIndex]);
                        if (fruitGradeInfo.param[Index].unGrade == 0x7f7f7f7f)
                            return;
                        uint SizeGradeIndex, QualfGradeIndex;
                        byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        string GradeName;
                        //通道1
                        if (fruitGradeInfo.param[Index].unGrade != 0x7f7f7f7f)
                        {
                            if (this.DiametertextBox1.Enabled == true)
                            {
                                this.DiametertextBox1.Text = fruitGradeInfo.param[Index].visionParam.unSelectBasis.ToString("0.0");//直径
                            }
                            if (this.AreatextBox1.Enabled == true)
                            {
                                this.AreatextBox1.Text = fruitGradeInfo.param[Index].visionParam.unArea.ToString();//投影面积
                            }
                            if (this.VolumetextBox1.Enabled == true)
                            {
                                this.VolumetextBox1.Text = fruitGradeInfo.param[Index].visionParam.unVolume.ToString();//体积
                            }
                            if (this.ColorRate0textBox1.Enabled == true)
                            {
                                this.ColorRate0textBox1.Text = (fruitGradeInfo.param[Index].visionParam.unColorRate0 / 2).ToString();//颜色比例1
                            }
                            if (this.ColorRate1textBox1.Enabled == true)
                            {
                                this.ColorRate1textBox1.Text = (fruitGradeInfo.param[Index].visionParam.unColorRate1 / 2).ToString();//颜色比例2
                            }
                            if (this.ColorRate2textBox1.Enabled == true)
                            {
                                this.ColorRate2textBox1.Text = (fruitGradeInfo.param[Index].visionParam.unColorRate2 / 2).ToString();//颜色比例3
                            }
                            if (this.FlawAreatextBox1.Enabled == true)
                            {
                                this.FlawAreatextBox1.Text = fruitGradeInfo.param[Index].visionParam.unFlawArea.ToString();//瑕疵面积
                            }
                            if (this.FlawNumtextBox1.Enabled == true)
                            {
                                this.FlawNumtextBox1.Text = fruitGradeInfo.param[Index].visionParam.unFlawNum.ToString();//瑕疵个数
                            }
                            if (this.VerticalAxistextBox1.Enabled == true) //Add by ChengSk - 20180802
                            {
                                //Modify by ChengSk - 20190923
                                //this.VerticalAxistextBox1.Text = fruitGradeInfo.param.visionParam.unVertiSACenterD.ToString("0.000");//垂轴径
                            }
                            if (this.DiameterRatiotextBox1.Enabled == true)
                            {
                                this.DiameterRatiotextBox1.Text = fruitGradeInfo.param[Index].visionParam.fDiameterRatio.ToString();//横径比
                            }
                            if (this.MinDRatiotextBox.Enabled == true)
                            {
                                this.MinDRatiotextBox.Text = fruitGradeInfo.param[Index].visionParam.fMinDRatio.ToString();//扁椭型横径比
                            }
                            if (this.BruiseAreatextBox1.Enabled == true)
                            {
                                this.BruiseAreatextBox1.Text = fruitGradeInfo.param[Index].uvParam.unBruiseArea.ToString("0.00");//擦伤面积
                            }
                            if (this.BruiseNumtextBox1.Enabled == true)
                            {
                                this.BruiseNumtextBox1.Text = fruitGradeInfo.param[Index].uvParam.unBruiseNum.ToString("0.00");//擦伤个数
                            }
                            if (this.RotAreatextBox1.Enabled == true)
                            {
                                this.RotAreatextBox1.Text = fruitGradeInfo.param[Index].uvParam.unRotArea.ToString("0.00");//腐烂面积
                            }
                            if (this.RotNumtextBox1.Enabled == true)
                            {
                                this.RotNumtextBox1.Text = fruitGradeInfo.param[Index].uvParam.unRotNum.ToString("0.00");//腐烂个数
                            }
                            if (this.WeighttextBox1.Enabled == true)
                            {
                                this.WeighttextBox1.Text = fruitGradeInfo.param[Index].fWeight.ToString("0.00");//重量
                            }
                            if (this.DensitytextBox1.Enabled == true)
                            {
                                this.DensitytextBox1.Text = fruitGradeInfo.param[Index].fDensity.ToString("0.00");//密度
                            }
                            if (this.SugartextBox1.Enabled == true)
                            {
                                if(fruitGradeInfo.param[Index].nirParam.fSugar == 0x7f7f7f7f)
                                    this.SugartextBox1.Text ="0.00";//糖度
                                else
                                    this.SugartextBox1.Text = fruitGradeInfo.param[Index].nirParam.fSugar.ToString("0.00");//糖度
                            }
                            if (this.AciditytextBox1.Enabled == true)
                            {
                                if (fruitGradeInfo.param[Index].nirParam.fAcidity == 0x7f7f7f7f)
                                    this.AciditytextBox1.Text = "0.00";
                                else 
                                    this.AciditytextBox1.Text = fruitGradeInfo.param[Index].nirParam.fAcidity.ToString("0.00");//酸度
                            }
                            if (this.HollowtextBox1.Enabled == true)
                            {
                                this.HollowtextBox1.Text = fruitGradeInfo.param[Index].nirParam.fHollow.ToString("0.00");//空心
                            }
                            if (this.SkintextBox1.Enabled == true)
                            {
                                this.SkintextBox1.Text = fruitGradeInfo.param[Index].nirParam.fSkin.ToString("0.00");//浮皮
                            }
                            if (this.BrowntextBox1.Enabled == true)
                            {
                                this.BrowntextBox1.Text = fruitGradeInfo.param[Index].nirParam.fBrown.ToString("0.00");//褐变
                            }
                            if (this.TangxintextBox1.Enabled == true)
                            {
                                this.TangxintextBox1.Text = fruitGradeInfo.param[Index].nirParam.fTangxin.ToString("0.00");//糖心
                            }
                            if (this.RigiditytextBox1.Enabled == true)
                            {
                                this.RigiditytextBox1.Text = fruitGradeInfo.param[Index].uvParam.unRigidity.ToString("0.00");//硬度
                            }
                            if (this.WatertextBox1.Enabled == true)
                            {
                                this.WatertextBox1.Text = fruitGradeInfo.param[Index].uvParam.unWater.ToString("0.00");//含水率 
                            }                                              
                            //等级名称
                            //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 57) > 0 && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 7) > 0)//品质与尺寸或者品质与重量
                            if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0)//品质与尺寸或者品质与重量
                            {
                                SizeGradeIndex = fruitGradeInfo.param[Index].unGrade & 15;
                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, SizeGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');

                                QualfGradeIndex = (fruitGradeInfo.param[Index].unGrade & 240) >> 4;
                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, QualfGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                GradeName += "." + Encoding.Default.GetString(temp).TrimEnd('\0');
                            }
                            //else if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 1)//品质
                            else if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 0) //只有品质
                            {
                                QualfGradeIndex = fruitGradeInfo.param[Index].unGrade & 240;
                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, QualfGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                            }
                            else//尺寸
                            {
                                SizeGradeIndex = fruitGradeInfo.param[Index].unGrade & 15;
                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, SizeGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                            }
                            this.GradetextBox1.Text = GradeName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("FruitParamForm中函数OnUpFruitGradeInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("FruitParamForm中函数OnUpFruitGradeInfo出错" + ex);
#endif
            }
        }
        else
        {
            try
            {
                if (this == Form.ActiveForm)//是否操作当前窗体
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new GlobalDataInterface.FruitGradeInfoEventHandler(OnUpFruitGradeInfo), fruitGradeInfo);
                    }
                    else
                    {
                        int ChannelInIPMIndex = Commonfunction.ChanelInIPMIndex(m_ChannelIDList[m_CurrentChannelIndex]);

                        if (fruitGradeInfo.param[ChannelInIPMIndex].unGrade == 0x7f7f7f7f)
                            return;
                        uint SizeGradeIndex, QualfGradeIndex;
                        byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        string GradeName;
                        //通道1
                        if (fruitGradeInfo.param[ChannelInIPMIndex].unGrade != 0x7f7f7f7f)
                        {
                            if (this.DiametertextBox1.Enabled == true)
                            {
                                this.DiametertextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unSelectBasis.ToString("0.0");//直径
                            }
                            if (this.AreatextBox1.Enabled == true)
                            {
                                this.AreatextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unArea.ToString();//投影面积
                            }
                            if (this.VolumetextBox1.Enabled == true)
                            {
                                this.VolumetextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unVolume.ToString();//体积
                            }
                            if (this.ColorRate0textBox1.Enabled == true)
                            {
                                this.ColorRate0textBox1.Text = (fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unColorRate0 / 2).ToString();//颜色比例1
                            }
                            if (this.ColorRate1textBox1.Enabled == true)
                            {
                                this.ColorRate1textBox1.Text = (fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unColorRate1 / 2).ToString();//颜色比例2
                            }
                            if (this.ColorRate2textBox1.Enabled == true)
                            {
                                this.ColorRate2textBox1.Text = (fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unColorRate2 / 2).ToString();//颜色比例3
                            }
                            if (this.FlawAreatextBox1.Enabled == true)
                            {
                                this.FlawAreatextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unFlawArea.ToString();//瑕疵面积
                            }
                            if (this.FlawNumtextBox1.Enabled == true)
                            {
                                this.FlawNumtextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unFlawNum.ToString();//瑕疵个数
                            }
                            if (this.VerticalAxistextBox1.Enabled == true) //Add by ChengSk - 20180802
                            {
                                //Modify by ChengSk - 20190923
                                //this.VerticalAxistextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.unVertiSACenterD.ToString("0.000");//垂轴径
                            }
                            if (this.DiameterRatiotextBox1.Enabled == true)
                            {
                                this.DiameterRatiotextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.fDiameterRatio.ToString();//横径比
                            }
                            if (this.MinDRatiotextBox.Enabled == true)
                            {
                                this.MinDRatiotextBox.Text = fruitGradeInfo.param[ChannelInIPMIndex].visionParam.fMinDRatio.ToString();//扁椭型横径比
                            }
                            if (this.BruiseAreatextBox1.Enabled == true)
                            {
                                this.BruiseAreatextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].uvParam.unBruiseArea.ToString("0.00");//擦伤面积
                            }
                            if (this.BruiseNumtextBox1.Enabled == true)
                            {
                                this.BruiseNumtextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].uvParam.unBruiseNum.ToString("0.00");//擦伤个数
                            }
                            if (this.RotAreatextBox1.Enabled == true)
                            {
                                this.RotAreatextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].uvParam.unRotArea.ToString("0.00");//腐烂面积
                            }
                            if (this.RotNumtextBox1.Enabled == true)
                            {
                                this.RotNumtextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].uvParam.unRotNum.ToString("0.00");//腐烂个数
                            }
                            if (this.WeighttextBox1.Enabled == true)
                            {
                                this.WeighttextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].fWeight.ToString("0.00");//重量
                            }
                            if (this.DensitytextBox1.Enabled == true)
                            {
                                this.DensitytextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].fDensity.ToString("0.00");//密度
                            }
                            if (this.SugartextBox1.Enabled == true)
                            {
                                if(fruitGradeInfo.param[ChannelInIPMIndex].nirParam.fSugar == 0x7f7f7f7f)
                                    this.SugartextBox1.Text ="0.00";//糖度
                                else
                                    this.SugartextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].nirParam.fSugar.ToString("0.00");//糖度
                            }
                            if (this.AciditytextBox1.Enabled == true)
                            {
                                if (fruitGradeInfo.param[ChannelInIPMIndex].nirParam.fAcidity == 0x7f7f7f7f)
                                    this.AciditytextBox1.Text = "0.00";
                                else 
                                    this.AciditytextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].nirParam.fAcidity.ToString("0.00");//酸度
                            }
                            if (this.HollowtextBox1.Enabled == true)
                            {
                                this.HollowtextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].nirParam.fHollow.ToString("0.00");//空心
                            }
                            if (this.SkintextBox1.Enabled == true)
                            {
                                this.SkintextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].nirParam.fSkin.ToString("0.00");//浮皮
                            }
                            if (this.BrowntextBox1.Enabled == true)
                            {
                                this.BrowntextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].nirParam.fBrown.ToString("0.00");//褐变
                            }
                            if (this.TangxintextBox1.Enabled == true)
                            {
                                this.TangxintextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].nirParam.fTangxin.ToString("0.00");//糖心
                            }
                            if (this.RigiditytextBox1.Enabled == true)
                            {
                                this.RigiditytextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].uvParam.unRigidity.ToString("0.00");//硬度
                            }
                            if (this.WatertextBox1.Enabled == true)
                            {
                                this.WatertextBox1.Text = fruitGradeInfo.param[ChannelInIPMIndex].uvParam.unWater.ToString("0.00");//含水率 
                            }                                              
                            //等级名称
                            //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 57) > 0 && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 7) > 0)//品质与尺寸或者品质与重量
                            if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0)//品质与尺寸或者品质与重量
                            {
                                SizeGradeIndex = fruitGradeInfo.param[ChannelInIPMIndex].unGrade & 15;
                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, SizeGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');

                                QualfGradeIndex = (fruitGradeInfo.param[ChannelInIPMIndex].unGrade & 240) >> 4;
                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, QualfGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                GradeName += "." + Encoding.Default.GetString(temp).TrimEnd('\0');
                            }
                            //else if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 1)//品质
                            else if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 0) //只有品质
                            {
                                QualfGradeIndex = fruitGradeInfo.param[ChannelInIPMIndex].unGrade & 240;
                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, QualfGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                            }
                            else//尺寸
                            {
                                SizeGradeIndex = fruitGradeInfo.param[ChannelInIPMIndex].unGrade & 15;
                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, SizeGradeIndex * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                GradeName = Encoding.Default.GetString(temp).TrimEnd('\0');
                            }
                            this.GradetextBox1.Text = GradeName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("FruitParamForm中函数OnUpFruitGradeInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("FruitParamForm中函数OnUpFruitGradeInfo出错" + ex);
#endif
            } 
        }
            
        }


        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FruitParamForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (GlobalDataInterface.global_IsTestMode)
            {
                //if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                //{
                //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPMChannel(m_CurrentSysIndex, m_CurrentIPMIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                //}
                //else if (GlobalDataInterface.nVer == 0)
                //{
                //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(m_CurrentSysIndex, m_CurrentIPMIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                //}
                GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(m_CurrentSysIndex, m_CurrentIPMIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
            }
            GlobalDataInterface.UpFruitGradeInfoEvent -= new GlobalDataInterface.FruitGradeInfoEventHandler(OnUpFruitGradeInfo); //Add by ChengSk - 20180830
        }

        /// <summary>
        /// Channel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int tempChannelIndex = this.ChannelcomboBox.SelectedIndex;
                int tempCurrentIPMIndex = Commonfunction.GetIPMIndex(m_ChannelIDList[tempChannelIndex]);
                int tempCurrentSysIndex = Commonfunction.GetSubsysIndex(m_ChannelIDList[tempChannelIndex]);

                if (GlobalDataInterface.global_IsTestMode)
                {
                    //if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                    //{
                    //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPMChannel(m_CurrentSysIndex, m_CurrentIPMIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                    //    //GlobalDataInterface.TransmitParam(m_IPMIDList[this.IPMcomboBox.SelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);
                    //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPMChannel(tempCurrentSysIndex, tempCurrentIPMIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);
                    //}

                    //else if (GlobalDataInterface.nVer == 0)
                    //{
                    //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(m_CurrentSysIndex, m_CurrentIPMIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                    //    //GlobalDataInterface.TransmitParam(m_IPMIDList[this.IPMcomboBox.SelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);
                    //    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(tempCurrentSysIndex, tempCurrentIPMIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);
                    //}
                    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(m_CurrentSysIndex, m_CurrentIPMIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                    //GlobalDataInterface.TransmitParam(m_IPMIDList[this.IPMcomboBox.SelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);
                    GlobalDataInterface.TransmitParam(Commonfunction.EncodeIPM(tempCurrentSysIndex, tempCurrentIPMIndex), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_ON, null);
                }
                m_CurrentChannelIndex = this.ChannelcomboBox.SelectedIndex;
                m_CurrentIPMIndex = Commonfunction.GetIPMIndex(m_ChannelIDList[m_CurrentChannelIndex]);
                m_CurrentSysIndex = Commonfunction.GetSubsysIndex(m_ChannelIDList[m_CurrentChannelIndex]);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("FruitParamForm中函数ChannelcomboBox_SelectedIndexChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("FruitParamForm中函数ChannelcomboBox_SelectedIndexChanged出错" + ex);
#endif
            }
        }

    }
}
