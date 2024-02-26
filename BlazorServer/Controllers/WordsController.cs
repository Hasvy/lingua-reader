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
                return BadRequest();

            var wordToDelete = await _appDbContext.SavedWords.SingleOrDefaultAsync(w => w.UserId.ToString() == user.Id && w.WordId == id);
            if (wordToDelete is null)
                return NotFound();

            _appDbContext.SavedWords.Remove(wordToDelete);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("api/words/GetWordsCount")]
        public async Task<IActionResult> GetWordsCount()
        {
            var user = await _userManager.GetUserAsync(User);       //TODO GetUser method
            if (user is null)
                return BadRequest();

            var usersWordsIds = _appDbContext.SavedWords.Where(w => w.UserId.ToString() == user.Id).ToList();
            int wordsCount = (from savedWord in usersWordsIds
                              join word in _dictionaryDbContext.Words
                                  on savedWord.WordId equals word.Id
                              where word.Language == user.DesiredLanguage
                              select word).Count();
            return Ok(wordsCount);
        }

        [HttpGet]
        [Route("api/words/GetWordsToLearn")]
        public async Task<IActionResult> GetWordsToLearn()
        {
            //TODO
            return null;
        }

        [HttpGet]
        [Route("api/words/GetAllUsersWords")]
        public async Task<IActionResult> GetAllUsersWords()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return BadRequest();

            //Find all words that user has
            var usersWordsIds = _appDbContext.SavedWords.Where(w => w.UserId.ToString() == user.Id).ToList();
            //Join them with the words table
            var completeQuery = from savedWord in usersWordsIds
                                join wordWithTranslations in _dictionaryDbContext.Words
                                    .Where(w => w.Language == user.DesiredLanguage)
                                    .DefaultIfEmpty()
                                    on savedWord.WordId equals wordWithTranslations?.Id
                                join translation in _dictionaryDbContext.Translations
                                    .Where(t => t.Language == user.NativeLanguage)
                                    .DefaultIfEmpty()
                                    on savedWord.WordId equals translation.WordId into translationsGroup
                                select new WordWithTranslations
                                {
                                    Id = wordWithTranslations.Id,
                                    DisplaySource = wordWithTranslations.DisplaySource,
                                    Translations = translationsGroup.ToList(),
                                    Language = wordWithTranslations.Language,
                                    IsWordSaved = wordWithTranslations.IsWordSaved
                                };

            //var words = from savedWord in usersWordsIds
            //            join word in _dictionaryDbContext.Words.DefaultIfEmpty()
            //                on savedWord.WordId equals word.Id
            //            where word.Language == user.DesiredLanguage
            //            select word;

            //var completeQuery = from wordWithTranslations in words
            //                    join translation in _dictionaryDbContext.Translations.DefaultIfEmpty()
            //                        on wordWithTranslations.Id equals translation.WordId into translationGroup
            //                    from translation in translationGroup.DefaultIfEmpty()
            //                    where translation.Language == user.NativeLanguage
            //                    select wordWithTranslations;

            //var query = from a in (from savedWord in usersWordsIds
            //            join word in _dictionaryDbContext.Words
            //                //.Where(w => w.Language == user.DesiredLanguage)
            //                //.DefaultIfEmpty()
            //                on savedWord.WordId equals word.Id
            //            where word.Language == user.DesiredLanguage
            //            select word).DefaultIfEmpty()
            //            join translation in _dictionaryDbContext.Translations
            //                on a.Id equals translation.WordId
            //            where translation.Language == user.DesiredLanguage
            //            //from word in wordGroup.DefaultIfEmpty()
            //            //join translation in _dictionaryDbContext.Translations
            //            //    .Where(t => t.Language == user.NativeLanguage)
            //            //    .DefaultIfEmpty()
            //            //    on savedWord.WordId equals translation.Id into translationGroup
            //            //from translation in translationGroup.DefaultIfEmpty()
            //            //where word.Language == user.DesiredLanguage
            //            //where word.Language == user.NativeLanguage
            //            select a;

            var usersWords = completeQuery.ToList();
            return Ok(usersWords);
        }
    }
}
