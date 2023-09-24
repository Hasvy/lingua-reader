﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Objects.Entities;

namespace BlazorServer.Controllers
{
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public BookController(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        [HttpPost]
        [Route("api/[controller]/Post")]
        public async Task<IActionResult> Post([FromBody] Book book)
        {
            if (ModelState.IsValid)
            {
                var newBook = new Book
                {
                    Text = book.Text
                };

                BookCover newBookCover = new BookCover 
                {
                    BookId = book.BookCover.BookId,
                    Title = book.BookCover.Title,
                    Author = book.BookCover.Author,
                    Description = book.BookCover.Description,
                    Format = book.BookCover.Format
                };

                newBook.BookCover = newBookCover;
                _appDbContext.Books.Add(newBook);
                await _appDbContext.SaveChangesAsync();

                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete]
        [Route("api/[controller]/Delete/{Id:Guid}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var bookToDelete = await _appDbContext.Books.SingleOrDefaultAsync(b => b.Id == id);
            
            if (bookToDelete == null)
            {
                return NotFound();
            }

            _appDbContext.Books.Remove(bookToDelete);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}