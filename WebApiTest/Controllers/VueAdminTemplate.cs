
using Microsoft.AspNetCore.Mvc;

namespace WebApiTest.Controllers
{
    [Route("vue-admin-template/user/[action]")]
    public class VueAdminTemplate : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult login()
        {
            //return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(new { username = "lishuai", code = 20000 }));
            return new JsonResult(new { username = "lishuai", code = 20000 });
        }
    }

    
}
