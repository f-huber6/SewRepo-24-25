using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Chat.Api.Controllers;
using Chat.Api.Hub;
using Chat.Database.Context;
using Chat.Database.Entities;

var assembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;
var key = Encoding.UTF8.GetBytes(conf["Jwt:Key"] ?? throw new Exception("Key not found"));
var issuer = conf["Jwt:Issuer"] ?? throw new Exception("Issuer not found");
var audience = conf["Jwt:Audience"] ?? throw new Exception("Audience not found");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlite(conf.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.MigrationsAssembly(assembly.FullName);
        });
});

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<IEmailSender, EmailService>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        options.Events = new JwtBearerEvents
        {   
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                
                if (!string.IsNullOrEmpty(accessToken) && 
                    (path.StartsWithSegments("/chathub") ||
                     path.StartsWithSegments("/friendHub")))
                {
                    context.Token = accessToken;
                }
                
                return Task.CompletedTask;
            }   
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policyBuilder =>
    {
        policyBuilder.
            WithOrigins("http://192.168.1.78:5143").
            AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
        //.AllowCredentials();
    });
});

builder.Services.AddScoped<EmailService>();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddSignalR();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5142);
});

var app = builder.Build();

app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthentication();
app.MapControllers();
app.UseAuthorization();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHub<ChatHub>("/chatHub");
app.MapHub<FriendHub>("/friendHub");
app.Run();