using AutoMapper;
using MassTransit;
using XRFID.Sample.Server.StateMachines.Shipment.Actions;
using XRFID.Sample.Server.StateMachines.Shipment.Contracts;
using XRFID.Sample.Server.StateMachines.Shipment.Interfaces;
using XRFID.Sample.Server.StateMachines.Shipment.Schedule;
using XRFID.Sample.Server.StateMachines.Shipment.States;

namespace XRFID.Sample.Server.StateMachines.Shipment.StateMachines;

public class ShipmentStateMachine :
    MassTransitStateMachine<ShipmentState>
{
    private readonly IConfiguration configuration;
    private readonly IMapper mapper;

    public ShipmentStateMachine(IConfiguration configuration, IMapper mapper)
    {
        this.configuration = configuration;
        this.mapper = mapper;

        InstanceState(x => x.CurrentState);

        //Inizializzazione Scheduler
        Schedule(() => ReadingTimeout, instance => instance.ReadingTimeoutTokenId, s =>
        {
            s.Delay = TimeSpan.FromMilliseconds(10000);

            s.Received = r => r.CorrelateById(context => context.Message.CorrelationId);
        });

        Schedule(() => GpoBuzzerTimeout, instance => instance.GpoBuzzerTimeoutTokenId, s =>
        {
            s.Delay = TimeSpan.FromMilliseconds(3000);

            s.Received = r => r.CorrelateById(context => context.Message.CorrelationId);
        });

        //Inizializzazione Eventi
        Event(() => InitializeEvent, x =>
        {
            x.CorrelateById(context => context.Message.CorrelationId);
            x.InsertOnInitial = true;
            x.SetSagaFactory(context => new ShipmentState
            {
                CorrelationId = context.Message.CorrelationId == Guid.Empty ? NewId.NextGuid() : context.Message.CorrelationId,
                HostName = context.Message.HostName,
                ReaderId = context.Message.ReaderId,
                DateModified = context.Message.Timestamp,
            });
        });

        Event(() => GpiEvent, x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => InitializeEventFaulted, x => x
            .CorrelateById(context => context.Message.Message.CorrelationId)  // Fault<T> includes the original message
            .SelectId(m => m.Message.Message.CorrelationId));

        //***************************** INITIAL STATE ********************************************************

        Initially(
            When(InitializeEvent).IfElse(x => x.Message.GpiValue == true,
            start => start
                .TransitionTo(LoadingData)
                .Activity(x => x.OfType<InitializeActivity>())
                .TransitionTo(Ready),
            stop => stop
                .Finalize()));

        During(Initial,
            Ignore(GpiEvent),
            Ignore(GpoBuzzerEvent),
            Ignore(ReadingTimeout.AnyReceived),
            Ignore(GpoBuzzerTimeout.AnyReceived));

        During(Initial,
            When(InitializeEventFaulted)
                .Unschedule(ReadingTimeout)
                .Activity(x => x.OfType<InitializeEventFaultedActivity>())
                .Then(ctx => ctx.Saga.DateModified = DateTime.Now)
                .Finalize());

        //***************************** Separa Stati ********************************************************

        During(Ready,
            Ignore(InitializeEvent),
            When(GpiEvent, x => x.Message.GpiId >= 2 && x.Message.GpiValue == true)
                .Activity(x => x.OfType<InitializeCreateMovementActivity>())
                .Then(ctx => ctx.Saga.DateModified = DateTime.Now)
                .Activity(x => x.OfType<StartReadingActivity>())
                .Then(ctx => ctx.Saga.DateModified = DateTime.Now)
                .TransitionTo(Reading));



        //***************************** Separa Stati ********************************************************

        During(Reading,
            When(GpiEvent, x => x.Message.GpiId >= 2 && x.Message.GpiValue == false)
                .Schedule(ReadingTimeout, context => context.Init<ReadingExpired>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                }),
                 context => TimeSpan.FromMilliseconds(10000))
                .Then(ctx => ctx.Saga.DateModified = DateTime.Now)
                .TransitionTo(Stopping));

        //***************************** Separa Stati ********************************************************


        During(Stopping,
           When(GpiEvent, x => x.Message.GpiId >= 2 && x.Message.GpiValue == true)
               .Unschedule(ReadingTimeout)
               .Then(ctx => ctx.Saga.DateModified = DateTime.Now)
               .TransitionTo(Reading),


           When(ReadingTimeout.Received)
               .Activity(x => x.OfType<StopReadingTimeoutActivity>())
               .Then(ctx => ctx.Saga.DateModified = DateTime.Now)
               .TransitionTo(Ready));



        //***************************** Separa Stati ********************************************************

        DuringAny(
            When(GpoBuzzerEvent)
                .Schedule(GpoBuzzerTimeout, context => context.Init<GpoBuzzerExpired>(new
                {
                    CorrelationId = context.Message.CorrelationId,
                    ReaderId = context.Message.ReaderId,
                    GpoBuzzerId = context.Message.GpoBuzzerId,
                    GpoBuzzerValue = !context.Message.GpoBuzzerValue, //buzzer off value
                    Timestamp = context.Message.Timestamp,
                }),
                 context => TimeSpan.FromMilliseconds(2000)),
            When(GpoBuzzerTimeout.Received)
               .Activity(x => x.OfType<GpoBuzzerActivity>()));

        DuringAny(
            When(GpiEvent, x => x.Message.GpiId == 1 && x.Message.GpiValue == false)
            .Unschedule(ReadingTimeout)
            .Unschedule(GpoBuzzerTimeout)
            .Activity(x => x.OfType<StopReadingActivity>())
            .TransitionTo(Consolidate).PublishAsync(context => context.Init<ShipmentMovementConsolidateRequest>(new
            {
                ReaderId = context.Saga.ReaderId,
                CorrelationId = context.Saga.CorrelationId,
                MovementId = context.Saga.MovementId,
            }))
            .Then(ctx =>
            {
                ctx.Saga.DateModified = DateTime.Now;
                ctx.Saga.DateCompleted = DateTime.Now;
            })
            .Finalize());

        DuringAny(
            When(InitializeEventFaulted)
            .Unschedule(ReadingTimeout)
            .Unschedule(GpoBuzzerTimeout)
            .Activity(x => x.OfType<InitializeEventFaultedActivity>())
            .Finalize());


        During(Final,
            Ignore(GpiEvent),
            Ignore(ReadingTimeout.AnyReceived),
            Ignore(GpoBuzzerEvent),
            Ignore(GpoBuzzerTimeout.AnyReceived),
            Ignore(InitializeEvent));

    }

    public Event<IInitializeEvent> InitializeEvent { get; private set; }
    public Event<Fault<IInitializeEvent>> InitializeEventFaulted { get; private set; }
    public Event<IGpiEvent> GpiEvent { get; private set; }
    public Event<IGpoBuzzerEvent> GpoBuzzerEvent { get; private set; }


    public State LoadingData { get; private set; }
    public State Ready { get; private set; }
    public State Stopping { get; private set; }
    public State Reading { get; private set; }
    public State Consolidate { get; private set; }
    public State Mismatch { get; private set; }

    public Schedule<ShipmentState, ReadingExpired> ReadingTimeout { get; private set; }
    public Schedule<ShipmentState, GpoBuzzerExpired> GpoBuzzerTimeout { get; private set; }
}
