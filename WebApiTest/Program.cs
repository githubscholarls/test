using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebApiTest.Utility;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region ʹ��cookie��Ȩ
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//    {
//        options.LoginPath = "/Home/Index";
//        options.AccessDeniedPath = "/Home/Index";
//    });
#endregion

#region �Զ����Ȩ

builder.Services.AddAuthentication(options =>
{
    //���һ��scheme����
    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
    //��������һ��Ĭ�ϼ�Ȩ
    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
    //��ѡ
    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;



});

#endregion

//����д��controllersWithViews���Ѱ���
builder.Services.AddAuthorization();

#region �Զ�����Ȩ

builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("Custom-Policy", policyBuilder => policyBuilder.RequireRole("Admin"));
    //or
    options.AddPolicy("AssertionAdminPolicy", policyBuilder =>
    {
        policyBuilder.RequireAssertion(context => context.User.HasClaim(c => c.Type == ClaimTypes.Role) && context.User.Claims.First(c => c.Type.Equals(ClaimTypes.Role)).Value == "Admin");
    });
});

#endregion


var app = builder.Build();

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
