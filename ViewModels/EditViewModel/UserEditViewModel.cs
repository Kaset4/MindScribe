using System.ComponentModel.DataAnnotations;

namespace MindScribe.ViewModels.EditViewModel
{
    public class UserEditViewModel
    {
        [Required(ErrorMessage = "Поле Имя обязательно к заполнению")]
        [DataType(DataType.Text)]
        [Display(Name = "Имя", Prompt = "Введите имя")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Поле Фамилия обязательно к заполнению")]
        [DataType(DataType.Text)]
        [Display(Name = "Фамилия", Prompt = "Введите фамилию")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Поле Email обязательно к заполнению")]
        [EmailAddress]
        [Display(Name = "Email", Prompt = "example.com")]
        public string Email { get; set; } = "";

        [DataType(DataType.Text)]
        [Display(Name = "Ссылка", Prompt = "Ссылка на картинку")]
        public string? Image { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "О себе", Prompt = "Введите информацию о себе")]
        public string? About { get; set; }

        public string UserId { get; set; } = "";
    }
}
