using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FruitSortingVtest1
{
    public partial class ProgressBoxForm : Form
    {
        string initPrompt = "";

        public ProgressBoxForm(string strPrompt)
        {
            InitializeComponent();
            initPrompt = strPrompt;
        }

        private void ProgressBoxForm_Load(object sender, EventArgs e)
        {
            this.lblPrompt.Text = initPrompt;
        }
    }
}
