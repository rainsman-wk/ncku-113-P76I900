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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static SearchEnginesApp.ToolModel;
using static System.Net.Mime.MediaTypeNames;


namespace SearchEnginesApp.Views
{
    public partial class SearchEngineTopView : UserControl
    {

        private CheckBox chkAllYears;

        private readonly SearchEngineTopPresenter _presenter;
        public SearchEngineTopView(SearchEngineTopPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            InitializeSearchControls();
            UpdateFileDataBase();
        }
        private void InitializeSearchControls()
        {
            // 年份過濾下拉選單
            cmbYearFilter = new ComboBox
            {
                Location = new Point(pmidTextBox.Left + pmidTextBox.Width + 10, pmidTextBox.Top),
                Size = new Size(80, pmidTextBox.Height),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // 添加年份選項（從今年開始往前10年）
            int currentYear = DateTime.Now.Year;
            for (int i = 0; i <= 10; i++)
            {
                cmbYearFilter.Items.Add((currentYear - i).ToString());
            }
            cmbYearFilter.SelectedIndex = 0; // 預設選擇今年

            // 最大結果數量控制
            nudMaxResults = new NumericUpDown
            {
                Location = new Point(cmbYearFilter.Left + cmbYearFilter.Width + 10, pmidTextBox.Top),
                Size = new Size(100, pmidTextBox.Height),
                Minimum = 1000,
                Maximum = 100000,
                Value = 50000,
                Increment = 1000
            };

            // 全部年份勾選框
            chkAllYears = new CheckBox
            {
                Location = new Point(nudMaxResults.Left + nudMaxResults.Width + 10, pmidTextBox.Top + 3),
                Text = "All Years",
                AutoSize = true
            };

            chkAllYears.CheckedChanged += (s, e) =>
            {
                cmbYearFilter.Enabled = !chkAllYears.Checked;
            };

            // 添加控件到表單
            this.Controls.AddRange(new Control[] { cmbYearFilter, nudMaxResults, chkAllYears });
        }
        private void UpdateFileDataBase()
        {
            _presenter.InitialzeSearchBookDB();
            _presenter.LoadSearchBookDB();
        }

        private async void buttonGetXmlFile_Click(object sender, EventArgs e)
        {
            try
            {
                buttonGetXmlFile.Enabled = false;
                var searchTerm = pmidTextBox.Text;

                if (string.IsNullOrEmpty(searchTerm))
                {
                    MessageBox.Show("Please enter a search term");
                    return;
                }

                // 構建完整的搜索詞
                var searchBuilder = new StringBuilder(searchTerm);

                // 如果不是選擇全部年份，添加年份限制
                if (!chkAllYears.Checked)
                {
                    string selectedYear = cmbYearFilter.SelectedItem.ToString();
                    searchBuilder.Append($" AND {selectedYear}[pdat]");
                }

                int maxResults = (int)nudMaxResults.Value;
                lblPmidLoadState.Text = "Searching PubMed...";
                var processor = new PubMedProcessor();
                var db = new Utils.SQLiteDb();

                var progress = new Progress<int>(value =>
                {
                    lblPmidLoadState.Text = $"Downloading: {value}%";
                });

                // 使用完整的搜索詞
                var pmids = await processor.SearchPubMed(searchBuilder.ToString(), maxResults);

                if (pmids.Count > 0)
                {
                    lblPmidLoadState.Text = $"Found {pmids.Count} articles, downloading details...";
                    var articles = await processor.GetArticleDetails(pmids, searchTerm, progress);
                    await db.AddArticlesAsync(articles);

                    var topKeywords = await db.GetTopKeywordsAsync(20);
                    var keywordSummary = string.Join("\n", topKeywords.Select(k => $"{k.Item1}: {k.Item2}"));

                    lblPmidLoadState.Text = $"Completed: {articles.Count} articles saved to database";

                    // 顯示關鍵字統計
                    MessageBox.Show($"Top 20 Keywords:\n\n{keywordSummary}", "Download Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblPmidLoadState.Text = "No articles found";
                    MessageBox.Show("No articles found for the given search criteria.",
                        "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblPmidLoadState.Text = "Error occurred during download";
            }
            finally
            {
                buttonGetXmlFile.Enabled = true;
            }
        }
    

        private void buttonLoadFile_Click(object sender, EventArgs e)
        {
            _presenter.ResetBookDatabase();

            OpenFileDialog LoadFiles = new OpenFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
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
                    SearchWordArg keywordArg = new SearchWordArg();
                    switch (CheckRadioButtonState())
                    {
                        case "Word":
                            // Operation by comma (,) or space ( )
                            keywordArg.SearchWords = text.Split(' ', ',').ToList();
                            keywordArg.Mode = SearchMode.Word;
                            break;
                        case "Phrase":
                            // Operation by comma (,)
                            keywordArg.SearchWords = text.Split(',').ToList();
                            keywordArg.Mode = SearchMode.Phrase;
                            break;
                        default:
                            // No operation
                            keywordArg.SearchWords.Add(text);
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

        private void btnLoadWord2Vec_Click(object sender, EventArgs e)
        {
            _presenter.Word2VecDataLoad();
        }

        private void btnLoadPubmed_Click(object sender, EventArgs e)
        {
            _presenter.LoadAbstractsData();
        }

        private void chkAllYears_CheckedChanged(object sender, EventArgs e)
        {
            if(chkAllYears.Checked)
            {
                cmbYearFilter.Enabled = false;
            }
            else
            {
                cmbYearFilter.Enabled = true;
            }
        }
    }

    public class PubMedProcessor
    {
        private readonly HttpClient client;
        private const int MaxBatchSize = 200; // PubMed API 建議的批次大小
        private const int MaxRetries = 3;
        private const int DelayBetweenRetries = 1000; // 毫秒

        public PubMedProcessor()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(5);
        }

        public async Task<List<string>> SearchPubMed(string searchTerm, int maxResults = 50000)
        {
            try
            {
                // 1. 使用 esearch 獲取 PMIDs
                string searchUrl = $"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?db=pubmed&term={Uri.EscapeDataString(searchTerm)}&retmax={maxResults}&usehistory=y";
                string searchResponse = await RetryWithDelayAsync(() => client.GetStringAsync(searchUrl));

                // 解析 WebEnv 和 QueryKey
                var doc = new XmlDocument();
                doc.LoadXml(searchResponse);
                string webEnv = doc.SelectSingleNode("//WebEnv")?.InnerText;
                string queryKey = doc.SelectSingleNode("//QueryKey")?.InnerText;
                var pmids = new List<string>();
                var idNodes = doc.SelectNodes("//IdList/Id");
                if (idNodes != null)
                {
                    foreach (XmlNode node in idNodes)
                    {
                        pmids.Add(node.InnerText);
                    }
                }

                return pmids;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching PubMed: {ex.Message}");
            }
        }

        public async Task<List<string>> GetAbstracts(List<string> pmids, IProgress<int> progress = null)
        {
            var abstracts = new List<string>();
            var batches = CreateBatches(pmids, MaxBatchSize);
            int processedBatches = 0;

            foreach (var batch in batches)
            {
                string ids = string.Join(",", batch);
                string url = $"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id={ids}&retmode=xml";

                try
                {
                    string xmlContent = await RetryWithDelayAsync(() => client.GetStringAsync(url));
                    var doc = new XmlDocument();
                    doc.LoadXml(xmlContent);

                    var abstractNodes = doc.SelectNodes("//Abstract/AbstractText");
                    if (abstractNodes != null)
                    {
                        foreach (XmlNode node in abstractNodes)
                        {
                            if (!string.IsNullOrWhiteSpace(node.InnerText))
                            {
                                abstracts.Add(node.InnerText);
                            }
                        }
                    }

                    processedBatches++;
                    if (progress != null)
                    {
                        int percentComplete = (int)((float)processedBatches / batches.Count * 100);
                        progress.Report(percentComplete);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching batch: {ex.Message}");
                }

                await Task.Delay(100); // 遵守 PubMed API 的速率限制
            }

            return abstracts;
        }

        private async Task<string> RetryWithDelayAsync(Func<Task<string>> operation)
        {
            for (int i = 0; i < MaxRetries; i++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception) when (i < MaxRetries - 1)
                {
                    await Task.Delay(DelayBetweenRetries);
                }
            }
            return await operation(); // 最後一次嘗試
        }
        public async Task<List<PubMedArticle>> GetArticleDetails(List<string> pmids, string searchTerm, IProgress<int> progress)
        {
            var articles = new List<PubMedArticle>();
            var batches = CreateBatches(pmids, MaxBatchSize);
            int processedBatches = 0;

            foreach (var batch in batches)
            {
                string ids = string.Join(",", batch);
                string url = $"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id={ids}&retmode=xml";

                try
                {
                    string xmlContent = await RetryWithDelayAsync(() => client.GetStringAsync(url));
                    var doc = new XmlDocument();
                    doc.LoadXml(xmlContent);

                    var articleNodes = doc.SelectNodes("//PubmedArticle");
                    if (articleNodes != null)
                    {
                        foreach (XmlNode articleNode in articleNodes)
                        {
                            try
                            {
                                var article = new PubMedArticle
                                {
                                    PMID = GetNodeValue(articleNode, ".//PMID"),
                                    Title = GetNodeValue(articleNode, ".//ArticleTitle"),
                                    Abstract = GetNodeValue(articleNode, ".//Abstract/AbstractText"),
                                    Authors = GetAuthors(articleNode),
                                    Keywords = GetKeywords(articleNode),
                                    PublicationDate = GetPublicationDate(articleNode),
                                    ImportDate = DateTime.Now,
                                    SearchTerm = searchTerm
                                };
                                articles.Add(article);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error parsing article: {ex.Message}");
                                continue;
                            }
                        }
                    }

                    processedBatches++;
                    if (progress != null)
                    {
                        int percentComplete = (int)((float)processedBatches / batches.Count * 100);
                        progress.Report(percentComplete);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching batch: {ex.Message}");
                }

                await Task.Delay(100);
            }

            return articles;
        }

        // 輔助方法來分割列表為批次
        private List<List<T>> CreateBatches<T>(List<T> source, int batchSize)
        {
            var result = new List<List<T>>();
            for (int i = 0; i < source.Count; i += batchSize)
            {
                result.Add(source.Skip(i).Take(batchSize).ToList());
            }
            return result;
        }
        private string GetNodeValue(XmlNode node, string xpath)
        {
            var selectedNode = node.SelectSingleNode(xpath);
            return selectedNode?.InnerText ?? string.Empty;
        }

        private string GetAuthors(XmlNode articleNode)
        {
            var authors = new List<string>();
            var authorNodes = articleNode.SelectNodes(".//Author");
            if (authorNodes != null)
            {
                foreach (XmlNode author in authorNodes)
                {
                    var lastName = GetNodeValue(author, ".//LastName");
                    var foreName = GetNodeValue(author, ".//ForeName");
                    if (!string.IsNullOrEmpty(lastName))
                    {
                        authors.Add($"{lastName} {foreName}".Trim());
                    }
                }
            }
            return string.Join("; ", authors);
        }

        private string GetKeywords(XmlNode articleNode)
        {
            var keywords = new List<string>();
            var keywordNodes = articleNode.SelectNodes(".//Keyword");
            if (keywordNodes != null)
            {
                foreach (XmlNode keyword in keywordNodes)
                {
                    if (!string.IsNullOrEmpty(keyword.InnerText))
                    {
                        keywords.Add(keyword.InnerText.Trim());
                    }
                }
            }
            return string.Join("; ", keywords);
        }

        private DateTime GetPublicationDate(XmlNode articleNode)
        {
            try
            {
                var yearNode = articleNode.SelectSingleNode(".//PubDate/Year");
                var monthNode = articleNode.SelectSingleNode(".//PubDate/Month");
                var dayNode = articleNode.SelectSingleNode(".//PubDate/Day");

                int year = yearNode != null ? int.Parse(yearNode.InnerText) : DateTime.Now.Year;
                int month = monthNode != null ? ParseMonth(monthNode.InnerText) : 1;
                int day = dayNode != null ? int.Parse(dayNode.InnerText) : 1;

                return new DateTime(year, month, day);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        private int ParseMonth(string month)
        {
            if (int.TryParse(month, out int result))
            {
                return result;
            }

            switch (month.ToLower())
            {
                case "jan": return 1;
                case "feb": return 2;
                case "mar": return 3;
                case "apr": return 4;
                case "may": return 5;
                case "jun": return 6;
                case "jul": return 7;
                case "aug": return 8;
                case "sep": return 9;
                case "oct": return 10;
                case "nov": return 11;
                case "dec": return 12;
                default: return 1;
            }
        }
    }

}
