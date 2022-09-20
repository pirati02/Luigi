using System.Text;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Neo4jClient;
using User.Api.Service;
using User.Neo4j;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureAppConfiguration(cfg =>
{
    cfg.AddJsonFile("Infrastructure/ConnectionStrings/neo4j.json", optional: false, reloadOnChange: true);
    cfg.AddJsonFile("Infrastructure/ConnectionStrings/local-identity-server.json", optional: false, reloadOnChange: true);
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
builder.Services.AddSingleton<IGraphClient>(Neo4jConnector.Connect(builder.Configuration));
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc(c =>
{
    c.Title = "totle";
    c.Version = 1.ToString();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();
app.UseFastEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.UseApimundo();
app.UseSwaggerUi3();
app.Run();