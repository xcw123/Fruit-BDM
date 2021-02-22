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
    public partial class ValidateForm : Form
    {
        public ValidateForm()
        {
            InitializeComponent();
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            try
            {
#if REALEASE
                if (GlobalDataInterface.global_IsTestMode)
            {
                if (this.PasswordtextBox.Text == "service2020")
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Dispose();
                    this.Close();
                }
                else
                {
                    //MessageBox.Show("密码输入错误！");
                    //MessageBox.Show("0x10001009 Password error!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    MessageBox.Show("0x10001009 " + LanguageContainer.ValidateFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ValidateFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
#endif

                if (this.PasswordtextBox.Text == "")
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Dispose();
                    this.Close();
                }
                else
                {
                    //MessageBox.Show("密码输入错误！");
                    //MessageBox.Show("0x10001009 Password error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show("0x10001009 " + LanguageContainer.ValidateFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ValidateFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ValidateForm中函数OKbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ValidateForm中函数OKbutton_Click出错" + ex);
#endif
            }
           
        }

        /// <summary>
        /// 回车
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordtextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Return)
                {
#if REALEASE
                    if (GlobalDataInterface.global_IsTestMode)
                {
                    if (this.PasswordtextBox.Text == "service2020")
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        //MessageBox.Show("密码输入错误！");
                        //MessageBox.Show("0x10001009 Password error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        MessageBox.Show("0x10001009 " + LanguageContainer.ValidateFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.ValidateFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
#endif

                    if (this.PasswordtextBox.Text == "")
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        //MessageBox.Show("密码输入错误！");
                        //MessageBox.Show("0x10001009 Password error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        MessageBox.Show("0x10001009 " + LanguageContainer.ValidateFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.ValidateFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ValidateForm中函数PasswordtextBox_KeyPress出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ValidateForm中函数PasswordtextBox_KeyPress出错" + ex);
#endif
            }
        }

        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PasswordtextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Passwordlabel_Click(object sender, EventArgs e)
        {

        }
    }
}
