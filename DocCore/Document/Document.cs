using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

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
    }
}
