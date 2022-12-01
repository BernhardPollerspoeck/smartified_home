namespace smart.contract;


public record ImageInfoDto(
    int Id,
    string Title,
    EImageType Type);

public record CreateImageDto(
    EImageType Type,
    string Title,
    byte[] Data);



public enum EImageType
{
    Unknown,

    Plan,
    PlanElement,
}