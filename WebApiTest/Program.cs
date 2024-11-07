using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
#if NET7_0_OR_GREATER
using Microsoft.AspNetCore.OutputCaching;
#endif
using Microsoft.OpenApi.Models;
using System.Configuration;
using System.Globalization;
using System.Net.Cache;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApiTest;
using WebApiTest.Utility;
using Npgsql;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Entity.Infrastructure;
using WebApiTest.Application.Database;
using WebApiTest.Application.Common.Interface;
using WebApiTest.Application.Common;
using HealthChecks.UI.Client;
using WebApiTest.Domain.Options;
using WebApiTest.Application.Common.Delegate;
using Polly;
using Microsoft.AspNetCore.Diagnostics;
using WebApiTest.Filter;
using WebApiTest.Middleware;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


// Add services to the container.

builder.Services.Configure<CustomSetting>(builder.Configuration.GetSection("CustomSetting"));

builder.Services.AddControllersWithViews(options =>
{
    //���ȼ�����app.UseExceptionHandler
    //options.Filters.Add<ApiExceptionFilterAttribute>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region httpclient
builder.Services.AddSingleton<CustomDelegatingHandler>();
builder.Services.AddHttpClient("Gaode", config =>
{
    config.BaseAddress = new Uri("https://restapi.amap.com/");
    config.Timeout = TimeSpan.FromSeconds(30);
    config.DefaultRequestHeaders.Accept.Clear();
    config.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    config.DefaultRequestHeaders.Add("User-Agent", "MyHttpClient");
}).AddHttpMessageHandler(handle => handle.GetRequiredService<CustomDelegatingHandler>());

builder.Services.AddHttpClient("httpClientFactoryPolly", config =>
{
    config.BaseAddress = new Uri("https://api.github.com/");
    config.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
}).AddTransientHttpErrorPolicy(builder=>builder.WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1),TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3) }));

#endregion




builder.Services.AddSingleton<IDbConnectionFactoryls>(_ => new NpgsqlConnectionFactory(config.GetConnectionString("PG_MAIN")!));

builder.Services.AddHealthChecks()
    //.AddCheck<PgDataBaseCheck>("/pgdb");
    //or
    .AddNpgSql(config.GetConnectionString("PG_MAIN")!)
    .AddMongoDb(config.GetConnectionString("Mongo_DB")!);
    
builder.Services.AddSingleton(new JwtHelper(builder.Configuration));

builder.Services.AddAuthenticationCore();

#region swagger��֤

builder.Services.AddSwaggerGen(options =>
{
    //��Ӱ�ȫ����
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "������token ����ʽΪ:Bearer token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    //��Ӱ�ȫҪ��
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference=new OpenApiReference()
            {
                Type=ReferenceType.SecurityScheme,
                Id="Bearer"
            }
        },
        new string[]{}
    }});

});

#endregion

#region ��֤

#region ��Ȩ

#region cookie��Ȩ

//ָ�� CookieAuthenticationDefaults.AuthenticationScheme Ĭ�ϼ�Ȩ
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
    options.LoginPath = "/api/auth/Check11";
        options.LogoutPath = "/api/login/LogOutCookies";
    options.AccessDeniedPath = "/api/auth/Check12";
})
    .AddCookie(Consts.CookiesAuth, config =>
    {
        config.LoginPath = "/api/auth/Check21";//Ĭ�� /Acount/Login
        config.AccessDeniedPath = "/api/auth/Check22";
    });

#endregion


#region �Զ����Ȩ

builder.Services.AddAuthentication(options =>
{
    //���һ��scheme����
    options.AddScheme<UrlTokenAuthenticationHandler>(Consts.UrlTokenScheme, "UrlTokenScheme-Demo");
    options.AddScheme<UrlToken2AuthenticationHandler>(Consts.UrlTokenScheme2, "UrlTokenScheme2-Demo");
    //����δָ��Ĭ�ϼ�Ȩ ��������һ��Ĭ�ϼ�Ȩ(��������authorize���Խӿڱ�500����)
    //options.DefaultAuthenticateScheme = Consts.UrlTokenScheme;
    //��ѡ
    //options.DefaultChallengeScheme = Consts.UrlTokenScheme;
    //options.DefaultSignInScheme = Consts.UrlTokenScheme;
    //options.DefaultSignOutScheme = Consts.UrlTokenScheme;
    //options.DefaultForbidScheme = Consts.UrlTokenScheme;
});
#endregion


#region JWT��Ȩ

//��װ��IAuthenticationHandler ָ�� JwtBearerDefaults.AuthenticationScheme  Ĭ�ϼ�Ȩ������ǰ��ָ���ģ�
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = "scholar",
            ValidIssuer = "scholar",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890123456789"))
        };
    });

#endregion

#endregion

#region ��Ȩ

#region �Զ�����Ȩ

//builder.Services.AddAuthorization(options =>
//{
//    //options.AddPolicy("Custom-Policy", policyBuilder => policyBuilder.RequireRole("Admin"));
//    //or
//    options.AddPolicy("AssertionAdminPolicy", policyBuilder =>
//    {
//        policyBuilder.RequireAssertion(context => context.User.HasClaim(c => c.Type == ClaimTypes.Role) && context.User.Claims.First(c => c.Type.Equals(ClaimTypes.Role)).Value == "Admin");
//    });
//});

#endregion

#endregion

#endregion



//����д��controllersWithViews���Ѱ���
//builder.Services.AddAuthorization();

#region net7��ӻ���

# if NET7_0_OR_GREATER
builder.Services.AddOutputCache(options =>
{
    // Define policies for all requests which are not configured per endpoint or per request
    options.AddBasePolicy(builder => builder.With(c => c.HttpContext.Request.Path.StartsWithSegments("/js")).Expire(TimeSpan.FromDays(1)));
    options.AddBasePolicy(builder => builder.With(c => c.HttpContext.Request.Path.StartsWithSegments("/js")).NoCache());

    //����������л���
    options.AddBasePolicy(builder => builder.Tag("tag-all"));

    options.AddPolicy("article", b => b.Tag("article"));
    options.AddPolicy("category", b => b.Tag("category"));
    options.AddPolicy("home", b => b.Tag("home"));
    options.AddPolicy("NoCache", b => b.NoCache());
});
#endif
#endregion


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyApp", builder =>
    {
        builder.WithOrigins("http://localhost:8080")  // �����ǰ�˵�ַ
               .AllowAnyMethod()                    // �����κ����󷽷���GET, POST, PUT, DELETE�ȣ�
               .AllowAnyHeader();                   // �����κ�ͷ��
    });
});

var app = builder.Build();

app.UseCors("AllowMyApp");

#region .net7ʹ�û���

#if NET7_0_OR_GREATER

app.UseOutputCache();

app.MapGet("/", Gravatar.WriteGravatar);

app.MapGet("/cached", Gravatar.WriteGravatar).CacheOutput();

app.MapGet("/nocache", Gravatar.WriteGravatar).CacheOutput(x => x.NoCache());

app.MapGet("/profile", Gravatar.WriteGravatar).CacheOutput("NoCache");

app.MapGet("/attribute", [OutputCache(PolicyName = "NoCache")] (context) => Gravatar.WriteGravatar(context));

var blog = app.MapGroup("blog").CacheOutput(x => x.Tag("blog"));
blog.MapGet("/", Gravatar.WriteGravatar);// prot:xx/blog :5482  3217  port:xx/blog/  :3968   2194
blog.MapGet("/same1", Gravatar.WriteGravatar);//0639   5348
blog.MapGet("/same2", Gravatar.WriteGravatar);//1442   3974  4655
blog.MapGet("/post/{id}", Gravatar.WriteGravatar).CacheOutput(x => x.Tag("blog", "byid")); // Calling CacheOutput() here overwrites the group's policy//id=1 4308  id=2  1916  id =3 4144    blog�黺�����ʧЧ

//����outputcache��ָ����tag�ӿڻ���
app.MapPost("/purge/{tag}", async (IOutputCacheStore cache, string tag) =>
{
    // POST such that the endpoint is not cached itself

    await cache.EvictByTagAsync(tag, default);
});

// Cached entries will vary by culture, but any other additional query is ignored and returns the same cached content
app.MapGet("/query", Gravatar.WriteGravatar).CacheOutput(p => p.SetVaryByQuery("culture"));

app.MapGet("/vary", Gravatar.WriteGravatar).CacheOutput(c => c.VaryByValue((context) => new KeyValuePair<string, string>("time", (DateTime.Now.Second % 2).ToString(CultureInfo.InvariantCulture))));

long requests = 0;

// Locking is enabled by default
app.MapGet("/lock", async (context) =>
{
    await Task.Delay(1000);
    await context.Response.WriteAsync($"<pre>{requests++}</pre>");
}).CacheOutput(p => p.SetLocking(false).Expire(TimeSpan.FromMilliseconds(1)));

// Etag
app.MapGet("/etag", async (context) =>
{
    // If the client sends an If-None-Match header with the etag value, the server
    // returns 304 if the cache entry is fresh instead of the full response

    var etag = $"\"{Guid.NewGuid():n}\"";
    context.Response.Headers.ETag = etag;

    await Gravatar.WriteGravatar(context);

    var cacheContext = context.Features.Get<IOutputCacheFeature>()?.Context;

}).CacheOutput();

// When the request header If-Modified-Since is provided, return 304 if the cached entry is older
app.MapGet("/ims", Gravatar.WriteGravatar).CacheOutput();

#endif

#endregion


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
if (app.Environment.IsDevelopment())
{
    //�쳣����
    //app.UseExceptionHandler(errorApp =>
    //{
    //    errorApp.Run(async context =>
    //    {
    //        context.Response.StatusCode = 500;
    //        context.Response.ContentType = "text/html";
    //        var exceptionHandlerPathFeature =
    //            context.Features.Get<IExceptionHandlerPathFeature>();
    //        if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
    //        {
    //            await context.Response.WriteAsync("File error thrown!");
    //        }
    //        else
    //        {
    //            await context.Response.WriteAsync("error");
    //        }
    //    });
    //});
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<GetBodyContentMiddleware>();

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/_health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});//.RequireAuthorization();
//app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
