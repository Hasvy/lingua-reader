using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Objects.Entities.Books.EpubBook;
using Objects.Entities.Books.PdfBook;

namespace BlazorServer.Controllers
{
    [ApiController]
    public class PdfBookController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public PdfBookController(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        [HttpPost]
        [Route("api/PdfBook/Post")]
        public async Task<IActionResult> PostPdf([FromBody] PdfBook book)
        {
            if (ModelState.IsValid)
            {
                _appDbContext.PdfBooks.Add(book);
                await _appDbContext.SaveChangesAsync();

                return Ok();
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("api/PdfBook/Get/{Id:Guid}")]
        public ActionResult<PdfBook> Get(Guid id)
        {
            var book = _appDbContext.PdfBooks.First(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(book);
            }
        }
    }
}
