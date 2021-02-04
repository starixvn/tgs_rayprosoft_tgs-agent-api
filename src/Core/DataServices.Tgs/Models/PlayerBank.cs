using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerBank
    {
        public Guid Id { get; set; }
        public string PlayerCode { get; set; }
        public string RealName { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public virtual Player Player { get; set; }
    }
}