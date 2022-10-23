using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using smart.database;
using System.Threading.Channels;

namespace smart.api.Services.Handlers.ProcessControlling;

public class HandlerControlService : BackgroundService
{
    #region fields
    private DockerClient _dockerClient;

    private readonly ChannelReader<HandlerControlMessage> _channelReader;
    private readonly IServiceProvider _serviceProvider;
    #endregion

    #region ctor
    public HandlerControlService(
        ChannelReader<HandlerControlMessage> channelReader,
        IServiceProvider serviceProvider)
    {
        _channelReader = channelReader;
        _serviceProvider = serviceProvider;
        _dockerClient = new DockerClientConfiguration().CreateClient();
    }
    #endregion

    #region BackgroundService
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _channelReader.ReadAllAsync(stoppingToken))
        {
            switch (message)
            {
                case HandlerControlMessage m when m.Action == EHandlerAction.Enabled:
                    await HandleEnabled(m.HandlerId);
                    break;
                case HandlerControlMessage m when m.Action == EHandlerAction.Disabled:
                    await HandleDisabled(m.HandlerId);
                    break;
            }
        }
    }
    #endregion

    #region handler
    private async Task HandleEnabled(int handlerId)
    {
        var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SmartContext>();

        var handler = await context
            .ElementHandlers
            .FirstOrDefaultAsync(h => h.Id == handlerId);
        if (handler is null)
        {
            //todo: log
            return;
        }

        var dockerKey = $"{handler.Id}-{handler.ElementType}";

        var i1 = await _dockerClient.Images.ListImagesAsync(new ImagesListParameters());

        //await _dockerClient.Images.CreateImageAsync(
        //    new ImagesCreateParameters
        //    {
        //        FromImage = "hello-world",
        //        Tag = "latest"
        //    },
        //    null,
        //    new Progress<JSONMessage>());

        var i2 = await _dockerClient.Images.ListImagesAsync(new ImagesListParameters());

        var c1 = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { Limit = 10 });

        //await _dockerClient.Containers.CreateContainerAsync(
        //    new CreateContainerParameters
        //    {
        //        Name = "tat1",
        //        Image = "hello-world",
        //    });

        var c2 = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { Limit = 10, });

        //check docker state
        //pull
        // start

    }
    private Task HandleDisabled(int handlerId)
    {
        //check docker state
        return Task.CompletedTask;
    }
    #endregion

}
