using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace XRFID.Demo.Server.Consumers.Mqtt;

public class GpioDataConsumerDefinition : ConsumerDefinition<GpioDataConsumer>
{
    public GpioDataConsumerDefinition()
    {
        // override the default endpoint name, for whatever reason
        //EndpointName = "ha-submit-order";

        // limit the number of messages consumed concurrently
        // this applies to the consumer only, not the endpoint
        ConcurrentMessageLimit = 100;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<GpioDataConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r =>
        {
            r.Handle<DbUpdateConcurrencyException>();
            r.Handle<DbUpdateException>(y => y.InnerException is SqlException e && e.Number == 2627); // This is the SQLServer error code for duplicate key, if you are using another Relational Db, the code might be different
            //r.Handle<DbUpdateException>(y => SqlServerTransientExceptionDetector.ShouldRetryOn(y.InnerException)); // This uses the same detector as EFCore Connection Resiliency
            r.Interval(5, TimeSpan.FromMilliseconds(100));
        });
        endpointConfigurator.UseInMemoryOutbox();
    }
}