﻿using SearchEnginesApp.Presenters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
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
        private readonly SearchEnginePresenter _presenter;
        public SearchEngineResultView(SearchEnginePresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
        }


        #region View Feature
        public void UpdateFileList(List<string> Name, List<FileContent> filedata)
        {
            ResetResultViewPage();
            

            if (Name.Count == filedata.Count )
            {
                for (int i = 0; i < filedata.Count; i++)
                {
                    BooksDataGridView.Rows.Add(Name[i], filedata[i].ContentCount(), filedata[i].ContentCountNoSpace()
                                                      , filedata[i].WordsCount(), filedata[i].SentenceCount()
                                                      , filedata[i].NonAsciiChar(), filedata[i].NonAsciiWord());
                }
            }
        }
        
        public void LoadSelectContent(List<string> Name, List<FileContent> filedata, KeywordArg keywordarg)
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
            HighlightKeywords(keywordarg, Color.Yellow);
        }
        
        
        public void UpdateSearchResult(List<string> Name, List<FileContent> filedata, KeywordArg keywordarg)
        {
            // Keyword Seraching in Files
            int selectidx = BooksDataGridView.SelectedRows.Count;
            string selectfile = GetSelectFile(selectidx);
            string content = String.Empty;
            string text = richTextBoxFileContent.Text;

            switch (keywordarg.Mode)
            {
                case SearchMode.Word:
                    foreach (var word in filedata[selectidx].Word)
                    {
                        // TODO : use word arrary, no fix index
                        if (word.Equals(keywordarg.Keywords[0], StringComparison.OrdinalIgnoreCase))
                        {
                            HighlightRows(Name[selectidx], Color.LightGreen);
                        }
                    }
                    break;
                case SearchMode.Phrase:
                    foreach (var phrase in filedata[selectidx].Sentence)
                    {
                        // TODO : use phrase arrary, no fix index
                        if (phrase.Contains(keywordarg.Keywords[0]))
                        {
                            HighlightRows(Name[selectidx], Color.YellowGreen);
                        }
                    }
                    break;
                case SearchMode.Others:
                default:
                    foreach (var others in filedata[selectidx].Content)
                    {
                        // TODO : use no limit arrary, no fix index
                        if (others.Contains(keywordarg.Keywords[0]))
                        {
                            HighlightRows(Name[selectidx], Color.LightSeaGreen);
                        }
                    }
                    break;
            }
        }
        
        #endregion View Feature
        public void UpdateLabelSearchResult(string text , Color color)
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

        public void HighlightRows(string keyword, Color color)
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

        private void HighlightKeywords(KeywordArg keywordarg, Color color)
        {
            int count = 0;
            Regex regex;
            foreach (string keyword in keywordarg.Keywords)
            {
                switch (keywordarg.Mode)
                {
                    case SearchMode.Word:
                    case SearchMode.Phrase:
                        regex = new Regex(@"\b" + Regex.Escape(keyword) + @"\b", RegexOptions.IgnoreCase);
                        break;
                    case SearchMode.Others:
                    default:
                        // No limited Serch Mode
                        regex = new Regex(keyword, RegexOptions.IgnoreCase);
                        break;
                }
                MatchCollection matches = regex.Matches(richTextBoxFileContent.Text);

                foreach (Match match in matches)
                {
                    richTextBoxFileContent.Select(match.Index, match.Length);
                    richTextBoxFileContent.SelectionBackColor = color;
                    count++;
                }
            }


            if (count == 0)
            {
                UpdateLabelSearchResult(String.Empty, Color.Red);
            }
            else
            {
                string result = $"Find Keyword(s) in Selected file" + Environment.NewLine;
                result += Environment.NewLine;
                result += $"Counts : {count}" + Environment.NewLine;
                UpdateLabelSearchResult(result, Color.Blue);
            }
        }

    }

}