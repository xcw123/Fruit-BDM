using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ListViewEx;
using GlacialComponents.Controls;
using Interface;
using Common;
using System.Diagnostics;
using System.Resources;

namespace FruitSortingVtest1
{
    public partial class ElectromagnetictestForm : Form   //Add by xcw - 20191031
    {
        private static List<int> m_ChanelIDList = new List<int>();
        int m_ChannelcomboBoxSelectedIndex = 0;
        int m_ChannelSeleccomboBoxSelectedIndex = 0;     //Add by ChengSk
        private static int m_ExitSubsysindex = 0;        //当前选择通道所属子系统
        private static int m_ExitSubsysChannelIndex = 0; //当前选择通道所属子系统第几个通道
        private stVolveTest m_VolveTest = new stVolveTest(true);//电磁阀测试参数
        private static stExitInfo[] tempExitInfo = new stExitInfo[ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM]; //Add by ChengSk
        private stGlobalExitInfo[] tempGlobalExitInfo = new stGlobalExitInfo[ConstPreDefine.MAX_SUBSYS_NUM];

        private static int m_ChannelExitMouseDownType = 0;//出口列表鼠标按下类型，0左键单击；1左键双击；2右键
        private static int m_CurrentItemIndex = 0;
        private bool m_IsVolveTesting = false;
        private Control[] ExitEditors;
        bool m_ChannelcomboBoxClick = false;
        private ResourceManager m_resourceManager = new ResourceManager(typeof(ElectromagnetictestForm));//创建VolveTestForm资源管理
        public ElectromagnetictestForm()
        {
            InitializeComponent();

            int MaxEntryNum = 20; //不适用滚动条的情况下显示的最大条目
            if (GlobalDataInterface.globalOut_SysConfig.nExitNum <= MaxEntryNum)
            {
                this.Height = (85 + 24 * (GlobalDataInterface.globalOut_SysConfig.nExitNum + 1) + 45);
                this.ExitlistViewEx.Height = 24 * (GlobalDataInterface.globalOut_SysConfig.nExitNum + 1);
                this.btnClosed.Location = new Point(200, this.Height - 65); //Add by xcw - 20191031
                //ExitlistViewEx.Location= new Point(100, this.Height - 40);
            }
            else
            {
                this.Height = (85 + 24 * (MaxEntryNum + 1) + 45); //30是边框
                
                this.ExitlistViewEx.Height = 24 * (MaxEntryNum + 1);
                this.btnClosed.Location = new Point(283, this.Height - 28 - 30 - 6);
                this.btnClosed.Location = new Point(200, this.Height - 65); //Add by xcw - 20191031
            }
        }
        /// <summary>
        /// 窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElectromagnetictestForm_Load(object sender, EventArgs e)
        {
            try
            {
                m_ChanelIDList.Clear();
                Commonfunction.GetAllChannelID(GlobalDataInterface.globalOut_SysConfig.nChannelInfo, ref m_ChanelIDList);

                //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                //{
                //    tempExitInfo[i] = new stExitInfo(true);
                //}
                //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                //{
                //    tempGlobalExitInfo[i] = new stGlobalExitInfo(true);

                //}
                //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                //{
                //    tempExitInfo[i].ToCopy(GlobalDataInterface.globalOut_ExitInfo[i]);
                //}
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    tempGlobalExitInfo[i].ToCopy(GlobalDataInterface.globalOut_GlobalExitInfo[i]);
                }
                //GlobalDataInterface.globalOut_GlobalExitInfo.CopyTo(tempGlobalExitInfo, 0);
                //this.AllEnablingbutton.Text = m_resourceManager.GetString("StartTestbutton.Text");
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
               
                this.ExitlistViewEx.Items.Clear();
                int SelId = m_ChanelIDList[0];
                m_ExitSubsysindex = Commonfunction.GetSubsysIndex(SelId);
                m_ExitSubsysChannelIndex = Commonfunction.GetChannelIndex(SelId);


                ListViewItem lvi;
                    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nExitNum; i++)
                    {
                        lvi = new ListViewItem(m_resourceManager.GetString("Outletlabel.Text") + string.Format(" {0}", i + 1));
                    //lvi.SubItems.Add(string.Format("{0}", tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nOffset));
                    //lvi.SubItems.Add(string.Format("{0}", tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDis));
                      lvi.SubItems.Add(tempGlobalExitInfo[m_ExitSubsysindex].Delay_time[i].ToString());
                      lvi.SubItems.Add(tempGlobalExitInfo[m_ExitSubsysindex].Hold_time[i].ToString());
                    //lvi.SubItems.Add(string.Format("{0}", tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[i].nDriverPin));  //Note by ChengSk - 20180402
                    this.ExitlistViewEx.Items.Add(lvi);
                    }
               
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数ChannelExitIntial出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数ChannelExitIntial出错" + ex);
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

       
        ///// <summary>
        ///// 通道出口保存配置
        ///// </summary>
        ///// <returns></returns>
        //private bool ChannelExitSaveConfig()
        //{
        //    try
        //    {
        //        if (m_ExitSubsysindex >= 0 && m_ExitSubsysChannelIndex >= 0 && m_ChannelSeleccomboBoxSelectedIndex >= 0)
        //        {
        //            for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
        //            {
        //                GlobalDataInterface.globalOut_ExitInfo[i].ToCopy(tempExitInfo[i]);
        //            }

        //            if (GlobalDataInterface.global_IsTestMode)
        //            {
        //                GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelSeleccomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_EXIT_INFO, null);
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show("通道出口设置界面保存配置出错：" + ex);
        //        //MessageBox.Show("0x10001007 Lane Outlet save error: " + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        MessageBox.Show("0x10001007 " + LanguageContainer.VolveTestFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
        //            LanguageContainer.VolveTestFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex],
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return false;
        //    }

        //}

        //private void 单击测试ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    stVolveTest volveTest = new stVolveTest(true);
        //    volveTest.ExitId = (byte)(m_CurrentMotorItemIndex + 48);
        //    if (GlobalDataInterface.global_IsTestMode)
        //        GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ExitChannelSelectIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, volveTest);
        //}

        private void 单机测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                stVolveTest volveTest = new stVolveTest(true);

                volveTest.ExitId = (byte)(m_CurrentItemIndex + 48);
                    GlobalDataInterface.TransmitParam(m_ChanelIDList[m_ChannelSeleccomboBoxSelectedIndex], (int)HC_FSM_COMMAND_TYPE.HC_CMD_TEST_VOLVE, volveTest);

            }
            catch (Exception ex)
            {
                Trace.WriteLine("ProjectSetForm-ChannelExit中函数Start_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProjectSetForm-ChannelExit中函数Start_Click出错" + ex);
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
                    //if (e.Item.BackColor == Color.Red) //测试中
                    //{
                    //    this.VolveTestcontextMenuStrip.Items[1].Enabled = true;
                    //    this.VolveTestcontextMenuStrip.Items[0].Enabled = false;
                    //}
                    //else//未测试
                    //{
                    //    this.VolveTestcontextMenuStrip.Items[0].Enabled = true;
                    //    this.VolveTestcontextMenuStrip.Items[1].Enabled = false;
                    //}
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
                            tempGlobalExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].Delay_time[e.Item.Index]= float.Parse(e.DisplayText);
                        break;
                    case 2:
                        if (e.Item.Index >= 0)
                            tempGlobalExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].Hold_time[e.Item.Index] = float.Parse(e.DisplayText);
                        //tempExitInfo[m_ExitSubsysindex * ConstPreDefine.MAX_CHANNEL_NUM + m_ExitSubsysChannelIndex].exits[e.Item.Index].nDis = (short)int.Parse(e.DisplayText);
                        break;
                    default: break;
                }
                //tempGlobalExitInfo.CopyTo(GlobalDataInterface.globalOut_GlobalExitInfo, 0);//add by xcw -20191209
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数ExitlistViewEx_SubItemEndEditing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数ExitlistViewEx_SubItemEndEditing出错" + ex);
#endif
            }
        }

        private void btnClosed_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            if (GlobalDataInterface.global_IsTestMode && GlobalDataInterface.globalOut_SysConfig.nSubsysNum == 1)
            {
                GlobalDataInterface.TransmitParam(Common.Commonfunction.EncodeSubsys(0), (int)HC_FSM_COMMAND_TYPE.HC_CMD_MOTOR_ENABLE, null);
            }
        }

        private void ExitInfoSendbutton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;//等待             
                tempGlobalExitInfo.CopyTo(GlobalDataInterface.globalOut_GlobalExitInfo, 0);//add by xcw -20191209
                if (GlobalDataInterface.global_IsTestMode)
                { 
                    for (int i = 0; i < GlobalDataInterface.globalOut_SysConfig.nSubsysNum; i++)
                    {
                        GlobalDataInterface.TransmitParam(Commonfunction.EncodeSubsys(i), (int)HC_FSM_COMMAND_TYPE.HC_CMD_GLOBAL_EXIT_INFO, null);
                    }
                }
                this.Cursor = Cursors.Default;//正常状态
            }
            catch (Exception ex)
            {
                Trace.WriteLine("VolveTestForm中函数ExitInfoSendbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("VolveTestForm中函数ExitInfoSendbutton_Click出错" + ex);
#endif
            }
            
        }
    }
}
