using System;

namespace TGS.Core.Models.Services.Data
{
    public record IdAndCodeAndConfigurationId
    {
        public Guid Id { get; init; }
        public string Code { get; init; }
        public Guid ConfigurationId { get; init; }
    }
}