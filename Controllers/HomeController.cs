using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using MindScribe.Models;
using MindScribe.ViewModels;
using NLog;
using System.Diagnostics;

namespace MindScribe.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Logger LoggerAction = LogManager.GetLogger("HomeController");
        private static readonly Logger LoggerError = LogManager.GetLogger("HomeController");


        [Route("")]
        [Route("[controller]/[action]")]
        public IActionResult Index()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                LoggerAction.Info("Переход на MyPage-AccountManager.");
                return RedirectToAction("MyPage", "AccountManager");
            }

            LoggerAction.Info("Переход на Index.");
            return View(new MainViewModel());
        }

        [Route("[action]")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            LoggerError.Info($"Ошибка на {new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }}");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
