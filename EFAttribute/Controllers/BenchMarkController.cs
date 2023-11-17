using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using EFAttribute.Domain.Entity;
using EFAttribute.Helper;
using EFAttribute.MyDbContext;
using Iced.Intel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace EFAttribute.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BenchMarkController : ControllerBase
    {
        private readonly TestDbContext testDbContext;
        private readonly IServiceProvider serviceProvider;
        public BenchMarkController(TestDbContext testDbContext, IServiceProvider serviceProvider)
        {
            this.testDbContext = testDbContext;
            this.serviceProvider = serviceProvider;
        }
        public record class test1(string name, string sex);
        [HttpPost]
        public IActionResult StartBenchMark()
        {
            {
                //22.2s   gc0
                //using (new OperationTimer("foreach ExecuteSqlInterpolated 100"))
                //{
                //    var sqlinter = "test01;update \"user\" set name = sqlinter where name='test1001'";
                //    for (int i = 0; i < 1000; i++)
                //    {
                //        testDbContext.Database.ExecuteSqlInterpolated($"insert into \"user\"(name) values({sqlinter});");
                //    }
                //}
                //23.7s  gc5
                //using (new OperationTimer("foreach ExecuteSqlInterpolated 1000"))
                //{
                //    var sqlinter = "test01;update \"user\" set name = sqlinter where name='test1002'";
                //    var sex = "nan";
                //    var str = new StringBuilder();
                //    int list_i = 0;
                //    List<object> list = new();
                //    for (int i = 0; i < 1000; i++)
                //    {
                //        //不行
                //        //str.Append($"insert into \"user\"(name,sex) values(@name{list_i},@sex{list_i++});");

                //        str.Append($"insert into \"user\"(name,sex) values(@p{list_i++},@p{list_i++});");
                //        list.Add(sqlinter);
                //        list.Add(sex + i);
                //    }

                //    testDbContext.Database.ExecuteSqlRawAsync(str.ToString(), list);
                //}
                //1s  gc0  无效果
                //using (new OperationTimer("foreach ExecuteSqlInterpolated 1000"))
                //{
                //    var sqlinter = "test01;update \"user\" set name = sqlinter where name='test1002'";
                //    var str = new StringBuilder();
                //    List<object> list = new();
                //    for (int i = 0; i < 1000; i++)
                //    {
                //        str.Append($"insert into \"user\"(name,sex) values(@name{i},@sex{i});");
                //        list.Add(new { name = sqlinter, sex = "nan" + i });
                //    }
                //    testDbContext.Database.ExecuteSqlRawAsync(str.ToString(), list);
                //}
            }
            BenchmarkRunner.Run<BenchMarkTest>();

            return Ok();
        }

        #region BenchMark


        [NonAction]
        [Benchmark]
        public void TestParameterSource(TestDbContext? context)
        {
            if (context is null)
            {
                Console.WriteLine("context is null");
                return;
            }
            var count = context.user.Count();
            Console.WriteLine("count:" + count);
        }

        #endregion
    }
    [NonController]
    public class BenchMarkTest
    {
        private int _queryId = 4;
        private TestDbContext testDbContext;


        [GlobalSetup]
        public async Task SetUp()
        {
            testDbContext = new TestDbContext("Data Source = wms.db");
        }
        //[Benchmark(sourceCodeLineNumber: 10)]
        //public void ExecuteSqlInterpolated(TestDbContext testDbContext)
        //{
        //    Console.WriteLine(BenchContext.bench_i++);
        //    var sqlinter = "test01;update \"user\" set name = sqlinter where name='test1001'";
        //    for (int i = 0; i < 1; i++)
        //    {
        //        testDbContext.Database.ExecuteSqlInterpolated($"insert into \"user\"(name) values({sqlinter});");
        //    }
        //}

        //[Benchmark(sourceCodeLineNumber: 10)]
        //public void BlukExecuteSqlInterpolated()
        //{

        //    Console.WriteLine(BenchContext.bench_j++);
        //    var sqlinter = "test01;update \"user\" set name = sqlinter where name='test1001'";
        //    var sex = "nan";
        //    var str = new StringBuilder();
        //    int list_i = 0;
        //    List<object> list = new();
        //    for (int i = 0; i < 1; i++)
        //    {
        //        str.Append($"insert into \"user\"(name,sex) values(@p{list_i++},@p{list_i++});");
        //        list.Add(sqlinter);
        //        list.Add(sex + i);
        //    }
        //    BenchContext.TestDbContext.Database.ExecuteSqlRawAsync(str.ToString(), list);
        //}


        //private readonly Func<TestDbContext, IEnumerable<string>> GetNames = (Func<TestDbContext, IEnumerable<string>>)EF.CompileAsyncQuery((TestDbContext context) =>
        //(from u in context.user.Where(u => u.id > 0) select u.name).ToList());
        //private readonly Func<TestDbContext, int,int> GetFirst = EF.CompileQuery((TestDbContext context, int id) =>
        //(from u in context.user where u.id == id select u.id).FirstOrDefault());

        //[Benchmark(sourceCodeLineNumber: 10)]
        //public void CompileQueryTest()
        //{
        //    var a = EF.CompileQuery((TestDbContext context) =>
        //(from u in context.user.Where(u => u.id > 0) select u.name).AsQueryable()).Invoke(testDbContext);
        //}
        //[Benchmark(sourceCodeLineNumber: 10)]
        //public void NoCompileQueryTest()
        //{
        //    var a = (from u in testDbContext.user.Where(u => u.id > 0) select u.name).AsEnumerable();
        //}
        //[Benchmark]
        //public void CompileQueryFirstTest()
        //{
        //    var a = GetFirst.Invoke(testDbContext, _queryId);
        //}
        [Benchmark]
        public void NoCompileQueryFirstTest()
        {
            //var a = (from u in testDbContext.user where u.id == _queryId select u.id).FirstOrDefault();
            var a = testDbContext.user.FirstOrDefault();
        }
    }

}
