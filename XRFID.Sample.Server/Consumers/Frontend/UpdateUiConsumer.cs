﻿using MassTransit;
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
}
