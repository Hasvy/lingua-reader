using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Objects.Entities;

namespace BlazorServer.Controllers
{
    [ApiController]
    public class BookContentController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public BookContentController(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        [HttpGet]
        [Route("api/[controller]/Get/{Id:Guid}")]
        public async Task<ActionResult<IEnumerable<BookContent>>> Get(Guid id)
        {
            var bookContent = await _appDbContext.BookContent.Where(bs => bs.BookId == id).ToListAsync();

            if (bookContent == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(bookContent);
            }
        }
    }
}
