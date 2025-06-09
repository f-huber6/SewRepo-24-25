using System.Reflection;
using BlazorApp1.Data;
using BlazorApp1.Mapping;
using Microsoft.EntityFrameworkCore;
//using University.InstructorService.API.Data;
//using University.InstructorService.API.Mapping;

var assembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;

builder.Services.AddDbContext<TestsContext>(options =>
{
    options.UseSqlite(conf.GetConnectionString("TestsConnection"), sqliteOptions =>
    {
        sqliteOptions.MigrationsAssembly(assembly.FullName);
    });
});

//builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClientTests", policyBuilder =>
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseCors("AllowBlazorClientTests");
app.MapControllers();
app.Run();