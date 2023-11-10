namespace Objects.Entities.Translator
{
    public class WordTranslation
    {
        public int Id { get; set; }
        public string DisplayTarget { get; set; } = null!;
        public string PosTag { get; set; } = null!;
        public float Confidence { get; set; }
        public string? PrefixWord { get; set; }
        public int WordId { get; set; }     //FK
        //public IList<string> backTranslations { get; set; }
    }
}