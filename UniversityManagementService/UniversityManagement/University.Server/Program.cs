using System.Reflection;
using Microsoft.EntityFrameworkCore;
using University.Database.Context;

var assembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlite(conf.GetConnectionString("DefaultConnection"), optionsBuilder =>
    {
        optionsBuilder.MigrationsAssembly(assembly.FullName);
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.Run();