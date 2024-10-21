using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace SearchEnginesApp.Utils
{
    public class ZipfChartForm : Form
    {
        private Chart ZipfChart;
        private CheckBox cbPortersAlgorithm;
        private CheckBox cbStopWord;
        private RichTextBox rtbWordInfo;
        private Button btnCurveReset;
        private Label lbWordInfo;
        private NumericUpDown numWordFreqMin;
        private NumericUpDown numWordFreqMax;

        private List<string> _tokens = new List<string>();
        private Size CurveSize = new Size(800, 500);


        public ZipfChartForm(Point formlocation, string title, List<string> tokens)
        {
            _tokens = tokens;


            // Initialize Form
            this.Text = title;
            this.Location = formlocation;
            this.Width = 1200;
            this.Height = 600;

            //  CheckBox PortersAlgorithm Configuration
            this.cbPortersAlgorithm = new CheckBox();
            this.cbPortersAlgorithm.Name = "cbPortersAlgorithm";
            this.cbPortersAlgorithm.Text = "Use Porters Algorithm";
            this.cbPortersAlgorithm.Size = new Size (200,20);
            this.cbPortersAlgorithm.Checked = false;
            this.cbPortersAlgorithm.Location = new Point(10, CurveSize.Height + 10);
            this.cbPortersAlgorithm.CheckedChanged += new System.EventHandler(this.ZipCurveSettingChanged);
            this.Controls.Add(this.cbPortersAlgorithm);

            //  CheckBox StopWord Configuration
            this.cbStopWord = new CheckBox();
            this.cbStopWord.Name = "cbStopWord";
            this.cbStopWord.Text = "Sort Stop Words";
            this.cbPortersAlgorithm.Size = new Size(200, 20);
            this.cbStopWord.Checked = false;
            this.cbStopWord.Location = new Point(10, cbPortersAlgorithm.Location.Y + cbPortersAlgorithm.Size.Height);
            this.cbStopWord.CheckedChanged += new System.EventHandler(this.ZipCurveSettingChanged);
            this.Controls.Add(this.cbStopWord);

            // label Word Information
            this.lbWordInfo = new Label();
            this.lbWordInfo.Name = "lbWordInfo";
            this.lbWordInfo.Text = "Word Information / Frequency(%) -> ";
            this.lbWordInfo.Location = new Point(CurveSize.Width + 20, 10);
            this.lbWordInfo.Size = new Size(200, 20);
            this.Controls.Add(this.lbWordInfo);

            // NumericUpDown Word Frequency Max Configuration
            this.numWordFreqMax = new NumericUpDown();
            this.numWordFreqMax.Name = "numWordFreqMax";
            this.numWordFreqMax.Location = new Point(lbWordInfo.Location.X + lbWordInfo.Size.Width, lbWordInfo.Location.Y - 5);
            this.numWordFreqMax.Size = new Size(50, 20);
            this.numWordFreqMax.Minimum = 0;
            this.numWordFreqMax.Maximum = 100;
            this.numWordFreqMax.DecimalPlaces = 1;
            this.numWordFreqMax.Increment = 0.5M;
            this.numWordFreqMax.Value = 10;
            this.numWordFreqMax.ValueChanged += new System.EventHandler(this.ZipCurveSettingChanged);
            this.Controls.Add(this.numWordFreqMax);


            // NumericUpDown Word Frequency Min Configuration
            this.numWordFreqMin = new NumericUpDown();
            this.numWordFreqMin.Name = "numWordFreqMin";
            this.numWordFreqMin.Location = new Point(numWordFreqMax.Location.X + numWordFreqMax.Size.Width, numWordFreqMax.Location.Y);
            this.numWordFreqMin.Size = new Size(50, 20);
            this.numWordFreqMin.Minimum = 0;
            this.numWordFreqMin.Maximum = 100;
            this.numWordFreqMin.DecimalPlaces = 2;
            this.numWordFreqMin.Increment = 0.01M;
            this.numWordFreqMin.Value = 0.15M;
            this.numWordFreqMin.ValueChanged += new System.EventHandler(this.ZipCurveSettingChanged);
            this.Controls.Add(this.numWordFreqMin);

            // Button Reset Curve Configuration
            this.btnCurveReset = new Button();
            this.btnCurveReset.Name = "btnCurveReset";
            this.btnCurveReset.Text = "Reset Curve";
            this.btnCurveReset.Location = new Point(cbPortersAlgorithm.Location.X + cbPortersAlgorithm.Size.Width + 20, cbPortersAlgorithm.Location.Y);
            this.btnCurveReset.Size = new Size(50, 20);
            this.btnCurveReset.Click += new System.EventHandler(this.btnCurveReset_Click);
            this.Controls.Add(this.btnCurveReset);

            // RichBox Infomration
            this.rtbWordInfo = new RichTextBox();
            this.rtbWordInfo.Name = "rtbWordInfo";
            this.rtbWordInfo.Location = new Point(CurveSize.Width + 20, lbWordInfo.Location.Y+ lbWordInfo.Size.Height);
            this.rtbWordInfo.Size = new Size(300, CurveSize.Height);
            this.rtbWordInfo.Text = "";
            this.rtbWordInfo.Font = new Font("Consolas", 8);
            this.rtbWordInfo.Modified = false;
            this.Controls.Add(this.rtbWordInfo);



            //Initialize Chart
            this.ZipfChart = new Chart();
            this.ZipfChart.Name = "ZipfChart";
            this.ZipfChart.Location = new System.Drawing.Point(10, 10);
            this.ZipfChart.Size = CurveSize;
            this.ZipfChart.Text = "Zip Chart";

            // Chart Conifguration 
            ChartArea chartArea = new ChartArea("ChartArea");
            this.ZipfChart.ChartAreas.Add(chartArea);
            //this.ZipfChart.ChartAreas[0].AxisX.Interval = 1;
            this.ZipfChart.ChartAreas[0].AxisX.Title = "Word";
            //this.ZipfChart.ChartAreas[0].AxisY.Interval = 1;
            this.ZipfChart.ChartAreas[0].AxisY.Title = "Frequency";

            // Series Configuration
            var allwords = KeywordExtractor.ExtractTokenToDict(_tokens, cbPortersAlgorithm.Checked,cbStopWord.Checked);
            UpdateCurve(allwords);

            // Lengends Configuration
            Legend legend = new Legend("ZipfLegend");
            this.ZipfChart.Legends.Add(legend);

            // Add Chart to Form
            this.Controls.Add(this.ZipfChart);

        }
        private void UpdateCurve(Dictionary<string, Tuple<int,double>> allWords)
        {
            // Clear Text 
            this.rtbWordInfo.Text= "";

            // Series Configuration
            Series series = new Series("ZifSeries");
            series.Name = CurveState();
            series.ChartType = SeriesChartType.Point;
            series.ChartArea = "ChartArea";
            series.Legend = "ZipfLegend";

            int wordCount = allWords.Count;
            foreach (var word in allWords)
            {
                DataPoint point = new DataPoint();
                int frequency = word.Value.Item1;
                double relative = word.Value.Item2 * 100;
                point.SetValueXY(word.Key, frequency);
                if((relative > (double)numWordFreqMin.Value) && (relative < (double)numWordFreqMax.Value))
                {
                    this.rtbWordInfo.AppendText(string.Format("{0,-30} {1,-5} {2,6:F2}%\n", word.Key, frequency, relative));
                    series.Points.Add(point);
                }
            }
            // Draw Chart on the view

            if(ZipfChart.Series.IsUniqueName(series.Name))
            {
                this.ZipfChart.Series.Add(series);
            }
            else
            {
                ZipfChart.Series[series.Name] = series;
            }

            this.ZipfChart.ChartAreas[0].AxisX.Interval = Math.Ceiling(allWords.Count * 0.05);
            this.ZipfChart.ChartAreas[0].AxisY.Interval = Math.Ceiling(allWords.Values.First().Item1 * 0.05);
        }
        private string CurveState()
        {
            string state = string.Empty;
            if (cbPortersAlgorithm.Checked)
            {
                state = "Porters Algorithm";
            }
            else
            {
                state = "Word Frequency";
            }

            if(cbStopWord.Checked)
            {
                state += " without StopWords";
            }
            else
            {
                state += " with All Words";
            }

            return state;
        }
        private void ZipCurveSettingChanged(object sender, EventArgs e)
        {
            var allwords = KeywordExtractor.ExtractTokenToDict(_tokens, cbPortersAlgorithm.Checked,cbStopWord.Checked);
            UpdateCurve(allwords);
        }
        private void btnCurveReset_Click(object sender, EventArgs e)
        {
            this.ZipfChart.Series.Clear();
            var allwords = KeywordExtractor.ExtractTokenToDict(_tokens, cbPortersAlgorithm.Checked, cbStopWord.Checked);
            UpdateCurve(allwords);
        }
    }


}
