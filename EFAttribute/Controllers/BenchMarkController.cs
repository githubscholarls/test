using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using EFAttribute.Domain.Entity;
using EFAttribute.Helper;
using EFAttribute.MyDbContext;
using Iced.Intel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
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
            //22.2s   gc0
            //using (new OperationTimer("foreach ExecuteSqlInterpolated 100"))
            //{
            //    var sqlinter = "test01;update \"user\" set name = sqlinter where name='test1001'";
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        testDbContext.Database.ExecuteSqlInterpolated($"insert into \"user\"(name) values({sqlinter});");
            //    }
            //}
            ////19.7s  gc5
            //using (new OperationTimer("foreach ExecuteSqlInterpolated 1000"))
            //{
            //    var sqlinter = "test01;update \"user\" set name = sqlinter where name='test1002'";
            //    var sex = "nan";
            //    var str = new StringBuilder();
            //    int list_i = 0;
            //    List<object> list = new();
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        str.Append($"insert into \"user\"(name,sex) values(@p{list_i++},@p{list_i++});");
            //        list.Add(sqlinter);
            //        list.Add(sex + i);
            //    }
            //    testDbContext.Database.ExecuteSqlRawAsync(str.ToString(), list);
            //}
            BenchContext.TestDbContext= testDbContext;
            BenchmarkRunner.Run<BenchMarkTest>();
            return Ok();
        }
    }

    public static class BenchContext
    {
        public static IServiceProvider serviceProvider;
        public static TestDbContext TestDbContext;
        public static int bench_i;
        public static int bench_j;
    }

    [NonController]
    [MemoryDiagnoser]
    public class BenchMarkTest
    {
        [Benchmark(sourceCodeLineNumber:10)]
        public void ExecuteSqlInterpolated(TestDbContext testDbContext)
        {
            Console.WriteLine(BenchContext.bench_i++);
            var sqlinter = "test01;update \"user\" set name = sqlinter where name='test1001'";
            for (int i = 0; i < 1; i++)
            {
                testDbContext.Database.ExecuteSqlInterpolated($"insert into \"user\"(name) values({sqlinter});");
            }
        }

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
    }

}
