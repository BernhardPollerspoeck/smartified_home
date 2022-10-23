namespace smart.contract;

public record HandlerDto(
    int Id,
    string Name,
    EHandlerType HandlerType,
    bool Enabled,
    bool ProcessRunning,
    bool SignalConnected);


public record CreateHandlerDto(
    string Name,
    EHandlerType HandlerType);



public enum EHandlerType
{
    Unknown = 0,
    Internal = 1,

    Shelly1 = 2,
}
