using System.Collections.Generic;
using System.Text.Json;

namespace TGS.Services.Operator
{
    public record AnnouncementInfo(
        string Subject,
        string Content,
        long From,
        long To,
        IDictionary<string, JsonDocument>? Localization);
}