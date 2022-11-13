namespace smart.contract.Handler;


public class ShellyStateDto
{
    public required int Id { get; init; }
    public required bool IsOn { get; init; }
    public required string DeviceTime { get; init; }
    public required DateTime? StateTimestamp { get; init; }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            ShellyStateDto dto => dto.Id == Id && dto.IsOn == IsOn,
            _ => false
        };
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, IsOn);
    }

}
