using SearchEnginesApp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.XPath;

namespace SearchEnginesApp
{
    public class ToolModel
    {
        public enum SearchMode
        {
            Word,
            Phrase,
            Others
        }
        public class SearchWordArg
        {
            public SearchMode Mode { get; set; }
            public List<string> SearchWords { get; set; }
            public bool InSelection { get; set; }
            public bool MatchCase { get; set; }
            public SearchWordArg()
            {
                Mode = SearchMode.Others;
                SearchWords = new List<string>();
                InSelection = false;
                MatchCase = false;
            }
            public SearchWordArg(bool selection, bool matchcase)
            {
                InSelection = selection;
                MatchCase = matchcase;
            }
        }


        #region Event Handler...
        #region Event List..
        public event EventHandler<FileEventArgs> FilesLoaded;
        public event EventHandler<FileAnalsisEventArgs> FileAnalysisLoaded;
        public event EventHandler<KeywordEventArgs> SearchKeywordLoaded;
        #endregion Event List..

        #region File Event ..

        public void SetEventLoadFiles()
        {
            List<string> xmlFiles = new List<string>();
            List<string> jsonFiles = new List<string>();
            GetFileList(out xmlFiles, out jsonFiles);
            OnFilesLoaded(new FileEventArgs(xmlFiles, jsonFiles));
        }
        protected virtual void OnFilesLoaded(FileEventArgs e)
        {
            FilesLoaded?.Invoke(this, e);
        }
        public class FileEventArgs : EventArgs
        {
            public List<string> XmlFiles { get; }
            public List<string> JsonFiles { get; }

            public FileEventArgs(List<string> xmlFiles, List<string> jsonFiles)
            {
                XmlFiles = xmlFiles;
                XmlFiles.Sort();
                JsonFiles = jsonFiles;
                JsonFiles.Sort();
            }
        }
        #endregion File Event ..

        #region File Analsis Event...

        #endregion File Analsis Event...
        public void SetEventUpdateSerchBook(List<SearchBooks> books)
        {
            bookDataBase = books;
            OnGetSearchData(new FileAnalsisEventArgs(bookDataBase));
        }
        protected virtual void OnGetSearchData(FileAnalsisEventArgs e)
        {
            FileAnalysisLoaded?.Invoke(this, e);
        }
        public class FileAnalsisEventArgs : EventArgs
        {
            public List<SearchBooks> SearchData { get; }

            public FileAnalsisEventArgs(List<SearchBooks> books)
            {
                SearchData = books;
            }
        }

        #region Search Data Event...
        public void SetKeywordSearchEvent()
        {
            OnGetKeywordData(new KeywordEventArgs(searchkey));
        }
        protected virtual void OnGetKeywordData(KeywordEventArgs e)
        {
            SearchKeywordLoaded?.Invoke(this, e);
        }
        public class KeywordEventArgs : EventArgs
        {
            public SearchWordArg Keywordarg { get; }

            public KeywordEventArgs(SearchWordArg keywordarg)
            {
                Keywordarg = keywordarg;
            }
        }
        #endregion Search Data Event...




        #endregion Event Handler...


        #region Struct ....

        #region FileBook

        #endregion FileBook
        public class FileBooks
        {
            public string File { get; set; }
            public bool CheckedState { get; set; }

            public FileBooks()
            {
                File = "";
                CheckedState = false;
            }

            public FileBooks(string value1, bool value2)
            {
                File = value1;
                CheckedState = value2;
            }
        }

        #region SearchBooks
        /// <summary>
        /// - Book
        /// - Content Struct
        ///   - Content
        ///   - Words
        ///   - Sentence
        /// - Calcuated Strcut
        ///   - characters (including spaces)
        ///   - characters (excluding spaces)
        ///   - words
        ///   - sentences
        ///   - non-ASCII characters
        ///   - non-ASCII words
        /// 
        /// 
        /// </summary>
        public class SearchBooks
        {
            public string Path { get; set; }

            public FileContent Content { get; set; }
            public SearchBooks()
            {
                Path = string.Empty;
                Content = new FileContent();
            }
            public SearchBooks(string path, FileContent content)
            {
                Path = path;
                Content = content;
            }
        }
        #endregion Testboox


        #region Content struct ...
        /// - Content Struct
        ///   - Content
        ///   - Words
        ///   - Sentence
        public class FileContent
        {
            public string Pmid { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty ;
            public string Journal { get; set; } = string.Empty;
            public Dictionary<string, int> WordFrequency { get; set; } = new Dictionary<string, int>();
            private List<string> abstractList = new List<string>();
            public List<string> Abstract
            {
                get { return abstractList; }
                set
                {
                    abstractList = value;
                    UpdateWordAndSentence();
                    UpdateWordTable();
                }
            }
            public List<string> Word { get; private set; } = new List<string>();
            public List<string> Sentence { get; private set; } = new List<string>();
            public FileContent() { }
        
            public int WordsCount()
            {
                return Word.Count;
            }
            public int SentenceCount()
            {
                return Sentence.Count;
            }
            public int AbstractCount()
            {
                int count = 0;
                foreach (var value in Abstract)
                {
                    count += value.Count();
                }
                return count;
            }
            public int AbstractCountNoSpace()
            {
                int count = 0;
                foreach (var value in Abstract)
                {
                    count += value.Replace(" ", "").Count();
                }
                return count;
            }
            public int AbstractCountNonAsciiChar()
            {
                int count = 0;
                // Set Non ASCII Char Pattern
                string pattern = @"[^\u0000-\u007F]";
                foreach (var value in Abstract)
                {
                    MatchCollection matches = Regex.Matches(value, pattern);
                    count += matches.Count;
                }
                return count;
            }
            public int AbstractCountNonAsciiWord()
            {
                int count = 0;
                // Set Non ASCII Word Pattern
                string pattern = @"\b\w*[^\u0000-\u007F]+\w*\b";
                foreach (var value in Abstract)
                {
                    MatchCollection matches = Regex.Matches(value, pattern);
                    count += matches.Count;
                }
                return count;
            }
            /// <summary>
            /// This function updates the Word and Sentence lists based on the Abstract content
            /// </summary>
            private void UpdateWordAndSentence()
            {
                Word = new List<string>();
                Sentence = new List<string>();
                foreach (var paragraph in Abstract)
                {
                    // Spilt Sentences by ".", "!", "?"
                    var sentences = Regex.Split(paragraph, @"(?<=[\.!\?])\s+");
                    Sentence.AddRange(sentences);

                    // Spilt Word by "\W" (and "\w" including numbers and "_"
                    foreach (var sentence in sentences)
                    {
                        var words = Regex.Split(sentence, @"\W+");
                        Word.AddRange(words.Where(w => !string.IsNullOrEmpty(w)));
                    }
                }
            }
            /// <summary>
            /// This function use for calculate word list
            /// </summary>
            private void UpdateWordTable()
            {
                WordFrequency = new Dictionary<string, int>();
                foreach (string word in Word)
                {
                    string lowerWord = word.ToLower();
                    if (WordFrequency.ContainsKey(lowerWord))
                    {
                        WordFrequency[lowerWord]++;
                    }
                    else
                    {
                        WordFrequency.Add(lowerWord, 1);
                    }
                }
            }
            public bool GetKeywordFreq(int rank, out string keyword, out int freq)
            {
                bool result = false;
                keyword = null;
                freq = 0;
                if ((WordFrequency == null) || (rank == 0 ))
                {  
                    return result;
                }

                if (rank > 0 && rank <= WordFrequency.Count)
                {
                    var sortedWords = WordFrequency.OrderByDescending(w => w.Value).ToList();
                    int targetFrequency = sortedWords[rank - 1].Value;

                    var wordsWithSameFrequency = sortedWords
               .Where(w => w.Value == targetFrequency)
               .Select(w => w.Key)
               .ToList();

                    string wordsList = string.Join(", ", wordsWithSameFrequency);
                    keyword = wordsList;
                    freq = targetFrequency;
                    result = true;
                }
                return result;
            }
            public List<string> GetKeywords(int rink, bool stopword = false)
            {
                var wordsDict = Utils.KeywordExtractor.ExtractTokenToDict(Word,false, stopword);
                List<string> keywords = wordsDict.Keys.ToList();
                return keywords.Take(rink).ToList();
            }
        }

        #endregion Content struct ...

        #endregion Struct ....

        #region Private variables
        private List<SearchBooks> bookDataBase = new List<SearchBooks>();
        private List<FileBooks> filebooks = new List<FileBooks>();
        private SearchWordArg searchkey = new SearchWordArg();
        private DatabaseHelper dbHelper = new DatabaseHelper();

        #endregion

        public void DatabaseInitialize()
        {
            dbHelper = new DatabaseHelper();
            dbHelper.InitializeDatabase();
        }
        public void GetSearchBookDB()
        {
            List<SearchBooks> books = dbHelper.GetBooks();
            foreach (var book in books)
            {
                AddFileList(book.Path);
            }
            SetEventLoadFiles();

            SetEventUpdateSerchBook(books);
        }

        public void ClearFileList()
        {
            filebooks = new List<FileBooks>();
        }
        public void AddFileList(string value)
        {
            FileBooks file = new FileBooks();
            file.File = value;
            file.CheckedState = false;
            filebooks.Add(file);
        }
        public void UpdateFileState(string name, bool check)
        {
            foreach (var list in filebooks)
            {
                if (Path.GetFileNameWithoutExtension(list.File) == name)
                {
                    list.CheckedState = check;
                    break;
                }
            }
        }
        public void GetFileList(out List<string> xmlfiles, out List<string> jsonfiles)
        {
            xmlfiles = new List<string>();
            jsonfiles = new List<string>();

            foreach (var value in filebooks)
            {
                var type = Path.GetExtension(value.File);
                var name = Path.GetFileNameWithoutExtension(value.File);
                if (string.Equals(type, ".xml")) { xmlfiles.Add(name); }
                if (string.Equals(type, ".json")) { jsonfiles.Add(name); }
            }
        }

        public bool IsSerachFileVailded()
        {
            return (filebooks.Count > 0);
        }
        public void GetSearchFileList(out List<string> searchfiles)
        {
            List<string> pathlist = new List<string>();
            foreach (var value in filebooks)
            {
                if (value.CheckedState)
                {
                    pathlist.Add(value.File);
                }
            }
            searchfiles = pathlist;
            searchfiles.Sort();
        }
        private string GetRelativePath(string basePath, string fullPath)
        {
            Uri baseUri = new Uri(basePath);
            Uri fileUri = new Uri(fullPath);
            Uri relativeUri = baseUri.MakeRelativeUri(fileUri);
            return relativeUri.ToString().Replace('/', '\\');
        }


            #region Serach Book feature
         public void CleanSearchBooks()
        {
            bookDataBase.Clear();
        }
        public bool RemoveSearchBooks(SearchBooks book)
        {
            bool result = false;
            for (int i = 0; i < bookDataBase.Count; i++)
            {
                if (bookDataBase[i].Path == book.Path)
                {
                    bookDataBase.RemoveAt(i);
                    result = true;
                    break;
                }
            }
            return result;
        }
        public List<SearchBooks> GetSerachBooks()
        {
            return bookDataBase;
        }
        public FileContent GetSearchBookContent(string filename)
        {
            FileContent content = new FileContent();
            foreach (var file in bookDataBase)
            {
                if (Path.GetFileNameWithoutExtension(file.Path) == filename)
                {
                    content = file.Content;
                }
            }
            return content;
        }

        #endregion Serach Book feature
        #region Search feature...
        public void ResetSerchKeyword()
        {
            searchkey = new SearchWordArg();
        }
        public void SetSearchWords(SearchWordArg searchwords)
        {
            this.searchkey = searchwords;
        }
        public void SetSearchKeyword(List<string> searchword)
        {
            searchkey.SearchWords = searchword;
        }
        public void SetSearchKeyword(SearchMode mode)
        {
            searchkey.Mode = mode;
        }
        public SearchWordArg GetSearchKeyword()
        {
            return searchkey;
        }
        #endregion Search feature...
    }


}
