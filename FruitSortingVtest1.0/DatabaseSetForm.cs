using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Interface;
using System.Diagnostics;

namespace FruitSortingVtest1
{
    public partial class DatabaseSetForm : Form
    {
        public DatabaseSetForm()
        {
            InitializeComponent();
        }

        private void DatabaseSetForm_Load(object sender, EventArgs e)
        {
            TxtDataSource.Text = Common.Commonfunction.GetAppSetting("数据源");
            TxtDataBase.Text = Common.Commonfunction.GetAppSetting("数据库");
            TxtUserName.Text = Common.Commonfunction.GetAppSetting("用户名");
            TxtPassword.Text = Common.Commonfunction.GetAppSetting("密码");
        }

        private void BtnConnectTest_Click(object sender, EventArgs e)
        {
            string strConn = "";
            strConn = "data source=" + TxtDataSource.Text.Trim() + ";database=" + TxtDataBase.Text.Trim() +
                ";user=" + TxtUserName.Text.Trim() + ";pwd=" + TxtPassword.Text.Trim();
            bool isConnSuccess = false;
            SqlConnection conn = new SqlConnection(strConn);
            try
            {
                conn.Open();
                isConnSuccess = true;
                conn.Close();   //Add 20180919
                conn.Dispose(); //Add 20180919
            }
            catch (Exception ex)
            {
                isConnSuccess = false;
            }
            if (isConnSuccess)
            {
                //MessageBox.Show("数据库连接成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //MessageBox.Show("0x30003101 Connect to sql server succeed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("0x30003101 " + LanguageContainer.DatabaseSetFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.DatabaseSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //MessageBox.Show("连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //MessageBox.Show("0x30003102 Connect to sql server failed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("0x30003102 " + LanguageContainer.DatabaseSetFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.DatabaseSetFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            GlobalDataInterface.currentDatabase = "success";
            GlobalDataInterface.dataBaseConn = "data source=" + TxtDataSource.Text.Trim() + ";database=" + TxtDataBase.Text.Trim() +
                ";user=" + TxtUserName.Text.Trim() + ";pwd=" + TxtPassword.Text.Trim();
            Common.Commonfunction.SetAppSetting("当前数据库", GlobalDataInterface.currentDatabase);
            Common.Commonfunction.SetAppSetting("数据源", TxtDataSource.Text.Trim());
            Common.Commonfunction.SetAppSetting("数据库", TxtDataBase.Text.Trim());
            Common.Commonfunction.SetAppSetting("用户名", TxtUserName.Text.Trim());
            Common.Commonfunction.SetAppSetting("密码", TxtPassword.Text.Trim());
            GlobalDataInterface.DatabaseSet = false;
            this.Close();
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Return)
                {
                    BtnConnectTest_Click(null, null);
                    this.BtnOK.Focus();
                }
            }
            catch (Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DatabaseSetForm中函数TxtPassword_KeyPress出错" + ex);
#endif
            }
        }

       

        
    }
}
