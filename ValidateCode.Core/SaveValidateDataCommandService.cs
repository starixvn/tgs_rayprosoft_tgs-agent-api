using System;
using System.Threading.Tasks;
using Valhalla.Services;

namespace TGS.Services.ValidateCode
{
    internal class SaveValidateDataCommandService : ICommandService<SaveValidateDataCommand>
    {
        private readonly IValidateCodeStore _validateCodeStore;

        public SaveValidateDataCommandService(IValidateCodeStore validateCodeStore)
        {
            _validateCodeStore = validateCodeStore ?? throw new ArgumentNullException(nameof(validateCodeStore));
        }

        public ValueTask ExecuteAsync(SaveValidateDataCommand command)
        {
            return _validateCodeStore.SaveDataAsync(command.ValidateCode, command.Content);
        }
    }
}
