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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SearchEnginesApp.Views
{
    public partial class SerchEngineView : UserControl
    {
        private readonly SerchEnginePresenter _presenter;
        public SerchEngineView(SerchEnginePresenter presenter)
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


        public struct TextBooks
        {
            private List<string> word;
            private List<string> sentence;
            public string FileName { get; set; }
            public string FileType { get; set; }

            public string FilePath { get; set; }
            public string Content { get; set; }
            public List<string> Word
            {
                get { return word; }
                set { word = value; }
            }
            public List<string> Sentence
            {
                get { return sentence; }
                set { sentence = value; }
            }
            public TextBooks(string filename, string filepath, string filetype, string content)
            {
                FileName = filename;
                FilePath = filepath;
                FileType = filetype;
                Content = content;
                word = new List<string>();
                sentence = new List<string>();
            }
        }
        public List<TextBooks> books = new List<TextBooks>();


        private void buttonLoadFile_Click(object sender, EventArgs e)
        {
            // Reset List
            books = new List<TextBooks>();
            BooksDataGridView.Rows.Clear();

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
                            TextBooks book = new TextBooks
                            {
                                FileName = LoadFiles.SafeFileNames[i],
                                FilePath = LoadFiles.FileNames[i],
                                FileType = Path.GetExtension(LoadFiles.FileNames[i]),
                                Content = GetXmlContent(LoadFiles.FileNames[i])
                            };
                            // Get Number of Words from Content
                            book.Word = book.Content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            // Get Number of Sentence
                            book.Sentence = book.Content.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                            BooksDataGridView.Rows.Add(i.ToString(), book.FileName, book.FileType, book.Content.Replace(" ", "").Length, book.Word.Count, book.Sentence.Count);
                            books.Add(book);

                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Load file Error{ex.Message}");
                }
            }

            //XmlDocument doc = new XmlDocument();

        }

        private string GetXmlContent(string path)
        {
            string context = "";
            if (File.Exists(path))
            {
                XDocument doc = XDocument.Load(path);
                var abstracts = doc.Descendants("Abstract").Descendants("AbstractText").Select(element => element.Value);
                foreach (string text in abstracts)
                {
                    context += text + Environment.NewLine;
                }
            }


            return context;
        }


        private void BooksDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show("Test");
        }

        private void BooksDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            MessageBox.Show("Test2");


        }

        private void textBoxQueryKeywords_TextChanged(object sender, EventArgs e)
        {
            // Reset Query State
            ResetHighlightRows();
            int count = 0;

            // Start Serach of keywords
            foreach (var book in books)
            {
                foreach (var word in book.Word)
                {
                    if (word.Equals(textBoxQueryKeywords.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        count++;
                        HighlightRows(book.FileName);
                    }
                }
            }
            if (count > 0)
            {
                labelQueryStatus.Text = $"Serach Result: {count.ToString()}";
                labelQueryStatus.ForeColor = Color.Blue;
            }
            else
            {
                labelQueryStatus.Text = $"Serach Result: Not Found Keyword(s)";
                labelQueryStatus.ForeColor = Color.Red;
            }

        }


        private void ResetHighlightRows()
        {
            foreach (DataGridViewRow row in BooksDataGridView.Rows)
            {
                row.DefaultCellStyle.BackColor = SystemColors.Window;
            }
        }

        private void HighlightRows(string keyword)
        {
            foreach (DataGridViewRow row in BooksDataGridView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().Contains(keyword))
                    {
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                        break;
                    }
                }
            }
        }

    }

}
