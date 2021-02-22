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
using System.Resources;
using System.Threading.Tasks;

namespace FruitSortingVtest1._0
{
    public partial class QualGradeSetForm : Form
    {
        QualityGradeInfo temp_qulityGradeInfo = new QualityGradeInfo(true);
        bool m_QualGradeSetlistViewExMouseDoubleClick = false;
        QaulGradeInfo m_qualGradeInfo;
        MainForm m_mainForm;
        int OldGradeCnt = 0; //已保存等级的数量 - Add by ChengSk - 20191118
        private ResourceManager m_resourceManager = new ResourceManager(typeof(QualGradeSetForm));//创建QualGradeSetForm资源管理

        public QualGradeSetForm(MainForm mainForm)
        {
            m_mainForm = mainForm;
            InitializeComponent();
        }

        private void QualGradeSetForm_Load(object sender, EventArgs e)
        {
            try
            {
                temp_qulityGradeInfo.ToCopy(GlobalDataInterface.Quality_GradeInfo);
                OldGradeCnt = temp_qulityGradeInfo.GradeCnt;
                ResetQualGradeSetlistViewExColumns();

                //if (temp_qulityGradeInfo.GradeCnt == 0)
                //{
                //    this.QualGradeSetcomboBox.Enabled = false;
                //    this.QualGradeSetlistViewEx.Enabled = false;
                //}
                //else
                //{
                //    this.QualGradeSetcomboBox.Enabled = true;
                //    this.QualGradeSetlistViewEx.Enabled = true;
                //}
                this.QualGradeSetcomboBox.Enabled = true;
                this.QualGradeSetlistViewEx.Enabled = true;

                //this.QualGradeSetcomboBox.SelectedIndex = temp_qulityGradeInfo.GradeCnt - 1;
                this.QualGradeSetcomboBox.SelectedIndex = temp_qulityGradeInfo.GradeCnt;

                SetQualGradeSetlistViewEx();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualGradeSetForm 中函数QualGradeSetForm_Load出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualGradeSetForm 中函数QualGradeSetForm_Load出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 重新设置列宽 Add by ChengSk - 20200716
        /// </summary>
        private void ResetQualGradeSetlistViewExColumns()
        {
            try
            {
                //if (temp_qulityGradeInfo.Item[0].ColorGrade == 0x7f && !GlobalDataInterface.SystemStructColor)
                //    this.QualGradeSetlistViewEx.Columns[1].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[1].Width = 70;

                //if (temp_qulityGradeInfo.Item[0].sbShapeGrade == 0x7f && !GlobalDataInterface.SystemStructShape)
                //    this.QualGradeSetlistViewEx.Columns[2].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[2].Width = 70;

                //if (temp_qulityGradeInfo.Item[0].sbFlaw == 0x7f && !GlobalDataInterface.SystemStructFlaw)
                //    this.QualGradeSetlistViewEx.Columns[3].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[3].Width = 70;


                //if (temp_qulityGradeInfo.Item[0].sbBruise == 0x7f && !GlobalDataInterface.SystemStructBruise)
                //    this.QualGradeSetlistViewEx.Columns[4].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[4].Width = 70;

                //if (temp_qulityGradeInfo.Item[0].sbRot == 0x7f && !GlobalDataInterface.SystemStructRot)
                //    this.QualGradeSetlistViewEx.Columns[5].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[5].Width = 70;

                //if (temp_qulityGradeInfo.Item[0].sbDensity == 0x7f && !GlobalDataInterface.SystemStructDensity)
                //    this.QualGradeSetlistViewEx.Columns[6].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[6].Width = 70;

                //if (temp_qulityGradeInfo.Item[0].sbSugar == 0x7f && !GlobalDataInterface.SystemStructSugar)
                //    this.QualGradeSetlistViewEx.Columns[7].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[7].Width = 70;

                //if (temp_qulityGradeInfo.Item[0].sbAcidity == 0x7f && !GlobalDataInterface.SystemStructAcidity)
                //    this.QualGradeSetlistViewEx.Columns[8].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[8].Width = 70;

                //if (temp_qulityGradeInfo.Item[0].sbHollow == 0x7f && !GlobalDataInterface.SystemStructHollow)
                //    this.QualGradeSetlistViewEx.Columns[9].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[9].Width = 70;

                //if (temp_qulityGradeInfo.Item[0].sbSkin == 0x7f && !GlobalDataInterface.SystemStructSkin)
                //    this.QualGradeSetlistViewEx.Columns[10].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[10].Width = 70;

                //if (temp_qulityGradeInfo.Item[0].sbBrown == 0x7f && !GlobalDataInterface.SystemStructBrown)
                //    this.QualGradeSetlistViewEx.Columns[11].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[11].Width = 70;

                //if (temp_qulityGradeInfo.Item[0].sbTangxin == 0x7f && !GlobalDataInterface.SystemStructTangxin)
                //    this.QualGradeSetlistViewEx.Columns[12].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[12].Width = 70;

                //if (temp_qulityGradeInfo.Item[0].sbRigidity == 0x7f && !GlobalDataInterface.SystemStructRigidity)
                //    this.QualGradeSetlistViewEx.Columns[13].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[13].Width = 70;

                //if (temp_qulityGradeInfo.Item[0].sbWater == 0x7f && !GlobalDataInterface.SystemStructWater)
                //    this.QualGradeSetlistViewEx.Columns[14].Width = 0;
                //else
                //    this.QualGradeSetlistViewEx.Columns[14].Width = 70;
                if (GlobalDataInterface.SystemStructColor && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x01) == 1))
                    this.QualGradeSetlistViewEx.Columns[1].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[1].Width = 0;

                if (GlobalDataInterface.SystemStructShape && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x01) == 1))
                    this.QualGradeSetlistViewEx.Columns[2].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[2].Width = 0;

                if (GlobalDataInterface.SystemStructFlaw && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x01) == 1))
                    this.QualGradeSetlistViewEx.Columns[3].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[3].Width = 0;


                if (GlobalDataInterface.SystemStructBruise && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x02) == 2))
                    this.QualGradeSetlistViewEx.Columns[4].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[4].Width = 0;

                if (GlobalDataInterface.SystemStructRot && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x02) == 2))
                    this.QualGradeSetlistViewEx.Columns[5].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[5].Width = 0;

                if (GlobalDataInterface.SystemStructDensity && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x04) == 4))
                    this.QualGradeSetlistViewEx.Columns[6].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[6].Width = 0;

                if (GlobalDataInterface.SystemStructSugar && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8))
                    this.QualGradeSetlistViewEx.Columns[7].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[7].Width = 0;

                if (GlobalDataInterface.SystemStructAcidity && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8))
                    this.QualGradeSetlistViewEx.Columns[8].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[8].Width = 0;

                if (GlobalDataInterface.SystemStructHollow && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8))
                    this.QualGradeSetlistViewEx.Columns[9].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[9].Width = 0;

                if (GlobalDataInterface.SystemStructSkin && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8))
                    this.QualGradeSetlistViewEx.Columns[10].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[10].Width = 0;

                if (GlobalDataInterface.SystemStructBrown && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8))
                    this.QualGradeSetlistViewEx.Columns[11].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[11].Width = 0;

                if (GlobalDataInterface.SystemStructTangxin && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8))
                    this.QualGradeSetlistViewEx.Columns[12].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[12].Width = 0;

                if (GlobalDataInterface.SystemStructRigidity && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x10) == 16))
                    this.QualGradeSetlistViewEx.Columns[13].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[13].Width = 0;

                if (GlobalDataInterface.SystemStructWater && ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x10) == 16))
                    this.QualGradeSetlistViewEx.Columns[14].Width = 70;
                else
                    this.QualGradeSetlistViewEx.Columns[14].Width = 0;
            }
            catch (Exception ee)
            {
                Trace.WriteLine("QualGradeSetForm 中函数ResetQualGradeSetlistViewExColumns出错" + ee);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualGradeSetForm 中函数ResetQualGradeSetlistViewExColumns出错" + ee);
#endif
            }
        }
        /// <summary>
        /// 设置品质等级列表
        /// </summary>
        private void SetQualGradeSetlistViewEx()
        {
            try
            {
                if (temp_qulityGradeInfo.GradeCnt == 0)
                {
                    this.QualGradeSetlistViewEx.Items.Clear();
                }
                else if (temp_qulityGradeInfo.GradeCnt > 0)
                {
                    //MessageBox.Show("OldGradeCnt=" + OldGradeCnt + " temp_qulityGradeInfo.GradeCnt=" + temp_qulityGradeInfo.GradeCnt);

                    try
                    {
                        //New Add by ChengSk - 20191118
                        if (OldGradeCnt < temp_qulityGradeInfo.GradeCnt) //等级数量增加
                        {
                            int diff = temp_qulityGradeInfo.GradeCnt - OldGradeCnt;
                            for (int i = temp_qulityGradeInfo.GradeCnt; i > diff; i--)
                            {
                                temp_qulityGradeInfo.Item[i - 1] = new QaulityGradeItem(true);
                                temp_qulityGradeInfo.Item[i - 1].ToCopy(temp_qulityGradeInfo.Item[i - 1 - diff]); //向后平移
                            }
                            for (int i = 0; i < diff; i++)
                            {
                                temp_qulityGradeInfo.Item[i] = new QaulityGradeItem(true); //新增加等级
                            }
                        }
                        else if (OldGradeCnt > temp_qulityGradeInfo.GradeCnt) //等级数量减少 Modify by ChengSk - 20191207
                        {
                            int diff = OldGradeCnt - temp_qulityGradeInfo.GradeCnt;
                            for (int i = 0; i < temp_qulityGradeInfo.GradeCnt; i++)
                            {
                                temp_qulityGradeInfo.Item[i] = new QaulityGradeItem(true);
                                temp_qulityGradeInfo.Item[i].ToCopy(temp_qulityGradeInfo.Item[i + diff]); //向前平移
                            }
                            for (int i = temp_qulityGradeInfo.GradeCnt; i < OldGradeCnt; i++)
                            {
                                temp_qulityGradeInfo.Item[i] = new QaulityGradeItem(true);
                            }
                        }
                        else
                        {
                            //等级数量相同，不做处理  Add by ChengSk - 20191207
                        }
                    }
                    catch (Exception ee)
                    {
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo("QualGradeSetForm 中函数‘等级数量变化’出错：" + ee.ToString());
#endif
                    }
                    //End

                    //MessageBox.Show("进来了");

                    ListViewItem item;
                    this.QualGradeSetlistViewEx.Items.Clear();
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];

                    for (int i = 0; i < temp_qulityGradeInfo.GradeCnt; i++)
                    {
                        if (temp_qulityGradeInfo.Item[i].ColorGrade != 0x7f && temp_qulityGradeInfo.Item[i].ColorGrade > GlobalDataInterface.ColorGradeNum)
                            temp_qulityGradeInfo.Item[i].ColorGrade = (sbyte)(GlobalDataInterface.ColorGradeNum - 1);
                        //if (Encoding.Default.GetString(GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName).TrimEnd('\0') != "")
                        //{
                        //    if (temp_qulityGradeInfo.Item[i].sbShapeGrade != 0x7f && temp_qulityGradeInfo.Item[i].sbShapeGrade > 0)
                        //        temp_qulityGradeInfo.Item[i].sbShapeGrade = 0x7f;
                        //}
                        //else
                        //{
                        //    if (temp_qulityGradeInfo.Item[i].sbShapeGrade != 0x7f && temp_qulityGradeInfo.Item[i].sbShapeGrade > 2)
                        //        temp_qulityGradeInfo.Item[i].sbShapeGrade = 1;
                        //}
                        if (temp_qulityGradeInfo.Item[i].sbShapeGrade != 0x7f && temp_qulityGradeInfo.Item[i].sbShapeGrade > GlobalDataInterface.ShapeGradeNum)
                            temp_qulityGradeInfo.Item[i].sbShapeGrade = (sbyte)(GlobalDataInterface.ShapeGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbFlaw != 0x7f && temp_qulityGradeInfo.Item[i].sbFlaw > GlobalDataInterface.FlawGradeNum)
                            temp_qulityGradeInfo.Item[i].sbFlaw = (sbyte)(GlobalDataInterface.FlawGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbBruise != 0x7f && temp_qulityGradeInfo.Item[i].sbBruise > GlobalDataInterface.BruiseGradeNum)
                            temp_qulityGradeInfo.Item[i].sbBruise = (sbyte)(GlobalDataInterface.BruiseGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbRot != 0x7f && temp_qulityGradeInfo.Item[i].sbRot > GlobalDataInterface.RotGradeNum)
                            temp_qulityGradeInfo.Item[i].sbRot = (sbyte)(GlobalDataInterface.RotGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbDensity != 0x7f && temp_qulityGradeInfo.Item[i].sbDensity > GlobalDataInterface.DensityGradeNum)
                            temp_qulityGradeInfo.Item[i].sbDensity = (sbyte)(GlobalDataInterface.DensityGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbSugar != 0x7f && temp_qulityGradeInfo.Item[i].sbSugar > GlobalDataInterface.SugarGradeNum)
                            temp_qulityGradeInfo.Item[i].sbSugar = (sbyte)(GlobalDataInterface.SugarGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbAcidity != 0x7f && temp_qulityGradeInfo.Item[i].sbAcidity > GlobalDataInterface.AcidityGradeNum)
                            temp_qulityGradeInfo.Item[i].sbAcidity = (sbyte)(GlobalDataInterface.AcidityGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbHollow != 0x7f && temp_qulityGradeInfo.Item[i].sbHollow > GlobalDataInterface.HollowGradeNum)
                            temp_qulityGradeInfo.Item[i].sbHollow = (sbyte)(GlobalDataInterface.HollowGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbSkin != 0x7f && temp_qulityGradeInfo.Item[i].sbSkin > GlobalDataInterface.SkinGradeNum)
                            temp_qulityGradeInfo.Item[i].sbSkin = (sbyte)(GlobalDataInterface.SkinGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbBrown != 0x7f && temp_qulityGradeInfo.Item[i].sbBrown > GlobalDataInterface.BrownGradeNum)
                            temp_qulityGradeInfo.Item[i].sbBrown = (sbyte)(GlobalDataInterface.BrownGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbTangxin != 0x7f && temp_qulityGradeInfo.Item[i].sbTangxin > GlobalDataInterface.TangxinGradeNum)
                            temp_qulityGradeInfo.Item[i].sbTangxin = (sbyte)(GlobalDataInterface.TangxinGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbRigidity != 0x7f && temp_qulityGradeInfo.Item[i].sbRigidity > GlobalDataInterface.RigidityGradeNum)
                            temp_qulityGradeInfo.Item[i].sbRigidity = (sbyte)(GlobalDataInterface.RigidityGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbWater != 0x7f && temp_qulityGradeInfo.Item[i].sbWater > GlobalDataInterface.WaterGradeNum)
                            temp_qulityGradeInfo.Item[i].sbWater = (sbyte)(GlobalDataInterface.WaterGradeNum - 1);

                        item = new ListViewItem(Encoding.Default.GetString(temp_qulityGradeInfo.Item[i].GradeName).TrimEnd('\0'));
                        if (temp_qulityGradeInfo.Item[i].GradeName[0] == 0)
                        {
                            //if (i == 0)
                            //{
                            //    item.SubItems[0].Text = "所有等级";
                            //    temp_qulityGradeInfo.Item[i].GradeName = Encoding.Default.GetBytes(item.SubItems[0].Text);
                            //}
                            //else
                            //{
                            //item.SubItems[0].Text = m_resourceManager.GetString("NewGradelabel.Text")+(i+1).ToString();
                            item.SubItems[0].Text = m_resourceManager.GetString("NewGradelabel.Text");//Modify by ChengSk - 20191118
                            temp_qulityGradeInfo.Item[i].GradeName = Encoding.Default.GetBytes(item.SubItems[0].Text);
                            //}
                        }
                        if (temp_qulityGradeInfo.Item[i].ColorGrade == 0x7f)
                        {
                            //if (i == 0 && GlobalDataInterface.ColorGradeNum > 0)
                            //{
                            //    byte[] tempName= new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                            //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strColorGradeName, (GlobalDataInterface.ColorGradeNum-1)*ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            //    item.SubItems.Add(Encoding.Default.GetString(tempName).TrimEnd('\0'));
                            //}
                            //else
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        }
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strColorGradeName, temp_qulityGradeInfo.Item[i].ColorGrade * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbShapeGrade == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, temp_qulityGradeInfo.Item[i].sbShapeGrade * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbFlaw == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stFlawareaGradeName, temp_qulityGradeInfo.Item[i].sbFlaw * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbBruise == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stBruiseGradeName, temp_qulityGradeInfo.Item[i].sbBruise * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbRot == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stRotGradeName, temp_qulityGradeInfo.Item[i].sbRot * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbDensity == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stDensityGradeName, temp_qulityGradeInfo.Item[i].sbDensity * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbSugar == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stSugarGradeName, temp_qulityGradeInfo.Item[i].sbSugar * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbAcidity == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stAcidityGradeName, temp_qulityGradeInfo.Item[i].sbAcidity * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbHollow == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stHollowGradeName, temp_qulityGradeInfo.Item[i].sbHollow * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbSkin == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stSkinGradeName, temp_qulityGradeInfo.Item[i].sbSkin * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbBrown == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stBrownGradeName, temp_qulityGradeInfo.Item[i].sbBrown * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbTangxin == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stTangxinGradeName, temp_qulityGradeInfo.Item[i].sbTangxin * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbRigidity == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stRigidityGradeName, temp_qulityGradeInfo.Item[i].sbRigidity * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        if (temp_qulityGradeInfo.Item[i].sbWater == 0x7f)
                            item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stWaterGradeName, temp_qulityGradeInfo.Item[i].sbWater * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                        }

                        //if (i == 0)
                        this.QualGradeSetlistViewEx.Items.Add(item);
                        //else
                        //    this.QualGradeSetlistViewEx.Items.Insert(0, item);
                    }
                    OldGradeCnt = temp_qulityGradeInfo.GradeCnt;  //Add by ChengSk - 2019118
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualGradeSetForm 中函数SetQualGradeSetlistViewEx出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualGradeSetForm 中函数SetQualGradeSetlistViewEx出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 启用设置等级参数
        /// </summary>
        /// <param name="Enabled"></param>
        public void EnableQualGradeSetcomboBox(bool Enabled)
        {
            this.QualGradeSetcomboBox.Enabled = Enabled;
            this.QualGradeSetlistViewEx.Enabled = Enabled;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QualGradeSetbutton_Click(object sender, EventArgs e)
        {
            GlobalDataInterface.qualityParamSetForm = new QualityParamSetForm(this, m_mainForm);
            GlobalDataInterface.qualityParamSetForm.ShowDialog();
        }
        private void QualGradeSetbutton_MouseClick(object sender, MouseEventArgs e)
        {
            GlobalDataInterface.qualgradeSet = true;
        }
        /// <summary>
        /// 等级数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QualGradeSetcomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            //temp_qulityGradeInfo.GradeCnt = comboBox.SelectedIndex + 1;
            temp_qulityGradeInfo.GradeCnt = comboBox.SelectedIndex;
            ResetQualGradeSetlistViewExColumns();
            SetQualGradeSetlistViewEx();
        }

        /// <summary>
        /// 品质等级列表双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QualGradeSetlistViewEx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            m_QualGradeSetlistViewExMouseDoubleClick = true;
        }

        /// <summary>
        /// 品质等级列表双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QualGradeSetlistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                ListViewEx.ListViewEx listViewEx = (ListViewEx.ListViewEx)sender;
                if (m_QualGradeSetlistViewExMouseDoubleClick)// && e.Item.Index != (listViewEx.Items.Count-1))
                {
                    //MessageBox.Show("e.Item.Index=" + e.Item.Index);
                    m_qualGradeInfo.QualName = Encoding.Default.GetString(temp_qulityGradeInfo.Item[e.Item.Index].GradeName).TrimEnd('\0');
                    m_qualGradeInfo.ColorIndex = temp_qulityGradeInfo.Item[e.Item.Index].ColorGrade;
                    m_qualGradeInfo.ShapeIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbShapeGrade;
                    m_qualGradeInfo.FlawIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbFlaw;
                    m_qualGradeInfo.BruiseIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbBruise;
                    m_qualGradeInfo.RotIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbRot;
                    m_qualGradeInfo.DensityIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbDensity;
                    m_qualGradeInfo.SugarIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbSugar;
                    m_qualGradeInfo.AcidityIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbAcidity;
                    m_qualGradeInfo.HollowIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbHollow;
                    m_qualGradeInfo.SkinIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbSkin;
                    m_qualGradeInfo.BrownIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbBrown;
                    m_qualGradeInfo.TangxinIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbTangxin;
                    m_qualGradeInfo.RigidityIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbRigidity;
                    m_qualGradeInfo.WaterIndex = temp_qulityGradeInfo.Item[e.Item.Index].sbWater;

                    QualitySetForm qualitySetForm = new QualitySetForm(this, m_qualGradeInfo);

                    if (qualitySetForm.ShowDialog() == DialogResult.OK)
                    {
                        byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        Array.Copy(temp, 0, temp_qulityGradeInfo.Item[e.Item.Index].GradeName, 0, temp_qulityGradeInfo.Item[e.Item.Index].GradeName.Length);//清零
                        temp_qulityGradeInfo.Item[e.Item.Index].GradeName = Encoding.Default.GetBytes(m_qualGradeInfo.QualName);

                        if (m_qualGradeInfo.ColorIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].ColorGrade = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].ColorGrade = (sbyte)m_qualGradeInfo.ColorIndex;

                        if (m_qualGradeInfo.ShapeIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbShapeGrade = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbShapeGrade = (sbyte)m_qualGradeInfo.ShapeIndex;

                        if (m_qualGradeInfo.FlawIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbFlaw = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbFlaw = (sbyte)m_qualGradeInfo.FlawIndex;

                        if (m_qualGradeInfo.BruiseIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbBruise = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbBruise = (sbyte)m_qualGradeInfo.BruiseIndex;

                        if (m_qualGradeInfo.RotIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbRot = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbRot = (sbyte)m_qualGradeInfo.RotIndex;

                        if (m_qualGradeInfo.DensityIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbDensity = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbDensity = (sbyte)m_qualGradeInfo.DensityIndex;

                        if (m_qualGradeInfo.SugarIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbSugar = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbSugar = (sbyte)m_qualGradeInfo.SugarIndex;

                        if (m_qualGradeInfo.AcidityIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbAcidity = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbAcidity = (sbyte)m_qualGradeInfo.AcidityIndex;

                        if (m_qualGradeInfo.HollowIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbHollow = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbHollow = (sbyte)m_qualGradeInfo.HollowIndex;

                        if (m_qualGradeInfo.SkinIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbSkin = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbSkin = (sbyte)m_qualGradeInfo.SkinIndex;

                        if (m_qualGradeInfo.BrownIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbBrown = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbBrown = (sbyte)m_qualGradeInfo.BrownIndex;

                        if (m_qualGradeInfo.TangxinIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbTangxin = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbTangxin = (sbyte)m_qualGradeInfo.TangxinIndex;

                        if (m_qualGradeInfo.RigidityIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbRigidity = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbRigidity = (sbyte)m_qualGradeInfo.RigidityIndex;

                        if (m_qualGradeInfo.WaterIndex == -1)
                            temp_qulityGradeInfo.Item[e.Item.Index].sbWater = 0x7f;
                        else
                            temp_qulityGradeInfo.Item[e.Item.Index].sbWater = (sbyte)m_qualGradeInfo.WaterIndex;

                        listViewEx.Items[e.Item.Index].SubItems[0].Text = m_qualGradeInfo.QualName;

                        byte[] tempName = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                        if (m_qualGradeInfo.ColorIndex == -1 || m_qualGradeInfo.ColorIndex == 0x7f7f)
                        {
                            //if (e.Item.Index == 0 && GlobalDataInterface.ColorGradeNum > 0)
                            //{
                            //    tempName = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                            //    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strColorGradeName, (GlobalDataInterface.ColorGradeNum - 1) * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            //    listViewEx.Items[e.Item.Index].SubItems[1].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                            //}
                            //else
                            listViewEx.Items[e.Item.Index].SubItems[1].Text = m_resourceManager.GetString("Alllabel.Text");
                        }
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strColorGradeName, m_qualGradeInfo.ColorIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[1].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.ShapeIndex == -1 || m_qualGradeInfo.ShapeIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[2].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, m_qualGradeInfo.ShapeIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[2].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.FlawIndex == -1 || m_qualGradeInfo.FlawIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[3].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stFlawareaGradeName, m_qualGradeInfo.FlawIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[3].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.BruiseIndex == -1 || m_qualGradeInfo.BruiseIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[4].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stBruiseGradeName, m_qualGradeInfo.BruiseIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[4].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.RotIndex == -1 || m_qualGradeInfo.RotIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[5].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stRotGradeName, m_qualGradeInfo.RotIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[5].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.DensityIndex == -1 || m_qualGradeInfo.DensityIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[6].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stDensityGradeName, m_qualGradeInfo.DensityIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[6].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.SugarIndex == -1 || m_qualGradeInfo.SugarIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[7].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stSugarGradeName, m_qualGradeInfo.SugarIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[7].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.AcidityIndex == -1 || m_qualGradeInfo.AcidityIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[8].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stAcidityGradeName, m_qualGradeInfo.AcidityIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[8].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.HollowIndex == -1 || m_qualGradeInfo.HollowIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[9].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stHollowGradeName, m_qualGradeInfo.HollowIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[9].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.SkinIndex == -1 || m_qualGradeInfo.SkinIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[10].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stSkinGradeName, m_qualGradeInfo.SkinIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[10].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.BrownIndex == -1 || m_qualGradeInfo.BrownIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[11].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stBrownGradeName, m_qualGradeInfo.BrownIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[11].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.TangxinIndex == -1 || m_qualGradeInfo.TangxinIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[12].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stTangxinGradeName, m_qualGradeInfo.TangxinIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[12].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.RigidityIndex == -1 || m_qualGradeInfo.RigidityIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[13].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stRigidityGradeName, m_qualGradeInfo.RigidityIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[13].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }

                        if (m_qualGradeInfo.WaterIndex == -1 || m_qualGradeInfo.WaterIndex == 0x7f)
                            listViewEx.Items[e.Item.Index].SubItems[14].Text = m_resourceManager.GetString("Alllabel.Text");
                        else
                        {
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stWaterGradeName, m_qualGradeInfo.WaterIndex * ConstPreDefine.MAX_TEXT_LENGTH, tempName, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            listViewEx.Items[e.Item.Index].SubItems[14].Text = Encoding.Default.GetString(tempName).TrimEnd('\0');
                        }
                    }
                    m_QualGradeSetlistViewExMouseDoubleClick = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualGradeSetForm 中函数QualGradeSetlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualGradeSetForm 中函数QualGradeSetlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 设置等级参数
        /// </summary>
        /// <param name="qualGradeInfo"></param>
        public void SetQualGradeInfo(QaulGradeInfo qualGradeInfo)
        {
            m_qualGradeInfo = qualGradeInfo;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        private bool QualGradeSetFormSaveConfig()
        {
            try
            {
                stGradeInfo tempGradeInfo = new stGradeInfo(true);
                tempGradeInfo.ToCopy(GlobalDataInterface.globalOut_GradeInfo);

                for (int i = 0; i < temp_qulityGradeInfo.GradeCnt; i++)
                {
                    if (temp_qulityGradeInfo.Item[i].GradeName[0] == 0)
                    {
                        //MessageBox.Show("品质等级名称不能为空！");
                        //MessageBox.Show("0x3000100D The grade name cannot be empty!","Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        MessageBox.Show("0x3000100D " + LanguageContainer.QualGradeSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.QualGradeSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    for (int j = i + 1; j < temp_qulityGradeInfo.GradeCnt; j++)
                    {
                        if (string.Equals(this.QualGradeSetlistViewEx.Items[i].SubItems[0].Text, this.QualGradeSetlistViewEx.Items[j].SubItems[0].Text))
                        //if (Array.Equals(temp_qulityGradeInfo.Item[i].GradeName, temp_qulityGradeInfo.Item[j].GradeName))
                        {
                            //MessageBox.Show("品质等级名称不能重复！");
                            //MessageBox.Show("0x3000100E The grade names cannot repeat!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show("0x3000100E " + LanguageContainer.QualGradeSetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                                LanguageContainer.QualGradeSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                    Array.Copy(temp, 0, GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, ConstPreDefine.MAX_TEXT_LENGTH);//清零
                    Array.Copy(temp_qulityGradeInfo.Item[i].GradeName, 0, GlobalDataInterface.globalOut_GradeInfo.strQualityGradeName, i * ConstPreDefine.MAX_TEXT_LENGTH, temp_qulityGradeInfo.Item[i].GradeName.Length);
                }
                GlobalDataInterface.Quality_GradeInfo = new QualityGradeInfo(true);
                GlobalDataInterface.Quality_GradeInfo.ToCopy(temp_qulityGradeInfo);

                //等级数量改变 出口等级应清零
                if (GlobalDataInterface.Quality_GradeInfo.GradeCnt != GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum)
                {
                    for (int i = 0; i < ConstPreDefine.MAX_QUALITY_GRADE_NUM; i++)
                    {
                        for (int j = 0; j < ConstPreDefine.MAX_SIZE_GRADE_NUM; j++)
                        {
                            GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit = 0;
                        }
                    }
                }

                int size = 1;
                if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                    size = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum = (byte)GlobalDataInterface.Quality_GradeInfo.GradeCnt;

                for (int i = 0; i < GlobalDataInterface.Quality_GradeInfo.GradeCnt; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbShapeSize = GlobalDataInterface.Quality_GradeInfo.Item[i].sbShapeGrade;
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nColorGrade = GlobalDataInterface.Quality_GradeInfo.Item[i].ColorGrade;
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbDensity = GlobalDataInterface.Quality_GradeInfo.Item[i].sbDensity;
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbFlawArea = GlobalDataInterface.Quality_GradeInfo.Item[i].sbFlaw;

                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbBruise = GlobalDataInterface.Quality_GradeInfo.Item[i].sbBruise;
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbRot = GlobalDataInterface.Quality_GradeInfo.Item[i].sbRot;

                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbSugar = GlobalDataInterface.Quality_GradeInfo.Item[i].sbSugar;
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbAcidity = GlobalDataInterface.Quality_GradeInfo.Item[i].sbAcidity;
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbHollow = GlobalDataInterface.Quality_GradeInfo.Item[i].sbHollow;
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbSkin = GlobalDataInterface.Quality_GradeInfo.Item[i].sbSkin;
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbBrown = GlobalDataInterface.Quality_GradeInfo.Item[i].sbBrown;
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbTangxin = GlobalDataInterface.Quality_GradeInfo.Item[i].sbTangxin;

                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbRigidity = GlobalDataInterface.Quality_GradeInfo.Item[i].sbRigidity;
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].sbWater = GlobalDataInterface.Quality_GradeInfo.Item[i].sbWater;


                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nMinSize = GlobalDataInterface.globalOut_GradeInfo.grades[j].nMinSize;
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nMaxSize = GlobalDataInterface.globalOut_GradeInfo.grades[j].nMaxSize;
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].nFruitNum = GlobalDataInterface.globalOut_GradeInfo.grades[j].nFruitNum;
                    }
                }
                if (!GlobalDataInterface.globalOut_GradeInfo.IsEqual(tempGradeInfo))
                {
                    m_mainForm.SetSeparationProgrameChangelabel(false, null);
                }
                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_COLOR_GRADE_INFO, null);
                    m_mainForm.staticCount = 0;  //切换品质后，分批显示变量初始化 Add by ChengSk - 20171222
                    if (GlobalDataInterface.sendBroadcastPackage) GlobalDataInterface.PadGradeConfigUpdate++;//等级配置信息更改，平板需更新
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualGradeSetForm 中函数QualGradeSetFormSaveConfig出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualGradeSetForm 中函数QualGradeSetFormSaveConfig出错" + ex);
#endif
                return false;
            }
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!QualGradeSetFormSaveConfig())
                {
                    //DialogResult result = MessageBox.Show("参数保存出错，还要关闭吗？", "保存出错", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //DialogResult result = MessageBox.Show("0x1000100A Grade Setting's configuration saved error!Do you want to close it?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    DialogResult result = MessageBox.Show("0x1000100A " + LanguageContainer.QualGradeSetFormMessagebox3Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.QualGradeSetFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        //if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 1 || GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)//品质
                        if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)//品质
                        {
                            m_mainForm.SetGradedataGridViewInfo();
                            m_mainForm.SetGradeSizelistViewEx();
                        }
                    }
                    else
                        return;
                }
                else
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    //if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType == 1 || GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)//品质
                    if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)//品质
                    {
                        //this.BeginInvoke(new GlobalDataInterface.GradeInterfaceFreshEvent(m_mainForm.SetGradedataGridViewInfo));
                        GlobalDataInterface.gGradeInterfaceFresh = false;
                        m_mainForm.SetGradedataGridViewInfo();
                        m_mainForm.SetGradeSizelistViewEx();
                        m_mainForm.SetAllExitListBox();
                        GlobalDataInterface.gGradeInterfaceFresh = true;

                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualGradeSetForm中函数OKbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualGradeSetForm中函数OKbutton_Click出错" + ex);
#endif
            }
        }

        private void AutoGradeSetbutton_Click(object sender, EventArgs e)
        {
            try
            {
                //ResetQualGradeSetlistViewExColumns();

                int TotalGradeNum = 1;
                int validColorGradeNum = 1;
                int validShapeGradeNum = 1;
                int validFlawGradeNum = 1;
                int validDensityGradeNum = 1;
                int validBruiseGradeNum = 1;
                int validRotGradeNum = 1;
                int validSugarGradeNum = 1;
                int validAcidityGradeNum = 1;
                int validHollowGradeNum = 1;
                int validSkinGradeNum = 1;
                int validBrownGradeNum = 1;
                int validTangxinGradeNum = 1;
                int validRigidityGradeNum = 1;
                int validWaterGradeNum = 1;

                if (GlobalDataInterface.SystemStructColor)
                {
                    //if ((GlobalDataInterface.globalOut_GradeInfo.ColorType & 0x01) == 0x00) //平均值 //Modify by ChengSk - 20200710
                    if ((GlobalDataInterface.globalOut_GradeInfo.ColorType & 0x08) == 0x00)   //平均值
                    {
                        if (GlobalDataInterface.globalOut_GradeInfo.ColorIntervals[0] == 0)  //用2个等级
                        {
                            TotalGradeNum = TotalGradeNum * 2;
                            validColorGradeNum = 2;
                        }
                        else //用3个等级
                        {
                            TotalGradeNum = TotalGradeNum * 3;
                            validColorGradeNum = 3;
                        }
                    }
                    else //百分比
                    {
                        TotalGradeNum = TotalGradeNum * GlobalDataInterface.ColorGradeNum;
                        validColorGradeNum = GlobalDataInterface.ColorGradeNum;
                    }
                }
                if (GlobalDataInterface.SystemStructShape)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.ShapeGradeNum;
                    validShapeGradeNum = GlobalDataInterface.ShapeGradeNum;
                }
                if (GlobalDataInterface.SystemStructFlaw)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.FlawGradeNum;
                    validFlawGradeNum = GlobalDataInterface.FlawGradeNum;
                }
                if (GlobalDataInterface.SystemStructDensity)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.DensityGradeNum;
                    validDensityGradeNum = GlobalDataInterface.DensityGradeNum;
                }
                if (GlobalDataInterface.SystemStructBruise)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.BruiseGradeNum;
                    validBruiseGradeNum = GlobalDataInterface.BruiseGradeNum;
                }
                if (GlobalDataInterface.SystemStructRot)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.RotGradeNum;
                    validRotGradeNum = GlobalDataInterface.RotGradeNum;
                }
                if (GlobalDataInterface.SystemStructSugar)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.SugarGradeNum;
                    validSugarGradeNum = GlobalDataInterface.SugarGradeNum;
                }
                if (GlobalDataInterface.SystemStructAcidity)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.AcidityGradeNum;
                    validAcidityGradeNum = GlobalDataInterface.AcidityGradeNum;
                }
                if (GlobalDataInterface.SystemStructHollow)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.HollowGradeNum;
                    validHollowGradeNum = GlobalDataInterface.HollowGradeNum;
                }
                if (GlobalDataInterface.SystemStructSkin)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.SkinGradeNum;
                    validSkinGradeNum = GlobalDataInterface.SkinGradeNum;
                }
                if (GlobalDataInterface.SystemStructBrown)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.BrownGradeNum;
                    validBrownGradeNum = GlobalDataInterface.BrownGradeNum;
                }
                if (GlobalDataInterface.SystemStructTangxin)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.TangxinGradeNum;
                    validTangxinGradeNum = GlobalDataInterface.TangxinGradeNum;
                }
                if (GlobalDataInterface.SystemStructRigidity)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.RigidityGradeNum;
                    validRigidityGradeNum = GlobalDataInterface.RigidityGradeNum;
                }
                if (GlobalDataInterface.SystemStructWater)
                {
                    TotalGradeNum = TotalGradeNum * GlobalDataInterface.WaterGradeNum;
                    validWaterGradeNum = GlobalDataInterface.WaterGradeNum;
                }

                if (TotalGradeNum > 16)
                {
                    MessageBox.Show("0x30001030 There are too many grades to set automatically!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                this.QualGradeSetcomboBox.SelectedIndex = TotalGradeNum;
                temp_qulityGradeInfo.GradeCnt = TotalGradeNum;

                // 准备自动生成等级信息
                if (temp_qulityGradeInfo.GradeCnt == 0)
                {
                    this.QualGradeSetlistViewEx.Items.Clear();
                }
                else if (temp_qulityGradeInfo.GradeCnt > 0)
                {
                    ListViewItem item;
                    this.QualGradeSetlistViewEx.Items.Clear();
                    byte[] temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];

                    for (int i = 0; i < temp_qulityGradeInfo.GradeCnt; i++)
                    {
                        if (temp_qulityGradeInfo.Item[i].ColorGrade != 0x7f && temp_qulityGradeInfo.Item[i].ColorGrade > GlobalDataInterface.ColorGradeNum)
                            temp_qulityGradeInfo.Item[i].ColorGrade = (sbyte)(GlobalDataInterface.ColorGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbShapeGrade != 0x7f && temp_qulityGradeInfo.Item[i].sbShapeGrade > GlobalDataInterface.ShapeGradeNum)
                            temp_qulityGradeInfo.Item[i].sbShapeGrade = (sbyte)(GlobalDataInterface.ShapeGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbFlaw != 0x7f && temp_qulityGradeInfo.Item[i].sbFlaw > GlobalDataInterface.FlawGradeNum)
                            temp_qulityGradeInfo.Item[i].sbFlaw = (sbyte)(GlobalDataInterface.FlawGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbBruise != 0x7f && temp_qulityGradeInfo.Item[i].sbBruise > GlobalDataInterface.BruiseGradeNum)
                            temp_qulityGradeInfo.Item[i].sbBruise = (sbyte)(GlobalDataInterface.BruiseGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbRot != 0x7f && temp_qulityGradeInfo.Item[i].sbRot > GlobalDataInterface.RotGradeNum)
                            temp_qulityGradeInfo.Item[i].sbRot = (sbyte)(GlobalDataInterface.RotGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbDensity != 0x7f && temp_qulityGradeInfo.Item[i].sbDensity > GlobalDataInterface.DensityGradeNum)
                            temp_qulityGradeInfo.Item[i].sbDensity = (sbyte)(GlobalDataInterface.DensityGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbSugar != 0x7f && temp_qulityGradeInfo.Item[i].sbSugar > GlobalDataInterface.SugarGradeNum)
                            temp_qulityGradeInfo.Item[i].sbSugar = (sbyte)(GlobalDataInterface.SugarGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbAcidity != 0x7f && temp_qulityGradeInfo.Item[i].sbAcidity > GlobalDataInterface.AcidityGradeNum)
                            temp_qulityGradeInfo.Item[i].sbAcidity = (sbyte)(GlobalDataInterface.AcidityGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbHollow != 0x7f && temp_qulityGradeInfo.Item[i].sbHollow > GlobalDataInterface.HollowGradeNum)
                            temp_qulityGradeInfo.Item[i].sbHollow = (sbyte)(GlobalDataInterface.HollowGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbSkin != 0x7f && temp_qulityGradeInfo.Item[i].sbSkin > GlobalDataInterface.SkinGradeNum)
                            temp_qulityGradeInfo.Item[i].sbSkin = (sbyte)(GlobalDataInterface.SkinGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbBrown != 0x7f && temp_qulityGradeInfo.Item[i].sbBrown > GlobalDataInterface.BrownGradeNum)
                            temp_qulityGradeInfo.Item[i].sbBrown = (sbyte)(GlobalDataInterface.BrownGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbTangxin != 0x7f && temp_qulityGradeInfo.Item[i].sbTangxin > GlobalDataInterface.TangxinGradeNum)
                            temp_qulityGradeInfo.Item[i].sbTangxin = (sbyte)(GlobalDataInterface.TangxinGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbRigidity != 0x7f && temp_qulityGradeInfo.Item[i].sbRigidity > GlobalDataInterface.RigidityGradeNum)
                            temp_qulityGradeInfo.Item[i].sbRigidity = (sbyte)(GlobalDataInterface.RigidityGradeNum - 1);
                        if (temp_qulityGradeInfo.Item[i].sbWater != 0x7f && temp_qulityGradeInfo.Item[i].sbWater > GlobalDataInterface.WaterGradeNum)
                            temp_qulityGradeInfo.Item[i].sbWater = (sbyte)(GlobalDataInterface.WaterGradeNum - 1);
                    }

                    int GradeCnt = 0;
                    string strGradeName = "";
                    string strG1Name = "";
                    string strG2Name = "";
                    string strG3Name = "";
                    string strG4Name = "";
                    string strG5Name = "";
                    string strG6Name = "";
                    string strG7Name = "";
                    string strG8Name = "";
                    string strG9Name = "";
                    string strG10Name = "";
                    string strG11Name = "";
                    string strG12Name = "";
                    string strG13Name = "";
                    string strG14Name = "";
                    for (int G1 = 0; G1 < validColorGradeNum; G1++)
                    {
                        if (validColorGradeNum > 1)
                        {
                            //temp_qulityGradeInfo.Item[G1].ColorGrade = (sbyte)G1;
                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strColorGradeName, G1 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                            strG1Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                        }
                        else
                        {
                            temp_qulityGradeInfo.Item[G1].ColorGrade = GlobalDataInterface.SystemStructColor == true ? (sbyte)0 : (sbyte)0x7F;
                        }
                        for (int G2 = 0; G2 < validShapeGradeNum; G2++)
                        {
                            if (validShapeGradeNum > 1)
                            {
                                //temp_qulityGradeInfo.Item[G2].sbShapeGrade = (sbyte)G2;
                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, G2 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                strG2Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                            }
                            else
                            {
                                temp_qulityGradeInfo.Item[G2].sbShapeGrade = GlobalDataInterface.SystemStructShape == true ? (sbyte)0 : (sbyte)0x7F;
                            }
                            for (int G3 = 0; G3 < validFlawGradeNum; G3++)
                            {
                                if (validFlawGradeNum > 1)
                                {
                                    //temp_qulityGradeInfo.Item[G3].sbFlaw = (sbyte)G3;
                                    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stFlawareaGradeName, G3 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                    strG3Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                                }
                                else
                                {
                                    temp_qulityGradeInfo.Item[G3].sbFlaw = GlobalDataInterface.SystemStructFlaw == true ? (sbyte)0 : (sbyte)0x7F;
                                }
                                for (int G4 = 0; G4 < validDensityGradeNum; G4++)
                                {
                                    if (validDensityGradeNum > 1)
                                    {
                                        //temp_qulityGradeInfo.Item[G4].sbDensity = (sbyte)G4;
                                        Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stDensityGradeName, G4 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                        strG4Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                                    }
                                    else
                                    {
                                        temp_qulityGradeInfo.Item[G4].sbDensity = GlobalDataInterface.SystemStructDensity == true ? (sbyte)0 : (sbyte)0x7F;
                                    }
                                    for (int G5 = 0; G5 < validBruiseGradeNum; G5++)
                                    {
                                        if (validBruiseGradeNum > 1)
                                        {
                                            //temp_qulityGradeInfo.Item[G5].sbBruise = (sbyte)G5;
                                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stBruiseGradeName, G5 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                            strG5Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                                        }
                                        else
                                        {
                                            temp_qulityGradeInfo.Item[G5].sbBruise = GlobalDataInterface.SystemStructBruise == true ? (sbyte)0 : (sbyte)0x7F;
                                        }
                                        for (int G6 = 0; G6 < validRotGradeNum; G6++)
                                        {
                                            if (validRotGradeNum > 1)
                                            {
                                                //temp_qulityGradeInfo.Item[G6].sbRot = (sbyte)G6;
                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stRotGradeName, G6 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                strG6Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                                            }
                                            else
                                            {
                                                temp_qulityGradeInfo.Item[G6].sbRot = GlobalDataInterface.SystemStructRot == true ? (sbyte)0 : (sbyte)0x7F;
                                            }
                                            for (int G7 = 0; G7 < validSugarGradeNum; G7++)
                                            {
                                                if (validSugarGradeNum > 1)
                                                {
                                                    //temp_qulityGradeInfo.Item[G7].sbSugar = (sbyte)G7;
                                                    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stSugarGradeName, G7 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                    strG7Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                                                }
                                                else
                                                {
                                                    temp_qulityGradeInfo.Item[G7].sbSugar = GlobalDataInterface.SystemStructSugar == true ? (sbyte)0 : (sbyte)0x7F;
                                                }
                                                for (int G8 = 0; G8 < validAcidityGradeNum; G8++)
                                                {
                                                    if (validAcidityGradeNum > 1)
                                                    {
                                                        //temp_qulityGradeInfo.Item[G8].sbAcidity = (sbyte)G8;
                                                        Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stAcidityGradeName, G8 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                        strG8Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                                                    }
                                                    else
                                                    {
                                                        temp_qulityGradeInfo.Item[G8].sbAcidity = GlobalDataInterface.SystemStructAcidity == true ? (sbyte)0 : (sbyte)0x7F;
                                                    }
                                                    for (int G9 = 0; G9 < validHollowGradeNum; G9++)
                                                    {
                                                        if (validHollowGradeNum > 1)
                                                        {
                                                            //temp_qulityGradeInfo.Item[G9].sbHollow = (sbyte)G9;
                                                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stHollowGradeName, G9 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                            strG9Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                                                        }
                                                        else
                                                        {
                                                            temp_qulityGradeInfo.Item[G9].sbHollow = GlobalDataInterface.SystemStructHollow == true ? (sbyte)0 : (sbyte)0x7F;
                                                        }
                                                        for (int G10 = 0; G10 < validSkinGradeNum; G10++)
                                                        {
                                                            if (validSkinGradeNum > 1)
                                                            {
                                                                //temp_qulityGradeInfo.Item[G10].sbSkin = (sbyte)G10;
                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stSkinGradeName, G10 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                strG10Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                                                            }
                                                            else
                                                            {
                                                                temp_qulityGradeInfo.Item[G10].sbSkin = GlobalDataInterface.SystemStructSkin == true ? (sbyte)0 : (sbyte)0x7F;
                                                            }
                                                            for (int G11 = 0; G11 < validBrownGradeNum; G11++)
                                                            {
                                                                if (validBrownGradeNum > 1)
                                                                {
                                                                    //temp_qulityGradeInfo.Item[G11].sbBrown = (sbyte)G11;
                                                                    Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stBrownGradeName, G11 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                    strG11Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                                                                }
                                                                else
                                                                {
                                                                    temp_qulityGradeInfo.Item[G11].sbBrown = GlobalDataInterface.SystemStructBrown == true ? (sbyte)0 : (sbyte)0x7F;
                                                                }
                                                                for (int G12 = 0; G12 < validTangxinGradeNum; G12++)
                                                                {
                                                                    if (validTangxinGradeNum > 1)
                                                                    {
                                                                        //temp_qulityGradeInfo.Item[G12].sbTangxin = (sbyte)G12;
                                                                        Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stTangxinGradeName, G12 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                        strG12Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                                                                    }
                                                                    else
                                                                    {
                                                                        temp_qulityGradeInfo.Item[G12].sbTangxin = GlobalDataInterface.SystemStructTangxin == true ? (sbyte)0 : (sbyte)0x7F;
                                                                    }
                                                                    for (int G13 = 0; G13 < validRigidityGradeNum; G13++)
                                                                    {
                                                                        if (validRigidityGradeNum > 1)
                                                                        {
                                                                            //temp_qulityGradeInfo.Item[G13].sbRigidity = (sbyte)G13;
                                                                            Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stRigidityGradeName, G13 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                            strG13Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                                                                        }
                                                                        else
                                                                        {
                                                                            temp_qulityGradeInfo.Item[G13].sbRigidity = GlobalDataInterface.SystemStructRigidity == true ? (sbyte)0 : (sbyte)0x7F;
                                                                        }
                                                                        for (int G14 = 0; G14 < validWaterGradeNum; G14++)
                                                                        {
                                                                            if (validWaterGradeNum > 1)
                                                                            {
                                                                                //temp_qulityGradeInfo.Item[G14].sbWater = (sbyte)G14;
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stWaterGradeName, G14 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                strG14Name = Encoding.Default.GetString(temp).TrimEnd('\0') + ".";
                                                                            }
                                                                            else
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G14].sbWater = GlobalDataInterface.SystemStructWater == true ? (sbyte)0 : (sbyte)0x7F;
                                                                            }

                                                                            strGradeName = strG1Name + strG2Name + strG3Name + strG4Name + strG5Name + strG6Name +
                                                                                strG7Name + strG8Name + strG9Name + strG10Name + strG11Name + strG12Name + strG12Name + strG13Name + strG14Name;
                                                                            strGradeName = strGradeName.Substring(0, strGradeName.Length - 1);
                                                                            item = new ListViewItem(strGradeName); //显示名称

                                                                            if (!GlobalDataInterface.SystemStructColor)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G1].ColorGrade = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strColorGradeName, G1 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            if (!GlobalDataInterface.SystemStructShape)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G2].sbShapeGrade = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.strShapeGradeName, G2 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            if (!GlobalDataInterface.SystemStructFlaw)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G3].sbFlaw = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stFlawareaGradeName, G3 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            if (!GlobalDataInterface.SystemStructBruise)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G5].sbBruise = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stBruiseGradeName, G5 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            if (!GlobalDataInterface.SystemStructRot)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G6].sbRot = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stRotGradeName, G6 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            if (!GlobalDataInterface.SystemStructDensity)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G4].sbDensity = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stDensityGradeName, G4 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            

                                                                            if (!GlobalDataInterface.SystemStructSugar)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G7].sbSugar = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stSugarGradeName, G7 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            if (!GlobalDataInterface.SystemStructAcidity)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G8].sbAcidity = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stAcidityGradeName, G8 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            if (!GlobalDataInterface.SystemStructHollow)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G9].sbHollow = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stHollowGradeName, G9 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            if (!GlobalDataInterface.SystemStructSkin)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G10].sbSkin = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stSkinGradeName, G10 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            if (!GlobalDataInterface.SystemStructBrown)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G11].sbBrown = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stBrownGradeName, G11 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            if (!GlobalDataInterface.SystemStructTangxin)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G12].sbTangxin = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stTangxinGradeName, G12 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            if (!GlobalDataInterface.SystemStructRigidity)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G13].sbRigidity = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stRigidityGradeName, G13 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            if (!GlobalDataInterface.SystemStructWater)
                                                                            {
                                                                                temp_qulityGradeInfo.Item[G14].sbWater = 0x7F;
                                                                                item.SubItems.Add(m_resourceManager.GetString("Alllabel.Text"));
                                                                            }
                                                                            else
                                                                            {
                                                                                Array.Copy(GlobalDataInterface.globalOut_GradeInfo.stWaterGradeName, G14 * ConstPreDefine.MAX_TEXT_LENGTH, temp, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                                                                                item.SubItems.Add(Encoding.Default.GetString(temp).TrimEnd('\0'));
                                                                            }

                                                                            temp = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
                                                                            Array.Copy(temp, 0, temp_qulityGradeInfo.Item[GradeCnt].GradeName, 0, temp_qulityGradeInfo.Item[GradeCnt].GradeName.Length);//清零
                                                                            temp_qulityGradeInfo.Item[GradeCnt].GradeName = Encoding.Default.GetBytes(strGradeName);
                                                                            temp_qulityGradeInfo.Item[GradeCnt].ColorGrade = GlobalDataInterface.SystemStructColor ? (sbyte)G1 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbShapeGrade = GlobalDataInterface.SystemStructShape ? (sbyte)G2 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbFlaw = GlobalDataInterface.SystemStructFlaw ? (sbyte)G3 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbDensity = GlobalDataInterface.SystemStructDensity ? (sbyte)G4 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbBruise = GlobalDataInterface.SystemStructBruise ? (sbyte)G5 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbRot = GlobalDataInterface.SystemStructRot ? (sbyte)G6 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbSugar = GlobalDataInterface.SystemStructSugar ? (sbyte)G7 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbAcidity = GlobalDataInterface.SystemStructAcidity ? (sbyte)G8 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbHollow = GlobalDataInterface.SystemStructHollow ? (sbyte)G9 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbSkin = GlobalDataInterface.SystemStructSkin ? (sbyte)G10 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbBrown = GlobalDataInterface.SystemStructBrown ? (sbyte)G11 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbTangxin = GlobalDataInterface.SystemStructTangxin ? (sbyte)G12 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbRigidity = GlobalDataInterface.SystemStructRigidity ? (sbyte)G13 : (sbyte)0x7F;
                                                                            temp_qulityGradeInfo.Item[GradeCnt].sbWater = GlobalDataInterface.SystemStructWater ? (sbyte)G14 : (sbyte)0x7F;
                                                                            GradeCnt++;

                                                                            this.QualGradeSetlistViewEx.Items.Add(item); //插入表
                                                                        } //G14
                                                                    } //G13
                                                                } //G12
                                                            } //G11
                                                        } //G10
                                                    } //G9
                                                } //G8
                                            } //G7
                                        } //G6
                                    } //G5
                                } //G4
                            } //G3
                        } //G2
                    } //G1
                }
                //Console.WriteLine("TEST");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("QualGradeSetForm 中函数AutoGradeSetbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("QualGradeSetForm 中函数AutoGradeSetbutton_Click出错" + ex);
#endif
            }
        }

        private void QualGradeSetlistViewEx_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            ColumnHeader header = this.QualGradeSetlistViewEx.Columns[e.ColumnIndex];
            e.NewWidth = QualGradeSetlistViewEx.Columns[e.ColumnIndex].Width;
            e.Cancel = true;
        }

        
    }
}
