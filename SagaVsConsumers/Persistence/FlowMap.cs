using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagaVsConsumers.Sagas;

namespace SagaVsConsumers.Persistence;

public class FlowMap : SagaClassMap<FlowState>
{
    protected override void Configure(EntityTypeBuilder<FlowState> entity, ModelBuilder model)
    {
        base.Configure(entity, model);

        entity.Property(s => s.ETag).IsRowVersion();
        
        entity.ToTable("Flows");
    }
}