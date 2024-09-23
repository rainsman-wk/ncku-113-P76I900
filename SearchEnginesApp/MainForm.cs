using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Net.Http;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using SearchEnginesApp.Presenters;
using SearchEnginesApp.Views;

namespace SearchEnginesApp
{
    public partial class MainForm : Form
    {
        private readonly ToolModel _toolModel;
        private readonly SearchEngineResultPresenter _SerchEnginePresenter;
        private readonly SearchEngineTopPresenter _SerchEngineTopPresenter;
        private readonly FilesTreePresenter _FilesTreePresenter;
        private readonly List<TabPage> _tabPages = new List<TabPage>();

        
        public MainForm()
        {
            InitializeComponent();
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            Text = $"SearchEngines {versionInfo.FileVersion}";

            _toolModel = new ToolModel();

            _SerchEnginePresenter = new SearchEngineResultPresenter(_toolModel);
            tpSearchResult.Controls.Add(_SerchEnginePresenter.ShowView());

            _SerchEngineTopPresenter = new SearchEngineTopPresenter(_toolModel);
            MainTopView.Controls.Add(_SerchEngineTopPresenter.ShowView());

            _FilesTreePresenter = new FilesTreePresenter(_toolModel);
            splitContainer1.Panel1.Controls.Add(_FilesTreePresenter.ShowView());

        }



    }
}
