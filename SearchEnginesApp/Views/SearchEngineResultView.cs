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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
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
        private const int _keywordCountIndex = 1;

        public SearchEngineResultView(SearchEngineResultPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;

            BooksDataGridView.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "FileName",
                HeaderText = "File Name",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                Visible = true,
                MinimumWidth = 100,
            });
            BooksDataGridView.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "KeyWordResultCount",
                HeaderText = "Keyword(s)",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                Visible = false,
                Width = 30,
                MinimumWidth = 30,
            });
            BooksDataGridView.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "Pmid",
                HeaderText = "Pmid",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                Visible = true,
                MinimumWidth = 80,
            });
            BooksDataGridView.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "Journal",
                HeaderText = "Journal",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                Visible = true,
                MinimumWidth = 120,
            });
            BooksDataGridView.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "Title",
                HeaderText = "Title",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                Visible = true,
                MinimumWidth = 150,
            });
        }

        #region View Feature
        public void UpdateFileList(List<string> Name, List<FileContent> filedata)
        {
            ResetResultViewPage();
            if (Name.Count == filedata.Count)
            {
                for (int i = 0; i < filedata.Count; i++)
                {
                    BooksDataGridView.Rows.Add(Name[i], 0, filedata[i].Pmid, filedata[i].Journal, filedata[i].Title);
                }
                int idx = BooksDataGridView.Rows.Count - 1;
                BooksDataGridView.Rows[idx].Selected = true;
                BooksDataGridView.CurrentCell = BooksDataGridView.Rows[idx].Cells[0];
                KeywordSearchResultCount(false);
                UpdateFileInformation(Name[Name.Count - 1]);


                lblKeywordsTitle.Text = "File List Top 10 Keywords" + $" (Database : {filedata.Count+1}) files";

            }



        }

        public void UpdateFileSearchResult(List<string> Name, List<FileContent> filedata, SearchWordArg keywordarg)
        {
            // Reset Search Result
            UpdateLabelSearchResult(String.Empty, SystemColors.WindowText);
            // Keyword Seraching in Files
            List<int> count = new List<int>();
            int filecount = 0;
            string pattern;
            Regex regex;
            RegexOptions regexoption = (keywordarg.MatchCase) ? RegexOptions.None : RegexOptions.IgnoreCase;
            StringComparison checkoption = (keywordarg.MatchCase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            for (int idx = 0; idx < Name.Count; idx++)
            {
                // StartCounting Match
                count.Add(0);
                foreach (string keyword in keywordarg.SearchWords)
                {
                    switch (keywordarg.Mode)
                    {
                        case SearchMode.Word:
                            pattern = @"\b(" + string.Join("|", keywordarg.SearchWords.Select(Regex.Escape)) + @")\b";
                            regex = new Regex(pattern, regexoption);
                            break;
                        case SearchMode.Phrase:
                            pattern = @"\b(" + string.Join("|", keywordarg.SearchWords.Select(Regex.Escape)) + @")\b";
                            regex = new Regex(pattern, regexoption);
                            break;
                        case SearchMode.Others:
                        default:
                            // No limited Serch Mode
                            regex = new Regex(keyword, regexoption);
                            break;
                    }
                    MatchCollection matches;

                    foreach (string text in filedata[idx].Abstract)
                    {
                        if ((keywordarg.Mode == SearchMode.Word) || (keywordarg.Mode == SearchMode.Phrase))
                        {
                            bool allKeywordsFound = true;
                            foreach (string kw in keywordarg.SearchWords)
                            {
                                Regex kwRegex = new Regex(@"\b" + Regex.Escape(kw) + @"\b", regexoption);
                                if (!kwRegex.IsMatch(text))
                                {
                                    allKeywordsFound = false;
                                    break;
                                }
                            }
                            if (allKeywordsFound)
                            {
                                count[idx]++;
                            }
                        }
                        else
                        {
                            matches = regex.Matches(text);
                            foreach (Match match in matches)
                            {
                                count[idx]++;
                            }
                        }


                    }
                }

                FileRows_UpdateKeywordCount(Name[idx], count[idx]);
                if (count[idx] > 0)
                {
                    FileRows_HighlightKeyword(Name[idx], Color.LightSeaGreen);
                    _presenter.GetHashCode();
                    filecount++;
                }
            }
            // Enable Result Counts
            KeywordSearchResultCount(true);
            // Display Search Result
            if (count.Count == 0)
            {
                UpdateFileSearchResultLabel(Color.Red, $"Search Word(s) not found in File");
            }
            else
            {
                int totalcount = 0;
                foreach (var num in count)
                {
                    totalcount += num;
                }
                UpdateFileSearchResultLabel(Color.Blue, $"Count:{totalcount} matches in {filecount} file(s)");
            }
            UpdateFileInformation(Name[Name.Count - 1]);
        }

        #endregion View Feature
        private void KeywordSearchResultCount(bool Enabled)
        {
            BooksDataGridView.Columns[_keywordCountIndex].Visible = Enabled;
        }
        public void UpdateLabelSearchResult(string text, Color color)
        {
            lblXmlInfo.Text = text;
            lblXmlInfo.ForeColor = color;
        }
        private void ResetResultViewPage()
        {
            // Reset BooksDataGridView View 
            foreach (DataGridViewRow row in BooksDataGridView.Rows)
            {
                row.DefaultCellStyle.BackColor = SystemColors.Window;
            }
            BooksDataGridView.Rows.Clear();

            //Reset Restult Label
            UpdateLabelSearchResult(String.Empty, SystemColors.WindowText);

            lblKeywordsTitle.Visible = false;
            lblFileKeywords.Visible = false;
        }

        public void FileRows_UpdateKeywordCount(string keyword, int count)
        {
            foreach (DataGridViewRow row in BooksDataGridView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().Contains(keyword))
                    {
                        row.Cells[_keywordCountIndex].Value = count.ToString();
                        break;
                    }
                }
            }
        }
        public void FileRows_HighlightKeyword(string keyword, Color color)
        {
            foreach (DataGridViewRow row in BooksDataGridView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().Contains(keyword))
                    {
                        row.Cells[_keywordCountIndex].Style.BackColor = color;
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
                if (rowIdx < BooksDataGridView.Rows.Count)
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

        private void UpdateFileSearchResultLabel(Color color, String result)
        {
            // Deault Value : File Search Result: None
            labelFileSearchResult.Text = result;
            labelFileSearchResult.ForeColor = color;
        }
        private void UpdateFileInformation(string FileName)
        {
            FileContent file = _presenter.GetFileContent(FileName);
            lblXmlInfo.Text = "File Information" + Environment.NewLine;
            lblXmlInfo.Text += $"Words : {file.WordsCount(),30}" + Environment.NewLine;
            lblXmlInfo.Text += $"Char(s) no spaces : {file.AbstractCountNoSpace(),11}" + Environment.NewLine;
            lblXmlInfo.Text += $"Char(s) with spaces : {file.AbstractCount(),9}" + Environment.NewLine;
            lblXmlInfo.Text += $"Paragraphs : {file.Abstract.Count(),22}" + Environment.NewLine;
            lblXmlInfo.Text += $"Lines : {file.SentenceCount(),30}" + Environment.NewLine;
            lblXmlInfo.Text += $"Non-Ascii Char(s) : {file.AbstractCountNonAsciiChar(),10}" + Environment.NewLine;
            lblXmlInfo.Text += $"Non-Ascii Word(s) : {file.AbstractCountNonAsciiWord(),9}" + Environment.NewLine;
            lblXmlInfo.ForeColor = Color.Blue;
        }

        private void BooksDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {

            }
            else
            {
                int selectidx = e.RowIndex;
                string selectfile = GetSelectFile(selectidx);
                // Update Xml File Information
                UpdateFileInformation(selectfile);
                // New Windows for XML file
                if (e.ColumnIndex == 0)
                {
                    FileContent file = _presenter.GetFileContent(selectfile);
                    ShowXmlInNewWindow(selectfile, file);
                }
            }
        }

        public void ShowXmlInNewWindow(string title, FileContent file)
        {
            string text = $"SearchEngine - [FileName]: {title}.xml [PMID]: {file.Pmid}";
            var existingForm = _openForms.FirstOrDefault(f => f.Text == text);
            if (existingForm != null)
            {
                existingForm.Focus();
                return;
            }

            XmlForm xmlForm = new XmlForm(new Point(640 * ((_openFormsCount) % 3), 120), _presenter.GetToolModel(), title, file, _presenter.GetSearchWords());
            xmlForm.StartPosition = FormStartPosition.Manual;
            _openForms.Add(xmlForm);
            xmlForm.FormClosed += (sender, e) => _openForms.Remove(xmlForm);
            xmlForm.Show();
            _openFormsCount++;
        }
        public void UpdateFilesTopKeywords(List<string> keywords)
        {
            string text = String.Empty;
            Color textColor = new Color();
            if (keywords.Count > 0)
            {
                text = string.Join(", ", keywords);
                textColor = Color.ForestGreen;
            }
            else
            {
                text = "No Keywords found";
                textColor = Color.IndianRed;
            }
            lblKeywordsTitle.Visible = true;
            lblFileKeywords.Visible = true;
            lblFileKeywords.ForeColor = textColor;
            lblFileKeywords.Text = text;

        }

        private void lblFileKeywords_DoubleClick(object sender, EventArgs e)
        {
            Dictionary<string, (int count, List<int> indices)> keywordsdict = _presenter.GetKeywordsDict(20);
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in keywordsdict)
            {
                sb.AppendLine($"Keyword: [{kvp.Key}] :  Count: {kvp.Value.count}");
            }
            MessageBox.Show(sb.ToString(), "Keywords Top 20 List");
        }

        private void btnZipfDistribution_Click(object sender, EventArgs e)
        {
           _presenter.GetSearchBookTokens();
        }
    }
}

