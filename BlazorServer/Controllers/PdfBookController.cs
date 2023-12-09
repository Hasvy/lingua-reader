﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Objects.Entities.Books.EpubBook;
using Objects.Entities.Books.PdfBook;

namespace BlazorServer.Controllers
{
    [Authorize]
    [ApiController]
    public class PdfBookController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        public PdfBookController(AppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _appDbContext = dbContext;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("api/PdfBook/Post")]
        public async Task<IActionResult> PostPdf([FromBody] PdfBook book)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user is not null)
                {
                    book.OwnerId = Guid.Parse(user.Id);
                    _appDbContext.PdfBooks.Add(book);
                    await _appDbContext.SaveChangesAsync();

                    return Ok();
                }
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
