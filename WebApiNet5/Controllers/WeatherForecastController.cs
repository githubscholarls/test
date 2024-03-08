using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiNet5.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet]
        public string AddWlNews1()
        {
            return "123手动阀手动阀";
        }
        [HttpGet]
        public IActionResult AddWlNews2()
        {
            return new JsonResult(new { name = "123手动阀手动阀" });
        }
        [HttpGet]
        public JsonResult AddWlNews3()
        {
            return new JsonResult(new { name = "123手动阀手动阀" });
        }
        [HttpGet]
        public JsonResult AddWlNews4()
        {
            var a = new JsonResult(new { name = "123手动阀手动阀" });
            var s = a.SerializerSettings;
            Console.WriteLine("sss4" + s?.ToString());

            return a;
        }
        [HttpGet]
        public JsonResult AddWlNews5()
        {
            var a = new JsonResult(new { name = "123手动阀手动阀" }, new Newtonsoft.Json.JsonSerializerSettings());
            var s = a.SerializerSettings;
            Console.WriteLine("sss5" + s?.ToString());
            return a;
        }
    }
}
