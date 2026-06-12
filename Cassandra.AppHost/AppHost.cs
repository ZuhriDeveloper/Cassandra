var builder = DistributedApplication.CreateBuilder(args);

// ── Database ───────────────────────────────────────────────────────────────────
// Pass the password via Aspire's parameter system so it is used consistently in BOTH
// the container env var (POSTGRES_PASSWORD) AND the injected connection strings.
var pgPassword = builder.AddParameter("postgres-password", "YYYjzk}ppk*CUP.65!X}!~!", secret: true);

var postgres = builder.AddPostgres("postgres", password: pgPassword)
    .WithDataVolume()
    .WithHostPort(54049)
    .WithLifetime(ContainerLifetime.Persistent);

var cassandradb = postgres.AddDatabase("cassandradb");

var webApi = builder.AddProject<Projects.Cassandra_WebApi>("webapi")
    .WithHttpHealthCheck("/health")
    .WithReference(cassandradb)
    .WaitFor(cassandradb);

builder.AddProject<Projects.Cassandra_WebUi>("webui")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(webApi)
    .WaitFor(webApi);

builder.Build().Run();
