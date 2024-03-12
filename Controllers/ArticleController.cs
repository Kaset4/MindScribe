using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindScribe.Data.UoW;
using MindScribe.Models;
using MindScribe.Repositories;
using MindScribe.ViewModels;
using MindScribe.ViewModels.EditViewModel;
using MindScribe.ViewModels.FromModels;

namespace MindScribe.Controllers
{
    public class ArticleController: Controller
    {
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ArticleController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IMapper mapper, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }

        [Authorize]
        [Route("NewArticle")]
        [HttpGet]
        public IActionResult NewArticle()
        {
            return View("NewArticle");
        }

        [Route("Article/{id}")]
        [HttpGet]
        public IActionResult Index(int id)
        {
            ArticleRepository? repository = _unitOfWork.GetRepository<Article>() as ArticleRepository;

            if (repository != null)
            {
                var article = repository.GetArticleById(id);
                if (article != null)
                {
                    var model = _mapper.Map<ArticleViewModel>(article);
                    ViewData["UserManager"] = _userManager;
                    return View("Article", model);
                }
                else
                {
                    // Обработка случая, когда статья не найдена
                    return NotFound();
                }
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                return NotFound();
            }
        }

        [Authorize]
        [Route("CreateArticle")]
        [HttpPost]
        public async Task<IActionResult> CreateArticle(ArticleViewModel model)
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            if (result != null)
            {
                var userModel = new UserViewModel(result);

                model.User_id = userModel.User.Id;
                model.Created_at = DateTime.Now;
                model.Updated_at = model.Created_at;

                var article = _mapper.Map<Article>(model);

                var repository = _unitOfWork.GetRepository<Article>() as ArticleRepository;

                if (repository != null)
                {
                    if (model != null)
                    {
                        repository.CreateArticle(article);
                        return RedirectToAction("Index", new { id = article.Id });
                    }
                    else
                    {
                        // Обработка случая, когда модель не была передана
                        return BadRequest("Модель статьи не была передана.");
                    }
                }
                else
                {
                    // Обработка случая, когда репозиторий не найден
                    return NotFound();
                }
            }
            else
            {
                // Обработка случая, когда пользователь не найден
                return NotFound("Пользователь не найден.");
            }
        }

        [Authorize]
        [Route("DeleteArticle")]
        [HttpPost]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            if (result != null)
            {
                var userModel = new UserViewModel(result);

                ArticleRepository? repository = _unitOfWork.GetRepository<Article>() as ArticleRepository;

                if (repository != null)
                {
                    var model = repository.GetArticleById(id);
                    if (model != null)
                    {
                        var article = _mapper.Map<Article>(model);
                        if (userModel.User.Id == article.User_id || User.IsInRole("Admin"))
                        {
                            repository.Delete(article);
                            return RedirectToAction("MyPage", "AccountManager");
                        }
                        else
                        {
                            // Обработка случая, когда пользователь не имеет прав на удаление статьи
                            return Forbid();
                        }
                    }
                    else
                    {
                        // Обработка случая, когда статья не найдена
                        return NotFound();
                    }
                }
                else
                {
                    // Обработка случая, когда репозиторий не найден
                    return NotFound();
                }
            }
            else
            {
                // Обработка случая, когда пользователь не найден
                return NotFound("Пользователь не найден.");
            }
        }

        [Authorize]
        [Route("ArticleEdit/{id}")]
        [HttpPost]
        public IActionResult ArticleEdit(int id)
        {
            var repository = _unitOfWork.GetRepository<Article>() as ArticleRepository;

            if (repository != null)
            {
                var article = repository.GetArticleById(id);

                if (article != null)
                {
                    var editArticleViewModel = new ArticleEditViewModel()
                    {
                        Article_Id = id,
                        Content_article = article.Content_article,
                        Title = article.Title,
                        Created_at = (article.Created_at as DateTime?) ?? DateTime.MinValue,
                        Updated_at = (article.Updated_at as DateTime?) ?? DateTime.MinValue,
                        Tags = article.Tags
                    };

                    ViewData["UserManager"] = _userManager;

                    return View("ArticleEdit", editArticleViewModel);
                }
                else
                {
                    // Обработка случая, когда статья не найдена
                    return NotFound();
                }
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                return NotFound();
            }
        }

        [Authorize]
        [Route("ArticleEdit/{id}")]
        [HttpGet]
        public IActionResult ArticleEditGet(int id)
        {
            var repository = _unitOfWork.GetRepository<Article>() as ArticleRepository;

            if (repository != null)
            {
                var article = repository.GetArticleById(id);

                if (article != null)
                {
                    var editArticleViewModel = new ArticleEditViewModel()
                    {
                        Article_Id = id,
                        Content_article = article.Content_article,
                        Title = article.Title,
                        Created_at = (article.Created_at as DateTime?) ?? DateTime.MinValue,
                        Updated_at = (article.Updated_at as DateTime?) ?? DateTime.MinValue,
                        Tags = article.Tags
                    };

                    ViewData["UserManager"] = _userManager;

                    return View("ArticleEdit", editArticleViewModel);
                }
                else
                {
                    // Обработка случая, когда статья не найдена
                    return NotFound();
                }
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                return NotFound();
            }
        }

        [Authorize]
        [Route("ArticleUpdate")]
        [HttpPost]
        public async Task<IActionResult> ArticleUpdate(ArticleEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = await _userManager.FindByIdAsync(model.id);

                var user = User;

                var tempUser = await _userManager.GetUserAsync(user);

                var repository = _unitOfWork.GetRepository<Article>() as ArticleRepository;

                if (repository != null)
                {
                    var article = repository.GetArticleById(model.Article_Id);

                    if (article != null && tempUser != null && article.User_id == tempUser.Id)
                    {
                        article.Updated_at = DateTime.Now;
                        article.Title = model.Title;
                        article.Content_article = model.Content_article;

                        repository.UpdateArticle(article);

                        return RedirectToAction("Index", new { id = article.Id });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Статья не найдена или вы не являетесь её владельцем.");
                        return View("ArticleEdit", model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Репозиторий статей не найден.");
                    return View("ArticleEdit", model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
                return View("ArticleEdit", model);
            }
        }
    }
}
