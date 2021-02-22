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
        byte[] m_WaterGradeName = new byte[ConstPreDefine.MAX_WATER_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        float[] m_fWaterFactor = new float[ConstPreDefine.MAX_WATER_GRADE_NUM];
        private Control[] WaterSetEditors;//含水率列表点击显示控件

        /// <summary>
        /// 初始化
        /// </summary>
        private void WaterSetInitial()
        {
            try
            {
                if (GlobalDataInterface.WaterGradeNum == 0)
                    GlobalDataInterface.WaterGradeNum = ConstPreDefine.MAX_WATER_GRADE_NUM;

                this.WatercomboBox.SelectedIndex = GlobalDataInterface.WaterGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.stWaterGradeName.CopyTo(m_WaterGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.fWaterFactor.CopyTo(m_fWaterFactor, 0);
                SetWaterListView();
                WaterSetEditors = new Control[] { this.WaterNametextBox, this.WaterNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-WaterSeForm中函数WaterSetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-WaterSeForm中函数WaterSetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置含水率列表参数
        /// </summary>
        private void SetWaterListView()
        {
            try
            {
                this.WaterlistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.WatercomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_WaterGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.WatercomboBox.SelectedIndex)
                        m_fWaterFactor[i] = 0.0f;
                    if (m_fWaterFactor[i] == 0.0)
                        item.SubItems.Add("0.000000");
                    else
                        item.SubItems.Add(m_fWaterFactor[i].ToString("#0.000000"));

                    this.WaterlistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-WaterSetForm中函数SetWaterListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-WaterSetForm中函数SetWaterListView出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 含水率等级设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WatercomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetWaterListView();
        }

        /// <summary>
        /// 含水率列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WaterlistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.WaterlistViewEx.Items.Count - 1) && e.SubItem == 1))
                    this.WaterlistViewEx.StartEditing(WaterSetEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-WaterSetForm中函数WaterlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-WaterSetForm中函数WaterlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 含水率列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WaterlistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_WaterGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText);
                        Array.Copy(temp, 0, m_WaterGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_fWaterFactor[e.Item.Index] = float.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-WaterSetForm中函数WaterlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-WaterSetForm中函数WaterlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool WaterSetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.WaterlistViewEx.Items.Count; i++)
                {
                    if (this.WaterlistViewEx.Items[i].SubItems[0].Text == "")
                    {
                        //MessageBox.Show("含水率等级名称不能为空！");
                        //MessageBox.Show("0x30001022 The water name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x30001022 " + LanguageContainer.WaterSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.WaterSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.WaterlistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.WaterlistViewEx.Items[j].SubItems[0].Text, this.WaterlistViewEx.Items[i].SubItems[0].Text))
                        {
                            //MessageBox.Show("含水率等级名称不能重名！");
                            //MessageBox.Show("0x30001023 The water names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x30001023 " + LanguageContainer.WaterSetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.WaterSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.WaterlistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.WaterlistViewEx.Items[j].SubItems[1].Text))
                        {
                            //MessageBox.Show(string.Format("含水率等级第{0}行的硬度值应大于第{1}行的硬度值", i, j));
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s parameter should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.WaterSetFormMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.WaterSetFormMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.WaterSetFormMessagebox3Sub3Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1, j + 1),
                                LanguageContainer.WaterSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                GlobalDataInterface.WaterGradeNum = this.WatercomboBox.SelectedIndex + 1;
                m_WaterGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.stWaterGradeName, 0);
                m_fWaterFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.fWaterFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-WaterSetForm中函数WaterSetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-WaterSetForm中函数WaterSetSaveConfig出错" + ex);
#endif
                return false;
            }
        }




    }
}
