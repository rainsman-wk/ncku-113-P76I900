using SearchEnginesApp.Utils;
using SearchEnginesApp.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static SearchEnginesApp.ToolModel;
using static SearchEnginesApp.Views.SearchEngineResultView;

namespace SearchEnginesApp.Presenters
{
    public class SearchEngineTopPresenter
    {
        private readonly ToolModel _toolModel;
        private SearchEngineTopView View { get; set; }
        private FilesTreeView FileView { get; set; }

        /// <summary>
        /// SerchEngineTop Presenter
        /// </summary>
        /// <param name="toolModel">The Tool Model</param>
        public SearchEngineTopPresenter(ToolModel toolModel)
        {
            _toolModel = toolModel;
        }


        /// <summary>
        /// Method to show the view
        /// </summary>
        /// <returns>UserControl, the view connected to this presenter</returns>
        public UserControl ShowView()
        {
            if (View == null || View.IsDisposed)
            {
                View = new SearchEngineTopView(this);
                /* Register the required events */
            }

            return View;
        }

        /// <summary>
        /// Method called when view is closed
        /// </summary>
        public void CloseView()
        {
            if (View != null && !View.IsDisposed) View.Dispose();
        }

        public FileContent GetXmlContent(string path)
        {
            FileContent context = new FileContent();
            if (File.Exists(path))
            {
                XmlDocument file = new XmlDocument();
                file.Load(path);

                XmlNodeList articleNodes = file.SelectNodes("//PubmedArticle");
                foreach (XmlNode articleNode in articleNodes)
                {
                    context.Title = articleNode.SelectSingleNode("MedlineCitation/Article/ArticleTitle")?.InnerText;
                    XmlNodeList abstractNodes = articleNode.SelectNodes("MedlineCitation/Article/Abstract/AbstractText");
                    context.Abstract = abstractNodes.Cast<XmlNode>().Select(node => node.InnerText).ToList();
                    context.Journal = articleNode.SelectSingleNode("MedlineCitation/Article/Journal/Title")?.InnerText;
                    context.Pmid = articleNode.SelectSingleNode("MedlineCitation/PMID")?.InnerText;
                }
            }
            return context;
        }

        public void ResetBookDatabase()
        {
            _toolModel.ClearFileList();
            _toolModel.CleanSearchBooks();
        }

        public void GetFileLoadfromDatabase()
        {
            _toolModel.SetEventLoadFiles();
        }
        public void InitialzeSearchBookDB()
        {
            _toolModel.DatabaseInitialize();
        }
        public void LoadSearchBookDB()
        {
            _toolModel.GetSearchBookDB();
        }

        public void AddFileListToSearchEngine(string file)
        {
            _toolModel.AddFileList(file);
        }

        #region Search Features...
        public bool CheckFileSelected()
        {
            return _toolModel.IsSerachFileVailded();
        }

        public void FileAnalysis()
        {
            // Reset Search DataBase
            _toolModel.CleanSearchBooks();

            List<string> searchlist = new List<string>();
            _toolModel.GetSearchFileList(out searchlist);

            List<SearchBooks> books = new List<SearchBooks>();

            DatabaseHelper dbHelper = new DatabaseHelper();

            for (int i = 0; i < searchlist.Count; i++)
            {
                var book = new SearchBooks(searchlist[i], GetXmlContent(searchlist[i]));
                books.Add(book);
                dbHelper.InsertBook(book);
            }
            // Trigger Event to other view to update Serach book
            _toolModel.SetEventUpdateSerchBook(books);
        }
        public void SetSearchKeyWord(SearchWordArg keyword)
        {
            _toolModel.SetSearchWords(keyword);
            _toolModel.SetKeywordSearchEvent();
        }
        #endregion Search Features...

        public void Word2VecDataLoad()
        {
            List<string> tokens = new List<string>();
            List<SearchBooks> books = _toolModel.GetSerachBooks();
            foreach (var book in books)
            {
                tokens.AddRange(book.Content.Word);
            }

            Word2VecForm word2Vec = new Word2VecForm(tokens, "searchbooks");
            word2Vec.Show();

        }

        public async void LoadAbstractsData()
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "SQLite Database Files (*.db)|*.db|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.Title = "Select PubMed Database File";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string dbPath = openFileDialog.FileName;
                        var db = new SQLiteDb(dbPath);

                        // 獲取所有文章的摘要
                        var articles = await db.GetAllArticlesAsync();

                        if (!articles.Any())
                        {
                            MessageBox.Show("No articles found in the database.", "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        string dbFileName = Path.GetFileName(dbPath);

                        // 收集所有摘要並處理成 tokens
                        List<string> abstractTokens = new List<string>();
                        foreach (var article in articles)
                        {
                            if (!string.IsNullOrEmpty(article.Abstract))
                            {
                                var tokens = article.Abstract
                                    .Split(new[] { ' ', '.', ',', ';', ':', '!', '?', '(', ')', '[', ']', '\n', '\r', '\t' },
                                        StringSplitOptions.RemoveEmptyEntries)
                                    .Select(t => t.Trim().ToLower())
                                    .Where(t => !string.IsNullOrEmpty(t))
                                    .ToList();

                                abstractTokens.AddRange(tokens);
                            }
                        }

                        if (abstractTokens.Any())
                        {
                            Word2VecForm word2Vec = new Word2VecForm(abstractTokens, dbFileName);
                            word2Vec.Show();
                        }
                        else
                        {
                            MessageBox.Show("No valid abstract content found.", "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading database: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async void LoadDocsForTfIdf()
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "SQLite Database Files (*.db)|*.db|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.Title = "Select PubMed Database File";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string dbPath = openFileDialog.FileName;
                        var db = new SQLiteDb(dbPath);
                        var articles = await db.GetAllArticlesAsync();
                        var abstracts = articles.Where(a => !string.IsNullOrEmpty(a.Abstract)).Take(200).Select(a => a.Abstract).ToList();

                        if (abstracts.Count == 0)
                        {
                            MessageBox.Show("No abstracts found in database.");
                            return;
                        }
                        
                        TfIdfAnalyzerForm tfidf = new TfIdfAnalyzerForm(abstracts);
                        tfidf.Show();


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading database: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public class SQLiteDb
        {
            private readonly string connectionString;

            public SQLiteDb(string dbPath)
            {
                if (string.IsNullOrEmpty(dbPath))
                {
                    throw new ArgumentException("Database path cannot be empty.");
                }

                connectionString = $"Data Source={dbPath};Version=3;";
            }

            public async Task<List<PubMedArticle>> GetAllArticlesAsync()
            {
                var articles = new List<PubMedArticle>();

                using (var connection = new SQLiteConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    SELECT Id, PMID, Title, Abstract, Authors, Keywords, 
                           PublicationDate, ImportDate, SearchTerm 
                    FROM PubMedArticles";

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                try
                                {
                                    var article = new PubMedArticle
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                        PMID = reader.GetString(reader.GetOrdinal("PMID")),
                                        Title = reader.GetString(reader.GetOrdinal("Title")),
                                        Abstract = reader.IsDBNull(reader.GetOrdinal("Abstract")) ?
                                            null : reader.GetString(reader.GetOrdinal("Abstract")),
                                        Authors = reader.IsDBNull(reader.GetOrdinal("Authors")) ?
                                            null : reader.GetString(reader.GetOrdinal("Authors")),
                                        Keywords = reader.IsDBNull(reader.GetOrdinal("Keywords")) ?
                                            null : reader.GetString(reader.GetOrdinal("Keywords")),
                                        PublicationDate = DateTime.Parse(
                                            reader.GetString(reader.GetOrdinal("PublicationDate"))),
                                        ImportDate = DateTime.Parse(
                                            reader.GetString(reader.GetOrdinal("ImportDate"))),
                                        SearchTerm = reader.GetString(reader.GetOrdinal("SearchTerm"))
                                    };
                                    articles.Add(article);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error reading article: {ex.Message}");
                                    continue;
                                }
                            }
                        }
                    }
                }

                return articles;
            }
        }
    }
}
