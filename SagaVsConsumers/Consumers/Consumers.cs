using System.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaVsConsumers.Persistence;

namespace SagaVsConsumers.Consumers;

public record StarterConsumer(FlowContext Db) : IConsumer<FlowRequested>
{
    public async Task Consume(ConsumeContext<FlowRequested> context)
    {
        await using var transaction = await Db.Database.BeginTransactionAsync(IsolationLevel.Snapshot);
        
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

        await transaction.CommitAsync();
        
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
        await using var transaction = await Db.Database.BeginTransactionAsync(IsolationLevel.Snapshot);
        
        var state = await Db.Set<FlowState>()
            .SingleOrDefaultAsync(s => s.CorrelationId == context.Message.CorrelationId);
        
        if (state is null) throw new InvalidOperationException("not found");

        state.State = "Completed";
        state.CompletionDate = DateTimeOffset.UtcNow;

        await transaction.CommitAsync();
        
        await Db.SaveChangesAsync();
    }
}