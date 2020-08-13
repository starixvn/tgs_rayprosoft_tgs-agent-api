using System;

namespace TGS.Core.DataService.Models
{
    public class GameProviderConfiguration : BaseEntity
    {
        public string Description { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SecurityKey { get; set; }
        public Guid GameProviderId { get; set; }
        public GameProvider GameProvider { get; set; }
        public string Endpoint { get; set; }
        public string ConfigurationData { get; set; }
        public bool SuppressDescription { get; set; }
    }
}