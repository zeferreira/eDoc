using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    class Indexer
    {
        private long wordQuantity;
        private long docQuantity;

        private Lexicon lexicon;
        private IRepositoryDocument repDoc;

        public Indexer()
        {
            this.lexicon = new Lexicon();
            this.repDoc = FactoryRepositoryDocument.GetRepositoryDocument(EnumRepositoryType.TXT);
        }

        public void StartIndexing()
        {
            List<Document> listOfDocs = repDoc.Search(false);

            Index(listOfDocs);
        }

        public void Load()
        {
            List<Document> listOfDocs = repDoc.Search(true);

            Index(listOfDocs);
        }

        public Word Search(string word)
        {
            return this.lexicon.GetWord(word);
        }

        void Index(List<Document> listOfDocs)
        {
            foreach (Document item in listOfDocs)
            {
                string text = item.GetText();

                string[] splitWords = text.Split(' ');

                for (int i = 0; i < splitWords.Length; i++)
                {
                    string wordTmp = SentenceParser.GetCleanSentence(splitWords[i]);
                    wordTmp = wordTmp.Replace(" ", string.Empty);

                    WordOccurrenceNode newNode;

                    if (this.lexicon.HasWord(wordTmp))
                    {
                        newNode = this.lexicon.GetOccurrenceByDoc(item,this.lexicon.GetWord(wordTmp));

                        if (newNode == null)
                        {
                            newNode = new WordOccurrenceNode();
                            newNode.Word = this.lexicon.GetWord(wordTmp);
                            newNode.Doc = item;

                            WordHit newhit = new WordHit();
                            newhit.Position = i;
                            //define frequency
                            newNode.Hits.Add(newhit);
                        }
                        else
                        {
                            WordHit newhit = new WordHit();
                            newhit.Position = i;
                            //define frequency
                            newNode.Hits.Add(newhit);
                        }
                    }
                    else
                    {
                        newNode = new WordOccurrenceNode();
                        newNode.Word = new Word();
                        newNode.Word.Text = wordTmp;

                        newNode.Doc = item;

                        WordHit newhit = new WordHit();
                        newhit.Position = i;
                        //define frequency
                        newNode.Hits.Add(newhit);
                        item.WordQuantity = splitWords.Length + 1;
                        newNode.Word.FirstOccurrence = newNode;
                        //add occurrence
                        lexicon.AddWordOccurrence(newNode);
                    }
                }
            }
        }



    }
}
