using SearchEnginesApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchEnginesApp.Presenters
{
    public class SerchEnginePresenter
    {
        private readonly ToolModel _toolModel;
        private SerchEngineView View { get; set; }

        /// <summary>
        /// SerchEngine Presenter
        /// </summary>
        /// <param name="toolModel">The Tool Model</param>
        public SerchEnginePresenter(ToolModel toolModel)
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
                View = new SerchEngineView(this);
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


    }




}
