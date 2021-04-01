using System;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class TeamInfoModel
    {
        public string Username { get; set; }
        public DateTimeOffset RegisteredOn { get; set; }
        public decimal Contribution { get; set; }
    }
}