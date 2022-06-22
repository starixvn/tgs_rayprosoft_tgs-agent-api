using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TF;

public class LaunchResponse
{
    [JsonPropertyName("launch_url")]
    public string LaunchUrl { get; set; }
}
