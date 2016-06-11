using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DocCore
{
    public class LexiconHashtable : ILexicon
    {
        Hashtable ht;
        private long maxLength;

        public long MaxLength 
        { 
            get 
            {
                return this.maxLength;   
            } 
        }

        public long Quantity
        {
            get { return ht.Count; }
        }

        //implements singleton pattern
        private static LexiconHashtable instance = null;
        private static readonly object padlock = new object();

        public static LexiconHashtable Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new LexiconHashtable();
                    }
                    return instance;
                }
            }
        }

        LexiconHashtable()
        {
            this.ht = new Hashtable();
            this.maxLength = int.MaxValue;
        }

        //public Word GetWord(string word)
        //{
        //    return ht[word.GetHashCode()] as Word;
        //}

        public Word GetWord(ref int wordID)
        {
            return ht[wordID] as Word;
        }

        void Add(Word word)
        {
            this.ht.Add(word.WordID, word);
        }

        //public bool HasWord(string word)
        //{
        //    if(this.ht.ContainsKey(Word.GetHashValue(word)))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        public bool HasWord(ref int wordID)
        {
            if (this.ht.ContainsKey(wordID))
            {
                return true;
            }

            return false;
        }

        public void AddWordOccurrence(WordOccurrenceNode newNode)
        {
            //Do not has any word
            if (!HasWord(ref newNode.Word.WordID ))
            {
                newNode.Word.FirstOccurrence = newNode;
                newNode.Word.LastOccurrence = newNode;
                newNode.Word.Quantity = newNode.Hits.Count;
                this.Add(newNode.Word);
            }
            else
            {
                //to do: memory allocation alert!! remove ref!! use newnode.Word.text or something, don't pass WordID as a parameter!!
                newNode.Word = this.GetWord(ref newNode.Word.WordID);

                newNode.PreviousOccurrence = newNode.Word.LastOccurrence;
                newNode.Word.LastOccurrence.NextOccurrence = newNode;
                newNode.Word.LastOccurrence = newNode;
                newNode.Word.Quantity += newNode.Hits.Count;
            }
        }

    }
}
