using MassTransit;
using SagaVsConsumers.Persistence;

namespace SagaVsConsumers.Sagas;

public class FlowSaga : MassTransitStateMachine<FlowState>
{
    public FlowSaga()
    {
        InstanceState(s => s.State);

        Event(() => FlowRequested);
        Event(() => FlowStarted);
        Event(() => FlowCompleted);

        Initially(
            When(FlowRequested)
                .Then(context =>
                {
                    context.Saga.CreationDate = DateTimeOffset.UtcNow;
                    context.Saga.Origin = "Saga";
                })
                .TransitionTo(Started)
                .Publish(context => new FlowStarted
                    { CorrelationId = context.Message.CorrelationId }));

        During(Started,
            When(FlowStarted)
                .Then(context => context.Saga.CompletionDate = DateTimeOffset.UtcNow)
                .TransitionTo(Completed));
    }


    public State Started { get; private set; }

    public State Completed { get; private set; }
    
    public Event<FlowRequested> FlowRequested { get; private set; }
    
    public Event<FlowStarted> FlowStarted { get; private set; }
    
    public Event<FlowCompleted> FlowCompleted { get; private set; }
}