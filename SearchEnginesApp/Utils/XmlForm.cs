using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static SearchEnginesApp.ToolModel;
using static System.Net.Mime.MediaTypeNames;


namespace SearchEnginesApp.Utils
{
    public class XmlForm : Form
    {
        private readonly ToolModel _toolModel;
        private RichTextBox rtbXmlContent;
        private Label lblTitle;
        private Label lblSearchWordsStatus;
        private Label lblKeywordsStatus;
        private Label lblXmlInfo;
        private GroupBox gbXmlFileInfo;

        private int rtbHight = 240;
        private int gpXmlInfoX = 215;
        private int gpXmlInfoY = 180;

        public XmlForm(Point formLocation , ToolModel toolModel, string title, FileContent content, SearchWordArg keywordArg)
        {
            this.Text = $"SearchEngine - [FileName]: {title}.xml [PMID]: {content.Pmid}";
            this.Location = formLocation;
            this.Width = 640;
            this.Height = 520;
            this.AutoScroll = true;
            _toolModel = toolModel;

            string text = String.Empty;
            foreach (string txt in content.Abstract)
            {
                text += txt + " ";
            }
            // Content Confiugration
            Point lctRtbxmlContent = new Point(10, 50);
            Size sizeRtbXmlContent = new Size(Width - 40, Height - rtbHight);
            rtbXmlContent = new RichTextBox { Text = text, Location = lctRtbxmlContent, Size = sizeRtbXmlContent };
            this.Controls.Add(rtbXmlContent);

            // Label Confiugration
            Point lctTitle = new Point(10, 10);
            Size sizeTitle = new Size(40, Width -40 );
            lblTitle = new Label { Text = $"Title:" + Environment.NewLine + $"{content.Title} ", AutoSize = true, Location = lctTitle, AutoEllipsis = true };
            this.Controls.Add(lblTitle);

            // Search Word(s)
            Point lctSearchWordsStatus = new Point(10, Height - (gpXmlInfoY-30));
            Size sizeSearchWordsStatus = new Size(40, 10);
            lblSearchWordsStatus = new Label { Text = "Search Word(s) Status: ", AutoSize = true , Location = lctSearchWordsStatus, Size = sizeSearchWordsStatus };
            this.Controls.Add(lblSearchWordsStatus);

            // Keyword(s)
            Point lctKeywordsStatus = new Point(10, Height - gpXmlInfoY);
            Size sizeKeywordsStatus = new Size(40, 30);
            lblKeywordsStatus = new Label { Text = "Keyword(s) List: ", AutoSize = true, Location = lctSearchWordsStatus, Size = sizeSearchWordsStatus };
            this.Controls.Add(lblKeywordsStatus);

            // File Information
            Point lctgbXmlFileInfo = new Point(Width - gpXmlInfoX, Height - gpXmlInfoY);
            Size sizegbXmlFileInfo = new Size(180, 110);
            lblXmlInfo = new Label { Name = "lblXmlInfo", Text = "File Not Selected", AutoSize = true, Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(80, 20) };
            gbXmlFileInfo = new GroupBox { Name = "gbXmlFileInfo", Text = "Xml File Infomation", Location=lctgbXmlFileInfo, Size = sizegbXmlFileInfo };
            this.gbXmlFileInfo.Controls.Add(this.lblXmlInfo);
            this.Controls.Add(gbXmlFileInfo);

            //Show Zipf Chart
            ZipfChartForm zipfChartForm = new ZipfChartForm(new Point(this.Location.X, this.Location.Y + 520), title, content.Word);
            zipfChartForm.Show();


            // Form Event Handle
            this.Resize += new EventHandler(XmlForm_Resize);
            this.FormClosing += new FormClosingEventHandler(XmlForm_FormClosing);
            // Tool Model Event Hanle
            _toolModel.SearchKeywordLoaded += OnKeywordChanged;

            //Update File Information
            UpdateFileInformation(content);
            //Update File Keywords
            UpdateFileKeywords(content.GetKeywords(5,true));

            //Update First Highligh Information
            HighlightKeywords(keywordArg, Color.Yellow);
        }

        private void OnKeywordChanged(object sender, KeywordEventArgs e)
        {
            if (e != null)
            {
                // Reset Highlight State
                ResetHighlitKeywords();
                // HighlightKeywords
                HighlightKeywords(e.Keywordarg, Color.Yellow);
            }

        }
        private void XmlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Unsubscribe from the event
            _toolModel.SearchKeywordLoaded -= OnKeywordChanged;
        }

        private void XmlForm_Resize(object sender, EventArgs e)
        {
            // Adjust the size and location of the controls
            rtbXmlContent.Size = new Size(this.Width - 40, this.Height - rtbHight);
            lblSearchWordsStatus.Location = new Point(10, this.Height - (gpXmlInfoY-30));
            lblKeywordsStatus.Location = new Point(10, this.Height - gpXmlInfoY);
            gbXmlFileInfo.Location = new Point(Width- gpXmlInfoX, Height- gpXmlInfoY);

        }

        private void ResetHighlitKeywords()
        {
            if (String.IsNullOrEmpty(rtbXmlContent.Text)) { return; }
            rtbXmlContent.SelectAll();
            rtbXmlContent.SelectionColor = Color.Black;
            rtbXmlContent.SelectionBackColor = Color.White;
        }

        private void HighlightKeywords(SearchWordArg keywordarg, Color color)
        {
            if (keywordarg == null) return;
            if (String.IsNullOrEmpty(rtbXmlContent.Text)) return;

            rtbXmlContent.SelectAll();
            rtbXmlContent.SelectionBackColor = System.Drawing.Color.White;

            int count = 0;
            string pattern;
            Regex regex;
            RegexOptions checkoption = (keywordarg.MatchCase) ? RegexOptions.None : RegexOptions.IgnoreCase;

            foreach (string keyword in keywordarg.SearchWords)
            {
                switch (keywordarg.Mode)
                {
                    case SearchMode.Word:
                    case SearchMode.Phrase:
                        pattern = @"\b(" + string.Join("|", keywordarg.SearchWords.Select(Regex.Escape)) + @")\b";
                        regex = new Regex(pattern, checkoption);
                        break;
                    case SearchMode.Others:
                    default:
                        // No limited Serch Mode
                        regex = new Regex(keyword, checkoption);
                        break;
                }
                string text = rtbXmlContent.SelectedText;
                MatchCollection matches;
                if (keywordarg.InSelection)
                {
                    int selectstart = rtbXmlContent.SelectionStart;
                    int selectLength = rtbXmlContent.SelectionLength;

                    matches = regex.Matches(text);
                    foreach (Match match in matches)
                    {
                        rtbXmlContent.Select(match.Index + selectstart, match.Length);
                        rtbXmlContent.SelectionBackColor = color;
                        count++;
                    }
                    rtbXmlContent.Select(selectstart, selectLength);
                }
                else
                {
                    if ((keywordarg.Mode == SearchMode.Word) || (keywordarg.Mode == SearchMode.Phrase))
                    {
                        bool allKeywordsFound = true;
                        foreach (string kw in keywordarg.SearchWords)
                        {
                            regex = new Regex(@"\b" + Regex.Escape(kw) + @"\b", checkoption);
                            if (!regex.IsMatch(text))
                            {
                                allKeywordsFound = false;
                                break;
                            }

                        }
                        if (allKeywordsFound)
                        {
                            pattern = @"\b(" + string.Join("|", keywordarg.SearchWords.Select(Regex.Escape)) + @")\b";
                            regex = new Regex(pattern, checkoption);
                            matches = regex.Matches(text);
                            foreach (Match match in matches)
                            {
                                rtbXmlContent.Select(match.Index, match.Length);
                                rtbXmlContent.SelectionBackColor = color;
                                count++;
                            }
                        }
                    }
                    else
                    {
                        matches = regex.Matches(text);
                        foreach (Match match in matches)
                        {
                            rtbXmlContent.Select(match.Index, match.Length);
                            rtbXmlContent.SelectionBackColor = color;
                            count++;
                        }
                    }
                }
            }

            if (count == 0)
            {
                UpdatePageSearchResultLabel(Color.Red, $"SearchWord(s) not found in Page");
            }
            else
            {
                string result = $"Find SearchWord(s) in Selected file" + Environment.NewLine;
                result += Environment.NewLine;
                result += $"Counts : {count}" + Environment.NewLine;
                UpdatePageSearchResultLabel(Color.Blue, $"Count:{count} matches in entire file");
            }
        }

        private void UpdatePageSearchResultLabel(Color color, String result)
        {
            // Deault Value : File Search Result: None
            lblSearchWordsStatus.Text = result;
            lblSearchWordsStatus.ForeColor = color;
        }
        private void UpdateFileKeywords(List<string> keywords)
        {
            lblKeywordsStatus.Text = $"Top 5 Keywords in file:" +Environment.NewLine;
            lblKeywordsStatus.Text += string.Join(", ", keywords);
        }

        private void UpdateFileInformation(FileContent file)
        {
            lblXmlInfo.Text = String.Empty;
            lblXmlInfo.Text += $"Words : {file.WordsCount(),30}" + Environment.NewLine;
            lblXmlInfo.Text += $"Char(s) no spaces : {file.AbstractCountNoSpace(),11}" + Environment.NewLine;
            lblXmlInfo.Text += $"Char(s) with spaces : {file.AbstractCount(),9}" + Environment.NewLine;
            lblXmlInfo.Text += $"Paragraphs : {file.Abstract.Count(),22}" + Environment.NewLine;
            lblXmlInfo.Text += $"Lines : {file.SentenceCount(),30}" + Environment.NewLine;
            lblXmlInfo.Text += $"Non-Ascii Char(s) : {file.AbstractCountNonAsciiChar(),10}" + Environment.NewLine;
            lblXmlInfo.Text += $"Non-Ascii Word(s) : {file.AbstractCountNonAsciiWord(),9}" + Environment.NewLine;
            lblXmlInfo.ForeColor = Color.Blue;
        }

    }
}
