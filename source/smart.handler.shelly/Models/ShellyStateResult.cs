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
