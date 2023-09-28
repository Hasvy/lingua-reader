using Microsoft.AspNetCore.Mvc;
using Objects.Entities;
using System.Text.Json;

namespace BlazorServer.Controllers
{
    [ApiController]
    public class BookFileController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public BookFileController(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        //[HttpPost]
        //[Route("api/[controller]/Post")]
        //public async Task<IActionResult> PostBookFile([FromBody] string base64)
        //{
        //    byte[] bytes = Convert.FromBase64String(base64);
        //    string path = "Uploads\\Users\\user1\\Books\\file.epub";

        //    //TODO security check before save!!!
        //    //TODO save confidence, save files encrypted.
        //    using (var stream = System.IO.File.Create(path))
        //    {
        //        stream.Write(bytes, 0, bytes.Length);
        //    }

        //    if (base64 == null)
        //    {
        //        return BadRequest();
        //    }
        //    else
        //    {
        //        return Ok();
        //    }
        //}

        [HttpGet]
        [Route("api/[controller]/Get/{Id:Guid}")]
        public async Task<IActionResult> GetBookFile(Guid id)
        {
            Book? book = _appDbContext.Books.SingleOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }
            string filename = id.ToString();
            string path = "Uploads\\Users\\user1\\Books\\" + filename;

            byte[] bytes = await System.IO.File.ReadAllBytesAsync(path);
            //string base64 = Convert.ToBase64String(bytes);
            

            if (bytes == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(JsonSerializer.SerializeToUtf8Bytes(bytes));
            }
        }
    }
}
