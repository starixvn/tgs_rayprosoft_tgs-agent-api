using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ClosedXML.Report;
using Microsoft.Extensions.FileProviders;
using Tgs.Payment.Clients;
using Tgs.Promotion.Clients;
using Tgs.Promotion.Clients.Models.Promotion;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Application.Models.Deposit;

namespace TGS.Services.BackOffice.Application.Queries.Deposit;

public class ExportDepositFormService : IGetService<ExportDepositForm, MemoryStream>
{
    private static readonly EmbeddedFileProvider _EmbeddedFileProvider =
        new EmbeddedFileProvider(Assembly.GetExecutingAssembly());

    private readonly IPaymentServiceClient _paymentServiceClient;
    private readonly IPromotionClient _promotionClient;
    private readonly IFileInfo _templateFileInfo;

    public ExportDepositFormService(
        IPaymentServiceClient paymentServiceClient,
        IPromotionClient promotionClient)
    {
        _paymentServiceClient =
            paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        _promotionClient = promotionClient ?? throw new ArgumentNullException(nameof(promotionClient));
        _templateFileInfo = _EmbeddedFileProvider.GetFileInfo("/Assets/deposit_forms_template.xlsx");
    }

    public async ValueTask<MemoryStream> GetAsync(ExportDepositForm query, CancellationToken cancellationToken)
    {
        var result = await _paymentServiceClient.QueryDepositFormsAsync(
            new DepositFormQueryConditions
            {
                BrandId = query.BrandId,
                ChannelName = query.ChannelName,
                CreateOnRange = query.CreateOnRange,
                PaymentMethodId = query.PaymentMethodId,
                Status = query.Status,
                ThirdPartyOrderNo = query.ThirdPartyOrderNo,
                FormId = query.FormId,
                UpdateOnRange = query.UpdateOnRange,
                UserName = query.UserName,
                OrderNo = query.OrderNo,
            },
            new Tgs.Payment.Clients.Paging
            {
                IsDesc = query.Paging.IsDesc,
                ShowAll = true,
                SortBy = query.Paging.SortBy
            });

        var resultPaymentMethods = await _paymentServiceClient.GetAllPaymentMethodsAsync()
        .ToDictionaryAsync(x => x.Id)
            .ConfigureAwait(false);

        var promotionNames = (await _promotionClient
            .GetPromotionNamesAsync(new GetPromotionNamesRequest(
                result.Items
                    .Where(info => info.PromotionId != null)
                    .Select(info => info.PromotionId.Value)
                    .ToArray()))
            .ConfigureAwait(false))
            .ToDictionary(model => model.Id, model => model.Name);

        var promotionBonus = (await _promotionClient.GetPromotionBonusOfDepositAsync(
            result.Items
                .Where(info => info.PromotionPlayerId != null)
                .Select(info => info.PromotionPlayerId.Value)
                .ToArray()).ConfigureAwait(false))
                .ToDictionary(model => model.Id, model => model.BonusAmount);

        var resource = new ResourceManager(typeof(Resources.Deposit));

        var exportData = result.Items?.Select((row, index) => new ExportDepositModel
        {
            SerialNumber = index + 1,
            UserName = row.UserName,
            Id = row.Id.ToString(),
            OrderNo = row.OrderNo,
            ThirdPartyOrderNo = row.ThirdPartyOrderNo,
            PaymentMethod = resource.GetString(resultPaymentMethods[row.PaymentMethodId].Name),
            PaymentName = row.ChannelName,
            Amount = row.Amount,
            ActualAmount = row.ActualAmount,
            ApplicationTime = row.CreatedOn.ToOffset(TimeSpan.FromHours(8D)).DateTime,
            UpdateTime = row.UpdatedOn.ToOffset(TimeSpan.FromHours(8D)).DateTime,
            Status = resource.GetString(row.Status.ToString()) ?? row.Status.ToString(),
            Remark = row.Remark,
            HandlingFee = row.Status == PaymentFlowState.Approved ? row.HandlingFee : null,
            ReviewedUserName = row.ReviewedUserName,
            PromotionName = row.PromotionId != null
                        && promotionNames.TryGetValue(row.PromotionId.Value, out var name)
                        ? name
                        : null,
            Bonus = row.PromotionPlayerId != null
                        && promotionBonus.TryGetValue(row.PromotionPlayerId.Value, out var bonus)
                        ? bonus
                        : null,
        });

        using var workbook = new XLWorkbook();

        GenerateWorkSheet(workbook, exportData, result.Sum);

        var ms = new MemoryStream();

        workbook.SaveAs(ms);
        ms.Position = 0;

        return ms;
    }

    private void GenerateWorkSheet(
        XLWorkbook target,
        IEnumerable<ExportDepositModel> data,
        decimal totalAmount)
    {
        using var fs = _templateFileInfo.CreateReadStream();
        using var template = new XLTemplate(fs);

        template.AddVariable(new
        {
            Rows = data,
            TotalAmount = totalAmount
        });

        template.Generate();

        template.Workbook.Worksheet("template").CopyTo(target, "Deposit");
    }
}
