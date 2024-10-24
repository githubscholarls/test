using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISqlSugarClient>(s =>
{
    //Scoped用SqlSugarClient 
    SqlSugarClient sqlSugar = new SqlSugarClient(new ConnectionConfig()
    {
        DbType = SqlSugar.DbType.MySql,
        ConnectionString = "host=121.41.53.37;uid=root;password=123456;database=sqlsugar;",
        IsAutoCloseConnection = true,
    },
   db =>
   {
       //每次上下文都会执行

       //获取IOC对象不要求在一个上下文
       //var log=s.GetService<Log>()

       //获取IOC对象要求在一个上下文
       //var appServive = s.GetService<IHttpContextAccessor>();
       //var log= appServive?.HttpContext?.RequestServices.GetService<Log>();

       db.Aop.OnLogExecuting = (sql, pars) =>
       {

           //获取原生SQL推荐 5.1.4.63  性能OK
           Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));

           //获取无参数化SQL 对性能有影响，特别大的SQL参数多的，调试使用
           //Console.WriteLine(UtilMethods.GetSqlString(DbType.SqlServer,sql,pars))


       };
   });
    return sqlSugar;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
