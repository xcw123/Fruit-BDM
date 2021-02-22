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
        byte[] m_HollowGradeName = new byte[ConstPreDefine.MAX_HOLLOW_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        float[] m_fHollowFactor = new float[ConstPreDefine.MAX_HOLLOW_GRADE_NUM];
        private Control[] HollowSetEditors;//空心列表点击显示控件

        /// <summary>
        /// 初始化
        /// </summary>
        private void HollowSetInitial()
        {
            try
            {
                if (GlobalDataInterface.HollowGradeNum == 0)
                    GlobalDataInterface.HollowGradeNum = ConstPreDefine.MAX_HOLLOW_GRADE_NUM;

                this.HollowcomboBox.SelectedIndex = GlobalDataInterface.HollowGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.stHollowGradeName.CopyTo(m_HollowGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.fHollowFactor.CopyTo(m_fHollowFactor, 0);
                SetHollowListView();
                HollowSetEditors = new Control[] { this.HollowNametextBox, this.HollowNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-HollowSetForm中函数HollowSetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-HollowSetForm中函数HollowSetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置空心列表参数
        /// </summary>
        private void SetHollowListView()
        {
            try
            {
                this.HollowlistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.HollowcomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_HollowGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.HollowcomboBox.SelectedIndex)
                        m_fHollowFactor[i] = 0.000000f;
                    if (m_fHollowFactor[i] == 0.000000f)
                        item.SubItems.Add("0.000000");
                    else
                        item.SubItems.Add(m_fHollowFactor[i].ToString("#0.000000"));

                    this.HollowlistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-HollowSetForm中函数SetHollowListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-HollowSetForm中函数SetHollowListView出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 空心数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HollowcomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetHollowListView();
        }

        /// <summary>
        /// 空心列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HollowlistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.HollowlistViewEx.Items.Count - 1) && e.SubItem == 1))
                    this.HollowlistViewEx.StartEditing(HollowSetEditors[e.SubItem], e.Item, e.SubItem);
            }                                       
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-HollowSetForm中函数HollowlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-HollowSetForm中函数HollowlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 空心列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HollowlistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_HollowGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText);
                        Array.Copy(temp, 0, m_HollowGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_fHollowFactor[e.Item.Index] = float.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-HollowSetForm中函数HollowlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-HollowSetForm中函数HollowlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool HollowSetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.HollowlistViewEx.Items.Count; i++)
                {
                    if (this.HollowlistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("硬度等级名称不能为空！");
                        //MessageBox.Show("0x30001026 The hardness name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x30001026 " + LanguageContainer.HollowSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.HollowSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.HollowlistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.HollowlistViewEx.Items[j].SubItems[0].Text, this.HollowlistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("硬度等级名称不能重名！");
                            //MessageBox.Show("0x30001027 The hardness names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x30001027 " + LanguageContainer.HollowSetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.HollowSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.HollowlistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.HollowlistViewEx.Items[j].SubItems[1].Text))
                        {
                            //MessageBox.Show(string.Format("硬度等级第{0}行的含糖量应大于第{1}行的含糖量", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s parameter should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.HollowSetFormMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] +
                                " {0} " + LanguageContainer.HollowSetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.HollowSetFormMessagebox3Sub3Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1, j + 1),
                                LanguageContainer.HollowSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                GlobalDataInterface.HollowGradeNum = this.HollowcomboBox.SelectedIndex + 1;
                m_HollowGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.stHollowGradeName, 0);
                m_fHollowFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.fHollowFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-HollowSetForm中函数HollowSetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-HollowSetForm中函数HollowSetSaveConfig出错" + ex);
#endif
                return false;
            }
        }

    }
}
