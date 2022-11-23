using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smart.core.Models;
public class StateElement
{
    public const string IS_ON = "STATE_IS_ON";

    public required int Id { get; init; }
    public required string? Connection { get; init; }
    public Dictionary<string, object?> Properties { get; init; }
    public required string? State { get; set; }

    public StateElement()
    {
        Properties = new()
        {
            { IS_ON,null },
        };
    }

}

