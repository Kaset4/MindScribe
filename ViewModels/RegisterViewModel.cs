using System.ComponentModel.DataAnnotations;

namespace MindScribe.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле Имя обязательно к заполнению")]
        [Display(Name = "Имя", Prompt = "Введите Имя")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Поле Фамилия обязательно к заполнению")]
        [Display(Name = "Фамилия", Prompt = "Введите Фамилию")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Поле Email обязательно к заполнению")]
        [Display(Name = "Email", Prompt = "Введите Почту")]
        public string EmailReg { get; set; } = "";


        [Required(ErrorMessage = "Поле Пароль обязательно к заполнению")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Пароль может содержать только английские буквы и цифры")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Введите Пароль")]
        [StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 5)]
        public string PasswordReg { get; set; } = "";

        [Required(ErrorMessage = "Поле Подтвердить пароль обязательно к заполнению")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Пароль может содержать только английские буквы и цифры")]
        [Compare("PasswordReg", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль", Prompt = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; } = "";

        [Required(ErrorMessage = "Поле Логин обязательно к заполнению")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Логин может содержать только английские буквы и цифры")]
        [Display(Name = "Логин", Prompt = "Введите Логин")]
        public string Login { get; set; } = "";
    }
}
