using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DocCore
{
    public class Lexicon
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

        public Lexicon()
        {
            this.ht = new Hashtable();
            this.maxLength = int.MaxValue;
        }

        public Word GetWord(string word)
        {
            return ht[word.GetHashCode()] as Word;
        }

        void Add(Word word)
        {
            this.ht.Add(word.GetHashCode(), word);
        }

        public bool HasWord(string word)
        {
            if(this.ht.ContainsKey(word.GetHashCode()))
            {
                return true;
            }

            return false;
        }

        public void AddWordOccurrence(WordOccurrenceNode newNode)
        {
            //Do not has any word
            if (!HasWord(newNode.Word.Text))
            {
                newNode.Word.FirstOccurrence = newNode;
                newNode.Word.LastOccurrence = newNode;
                newNode.Word.Quantity = newNode.Hits.Count;
                this.Add(newNode.Word);
            }
            else
            {
                newNode.Word = this.GetWord(newNode.Word.Text);

                newNode.PreviousOccurrence = newNode.Word.LastOccurrence;
                newNode.Word.LastOccurrence.NextOccurrence = newNode;
                newNode.Word.LastOccurrence = newNode;
                newNode.Word.Quantity += newNode.Hits.Count;
            }

        }

    }
}
