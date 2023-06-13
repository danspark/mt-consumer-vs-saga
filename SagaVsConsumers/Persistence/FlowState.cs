using MassTransit;

namespace SagaVsConsumers.Persistence;

public class FlowState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    
    public string State { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    public DateTimeOffset? CompletionDate { get; set; }

    public string Origin { get; set; }
    
    public byte[] ETag { get; set; }
}