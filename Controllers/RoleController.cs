using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindScribe.Data.UoW;
using MindScribe.Models;

namespace MindScribe.Controllers
{
    public class RoleController: Controller
    {
        //private readonly IMapper _mapper;

        //private readonly UserManager<User> _userManager;
        //private readonly SignInManager<User> _signInManager;
        //private readonly UnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            //_userManager = userManager;
            //_signInManager = signInManager;
            //_mapper = mapper;
            //_unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }

        [Route("Role")]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Role()
        {
            return View("Role");
        }

        [Route("CreateRole")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (ModelState.IsValid)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        // Роль успешно создана
                        return RedirectToAction("Role");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Роль с таким именем уже существует");
                }
            }
            return View("Role");
        }

        [Route("DeleteRole")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        // Роль успешно удалена
                        return RedirectToAction("Role");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Роль не найдена");
                }
            }
            return View("Role");
        }

        [Route("EditRole")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditRole(string roleName, string newRoleName)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    role.Name = newRoleName; // Обновляем имя роли
                    var result = await _roleManager.UpdateAsync(role); // Обновляем роль в базе данных
                    if (result.Succeeded)
                    {
                        // Роль успешно отредактирована
                        return RedirectToAction("Role");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Роль не найдена");
                }
            }
            return View("Role");
        }
    }
}
