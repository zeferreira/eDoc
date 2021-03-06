﻿using System;


namespace DocCore
{
    public interface ILexicon
    {
        long MaxLength { get; }
        long Quantity { get; }
        void AddNewWord(Word word);
        //void AddWordOccurrence(WordOccurrenceNode newNode);
        //Word GetWord(string word);
        //bool HasWord(string word);
        Word GetWord(ref int wordID);
        bool HasWord(ref int wordID);
        void WriteToStorage();
        void LoadFromStorage();
    }
}
