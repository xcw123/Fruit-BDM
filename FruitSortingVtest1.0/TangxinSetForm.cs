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
        byte[] m_TangxinGradeName = new byte[ConstPreDefine.MAX_BROWN_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        float[] m_fTangxinFactor = new float[ConstPreDefine.MAX_BROWN_GRADE_NUM];
        private Control[] TangxinSetEditors;//糖心列表点击显示控件

        /// <summary>
        /// 初始化
        /// </summary>
        private void TangxinSetInitial()
        {
            try
            {
                if (GlobalDataInterface.TangxinGradeNum == 0)
                    GlobalDataInterface.TangxinGradeNum = ConstPreDefine.MAX_TANGXIN_GRADE_NUM;

                this.TangxincomboBox.SelectedIndex = GlobalDataInterface.TangxinGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.stTangxinGradeName.CopyTo(m_TangxinGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.fTangxinFactor.CopyTo(m_fTangxinFactor, 0);
                SetTangxinListView();
                TangxinSetEditors = new Control[] { this.TangxinNametextBox, this.TangxinNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-TangxinSetForm中函数TangxinSetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-TangxinSetForm中函数TangxinSetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置褐变列表参数
        /// </summary>
        private void SetTangxinListView()
        {
            try
            {
                this.TangxinlistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.TangxincomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_TangxinGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.TangxincomboBox.SelectedIndex)
                        m_fTangxinFactor[i] = 0.000000f;
                    if (m_fTangxinFactor[i] == 0.000000f)
                        item.SubItems.Add("0.000000");
                    else
                        item.SubItems.Add(m_fTangxinFactor[i].ToString("#0.000000"));

                    this.TangxinlistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-TangxinSetForm中函数SetTangxinListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-TangxinSetForm中函数SetTangxinListView出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 糖心数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TangxincomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetTangxinListView();
        }

        /// <summary>
        /// 糖心列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TangxinlistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.TangxinlistViewEx.Items.Count - 1) && e.SubItem == 1))
                    this.TangxinlistViewEx.StartEditing(TangxinSetEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-TangxinSetForm中函数TangxinlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-TangxinSetForm中函数TangxinlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 糖心列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TangxinlistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_TangxinGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText);
                        Array.Copy(temp, 0, m_TangxinGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_fTangxinFactor[e.Item.Index] = float.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-TangxinSetForm中函数TangxinlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-TangxinSetForm中函数TangxinlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool TangxinSetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.TangxinlistViewEx.Items.Count; i++)
                {
                    if (this.TangxinlistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("糖心等级名称不能为空！");
                        //MessageBox.Show("0x3000102C The tangxin name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x3000102C " + LanguageContainer.TangxinSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.TangxinSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.TangxinlistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.TangxinlistViewEx.Items[j].SubItems[0].Text, this.TangxinlistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("糖心等级名称不能重名！");
                            //MessageBox.Show("0x3000102D The brown names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3000102D " + LanguageContainer.TangxinSetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.TangxinSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.TangxinlistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.TangxinlistViewEx.Items[j].SubItems[1].Text))
                        {
                            //MessageBox.Show(string.Format("糖心等级第{0}行的含糖量应大于第{1}行的含糖量", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s parameter should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.TangxinSetFormMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.TangxinSetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.TangxinSetFormMessagebox3Sub3Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1, j + 1),
                                LanguageContainer.TangxinSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                GlobalDataInterface.TangxinGradeNum = this.TangxincomboBox.SelectedIndex + 1;
                m_TangxinGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.stTangxinGradeName, 0);
                m_fTangxinFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.fTangxinFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-TangxinSetForm中函数TangxinSetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-TangxinSetForm中函数TangxinSetSaveConfig出错" + ex);
#endif
                return false;
            }
        }

    }
}
