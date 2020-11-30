using System.Linq;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.DataService
{
    public interface IPaymentMappingShared
    {
        string[] GetDesignatedUsers(Payment payment);
    }

    public class PaymentMappingShared : IPaymentMappingShared
    {
        string[] IPaymentMappingShared.GetDesignatedUsers(Payment payment)
        {
            var vipLevelIds = payment.VipLevels.Select(x => x.VipLevelId).ToArray();
            var playerCodes = payment.PaymentPlayerList.Where(x => x.ListType == PlayerListType.White)
                .Select(p => p.PlayerCode).ToArray();
            var isAll = payment.PlayerListCondition == PlayerListCondition.All;

            return payment.PaymentPlayerList
                .Where(x => playerCodes.Contains(x.PlayerCode))
                .Where(x =>
                    x.Player.AccumulatedDeposit >= payment.DepositedAmountMin &&
                    (payment.DepositedAmountUnlimited || x.Player.AccumulatedDeposit < payment.DepositedAmountMax))
                .Where(isAll && payment.IdentityAuthentication,
                    x => x.Player.IdentityAuthentication)
                .Where(isAll && payment.PhoneAuthentication,
                    x => x.Player.PhoneAuthentication)
                .Where(isAll && payment.VipLevels.Any(),
                    x => x.Player.VipId != null && vipLevelIds.Contains(x.Player.VipId.Value))
                .Select(x => x.Player.RealName).ToArray();
        }
    }
}