using MassTransit;
using XRFID.Demo.Server.Entities;
using XRFID.Demo.Server.Repositories;
using XRFID.Demo.Server.StateMachines.Shipment.Interfaces;
using XRFID.Demo.Server.StateMachines.Shipment.States;
using XRFID.Demo.Server.Utilities;

namespace XRFID.Demo.Server.StateMachines.Shipment.Actions;

public class StartReadingActivity :
    IStateMachineActivity<ShipmentState, IGpiEvent>
{
    private readonly CommandUtility commandUtility;
    private readonly ReaderRepository readerRepository;
    private readonly ILogger<StartReadingActivity> logger;

    public StartReadingActivity(CommandUtility commandUtility,
                                ReaderRepository readerRepository,
                                ILogger<StartReadingActivity> logger)
    {
        this.commandUtility = commandUtility;
        this.readerRepository = readerRepository;
        this.logger = logger;
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<ShipmentState, IGpiEvent> context, IBehavior<ShipmentState, IGpiEvent> next)
    {
        Reader reader = (await readerRepository.GetAsync(q => q.Id == context.Saga.ReaderId)).First();

        try
        {
            if (reader.Name is null)
            {
                throw new Exception($"Missing Reader Name for id {reader.Id}");
            }
            await commandUtility.StartReading(reader.Id, reader.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw new Exception(ex.Message);
        }

        await next.Execute(context);

    }

    public Task Faulted<TException>(BehaviorExceptionContext<ShipmentState, IGpiEvent, TException> context, IBehavior<ShipmentState, IGpiEvent> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("gpiStartEvent");
    }
}