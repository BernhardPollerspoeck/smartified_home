using smart.contract;
using smart.contract.Handler;
using smart.database;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace smart.handler.shelly.Models;
internal class ShellyStateResult
{
    [JsonPropertyName("time")]
    public string Time { get; set; } = default!;
    [JsonPropertyName("relays")]
    public Relay[] Relays { get; set; } = default!;

    public DateTime? HandlerTimestamp { get; set; }
}

internal class Relay
{
    [JsonPropertyName("ison")]
    public bool Ison { get; set; }
}

internal class ShellyHomeElement
{

    public int Id { get; }
    public string? Connection { get; }
    public string Name { get; }
    public EElementType ElementType { get; }
    public string HandlerName { get; }

    public ShellyStateDto? ItemState { get; set; }

    public ShellyHomeElement(HomeElement item)
    {
        Id = item.Id;
        Connection = item.ConnectionInfo;
        Name = item.Name;
        ElementType = item.ElementType;
        HandlerName = item.ElementHandler.Name;

        if (item.StateData is not null)
        {
            ItemState = JsonSerializer.Deserialize<ShellyStateDto>(item.StateData)!;
        }
    }

}
