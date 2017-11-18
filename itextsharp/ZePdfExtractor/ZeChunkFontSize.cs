using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using System.Drawing;

//http://stackoverflow.com/questions/113989/test-if-a-font-is-installed
//http://stackoverflow.com/questions/6882098/how-can-i-get-text-formatting-with-itextsharp/6884297#6884297

namespace PDFzeExtractor
{
    public class ZeChunkFontSize : PDFzeExtractor.TextChunk
    {
        public ZeChunkFontSize(String str, Vector startLocation, Vector endLocation, float charSpaceWidth) :
            base(str, new TextChunkLocationDefaultImp(startLocation, endLocation, charSpaceWidth))
        {
        }

        public ZeChunkFontSize(String str, ITextChunkLocation location) :
            base(str,location)
        {

        }

        string curFont;
        public string CurFont
        {
            get { return curFont; }
            set { curFont = value; }
        }

        Single curFontSize;
        public Single CurFontSize
        {
            get { return curFontSize; }
            set { curFontSize = value; }
        }

    }
}
