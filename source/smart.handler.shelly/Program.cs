using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using smart.handler.shelly.Services;

var host = Host.CreateDefaultBuilder(args);

host.ConfigureServices((o, s) =>
{
    s.AddHttpClient();

    s.AddHostedService<MainHandler>();
});

await host.Build().RunAsync();