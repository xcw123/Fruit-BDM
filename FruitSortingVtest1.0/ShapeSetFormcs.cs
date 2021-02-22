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
//        struct ShapeParam
//        {
//            public bool bCheck;
//            public string name;
//            public string str1;
//            public string str2;
//        };
//        ShapeParam[] m_shapeList = new ShapeParam[10];
//        string[] m_CheckGradeName = new string[3];
//        public void ShapeSetFormInitial()
//        {
//            try
//            {
//                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
//                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, 0, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
//                m_CheckGradeName[0] = Encoding.Default.GetString(temp).TrimEnd('\0');
//                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
//                m_CheckGradeName[1] = Encoding.Default.GetString(temp).TrimEnd('\0');
//                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, ConstPreDefine.MAX_TEXT_LENGTH * 2, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
//                m_CheckGradeName[2] = Encoding.Default.GetString(temp).TrimEnd('\0');

//                this.ShapeNametextBox1.Text = m_CheckGradeName[0];
//                this.ShapeNametextBox2.Text = m_CheckGradeName[1];
//                this.ShapeNamelabel1.Text = "<" + m_CheckGradeName[2] + m_resourceManager.GetString("Islabel.Text");
//                this.ShapeNamelabel2.Text = ">" + m_CheckGradeName[2] + m_resourceManager.GetString("Islabel.Text");
//                this.ShapeTypelabel.Text = m_CheckGradeName[2];

//                bool bCompare = false;
//                for (int i = 0; i < 10; i++)
//                {
//                    m_shapeList[i].name = Commonfunction.GetAppSetting(string.Format("形状参数{0}-类型", i));
//                    m_shapeList[i].str1 = Commonfunction.GetAppSetting(string.Format("形状参数{0}-小于", i));
//                    m_shapeList[i].str2 = Commonfunction.GetAppSetting(string.Format("形状参数{0}-大于", i));

//                    if (string.Equals(m_CheckGradeName[0], m_shapeList[i].str1) && string.Equals(m_CheckGradeName[1], m_shapeList[i].str2) && string.Equals(m_CheckGradeName[2], m_shapeList[i].name))
//                    {
//                        if (this.ShapeNametextBox1.Text != "" && this.ShapeNametextBox2.Text != "")
//                        {
//                            m_shapeList[i].bCheck = true;
//                            //this.ShapeTypelabel.Text = m_shapeList[i].name;
//                            //this.ShapeNamelabel1.Text = "小于" + m_shapeList[i].name + "为";
//                            //this.ShapeNamelabel2.Text = "大于" + m_shapeList[i].name + "为";
//                            bCompare = true;
//                        }
//                    }
//                    else
//                        m_shapeList[i].bCheck = false;
//                    if (!bCompare)
//                        this.ShapeTypelabel.Text = "";
//                    this.ShapeParamglacialList.Items[i].SubItems[0].Checked = m_shapeList[i].bCheck;
//                    this.ShapeParamglacialList.Items[i].SubItems[1].Text = m_shapeList[i].name;
//                    this.ShapeParamglacialList.Items[i].SubItems[2].Text = m_shapeList[i].str1;
//                    this.ShapeParamglacialList.Items[i].SubItems[3].Text = m_shapeList[i].str2;
//                }
//                this.ShapeTypelabel.Location = new Point(this.ShapeFactornumericUpDown.Location.X - this.ShapeTypelabel.Size.Width, this.ShapeTypelabel.Location.Y);
//                this.ShapeNamelabel1.Location = new Point(this.ShapeNametextBox1.Location.X - this.ShapeNamelabel1.Size.Width, this.ShapeNamelabel1.Location.Y);
//                this.ShapeNamelabel2.Location = new Point(this.ShapeNametextBox2.Location.X - this.ShapeNamelabel2.Size.Width, this.ShapeNamelabel2.Location.Y);
//                this.ShapeFactornumericUpDown.Text = GlobalDataInterface.globalOut_GradeInfo.fShapeFactor.ToString("#0.00");

//            }
//            catch (Exception ex)
//            {
//                Trace.WriteLine("QualityParamSetForm-ShapeSetForm中函数ShapeSetFormInitial出错" + ex);
//#if REALEASE
//                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ShapeSetForm中函数ShapeSetFormInitial出错" + ex);
//#endif
//            }
//        }

//        /// <summary>
//        /// 形状列表勾选事件
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="e"></param>
//        private void ShapeParamglacialList_SelectedIndexChanged(object source, GlacialComponents.Controls.ClickEventArgs e)
//        {
//            try
//            {
//                GlacialList glacialList = (GlacialList)source;
//                m_shapeList[e.ItemIndex].bCheck = glacialList.Items[e.ItemIndex].SubItems[e.ColumnIndex].Checked;
//                if (m_shapeList[e.ItemIndex].bCheck)
//                {
//                    this.ShapeTypelabel.Text = glacialList.Items[e.ItemIndex].SubItems[1].Text;
//                    this.ShapeNamelabel1.Text = "<" + glacialList.Items[e.ItemIndex].SubItems[1].Text + m_resourceManager.GetString("Islabel.Text");
//                    this.ShapeNamelabel2.Text = "<" + glacialList.Items[e.ItemIndex].SubItems[1].Text + m_resourceManager.GetString("Islabel.Text");
//                    this.ShapeNametextBox1.Text = glacialList.Items[e.ItemIndex].SubItems[2].Text;
//                    this.ShapeNametextBox2.Text = glacialList.Items[e.ItemIndex].SubItems[3].Text;
//                    m_CheckGradeName[0] = this.ShapeNametextBox1.Text;
//                    m_CheckGradeName[1] = this.ShapeNametextBox2.Text;
//                    m_CheckGradeName[2] = this.ShapeTypelabel.Text;
//                    this.ShapeTypelabel.Location = new Point(this.ShapeFactornumericUpDown.Location.X - this.ShapeTypelabel.Size.Width, this.ShapeTypelabel.Location.Y);
//                    this.ShapeNamelabel1.Location = new Point(this.ShapeNametextBox1.Location.X - this.ShapeNamelabel1.Size.Width, this.ShapeNamelabel1.Location.Y);
//                    this.ShapeNamelabel2.Location = new Point(this.ShapeNametextBox2.Location.X - this.ShapeNamelabel2.Size.Width, this.ShapeNamelabel2.Location.Y);
//                }
//                for (int i = 0; i < 10; i++)
//                {
//                    if (i != e.ItemIndex)
//                    {
//                        glacialList.Items[i].SubItems[0].Checked = false;
//                        m_shapeList[i].bCheck = false;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Trace.WriteLine("QualityParamSetForm-ShapeSetForm中函数ShapeParamglacialList_SelectedIndexChanged出错" + ex);
//#if REALEASE
//                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ShapeSetForm中函数ShapeParamglacialList_SelectedIndexChanged出错" + ex);
//#endif
//            }
//        }

//        /// <summary>
//        /// 形状保存函数
//        /// </summary>
//        /// <returns></returns>
//        public bool ShapeSetSaveConfig()
//        {
//            try
//            {
//                if (m_CheckGradeName[0] == string.Empty)
//                {
//                    //MessageBox.Show("形状等级名称不能为空！");
//                    MessageBox.Show("0x30001013 The shape name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                    this.ShapeNametextBox1.Focus();
//                    return false;
//                }
//                if (m_CheckGradeName[1] == string.Empty)
//                {
//                    //MessageBox.Show("形状等级名称不能为空！");
//                    MessageBox.Show("0x30001013 The shape name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                    this.ShapeNametextBox2.Focus();
//                    return false;
//                }
//                if (string.Equals(m_CheckGradeName[0], m_CheckGradeName[1]))
//                {
//                    //MessageBox.Show("形状等级名称不能重名！");
//                    MessageBox.Show("0x30001014 The shape names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                    this.ShapeNametextBox2.Focus();
//                    return false;
//                }
//                byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
//                Array.Copy(temp, 0, GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, ConstPreDefine.MAX_TEXT_LENGTH * 2, temp.Length);
//                temp = Encoding.Default.GetBytes(m_CheckGradeName[2]);
//                Array.Copy(temp, 0, GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, ConstPreDefine.MAX_TEXT_LENGTH * 2, temp.Length);
//                //m_CheckGradeName[2] = Encoding.Default.GetString(temp).TrimEnd('\0');

//                GlobalDataInterface.globalOut_GradeInfo.fShapeFactor = float.Parse(this.ShapeFactornumericUpDown.Text);
//                temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
//                Array.Copy(temp, 0, GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, 0, temp.Length);
//                temp = Encoding.Default.GetBytes(m_CheckGradeName[0]);
//                Array.Copy(temp, 0, GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, 0, temp.Length);

//                temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
//                Array.Copy(temp, 0, GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
//                temp = Encoding.Default.GetBytes(m_CheckGradeName[1]);
//                Array.Copy(temp, 0, GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, ConstPreDefine.MAX_TEXT_LENGTH, temp.Length);
//                //m_CheckGradeName[1] = Encoding.Default.GetString(temp).TrimEnd('\0');

//                for (int i = 0; i < 10; i++)
//                {
//                    Commonfunction.SetAppSetting(string.Format("形状参数{0}-类型", i), this.ShapeParamglacialList.Items[i].SubItems[1].Text);
//                    Commonfunction.SetAppSetting(string.Format("形状参数{0}-小于", i), this.ShapeParamglacialList.Items[i].SubItems[2].Text);
//                    Commonfunction.SetAppSetting(string.Format("形状参数{0}-大于", i), this.ShapeParamglacialList.Items[i].SubItems[3].Text);
//                }
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Trace.WriteLine("QualityParamSetForm-ShapeSetForm中函数ShapeSetSaveConfig出错" + ex);
//#if REALEASE
//                GlobalDataInterface.WriteErrorInfo("QualityParamSetForm-ShapeSetForm中函数ShapeSetSaveConfig出错" + ex);
//#endif
//                return false;
//            }
//        }

    }
}
