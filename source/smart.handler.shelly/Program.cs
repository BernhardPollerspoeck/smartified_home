using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using smart.core.Models;
using smart.database;
using smart.handler.shelly.Services;

var host = Host.CreateDefaultBuilder(args);

host.ConfigureServices((o, s) =>
{

    var handlerSettings = new HandlerSettings();
    o.Configuration.GetSection(nameof(HandlerSettings)).Bind(handlerSettings);
    s.Configure<HandlerSettings>(o.Configuration.GetSection(nameof(HandlerSettings)));

    #region database
    var version = new Version(10, 6, 4);
    var serverVersion = new MariaDbServerVersion(version);
    s.AddDbContext<SmartContext>(options =>
        options.UseMySql(o.Configuration.GetConnectionString("shelly"), serverVersion));
    #endregion


    s.AddHttpClient();

    s.AddHostedService<MainHandler>();
});

await host.Build().RunAsync();