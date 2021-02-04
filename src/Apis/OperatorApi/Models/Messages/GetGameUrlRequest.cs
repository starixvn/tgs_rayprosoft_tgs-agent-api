using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetGameUrlRequest : IRequest<GetGameUrlResponse>
    {
        [Required]
        [BindProperty(Name = "gpcode", SupportsGet = true)]
        public string GameProviderCode { get; init; }

        [Required]
        [BindProperty(Name = "gid", SupportsGet = true)]
        public int GameId { get; init; }

        [Required]
        [BindProperty(Name = "language", SupportsGet = true)]
        public string LanguageCode { get; init; }
    }
}