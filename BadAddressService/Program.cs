using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BadAddressService;

/*
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
*/

// In Blazor WebAssembly projects you cannot register a server-side DbContext or connect directly to SQL Server.
// Move DbContext registration to the server (host) project (e.g. the ASP.NET Core host's Program.cs).
// Example for the server project:
//
// // using Microsoft.EntityFrameworkCore;
// // builder.Services.AddDbContextFactory<YourDbContext>(options =>
// //     options.UseSqlServer(builder.Configuration.GetConnectionString("NonProduction") ?? 
// //         throw new InvalidOperationException("Connection string not found.")));
//
// The registration was removed from the WebAssembly client to avoid compilation errors.
