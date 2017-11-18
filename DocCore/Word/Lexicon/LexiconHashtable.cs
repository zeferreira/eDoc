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

        public void AddNewWord(Word word)
        {
            this.ht.Add(word.WordID, word);
        }

        public bool HasWord(ref int wordID)
        {
            if (this.ht.ContainsKey(wordID))
            {
                return true;
            }

            return false;
        }

        public void WriteToStorage()
        {
            
        }

        public void LoadFromStorage()
        {
            
        }
    }
}
