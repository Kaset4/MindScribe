namespace MindScribe.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Article_id { get; set; }
        public string Content { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
