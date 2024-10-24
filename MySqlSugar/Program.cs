using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISqlSugarClient>(s =>
{
    //Scoped��SqlSugarClient 
    SqlSugarClient sqlSugar = new SqlSugarClient(new ConnectionConfig()
    {
        DbType = SqlSugar.DbType.MySql,
        ConnectionString = "host=121.41.53.37;uid=root;password=123456;database=sqlsugar;",
        IsAutoCloseConnection = true,
    },
   db =>
   {
       //ÿ�������Ķ���ִ��

       //��ȡIOC����Ҫ����һ��������
       //var log=s.GetService<Log>()

       //��ȡIOC����Ҫ����һ��������
       //var appServive = s.GetService<IHttpContextAccessor>();
       //var log= appServive?.HttpContext?.RequestServices.GetService<Log>();

       db.Aop.OnLogExecuting = (sql, pars) =>
       {

           //��ȡԭ��SQL�Ƽ� 5.1.4.63  ����OK
           Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));

           //��ȡ�޲�����SQL ��������Ӱ�죬�ر���SQL������ģ�����ʹ��
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
