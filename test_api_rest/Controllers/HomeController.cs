using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using test_api_rest.Models;
using test_api_rest.Services.Interfaces;

namespace test_api_rest.Controllers
{
    public class HomeController(ILoginService loginService, ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly ILoginService _loginService = loginService;


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] User user)
        {
            UserAccessData userAccessData = await _loginService.LoginAuthentication(user);

            HttpContext.Session.SetString("UserAccesData", JsonSerializer.Serialize(userAccessData));            

            return RedirectToAction("Index", "Request");
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
