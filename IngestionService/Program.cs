using Confluent.Kafka;
using IngestionService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", false)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File("logs/IngestionApp-.log", rollingInterval: RollingInterval.Day)
        .CreateLogger();
        
    
var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(config);

//setup Http Client Config
services.AddHttpClient<HttpClientService>(client =>
{
    client.BaseAddress = new Uri(config["BASE_ADDRESS_URL"] ?? 
            throw new InvalidOperationException("Invalid base Address"));
    client.Timeout = TimeSpan.FromSeconds(30);
});

//setup Kafka Producer config
services.AddSingleton<IProducer<Null, string>>(_ =>
{
    var producerConfig = new ProducerConfig
    {
        BootstrapServers = config["KAFKA_BOOTSTRAP_SERVER"]
    };

    return new ProducerBuilder<Null, string>(producerConfig).Build();
});

services.AddLogging(log =>
{
    log.ClearProviders();
    log.AddSerilog(Log.Logger);
});

services.AddSingleton<ProducerService>();
services.AddSingleton<StationInformationService>();
services.AddSingleton<StationStatusService>();
services.AddSingleton<VehicleTypeService>();
services.AddSingleton<OrchestratorService>();

var provider = services.BuildServiceProvider();
var orchestrator = provider.GetRequiredService<OrchestratorService>();
var producerService = provider.GetRequiredService<ProducerService>();

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    await orchestrator.RunAsync(cts.Token);
}
catch(OperationCanceledException) when (cts.IsCancellationRequested)
{
    System.Console.WriteLine("Operation cancel");
}
finally
{
    producerService.Dispose();
}