using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class WordOccurrenceNode
    {
        private Word word;

        public Word Word
        {
            get { return word; }
            set { word = value; }
        }

        private Document doc;

        public Document Doc
        {
            get { return doc; }
            set { doc = value; }
        }

        //private float frequency;
        private double rank;
        //used to order the list
        public double Rank
        {
            get 
            {
                return rank; 
            }
            set 
            { 
                rank = value; 
            }
        }

        public double Frequency
        {
            get {
                
                return ((double) hits.Count) / ((double)doc.WordQuantity); 
            }
        }

        private List<WordHit> hits;

        public List<WordHit> Hits
        {
            get 
            {
                if (this.hits == null)
                {
                    this.hits = new List<WordHit>();
                }
                return hits; 
            }
            set 
            { 
                hits = value;
                
            }
        }

        private int quantityHits;

        public int QuantityHits
        {
            get 
            {
                this.quantityHits = hits.Count;
                return quantityHits; 
            }
            set { quantityHits = value; }
        }

        private WordOccurrenceNode nextOccurrence;

        public WordOccurrenceNode NextOccurrence
        {
            get { return nextOccurrence; }
            set { nextOccurrence = value; }
        }

        private WordOccurrenceNode previousOccurrence;

        public WordOccurrenceNode PreviousOccurrence
        {
            get { return previousOccurrence; }
            set { previousOccurrence = value; }
        }

        public bool HasNext()
        {
            if (this.nextOccurrence == null)
                return false;
            else
                return true;
        }

        public bool HasPrevious()
        {
            if (this.previousOccurrence == null)
                return false;
            else
                return true;
        }

        public void Add(WordOccurrenceNode newNode)
        {
            if (this.HasNext())
            {
                this.NextOccurrence.Add(newNode);
            }
            else
            {
                this.NextOccurrence = newNode;
                newNode.PreviousOccurrence = this;
            }
        }
    }
}
