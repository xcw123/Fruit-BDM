using Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FruitSortingVtest1
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = Image.FromFile("Logo/Starting.png");  //动态加载 Add by ChengSk - 2017/08/22
            this.TransparencyKey = this.BackColor;
            this.InitialStatuslabel.Text = LanguageContainer.ProgramMessagebox9Text[GlobalDataInterface.selectLanguageIndex];
        }
    }
}
