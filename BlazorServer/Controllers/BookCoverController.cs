using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Objects.Entities;

namespace BlazorServer.Controllers
{
    [ApiController]
    public class BookCoverController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public BookCoverController(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        [HttpGet]
        [Route("api/[controller]/Get")]
        public async Task<ActionResult<IEnumerable<BookCover>>> Get()
        {
            var bookCovers = await _appDbContext.BookCovers.ToListAsync();

            if (bookCovers == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(bookCovers);
            }
        }

        [HttpPut]
        [Route("api/[controller]/Language/Put")]
        public async Task<ActionResult> PutLanguage([FromQuery] string newLanguage, [FromQuery] Guid bookCoverId)
        {
            BookCover? bookCover = await _appDbContext.BookCovers.SingleAsync(bc => bc.Id == bookCoverId);
            if (bookCover is not null)
            {
                bookCover.Language = newLanguage;
                await _appDbContext.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
