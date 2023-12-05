using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
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
            var imgEls = sectionHtml.QuerySelectorAll("img, image");

            foreach (var imgEl in imgEls)
            {
                string? attributeName = GetAttributeName(imgEl);
                if (attributeName != null)
                {
                    string? src = imgEl.GetAttribute(attributeName);
                    if (src is not null)
                    {
                        var img = epubBookRef.Content.Images.Local.SingleOrDefault(i => Path.GetFileName(i.FilePath) == Path.GetFileName(src));
                        if (img is not null)
                        {
                            byte[] imgBytes = await img.ReadContentAsBytesAsync();
                            string imgBase64 = Convert.ToBase64String(imgBytes);
                            imgEl.SetAttribute(attributeName, $"data:image/{Path.GetExtension(src).Replace(".", "")};base64," + imgBase64);
                        }
                    }
                }
            }
            foreach (var cssEl in cssEls)
            {
                string? src = cssEl.GetAttribute(AttributeNames.Href);
                if (src is not null)
                {
                    var css = epubBookRef.Content.Css.Local.SingleOrDefault(c => Path.GetFileName(c.FilePath) == Path.GetFileName(src));
                    if (css is not null)
                    {
                        byte[] imgBytes = await css.ReadContentAsBytesAsync();
                        string imgBase64 = Convert.ToBase64String(imgBytes);
                        cssEl.SetAttribute(AttributeNames.Href, $"data:text/{Path.GetExtension(src).Replace(".", "")};base64," + imgBase64);
                    }
                }
            }

            return sectionHtml.ToHtml();
        }

        private string? GetAttributeName(IElement imageElement)
        {
            if (imageElement.HasAttribute(AttributeNames.Src))
            {
                return AttributeNames.Src;
            }
            if (imageElement.HasAttribute(AttributeNames.Href))
            {
                return AttributeNames.Href;
            }
            return null;
        }
    }
}
