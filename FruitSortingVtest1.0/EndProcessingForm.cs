using FruitSortingVtest1._0;
using Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FruitSortingVtest1
{
    public partial class EndProcessingForm : Form
    {
        MainForm m_mainForm;

        public EndProcessingForm(MainForm mainForm)
        {
            InitializeComponent();
            m_mainForm = mainForm;
        }

        private void EndProcessingForm_Load(object sender, EventArgs e)
        {

        }
        int endsavemode = 0; //关闭模式 0 - 是； 1 - 否 ； 2 - 取消
        private void buttonOK_Click(object sender, EventArgs e)
        {
            m_mainForm.EndSaveMode = "1";
            endsavemode = 0;
            m_mainForm.bIsEndProcess = true;
            this.Close();
        }

        private void buttonNO_Click(object sender, EventArgs e)
        {
            m_mainForm.EndSaveMode = "2";
            endsavemode = 1;
            m_mainForm.bIsEndProcess = true;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            m_mainForm.bIsEndProcess = false;
            endsavemode = 2;
            this.Close();
        }

        private void EndProcessingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (endsavemode == 2)
            {
                if ((GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x01) == 1 ||
               (GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x04) == 4 ||
               (GlobalDataInterface.globalOut_SysConfig.nClassificationInfo & 0x08) == 8)
                {
                    m_mainForm.bIsEndProcess = false;
                }
            }
           
            
        }
        
    }
}
