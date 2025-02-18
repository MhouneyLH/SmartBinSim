namespace SmartBinSensor.SensorMessage;

internal sealed record SensorMessage(SensorMessageId Id, DateTime Timestamp, double FillLevel);
