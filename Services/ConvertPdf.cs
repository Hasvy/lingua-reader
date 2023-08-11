using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iText;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;

namespace Services
{
    public class ConvertPdf
    {

        public static string GetText(Stream stream)
        {
            string text = "";
            PdfReader reader = new PdfReader(stream);
            using (var ms = new MemoryStream())
            {
                using (PdfReader reader22 = new PdfReader(stream))
                {
                    PdfDocument document = new PdfDocument(reader);
                    for (int page = 1; page <= document.GetNumberOfPages(); page++)
                    {
                        ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                        text += PdfTextExtractor.GetTextFromPage(document.GetPage(page), strategy);
                    }
                }
                return text;
            }
        }
    }
}
