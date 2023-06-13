using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SagaVsConsumers.Persistence;

public class FlowContextFactory : IDesignTimeDbContextFactory<FlowContext>
{
    public FlowContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<FlowContext>();

        optionsBuilder.UseSqlServer(configuration.GetConnectionString("Database"), m =>
        {
            m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            m.MigrationsHistoryTable($"__{nameof(FlowContext)}");
        });

        return new FlowContext(optionsBuilder.Options);
    }
}