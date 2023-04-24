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
            //ʵ�ü���
            {
                //1����ס  alt ������ѡ��������ճ������
                //2���Ҳ�  ������ť�� ������   ���Խ�ͬһ�������������Ļ    ������qq��������
                //3��https://dotnet.microsoft.com/en-us/download/intellisense   ����������ʾ   https://blog.csdn.net/xjj800211/article/details/112657800
            }
            //execute
            {
                //ֱ�Ӹ�������(�ֱ����񲻿��ˣ����ͻ���)
                //dbContext.user.Where(u => u.id > 0).ExecuteUpdate(u => u.SetProperty(f => f.lastLoginTime, f => DateTime.Now.ToString()));
                //dbContext.user.Where(u => u.id == 2).ExecuteUpdate(u => u.SetProperty(f => f.nowAddress, f => "executeUpdate Address"));
            }
            //���
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

                //select ע�ⲻͬ���Ƿ���ͬһ��Ϊ��ֵͬ�������ѯΪinner
                var ls = (from u in dbContext.user where u.id==1 select u).ToList();
                var ls2 = dbContext.user.Where(u => u.id == 1).FirstOrDefault();
            }

            return Ok();
        }

        [HttpGet]
        public IActionResult EFDocument()
        {

            #region ����ɾ��



            #endregion


            #region �����������
            {
                //��ӹ�������
                //var usr = new user()
                //{
                //    name = "������ӹ�������1",
                //    wechats = new List<wechat>()
                //{
                //    new wechat() { Name = "��������11", Money = "1" },
                //    new wechat() { Name = "��������12", Money = "1" },
                //}
                //};
                //dbContext.user.Add(usr);
                //dbContext.SaveChanges();

                //��ӹ���ʵ��һ��Ԫ��
                //var usr1 = dbContext.user.Include(u => u.wechats).Where(u=>u.wechats.Any()).First();
                //var wechat = new wechat() { Name = "������� user.wechats.add" };
                //usr1.wechats.Add(wechat);
                //dbContext.SaveChanges();

                //���Ĺ�ϵ
                //var usr = new user { name = "��������ʵ��Ĺ�ϵ" };
                //var wech = dbContext.wechat.First();
                //wech.user = usr;
                //dbContext.SaveChanges();

                //ɾ����ϵ
                var usr1 = dbContext.user.Include(u => u.wechats).Where(u=>u.wechats.Any()).First();
                usr1.wechats.Remove(usr1.wechats.First());
                dbContext.SaveChanges();

                var usrList = dbContext.user.Include(u => u.wechats).ToList();

            }
            #endregion

            #region ��������
            {
                //var usr = new user() { name = "attach add", homeAddress = "��ˮ" ,bornAdress="����"};
                //dbContext.Attach(usr);
                //dbContext.SaveChanges();


                //����id��attach
                var attach = dbContext.user.Where(u => u.name == "attach add").FirstOrDefault();
                var attachId = attach.id;

                //ȡ��׷�� ͬһ�������Ĳ���׷����һ����id  ��д����attach����
                dbContext.Entry(attach).State = EntityState.Detached;

                //����ȫ���ֶ�
                //var usr1 = new user() { id = attachId, homeAddress = "��ˮId Attach" };
                //dbContext.Attach(usr1);
                //usr1.homeAddress = "Id Attach Modify";
                //dbContext.SaveChanges();

                //���²����ֶ�
                var usr1 = new user() { id = attachId, homeAddress = "��ˮId Attach Attach", bornAddress = "���� IsModify Is False" };
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