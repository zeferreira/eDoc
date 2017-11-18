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

        private string url;

        public string Url
        {
            get { return url; }
            set
            {
                url = value;
            }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
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
            return Word.GetHashValue(file);
        }

        public Hashtable GetPostingListClass()
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
                int wordTmpHashValue = Word.GetHashValue(wordTmp);

                //get frequency for each document word
                if (postingList.ContainsKey(wordTmpHashValue))
                {
                    WordOccurrenceNode node = postingList[wordTmpHashValue] as WordOccurrenceNode;

                    WordHit newhit = new WordHit();
                    newhit.Position = i;
                    node.Hits.Add(newhit);
                }
                else if(!string.IsNullOrEmpty(wordTmp))
                {
                    WordOccurrenceNode newNode = new WordOccurrenceNode();
                    newNode.Word = new Word();
                    newNode.Word.WordID = wordTmpHashValue;
                    newNode.Word.Text = wordTmp;

                    newNode.Doc = this;

                    WordHit newhit = new WordHit();
                    newhit.Position = i;
                    //define frequency
                    newNode.Hits.Add(newhit);

                    postingList.Add(wordTmpHashValue, newNode);
                }
            }

            GC.ReRegisterForFinalize(text);
            GC.ReRegisterForFinalize(splitWords);
            GC.Collect();

            return postingList;
        }

        public Hashtable GetBytePostingListClass()
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

                //get frequency for each document word
                if (postingList.ContainsKey(wordTmp.GetHashCode()))
                {
                    WordOccurrenceNode node = postingList[wordTmp.GetHashCode()] as WordOccurrenceNode;

                    WordHit newhit = new WordHit();
                    newhit.Position = i;
                    node.Hits.Add(newhit);
                }
                else if (!string.IsNullOrEmpty(wordTmp))
                {
                    WordOccurrenceNode newNode = new WordOccurrenceNode();
                    newNode.Word = new Word();
                    newNode.Word.WordID = wordTmp.GetHashCode();
                    newNode.Word.Text = wordTmp;

                    newNode.Doc = this;

                    WordHit newhit = new WordHit();
                    newhit.Position = i;
                    //define frequency
                    newNode.Hits.Add(newhit);

                    postingList.Add(wordTmp.GetHashCode(), newNode);
                }
            }

            GC.ReRegisterForFinalize(text);
            GC.ReRegisterForFinalize(splitWords);
            GC.Collect();

            return postingList;
        }
        

    }
}
