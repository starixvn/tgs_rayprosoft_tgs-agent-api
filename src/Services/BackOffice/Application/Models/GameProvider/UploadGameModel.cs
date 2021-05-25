using System;

namespace TGS.Services.BackOffice.Application.Models.GameProvider
{
    public class UploadGameModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string RuleUrl { get; set; }
        public string ExternalId { get; set; }
        public string GameType { get; set; }
        public string Platform { get; set; }
        public Boolean? HasTurnoverOnTieBet { get; set; }
        public string EndpointPath { get; set; }
        public string Description { get; set; }
        public Boolean? SubGame { get; set; }
        public string AvailableCurrencies { get; set; }
    }
}