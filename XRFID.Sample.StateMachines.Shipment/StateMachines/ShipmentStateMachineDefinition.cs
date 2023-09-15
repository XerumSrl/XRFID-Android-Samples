using MassTransit;
using XRFID.Server.StateMachines.Shipment.States;

namespace XRFID.Sample.StateMachines.Shipment.StateMachines;
public class ShipmentStateMachineDefinition : SagaDefinition<ShipmentState>
{
    private const int ConcurrencyLimit = 120; // this can go up, depending upon the database capacity

    public ShipmentStateMachineDefinition()
    {
        ConcurrentMessageLimit = 100;
        Endpoint(e =>
        {
            e.PrefetchCount = ConcurrencyLimit;
        });
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<ShipmentState> sagaConfigurator)
    {
        //endpointConfigurator.UseRetry(r =>
        //{
        //    r.Handle<DbUpdateConcurrencyException>();
        //    r.Handle<DbUpdateException>(y => y.InnerException is SqlException e && e.Number == 2627); // This is the SQLServer error code for duplicate key, if you are using another Relational Db, the code might be different
        //    //r.Handle<DbUpdateException>(y => SqlServerTransientExceptionDetector.ShouldRetryOn(y.InnerException)); // This uses the same detector as EFCore Connection Resiliency
        //    //r.Interval(5, TimeSpan.FromMilliseconds(200));
        //});
        endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
        endpointConfigurator.UseInMemoryOutbox();

        //var partition = endpointConfigurator.CreatePartitioner(ConcurrencyLimit);

        //sagaConfigurator.Message<ITagEvent>(x => x.UsePartitioner(partition, m => m.Message.Epc));

    }
}