using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiTest.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddCookie()
        {
            HttpContext.Response.Cookies.Append("Cookie", "AddCookie Add");
            return Ok();
        }

        //无cookie访问，跳转至https://localhost:5001/Home/Index?ReturnUrl=%2FHome%2FPrivacy
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult PrivacyAllowAnonymous()
        {
            return Ok();
        }
    }
}
