using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace DocCore
{
    public interface IInvertedFile
    {
        void AddWordOccurrence(WordOccurrenceNode wordOccur);
        List<WordOccurrenceNode> GetWordOccurrencies(Word word);
        void WriteToStorage();
    }
}
