using System.Text.Json.Serialization;
using TGS.Services.Operator;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetAnnouncementsResponse
    {
        [JsonPropertyName("data")]
        public Announcement[] Announcements { get; init; }
    }
}