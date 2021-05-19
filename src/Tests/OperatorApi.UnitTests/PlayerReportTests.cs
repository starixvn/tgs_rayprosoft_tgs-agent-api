using System;
using System.Collections.Generic;
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
            fakeHttpContext.Items.Add("playerToken", new PlayerTokenData
            {
                PlayerCode = playerCode
            });

            var roundModels = new List<RoundModel>
            {
                new RoundModel()
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = new DateTimeOffset(new DateTime(2021, 4, 19))
                },
                new RoundModel()
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = new DateTimeOffset(new DateTime(2021, 5, 10))
                },
                new RoundModel()
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = new DateTimeOffset(new DateTime(2021, 5, 19))
                }
            };

            fakeReportClient
                .GetRoundsAsync(Arg.Any<GetRoundsRequest>())
                .Returns(new PagedResponse<RoundModel>
                {
                    Items = roundModels.ToArray(),
                    TotalCount = roundModels.Count,
                    Message = string.Empty,
                    Valid = false
                });
            var sut = new PlayerReportController(fakeReportClient);
            sut.ControllerContext = new ControllerContext()
            {
                HttpContext = fakeHttpContext
            };

            // act
            var r = await sut.GetRoundsAsync(playerRoundRequest);

            // assert
            Assert.That(r.Items, Is.Ordered.Descending.By("CreatedOn"));
        }
    }
}