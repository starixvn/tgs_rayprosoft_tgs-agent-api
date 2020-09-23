using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.DataInitializers
{
    partial class DataInitializer
    {
        internal interface ICurrency
        {
            string Code { get; }
            string ExternalCode { get; }
            decimal? PlayerCapAmount { get; }
        }

        internal record BaseCurrency : ICurrency
        {
            private readonly string _code;
            private readonly string _externalCode;
            private readonly decimal? _playerCapAmount;

            public BaseCurrency(string code, string externalCode, decimal? playerCapAmount)
            {
                _code = code;
                _externalCode = externalCode ?? code;
                _playerCapAmount = playerCapAmount;
            }

            string ICurrency.Code => _code;
            string ICurrency.ExternalCode => _externalCode;
            decimal? ICurrency.PlayerCapAmount => _playerCapAmount;
        }
    }
}