using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Services.BackOffice.Application.Commands.PlayerIdentity;

namespace TGS.Services.BackOffice.Domain;

public interface IBackOfficeRepository
{
    ValueTask BatchUpdateOrAddBrandMaintenanceAsync(Guid[] brandIds, DateTimeOffset beginTime, DateTimeOffset endTime);

    ValueTask ChangeBannerOrderAsync(Guid id, int order, string modifier);

    ValueTask ChangeStatusAsync(Guid id, int status, string modifier);

    ValueTask<Guid> CreateBannerAsync(BannerModifyInfo createInfo, string modifier);

    IAsyncEnumerable<LocalizationStringInfo> ExportLocalizationStringsAsync(string systemCode);

    ValueTask<BannerInfo> GetBannerByIdAsync(Guid id);

    ValueTask<BrandLogoInfo> GetBrandLogoInfoAsync(Guid brandId);

    ValueTask<BrandMaintenanceModel> GetBrandMaintenanceByBrandCodeAsync(Guid brandMaintenanceId);

    ValueTask ImportLocalizationStringsAsync(
            string systemCode,
        IEnumerable<LocalizationStringInfo> localizationStrings);

    IAsyncEnumerable<GameData> QueryAllGamesAsync(Guid brandId);

    IAsyncEnumerable<BrandMaintenanceInfo> QueryBrandMaintenanceByBrandCodeAsync(string brandCode);

    IAsyncEnumerable<BannerInfo> QueryExpiredBannersAsync(Guid brandId);

    IAsyncEnumerable<PlayerIdentityAuthenticationInfo> QueryPlayerIdentityAuthenticationsAsync(
            Guid brandId,
        PlayerIdentityReviewState? status,
        string username,
        DateTimeOffset? beginCreatedOn,
        DateTimeOffset? endCreatedOn,
        DateTimeOffset? beginReviewedOn,
        DateTimeOffset? endReviewedOn);

    IAsyncEnumerable<BannerInfo> QueryRejectedBannersAsync(Guid brandId);

    IAsyncEnumerable<BannerInfo> QueryReviewingBannersAsync(Guid brandId);

    IAsyncEnumerable<BannerInfo> QueryValidBannersAsync(Guid brandId);

    ValueTask ReviewBannerAsync(Guid id, BannerReviewState reviewState, string remark, string reviewer);

    ValueTask SetIsEnableBannerAsync(Guid id, bool isEnable, string modifier);

    ValueTask UpdateBannerAsync(Guid id, BannerModifyInfo updateInfo, string modifier);

    ValueTask UpdateBrandLogoInfoAsync(BrandLogoInfo info, string modifier);

    ValueTask UpdateBrandMaintenanceAsync(Guid brandMaintenanceId, string Content);
}
