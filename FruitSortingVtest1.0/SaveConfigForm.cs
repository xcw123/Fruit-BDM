using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Interface;
using Common;
using System.Diagnostics;
using System.Resources;
using FruitSortingVtest1.DB;

namespace FruitSortingVtest1._0
{
    public partial class SaveConfigForm : Form
    {
        private bool m_IsProjectConfig;
        MainForm m_mainform;
        private ResourceManager m_resourceManager = new ResourceManager(typeof(SaveConfigForm));//创建SaveConfigForm资源管理

        public SaveConfigForm(MainForm mainform,bool IsProjectConfig)
        {
            m_mainform = mainform;
            m_IsProjectConfig = IsProjectConfig;
            InitializeComponent();
        }

        /// <summary>
        /// 保存配置界面初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveConfigForm_Load(object sender, EventArgs e)
        {
            try
            {
                List<string> configList = new List<string>();
                if (m_IsProjectConfig)
                {
                    this.Text = m_resourceManager.GetString("SaveProjectConfiglabel.Text");
                    Commonfunction.GetAllProjectSettingFileName(ref configList);
                }
                else
                {
                    this.Text = m_resourceManager.GetString("SaveUserConfiglabel.Text");
                    Commonfunction.GetAllCommonSettingFileName(ref configList);
                }
                if (configList.Count > 0)
                {
                    for (int i = 0; i < configList.Count; i++)
                    {
                        this.ConfiglistBox.Items.Add(configList[i]);
                    }
                    this.ConfiglistBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("SaveConfigForm中函数SaveConfigForm_Load出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("SaveConfigForm中函数SaveConfigForm_Load出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 配置文件列表选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfiglistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListBox listBox = (ListBox)sender;
                if (listBox.SelectedIndex < 0)
                    return;
                this.EditConfigtextBox.Text = (string)listBox.Items[listBox.SelectedIndex];
            }
            catch (Exception ex)
            {
                Trace.WriteLine("SaveConfigForm中函数ConfiglistBox_SelectedIndexChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("SaveConfigForm中函数ConfiglistBox_SelectedIndexChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 保存配置控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveConfigbutton_Click(object sender, EventArgs e)
        {
            if (this.EditConfigtextBox.Text == "")
            {
                //MessageBox.Show("配置文件名称不能为空！");
                //MessageBox.Show("0x30001020 The configuration file's name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("0x30001020 " + LanguageContainer.SaveConfigFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.SaveConfigFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.EditConfigtextBox.Focus();
                return;
            }
            if (!Directory.Exists(System.Environment.CurrentDirectory + "\\config\\" + GlobalDataInterface.VERSION_SHOW+"\\"))
                Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\config\\"+ GlobalDataInterface.VERSION_SHOW+"\\");
            string FileName = System.Environment.CurrentDirectory; 
            FileName += "\\config\\" + GlobalDataInterface.VERSION_SHOW + "\\";
            //if (this.EditConfigtextBox.Text=="")
            //{
                FileName += this.EditConfigtextBox.Text;
                if (m_IsProjectConfig)
                    FileName += ".exp";
                else
                    FileName += ".cmc";
            //}
            //else
            //{
            //    FileName += (string)this.ConfiglistBox.Items[this.ConfiglistBox.SelectedIndex]; 
            //}
            
            FileInfo configFile = new FileInfo(FileName);
            byte[] FileData;
            FileStream configStream;
            try
            {
                if (configFile.Exists)
                {
                    //DialogResult result = MessageBox.Show("是否覆盖原来的配置信息?", "保存配置", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //DialogResult result = MessageBox.Show("0x30001021 Whether to overwrite the original configuration information?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    DialogResult result = MessageBox.Show("0x30001021 " + LanguageContainer.SaveConfigFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.SaveConfigFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                configStream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
                if (m_IsProjectConfig)
                {
                    //寻找最长结构体长度
                    int MaxLenth = Marshal.SizeOf(typeof(stSysConfig));
                    if (MaxLenth < Marshal.SizeOf(typeof(stExitInfo)))
                        MaxLenth = Marshal.SizeOf(typeof(stExitInfo));
                    if (MaxLenth < Marshal.SizeOf(typeof(stWeightBaseInfo)))
                        MaxLenth = Marshal.SizeOf(typeof(stWeightBaseInfo));
                    if (MaxLenth < Marshal.SizeOf(typeof(stParas)))
                        MaxLenth = Marshal.SizeOf(typeof(stParas));
                    if (MaxLenth < Marshal.SizeOf(typeof(stGlobalExitInfo)))
                        MaxLenth = Marshal.SizeOf(typeof(stGlobalExitInfo));
                    if (MaxLenth < Marshal.SizeOf(typeof(stGlobalWeightBaseInfo)))
                        MaxLenth = Marshal.SizeOf(typeof(stGlobalWeightBaseInfo));
                    //if (MaxLenth < Marshal.SizeOf(typeof(stSpotDetectThresh)))
                    //    MaxLenth = Marshal.SizeOf(typeof(stSpotDetectThresh));

                    FileData = new byte[MaxLenth];
                    configStream.Seek(0, SeekOrigin.Begin);

                    byte[] version = BitConverter.GetBytes(GlobalDataInterface.Version);
                    configStream.Write(version, 0, sizeof(Int32));//2016-12-6加版本号校验

                    FileData = Commonfunction.StructToBytes(GlobalDataInterface.globalOut_SysConfig);
                    configStream.Write(FileData, 0, Marshal.SizeOf(typeof(stSysConfig)));

                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    {
                        FileData = Commonfunction.StructToBytes(GlobalDataInterface.globalOut_ExitInfo[i]);
                        configStream.Write(FileData, 0, Marshal.SizeOf(typeof(stExitInfo)));
                    }
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM; i++)
                    {
                        FileData = Commonfunction.StructToBytes(GlobalDataInterface.globalOut_WeightBaseInfo[i]);
                        configStream.Write(FileData, 0, Marshal.SizeOf(typeof(stWeightBaseInfo)));
                    }
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM; i++)
                    {
                        FileData = Commonfunction.StructToBytes(GlobalDataInterface.globalOut_Paras[i]);
                        configStream.Write(FileData, 0, Marshal.SizeOf(typeof(stParas)));
                    }
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                    {
                        FileData = Commonfunction.StructToBytes(GlobalDataInterface.globalOut_GlobalExitInfo[i]);
                        configStream.Write(FileData, 0, Marshal.SizeOf(typeof(stGlobalExitInfo)));
                    }
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                    {
                        FileData = Commonfunction.StructToBytes(GlobalDataInterface.globalOut_GlobalWeightBaseInfo[i]);
                        configStream.Write(FileData, 0, Marshal.SizeOf(typeof(stGlobalWeightBaseInfo)));
                    }
                    //for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_IPM_NUM; i++)
                    //{
                    //    FileData = Commonfunction.StructToBytes(GlobalDataInterface.globalOut_SpotDetectThresh[i]);
                    //    configStream.Write(FileData, 0, Marshal.SizeOf(typeof(stSpotDetectThresh)));
                    //}

                    configStream.Write(FileData, 0, sizeof(bool));
                    FileData = BitConverter.GetBytes(GlobalDataInterface.CIRAvailable);
                    configStream.Write(FileData, 0, sizeof(bool));
                    FileData = BitConverter.GetBytes(GlobalDataInterface.WeightAvailable);
                    configStream.Write(FileData, 0, sizeof(bool));
                    GlobalDataInterface.NetStateSum = BitConverter.ToBoolean(FileData, 0);
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                    {
                        FileData = BitConverter.GetBytes(GlobalDataInterface.NetState[i]);
                        configStream.Write(FileData, 0, sizeof(int));
                    }
                    FileData = BitConverter.GetBytes(GlobalDataInterface.CupStateSum);
                    configStream.Write(FileData, 0, sizeof(bool));
                    for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                    {
                        FileData = BitConverter.GetBytes(GlobalDataInterface.CupState[i]);
                        configStream.Write(FileData, 0, sizeof(int));
                    }
                    configStream.Close();
                }
                else//保存用户配置
                {
                    FileData = new byte[Marshal.SizeOf(typeof(stGradeInfo))];
                    configStream.Seek(0, SeekOrigin.Begin);

                    byte[] version = BitConverter.GetBytes(GlobalDataInterface.Version);
                    configStream.Write(version, 0, sizeof(Int32));//2016-12-6加版本号校验

                    FileData = Commonfunction.StructToBytes(GlobalDataInterface.globalOut_GradeInfo);
                    configStream.Write(FileData, 0, Marshal.SizeOf(typeof(stGradeInfo)));

                    /*用户配置增加颜色界面-》颜色列表背景颜色保存*/
                    string color;
                    byte[] tempcolor = new byte[ConstPreDefine.MAX_TEXT_LENGTH]; 
                    stColorList colorlist = new stColorList(true);
                    color = Commonfunction.GetAppSetting("颜色参数-颜色1");
                    tempcolor = Encoding.Default.GetBytes(color);
                    Array.Copy(tempcolor, colorlist.color1, tempcolor.Length);
                    color = Commonfunction.GetAppSetting("颜色参数-颜色2");
                    tempcolor = Encoding.Default.GetBytes(color);
                    Array.Copy(tempcolor, colorlist.color2, tempcolor.Length);
                    color = Commonfunction.GetAppSetting("颜色参数-颜色3");
                    tempcolor = Encoding.Default.GetBytes(color);
                    Array.Copy(tempcolor, colorlist.color3, tempcolor.Length);
                    FileData = Commonfunction.StructToBytes(colorlist);
                    configStream.Write(FileData, 0, Marshal.SizeOf(typeof(stColorList)));

                    //Add by ChengSk - 20190929
                    string clientinfo;
                    byte[] tempclientinfo = new byte[ConstPreDefine.MAX_CLIENTINFO_LENGTH];
                    string ClientName = ""; //客户名称 Add by ChengSk - 20191115
                    string FarmName = "";   //农场名称 Add by ChengSk - 20191115
                    string FruitName = "";  //水果名称 Add by ChengSk - 20191115
                    string clientInfoContent = FileOperate.ReadFile(1, m_mainform.clientInfoFileName); //Modify by ChengSk - 20180308
                    if (clientInfoContent == null || clientInfoContent == "")
                    {
                        ClientName = "";
                        FarmName = "";
                        FruitName = "";
                    }
                    else
                    {
                        string[] clientInfoContentItem = clientInfoContent.Split('，');
                        ClientName = clientInfoContentItem[0].Trim();
                        FarmName = clientInfoContentItem[1].Trim();
                        FruitName = clientInfoContentItem[2].Trim();
                    }
                    stClientInfo ClientInfo = new stClientInfo(true);
                    clientinfo = ClientName;   //客户名称
                    tempclientinfo = Encoding.Default.GetBytes(clientinfo);
                    Array.Copy(tempclientinfo, ClientInfo.customerName, tempclientinfo.Length);
                    clientinfo = FarmName;     //农场名称
                    tempclientinfo = Encoding.Default.GetBytes(clientinfo);
                    Array.Copy(tempclientinfo, ClientInfo.farmName, tempclientinfo.Length);
                    clientinfo = FruitName;    //水果名称
                    tempclientinfo = Encoding.Default.GetBytes(clientinfo);
                    Array.Copy(tempclientinfo, ClientInfo.fruitName, tempclientinfo.Length);
                    FileData = Commonfunction.StructToBytes(ClientInfo);
                    configStream.Write(FileData, 0, Marshal.SizeOf(typeof(stClientInfo)));

                    configStream.Close();
                    m_mainform.SetSeparationProgrameChangelabel(true, this.EditConfigtextBox.Text);//ivycc 2013.11.26
                }

            }
            catch(Exception ex)
            {
                //MessageBox.Show("保存配置文件失败：" + ex);
                //MessageBox.Show("0x1000100C Save configuration failed:" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("0x1000100C " + LanguageContainer.SaveConfigFormMessagebox3Text[GlobalDataInterface.selectLanguageIndex] + ex,
                    LanguageContainer.SaveConfigFormMessageboxErrorCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
