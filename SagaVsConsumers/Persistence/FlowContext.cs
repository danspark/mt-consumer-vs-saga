using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace SagaVsConsumers.Persistence;

public class FlowContext : SagaDbContext
{
    public FlowContext(DbContextOptions<FlowContext> options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations => new[] { new FlowMap() };
}