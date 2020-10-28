using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Payment
{
    public class PaymentUserAccessControlModel
    {
        public ListType ListType { get; set; }
        public string PlayerCode { get; set; }
        public string PlayerName { get; set; }
    }
}