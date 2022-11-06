namespace smart.contract.Handler;


public record ShellyStateDto(
    int Id,
    bool IsOn,
    string DeviceTime,
    DateTime? StateTimestamp);
