using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Objects.Entities;
using Objects.Entities.Books;
using System.Security.Claims;

namespace BlazorServer.Controllers
{
    [Authorize]
    [ApiController]
    public class BookCoverController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _appDbContext;
        public BookCoverController(AppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _appDbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("api/[controller]/Get")]
        public async Task<ActionResult<IEnumerable<BookCover>>> Get()
        {
            List<BookCover>? bookCovers = new List<BookCover>();
            List<AbstractBook>? abstractBooks = null;
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            //var user = await _userManager.GetUserAsync(User);
            if (user is not null)
            {
                abstractBooks = await _appDbContext.AbstractBooks.Where(ab => ab.OwnerId == Guid.Parse(user.Id)).ToListAsync();
                foreach (var book in abstractBooks)
                {
                    bookCovers.Add(_appDbContext.BookCovers.Single(bc => bc.BookId == book.Id));
                }
            }

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
