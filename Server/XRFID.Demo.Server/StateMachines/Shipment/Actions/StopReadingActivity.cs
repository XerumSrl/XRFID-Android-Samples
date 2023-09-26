using MassTransit;
using System.Text.Json;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;
using XRFID.Demo.Server.Repositories;
using XRFID.Demo.Server.StateMachines.Shipment.Interfaces;
using XRFID.Demo.Server.StateMachines.Shipment.States;
using XRFID.Demo.Server.Utilities;

namespace XRFID.Demo.Server.StateMachines.Shipment.Actions;

public class StopReadingActivity :
    IStateMachineActivity<ShipmentState, IGpiEvent>
{
    private readonly CommandUtility commandUtility;
    private readonly ReaderRepository readerRepository;
    private readonly UnitOfWork uowk;
    private readonly ILogger<StopReadingActivity> logger;
    private readonly GpoUtility gpoUtility;

    public StopReadingActivity(CommandUtility commandUtility,
        ReaderRepository readerRepository,
        UnitOfWork uowk,
        ILogger<StopReadingActivity> logger,
        GpoUtility gpoUtility)
    {
        this.commandUtility = commandUtility;
        this.readerRepository = readerRepository;
        this.uowk = uowk;
        this.logger = logger;
        this.gpoUtility = gpoUtility;
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<ShipmentState, IGpiEvent> context, IBehavior<ShipmentState, IGpiEvent> next)
    {
        try
        {
            Reader reader = await readerRepository.GetAsync(context.Saga.ReaderId) ?? throw new Exception("Missing Reader");

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
            await gpoUtility.SetGpo(reader.Id, reader.Name, gpo.GreenLed, false); //Green off
            await gpoUtility.SetGpo(reader.Id, reader.Name, gpo.YellowLed, false); //Yellow off
            await gpoUtility.SetGpo(reader.Id, reader.Name, gpo.RedLed, true); //Red on

            //reset reader correlation id
            reader.CorrelationId = Guid.Empty;
            await readerRepository.UpdateAsync(reader);
            await uowk.SaveAsync();

            logger.LogDebug("Reset CorrelationId");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }

        await next.Execute(context);

    }

    public Task Faulted<TException>(BehaviorExceptionContext<ShipmentState, IGpiEvent, TException> context, IBehavior<ShipmentState, IGpiEvent> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("gpiStopEvent");
    }
}