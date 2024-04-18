using Blazored.LocalStorage;

using CN.Web.App;
using CN.Web.App.Providers;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

string baseUri = Environment.GetEnvironmentVariable("API_URI", EnvironmentVariableTarget.Process) ?? @"https://cn.leobottaro.com/";
if (string.IsNullOrEmpty(baseUri))
    Console.Error.WriteLine("API_URI environment variable is not defined.");



builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseUri) });
builder.Services.AddScoped<ChannelManager>();
builder.Services.AddBlazoredLocalStorageAsSingleton();

var app = builder.Build();



await app.RunAsync();
