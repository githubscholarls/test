using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Security.Permissions;
using System.Security.Policy;

namespace WebApiTest.Controllers
{

    [ApiController]
    [Route("[controller]/[action]")]
    public class DeBugController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly ConcurrentDictionary<string, string> _headers = new ConcurrentDictionary<string, string>();
        private readonly ILogger<DeBugController> _logger;
        public DeBugController(IDistributedCache cache, ILogger<DeBugController> logger)
        {
            _cache = cache;
            _logger = logger;
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

        [HttpGet]
        public async Task<IActionResult> TestHttpClient([FromServices]IHttpClientFactory httpClientFactory)
        {
            using var httpClient = httpClientFactory.CreateClient("Gaode");
            //key=e19117b7c695ab26f1c3c3aa2369065e&origin=116.395645,39.929985&destination=121.579005,29.885258&strategy=34&show_fields=cost
            var queryParams = new Dictionary<string, string> { { "key", "e19117b7c695ab26f1c3c3aa2369065e" }, { "origin", "116.395645,39.929985" }, { "destination", "121.579005,29.885258" }, { "show_fields", "cost" }, { "strategy", "34" } };
            var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;
            var urlWithParams = $"v5/direction/driving?{queryString}";

            var response = await httpClient.GetAsync(urlWithParams);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<DirectionDrivingRes>();

            return Ok(JsonConvert.SerializeObject(result));
        }
        [HttpGet]
        public async Task<IActionResult> TesthttpClientFactoryPolly([FromServices] IHttpClientFactory httpClientFactory)
        {
            try
            {
                using var httpClient = httpClientFactory.CreateClient("httpClientFactoryPolly");
                var response = await httpClient.GetAsync("/someapi");
                response.EnsureSuccessStatusCode();
                return Ok($"content:{response.Content.ReadAsStreamAsync().Result}");
            }
            catch (Exception e)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> TesthttpClientFactorySocket([FromServices]IHttpClientFactory httpClientFactory)
        {
            // TCP    192.168.0.38:52349     8.130.26.7:80          TIME_WAIT       0       * 20
            //TCP    192.168.0.38:59272     8.130.26.7:22          ESTABLISHED     54404
            //for (int i = 0; i < 20; i++)//这个导致本项目所在主机的端口被大量占用，与下面请求服务器端占用相同的端口只有一个连接
            //{
            //    using (var client = new HttpClient())
            //    {
            //        var result = await client.GetAsync("http://8.130.26.7/");
            //        Console.WriteLine($"请求返回状态码：{result.StatusCode}");
            //    }
            //}
            //await Console.Out.WriteLineAsync("访问完毕");


            //TCP    192.168.0.38:52258     8.130.26.7:80          ESTABLISHED     87880
            //TCP    192.168.0.38:59272     8.130.26.7:22          ESTABLISHED     54404
            for (int i = 0; i < 20; i++)
            {
                using (var client = httpClientFactory.CreateClient())
                {
                    var result = await client.GetAsync("http://8.130.26.7/");
                    Console.WriteLine($"请求返回状态码：{result.StatusCode}");
                }
            }
            await Console.Out.WriteLineAsync("访问完毕");


            return Ok();
        }
        
        public record class FromBodyContent(string obj);
        [HttpPost]
        public async Task<IActionResult> MoreGetFromBodyContent(FromBodyContent obj)
        {
            await Console.Out.WriteLineAsync("接口中获取body内容为：" + Newtonsoft.Json.JsonConvert.SerializeObject(obj));

            //多次读取
            //Request.EnableBuffering();
            //using (var rea = new StreamReader(Request.Body))
            //{
            //    var body = await rea.ReadToEndAsync();
            //    Console.WriteLine(body.ToString());
            //    // Do something
            //    Request.Body.Position = 0;
            //    body = await rea.ReadToEndAsync();
            //}

            return Ok();
        }
    }
}
