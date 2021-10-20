using System;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class TeamInfoModel
    {
        public decimal Contribution { get; set; }

        public DateTimeOffset? RegisteredOn { get; set; }

        public string Username { get; set; }
    }
}
