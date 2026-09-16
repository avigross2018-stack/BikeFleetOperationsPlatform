

using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessingService.Services;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", false)
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(config);

services.AddSingleton<IConsumer<Ignore, string>>(_ =>
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

services.AddSingleton<ConsumerService>();
services.AddSingleton<RedisService>();