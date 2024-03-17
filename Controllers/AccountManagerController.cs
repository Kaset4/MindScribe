using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindScribe.Data.UoW;
using MindScribe.Models;
using MindScribe.Repositories;
using MindScribe.ViewModels;
using MindScribe.ViewModels.EditViewModel;
using MindScribe.ViewModels.FromModel;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace MindScribe.Controllers
{
    public class AccountManagerController : Controller
    {
        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UnitOfWork _unitOfWork;

        private static readonly Logger LoggerAction = LogManager.GetLogger("HomeController");
        private static readonly Logger LoggerError = LogManager.GetLogger("HomeController");

        public AccountManagerController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            LoggerAction.Info("Переход на View(Login)");
            return View("Login");
        }

        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit()
        {
            LoggerAction.Info("Переход на View(editViewModel).");
            var currentUser = _userManager.GetUserAsync(User).Result;

            if (currentUser != null)
            {
                var editViewModel = new UserEditViewModel
                {
                    UserId = currentUser.Id,
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Email = currentUser.Email != null ? currentUser.Email : "",
                    Image = currentUser.Image,
                    About = currentUser.About
                };

                LoggerAction.Info("Успешный поиск текущего пользователя.");
                return View(editViewModel);
            }
            else
            {
                // Обработка случая, когда текущий пользователь не найден
                LoggerAction.Info("Пользователь не найден.");
                LoggerError.Info("Ошибка. Пользователь не найден.");
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            LoggerAction.Info("Переход на View(new LoginViewModel { ReturnUrl = returnUrl })");
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            LoggerAction.Info("Процесс авторизации пользователя.");
            if (ModelState.IsValid)
            {

                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var loggedInUser = await _userManager.FindByNameAsync(model.UserName);

                    // Получаем роли пользователя
                    if (loggedInUser != null)
                    {
                        var roles = await _userManager.GetRolesAsync(loggedInUser);

                        // Создаем клеймы для ролей пользователя
                        var claims = new List<Claim>();
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        // Добавляем клеймы в утверждения пользователя
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        // Обновляем контекст аутентификации
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            LoggerAction.Info("Успешная авторизация. Переход на MyPage-AccountManager");
                            return RedirectToAction("MyPage", "AccountManager");
                        }
                    }
                }
                else
                {
                    LoggerAction.Info("Неудачаня авторизации(неправильный логин и (или) пароль).");
                    LoggerError.Info("Ошибка. Неудачаня авторизации(неправильный логин и (или) пароль).");
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    return View("Login", model);
                }
            }
            return View("Login", model);
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            LoggerAction.Info("Выход из аккаунта. Переход на Index-Home");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [Route("MyPage")]
        [HttpGet]
        public async Task<IActionResult> MyPage()
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            if (result != null)
            {
                LoggerAction.Info("Успешный поиск пользователя. Переход на View(\"User\", model)");
                var model = new UserViewModel(result);
                model.Articles = await GetAllArticleByAuthor(model.User);
                return View("User", model);
            }
            else
            {
                // Обработка случая, когда пользователь не найден
                LoggerAction.Info("Пользователь не найден.");
                LoggerError.Info("Ошибка. Пользователь не найден.");
                return NotFound("Пользователь не найден.");
            }
        }

        [Authorize]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user != null)
                {
                    user.Convert(model);

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        LoggerAction.Info("Успешное обновление данных пользователя. Переход на MyPage-AccountManager");
                        return RedirectToAction("MyPage", "AccountManager");
                    }
                    else
                    {
                        LoggerAction.Info("Данные не были обновлены. Перенаправление на Edit-AccountManager.");
                        LoggerError.Info("Данные не были обновлены. Ошибка введенных данных.");
                        return RedirectToAction("Edit", "AccountManager");
                    }
                }
                else
                {
                    // Обработка случая, когда пользователь не найден
                    LoggerAction.Info("Пользователь не найден.");
                    LoggerError.Info("Пользователь не найден.");
                    return NotFound("Пользователь не найден.");
                }
            }
            else
            {
                LoggerAction.Info("Данные не были обновлены. Перенаправление на View(\"Edit\", model).");
                LoggerError.Info("Некорректные данные. Ошибка введенных данных.");
                ModelState.AddModelError("", "Некорректные данные");
                return View("Edit", model);
            }
        }

        //Register
        [Route("Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByNameAsync(model.Login);
                if (userExists != null)
                {
                    LoggerAction.Info("Неудачная регистрация. Перенаправление на View(\"Register\", model).");
                    LoggerError.Info("Ошибка. Логин уже занят. Ошибка введенных данных.");
                    ModelState.AddModelError("", "Логин уже занят");
                    return View("Register", model);
                }

                var emailExists = await _userManager.FindByEmailAsync(model.EmailReg);
                if (emailExists != null)
                {
                    LoggerAction.Info("Неудачная регистрация. Перенаправление на View(\"Register\", model).");
                    LoggerError.Info("Ошибка. Email уже занят. Ошибка в веденных данных.");
                    ModelState.AddModelError("", "Пользователь с такой почтой уже зарегистрирован");
                    return View("Register", model);
                }


                var user = _mapper.Map<User>(model);

                var result = await _userManager.CreateAsync(user, model.PasswordReg);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    await _signInManager.SignInAsync(user, false);
                    LoggerAction.Info("Удачная регистрация. Перенаправление на MyPage-AccountManager.");
                    return RedirectToAction("MyPage", "AccountManager");
                }
                else
                {
                    LoggerError.Info("Ошибка. Неудачная регистрация.");
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View("Register", model);
        }



        // /Register

        

        private async Task<List<Article>> GetAllArticleByAuthor(User user)
        {
            LoggerAction.Info("Поиск всех статей пользователя.");
            return await Task.Run(() =>
            {
                var repository = _unitOfWork.GetRepository<Article>() as ArticleRepository;

                if (repository != null)
                {
                    return repository.GetArticlesByAuthorId(user);
                }
                else
                {
                    // Обработка случая, когда репозиторий не найден
                    LoggerError.Info("Ошибка. Репозиторий не найден.");
                    return new List<Article>(); // или возвращайте null или выбрасывайте исключение
                }
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051:Закрытые члены не используются", Justification = "<Может быть дальше понадобится.>")]
        private async Task<List<Article>> GetAllArticleByAuthor()
        {
            LoggerAction.Info("Поиск всех статей пользователя.");
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            if (result != null)
            {
                var repository = _unitOfWork.GetRepository<Article>() as ArticleRepository;

                if (repository != null)
                {
                    return repository.GetArticlesByAuthorId(result);
                }
                else
                {
                    // Обработка случая, когда репозиторий не найден
                    LoggerError.Info("Ошибка. Репозиторий не найден.");
                    return new List<Article>(); // или возвращайте null или выбрасывайте исключение
                }
            }
            else
            {
                // Обработка случая, когда пользователь не найден
                LoggerError.Info("Ошибка. Пользователь не найден.");
                return new List<Article>(); // или возвращайте null или выбрасывайте исключение
            }
        }

        // Admin

        [Route("AdminPanel")]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AdminPanel()
        {
            LoggerError.Info("Переход на View(\"AdminPanel\", users).");
            var users = await _userManager.Users.ToListAsync();
            return View("AdminPanel", users);
        }

        [Route("DeleteUser")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {

            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var repository = _unitOfWork.GetRepository<User>() as UserRepository;

                if (repository != null)
                {
                    LoggerError.Info("Пользователь успешно удалён из базы.");
                    repository.DeleteUser(user);
                }
                else
                {
                    // Обработка случая, когда репозиторий не найден
                    LoggerError.Info("Ошибка. Репозиторий пользователей не найден.");
                    return NotFound("Репозиторий пользователей не найден.");
                }
            }
            else
            {
                // Обработка случая, когда пользователь не найден
                LoggerError.Info("Пользователей не найден.");
                LoggerError.Info("Ошибка. Пользователей не найден.");
                return NotFound("Пользователь не найден.");
            }

            return RedirectToAction("AdminPanel", "AccountManager");

        }

        // /Admin
    }
}
