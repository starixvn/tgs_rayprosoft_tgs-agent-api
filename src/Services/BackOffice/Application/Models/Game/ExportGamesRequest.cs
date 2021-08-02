using System;
using System.Net.Http;
using MediatR;

namespace TGS.Services.BackOffice.Application.Models.Game
{
    public record ExportGamesRequest(Guid Id) : IRequest<HttpResponseMessage>;
}