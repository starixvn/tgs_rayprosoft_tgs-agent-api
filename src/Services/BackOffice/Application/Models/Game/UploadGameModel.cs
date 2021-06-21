using System;

namespace TGS.Services.BackOffice.Application.Models.Game
{
    public class UploadGameModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string RuleUrl { get; set; }
        public string ExternalId { get; set; }
        public string Type { get; set; }
        public string PlatformType { get; set; }
        public Boolean? HasTurnoverOnTieBet { get; set; }
        public string EndpointPath { get; set; }
        public string Description { get; set; }
        public Boolean? SubGame { get; set; }
        public Boolean? AvailableToBrand { get; set; }
        public Boolean? Html5 { get; set; }
        public string AvailableCurrencies { get; set; }
    }
}