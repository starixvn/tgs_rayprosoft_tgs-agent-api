using System;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace TGS.Services.BackOffice.Application.Models.GameProvider
{
    public class UploadGamesRequest : IRequest<UploadGamesResponse>
    {
        public Guid? Id { get; set; }
        public IFormFile File { get; set; }
    }
}