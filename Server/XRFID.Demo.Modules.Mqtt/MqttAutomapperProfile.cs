using AutoMapper;
using MQTTnet;

namespace XRFID.Demo.Modules.Mqtt;

public class MqttAutomapperProfile : Profile
{
    public MqttAutomapperProfile()
    {
        CreateMap<MqttApplicationMessage, ZebraMqttApplicationMessage>().ReverseMap().ForAllMembers(opt => opt.AllowNull());
    }
}
