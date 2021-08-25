using System;
using TGS.Core.Models;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal interface IBrand
        {
            Guid Id { get; }
            string Code { get; }
            string Name { get; }
            string OperationalEmail { get; }
            string TimeZoneId { get; }
            string ClientId { get; }
            string SecretKey { get; }
            string ApiKey { get; }
            SettlementIntervalType Type { get; }
            int? SettlementDay { get; }

            ICurrency[] Currencies => new ICurrency[]
            {
                new CNYCurrency(),
                new IDRCurrency(),
                new IDR1000Currency(),
                new INRCurrency(),
                new VNDCurrency(),
                new VND1000Currency(),
                new THBCurrency(),
                new EURCurrency(),
                new GBPCurrency(),
                new KRWCurrency(),
                new JPYCurrency()
            };

            ILanguage[] Languages => new ILanguage[]
            {
                new CNLanguage(),
                new USLanguage(),
                new IDLanguage(),
                new VNLanguage(),
                new THLanguage(),
                new TWLanguage(),
                new JPLanguage(),
                new KRLanguage()
            };

            string[] GameProviders => Array.Empty<string>();

            string[] Groups => new[]
            {
                "Normal"
            };
        }

        internal record TgsBrand : IBrand
        {
            Guid IBrand.Id => new("D71AC5ED-B7D9-49F2-A129-8709FA4CC02A");

            string IBrand.Code => "TGS";

            string IBrand.Name => "Turnkey Game System";

            string IBrand.OperationalEmail => "tgs.services@gmail.com";

            string IBrand.TimeZoneId => "Asia/Taipei"; // Linux ts timezone

            string IBrand.ClientId => "TGS";

            string IBrand.SecretKey => "GcHmUWH0WVqWq5JcSPS5qkeoDcTmkBBE";

            string IBrand.ApiKey => "xdZYrxH3X8xXn2KlhWqB3VB4TjZrHfp4rM5xbHfa8cU3oz6CZDL3HqcuZ07pPQFV";

            SettlementIntervalType IBrand.Type => SettlementIntervalType.Monthly;

            int? IBrand.SettlementDay => 13;

            string[] IBrand.GameProviders => new[]
            {
                //Consts.GameProvider.RedTiger,
                Consts.GameProvider.SBOBET
            };
        }

        internal record GpiBrand : IBrand
        {
            Guid IBrand.Id => new("4E86719F-243F-46F6-B010-5C758B7285AF");

            string IBrand.Code => "GPI";

            string IBrand.Name => "GameProvider Integration";

            string IBrand.OperationalEmail => "tgs.services@gmail.com";

            string IBrand.TimeZoneId => "Asia/Taipei"; // Linux ts timezone

            string IBrand.ClientId => "GPI";

            string IBrand.SecretKey => "L5SdeH9DuREUjoG3FN9vtkBY4FtKFitI";

            string IBrand.ApiKey => "I7nKiimrZQOqnJ6ryjUw74c0Hi64KC6Myunhob4cHh8VOUIuZh4p63czefxyyzbZ";

            SettlementIntervalType IBrand.Type => SettlementIntervalType.Daily;

            int? IBrand.SettlementDay => null;
        }
    }
}