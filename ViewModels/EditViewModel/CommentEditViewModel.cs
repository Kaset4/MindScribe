using System.ComponentModel.DataAnnotations;

namespace MindScribe.ViewModels.EditViewModel
{
    public class CommentEditViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Название статьи", Prompt = "Введите название")]
        public string Content_comment { get; set; } = "";

        public int Id { get; set; }
        public int Article_id { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        public string User_id { get; set; } = "";
    }
}
