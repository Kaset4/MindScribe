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
    public class CommentController: Controller
    {
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;

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
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            var userModel = new UserViewModel(result);

            model.User_id = userModel.User.Id;
            model.Created_at = DateTime.Now;
            model.Updated_at = DateTime.Now;
            model.ArticleId = Id;

            // Далее ваша логика сохранения комментария с articleId

            var repository = _unitOfWork.GetRepository<Comment>() as CommentRepository;

            var commentModel = _mapper.Map<Comment>(model);

            repository.CreateComment(commentModel);

            return RedirectToAction("Index", "Article", new { id = Id }); // Перенаправление на страницу статьи
        }

        [Authorize]
        [Route("Article/{Id}/DeleteComment")]
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int Id, int commentId)
        {
            var repository = _unitOfWork.GetRepository<Comment>() as CommentRepository;

            var comment = repository.GetCommentById(commentId);

            repository.Delete(comment);

            return RedirectToAction("Index", "Article", new { id = Id }); // Перенаправление на страницу статьи
        }

        [Authorize]
        [Route("Article/{Id}/EditComment")]
        [HttpPost]
        public async Task<IActionResult> EditComment(int Id, int commentId, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return RedirectToAction("Index", "Article", new { id = Id });
            }

            var repository = _unitOfWork.GetRepository<Comment>() as CommentRepository;
            var model = repository.GetCommentById(commentId);
            var comment = _mapper.Map<Comment>(model);

            comment.Content_comment = content;
            comment.Updated_at = DateTime.Now;

            //var comment = repository.GetCommentById(commentId);

            repository.UpdateComment(comment);

            return RedirectToAction("Index", "Article", new { id = Id }); // Перенаправление на страницу статьи
        }
    }
}
