using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PDFzeExtractor
{

    /**
     * Extracts text from a PDF file.
     * @since   2.1.4
     */
    public static class ZePdfTextExtractor
    {

        /**
         * Extract text from a specified page using an extraction strategy.
         * Also allows registration of custom ContentOperators
         * @param reader the reader to extract text from
         * @param pageNumber the page to extract text from
         * @param strategy the strategy to use for extracting text
         * @param additionalContentOperators an optional dictionary of custom IContentOperators for rendering instructions
         * @return the extracted text
         * @throws IOException if any operation fails while reading from the provided PdfReader
         */
        public static String GetTextFromPage(PdfReader reader, int pageNumber, ITextExtractionStrategy strategy, IDictionary<string, IContentOperator> additionalContentOperators)
        {
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            return parser.ProcessContent(pageNumber, strategy, additionalContentOperators).GetResultantText();

        }

        /**
         * Extract text from a specified page using an extraction strategy.
         * @param reader the reader to extract text from
         * @param pageNumber the page to extract text from
         * @param strategy the strategy to use for extracting text
         * @return the extracted text
         * @throws IOException if any operation fails while reading from the provided PdfReader
         * @since 5.0.2
         */
        public static String GetTextFromPage(PdfReader reader, int pageNumber, ITextExtractionStrategy strategy)
        {
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            return parser.ProcessContent(pageNumber, strategy, new Dictionary<string, IContentOperator>()).GetResultantText();

        }

        public static List<ZeChunkFontSize> GetTextFromPage(PdfReader reader, int pageNumber, ZeITextExtractionStrategy strategy)
        {
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            return parser.ProcessContent(pageNumber, strategy, new Dictionary<string, IContentOperator>()).GetResultantTextChunks();

        }

        /**
         * Extract text from a specified page using the default strategy.
         * <p><strong>Note:</strong> the default strategy is subject to change.  If using a specific strategy
         * is important, use {@link PdfTextExtractor#getTextFromPage(PdfReader, int, TextExtractionStrategy)}
         * @param reader the reader to extract text from
         * @param pageNumber the page to extract text from
         * @return the extracted text
         * @throws IOException if any operation fails while reading from the provided PdfReader
         * @since 5.0.2
         */
        public static String GetTextFromPage(PdfReader reader, int pageNumber)
        {
            return GetTextFromPage(reader, pageNumber, new LocationTextExtractionStrategy());
        }

    }
}
