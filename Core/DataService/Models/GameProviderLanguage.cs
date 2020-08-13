using System;

namespace TGS.Core.DataService.Models
{
    public class GameProviderLanguage
    {
        public Guid Id { get; set; }
        public Guid GameProviderId { get; set; }
        public GameProvider GameProvider { get; set; }
        public string CultureCode { get; set; }
        public Language Language { get; set; }
        public string GameProviderCultureCode { get; set; } 
        public int DisplayOrder { get; set; }
    }
}