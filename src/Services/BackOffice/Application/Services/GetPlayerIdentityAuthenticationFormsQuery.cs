using System;
using TGS.Services.BackOffice.Application.Commands.PlayerIdentity;
using TGS.Services.BackOffice.Domain;

#nullable enable

namespace TGS.Services.BackOffice.Application.Services;

public record GetPlayerIdentityAuthenticationFormsQuery(
    Guid BrandId,
    PlayerIdentityReviewState? Status,
    string? Username,
    DateTimeOffset? BeginCreatedOn,
    DateTimeOffset? EndCreatedOn,
    DateTimeOffset? BeginReviewedOn,
    DateTimeOffset? EndReviewedOn,
    Paging.Paging<PlayerIdentityAuthenticationInfo> Paging);
