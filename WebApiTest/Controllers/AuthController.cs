using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Web;
using WebApiTest.Utility;

namespace WebApiTest.Controllers
{
    [Route("/api/[controller]/[action]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> logger;
        public AuthController(ILogger<AuthController> logger)
        {
            this.logger = logger;
        }
        [HttpGet]
        public IActionResult Helper()
        {
            var str=new StringBuilder();

            str.AppendLine("鉴权： ");
            str.AppendLine("                Authorize中Scheme任一通过即可,即Cookie鉴权中SignIn和其他鉴权AuthenticateAsync返回AuthenticateResult.Success(注：提前的HttpContext.Response.Redirect会导致鉴权挑战提前终止)");
            
            str.AppendLine("鉴权顺序优先级：");
            str.AppendLine("                没指定scheme使用最后一个调用的AddAuthentication defaultScheme");
            str.AppendLine("                cookie和自定义并存  先自定义再cookie（自定义重定向则不指定cookie中配置loginPath）");
            str.AppendLine("                cookie和jwt  只执行jwt");
            str.AppendLine("                多个自定义，按照authorize顺序执行方案");

            return Ok(str.ToString());
        }
        [HttpGet]
        [Authorize]
        public IActionResult Default()
        {
            return Ok();
        }

        #region Cookies

        [HttpGet]

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public IActionResult Privacy1()
        {
            return Ok("SUCCESS   Privacy1");
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = Consts.CookiesAuth)]
        public IActionResult Privacy2()
        {
            return Ok("SUCCESS   Privacy2");
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = $"{Consts.CookiesAuth},{Consts.Cookies}")]
        public IActionResult Privacy1And2()
        {
            return Ok("1 and 2");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult PrivacyAllowAnonymous()
        {
            return Ok();
        }

        #endregion

        #region UrlToken

        #region Consts.UrlTokenScheme鉴权 执行AuthenticateAsync 返回AuthenticateResult.Success 则通过鉴权
        [HttpGet]
        [Authorize(AuthenticationSchemes = Consts.UrlTokenScheme)]
        public async Task<IActionResult> UrlToken()
        {
            #region 手动鉴权 == [Authorize]
            var result = await HttpContext.AuthenticateAsync(Consts.UrlTokenScheme);

            if (result?.Principal == null)
            {
                return new JsonResult(new
                {
                    Result = false,
                    Message = "认证失败,未登录"
                });
            }
            else
            {
                HttpContext.User = result.Principal;
                foreach (var item in HttpContext.User.Identities.First().Claims)
                {
                    logger.LogInformation($"claim: {item.Type} {item.Value}");
                }
                return new JsonResult(new
                {
                    Result = true,
                    Message = "认证成功"
                });
            }
            #endregion
        }
        #endregion

        #region 角色授权
        /// <summary>
        /// https://localhost:7027/api/Auth/AdminRole?UrlToken=lishuai  成功
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = Consts.Admin)]
        //[Authorize(Roles = "Admin,User")]//admin or User
        public async Task<IActionResult> AdminRole()
        {
            return Ok(nameof(AdminRole));
        }
        /// <summary>
        /// https://localhost:7027/api/Auth/UserRole?UrlToken=lishuai  被403   (执行鉴权中的 ForbidAsync)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = Consts.User)]
        public async Task<IActionResult> UserRole()
        {
            return Ok(nameof(UserRole));
        }

        #endregion


        #region 自定义授权
        [HttpGet]
        //[Authorize(Policy = Consts.Custom-Policy)]  //equals  [Authorize(Roles = Consts.Admin)]    
        //or
        [Authorize(Policy = Consts.AssertionAdminPolicy)]
        public async Task<IActionResult> AdminPolicy()
        {
            return Ok(nameof(AdminPolicy));
        }


        #endregion

        #endregion

        #region JWTToken


        [HttpGet]
        public string NoJwtToken()
        {
            return nameof(NoJwtToken);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes =Consts.Bearer)]
        public string NeedJwtToken()
        {
            return nameof(NeedJwtToken);
        }

        #endregion

        #region UrlTokenAndCookieAndJWT
        [HttpGet]
        [Authorize(AuthenticationSchemes = Consts.UrlTokenScheme)]
        public async Task<IActionResult> UrlTokenScheme()
        {
            return Ok(nameof(UrlTokenScheme));
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = $"{Consts.Cookies},{Consts.UrlTokenScheme}")]
        public async Task<IActionResult> CookiesAndUrlTokenScheme()
        {
            return Ok(nameof(CookiesAndUrlTokenScheme));
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = $"{Consts.UrlTokenScheme},{Consts.Cookies}")]
        public async Task<IActionResult> UrlTokenAndCookiesScheme()
        {
            return Ok(nameof(UrlTokenAndCookiesScheme));
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = $"{Consts.UrlTokenScheme},{Consts.UrlTokenScheme2}")]
        public async Task<IActionResult> UrlTokenAnd2Scheme()
        {
            return Ok(nameof(UrlTokenAnd2Scheme));
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = $"{Consts.Cookies},{Consts.UrlTokenScheme2},{Consts.UrlTokenScheme}")]
        public async Task<IActionResult> CookiesAndUrlToken2AndUrlTokenScheme()
        {
            return Ok(nameof(CookiesAndUrlToken2AndUrlTokenScheme));
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = $"{Consts.UrlTokenScheme2},{Consts.UrlTokenScheme},{Consts.Cookies}")]
        public async Task<IActionResult> UrlToken2AndUrlTokenAndCookiesScheme()
        {
            return Ok(nameof(UrlToken2AndUrlTokenAndCookiesScheme));
        }
        #endregion




        #region 认证接口

        #region Cookies
        /// <summary>
        /// cookie方案1鉴权
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Check11()
        {
            var cookies = HttpContext.Request.Cookies["Cookies"];
            if (cookies != null)
            {
                //解析cookies 获取信息，判断是否有权限

                var claimIdentity = new ClaimsIdentity("Cookies123");
                //claimIdentity.AddClaim(new Claim(ClaimTypes.Name, "lishuai"));
                //claimIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));

                //指定cookie鉴权 Cookies方案通过
                await HttpContext.SignInAsync(Consts.Cookies, new ClaimsPrincipal(claimIdentity));
                var ru = HttpContext.Request.Query["ReturnUrl"];
                if (ru.Any())
                    return Redirect(HttpUtility.UrlDecode(ru.First() ?? "/"));
                return Redirect("/");
            }
            else
            {
                return Unauthorized();
                return Ok("cookies 1 ,未通过鉴权");
            }
        }
        /// <summary>
        /// Cookies Scheme Denied Called (403)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        private IActionResult Check12()
        {
            return StatusCode(403);
            return Ok("cookies 1 ，授权未通过");
        }

        [HttpGet]
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
                await HttpContext.SignInAsync(Consts.CookiesAuth, user);
                var ru = HttpContext.Request.Query["ReturnUrl"];
                if (ru.Any())
                    return Redirect(ru.First() ?? "/");
                return Ok("NoReturnUrl");
            }
            return Ok("cookies 2 ,鉴权未通过");
        }
        [HttpGet]
        public IActionResult Check22()
        {
            return Ok("cookies 2 , 授权未通过");
        }
        #endregion

        #region CustomAuthorizeHandle
        [HttpGet]
        public IActionResult Login()
        {
            return Ok("Auth 未通过鉴权，请登录");
        }

        [HttpGet]
        public IActionResult Denied()
        {
            return Ok("Auth  授权未通过");
        }
        #endregion

        #endregion


    }
}
