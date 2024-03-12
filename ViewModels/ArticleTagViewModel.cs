using Microsoft.AspNetCore.Identity;
using MindScribe.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace MindScribe.ViewModels
{
    public class ArticleTagViewModel
    {
        public string NameTag { get; set; } = "";

        [Column("Article_id")]
        public int ArticleId { get; set; }

    }
}
