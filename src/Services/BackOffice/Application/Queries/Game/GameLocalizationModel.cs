using System.Collections.Generic;
using System.Text.Json;

namespace TGS.Services.BackOffice.Application.Queries.Game
{
    public class GameLocalizationModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, JsonDocument> Localization { get; set; }
    }
}