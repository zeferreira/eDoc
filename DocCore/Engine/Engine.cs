using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class Engine
    {
        private Indexer indexer;
        private EngineConfiguration engineConfiguration;
        private IRepositoryLog logRep;

        public Engine()
        {
            this.engineConfiguration = new EngineConfiguration();
            this.logRep = FactoryRepositoryLog.GetRepositoryLog();
        }

        public List<Document> Search(string sentence)
        {
            int maxSentence = engineConfiguration.MaxSentence;

            List<Document> resultList = new List<Document>();
            List<Word> wordFound = new List<Word>();

            string sentenceClear = SentenceParser.GetCleanSentence(sentence);

            string[] splitWords = sentenceClear.Split(' ');

            for (int i = 0; ((i < splitWords.Length) && (i < maxSentence)); i++)
            {
                Word wf = indexer.Search(splitWords[i]);
                
                if(wf != null)
                    wordFound.Add(wf);
            }

            foreach (Word item in wordFound)
            {
                WordOccurrenceNode firstOcc = item.FirstOccurrence;
                resultList.Add(firstOcc.Doc);
                
                WordOccurrenceNode tmp = firstOcc;

                while (tmp.HasNext())
                {
                    tmp = tmp.NextOccurrence;
                    resultList.Add(tmp.Doc);
                }
            }

            return resultList;
        }

        public void Load()
        {
            this.indexer = new Indexer();

            this.indexer.Load();
        }

        

    }
}
