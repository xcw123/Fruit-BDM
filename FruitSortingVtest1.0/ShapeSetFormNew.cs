using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interface;
using System.Drawing;
using GlacialComponents.Controls;
using Common;
using System.Diagnostics;

namespace FruitSortingVtest1._0
{
    public partial class QualityParamSetForm : Form
    {
        byte[] m_ShapeGradeName = new byte[ConstPreDefine.MAX_SHAPE_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        float[] m_fShapeFactor = new float[ConstPreDefine.MAX_SHAPE_GRADE_NUM];
        private Control[] ShapeSetEditors;//形状列表点击显示控件

        /// <summary>
        /// 初始化
        /// </summary>
        private void ShapeSetFormInitial()
        {
            try
            {
                if (GlobalDataInterface.ShapeGradeNum == 0)
                    GlobalDataInterface.ShapeGradeNum = ConstPreDefine.MAX_SHAPE_GRADE_NUM;

                this.ShapecomboBox.SelectedIndex = GlobalDataInterface.ShapeGradeNum - 1;
                GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName.CopyTo(m_ShapeGradeName, 0);
                GlobalDataInterface.globalOut_GradeInfo.fShapeFactor.CopyTo(m_fShapeFactor, 0);
                SetShapeListView();
                ShapeSetEditors = new Control[] { this.ShapeNametextBox, this.ShapeNumnumericUpDown };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ShapeSetForm中函数ShapeSetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ShapeSetForm中函数ShapeSetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置形状列表参数
        /// </summary>
        private void SetShapeListView()
        {
            try
            {
                this.ShapelistViewEx.Items.Clear();
                ListViewItem item;
                for (int i = 0; i < this.ShapecomboBox.SelectedIndex + 1; i++)
                {
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(m_ShapeGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);

                    item = new ListViewItem(Encoding.Default.GetString(temp).TrimEnd('\0'));
                    if (i == this.ShapecomboBox.SelectedIndex)
                        m_fShapeFactor[i] = 0.000000f;
                    if (m_fShapeFactor[i] == 0.000000f)
                        item.SubItems.Add("0.000000");
                    else
                        item.SubItems.Add(m_fShapeFactor[i].ToString("#0.000000"));

                    this.ShapelistViewEx.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ShapeSetForm中函数SetShapeListView出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ShapeSetForm中函数SetShapeListView出错" + ex);
#endif
            }
        }


        /// <summary>
        /// 形状数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShapecomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetShapeListView();
        }
        /// <summary>
        /// 形状列表单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShapelistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (!(e.Item.Index == (this.ShapelistViewEx.Items.Count - 1) && e.SubItem == 1))
                    this.ShapelistViewEx.StartEditing(ShapeSetEditors[e.SubItem], e.Item, e.SubItem);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ShapeSetForm中函数ShapelistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ShapeSetForm中函数ShapelistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 形状列表编辑完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShapelistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                switch (e.SubItem)
                {
                    case 0:
                        Array.Copy(temp, 0, m_ShapeGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        temp = Encoding.Default.GetBytes(e.DisplayText.Trim()); //去掉后缀空字符串 Modify by ChengSk - 20190118
                        Array.Copy(temp, 0, m_ShapeGradeName, e.Item.Index * ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
                        break;
                    case 1:
                        m_fShapeFactor[e.Item.Index] = float.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ShapeSetForm中函数ShapelistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ShapeSetForm中函数ShapelistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool ShapeSetSaveConfig()
        {
            try
            {
                for (int i = 0; i < this.ShapelistViewEx.Items.Count; i++)
                {
                    if (this.ShapelistViewEx.Items[i].SubItems[0].Text == "")
                    {
                       
                        //MessageBox.Show("0x3000101B The Shape name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("0x3000101B " + LanguageContainer.ShapeSetFormNewMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.ShapeSetFormNewMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < this.ShapelistViewEx.Items.Count; j++)
                    {
                        if (string.Equals(this.ShapelistViewEx.Items[j].SubItems[0].Text, this.ShapelistViewEx.Items[i].SubItems[0].Text))
                        {
                            
                            //MessageBox.Show("0x3000101C The Shape names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3000101C " + LanguageContainer.ShapeSetFormNewMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.ShapeSetFormNewMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        if (float.Parse(this.ShapelistViewEx.Items[i].SubItems[1].Text) < float.Parse(this.ShapelistViewEx.Items[j].SubItems[1].Text))
                        {
                            
                            //MessageBox.Show(string.Format("0x30001010 Invalid parameter!Row {0}'s parameter should be larger than Row {1}s'", i + 1, j + 1), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(string.Format("0x30001010 " + LanguageContainer.ShapeSetFormNewMessagebox3Sub1Text[GlobalDataInterface.selectLanguageIndex] + " {0} " +
                                LanguageContainer.ShapeSetFormNewMessagebox3Sub2Text[GlobalDataInterface.selectLanguageIndex] + " {1} " +
                                LanguageContainer.ShapeSetFormNewMessagebox3Sub3Text[GlobalDataInterface.selectLanguageIndex],
                                i + 1, j + 1),
                                LanguageContainer.ShapeSetFormNewMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                GlobalDataInterface.ShapeGradeNum = this.ShapecomboBox.SelectedIndex + 1;
                m_ShapeGradeName.CopyTo(GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, 0);
                m_fShapeFactor.CopyTo(GlobalDataInterface.globalOut_GradeInfo.fShapeFactor, 0);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualityParamSetForm-ShapeSetForm中函数ShapeSetSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ShapeSetForm中函数ShapeSetSaveConfig出错" + ex);
#endif
                return false;
            }
        }
    }
}
