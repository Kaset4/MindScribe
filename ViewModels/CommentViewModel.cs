using Microsoft.AspNetCore.Identity;
using MindScribe.Models;
using System.ComponentModel.DataAnnotations;

namespace MindScribe.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public int Article_id { get; set; }

        [Required]
        [Display(Name = "Комментарий", Prompt = "Введите текст")]
        public string Content_comment { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        public string User_id { get; set; }


    }
}
