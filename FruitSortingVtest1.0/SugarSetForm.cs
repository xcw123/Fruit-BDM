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
        byte[] m_SugarGradeName = new byte[ConstPreDefine.MAX_SUGAR_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        float[] m_fSugarFactor = new float[ConstPreDefine.MAX_SUGAR_GRADE_NUM];
        private Control[] SugarSetEditors;//含糖量列表点击显示控件

        /// <summary>
        /// 初始化
        /// </summary>
        private void SugarSetInitial()
        {
            try
            {
                if (GlobalDataInterface.SugarGradeNum == 0)
                    GlobalDataInterface.SugarGradeNum = ConstPreDefine.MAX_SUGAR_GRADE_NUM;

                this.SugarcomboBox.SelectedIndex = GlobalDataInterface.SugarGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.stSugarGradeName.CopyTo(m_SugarGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.fSugarFactor.CopyTo(m_fSugarFactor, 0);
                SetSugarListView();
                SugarSetEditors = new Control[] { this.SugarNametextBox, this.SugarNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-SugarSetForm中函数SugarSetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-SugarSetForm中函数SugarSetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置含糖量列表参数
        /// </summary>
        private void SetSugarListView()
        {
            try
            {
                this.SugarlistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.SugarcomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_SugarGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.SugarcomboBox.SelectedIndex)
                        m_fSugarFactor[i] = 0.000000f;
                    if (m_fSugarFactor[i] == 0.000000f)
                        item.SubItems.Add("0.000000");
                    else
                        item.SubItems.Add(m_fSugarFactor[i].ToString("#0.000000"));

                    this.SugarlistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-SugarSetForm中函数SetSugarListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-SugarSetForm中函数SetSugarListView出错" + ex);
#endif
            }
        }

      
        /// <summary>
        /// 含糖量数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SugarcomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetSugarListView();
        }
        /// <summary>
        /// 含糖量列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SugarlistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.SugarlistViewEx.Items.Count - 1) && e.SubItem == 1))
                    this.SugarlistViewEx.StartEditing(SugarSetEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-SugarSetForm中函数SugarlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-SugarSetForm中函数SugarlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 含糖量列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SugarlistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_SugarGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText.Trim()); //去掉后缀空字符串 Modify by ChengSk - 20190118
                        Array.Copy(temp, 0, m_SugarGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_fSugarFactor[e.Item.Index] = float.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-SugarSetForm中函数SugarlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-SugarSetForm中函数SugarlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool SugarSetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.SugarlistViewEx.Items.Count; i++)
                {
                    if (this.SugarlistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("含糖量等级名称不能为空！");
                        //MessageBox.Show("0x3000101B The sugar name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x3000101B " + LanguageContainer.SugarSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.SugarSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.SugarlistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.SugarlistViewEx.Items[j].SubItems[0].Text, this.SugarlistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("含糖量等级名称不能重名！");
                            //MessageBox.Show("0x3000101C The sugar names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3000101C " + LanguageContainer.SugarSetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.SugarSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.SugarlistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.SugarlistViewEx.Items[j].SubItems[1].Text))
                        {
                            //MessageBox.Show(string.Format("含糖量等级第{0}行的含糖量应大于第{1}行的含糖量", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s parameter should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.SugarSetFormMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.SugarSetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.SugarSetFormMessagebox3Sub3Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1, j + 1),
                                LanguageContainer.SugarSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                GlobalDataInterface.SugarGradeNum = this.SugarcomboBox.SelectedIndex + 1;
                m_SugarGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.stSugarGradeName, 0);
                m_fSugarFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.fSugarFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-SugarSetForm中函数SugarSetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-SugarSetForm中函数SugarSetSaveConfig出错" + ex);
#endif
                return false;
            }
        }
    }
}
