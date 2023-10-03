using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Parser;
using iText.StyledXmlParser;
using VersOne.Epub;

namespace Services
{
    public class HtmlParserService
    {
        private readonly HtmlParser _htmlParser;
        public HtmlParserService(HtmlParser htmlParser)
        {
            _htmlParser = htmlParser;
        }

        public async Task<string> Parse(string html, EpubBookRef epubBookRef)
        {
            var sectionHtml = _htmlParser.ParseDocument(html);
            var cssEls = sectionHtml.QuerySelectorAll("link[rel='stylesheet']");
            var imgEls = sectionHtml.QuerySelectorAll("img");

            foreach (var imgEl in imgEls)
            {
                string src = imgEl.GetAttribute("src");
                var img = epubBookRef.Content.Images.Local.SingleOrDefault(i => Path.GetFileName(i.FilePath) == Path.GetFileName(src));
                if (img is not null)
                {
                    byte[] imgBytes = await img.ReadContentAsBytesAsync();
                    string imgBase64 = Convert.ToBase64String(imgBytes);
                    imgEl.SetAttribute("src", $"data:image/{Path.GetExtension(src).Replace(".", "")};base64," + imgBase64);
                }
            }
            foreach (var cssEl in cssEls)
            {
                string src = cssEl.GetAttribute("href");
                var css = epubBookRef.Content.Css.Local.SingleOrDefault(c => Path.GetFileName(c.FilePath) == Path.GetFileName(src));
                if (css is not null)
                {
                    byte[] imgBytes = await css.ReadContentAsBytesAsync();
                    string imgBase64 = Convert.ToBase64String(imgBytes);
                    cssEl.SetAttribute("href", $"data:text/{Path.GetExtension(src).Replace(".", "")};base64," + imgBase64);
                }
            }

            return sectionHtml.ToHtml();
        }
    }
}
