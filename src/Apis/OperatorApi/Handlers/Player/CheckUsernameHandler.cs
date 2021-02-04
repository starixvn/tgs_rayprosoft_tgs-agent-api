using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Extensions;
using TGS.Services.Operations;

namespace TGS.Apis.OperatorApi.Handlers.Player
{
    public sealed class CheckUsernameHandler : IRequestHandler<CheckUsernameRequest, CheckUsernameResponse>
    {
        private readonly HttpContext _httpContext;
        private readonly IPlayerOperations _playerOperations;

        public CheckUsernameHandler(
            IHttpContextAccessor httpContextAccessor,
            IPlayerOperations playerOperations)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _playerOperations = playerOperations;
        }

        async Task<CheckUsernameResponse> IRequestHandler<CheckUsernameRequest, CheckUsernameResponse>.Handle(
            CheckUsernameRequest request, CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();
            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(brandId, request.Username);

            return new()
            {
                Exists = playerCode.IsNotNullOrEmpty()
            };
        }
    }
}