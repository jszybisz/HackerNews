using System.Text.Json.Serialization;

namespace HackerNews.Contracts
{
    public class SourceStory
    {
        public string by { get; set; }
        public int descendants { get; set; }
        public int id { get; set; }
        public List<int> kids { get; set; }
        public int score { get; set; }
        public long time { get; set; } // Note: Time is a long to store the Unix timestamp
        public string title { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public bool deleted { get; set; }
    }
}
