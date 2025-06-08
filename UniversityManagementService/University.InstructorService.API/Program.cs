using System.Reflection;
using Microsoft.EntityFrameworkCore;
using University.InstructorService.API.Data;
using University.InstructorService.API.Mapping;

var assembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;

builder.Services.AddDbContext<InstructorContext>(options =>
{
    options.UseSqlite(conf.GetConnectionString("InstructorConnection"), sqliteOptions =>
    {
        sqliteOptions.MigrationsAssembly(assembly.FullName);
    });
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClientInstructor", policyBuilder =>
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

app.UseCors("AllowBlazorClientInstructor");
app.MapControllers();
app.Run();