var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL container is configured with an auto-generated password by default
// and supports setting the default database name via an environment variable & running *.sql/*.sh scripts in a bind mount.
var todosDbName = "Todos";

var postgres = builder.AddPostgres("postgres")
    // Set the name of the default database to auto-create on container startup.
    .WithEnvironment("POSTGRES_DB", todosDbName)
    // Mount the SQL scripts directory into the container so that the init scripts run.
    .WithBindMount("../DatabaseContainers.ApiService/data/postgres", "/docker-entrypoint-initdb.d")
    // Configure the container to store data in a volume so that it persists across instances.
    .WithDataVolume()
    .WithPgWeb()
    // Keep the container running between app host sessions.
    .WithLifetime(ContainerLifetime.Persistent);

// Add the default database to the application model so that it can be referenced by other resources.
var todosDb = postgres.AddDatabase(todosDbName);



builder.AddProject<Projects.DatabaseContainers_ApiService>("apiservice")
    .WithReference(todosDb)
    .WaitFor(todosDb);

builder.Build().Run();
