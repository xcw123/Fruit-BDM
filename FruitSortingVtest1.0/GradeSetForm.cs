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
using System.Runtime.InteropServices;
using ListViewEx;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Resources;

namespace FruitSortingVtest1._0
{
    public partial class GradeSetForm : Form
    {
        //尺寸等级子项目
        [StructLayout(LayoutKind.Sequential)]
        struct stSizeGradeItem
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH)]
            public byte[] strGradeName;
            public int nMinSize;
            public int nFruitNum;

            public stSizeGradeItem(bool IsOK)
            {
                strGradeName = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                nMinSize = 0;
                nFruitNum = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct stSizeGradeInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SIZE_GRADE_NUM)]
            public stSizeGradeItem[] item;
            public int nGradeCnt;
            public int nMaxSize;
            public int nMinSize;

            public stSizeGradeInfo(bool IsOK)
            {
                item = new stSizeGradeItem[ConstPreDefine.MAX_SIZE_GRADE_NUM];
                for (int i = 0; i < ConstPreDefine.MAX_SIZE_GRADE_NUM; i++)
                {
                    item[i] = new stSizeGradeItem(true);
                }
                nGradeCnt = 0;
                nMaxSize = 0;
                nMinSize = 0;
            }
        }

        stGradeInfo tempGradeInfo = new stGradeInfo(true);
        private Control[] GradeEditors;
        int[] m_weightStandard = new int[17]{ 23, 27, 32, 40, 48, 56, 64, 72, 80, 88, 100, 113, 125, 138, 140, 152, 165 };
       // stSizeGradeInfo sizeGradeInfo = new stSizeGradeInfo(true);
        //int m_GradeNum = 0;//等级数量
        float m_PackingWeight1 = 20.0f;
        float m_PackingWeight2 = 15.0f;
        MainForm m_mainForm;
        private ResourceManager m_resourceManager = new ResourceManager(typeof(GradeSetForm));//创建GradeSetForm资源管理

        AutoSizeFormClass asc = new AutoSizeFormClass();//声明大小自适应类实例  
        bool m_GradeSetFormIsInitial = false;
        List<int> m_FruitTypeID = new List<int>();//水果ID
        bool IsneedChangeGradedataGridView = false;
        public GradeSetForm(MainForm mainForm)
        {  
            InitializeComponent();
            m_mainForm = mainForm;
            GlobalDataInterface.UpStatisticInfoEvent += new GlobalDataInterface.StatisticInfoEventHandler(OnUpStatisticInfoEvent); 
        }

        private void GradeSetForm_Load(object sender, EventArgs e)
        {
            try
            {
                IsneedChangeGradedataGridView = false;
                tempGradeInfo.ToCopy(GlobalDataInterface.globalOut_GradeInfo);
                //for (int i = 0; i < ConstPreDefine.MAX_SIZE_GRADE_NUM; i++)
                //{
                //    if(tempGradeInfo.grades[i].nFruitNum =-)
                //}
                //if ((tempGradeInfo.nClassifyType & 0x0020) > 0)//体积
                //{
                //    int qulNum = 1;
                //    if ((tempGradeInfo.nClassifyType & 1) > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)
                //        qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                //    //for (int j = 0; j < qulNum; j++)
                //    //{
                //    //    for (int i = 0; i < ConstPreDefine.MAX_SIZE_GRADE_NUM; i++)
                //    //    {
                //    //        if (tempGradeInfo.grades[j * ConstPreDefine.MAX_SIZE_GRADE_NUM+i].nFruitNum == 0x7f7f7f7f)
                //    //            break;
                //    //        tempGradeInfo.grades[j * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].nMinSize /= 1000;
                //    //        // m_GradeNum++;
                //    //    }
                //    //}
                //}
                //m_GradeNum = GlobalDataInterface.GradeSizeNum;
                //if (m_GradeNum == 0)
                //{
                //    m_GradeNum = 4;//默认4个级别
                //}
                string sWeightStandardConfig = Commonfunction.GetAppSetting("重量等级标准");
                string[] sWeightStandardArray = sWeightStandardConfig.Split(';');//分割具体水果种类
                
                byte[] btempArray = new byte[ConstPreDefine.MAX_FRUIT_TEXT_LENGTH];
                for (int i = 0; i < sWeightStandardArray.Length - 1; i++)
                {
                    m_weightStandard[i] = int.Parse(sWeightStandardArray[i]);
                }

                bool bCheckSize = ((tempGradeInfo.nClassifyType & 0x1C) > 0) && GlobalDataInterface.CIRAvailable;
                bool bCheckWeight = ((tempGradeInfo.nClassifyType & 0x03) > 0) && GlobalDataInterface.WeightAvailable;
                bool enableSize = GlobalDataInterface.CIRAvailable && (!bCheckWeight);
                bool enableWeight = GlobalDataInterface.WeightAvailable && (!bCheckSize);

                this.SizecheckBox.Checked = bCheckSize;
                this.SizecheckBox.Enabled = GlobalDataInterface.CIRAvailable;
                this.DiameterradioButton.Enabled = !bCheckWeight && bCheckSize;
                this.ArearadioButton.Enabled = !bCheckWeight && bCheckSize;
                this.VolumeradioButton.Enabled = !bCheckWeight && bCheckSize;

                this.WeightcheckBox.Checked = bCheckWeight;
                this.WeightcheckBox.Enabled = GlobalDataInterface.WeightAvailable;
                this.GramradioButton.Enabled = !bCheckSize && bCheckWeight;
                this.NumradioButton.Enabled = !bCheckSize && bCheckWeight;

                //只有视觉系统，尺寸必须被选择
                if (GlobalDataInterface.CIRAvailable && (!GlobalDataInterface.WeightAvailable))
                {
                    this.SizecheckBox.Enabled = true;
                    this.DiameterradioButton.Enabled = true;
                    this.ArearadioButton.Enabled = true;
                    this.VolumeradioButton.Enabled = true;
                    this.SizecheckBox.Checked = true;
                    //SetCurClassifyType();
                }
                //只有重量系统，重量必须被选择
                if ((!GlobalDataInterface.CIRAvailable) && GlobalDataInterface.WeightAvailable)
                {
                    this.WeightcheckBox.Checked = true;
                    this.WeightcheckBox.Enabled = true;
                    this.GramradioButton.Enabled = true;
                    this.NumradioButton.Enabled = true;
                    //SetCurClassifyType();
                }

                RefreshRadioButoon();
                InitialGradeSizeList();
                FillGradeSizeData();

                //贴标工作方式
                switch (tempGradeInfo.nLabelType)
                {
                    case 0:
                        this.NoLabelTyperadioButton.Checked = true;
                        this.GradeLabelTyperadioButton.Checked = false;
                        this.ExitLabelTyperadioButton.Checked = false;
                        break;
                    case 1:
                        this.NoLabelTyperadioButton.Checked = false;
                        this.GradeLabelTyperadioButton.Checked = true;
                        this.ExitLabelTyperadioButton.Checked = false;
                        break;
                    case 2:
                        this.NoLabelTyperadioButton.Checked = false;
                        this.GradeLabelTyperadioButton.Checked = false;
                        this.ExitLabelTyperadioButton.Checked = true;
                        break;
                    default: break;
                }

                //等级数量
                this.GradeNumcomboBox.SelectedIndex = tempGradeInfo.nSizeGradeNum - 1;
                if (tempGradeInfo.nClassifyType == 0)
                    this.GradeNumcomboBox.Enabled = false;

                //水果种类
                this.FruitTypescomboBox.Items.Clear();
                string fruittypeconfig = Commonfunction.GetAppSetting("已选水果种类");
                string[] sFruitSelecttArray = fruittypeconfig.Split(';');//分割已选水果种类
                for (int i = 0; i < sFruitSelecttArray.Length - 1; i++)
                {
                    this.FruitTypescomboBox.Items.Add(sFruitSelecttArray[i]);
                    string[] tempArray = sFruitSelecttArray[i].Split(new char[2] { '.', '-' });//分割ID号
                    int ID = (int.Parse(tempArray[0])-1) * ConstPreDefine.MAX_FRUIT_TYPE_SUB_CLASS_NUM+ int.Parse(tempArray[1]);
                   
                    m_FruitTypeID.Add(ID);//加入ID号
                    if (tempGradeInfo.nFruitType == ID)
                    {
                        this.FruitTypescomboBox.SelectedIndex = i;

                        byte[] temp = new byte[ConstPreDefine.MAX_FRUIT_NAME_LENGTH];
                        Array.Copy(temp, 0, tempGradeInfo.strFruitName, 0, ConstPreDefine.MAX_FRUIT_NAME_LENGTH); //清零
                        temp = Encoding.Default.GetBytes(tempArray[2]);  //水果种类
                        Array.Copy(temp, 0, tempGradeInfo.strFruitName, 0, temp.Length); //更新水果种类
                    }
                }

                //出口报警
                this.AlarmThresholdtextBox.Text = Commonfunction.GetAppSetting("出口报警阈值"); //Add by ChengSk - 20180122

                //
                //int HighClassifyType = tempGradeInfo.nFruitType;
                //if (HighClassifyType != 0x7f)
                //{
                //    if (HighClassifyType == 0x1F)
                //    {
                //        this.FruitTypescomboBox.SelectedIndex = this.FruitTypescomboBox.Items.Count - 1;
                //    }
                //    else if (HighClassifyType == 0x1D)
                //    {
                //        this.FruitTypescomboBox.SelectedIndex = this.FruitTypescomboBox.Items.Count - 2;
                //    }
                //    else
                //    {
                //        this.FruitTypescomboBox.SelectedIndex = HighClassifyType;
                //    }
                //}
                
                //贴标名称
                this.LabelNametextBox1.Text = Commonfunction.GetAppSetting("贴标机1");
                this.LabelNametextBox2.Text = Commonfunction.GetAppSetting("贴标机2");
                this.LabelNametextBox3.Text = Commonfunction.GetAppSetting("贴标机3");
                this.LabelNametextBox4.Text = Commonfunction.GetAppSetting("贴标机4");
                if (tempGradeInfo.nLabelType != 0)
                {
                    this.LabelNametextBox1.Enabled = true;
                    this.LabelNametextBox2.Enabled = true;
                    this.LabelNametextBox3.Enabled = true;
                    this.LabelNametextBox4.Enabled = true;
                }
                else
                {
                    this.LabelNametextBox1.Enabled = false;
                    this.LabelNametextBox2.Enabled = false;
                    this.LabelNametextBox3.Enabled = false;
                    this.LabelNametextBox4.Enabled = false;
                }
                GradeEditors = new Control[] { GradeNametextBox, MinnumericUpDown, PackingnumericUpDown };
                //asc.controllInitializeSize(this);
                //if (GlobalDataInterface.gResolutionWidthScale != 1.0f || GlobalDataInterface.gResolutionHeightScale != 1.0f)
                //{
                //    this.Width = (int)(this.Width * GlobalDataInterface.gResolutionWidthScale);
                //    this.Height = (int)(this.Height * GlobalDataInterface.gResolutionHeightScale);
                //    asc.controlAutoSize(this);
                //}
                m_GradeSetFormIsInitial = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数GradeSetForm_Load出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数GradeSetForm_Load出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 初始化等级列表与重量列表
        /// </summary>
        void InitialGradeSizeList()
        {
            try
            {
                this.GradeSizelistViewEx.Clear();
                //创建列
                if ((tempGradeInfo.nClassifyType & 0x03) > 0)//重量
                {
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExName.Text"), 100, HorizontalAlignment.Center);
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExMinWeight.Text"), 300, HorizontalAlignment.Center);
                    if ((tempGradeInfo.nClassifyType & 0x0001) > 0)
                        this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExTargetWeight.Text"), 160, HorizontalAlignment.Center);
                    else
                        this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExTargetNumber.Text"), 160, HorizontalAlignment.Center);
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExWeightPerBox.Text"), 160, HorizontalAlignment.Center);
                }
                else if ((tempGradeInfo.nClassifyType & 0x0004) > 0)//尺寸-》直径
                {
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExName.Text"), 120, HorizontalAlignment.Center);
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExMiniSize.Text"), 300, HorizontalAlignment.Center);
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExTargetNumber.Text"), 300, HorizontalAlignment.Center);
                }
                else if ((tempGradeInfo.nClassifyType & 0x0010) > 0)//尺寸-》体积
                {
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExName.Text"), 120, HorizontalAlignment.Center);
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExMinVolume.Text"), 300, HorizontalAlignment.Center);
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExTargetNumber.Text"), 300, HorizontalAlignment.Center);
                }
                else if ((tempGradeInfo.nClassifyType & 0x0008) > 0)//尺寸-》面积
                {
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExName.Text"), 120, HorizontalAlignment.Center);
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExMinArea.Text"), 300, HorizontalAlignment.Center);
                    this.GradeSizelistViewEx.Columns.Add(m_resourceManager.GetString("GradeSizelistViewExTargetNumber.Text"), 300, HorizontalAlignment.Center);
                }
                //重量列表
                if ((tempGradeInfo.nClassifyType & 0x03) > 0)//重量
                {
                    this.AutoWeightbutton.Visible = true;
                    this.Weightlabel1.Visible = true;
                    this.WeightnumericUpDown1.Visible = true;
                    this.Unitlabel1.Visible = true;
                    this.Weightlabel2.Visible = true;
                    this.WeightnumericUpDown2.Visible = true;
                    this.Unitlabel2.Visible = true;
                    this.GradeWeightlistViewEx.Visible = true;
                    this.GradeSizelistViewEx.Height = 221;
                    this.WeightnumericUpDown1.Text = Commonfunction.GetAppSetting("装箱数1");
                    m_PackingWeight1 = float.Parse(this.WeightnumericUpDown1.Text);
                    this.WeightnumericUpDown2.Text = Commonfunction.GetAppSetting("装箱数2");
                    m_PackingWeight2 = float.Parse(this.WeightnumericUpDown2.Text);
                }
                else
                {
                    this.AutoWeightbutton.Visible = false;
                    this.Weightlabel1.Visible = false;
                    this.WeightnumericUpDown1.Visible = false;
                    this.Unitlabel1.Visible = false;
                    this.Weightlabel2.Visible = false;
                    this.WeightnumericUpDown2.Visible = false;
                    this.Unitlabel2.Visible = false;
                    this.GradeWeightlistViewEx.Visible = false;
                    this.GradeSizelistViewEx.Height = 289;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数InitialGradeSizeList出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数InitialGradeSizeList出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 填充等级列表与重量列表
        /// </summary>
        void FillGradeSizeData()
        {
            try
            {
                string gradeName;
                ListViewItem lvi;
                byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                this.GradeSizelistViewEx.Items.Clear();

                for (int i = 0; i < tempGradeInfo.nSizeGradeNum; i++)
                {
                    if (tempGradeInfo.strSizeGradeName[i * ConstPreDefine.MAX_TEXT_LENGTH] == 0)
                        gradeName = "";
                    else
                    {
                        Array.Copy(tempGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, tempByte, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        gradeName = Encoding.Default.GetString(tempByte).TrimEnd('\0');
                    }
                    lvi = new ListViewItem(gradeName);

                    //if (tempGradeInfo.grades[i].nMinSize == 0x7f7f7f7f)
                    if (tempGradeInfo.grades[i].nMinSize == 0x7f7f7f7f || i == (tempGradeInfo.nSizeGradeNum - 1))
                        tempGradeInfo.grades[i].nMinSize = 0.0f;
                    //新增
                    for (int k = 1; k < tempGradeInfo.nQualityGradeNum; k++)
                    {
                        tempGradeInfo.grades[k * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].nMinSize = tempGradeInfo.grades[i].nMinSize;
                    }
                    lvi.SubItems.Add(tempGradeInfo.grades[i].nMinSize.ToString("0.0"));

                    if (tempGradeInfo.grades[i].nFruitNum == 0x7f7f7f7f)
                        tempGradeInfo.grades[i].nFruitNum = 0;
                    //新增
                    for (int k = 1; k < tempGradeInfo.nQualityGradeNum; k++)
                    {
                        tempGradeInfo.grades[k * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].nFruitNum = tempGradeInfo.grades[i].nFruitNum;
                    }
                    lvi.SubItems.Add(tempGradeInfo.grades[i].nFruitNum.ToString());
                    //if ((tempGradeInfo.nClassifyType & 0x06) > 0)
                    if ((tempGradeInfo.nClassifyType & 0x03) > 0)
                        lvi.SubItems.Add("");
                    this.GradeSizelistViewEx.Items.Add(lvi);
                }

                //if ((tempGradeInfo.nClassifyType & 0x06) > 0)//重量
                //if ((tempGradeInfo.nClassifyType & 0x03) > 0)//重量
                //{
                //    int ColumnsCnt = this.GradeWeightlistViewEx.Columns.Count;
                //    for (int i = ColumnsCnt - 1; i > 0; i--)
                //        this.GradeWeightlistViewEx.Columns.RemoveAt(i);
                //    for (int i = 1; i <= tempGradeInfo.nSizeGradeNum * 2 + 1; i++)
                //    {
                //        this.GradeWeightlistViewEx.Columns.Add(i.ToString(), 40, HorizontalAlignment.Center);
                //        for (int j = 0; j < 6; j++)
                //        {
                //            this.GradeWeightlistViewEx.Items[j].SubItems.Add("");
                //        }
                //        if (i % 2 == 1)
                //        {
                //            this.GradeWeightlistViewEx.Items[0].SubItems[i].Text = m_weightStandard[i / 2].ToString();
                //            this.GradeWeightlistViewEx.Items[3].SubItems[i].Text = m_weightStandard[i / 2].ToString();
                //            this.GradeWeightlistViewEx.Items[1].SubItems[i].Text = ((int)(m_PackingWeight1 * 1000 / m_weightStandard[i / 2])).ToString();
                //            this.GradeWeightlistViewEx.Items[4].SubItems[i].Text = ((int)(m_PackingWeight2 * 1000 / m_weightStandard[i / 2])).ToString();
                //        }
                //    }
                //    int a, b;
                //    for (int i = 1; i < tempGradeInfo.nSizeGradeNum * 2 + 1; i++)
                //    {
                //        if (i % 2 == 0)
                //        {
                //            a = int.Parse(this.GradeWeightlistViewEx.Items[1].SubItems[i - 1].Text);
                //            b = int.Parse(this.GradeWeightlistViewEx.Items[1].SubItems[i + 1].Text);
                //            this.GradeWeightlistViewEx.Items[2].SubItems[i].Text = ((a + b) / 2).ToString();
                //            a = int.Parse(this.GradeWeightlistViewEx.Items[4].SubItems[i - 1].Text);
                //            b = int.Parse(this.GradeWeightlistViewEx.Items[4].SubItems[i + 1].Text);
                //            this.GradeWeightlistViewEx.Items[5].SubItems[i].Text = ((a + b) / 2).ToString();
                //        }
                //    }

                //}

                //增加一位小数
                if ((tempGradeInfo.nClassifyType & 0x03) > 0)//重量
                {
                    int ColumnsCnt = this.GradeWeightlistViewEx.Columns.Count;
                    for (int i = ColumnsCnt - 1; i > 0; i--)
                        this.GradeWeightlistViewEx.Columns.RemoveAt(i);
                    for (int i = 1; i <= tempGradeInfo.nSizeGradeNum * 2 + 1; i++)
                    {
                        this.GradeWeightlistViewEx.Columns.Add(i.ToString(), 42, HorizontalAlignment.Center);
                        for (int j = 0; j < 6; j++)
                        {
                            this.GradeWeightlistViewEx.Items[j].SubItems.Add("");
                        }
                        if (i % 2 == 1)
                        {
                            this.GradeWeightlistViewEx.Items[0].SubItems[i].Text = m_weightStandard[i / 2].ToString();
                            this.GradeWeightlistViewEx.Items[3].SubItems[i].Text = m_weightStandard[i / 2].ToString();
                            this.GradeWeightlistViewEx.Items[1].SubItems[i].Text = (m_PackingWeight1 * 1000 / m_weightStandard[i / 2]).ToString("0.0");
                            this.GradeWeightlistViewEx.Items[4].SubItems[i].Text = (m_PackingWeight2 * 1000 / m_weightStandard[i / 2]).ToString("0.0");
                        }
                    }
                    float a, b;
                    for (int i = 1; i < tempGradeInfo.nSizeGradeNum * 2 + 1; i++)
                    {
                        if (i % 2 == 0)
                        {
                            a = float.Parse(this.GradeWeightlistViewEx.Items[1].SubItems[i - 1].Text);
                            b = float.Parse(this.GradeWeightlistViewEx.Items[1].SubItems[i + 1].Text);
                            this.GradeWeightlistViewEx.Items[2].SubItems[i].Text = ((a + b) / 2).ToString("0.0");
                            a = float.Parse(this.GradeWeightlistViewEx.Items[4].SubItems[i - 1].Text);
                            b = float.Parse(this.GradeWeightlistViewEx.Items[4].SubItems[i + 1].Text);
                            this.GradeWeightlistViewEx.Items[5].SubItems[i].Text = ((a + b) / 2).ToString("0.0");
                        }
                    }
                    this.GradeWeightlistViewEx.Scrollable = true;
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数FillGradeSizeData出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数FillGradeSizeData出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 刷新RadioButoon
        /// </summary>
        private void RefreshRadioButoon()
        {
            try
            {
                bool bCheckSize = ((tempGradeInfo.nClassifyType & 0x1C) > 0);
                bool bCheckWeight = ((tempGradeInfo.nClassifyType & 0x03) > 0);

                if (bCheckSize)
                {
                    if ((tempGradeInfo.nClassifyType & 0x0010) == 0x0010)//体积
                    {
                        this.DiameterradioButton.Checked = false;
                        this.ArearadioButton.Checked = false;
                        this.VolumeradioButton.Checked = true;
                    }
                    else if ((tempGradeInfo.nClassifyType & 0x0008) == 0x0008)//面积
                    {
                        this.DiameterradioButton.Checked = false;
                        this.ArearadioButton.Checked = true;
                        this.VolumeradioButton.Checked = false;
                    }
                    else //直径
                    {
                        this.DiameterradioButton.Checked = true;
                        this.ArearadioButton.Checked = false;
                        this.VolumeradioButton.Checked = false;
                    }
                }
                if (bCheckWeight)
                {
                    if ((tempGradeInfo.nClassifyType & 0x0002) == 0x0002)//个
                    {
                        this.GramradioButton.Checked = false;
                        this.NumradioButton.Checked = true;
                    }
                    else
                    {
                        this.GramradioButton.Checked = true;
                        this.NumradioButton.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数RefreshRadioButoon出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数RefreshRadioButoon出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 品质
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QualitycheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                SetCurClassifyType();
                InitialGradeSizeList();
                FillGradeSizeData();
                RefreshRadioButoon();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数QualitycheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数QualitycheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 重新设置等级类型
        /// </summary>
        /// <returns></returns>
        private void SetCurClassifyType()
        {
            try
            {
                tempGradeInfo.nClassifyType = 0;
                bool bCheckSize = this.SizecheckBox.Checked; ;
                bool bCheckWeight = this.WeightcheckBox.Checked;
                if (bCheckSize)//尺寸
                {
                    if (this.DiameterradioButton.Checked)//直径
                        tempGradeInfo.nClassifyType |= 4;
                    else if (this.ArearadioButton.Checked)//面积
                        tempGradeInfo.nClassifyType |= 8;
                    else//体积
                        tempGradeInfo.nClassifyType |= 16;
                }
                if (bCheckWeight)
                {
                    if (this.GramradioButton.Checked)
                        tempGradeInfo.nClassifyType |= 1;
                    else
                        tempGradeInfo.nClassifyType |= 2;
                }
                if (tempGradeInfo.nClassifyType == 0)
                    this.GradeNumcomboBox.Enabled = false;
                else
                    this.GradeNumcomboBox.Enabled = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数SetCurClassifyType出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数SetCurClassifyType出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 尺寸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SizecheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.DiameterradioButton.Enabled = true;
                    this.ArearadioButton.Enabled = true;
                    this.VolumeradioButton.Enabled = true;
                    this.DiameterradioButton.Checked = true;
                    if (GlobalDataInterface.WeightAvailable)
                    {
                        this.WeightcheckBox.Checked = false;
                        this.GramradioButton.Enabled = false;
                        this.NumradioButton.Enabled = false;
                    }

                }
                else
                {
                    //只有视觉系统，尺寸必须被选择
                    //if ((!GlobalDataInterface.WeightAvailable) && GlobalDataInterface.CIRAvailable)
                    //{
                    //    checkBox.Checked = true;
                    //    return;
                    //}
                    this.DiameterradioButton.Enabled = false;
                    this.ArearadioButton.Enabled = false;
                    this.VolumeradioButton.Enabled = false;
                    //if (GlobalDataInterface.WeightAvailable)
                    //{
                    //    this.WeightcheckBox.Enabled = true;
                    //    this.WeightcheckBox.Checked = true;
                    //    this.GramradioButton.Enabled = true;
                    //    this.NumradioButton.Enabled = true;
                    //}
                }
                //asc.controllInitializeSize(this);
                SetCurClassifyType();
                InitialGradeSizeList();
                FillGradeSizeData();
                RefreshRadioButoon();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数SizecheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数SizecheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 直径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiameterradioButton_Click(object sender, EventArgs e)
        {
            try
            {
                SetCurClassifyType();
                InitialGradeSizeList();
                FillGradeSizeData();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数DiameterradioButton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数DiameterradioButton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 面积
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArearadioButton_Click(object sender, EventArgs e)
        {
            try
            {
                SetCurClassifyType();
                InitialGradeSizeList();
                FillGradeSizeData();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数ArearadioButton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数ArearadioButton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 体积
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolumeradioButton_Click(object sender, EventArgs e)
        {
            try
            {
                SetCurClassifyType();
                InitialGradeSizeList();
                FillGradeSizeData();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数VolumeradioButton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数VolumeradioButton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 克
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GramradioButton_Click(object sender, EventArgs e)
        {
            try
            {
                SetCurClassifyType();
                InitialGradeSizeList();
                FillGradeSizeData();
                
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数GramradioButton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数GramradioButton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumradioButton_Click(object sender, EventArgs e)
        {
            try
            {
                SetCurClassifyType();
                InitialGradeSizeList();
                FillGradeSizeData();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数NumradioButton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数NumradioButton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 不贴标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoLabelTyperadioButton_Click(object sender, EventArgs e)
        {
            try
            {
                RadioButton radioButton = (RadioButton)sender;
                if (radioButton.Checked)
                {
                    this.LabelNametextBox1.Enabled = false;
                    this.LabelNametextBox2.Enabled = false;
                    this.LabelNametextBox3.Enabled = false;
                    this.LabelNametextBox4.Enabled = false;
                    tempGradeInfo.nLabelType = 0;
                }
                else
                {
                    this.LabelNametextBox1.Enabled = true;
                    this.LabelNametextBox2.Enabled = true;
                    this.LabelNametextBox3.Enabled = true;
                    this.LabelNametextBox4.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数NoLabelTyperadioButton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数NoLabelTyperadioButton_Click出错" + ex);
#endif
            }

        }

        /// <summary>
        /// 按等级方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GradeLabelTyperadioButton_Click(object sender, EventArgs e)
        {
            try
            {
                RadioButton radioButton = (RadioButton)sender;
                if (radioButton.Checked)
                {
                    this.LabelNametextBox1.Enabled = true;
                    this.LabelNametextBox2.Enabled = true;
                    this.LabelNametextBox3.Enabled = true;
                    this.LabelNametextBox4.Enabled = true;
                    tempGradeInfo.nLabelType = 1;
                }
                else
                {
                    this.LabelNametextBox1.Enabled = false;
                    this.LabelNametextBox2.Enabled = false;
                    this.LabelNametextBox3.Enabled = false;
                    this.LabelNametextBox4.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数GradeLabelTyperadioButton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数GradeLabelTyperadioButton_Click出错" + ex);
#endif
            }
        }


        /// <summary>
        /// 按出口方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitLabelTyperadioButton_Click(object sender, EventArgs e)
        {
            try
            {
                RadioButton radioButton = (RadioButton)sender;
                if (radioButton.Checked)
                {
                    this.LabelNametextBox1.Enabled = true;
                    this.LabelNametextBox2.Enabled = true;
                    this.LabelNametextBox3.Enabled = true;
                    this.LabelNametextBox4.Enabled = true;
                    tempGradeInfo.nLabelType = 2;
                }
                else
                {
                    this.LabelNametextBox1.Enabled = false;
                    this.LabelNametextBox2.Enabled = false;
                    this.LabelNametextBox3.Enabled = false;
                    this.LabelNametextBox4.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数ExitLabelTyperadioButton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数ExitLabelTyperadioButton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 等级数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GradeNumcomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                if (tempGradeInfo.nSizeGradeNum!=this.GradeNumcomboBox.SelectedIndex + 1)
                {
                    IsneedChangeGradedataGridView = true;
                    tempGradeInfo.nSizeGradeNum = (byte)(this.GradeNumcomboBox.SelectedIndex + 1);
                
                    FillGradeSizeData();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数GradeNumcomboBox_SelectionChangeCommitted出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数GradeNumcomboBox_SelectionChangeCommitted出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 等级列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GradeSizelistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.SubItem == 1 && e.Item.Index == tempGradeInfo.nSizeGradeNum - 1) && e.SubItem != 3)
                    this.GradeSizelistViewEx.StartEditing(GradeEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数GradeSizelistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数GradeSizelistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 等级列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GradeSizelistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                ListViewEx.ListViewEx listviewex = (ListViewEx.ListViewEx)sender;
                byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                int qulNum = 1;
                switch (e.SubItem)
                {
                    case 0://等级名称
                        Array.Copy(tempByte, 0, tempGradeInfo.strSizeGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, tempByte.Length);
                        tempByte = Encoding.Default.GetBytes(e.DisplayText);
                        Array.Copy(tempByte, 0, tempGradeInfo.strSizeGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, tempByte.Length);
                        IsneedChangeGradedataGridView = true;
                        break;
                    case 1:
                        //if ((tempGradeInfo.nClassifyType & 1) > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//有品质
                        if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//有品质
                        {
                            qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                        }
                        for (int i = 0; i < qulNum; i++)
                            tempGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + e.Item.Index].nMinSize = float.Parse(e.DisplayText);
                        break;
                    case 2:
                        //if ((tempGradeInfo.nClassifyType & 1) > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//有品质
                        if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//有品质
                        {
                            qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                        }
                        for (int i = 0; i < qulNum; i++)
                            tempGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + e.Item.Index].nFruitNum = int.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数GradeSizelistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数GradeSizelistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        
        /// <summary>
        /// 重量列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GradeWeightlistViewEx_SubItemClicked(object sender, SubItemEventArgs e)
        {
            try
            {
                if ((e.SubItem % 2 == 1) && (e.Item.Index == 0 || e.Item.Index == 3))
                    this.GradeWeightlistViewEx.StartEditing(GradeEditors[2], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数GradeWeightlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数GradeWeightlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 重量列表完成编辑事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GradeWeightlistViewEx_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        {
            try
            {
                ListViewEx.ListViewEx listviewex = (ListViewEx.ListViewEx)sender;
                byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                float a, b;
                switch (e.Item.Index)
                {
                    case 0:
                        if (e.SubItem % 2 == 1)
                        {
                            a = m_PackingWeight1 * 1000.0f / int.Parse(e.DisplayText);
                            this.GradeWeightlistViewEx.Items[1].SubItems[e.SubItem].Text = a.ToString("0.0");
                            if (e.SubItem == 1)
                            {
                                b = float.Parse(this.GradeWeightlistViewEx.Items[1].SubItems[e.SubItem + 2].Text);
                                this.GradeWeightlistViewEx.Items[2].SubItems[e.SubItem + 1].Text = ((a + b) / 2).ToString("0.0");
                            }
                            else if (e.SubItem > 1 && e.SubItem < tempGradeInfo.nSizeGradeNum)
                            {
                                b = float.Parse(this.GradeWeightlistViewEx.Items[1].SubItems[e.SubItem + 2].Text);
                                this.GradeWeightlistViewEx.Items[2].SubItems[e.SubItem + 1].Text = ((a + b) / 2).ToString("0.0");
                                b = float.Parse(this.GradeWeightlistViewEx.Items[1].SubItems[e.SubItem - 2].Text);
                                this.GradeWeightlistViewEx.Items[2].SubItems[e.SubItem - 1].Text = ((a + b) / 2).ToString("0.0");
                            }
                            else
                            {
                                b = float.Parse(this.GradeWeightlistViewEx.Items[1].SubItems[e.SubItem - 2].Text);
                                this.GradeWeightlistViewEx.Items[2].SubItems[e.SubItem - 1].Text = ((a + b) / 2).ToString("0.0");
                            }
                        }
                        m_weightStandard[e.SubItem / 2] = int.Parse(e.DisplayText);
                        break;
                    case 3:
                        if (e.SubItem % 2 == 1)
                        {
                            a = m_PackingWeight2 * 1000.0f / int.Parse(e.DisplayText);
                            this.GradeWeightlistViewEx.Items[4].SubItems[e.SubItem].Text = a.ToString("0.0");
                            if (e.SubItem == 1)
                            {
                                b = float.Parse(this.GradeWeightlistViewEx.Items[4].SubItems[e.SubItem + 2].Text);
                                this.GradeWeightlistViewEx.Items[5].SubItems[e.SubItem + 1].Text = ((a + b) / 2).ToString("0.0");
                            }
                            else if (e.SubItem > 1 && e.SubItem < tempGradeInfo.nSizeGradeNum)
                            {
                                b = float.Parse(this.GradeWeightlistViewEx.Items[4].SubItems[e.SubItem + 2].Text);
                                this.GradeWeightlistViewEx.Items[5].SubItems[e.SubItem + 1].Text = ((a + b) / 2).ToString("0.0");
                                b = float.Parse(this.GradeWeightlistViewEx.Items[4].SubItems[e.SubItem - 2].Text);
                                this.GradeWeightlistViewEx.Items[5].SubItems[e.SubItem - 1].Text = ((a + b) / 2).ToString("0.0");
                            }
                            else
                            {
                                b = float.Parse(this.GradeWeightlistViewEx.Items[4].SubItems[e.SubItem - 2].Text);
                                this.GradeWeightlistViewEx.Items[5].SubItems[e.SubItem - 1].Text = ((a + b) / 2).ToString("0.0");
                            }
                        }
                        m_weightStandard[e.SubItem / 2] = int.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数GradeWeightlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数GradeWeightlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 重量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeightcheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.GramradioButton.Enabled = true;
                    this.NumradioButton.Enabled = true;
                    if (GlobalDataInterface.CIRAvailable)
                    {
                        this.SizecheckBox.Checked = false;
                        this.SizecheckBox.Enabled = true;
                        this.DiameterradioButton.Enabled = false;
                        this.ArearadioButton.Enabled = false;
                        this.VolumeradioButton.Enabled = false;
                    }

                }
                else
                {
                    //只有重量系统，重量必须被选择
                    //if ((!GlobalDataInterface.CIRAvailable) && GlobalDataInterface.WeightAvailable)
                    //{
                    //    checkBox.Checked = true;
                    //    return;
                    //}
                    this.GramradioButton.Enabled = false;
                    this.NumradioButton.Enabled = false;
                    //if (GlobalDataInterface.CIRAvailable)
                    //{
                    //    this.SizecheckBox.Checked = true;
                    //    this.SizecheckBox.Enabled = true;
                    //    this.DiameterradioButton.Enabled = true;
                    //    this.ArearadioButton.Enabled = true;
                    //    this.VolumeradioButton.Enabled = true;
                    //}
                }
                SetCurClassifyType();
                InitialGradeSizeList();
                FillGradeSizeData();
                RefreshRadioButoon();
                asc.controllInitializeSize(this);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数WeightcheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数WeightcheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 装箱重量1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeightnumericUpDown1_Validated(object sender, EventArgs e)
        {
            try
            {
                NumericUpDown numericUpDown = (NumericUpDown)sender;
                m_PackingWeight1 = float.Parse(numericUpDown.Text);
                FillGradeSizeData();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数WeightnumericUpDown1_Validated出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数WeightnumericUpDown1_Validated出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 装箱重量2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeightnumericUpDown2_Validated(object sender, EventArgs e)
        {
            try
            {
                NumericUpDown numericUpDown = (NumericUpDown)sender;
                m_PackingWeight2 = float.Parse(numericUpDown.Text);
                FillGradeSizeData();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数WeightnumericUpDown2_Validated出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数WeightnumericUpDown2_Validated出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 自动生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoWeightbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (tempGradeInfo.nSizeGradeNum > 0)
                {
                    IsneedChangeGradedataGridView = true;
                    FillGradeSizeData();
                    
                    int qulNum = 1;
                    //if ((tempGradeInfo.nClassifyType & 1) > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//有品质
                    if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//有品质
                    {
                        qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                    }
                    for (int i = 0; i < tempGradeInfo.nSizeGradeNum; i++)
                    {
                        byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        this.GradeSizelistViewEx.Items[i].SubItems[0].Text = this.GradeWeightlistViewEx.Items[0].SubItems[i * 2 + 1].Text;
                        Array.Copy(tempByte, 0, tempGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, tempByte.Length);
                        tempByte = Encoding.Default.GetBytes(this.GradeSizelistViewEx.Items[i].SubItems[0].Text);
                        Array.Copy(tempByte, 0, tempGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, tempByte.Length);
                        if (i != tempGradeInfo.nSizeGradeNum - 1)
                        {
                            this.GradeSizelistViewEx.Items[i].SubItems[1].Text = this.GradeWeightlistViewEx.Items[2].SubItems[i * 2 + 2].Text;
                            for (int j = 0; j < qulNum; j++)
                                tempGradeInfo.grades[j * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].nMinSize = float.Parse(this.GradeSizelistViewEx.Items[i].SubItems[1].Text);
                        }
                        else
                        {
                            this.GradeSizelistViewEx.Items[i].SubItems[1].Text = "0.0";
                            for (int j = 0; j < qulNum; j++)
                                tempGradeInfo.grades[j * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].nMinSize = 0.0f;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数AutoWeightbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数AutoWeightbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private bool SaveConfig()
        {
            try
            {
                if (!VerifyData())
                    return false;
                if (tempGradeInfo.nClassifyType == 0)
                    return false;

                if (GlobalDataInterface.CIRAvailable && GlobalDataInterface.WeightAvailable)
                {
                    if (!(this.SizecheckBox.Checked || this.WeightcheckBox.Checked))
                    {
                        //MessageBox.Show("请选择分选标准(尺寸/重量)");
                        //MessageBox.Show("0x30001009 Please select the sorting standard(size/weight)!","Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        MessageBox.Show("0x30001009 " + LanguageContainer.GradeSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.GradeSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }

                //水果种类
                //byte HighClassifyType = 0;
                //switch (this.FruitTypescomboBox.Text.Trim())
                //{
                //    case "梨":
                //        HighClassifyType = 0;
                //        break;
                //    case "红枣":
                //        HighClassifyType = 1;
                //        break;
                //    case "蜜桔":
                //        HighClassifyType = 2;
                //        break;
                //    case "蜜柚":
                //        HighClassifyType = 3;
                //        break;
                //    case "柠檬":
                //        HighClassifyType = 4;
                //        break;
                //    case "苹果":
                //        HighClassifyType = 5;
                //        break;
                //    case "脐橙":
                //        HighClassifyType = 6;
                //        break;
                //    case "石榴":
                //        HighClassifyType = 7;
                //        break;
                //    case "山竹":
                //        HighClassifyType = 8;
                //        break;
                //    case "土豆":
                //        HighClassifyType = 9;
                //        break;
                //    case "甜椒":
                //        HighClassifyType = 10;
                //        break;
                //    case "甜柿":
                //        HighClassifyType = 11;
                //        break;
                //    case "桃子":
                //        HighClassifyType = 12;
                //        break;
                //    case "洋葱":
                //        HighClassifyType = 13;
                //        break;
                //    case "猕猴桃":
                //        HighClassifyType = 14;
                //        break;
                //    case "圣女果":
                //        HighClassifyType = 15;
                //        break;
                //    case "西红柿":
                //        HighClassifyType = 16;
                //        break;
                //    case "椭圆形水果":
                //        HighClassifyType = 30;
                //        break;
                //    case "扁圆形水果":
                //        HighClassifyType = 31;
                //        break;
                //    default:
                //        break;
                //}
                //if (this.FruitTypescomboBox.Text.Trim() != "")
                //{
                //    if (this.FruitTypescomboBox.SelectedIndex == (this.FruitTypescomboBox.Items.Count -1))
                //    {
                //        HighClassifyType = 0x1F;
                //    }
                //    else if (this.FruitTypescomboBox.SelectedIndex == (this.FruitTypescomboBox.Items.Count - 2))
                //    {
                //        HighClassifyType = 0x1E;
                //    }
                //    else
                //    {
                //        HighClassifyType = (byte)this.FruitTypescomboBox.SelectedIndex;
                //    }
                //}
                //else
                //{
                //    HighClassifyType = 0x7F;
                //}
                tempGradeInfo.nFruitType = m_FruitTypeID[this.FruitTypescomboBox.SelectedIndex];
                GlobalDataInterface.fAlarmRatioThreshold = ((float)int.Parse(this.AlarmThresholdtextBox.Text)) / 100;
                Commonfunction.SetAppSetting("出口报警阈值", this.AlarmThresholdtextBox.Text);

                string sWeightStandardConfig = "";
                for(int i = 0;i < m_weightStandard.Length;i++)
                {
                    sWeightStandardConfig += m_weightStandard[i].ToString().TrimEnd('\0') + ";";
                }
                Commonfunction.SetAppSetting("重量等级标准", sWeightStandardConfig);

                int qulNum = 1;
                if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)
                    qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                //if ((tempGradeInfo.nClassifyType & 0x0020)>0)//尺寸-》体积
                //{
                //    for (int j = 0; j < qulNum; j++)
                //    {
                //        for (int i = 0; i < m_GradeNum; i++)
                //        {
                //            tempGradeInfo.grades[j * m_GradeNum+i].nMinSize *= 1000; ///有疑问
                //        }
                //    }
                //}
                //最大最小值
                for (int j = 0; j < qulNum; j++)
                {
                    tempGradeInfo.grades[j * ConstPreDefine.MAX_SIZE_GRADE_NUM + tempGradeInfo.nSizeGradeNum - 1].nMinSize = 0;
                    tempGradeInfo.grades[j * tempGradeInfo.nSizeGradeNum].nMaxSize = 1000000;
                }
                for (int i = 0; i < qulNum; i++)
                {
                    for (int j = 1; j < tempGradeInfo.nSizeGradeNum; j++)
                    {
                        tempGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nMaxSize = tempGradeInfo.grades[i * ConstPreDefine.MAX_QUALITY_GRADE_NUM + j - 1].nMinSize;

                    }
                }


                //等级数量改变 出口等级应清零
                if (tempGradeInfo.nSizeGradeNum != GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum)
                {
                    for (int i = 0; i < ConstPreDefine.MAX_QUALITY_GRADE_NUM; i++)
                    {
                        for (int j = 0; j < ConstPreDefine.MAX_SIZE_GRADE_NUM; j++)
                        {
                            tempGradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit = 0;
                        }
                    }
                }

                //装箱数
                Commonfunction.SetAppSetting("装箱数1", this.WeightnumericUpDown1.Text);
                Commonfunction.SetAppSetting("装箱数2", this.WeightnumericUpDown2.Text);

                //贴标名称
                Commonfunction.SetAppSetting("贴标机1", this.LabelNametextBox1.Text);
                Commonfunction.SetAppSetting("贴标机2", this.LabelNametextBox2.Text);
                Commonfunction.SetAppSetting("贴标机3", this.LabelNametextBox3.Text);
                Commonfunction.SetAppSetting("贴标机4", this.LabelNametextBox4.Text);
                //ivycc 2013-11-4
                //if (((tempGradeInfo.nClassifyType & 1) == 0)&&((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 1)>0))//去掉品质 品质等级清零
                //{
                //    GlobalDataInterface.Quality_GradeInfo = new QualityGradeInfo(true);
                //    tempGradeInfo.nQualityGradeNum = 0;
                //    tempGradeInfo.strQualityGradeName = new byte[ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
                //}
                //ivycc 2013-11-4 end
                if (!GlobalDataInterface.globalOut_GradeInfo.IsEqual(tempGradeInfo))
                {
                    m_mainForm.SetSeparationProgrameChangelabel(false,null);
                }
                GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName = new byte[ConstPreDefine.MAX_SIZE_GRADE_NUM* ConstPreDefine.MAX_TEXT_LENGTH];
                GlobalDataInterface.globalOut_GradeInfo.ToCopy(tempGradeInfo);
                //GlobalDataInterface.GradeSizeNum = m_GradeNum;

                if (GlobalDataInterface.global_IsTestMode)
                {
                    //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                    int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                    if (global_IsTest != 0) //add by xcw 20201211
                    {
                        MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                        LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数SaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数SaveConfig出错" + ex);
#endif
                return false;
            }
        }

        /// <summary>
        /// 验证等级列表内容是否有效
        /// </summary>
        /// <returns></returns>
        private bool VerifyData()
        {
            try
            {
                float preval = tempGradeInfo.grades[0].nMinSize;
                float curval = 0;
                for (int i = 0; i < tempGradeInfo.nSizeGradeNum; i++)
                {
                    byte[] tempByte1 = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    byte[] tempByte2 = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    if (tempGradeInfo.strSizeGradeName[i * ConstPreDefine.MAX_TEXT_LENGTH] == 0)
                    {
                        //MessageBox.Show(string.Format("第{0}行等级名称不能为空！", i+1));
                        //MessageBox.Show(string.Format("0x3000100A Row{0}'s grade name cannot be empty!", i + 1),"Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        MessageBox.Show(string.Format("0x3000100A " + LanguageContainer.GradeSetFormMessagebox7Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                            LanguageContainer.GradeSetFormMessagebox7Sub2Text[GlobalDataInterface.selectLanguageIndex],
                            i + 1),
                            LanguageContainer.GradeSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    Array.Copy(tempGradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, tempByte1, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                    for (int j = i + 1; j < tempGradeInfo.nSizeGradeNum; j++)
                    {
                        Array.Copy(tempGradeInfo.strSizeGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, tempByte2, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        if (String.Compare(Encoding.Default.GetString(tempByte1), Encoding.Default.GetString(tempByte2)) == 0)
                        {
                            //MessageBox.Show(string.Format("第{0}行与第{1}行等级名称重名！", i+1, j+1));
                            //MessageBox.Show(string.Format("0x3000100B Row{0}'s grade name is same as the Row{1}s'!", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x3000100B " + LanguageContainer.GradeSetFormMessagebox7Sub3Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.GradeSetFormMessagebox7Sub4Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.GradeSetFormMessagebox7Sub5Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1, j + 1),
                                LanguageContainer.GradeSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                    if (i != 0)
                    {
                        curval = tempGradeInfo.grades[i].nMinSize;

                        if (curval >= preval || curval < 0)
                        {
                            //MessageBox.Show(string.Format("等级列表中第{0}行为无效数值！", i+1));
                            //MessageBox.Show(string.Format("0x3000100C Row{0} has an invalid value!", i + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x3000100C " + LanguageContainer.GradeSetFormMessagebox7Sub6Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.GradeSetFormMessagebox7Sub7Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1),
                                LanguageContainer.GradeSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        preval = curval;
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数VerifyData出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数VerifyData出错" + ex);
#endif
                return false;
            }
        }

        /// <summary>
        /// 上行水果统计信息刷新
        /// </summary>
        /// <param name="statistic"></param>
        private void OnUpStatisticInfoEvent()
        {
            try
            {
                if (this == Form.ActiveForm)//是否操作当前窗体
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new GlobalDataInterface.StatisticInfoEventHandler(OnUpStatisticInfoEvent));
                    }
                    else
                    {
                        if (tempGradeInfo.nSizeGradeNum > 0)
                        {
                            if (this.GradeSizelistViewEx.Columns.Count == 4)//重量
                            {
                                for (int i = 0; i < tempGradeInfo.nSizeGradeNum; i++)
                                {
                                    int Sum = 0;
                                    for (int k = 0; k < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; k++)
                                    {
                                        Sum += GlobalDataInterface.globalIn_statistics[k].nBoxGradeWeight[i];
                                    }
                                    //this.GradeSizelistViewEx.Items[i].SubItems[3].Text = Sum.ToString();//有疑问
                                    this.GradeSizelistViewEx.Items[i].SubItems[3].Text = "";//有疑问
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数OnUpStatisticInfoEvent出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数OnUpStatisticInfoEvent出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SaveConfig())
                    return;
                //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 1) == 0 || GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//只有尺寸或重量
                if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0 || GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum == 0)//只有尺寸或重量
                {
                    
                        GlobalDataInterface.gGradeInterfaceFresh = false;
                       //IAsyncResult result= this.BeginInvoke(new GlobalDataInterface.GradeInterfaceFreshEvent(m_mainForm.SetGradedataGridViewInfo));
                        if (IsneedChangeGradedataGridView)
                            m_mainForm.SetGradedataGridViewInfo();
                        m_mainForm.SetGradeSizelistViewEx();
                        m_mainForm.SetAllExitListBox();
                        GlobalDataInterface.gGradeInterfaceFresh = true;
                    
                }
                m_mainForm.SetMainstatusStrip();
                m_mainForm.SetQaulitytoolStripButtonEnabled();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函数OKbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数OKbutton_Click出错" + ex);
#endif
            }

        }

        /// <summary>
        /// 尺寸变化响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GradeSetForm_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if(m_GradeSetFormIsInitial)
                    asc.controlAutoSize(this);//自适应
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函GradeSetForm_SizeChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数GradeSetForm_SizeChanged出错" + ex);
#endif
            }
        }

        private void GradeSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                GlobalDataInterface.UpStatisticInfoEvent -= new GlobalDataInterface.StatisticInfoEventHandler(OnUpStatisticInfoEvent); //Add by ChengSk - 20180830
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函GradeSetForm_FormClosing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数GradeSetForm_FormClosing出错" + ex);
#endif
            }
        }

        private void FruitTypescomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;
                string strContent = comboBox.Text.Trim();
                //string[] strFruitNameSplits = strContent.Split('-');
                string[] strFruitNameSplits = strContent.Split(new char[2] { '.', '-' });
                tempGradeInfo.nFruitType = (int.Parse(strFruitNameSplits[0])-1)* ConstPreDefine.MAX_FRUIT_TYPE_SUB_CLASS_NUM+ int.Parse(strFruitNameSplits[1]);

                byte[] temp = new byte[ConstPreDefine.MAX_FRUIT_NAME_LENGTH];
                Array.Copy(temp, 0, tempGradeInfo.strFruitName, 0, ConstPreDefine.MAX_FRUIT_NAME_LENGTH); //清零
                temp = Encoding.Default.GetBytes(strFruitNameSplits[2]);
                Array.Copy(temp, 0, tempGradeInfo.strFruitName, 0, temp.Length);  //更新水果种类
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GradeSetForm中函FruitTypescomboBox_SelectedIndexChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("GradeSetForm中函数FruitTypescomboBox_SelectedIndexChanged出错" + ex);
#endif
            }
        }
        
    }
}
