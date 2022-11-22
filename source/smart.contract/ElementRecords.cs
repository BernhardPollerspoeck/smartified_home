namespace smart.contract;

public record CreateElementDto(
    string Name,
    EHandlerType HandlerType,
    EElementType ElementType,
    string ConnectionInfo);

public record ElementDto(
    int Id,
    string Name,
    EElementType ElementType,
    bool HandlerValidated);

public record ElementCommandDto(
    int Id,
    string Command);

public enum EElementType
{
    Unknown = 0,
    Internal = 1,

    Shelly1 = 2,
}
