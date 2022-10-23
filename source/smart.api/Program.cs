using asp.net.core.helper.core.Seed.Extensions;
using bp.net.Auth.Server.Models;
using Microsoft.EntityFrameworkCore;
using smart.api.Controllers.Handlers;
using smart.api.Middlewares;
using smart.api.Models;
using smart.api.Services;
using smart.api.Services.Handlers;
using smart.api.Services.Handlers.ProcessControlling;
using smart.database;
using smart.resources;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

var apiSettings = new ApiSettings();
builder.Configuration.GetSection(nameof(ApiSettings)).Bind(apiSettings);
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(nameof(ApiSettings)));

#region database
if (apiSettings.Database is null)
{
    throw new AppException(SmartResources.Api_Init_missing_database);
}
var version = new Version(10, 6, 4);
var serverVersion = new MariaDbServerVersion(version);
builder.Services.AddDbContext<SmartContext>(options =>
    options.UseMySql(apiSettings.Database, serverVersion));
#endregion

#region meta services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordRuleService, PasswordRuleService>();
#endregion

builder.Services.AddTransient<HandlerService>();
builder.Services.AddHostedService<HandlerControlService>();

//channel streaming
builder.Services.AddSingleton(Channel.CreateUnbounded<HandlerControlMessage>(new UnboundedChannelOptions
{
    SingleReader = true,
}));
builder.Services.AddSingleton(sc => sc.GetRequiredService<Channel<HandlerControlMessage>>().Reader);
builder.Services.AddSingleton(sc => sc.GetRequiredService<Channel<HandlerControlMessage>>().Writer);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region api url
if (apiSettings.Urls is null)
{
    throw new AppException(SmartResources.Api_Init_missing_urls);
}
builder.WebHost.UseUrls(apiSettings.Urls);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


#region middlewares
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<JwtMiddleware>();
#endregion


#region database setup
app.MigrateDatabase<SmartContext>();
app.SeedDatabase<SmartContext>(typeof(Program), app.Services);
#endregion


app.MapControllers();

app.Run();
