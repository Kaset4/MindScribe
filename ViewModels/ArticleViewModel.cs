using Microsoft.AspNetCore.Identity;
using MindScribe.Models;
using System.ComponentModel.DataAnnotations;

namespace MindScribe.ViewModels
{
    public class ArticleViewModel
    {
        [Required]
        [Display(Name = "Название статьи", Prompt = "Введите название статьи")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Контент", Prompt = "Введите текст")]
        public string Content_article { get; set; }

        public string User_id { get; set; }
        public List<ArticleTag>? Tags { get; set; }
        public List<Comment>? Comments { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }

        public CommentViewModel CommentView { get; set; }

        public ArticleViewModel()
        {
            CommentView = new CommentViewModel();
        }

        public async Task<string> GetFullNameAuthor(UserManager<User> userManager)
        {
            var user = await userManager.FindByIdAsync(User_id);
            return user != null ? $"{user.LastName} {user.FirstName}" : "Unknown Author";
        }
    }
}
