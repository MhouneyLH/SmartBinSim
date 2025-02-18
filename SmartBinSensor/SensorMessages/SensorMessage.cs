namespace SmartBinSensor.SensorMessages;

internal sealed record SensorMessage(BinSensorId Id, DateTime Timestamp, double FillPercentage);
