using SearchEnginesApp.Presenters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SearchEnginesApp.ToolModel;
using static System.Net.Mime.MediaTypeNames;


namespace SearchEnginesApp.Views
{
    public partial class SearchEngineTopView : UserControl
    {

        private readonly SearchEngineTopPresenter _presenter;
        public SearchEngineTopView(SearchEngineTopPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;


        }

        private async void buttonGetXmlFile_Click(object sender, EventArgs e)
        {
            string pmid = pmidTextBox.Text;
            if (string.IsNullOrEmpty(pmid))
            {
                MessageBox.Show("Please Enter PMID from URL");
                return;
            }

            string url = $"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id={pmid}&retmode=xml";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string xmlContent = await client.GetStringAsync(url);
                    string SaveFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{pmid}.xml");
                    File.WriteAllText(SaveFilePath, xmlContent);
                    MessageBox.Show("XML from URL Save Pass");

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Download XML File from URL Error：{ex.Message}");
                }
            }
        }

        private void buttonLoadFile_Click(object sender, EventArgs e)
        {
            //Reset BookDatabase
            _presenter.ResetBookDatabase();
            // Load files
            OpenFileDialog LoadFiles = new OpenFileDialog
            {
                Filter = "XML and JSON Files|*.xml;*.json",
                Multiselect = true,
            };
            if (LoadFiles.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (LoadFiles.FileNames.Count() > 0)
                    {
                        for (int i = 0; i < LoadFiles.FileNames.Count(); i++)
                        {
                            _presenter.AddFileListToSearchEngine(LoadFiles.FileNames[i]);
                        }
                        _presenter.GetFileLoadfromDatabase();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Load file Error{ex.Message}");
                }
            }

        }

        private void buttonFileAnalysis_Click(object sender, EventArgs e)
        {
            _presenter.FileAnalysis();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if(_presenter.CheckFileSelected())
            {
                string text = cbSerachContent.Text;
                if(text != String.Empty)
                {
                    KeywordArg keywordArg = new KeywordArg();
                    switch (CheckRadioButtonState())
                    {
                        case "Word":
                            // Operation by comma (,) or space ( )
                            keywordArg.Keywords = text.Split(' ', ',').ToList();
                            keywordArg.Mode = SearchMode.Word;
                            break;
                        case "Phrase":
                            // Operation by comma (,)
                            keywordArg.Keywords = text.Split(',').ToList();
                            keywordArg.Mode = SearchMode.Phrase;
                            break;
                        default:
                            // No operation
                            keywordArg.Keywords.Add(text);
                            keywordArg.Mode = SearchMode.Others;
                            break;
                    }
                    keywordArg.InSelection = cbInSelection.Checked;
                    keywordArg.MatchCase = cbMatchCase.Checked;
                    _presenter.SetSearchKeyWord(keywordArg);

                    // Save Search Histroy
                    string selectedItem = cbSerachContent.Text;
                    if(!cbSerachContent.Items.Contains(selectedItem) && !string.IsNullOrWhiteSpace(selectedItem))
                    {
                        cbSerachContent.Items.Insert(0,selectedItem);
                    }

                }
                else
                {
                    MessageBox.Show($"Please fill Keyword(s)", "Action Warning");
                }
            }
            else
            {
                MessageBox.Show($"Please Select Serach Files", "Action Warning");
            }
        }

        private void cbSerachContent_TextUpdate(object sender, EventArgs e)
        {
            cbSerachContent.ForeColor = Color.Black;
            KeyWordStringCheck(cbSerachContent.Text);
        }
        private void KeyWordStringCheck(string text)
        {
            bool inputfail = false;
            List<string> checkstring = new List<string>();
            if (string.IsNullOrEmpty(text))
            {
                cbSerachContent.ForeColor = Color.Black;
            }
            else
            {
                switch (CheckRadioButtonState())
                {
                    case "Word":
                        checkstring = text.Split(',',' ').ToList();
                        if(checkstring.Count>0)
                        {
                            foreach(string s in checkstring)
                            {
                                if(Regex.IsMatch(s, @"^\S+$") == false) { inputfail |= true; }
                            }
                        }
                        break;
                    case "Phrase":
                        checkstring = text.Split(',').ToList();
                        if (checkstring.Count > 0)
                        {
                            foreach (string s in checkstring)
                            {
                                if (Regex.IsMatch(s, @"\s") == false) { inputfail |= true; }

                            }
                        }
                        break;
                    default:
                        cbSerachContent.ForeColor = Color.Black;
                        break;
                }

                cbSerachContent.ForeColor = (inputfail) ? Color.Red : Color.Blue;

            }
        }

        private string CheckRadioButtonState()
        {
            string selectBtn = string.Empty;
            if(rbWord.Checked)
            {
                selectBtn = rbWord.Text;
            }
            else if (rbPhrase.Checked)
            {
                selectBtn = rbPhrase.Text;
            }
            else
            {
                selectBtn = rbOthers.Text;
            }
            return selectBtn;
        }

        private void SearchMode_CheckedChanged(object sender, EventArgs e)
        {
            KeyWordStringCheck(cbSerachContent.Text);
        }
    }
}
