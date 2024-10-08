﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Claims;

namespace WebApiTest.Utility
{
    public class UrlTokenAuthenticationHandler : IAuthenticationHandler,IAuthenticationSignInHandler,IAuthenticationSignOutHandler
    {
        private AuthenticationScheme authenticationScheme;
        private HttpContext HttpContext;
        private ILogger<UrlTokenAuthenticationHandler> logger;

        public UrlTokenAuthenticationHandler(ILogger<UrlTokenAuthenticationHandler> logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// 鉴权操作
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            //return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), null, authenticationScheme.Name)));
            //上面保证110直接通过鉴权

            logger.LogInformation(nameof(AuthenticateAsync));
            string userInfo = HttpContext.Request.Query["UrlToken"];
            if(userInfo == null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            else if("lishuai".Equals(userInfo))
            {
                var claimIdentity = new ClaimsIdentity("UrlToken1Authentice");
                claimIdentity.AddClaim(new Claim(ClaimTypes.Name, "lishuai"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role,"Admin"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Email, "xxxxxx"));
                claimIdentity.AddClaim(new Claim("aa", "bb"));

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimIdentity);

                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, null, authenticationScheme.Name)));
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("UrlToken is wrong"));
            }
        }
        /// <summary>
        /// 未登录
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task ChallengeAsync(AuthenticationProperties? properties)
        {
            logger.LogInformation(nameof(UrlTokenAuthenticationHandler)+" "+nameof(ChallengeAsync));
            //会导致cookie login 不会被执行
            //HttpContext.Response.Redirect("https://localhost:7027/api/auth/login");
            return Task.CompletedTask;
        }
        /// <summary>
        /// 未授权  无权限
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task ForbidAsync(AuthenticationProperties? properties)
        {
            logger.LogInformation(nameof(ForbidAsync));
            //HttpContext.Response.Redirect("https://localhost:7027/api/auth/Denied");
            HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            logger.LogInformation(nameof(UrlTokenAuthenticationHandler) + " " + nameof(InitializeAsync));
            authenticationScheme = scheme;
            HttpContext= context;
            return Task.CompletedTask;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task SignInAsync(ClaimsPrincipal user,AuthenticationProperties properties)
        {
            var ticket = new AuthenticationTicket(user, properties, authenticationScheme.Name);
            HttpContext.Response.Cookies.Append("UrlTokenCookie", JsonConvert.SerializeObject(ticket.Principal.Claims));
            return Task.CompletedTask;
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task SignOutAsync(AuthenticationProperties properties)
        {
            HttpContext.Response.Cookies.Delete("UrlTokenCookie");
            return Task.CompletedTask;
        }
    }
}
