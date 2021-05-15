using System;
using System.Text.Json;

namespace TGS.Services.BackOffice.Application.Models.PlayerBank
{
    public class PlayerBankRequest : BaseRequest
    {
        public string PlayerCode { get; set; }
        public string RealName { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public Guid BrandId { get; set; }
        public Guid PaymentChannelId { get; set; }
        public string BankType { get; set; }
        public JsonDocument Detail { get; set; }
    }
}