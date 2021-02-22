using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interface;
using Common;
using System.Diagnostics;
using System.Resources;

namespace FruitSortingVtest1
{
    public partial class TestCupForm : Form
    {
        private ResourceManager m_resourceManager = new ResourceManager(typeof(TestCupForm));//创建TestCupForm资源管理
        public TestCupForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestCupForm_Load(object sender, EventArgs e)
        {
            this.Infolabel.Text = m_resourceManager.GetString("PrepareInfolabel.Text");
            this.Testbutton.Text = m_resourceManager.GetString("Testbutton.Text");
        }

        /// <summary>
        /// 果杯测试按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Testbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Testbutton.Text == m_resourceManager.GetString("Testbutton.Text"))
                {
                    this.Testbutton.Text = m_resourceManager.GetString("StopTestlabel.Text");
                    this.Infolabel.Text = m_resourceManager.GetString("TestInfolabel.Text");
                    if (GlobalDataInterface.global_IsTestMode)
                        GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_CUP_ON, null);
                }
                else
                {
                    this.Testbutton.Text = m_resourceManager.GetString("Testbutton.Text");
                    this.Infolabel.Text = m_resourceManager.GetString("PrepareInfolabel.Text");
                    if (GlobalDataInterface.global_IsTestMode)
                        GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_CUP_OFF, null);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("TestCupForm中函数Testbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("TestCupForm中函数Testbutton_Click出错" + ex);
#endif
            }
        }

        
        /// <summary>
        /// 关闭窗口事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestCupForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (this.Testbutton.Text == m_resourceManager.GetString("StopTestlabel.Text"))
                {
                    this.Testbutton.Text = m_resourceManager.GetString("Testbutton.Text");
                    this.Infolabel.Text = m_resourceManager.GetString("PrepareInfolabel.Text");
                    if (GlobalDataInterface.global_IsTestMode)
                        GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_CUP_OFF, null);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("TestCupForm中函数TestCupForm_FormClosed出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("TestCupForm中函数TestCupForm_FormClosed出错" + ex);
#endif
            }
        }
    }
}
