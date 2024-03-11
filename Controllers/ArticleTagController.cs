using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindScribe.Data.UoW;
using MindScribe.Models;
using MindScribe.Repositories;
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
        [Route("CreateTag")]
        [HttpPost]
        public IActionResult CreateTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                // Возвращаем ошибку
                return BadRequest("Неверное имя тега");
            }

            // Создаем новый объект ArticleTag
            var newTag = new ArticleTag { NameTag = tagName };

            // Сохраняем новый тег в базе данных

            var repository = _unitOfWork.GetRepository<ArticleTag>() as ArticleTagRepository;
            repository.Create(newTag);

            // Возвращаем успешный результат
            return Ok(newTag);
        }
    }
}
