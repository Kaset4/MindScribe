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

namespace MindScribe.Controllers
{
    public class AccountManagerController : Controller
    {
        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UnitOfWork _unitOfWork;

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
            return View("Login");
        }

        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit()
        {
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

                return View(editViewModel);
            }
            else
            {
                // Обработка случая, когда текущий пользователь не найден
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

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

                            return RedirectToAction("MyPage", "AccountManager");
                        }
                    }
                }
                else
                {
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
                var model = new UserViewModel(result);
                model.Articles = await GetAllArticleByAuthor(model.User);
                return View("User", model);
            }
            else
            {
                // Обработка случая, когда пользователь не найден
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
                        return RedirectToAction("MyPage", "AccountManager");
                    }
                    else
                    {
                        return RedirectToAction("Edit", "AccountManager");
                    }
                }
                else
                {
                    // Обработка случая, когда пользователь не найден
                    return NotFound("Пользователь не найден.");
                }
            }
            else
            {
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
                    ModelState.AddModelError("", "Логин уже занят");
                    return View("Register", model);
                }

                var emailExists = await _userManager.FindByEmailAsync(model.EmailReg);
                if (emailExists != null)
                {
                    ModelState.AddModelError("", "Пользователь с такой почтой уже зарегистрирован");
                    return View("Register", model);
                }


                var user = _mapper.Map<User>(model);

                var result = await _userManager.CreateAsync(user, model.PasswordReg);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("MyPage", "AccountManager");
                }
                else
                {
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
                    return new List<Article>(); // или возвращайте null или выбрасывайте исключение
                }
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051:Закрытые члены не используются", Justification = "<Может быть дальше понадобится.>")]
        private async Task<List<Article>> GetAllArticleByAuthor()
        {
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
                    return new List<Article>(); // или возвращайте null или выбрасывайте исключение
                }
            }
            else
            {
                // Обработка случая, когда пользователь не найден
                return new List<Article>(); // или возвращайте null или выбрасывайте исключение
            }
        }

        // Admin

        [Route("AdminPanel")]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AdminPanel()
        {
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
                    repository.DeleteUser(user);
                }
                else
                {
                    // Обработка случая, когда репозиторий не найден
                    // Можно выбросить исключение, вернуть пользователю сообщение об ошибке или выполнить другие действия
                    return NotFound("Репозиторий пользователей не найден.");
                }
            }
            else
            {
                // Обработка случая, когда пользователь не найден
                // Можно выбросить исключение, вернуть пользователю сообщение об ошибке или выполнить другие действия
                return NotFound("Пользователь не найден.");
            }

            return RedirectToAction("AdminPanel", "AccountManager");

        }

        // /Admin
    }
}
