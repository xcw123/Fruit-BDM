using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FruitSortingVtest1.DB;
using Interface;

namespace FruitSortingVtest1
{
    public partial class ClientInfoUpdateForm : Form
    {
        private ProcessInfoForm processInfoForm;
        private string strCustomerName; //所有保存的客户名称
        private string[] customerName;
        private string strFarmName;     //所有保存的农场名称
        private string[] farmName;
        private string strFruitName;    //所有保存的水果品种
        private string[] fruitName;
        private DataBaseOperation databaseOperation = new DataBaseOperation();    //创建数据库操作对象

        public ClientInfoUpdateForm()
        {
            InitializeComponent();
        }

        private void ClientInfoUpdateForm_Load(object sender, EventArgs e)
        {
            processInfoForm = (ProcessInfoForm)this.Owner;
            //获取客户信息
            strCustomerName = FileOperate.ReadFile(2, processInfoForm.mainForm.clientInfoFileName);
            strFarmName = FileOperate.ReadFile(3, processInfoForm.mainForm.clientInfoFileName);
            strFruitName = FileOperate.ReadFile(4, processInfoForm.mainForm.clientInfoFileName);
            //往ComboBox中添加选项
            if (strCustomerName != null)
            {
                CboClientName.Items.Clear();
                customerName = strCustomerName.Split('，');
                for (int i = 0; i < customerName.Length; i++)
                {
                    CboClientName.Items.Add(customerName[i]);
                }
            }
            if (strFarmName != null)
            {
                CboFarmName.Items.Clear();
                farmName = strFarmName.Split('，');
                for (int i = 0; i < farmName.Length; i++)
                {
                    CboFarmName.Items.Add(farmName[i]);
                }
            }
            if (strFruitName != null)
            {
                CboFruitName.Items.Clear();
                fruitName = strFruitName.Split('，');
                for (int i = 0; i < fruitName.Length; i++)
                {
                    CboFruitName.Items.Add(fruitName[i]);
                }
            }
            //当前选中的ComboBox框
            CboClientName.Text = processInfoForm.currentSelectCustomerName.Trim();
            CboFarmName.Text = processInfoForm.currentSelectFarmName.Trim();
            CboFruitName.Text = processInfoForm.currentSelectFruitName.Trim();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string strClientNameTemp = CboClientName.Text.Trim();
                string strFarmNameTemp = CboFarmName.Text.Trim();
                string strFruitNameTemp = CboFruitName.Text.Trim();
                //更新水果信息的客户信息
                //bool bUpdateFruitCustomerInfo = BusinessFacade.UpdateFruitCustomerInfo(Convert.ToInt32(processInfoForm.currentSelectCustomerID),
                //    strClientNameTemp, strFarmNameTemp, strFruitNameTemp);
                bool bUpdateFruitCustomerInfo = databaseOperation.UpdateFruitCustomerInfo(Convert.ToInt32(processInfoForm.currentSelectCustomerID),
                    strClientNameTemp, strFarmNameTemp, strFruitNameTemp);
                if (!bUpdateFruitCustomerInfo)
                {
                    //MessageBox.Show("更新水果信息客户信息失败！");
                    //MessageBox.Show("0x30003103 Update customer information failed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("0x30003103 " + LanguageContainer.ClientInfoUpdateFormRangeMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.ClientInfoUpdateFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //更新已保存的客户信息
                string clientInfoContent = strClientNameTemp + "，" + strFarmNameTemp + "，" + strFruitNameTemp;
                StringBuilder sb = new StringBuilder();
                if (processInfoForm.mainForm.clientInfoContent == null)
                {
                    sb.Append(clientInfoContent);
                    sb.Append("\r\n" + strClientNameTemp);
                    sb.Append("\r\n" + strFarmNameTemp);
                    sb.Append("\r\n" + strFruitNameTemp);
                    FileOperate.WriteFile(sb, processInfoForm.mainForm.clientInfoFileName);
                    processInfoForm.mainForm.clientInfoContent = clientInfoContent;
                }
                else
                {
                    FileOperate.EditFile(1, clientInfoContent, processInfoForm.mainForm.clientInfoFileName);
                }
                //再次获取客户信息
                strCustomerName = FileOperate.ReadFile(2, processInfoForm.mainForm.clientInfoFileName);
                strFarmName = FileOperate.ReadFile(3, processInfoForm.mainForm.clientInfoFileName);
                strFruitName = FileOperate.ReadFile(4, processInfoForm.mainForm.clientInfoFileName);
                FileOperate.EditFile(2, FunctionInterface.CombineString(strCustomerName, strClientNameTemp), processInfoForm.mainForm.clientInfoFileName);
                FileOperate.EditFile(3, FunctionInterface.CombineString(strFarmName, strFarmNameTemp), processInfoForm.mainForm.clientInfoFileName);
                FileOperate.EditFile(4, FunctionInterface.CombineString(strFruitName, strFruitNameTemp), processInfoForm.mainForm.clientInfoFileName);
                //更新ComboBox框
                processInfoForm.UpdateComboBoxContent();
                //更新主界面客户信息
                GlobalDataInterface.dataInterface.CustomerName = strClientNameTemp;
                GlobalDataInterface.dataInterface.FarmName = strFarmNameTemp;
                GlobalDataInterface.dataInterface.FruitName = strFruitNameTemp;
                processInfoForm.mainForm.UpdateClientInfoState();
                //更新当前选中项
                if (UpdateListViewEvent != null)
                {
                    UpdateListViewEvent(strClientNameTemp, strFarmNameTemp, strFruitNameTemp);
                }
                this.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }  
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public delegate void ListViewEventHandler(string clientName,string farmName,string fruitName);
        public static event ListViewEventHandler UpdateListViewEvent;
    }
}
