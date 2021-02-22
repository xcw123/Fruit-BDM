using FruitSortingVtest1.DB;
using Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace FruitSortingVtest1
{
    public partial class SeparationEfficiencyChartForm : Form
    {
        private string currentDate; //当前显示日期
        private DataBaseOperation databaseOperation = new DataBaseOperation();    //创建数据库操作对象

        public SeparationEfficiencyChartForm(string strDate)
        {
            InitializeComponent();
            currentDate = strDate;
        }

        private void SeparationEfficiencyChartForm_Load(object sender, EventArgs e)
        {
            this.Chartpanel.Controls.Clear();
            DrawSeparationEfficiencyChart();
        }

        /// <summary>
        /// 绘制分选效率图
        /// </summary>
        private void DrawSeparationEfficiencyChart()
        {
            try
            {
                Chart chart = new Chart();
                chart.Name = currentDate;
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
                chartArea.AxisY.Enabled = AxisEnabled.True;
                chartArea.AxisY.MajorGrid.Enabled = false;
                chartArea.AxisY.LabelStyle.Format = "0%";
                chartArea.AxisY.Maximum = 1D;
                chartArea.AxisY2.Enabled = AxisEnabled.False;
                chartArea.AxisY2.MajorGrid.Enabled = false;
                chartArea.Name = "ChartArea";
                chart.ChartAreas.Add(chartArea);

                Legend legend = new Legend();
                legend.Name = "Legend";
                legend.Alignment = System.Drawing.StringAlignment.Center;
                legend.Docking = Docking.Bottom;
                chart.Legends.Add(legend);
                chart.Size = new System.Drawing.Size(this.Chartpanel.Width, this.Chartpanel.Height);
                chart.Location = new System.Drawing.Point(0, 0);

                Series series = new Series();
                series.ChartArea = "ChartArea";
                series.ChartType = SeriesChartType.Line;
                series.Legend = "Legend";
                series.Name = currentDate;
                series.XValueType = ChartValueType.Time;
                series.YValueType = ChartValueType.Double;

                chart.Series.Add(series);
                chart.Text = "chart_" + currentDate;
                this.Chartpanel.Controls.Add(chart);

                chart.ChartAreas[0].AxisX.Minimum = DateTime.Parse("00:00:00").ToOADate();
                chart.ChartAreas[0].AxisX.Maximum = DateTime.Parse("23:59:59").ToOADate();

                DataSet dst = databaseOperation.GetSeparationEfficiencyInfo(currentDate);
                if(dst.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {
                        chart.Series[0].Points.AddXY(DateTime.Parse(dst.Tables[0].Rows[i]["CurrentTime"].ToString()).ToOADate(),
                            double.Parse(dst.Tables[0].Rows[i]["EfficiencyValue"].ToString()));
                    }
                        
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("DrawSeparationEfficiencyChart: " + ex.ToString());
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DrawSeparationEfficiencyChart: " + ex.ToString());
#endif
            }
        }

        private void SeparationEfficiencyChartForm_SizeChanged(object sender, EventArgs e)
        {
            this.Chartpanel.Controls.Clear();
            DrawSeparationEfficiencyChart();
        }
    }
}
