using MassTransit;
using Microsoft.AspNetCore.SignalR;
using XRFID.Demo.Pages.Hubs;
using XRFID.Demo.Server.Contracts;
using XRFID.Demo.Server.Workers;

namespace XRFID.Demo.Server.Consumers.Frontend;

public class UpdateUiConsumer : IConsumer<StateMachineUiTagPublish>, IConsumer<StateMachineStatusPublish>
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
        if (context.Message.Tag is null)
        {
            logger.LogDebug("[Consume<StateMachineUiTagPublish>] State Machine Message Receved");
        }
        else
        {
            logger.LogDebug("[Consume<StateMachineUiTagPublish>] Tag {Epc} Receved", context.Message.Tag.Tag.Epc);
        }

        //Reaload items
        if (await worker.IdIsEqual(context.Message.ActivMoveId))
        {
            if (context.Message.Tag is not null)
            {
                await worker.SetViewItem(context.Message.Tag.Tag.Epc);
            }
        }

        //Send signalR messages
        await hubContext.Clients.All.SendAsync("RefreshTag");
    }

    public async Task Consume(ConsumeContext<StateMachineStatusPublish> context)
    {
        await worker.EditSMStatus(context.Message.State);

        await hubContext.Clients.All.SendAsync("RefreshState");
    }
}
