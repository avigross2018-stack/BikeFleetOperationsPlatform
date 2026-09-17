

using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ProcessingService.Db;
using ProcessingService.Repos;
using ProcessingService.Services;
using StackExchange.Redis;

//Variables configuration
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", false)
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(config);

//Kafka Consumer configuration
services.AddScoped<IConsumer<Ignore, string>>(_ =>
{
    var consumerConfig = new ConsumerConfig
    {
        BootstrapServers = config["KAFKA_BOOTSTRAP_SERVER"],
        GroupId = config["KAFKA_GROUP_ID"],
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = false
    };

    return new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
});

//Redis Configuration
services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    return ConnectionMultiplexer.Connect(config["REDIS_CONNECTION"]!);
});

//Mongo Configuration
services.AddSingleton<IMongoDatabase>(_ =>
{
    var client = new MongoClient(config["MONGO_CONNECTION_STRING"]);
    return client.GetDatabase(config["MONGO_DATABASE"]);
});

//MySql cConfiguration
services.AddDbContext<BikesDbContext>(op =>
{
    op.UseMySql(config["MYSQL_CONNECTION_STRING"],
    MySqlServerVersion.AutoDetect(config["MYSQL_CONNECTION_STRING"]));
});

services.AddScoped<StationInfoRepo>();
services.AddScoped<VehicleTypesRepo>();

services.AddSingleton<RedisService>();
services.AddSingleton<MongoService>();

services.AddScoped<StationStatusHandler>();
services.AddScoped<StationInfoHandler>();
services.AddScoped<VehicleTypesHandler>();

services.AddScoped<OrchestratorService>();

var orchestratorProvider = services.BuildServiceProvider();
var orchestrator = orchestratorProvider.GetRequiredService<OrchestratorService>();

var mysqlProvider = services.BuildServiceProvider();
var mysql = mysqlProvider.GetRequiredService<BikesDbContext>();

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};
try
{
    await mysql.Database.EnsureCreatedAsync();
    await orchestrator.RunAsync(
    cts.Token,
    [
        config["KAFKA_STATION_INFO_TOPIC"] ??
            throw new InvalidOperationException("Topic name is missing"),
        config["KAFKA_STATION_STATUS_TOPIC"] ??
            throw new InvalidOperationException("Topic name is missing"),
        config["KAFKA_VEHICLE_TYPES_TOPIC"] ??
            throw new InvalidOperationException("Topic name is missing")
    ]);
}
catch(Exception ex)
{
    System.Console.WriteLine($"ERROR: {ex}");
}
