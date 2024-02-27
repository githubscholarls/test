using EFAttribute.Domain.Entity;
using EFAttribute.MyDbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SQLitePCL;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text;

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
        private readonly TestDbContext dbContext;
        private readonly TestCpuContext testCpuContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, TestDbContext testDbContext, TestCpuContext testCpuContext)
        {
            _logger = logger;
            dbContext = testDbContext;
            this.testCpuContext = testCpuContext;
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
                    //schoolAddress = "zhengzhou",
                    //bornAddress = "henan"
                };
                dbContext.Add(usr);
                dbContext.SaveChanges();

                //select ע�ⲻͬ���Ƿ���ͬһ��Ϊ��ֵͬ�������ѯΪinner
                var ls = (from u in dbContext.user where u.id == 1 select u).ToList();
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
                var usr1 = dbContext.user.Include(u => u.wechats).Where(u => u.wechats.Any()).First();
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
                //var usr1 = new user() { id = attachId, homeAddress = "��ˮId Attach Attach", bornAddress = "���� IsModify Is False" };
                //dbContext.Entry(usr1).Property(u => u.homeAddress).IsModified = true;
                dbContext.SaveChanges();

                //dbContext.Entry(usr1).State = EntityState.Detached;

                var attach1 = dbContext.user.Where(u => u.name == "attach add").FirstOrDefault();


                #endregion

            }
            return Ok();
        }

        private int Getint()
        {
            return 1 + 1;
        }
        private int Getint1()
        {
            return 1 + 1;
        }
        private int Getint2()
        {
            return 1 + 1;
        }

        [HttpGet]
        public IActionResult TestAdd()
        {
            dbContext.Add(new user() { name = "testAdd" });
            dbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IActionResult ExecuteSqlScript()
        {
            var str = "'lishuai' ;update \"user\"set sex =1 where id =3";

            //��Ҳ�ᱨ��  û��add_time  
            //var r = dbContext.user.FromSql($"select id,name,sex from \"user\"").ToList();
            //var r1 = dbContext.user.FromSql($"select  id,name,sex from \"user\" where name={str}").ToList();
            //var r2 = dbContext.user.FromSqlRaw($"select  id,name,sex from \"user\" where name={str}").ToList();
            //var r3 = dbContext.user.FromSqlInterpolated($"select  id,name,sex from \"user\" where name={str}").ToList();

            {

                //��ֹsqlע��
                //dbContext.Database.ExecuteSql($"select  id,name,sex from \"user\" where name={str}");
                ////���ܷ�ֹ
                //dbContext.Database.ExecuteSqlRaw($"select  id,name,sex from \"user\" where name={str}");
                ////��ֹsqlע��
                //dbContext.Database.ExecuteSqlInterpolated($"select  id,name,sex from \"user\" where name={str}");
            }
            {
                var strr = $"select  id,name,sex from \"user\" where name={str}";
                //SQLite Error 1: 'near "@p0": syntax error'.
                //net 7
                //dbContext.Database.ExecuteSql($"{strr}");
                dbContext.Database.ExecuteSqlRaw($"{strr}");
                dbContext.Database.ExecuteSqlInterpolated($"{strr}");
            }

            return Ok();
        }


        [HttpGet]
        public IActionResult AddTestDataBeforeTestDetectChanges()
        {
            var count = testCpuContext.user.Count();

            //��ʼ��10������

            var insertCount = 100000 - count;
            List<user> addList = new();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < insertCount; i++)
            {
                //addList.Add(new() { name = "testDetectChangesName" + i });

                sb.Append($"INSERT INTO \"user\" (name) VALUES ('testDetectChangesName{i}');");
            }
            testCpuContext.Database.ExecuteSqlRaw($"{sb}");

            //dbContext.user.AddRange(addList);
            testCpuContext.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// --10�����ݸ���
        //--cpu 30%   67418ms
        //select count(1) from "user" u where "name" like 'v1%'
        //--cpu 18%   69494ms
        //select count(1) from "user" u where "name" like 'v2%'
        //--cpu 8%    55374ms
        //select count(1) from "user" u where "name" like 'v3%'
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Test1DetectChangesCpuHigh()
        {

            var watch = new Stopwatch();
            watch.Start();

            var list = testCpuContext.user.ToList();

            if (list.Count() > 3000)
            {
                int MaxCount = list.Count() / 3000 + 1;
                for (int i = 0; i < MaxCount; i++)
                {
                    var list_editchelineDetails = list.OrderBy(p => p.id).Skip(i * 3000).Take(3000).ToList();
                    if (list_editchelineDetails != null && list_editchelineDetails.Count() > 0)
                    {
                        for (int j = 0; j < list_editchelineDetails.Count(); j++)
                        {
                            list_editchelineDetails[j].name = "v1" + list_editchelineDetails[j].name;
                            list_editchelineDetails[j].sex = "v1sex";
                            list_editchelineDetails[j].last_login_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            testCpuContext.Entry<pguser>(list_editchelineDetails[j]).Property("name").IsModified = true;
                            testCpuContext.Entry<pguser>(list_editchelineDetails[j]).Property("sex").IsModified = true;
                            testCpuContext.Entry<pguser>(list_editchelineDetails[j]).Property("last_login_time").IsModified = true;
                        }
                        testCpuContext.SaveChanges();
                    }
                }
            }


            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            return Ok();



        }
        [HttpGet]
        public IActionResult Test2DetectChangesCpuHigh()
        {
            var watch = new Stopwatch();
            watch.Start();


            var list = testCpuContext.user.ToList();

            var a = testCpuContext.ChangeTracker.AutoDetectChangesEnabled;
            Console.WriteLine("AutoDetectChangesEnabledĬ��ֵ" + a.ToString());

            var b = testCpuContext.ChangeTracker.AutoDetectChangesEnabled = false;

            Console.WriteLine("AutoDetectChangesEnabled�޸ĺ�" + b.ToString());

            if (list.Count() > 3000)
            {
                int MaxCount = list.Count() / 3000 + 1;
                for (int i = 0; i < MaxCount; i++)
                {
                    var list_editchelineDetails = list.OrderBy(p => p.id).Skip(i * 3000).Take(3000).ToList();
                    if (list_editchelineDetails != null && list_editchelineDetails.Count() > 0)
                    {
                        for (int j = 0; j < list_editchelineDetails.Count(); j++)
                        {
                            //Console.WriteLine("�޸�ǰ:" + dbContext.Entry(list_editchelineDetails[j]).State);  //  Unchanged
                            //Console.WriteLine("�޸�ǰ:"+list_editchelineDetails[j].name);
                            //Console.WriteLine("�޸�ǰ:" + list_editchelineDetails[j].sex);
                            //Console.WriteLine("�޸�ǰ:" + list_editchelineDetails[j].lastLoginTime);


                            //�������� IsModified 
                            list_editchelineDetails[j].name = "v2" + list_editchelineDetails[j].name;
                            list_editchelineDetails[j].sex = "v2sex";
                            list_editchelineDetails[j].last_login_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            testCpuContext.Entry(list_editchelineDetails[j]).Property(x => x.name).IsModified = true;
                            testCpuContext.Entry(list_editchelineDetails[j]).Property(x => x.sex).IsModified = true;
                            testCpuContext.Entry(list_editchelineDetails[j]).Property(x => x.last_login_time).IsModified = true;

                            //��ʹ��  Property("add_vyear_new").IsModified = true;    ��    dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                            //dbContext.Entry(list_editchelineDetails[j]).Property(x => x.name).CurrentValue = "v2" + list_editchelineDetails[j].name;
                            //dbContext.Entry(list_editchelineDetails[j]).Property(x => x.sex).CurrentValue = "v2sex";
                            //dbContext.Entry(list_editchelineDetails[j]).Property(x => x.lastLoginTime).CurrentValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");



                            //Console.WriteLine("�޸ĺ�:" + list_editchelineDetails[j].name);
                            //Console.WriteLine("�޸ĺ�:" + list_editchelineDetails[j].sex);
                            //Console.WriteLine("�޸ĺ�:" + list_editchelineDetails[j].lastLoginTime);
                            //Console.WriteLine("�޸ĺ�:" + dbContext.Entry(list_editchelineDetails[j]).State);
                        }
                        testCpuContext.SaveChanges();
                    }
                }
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);

            return Ok();

        }

        [HttpGet]
        public IActionResult Test3FromSqlNoDetectChanges()
        {
            var watch = new Stopwatch();
            watch.Start();

            var list = testCpuContext.user.ToList();

            if (list.Count() > 3000)
            {
                int MaxCount = list.Count() / 3000 + 1;
                for (int i = 0; i < MaxCount; i++)
                {
                    var list_editchelineDetails = list.OrderBy(p => p.id).Skip(i * 3000).Take(3000).ToList();
                    if (list_editchelineDetails != null && list_editchelineDetails.Count() > 0)
                    {
                        FormattableString fs;
                        StringBuilder sb = new();

                        List<NpgsqlParameter> parameters = new();
                        for (int j = 0; j < list_editchelineDetails.Count(); j++)
                        {
                            sb.Append($"update \"user\" set name = 'v3',sex='nv',verify=@curj{j} where id = @uid{j};");

                            parameters.Add(new NpgsqlParameter($"@curj{j}", j));
                            parameters.Add(new NpgsqlParameter($"@uid{j}", list_editchelineDetails[j].id));
                            //dbContext.Database.ExecuteSqlInterpolated($"update user set name = 'v3',sex='nv' where id = @uid{j};");
                        }
                        testCpuContext.Database.ExecuteSqlRaw(sb.ToString(), parameters);
                    }
                }
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            return Ok();
        }
    }
}