# MassTransit Consumers vs Saga comparison

1. `docker compose up -d`
2. `dotnet ef database update --project SagaVsConsumers`
3. `k6 run ./SagaVsConsumers/load-test.js`
4. In SSMS or Datagrip: `select sum(datediff(ms, CreationDate, CompletionDate)) as totalDurationMs, Origin from Flows group by Origin`