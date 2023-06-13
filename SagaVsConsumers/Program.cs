using System.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaVsConsumers.Persistence;
using SagaVsConsumers.Consumers;
using SagaVsConsumers.Sagas;

var builder = WebApplication.CreateBuilder(args);

var useSaga = builder.Configuration.GetValue<bool>("UseSaga");

builder.Services.AddMassTransit(mt =>
{
    if (useSaga)
    {
        mt.AddSagaStateMachine<FlowSaga, FlowState>(saga =>
            {
                // could be used if we need to process a message type differently than others (concurrency, retry, etc)
                // saga.Message<FlowRequested>(conf => { conf.UseCircuitBreaker(); });
            })
            .EntityFrameworkRepository(ef =>
            {
                ef.ConcurrencyMode = ConcurrencyMode.Optimistic;
                ef.IsolationLevel = IsolationLevel.Snapshot;

                ef.AddDbContext<FlowContext, FlowContext>((p, b) =>
                {
                    b.UseSqlServer(p.GetRequiredService<IConfiguration>().GetConnectionString("Database"));
                });
            });
    }
    else
    {
        mt.AddDbContext<FlowContext, FlowContext>((p, b) =>
        {
            b.UseSqlServer(p.GetRequiredService<IConfiguration>().GetConnectionString("Database"));
        });
        
        mt.AddConsumersFromNamespaceContaining<ConsumerEvent>();
    }
    
    mt.UsingRabbitMq((ctx, rmq) =>
    {
        rmq.UseMessageRetry(r => r.Immediate(3));
        
        rmq.UseInMemoryOutbox();
        
        rmq.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

app.MapPost("/", async (IPublishEndpoint pub) =>
{
    await pub.Publish(useSaga
        ? new SagaVsConsumers.Sagas.FlowRequested
        {
            CorrelationId = Guid.NewGuid()
        }
        : new SagaVsConsumers.Consumers.FlowRequested
        {
            CorrelationId = Guid.NewGuid()
        });
});

app.Run();