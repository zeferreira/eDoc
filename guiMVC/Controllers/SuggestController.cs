using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Configuration;
using System.Text;
using System.IO;
using guiMVC.Models;
using System.ComponentModel.DataAnnotations;
using CrawlerCore;
using DocCore;
using CrawlerCore;


namespace guiMVC.Controllers
{
    public class SuggestController : Controller
    {
        IEngine eng;
        EngineConfiguration engConf;
        int resultPageSize = 10;
        //
        // GET: /Suggest/
        public ActionResult Index()
        {

                return View();
        }

        //
        // Post: /Suggest/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection collection)
        {
            IRepositoryDocumentCandidate repCandidate = FactoryRepositoryDocumentCandidate.GetRepositoryDocumentCandidate();

            string originalUrl = Request.Form["OriginalUrl"];

            DocumentCandidate docCandidate = new DocumentCandidate(originalUrl);
            if (docCandidate.HasValidUrl())
            {
                repCandidate.Insert(docCandidate);
                TempData["success"] = "Your Url ( " + docCandidate.OriginalUrl + " ) will be indexed in the future ;)";
            }
            else
            {
                ModelState.AddModelError("", "Invalid Url( " + docCandidate.OriginalUrl + ")");
            }

            return View();
        }


    }
}
