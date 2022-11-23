using System.Text.Json.Serialization;

namespace smart.handler.shelly.Models;

internal class Relay
{
    [JsonPropertyName("ison")]
    public bool Ison { get; set; }
}
