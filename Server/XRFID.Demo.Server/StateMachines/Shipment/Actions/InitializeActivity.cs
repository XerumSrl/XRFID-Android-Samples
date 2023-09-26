using MassTransit;
using System.Text.Json;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;
using XRFID.Demo.Server.Repositories;
using XRFID.Demo.Server.StateMachines.Shipment.Interfaces;
using XRFID.Demo.Server.StateMachines.Shipment.States;
using XRFID.Demo.Server.Utilities;

namespace XRFID.Demo.Server.StateMachines.Shipment.Actions;
public class InitializeActivity :
    IStateMachineActivity<ShipmentState, IInitializeEvent>
{
    private readonly ILogger<InitializeActivity> logger;
    private readonly ReaderRepository readerRepository;
    private readonly GpoUtility gpoUtility;
    private readonly UnitOfWork uowk;

    public InitializeActivity(ILogger<InitializeActivity> logger,
                              ReaderRepository readerRepository,
                              GpoUtility gpoUtility,
                              UnitOfWork uowk)
    {
        this.logger = logger;
        this.readerRepository = readerRepository;
        this.gpoUtility = gpoUtility;
        this.uowk = uowk;
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<ShipmentState, IInitializeEvent> context, IBehavior<ShipmentState, IInitializeEvent> next)
    {
        Reader reader = (await readerRepository.GetAsync(q => q.Id == context.Saga.ReaderId)).First();

        if (reader is null || context.Message.ReaderId == Guid.Empty)
        {
            logger.LogWarning("StartActivity|Unable to initialize shipment state machine. Reader id is empty");
            throw new InvalidOperationException("Unable to initialize shipment state machine. Reader id is empty");
        }

        if (reader.CorrelationId != context.Saga.CorrelationId)
        {
            logger.LogDebug("StartActivity|Updating reader {Id}: CorrelationId: {CorrelationId} -> {CorrelationId} MovementId: {MovementId} -> {MovementId}",
                reader.Id, reader.CorrelationId, context.Saga.CorrelationId, reader.ActiveMovementId, context.Saga.MovementId);

            reader.CorrelationId = context.Saga.CorrelationId;

            await readerRepository.UpdateAsync(reader);
            await uowk.SaveAsync();
        }

        try
        {
            if (reader.Name is null)
            {
                throw new Exception($"Missing Reader Name for id {reader.Id}");
            }
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
            await gpoUtility.SetGpo(reader.Id, reader.Name, gpo.YellowLed, false); //Yellow off
            await gpoUtility.SetGpo(reader.Id, reader.Name, gpo.RedLed, false); //Red off

        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw new Exception(ex.Message);
        }

        await next.Execute(context);

    }

    public Task Faulted<TException>(BehaviorExceptionContext<ShipmentState, IInitializeEvent, TException> context, IBehavior<ShipmentState, IInitializeEvent> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("start");
    }
}
