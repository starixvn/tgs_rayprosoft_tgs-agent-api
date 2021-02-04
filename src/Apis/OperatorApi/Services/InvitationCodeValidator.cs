using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;

namespace TGS.Apis.OperatorApi.Services
{
    public interface IInvitationCodeValidator
    {
        Task VerifyInviteeAsync(string inviterPlayerCode, Guid inviteeBrandId, string inviteeCurrency,
            string invitationCode);
    }

    public class InvitationCodeValidator : IInvitationCodeValidator
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandOperations _brandOperations;

        public InvitationCodeValidator(
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations)
        {
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
        }

        async Task IInvitationCodeValidator.VerifyInviteeAsync(string inviterPlayerCode, Guid inviteeBrandId, string inviteeCurrency, string invitationCode)
        {
            if (inviterPlayerCode.IsNullOrEmpty())
            {
                throw new InvalidInvitationCodeException(invitationCode, "No inviter player is found");
            }

            var inviter = await _playerOperations.GetCachedPlayerAsync(inviterPlayerCode);
            if (inviter.BrandId != inviteeBrandId)
            {
                var brandCode = await _brandOperations.GetBrandCodeByIdAsync(inviteeBrandId);
                throw new InvalidInvitationCodeException(invitationCode, $"Expected inviter player belongs to brand '{brandCode}' but is '{inviter.BrandCode}' ");
            }

            if (inviter.DefaultWalletCurrencyCode.IgnoreCaseNotEquals(inviteeCurrency))
            {
                throw new InvalidInvitationCodeException(invitationCode, $"Expected inviter player with currency '{inviteeCurrency}' but is '{inviter.DefaultWalletCurrencyCode}' ");
            }
        }
    }
}
