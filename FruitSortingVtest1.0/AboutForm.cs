using Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FruitSortingVtest1
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            try
            {
                labelHCCompileDateTime.Text = System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location).ToString();

                string strYear = "";
                string strMonth = "";
                string strDay = "";
                string strHour = "00";
                string strMinute = "00";
                string strSecond = "00";

                string IPMInfo = Encoding.Default.GetString(GlobalDataInterface.globalIn_defaultInis[0].cIPMInfo, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                //labelIPMCompileDateTime.Text = IPMInfo.Substring(0, IPMInfo.IndexOf("\0"));
                IPMInfo = IPMInfo.Trim();
                //IPMInfo = "201911121710";
                if (GlobalDataInterface.nVer == 0) //Modify by xcw - 20200903
                {
                    labelVersion.Text = "5.0-L";
                }
                else if (GlobalDataInterface.nVer == 1)
                {
                    labelVersion.Text = "5.0-S";
                }
                if (IPMInfo.Length >= 12)
                {
                    strYear = IPMInfo.Substring(0, 4);
                    strMonth= IPMInfo.Substring(4, 2);
                    strDay = IPMInfo.Substring(6, 2);
                    strHour = IPMInfo.Substring(8, 2);
                    strMinute = IPMInfo.Substring(10, 2);
                    strSecond = "00";
                }
                labelIPMCompileDateTime.Text = strYear + "/" + strMonth + "/" + strDay + " " + strHour + ":" + strMinute + ":" + strSecond;

                string cFSMInfo = Encoding.Default.GetString(GlobalDataInterface.globalIn_defaultInis[0].cFSMInfo, 0, ConstPreDefine.MAX_TEXT_LENGTH);
                //labelFSMCompileDateTime.Text = cFSMInfo.Substring(0, cFSMInfo.IndexOf("\0"));
                cFSMInfo = cFSMInfo.Trim();
                //cFSMInfo = "20191213";
                if(cFSMInfo.Length >= 8)
                {
                    strYear = cFSMInfo.Substring(0, 4);
                    strMonth = cFSMInfo.Substring(4, 2);
                    strDay = cFSMInfo.Substring(6, 2);
                    strHour = "00";
                    strMinute = "00";
                    strSecond = "00";
                }
                labelFSMCompileDateTime.Text = strYear + "/" + strMonth + "/" + strDay + " " + strHour + ":" + strMinute + ":" + strSecond;
            }
            catch(Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("AboutForm中函数AboutForm_Load出错：" + ex.ToString());
#endif
            }
        }
    }
}
