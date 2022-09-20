using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.Cookies;
using Neo4jClient;
using User.Api.Infrastructure;
using User.Api.Service;
using User.Neo4j;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureAppConfiguration(cfg =>
{
    cfg.AddJsonFile("Infrastructure/ConnectionStrings/neo4j.json", optional: false, reloadOnChange: true);
    cfg.AddJsonFile("Infrastructure/ConnectionStrings/local-identity-server.json", optional: false,
        reloadOnChange: true);
    cfg.AddJsonFile("Infrastructure/ConnectionStrings/rabbitmq.json", optional: false, reloadOnChange: true);
});

#region ConfigureServices

builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddSingleton<IGraphClient>(Neo4jConnector.Connect(builder.Configuration));
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc(c =>
{
    c.Title = "totle";
    c.Version = 1.ToString();
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.ConnectToRabbitMq(builder.Configuration);

#endregion

#region Register Middlewares

var app = builder.Build();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseApimundo();
app.UseSwaggerUi3();
app.Run();

#endregion