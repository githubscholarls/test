
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
            return new JsonResult(new {  data=new { name = "lishuai", token = "this is token" }, code = 20000  });
        }
        [HttpGet]
        public IActionResult info()
        {
            //return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(new { username = "lishuai", code = 20000 }));
            return new JsonResult(new { data = new { name = "lishuai", token = "this is token" }, code = 20000 });
        }
    }

    
}
