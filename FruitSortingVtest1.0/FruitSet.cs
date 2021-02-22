using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using ListViewEx;
using GlacialComponents.Controls;
using Interface;
using Common;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FruitSortingVtest1._0
{
    
    public partial class ProjectSetForm : Form
    {
        stFruitType m_FruitType = new stFruitType(true);
        string fruittypeconfig;
        string[] sFruitMajortArray, sFruitMajortArrayEn, sFruitMajortArrayEs;
        private void FruitSetInitial()
        {
            try
            {
                this.FruitTypedataGridView.Font = new Font("微软雅黑", 9, FontStyle.Regular);
                this.FruitTypedataGridView.AllowUserToAddRows = false;
                this.FruitTypedataGridView.Rows.Clear();
                this.FruitTypedataGridView.ReadOnly = false;
                this.FruitTypedataGridView.RowHeadersWidth =190;//行表头宽度
                this.FruitTypedataGridView.TopLeftHeaderCell.Value = m_resourceManager.GetString("FruitTypeGridViewTopLeftHeadCelllabel.Text");
                DataGridViewRow row;
               // m_FruitType.iCurrentFruitNumber = int.Parse(Commonfunction.GetAppSetting("已选水果数量"));
                fruittypeconfig = Commonfunction.GetAppSetting("水果种类大类");
                sFruitMajortArray = fruittypeconfig.Split(';'); //分割具体水果种类
                if(GlobalDataInterface.selectLanguage == "en")
                {
                    fruittypeconfig = Commonfunction.GetAppSetting("水果种类大类en");
                    sFruitMajortArrayEn = fruittypeconfig.Split(';');//分割具体水果种类
                }
                if(GlobalDataInterface.selectLanguage == "es") 
                {
                    fruittypeconfig = Commonfunction.GetAppSetting("水果种类大类es");
                    sFruitMajortArrayEs = fruittypeconfig.Split(';');//分割具体水果种类 //Add by ChengSk - 20181016
                }
                byte[] btempArray = new byte[ConstPreDefine.MAX_FRUIT_TEXT_LENGTH];
                for (int i = 0; i < sFruitMajortArray.Length-1; i++)
                {
                    row = new DataGridViewRow();
                    row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomLeft;//设置文字格式
                    this.FruitTypedataGridView.Rows.Add(row);
                    if (GlobalDataInterface.selectLanguage == "en") //英语
                        this.FruitTypedataGridView.Rows[i].HeaderCell.Value = sFruitMajortArrayEn[i];//水果大类名称
                    else if (GlobalDataInterface.selectLanguage == "es")  //西班牙语
                        this.FruitTypedataGridView.Rows[i].HeaderCell.Value = sFruitMajortArrayEs[i];//水果大类名称
                    else //中文
                        this.FruitTypedataGridView.Rows[i].HeaderCell.Value = sFruitMajortArray[i];  //水果大类名称
                    this.FruitTypedataGridView[0, i].Value = GlobalDataInterface.globalOut_AnalogDensity.uAnalogDensity[i].ToString("#0.00");//水果密度
                    fruittypeconfig = Commonfunction.GetAppSetting(sFruitMajortArray[i]);
                    if (fruittypeconfig == "")
                        continue;
                    string[] sFruitSubArray = fruittypeconfig.Split(';');
                    for (int j = 0; j < sFruitSubArray.Length-1; j++)
                    {
                        m_FruitType.member[i * ConstPreDefine.MAX_FRUIT_TYPE_SUB_CLASS_NUM + j].iFruitTypeID = i * ConstPreDefine.MAX_FRUIT_TYPE_SUB_CLASS_NUM + j + 1;
                        btempArray = Encoding.Default.GetBytes(sFruitSubArray[j]);
                        Array.Copy(btempArray, m_FruitType.member[i * ConstPreDefine.MAX_FRUIT_TYPE_SUB_CLASS_NUM + j].bFruitName, btempArray.Length);
                        this.FruitTypedataGridView[j+1, i].Value = sFruitSubArray[j];
                    }
                }
                /*标记已选水果种类*/
                fruittypeconfig = Commonfunction.GetAppSetting("已选水果种类");
                string[] sFruitSelecttArray = fruittypeconfig.Split(';');//分割已选水果种类
                for (int i = 0; i < sFruitSelecttArray.Length-1; i++)
                {
                    //string[] tempArray = s.Split(new char[3] { 'c', 'd', 'e' });
                    //string[] tempArray = sFruitSelecttArray[i].Split('-');//分割ID号
                    string[] tempArray = sFruitSelecttArray[i].Split(new char[2] { '.', '-'});//分割ID号
                    int ID = int.Parse(tempArray[0]);
                    int ColoID= int.Parse(tempArray[1]);
                    int RowIndex = ID -1/*/ ConstPreDefine.MAX_FRUIT_TYPE_SUB_CLASS_NUM*/;
                    int ColoumnIndex = ColoID % ConstPreDefine.MAX_FRUIT_TYPE_SUB_CLASS_NUM;
                    this.FruitTypedataGridView[ColoumnIndex, RowIndex].Style.BackColor = Color.Pink;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-FruitSet中函数FruitSetInitial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-FruitSet中函数FruitSetInitial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 双击表格（选定水果品种）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FruitTypedataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((e.ColumnIndex != 0) && (this.FruitTypedataGridView[e.ColumnIndex, e.RowIndex].Value != null))
                {
                    if (this.FruitTypedataGridView[e.ColumnIndex, e.RowIndex].Style.BackColor == Color.Pink)
                    {
                        this.FruitTypedataGridView[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                    }
                    else
                    {
                        this.FruitTypedataGridView[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Pink;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-FruitSet中函数FruitTypedataGridView_CellDoubleClick出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-FruitSet中函数FruitTypedataGridView_CellDoubleClick出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 验证输入，排除数字以外按键，限制密度只能输入数字键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FruitTypedataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (this.FruitTypedataGridView.CurrentCell.ColumnIndex == 0)
                {
                    e.Control.KeyPress += new KeyPressEventHandler(FruitTypedataGridViewCells_KeyPress);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-FruitSet中函数FruitTypedataGridView_EditingControlShowing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-FruitSet中函数FruitTypedataGridView_EditingControlShowing出错" + ex);
#endif
            }
        }
        
        /// <summary>
        /// 验证输入，排除数字以外按键，限制密度只能输入数字键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FruitTypedataGridViewCells_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-FruitSet中函数FruitTypedataGridView_EditingControlShowing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-FruitSet中函数FruitTypedataGridViewCells_KeyPress出错" + ex);
#endif
            }
        }

        bool IsCellValidate = false;
        private void FruitTypedataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex < 0 || e.RowIndex < 0)  //Add by ChengSk - 20180507
                    return;

                if (IsCellValidate)
                {
                    IsCellValidate = false;
                    if (e.ColumnIndex == 0)
                    {
                        this.FruitTypedataGridView[e.ColumnIndex, e.RowIndex].Value = GlobalDataInterface.globalOut_AnalogDensity.uAnalogDensity[e.RowIndex].ToString("#0.00");
                    }
                }
                if ((this.FruitTypedataGridView[e.ColumnIndex, e.RowIndex].Value == null) && e.ColumnIndex>0)
                {
                    this.FruitTypedataGridView[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-FruitSet中函数FruitTypedataGridView_CellValidating出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-FruitSet中函数FruitTypedataGridView_CellValidating出错" + ex);
#endif
            }
        }


        /// <summary>
        /// 验证输入，限制密度只能输入数字,范围为0-255
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FruitTypedataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                FruitTypedataGridView.Rows[e.RowIndex].ErrorText = "";
                int result = -1;
                if (e.ColumnIndex == 0)
                {
                    int i = FruitTypedataGridView.CurrentCell.ColumnIndex;
                    //string regexString = "^[0-9]+(.[0-9]{2})?$";
                    //Match M = Regex.Match(e.FormattedValue.ToString(), regexString);
                    //if (!M.Success)
                    //{
                    //    e.Cancel = true;
                    //    FruitTypedataGridView.Rows[e.RowIndex].ErrorText = "Density formate input error,please reenter!";
                    //    return;
                    //}
                    //else
                    //{

                    //    GlobalDataInterface.globalOut_AnalogDensity.uAnalogDensity[e.RowIndex] = float.Parse(e.FormattedValue.ToString());
                    //    this.FruitTypedataGridView[e.ColumnIndex, e.RowIndex].Value = GlobalDataInterface.globalOut_AnalogDensity.uAnalogDensity[e.RowIndex].ToString("#0.00");
                    //}
                    int.TryParse(e.FormattedValue.ToString(), out result);
                    if ((result > 1000) || (result < 0))
                    {
                        e.Cancel = true;
                        FruitTypedataGridView.Rows[e.RowIndex].ErrorText = "Density formate input error,please reenter!";
                        IsCellValidate = false;
                        return;
                    }
                    else
                    {

                        GlobalDataInterface.globalOut_AnalogDensity.uAnalogDensity[e.RowIndex] = float.Parse(e.FormattedValue.ToString());
                        IsCellValidate = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-FruitSet中函数FruitTypedataGridView_CellValidating出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-FruitSet中函数FruitTypedataGridView_CellValidating出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 立即生效按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FruitEffectbutton_Click(object sender, EventArgs e)
        {
            try
            {
                

                string furitselect = "";
                this.FruitEffectbutton.Enabled = false;
                this.EffectButtonDelaytimer5.Enabled = true;
                for (int i = 0; i < this.FruitTypedataGridView.Rows.Count; i++)
                {
                    string fruitsub = "";
                    for (int j = 1; j < this.FruitTypedataGridView.ColumnCount; j++)
                    {
                        if (this.FruitTypedataGridView[j, i].Value != null)
                            fruitsub += this.FruitTypedataGridView[j, i].Value.ToString().TrimEnd('\0') + ";";

                        if (this.FruitTypedataGridView[j, i].Style.BackColor == Color.Pink)
                            furitselect += (i+1).ToString() +"."+j+ '-'
                                           + this.FruitTypedataGridView[j, i].Value.ToString().TrimEnd('\0') + ";";
                    }

                    Commonfunction.SetAppSetting(sFruitMajortArray[i], fruitsub);                 
                }
                
                Commonfunction.SetAppSetting("已选水果种类", furitselect);
                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_DENSITY_INFO, null);//下发密度信息
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-FruitSet中函数FruitEffectbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-FruitSet中函数FruitEffectbutton_Click出错" + ex);
#endif
            }

        }
        /// <summary>
        /// 立即生效后延迟1.5秒再启用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffectButtonDelaytimer5_Tick(object sender, EventArgs e)
        {
            this.FruitEffectbutton.Enabled = true;
            this.EffectButtonDelaytimer5.Enabled = false;
        }
       
    }
}
