using System.Reflection;
using Microsoft.EntityFrameworkCore;
using University.StudentService.API.Data;

var assembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;

builder.Services.AddDbContext<LearnMaterialContext>(options =>
{
    options.UseSqlite(conf.GetConnectionString("LearnMaterialConnection"), sqliteOptions =>
    {
        sqliteOptions.MigrationsAssembly(assembly.FullName);
    });
});

//builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClientLearnMaterial", policyBuilder =>
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

app.UseCors("AllowBlazorClientLearnMaterial");
app.MapControllers();
app.Run();