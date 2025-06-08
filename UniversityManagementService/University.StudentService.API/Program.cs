using System.Reflection;
using Microsoft.EntityFrameworkCore;
using University.StudentService.API.Data;
using University.StudentService.API.Mapping;

var assembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;

builder.Services.AddDbContext<StudentContext>(options =>
{
    options.UseSqlite(conf.GetConnectionString("StudentConnection"), sqliteOptions =>
    {
        sqliteOptions.MigrationsAssembly(assembly.FullName);
    });
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClientStudent", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:5134")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.UseCors("AllowBlazorClientStudent");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.Run();