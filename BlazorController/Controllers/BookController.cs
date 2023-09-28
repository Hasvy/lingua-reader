using Microsoft.AspNetCore.Http;
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
                _appDbContext.Books.Add(book);
                await _appDbContext.SaveChangesAsync();

                byte[] bytes = Convert.FromBase64String(book.BookContentFile);
                string filename = book.Id.ToString();
                string path = "Uploads\\Users\\user1\\Books\\" + filename;

                //TODO security check before save!!!
                //TODO save confidence, save files encrypted.   Maybe use File.Encrypt
                using (var stream = System.IO.File.Create(path))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete]
        [Route("api/[controller]/Delete/{Id:Guid}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var bookToDelete = await _appDbContext.Books.SingleOrDefaultAsync(b => b.Id == id);
            string filename = id.ToString();
            string path = "Uploads\\Users\\user1\\Books\\" + filename;

            if (bookToDelete == null)
            {
                return NotFound();
            }

            System.IO.File.Delete(path);
            _appDbContext.Books.Remove(bookToDelete);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
