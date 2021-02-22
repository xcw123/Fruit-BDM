using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Interface;
using System.Diagnostics;

namespace FruitSortingVtest1._0
{
    public partial class BootFlashBurnForm : Form
    {
        byte[] m_FileData;
        FileInfo m_file;
        public BootFlashBurnForm()
        {
            InitializeComponent();
            GlobalDataInterface.UpBurnFlashProgressEvent += new GlobalDataInterface.BurnFlashProgressEventHandler(OnUpBurnFlashProgressEvent); 
        }

        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BootFlashBurnForm_Load(object sender, EventArgs e)
        {
            try
            {
                //this.FileRoottextBox.Text = System.Environment.CurrentDirectory;
                this.Startbutton.Enabled = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("BootFlashBurnForm中函数BootFlashBurnForm_Load出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("BootFlashBurnForm中函数BootFlashBurnForm_Load出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 浏览按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scanbutton_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = "";
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = System.Environment.CurrentDirectory;
                openFileDialog.Filter = "bin files(*.bin)|*.bin|All files(*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;
                    m_file = new FileInfo(openFileDialog.FileName);
                    m_FileData = new byte[m_file.Length];
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader reader = new BinaryReader(fs);
                    m_FileData = reader.ReadBytes((int)m_file.Length);
                    reader.Close();
                    fs.Close();
                    this.FileRoottextBox.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("BootFlashBurnForm中函数Scanbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("BootFlashBurnForm中函数Scanbutton_Click出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 开始烧写
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Startbutton_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] data = new byte[m_file.Length + sizeof(int)];//下传参数
                byte[] datalength = new byte[sizeof(int)];
                if (this.DataLenghthnumericUpDown.Text == "0")
                {
                    //MessageBox.Show("0x30001032 Please input the data length!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("0x30001032 " + LanguageContainer.BootFlashBurnFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                        LanguageContainer.BootFlashBurnFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                datalength = BitConverter.GetBytes(int.Parse(this.DataLenghthnumericUpDown.Text));
                Array.Copy(datalength, 0, data, 0, datalength.Length);
                Array.Copy(m_FileData, 0, data, datalength.Length - 1, m_FileData.Length);
                if (GlobalDataInterface.global_IsTestMode)
                {
                    GlobalDataInterface.TransmitParam(Common.Commonfunction.EncodeSubsys(0), (int)HC_FSM_COMMAND_TYPE.HC_CMD_BOOT_FLASH_BURN, data);//只给16发送
                }
                this.Startbutton.Enabled = false;

            }
            catch (Exception ex)
            {
                Trace.WriteLine("BootFlashBurnForm中函数Startbutton_Click出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("BootFlashBurnForm中函数Startbutton_Click出错" + ex);
#endif
            }

        }

        /// <summary>
        /// 数据长度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataLenghthnumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(this.DataLenghthnumericUpDown.Text) > 0)
                {
                    this.BurnprogressBar.Maximum = int.Parse(this.DataLenghthnumericUpDown.Text);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("BootFlashBurnForm中函数DataLenghthnumericUpDown_ValueChanged出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("BootFlashBurnForm中函数DataLenghthnumericUpDown_ValueChanged出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 文件地址悬停事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileRoottextBox_MouseHover(object sender, EventArgs e)
        {
            try
            {
                this.FileRoottoolTip.SetToolTip(FileRoottextBox, this.FileRoottextBox.Text);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("BootFlashBurnForm中函数FileRoottextBox_MouseHover出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("BootFlashBurnForm中函数FileRoottextBox_MouseHover出错" + ex);
#endif
            }
        }


        /// <summary>
        /// 刷新进度条
        /// </summary>
        /// <param name="burnProgress"></param>
        private void OnUpBurnFlashProgressEvent(int burnProgress)
        {
            try
            {
                if (this == Form.ActiveForm)//是否操作当前窗体
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new GlobalDataInterface.BurnFlashProgressEventHandler(OnUpBurnFlashProgressEvent), burnProgress);
                    }
                    else
                    {
                        if (this.BurnprogressBar.Value != burnProgress)
                        {
                            this.BurnprogressBar.Value = burnProgress;
                            if(burnProgress==int.Parse(this.DataLenghthnumericUpDown.Text))
                            {
                                this.Startbutton.Enabled = true;
                                //DialogResult result = MessageBox.Show("Burning successfully!");
                                DialogResult result = MessageBox.Show(LanguageContainer.BootFlashBurnFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex]);
                                if (result == DialogResult.OK)
                                {
                                    this.Dispose();
                                    this.Close();
                                    //GlobalDataInterface.mainform.Dispose();
                                    //GlobalDataInterface.mainform.Close();
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("BootFlashBurnForm中函数OnUpBurnFlashProgressEvent出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("BootFlashBurnForm中函数OnUpBurnFlashProgressEvent出错" + ex);
#endif
            }
        }

        private void BootFlashBurnForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                GlobalDataInterface.UpBurnFlashProgressEvent -= new GlobalDataInterface.BurnFlashProgressEventHandler(OnUpBurnFlashProgressEvent); //Add by ChengSk - 20180830
            }
            catch (Exception ex)
            {
                Trace.WriteLine("BootFlashBurnForm中函数BootFlashBurnForm_FormClosing出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("BootFlashBurnForm中函数BootFlashBurnForm_FormClosing出错" + ex);
#endif
            }
        } 
    }
}
