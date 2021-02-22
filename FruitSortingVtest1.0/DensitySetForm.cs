using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interface;
using System.Collections;
using System.Diagnostics;

namespace FruitSortingVtest1._0
{
    public partial class QualityParamSetForm : Form
    {
        byte[] m_DensityGradeName = new byte[ConstPreDefine.MAX_DENSITY_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        float[] m_fDensityFactor = new float[ConstPreDefine.MAX_DENSITY_GRADE_NUM];
        private Control[] DensitySetEditors;//密度列表点击显示控件

        /// <summary>
        /// 初始化
        /// </summary>
        private void DensitySetInitial()
        {
            try
            {
                if (GlobalDataInterface.DensityGradeNum == 0)
                    GlobalDataInterface.DensityGradeNum = ConstPreDefine.MAX_DENSITY_GRADE_NUM;

                this.DensitycomboBox.SelectedIndex = GlobalDataInterface.DensityGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.stDensityGradeName.CopyTo(m_DensityGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.fDensityFactor.CopyTo(m_fDensityFactor, 0);
                SetDensityListView();
                DensitySetEditors = new Control[] { this.DensityNametextBox, this.DensityNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-DensitySeFormt中函数DensitySetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-DensitySeFormt中函数DensitySetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置密度列表参数
        /// </summary>
        private void SetDensityListView()
        {
            try
            {
                this.DensitylistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.DensitycomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_DensityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.DensitycomboBox.SelectedIndex)
                        m_fDensityFactor[i] = 0.0f;
                    if (m_fDensityFactor[i] == 0.0)
                        item.SubItems.Add("0.000000");
                    else
                        item.SubItems.Add(m_fDensityFactor[i].ToString("#0.000000"));

                    this.DensitylistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-DensitySeFormt中函数SetDensityListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-DensitySeFormt中函数SetDensityListView出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 密度等级数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DensitycomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetDensityListView();
        }

        /// <summary>
        /// 密度列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DensitylistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.DensitylistViewEx.Items.Count - 1) && e.SubItem == 1))
                    this.DensitylistViewEx.StartEditing(DensitySetEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-DensitySeFormt中函数DensitylistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-DensitySeFormt中函数DensitylistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 密度列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DensitylistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_DensityGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText.Trim()); //去掉后缀空字符串 Modify by ChengSk - 20190118
                        Array.Copy(temp, 0, m_DensityGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_fDensityFactor[e.Item.Index] = float.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-DensitySeFormt中函数DensitylistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-DensitySeFormt中函数DensitylistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool DensitySetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.DensitylistViewEx.Items.Count; i++)
                {
                    if (this.DensitylistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("密度等级名称不能为空！");
                        //MessageBox.Show("0x30001015 The density name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x30001015 " + LanguageContainer.DensitySetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.DensitySetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        return false;
                    }
                    for (int j = i + 1; j < this.DensitylistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.DensitylistViewEx.Items[j].SubItems[0].Text, this.DensitylistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("密度等级名称不能重名！");
                            //MessageBox.Show("0x30001016 The color names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x30001016 " + LanguageContainer.DensitySetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex], 
                                LanguageContainer.DensitySetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);                            
                            return false;
                        }

                        if (float.Parse(this.DensitylistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.DensitylistViewEx.Items[j].SubItems[1].Text))
                        {
                            //MessageBox.Show(string.Format("密度等级第{0}行的密度值应大于第{1}行的密度值", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s parameter should be larger than Row {1}s'",i+1,j+1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.DensitySetFormMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.DensitySetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.DensitySetFormMessagebox3Sub3Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1, j + 1),
                                LanguageContainer.DensitySetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);                        
                            return false;
                        }
                    }
                }
                GlobalDataInterface.DensityGradeNum = this.DensitycomboBox.SelectedIndex + 1;
                m_DensityGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.stDensityGradeName, 0);
                m_fDensityFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.fDensityFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-DensitySeFormt中函数DensitySetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-DensitySeFormt中函数DensitySetSaveConfig出错" + ex);
#endif
                return false;
            }
        }
    }
}
