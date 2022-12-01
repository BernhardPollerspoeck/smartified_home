using smart.contract;

namespace smart.core.Models;
public interface IHandlerClient
{
    Task OnNewElements(IEnumerable<StateElement> elements);
    Task OnPollRequest(IEnumerable<StateElement> elements);
    Task OnElementCommand(StateElement element, string command);
}

public interface IHandlerHub
{
    Task OnHandlerAlive(int handlerId, EHandlerType handlerType);
    Task OnElementStatesChanged(IEnumerable<StateElement> elements);
}
