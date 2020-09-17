using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Players;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.BackendOffice.Controllers
{
    public partial class PlayerController
    {
        [Route("{playerCode}/bank/{id}")]
        [HttpPut, Description("Update player bank")]
        public async Task<MessageResponse> UpdatePlayerBankAsync(string playerCode, Guid id, [FromBody] PlayerBankRequest request)
        {
            var bank = await _bankQueries.GetSingleAsync(p => p.Id == id);
            if (bank == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Player.BankNotFound);
            }

            bank.PlayerCode = playerCode;
            bank.Username = request.Username;
            bank.BankName = request.BankName;
            bank.BankAccount = request.BankAccount;
            await _bankCommands.UpdateAsync(bank);
            return CreateValidResponse<MessageResponse>();
        }

        [Route("{playerCode}/bank")]
        [HttpPost, Description("Create a player bank")]
        public async Task<MessageResponse> CreatePlayerBankAsync(string playerCode, [FromBody] PlayerBankRequest request)
        {
            var bank = new PlayerBank
            {
                Id = GuidExtensions.NewSequentialGuid(),
                PlayerCode = playerCode,
                Username = request.Username,
                BankAccount = request.BankAccount,
                BankName = request.BankName
            };
            await _bankCommands.CreateAsync(bank);
            return CreateValidResponse<MessageResponse>();
        }

        [Route("{playerCode}/bank/{id}")]
        [HttpDelete, Description("Delete a player bank")]
        public async Task<MessageResponse> DeletePlayerBankAsync(string playerCode, Guid id)
        {
            var bank = await _bankQueries.GetSingleAsync(p => p.Id == id);
            if (bank == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Player.BankNotFound);
            }

            await _bankCommands.DeleteAsync(bank);
            return CreateValidResponse<MessageResponse>();
        }
    }
}
