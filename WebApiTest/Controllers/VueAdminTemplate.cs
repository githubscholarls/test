
using Microsoft.AspNetCore.Mvc;

namespace WebApiTest.Controllers
{

    public class ApiResponse<T>
    {
        public T data { get; set; }
        public int code { get; set; } = 20000;
    }
    [Route("vue-admin-template/user/[action]")]
    public class VueAdminUserTemplate : ControllerBase
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


    [Route("vue-admin-template/table/[action]")]
    public class VueAdminTableTemplate : ControllerBase
    {
        [HttpGet]
        public IActionResult list()
        {
            var items = new List<string>() { "撒发射点", "sf法撒旦", "的萨芬" };
            var res = new ListRes() { data = new ListData() { items = items } };
            return new JsonResult(new ApiResponse<ListRes>() { data = res });

        }

        public class ListRes
        {
            public ListData data { get; set; } = new();
        }
        public class ListData
        {
            public List<string> items { get; set; } = new();
        }

    }

    
}
