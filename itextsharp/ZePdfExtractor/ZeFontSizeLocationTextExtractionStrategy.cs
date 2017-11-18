using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using System.Drawing;
using System.Runtime.InteropServices;
using iTextSharp.text.io;


//http://stackoverflow.com/questions/113989/test-if-a-font-is-installed
//http://stackoverflow.com/questions/6882098/how-can-i-get-text-formatting-with-itextsharp/6884297#6884297

namespace PDFzeExtractor
{
    public class ZeFontSizeLocationTextExtractionStrategy : ZeITextExtractionStrategy
    {
        /** set to true for debugging */
        public static bool DUMP_STATE = false;

        /** a summary of all found text */
        private List<TextChunk> locationalResult = new List<TextChunk>();

        private readonly ITextChunkLocationStrategy tclStrat;

        /**
         * Creates a new text extraction renderer.
         */
        public ZeFontSizeLocationTextExtractionStrategy()
            : this(new TextChunkLocationStrategyDefaultImp())
        {
        }

        /**
         * Creates a new text extraction renderer, with a custom strategy for
         * creating new TextChunkLocation objects based on the input of the
         * TextRenderInfo.
         * @param strat the custom strategy
         */
        public ZeFontSizeLocationTextExtractionStrategy(ITextChunkLocationStrategy strat)
        {
            tclStrat = strat;
        }

        /**
         * @see com.itextpdf.text.pdf.parser.RenderListener#beginTextBlock()
         */
        public virtual void BeginTextBlock()
        {
        }

        /**
         * @see com.itextpdf.text.pdf.parser.RenderListener#endTextBlock()
         */
        public virtual void EndTextBlock()
        {
        }

        /**
         * @param str
         * @return true if the string starts with a space character, false if the string is empty or starts with a non-space character
         */
        private bool StartsWithSpace(String str)
        {
            if (str.Length == 0) return false;
            return str[0] == ' ';
        }

        /**
         * @param str
         * @return true if the string ends with a space character, false if the string is empty or ends with a non-space character
         */
        private bool EndsWithSpace(String str)
        {
            if (str.Length == 0) return false;
            return str[str.Length - 1] == ' ';
        }

        /**
         * Filters the provided list with the provided filter
         * @param textChunks a list of all TextChunks that this strategy found during processing
         * @param filter the filter to apply.  If null, filtering will be skipped.
         * @return the filtered list
         * @since 5.3.3
         */
        private List<TextChunk> filterTextChunks(List<TextChunk> textChunks, ITextChunkFilter filter)
        {
            if (filter == null)
            {
                return textChunks;
            }

            List<TextChunk> filtered = new List<TextChunk>();

            foreach (TextChunk textChunk in textChunks)
            {
                if (filter.Accept(textChunk))
                {
                    filtered.Add(textChunk);
                }
            }

            return filtered;
        }

        /**
         * Determines if a space character should be inserted between a previous chunk and the current chunk.
         * This method is exposed as a callback so subclasses can fine time the algorithm for determining whether a space should be inserted or not.
         * By default, this method will insert a space if the there is a gap of more than half the font space character width between the end of the
         * previous chunk and the beginning of the current chunk.  It will also indicate that a space is needed if the starting point of the new chunk 
         * appears *before* the end of the previous chunk (i.e. overlapping text).
         * @param chunk the new chunk being evaluated
         * @param previousChunk the chunk that appeared immediately before the current chunk
         * @return true if the two chunks represent different words (i.e. should have a space between them).  False otherwise.
         */
        protected virtual bool IsChunkAtWordBoundary(TextChunk chunk, TextChunk previousChunk)
        {
            return chunk.Location.IsAtWordBoundary(previousChunk.Location);
        }

        /**
         * 
         * @see com.itextpdf.text.pdf.parser.RenderListener#renderText(com.itextpdf.text.pdf.parser.TextRenderInfo)
         */
        public virtual void RenderText(TextRenderInfo renderInfo)
        {
            LineSegment segment = renderInfo.GetBaseline();
            if (renderInfo.GetRise() != 0)
            { // remove the rise from the baseline - we do this because the text from a super/subscript render operations should probably be considered as part of the baseline of the text the super/sub is relative to 
                Matrix riseOffsetTransform = new Matrix(0, -renderInfo.GetRise());
                segment = segment.TransformBy(riseOffsetTransform);
            }

            //===============================

            string defaultFontName = "Arial";

            string curFont = renderInfo.GetFont().PostscriptFontName;

            if (!IsFontInstalled(curFont) && (IsFontInstalled(defaultFontName)))
            {
                curFont = defaultFontName;
            }

            //Check if faux bold is used
            if ((renderInfo.GetTextRenderMode() == (int)TextRenderMode.FillThenStrokeText))
            {
                curFont += "-Bold";
            }

            //This code assumes that if the baseline changes then we're on a newline
            Vector curBaseline = renderInfo.GetBaseline().GetStartPoint();
            Vector topRight = renderInfo.GetAscentLine().GetEndPoint();
            iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(curBaseline[Vector.I1], curBaseline[Vector.I2], topRight[Vector.I1], topRight[Vector.I2]);
            Single curFontSize = (Single)Math.Round(rect.Height);

            //===============================
            
            ZeChunkFontSize tc = new ZeChunkFontSize(renderInfo.GetText(), tclStrat.CreateLocation(renderInfo, segment));
            tc.CurFontSize = curFontSize;
            tc.CurFont = curFont;
            locationalResult.Add(tc);
        }


        /**
         * Gets text that meets the specified filter
         * If multiple text extractions will be performed for the same page (i.e. for different physical regions of the page), 
         * filtering at this level is more efficient than filtering using {@link FilteredRenderListener} - but not nearly as powerful
         * because most of the RenderInfo state is not captured in {@link TextChunk}
         * @param chunkFilter the filter to to apply
         * @return the text results so far, filtered using the specified filter
         */
        public virtual String GetResultantText(ITextChunkFilter chunkFilter)
        {
            if (DUMP_STATE)
            {
                DumpState();
            }

            List<TextChunk> filteredTextChunks = filterTextChunks(locationalResult, chunkFilter);
            filteredTextChunks.Sort();

            StringBuilder sb = new StringBuilder();
            TextChunk lastChunk = null;
            foreach (TextChunk chunk in filteredTextChunks)
            {
                if (lastChunk == null)
                {
                    sb.Append(chunk.Text);
                }
                else
                {
                    if (chunk.SameLine(lastChunk))
                    {
                        // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                        if (IsChunkAtWordBoundary(chunk, lastChunk) && !StartsWithSpace(chunk.Text) && !EndsWithSpace(lastChunk.Text))
                            sb.Append(' ');

                        sb.Append(chunk.Text);
                    }
                    else
                    {
                        sb.Append('\n');
                        sb.Append(chunk.Text);
                    }
                }
                lastChunk = chunk;
            }

            return sb.ToString();
        }

        public virtual List<ZeChunkFontSize> GetResultantTextChunks(ITextChunkFilter chunkFilter)
        {
            if (DUMP_STATE)
            {
                DumpState();
            }

            List<TextChunk> filteredTextChunks = filterTextChunks(locationalResult, chunkFilter);
            filteredTextChunks.Sort();

            //StringBuilder sb = new StringBuilder();

            List<ZeChunkFontSize> result = new List<ZeChunkFontSize>();
            TextChunk lastChunk = null;
            foreach (TextChunk chunk in filteredTextChunks)
            {
                if (lastChunk == null)
                {
                    //sb.Append(chunk.Text);
                    result.Add(chunk as ZeChunkFontSize);
                }
                else
                {
                    if (chunk.SameLine(lastChunk))
                    {
                        // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                        if (IsChunkAtWordBoundary(chunk, lastChunk) && !StartsWithSpace(chunk.Text) && !EndsWithSpace(lastChunk.Text))
                        {
                            //sb.Append(' ');
                            ZeChunkFontSize ntc = new ZeChunkFontSize(" ", lastChunk.Location);
                            ntc.CurFont = (lastChunk as ZeChunkFontSize).CurFont;
                            ntc.CurFontSize = (lastChunk as ZeChunkFontSize).CurFontSize;

                            result.Add(ntc);
                        }

                        //sb.Append(chunk.Text);
                        result.Add(chunk as ZeChunkFontSize);
                    }
                    else
                    {
                        //sb.Append('\n');
                        //sb.Append(chunk.Text);

                        ZeChunkFontSize ntc = new ZeChunkFontSize("\n", lastChunk.Location);
                        ntc.CurFont = (lastChunk as ZeChunkFontSize).CurFont;
                        ntc.CurFontSize = (lastChunk as ZeChunkFontSize).CurFontSize;

                        result.Add(ntc);
                        result.Add(chunk as ZeChunkFontSize);
                    }
                }
                lastChunk = chunk;
            }

            return result;
        }

        public List<ZeChunkFontSize> GetResultantTextChunks()
        {
            return GetResultantTextChunks(null);
        }
        /**
         * Returns the result so far.
         * @return  a String with the resulting text.
         */
        public virtual String GetResultantText()
        {
            return GetResultantText(null);
        }

        /** Used for debugging only */
        private void DumpState()
        {
            foreach (TextChunk location in locationalResult)
            {

                location.PrintDiagnostics();

                Console.Out.WriteLine();
            }

        }


        /**
         *
         * @param int1
         * @param int2
         * @return comparison of the two integers
         */
        private static int CompareInts(int int1, int int2)
        {
            return int1 == int2 ? 0 : int1 < int2 ? -1 : 1;
        }

        /**
         * no-op method - this renderer isn't interested in image events
         * @see com.itextpdf.text.pdf.parser.RenderListener#renderImage(com.itextpdf.text.pdf.parser.ImageRenderInfo)
         * @since 5.0.1
         */
        public virtual void RenderImage(ImageRenderInfo renderInfo)
        {
            // do nothing
        }

        /**
         * Specifies a filter for filtering {@link TextChunk} objects during text extraction 
         * @see LocationTextExtractionStrategy#getResultantText(TextChunkFilter)
         * @since 5.3.3
         */
        public interface ITextChunkFilter
        {
            /**
             * @param textChunk the chunk to check
             * @return true if the chunk should be allowed
             */
            bool Accept(TextChunk textChunk);
        }

        #region just for Windows
        public static bool IsFontInstalled(string fontName)
        {
            bool installed = IsFontInstalled(fontName, FontStyle.Regular);
            if (!installed) { installed = IsFontInstalled(fontName, FontStyle.Bold); }
            if (!installed) { installed = IsFontInstalled(fontName, FontStyle.Italic); }

            return installed;
        }

        public static bool IsFontInstalled(string fontName, FontStyle style)
        {
            bool installed = false;
            const float emSize = 8.0f;

            try
            {
                using (var testFont = new Font(fontName, emSize, style))
                {
                    installed = (0 == string.Compare(fontName, testFont.Name, StringComparison.InvariantCultureIgnoreCase));
                }
            }
            catch
            {
            }

            return installed;
        }
        #endregion 
    }



    /**
 * Represents a chunk of text, it's orientation, and location relative to the orientation vector
 */
    public class TextChunk : IComparable<TextChunk>
    {
        /** the text of the chunk */
        private readonly String text;
        private readonly ITextChunkLocation location;

        public TextChunk(String str, Vector startLocation, Vector endLocation, float charSpaceWidth) :
            this(str, new TextChunkLocationDefaultImp(startLocation, endLocation, charSpaceWidth))
        {
        }

        public TextChunk(String str, ITextChunkLocation location)
        {
            this.text = str;
            this.location = location;
        }

        /**
             * @return the start location of the text
             */
        public virtual Vector StartLocation
        {
            get { return Location.StartLocation; }
        }

        /**
         * @return the end location of the text
         */
        public virtual Vector EndLocation
        {
            get { return Location.EndLocation; }
        }

        /**
         * @return the width of a single space character as rendered by this chunk
         */
        public virtual float CharSpaceWidth
        {
            get { return Location.CharSpaceWidth; }
        }

        public virtual String Text
        {
            get { return text; }
        }

        public virtual ITextChunkLocation Location
        {
            get { return location; }
        }

        public virtual void PrintDiagnostics()
        {
            Console.Out.WriteLine("Text (@" + StartLocation + " -> " + EndLocation + "): " + Text);
            Console.Out.WriteLine("orientationMagnitude: " + Location.OrientationMagnitude);
            Console.Out.WriteLine("distPerpendicular: " + Location.DistPerpendicular);
            Console.Out.WriteLine("distParallel: " + Location.DistParallelStart);
        }

        /**
         * Computes the distance between the end of 'other' and the beginning of this chunk
         * in the direction of this chunk's orientation vector.  Note that it's a bad idea
         * to call this for chunks that aren't on the same line and orientation, but we don't
         * explicitly check for that condition for performance reasons.
         * @param other
         * @return the number of spaces between the end of 'other' and the beginning of this chunk
         */
        public virtual float DistanceFromEndOf(TextChunk other)
        {
            return Location.DistanceFromEndOf(other.Location);
        }

        /**
         * Compares based on orientation, perpendicular distance, then parallel distance
         * @see java.lang.Comparable#compareTo(java.lang.Object)
         */
        public virtual int CompareTo(TextChunk other)
        {
            return Location.CompareTo(other.Location);
        }

        /**
         * @param as the location to compare to
         * @return true is this location is on the the same line as the other
         */
        public virtual bool SameLine(TextChunk lastChunk)
        {
            return Location.SameLine(lastChunk.Location);
        }
    }
    
    public interface ITextChunkLocationStrategy
    {
        ITextChunkLocation CreateLocation(TextRenderInfo renderInfo, LineSegment baseline);
    }

    public interface ITextChunkLocation : IComparable<ITextChunkLocation>
    {

        /** the starting location of the chunk */
        Vector StartLocation { get; }
        /** the ending location of the chunk */
        Vector EndLocation { get; }
        /** the orientation as a scalar for quick sorting */
        int OrientationMagnitude { get; }
        /** perpendicular distance to the orientation unit vector (i.e. the Y position in an unrotated coordinate system)
         * we round to the nearest integer to handle the fuzziness of comparing floats */
        int DistPerpendicular { get; }
        /** distance of the start of the chunk parallel to the orientation unit vector (i.e. the X position in an unrotated coordinate system) */
        float DistParallelStart { get; }
        /** distance of the end of the chunk parallel to the orientation unit vector (i.e. the X position in an unrotated coordinate system) */
        float DistParallelEnd { get; }
        /** the width of a single space character in the font of the chunk */
        float CharSpaceWidth { get; }
        /**
         * @param comparedLine the location to compare to
         * @return true is this location is on the the same line as the other
         */
        bool SameLine(ITextChunkLocation other);
        /**
         * Computes the distance between the end of 'other' and the beginning of this chunk
         * in the direction of this chunk's orientation vector.  Note that it's a bad idea
         * to call this for chunks that aren't on the same line and orientation, but we don't
         * explicitly check for that condition for performance reasons.
         * @param other
         * @return the number of spaces between the end of 'other' and the beginning of this chunk
         */
        float DistanceFromEndOf(ITextChunkLocation other);

        bool IsAtWordBoundary(ITextChunkLocation previous);
    }

    public class TextChunkLocationStrategyDefaultImp : ITextChunkLocationStrategy
    {
        public ITextChunkLocation CreateLocation(TextRenderInfo renderInfo, LineSegment baseline)
        {
            return new TextChunkLocationDefaultImp(baseline.GetStartPoint(), baseline.GetEndPoint(), renderInfo.GetSingleSpaceWidth());
        }
    }

    public class TextChunkLocationDefaultImp : ITextChunkLocation
    {
        /** unit vector in the orientation of the chunk */
        private readonly Vector orientationVector;

        private readonly Vector startLocation;
        private readonly Vector endLocation;
        private readonly int orientationMagnitude;
        private readonly int distPerpendicular;
        private readonly float distParallelStart;
        private readonly float distParallelEnd;
        private readonly float charSpaceWidth;

        public TextChunkLocationDefaultImp(Vector startLocation, Vector endLocation, float charSpaceWidth)
        {
            this.startLocation = startLocation;
            this.endLocation = endLocation;
            this.charSpaceWidth = charSpaceWidth;

            Vector oVector = endLocation.Subtract(startLocation);
            if (oVector.Length == 0)
            {
                oVector = new Vector(1, 0, 0);
            }
            orientationVector = oVector.Normalize();
            orientationMagnitude = (int)(Math.Atan2(orientationVector[Vector.I2], orientationVector[Vector.I1]) * 1000);

            // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
            // the two vectors we are crossing are in the same plane, so the result will be purely
            // in the z-axis (out of plane) direction, so we just take the I3 component of the result
            Vector origin = new Vector(0, 0, 1);
            distPerpendicular = (int)(startLocation.Subtract(origin)).Cross(orientationVector)[Vector.I3];

            distParallelStart = orientationVector.Dot(startLocation);
            distParallelEnd = orientationVector.Dot(endLocation);
        }

        public virtual Vector StartLocation
        {
            get { return startLocation; }
        }

        public virtual Vector EndLocation
        {
            get { return endLocation; }
        }

        public virtual int OrientationMagnitude
        {
            get { return orientationMagnitude; }
        }

        public virtual int DistPerpendicular
        {
            get { return distPerpendicular; }
        }

        public virtual float DistParallelStart
        {
            get { return distParallelStart; }
        }

        public virtual float DistParallelEnd
        {
            get { return distParallelEnd; }
        }

        public virtual float CharSpaceWidth
        {
            get { return charSpaceWidth; }
        }

        public virtual bool SameLine(ITextChunkLocation other)
        {
            return OrientationMagnitude == other.OrientationMagnitude &&
                   DistPerpendicular == other.DistPerpendicular;
        }

        public virtual float DistanceFromEndOf(ITextChunkLocation other)
        {
            float distance = DistParallelStart - other.DistParallelEnd;
            return distance;
        }

        public virtual bool IsAtWordBoundary(ITextChunkLocation previous)
        {
            /**
             * Here we handle a very specific case which in PDF may look like:
             * -.232 Tc [( P)-226.2(r)-231.8(e)-230.8(f)-238(a)-238.9(c)-228.9(e)]TJ
             * The font's charSpace width is 0.232 and it's compensated with charSpacing of 0.232.
             * And a resultant TextChunk.charSpaceWidth comes to TextChunk constructor as 0.
             * In this case every chunk is considered as a word boundary and space is added.
             * We should consider charSpaceWidth equal (or close) to zero as a no-space.
             */
            if (CharSpaceWidth < 0.1f)
                return false;

            float dist = DistanceFromEndOf(previous);

            return dist < -CharSpaceWidth || dist > CharSpaceWidth / 2.0f;
        }

        /**
         * Compares based on orientation, perpendicular distance, then parallel distance
         * @see java.lang.Comparable#compareTo(java.lang.Object)
         */
        public virtual int CompareTo(ITextChunkLocation other)
        {
            if (this == other) return 0; // not really needed, but just in case

            int rslt;
            rslt = CompareInts(OrientationMagnitude, other.OrientationMagnitude);
            if (rslt != 0) return rslt;

            rslt = CompareInts(DistPerpendicular, other.DistPerpendicular);
            if (rslt != 0) return rslt;

            // note: it's never safe to check floating point numbers for equality, and if two chunks
            // are truly right on top of each other, which one comes first or second just doesn't matter
            // so we arbitrarily choose this way.
            rslt = DistParallelStart < other.DistParallelStart ? -1 : 1;

            return rslt;
        }
        private static int CompareInts(int int1, int int2)
        {
            return int1 == int2 ? 0 : int1 < int2 ? -1 : 1;
        }
    }

    enum TextRenderMode
    {
        FillText = 0,
        StrokeText = 1,
        FillThenStrokeText = 2,
        Invisible = 3,
        FillTextAndAddToPathForClipping = 4,
        StrokeTextAndAddToPathForClipping = 5,
        FillThenStrokeTextAndAddToPathForClipping = 6,
        AddTextToPaddForClipping = 7
    }

}
