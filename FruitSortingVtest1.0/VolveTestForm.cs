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
    public partial class VolveTestForm : Form
    {
        private static List<int> m_ChanelIDList = new List<int>();
        int m_ChannelcomboBoxSelectedIndex = -1;
        int m_ChannelSeleccomboBoxSelectedIndex = -1;     //Add by ChengSk
        private static int m_ExitSubsysindex = -1;        //当前选择通道所属子系统
        private static int m_ExitSubsysChannelIndex = -1; //当前选择通道所属子系统第几个通道
        private stVolveTest m_VolveTest = new stVolveTest(true);//电磁阀测试参数
        private static stExitInfo[] tempExitInfo = new stExitInfo[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM]; //Add by ChengSk
        private static int m_ChannelExitMouseDownType = 0;//出口列表鼠标按下类型，0左键单击；1左键双击；2右键
        private static int m_CurrentItemIndex = -1;
        private bool m_IsVolveTesting = false;
        private Control[] ExitEditors;
        bool m_ChannelcomboBoxClick = false;     
        private ResourceManager m_resourceManager = new ResourceManager(typeof(VolveTestForm));//创建VolveTestForm资源管理

        public VolveTestForm()
        {
            InitializeComponent();

            int MaxEntryNum = 20; //不适用滚动条的情况下显示的最大条目
            if (GlobalDataInterface.globalOut_SysConfig.nExitNum <= MaxEntryNum)
            {
                this.Height = 10 + 100 + 10 + (55 + 24 * (GlobalDataInterface.globalOut_SysConfig.nExitNum + 1) + 36) + 40 + 30; //30是边框
                this.groupBox1.Height = 100;
                this.groupBox1.Location = new Point(20, 10);
                this.groupBox2.Height = (55 + 24 * (GlobalDataInterface.globalOut_SysConfig.nExitNum + 1) + 36);
                this.groupBox2.Location = new Point(20, 120);
                this.ExitlistViewEx.Height = 24 * (GlobalDataInterface.globalOut_SysConfig.nExitNum + 1);
                this.ChannelExitEffectbutton.Location = new Point(264, this.groupBox2.Height - 31);
                this.Cancelbutton.Location = new Point(283, this.Height - 28 - 30 - 6);
            }
            else
            {
                this.Height = 10 + 100 + 10 + (55 + 24 * (MaxEntryNum + 1) + 36) + 40 + 30; //30是边框
                this.groupBox1.Height = 100;
                this.groupBox1.Location = new Point(20, 10);
                this.groupBox2.Height = (55 + 24 * (MaxEntryNum + 1) + 36);
                this.groupBox2.Location = new Point(20, 120);
                this.ExitlistViewEx.Height = 24 * (MaxEntryNum + 1);
                this.ChannelExitEffectbutton.Location = new Point(264, this.groupBox2.Height - 31);
                this.Cancelbutton.Location = new Point(283, this.Height - 28 - 30 - 6);
            }    
        }

        /// <summary>
        /// 窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolveTestForm_Load(object sender, EventArgs e)
        {
            try
            {
                m_ChanelIDList.Clear();
                Commonfunction.GetAllChannelID(GlobalDataInterface.globalOut_SysConfig.nChannelInfo, ref m_ChanelIDList);

                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                {
                    tempExitInfo[i] = new stExitInfo(true);
                }
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                {
                    tempExitInfo[i].ToCopy(GlobalDataInterface.globalOut_ExitInfo[i]);
                }

                for (int i = 0; i < m_ChanelIDList.Count; i++)
                {
                    this.ChannelcomboBox.Items.Add(m_resourceManager.GetString("Lanelabel.Text") + string.Format(" {0}", i + 1));
                    this.ChannelSeleccomboBox.Items.Add(m_resourceManager.GetString("Lanelabel.Text") + string.Format(" {0}", i + 1));
                }
                if (m_ChanelIDList.Count > 0)
                {
                    this.ChannelcomboBox.SelectedIndex = 0;
                    m_ChannelcomboBoxSelectedIndex = 0;
                    this.ChannelSeleccomboBox.SelectedIndex = 0;
                    m_ChannelSeleccomboBoxSelectedIndex = 0;
                }

                this.StartTestbutton.Text = m_resourceManager.GetString("StartTestbutton.Text");
                ExitEditors = new Control[] { OffsetnumericUpDown, DistancenumericUpDown };  //Modify by ChengSk - 20180402
                SetExitlistViewEx();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数VolveTestForm_Load出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数VolveTestForm_Load出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 通道出口初始化
        /// </summary>
        private void SetExitlistViewEx()
        {
            try
            {
                if (this.ChannelSeleccomboBox.SelectedIndex >= 0)
                {
                    this.ExitlistViewEx.Items.Clear();
                    int SelId = m_ChanelIDList[this.ChannelSeleccomboBox.SelectedIndex];
                    m_ExitSubsysindex = Commonfunction.GetSubsysIndex(SelId);
                    m_ExitSubsysChannelIndex = Commonfunction.GetChannelIndex(SelId);

                    ListViewItem lvi;
                    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nExitNum; i++)
                    {
                        lvi = new ListViewItem(m_resourceManager.GetString("Outletlabel.Text") + string.Format(" {0}", i + 1));
                        lvi.SubItems.Add(string.Format("{0}", tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nOffset));
                        lvi.SubItems.Add(string.Format("{0}", tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDis));
                        //lvi.SubItems.Add(string.Format("{0}", tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDriverPin));  //Note by ChengSk - 20180402
                        this.ExitlistViewEx.Items.Add(lvi);
                    }        
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数ChannelExitIntial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数ChannelExitIntial出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 通道选择改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //if (m_ChannelcomboBoxClick && m_ChannelcomboBoxSelectedIndex >= 0)
                if (m_ChannelcomboBoxSelectedIndex >= 0) //Modify by ChengSk - 20191104
                {
                    if (this.StartTestbutton.Text == m_resourceManager.GetString("StopTestlabel.Text"))
                    {
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            stVolveTest VolveTest = new stVolveTest(true);//电磁阀测试参数
                            VolveTest.ExitId = 255;
                            GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelcomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, VolveTest);
                        }
                        this.StartTestbutton.Text = m_resourceManager.GetString("StartTestbutton.Text");
                    }
                    m_ChannelcomboBoxSelectedIndex = this.ChannelcomboBox.SelectedIndex;
                    //m_ChannelcomboBoxClick = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数ChannelcomboBox_SelectedIndexChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数ChannelcomboBox_SelectedIndexChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 通道选择控件单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelcomboBox_Click(object sender, EventArgs e)
        {
            m_ChannelcomboBoxClick = true;
        }

        /// <summary>
        /// 开始测试按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartTestbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ChannelcomboBoxSelectedIndex >= 0)
                {
                    if (this.StartTestbutton.Text == m_resourceManager.GetString("StartTestbutton.Text"))
                    {
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            stVolveTest VolveTest = new stVolveTest(true);//电磁阀测试参数
                            VolveTest.ExitId = 254;
                            GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelcomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, VolveTest);
                        }
                        this.StartTestbutton.Text = m_resourceManager.GetString("StopTestlabel.Text");
                    }
                    else
                    {
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            stVolveTest VolveTest = new stVolveTest(true);//电磁阀测试参数
                            VolveTest.ExitId = 255;
                            GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelcomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, VolveTest);
                        }
                        this.StartTestbutton.Text = m_resourceManager.GetString("StartTestbutton.Text");
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数StartTestbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数StartTestbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolveTestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                //if (m_ChannelcomboBoxSelectedIndex >= 0)
                //{
                //    if (this.StartTestbutton.Text == m_resourceManager.GetString("StopTestlabel.Text"))
                //    {
                //        if (GlobalDataInterface.global_IsTestMode)
                //        {
                //            stVolveTest VolveTest = new stVolveTest(true);//电磁阀测试参数
                //            VolveTest.ExitId = 255;
                //            GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelcomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, VolveTest);
                //        }
                //        //this.StartTestbutton.Text = "开始测试";
                //    }
                //}

                if (GlobalDataInterface.global_IsTestMode) //Modify by ChengSk - 20180704
                {
                    stVolveTest VolveTest = new stVolveTest(true);//电磁阀测试参数
                    VolveTest.ExitId = 255;
                    GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelcomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, VolveTest);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数VolveTestForm_FormClosed出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数VolveTestForm_FormClosed出错" + ex);
#endif
            }
        }

        private void ChannelSeleccomboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                ComboBox combobox = (ComboBox)sender;
                if (m_ChannelSeleccomboBoxSelectedIndex != combobox.SelectedIndex)
                {
                    if (m_VolveTest.ExitId != 255)
                    {
                        if (m_VolveTest.ExitId <= 100)
                        {
                            this.ExitlistViewEx.Items[m_VolveTest.ExitId].BackColor = Color.White;
                        }
                        m_VolveTest.ExitId = 255;
                        if (GlobalDataInterface.global_IsTestMode)
                            GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelSeleccomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, m_VolveTest);
                        m_IsVolveTesting = false;
                    }
                    SetExitlistViewEx();
                    m_ChannelSeleccomboBoxSelectedIndex = combobox.SelectedIndex;
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数ChannelSeleccomboBox_SelectionChangeCommitted出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数ChannelSeleccomboBox_SelectionChangeCommitted出错" + ex);
#endif
            }
        }

        private void ExitlistViewEx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    m_ChannelExitMouseDownType = 1;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数ExitlistViewEx_MouseDoubleClick出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数ExitlistViewEx_MouseDoubleClick出错" + ex);
#endif
            }
        }

        private void ExitlistViewEx_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    m_ChannelExitMouseDownType = 0;
                else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    m_ChannelExitMouseDownType = 2;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数ExitlistViewEx_MouseDown出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数ExitlistViewEx_MouseDown出错" + ex);
#endif
            }
        }

        private void ExitlistViewEx_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            try
            {
                if (m_ChannelExitMouseDownType == 1)//左键
                {
                    if (e.SubItem > 0 && e.Item.Index >= 0)
                        this.ExitlistViewEx.StartEditing(ExitEditors[e.SubItem - 1], e.Item, e.SubItem);
                }
                else if (m_ChannelExitMouseDownType == 2)//右键
                {
                    m_CurrentItemIndex = e.Item.Index;
                    if (e.Item.BackColor == Color.Red) //测试中
                    {
                        this.VolveTestcontextMenuStrip.Items[1].Enabled = true;
                        this.VolveTestcontextMenuStrip.Items[0].Enabled = false;
                    }
                    else//未测试
                    {
                        this.VolveTestcontextMenuStrip.Items[0].Enabled = true;
                        this.VolveTestcontextMenuStrip.Items[1].Enabled = false;
                    }
                    Point point = this.ExitlistViewEx.PointToClient(Control.MousePosition);
                    this.VolveTestcontextMenuStrip.Show(this.ExitlistViewEx, point);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数ExitlistViewEx_SubItemClicked出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数ExitlistViewEx_SubItemClicked出错" + ex);
#endif
            }
        }

        private void ExitlistViewEx_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            try
            {
                ListViewEx.ListViewEx listviewex = (ListViewEx.ListViewEx)sender;

                switch (e.SubItem)
                {
                    case 1:
                        if (e.Item.Index >= 0)
                            tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index].nOffset = (short)int.Parse(e.DisplayText);
                        break;     
                    case 2:
                        if (e.Item.Index >= 0)
                            tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index].nDis = (short)int.Parse(e.DisplayText);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数ExitlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数ExitlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_VolveTest.ExitId != 255)
                {
                    if (m_VolveTest.ExitId <= 100)
                    {
                        this.ExitlistViewEx.Items[m_VolveTest.ExitId].BackColor = Color.White;
                        this.ExitlistViewEx.Items[m_VolveTest.ExitId].Font = new Font("宋体", 9, FontStyle.Regular);
                    }
                }
                m_VolveTest.ExitId = (byte)(m_CurrentItemIndex);
                this.ExitlistViewEx.Items[m_CurrentItemIndex].BackColor = Color.Red;
                this.ExitlistViewEx.Items[m_CurrentItemIndex].Font = new Font("黑体", 9, FontStyle.Italic | FontStyle.Bold);
                
                if (GlobalDataInterface.global_IsTestMode)
                {
                    if(this.chkSidebySideTest.Checked) //Modify by ChengSk - 20190726
                        GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelSeleccomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_ALL_LANE_VOLVE, m_VolveTest);
                    else
                        GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelSeleccomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, m_VolveTest);
                }
                    
                m_IsVolveTesting = true;
                m_CurrentItemIndex = -1;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数Start_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数Start_Click出错" + ex);
#endif
            }
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            try
            {
                this.ExitlistViewEx.Items[m_CurrentItemIndex].BackColor = Color.White;
                this.ExitlistViewEx.Items[m_CurrentItemIndex].Font = new Font("宋体", 9, FontStyle.Regular);//ChengSk

                if (GlobalDataInterface.global_IsTestMode)
                {
                    stVolveTest stop_VolveTest = new stVolveTest(true);//停止电磁阀测试

                    if (this.chkSidebySideTest.Checked) //Modify by ChengSk - 20190726
                        GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelSeleccomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_ALL_LANE_VOLVE, stop_VolveTest);
                    else
                        GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelSeleccomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, stop_VolveTest);
                }
                m_CurrentItemIndex = -1;
                m_IsVolveTesting = false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestFormExit中函数Stop_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数Stop_Click出错" + ex);
#endif
            }
        }

        private void ChannelExitEffectbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ChannelExitSaveConfig())
                    return;
                this.ChannelExitEffectbutton.Enabled = false;
                this.EffectButtonDelaytimer3.Enabled = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestFormExit中函数ChannelExitEffectbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数ChannelExitEffectbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 通道出口保存配置
        /// </summary>
        /// <returns></returns>
        private bool ChannelExitSaveConfig()
        {
            try
            {
                if (m_ExitSubsysindex >= 0 && m_ExitSubsysChannelIndex >= 0 && m_ChannelSeleccomboBoxSelectedIndex >= 0)
                {
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    {
                        GlobalDataInterface.globalOut_ExitInfo[i].ToCopy(tempExitInfo[i]);
                    }

                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelSeleccomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("通道出口设置界面保存配置出错：" + ex);
                //MessageBox.Show("0x10001007 Lane Outlet save error: " + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("0x10001007 " + LanguageContainer.VolveTestFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.VolveTestFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        /// <summary>
        /// 立即生效后延迟1.5秒再启用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffectButtonDelaytimer3_Tick(object sender, EventArgs e)
        {
            this.ChannelExitEffectbutton.Enabled = true;
            this.EffectButtonDelaytimer3.Enabled = false;
        }
    }
}
