using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using static SearchEnginesApp.ToolModel;
using System.Data.Entity;
using System.IO;
using System.Data.Common;


namespace SearchEnginesApp.Utils
{
    public class DatabaseHelper
    {
        private string connectionString = "Data Source=searchbooks.db;Version=3";

        public void InitializeDatabase()
        {
            // Ensure the database file exists
            if (!System.IO.File.Exists("searchbooks.db"))
            {
                System.IO.File.Create("searchbooks.db").Dispose();
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS SearchBooks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Path TEXT,
                    Pmid TEXT,
                    Title TEXT,
                    Journal TEXT,
                    Abstract TEXT
                )";
                using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void InsertBook(SearchBooks book)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string insertQuery = @"
                INSERT INTO SearchBooks (Path, Pmid, Title, Journal, Abstract)
                VALUES (@Path, @Pmid, @Title, @Journal, @Abstract)";
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Path", book.Path);
                    command.Parameters.AddWithValue("@Pmid", book.Content.Pmid);
                    command.Parameters.AddWithValue("@Title", book.Content.Title);
                    command.Parameters.AddWithValue("@Journal", book.Content.Journal);
                    command.Parameters.AddWithValue("@Abstract", string.Join(" ", book.Content.Abstract));
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<SearchBooks> GetBooks()
        {
            List<SearchBooks> books = new List<SearchBooks>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string selectQuery = "SELECT * FROM SearchBooks";
                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var book = new SearchBooks
                            {
                                Path = reader["Path"].ToString(),
                                Content = new FileContent
                                {
                                    Pmid = reader["Pmid"].ToString(),
                                    Title = reader["Title"].ToString(),
                                    Journal = reader["Journal"].ToString(),
                                    Abstract = reader["Abstract"].ToString().Split(' ').ToList()
                                }
                            };
                            books.Add(book);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving books: {ex.Message}");
            }
            return books;

        }
    }
    public class PubMedArticle
    {
        public int Id { get; set; }
        public string PMID { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Authors { get; set; }
        public string Keywords { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime ImportDate { get; set; }
        public string SearchTerm { get; set; }
    }

    public class SQLiteDb
    {
        private readonly string connectionString;

        public SQLiteDb()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PubMed.db");
            connectionString = $"Data Source={dbPath};Version=3;";
            CreateDatabase();
        }

        private void CreateDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS PubMedArticles (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PMID TEXT UNIQUE,
                        Title TEXT,
                        Abstract TEXT,
                        Authors TEXT,
                        Keywords TEXT,
                        PublicationDate TEXT,
                        ImportDate TEXT,
                        SearchTerm TEXT
                    );
                    CREATE INDEX IF NOT EXISTS idx_pmid ON PubMedArticles(PMID);
                    CREATE INDEX IF NOT EXISTS idx_searchterm ON PubMedArticles(SearchTerm);";
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task AddArticlesAsync(List<PubMedArticle> articles)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = @"
                            INSERT OR IGNORE INTO PubMedArticles 
                            (PMID, Title, Abstract, Authors, Keywords, PublicationDate, ImportDate, SearchTerm)
                            VALUES 
                            (@PMID, @Title, @Abstract, @Authors, @Keywords, @PublicationDate, @ImportDate, @SearchTerm)";

                            var pmidParam = command.CreateParameter();
                            var titleParam = command.CreateParameter();
                            var abstractParam = command.CreateParameter();
                            var authorsParam = command.CreateParameter();
                            var keywordsParam = command.CreateParameter();
                            var pubDateParam = command.CreateParameter();
                            var impDateParam = command.CreateParameter();
                            var searchTermParam = command.CreateParameter();

                            command.Parameters.Add(pmidParam);
                            command.Parameters.Add(titleParam);
                            command.Parameters.Add(abstractParam);
                            command.Parameters.Add(authorsParam);
                            command.Parameters.Add(keywordsParam);
                            command.Parameters.Add(pubDateParam);
                            command.Parameters.Add(impDateParam);
                            command.Parameters.Add(searchTermParam);

                            foreach (var article in articles)
                            {
                                pmidParam.ParameterName = "@PMID";
                                pmidParam.Value = article.PMID;
                                titleParam.ParameterName = "@Title";
                                titleParam.Value = article.Title;
                                abstractParam.ParameterName = "@Abstract";
                                abstractParam.Value = article.Abstract;
                                authorsParam.ParameterName = "@Authors";
                                authorsParam.Value = article.Authors;
                                keywordsParam.ParameterName = "@Keywords";
                                keywordsParam.Value = article.Keywords;
                                pubDateParam.ParameterName = "@PublicationDate";
                                pubDateParam.Value = article.PublicationDate.ToString("yyyy-MM-dd");
                                impDateParam.ParameterName = "@ImportDate";
                                impDateParam.Value = article.ImportDate.ToString("yyyy-MM-dd HH:mm:ss");
                                searchTermParam.ParameterName = "@SearchTerm";
                                searchTermParam.Value = article.SearchTerm;

                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<List<PubMedArticle>> GetArticlesBySearchTermAsync(string searchTerm)
        {
            var articles = new List<PubMedArticle>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM PubMedArticles WHERE SearchTerm = @SearchTerm";
                    command.Parameters.AddWithValue("@SearchTerm", searchTerm);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            articles.Add(ReadArticle(reader));
                        }
                    }
                }
            }
            return articles;
        }

        public async Task<List<string>> GetAllAbstractsAsync()
        {
            var abstracts = new List<string>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Abstract FROM PubMedArticles WHERE Abstract IS NOT NULL AND Abstract != ''";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            abstracts.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return abstracts;
        }

        private PubMedArticle ReadArticle(DbDataReader reader)
        {
            return new PubMedArticle
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                PMID = reader.GetString(reader.GetOrdinal("PMID")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Abstract = reader.IsDBNull(reader.GetOrdinal("Abstract")) ? null : reader.GetString(reader.GetOrdinal("Abstract")),
                Authors = reader.IsDBNull(reader.GetOrdinal("Authors")) ? null : reader.GetString(reader.GetOrdinal("Authors")),
                Keywords = reader.IsDBNull(reader.GetOrdinal("Keywords")) ? null : reader.GetString(reader.GetOrdinal("Keywords")),
                PublicationDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("PublicationDate"))),
                ImportDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("ImportDate"))),
                SearchTerm = reader.GetString(reader.GetOrdinal("SearchTerm"))
            };
        }

        public async Task<List<(string term, int count)>> GetTopKeywordsAsync(int top = 100)
        {
            var keywords = new List<(string term, int count)>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    WITH RECURSIVE split(keyword, str) AS (
                        SELECT '', Keywords || ';'
                        FROM PubMedArticles
                        WHERE Keywords IS NOT NULL
                        UNION ALL
                        SELECT
                            substr(str, 0, instr(str, ';')),
                            substr(str, instr(str, ';')+1)
                        FROM split WHERE str != ''
                    )
                    SELECT trim(keyword) as term, COUNT(*) as count
                    FROM split
                    WHERE keyword != ''
                    GROUP BY trim(keyword)
                    ORDER BY count DESC
                    LIMIT @Top";
                    command.Parameters.AddWithValue("@Top", top);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            keywords.Add((
                                reader.GetString(0),
                                reader.GetInt32(1)
                            ));
                        }
                    }
                }
            }
            return keywords;
        }
    }
}
