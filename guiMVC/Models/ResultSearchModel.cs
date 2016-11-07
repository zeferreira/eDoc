using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocCore;

namespace guiMVC.Models
{
    public class ResultSearchModel
    {
        public List<DocumentResult> results;
        public string query;
        public int start;
    }
}