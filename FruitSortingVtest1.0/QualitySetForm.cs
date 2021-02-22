using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interface;
using System.Diagnostics;

namespace FruitSortingVtest1._0
{
    public partial class QualitySetForm : Form
    {
        QaulGradeInfo m_qualGradeInfo = new QaulGradeInfo();
        QualGradeSetForm m_qaulGradeSetForm;
        public QualitySetForm(QualGradeSetForm qaulGradeSetForm,QaulGradeInfo qualGradeInfo)
        {
            m_qaulGradeSetForm = qaulGradeSetForm;
            m_qualGradeInfo = qualGradeInfo;
            InitializeComponent();
        }

        
        /// <summary>
        /// 初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QualitySetForm_Load(object sender, EventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                this.NametextBox.Text = m_qualGradeInfo.QualName;
                //CIR视觉系统
                if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x01) == 1)
                {
                    //颜色
                    if (GlobalDataInterface.SystemStructColor)
                    {
                        this.ColorgroupBox.Enabled = true;

                        this.ColorcheckBox.Enabled = GlobalDataInterface.SystemStructColor;
                        for (int i = 0; i < GlobalDataInterface.ColorGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strColorGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.ColorcomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.ColorGradeNum > 0 && m_qualGradeInfo.ColorIndex != 0X7F)
                        {
                            if (m_qualGradeInfo.ColorIndex < this.ColorcomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.ColorcomboBox.SelectedIndex = m_qualGradeInfo.ColorIndex;
                            else
                                this.ColorcomboBox.SelectedIndex = this.ColorcomboBox.Items.Count - 1;
                        }
                        if (m_qualGradeInfo.ColorIndex >= 0 && m_qualGradeInfo.ColorIndex != 0X7F)
                        {
                            this.ColorcheckBox.Checked = true;
                            this.ColorcomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.ColorgroupBox.Enabled = false;
                    }
                    //形状
                    if (GlobalDataInterface.SystemStructShape)
                    {
                        this.ShapegroupBox.Enabled = true;

                        this.ShapecheckBox.Enabled = GlobalDataInterface.SystemStructShape;
                        for (int i = 0; i < GlobalDataInterface.ShapeGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.ShapecomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.ShapeGradeNum > 0 && m_qualGradeInfo.ShapeIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.ShapeIndex < this.ShapecomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.ShapecomboBox.SelectedIndex = m_qualGradeInfo.ShapeIndex;
                            else
                                this.ShapecomboBox.SelectedIndex = this.ShapecomboBox.Items.Count - 1;
                        }
                        if (m_qualGradeInfo.ShapeIndex >= 0 && m_qualGradeInfo.ShapeIndex != 0X7F)
                        {
                            this.ShapecheckBox.Checked = true;
                            this.ShapecomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.ShapegroupBox.Enabled = false;
                    }
                    
                    //if (GlobalDataInterface.SystemStructShape)
                    //{
                    //    this.ShapegroupBox.Enabled = true;

                    //    this.ShapecheckBox.Enabled = GlobalDataInterface.SystemStructShape;
                    //    if (GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName[0] != 0)
                    //    {
                    //        for (int i = 0; i < 2; i++)
                    //        {
                    //            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                    //            this.ShapecomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    //        }
                    //        if (m_qualGradeInfo.ShapeIndex != 0x7f)
                    //            this.ShapecomboBox.SelectedIndex = m_qualGradeInfo.ShapeIndex;

                    //    }
                    //    if (m_qualGradeInfo.ShapeIndex >= 0 && m_qualGradeInfo.ShapeIndex != 0X7F)
                    //    {
                    //        this.ShapecheckBox.Checked = true;
                    //        this.ShapecomboBox.Enabled = true;
                    //    }
                    //}
                    //else
                    //{
                    //    this.ShapegroupBox.Enabled = false;
                    //}
                    //瑕疵
                    if (GlobalDataInterface.SystemStructFlaw)
                    {
                        this.FlawgroupBox.Enabled = true;

                        this.FlawcheckBox.Enabled = GlobalDataInterface.SystemStructFlaw;
                        for (int i = 0; i < GlobalDataInterface.FlawGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stFlawareaGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.FlawcomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.FlawGradeNum > 0 && m_qualGradeInfo.FlawIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.FlawIndex < this.FlawcomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.FlawcomboBox.SelectedIndex = m_qualGradeInfo.FlawIndex;
                            else
                                this.FlawcomboBox.SelectedIndex = this.FlawcomboBox.Items.Count - 1;
                        }   
                        if (m_qualGradeInfo.FlawIndex >= 0 && m_qualGradeInfo.FlawIndex != 0X7F)
                        {
                            this.FlawcheckBox.Checked = true;
                            this.FlawcomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.FlawgroupBox.Enabled = false;
                    }
                }
                else
                {
                    this.ColorgroupBox.Enabled = false;
                    this.ShapegroupBox.Enabled = false;
                    this.FlawgroupBox.Enabled = false;
                }
                //UV视觉系统
                if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x02) == 2)
                {
                    //擦伤
                    if (GlobalDataInterface.SystemStructBruise)
                    {
                        this.BruisegroupBox.Enabled = true;

                        this.BruisecheckBox.Enabled = GlobalDataInterface.SystemStructBruise;
                        for (int i = 0; i < GlobalDataInterface.BruiseGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stBruiseGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.BruisecomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.BruiseGradeNum > 0 && m_qualGradeInfo.BruiseIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.BruiseIndex < this.BruisecomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.BruisecomboBox.SelectedIndex = m_qualGradeInfo.BruiseIndex;
                            else
                                this.BruisecomboBox.SelectedIndex = this.BruisecomboBox.Items.Count - 1;
                        }
                        if (m_qualGradeInfo.BruiseIndex >= 0 && m_qualGradeInfo.BruiseIndex != 0X7F)
                        {
                            this.BruisecheckBox.Checked = true;
                            this.BruisecomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.BruisegroupBox.Enabled = false;
                    }
                    //腐烂
                    if (GlobalDataInterface.SystemStructRot)
                    {
                        this.RotgroupBox.Enabled = true;

                        this.RotcheckBox.Enabled = GlobalDataInterface.SystemStructRot;
                        for (int i = 0; i < GlobalDataInterface.RotGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stRotGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.RotcomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.RotGradeNum > 0 && m_qualGradeInfo.RotIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.RotIndex < this.RotcomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.RotcomboBox.SelectedIndex = m_qualGradeInfo.RotIndex;
                            else
                                this.RotcomboBox.SelectedIndex = this.RotcomboBox.Items.Count - 1;
                        }
                        if (m_qualGradeInfo.RotIndex >= 0 && m_qualGradeInfo.RotIndex != 0X7F)
                        {
                            this.RotcheckBox.Checked = true;
                            this.RotcomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.RotgroupBox.Enabled = false;
                    }
                }
                else
                {
                    this.BruisegroupBox.Enabled = false;
                    this.RotgroupBox.Enabled = false;
                }
                //重量系统
                if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x04) == 4)
                {
                    //密度
                    if (GlobalDataInterface.SystemStructDensity)
                    {
                        this.DensitygroupBox.Enabled = true;

                        this.DensitycheckBox.Enabled = GlobalDataInterface.SystemStructDensity;
                        for (int i = 0; i < GlobalDataInterface.DensityGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stDensityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.DensitycomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.DensityGradeNum > 0 && m_qualGradeInfo.DensityIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.DensityIndex < this.DensitycomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.DensitycomboBox.SelectedIndex = m_qualGradeInfo.DensityIndex;
                            else
                                this.DensitycomboBox.SelectedIndex = this.DensitycomboBox.Items.Count - 1;
                        }
                        if (m_qualGradeInfo.DensityIndex >= 0 && m_qualGradeInfo.DensityIndex != 0X7F)
                        {
                            this.DensitycheckBox.Checked = true;
                            this.DensitycomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.DensitygroupBox.Enabled = false;
                    }
                }
                else
                {
                    this.DensitygroupBox.Enabled = false;
                }
                //内部品质
                if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8)
                {
                    //糖度
                    if (GlobalDataInterface.SystemStructSugar)
                    {
                        this.SugargroupBox.Enabled = true;

                        this.SugarcheckBox.Enabled = GlobalDataInterface.SystemStructSugar;
                        for (int i = 0; i < GlobalDataInterface.SugarGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stSugarGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.SugarcomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.SugarGradeNum > 0 && m_qualGradeInfo.SugarIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.SugarIndex < this.SugarcomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.SugarcomboBox.SelectedIndex = m_qualGradeInfo.SugarIndex;
                            else
                                this.SugarcomboBox.SelectedIndex = this.SugarcomboBox.Items.Count - 1;
                        }
                        if (m_qualGradeInfo.SugarIndex >= 0 && m_qualGradeInfo.SugarIndex != 0X7F)
                        {
                            this.SugarcheckBox.Checked = true;
                            this.SugarcomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.SugargroupBox.Enabled = false;
                    }
                    //酸度
                    if (GlobalDataInterface.SystemStructAcidity)
                    {
                        this.AciditygroupBox.Enabled = true;

                        this.AciditycheckBox.Enabled = GlobalDataInterface.SystemStructAcidity;
                        for (int i = 0; i < GlobalDataInterface.AcidityGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stAcidityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.AciditycomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.AcidityGradeNum > 0 && m_qualGradeInfo.AcidityIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.AcidityIndex < this.AciditycomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.AciditycomboBox.SelectedIndex = m_qualGradeInfo.AcidityIndex;
                            else
                                this.AciditycomboBox.SelectedIndex = this.AciditycomboBox.Items.Count - 1;
                        }    
                        if (m_qualGradeInfo.AcidityIndex >= 0 && m_qualGradeInfo.AcidityIndex != 0X7F)
                        {
                            this.AciditycheckBox.Checked = true;
                            this.AciditycomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.AciditygroupBox.Enabled = false;
                    }
                    //空心
                    if (GlobalDataInterface.SystemStructHollow)
                    {
                        this.HollowgroupBox.Enabled = true;

                        this.HollowcheckBox.Enabled = GlobalDataInterface.SystemStructHollow;
                        for (int i = 0; i < GlobalDataInterface.HollowGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stHollowGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.HollowcomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.HollowGradeNum > 0 && m_qualGradeInfo.HollowIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.HollowIndex < this.HollowcomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.HollowcomboBox.SelectedIndex = m_qualGradeInfo.HollowIndex;
                            else
                                this.HollowcomboBox.SelectedIndex = this.HollowcomboBox.Items.Count - 1;
                        }
                        if (m_qualGradeInfo.HollowIndex >= 0 && m_qualGradeInfo.HollowIndex != 0X7F)
                        {
                            this.HollowcheckBox.Checked = true;
                            this.HollowcomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.HollowgroupBox.Enabled = false;
                    }
                    //浮皮
                    if (GlobalDataInterface.SystemStructSkin)
                    {
                        this.SkingroupBox.Enabled = true;

                        this.SkincheckBox.Enabled = GlobalDataInterface.SystemStructSkin;
                        for (int i = 0; i < GlobalDataInterface.SkinGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stSkinGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.SkincomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.SkinGradeNum > 0 && m_qualGradeInfo.SkinIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.SkinIndex < this.SkincomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.SkincomboBox.SelectedIndex = m_qualGradeInfo.SkinIndex;
                            else
                                this.SkincomboBox.SelectedIndex = this.SkincomboBox.Items.Count - 1;
                        }   
                        if (m_qualGradeInfo.SkinIndex >= 0 && m_qualGradeInfo.SkinIndex != 0X7F)
                        {
                            this.SkincheckBox.Checked = true;
                            this.SkincomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.SkingroupBox.Enabled = false;
                    }
                    //褐变
                    if (GlobalDataInterface.SystemStructBrown)
                    {
                        this.BrowngroupBox.Enabled = true;

                        this.BrowncheckBox.Enabled = GlobalDataInterface.SystemStructBrown;
                        for (int i = 0; i < GlobalDataInterface.BrownGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stBrownGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.BrowncomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.BrownGradeNum > 0 && m_qualGradeInfo.BrownIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.BrownIndex < this.BrowncomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.BrowncomboBox.SelectedIndex = m_qualGradeInfo.BrownIndex;
                            else
                                this.BrowncomboBox.SelectedIndex = this.BrowncomboBox.Items.Count - 1;
                        }
                        if (m_qualGradeInfo.BrownIndex >= 0 && m_qualGradeInfo.BrownIndex != 0X7F)
                        {
                            this.BrowncheckBox.Checked = true;
                            this.BrowncomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.BrowngroupBox.Enabled = false;
                    }
                    //糖心
                    if (GlobalDataInterface.SystemStructTangxin)
                    {
                        this.TangxingroupBox.Enabled = true;

                        this.TangxincheckBox.Enabled = GlobalDataInterface.SystemStructTangxin;
                        for (int i = 0; i < GlobalDataInterface.TangxinGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stTangxinGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.TangxincomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.TangxinGradeNum > 0 && m_qualGradeInfo.TangxinIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.TangxinIndex < this.TangxincomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.TangxincomboBox.SelectedIndex = m_qualGradeInfo.TangxinIndex;
                            else
                                this.TangxincomboBox.SelectedIndex = this.TangxincomboBox.Items.Count - 1;
                        }
                        if (m_qualGradeInfo.TangxinIndex >= 0 && m_qualGradeInfo.TangxinIndex != 0X7F)
                        {
                            this.TangxincheckBox.Checked = true;
                            this.TangxincomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.TangxingroupBox.Enabled = false;
                    }
                }
                else
                {
                    this.SugargroupBox.Enabled = false;
                    this.AciditygroupBox.Enabled = false;
                    this.HollowgroupBox.Enabled = false;
                    this.SkingroupBox.Enabled = false;
                    this.BrowngroupBox.Enabled = false;
                    this.TangxingroupBox.Enabled = false;
                }
                //超声波系统
                if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x10) == 16)
                {
                    //硬度
                    if (GlobalDataInterface.SystemStructRigidity)
                    {
                        this.RigiditygroupBox.Enabled = true;

                        this.RigiditycheckBox.Enabled = GlobalDataInterface.SystemStructRigidity;
                        for (int i = 0; i < GlobalDataInterface.RigidityGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stRigidityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.RigiditycomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.RigidityGradeNum > 0 && m_qualGradeInfo.RigidityIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.RigidityIndex < this.RigiditycomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.RigiditycomboBox.SelectedIndex = m_qualGradeInfo.RigidityIndex;
                            else
                                this.RigiditycomboBox.SelectedIndex = this.RigiditycomboBox.Items.Count - 1;
                        } 
                        if (m_qualGradeInfo.RigidityIndex >= 0 && m_qualGradeInfo.RigidityIndex != 0X7F)
                        {
                            this.RigiditycheckBox.Checked = true;
                            this.RigiditycomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.RigiditygroupBox.Enabled = false;
                    }
                    //含水率
                    if (GlobalDataInterface.SystemStructWater)
                    {
                        this.WatergroupBox.Enabled = true;

                        this.WatercheckBox.Enabled = GlobalDataInterface.SystemStructWater;
                        for (int i = 0; i < GlobalDataInterface.WaterGradeNum; i++)
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stWaterGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            this.WatercomboBox.Items.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }
                        if (GlobalDataInterface.WaterGradeNum > 0 && m_qualGradeInfo.WaterIndex != 0x7f)
                        {
                            if (m_qualGradeInfo.WaterIndex < this.WatercomboBox.Items.Count) //Modify by ChengSk - 20180704
                                this.WatercomboBox.SelectedIndex = m_qualGradeInfo.WaterIndex;
                            else
                                this.WatercomboBox.SelectedIndex = this.WatercomboBox.Items.Count - 1;
                        }
                        if (m_qualGradeInfo.WaterIndex >= 0 && m_qualGradeInfo.WaterIndex != 0X7F)
                        {
                            this.WatercheckBox.Checked = true;
                            this.WatercomboBox.Enabled = true;
                        }
                    }
                    else
                    {
                        this.WatergroupBox.Enabled = false;
                    }
                }
                else
                {
                    this.RigiditygroupBox.Enabled = false;
                    this.WatergroupBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数OKbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数OKbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorcheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.ColorcomboBox.Enabled = true;
                    this.ColorcomboBox.SelectedIndex = GlobalDataInterface.ColorGradeNum - 1;
                }
                else
                {
                    this.ColorcomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数ColorcheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数ColorcheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 形状
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShapecheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.ShapecomboBox.Enabled = true;
                    this.ShapecomboBox.SelectedIndex = GlobalDataInterface.ShapeGradeNum - 1;
                }
                else
                {
                    this.ShapecomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数ShapecheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数ShapecheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 瑕疵
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlawcheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.FlawcomboBox.Enabled = true;
                    this.FlawcomboBox.SelectedIndex = GlobalDataInterface.FlawGradeNum - 1;
                }
                else
                {
                    this.FlawcomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数FlawcheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数FlawcheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 擦伤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BruisecheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.BruisecomboBox.Enabled = true;
                    this.BruisecomboBox.SelectedIndex = GlobalDataInterface.BruiseGradeNum - 1;
                }
                else
                {
                    this.BruisecomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数BruisecheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数BruisecheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 腐烂
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotcheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.RotcomboBox.Enabled = true;
                    this.RotcomboBox.SelectedIndex = GlobalDataInterface.RotGradeNum - 1;
                }
                else
                {
                    this.RotcomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数RotcheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数RotcheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 密度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DensitycheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.DensitycomboBox.Enabled = true;
                    this.DensitycomboBox.SelectedIndex = GlobalDataInterface.DensityGradeNum - 1;
                }
                else
                {
                    this.DensitycomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数DensitycheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数DensitycheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 糖度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SugarcheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.SugarcomboBox.Enabled = true;
                    this.SugarcomboBox.SelectedIndex = GlobalDataInterface.SugarGradeNum - 1;
                }
                else
                {
                    this.SugarcomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数SugarcheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数SugarcheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 酸度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AciditycheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.AciditycomboBox.Enabled = true;
                    this.AciditycomboBox.SelectedIndex = GlobalDataInterface.AcidityGradeNum - 1;
                }
                else
                {
                    this.AciditycomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数AciditycheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数AciditycheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 空心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HollowcheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.HollowcomboBox.Enabled = true;
                    this.HollowcomboBox.SelectedIndex = GlobalDataInterface.HollowGradeNum - 1;
                }
                else
                {
                    this.HollowcomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数HollowcheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数HollowcheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 浮皮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkincheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.SkincomboBox.Enabled = true;
                    this.SkincomboBox.SelectedIndex = GlobalDataInterface.SkinGradeNum - 1;
                }
                else
                {
                    this.SkincomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数SkincheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数SkincheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 褐变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowncheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.BrowncomboBox.Enabled = true;
                    this.BrowncomboBox.SelectedIndex = GlobalDataInterface.BrownGradeNum - 1;
                }
                else
                {
                    this.BrowncomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数SkincheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数SkincheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 糖心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TangxincheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.TangxincomboBox.Enabled = true;
                    this.TangxincomboBox.SelectedIndex = GlobalDataInterface.TangxinGradeNum - 1;
                }
                else
                {
                    this.TangxincomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数TangxincheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数TangxincheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 硬度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RigiditycheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.RigiditycomboBox.Enabled = true;
                    this.RigiditycomboBox.SelectedIndex = GlobalDataInterface.RigidityGradeNum - 1;
                }
                else
                {
                    this.RigiditycomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数RigiditycheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数RigiditycheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 含水率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WatercheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Checked)
                {
                    this.WatercomboBox.Enabled = true;
                    this.WatercomboBox.SelectedIndex = GlobalDataInterface.WaterGradeNum - 1;
                }
                else
                {
                    this.WatercomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数WatercheckBox_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数WatercheckBox_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.NametextBox.Text == "")
                {
                    //MessageBox.Show("名称不能为空！");
                    //MessageBox.Show("0x3000100D The grade name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("0x3000100D " + LanguageContainer.QualitySetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.QualitySetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (this.NametextBox.Text.Length > ConstPreDefine.MAX_TEXT_LENGTH)
                {
                    //MessageBox.Show("名称长度超过限制！");
                    //MessageBox.Show("0x3000100F The length of grade name is out of limit!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("0x3000100F " + LanguageContainer.QualitySetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.QualitySetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                m_qualGradeInfo.QualName = this.NametextBox.Text;
                //颜色
                if (this.ColorcheckBox.Checked)
                    m_qualGradeInfo.ColorIndex = this.ColorcomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.ColorIndex = -1;
                //形状
                if (this.ShapecheckBox.Checked)
                    m_qualGradeInfo.ShapeIndex = this.ShapecomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.ShapeIndex = -1;
                //瑕疵
                if (this.FlawcheckBox.Checked)
                    m_qualGradeInfo.FlawIndex = this.FlawcomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.FlawIndex = -1;
                //擦伤
                if (this.BruisecheckBox.Checked)
                    m_qualGradeInfo.BruiseIndex = this.BruisecomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.BruiseIndex = -1;
                //腐烂
                if (this.RotcheckBox.Checked)
                    m_qualGradeInfo.RotIndex = this.RotcomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.RotIndex = -1;
                //密度
                if (this.DensitycheckBox.Checked)
                    m_qualGradeInfo.DensityIndex = this.DensitycomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.DensityIndex = -1;
                //糖度
                if (this.SugarcheckBox.Checked)
                    m_qualGradeInfo.SugarIndex = this.SugarcomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.SugarIndex = -1;
                //酸度
                if (this.AciditycheckBox.Checked)
                    m_qualGradeInfo.AcidityIndex = this.AciditycomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.AcidityIndex = -1;
                //空心
                if (this.HollowcheckBox.Checked)
                    m_qualGradeInfo.HollowIndex = this.HollowcomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.HollowIndex = -1;
                //浮皮
                if (this.SkincheckBox.Checked)
                    m_qualGradeInfo.SkinIndex = this.SkincomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.SkinIndex = -1;
                //褐变
                if (this.BrowncheckBox.Checked)
                    m_qualGradeInfo.BrownIndex = this.BrowncomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.BrownIndex = -1;
                //糖心
                if (this.TangxincheckBox.Checked)
                    m_qualGradeInfo.TangxinIndex = this.TangxincomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.TangxinIndex = -1;
                //硬度
                if (this.RigiditycheckBox.Checked)
                    m_qualGradeInfo.RigidityIndex = this.RigiditycomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.RigidityIndex = -1;
                //含水率
                if (this.WatercheckBox.Checked)
                    m_qualGradeInfo.WaterIndex = this.WatercomboBox.SelectedIndex;
                else
                    m_qualGradeInfo.WaterIndex = -1;
                
                m_qaulGradeSetForm.SetQualGradeInfo(m_qualGradeInfo);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数OKbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数OKbutton_Click出错" + ex);
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
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数Cancelbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数Cancelbutton_Click出错" + ex);
#endif
            }
        }

        private void QualitySetForm_FormClosing(object sender, FormClosingEventArgs e) //Add by ChengSk - 20180717
        {
            try
            {
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualitySetForm中函数QualitySetForm_FormClosing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualitySetForm中函数QualitySetForm_FormClosing出错" + ex);
#endif
            }
        }

        

        

        

        

        

        

        

        

    }
}
