using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using static SearchEnginesApp.ToolModel;

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
}
