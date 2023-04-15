using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace WebApiTest.Controllers
{
    public class HomeController : Controller
    {
        #region 模拟登录

        /// <summary>
        /// Cookies 方案登录
        /// </summary>
        /// <returns></returns>
        public IActionResult AddCookie()
        {
            HttpContext.Response.Cookies.Append("Cookies", "AddCookie Add");
            return Ok("AddCookie");
        }

        public IActionResult AddMyAppCookie()
        {
            HttpContext.Response.Cookies.Append("MyAppCookie-add", "sss");
            return Ok("MyAppCookie-add");
        }


        #endregion

        #region Cookie方案 处理程序
        /// <summary>
        /// cookie方案1鉴权
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Check11()
        {
            var cookies = HttpContext.Request.Cookies["Cookies"];
            if (cookies != null)
            {
                var claimIdentity = new ClaimsIdentity("Cookies123");
                claimIdentity.AddClaim(new Claim(ClaimTypes.Name, "lishuai"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));

                //指定cookie鉴权 Cookies方案通过
                await HttpContext.SignInAsync("Cookies",new ClaimsPrincipal(claimIdentity));
                return Redirect(HttpContext.Request.Query["ReturnUrl"]);
            }
            return Ok("no cookies 1 , fail");
        }
        /// <summary>
        /// Cookies Scheme Denied Called (403)
        /// </summary>
        /// <returns></returns>
        public IActionResult Check12()
        {
            return Ok("denied 1 Fail");
        }

        public async Task<IActionResult> Check21()
        {
            var cookies = HttpContext.Request.Cookies["MyAppCookie-add"];
            if (cookies != null)
            {

                var claimIdentity = new ClaimsIdentity("CookieAuth11");
                claimIdentity.AddClaim(new Claim(ClaimTypes.Name, "lishuai"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));

                var user = new ClaimsPrincipal(claimIdentity);
                //会导致死循环重定向
                //await HttpContext.SignInAsync("Cookies", user);
                await HttpContext.SignInAsync("CookieAuth", user);
                var ru = HttpContext.Request.Query["ReturnUrl"];
                if(ru.Any())
                    return Redirect(ru.First());
                return Ok("NoReturnUrl");
            }
            return Ok("no cookies 2 , fail");
        }
        public IActionResult Check22()
        {
            return Ok("denied 2 Fail");
        }

        #endregion


        #region Cookie验证


        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public IActionResult Privacy1()
        {
            return Ok("SUCCESS   Privacy1");
        }

        //手动指定使用第二个cookie鉴权
        [Authorize(AuthenticationSchemes = "CookieAuth")]
        public IActionResult Privacy2()
        {
            return Ok("SUCCESS   Privacy2");
        }
        /// <summary>
        /// 多个鉴权方案同时使用(有一个鉴权通过就行)
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "CookieAuth,Cookies")]
        public IActionResult Privacy1And2() 
        {
            return Ok("1 and 2");
        }

        [AllowAnonymous]
        public IActionResult PrivacyAllowAnonymous()
        {
            return Ok();
        }

        #endregion



    }
}
