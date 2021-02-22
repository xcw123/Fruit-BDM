using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interface;
using Common;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace FruitSortingVtest1._0
{
    public partial class MainForm : Form
    {

        /// <summary>
        /// 菜单->文件->加载配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 加载配置toolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadConfigNewForm lcForm = new LoadConfigNewForm(this, null, false);
            lcForm.ShowDialog();
        }

        /// <summary>
        /// 菜单->文件->保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 保存配置toolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveConfigForm scForm = new SaveConfigForm(this,false);
            scForm.ShowDialog();
        }

        /// <summary>
        /// 菜单->文件->退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 菜单->设置->等级设定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 等级设定toolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalDataInterface.gradeForm = new GradeSetForm(this);
            GlobalDataInterface.gradeForm.ShowDialog();
        }

        /// <summary>
        /// 菜单->设置->工程设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 工程设置toolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (GlobalDataInterface.OpenProjectSetFormNumber > 0)
            //    return; //Add by ChengSk - 20190111 每次仅打开1个工程配置窗体
            ValidateForm validateForm = new ValidateForm();
            if (validateForm.ShowDialog() == DialogResult.OK)
            {
                if (GlobalDataInterface.global_IsTestMode)
                    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_PROJ_OPENED, null);
                GlobalDataInterface.projectSet = new ProjectSetForm(this);
                GlobalDataInterface.projectSet.ShowDialog();
            }
        }

        /// <summary>
        /// 菜单->设置->备份设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 备份设置toolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "FS格式(*.fs)|";
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string temp = dlg.FileName + ".fs";
                    FileInfo File = new FileInfo(temp);
                    if (File.Exists)
                    {
                        //DialogResult result = MessageBox.Show("是否覆盖原来的配置信息?", "保存配置", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //DialogResult result = MessageBox.Show("0x30001021 Whether to overwrite the original configuration information?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        DialogResult result = MessageBox.Show("0x30001021 " + LanguageContainer.MainMenuMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.MainMenuMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }
                    if(Commonfunction.BackupConfigure(dlg.FileName))
                        //MessageBox.Show("Backup configure successed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show(LanguageContainer.MainMenuMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.MainMenuMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        //MessageBox.Show("Backup configure failed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show(LanguageContainer.MainMenuMessagebox3Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.MainMenuMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainMenu中函数备份设置toolStripMenuItem_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainMenu中函数备份设置toolStripMenuItem_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 菜单->设置->恢复设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 恢复设置toolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "FS格式(*.fs)|";
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if(Commonfunction.RecoveryConfigure(dlg.FileName))
                        //MessageBox.Show("Recovery configure successed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show(LanguageContainer.MainMenuMessagebox4Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.MainMenuMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        //MessageBox.Show("Recovery configure failed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show(LanguageContainer.MainMenuMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                            LanguageContainer.MainMenuMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainMenu中函数恢复设置toolStripMenuItem_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainMenu中函数恢复设置toolStripMenuItem_Click出错" + ex);
#endif
            }
        }

        

        /// <summary>
        /// 菜单->工具->电磁阀测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 电磁阀测试toolStripMenuItem_Click(object sender, EventArgs e)
        {
            VolveTestForm volveTestForm = new VolveTestForm();
            volveTestForm.ShowDialog();
        }

       
        /// <summary>
        /// 菜单->工具->电机使能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 电机使能ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ElectromagnetictestForm electromagnetictest = new ElectromagnetictestForm();
            electromagnetictest.ShowDialog();
        }


        /// <summary>
        /// 菜单->工具->数据清零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 数据清零toolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount < 100)
                {
                    this.SeparationEfficiencyChangelabel.Text = "0.0%";
                    this.RealWeightCountChangelabel.Text = "0.00";
                    this.AverWeightCountChangelabel.Text = "0.0";
                    if (GlobalDataInterface.global_IsTestMode)
                    {
                        GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_CLEAR_DATA, null);
                        GlobalDataInterface.WriteErrorInfo("***** 操作员点击了清零操作，当前批个数：" +
                            GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount.ToString() + " *****"); //Add by ChengSk - 20181228
                    }
                    m_ClearZero = true;
                }
                else
                {
                    //if (MessageBox.Show("确定进行数据清零操作？", "温馨提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    //if (MessageBox.Show("0x30001108 Are you sure to clear data？", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    if (MessageBox.Show("0x30001108 " + LanguageContainer.MainMenuMessagebox6Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.MainMenuMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        this.SeparationEfficiencyChangelabel.Text = "0.0%";
                        this.RealWeightCountChangelabel.Text = "0.00";
                        this.AverWeightCountChangelabel.Text = "0.0";
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_CLEAR_DATA, null);
                            GlobalDataInterface.WriteErrorInfo("***** 操作员点击了清零操作，当前批个数：" +
                            GlobalDataInterface.dataInterface.IoStStatistics.nTotalCount.ToString() + " *****"); //Add by ChengSk - 20181228
                        }  
                        m_ClearZero = true;
                    }
                }
                
                m_ExitSortingStatisticDic = new Dictionary<int, Queue<string>>(); //初始化
                m_AllSortingStatisticQueue = new Queue<string>(); //初始化
                for (int i = 0; i < GlobalDataInterface.ExitList.Count; i++)    //Add by ChengSk - 20180122
                {
                    Queue<string> exitSortSumQueue = new Queue<string>();
                    m_ExitSortingStatisticDic.Add(i, exitSortSumQueue);   //初始化时出口的统计量为空
                }
                GlobalDataInterface.uCurrentSampleExitFruitTotals = 0;    //Add by ChengSk - 20180202
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainMenu中函数数据清零toolStripMenuItem_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainMenu中函数数据清零toolStripMenuItem_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 菜单->工具->出口清空  Add by ChengSk - 20190929
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 出口清空toolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show(LanguageContainer.MainFormMessagebox18Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.MainFormMessageboxQuestionCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return;

                int sizeNum = 1;
                int qulNum = 1;
                if (GlobalDataInterface.globalOut_GradeInfo.nClassifyType > 0 && GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum > 0)//品质与尺寸或者品质与重量
                {
                    qulNum = GlobalDataInterface.globalOut_GradeInfo.nQualityGradeNum;
                }
                if (GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum > 0)
                    sizeNum = GlobalDataInterface.globalOut_GradeInfo.nSizeGradeNum;
                for (int i = 0; i < qulNum; i++)
                {
                    for (int j = 0; j < sizeNum; j++)
                    {
                        GlobalDataInterface.globalOut_GradeInfo.grades[i * ConstPreDefine.MAX_SIZE_GRADE_NUM + j].exit = 0;
                    }
                }
                //SetAllExitListBox();
                //for (int i = 0; i < sizeNum; i++)
                //{
                //    for(int j=0; j< qulNum; j++)
                //    {
                //        this.GradedataGridView[i, j].Style.BackColor = Color.Pink;
                //        this.GradedataGridView[i, j].Style.SelectionBackColor = Color.Pink; //Add by ChengSk - 20191024
                //    }
                //}
                if (GlobalDataInterface.global_IsTestMode)
                {
                    //GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);//更新到FSM
                    int global_IsTest = GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_GRADE_INFO, null);
                    if (global_IsTest != 0) //add by xcw 20201211
                    {
                        MessageBox.Show(LanguageContainer.MainFormMessagebox19Text[GlobalDataInterface.selectLanguageIndex],
                                        LanguageContainer.MainFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex],
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                SetAllExitListBox();
                for (int i = 0; i < sizeNum; i++)
                {
                    for (int j = 0; j < qulNum; j++)
                    {
                        this.GradedataGridView[i, j].Style.BackColor = Color.Pink;
                        this.GradedataGridView[i, j].Style.SelectionBackColor = Color.Pink; //Add by ChengSk - 20191024
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainMenu中函数出口清空toolStripMenuItem_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainMenu中函数出口清空toolStripMenuItem_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 菜单->工具->复位果杯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 复位果杯toolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ResetCupState();
                this.复位果杯toolStripMenuItem.Enabled = false;
                if (GlobalDataInterface.global_IsTestMode)
                    GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_WEIGHTRESET, null);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainMenu中函数复位果杯toolStripMenuItem_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainMenu中函数复位果杯toolStripMenuItem_Click出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 菜单->工具->测试果杯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 测试果杯toolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestCupForm testCupForm = new TestCupForm();
            testCupForm.ShowDialog();
        }

        /// <summary>
        /// 菜单->工具->固定窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 固定窗口toolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
                if (toolStripMenuItem.Checked == false) //未选中
                {
                    this.splitContainer2.IsSplitterFixed = true;
                    this.splitContainer3.IsSplitterFixed = true;
                    toolStripMenuItem.Checked = true;

                    //拆分器距离
                    GlobalDataInterface.SplitterDistance2 = splitContainer2.SplitterDistance;
                    Commonfunction.SetAppSetting("拆分器距离", splitContainer2.SplitterDistance.ToString());

                    //拆分器距离
                    GlobalDataInterface.SplitterDistance3 = splitContainer3.SplitterDistance;
                    Commonfunction.SetAppSetting("拆分器距离2", splitContainer3.SplitterDistance.ToString());

                    //准备选中
                    if (bIsOnceMinimumSized)
                    {
                        GlobalDataInterface.ExitVerticalScroll = currentExitVerticalScroll;  //更新当前滚动条位置
                    }
                    else
                    {
                        GlobalDataInterface.ExitVerticalScroll = this.splitContainer2.Panel1.VerticalScroll.Value;
                    }         
                    //出口垂直滚动条保存
                    Commonfunction.SetAppSetting("出口垂直滚动条", GlobalDataInterface.ExitVerticalScroll.ToString());
                }
                else //选中
                {
                    this.splitContainer2.IsSplitterFixed = false;
                    this.splitContainer3.IsSplitterFixed = false;
                    toolStripMenuItem.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainMenu中函数固定窗口toolStripMenuItem_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainMenu中函数固定窗口toolStripMenuItem_Click出错" + ex);
#endif
            }
        }
        /// <summary>
        /// 菜单->工具->fSM程序烧写
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fSM程序烧写ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalDataInterface.bootFlashBurnForm = new BootFlashBurnForm();
            GlobalDataInterface.bootFlashBurnForm.ShowDialog();
        }

        /// <summary>
        /// WIFI功能:打开WIFI,IPAD终端出口信息显示系统才能接收到数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WIFI功能ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
                if (toolStripMenuItem.Checked == false)
                {
                    ////绑定固定IP检查
                    //string hostName = Dns.GetHostName();
                    //IPHostEntry IPEntry = Dns.GetHostEntry(hostName);
                    //IPAddress HCIP = IPAddress.Parse(ConstPreDefine.HC_IP_ADDR);
                    //string currentIP = "";
                    //bool IsExistCorrentIP = false;
                    //for (int i = 0; i < IPEntry.AddressList.Length; i++)
                    //{
                    //    if (IPEntry.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    //    {
                    //        currentIP = IPEntry.AddressList[i].ToString();
                    //        if (string.Equals(currentIP, ConstPreDefine.BROADCAST_LOCAL_IP_ADDR))
                    //        {
                    //            IsExistCorrentIP = true;
                    //            break;
                    //        }
                    //    }
                    //}
                    //if (!IsExistCorrentIP)
                    //{
                    //    //MessageBox.Show("IP地址错误或没有网络连接！");
                    //    MessageBox.Show("0x10000001 BROADCAST_LOCAL_IP address error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return;
                    //}
                    toolStripMenuItem.Checked = true;
                    GlobalDataInterface.sendBroadcastPackage = true;
                }
                else
                {
                    toolStripMenuItem.Checked = false;
                    GlobalDataInterface.sendBroadcastPackage = false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MainMenu中函数WIFI功能ToolStripMenuItem_Click出错，错误原因：" + ex.ToString());
            }
        }

        /// <summary>
        /// 菜单->工具->瑕疵检测测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 瑕疵检测测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalDataInterface.spotDetectTestForm = new SpotDetectTestForm();
            GlobalDataInterface.spotDetectTestForm.ShowDialog();
        }

        /// <summary>
        /// 关闭IPM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 关闭IPMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IPMOperationForm ipmOperationForm = new IPMOperationForm();
            ipmOperationForm.ShowDialog();
            ////统计每个子系统的通道数
            //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
            //{
            //    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
            //    {
            //        if (GlobalDataInterface.globalOut_SysConfig.nChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
            //        {
                        
                            
                      
            //        }
            //    }
            //}
        }

        private void 数据恢复ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataValidateForm datavalidateForm = new DataValidateForm();
            if (datavalidateForm.ShowDialog() == DialogResult.OK)
            {
                DataDownloadForm datadownloadForm = new DataDownloadForm();
                datadownloadForm.ShowDialog();
            }
        }

        /// <summary>
        /// 用户手册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 用户手册ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string strManualName = Commonfunction.GetAppSetting("用户手册");
                string fileNewPat = Application.StartupPath;
                if (!Directory.Exists(fileNewPat + "\\handbook\\"))
                {
                    Directory.CreateDirectory(fileNewPat + "\\handbook\\");
                }
                strManualName = strManualName.Replace("**", GlobalDataInterface.selectLanguage);//Add by xcw - 20191031
                string fileName = fileNewPat + "\\handbook\\" + strManualName;
                //MessageBox.Show("**" + fileName + "**");
                if (File.Exists(fileName))
                {
                    Process process1 = new Process();
                    process1.StartInfo.FileName = fileName;
                    process1.StartInfo.Arguments = "";
                    process1.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                    process1.Start();
                }
                else
                {
                    MessageBox.Show("File does not exist!");
                }
            }
            catch (Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("MainForm中函数用户手册ToolStripMenuItem_Click出错" + ex);
#endif
            }
        }



        /// <summary>
        /// 语言切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 语言切换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageSelectForm languageSelectForm = new LanguageSelectForm(1);
            languageSelectForm.ShowDialog();
        }

        /// <summary>
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog(); //Add by ChengSk - 20191111
        }

        /// <summary>
        /// 等级设定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GradetoolStripButton_Click(object sender, EventArgs e)
        {
            GlobalDataInterface.gradeForm = new GradeSetForm(this);
            GlobalDataInterface.gradeForm.ShowDialog();
        }

        /// <summary>
        /// 内部品质
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QualitytoolStripButton_Click(object sender, EventArgs e)
        {
            GlobalDataInterface.innerQualityForm = new InnerQualityForm(this);
            GlobalDataInterface.innerQualityForm.ShowDialog();
        }


        /// <summary>
        /// 品质设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QaulitytoolStripButton_Click(object sender, EventArgs e)
        {
            QualGradeSetForm qualGradeSetForm = new QualGradeSetForm(this);
            qualGradeSetForm.ShowDialog();
        }

        /// <summary>
        /// 水果信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FruitInfotoolStripButton_Click(object sender, EventArgs e)
        {
            GlobalDataInterface.fruitParamForm = new FruitParamForm();
            GlobalDataInterface.fruitParamForm.ShowDialog();
        }

        /// <summary>
        /// 载入程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadtoolStripButton_Click(object sender, EventArgs e)
        {
            LoadConfigNewForm lcForm = new LoadConfigNewForm(this, null, false);
            lcForm.ShowDialog();
        }

        /// <summary>
        /// 保存程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavetoolStripButton_Click(object sender, EventArgs e)
        {
            SaveConfigForm scForm = new SaveConfigForm(this,false);
            scForm.ShowDialog();
        }
    }
}
