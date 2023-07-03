using Microsoft.AspNetCore.Mvc;
using System.Security.Permissions;

namespace WebApiTest.Controllers
{

    [ApiController]
    [Route("[controller]/[action]")]
    public class DeBugController:ControllerBase
    {
        [HttpGet]
        public IActionResult TestCrashDump()
        {
            //1. crash
            Task.Factory.StartNew(() =>
            {
                Test("a");
            });
            return Ok();
        }
        [NonAction]
        public static string Test(string a)
        {
            return Test("a" + a.Length);
        }
        [HttpGet]
        public IActionResult TestCpuDump()
        {
            Task.Factory.StartNew(() => { bool b = true; while (true) { b = !b; } });
            Task.Factory.StartNew(() => { bool b = true; while (true) { b = !b; } });
            return Ok();
        }
    }
}
