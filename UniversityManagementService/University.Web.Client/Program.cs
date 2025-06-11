using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using University.Client.Services;
using University.Web.Client;
using University.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient<StudentHttpService>(c =>
    c.BaseAddress = new Uri("http://localhost:5180"));

builder.Services.AddHttpClient<InstructorHttpService>(c =>
    c.BaseAddress = new Uri("http://localhost:5198"));

builder.Services.AddHttpClient<CourseHttpService>(c =>
    c.BaseAddress = new Uri("http://localhost:5198"));

builder.Services.AddHttpClient<LectureHttpService>(c => 
    c.BaseAddress = new Uri("http://localhost:5198"));

builder.Services.AddMudServices();

await builder.Build().RunAsync();