using FruitSortingVtest1.DB;
using Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace FruitSortingVtest1
{
    public partial class SeparationLogForm : Form
    {
        private int DisplayChartHeight = 60;  //单个Chart图的高度
        private int DisplayDateTimeAddPanelWidth = 140;  //累计计时面板宽度
        private int TotalMinutes = 0; //总的累计分钟数
        private int[] TodayMinutes;   //当天累计分钟数
        private DataBaseOperation databaseOperation = new DataBaseOperation();    //创建数据库操作对象
        private ResourceManager m_resourceManager = new ResourceManager(typeof(SeparationLogForm));//创建SeparationLogForm资源管理

        public SeparationLogForm()
        {
            InitializeComponent();
        }

        private void SeparationLogForm_Load(object sender, EventArgs e)
        {
            this.Displaypanel.Width = this.splitContainer1.Panel2.Width - 20;
            this.Displaypanel.Height = this.splitContainer1.Panel2.Height;
            //DrawTop20RunningTimeChart(); //Add by xcw - 20191031
            DrawSeparationRunningTimeChart("", ""); //自动加载最新20条数据 Add by ChengSk - 20191104
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DateTime dt1 = DateTime.Parse(this.SelectdateTimePicker1.Text);
            DateTime dt2 = DateTime.Parse(this.SelectdateTimePicker2.Text);
            TimeSpan ts3 = dt1.Subtract(dt2).Duration();
            if (ts3.Days > 31)
            {
                //MessageBox.Show("30001103 The time interval can not exceed 31 days!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("30001103 " + LanguageContainer.SeparationLogFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.SeparationLogFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex],
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.Displaypanel.Controls.Clear();
            DrawSeparationRunningTimeChart(this.SelectdateTimePicker1.Text, this.SelectdateTimePicker2.Text);

            //MessageBox.Show(this.SelectdateTimePicker.Text);
            //bool bFlags = false;
            //for(int i=1; i<=28; i++)
            //{
            //    bFlags = databaseOperation.InsertRunningTimeInfo("2018-02-" + i.ToString(), "15:20:22", "18:10:44");
            //    bFlags = databaseOperation.InsertSeparationEfficiencyInfo("0.80", "2018-02-" + i.ToString(), "15:30:55");
            //}
            //MessageBox.Show(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            this.Displaypanel.Top = -vScrollBar1.Value;
        }

        /// <summary>
        /// 绘制分选机运行时间前20条信息Chart图
        /// </summary>
        private void DrawTop20RunningTimeChart()
        {
            try
            {
                //string strMinDate = strCurrentMonth + "-01";  //当月最小日期
                //string strMaxDate = strCurrentMonth + "-31";  //当月最大日期
                string strSQL = "select top 20 RunningDate from tb_RunningTimeInfo  order by ID desc "; //Add by xcw - 20191031
                DataSet dstDays = databaseOperation.SelectData(strSQL);
                if (dstDays.Tables[0].Rows.Count == 0)//当月无数据，返回
                    return;

                // 加1：该区域用于绘制显示“累计结果”
                if (DisplayChartHeight * (dstDays.Tables[0].Rows.Count + 1) < this.splitContainer1.Panel2.Height)
                {
                    this.vScrollBar1.Visible = false;
                    this.Displaypanel.Width = this.splitContainer1.Panel2.Width;
                    this.Displaypanel.Height = this.splitContainer1.Panel2.Height;
                }
                else
                {
                    this.vScrollBar1.Visible = true;
                    this.vScrollBar1.Maximum = DisplayChartHeight * dstDays.Tables[0].Rows.Count - this.splitContainer1.Panel2.Height;
                    this.Displaypanel.Width = this.splitContainer1.Panel2.Width - 20;
                    this.Displaypanel.Height = DisplayChartHeight * dstDays.Tables[0].Rows.Count;
                }

                TotalMinutes = 0;
                TodayMinutes = new int[dstDays.Tables[0].Rows.Count];
                for (int i = 0; i < dstDays.Tables[0].Rows.Count; i++)
                {
                    Chart chart = new Chart();
                    chart.Name = dstDays.Tables[0].Rows[i]["RunningDate"].ToString();
                    chart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
                    chart.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(130)))), ((int)(((byte)(111)))))};

                    ChartArea chartArea = new ChartArea();
                    chartArea.AxisX.Enabled = AxisEnabled.True;
                    chartArea.AxisX.MajorGrid.Enabled = false;
                    chartArea.AxisX.ScaleBreakStyle.Enabled = true;
                    chartArea.AxisX.Interval = 1D;
                    chartArea.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Hours;
                    chartArea.AxisX2.Enabled = AxisEnabled.False;
                    chartArea.AxisX2.MajorGrid.Enabled = false;
                    chartArea.AxisY.Enabled = AxisEnabled.False;
                    chartArea.AxisY.MajorGrid.Enabled = false;
                    chartArea.AxisY.LabelStyle.Format = "0%";
                    chartArea.AxisY.Maximum = 1D;
                    chartArea.AxisY2.Enabled = AxisEnabled.False;
                    chartArea.AxisY2.MajorGrid.Enabled = false;
                    chartArea.Name = "ChartArea";
                    chart.ChartAreas.Add(chartArea);

                    Legend legend = new Legend();
                    legend.Name = "Legend";
                    legend.Docking = Docking.Top;
                    chart.Legends.Add(legend);
                    chart.Size = new System.Drawing.Size(this.Displaypanel.Width - DisplayDateTimeAddPanelWidth, DisplayChartHeight - 5);
                    chart.Location = new System.Drawing.Point(0, i * DisplayChartHeight + 1);

                    Series series = new Series();
                    series.ChartArea = "ChartArea";
                    series.ChartType = SeriesChartType.Area;
                    series.Legend = "Legend";
                    series.Name = dstDays.Tables[0].Rows[i]["RunningDate"].ToString();
                    series.XValueType = ChartValueType.Time;
                    series.YValueType = ChartValueType.Int32;

                    chart.Series.Add(series);
                    chart.Text = "chart_" + dstDays.Tables[0].Rows[i]["RunningDate"].ToString();
                    chart.DoubleClick += new System.EventHandler(chart_Click);
                    this.Displaypanel.Controls.Add(chart);

                    chart.ChartAreas[0].AxisX.Minimum = DateTime.Parse("00:00:00").ToOADate();
                    chart.ChartAreas[0].AxisX.Maximum = DateTime.Parse("23:59:59").ToOADate();

                    DataSet dstChildRunningTime = databaseOperation.GetRunningTimeInfo(dstDays.Tables[0].Rows[i]["RunningDate"].ToString());
                    for (int j = 0; j < dstChildRunningTime.Tables[0].Rows.Count; j++)
                    {
                        double startTime = DateTime.Parse(dstChildRunningTime.Tables[0].Rows[j]["StartTime"].ToString()).ToOADate();
                        double stopTime = DateTime.Parse(dstChildRunningTime.Tables[0].Rows[j]["StopTime"].ToString()).ToOADate();
                        if (startTime == stopTime)
                            continue;
                        int k = 1;
                        double currentTime = 0;
                        chart.Series[0].Points.AddXY(startTime, 0);  //开始时间赋值0（一定要赋值）
                        while (currentTime < stopTime)
                        {
                            currentTime = DateTime.Parse(dstChildRunningTime.Tables[0].Rows[j]["StartTime"].ToString()).AddSeconds(k * 60).ToOADate();
                            chart.Series[0].Points.AddXY(currentTime, 1);
                            k++;
                        }
                        chart.Series[0].Points.AddXY(stopTime, 0);   //结束时间赋值0（一定要赋值）

                        //累计计时
                        //string[] dates = dstChildRunningTime.Tables[0].Rows[j]["RunningDate"].ToString().Split('-');
                        //string[] time1 = dstChildRunningTime.Tables[0].Rows[j]["StartTime"].ToString().Split(':');
                        //string[] time2 = dstChildRunningTime.Tables[0].Rows[j]["StopTime"].ToString().Split(':');
                        DateTime dt1 = DateTime.Parse(dstChildRunningTime.Tables[0].Rows[j]["StartTime"].ToString());
                        DateTime dt2 = DateTime.Parse(dstChildRunningTime.Tables[0].Rows[j]["StopTime"].ToString());
                        TimeSpan ts3 = dt1.Subtract(dt2).Duration();
                        TodayMinutes[i] += ts3.Hours * 60 + ts3.Minutes;
                    }
                    TotalMinutes += TodayMinutes[i];

                    Label label = new Label();  //当天累计分钟数
                    label.Text = m_resourceManager.GetString("labelAdd.Text") + " " + TodayMinutes[i].ToString() + " " + m_resourceManager.GetString("labelMinute.Text");
                    label.Location = new System.Drawing.Point(this.Displaypanel.Width + 5 - DisplayDateTimeAddPanelWidth, i * DisplayChartHeight + DisplayChartHeight / 2 + 2);
                    this.Displaypanel.Controls.Add(label);
                }

                Label sumlabel = new Label();   //总的累计分钟数
                //sumlabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                sumlabel.Text = m_resourceManager.GetString("labelTotal.Text") + " " + TotalMinutes.ToString() + " " + m_resourceManager.GetString("labelMinute.Text");
                sumlabel.Location = new System.Drawing.Point(this.Displaypanel.Width + 5 - DisplayDateTimeAddPanelWidth, dstDays.Tables[0].Rows.Count * DisplayChartHeight + DisplayChartHeight / 2 + 2);
                this.Displaypanel.Controls.Add(sumlabel);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("DrawSeparationRunningTimeChart: " + ex.ToString());
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DrawSeparationRunningTimeChart: " + ex.ToString());
#endif
            }
        }

        /// <summary>
        /// 绘制分选机运行时间Chart图
        /// </summary>
        /// <param name="strCurrentMonth">当前月份</param>
        //private void DrawSeparationRunningTimeChart(string strCurrentMonth)
        private void DrawSeparationRunningTimeChart(string strTimePicker1, string strTimePicker2)
        {
            try
            {
                //string strMinDate = strCurrentMonth + "-01";  //当月最小日期
                //string strMaxDate = strCurrentMonth + "-31";  //当月最大日期
                string strMinDate = strTimePicker1;
                string strMaxDate = strTimePicker2;
                
                string strSQL = "";

                if (strMinDate == "" || strMaxDate == "")  //Modify by ChengSk - 20191104
                {
                    strSQL = "select distinct top 10 RunningDate from tb_RunningTimeInfo order by RunningDate desc";  //取最近10天的记录
                }
                else
                {
                    strSQL = "select distinct RunningDate from tb_RunningTimeInfo where RunningDate >= '" + strMinDate + "' and RunningDate <= '" + strMaxDate + "' order by RunningDate asc";
                }

                DataSet dstDays = databaseOperation.SelectData(strSQL);
                if (dstDays.Tables[0].Rows.Count == 0)//当月无数据，返回
                    return;

                // 加1：该区域用于绘制显示“累计结果”
                if (DisplayChartHeight * (dstDays.Tables[0].Rows.Count + 1) < this.splitContainer1.Panel2.Height)
                {
                    this.vScrollBar1.Visible = false;
                    this.Displaypanel.Width = this.splitContainer1.Panel2.Width;
                    this.Displaypanel.Height = this.splitContainer1.Panel2.Height;
                }
                else
                {
                    this.vScrollBar1.Visible = true;
                    this.vScrollBar1.Maximum = DisplayChartHeight * dstDays.Tables[0].Rows.Count - this.splitContainer1.Panel2.Height;
                    this.Displaypanel.Width = this.splitContainer1.Panel2.Width - 20;
                    this.Displaypanel.Height = DisplayChartHeight * dstDays.Tables[0].Rows.Count;
                }

                TotalMinutes = 0;
                TodayMinutes = new int[dstDays.Tables[0].Rows.Count];
                for (int i = 0; i < dstDays.Tables[0].Rows.Count; i++)
                {
                    Chart chart = new Chart();
                    chart.Name = dstDays.Tables[0].Rows[i]["RunningDate"].ToString();
                    chart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
                    chart.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(130)))), ((int)(((byte)(111)))))};

                    ChartArea chartArea = new ChartArea();
                    chartArea.AxisX.Enabled = AxisEnabled.True;
                    chartArea.AxisX.MajorGrid.Enabled = false;
                    chartArea.AxisX.ScaleBreakStyle.Enabled = true;
                    chartArea.AxisX.Interval = 1D;
                    chartArea.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Hours;
                    chartArea.AxisX2.Enabled = AxisEnabled.False;
                    chartArea.AxisX2.MajorGrid.Enabled = false;
                    chartArea.AxisY.Enabled = AxisEnabled.False;
                    chartArea.AxisY.MajorGrid.Enabled = false;
                    chartArea.AxisY.LabelStyle.Format = "0%";
                    chartArea.AxisY.Maximum = 1D;
                    chartArea.AxisY2.Enabled = AxisEnabled.False;
                    chartArea.AxisY2.MajorGrid.Enabled = false;
                    chartArea.Name = "ChartArea";
                    chart.ChartAreas.Add(chartArea);

                    Legend legend = new Legend();
                    legend.Name = "Legend";
                    legend.Docking = Docking.Top;
                    chart.Legends.Add(legend);
                    chart.Size = new System.Drawing.Size(this.Displaypanel.Width - DisplayDateTimeAddPanelWidth, DisplayChartHeight - 5);
                    chart.Location = new System.Drawing.Point(0, i * DisplayChartHeight + 1);

                    Series series = new Series();
                    series.ChartArea = "ChartArea";
                    series.ChartType = SeriesChartType.Area;
                    series.Legend = "Legend";
                    series.Name = dstDays.Tables[0].Rows[i]["RunningDate"].ToString();
                    series.XValueType = ChartValueType.Time;
                    series.YValueType = ChartValueType.Int32;

                    chart.Series.Add(series);
                    chart.Text = "chart_" + dstDays.Tables[0].Rows[i]["RunningDate"].ToString();
                    chart.DoubleClick += new System.EventHandler(chart_Click);
                    this.Displaypanel.Controls.Add(chart);

                    chart.ChartAreas[0].AxisX.Minimum = DateTime.Parse("00:00:00").ToOADate();
                    chart.ChartAreas[0].AxisX.Maximum = DateTime.Parse("23:59:59").ToOADate();

                    DataSet dstChildRunningTime = databaseOperation.GetRunningTimeInfo(dstDays.Tables[0].Rows[i]["RunningDate"].ToString());
                    for (int j = 0; j < dstChildRunningTime.Tables[0].Rows.Count; j++)
                    {
                        double startTime = DateTime.Parse(dstChildRunningTime.Tables[0].Rows[j]["StartTime"].ToString()).ToOADate();
                        double stopTime = DateTime.Parse(dstChildRunningTime.Tables[0].Rows[j]["StopTime"].ToString()).ToOADate();
                        if (startTime == stopTime)
                            continue;
                        int k = 1;
                        double currentTime = 0;
                        chart.Series[0].Points.AddXY(startTime, 0);  //开始时间赋值0（一定要赋值）
                        while (currentTime < stopTime)
                        {
                            currentTime = DateTime.Parse(dstChildRunningTime.Tables[0].Rows[j]["StartTime"].ToString()).AddSeconds(k * 60).ToOADate();
                            chart.Series[0].Points.AddXY(currentTime, 1);
                            k++;
                        }
                        chart.Series[0].Points.AddXY(stopTime, 0);   //结束时间赋值0（一定要赋值）

                        //累计计时
                        //string[] dates = dstChildRunningTime.Tables[0].Rows[j]["RunningDate"].ToString().Split('-');
                        //string[] time1 = dstChildRunningTime.Tables[0].Rows[j]["StartTime"].ToString().Split(':');
                        //string[] time2 = dstChildRunningTime.Tables[0].Rows[j]["StopTime"].ToString().Split(':');
                        DateTime dt1 = DateTime.Parse(dstChildRunningTime.Tables[0].Rows[j]["StartTime"].ToString());
                        DateTime dt2 = DateTime.Parse(dstChildRunningTime.Tables[0].Rows[j]["StopTime"].ToString());
                        TimeSpan ts3 = dt1.Subtract(dt2).Duration();
                        TodayMinutes[i] += ts3.Hours * 60 + ts3.Minutes;
                    }
                    TotalMinutes += TodayMinutes[i];

                    Label label = new Label();  //当天累计分钟数
                    label.Text = m_resourceManager.GetString("labelAdd.Text") + " " + TodayMinutes[i].ToString() + " " + m_resourceManager.GetString("labelMinute.Text");
                    label.Location = new System.Drawing.Point(this.Displaypanel.Width + 5 - DisplayDateTimeAddPanelWidth, i * DisplayChartHeight + DisplayChartHeight / 2 + 2);
                    this.Displaypanel.Controls.Add(label);
                }

                Label sumlabel = new Label();   //总的累计分钟数
                //sumlabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                sumlabel.Text = m_resourceManager.GetString("labelTotal.Text") + " " + TotalMinutes.ToString() + " " + m_resourceManager.GetString("labelMinute.Text");
                sumlabel.Location = new System.Drawing.Point(this.Displaypanel.Width + 5 - DisplayDateTimeAddPanelWidth, dstDays.Tables[0].Rows.Count * DisplayChartHeight + DisplayChartHeight / 2 + 2);
                this.Displaypanel.Controls.Add(sumlabel);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("DrawSeparationRunningTimeChart: " + ex.ToString());
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DrawSeparationRunningTimeChart: " + ex.ToString());
#endif
            }
        }

        private void chart_Click(object sender, EventArgs e)
        {
            Chart chart = (Chart)sender;
            SeparationEfficiencyChartForm separationEfficiencyChartForm = new SeparationEfficiencyChartForm(chart.Name);
            separationEfficiencyChartForm.Show();
        }

        private void SeparationLogForm_SizeChanged(object sender, EventArgs e)
        {
            this.Displaypanel.Controls.Clear();
            DrawSeparationRunningTimeChart(this.SelectdateTimePicker1.Text, this.SelectdateTimePicker2.Text);
        }
    }
}
