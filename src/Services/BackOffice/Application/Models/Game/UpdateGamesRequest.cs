using System;
using MediatR;

namespace TGS.Services.BackOffice.Application.Models.Game
{
    public class CreateGamesRequest : BaseRequest, IRequest<CreateGamesResponse>
    {
        public Guid GameProviderId { get; set; }
        public GameModel[] Games { get; set; }
    }
}