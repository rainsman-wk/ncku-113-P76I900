using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SearchEnginesApp.Utils
{
    public class ZipfChartForm : Form
    {
        private Chart ZipfChart;

        public ZipfChartForm(Point formlocation, string title, Dictionary<string, (int count, List<int> indices)> wordData)
        {
            // Initialize Form
            this.Text = title;
            this.Location = formlocation;
            this.Width = 800;
            this.Height = 600;


            //Initialize Chart
            this.ZipfChart = new Chart();
            this.ZipfChart.Name = "ZipfChart";
            this.ZipfChart.Location = new System.Drawing.Point(10, 10);
            this.ZipfChart.Size = new System.Drawing.Size(500, 400);
            this.ZipfChart.Text = "Zip Chart";

            // Chart Conifguration 
            ChartArea chartArea = new ChartArea("ChartArea");
            this.ZipfChart.ChartAreas.Add(chartArea);
            this.ZipfChart.ChartAreas[0].AxisX.Interval = 1;
            this.ZipfChart.ChartAreas[0].AxisX.Title = "Word";
            this.ZipfChart.ChartAreas[0].AxisY.Interval = 1;
            this.ZipfChart.ChartAreas[0].AxisY.Title = "Frequency";

            // Series Configuration
            UpdateCurve(wordData);

            // Lengends Configuration
            Legend legend = new Legend("ZipfLegend");
            this.ZipfChart.Legends.Add(legend);

            // Add Chart to Form
            this.Controls.Add(this.ZipfChart);
        }
        private void UpdateCurve(Dictionary<string, (int count, List<int> indices)> wordData)
        {
            // Series Configuration
            Series series = new Series("ZifSeries");
            series.ChartType = SeriesChartType.Line;
            series.ChartArea = "ChartArea";
            series.Legend = "ZipfLegend";
            series.Name = "Series";

            var allWords = wordData.OrderByDescending(w => w.Value.count).ToList();
            int wordCount = allWords.Count;
            for (int i = 0; i < wordCount; i++)
            {
                DataPoint point = new DataPoint();
                point.SetValueXY(allWords[i].Key, allWords[i].Value.count);
                series.Points.Add(point);
            }
            // Draw Chart on the view
            this.ZipfChart.Series.Add(series);
        }

    }


}
