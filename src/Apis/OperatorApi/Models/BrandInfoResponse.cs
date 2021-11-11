using System;

namespace TGS.Apis.OperatorApi.Models
{
    public class BrandInfoResponse
    {
        public Guid BrandId { get; set; }

        public string Code { get; set; }

        public Uri DesktopLogoUrl { get; set; }

        public bool HasInvitationCode { get; set; }

        public Uri LoadingUrl { get; set; }

        public Uri MobileLogoUrl { get; set; }

        public bool SmsEnable { get; set; }
    }
}
