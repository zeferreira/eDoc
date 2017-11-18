using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;

namespace PDFzeExtractor
{
    public interface ZeITextExtractionStrategy : IRenderListener
    {
        /**
         * Returns the result so far.
         * @return  a String with the resulting text.
         */
        List<ZeChunkFontSize> GetResultantTextChunks();
        String GetResultantText();
    }
}
