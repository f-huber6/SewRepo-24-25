using System.Reflection;
using System.Text;
using ChatApplication.Components;
using ChatApplication.Hub;
using Database.Context;
using Database.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.CompilerServices;

var assembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddServerSideBlazor();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policyBuilder =>
    {
        policyBuilder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});
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

var key = Encoding.UTF8.GetBytes(conf["Jwt:Key"] ?? throw new Exception("JWT Key not found"));
var issuer = conf["Jwt:Issuer"] ?? throw new Exception("JWT Issuer not found");
var audience = conf["Jwt:Audience"] ?? throw new Exception("JWT Audience not found");

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
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = issuer,
            ValidAudience = audience
        };
    });

var app = builder.Build();
app.UseCors("CorsPolicy");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

//Brauch i ned wü i ned app.UseHttpsRedirection();
app.UseAuthentication();
app.MapControllers();
app.UseAuthorization();
app.UseStaticFiles();
app.UseAntiforgery();
app.Run();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapBlazorHub();
app.MapHub<ChatHub>("/chat");

app.Run();