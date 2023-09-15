using MassTransit;
using System.Text.Json;
using XRFID.Sample.Server.Entities;
using XRFID.Sample.Server.Repositories;
using XRFID.Sample.Server.StateMachines.Shipment.Schedule;
using XRFID.Sample.Server.StateMachines.Shipment.States;
using XRFID.Sample.Server.Utilities;

namespace XRFID.Sample.Server.StateMachines.Shipment.Actions;

public class GpoBuzzerActivity :
    IStateMachineActivity<ShipmentState, GpoBuzzerExpired>
{
    private readonly ReaderRepository readerRepository;
    private readonly ILogger<GpoBuzzerActivity> logger;
    private readonly GpoUtility gpoUtility;

    public GpoBuzzerActivity(ReaderRepository readerRepository,
                             ILogger<GpoBuzzerActivity> logger,
                             GpoUtility gpoUtility)
    {
        this.readerRepository = readerRepository;
        this.logger = logger;
        this.gpoUtility = gpoUtility;
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<ShipmentState, GpoBuzzerExpired> context, IBehavior<ShipmentState, GpoBuzzerExpired> next)
    {
        logger.LogDebug("GpoBuzzerActivity|BuzzerGpoExpired on ReaderId: {ReaderId}", context.Message.CorrelationId);

        Reader reader = (await readerRepository.GetAsync(q => q.Id == context.Saga.ReaderId)).First();
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

            await gpoUtility.SetGpo(reader.Id, reader.Name, gpo.Buzzer, context.Message.GpoBuzzerValue); //Buzzer off
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw new Exception(ex.Message);
        }

        await next.Execute(context);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<ShipmentState, GpoBuzzerExpired, TException> context, IBehavior<ShipmentState, GpoBuzzerExpired> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("gpoBuzzer");
    }
}