using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interface;
using System.Threading;
using System.Diagnostics;
using Common;
using FruitSortingVtest1.DB;
using System.IO;

namespace FruitSortingVtest1
{
    public partial class LanguageSelectForm : Form
    {
        bool bChangeLanguageFlags = false;
        string strCurrentLangeage = "";

        public LanguageSelectForm()
        {
            InitializeComponent();
        }

        public LanguageSelectForm(int changeLanguage)
        {
            InitializeComponent();

            bChangeLanguageFlags = true;     //切换语言版本
        }

        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageSelectForm_Load(object sender, EventArgs e)
        {
            //遍历语言类型
            foreach(int language in Enum.GetValues(typeof(LANGUAGE_TYPE)))
            {
                string strLanName = Enum.GetName(typeof(LANGUAGE_TYPE),language);//获取名称
                LanguageSelectcomboBox.Items.Add(strLanName);
            }

            GlobalDataInterface.selectLanguage = Common.Commonfunction.GetAppSetting("选择的语言");
            if (GlobalDataInterface.selectLanguage == "null" || GlobalDataInterface.selectLanguage == "")
            {
                LanguageSelectcomboBox.Text = "";
            }
            else
            {
                switch(GlobalDataInterface.selectLanguage)
                {
                    case "zh":
                        LanguageSelectcomboBox.Text = "Chinese";
                        break;
                    case "en":
                        LanguageSelectcomboBox.Text = "English";
                        break;
                    case "es":
                        LanguageSelectcomboBox.Text = "Spanish";
                        break;
                    default:
                        break;
                }
            }
            strCurrentLangeage = GlobalDataInterface.selectLanguage;
        }
        string path;//add by xcw - 20191028
        string currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
        
        /// <summary>
        /// 检查是否存在文件夹
        /// </summary>
        public void change()
        {
            PrintProtocol.logoPathName = currentPath + GlobalDataInterface.SelectlogoPathName;  //更改LOGO标签地址(相对地址转绝对地址)//Add by xcw - 20200615
            string currentDefaultPath = currentPath + "config\\"; //Add by xcw - 20191031
            path = currentDefaultPath + "languagekind.txt";
            if (!Directory.Exists(currentDefaultPath))
            {
                Directory.CreateDirectory(currentDefaultPath);
            }

            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
            }

        }
        /// <summary>
        /// 清空文本
        /// </summary>
        public void ClearTxt()
        {
            change();
            FileStream fs = new FileStream(@path, FileMode.Truncate, FileAccess.ReadWrite); //Add by xcw - 20191031
            fs.Close();
        }
        /// <summary>
        /// 写入文本文件
        /// </summary>
        /// <param name="value"></param>
        public void text(string value)
        {
            FileStream f = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite); //Add by xcw - 20191031
            StreamWriter sw = new StreamWriter(f);
            sw.WriteLine(value);
            sw.Flush();
            sw.Close();
            f.Close();
        }
        /// <summary>
        /// 确认按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKbutton_Click(object sender, EventArgs e)
        {
            
            switch (LanguageSelectcomboBox.SelectedIndex)
            {
                case (int)LANGUAGE_TYPE.Chinese:
                    GlobalDataInterface.selectLanguage = "zh";
                    break;
                case (int)LANGUAGE_TYPE.English:
                    GlobalDataInterface.selectLanguage = "en";
                    break;
                case (int)LANGUAGE_TYPE.Spanish:
                    GlobalDataInterface.selectLanguage = "es";
                    break;
                default:
                    return;
            }

            Common.Commonfunction.SetAppSetting("选择的语言", GlobalDataInterface.selectLanguage);    //Note by ChengSk - 20190926

            if (bChangeLanguageFlags == true && GlobalDataInterface.selectLanguage != strCurrentLangeage)
            {
                //MessageBox.Show("版本语言切换成功，重启软件后生效！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult result = MessageBox.Show(LanguageContainer.LanguageSelectFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.LanguageSelectFormMessageboxQuestionCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    List<int> ChanelIDList = new List<int>();
                    Commonfunction.GetAllChannelID(GlobalDataInterface.globalOut_SysConfig.nChannelInfo, ref ChanelIDList);
                    if (ChanelIDList.Count > 0)
                    {
                        if (GlobalDataInterface.global_IsTestMode)
                        {
                            GlobalDataInterface.TransmitParam(-1, (int)HC_FSM_COMMAND_TYPE.HC_CMD_DISPLAY_OFF, null);
                        }
                    }
                    Common.Commonfunction.SetAppSetting("选择的语言", GlobalDataInterface.selectLanguage); //Add by ChengSk - 20190926
                    try
                    {
                        if (GlobalDataInterface.usedSeparationLogFlags)
                        {
                            DataBaseOperation databaseOperation = new DataBaseOperation();
                            DataSet dst = databaseOperation.GetRunningTimeInfoByStopTime("");
                            if (dst.Tables[0].Rows.Count > 0)
                            {
                                DateTime dt = DateTime.Now;
                                databaseOperation.UpdateRunningStopTime(int.Parse(dst.Tables[0].Rows[0]["ID"].ToString()), dt.ToString("HH:mm:ss"));
                            }
                        }
                    }
                    catch (Exception ex) { }

                    //Application.ExitThread();
                    //Thread thtmp = new Thread(new ParameterizedThreadStart(run));
                    //object appName = Application.ExecutablePath;
                    //Thread.Sleep(1);
                    //thtmp.Start(appName);//软件关闭重启 Add by ChengSk - 20180723

                    //Common.Commonfunction.SetAppSetting("选择的语言", GlobalDataInterface.selectLanguage); //Add by ChengSk - 20190926
                    
                    ////方法1
                    //Application.Restart(); //软件自动重启 Modify by ChengSk - 20190926

                    //方法2
                    //开启新的实例
                    Process.Start(Application.ExecutablePath);
                    //关闭当前实例
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    GlobalDataInterface.selectLanguage = strCurrentLangeage;     //Add by xcw - 20191031
                    Common.Commonfunction.SetAppSetting("选择的语言", GlobalDataInterface.selectLanguage);
                    this.Close();
                }
            }
            //else if(bChangeLanguageFlags == false) //Add by ChengSk - 20191104
            //{
            //    Common.Commonfunction.SetAppSetting("选择的语言", GlobalDataInterface.selectLanguage);
            //}

            ClearTxt(); //Add by xcw - 20191031
            text(GlobalDataInterface.selectLanguage);

            this.Close();
        }

        private void run(Object obj)
        {
            Process ps = new Process();
            ps.StartInfo.FileName = obj.ToString();
            ps.Start();
        }

        private void Closedbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
