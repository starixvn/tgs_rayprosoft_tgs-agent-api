using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Lobby : BaseEntity
    {
        public string Description { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public PlatformType PlatformType { get; set; }

        public string Path { get; set; }

        public Guid? LobbyTemplateId { get; set; }

        public virtual ICollection<BitVector32.Section> Sections { get; set; }

        public Guid ResellerId { get; set; }

        public Reseller Reseller { get; set; }

    }
}