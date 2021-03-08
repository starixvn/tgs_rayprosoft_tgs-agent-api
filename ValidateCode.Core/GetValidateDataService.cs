using System;
using System.Threading.Tasks;
using Valhalla.Services;

namespace TGS.Services.ValidateCode
{
    internal class GetValidateDataService : IGetService<GetValidateDataQuery, string>
    {
        private readonly IValidateCodeStore _validateCodeStore;

        public GetValidateDataService(IValidateCodeStore validateCodeStore)
        {
            _validateCodeStore = validateCodeStore ?? throw new ArgumentNullException(nameof(validateCodeStore));
        }

        public ValueTask<string> GetAsync(GetValidateDataQuery query)
        {
            return _validateCodeStore.GetDataAsync(query.ValidateCode);
        }
    }
}
