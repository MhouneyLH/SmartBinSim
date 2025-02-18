using System.CommandLine;
using DotNetEnv;
using SmartBinSensor;

Env.Load();

string hubBaseAddress =
    Env.GetString("HUB_BASE_ADDRESS")
    ?? throw new ArgumentException(
        "HUB_BASE_ADDRESS is not set in the .env file. No connection to SmartBinHub will be possible for sending the data"
    );

Console.WriteLine("Will be sending HTTP messages to hub on base address: " + hubBaseAddress);

HttpClient hubHttpClient = new() { BaseAddress = new Uri(hubBaseAddress) };

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
    async (generationIntervalInMs, initialFillPercentage, verbose) =>
    {
        BinSensor sensor = BinSensor.CreateNew(
            generationIntervalInMs,
            initialFillPercentage,
            verbose,
            hubHttpClient
        );

        await sensor.MeasureAsync();
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
