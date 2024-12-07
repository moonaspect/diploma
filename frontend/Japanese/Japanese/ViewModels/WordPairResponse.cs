namespace Japanese.ViewModels
{
    /// <summary>
    /// Represents the API response containing word pairs and metadata.
    /// </summary>
    public class WordPairResponse
    {
        public int TotalItems { get; set; }
        public List<WordPair> Items { get; set; } = new();
    }
}
