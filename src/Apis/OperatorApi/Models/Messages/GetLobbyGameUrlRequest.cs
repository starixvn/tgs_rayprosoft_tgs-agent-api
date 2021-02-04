using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetLobbyGameUrlRequest : IRequest<GetGameUrlResponse>
    {
        [Required]
        [BindProperty(Name = "gpcode", SupportsGet = true)]
        public string GameProviderCode { get; init; }

        [Required]
        [BindProperty(Name = "gtype", SupportsGet = true)]
        public GameType GameType { get; init; }

        [Required]
        [BindProperty(Name = "language", SupportsGet = true)]
        public string LanguageCode { get; init; }
    }
}