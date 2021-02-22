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
        byte[] m_RigidityGradeName = new byte[ConstPreDefine.MAX_RIGIDITY_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        float[] m_fRigidityFactor = new float[ConstPreDefine.MAX_RIGIDITY_GRADE_NUM];
        private Control[] RigiditySetEditors;//硬度列表点击显示控件

        /// <summary>
        /// 初始化
        /// </summary>
        private void RigiditySetInitial()
        {
            try
            {
                if (GlobalDataInterface.RigidityGradeNum == 0)
                    GlobalDataInterface.RigidityGradeNum = ConstPreDefine.MAX_RIGIDITY_GRADE_NUM;

                this.RigiditycomboBox.SelectedIndex = GlobalDataInterface.RigidityGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.stRigidityGradeName.CopyTo(m_RigidityGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.fRigidityFactor.CopyTo(m_fRigidityFactor, 0);
                SetRigidityListView();
                RigiditySetEditors = new Control[] { this.RigidityNametextBox, this.RigidityNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RigiditySeForm中函数RigiditySetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RigiditySeForm中函数RigiditySetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置硬度列表参数
        /// </summary>
        private void SetRigidityListView()
        {
            try
            {
                this.RigiditylistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.RigiditycomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_RigidityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.RigiditycomboBox.SelectedIndex)
                        m_fRigidityFactor[i] = 0.0f;
                    if (m_fRigidityFactor[i] == 0.0)
                        item.SubItems.Add("0.000000");
                    else
                        item.SubItems.Add(m_fRigidityFactor[i].ToString("#0.000000"));

                    this.RigiditylistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RigiditySeForm中函数SetRigidityListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RigiditySeForm中函数SetRigidityListView出错" + ex);
#endif
            }
        }       
        /// <summary>
        /// 硬度等级数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RigiditycomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetRigidityListView();

        }
        /// <summary>
        /// 硬度列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RigiditylistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.RigiditylistViewEx.Items.Count - 1) && e.SubItem == 1))
                    this.RigiditylistViewEx.StartEditing(RigiditySetEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RigiditySeForm中函数RigiditylistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RigiditySeForm中函数RigiditylistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 硬度列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RigiditylistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_RigidityGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText.Trim()); //去掉后缀空字符串 Modify by ChengSk - 20190118
                        Array.Copy(temp, 0, m_RigidityGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_fRigidityFactor[e.Item.Index] = float.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RigiditySeForm中函数RigiditylistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RigiditySeForm中函数RigiditylistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool RigiditySetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.RigiditylistViewEx.Items.Count; i++)
                {
                    if (this.RigiditylistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("硬度等级名称不能为空！");
                        //MessageBox.Show("0x30001019 The hardness name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x30001019 " + LanguageContainer.RigiditySetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.RigiditySetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.RigiditylistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.RigiditylistViewEx.Items[j].SubItems[0].Text, this.RigiditylistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("硬度等级名称不能重名！");
                            //MessageBox.Show("0x3000101A The hardness names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3000101A " + LanguageContainer.RigiditySetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.RigiditySetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.RigiditylistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.RigiditylistViewEx.Items[j].SubItems[1].Text))
                        {
                            //MessageBox.Show(string.Format("硬度等级第{0}行的硬度值应大于第{1}行的硬度值", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s parameter should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.RigiditySetFormMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.RigiditySetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.RigiditySetFormMessagebox3Sub3Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1, j + 1),
                                LanguageContainer.RigiditySetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                GlobalDataInterface.RigidityGradeNum = this.RigiditycomboBox.SelectedIndex + 1;
                m_RigidityGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.stRigidityGradeName, 0);
                m_fRigidityFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.fRigidityFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-RigiditySeForm中函数RigiditySetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-RigiditySeForm中函数RigiditySetSaveConfig出错" + ex);
#endif
                return false;
            }
        }
    }
}
