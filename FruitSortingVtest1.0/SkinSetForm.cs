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
        byte[] m_SkinGradeName = new byte[ConstPreDefine.MAX_SKIN_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        float[] m_fSkinFactor = new float[ConstPreDefine.MAX_SKIN_GRADE_NUM];
        private Control[] SkinSetEditors;//浮皮列表点击显示控件

        /// <summary>
        /// 初始化
        /// </summary>
        private void SkinSetInitial()
        {
            try
            {
                if (GlobalDataInterface.SkinGradeNum == 0)
                    GlobalDataInterface.SkinGradeNum = ConstPreDefine.MAX_SKIN_GRADE_NUM;

                this.SkincomboBox.SelectedIndex = GlobalDataInterface.SkinGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.stSkinGradeName.CopyTo(m_SkinGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.fSkinFactor.CopyTo(m_fSkinFactor, 0);
                SetSkinListView();
                SkinSetEditors = new Control[] { this.SkinNametextBox, this.SkinNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-SkinSetForm中函数SkinSetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-SkinSetForm中函数SkinSetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置浮皮列表参数
        /// </summary>
        private void SetSkinListView()
        {
            try
            {
                this.SkinlistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.SkincomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_SkinGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.SkincomboBox.SelectedIndex)
                        m_fSkinFactor[i] = 0.000000f;
                    if (m_fSkinFactor[i] == 0.000000f)
                        item.SubItems.Add("0.000000");
                    else
                        item.SubItems.Add(m_fSkinFactor[i].ToString("#0.000000"));

                    this.SkinlistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-SkinSetForm中函数SetSkinListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-SkinSetForm中函数SetSkinListView出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 浮皮数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkincomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetSkinListView();
        }

        /// <summary>
        /// 浮皮列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkinlistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.SkinlistViewEx.Items.Count - 1) && e.SubItem == 1))
                    this.SkinlistViewEx.StartEditing(SkinSetEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-SkinSetForm中函数SkinlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-SkinSetForm中函数SkinlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 浮皮列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkinlistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_SkinGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText);
                        Array.Copy(temp, 0, m_SkinGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_fSkinFactor[e.Item.Index] = float.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-SkinSetForm中函数SkinlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-SkinSetForm中函数SkinlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool SkinSetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.SkinlistViewEx.Items.Count; i++)
                {
                    if (this.SkinlistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("干物质等级名称不能为空！");
                        //MessageBox.Show("0x30001028 The dry matter name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x30001028 " + LanguageContainer.SkinSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.SkinSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.SkinlistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.SkinlistViewEx.Items[j].SubItems[0].Text, this.SkinlistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("干物质等级名称不能重名！");
                            //MessageBox.Show("0x30001029 The dry matter names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x30001029 " + LanguageContainer.SkinSetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.SkinSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.SkinlistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.SkinlistViewEx.Items[j].SubItems[1].Text))
                        {
                            //MessageBox.Show(string.Format("干物质等级第{0}行的含糖量应大于第{1}行的含糖量", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s parameter should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.SkinSetFormMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] +
                                " {0} " + LanguageContainer.SkinSetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] +
                                " {1} " + LanguageContainer.SkinSetFormMessagebox3Sub3Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1, j + 1),
                                LanguageContainer.SkinSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                GlobalDataInterface.SkinGradeNum = this.SkincomboBox.SelectedIndex + 1;
                m_SkinGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.stSkinGradeName, 0);
                m_fSkinFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.fSkinFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-SkinSetForm中函数SkinSetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-SkinSetForm中函数SkinSetSaveConfig出错" + ex);
#endif
                return false;
            }
        }

    }
}
