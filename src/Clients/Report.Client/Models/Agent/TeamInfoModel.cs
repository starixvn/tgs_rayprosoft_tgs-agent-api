using System;

namespace TGS.Clients.Report.Client.Models.Agent
{
    public class TeamInfoModel
    {
        public string Username { get; set; }
        public DateTimeOffset RegisteredOn { get; set; }
        public decimal Contribution { get; set; }
    }
}