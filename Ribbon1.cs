using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDF_Combiner;

namespace PDF_Combiner
{
    public partial class Ribbon1
    {

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void btnCombinePDFs_Click(object sender, RibbonControlEventArgs e)
        {
           pdfCombiner.TaskPane.Visible = true;
        }
    }
}
