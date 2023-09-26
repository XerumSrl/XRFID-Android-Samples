using MassTransit;
using System.Text.Json;
using XRFID.Demo.Server.Entities;
using XRFID.Demo.Server.Repositories;
using XRFID.Demo.Server.StateMachines.Shipment.Interfaces;
using XRFID.Demo.Server.StateMachines.Shipment.States;
using XRFID.Demo.Server.Utilities;

namespace XRFID.Demo.Server.StateMachines.Shipment.Actions;

public class InitializeEventFaultedActivity :
    IStateMachineActivity<ShipmentState, Fault<IInitializeEvent>>
{
    private readonly CommandUtility commandUtility;
    private readonly ReaderRepository readerRepository;
    private readonly ILogger<InitializeEventFaultedActivity> logger;
    private readonly GpoUtility gpoUtility;

    public InitializeEventFaultedActivity(CommandUtility commandUtility,
                                          ReaderRepository readerRepository,
                                          ILogger<InitializeEventFaultedActivity> logger,
                                          GpoUtility gpoUtility)
    {
        this.commandUtility = commandUtility;
        this.readerRepository = readerRepository;
        this.logger = logger;
        this.gpoUtility = gpoUtility;
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<ShipmentState, Fault<IInitializeEvent>> context, IBehavior<ShipmentState, Fault<IInitializeEvent>> next)
    {
        Reader reader = (await readerRepository.GetAsync(q => q.Id == context.Saga.ReaderId)).First();
        try
        {
            if (reader.Name is null)
            {
                throw new Exception($"Missing Reader Name for id {reader.Id}");
            }
            await commandUtility.StopReading(reader.Id, reader.Name);

            if (reader.GpoConfiguration is null)
            {
                throw new Exception("Reader Gpo Configuration is null");
            }
            GpoConfiguration? gpo = new();

            gpo = JsonSerializer.Deserialize<GpoConfiguration>(reader.GpoConfiguration);
            if (gpo is null)
            {
                throw new Exception("Error in Reader Gpo Configuration");
            }

            await gpoUtility.SetGpo(reader.Id, reader.Name, gpo.Buzzer, false); //Buzzer off
            await gpoUtility.SetGpo(reader.Id, reader.Name, gpo.GreenLed, true); //Green on
            await gpoUtility.SetGpo(reader.Id, reader.Name, gpo.YellowLed, true); //Yellow on
            await gpoUtility.SetGpo(reader.Id, reader.Name, gpo.RedLed, true); //Red on
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw new Exception(ex.Message);
        }

        ////reset reader correlation id
        //await readerManager.ResetCorrelationId(context.Message.Message.ReaderId);
        //logger.LogDebug("InitializeEventFaultedActivity|Resetting reader correlation id. ReaderId: {ReaderId}", context.Message.Message.ReaderId);

        await next.Execute(context);

    }

    public Task Faulted<TException>(BehaviorExceptionContext<ShipmentState, Fault<IInitializeEvent>, TException> context, IBehavior<ShipmentState, Fault<IInitializeEvent>> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("initializeEventFaulted");
    }
}