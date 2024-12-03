namespace HackerNews.Contracts
{

    public class Story
    {
        //NOTE: Property names are lowercase to match the requirement. Alternatively this can be set to uppercase, but custom Json converter should be used to serialize and deserialize values
        public string title { get; set; }
        public string uri { get; set; }
        public string postedBy { get; set; }

        public string time { get; set; }
        public int score { get; set; }
        public int commentCount { get; set; }
    }
}
