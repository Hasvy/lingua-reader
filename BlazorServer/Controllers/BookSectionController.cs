using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Objects.Entities.Books.EpubBook;
using Objects.Entities.Books.PdfBook;
using Objects.Entities.Books.TxtBook;
using static System.Reflection.Metadata.BlobBuilder;

namespace BlazorServer.Controllers
{
    [ApiController]
    public class BookSectionController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public BookSectionController(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        [HttpGet]
        [Route("api/[controller]/Get/{Id:Guid}")]
        public async Task<ActionResult<IEnumerable<BookSection>>> Get(Guid id)
        {
            var bookSections = await _appDbContext.BookSections.Where(bs => bs.EpubBookId == id)
                                                               .OrderBy(bs => bs.OrderNumber)
                                                               .ToListAsync();

            if (bookSections == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(bookSections);
            }
        }

        [HttpGet]
        [Route("api/TxtBook/Get/{Id:Guid}")]
        public ActionResult<TxtBook> GetTxtBook(Guid id)
        {
            var book = _appDbContext.TxtBooks.First(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(book);
            }
        }

        //[HttpGet]
        //[Route("api/[controller]/html")]
        //public async Task<ActionResult<IEnumerable<BookSection>>> GetHtml()
        //{
        //    //var bookSections = await _appDbContext.BookSections.Where(bs => bs.BookId == id).ToListAsync();
        //    string htmlContent = System.IO.File.ReadAllText("Uploads\\Users\\user1\\Books\\451\\OEBPS\\Text\\Section0002.xhtml");
        //    if (htmlContent == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        return Ok(htmlContent);
        //    }
        //}
    }
}
