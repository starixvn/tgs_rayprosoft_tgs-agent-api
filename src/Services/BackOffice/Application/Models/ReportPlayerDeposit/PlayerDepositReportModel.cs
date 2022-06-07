using System;

namespace TGS.Services.BackOffice.Application.Models.ReportPlayerDeposit
{
    public record PlayerDepositReportModel
    {
        public DateTimeOffset ReportDate { get; init; }

        public int FirstDepositCount { get; init; }

        public int NoSecondDepositCount { get; init; }
    }
}