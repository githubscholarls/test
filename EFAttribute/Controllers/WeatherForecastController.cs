using EFAttribute.Domain.Entity;
using EFAttribute.MyDbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFAttribute.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private TestDbContext dbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, TestDbContext testDbContext)
        {
            _logger = logger;
            dbContext = testDbContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public IActionResult ExecuteSql()
        {
            var list = dbContext.user.ToList();
            //实用技巧
            {
                //1：按住  alt 鼠标左键选择区域复制粘贴代码
                //2：右侧  滑动按钮上 设置下   可以将同一个代码分两个屏幕    不用再qq钉在桌面
                //3：https://dotnet.microsoft.com/en-us/download/intellisense   汉化智能提示   https://blog.csdn.net/xjj800211/article/details/112657800
            }
            //execute
            {
                //直接更新数据(分表后好像不可了，报客户端)
                //dbContext.user.Where(u => u.id > 0).ExecuteUpdate(u => u.SetProperty(f => f.lastLoginTime, f => DateTime.Now.ToString()));
                //dbContext.user.Where(u => u.id == 2).ExecuteUpdate(u => u.SetProperty(f => f.nowAddress, f => "executeUpdate Address"));
            }
            //拆表
            {
                //insert
                var usr = new user()
                {
                    name = "lishuai",
                    sex = "nan",
                    schoolAddress = "zhengzhou",
                    bornAddress = "henan"
                };
                dbContext.Add(usr);
                dbContext.SaveChanges();

                //select 注意不同表是否都有同一列为相同值，拆表后查询为inner
                var ls = (from u in dbContext.user where u.id==1 select u).ToList();
                var ls2 = dbContext.user.Where(u => u.id == 1).FirstOrDefault();
            }

            return Ok();
        }

        [HttpGet]
        public IActionResult EFDocument()
        {

            #region 级联删除



            #endregion


            #region 保存相关数据
            {
                //添加关联数据
                //var usr = new user()
                //{
                //    name = "测试添加关联数据1",
                //    wechats = new List<wechat>()
                //{
                //    new wechat() { Name = "关联数据11", Money = "1" },
                //    new wechat() { Name = "关联数据12", Money = "1" },
                //}
                //};
                //dbContext.user.Add(usr);
                //dbContext.SaveChanges();

                //添加关联实体一个元素
                //var usr1 = dbContext.user.Include(u => u.wechats).Where(u=>u.wechats.Any()).First();
                //var wechat = new wechat() { Name = "关联添加 user.wechats.add" };
                //usr1.wechats.Add(wechat);
                //dbContext.SaveChanges();

                //更改关系
                //var usr = new user { name = "更改依赖实体的关系" };
                //var wech = dbContext.wechat.First();
                //wech.user = usr;
                //dbContext.SaveChanges();

                //删除关系
                var usr1 = dbContext.user.Include(u => u.wechats).Where(u=>u.wechats.Any()).First();
                usr1.wechats.Remove(usr1.wechats.First());
                dbContext.SaveChanges();

                var usrList = dbContext.user.Include(u => u.wechats).ToList();

            }
            #endregion

            #region 保存数据
            {
                //var usr = new user() { name = "attach add", homeAddress = "金水" ,bornAdress="南阳"};
                //dbContext.Attach(usr);
                //dbContext.SaveChanges();


                //检测带id的attach
                var attach = dbContext.user.Where(u => u.name == "attach add").FirstOrDefault();
                var attachId = attach.id;

                //取消追踪 同一个上下文不能追踪俩一样的id  不写下面attach报错
                dbContext.Entry(attach).State = EntityState.Detached;

                //更新全部字段
                //var usr1 = new user() { id = attachId, homeAddress = "金水Id Attach" };
                //dbContext.Attach(usr1);
                //usr1.homeAddress = "Id Attach Modify";
                //dbContext.SaveChanges();

                //更新部分字段
                var usr1 = new user() { id = attachId, homeAddress = "金水Id Attach Attach", bornAddress = "南阳 IsModify Is False" };
                dbContext.Entry(usr1).Property(u => u.homeAddress).IsModified = true;
                dbContext.SaveChanges();

                dbContext.Entry(usr1).State = EntityState.Detached;

                var attach1 = dbContext.user.Where(u => u.name == "attach add").FirstOrDefault();


                #endregion

            }
            return Ok();
        }

    }
}