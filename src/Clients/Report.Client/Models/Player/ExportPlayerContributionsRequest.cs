using System;

namespace TGS.Clients.Report.Client.Models.Player
{
    public record ExportPlayerContributionsRequest(Guid BrandId, string Username, int? Level, DateTimeOffset From,
        DateTimeOffset To, string Culture);
}