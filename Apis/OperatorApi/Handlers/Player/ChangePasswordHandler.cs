using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Handlers.Player
{
    public sealed class ChangePasswordHandler : IRequestHandler<ChangePasswordRequest, ChangePasswordResponse>
    {
        private readonly HttpContext _httpContext;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IPlayerOperations _playerOperations;
        private readonly IPlayerQueries _playerQueries;
        private readonly IStatusValidator _statusValidator;

        public ChangePasswordHandler(
            IHttpContextAccessor httpContextAccessor,
            IPlayerQueries playerQueries,
            IPlayerOperations playerOperations,
            IPasswordHandler passwordHandler,
            IStatusValidator statusValidator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _passwordHandler = passwordHandler;
            _statusValidator = statusValidator;
            _playerQueries = playerQueries;
            _playerOperations = playerOperations;
        }

        async Task<ChangePasswordResponse> IRequestHandler<ChangePasswordRequest, ChangePasswordResponse>.Handle(
            ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var playerTokenData = _httpContext.Items.GetPlayerTokenData();
            var playerCode = playerTokenData.PlayerCode;

            var (salt, password) = await _playerQueries.GetSaltAndPasswordByPlayerCodeAsync(playerCode);
            if (!_passwordHandler.Verify(password, salt, request.OldPassword))
            {
                throw new InvalidCredentialsException(Resources.Player.PasswordMismatched);
            }

            var username = await _playerOperations.GetUsernameByPlayerCodeAsync(playerCode);
            var updatedOn = DateTimeOffset.UtcNow;

            var (newSalt, newPassword) = _passwordHandler.GenerateCredential(request.NewPassword);
            await _playerOperations.ChangePlayerPasswordAsync(new()
            {
                Username = username,
                PlayerCode = playerCode,
                Salt = newSalt,
                Password = newPassword,
                UpdatedOn = updatedOn
            });

            return new()
            {
                UpdateTime = updatedOn
            };
        }
    }
}