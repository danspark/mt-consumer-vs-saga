using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaVsConsumers.Persistence;
using SagaVsConsumers.Sagas;

namespace SagaVsConsumers.Consumers;

public record StarterConsumer(FlowContext Db) : IConsumer<FlowRequested>
{
    public async Task Consume(ConsumeContext<FlowRequested> context)
    {
        var state = await Db.Set<FlowState>()
            .SingleOrDefaultAsync(s => s.CorrelationId == context.Message.CorrelationId);

        if (state is not null) throw new InvalidOperationException("Already created");
        
        state = new FlowState
        {
            State = "Started",
            CorrelationId = context.Message.CorrelationId,
            Origin = "Consumer",
            CreationDate = DateTimeOffset.UtcNow
        };
        
        Db.Add(state);

        await Db.SaveChangesAsync();
        
        await context.Publish(new FlowStarted
        {
            CorrelationId = context.Message.CorrelationId
        });
    }
}

public record FinisherConsumer(FlowContext Db) : IConsumer<FlowStarted>
{
    public async Task Consume(ConsumeContext<FlowStarted> context)
    {
        var state = await Db.Set<FlowState>()
            .SingleOrDefaultAsync(s => s.CorrelationId == context.Message.CorrelationId);
        
        if (state is null) throw new InvalidOperationException("not found");

        state.State = "Completed";
        state.CompletionDate = DateTimeOffset.UtcNow;

        await Db.SaveChangesAsync();
    }
}