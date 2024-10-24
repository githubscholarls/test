using Microsoft.AspNetCore.Mvc;
using MySqlSugar.Models;
using SqlSugar;
using System.Text;

namespace MySqlSugar.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ISqlSugarClient myDbContext;
        public HomeController(ISqlSugarClient myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"RemoteIpAddress：{HttpContext.Connection.RemoteIpAddress}");

            if (Request.Headers.ContainsKey("X-Real-IP"))
            {
                sb.AppendLine($"X-Real-IP：{Request.Headers["X-Real-IP"].ToString()}");
            }

            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                sb.AppendLine($"X-Forwarded-For：{Request.Headers["X-Forwarded-For"].ToString()}");
            }
            return Ok(sb.ToString());
        }
        [HttpGet]
        public IActionResult GroupBy()
        {


            //创建数据库对象 (用法和EF Dappper一样通过new保证线程安全)
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "host=121.41.53.37;uid=root;password=123456;database=sqlsugar;",
                DbType = DbType.MySql,
                IsAutoCloseConnection = true
            },
            db =>
            {

                db.Aop.OnLogExecuting = (sql, pars) =>
                {

                    //获取原生SQL推荐 5.1.4.63  性能OK
                    Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));

                    //获取无参数化SQL 对性能有影响，特别大的SQL参数多的，调试使用
                    //Console.WriteLine(UtilMethods.GetSqlString(DbType.SqlServer,sql,pars))


                };

                //注意多租户 有几个设置几个
                //db.GetConnection(i).Aop

            });



            //查询表的所有
            var list = Db.Queryable<Huiyuan>().ToList();


            var a = Db.Queryable<Huiyuan>().GroupBy(h => h.Sex).Select(h => new
            {
                h.Sex,
                count = SqlFunc.AggregateCount(h)
            });




            return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(a));
        }


        [HttpGet]
        public IActionResult Update()
        {

            var list = myDbContext.Queryable<Huiyuan>().ToList();




            //查询
            {


                //分组查询
                var listH = myDbContext.Queryable<Huiyuan>().GroupBy(h=>h.Sex).Select(h=>new {h.Sex, count = SqlFunc.AggregateCount(h)}).ToList();

                //子查询
                var existOrderHuiyuan = myDbContext.Queryable<Huiyuan>().Where(h => SqlFunc.Subqueryable<order>().Where(o => o.huiyuan_id == h.Id).Any()).Select(h => h.Id).ToList();


                //连接查询                    男性会员的订单，金额大于10的，小于10的按照0处理   连接后过滤money <=10   返回1个     money连接后再查询的
                var huiyuanOrderList = myDbContext.Queryable<Huiyuan>().Where(h=>h.Sex==1).LeftJoin<order>((h,o)=>h.Id==o.huiyuan_id).Where((h,o)=>o.money>10).GroupBy((h,o)=>h.Id).Select((h,o)=>new
                {
                    h.Id,
                    maxmoney = SqlFunc.AggregateMax(o.money)
                }).ToList();

                //连接查询          每个表都过滤后再连接  left 没有过滤money <= 10 的  返回4个
                var order = myDbContext.Queryable<order>().Where(o => o.money > 10);
                var huiyuanOrderList2 = myDbContext.Queryable<Huiyuan>().Where(h => h.Sex == 1).LeftJoin(order, (h, o) => h.Id == o.huiyuan_id).GroupBy((h, o) => h.Id).Select((h, o) => new
                {
                    h.Id,
                    maxmoney = SqlFunc.AggregateMax(o.money)
                }).ToList();

            }


            //新增
            {
                
                var addList = new List<Huiyuan>() { new Huiyuan() { Sex = 1, Age = 11, Name = "insert" }, new Huiyuan() { Sex = 2, Age = 12, Name = "insert2" } };
                //插入
                myDbContext.Insertable(addList).ExecuteCommand();




                var existList = myDbContext.Queryable<Huiyuan>().Where(h => h.Name.StartsWith("insert")).ToList();

                existList.ForEach(l =>
                {
                    l.Name += " insert_or_update";
                    l.Grade = "四年级";
                });
                existList.Add(new Huiyuan() { Name = "second add" });
                //插入或者更新
                myDbContext.Storageable<Huiyuan>(existList).ExecuteCommand();

            }



            //更新
            {


                list.ForEach(l => l.Name += " update ");
                //根据对象的所有字段更新数据库
                myDbContext.Updateable(list).ExecuteCommand();


                list.ForEach(l =>
                {
                    l.Age += 10;
                    l.Name += " Ignore";
                });
                //不更新某列
                myDbContext.Updateable(list).IgnoreColumns(l => new { l.Age }).ExecuteCommand();


                 list.ForEach(l =>
                {
                    l.Age += 100;
                    l.Name += " updateColumn ";
                });
                //只更新某列
                myDbContext.Updateable(list).UpdateColumns(l => new { l.Name }).ExecuteCommand();





            }


            return Ok();

        }
    }



}
