using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetAnnouncementsResponse
    {
        [JsonPropertyName("data")]
        public Announcement[] Announcements { get; init;  }
    }

    public record Announcement
    {
        [JsonPropertyName("subject")]
        public string Subject { get; init; }

        [JsonPropertyName("content")]
        public string Content { get; init; }

        [JsonPropertyName("from")]
        public long From { get; init; }

        [JsonPropertyName("to")]
        public long To { get; init; }
    }
}
