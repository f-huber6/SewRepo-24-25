using System.Reflection;
using ChatApplication.Components;
using ChatApplication.Hub;
using ChatApplication.Services;
using Database.Context;
using Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var assembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
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


var app = builder.Build();
app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("api/messages", async ([FromBody] Message message, MessageService messageService ) =>
{
    await messageService.GetAllMessagesAsync();
});

app.MapPost("api/messages", async ([FromBody] Message message, MessageService service) =>
{
    if (message == null || string.IsNullOrWhiteSpace(message.Content))
    {
        return Results.BadRequest("Message content cannot be empty");
    }

    await service.AddMessageAsync(message);
    return Results.Ok();
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapBlazorHub();
app.MapHub<ChatHub>("/chat");

app.Run();