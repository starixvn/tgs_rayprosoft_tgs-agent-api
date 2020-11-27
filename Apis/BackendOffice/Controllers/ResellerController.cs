using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Reseller;
using TGS.Apis.BackendOffice.DataService.Services;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Messages;
using Reseller = TGS.Apis.BackendOffice.Resources.Reseller;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResellerController : BaseContextController
    {
        private readonly IMapper _mapper;
        private readonly IResellerService _service;

        public ResellerController(IResellerService service,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(
            httpContextAccessor)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        [Description("Get resellers: 公司列表")]
        public Task<PagedResponse<ResellerModel>> GetResellersAsync([FromQuery] GetResellersRequest request)
        {
            return _service.GetResellersAsync().Where(request.Keyword,
                    x => x.Code.Contains(request.Keyword) || x.Name.Contains(request.Keyword) ||
                         x.Description.Contains(request.Keyword) || x.OperationalEmail.Contains(request.Keyword))
                .Select(x => new ResellerModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    Description = x.Description,
                    OperationalEmail = x.OperationalEmail,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn
                })
                .AsNoTracking().CreatePagedResponseAsync(request);
        }

        [HttpGet("{id}")]
        [Description("Get a Reseller")]
        public async Task<GetResellerResponse> GetResellerAsync(Guid id)
        {
            var reseller = await _service.GetResellerAsync(x => x.Id == id,
                x => x.Include(i => i.Currencies)
                    .Include(i => i.Languages), true);

            if (reseller == null)
            {
                return CreateInvalidResponse<GetResellerResponse>(Reseller.ResellerNotFound);
            }

            return new GetResellerResponse
            {
                Valid = true,
                Reseller = new ResellerModel
                {
                    Id = reseller.Id,
                    Code = reseller.Code,
                    Name = reseller.Name,
                    ResellerCurrencies = reseller.Currencies
                        .Select(x => _mapper.Map<Currency, ResellerCurrencyModel>(x.Currency)).ToArray(),
                    ResellerLanguages = reseller.Languages
                        .Select(x => _mapper.Map<Language, ResellerLanguageModel>(x.Language)).ToArray(),
                    Description = reseller.Description,
                    OperationalEmail = reseller.OperationalEmail,
                    CreatedOn = reseller.CreatedOn,
                    UpdatedOn = reseller.UpdatedOn
                }
            };
        }

        [HttpPost]
        public async Task<MessageResponse> CreateResellerAsync(CreateResellerRequest request)
        {
            await _service.CreateResellerAsync(request, Username);

            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{id}")]
        public async Task<MessageResponse> UpdateResellerAsync(Guid id, UpdateResellerRequest request)
        {
            await _service.UpdatePaymentAsync(request, Username);

            return CreateValidResponse<MessageResponse>();
        }
    }
}