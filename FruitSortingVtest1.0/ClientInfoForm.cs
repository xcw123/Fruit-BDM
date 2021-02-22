using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FruitSortingVtest1._0;
using Interface;
using FruitSortingVtest1.DB;
using System.Runtime.InteropServices;
using FruitSortingVtest1.ExCtrls;
using System.Drawing.Drawing2D;

namespace FruitSortingVtest1
{
    public partial class ClientInfoForm : Form
    {
        private MainForm mainForm;
        private string strCustomerName; //所有保存的客户名称
        private string[] customerName;
        private string strFarmName;     //所有保存的农场名称
        private string[] farmName;
        private string strFruitName;    //所有保存的水果品种
        private string[] fruitName;
        private DataBaseOperation databaseOperation = new DataBaseOperation();    //创建数据库操作对象

        #region  用于Combobox控件Item子项可删除功能

        [DllImport("user32")]
        private static extern int GetComboBoxInfo(IntPtr hwnd, out COMBOBOXINFO comboInfo);
        struct RECT
        {
            public int left, top, right, bottom;
        }
        struct COMBOBOXINFO
        {
            public int cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public int stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndItem;
            public IntPtr hwndList;
        }
        #endregion

        bool init1;
        bool init2;
        bool init3;
        IntPtr hwnd1;
        IntPtr hwnd2;
        IntPtr hwnd3;
        NativeCombo nativeCombo1 = new NativeCombo();
        NativeCombo nativeCombo2 = new NativeCombo();
        NativeCombo nativeCombo3 = new NativeCombo();
        //This is to store the Rectangle info of your Icons
        //Key:  the Item index
        //Value: the Rectangle of the Icon of the item (not the Rectangle of the item)
        Dictionary<int, Rectangle> dict1 = new Dictionary<int, Rectangle>();
        Dictionary<int, Rectangle> dict2 = new Dictionary<int, Rectangle>();
        Dictionary<int, Rectangle> dict3 = new Dictionary<int, Rectangle>();

        public class NativeCombo : NativeWindow
        {
            //this is custom MouseDown event to hook into later
            public event MouseEventHandler MouseDown;
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x201)//WM_LBUTTONDOWN = 0x201
                {
                    int x = m.LParam.ToInt32() & 0x00ff;
                    int y = m.LParam.ToInt32() >> 16;
                    if (MouseDown != null) MouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, x, y, 0));
                }
                base.WndProc(ref m);
            }
        }

        public ClientInfoForm()
        {
            InitializeComponent();

            CboClientName.HandleCreated += (s, e) =>
            {
                COMBOBOXINFO combo = new COMBOBOXINFO();
                combo.cbSize = Marshal.SizeOf(combo);
                GetComboBoxInfo(CboClientName.Handle, out combo);
                hwnd1 = combo.hwndList;
                init1 = false;
            };

            CboFarmName.HandleCreated += (s, e) =>
            {
                COMBOBOXINFO combo = new COMBOBOXINFO();
                combo.cbSize = Marshal.SizeOf(combo);
                GetComboBoxInfo(CboFarmName.Handle, out combo);
                hwnd2 = combo.hwndList;
                init2 = false;
            };

            CboFruitName.HandleCreated += (s, e) =>
            {
                COMBOBOXINFO combo = new COMBOBOXINFO();
                combo.cbSize = Marshal.SizeOf(combo);
                GetComboBoxInfo(CboFruitName.Handle, out combo);
                hwnd3 = combo.hwndList;
                init3 = false;
            };
        }

        private void ClientInfoForm_Load(object sender, EventArgs e)
        {
            mainForm = (MainForm)this.Owner;            
            //获取客户信息
            strCustomerName = FileOperate.ReadFile(2, mainForm.clientInfoFileName);
            strFarmName = FileOperate.ReadFile(3, mainForm.clientInfoFileName);
            strFruitName = FileOperate.ReadFile(4, mainForm.clientInfoFileName);
            //往ComboBox中添加选项
            if (strCustomerName != null)
            {
                CboClientName.Items.Clear();
                customerName = strCustomerName.Split('，');
                CboClientName.DrawMode = DrawMode.OwnerDrawFixed;
                for (int i = 0; i < customerName.Length; i++)
                {
                    //CboClientName.Items.Add(customerName[i]);
                    CboClientName.Items.Add(new comboItem(customerName[i].Trim(), pictureBox1.Image)); //Modify by ChengSk - 20190927
                }
            }
            if (strFarmName != null)
            {
                CboFarmName.Items.Clear();
                farmName = strFarmName.Split('，');
                CboFarmName.DrawMode = DrawMode.OwnerDrawFixed;
                for (int i = 0; i < farmName.Length; i++)
                {
                    //CboFarmName.Items.Add(farmName[i]);
                    CboFarmName.Items.Add(new comboItem(farmName[i].Trim(), pictureBox1.Image)); //Modify by ChengSk - 20190927
                }
            }
            if (strFruitName != null)
            {
                CboFruitName.Items.Clear();
                fruitName = strFruitName.Split('，');
                CboFruitName.DrawMode = DrawMode.OwnerDrawFixed;
                for (int i = 0; i < fruitName.Length; i++)
                {
                    //CboFruitName.Items.Add(fruitName[i]);
                    CboFruitName.Items.Add(new comboItem(fruitName[i].Trim(), pictureBox1.Image)); //Modify by ChengSk - 20190927
                }
            }

            string clientInfoContent = FileOperate.ReadFile(1, mainForm.clientInfoFileName); //Modify by ChengSk - 20180308
            if (clientInfoContent == null || clientInfoContent == "")
            {
                CboClientName.Text = "";
                CboFarmName.Text = "";
                CboFruitName.Text = "";
            }
            else
            {
                string[] clientInfoContentItem = clientInfoContent.Split('，');
                CboClientName.Text = clientInfoContentItem[0].Trim();
                CboFarmName.Text = clientInfoContentItem[1].Trim();
                CboFruitName.Text = clientInfoContentItem[2].Trim();
            }
            //CboClientName.Text = GlobalDataInterface.dataInterface.CustomerName.Trim();
            //CboFarmName.Text = GlobalDataInterface.dataInterface.FarmName.Trim();
            //CboFruitName.Text = GlobalDataInterface.dataInterface.FruitName.Trim();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string strClientNameTemp = CboClientName.Text.Trim();
                string strFarmNameTemp = CboFarmName.Text.Trim();
                string strFruitNameTemp = CboFruitName.Text.Trim();
                GlobalDataInterface.dataInterface.CustomerName = strClientNameTemp;
                GlobalDataInterface.dataInterface.FarmName = strFarmNameTemp;
                GlobalDataInterface.dataInterface.FruitName = strFruitNameTemp;
                string clientInfoContent = strClientNameTemp + "，" + strFarmNameTemp + "，" + strFruitNameTemp;
                StringBuilder sb = new StringBuilder();
                if (mainForm.clientInfoContent == null)
                {
                    sb.Append(clientInfoContent);
                    sb.Append("\r\n" + strClientNameTemp);
                    sb.Append("\r\n" + strFarmNameTemp);
                    sb.Append("\r\n" + strFruitNameTemp);
                    FileOperate.WriteFile(sb, mainForm.clientInfoFileName);
                    mainForm.clientInfoContent = clientInfoContent;
                }
                else
                {
                    FileOperate.EditFile(1, clientInfoContent, mainForm.clientInfoFileName);
                }
                //再次获取客户信息
                //strCustomerName = FileOperate.ReadFile(2, mainForm.clientInfoFileName);
                strCustomerName = "";
                for (int i = 0; i < customerName.Count(); i++)
                    strCustomerName += customerName[i].Trim() + "，";
                if (strCustomerName.Length > 1)
                    strCustomerName = strCustomerName.Substring(0, strCustomerName.Length - 1);
                //strFarmName = FileOperate.ReadFile(3, mainForm.clientInfoFileName);
                strFarmName = "";
                for (int i=0; i < farmName.Count(); i++)
                    strFarmName += farmName[i].Trim()+ "，";
                if (strFarmName.Length > 1)
                    strFarmName = strFarmName.Substring(0, strFarmName.Length - 1);
                //strFruitName = FileOperate.ReadFile(4, mainForm.clientInfoFileName);
                strFruitName = "";
                for (int i=0; i < fruitName.Count(); i++)
                    strFruitName += fruitName[i].Trim()+ "，";
                if (strFruitName.Length > 1)
                    strFruitName = strFruitName.Substring(0, strFruitName.Length - 1);
                //MessageBox.Show("strCustomerName=" + strCustomerName + " * strClientNameTemp=" + strClientNameTemp);
                FileOperate.EditFile(2, FunctionInterface.CombineString(strCustomerName, strClientNameTemp), mainForm.clientInfoFileName);
                FileOperate.EditFile(3, FunctionInterface.CombineString(strFarmName, strFarmNameTemp), mainForm.clientInfoFileName);
                FileOperate.EditFile(4, FunctionInterface.CombineString(strFruitName, strFruitNameTemp), mainForm.clientInfoFileName);
                mainForm.UpdateClientInfoState();
                //判断当前数据库是否为空
                //if (BusinessFacade.GetFruitTopCustomerID().Tables[0].Rows.Count == 0)
                if (databaseOperation.GetFruitTopCustomerID().Tables[0].Rows.Count == 0) 
                {
                    this.Close();
                    return;
                }
                //加工过程中：修改当前加工条目的客户信息
                //int newCustomerID = Convert.ToInt32(BusinessFacade.GetFruitTopCustomerID().Tables[0].Rows[0]["CustomerID"].ToString()); //获取最新客户信息的ID号
                int newCustomerID = Convert.ToInt32(databaseOperation.GetFruitTopCustomerID().Tables[0].Rows[0]["CustomerID"].ToString()); //获取最新客户信息的ID号
                DataSet dst1;                                              //统计信息时获取tb_FruitInfo
                //dst1 = BusinessFacade.GetFruitByCustomerID(newCustomerID); //获取tb_FruitInfo
                dst1 = databaseOperation.GetFruitByCustomerID(newCustomerID); //获取tb_FruitInfo
                if (dst1.Tables[0].Rows[0]["CompletedState"].ToString().Equals("0")) 
                {
                    //BusinessFacade.UpdateFruitCustomerInfo(newCustomerID, strClientNameTemp, strFarmNameTemp, strFruitNameTemp);
                    databaseOperation.UpdateFruitCustomerInfo(newCustomerID, strClientNameTemp, strFarmNameTemp, strFruitNameTemp);
                }
                this.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
            
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CboClientName_DrawItem(object sender, DrawItemEventArgs e)
        {
            if ((e.State & DrawItemState.Selected) != 0)//鼠标选中在这个项上
            {
                //渐变画刷
                LinearGradientBrush brush = new LinearGradientBrush(e.Bounds, Color.FromArgb(255, 251, 237),
                                                 Color.FromArgb(255, 236, 181), LinearGradientMode.Vertical);
                //填充区域
                Rectangle borderRect = new Rectangle(0, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 2);
                e.Graphics.FillRectangle(brush, borderRect);
                //画边框
                Pen pen = new Pen(Color.FromArgb(229, 195, 101));
                e.Graphics.DrawRectangle(pen, borderRect);
            }
            else
            {
                SolidBrush brush = new SolidBrush(Color.FromArgb(217, 223, 230));
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            //获得项图片,绘制图片
            comboItem item = (comboItem)CboClientName.Items[e.Index];
            Image img = item.Img;

            //图片绘制的区域
            Rectangle imgRect = new Rectangle(e.Bounds.Width - 18, e.Bounds.Y, 15, 15);
            dict1[e.Index] = imgRect;
            if (img != null && (e.State & DrawItemState.Selected) != 0)
            {
                e.Graphics.DrawImage(img, imgRect);
            }
            Rectangle textRect =
                new Rectangle(10, imgRect.Y, e.Bounds.Width - imgRect.Width, e.Bounds.Height + 2);
            string itemText = CboClientName.Items[e.Index].ToString();
            itemText = itemText.Trim();   //Add by ChengSk - 20191024
            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(itemText, new Font("宋体", 12), Brushes.Black, textRect, strFormat);
        }

        private void CboClientName_DropDown(object sender, EventArgs e)
        {
            if (!init1)
            {
                //Register the MouseDown event handler <--- THIS is WHAT you want.
                nativeCombo1.MouseDown += comboListMouseDown1;
                nativeCombo1.AssignHandle(hwnd1);
                init1 = true;
            }
        }

        //This is the MouseDown event handler to handle the clicked icon
        private void comboListMouseDown1(object sender, MouseEventArgs e)
        {
            foreach (var kv in dict1)
            {
                if (kv.Value.Contains(e.Location))
                {
                    //Show the item index whose the corresponding icon was held down
                    //MessageBox.Show(kv.Key.ToString());
                    if(kv.Key < customerName.Count())
                    {
                        List<string> tempCustomerName = new List<string>();
                        for(int i=0; i< customerName.Count(); i++)
                        {
                            if (i != kv.Key)
                                tempCustomerName.Add(customerName[i].Trim());
                        }
                        customerName = tempCustomerName.ToArray();

                        CboClientName.Items.Clear();
                        for (int i = 0; i < customerName.Length; i++)
                        {
                            CboClientName.Items.Add(new comboItem(customerName[i].Trim(), pictureBox1.Image));
                        }
                    }
                    return;
                }
            }
        }

        private void CboFarmName_DrawItem(object sender, DrawItemEventArgs e)
        {
            if ((e.State & DrawItemState.Selected) != 0)//鼠标选中在这个项上
            {
                //渐变画刷
                LinearGradientBrush brush = new LinearGradientBrush(e.Bounds, Color.FromArgb(255, 251, 237),
                                                 Color.FromArgb(255, 236, 181), LinearGradientMode.Vertical);
                //填充区域
                Rectangle borderRect = new Rectangle(0, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 2);
                e.Graphics.FillRectangle(brush, borderRect);
                //画边框
                Pen pen = new Pen(Color.FromArgb(229, 195, 101));
                e.Graphics.DrawRectangle(pen, borderRect);
            }
            else
            {
                SolidBrush brush = new SolidBrush(Color.FromArgb(217, 223, 230));
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            //获得项图片,绘制图片
            comboItem item = (comboItem)CboFarmName.Items[e.Index];
            Image img = item.Img;

            //图片绘制的区域
            Rectangle imgRect = new Rectangle(e.Bounds.Width - 18, e.Bounds.Y, 15, 15);
            dict2[e.Index] = imgRect;
            if (img != null && (e.State & DrawItemState.Selected) != 0)
            {
                e.Graphics.DrawImage(img, imgRect);
            }
            Rectangle textRect =
                new Rectangle(10, imgRect.Y, e.Bounds.Width - imgRect.Width, e.Bounds.Height + 2);
            string itemText = CboFarmName.Items[e.Index].ToString();
            itemText = itemText.Trim();
            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(itemText, new Font("宋体", 12), Brushes.Black, textRect, strFormat);
        }

        private void CboFarmName_DropDown(object sender, EventArgs e)
        {
            if (!init2)
            {
                //Register the MouseDown event handler <--- THIS is WHAT you want.
                nativeCombo2.MouseDown += comboListMouseDown2;
                nativeCombo2.AssignHandle(hwnd2);
                init2 = true;
            }
        }

        //This is the MouseDown event handler to handle the clicked icon
        private void comboListMouseDown2(object sender, MouseEventArgs e)
        {
            foreach (var kv in dict2)
            {
                if (kv.Value.Contains(e.Location))
                {
                    //Show the item index whose the corresponding icon was held down
                    //MessageBox.Show(kv.Key.ToString());

                    if (kv.Key < farmName.Count())
                    {
                        List<string> tempCboFarmName = new List<string>();
                        for (int i = 0; i < farmName.Count(); i++)
                        {
                            if (i != kv.Key)
                                tempCboFarmName.Add(farmName[i].Trim());
                        }
                        farmName = tempCboFarmName.ToArray();

                        CboFarmName.Items.Clear();
                        for (int i = 0; i < farmName.Length; i++)
                        {
                            CboFarmName.Items.Add(new comboItem(farmName[i].Trim(), pictureBox1.Image));
                        }
                    }
                    return;
                }
            }
        }

        private void CboFruitName_DrawItem(object sender, DrawItemEventArgs e)
        {
            if ((e.State & DrawItemState.Selected) != 0)//鼠标选中在这个项上
            {
                //渐变画刷
                LinearGradientBrush brush = new LinearGradientBrush(e.Bounds, Color.FromArgb(255, 251, 237),
                                                 Color.FromArgb(255, 236, 181), LinearGradientMode.Vertical);
                //填充区域
                Rectangle borderRect = new Rectangle(0, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 2);
                e.Graphics.FillRectangle(brush, borderRect);
                //画边框
                Pen pen = new Pen(Color.FromArgb(229, 195, 101));
                e.Graphics.DrawRectangle(pen, borderRect);
            }
            else
            {
                SolidBrush brush = new SolidBrush(Color.FromArgb(217, 223, 230));
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            //获得项图片,绘制图片
            comboItem item = (comboItem)CboFruitName.Items[e.Index];
            Image img = item.Img;

            //图片绘制的区域
            Rectangle imgRect = new Rectangle(e.Bounds.Width - 18, e.Bounds.Y, 15, 15);
            dict3[e.Index] = imgRect;
            if (img != null && (e.State & DrawItemState.Selected) != 0)
            {
                e.Graphics.DrawImage(img, imgRect);
            }
            Rectangle textRect =
                new Rectangle(10, imgRect.Y, e.Bounds.Width - imgRect.Width, e.Bounds.Height + 2);
            string itemText = CboFruitName.Items[e.Index].ToString();
            itemText = itemText.Trim();
            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(itemText, new Font("宋体", 12), Brushes.Black, textRect, strFormat);
        }

        private void CboFruitName_DropDown(object sender, EventArgs e)
        {
            if (!init3)
            {
                //Register the MouseDown event handler <--- THIS is WHAT you want.
                nativeCombo3.MouseDown += comboListMouseDown3;
                nativeCombo3.AssignHandle(hwnd3);
                init3 = true;
            }
        }

        //This is the MouseDown event handler to handle the clicked icon
        private void comboListMouseDown3(object sender, MouseEventArgs e)
        {
            foreach (var kv in dict3)
            {
                if (kv.Value.Contains(e.Location))
                {
                    //Show the item index whose the corresponding icon was held down
                    //MessageBox.Show(kv.Key.ToString());

                    if (kv.Key < fruitName.Count())
                    {
                        List<string> tempCboFruitName = new List<string>();
                        for (int i = 0; i < fruitName.Count(); i++)
                        {
                            if (i != kv.Key)
                                tempCboFruitName.Add(fruitName[i].Trim());
                        }
                        fruitName = tempCboFruitName.ToArray();

                        CboFruitName.Items.Clear();
                        for (int i = 0; i < fruitName.Length; i++)
                        {
                            CboFruitName.Items.Add(new comboItem(fruitName[i].Trim(), pictureBox1.Image));
                        }
                    }
                    return;
                }
            }
        }
    }
}
