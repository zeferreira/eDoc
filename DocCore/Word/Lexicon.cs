using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DocCore
{
    public class Lexicon
    {
        Hashtable ht;

        public long Quantity
        {
            get { return ht.Count; }
        }

        public Lexicon()
        {
            this.ht = new Hashtable();
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
                this.Add(newNode.Word);
            }
            else
            {
                Word actualWord = this.GetWord(newNode.Word.Text);
                WordOccurrenceNode firstNode = actualWord.FirstOccurrence;

                WordOccurrenceNode hasDocWithWord = this.GetOccurrenceByDoc(newNode.Doc, actualWord);

                if (firstNode.Doc.GetHashCode() == newNode.Doc.GetHashCode())
                {
                    firstNode.Hits.Add(newNode.Hits[0]);
                }
                else
                {
                    if (firstNode.Rank < newNode.Rank)
                    {
                        actualWord.FirstOccurrence = newNode;
                        firstNode.PreviousOccurrence = newNode;
                        newNode.NextOccurrence = firstNode;
                    }
                    else
                    {
                        firstNode.Add(newNode);
                    }
                }
            }

        }

        public WordOccurrenceNode GetOccurrenceByDoc(Document doc, Word word)
        {
            WordOccurrenceNode tmpOccurrence;
            WordOccurrenceNode firstOccurrence = word.FirstOccurrence;

            do
            {
                tmpOccurrence = firstOccurrence;

                if (tmpOccurrence.Doc.GetHashCode() == doc.GetHashCode())
                {
                    return tmpOccurrence;
                }

            } while (tmpOccurrence.HasNext());

            return null;
        }
    }
}
