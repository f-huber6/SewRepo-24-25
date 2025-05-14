using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Chat.Client;
using MudBlazor.Services;
using Chat.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityStateProvider>();
builder.Services.AddScoped<IdentityStateProvider>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<AuthHeaderService>();
builder.Services.AddScoped<UnreadMessageService>();
builder.Services.AddScoped<FriendRequestService>();
builder.Services.AddScoped<FriendService>();

builder.Services.AddHttpClient("ChatClient",client =>
    {
        client.BaseAddress = new Uri("http://192.168.1.78:5142");
    })
    .AddHttpMessageHandler<AuthHeaderService>();

builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ChatClient");
});

await builder.Build().RunAsync();