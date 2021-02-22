using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FruitSortingVtest1._0
{
    public partial class MessageBoxForm : Form
    {
        public MessageBoxForm(string Text)
        {
            InitializeComponent();
            this.Textlabel.Text = Text;
        }

        private void MessageBoxForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
