using System;

namespace TGS.Apis.BackendOffice.Models.Agent
{
    public record TeamInfoReportModel
    {
        public string Username { get; init; }

        public DateTimeOffset RegisteredOn { get; init; }

        public decimal Contribution { get; init; }
    }
}