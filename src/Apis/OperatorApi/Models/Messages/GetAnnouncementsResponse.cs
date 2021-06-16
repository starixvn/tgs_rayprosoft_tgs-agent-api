using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetAnnouncementsResponse
    {
        [JsonPropertyName("data")]
        public Announcement[] Announcements { get; init; }
    }
}