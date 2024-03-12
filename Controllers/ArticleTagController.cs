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
        private readonly IMapper _mapper;

        private readonly UnitOfWork _unitOfWork;

        public ArticleTagController(IMapper mapper, UnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [Route("DeleteTag")]
        [HttpPost]
        public IActionResult DeleteTag()
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
                return RedirectToAction("ArticleEdit", "Article", new { id });
            }

            // Создаем новый объект ArticleTag
            var newTag = new ArticleTagViewModel
            {
                NameTag = tagName,
                ArticleId = id
            };

            // Сохраняем новый тег в базе данных

            var repository = _unitOfWork.GetRepository<ArticleTag>() as ArticleTagRepository;

            if (repository != null)
            {
                var model = _mapper.Map<ArticleTag>(newTag);
                repository.Create(model);
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                return NotFound("Репозиторий не найден.");
            }

            // Возвращаем успешный результат
            return RedirectToAction("ArticleEdit", "Article", new { id });
        }

        [Authorize]
        [Route("ArticleEdit/{Id}/EditTag")]
        [HttpPost]
        public IActionResult EditTag(int tagId, int id, string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                return RedirectToAction("ArticleEdit", "Article", new { id });
            }

            var repository = _unitOfWork.GetRepository<ArticleTag>() as ArticleTagRepository;

            if (repository != null)
            {
                var model = repository.GetArticleTagById(tagId);
                if (model != null)
                {
                    var articleTag = _mapper.Map<ArticleTag>(model);
                    articleTag.NameTag = newName;

                    repository.UpdateArticleTag(articleTag);

                    return RedirectToAction("ArticleEdit", "Article", new { id }); // Перенаправление на страницу статьи
                }
                else
                {
                    // Обработка случая, когда тег не найден
                    return NotFound("Тег не найден.");
                }
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                return NotFound("Репозиторий не найден.");
            }
        }

        [Authorize]
        [Route("ArticleEdit/{Id}/DeleteTag")]
        [HttpPost]
        public IActionResult DeleteTag(int tagId, int id)
        {
            var repository = _unitOfWork.GetRepository<ArticleTag>() as ArticleTagRepository;

            if (repository != null)
            {
                var model = repository.GetArticleTagById(tagId);
                if (model != null)
                {
                    repository.Delete(model);
                    return RedirectToAction("ArticleEdit", "Article", new { id }); // Перенаправление на страницу статьи
                }
                else
                {
                    // Обработка случая, когда тег не найден
                    return NotFound("Тег не найден.");
                }
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                return NotFound("Репозиторий не найден.");
            }
        }
    }
}
