using System;
using System.IO;
using MediatR;

namespace TGS.Services.Report.Application.Models.Player
{
    public record ExportPlayerContributionsRequest(Guid BrandId, string Username, int? Level, DateTimeOffset From,
        DateTimeOffset To) : IRequest<MemoryStream>;
}