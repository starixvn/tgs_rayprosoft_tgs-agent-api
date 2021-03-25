using System;

namespace TGS.Services.Report.Application.Models.Player
{
    public class PlayerModel
    {
        public string Username { get; set; }
        public DateTimeOffset RegisteredOn { get; set; }
        public decimal Contribution { get; set; }
    }
}