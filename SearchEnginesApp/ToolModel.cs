using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static SearchEnginesApp.ToolModel;
using static SearchEnginesApp.Views.SearchEngineResultView;

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
        public class KeywordArg
        {
            public SearchMode Mode { get; set; }
            public List<string> Keywords { get; set; }
            public KeywordArg()
            {
                Mode = SearchMode.Others;
                Keywords = new List<string>();
            }
        }


        #region Event Handler...
        #region Event List..
        public event EventHandler<FileEventArgs> FilesLoaded;
        public event EventHandler<SearchDataEventArgs> SearchResultLoaded;
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

        #region Search Data Event...
        public void SetEventUpdateSerchBook()
        {
            OnGetSearchData(new SearchDataEventArgs(bookDataBase,keywordarg));
        }
        protected virtual void OnGetSearchData(SearchDataEventArgs e)
        {
            SearchResultLoaded?.Invoke(this, e);
        }
        public class SearchDataEventArgs : EventArgs
        {
            public List<SearchBooks> SearchData { get; }
            public KeywordArg Keywordarg { get; }

            public SearchDataEventArgs(List<SearchBooks> books, KeywordArg keywordarg)
            {
                SearchData = books;
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


        }
        #endregion Testboox


        #region Content struct ...
        /// - Content Struct
        ///   - Content
        ///   - Words
        ///   - Sentence
        public class FileContent
        {
            public List<string> Content { get; set; }
            public List<string> Word { get; set; }
            public List<string> Sentence { get; set; }

            public FileContent()
            {
                Content = new List<string>();
                Word = new List<string>();
                Sentence = new List<string>();
            }
            public int WordsCount()
            {
                return Word.Count;
            }
            public int SentenceCount()
            {
                return Sentence.Count;
            }
            public int ContentCount()
            {
                int count = 0;
                foreach (var value in Content)
                {
                    count+= value.Count();
                }
                return count;
            }
            public int ContentCountNoSpace()
            {
                int count = 0;
                foreach (var value in Content)
                {
                    count += value.Replace(" ", "").Count();
                }
                return count;
            }
            public int NonAsciiChar()
            {
                int count = 0;
                // Set Non ASCII Char Pattern
                string pattern = @"[^\u0000-\u007F]";
                foreach (var value in Content)
                {
                    MatchCollection matches = Regex.Matches(value, pattern);
                    count += matches.Count;
#if (DEBUG)
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss.ss")} : DEBUG_MSG : NonAsciiChar : {matches.Count}");
#endif
                }
                return count;
            }
            public int NonAsciiWord()
            {
                int count = 0;
                // Set Non ASCII Word Pattern
                string pattern = @"\b\w*[^\u0000-\u007F]+\w*\b";
                foreach (var value in Content)
                {
                    MatchCollection matches = Regex.Matches(value, pattern);
                    count += matches.Count;
#if (DEBUG)
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss.ss")} : DEBUG_MSG : NonAsciiWord : {matches.Count}");
#endif
                }
                return count;
            }

        }

        #endregion Content struct ...

        #endregion Struct ....

        #region Private variables
        private List<SearchBooks> bookDataBase = new List<SearchBooks>();
        private List<FileBooks> filebooks = new List<FileBooks>();
        private KeywordArg keywordarg = new KeywordArg();

        #endregion

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
        public void UpdateFileState(string value, bool check)
        {
            foreach (var list in filebooks)
            {
                if (Path.GetFileNameWithoutExtension(list.File) == value)
                {
                    list.CheckedState = check;
#if (DEBUG)
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss.ss")} : DEBUG_MSG : Filebooks Update [{Path.GetFileName(list.File)}] State[{list.CheckedState.ToString()}]");
#endif
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
                if (string.Equals(type, ".json")){ jsonfiles.Add(name); }
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


        #region Serach Book feature

        public void CleanSearchBooks()
        {
            bookDataBase.Clear();
        }
        public void AddSearchBooks(SearchBooks book)
        {
            bookDataBase.Add(book);
#if (DEBUG)
            if(bookDataBase.Count >0)
            {
                int idx = bookDataBase.Count - 1;
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss.ss")} : DEBUG_MSG : bookDataBase [{Path.GetFileName(bookDataBase[idx].Path)}] Book [{Path.GetFileName(book.Path)}] ");
            }

#endif


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

        public void ResetSerchKeyword()
        {
            keywordarg = new KeywordArg();
        }
        public void SetSearchKeyword(KeywordArg keyword)
        {
            keywordarg = keyword;
        }
        public void SetSearchKeyword(List<string> keyword)
        {
            keywordarg.Keywords = keyword;
        }
        public void SetSearchKeyword(SearchMode mode)
        {
            keywordarg.Mode = mode;
        }
        public KeywordArg GetSearchKeyword()
        {
            return keywordarg;
        }
        #endregion Serach Book feature

    }


}
