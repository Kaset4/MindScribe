using System.ComponentModel.DataAnnotations;

namespace MindScribe.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле Логин обязательно к заполнению")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Логин может содержать только английские буквы и цифры")]
        [Display(Name = "Логин", Prompt = "Введите логин")]
        public string UserName { get; set; } = "";

        [Required(ErrorMessage = "Поле Пароль обязательно к заполнению")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Пароль может содержать только английские буквы и цифры")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Введите пароль")]
        public string Password { get; set; } = "";

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
