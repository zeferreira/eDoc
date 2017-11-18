using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace DocCore
{
    /// <summary>
    /// This code is bad. Have memory allocation activities that does not need but
    /// the constructor is a better version of Singleton.
    /// </summary>

    public class InvertedFileMemory : IInvertedFile
    {
        EngineConfiguration conf;
        IRepositoryDocument docIndex;
        ILexicon lexicon;
        int numberOfPages = 0;

        private Hashtable dictionaryTemp;

        string finalInvertedfileName;

        private InvertedFileMemory()
        {
            this.finalInvertedfileName = string.Empty;
            this.docIndex = FactoryRepositoryDocument.GetRepositoryDocument();
            this.lexicon = FactoryLexicon.GetLexicon();
            this.dictionaryTemp = new Hashtable();
        }

        public static InvertedFileMemory Instance { get { return Nested.Instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly InvertedFileMemory Instance = new InvertedFileMemory();
        }

        public void AddWordOccurrence(WordOccurrenceNode wordOccur)
        {
            Page currentPage;
            Page firstPage;

            try
            {
                lock (this)
                {
                    if (dictionaryTemp.Contains(wordOccur.Word.WordID))
                    {
                        currentPage = ((Page)dictionaryTemp[wordOccur.Word.WordID]).LastPage;
                        firstPage = (Page)dictionaryTemp[wordOccur.Word.WordID];

                        if (currentPage.IsFullForOccurrence(wordOccur))
                        {
                            currentPage.NextPage = new Page();
                            firstPage.LastPage = currentPage.NextPage;
                            this.numberOfPages++;
                            currentPage = currentPage.NextPage;
                        }
                    }
                    else
                    {
                        currentPage = new Page();
                        numberOfPages++;
                        dictionaryTemp[wordOccur.Word.WordID] = currentPage;
                        currentPage.LastPage = currentPage;
                        currentPage.NextPage = null;
                    }

                    currentPage.Write(wordOccur);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("\n Cannot possible to create or write Memory Page." + e.Message);
                return;
            }
        }
        
        public List<WordOccurrenceNode> GetWordOccurrencies(Word word)
        {
            List<WordOccurrenceNode> result = new List<WordOccurrenceNode>();

            try
            {
                Page firstPage = (Page)dictionaryTemp[word.WordID];
                Page currentPage = firstPage;
                //open the file

                while ((currentPage != null) && (result.Count < conf.MaxResultList))
                {
                    List<WordOccurrenceNode> TempResult = currentPage.GetWordOccurrencies(word);

                    foreach (WordOccurrenceNode item in TempResult)
                    {
                        item.Doc = this.docIndex.Read(item.DocID);
                        result.Add(item);
                    }

                    currentPage = currentPage.NextPage;
                }

                return result;
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        public void WriteToStorage()
        {

        }

    }
}
