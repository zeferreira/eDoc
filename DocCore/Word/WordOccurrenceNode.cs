﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    public class WordOccurrenceNode
    {
        //private float frequency;
        private float rank;
        //used to order the list
        public float Rank
        {
            get 
            {
                this.Rank = Frequency;
                return rank; 
            }
            set 
            { 
                rank = value; 
            }
        }
        

        public float Frequency
        {
            get {
                
                return hits.Count / doc.WordQuantity; 
                
            }
        }

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
            set { hits = value; }
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
                if (newNode.Rank > this.NextOccurrence.Rank)
                {
                    newNode.NextOccurrence = this.NextOccurrence;
                    this.NextOccurrence.PreviousOccurrence = newNode;
                }
                else 
                {
                    this.NextOccurrence.Add(newNode);
                }

            }
            else
            {
                this.NextOccurrence = newNode;
                newNode.PreviousOccurrence = this;
            }
        }
    }
}