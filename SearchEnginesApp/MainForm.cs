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
        private readonly SerchEnginePresenter _SerchEnginePresenter;


        public MainForm()
        {
            InitializeComponent();
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            Text = $"SearchEngines {versionInfo.FileVersion}";

            _SerchEnginePresenter = new SerchEnginePresenter(_toolModel);
            panelView.Controls.Add(_SerchEnginePresenter.ShowView());

        }
 

    }
}
