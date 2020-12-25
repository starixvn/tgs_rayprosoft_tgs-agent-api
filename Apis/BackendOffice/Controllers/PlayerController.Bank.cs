using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Players;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;
using Player = TGS.Apis.BackendOffice.Resources.Player;

namespace TGS.Apis.BackendOffice.Controllers
{
    public partial class PlayerController
    {
        [Route("{playerCode}/bank/{id}")]
        [HttpPut]
        [Description("Update player bank")]
        public async Task<MessageResponse> UpdatePlayerBankAsync(string playerCode, Guid id,
            [FromBody] PlayerBankRequest request)
        {
            var bank = await _bankQueries.GetSingleAsync(p => p.Id == id);
            if (bank == null)
            {
                return CreateInvalidResponse<MessageResponse>(Player.BankNotFound);
            }

            bank.PlayerCode = playerCode;
            bank.RealName = request.RealName;
            bank.BankName = request.BankName;
            bank.BankAccount = request.BankAccount;
            await _bankCommands.UpdateAsync(bank);
            return CreateValidResponse<MessageResponse>();
        }

        [Route("{playerCode}/bank")]
        [HttpPost]
        [Description("Create a player bank")]
        public async Task<MessageResponse> CreatePlayerBankAsync(string playerCode,
            [FromBody] PlayerBankRequest request)
        {
            var bank = new PlayerBank
            {
                Id = GuidExtensions.NewSequentialGuid(),
                PlayerCode = playerCode,
                RealName = request.RealName,
                BankAccount = request.BankAccount,
                BankName = request.BankName
            };
            await _bankCommands.CreateAsync(bank);
            return CreateValidResponse<MessageResponse>();
        }

        [Route("{playerCode}/bank/{id}")]
        [HttpDelete]
        [Description("Delete a player bank")]
        public async Task<MessageResponse> DeletePlayerBankAsync(string playerCode, Guid id)
        {
            var bank = await _bankQueries.GetSingleAsync(p => p.Id == id);
            if (bank == null)
            {
                return CreateInvalidResponse<MessageResponse>(Player.BankNotFound);
            }

            await _bankCommands.DeleteAsync(bank);
            return CreateValidResponse<MessageResponse>();
        }
    }
}