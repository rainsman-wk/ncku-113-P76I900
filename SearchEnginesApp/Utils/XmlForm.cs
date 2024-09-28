using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SearchEnginesApp.ToolModel;

namespace SearchEnginesApp.Utils
{
    public class XmlForm : Form
    {
        private readonly ToolModel _toolModel;
        private RichTextBox rtbXmlContent;
        private Label lblTitle;
        private Label lblKeywordStatus;

        public XmlForm(ToolModel toolModel, string title, string content)
        {
            this.Text = title;
            this.Width = 400;
            this.Height = 500;

            _toolModel = toolModel;

            rtbXmlContent = new RichTextBox { Dock = DockStyle.Fill, Text = content };
            lblTitle = new Label { Text = $"Title: {title}", Dock = DockStyle.Top, AutoSize = true };
            lblKeywordStatus = new Label { Text = "Keyword Status: ", Dock = DockStyle.Top, AutoSize = true };
            _toolModel.SearchKeywordLoaded += OnKeywordChanged; 
            this.Controls.Add(rtbXmlContent);

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


        private void ResetHighlitKeywords()
        {
            rtbXmlContent.SelectAll();
            rtbXmlContent.SelectionColor = Color.Black;
            rtbXmlContent.SelectionBackColor = Color.White;
        }

        private void HighlightKeywords(KeywordArg keywordarg, Color color)
        {
            if (keywordarg == null) return;

            int startIndex = 0;
            rtbXmlContent.SelectAll();
            rtbXmlContent.SelectionBackColor = System.Drawing.Color.White;

            int count = 0;
            Regex regex;
            RegexOptions checkoption = (keywordarg.MatchCase) ? RegexOptions.None : RegexOptions.IgnoreCase;

            foreach (string keyword in keywordarg.Keywords)
            {
                switch (keywordarg.Mode)
                {
                    case SearchMode.Word:
                    case SearchMode.Phrase:
                        regex = new Regex(@"\b" + Regex.Escape(keyword) + @"\b", checkoption);
                        break;
                    case SearchMode.Others:
                    default:
                        // No limited Serch Mode
                        regex = new Regex(keyword, checkoption);
                        break;
                }

                MatchCollection matches;
                if (keywordarg.InSelection)
                {
                    int selectstart = rtbXmlContent.SelectionStart;
                    int selectLength = rtbXmlContent.SelectionLength;

                    matches = regex.Matches(rtbXmlContent.SelectedText);
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
                    matches = regex.Matches(rtbXmlContent.Text);
                    foreach (Match match in matches)
                    {
                        rtbXmlContent.Select(match.Index, match.Length);
                        rtbXmlContent.SelectionBackColor = color;
                        count++;
                    }
                }
            }

            if (count == 0)
            {
                UpdatePageSearchResultLabel(Color.Red, $"Keyword(s) not found in Page");
            }
            else
            {
                string result = $"Find Keyword(s) in Selected file" + Environment.NewLine;
                result += Environment.NewLine;
                result += $"Counts : {count}" + Environment.NewLine;
                UpdatePageSearchResultLabel(Color.Blue, $"Count:{count} matches in entire file");
            }
        }

        private void UpdatePageSearchResultLabel(Color color, String result)
        {
            // Deault Value : File Search Result: None
            lblKeywordStatus.Text = result;
            lblKeywordStatus.ForeColor = color;
        }
    }
}
