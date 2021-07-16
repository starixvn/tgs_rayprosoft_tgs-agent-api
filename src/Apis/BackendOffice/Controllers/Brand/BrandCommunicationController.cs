using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.BrandCommunication;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Commands.BrandCommunication;
using TGS.Services.BackOffice.Application.Queries.BrandCommunication;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.Brand
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandCommunicationController : BaseContextController
    {
        [HttpGet("{brandId}")]
        [Description("Get a brand's communications")]
        public IAsyncEnumerable<BrandCommunicationModel> GetBrandCommunicationsAsync(
            Guid brandId,
            [FromServices] IQueryService<BrandCommunicationQuery, BrandCommunicationInfo> queryService)
        {
            var query = new BrandCommunicationQuery(brandId);
            var info = queryService.QueryAsync(query);

            return info.Select(x =>
                new BrandCommunicationModel()
                {
                    Id = x.Id.ToString(),
                    Type = x.CommunicationType,
                    Phone = x.Phone,
                    AppId = x.AppId,
                    QRCodeUrl = x.QRCodeUrl
                });
        }

        [HttpPost]
        [Description("Create a brand communication")]
        public async Task<MessageResponse> CreateBrandCommunicationAsync(
            [FromServices] ICommandService<CreateBrandCommunicationCommand> createService,
            [FromBody] CreateBrandCommunicationRequest request)
        {
            var command = new CreateBrandCommunicationCommand()
            {
                BrandId = request.BrandId,
                CommunicationType = request.Type.ToString(),
                Phone = request.Phone,
                AppId = request.AppId,
                QRCodeUrl = request.QRCodeUrl
            };
            await createService.ExecuteAsync(command).ConfigureAwait(false);

            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{id}")]
        [Description("Update a brand communication")]
        public async Task<MessageResponse> UpdateBrandCommAsync(
            Guid id,
            [FromBody] UpdateBrandCommunicationRequest request,
            [FromServices] ICommandService<UpdateBrandCommunicationCommand> updateService)
        {
            var command = new UpdateBrandCommunicationCommand()
            {
                Id = id,
                CommunicationType = request.Type.ToString(),
                Phone = request.Phone,
                AppdId = request.AppId,
                QRCodeUrl = request.QRCodeUrl
            };
            await updateService.ExecuteAsync(command).ConfigureAwait(false);

            return CreateValidResponse<MessageResponse>();
        }

        [HttpDelete("{id}")]
        [Description("Delete a brand communication")]
        public async Task<MessageResponse> DeleteAsync(
            Guid id,
            [FromServices] ICommandService<DeleteBrandCommunicationCommand> deleteService)
        {
            var command = new DeleteBrandCommunicationCommand(id);
            await deleteService.ExecuteAsync(command).ConfigureAwait(false);

            return CreateValidResponse<MessageResponse>();
        }
    }
}