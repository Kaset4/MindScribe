using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindScribe.Data.UoW;
using MindScribe.Models;
using MindScribe.Repositories;
using MindScribe.ViewModels;
using MindScribe.ViewModels.EditViewModel;

namespace MindScribe.Controllers
{
    public class ArticleTagController: Controller
    {
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ArticleTagController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IMapper mapper, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }

        [Authorize]
        [Route("DeleteTag")]
        [HttpPost]
        public IActionResult DeleteTag(ArticleEditViewModel model, ArticleTag articleTag)
        {
            

            return View("DeleteTag");
        }

        [Authorize]
        [Route("ArticleEdit/{id}/CreateTag")]
        [HttpPost]
        public IActionResult CreateTag(int id, string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                // Возвращаем ошибку
                return RedirectToAction("ArticleEdit", "Article", new { id = id });
            }

            // Создаем новый объект ArticleTag
            var newTag = new ArticleTagViewModel
            {
                NameTag = tagName,
                ArticleId = id
            };

            // Сохраняем новый тег в базе данных

            var repository = _unitOfWork.GetRepository<ArticleTag>() as ArticleTagRepository;
            var repositoryArticle = _unitOfWork.GetRepository<Article>() as ArticleRepository;

            var editArticleViewModel = repositoryArticle.GetArticleById(id);

            var model = _mapper.Map<ArticleTag>(newTag);

            repository.Create(model);

            // Возвращаем успешный результат
            return RedirectToAction("ArticleEdit", "Article", new { id = id });
        }

        [Authorize]
        [Route("ArticleEdit/{Id}/EditTag")]
        [HttpPost]
        public async Task<IActionResult> EditTag(int tagId, int id, string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                return RedirectToAction("ArticleEdit", "Article", new { id = id });
            }

            var repository = _unitOfWork.GetRepository<ArticleTag>() as ArticleTagRepository;
            var model = repository.GetArticleTagById(tagId);
            var articleTag = _mapper.Map<ArticleTag>(model);

            articleTag.NameTag = newName;

            //var comment = repository.GetCommentById(commentId);

            repository.UpdateArticleTag(articleTag);

            return RedirectToAction("ArticleEdit", "Article", new { id = id }); // Перенаправление на страницу статьи
        }

        [Authorize]
        [Route("ArticleEdit/{Id}/DeleteTag")]
        [HttpPost]
        public async Task<IActionResult> DeleteTag(int tagId, int id)
        {
            var repository = _unitOfWork.GetRepository<ArticleTag>() as ArticleTagRepository;

            var model = repository.GetArticleTagById(tagId);

            repository.Delete(model);

            return RedirectToAction("ArticleEdit", "Article", new { id = id });// Перенаправление на страницу статьи
        }
    }
}
