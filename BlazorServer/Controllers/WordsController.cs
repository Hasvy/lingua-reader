using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Objects.Entities;
using Objects.Entities.Translator;
using Objects.Entities.Words;

namespace BlazorServer.Controllers
{
    [ApiController]
    public class WordsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        //private readonly DictionaryDbContext _dictionaryDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public WordsController(AppDbContext appDbContext, UserManager<ApplicationUser> userManager)
        {
            _appDbContext = appDbContext;
            //_dictionaryDbContext = dictionaryDbContext;
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

        [HttpPost]
        [Route("api/words/DeleteWords")]
        public async Task<IActionResult> DeleteWords([FromBody] List<int> wordsToDeleteIds)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return BadRequest();

            var usersWordsIds = _appDbContext.SavedWords.Where(w => w.UserId.ToString() == user.Id).ToList();
            var wordsToDelete = from savedWord in usersWordsIds
                                join id in wordsToDeleteIds
                                    on savedWord.WordId equals id
                                select savedWord;
            if (wordsToDelete is null)
                return NotFound();

            _appDbContext.SavedWords.RemoveRange(wordsToDelete);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("api/words/GetWordsToLearn")]
        public async Task<IActionResult> GetWordsToLearn()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return BadRequest();

            int count = 10;
            var usersWordsIds = _appDbContext.SavedWords.Where(w => w.UserId.ToString() == user.Id);
            var wordsWithTranslations = JoinWordsWithTranslations(usersWordsIds, user, true);
            wordsWithTranslations = wordsWithTranslations.OrderBy(r => Guid.NewGuid()).Take(count).ToList();
            List<WordToLearn> wordsToLearn = new List<WordToLearn>();
            foreach (var word in wordsWithTranslations)
            {
                wordsToLearn.Add(new WordToLearn { WordWithTranslations = word });
            }
            foreach (var word in wordsToLearn)
            {
                word.WrongVariants = GetWrongVariants(3, word, user);
            }
            return Ok(wordsToLearn);
        }

        [HttpGet]
        [Route("api/words/GetAllUsersWords")]
        public async Task<IActionResult> GetAllUsersWords()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return BadRequest();

            //Find all wordsWithTranslations that user has
            var usersWordsIds = _appDbContext.SavedWords.Where(w => w.UserId.ToString() == user.Id).ToList();
            //Join them with the wordsWithTranslations table
            var completeQuery = JoinWordsWithTranslations(usersWordsIds, user, true);
            var usersWords = completeQuery.ToList();
            return Ok(usersWords);
        }

        private IEnumerable<WordWithTranslations> JoinWordsWithTranslations(IEnumerable<SavedWord> savedWords, ApplicationUser user, bool isWordsSaved = false)
        {
            return from savedWord in savedWords
                   join wordWithTranslations in _appDbContext.Words
                       .Where(w => w.Language == user.DesiredLanguage)
                       .DefaultIfEmpty()
                       on savedWord.WordId equals wordWithTranslations?.Id
                   join translation in _appDbContext.Translations
                       .Where(t => t.Language == user.NativeLanguage)
                       on savedWord.WordId equals translation.WordId into translationsGroup
                   where translationsGroup.Any()
                   select new WordWithTranslations
                   {
                       Id = wordWithTranslations.Id,
                       DisplaySource = wordWithTranslations.DisplaySource,
                       Translations = translationsGroup.ToList(),
                       Language = wordWithTranslations.Language,
                       IsWordSaved = isWordsSaved
                   };
        }

        private WordTranslation[] GetWrongVariants(int count, WordToLearn wordToLearn, ApplicationUser user)
        {
            //List<string> posTags = new List<string>();      //Types of speech of the right variant - noun, adv, verb...
            //foreach (var translation in wordToLearn.WordWithTranslations.Translations)
            //{
            //    if (!posTags.Contains(translation.PosTag))
            //        posTags.Add(translation.PosTag);
            //}
            string posTag = wordToLearn.WordWithTranslations.Translations.First().PosTag;

            var variants = _appDbContext.Translations
                //Get only variants in user's native language and wordsWithTranslations of the same type of speech as the right variant
                //.Where(v => v.Language == user.NativeLanguage && posTags.Contains(v.PosTag)).AsEnumerable()
                .Where(v => v.Language == user.NativeLanguage && v.PosTag == posTag).AsEnumerable()
                //Prevent all right translations of the word from being in WrongVariants
                .Where(v => !wordToLearn.WordWithTranslations.Translations.Any(t => t.DisplayTarget == v.DisplayTarget))
                .OrderBy(r => Guid.NewGuid())
                .Take(count)
                .ToArray();

            if (variants.Count() == 3)
            {
                return variants;
            }
            else
            {
                return _appDbContext.Translations
                    .Where(v => v.Language == user.NativeLanguage).AsEnumerable()
                    .Where(v => !wordToLearn.WordWithTranslations.Translations.Any(t => t.DisplayTarget == v.DisplayTarget))
                    .OrderBy(r => Guid.NewGuid())
                    .Take(count)
                    .ToArray();
            }
        }
    }
}
