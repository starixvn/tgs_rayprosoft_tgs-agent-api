using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Controllers;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Round;
using TGS.Core.Models;
using TGS.Core.Models.Messages;
using TGS.Core.Models.Tokens;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PlayerReportTests
    {
        [Test]
        public async Task Player_Report_Should_Order_Desc_By_CreateOn()
        {
            // arrange
            var fakeReportClient = Substitute.For<IReportClient>();

            var playerRoundRequest = new GetPlayerRoundReportRequest()
            {
                From = new DateTimeOffset(new DateTime(2021, 4, 19)),
                To = new DateTimeOffset(new DateTime(2021, 5, 19, 23, 59, 59)),
                Page = 1,
                Rows = 20,
                IsDesc = true,
                SortBy = string.Empty,
                ShowAll = false
            };

            var playerCode = "playerCode";
            var fakeHttpContext = new DefaultHttpContext();
            var brandId = Guid.NewGuid();
            fakeHttpContext.Items.Add(Consts.ContextItems.BrandId, brandId);
            fakeHttpContext.Items.Add("playerToken", new PlayerTokenData { PlayerCode = playerCode });
            var fakeHttpContextAccessor = new HttpContextAccessor { HttpContext = fakeHttpContext };

            var roundsRequest = new GetRoundsRequest
            {
                BrandId = brandId,
                PlayerCode = playerCode,
                From = playerRoundRequest.From,
                To = playerRoundRequest.To,
                Page = playerRoundRequest.Page,
                Rows = playerRoundRequest.Rows,
                OrderBy = playerRoundRequest.IsDesc ? SortOrder.Descending : SortOrder.Ascending,
                SortBy = playerRoundRequest.SortBy,
                ShowAll = playerRoundRequest.ShowAll
            };

            var roundModels = new List<RoundModel>();
            roundModels.Add(new RoundModel() { CreatedOn = new DateTimeOffset(new DateTime(2021, 4, 19)) });
            roundModels.Add(new RoundModel() { CreatedOn = new DateTimeOffset(new DateTime(2021, 5, 10)) });
            roundModels.Add(new RoundModel() { CreatedOn = new DateTimeOffset(new DateTime(2021, 5, 19)) });
            fakeReportClient.GetRoundsAsync(roundsRequest).Returns(new PagedResponse<RoundModel> { Items = roundModels.ToArray() });
            var sut = new PlayerReportController(fakeReportClient);

            sut.ControllerContext = new ControllerContext() { HttpContext = fakeHttpContext };

            var resp = new PagingResponse<RoundModel>() { Items = roundModels.ToArray(), TotalCount = roundModels.Count };

            // act
            //準備3筆假資料
            var r = await sut.GetRoundsAsync(playerRoundRequest);
            r.Returns(resp);
            //var actual = r.Items

            //Assert.IsTrue(r.Items.SequenceEqual(actual));
            Assert.That(roundModels.ToArray(), Is.Ordered.Descending.By("CreatedOn"));
        }
    }
}