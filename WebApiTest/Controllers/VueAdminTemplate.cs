
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
            var items = new List<ListItem>() { new ListItem() { title = "标题1", author = "上作者" }, new ListItem() { title = "第一本书", author = "刘建伟" } };
            return new JsonResult(new ApiResponse<ListData>() { data = new ListData() { items = items } });

        }

        public class ListData
        {
            public List<ListItem> items { get; set; } = new();
        }

        public class ListItem
        {
            public string title { get; set; }

            public string author { get; set; }
        }
    }

    
}
