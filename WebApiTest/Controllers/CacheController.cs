using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace WebApiTest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IOutputCacheStore cache;
        public CacheController(IOutputCacheStore cache)
        {
            this.cache = cache;
        }
        /// <summary>
        /// 删除所有缓存或者指定缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult PurgeEveryCache(string? tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                cache.EvictByTagAsync("tag-all", default);
            }
            else
            {
                cache.EvictByTagAsync(tagName, default);
            }
            
            return Ok();
        }

        //既有article ，也有basePolicy
        [OutputCache(PolicyName ="article")]
        [HttpGet]
        public IActionResult ArticleList()
        {
            return Ok("article"+Guid.NewGuid().ToString());

        }
        [OutputCache(PolicyName = "category")]
        [HttpGet]
        public IActionResult CategoryList()
        {
            return Ok("category"+Guid.NewGuid().ToString());

        }
        //有BasePolicy
        [HttpGet]
        public IActionResult BannerList()
        {
            return Ok("banner" + Guid.NewGuid().ToString());

        }

    }


    [Route("api/[controller]/[action]")]
    [ApiController]
    // 既有home，也有basePolicy
    [OutputCache(PolicyName = "home")]
    public class HomeController : Controller
    {

        [HttpGet]
        public IActionResult Home1()
        {
            return Ok("Home1" + Guid.NewGuid().ToString());
        }

        [HttpGet]
        public IActionResult Home2()
        {
            return Ok("Home2" + Guid.NewGuid().ToString());
        }
    }
}
