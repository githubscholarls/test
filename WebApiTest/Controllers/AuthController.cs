using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebApiTest.Utility;

namespace WebApiTest.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> logger;
        public AuthController(ILogger<AuthController> logger)
        {
            this.logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region UrlToken

        [Authorize]
        public async Task<IActionResult> UrlToken()
        {
            #region 手动鉴权 == [Authorize]
            var result = await HttpContext.AuthenticateAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
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

        #region 角色授权
        /// <summary>
        /// https://localhost:7027/Auth/AdminRole?UrlToken=lishuai  成功
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        //[Authorize(Roles = "Admin,User")]//admin or User
        public async Task<IActionResult> AdminRole()
        {
            return Ok(nameof(AdminRole));
        }
        /// <summary>
        /// https://localhost:7027/Auth/UserRole?UrlToken=lishuai  被403
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UserRole()
        {
            return Ok(nameof(UserRole));
        }

        #endregion


        #region 自定义授权
        //[Authorize(Policy = "Custom-Policy")]  //equals  [Authorize(Roles = "Admin")]    
        //or
        [Authorize(Policy = "AssertionAdminPolicy")]
        public async Task<IActionResult> AdminPolicy()
        {
            return Ok(nameof(AdminPolicy));
        }


        #endregion

        #endregion
    }
}
