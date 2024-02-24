using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Objects.Entities;
using Objects.Entities.Translator;
using Objects.Entities.Words;
using System;

namespace BlazorServer.Controllers
{
    [ApiController]
    public class WordsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly DictionaryDbContext _dictionaryDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public WordsController(AppDbContext appDbContext, DictionaryDbContext dictionaryDbContext, UserManager<ApplicationUser> userManager)
        {
            _appDbContext = appDbContext;
            _dictionaryDbContext = dictionaryDbContext;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("api/words/SaveWord")]
        public async Task<IActionResult> SaveWord([FromBody] int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return BadRequest();

            var duplicateCheck = _appDbContext.SavedWords.Any(w => w.UserId.ToString() == user.Id && w.WordId == id);
            if (duplicateCheck is true)
                return Conflict();

            var savedWord = new SavedWord { UserId = Guid.Parse(user.Id), WordId = id };
            await _appDbContext.SavedWords.AddAsync(savedWord);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("api/words/DeleteWord/{Id:int}")]
        public async Task<IActionResult> DeleteWord(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return BadRequest();
            }

            var wordToDelete = await _appDbContext.SavedWords.SingleOrDefaultAsync(w => w.UserId.ToString() == user.Id && w.WordId == id);
            if (wordToDelete is null)
                return NotFound();

            _appDbContext.SavedWords.Remove(wordToDelete);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("api/words/GetUsersWords")]
        public async Task<IActionResult> GetUsersWords()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return BadRequest();
            }
            //Find all words that user has
            var usersWordsIds = _appDbContext.SavedWords.Where(w => w.UserId.ToString() == user.Id).ToList();
            //Join them with the words table
            var query = from savedWord in usersWordsIds
                        join translatorWordResponse in _dictionaryDbContext.Words
                        on savedWord.WordId equals translatorWordResponse.Id
                        select translatorWordResponse;
                        //{
                        //    Id = savedWord.WordId,
                        //    Language = translatorWordResponse.Language,
                        //    DisplaySource = translatorWordResponse.DisplaySource,
                        //    UserId = savedWord.UserId
                        //};

            
            var usersWords = query.ToList();
            return Ok(usersWords);
        }
    }
}
