using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindScribe.Data.UoW;
using MindScribe.Models;
using MindScribe.Repositories;
using MindScribe.ViewModels;
using MindScribe.ViewModels.EditViewModel;
using NLog;
using NLog.Fluent;

namespace MindScribe.Controllers
{
    public class CommentController: Controller
    {
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;

        private static readonly Logger LoggerAction = LogManager.GetLogger("CommentController");
        private static readonly Logger LoggerError = LogManager.GetLogger("CommentController");

        public CommentController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IMapper mapper, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }
        [Authorize]
        [Route("Article/{Id}/SendComment")]
        [HttpPost]
        public async Task<IActionResult> SendComment(int Id, CommentViewModel model)
        {
            LoggerAction.Info("Попытка отправки комментария.");
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            if (result == null)
            {
                // Обработка случая, когда пользователь не найден
                LoggerError.Info("Ошибка. Пользователь не найден.");
                return NotFound("Пользователь не найден.");
            }

            var userModel = new UserViewModel(result);

            model.User_id = userModel.User.Id;
            model.Created_at = DateTime.Now;
            model.Updated_at = DateTime.Now;
            model.ArticleId = Id;

            

            var repository = _unitOfWork.GetRepository<Comment>() as CommentRepository;

            if (repository != null)
            {
                var commentModel = _mapper.Map<Comment>(model);

                LoggerAction.Info("Успешное Создание комментария.");
                repository.CreateComment(commentModel);
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                LoggerError.Info("Ошибка. Репозиторий комментариев не найден.");
                return NotFound("Репозиторий комментариев не найден.");
            }

            return RedirectToAction("Index", "Article", new { id = Id }); // Перенаправление на страницу статьи
        }

        [Authorize]
        [Route("Article/{Id}/DeleteComment")]
        [HttpPost]
        public IActionResult DeleteComment(int Id, int commentId)
        {
            LoggerAction.Info("Попытка удаления комментария.");

            var repository = _unitOfWork.GetRepository<Comment>() as CommentRepository;

            if (repository != null)
            {
                var comment = repository.GetCommentById(commentId);
                if (comment != null)
                {
                    repository.Delete(comment);
                    LoggerAction.Info("Успешное удаление комментария.");
                }
                else
                {
                    // Обработка случая, когда комментарий не найден
                    LoggerError.Info("Ошибка. Комментарий не найден.");
                    return NotFound("Комментарий не найден.");
                }
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                LoggerError.Info("Ошибка. Репозиторий не найден.");
                return NotFound("Репозиторий комментариев не найден.");
            }

            return RedirectToAction("Index", "Article", new { id = Id }); // Перенаправление на страницу статьи
        }

        [Authorize]
        [Route("Article/{Id}/EditComment")]
        [HttpPost]
        public IActionResult EditComment(int Id, int commentId, string content)
        {
            LoggerAction.Info("Попытка изменения комментария.");
            if (string.IsNullOrEmpty(content))
            {
                return RedirectToAction("Index", "Article", new { id = Id });
            }

            CommentRepository? repository = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            if (repository != null)
            {
                var model = repository.GetCommentById(commentId);
                if (model != null)
                {
                    var comment = _mapper.Map<Comment>(model);
                    comment.Content_comment = content;
                    comment.Updated_at = DateTime.Now;
                    repository.UpdateComment(comment);
                    LoggerAction.Info("Успешное изменение комментария.");
                }
                else
                {
                    // Обработка случая, когда комментарий не найден
                    LoggerError.Info("Ошибка. Комментарий не найден.");
                    return NotFound("Комментарий не найден.");
                }
            }
            else
            {
                // Обработка случая, когда репозиторий не найден
                LoggerError.Info("Ошибка. Репозиторий комментариев не найден.");
                return NotFound("Репозиторий комментариев не найден.");
            }

            return RedirectToAction("Index", "Article", new { id = Id }); // Перенаправление на страницу статьи
        }
    }
}
