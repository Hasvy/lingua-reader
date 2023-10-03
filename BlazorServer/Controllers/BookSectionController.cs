using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Objects.Entities;
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
            var bookSections = await _appDbContext.BookSections.Where(bs => bs.BookId == id)
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
