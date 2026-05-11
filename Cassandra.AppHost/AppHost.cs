var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithHostPort(54049)
    .WithEnvironment("POSTGRES_PASSWORD", "YYYjzk}ppk*CUP.65!X}!~!")
    .AddDatabase("cassandradb");

var apiService = builder.AddProject<Projects.Cassandra_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.AddProject<Projects.Cassandra_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();