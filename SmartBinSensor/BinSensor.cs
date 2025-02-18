using System.Text;
using System.Text.Json;
using SmartBinSensor.SensorMessages;

namespace SmartBinSensor;

internal sealed class BinSensor
{
    private readonly BinSensorId _id;
    private readonly int _generationIntervalInMs;
    private readonly double _initialFillPercentage;
    private readonly bool _verbose;
    private readonly HttpClient _httpClient;

    private const double MAX_FILL_PERCENTAGE = 120.0;

    private readonly JsonSerializerOptions converterOptions =
        new() { Converters = { new SensorMessageJsonConverter() } };

    private BinSensor() { }

    private BinSensor(
        int generationIntervalInMs,
        double initialFillPercentage,
        bool verbose,
        HttpClient httpClient
    )
    {
        _id = new BinSensorId(Guid.NewGuid());
        _generationIntervalInMs = generationIntervalInMs;
        _initialFillPercentage = initialFillPercentage;
        _verbose = verbose;
        _httpClient = httpClient;
    }

    public static BinSensor CreateNew(
        int generationIntervalInMs,
        double initialFillPercentage,
        bool verbose,
        HttpClient httpClient
    ) => new(generationIntervalInMs, initialFillPercentage, verbose, httpClient);

    public async Task MeasureAsync()
    {
        var sensorMessage = new SensorMessage(_id, DateTime.UtcNow, _initialFillPercentage);

        var random = new Random();

        var canellationSource = new CancellationTokenSource();
        var token = canellationSource.Token;

        while (!token.IsCancellationRequested)
        {
            sensorMessage = sensorMessage with
            {
                Timestamp = DateTime.UtcNow,
                FillPercentage = Math.Min(
                    MAX_FILL_PERCENTAGE,
                    sensorMessage.FillPercentage + random.NextDouble()
                ),
            };

            await SendSensorMessageAsync(sensorMessage);
            await Task.Delay(_generationIntervalInMs);
        }
    }

    private async Task SendSensorMessageAsync(SensorMessage sensorMessage)
    {
        var jsonSensorMessage = JsonSerializer.Serialize(sensorMessage, converterOptions);

        if (_verbose)
        {
            Console.WriteLine($"Sending message to hub: {jsonSensorMessage}");
        }

        var content = new StringContent(jsonSensorMessage, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("sensor-data", content);

        response.EnsureSuccessStatusCode();
    }
}
