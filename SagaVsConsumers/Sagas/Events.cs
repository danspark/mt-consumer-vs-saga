using MassTransit;

namespace SagaVsConsumers.Sagas;

public abstract record SagaEvent : CorrelatedBy<Guid>
{
    public required Guid CorrelationId { get; init; }
}

public record FlowRequested : SagaEvent;

public record FlowStarted : SagaEvent;

public record FlowCompleted : SagaEvent;