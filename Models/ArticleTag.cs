using System.ComponentModel.DataAnnotations.Schema;

namespace MindScribe.Models
{
    public class ArticleTag
    {
        public int Id{ get; set; }
        public string NameTag { get; set; } = "";

        [Column("Article_id")]
        public int ArticleId { get; set; }
    }
}
