using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MindScribe.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Column("Article_id")]
        public int ArticleId { get; set; }
        public string Content_comment { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        public string User_id { get; set; }

        public async Task<string> GetFullNameComment(UserManager<User> userManager)
        {
            var user = await userManager.FindByIdAsync(User_id);
            return $"{user.LastName} {user.FirstName}";
        }
    }
}
