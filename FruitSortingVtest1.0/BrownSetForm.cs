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
        byte[] m_BrownGradeName = new byte[ConstPreDefine.MAX_BROWN_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        float[] m_fBrownFactor = new float[ConstPreDefine.MAX_BROWN_GRADE_NUM];
        private Control[] BrownSetEditors;//褐变列表点击显示控件

        /// <summary>
        /// 初始化
        /// </summary>
        private void BrownSetInitial()
        {
            try
            {
                if (GlobalDataInterface.BrownGradeNum == 0)
                    GlobalDataInterface.BrownGradeNum = ConstPreDefine.MAX_BROWN_GRADE_NUM;

                this.BrowncomboBox.SelectedIndex = GlobalDataInterface.BrownGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.stBrownGradeName.CopyTo(m_BrownGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.fBrownFactor.CopyTo(m_fBrownFactor, 0);
                SetBrownListView();
                BrownSetEditors = new Control[] { this.BrownNametextBox, this.BrownNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BrownSetForm中函数BrownSetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BrownSetForm中函数BrownSetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置褐变列表参数
        /// </summary>
        private void SetBrownListView()
        {
            try
            {
                this.BrownlistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.BrowncomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_BrownGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.BrowncomboBox.SelectedIndex)
                        m_fBrownFactor[i] = 0.000000f;
                    if (m_fBrownFactor[i] == 0.000000f)
                        item.SubItems.Add("0.000000");
                    else
                        item.SubItems.Add(m_fBrownFactor[i].ToString("#0.000000"));

                    this.BrownlistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BrownSetForm中函数SetBrownListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BrownSetForm中函数SetBrownListView出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 褐变数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowncomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetBrownListView();
        }

        /// <summary>
        /// 褐变列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrownlistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.BrownlistViewEx.Items.Count - 1) && e.SubItem == 1))
                    this.BrownlistViewEx.StartEditing(BrownSetEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BrownSetForm中函数BrownlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BrownSetForm中函数BrownlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 褐变列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrownlistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_BrownGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText);
                        Array.Copy(temp, 0, m_BrownGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_fBrownFactor[e.Item.Index] = float.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BrownSetForm中函数BrownlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BrownSetForm中函数BrownlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool BrownSetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.BrownlistViewEx.Items.Count; i++)
                {
                    if (this.BrownlistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("褐变等级名称不能为空！");
                        //MessageBox.Show("0x3000102A The brown name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x3000102A " + LanguageContainer.BrownSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.BrownSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.BrownlistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.BrownlistViewEx.Items[j].SubItems[0].Text, this.BrownlistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("褐变等级名称不能重名！");
                            //MessageBox.Show("0x3000102B The brown names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3000102B " + LanguageContainer.BrownSetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.BrownSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.BrownlistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.BrownlistViewEx.Items[j].SubItems[1].Text))
                        {
                            //MessageBox.Show(string.Format("褐变等级第{0}行的含糖量应大于第{1}行的含糖量", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s parameter should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.BrownSetFormMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.BrownSetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.BrownSetFormMessagebox3Sub3Text[GlobalDataInterface.selectLanguageIndex], i + 1, j + 1),
                                LanguageContainer.BrownSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                GlobalDataInterface.BrownGradeNum = this.BrowncomboBox.SelectedIndex + 1;
                m_BrownGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.stBrownGradeName, 0);
                m_fBrownFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.fBrownFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-BrownSetForm中函数BrownSetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-BrownSetForm中函数BrownSetSaveConfig出错" + ex);
#endif
                return false;
            }
        }

    }
}
