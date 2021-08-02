using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class BrandLiveChat
    {
        public Guid BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public string ChatName { get; set; }
        public string ChatSetting { get; set; }
    }
}