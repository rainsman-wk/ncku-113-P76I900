﻿using SearchEnginesApp.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                    context.Abstract.AddRange(abstractNodes.Cast<XmlNode>().Select(node => node.InnerText));
                    context.Journal = articleNode.SelectSingleNode("MedlineCitation/Article/Journal/Title")?.InnerText;
                    context.Pmid = articleNode.SelectSingleNode("MedlineCitation/PMID")?.InnerText;
                }
                foreach(var text in context.Abstract)
                {
                    context.Word.AddRange(text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                    context.Sentence.AddRange(text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).ToList());
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

            for(int i = 0; i< searchlist.Count; i++)
            {
                books.Add(new SearchBooks(searchlist[i], GetXmlContent(searchlist[i])));
            }
            // Trigger Event to other view to update Serach book
            _toolModel.SetEventUpdateSerchBook(books);
        }
        public void SetSearchKeyWord(KeywordArg keyword)
        {
            _toolModel.SetSearchKeyword(keyword);
            _toolModel.SetKeywordSearchEvent();
        }
        #endregion Search Features...



    }




}
