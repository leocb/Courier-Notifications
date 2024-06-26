using Blazored.LocalStorage;

using CN.Web.App;
using CN.Web.App.Providers;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(c => new HttpClient());
builder.Services.AddScoped<ChannelManager>();
builder.Services.AddBlazoredLocalStorageAsSingleton();

var app = builder.Build();

await app.RunAsync();
