using System.CommandLine;
using SmartBinSensor;

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
        var sensorData = new SensorData(
            new SensorDataId(Guid.NewGuid()),
            DateTime.UtcNow,
            initialFillPercentage
        );
        var random = new Random();

        var canellationSource = new CancellationTokenSource();
        var token = canellationSource.Token;

        while (!token.IsCancellationRequested)
        {
            sensorData = sensorData with
            {
                Timestamp = DateTime.UtcNow,
                FillLevel = Math.Min(100.0, sensorData.FillLevel + random.NextDouble()),
            };

            if (verbose)
            {
                Console.WriteLine($"SensorData: {sensorData}");
            }

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

namespace SmartBinSensor
{
    internal sealed record SensorDataId(Guid Value);

    internal sealed record SensorData(SensorDataId Id, DateTime Timestamp, double FillLevel);

    internal static class DefaultConfig
    {
        public static readonly int GENERATION_INTERVAL_IN_MS = 1000;
        public static readonly double INITIAL_FILL_PERCENTAGE = 0.0;
        public static readonly bool VERBOSE = false;
    };
}
