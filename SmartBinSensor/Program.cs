using System.CommandLine;
using System.Text.Json;
using SmartBinSensor;
using SmartBinSensor.SensorMessage;

var rootCommand = new RootCommand("SmartBinSensor")
{
    new Option<int>(
        ["--generation-interval-in-ms", "-gi"],
        () => DefaultConfig.GENERATION_INTERVAL_IN_MS,
        "Generation interval in milliseconds"
    ),
    new Option<double>(
        ["--initial-fill-percentage", "-fp"],
        () => DefaultConfig.INITIAL_FILL_PERCENTAGE,
        "Initial fill percentage"
    ),
    new Option<bool>(["--verbose", "-v"], () => DefaultConfig.VERBOSE, "Verbose output"),
};

rootCommand.SetHandler(
    async (int generationIntervalInMs, double initialFillPercentage, bool verbose) =>
    {
        var sensorMessage = new SensorMessage(
            new SensorMessageId(Guid.NewGuid()),
            DateTime.UtcNow,
            initialFillPercentage
        );
        var random = new Random();

        var canellationSource = new CancellationTokenSource();
        var token = canellationSource.Token;

        while (!token.IsCancellationRequested)
        {
            sensorMessage = sensorMessage with
            {
                Timestamp = DateTime.UtcNow,
                FillLevel = Math.Min(100.0, sensorMessage.FillLevel + random.NextDouble()),
            };

            var converterOptions = new JsonSerializerOptions
            {
                Converters = { new SensorMessageJsonConverter() },
            };
            var jsonSensorMessage = JsonSerializer.Serialize(sensorMessage, converterOptions);


            if (verbose)
            {
                Console.WriteLine($"Sending message: {jsonSensorMessage}");
            }
            // todo: send message to hub

            await Task.Delay(generationIntervalInMs);
        }
    },
    rootCommand
        .Children.OfType<Option<int>>()
        .FirstOrDefault(o => o.Name == "generation-interval-in-ms"),
    rootCommand
        .Children.OfType<Option<double>>()
        .FirstOrDefault(o => o.Name == "initial-fill-percentage"),
    rootCommand.Children.OfType<Option<bool>>().FirstOrDefault(o => o.Name == "verbose")
);

await rootCommand.InvokeAsync(args);