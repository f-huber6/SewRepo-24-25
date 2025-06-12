using System.Reflection;
using BlazorApp1.Data;
using BlazorApp1.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Test API",
        Version = "v1",
        Description = "API zum Verwalten von Tests und Studenten"
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Generiert JSON-Endpunkt
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API v1");
        options.RoutePrefix = "swagger"; // Swagger erreichbar unter /swagger
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseCors("AllowBlazorClientTests");
app.MapControllers();
app.Run();