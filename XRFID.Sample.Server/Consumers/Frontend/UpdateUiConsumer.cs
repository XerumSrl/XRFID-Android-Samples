using MassTransit;
using Microsoft.AspNetCore.SignalR;
using XRFID.Sample.Pages.Hubs;
using XRFID.Sample.Server.Contracts;
using XRFID.Sample.Server.Workers;

namespace XRFID.Sample.Server.Consumers.Frontend;

public class UpdateUiConsumer : IConsumer<StateMachineUiTagPublish>
{
    private readonly CheckPageWorker worker;
    private readonly IHubContext<UiMessageHub> hubContext;
    private readonly ILogger<UpdateUiConsumer> logger;

    public UpdateUiConsumer(CheckPageWorker worker,
                            IHubContext<UiMessageHub> hubContext,
                            ILogger<UpdateUiConsumer> logger)
    {
        this.worker = worker;
        this.hubContext = hubContext;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<StateMachineUiTagPublish> context)
    {
        //Reaload items
        await worker.SetViewItems();

        //Send signalR messages
        await hubContext.Clients.All.SendAsync("RefreshTag");
    }
}
