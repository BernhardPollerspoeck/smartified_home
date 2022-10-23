namespace smart.api.Services.Handlers.ProcessControlling;

public record HandlerControlMessage(
    EHandlerAction Action,
    int HandlerId);
