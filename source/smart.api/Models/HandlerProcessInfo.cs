using smart.database;
using System.Diagnostics;

namespace smart.api.Models;

public class HandlerProcessInfo
{
    public ElementHandler Handler { get; }
    public Process Process { get; }
    public string HandlerExecuteable { get; }

    public HandlerProcessInfo(ElementHandler handler, Process process, string handlerExecuteable)
    {
        Handler = handler;
        Process = process;
        HandlerExecuteable = handlerExecuteable;
    }

}
