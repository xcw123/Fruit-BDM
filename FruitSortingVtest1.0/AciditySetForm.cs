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
    public partial class QualityParamSetForm : Form
    {
        byte[] m_AcidityGradeName = new byte[ConstPreDefine.MAX_ACIDITY_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        float[] m_fAcidityFactor = new float[ConstPreDefine.MAX_ACIDITY_GRADE_NUM];
        private Control[] AciditySetEditors;//酸度列表点击显示控件

        /// <summary>
        /// 初始化
        /// </summary>
        private void AciditySetInitial()
        {
            try
            {
                if (GlobalDataInterface.AcidityGradeNum == 0)
                    GlobalDataInterface.AcidityGradeNum = ConstPreDefine.MAX_ACIDITY_GRADE_NUM;

                this.AciditycomboBox.SelectedIndex = GlobalDataInterface.AcidityGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.stAcidityGradeName.CopyTo(m_AcidityGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.fAcidityFactor.CopyTo(m_fAcidityFactor, 0);
                SetAcidityListView();
                AciditySetEditors = new Control[] { this.AcidityNametextBox, this.AcidityNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-AciditySetForm中函数AciditySetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-AciditySetForm中函数AciditySetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置酸度列表参数
        /// </summary>
        private void SetAcidityListView()
        {
            try
            {
                this.AciditylistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.AciditycomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_AcidityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.AciditycomboBox.SelectedIndex)
                        m_fAcidityFactor[i] = 0.000000f;
                    if (m_fAcidityFactor[i] == 0.000000f)
                        item.SubItems.Add("0.000000");
                    else
                        item.SubItems.Add(m_fAcidityFactor[i].ToString("#0.000000"));

                    this.AciditylistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-AciditySetForm中函数SetAcidityListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-AciditySetForm中函数SetAcidityListView出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 酸度数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AciditycomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetAcidityListView();
        }

        /// <summary>
        /// 酸度列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AciditylistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.AciditylistViewEx.Items.Count - 1) && e.SubItem == 1))
                    this.AciditylistViewEx.StartEditing(AciditySetEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-AciditySetForm中函数AciditylistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-AciditySetForm中函数AciditylistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 酸度列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AciditylistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_AcidityGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText);
                        Array.Copy(temp, 0, m_AcidityGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_fAcidityFactor[e.Item.Index] = float.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-AciditySetForm中函数AciditylistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-AciditySetForm中函数AciditylistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool AciditySetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.AciditylistViewEx.Items.Count; i++)
                {
                    if (this.AciditylistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("酸度等级名称不能为空！");
                        //MessageBox.Show("0x30001024 The acidity name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x30001024 " + LanguageContainer.AciditySetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.AciditySetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.AciditylistViewEx .Items.Count; j++)
                    {
                        if (string.Equals(this.AciditylistViewEx.Items[j].SubItems[0].Text, this.AciditylistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("酸度等级名称不能重名！");
                            //MessageBox.Show("0x30001025 The acidity names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x30001025 " + LanguageContainer.AciditySetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.AciditySetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.AciditylistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.AciditylistViewEx.Items[j].SubItems[1].Text))
                        {
                            //MessageBox.Show(string.Format("酸度等级第{0}行的含糖量应大于第{1}行的含糖量", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s parameter should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.AciditySetFormMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.AciditySetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.AciditySetFormMessagebox3Sub3Text[GlobalDataInterface.selectLanguageIndex], i + 1, j + 1),
                                LanguageContainer.AciditySetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                GlobalDataInterface.AcidityGradeNum = this.AciditycomboBox.SelectedIndex + 1;
                m_AcidityGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.stAcidityGradeName, 0);
                m_fAcidityFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.fAcidityFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-AciditySetForm中函数AciditySetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-AciditySetForm中函数AciditySetSaveConfig出错" + ex);
#endif
                return false;
            }
        }

    }
}
