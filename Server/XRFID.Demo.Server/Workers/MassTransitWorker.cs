using MassTransit;
using XRFID.Demo.Modules.Mqtt.Events;

namespace XRFID.Demo.Server.Workers;

public class MassTransitWorker : BackgroundService
{
    readonly IBus _bus;

    public MassTransitWorker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _bus.Publish(new Heartbeat(), stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }
    }
}
