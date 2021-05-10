using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Player
{
    public record CreatePlayerAdjustmentRequest : IRequest<MessageResponse>
    {
        public string PlayerCode { get; set; }
        public AdjustmentType AdjustType { get; set; }
        public AdjustmentTransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}