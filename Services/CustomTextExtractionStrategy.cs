using System.Text;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Services
{
    public class CustomTextExtractionStrategy : ITextExtractionStrategy
    {
        private Vector lastStart = null!;
        private Vector lastEnd = null!;

        private bool? fontIsBold = false;
        private bool isNewLine = true;
        private Vector startLine = null!;
        private Vector endLine = null!;

        private readonly StringBuilder result = new StringBuilder();
        private readonly StringBuilder lineResult = new StringBuilder();
        private string lastLine = string.Empty;
        private readonly char[] charsCheck = { '.', '?', '!' };       //Chars to check end of a paragraph.
        private Encoding @Encoding { get; }

        public CustomTextExtractionStrategy(Encoding encoding) : base()
        {
            @Encoding = encoding;
        }

        public virtual void EventOccurred(IEventData data, EventType type)
        {
            if (type.Equals(EventType.RENDER_TEXT))
            {
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                bool firstRender = lineResult.Length == 0;        //result.Length == 0 && 
                bool hardReturn = false;
                LineSegment segment = renderInfo.GetBaseline();
                int fontSize = (int)renderInfo.GetFontSize();
                IsTextBold(renderInfo);

                Vector start = segment.GetStartPoint();
                Vector end = segment.GetEndPoint();

                if (firstRender)
                {
                    lineResult.Insert(0, "<p>");
                }

                if (!firstRender)
                {
                    Vector x1 = lastStart;
                    Vector x2 = lastEnd;
                    // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
                    Vector vDiff = x2.Subtract(x1);
                    Vector vDiff2 = x1.Subtract(start);
                    Vector cross = vDiff.Cross(vDiff2);
                    float diff = cross.LengthSquared();
                    float diff2 = vDiff.LengthSquared();
                    float disst = diff / diff2;

                    float dist = (x2.Subtract(x1)).Cross((x1.Subtract(start))).LengthSquared() / x2.Subtract(x1).LengthSquared();
                    // we should probably base this on the current font metrics, but 1 pt seems to be sufficient for the time being
                    float sameLineThreshold = 1f;
                    if (dist > sameLineThreshold)
                    {
                        hardReturn = true;
                    }
                }

                if (isNewLine)
                {
                    if (lastLine.Contains("</p>"))
                    {
                        lineResult.Insert(0, $"<p>");
                        //lineResult.Insert(0, $"<p style=\"font-size:{fontSize}px\">");
                    }
                    startLine = lastStart ??= start;
                    isNewLine = false;
                }
                // Note:  Technically, we should check both the start and end positions, in case the angle of the text changed without any displacement
                // but this sort of thing probably doesn't happen much in reality, so we'll leave it alone for now
                if (hardReturn)
                {
                    //System.out.println("<< Hard Return >>");
                    endLine = lastEnd;
                    //Console.WriteLine(lineResult.ToString());
                    //Console.WriteLine(startLine.ToString() + " " + endLine.ToString() + "\n");
                    AppendTextChunk("\n");
                    int check = (lineResult.Length >= 3) ? lineResult.ToString().Substring(lineResult.Length - 3).IndexOfAny(charsCheck) : -1;
                    if (check != -1)
                    {
                        lineResult.Append("</p>");
                    }
                    AppendTextLine(lineResult.ToString());
                    lastLine = lineResult.ToString();
                    lineResult.Clear();
                    isNewLine = true;
                }
                else
                {
                    if (!firstRender)
                    {
                        // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                        if (lineResult[lineResult.Length - 1] != ' ' && renderInfo.GetText().Length > 0 && renderInfo.GetText()[0] != ' ')
                        {
                            float spacing = lastEnd.Subtract(start).Length();       //problem with czech lang spacing, maybe becaouse of enconding
                            if (spacing > renderInfo.GetSingleSpaceWidth() / 2f)
                            {
                                AppendTextChunk(" ");
                            }
                        }
                    }
                }
                //System.out.println("Inserting implied space before '" + renderInfo.getText() + "'");
                //System.out.println("Displaying first string of content '" + text + "' :: x1 = " + x1);
                //System.out.println("[" + renderInfo.getStartPoint() + "]->[" + renderInfo.getEndPoint() + "] " + renderInfo.getText());
                AppendTextChunk(renderInfo.GetText());
                lastStart = start;
                lastEnd = end;
            }

            if (type.Equals(EventType.RENDER_IMAGE))
            {
                ImageRenderInfo renderInfo = (ImageRenderInfo)data;
                var image = renderInfo.GetImage();
                if (image != null)
                {
                    float height = image.GetHeight();
                    float width = image.GetWidth();

                    byte[] bytes = image.GetImageBytes();
                    var base64 = Convert.ToBase64String(bytes);
                    var ext = image.IdentifyImageFileExtension();
                    AppendTextLine($"<img style=\"height:{height}; width:{width};\" src=\"data:image/{ext};base64,{base64}\">");
                }
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents()
        {
            LinkedHashSet<EventType> events = new LinkedHashSet<EventType>()
            {
                EventType.RENDER_TEXT,
                EventType.RENDER_IMAGE
            };
            return JavaCollectionsUtil.UnmodifiableSet(events);
            //return JavaCollectionsUtil.UnmodifiableSet(new LinkedHashSet<EventType>(JavaCollectionsUtil.SingletonList(
            //    EventType.RENDER_TEXT)));
        }

        /// <summary>Returns the result so far.</summary>
        /// <returns>a String with the resulting text.</returns>
        public virtual string GetResultantText()
        {
            return result.ToString();
        }

        /// <summary>Used to actually append text to the text results.</summary>
        /// <remarks>
        /// Used to actually append text to the text results.  Subclasses can use this to insert
        /// text that wouldn't normally be included in text parsing (e.g. result of OCR performed against
        /// image content)
        /// </remarks>
        /// <param name="text">the text to append to the text results accumulated so far</param>
        protected internal void AppendTextChunk(string textString)
        {
            lineResult.Append(textString);
        }

        protected internal void AppendTextLine(string textLine)
        {
            result.Append(textLine);
        }

        private void IsTextBold(TextRenderInfo renderInfo)
        {
            if (renderInfo.GetFont().GetFontProgram().GetFontNames().GetFontName().Contains("Bold"))
            {
                if (fontIsBold is false)
                {
                    lineResult.Append("<b>");
                }
                fontIsBold = true;
            }
            else
            {
                if (fontIsBold is true)
                {
                    lineResult.Append("</b>");
                }
                fontIsBold = false;
            }
        }
    }
}
