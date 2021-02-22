using Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Interface.TIpcSystemSpecAbsDataFormat;

namespace FruitSortingVtest1._0
{
    public partial class CountDownForm : Form
    {
        private InnerQualityForm innerQualityForm;
        private List<SendCommand> _list_SendCommand;
        public List<SendCommand> list_SendCommand
        {
            get { return _list_SendCommand; }
            set { _list_SendCommand = value; }
        }
        private int i = 0;
        private int sumInterval = 0;
        private int index = 0;
        private int preInterval = 0;
        public CountDownForm()
        {
            InitializeComponent();
        }

        private void CountDownForm_Load(object sender, EventArgs e)
        {
            innerQualityForm = (InnerQualityForm)this.Owner;
            sumInterval = _list_SendCommand.Sum(p => p.Interval);
            this.lblCountDown.Text = sumInterval.ToString();
            this.CountDowTimer.Enabled = true;
        }

        private void CountDownForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.CountDowTimer.Enabled = false;
        }

        private void CountDowTimer_Tick(object sender, EventArgs e)
        {
            this.lblCountDown.Text = (sumInterval - i - 1).ToString();
            SendCommand sendCommand = this._list_SendCommand[index];
            if (preInterval == 0)
            {
                innerQualityForm.MessageDataSend(sendCommand.MsgType, sendCommand.MsgData);
            }
            preInterval++;
            if (preInterval == sendCommand.Interval)
            {
                preInterval = 0;
                index++;
            }
            i++;
            if (index == this._list_SendCommand.Count)
            {
                this.Close();
            }
        }
    }
}
