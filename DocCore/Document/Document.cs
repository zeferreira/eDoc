using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace DocCore
{
    public class Document
    {
        private int docID;

        public int DocID
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

        public Document()
        {

        }

        public string GetText()
        {
            this.parser = FactoryParser.GetParser(File);
            string text = Useful.RemoveForbbidenSymbols( this.parser.GetText(file)); //to do: 33% of time to read full text

            return text.ToLower(); //0% of time O.o
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
                string wordTmp = QueryParser.GetCleanQuery(splitWords[i]);
                wordTmp = wordTmp.Replace(" ", string.Empty);

                int key = wordTmp.GetHashCode();

                //get frequency for each document word
                if (postingList.ContainsKey(key))
                {
                    WordOccurrenceNode node = postingList[key] as WordOccurrenceNode;

                    WordHit newhit = new WordHit();
                    newhit.Position = i;
                    node.Hits.Add(newhit);
                }
                else if(!string.IsNullOrEmpty(wordTmp))
                {
                    WordOccurrenceNode newNode = new WordOccurrenceNode();
                    newNode.Word = new Word();
                    newNode.Word.WordID = key;
                    newNode.Word.Text = wordTmp;

                    newNode.Doc = this;

                    WordHit newhit = new WordHit();
                    newhit.Position = i;
                    //define frequency
                    newNode.Hits.Add(newhit);

                    postingList.Add(key, newNode);
                }
            }

            GC.ReRegisterForFinalize(text);
            GC.ReRegisterForFinalize(splitWords);
            GC.Collect();

            return postingList;
        }
        
    }
}
