using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;
using Microsoft.Office.Tools.Excel;

namespace PDF_Combiner
{
    public partial class pdfCombiner
    {
        private static pdfPane pane;
        private static CustomTaskPane taskPane;

        public static pdfPane Pane { get => pane; }
        public static CustomTaskPane TaskPane { get => taskPane; }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            pane = new pdfPane();
            taskPane = this.CustomTaskPanes.Add(pane, "Merge PDFs");
            taskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionRight;
            taskPane.Width = 300;
        }
        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e) {
            taskPane = null;
            pane = null;
        }

        #endregion
    }
}
