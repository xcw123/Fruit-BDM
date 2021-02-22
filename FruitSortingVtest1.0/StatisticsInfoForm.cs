using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FruitSortingVtest1._0;
using FruitSortingVtest1.DB;
using Interface;

namespace FruitSortingVtest1
{
    public partial class StatisticsInfoForm : Form
    {

        public StatisticsInfoForm()
        {   
            InitializeComponent();
            GlobalDataInterface.UpdateDataInterfaceEvent += new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent);
        }

        private void OnUpdateDataInterfaceEvent(DataInterface dataInterface)
        {

        }

        private void StatisticsInfoForm_Load(object sender, EventArgs e)
        {
        
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {

        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StatisticsInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            GlobalDataInterface.UpdateDataInterfaceEvent -= new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent);  //Add by ChengSk - 20180830
        }       
    }
}
