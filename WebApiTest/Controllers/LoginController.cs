using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiTest.Utility;

namespace WebApiTest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> logger;
        private readonly JwtHelper _jwtHelper;
        public LoginController(ILogger<LoginController> logger,JwtHelper jwtHelper)
        {
            this.logger = logger;
            _jwtHelper = jwtHelper;
        }

        #region 模拟登录

        [HttpGet]
        public IActionResult AddCookie()
        {
            HttpContext.Response.Cookies.Append("Cookies", "AddCookie Add");
            return Ok("AddCookie");
        }
        [HttpGet]
        public IActionResult AddMyAppCookie()
        {
            HttpContext.Response.Cookies.Append("MyAppCookie-add", "sss");
            return Ok("MyAppCookie-add");
        }

        [HttpGet]
        public string CreateJWTToken()
        {
            return _jwtHelper.CreateToken();
        }

        #endregion

        #region 模拟退出
        [HttpGet]
        public IActionResult LogOutCookies()
        {
            HttpContext.Response.Cookies.Delete(Consts.Cookies);
            HttpContext.SignOutAsync(Consts.Cookies);
            return Ok();
        }

        #endregion
    }
}
