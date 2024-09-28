using SearchEnginesApp.Presenters;
using SearchEnginesApp.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static SearchEnginesApp.ToolModel;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace SearchEnginesApp.Views
{
    public partial class SearchEngineResultView : UserControl
    {
        private readonly SearchEngineResultPresenter _presenter;
        private int _openFormsCount = 0;
        private List<XmlForm> _openForms = new List<XmlForm>();

        public SearchEngineResultView(SearchEngineResultPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
        }

        #region View Feature
        public void UpdateFileList(List<string> Name, List<FileContent> filedata)
        {
            ResetResultViewPage();
            if (Name.Count == filedata.Count)
            {
                for (int i = 0; i < filedata.Count; i++)
                {
                    BooksDataGridView.Rows.Add(Name[i], filedata[i].ContentCount(), filedata[i].ContentCountNoSpace()
                                                      , filedata[i].WordsCount(), filedata[i].SentenceCount()
                                                      , filedata[i].NonAsciiChar(), filedata[i].NonAsciiWord());
                }
            }
        }

        public void LoadSelectContent(List<string> Name, List<FileContent> filedata)
        {
            int selectidx = BooksDataGridView.SelectedRows.Count;
            string selectfile = GetSelectFile(selectidx);
            string content = String.Empty;
            foreach (var file in Name)
            {
                if (file == selectfile)
                {
                    for (int i = 0; i < filedata[selectidx].Content.Count; i++)
                    {
                        content += filedata[selectidx].Content[i] + Environment.NewLine;
                    }
                    richTextBoxFileContent.Text = content;
                }
            }
        }

        public void UpdateFileSearchResult(List<string> Name, List<FileContent> filedata, KeywordArg keywordarg)
        {
            // Keyword Seraching in Files
            int count = 0;
            StringComparison checkoption = (keywordarg.MatchCase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            for (int i = 0; i < Name.Count; i++)
            {
                bool result = false;
                switch (keywordarg.Mode)
                {
                    case SearchMode.Word:
                        foreach (var word in filedata[i].Word)
                        {   // TODO : use word arrary, no fix index
                            if (word.Equals(keywordarg.Keywords[0], checkoption))
                            {
                                HighlightKeyword_FileRows(Name[i], Color.LightGreen);
                                result = true;
                            }
                        }
                        break;
                    case SearchMode.Phrase:
                        foreach (var phrase in filedata[i].Sentence)
                        {   // TODO : use phrase arrary, no fix index
                            if (phrase.Equals(keywordarg.Keywords[0], checkoption))
                            {
                                HighlightKeyword_FileRows(Name[i], Color.YellowGreen);
                                result = true;
                            }
                        }
                        break;
                    case SearchMode.Others:
                    default:
                        Regex regex = new Regex(keywordarg.Keywords[0], (keywordarg.MatchCase) ? RegexOptions.None : RegexOptions.IgnoreCase);
                        foreach (var others in filedata[i].Content)
                        {
                            MatchCollection matches = regex.Matches(others);
                            if(matches.Count>0)
                            {
                                HighlightKeyword_FileRows(Name[i], Color.LightSeaGreen);
                                result = true;
                            }
                          }
                        break;
                }
                if (result) { count++; }
            }
            if (count == 0)
            {
                UpdateFileSearchResultLabel(Color.Red, $"Keyword(s) not found in File");
            }
            else
            {
                UpdateFileSearchResultLabel(Color.Blue, $"Count:{count} matches in file List");
            }
        }

        public void UpdateContentSearchResult(KeywordArg keywordarg)
        {
            //Reset Highlight Keywords
            ResetHighlitKeywords(keywordarg.InSelection);
            // Keyword Seraching in Files
            HighlightKeywords_ContentPage(keywordarg, Color.Yellow);
        }

        #endregion View Feature
        public void UpdateLabelSearchResult(string text, Color color)
        {
            labelResult.Text = text;
            labelResult.ForeColor = color;
        }
        public void ResetResultViewPage()
        {
            // Reset BooksDataGridView View 
            foreach (DataGridViewRow row in BooksDataGridView.Rows)
            {
                row.DefaultCellStyle.BackColor = SystemColors.Window;
            }
            BooksDataGridView.Rows.Clear();

            //Reset Restult Label
            UpdateLabelSearchResult(String.Empty, SystemColors.WindowText);
        }

        public void HighlightKeyword_FileRows(string keyword, Color color)
        {
            foreach (DataGridViewRow row in BooksDataGridView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().Contains(keyword))
                    {
                        row.DefaultCellStyle.BackColor = color;
                        break;
                    }
                }
            }
        }
        public string GetSelectFile(int index)
        {
            string filename = string.Empty;
            try
            {
                int rowIdx = index;
                int columnIdx = 0;
                if (BooksDataGridView.Rows.Count > rowIdx && BooksDataGridView.Columns.Count > columnIdx)
                {
                    filename = BooksDataGridView.Rows[rowIdx].Cells[columnIdx].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Select File Error：{ex.Message}");
            }
            return filename;
        }

        private void ResetHighlitKeywords(bool Inselection)
        {
            int start = richTextBoxFileContent.SelectionStart;
            int length = richTextBoxFileContent.SelectionLength;

            //Reset HighLight Keywords
            richTextBoxFileContent.SelectAll();
            richTextBoxFileContent.SelectionColor = Color.Black;
            richTextBoxFileContent.SelectionBackColor = Color.White;
            if (Inselection)
            {
                richTextBoxFileContent.Select(start, length);
            }
        }
        private void HighlightKeywords_ContentPage(KeywordArg keywordarg, Color color)
        {
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
                    int selectstart = richTextBoxFileContent.SelectionStart;
                    int selectLength = richTextBoxFileContent.SelectionLength;

                    matches = regex.Matches(richTextBoxFileContent.SelectedText);
                    foreach (Match match in matches)
                    {
                        richTextBoxFileContent.Select(match.Index + selectstart, match.Length);
                        richTextBoxFileContent.SelectionBackColor = color;
                        count++;
                    }
                    richTextBoxFileContent.Select(selectstart, selectLength);
                }
                else
                {
                    matches = regex.Matches(richTextBoxFileContent.Text);
                    foreach (Match match in matches)
                    {
                        richTextBoxFileContent.Select(match.Index, match.Length);
                        richTextBoxFileContent.SelectionBackColor = color;
                        count++;
                    }
                }
            }

            if (count == 0)
            {
                UpdateLabelSearchResult(String.Empty, Color.Red);
                UpdatePageSearchResultLabel(Color.Red, $"Keyword(s) not found in Page");
            }
            else
            {
                string result = $"Find Keyword(s) in Selected file" + Environment.NewLine;
                result += Environment.NewLine;
                result += $"Counts : {count}" + Environment.NewLine;
                UpdateLabelSearchResult(result, Color.Blue);
                UpdatePageSearchResultLabel(Color.Blue, $"Count:{count} matches in entire file");

            }
        }

        private void UpdateFileSearchResultLabel(Color color, String result)
        {
            // Deault Value : File Search Result: None
            labelFileSearchResult.Text = result;
            labelFileSearchResult.ForeColor = color;
        }

        private void UpdatePageSearchResultLabel(Color color, String result)
        {
            // Deault Value : File Search Result: None
            labelPageSearchResult.Text = result;
            labelPageSearchResult.ForeColor = color;
        }

        private void BooksDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(BooksDataGridView.SelectedRows.Count>0)
            {
                int selectidx = BooksDataGridView.SelectedRows[0].Index;
                string selectfile = GetSelectFile(selectidx);
                string content = _presenter.GetFileContent(selectfile);
                richTextBoxFileContent.Text = content;
                ShowXmlInNewWindow(selectfile, content);
            }
        }

        public void ShowXmlInNewWindow(string title, string content)
        {
            var existingForm = _openForms.FirstOrDefault(f => f.Text == title);
            if (existingForm != null)
            {
                existingForm.Focus();
                return;
            }

            XmlForm xmlForm = new XmlForm(_presenter.GetToolModel(), title, content);
            xmlForm.StartPosition = FormStartPosition.Manual;
            xmlForm.Location = new System.Drawing.Point(400 * _openFormsCount++, 0);
            _openForms.Add(xmlForm);
            xmlForm.FormClosed += (sender, e) => _openForms.Remove(xmlForm);
            xmlForm.Show();
        }
    }
}
