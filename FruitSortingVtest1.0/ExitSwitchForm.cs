using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using Interface;
using System.Diagnostics;
using System.Resources;

namespace FruitSortingVtest1._0
{
    public partial class ExitSwitchForm : Form
    {
        bool m_bSwitchNumber, m_bSwitchWeight, m_bSwitchVolume;
        int m_nContorlIndex = 0;
        int m_nCurrentExitIndex = 0;
        byte m_nLabel = 0;
        bool[,] m_bIsExitGrade = new bool[ConstPreDefine.MAX_SIZE_GRADE_NUM, ConstPreDefine.MAX_QUALITY_GRADE_NUM];
        MainForm m_mainForm;
        private ResourceManager m_resourceManager = new ResourceManager(typeof(ExitSwitchForm));//创建ExitSwitchForm资源管理
        stMotorInfo tempMotorInfo = new stMotorInfo(true);
        
        public ExitSwitchForm(MainForm mainForm, int ContorlIndex, int ExitIndex)
        {
            InitializeComponent();
            this.Text = string.Format(m_resourceManager.GetString("Outletlabel.Text") + "-{0} " + m_resourceManager.GetString("Settinglabel.Text"), ExitIndex + 1);
            m_nContorlIndex = ContorlIndex;
            m_nCurrentExitIndex = ExitIndex;
            m_mainForm = mainForm;
        }

        private void ExitSwitchForm_Load(object sender, EventArgs e)
        {
            try
            {
                //GlobalDataInterface.globalOut_GradeInfo.nSwitchLabel[m_nCurrentExitIndex] = 0; //add by xcw 20200528
                //(GlobalDataInterface.globalOut_GradeInfo.nSwitchLabel[m_nCurrentExitIndex]=
                m_bSwitchNumber = (GlobalDataInterface.globalOut_GradeInfo.nSwitchLabel[m_nCurrentExitIndex] == 0);
                m_bSwitchWeight = (GlobalDataInterface.globalOut_GradeInfo.nSwitchLabel[m_nCurrentExitIndex] == 1);
                m_bSwitchVolume = (GlobalDataInterface.globalOut_GradeInfo.nSwitchLabel[m_nCurrentExitIndex] == 2);
                if (m_bSwitchNumber)
                {
                    //GlobalDataInterface.globalOut_GradeInfo.nExitSwitchNum[m_nCurrentExitIndex] = GlobalDataInterface.globalOut_GradeInfo.nCheckNum;
                    //this.NumnumericUpDown.Text = GlobalDataInterface.globalOut_GradeInfo.nCheckNum.ToString();
                    this.NumnumericUpDown.Text = GlobalDataInterface.globalOut_GradeInfo.nExitSwitchNum[m_nCurrentExitIndex].ToString();
                    this.WeightnumericUpDown.Text = "0";
                    this.VolumenumericUpDown.Text = "0";
                    
                }
                else if (m_bSwitchWeight)
                {
                    this.NumnumericUpDown.Text = "0";
                    this.WeightnumericUpDown.Text = GlobalDataInterface.globalOut_GradeInfo.nExitSwitchNum[m_nCurrentExitIndex].ToString();
                    this.VolumenumericUpDown.Text = "0";
                }
                else if (m_bSwitchVolume)
                {
                    this.NumnumericUpDown.Text = "0";
                    this.WeightnumericUpDown.Text = "0";
                    this.VolumenumericUpDown.Text = GlobalDataInterface.globalOut_GradeInfo.nExitSwitchNum[m_nCurrentExitIndex].ToString();
                }
                this.NumnumericUpDown.Enabled = m_bSwitchNumber;
                this.WeightnumericUpDown.Enabled = m_bSwitchWeight;
                this.VolumenumericUpDown.Enabled = m_bSwitchVolume;

                this.NumcheckBox.Checked = m_bSwitchNumber;
                this.WeightcheckBox.Checked = m_bSwitchWeight;
                this.VolumecheckBox.Checked = m_bSwitchVolume;

                if (GlobalDataInterface.globalOut_GradeInfo.nLabelType != 2)
                {
                    this.LabelcheckBox1.Enabled = false;
                    this.LabelcheckBox2.Enabled = false;
                    this.LabelcheckBox3.Enabled = false;
                    this.LabelcheckBox4.Enabled = false;
                }
                else
                {
                    this.LabelcheckBox1.Enabled = true;
                    this.LabelcheckBox2.Enabled = true;
                    this.LabelcheckBox3.Enabled = true;
                    this.LabelcheckBox4.Enabled = true;
                    if (GlobalDataInterface.globalOut_GradeInfo.nLabelbyExit[m_nCurrentExitIndex] > 0)
                    {
                        m_nLabel = GlobalDataInterface.globalOut_GradeInfo.nLabelbyExit[m_nCurrentExitIndex];  //Add by ChengSk - 2017/11/01
                        //switch (GlobalDataInterface.globalOut_GradeInfo.nLabelbyExit[m_nCurrentExitIndex])   //Note by ChengSk - 2017/11/01
                        switch (m_nLabel)
                        {
                            case 1:
                                this.LabelcheckBox1.Checked = true;
                                break;
                            case 2:
                                this.LabelcheckBox2.Checked = true;
                                break;
                            case 3:
                                this.LabelcheckBox3.Checked = true;
                                break;
                            case 4:
                                this.LabelcheckBox4.Checked = true;
                                break;
                            default: break;
                        }

                    }
                }

                string str = Commonfunction.GetAppSetting("贴标机1");
                if (str != "")
                    this.LabelcheckBox1.Text = str;
                str = Commonfunction.GetAppSetting("贴标机2");
                if (str != "")
                    this.LabelcheckBox2.Text = str;
                str = Commonfunction.GetAppSetting("贴标机3");
                if (str != "")
                    this.LabelcheckBox3.Text = str;
                str = Commonfunction.GetAppSetting("贴标机4");
                if (str != "")
                    this.LabelcheckBox4.Text = str;

                int sizeNum, qualNum;
                //if ((((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x06) > 0) || ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0X38) > 0)) && ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 1) > 0))//既有品质也有尺寸
                if ((GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0) && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0))//既有品质也有尺寸
                {
                    sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                    qualNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                }
                //else if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 1)
                else if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 0)
                {
                    sizeNum = 1;
                    qualNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                }
                else
                {
                    sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                    qualNum = 1;
                }

                string strGradeNames = "";
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                for (int i = 0; i < sizeNum; i++)
                {
                    for (int j = 0; j < qualNum; j++)
                    {
                        m_bIsExitGrade[i, j] = ((GlobalDataInterface.globalOut_GradeInfo.grades[j * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].exit >> m_nCurrentExitIndex) & 1) == 1;
                        if(m_bIsExitGrade[i, j]) //被选中
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            strGradeNames += Encoding.Default.GetString(temp).TrimEnd('\0');
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            strGradeNames += Encoding.Default.GetString(temp).TrimEnd('\0');
                            strGradeNames += ",";
                        }
                    }
                }
                if (strGradeNames.Length > 0)
                    strGradeNames = strGradeNames.Substring(0, strGradeNames.Length - 1);
                this.ProductNametextBox.Text = strGradeNames;
                InitialExitGradeGrid();
                for (int i = 0; i < sizeNum; i++)
                {
                    for (int j = 0; j < qualNum; j++)
                    {
                        //MessageBox.Show(GlobalDataInterface.globalOut_GradeInfo.grades[j * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].exit.ToString());
                        if (GlobalDataInterface.globalOut_GradeInfo.grades[j * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].exit > 0)
                            this.ExitSwitchdataGridView[i, j].Style.BackColor = Color.White;
                        else
                            //this.ExitSwitchdataGridView[i, j].Style.BackColor = Color.Yellow;
                            this.ExitSwitchdataGridView[i, j].Value = this.OrangeimageList.Images[1];//Modify by xcw - 20191118

                    }
                }
                tempMotorInfo.ToCopy(GlobalDataInterface.globalOut_MotorInfo[m_nCurrentExitIndex]);
                //this.MotorEnableSwitchcomboBox.SelectedIndex = tempMotorInfo.bMotorSwitch;
                this.DelayTimenumericUpDown.Text = GlobalDataInterface.globalOut_GlobalExitInfo[0].Delay_time[m_nCurrentExitIndex].ToString();
                this.HoldTimenumericUpDown.Text = GlobalDataInterface.globalOut_GlobalExitInfo[0].Hold_time[m_nCurrentExitIndex].ToString();//add by xcw - 20191209
                this.MotorEnableSwitchcomboBox.SelectedIndex = tempMotorInfo.bMotorSwitch - 2;   //Modify by ChengSk - 20190708
                switch (tempMotorInfo.bMotorSwitch)
                {
                    case 0://0表示个数使能，1表示重量使能
                        this.MotorEnableSwitchNumlabel.Enabled = true;
                        this.MotorEnableSwitchNumnumericUpDown.Enabled = true;
                        this.MotorEnableSwitchWeightlabel.Enabled = false;
                        this.MotorEnableSwitchWeightnumericUpDown.Enabled = false;
                        break;
                    case 1:
                        this.MotorEnableSwitchNumlabel.Enabled = false;
                        this.MotorEnableSwitchNumnumericUpDown.Enabled = false;
                        this.MotorEnableSwitchWeightlabel.Enabled = true;
                        this.MotorEnableSwitchWeightnumericUpDown.Enabled = true;
                        break;
                    case 2:
                        this.MotorEnableSwitchNumlabel.Enabled = false;
                        this.MotorEnableSwitchNumnumericUpDown.Enabled = false;
                        this.MotorEnableSwitchWeightlabel.Enabled = false;
                        this.MotorEnableSwitchWeightnumericUpDown.Enabled = false;
                        break;
                    default: break;
                }
                this.MotorEnableSwitchNumnumericUpDown.Text = tempMotorInfo.nMotorEnableSwitchNum.ToString();
                this.MotorEnableSwitchWeightnumericUpDown.Text = tempMotorInfo.nMotorEnableSwitchWeight.ToString();
                //this.DelayTimenumericUpDown.Text = tempMotorInfo.fDelay_time.ToString();
                //this.HoldTimenumericUpDown.Text = tempMotorInfo.fHold_time.ToString();

                
                string strDisplayNameConfig = "出口" + (m_nCurrentExitIndex + 1).ToString() + "显示名称";
                this.DisplayNametextBox.Text = Commonfunction.GetAppSetting(strDisplayNameConfig);
                string strAdditionalTextConfig = "出口" + (m_nCurrentExitIndex + 1).ToString() + "附加信息";
                this.AdditionalTextrichTextBox.Text = Commonfunction.GetAppSetting(strAdditionalTextConfig);

                string strexitDisplayTypeConfig = "出口显示名称类型";
                long exitDisplayType = long.Parse(Commonfunction.GetAppSetting(strexitDisplayTypeConfig));
                if (((exitDisplayType >> m_nCurrentExitIndex) & 1) == 0)
                    this.ReplaceSelectcheckBox.Checked = true;
                else
                    this.ReplaceSelectcheckBox.Checked = false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数ExitSwitchForm_Load出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数ExitSwitchForm_Load出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 校验抽检出口的等级数量是否超过最大值
        /// </summary>
        /// <returns></returns>
        private bool CheckSampleOutletGradeInfo(int maxGradeNum)
        {
            try
            {
                int sizeNum, qualNum;
                if ((GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0) && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0))//既有品质也有尺寸
                {
                    sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                    qualNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                }
                else if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 0)
                {
                    sizeNum = 1;
                    qualNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                }
                else
                {
                    sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                    qualNum = 1;
                }

                int nGradeSum = 0; //该出口的选中的等级数量
                for (int i = 0; i < sizeNum; i++)
                {
                    for (int j = 0; j < qualNum; j++)
                    {
                        if (m_bIsExitGrade[i, j])  //被选中
                        {
                            nGradeSum++;
                        }
                    }
                }

                bool bResult = ((nGradeSum >= maxGradeNum) ? true : false);
                return bResult;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数CheckSampleOutletGradeInfo出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数CheckSampleOutletGradeInfo出错" + ex);
#endif
                return true;
            }
        }

        /// <summary>
        /// 更新产品名称
        /// </summary>
        private void UpdateProductNameText()
        {
            try
            {
                int sizeNum, qualNum;
                if ((GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0) && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0))//既有品质也有尺寸
                {
                    sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                    qualNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                }
                else if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 0)
                {
                    sizeNum = 1;
                    qualNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                }
                else
                {
                    sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                    qualNum = 1;
                }

                string strGradeNames = "";
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                for (int i = 0; i < sizeNum; i++)
                {
                    for (int j = 0; j < qualNum; j++)
                    {
                        if (m_bIsExitGrade[i, j]) //被选中
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            strGradeNames += Encoding.Default.GetString(temp).TrimEnd('\0');
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, j * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            strGradeNames += Encoding.Default.GetString(temp).TrimEnd('\0');
                            strGradeNames += ",";
                        }
                    }
                }
                if (strGradeNames.Length > 0)
                    strGradeNames = strGradeNames.Substring(0, strGradeNames.Length - 1);
                this.ProductNametextBox.Text = strGradeNames;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数UpdateProductNameText出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数UpdateProductNameText出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 初始化ExitGradeGrid
        /// </summary>
        private void InitialExitGradeGrid()
        {
            try
            {
                this.ExitSwitchdataGridView.Columns.Clear();
                this.ExitSwitchdataGridView.Rows.Clear();
                this.ExitSwitchdataGridView.ReadOnly = true;
                this.ExitSwitchdataGridView.BackgroundColor = Color.White;
                this.ExitSwitchdataGridView.AllowUserToAddRows = false;
                int rowNum = 1;
                int colNum = 1;
                //if ((((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x06) > 0) || ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0X38) > 0)) && ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 1) > 0))//既有品质也有尺寸
                if ((GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0) && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0))//既有品质也有尺寸
                {
                    rowNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                    colNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                }
                else
                {
                    //if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 1)//只有品质
                    if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 0)//只有品质
                    {
                        rowNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                    }
                    else//只有尺寸
                    {
                        colNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                    }
                }

                DataGridViewImageColumn column;
                for (int i = 1; i <= colNum; i++)
                {
                    column = new DataGridViewImageColumn();
                    byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strSizeGradeName, (i - 1) * ConstPreDefine.MAX_TEXT_LENGTH, tempByte, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                    column.Width = 32;
                    column.HeaderText = Encoding.Default.GetString(tempByte).TrimEnd('\0');
                    this.ExitSwitchdataGridView.Columns.Add(column);
                }
                DataGridViewRow row;
                if (rowNum == 1)
                {
                    
                        row = new DataGridViewRow();
                        //if ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 1) > 0)
                        if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)
                        {
                            byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, 0, tempByte, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            row.HeaderCell.Value = Encoding.Default.GetString(tempByte).TrimEnd('\0');
                        }
                        row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        row.Height = 32;
                        this.ExitSwitchdataGridView.Rows.Add(row);                   
                }
                else
                {

                    for (int i = 0; i < rowNum; i++)
                    {
                        row = new DataGridViewRow();
                        byte[] tempByte = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, tempByte, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                        row.HeaderCell.Value = Encoding.Default.GetString(tempByte).TrimEnd('\0');
                        row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                        row.Height = 32;
                        this.ExitSwitchdataGridView.Rows.Add(row);
                    }
                }

                for (int i = 0; i < this.ExitSwitchdataGridView.Rows.Count; i++)
                {
                    for (int j = 0; j < this.ExitSwitchdataGridView.Columns.Count; j++)
                    {
                        if (m_bIsExitGrade[j, i])
                            this.ExitSwitchdataGridView[j, i].Value = this.OrangeimageList.Images[0];
                        else
                            this.ExitSwitchdataGridView[j, i].Value = this.OrangeimageList.Images[2];
                    }
                }
                this.ExitSwitchdataGridView.AllowUserToAddRows = false;
                this.ExitSwitchdataGridView.RowHeadersWidth = 80;
                this.ExitSwitchdataGridView.TopLeftHeaderCell.Value = m_resourceManager.GetString("Gradelabel.Text");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数InitialExitGradeGrid出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数InitialExitGradeGrid出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 个数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumcheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    m_bSwitchNumber = true;
                    m_bSwitchWeight = false;
                    m_bSwitchVolume = false;
                    this.WeightcheckBox.Checked = false;
                    this.VolumecheckBox.Checked = false;
                    this.NumnumericUpDown.Enabled = m_bSwitchNumber;
                    this.WeightnumericUpDown.Enabled = m_bSwitchWeight;
                    this.VolumenumericUpDown.Enabled = m_bSwitchVolume;
                    this.NumnumericUpDown.Focus();
                    this.NumnumericUpDown.Text = "0";  
                    //this.NumnumericUpDown.Text = GlobalDataInterface.globalOut_GradeInfo.nCheckNum.ToString();
                    if (tempMotorInfo.bMotorSwitch == 2)
                    {
                        this.MotorEnableSwitchNumnumericUpDown.Text = this.NumnumericUpDown.Text;
                    }
                }
                else
                {
                    m_bSwitchNumber = false;
                    this.NumnumericUpDown.Enabled = m_bSwitchNumber;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数NumcheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数NumcheckBox_Click出错" + ex);
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
                    m_bSwitchNumber = false;
                    m_bSwitchWeight = true;
                    m_bSwitchVolume = false;
                    this.NumcheckBox.Checked = false;
                    this.VolumecheckBox.Checked = false;
                    this.NumnumericUpDown.Enabled = m_bSwitchNumber;
                    this.WeightnumericUpDown.Enabled = m_bSwitchWeight;
                    this.VolumenumericUpDown.Enabled = m_bSwitchVolume;
                    this.WeightnumericUpDown.Focus();
                    this.WeightnumericUpDown.Text = "0";
                    if (tempMotorInfo.bMotorSwitch == 2)
                    {
                        this.MotorEnableSwitchWeightnumericUpDown.Text = this.WeightnumericUpDown.Text;
                    }
                }
                else
                {
                    m_bSwitchWeight = false;
                    this.WeightnumericUpDown.Enabled = m_bSwitchWeight;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数WeightcheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数WeightcheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 体积
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolumecheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    m_bSwitchNumber = false;
                    m_bSwitchWeight = false;
                    m_bSwitchVolume = true;
                    this.NumcheckBox.Checked = false;
                    this.WeightcheckBox.Checked = false;
                    this.NumnumericUpDown.Enabled = m_bSwitchNumber;
                    this.WeightnumericUpDown.Enabled = m_bSwitchWeight;
                    this.VolumenumericUpDown.Enabled = m_bSwitchVolume;
                    this.VolumenumericUpDown.Focus();
                    this.VolumenumericUpDown.Text = "0";
                }
                else
                {
                    m_bSwitchVolume = false;
                    this.VolumenumericUpDown.Enabled = m_bSwitchVolume;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数VolumecheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数VolumecheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 贴标1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelcheckBox1_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    m_nLabel = 1;
                    this.LabelcheckBox2.Checked = false;
                    this.LabelcheckBox3.Checked = false;
                    this.LabelcheckBox4.Checked = false;
                }
                else
                {
                    m_nLabel = 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数LabelcheckBox1_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数LabelcheckBox1_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 贴标2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelcheckBox2_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    m_nLabel = 2;
                    this.LabelcheckBox1.Checked = false;
                    this.LabelcheckBox3.Checked = false;
                    this.LabelcheckBox4.Checked = false;
                }
                else
                {
                    m_nLabel = 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数LabelcheckBox2_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数LabelcheckBox2_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 贴标3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelcheckBox3_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    m_nLabel = 3;
                    this.LabelcheckBox1.Checked = false;
                    this.LabelcheckBox2.Checked = false;
                    this.LabelcheckBox4.Checked = false;
                }
                else
                {
                    m_nLabel = 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数LabelcheckBox3_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数LabelcheckBox3_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 贴标4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelcheckBox4_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    m_nLabel = 4;
                    this.LabelcheckBox1.Checked = false;
                    this.LabelcheckBox2.Checked = false;
                    this.LabelcheckBox3.Checked = false;
                }
                else
                {
                    m_nLabel = 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数LabelcheckBox4_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数LabelcheckBox4_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 单元格双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitSwitchdataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    if (m_bIsExitGrade[e.ColumnIndex, e.RowIndex])
                    {
                        this.ExitSwitchdataGridView[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                        this.ExitSwitchdataGridView[e.ColumnIndex, e.RowIndex].Value = this.OrangeimageList.Images[1];
                        m_bIsExitGrade[e.ColumnIndex, e.RowIndex] = false;
                    }
                    else //准备选中阶段
                    {
                        if (GlobalDataInterface.nSampleOutlet == m_nCurrentExitIndex + 1)    //抽检出口
                        {
                            if (CheckSampleOutletGradeInfo(1) == true) return; //抽检出口最多只能放一个等级
                        }
                        this.ExitSwitchdataGridView[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                        this.ExitSwitchdataGridView[e.ColumnIndex, e.RowIndex].Value = this.OrangeimageList.Images[0];
                        m_bIsExitGrade[e.ColumnIndex, e.RowIndex] = true;
                    }
                    UpdateProductNameText(); //更新产品名称
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数ExitSwitchdataGridView_CellContentDoubleClick出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数ExitSwitchdataGridView_CellContentDoubleClick出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 使能模式选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MotorEnableSwitchcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            //tempMotorInfo.bMotorSwitch = (byte)this.MotorEnableSwitchcomboBox.SelectedIndex;
            tempMotorInfo.bMotorSwitch = (byte)(this.MotorEnableSwitchcomboBox.SelectedIndex + 2); //Modify by ChengSk - 20190708
            switch(tempMotorInfo.bMotorSwitch)
            {
                case 0://0表示个数使能，1表示重量使能
                    this.MotorEnableSwitchNumlabel.Enabled = true;
                    this.MotorEnableSwitchNumnumericUpDown.Enabled = true;
                    this.MotorEnableSwitchWeightlabel.Enabled = false;
                    this.MotorEnableSwitchWeightnumericUpDown.Enabled = false;
                    break;
                case 1:
                    this.MotorEnableSwitchNumlabel.Enabled = false;
                    this.MotorEnableSwitchNumnumericUpDown.Enabled = false;
                    this.MotorEnableSwitchWeightlabel.Enabled = true;
                    this.MotorEnableSwitchWeightnumericUpDown.Enabled = true;
                    break;
               case 2:
                    this.MotorEnableSwitchNumlabel.Enabled = false;
                    this.MotorEnableSwitchNumnumericUpDown.Enabled = false;
                    this.MotorEnableSwitchWeightlabel.Enabled = false;
                    this.MotorEnableSwitchWeightnumericUpDown.Enabled = false;
                    if (m_bSwitchNumber)
                    {
                        this.MotorEnableSwitchNumnumericUpDown.Text = this.NumnumericUpDown.Text;
                    }
                    else if (m_bSwitchWeight)
                    {
                        this.MotorEnableSwitchWeightnumericUpDown.Text = this.WeightnumericUpDown.Text;
                    }
                    break;
               default: break;
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
                stGradeInfo tempGradeInfo = new stGradeInfo(true);
                tempGradeInfo.ToCopy(GlobalDataInterface.globalOut_GradeInfo);
                if (m_bSwitchNumber)
                {
                    GlobalDataInterface.globalOut_GradeInfo.nSwitchLabel[m_nCurrentExitIndex] = 0;
                    GlobalDataInterface.globalOut_GradeInfo.nExitSwitchNum[m_nCurrentExitIndex] = int.Parse(this.NumnumericUpDown.Text);
                }
                else if (m_bSwitchWeight)
                {
                    GlobalDataInterface.globalOut_GradeInfo.nSwitchLabel[m_nCurrentExitIndex] = 1;
                    GlobalDataInterface.globalOut_GradeInfo.nExitSwitchNum[m_nCurrentExitIndex] = int.Parse(this.WeightnumericUpDown.Text);
                }
                else if (m_bSwitchVolume)
                {
                    GlobalDataInterface.globalOut_GradeInfo.nSwitchLabel[m_nCurrentExitIndex] = 2;
                    GlobalDataInterface.globalOut_GradeInfo.nExitSwitchNum[m_nCurrentExitIndex] = int.Parse(this.VolumenumericUpDown.Text);
                }
                else
                {
                    GlobalDataInterface.globalOut_GradeInfo.nSwitchLabel[m_nCurrentExitIndex] = 0x7f;
                    GlobalDataInterface.globalOut_GradeInfo.nExitSwitchNum[m_nCurrentExitIndex] = 0x7f7f7f7f;
                }
                if (GlobalDataInterface.globalOut_GradeInfo.nLabelType == 2)
                {
                    GlobalDataInterface.globalOut_GradeInfo.nLabelbyExit[m_nCurrentExitIndex] = m_nLabel;
                }

                int sizeNum, qualNum;
                //if ((((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0x06) > 0) || ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 0X38) > 0)) && ((GlobalDataInterface.globalOut_GradeInfo.nClassifyType & 1) > 0))//既有品质也有尺寸
                if ((GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0) && (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0))//既有品质也有尺寸
                {
                    sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                    qualNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                }
                //else if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 1)
                else if (GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0 && GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 0)
                {
                    sizeNum = 1;
                    qualNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                }
                else
                {
                    sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                    qualNum = 1;
                }
                for (int i = 0; i < sizeNum; i++)
                {
                    for (int j = 0; j < qualNum; j++)
                    {
                        if (m_bIsExitGrade[i, j])
                            GlobalDataInterface.globalOut_GradeInfo.grades[j * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].exit |= ((long)1 << m_nCurrentExitIndex);
                        else
                            GlobalDataInterface.globalOut_GradeInfo.grades[j * ConstPreDefine.MAX_SIZE_GRADE_NUM + i].exit &= ~((long)1 << m_nCurrentExitIndex);
                    }
                }
                if (!GlobalDataInterface.globalOut_GradeInfo.IsEqual(tempGradeInfo))
                {
                    m_mainForm.SetSeparationProgrameChangelabel(false, null);
                }

                //tempMotorInfo.bExitId = (byte)m_nCurrentExitIndex;
                //tempMotorInfo.bMotorSwitch = (byte)this.MotorEnableSwitchcomboBox.SelectedIndex;
                //tempMotorInfo.nMotorEnableSwitchNum = int.Parse(this.MotorEnableSwitchNumnumericUpDown.Text);
                //tempMotorInfo.nMotorEnableSwitchWeight = int.Parse(this.MotorEnableSwitchWeightnumericUpDown.Text);
                //tempMotorInfo.fDelay_time = float.Parse(this.DelayTimenumericUpDown.Text);
                //tempMotorInfo.fHold_time = float.Parse(this.HoldTimenumericUpDown.Text);
                //GlobalDataInterface.globalOut_MotorInfo[m_nCurrentExitIndex].ToCopy(tempMotorInfo);

                if (GlobalDataInterface.global_IsTestMode)
                {
                    // GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_MOTOR_INFO, tempMotorInfo);
                    //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                    int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                    if (global_IsTest != 0) //add by xcw 20201211
                    {
                        MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                        LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                }

                //将电机命令立即生效按钮整合到确定按钮 2015-6-25 ivycc 原按钮功能保留 只是不显示
                tempMotorInfo.bExitId = (byte)m_nCurrentExitIndex;
                //tempMotorInfo.bMotorSwitch = (byte)this.MotorEnableSwitchcomboBox.SelectedIndex;
                tempMotorInfo.bMotorSwitch = (byte)(this.MotorEnableSwitchcomboBox.SelectedIndex + 2);  //Modify by ChengSk - 20190708 
                if (tempMotorInfo.bMotorSwitch == 2)
                {
                    if (m_bSwitchNumber)
                    {
                        tempMotorInfo.nMotorEnableSwitchNum = int.Parse(this.NumnumericUpDown.Text);
                    }
                    else if (m_bSwitchWeight)
                    {
                        tempMotorInfo.nMotorEnableSwitchWeight = int.Parse(this.WeightnumericUpDown.Text);
                    }
                }
                else 
                {
                    tempMotorInfo.nMotorEnableSwitchNum = int.Parse(this.MotorEnableSwitchNumnumericUpDown.Text);
                    tempMotorInfo.nMotorEnableSwitchWeight = int.Parse(this.MotorEnableSwitchWeightnumericUpDown.Text);
                }
                tempMotorInfo.fDelay_time = float.Parse(this.DelayTimenumericUpDown.Text);
                tempMotorInfo.fHold_time = float.Parse(this.HoldTimenumericUpDown.Text);
                GlobalDataInterface.globalOut_GlobalExitInfo[0].Delay_time[m_nCurrentExitIndex] = float.Parse(this.DelayTimenumericUpDown.Text);
                GlobalDataInterface.globalOut_GlobalExitInfo[0].Hold_time[m_nCurrentExitIndex] = float.Parse(this.HoldTimenumericUpDown.Text);  //add by xcw - 20191209
                GlobalDataInterface.globalOut_MotorInfo[m_nCurrentExitIndex].ToCopy(tempMotorInfo);
                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_MOTOR_INFO, tempMotorInfo);
                }

                string strDisplayNameConfig = "出口" + (m_nCurrentExitIndex + 1).ToString() + "显示名称";
                Commonfunction.SetAppSetting(strDisplayNameConfig, this.DisplayNametextBox.Text.Trim());
                string strAdditionalTextConfig = "出口" + (m_nCurrentExitIndex + 1).ToString() + "附加信息";
                Commonfunction.SetAppSetting(strAdditionalTextConfig, this.AdditionalTextrichTextBox.Text.Trim());
                GlobalDataInterface.UpData_gradeinfo = true;
                m_mainForm.SetExitListBox(m_nContorlIndex, m_nCurrentExitIndex);
                GlobalDataInterface.UpData_gradeinfo = false;
                this.ExitSwitchdataGridView.DataSource = null; //释放资源，不知是否有效
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数OKbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数OKbutton_Click出错" + ex);
#endif
            }
            
        }

        /// <summary>
        /// 取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            try
            {
                this.ExitSwitchdataGridView.DataSource = null; //释放资源，不知是否有效
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数Cancelbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数Cancelbutton_Click出错" + ex);
#endif
            }
        }

       /// <summary>
       /// 电机参数发送按钮
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void MotorSendbutton_Click(object sender, EventArgs e)
        {
            tempMotorInfo.bExitId = (byte)m_nCurrentExitIndex;
            //tempMotorInfo.bMotorSwitch = (byte)this.MotorEnableSwitchcomboBox.SelectedIndex;
            tempMotorInfo.bMotorSwitch = (byte)(this.MotorEnableSwitchcomboBox.SelectedIndex + 2);    //Modify by ChengSk - 20190708
            tempMotorInfo.nMotorEnableSwitchNum = int.Parse(this.MotorEnableSwitchNumnumericUpDown.Text);
            tempMotorInfo.nMotorEnableSwitchWeight = int.Parse(this.MotorEnableSwitchWeightnumericUpDown.Text);
            tempMotorInfo.fDelay_time = float.Parse(this.DelayTimenumericUpDown.Text);
            tempMotorInfo.fHold_time = float.Parse(this.HoldTimenumericUpDown.Text);
            GlobalDataInterface.globalOut_GlobalExitInfo[m_nCurrentExitIndex].Delay_time[m_nCurrentExitIndex] = float.Parse(this.DelayTimenumericUpDown.Text);
            GlobalDataInterface.globalOut_GlobalExitInfo[m_nCurrentExitIndex].Hold_time[m_nCurrentExitIndex] = float.Parse(this.HoldTimenumericUpDown.Text);
            GlobalDataInterface.globalOut_MotorInfo[m_nCurrentExitIndex].ToCopy(tempMotorInfo);
            if (GlobalDataInterface.global_IsTestMode)
            {
                GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_MOTOR_INFO, tempMotorInfo);
            }
        }

        private void ReplaceSelectcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string strexitDisplayTypeConfig = "出口显示名称类型";
                long exitDisplayType = long.Parse(Commonfunction.GetAppSetting(strexitDisplayTypeConfig));

                if (this.ReplaceSelectcheckBox.Checked == true) //0
                {
                    this.DisplayNametextBox.ReadOnly = true;
                    exitDisplayType &= ~((long)1 << m_nCurrentExitIndex);
                }
                else //1
                {
                    this.DisplayNametextBox.ReadOnly = false;
                    exitDisplayType |= ((long)1 << m_nCurrentExitIndex);     
                }

                Commonfunction.SetAppSetting(strexitDisplayTypeConfig, exitDisplayType.ToString());
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数ReplaceSelectcheckBox_CheckedChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数ReplaceSelectcheckBox_CheckedChanged出错" + ex);
#endif
            }   
        }

        private void ExitSwitchForm_FormClosing(object sender, FormClosingEventArgs e) //Add by ChengSk - 20180717
        {
            try
            {
                this.ExitSwitchdataGridView.DataSource = null; //释放资源，不知是否有效
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ExitSwitchForm中函数ExitSwitchForm_FormClosing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ExitSwitchForm中函数ExitSwitchForm_FormClosing出错" + ex);
#endif
            }
        }

    }
}
