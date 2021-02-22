using Common;
using FruitSortingVtest1.DB;
using Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FruitSortingVtest1
{
    public partial class DeviceRegisterForm : Form
    {
        public DeviceRegisterForm()
        {
            InitializeComponent();
        }

        private void DeviceRegisterForm_Load(object sender, EventArgs e)
        {
            this.combMacAddress.Text = GlobalDataInterface.MacAddress;
            List<string> lstMacAddress = GetMacAddress();
            if(lstMacAddress != null && lstMacAddress.Count > 0)
            {
                for(int i=0; i<lstMacAddress.Count; i++)
                {
                    this.combMacAddress.Items.Add(lstMacAddress[i].Replace(":", "-"));
                }
            }
            //this.txtMacAddress.Text = GetMacAddress().Replace(":", "-");  //从本机获取MAC
            this.txtDeviceNumber.Text = GlobalDataInterface.DeviceNumber;
            this.dtpFactoryTime.Text = GlobalDataInterface.FactoryTime;
            this.txtCountry.Text = GlobalDataInterface.Country;
            this.txtArea.Text = GlobalDataInterface.Area;
            this.txtDetailAddress.Text = GlobalDataInterface.DetailAddress;
            this.txtContactor.Text = GlobalDataInterface.Contactor;
            this.txtPhoneNumber.Text = GlobalDataInterface.PhoneNumber;
            this.txtTVAccount.Text = GlobalDataInterface.TVAccount;
            this.txtTVPassword.Text = GlobalDataInterface.TVPassword;
        }

        /// <summary>  
        /// 获取本机MAC地址  
        /// </summary>  
        /// <returns>本机MAC地址</returns>  
        public static List<string> GetMacAddress()
        {
            try
            {
                List<string> strMac = new List<string>();
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        strMac.Add(mo["MacAddress"].ToString());
                    }
                }
                moc = null;
                mc = null;
                return strMac;
            }
            catch
            {
                return null;
            }
        }

        private void RegisterEvent(DeviceInfo arg)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(RegisterEventThread), arg);
            }
            catch (Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DataDownloadForm中函数HttpTest出错" + ex);
#endif
            }
        }

        private void RegisterEventThread(object arg)
        {
            try
            {
                DeviceInfo deviceInfo = new DeviceInfo();
                deviceInfo = (DeviceInfo)arg;

                string strData = Commonfunction.getJsonByObject(deviceInfo);
                string strUrl = GlobalDataInterface.ServerURL + "UpLoadDeviceRegisterInfo?data=";
                string result = HttpHelper.OpenReadWithHttps(strUrl, strData, 10000, new IPEndPoint(IPAddress.Parse(GlobalDataInterface.ServerBindLocalIP), 0));
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                string reMessage = jo["message"].ToString();
                string reResult = jo["result"].ToString();
                string reStatus = jo["status"].ToString();
                string reStatusCode = jo["statusCode"].ToString();
                if ((reMessage.Contains("注册成功") || reMessage.Contains("更新成功")) && reMessage.Contains(GlobalDataInterface.DeviceNumber))
                {
                    MessageBox.Show(reMessage, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(reMessage, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DataDownloadForm中函数RegisterEventThread出错" + ex);
#endif
                MessageBox.Show("注册失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if(this.txtDeviceNumber.Text.Trim() == "")
            {
                MessageBox.Show("设备编号不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtDeviceNumber.Focus();
                return;
            }
            if(this.combMacAddress.Text.Trim() == "")
            {
                MessageBox.Show("MAC地址不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.combMacAddress.Focus();
                return;
            }
            if(this.txtRegistrationCode.Text.Trim() == "")
            {
                MessageBox.Show("注册码不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtRegistrationCode.Focus();
                return;
            }

            DeviceInfo deviceInfo = new DeviceInfo();
            deviceInfo.DeviceNumber = this.txtDeviceNumber.Text.Trim();
            deviceInfo.MacAddress = this.combMacAddress.Text.Trim();
            deviceInfo.FactoryTime = this.dtpFactoryTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            deviceInfo.Country = this.txtCountry.Text.Trim();
            deviceInfo.Area = this.txtArea.Text.Trim();
            deviceInfo.DetailAddress = this.txtDetailAddress.Text.Trim();
            deviceInfo.Contactor = this.txtContactor.Text.Trim();
            deviceInfo.PhoneNumber = this.txtPhoneNumber.Text.Trim();
            deviceInfo.TVAccount = this.txtTVAccount.Text.Trim();
            deviceInfo.TVPassword = this.txtTVPassword.Text.Trim();
            deviceInfo.RegistrationCode = this.txtRegistrationCode.Text.Trim();

            Commonfunction.SetAppSetting("设备编号", deviceInfo.DeviceNumber);
            Commonfunction.SetAppSetting("Mac地址", deviceInfo.MacAddress);
            Commonfunction.SetAppSetting("出厂日期", deviceInfo.FactoryTime);
            Commonfunction.SetAppSetting("国家", deviceInfo.Country);
            Commonfunction.SetAppSetting("地区", deviceInfo.Area);
            Commonfunction.SetAppSetting("详细地址", deviceInfo.DetailAddress);
            Commonfunction.SetAppSetting("负责人", deviceInfo.Contactor);
            Commonfunction.SetAppSetting("联系电话", deviceInfo.PhoneNumber);
            Commonfunction.SetAppSetting("TV账号", deviceInfo.TVAccount);
            Commonfunction.SetAppSetting("TV密码", deviceInfo.TVPassword);

            GlobalDataInterface.DeviceNumber = deviceInfo.DeviceNumber;
            GlobalDataInterface.MacAddress = deviceInfo.MacAddress;
            GlobalDataInterface.FactoryTime = deviceInfo.FactoryTime;
            GlobalDataInterface.Country = deviceInfo.Country;
            GlobalDataInterface.Area = deviceInfo.Area;
            GlobalDataInterface.DetailAddress = deviceInfo.DetailAddress;
            GlobalDataInterface.Contactor = deviceInfo.Contactor;
            GlobalDataInterface.PhoneNumber = deviceInfo.PhoneNumber;
            GlobalDataInterface.TVAccount = deviceInfo.TVAccount;
            GlobalDataInterface.TVPassword = deviceInfo.TVPassword;

            RegisterEvent(deviceInfo);
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void combMacAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int index = combMacAddress.SelectedIndex;
            //this.txtDeviceNumber.Text = "RM" + DateTime.Now.ToString("yyyyMMdd") +
            //    combMacAddress.Items[index].ToString().Replace("-", "");
        }
    }
}
