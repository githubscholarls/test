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

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new JwtHelper(builder.Configuration));

builder.Services.AddAuthenticationCore();

#region swagger��֤

builder.Services.AddSwaggerGen(options =>
{
    //���Ӱ�ȫ����
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "������token ����ʽΪ:Bearer token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    //���Ӱ�ȫҪ��
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
    //����һ��scheme����
    options.AddScheme<UrlTokenAuthenticationHandler>(Consts.UrlTokenScheme, "UrlTokenScheme-Demo");
    options.AddScheme<UrlToken2AuthenticationHandler>(Consts.UrlTokenScheme2, "UrlTokenScheme2-Demo");
    //����δָ��Ĭ�ϼ�Ȩ ��������һ��Ĭ�ϼ�Ȩ(��������authorize���Խӿڱ�500����)
    //options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
    //��ѡ
    //options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
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

#region net7���ӻ���

# if NET7_0_OR_GREATER
builder.Services.AddOutputCache(options =>
{
    // Define policies for all requests which are not configured per endpoint or per request
    options.AddBasePolicy(builder => builder.With(c => c.HttpContext.Request.Path.StartsWithSegments("/js")).Expire(TimeSpan.FromDays(1)));
    options.AddBasePolicy(builder => builder.With(c => c.HttpContext.Request.Path.StartsWithSegments("/js")).NoCache());


    options.AddPolicy("NoCache", b => b.NoCache());
});
#endif
#endregion

var app = builder.Build();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();