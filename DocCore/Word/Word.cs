using System;
using System.Collections.Generic;
using System.Text;

namespace DocCore
{
    /// <summary>
    /// Word is any thing between spaces
    /// </summary>
    public class Word
    {
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        //private long id;
        private int quant;

        private WordOccurrenceNode firstOccurrence;

        public WordOccurrenceNode FirstOccurrence
        {
            get { return firstOccurrence; }
            set { firstOccurrence = value; }
        }

        public override int GetHashCode()
        {
            return text.GetHashCode();
        }
    }
}

/// quanto mais uma palavra aparece em documentos distintos, menos relevante ela é para a pesquisa.
/// Exemplo: artigos: a, o, etc....