namespace MindScribe.Models
{
    public class Article
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content_article { get; set; }
        public string User_id { get; set; }
        public List<ArticleTag>? Tags { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
