namespace smart.contract;

public record CreateElementDto(
    string Name,
    EHandlerType ElementType,
    string ConnectionInfo);

public record ElementDto(
    int Id,
    string Name,
    EHandlerType ElementType,
    bool HandlerValidated);

public record ElementCommandDto(
    int Id,
    string Command);