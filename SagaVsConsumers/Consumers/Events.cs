namespace SagaVsConsumers.Consumers;

public abstract record ConsumerEvent
{
    public Guid CorrelationId { get; init; }
}

public record FlowRequested : ConsumerEvent;

public record FlowStarted : ConsumerEvent;

public record FlowCompleted : ConsumerEvent;