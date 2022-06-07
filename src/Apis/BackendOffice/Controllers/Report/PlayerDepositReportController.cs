using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.ReportPlayerDeposit;
using Tgs.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.Report;

[Route("api/[controller]")]
[ApiController]
public class PlayerDepositReportController : BaseContextController
{
    [HttpGet]
    [Description("Get player deposits")]
    public async Task<PagedResponse<PlayerDepositReportModel>> GetPlayerDepositsAsync(
        [FromServices] IGetService<GetPlayerDepositsReportRequest, PagedResponse<PlayerDepositReportModel>>
            playerDepositService,
        [FromQuery] GetPlayerDepositsReportRequest request)
        => await playerDepositService
            .GetAsync(request)
            .ConfigureAwait(false);

    [HttpGet("playerFirstDepositDetails")]
    [Description("Get player first deposit details report")]
    public async Task<PagedResponse<PlayerFirstDepositDetailReportModel>> GetPlayerFirstDepositDetailsAsync(
        [FromServices]
        IGetService<GetPlayerFirstDepositDetailsReportRequest, PagedResponse<PlayerFirstDepositDetailReportModel>>
            playerDepositService,
        [FromQuery] GetPlayerFirstDepositDetailsReportRequest request)
        => await playerDepositService
            .GetAsync(request)
            .ConfigureAwait(false);

    [HttpGet("playerNoSecondDepositDetails")]
    [Description("Get first deposit details report")]
    public async Task<PagedResponse<PlayerNoSecondDepositDetailReportModel>> GetPlayerNoSecondDepositDetailsAsync(
        [FromServices]
        IGetService<GetPlayerNoSecondDepositDetailsReportRequest, PagedResponse<PlayerNoSecondDepositDetailReportModel>>
            playerDepositService,
        [FromQuery] GetPlayerNoSecondDepositDetailsReportRequest request)
        => await playerDepositService
            .GetAsync(request)
            .ConfigureAwait(false);
}