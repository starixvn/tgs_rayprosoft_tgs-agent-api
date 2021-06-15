using System.Text.Json.Serialization;

namespace TGS.Services.Operator
{
    public record AnnouncementInfo
    {
        [JsonPropertyName("data")]
        public Announcement[] Announcements { get; init; }
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