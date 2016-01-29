using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace DocCore
{
    public class Document
    {
        private long docID;

        public long DocID
        {
            get { return docID; }
            set { docID = value; }
        }
        
        //private string description;
        //private string referenceName;
        //private DateTime creationDate;

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        
        private string file;

        public string File
        {
            get { return file; }
            set 
            {
                file = value;
                this.DocID = this.GetHashCode();    
            }
        }

        private int wordQuantity;

        public int WordQuantity
        {
            get { return wordQuantity; }
            set { wordQuantity = value; }
        }

        private IDocParser parser;

        public string GetText()
        {
            this.parser = FactoryParser.GetParser(File);
            string text = Util.RemoveForbbidenSymbols( this.parser.GetText(file));

            return text.ToLower(); ;
        }

        public override int GetHashCode()
        {
            return file.GetHashCode() ;
        }

        public Hashtable GetPostingList()
        {
            Hashtable postingList = new Hashtable();

            string text = this.GetText();

            string[] splitWords = text.Split(' ');
            this.WordQuantity = splitWords.Length + 1;

            //index words
            for (int i = 0; i < splitWords.Length; i++)
            {
                string wordTmp = SentenceParser.GetCleanSentence(splitWords[i]);
                wordTmp = wordTmp.Replace(" ", string.Empty);

                int key = wordTmp.GetHashCode();

                //get frequency for eac document word
                if (postingList.ContainsKey(key))
                {
                    WordOccurrenceNode node = postingList[key] as WordOccurrenceNode;

                    WordHit newhit = new WordHit();
                    newhit.Position = i;
                    node.Hits.Add(newhit);
                }
                else
                {
                    WordOccurrenceNode newNode = new WordOccurrenceNode();
                    newNode.Word = new Word();
                    newNode.Word.Text = wordTmp;

                    newNode.Doc = this;

                    WordHit newhit = new WordHit();
                    newhit.Position = i;
                    //define frequency
                    newNode.Hits.Add(newhit);

                    postingList.Add(key, newNode);
                }
            }

            return postingList;
        }
    }
}
