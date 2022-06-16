using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.TCG.Incoming;

public record TCGCloseRequest
{
    [Required]
    [JsonPropertyName("ref_no")]
    public string RefNo { get; init; }
    [Required]
    [JsonPropertyName("username")]
    public string UserName { get; init; }
    [Required]
    [JsonPropertyName("additionalInformation")]
    public JsonDocument AdditionalInformation { get; init; }
}
