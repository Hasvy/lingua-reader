namespace Objects.Entities.Translator
{
    public class WordTranslation
    {
        public int Id { get; set; }
        public string displayTarget { get; set; } = null!;
        public string? posTag { get; set; }
        public float confidence { get; set; }
        public string? prefixWord { get; set; }
        //public IList<string> backTranslations { get; set; }
    }
}