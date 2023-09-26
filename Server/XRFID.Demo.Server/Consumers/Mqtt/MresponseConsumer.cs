using MassTransit;
using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Payloads;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Repositories;

namespace XRFID.Demo.Server.Consumers.Mqtt;

public class MresponseConsumer : IRequestConsumer<GetVersionResponse>,
                                 IRequestConsumer<GetNetworkResponse>,
                                 IRequestConsumer<GetStatusResponse>,
                                 IRequestConsumer<GetModeResponse>
{
    private readonly ReaderRepository readerRepository;
    private readonly UnitOfWork uowk;
    private readonly ILogger<MresponseConsumer> logger;

    public MresponseConsumer(ReaderRepository readerRepository,
                             UnitOfWork uowk,
                             ILogger<MresponseConsumer> logger)
    {
        this.readerRepository = readerRepository;
        this.uowk = uowk;
        this.logger = logger;
    }


    public async Task Consume(ConsumeContext<GetVersionResponse> context)
    {
        logger.LogTrace("[Consume<GetVersionResponse>] message: {@message}", context.Message);
        //var readerName = context.Message.HostName;

        var reader = (await readerRepository.GetAsync(q => q.Name == context.Message.HostName)).FirstOrDefault();
        if (reader is null)
        {
            logger.LogWarning("[Consume<GetVersionResponse>] No reader found in table for {readerName}", context.Message.HostName);
            return;
        }

        //update table reader information 
        reader.SerialNumber = context.Message.SerialNumber;
        reader.ReaderOS = context.Message.ReaderOS;
        reader.Model = context.Message.Model;
        reader.Version = context.Message.ReaderApplication;

        await readerRepository.UpdateAsync(reader);
        await uowk.SaveAsync();

    }

    public async Task Consume(ConsumeContext<GetNetworkResponse> context)
    {
        logger.LogTrace("[Consume<GetNetworkResponse>] message: {@message}", context.Message);

        var reader = (await readerRepository.GetAsync(q => q.Name == context.Message.HostName)).FirstOrDefault();
        if (reader is null)
        {
            logger.LogWarning("[Consume<GetNetworkResponse>] No reader found in table for {readerName}", context.Message.HostName);
            return;
        }

        //update table reader information 
        //reader.Name = context.Message.HostName;
        reader.Ip = context.Message.IpAddress;
        reader.Uid = context.Message.MacAddress;
        reader.MacAddress = context.Message.MacAddress;

        await readerRepository.UpdateAsync(reader);
        await uowk.SaveAsync();
    }

    public async Task Consume(ConsumeContext<GetStatusResponse> context)
    {
        logger.LogTrace("[Consume<GetStatusResponse>] message: {@message}", context.Message);
        //var readerName = context.Message.HostName;

        var reader = (await readerRepository.GetAsync(q => q.Name == context.Message.HostName)).FirstOrDefault();
        if (reader is null)
        {
            logger.LogWarning("[Consume<GetStatusResponse>] No reader found in table for {readerName}", context.Message.HostName);
            return;
        }
    }

    public async Task Consume(ConsumeContext<GetModeResponse> context)
    {
        logger.LogTrace("[Consume<GetModeResponse>] message: {@message}", context.Message);
        //var readerName = context.Message.HostName;

        var reader = (await readerRepository.GetAsync(q => q.Name == context.Message.HostName)).FirstOrDefault();
        if (reader is null)
        {
            logger.LogWarning("[Consume<GetModeResponse>] No reader found in table for {readerName}", context.Message.HostName);
            return;
        }

        //update table reader information 
        //reader.Name = context.Message.HostName;
        //reader.Ip = context.Message.IpAddress;
        //reader.Uid = context.Message.MacAddress;
    }
}
