using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocCore;
using System.Diagnostics;

namespace WebGuiTest
{
    public partial class About : System.Web.UI.Page
    {
        Indexer index;
        ILexicon lexicon;

        protected void Page_Load(object sender, EventArgs e)
        {
            index = Indexer.Instance;

            Process currentProc = Process.GetCurrentProcess();

            long memoryUsed = currentProc.PrivateMemorySize64;

            this.lblMemory.Text = "Memory: " + Useful.GetFormatedSizeString(memoryUsed);
            this.lblFiles.Text = "Indexed Files: " + index.TotalDocumentQuantity;
            this.lblWords.Text = "Total Word Quantity: " + index.TotalWordQuantity;
        }
    }
}