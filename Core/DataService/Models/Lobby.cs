using System;
using TGS.Core.DataServices.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Lobby : BaseEntity
    {
        public string Description { get; set; }

        public PlatformType PlatformType { get; set; }

        public string Path { get; set; }

        public Guid ResellerId { get; set; }

        public Reseller Reseller { get; set; }

    }
}