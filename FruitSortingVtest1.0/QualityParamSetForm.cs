using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interface;
using System.Collections;
using System.Diagnostics;
using System.Resources;
using Common;

namespace FruitSortingVtest1._0
{
    
    public partial class QualityParamSetForm : Form
    {
        Hashtable htPages = new Hashtable();//装载tabpages
        QualGradeSetForm m_qualGradeSetForm;
        MainForm m_mainForm;
        int TabSelectedIndex; //选择框索引 Add by ChengSk - 20180724
        private ResourceManager m_resourceManager = new ResourceManager(typeof(QualityParamSetForm));//创建Mainform资源管理
        List<string> PagesIndex = new List<string>();
        AutoSizeFormClass asc = new AutoSizeFormClass();//声明大小自适应类实例  

        public QualityParamSetForm(QualGradeSetForm qualGradeSetForm, MainForm mainForm)
        {
            m_qualGradeSetForm = qualGradeSetForm;
            m_mainForm = mainForm;
            //try
            //{
                InitializeComponent();
            //}
            //catch (Exception ex)
            //{
                int I = 0;
            //}
            GlobalDataInterface.UpSpliceImageDataEvent += new GlobalDataInterface.SpliceImageDataEventHandler(OnUpSpliceImageData);
            GlobalDataInterface.UpSpliceFlawImageDataEvent += new GlobalDataInterface.SpliceImageDataEventHandler(OnUpFlawSpliceImageData);
            GlobalDataInterface.UpSpotImageDataEvent += new GlobalDataInterface.SpotImageDataEventHandler(OnUpSpotImageData);
            GlobalDataInterface.UpBruiseImageDataEvent += new GlobalDataInterface.BruiseImageDataEventHandler(OnUpBruiseImageData);
            GlobalDataInterface.UpRotImageDataEvent += new GlobalDataInterface.RotImageDataEventHandler(OnUpRotImageData);
            asc.controllInitializeSize(this); 
        }
        bool ColorSet = false;  //TabPages移除首项存在异常，单独操作
        private void QualitySetForm_Load(object sender, EventArgs e)
        {
            try
            {
                //this.ColorlistViewEx.Scrollable = false;

                //颜色
                if (GlobalDataInterface.SystemStructColor && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x01) == 1))
                {
                    ColorSet = true;
                    if (QualitytabControl.TabPages.IndexOf(this.ColorSettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["ColorSettabPage"];
                        QualitytabControl.TabPages.Insert(0, tabPage);
                    }
                    PagesIndex.Add("0");
                    ColorFormInitial();
                }
                else
                {
                    if ((TabPage)htPages["ColorSettabPage"] == null)
                    {
                        ColorSet = false;
                        htPages.Add("ColorSettabPage", this.ColorSettabPage);

                        //QualitytabControl.TabPages.RemoveAt(a);
                        //QualitytabControl.TabPages.Remove(this.ColorSettabPage);

                    }
                }//add by xcw 20201207
               
                //形状
                if (GlobalDataInterface.SystemStructShape && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x01) == 1))
                {
                    //int a = QualitytabControl.TabPages.IndexOf(this.ShapeSettabPage);
                    if (QualitytabControl.TabPages.IndexOf(this.ShapeSettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["ShapeSettabPage"];
                        QualitytabControl.TabPages.Insert(1, tabPage);
                        
                    }
                    PagesIndex.Add("1");
                    ShapeSetFormInitial();
                }
                else
                {
                    if ((TabPage)htPages["ShapeSettabPage"] == null)
                    {
                        htPages.Add("ShapeSettabPage", this.ShapeSettabPage);
                        QualitytabControl.TabPages.Remove(this.ShapeSettabPage);
                    }
                }
                //瑕疵
                if (GlobalDataInterface.SystemStructFlaw && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x01) == 1))
                {
                    //int a = QualitytabControl.TabPages.IndexOf(this.FlawSettabPage);
                    if (QualitytabControl.TabPages.IndexOf(this.FlawSettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["FlawSettabPage"];
                        QualitytabControl.TabPages.Insert(2, tabPage);
                    }
                    PagesIndex.Add("2");
                    FlawSetInitial();
                }
                else
                {
                    if ((TabPage)htPages["FlawSettabPage"] == null)
                    {
                        htPages.Add("FlawSettabPage", this.FlawSettabPage);
                        QualitytabControl.TabPages.Remove(this.FlawSettabPage);
                    }
                }
                //擦伤
                if (GlobalDataInterface.SystemStructBruise && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x02) == 2))
                {
                    if (QualitytabControl.TabPages.IndexOf(this.BruiseSettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["BruiseSettabPage"];
                        QualitytabControl.TabPages.Insert(3, tabPage);
                    }
                    PagesIndex.Add("3");
                    BruiseSetInitial();
                }
                else
                {
                    if ((TabPage)htPages["BruiseSettabPage"] == null)
                    {
                        htPages.Add("BruiseSettabPage", this.BruiseSettabPage);
                        QualitytabControl.TabPages.Remove(this.BruiseSettabPage);
                    }
                }
                //腐烂
                if (GlobalDataInterface.SystemStructRot && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x02) == 2))
                {
                    if (QualitytabControl.TabPages.IndexOf(this.RotSettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["RotSettabPage"];
                        QualitytabControl.TabPages.Insert(4, tabPage);
                    }
                    PagesIndex.Add("4");
                    RotSetInitial();
                }
                else
                {
                    if ((TabPage)htPages["RotSettabPage"] == null)
                    {
                        htPages.Add("RotSettabPage", this.RotSettabPage);
                        QualitytabControl.TabPages.Remove(this.RotSettabPage);
                    }
                }
                //密度
                if (GlobalDataInterface.SystemStructDensity && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x04) == 4))
                {
                    if (QualitytabControl.TabPages.IndexOf(this.DensitySettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["DensitySettabPage"];
                        QualitytabControl.TabPages.Insert(5, tabPage);
                    }
                    PagesIndex.Add("5");
                    DensitySetInitial();
                }
                else
                {
                    if ((TabPage)htPages["DensitySettabPage"] == null)
                    {
                        htPages.Add("DensitySettabPage", this.DensitySettabPage);
                        QualitytabControl.TabPages.Remove(this.DensitySettabPage);
                    }
                }
                //糖度
                if (GlobalDataInterface.SystemStructSugar && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8))
                {
                    if (QualitytabControl.TabPages.IndexOf(this.SugarSettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["SugarSettabPage"];
                        QualitytabControl.TabPages.Insert(6, tabPage);
                    }
                    PagesIndex.Add("6");
                    SugarSetInitial();
                }
                else
                {
                    if ((TabPage)htPages["SugarSettabPage"] == null)
                    {
                        htPages.Add("SugarSettabPage", this.SugarSettabPage);
                        QualitytabControl.TabPages.Remove(this.SugarSettabPage);
                    }
                }
                //酸度
                if (GlobalDataInterface.SystemStructAcidity && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8))
                {
                    if (QualitytabControl.TabPages.IndexOf(this.AciditySettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["AciditySettabPage"];
                        QualitytabControl.TabPages.Insert(7, tabPage);
                    }
                    PagesIndex.Add("7");
                    AciditySetInitial();
                }
                else
                {
                    if ((TabPage)htPages["AciditySettabPage"] == null)
                    {
                        htPages.Add("AciditySettabPage", this.AciditySettabPage);
                        QualitytabControl.TabPages.Remove(this.AciditySettabPage);
                    }
                }
                //空心
                if (GlobalDataInterface.SystemStructHollow && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8))
                {
                    if (QualitytabControl.TabPages.IndexOf(this.HollowSettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["HollowSettabPage"];
                        QualitytabControl.TabPages.Insert(8, tabPage);
                    }
                    PagesIndex.Add("8");
                    HollowSetInitial();
                }
                else
                {
                    if ((TabPage)htPages["HollowSettabPage"] == null)
                    {
                        htPages.Add("HollowSettabPage", this.HollowSettabPage);
                        QualitytabControl.TabPages.Remove(this.HollowSettabPage);
                    }
                }
                //浮皮
                if (GlobalDataInterface.SystemStructSkin && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8))
                {
                    if (QualitytabControl.TabPages.IndexOf(this.SkinSettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["SkinSettabPage"];
                        QualitytabControl.TabPages.Insert(9, tabPage);
                    }
                    PagesIndex.Add("9");
                    SkinSetInitial();
                }
                else
                {
                    if ((TabPage)htPages["SkinSettabPage"] == null)
                    {
                        htPages.Add("SkinSettabPage", this.SkinSettabPage);
                        QualitytabControl.TabPages.Remove(this.SkinSettabPage);
                    }
                }
                //褐变
                if (GlobalDataInterface.SystemStructBrown && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8))
                {
                    if (QualitytabControl.TabPages.IndexOf(this.BrownSettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["BrownSettabPage"];
                        QualitytabControl.TabPages.Insert(10, tabPage);
                    }
                    PagesIndex.Add("10");
                    BrownSetInitial();
                }
                else
                {
                    if ((TabPage)htPages["BrownSettabPage"] == null)
                    {
                        htPages.Add("BrownSettabPage", this.BrownSettabPage);
                        QualitytabControl.TabPages.Remove(this.BrownSettabPage);
                    }
                }
                //糖心
                if (GlobalDataInterface.SystemStructTangxin && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8))
                {
                    if (QualitytabControl.TabPages.IndexOf(this.TangxinSettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["TangxinSettabPage"];
                        QualitytabControl.TabPages.Insert(11, tabPage);
                    }
                    PagesIndex.Add("11");
                    TangxinSetInitial();
                }
                else
                {
                    if ((TabPage)htPages["TangxinSettabPage"] == null)
                    {
                        htPages.Add("TangxinSettabPage", this.TangxinSettabPage);
                        QualitytabControl.TabPages.Remove(this.TangxinSettabPage);
                    }
                }
                //硬度
                if (GlobalDataInterface.SystemStructRigidity && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x10) == 16))
                {
                    if (QualitytabControl.TabPages.IndexOf(this.RigiditySettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["RigiditySettabPage"];
                        QualitytabControl.TabPages.Insert(12, tabPage);
                    }
                    PagesIndex.Add("12");
                    RigiditySetInitial();
                }
                else
                {
                    if ((TabPage)htPages["RigiditySettabPage"] == null)
                    {
                        htPages.Add("RigiditySettabPage", this.RigiditySettabPage);
                        QualitytabControl.TabPages.Remove(this.RigiditySettabPage);
                    }
                }
                //含水率
                if (GlobalDataInterface.SystemStructWater && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x10) == 16))
                {
                    if (QualitytabControl.TabPages.IndexOf(this.WaterSettabPage) < 0)
                    {
                        TabPage tabPage = (TabPage)htPages["WaterSettabPage"];
                        QualitytabControl.TabPages.Insert(13, tabPage);
                    }
                    PagesIndex.Add("13");
                    WaterSetInitial();
                }
                else
                {
                    if ((TabPage)htPages["WaterSettabPage"] == null)
                    {
                        htPages.Add("WaterSettabPage", this.WaterSettabPage);
                        QualitytabControl.TabPages.Remove(this.WaterSettabPage);
                    }
                }
                if (!ColorSet) //add by xcw 20210115
                {
                    QualitytabControl.TabPages.Remove(this.ColorSettabPage);
                }

                this.nTagInfoYtextBox.Text = GlobalDataInterface.globalOut_GradeInfo.nTagInfo[4].ToString();
                this.nTagInfoHtextBox.Text = GlobalDataInterface.globalOut_GradeInfo.nTagInfo[5].ToString();//modify by xcw 20200709
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm中函数QualitySetForm_Load出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm中函数QualitySetForm_Load出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKbutton_Click(object sender, EventArgs e)
        {
            try
            {
                bool SavingError = false;
                GlobalDataInterface.globalOut_GradeInfo.nTagInfo[4]= (byte)(int.Parse(this.nTagInfoYtextBox.Text));
                GlobalDataInterface.globalOut_GradeInfo.nTagInfo[5] = (byte)(int.Parse(this.nTagInfoHtextBox.Text));//modify by xcw 20200709
                stGradeInfo tempGradeInfo = new stGradeInfo(true);
                tempGradeInfo.ToCopy(GlobalDataInterface.globalOut_GradeInfo);

                if (QualitytabControl.TabPages.IndexOf(this.ColorSettabPage) >= 0)
                {
                    if (!ColorSetSaveConfig())
                    {
                        this.ColorSettabPage.Select();
                        SavingError = true;
                    }
                }
                if (QualitytabControl.TabPages.IndexOf(this.ShapeSettabPage) >= 0)
                {
                    if (!ShapeSetSaveConfig())
                    {
                        this.ShapeSettabPage.Select();
                        SavingError = true;
                    }
                }
                if (QualitytabControl.TabPages.IndexOf(this.FlawSettabPage) >= 0)
                {
                    if (!FlawSetSaveConfig())
                    {
                        this.FlawSettabPage.Select();
                        SavingError = true;
                    }
                }
                if (QualitytabControl.TabPages.IndexOf(this.BruiseSettabPage) >= 0)
                {
                    if (!BruiseSetSaveConfig())
                    {
                        this.BruiseSettabPage.Select();
                        SavingError = true;
                    }
                }

                if (QualitytabControl.TabPages.IndexOf(this.RotSettabPage) >= 0)
                {
                    if (!RotSetSaveConfig())
                    {
                        this.RotSettabPage.Select();
                        SavingError = true;
                    }
                }
                if (QualitytabControl.TabPages.IndexOf(this.DensitySettabPage) >= 0)
                {
                    if (!DensitySetSaveConfig())
                    {
                        this.DensitySettabPage.Select();
                        SavingError = true;
                    }
                }
                if (QualitytabControl.TabPages.IndexOf(this.SugarSettabPage) >= 0)
                {
                    if (!SugarSetSaveConfig())
                    {
                        this.SugarSettabPage.Select();
                        SavingError = true;
                    }
                }
                if (QualitytabControl.TabPages.IndexOf(this.AciditySettabPage) >= 0)
                {
                    if (!AciditySetSaveConfig())
                    {
                        this.AciditySettabPage.Select();
                        SavingError = true;
                    }
                }
                if (QualitytabControl.TabPages.IndexOf(this.HollowSettabPage) >= 0)
                {
                    if (!HollowSetSaveConfig())
                    {
                        this.HollowSettabPage.Select();
                        SavingError = true;
                    }
                }
                if (QualitytabControl.TabPages.IndexOf(this.SkinSettabPage) >= 0)
                {
                    if (!SkinSetSaveConfig())
                    {
                        this.SkinSettabPage.Select();
                        SavingError = true;
                    }
                }
                if (QualitytabControl.TabPages.IndexOf(this.BrownSettabPage) >= 0)
                {
                    if (!BrownSetSaveConfig())
                    {
                        this.BrownSettabPage.Select();
                        SavingError = true;
                    }
                }
                if (QualitytabControl.TabPages.IndexOf(this.TangxinSettabPage) >= 0)
                {
                    if (!TangxinSetSaveConfig())
                    {
                        this.TangxinSettabPage.Select();
                        SavingError = true;
                    }
                }
                if (QualitytabControl.TabPages.IndexOf(this.RigiditySettabPage) >= 0)
                {
                    if (!RigiditySetSaveConfig())
                    {
                        this.RigiditySettabPage.Select();
                        SavingError = true;
                    }
                }
                if (QualitytabControl.TabPages.IndexOf(this.WaterSettabPage) >= 0)
                {
                    if (!WaterSetSaveConfig())
                    {
                        this.WaterSettabPage.Select();
                        SavingError = true;
                    }
                }          

                if (!GlobalDataInterface.globalOut_GradeInfo.IsEqual(tempGradeInfo))
                {
                    m_mainForm.SetSeparationProgrameChangelabel(false, null);
                }

                if (SavingError)
                {
                    //DialogResult result = MessageBox.Show("参数保存出错，还要关闭吗？", "保存出错", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //DialogResult result = MessageBox.Show("0x1000100A Grade Setting's configuration saved error!Do you want to close it?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    DialogResult result = MessageBox.Show("0x1000100A " + LanguageContainer.QualityParamSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.QualityParamSetFormMessageboxQuestionCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    else
                        return;
                }
                else
                {

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }

                if (GlobalDataInterface.ColorGradeNum == 0)
                    m_qualGradeSetForm.EnableQualGradeSetcomboBox(false);
                else
                    m_qualGradeSetForm.EnableQualGradeSetcomboBox(true);

            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm中函数OKbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm中函数OKbutton_Click出错" + ex);
#endif
            }
            
        }

        private void QualityParamSetForm_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }

        private void QualityParamSetForm_KeyDown(object sender, KeyEventArgs e)
        {
            int SelectChannelNo = 0; //通道序列号
            if (e.Alt)
            {
                m_cameraIndex = 1;
                if (e.KeyCode == Keys.D1)
                {
                    e.Handled = true;
                    SelectChannelNo = 1;
                }
                else if (e.KeyCode == Keys.D2)
                {
                    e.Handled = true;
                    SelectChannelNo = 2;
                }
                else if (e.KeyCode == Keys.D3 && e.Alt)
                {
                    e.Handled = true;
                    SelectChannelNo = 3;
                }
                else if (e.KeyCode == Keys.D4 && e.Alt)
                {
                    e.Handled = true;
                    SelectChannelNo = 4;
                }
                else if (e.KeyCode == Keys.D5 && e.Alt)
                {
                    e.Handled = true;
                    SelectChannelNo = 5;
                }
                else if (e.KeyCode == Keys.D6 && e.Alt)
                {
                    e.Handled = true;
                    SelectChannelNo = 6;
                }
                else if (e.KeyCode == Keys.D7 && e.Alt)
                {
                    e.Handled = true;
                    SelectChannelNo = 7;
                }
                else if (e.KeyCode == Keys.D8 && e.Alt)
                {
                    e.Handled = true;
                    SelectChannelNo = 8;
                }
            }
            else if (e.Control)
            {
                m_cameraIndex = 0;
                if (e.KeyCode == Keys.D1)
                {
                    e.Handled = true;
                    SelectChannelNo = 1;
                }

                else if (e.KeyCode == Keys.D2 )
                {
                    e.Handled = true;
                    SelectChannelNo = 2;
                }
                else if (e.KeyCode == Keys.D3 )
                {
                    e.Handled = true;
                    SelectChannelNo = 3;
                }
                else if (e.KeyCode == Keys.D4 )
                {
                    e.Handled = true;
                    SelectChannelNo = 4;
                }
                else if (e.KeyCode == Keys.D5)
                {
                    e.Handled = true;
                    SelectChannelNo = 5;
                }
                else if (e.KeyCode == Keys.D6 )
                {
                    e.Handled = true;
                    SelectChannelNo = 6;
                }
                else if (e.KeyCode == Keys.D7)
                {
                    e.Handled = true;
                    SelectChannelNo = 7;
                }
                else if (e.KeyCode == Keys.D8)
                {
                    e.Handled = true;
                    SelectChannelNo = 8;
                }
            }



            

            if (SelectChannelNo == 0)    //非“Ctrl+数字”则返回
                return;

            if (TabSelectedIndex == 0)      //颜色参数设置子界面
            {
                //MessageBox.Show("Tab:" + TabSelectedIndex.ToString() + " 快捷键:" + SelectChannelNo.ToString());
                TabControlSelectedIndex0Event(SelectChannelNo);
            }
            else if (TabSelectedIndex == 2) //瑕疵参数设置子界面
            {
                //MessageBox.Show("Tab:" + TabSelectedIndex.ToString() + " 快捷键:" + SelectChannelNo.ToString());
                TabControlSelectedIndex2Event(SelectChannelNo);
            }
            else if (TabSelectedIndex == 3) //擦伤参数设置子界面
            {
                //MessageBox.Show("Tab:" + TabSelectedIndex.ToString() + " 快捷键:" + SelectChannelNo.ToString());
                TabControlSelectedIndex3Event(SelectChannelNo);
            }
            else if (TabSelectedIndex == 4) //腐烂参数设置子界面
            {
                //MessageBox.Show("Tab:" + TabSelectedIndex.ToString() + " 快捷键:" + SelectChannelNo.ToString());
                TabControlSelectedIndex4Event(SelectChannelNo);
            }
        }

        private void QualitytabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = int.Parse(PagesIndex[QualitytabControl.SelectedIndex]);

            switch (index)
            {
                case 0:
                    TabSelectedIndex = 0;
                    break;
                case 1:
                    TabSelectedIndex = 1;
                    break;
                case 2:
                    TabSelectedIndex = 2;
                    break;
                case 3:
                    TabSelectedIndex = 3;
                    break;
                case 4:
                    TabSelectedIndex = 4;
                    break;
                case 5:
                    TabSelectedIndex = 5;
                    break;
                case 6:
                    TabSelectedIndex = 6;
                    break;
                case 7:
                    TabSelectedIndex = 7;
                    break;
                case 8:
                    TabSelectedIndex = 8;
                    break;
                case 9:
                    TabSelectedIndex = 9;
                    break;
                case 10:
                    TabSelectedIndex = 10;
                    break;
                case 11:
                    TabSelectedIndex = 11;
                    break;
                case 12:
                    TabSelectedIndex = 12;
                    break;
                case 13:
                    TabSelectedIndex = 13;
                    break;
                default:
                    break;
            }
            //switch(QualitytabControl.SelectedTab.Text)
            //{
            //    case "颜色参数设置":
            //        TabSelectedIndex = 0;
            //        break;
            //    case "形状参数设置":
            //        TabSelectedIndex = 1;
            //        break;
            //    case "瑕疵参数设置":
            //        TabSelectedIndex = 2;
            //        break;
            //    case "擦伤参数设置":
            //        TabSelectedIndex = 3;
            //        break;
            //    case "腐烂参数设置":
            //        TabSelectedIndex = 4;
            //        break;
            //    case "密度参数设置":
            //        TabSelectedIndex = 5;
            //        break;
            //    case "糖度参数设置":
            //        TabSelectedIndex = 6;
            //        break;
            //    case "酸度参数设置":
            //        TabSelectedIndex = 7;
            //        break;
            //    case "参数设置":
            //        TabSelectedIndex = 8;
            //        break;
            //    case "硬度参数设置":
            //        TabSelectedIndex = 9;
            //        break;
            //    case "干物质参数设置":
            //        TabSelectedIndex = 10;
            //        break;
            //    case "褐变参数设置":
            //        TabSelectedIndex = 11;
            //        break;
            //    //case "硬度参数设置":
            //    //    TabSelectedIndex = 12;
            //    //    break;
            //    case "含水参数设置":
            //        TabSelectedIndex = 13;
            //        break;
            //    default:
            //        TabSelectedIndex = 12;
            //        break;
            //}

            //MessageBox.Show("选中: " + TabSelectedIndex.ToString());
        }

        /// <summary>
        /// 颜色参数设置子界面快捷键事件
        /// </summary>
        /// <param name="selectchannelNo"></param>
        private void TabControlSelectedIndex0Event(int selectchannelNo)
        {
            int SelectChannelNo = selectchannelNo;
            if (SelectChannelNo > 0 && SelectChannelNo <= ChannelNum)
            {
                try
                {
                    m_CurrentChannelIndex = SelectChannelNo - 1;
                    if (GlobalDataInterface.nVer == 1)
                    {
                        m_CurrentIPM_ID = m_ChanelIDList[m_CurrentChannelIndex];
                    }
                    else if (GlobalDataInterface.nVer == 0)
                    {
                        m_CurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));
                    }
                    //m_CurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));

                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(m_CurrentIPM_ID, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SINGLE_SAMPLE, null);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("QualityParamSetForm中函数TabControlSelectedIndex1Event出错" + ex + "\n" + ex.StackTrace);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("QualityParamSetForm中函数TabControlSelectedIndex1Event出错" + ex + "\n" + ex.StackTrace);
#endif
                }
            }
        }

        /// <summary>
        /// 瑕疵参数设置子界面快捷键事件
        /// </summary>
        /// <param name="selectchannelNo"></param>
        private void TabControlSelectedIndex2Event(int selectchannelNo)
        {
            int SelectChannelNo = selectchannelNo;
            if (SelectChannelNo > 0 && SelectChannelNo <= ChannelNum)
            {
                try
                {
                    m_CurrentChannelIndex = SelectChannelNo - 1;
                    if (GlobalDataInterface.nVer == 1)
                    {
                        //m_CurrentIPM_ID = Commonfunction.EncodeIPMChannel(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));
                        m_FlawCurrentIPM_ID = m_ChanelIDList[m_CurrentChannelIndex];

                    }
                    else if (GlobalDataInterface.nVer == 0)
                    {
                        m_FlawCurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));
                    }
                    //m_CurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));

                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        //GlobalDataInterface.TransmitParam(m_CurrentIPM_ID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                        //GlobalDataInterface.TransmitParam(m_CurrentIPM_ID, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SINGLE_SAMPLE, null); //新增 Add by ChengSk - 20190508
                        GlobalDataInterface.TransmitParam(m_FlawCurrentIPM_ID, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SINGLE_SAMPLE_SPOT, null);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("QualityParamSetForm中函数TabControlSelectedIndex2Event出错" + ex + "\n" + ex.StackTrace);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("QualityParamSetForm中函数TabControlSelectedIndex2Event出错" + ex + "\n" + ex.StackTrace);
#endif
                }
            }
        }

        /// <summary>
        /// 擦伤参数设置子界面快捷键事件
        /// </summary>
        /// <param name="selectchannelNo"></param>
        private void TabControlSelectedIndex3Event(int selectchannelNo)
        {
            int SelectChannelNo = selectchannelNo;
            if (SelectChannelNo > 0 && SelectChannelNo <= ChannelNum)
            {
                try
                {
                    m_CurrentBruiseChannelIndex = SelectChannelNo - 1;
                    if (GlobalDataInterface.nVer == 1)
                    {
                        //m_BruiseCurrentIPM_ID = Commonfunction.EncodeIPMChannel(Commonfunction.GetSubsysIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]), Commonfunction.GetIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]));
                        //m_CurrentIPM_ID = Commonfunction.EncodeIPMChannel(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));
                        m_BruiseCurrentIPM_ID = m_BruiseChanelIDList[m_CurrentBruiseChannelIndex];
                        m_CurrentIPM_ID = m_ChanelIDList[m_CurrentChannelIndex];
                    }
                    else if (GlobalDataInterface.nVer == 0)
                    {
                        m_BruiseCurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]), Commonfunction.GetIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]));
                        m_CurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));
                    }
                    //m_BruiseCurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]), Commonfunction.GetIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]));
                    //m_CurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_ChanelIDList[m_CurrentChannelIndex]), Commonfunction.GetIPMIndex(m_ChanelIDList[m_CurrentChannelIndex]));

                    stCameraNum cameraNum = new stCameraNum(true);
                    // cameraNum.bChannelIndex = (byte)Commonfunction.ChanelInIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]);
                    //switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo)
                    //{
                    //    case 1:
                    //        cameraNum.cCameraNum = 0;
                    //        break;
                    //    case 2:
                    //        cameraNum.cCameraNum = 0;
                    //        break;
                    //    case 4:
                    //        cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                    //        break;
                    //    case 8:
                    //        cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                    //        break;
                    //}
                    switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo) //Modify by ChengSk - 20190520
                    {
                        case 64:  //NIR2-右
                            cameraNum.cCameraNum = 0;
                            break;
                        case 128: //NIR2-中
                            cameraNum.cCameraNum = 0;
                            break;
                        case 256: //NIR2-左
                            cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                            break;
                        case 1:   //彩色-右
                            cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_BruiseChanelIDList[m_CurrentBruiseChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                            break;
                    }
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(m_BruiseCurrentIPM_ID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                        GlobalDataInterface.TransmitParam(m_BruiseCurrentIPM_ID, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SINGLE_SAMPLE_SPOT, cameraNum);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("QualityParamSetForm中函数TabControlSelectedIndex3Event出错" + ex + "\n" + ex.StackTrace);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("QualityParamSetForm中函数TabControlSelectedIndex3Event出错" + ex + "\n" + ex.StackTrace);
#endif
                }
            }     
        }

        /// <summary>
        /// 腐烂参数设置子界面快捷键事件
        /// </summary>
        /// <param name="selectchannelNo"></param>
        private void TabControlSelectedIndex4Event(int selectchannelNo)
        {
            int SelectChannelNo = selectchannelNo;
            if (SelectChannelNo > 0 && SelectChannelNo <= ChannelNum)
            {
                try
                {
                    m_CurrentRotChannelIndex = SelectChannelNo - 1;
                    //m_RotCurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]), Commonfunction.GetIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]));
                    if (GlobalDataInterface.nVer == 1)            //版本号判断 add by xcw 20200604
                    {
                        m_RotCurrentIPM_ID =m_RotChanelIDList[m_CurrentRotChannelIndex];

                        //m_RotCurrentIPM_ID = Commonfunction.EncodeIPMChannel(Commonfunction.GetSubsysIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]), Commonfunction.GetIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]));
                    }
                    else if (GlobalDataInterface.nVer == 0)
                    {
                        m_RotCurrentIPM_ID = Commonfunction.EncodeIPM(Commonfunction.GetSubsysIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]), Commonfunction.GetIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]));
                    }
                    stCameraNum cameraNum = new stCameraNum(true);
                    // cameraNum.bChannelIndex = (byte)Commonfunction.ChanelInIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]);
                    //switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo)
                    //{
                    //    case 1:
                    //        cameraNum.cCameraNum = 0;
                    //        break;
                    //    case 2:
                    //        cameraNum.cCameraNum = 0;
                    //        break;
                    //    case 4:
                    //        cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                    //        break;
                    //    case 8:
                    //        cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                    //        break;
                    //}
                    switch (GlobalDataInterface.globalOut_SysConfig.nSystemInfo) //Modify by ChengSk - 20190520
                    {
                        case 64:  //NIR2-右
                            cameraNum.cCameraNum = 0;
                            break;
                        case 128: //NIR2-中
                            cameraNum.cCameraNum = 0;
                            break;
                        case 256: //NIR2-左
                            cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                            break;
                        case 1:   //彩色-右
                            cameraNum.cCameraNum = (byte)(Commonfunction.ChanelInIPMIndex(m_RotChanelIDList[m_CurrentRotChannelIndex]) * ConstPreDefine.CHANNEL_NUM);
                            break;
                    }
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(m_RotCurrentIPM_ID, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADEINFO_OFF, null);
                        GlobalDataInterface.TransmitParam(m_RotCurrentIPM_ID, (int)HC_IPM_COMMAND_TYPE.HC_CMD_SINGLE_SAMPLE_SPOT, cameraNum);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("QualityParamSetForm中函数TabControlSelectedIndex4Event出错" + ex + "\n" + ex.StackTrace);
#if REALEASE
                    GlobalDataInterface.WriteErrorInfo("QualityParamSetForm中函数TabControlSelectedIndex4Event出错" + ex + "\n" + ex.StackTrace);
#endif
                }
            }
        }

        private void QualityParamSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                GlobalDataInterface.UpSpliceImageDataEvent -= new GlobalDataInterface.SpliceImageDataEventHandler(OnUpSpliceImageData); //Add by ChengSk - 20180830
                GlobalDataInterface.UpSpliceFlawImageDataEvent -= new GlobalDataInterface.SpliceImageDataEventHandler(OnUpFlawSpliceImageData);//Add by xcw - 20200909
                GlobalDataInterface.UpSpotImageDataEvent -= new GlobalDataInterface.SpotImageDataEventHandler(OnUpSpotImageData);   //Add by ChengSk - 20180830
                GlobalDataInterface.UpBruiseImageDataEvent -= new GlobalDataInterface.BruiseImageDataEventHandler(OnUpBruiseImageData); //Add by ChengSk - 20180830
                GlobalDataInterface.UpRotImageDataEvent -= new GlobalDataInterface.RotImageDataEventHandler(OnUpRotImageData);      //Add by ChengSk - 20180830
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm中函数QualityParamSetForm_FormClosing出错" + ex + "\n" + ex.StackTrace);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm中函数QualityParamSetForm_FormClosing出错" + ex + "\n" + ex.StackTrace);
#endif
            }
        }

     
    }
}
