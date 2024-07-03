using Blazored.LocalStorage;

using CN.Web.App;
using CN.Web.App.Providers;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Load Client-side settings
// /!\ CAUTION /!\ do NOT store secrets here. These are visible client side!
var settings = new ClientSideSettings();
builder.Configuration.Bind(settings);
builder.Services.AddSingleton(settings);

// setup backend url
string? baseAddress = settings.ApiHostname;
if (string.IsNullOrEmpty(baseAddress))
    baseAddress = builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped(c => new HttpClient() { BaseAddress = new(baseAddress)});

// Other services
builder.Services.AddScoped<ChannelManager>();
builder.Services.AddBlazoredLocalStorageAsSingleton();

var app = builder.Build();

await app.RunAsync();
