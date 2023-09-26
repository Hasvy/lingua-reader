using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Objects.Entities;

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
            var bookSections = await _appDbContext.BookSections.Where(bs => bs.BookId == id).ToListAsync();

            if (bookSections == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(bookSections);
            }
        }
    }
}
