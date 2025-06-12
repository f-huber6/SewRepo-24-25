using System.Reflection;
using Microsoft.EntityFrameworkCore;
using University.EnrollmentService.API.Data;
using University.Shared.Client;

var assembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;

builder.Services.AddDbContext<EnrollmentContext>(options =>
{
    options.UseSqlite(conf.GetConnectionString("EnrollmentConnection"), sqliteOptions =>
    {
        sqliteOptions.MigrationsAssembly(assembly.FullName);
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClientEnrollment", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:5134")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddHttpClient<IStudentClient, StudentClient>(client => 
    client.BaseAddress = new Uri("http://localhost:5180"));

builder.Services.AddHttpClient<ILectureClient, LectureClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5198/");
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowBlazorClientEnrollment");
app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.Run();