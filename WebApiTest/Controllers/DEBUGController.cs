using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Security.Permissions;

namespace WebApiTest.Controllers
{

    [ApiController]
    [Route("[controller]/[action]")]
    public class DeBugController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly ConcurrentDictionary<string, string> _headers = new ConcurrentDictionary<string, string>();
        public DeBugController(IDistributedCache cache)
        {
            _cache = cache;
        }
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


        [HttpGet]
        public async Task<IActionResult> TestDicAndDistribute()
        {
            var key = "AppVideoStyleViewInstructions";
            await _cache.RemoveAsync(key);
            if (_cache.GetString(key) == null)
            {
                _cache.SetString(key, "");
            }

            _headers.TryAdd(key, "");
            var task11 = Task.Run(async () =>
            {
                await Task.Delay(1000);
                for (int i = 0; i < 1000; i++)
                {
                    var val = _headers[key];
                    if (!val.Contains(",k" + i + ","))
                    {
                        if (_headers.TryUpdate(key, val + ",k" + i + ",", val))
                        {

                        }
                        else
                        {
                            Console.WriteLine($"当前i{i}获取的数据 为脏数据  当前值：{val} 实际值:{_headers[key]}");
                        }
                        //_headers[key] +=  ",k" + i+",";
                    }
                }
            });
            var task21 = Task.Run(async () =>
            {
                for (int i = 1000; i < 2000; i++)
                {
                    var val = _headers[key];
                    if (!val.Contains(",k" + i + ","))
                    {
                        await Task.Delay(50);
                        if (_headers.TryUpdate(key, val + ",k" + i + ",", val))
                        {

                        }
                        else
                        {
                            Console.WriteLine($"当前i{i}获取的数据 为脏数据  当前值：{val} 实际值:{_headers[key]}");
                        }
                        //_headers[key] += ",k" + i + ",";
                    }
                }
            });

            var task1 = Task.Run(async () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    var val = await _cache.GetStringAsync(key);
                    if (!val.Contains("k" + i))
                    {
                        _cache.SetString(key, _cache.GetString(key) + " k" + i);
                    }
                }
            });
            var task2 = Task.Run(async () =>
            {
                for (int i = 100; i < 200; i++)
                {
                    var val = await _cache.GetStringAsync(key);
                    if (!val.Contains("k" + i))
                    {
                        _cache.SetString(key, _cache.GetString(key) + " k" + i);
                    }
                }
            });

            Task.WaitAll(task1, task2, task11, task21);
            var res = await _cache.GetStringAsync(key);
            var vals = await _cache.GetStringAsync(key);
            var vals1 = _headers[key];

            return Ok();
        }
    }
}
