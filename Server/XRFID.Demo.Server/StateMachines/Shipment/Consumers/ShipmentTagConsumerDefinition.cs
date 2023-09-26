using MassTransit;

namespace XRFID.Demo.Server.StateMachines.Shipment.Consumers;

internal class ShipmentTagConsumerDefinition :
    ConsumerDefinition<ShipmentTagConsumer>
{
    readonly IServiceProvider serviceProvider;

    public ShipmentTagConsumerDefinition(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        ConcurrentMessageLimit = 100;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<ShipmentTagConsumer> consumerConfigurator)
    {
        //endpointConfigurator.UseMessageRetry(r =>
        //{
        //    r.Handle<DbUpdateConcurrencyException>();
        //    r.Handle<DbUpdateException>(y => y.InnerException is SqlException e && e.Number == 2627); // This is the SQLServer error code for duplicate key, if you are using another Relational Db, the code might be different
        //    //r.Handle<DbUpdateException>(y => SqlServerTransientExceptionDetector.ShouldRetryOn(y.InnerException)); // This uses the same detector as EFCore Connection Resiliency
        //    r.Interval(5, TimeSpan.FromMilliseconds(200));
        //});
        //endpointConfigurator.UseInMemoryOutbox();

        endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
        endpointConfigurator.UseInMemoryOutbox();
    }

}
