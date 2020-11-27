using System;
using TGS.Core.Models;

namespace TGS.Core.DataServices.DataInitializers
{
    partial class DataInitializer
    {
        internal interface IBrand
        {
            Guid Id { get; }
            string Code { get; }
            string Name { get; }
            string OperationalEmail { get; }
            string TimeZoneId {  get; }
            string ClientId {  get; }
            string SecretKey {  get; }
            string ApiKey { get; }
            ICurrency[] Currencies {  get; }
            ILanguage[] Languages {  get; }
            string[] GameProviders { get; }
        }

        internal record TgsBrand : IBrand
        {
            Guid IBrand.Id => new Guid("D71AC5ED-B7D9-49F2-A129-8709FA4CC02A");

            string IBrand.Code => "TGS";

            string IBrand.Name => "Turnkey Game System";

            string IBrand.OperationalEmail => "tgs.services@gmail.com";

            string IBrand.TimeZoneId => "Asia/Taipei";  // Linux ts timezone

            string IBrand.ClientId => "TGS";

            string IBrand.SecretKey => "GcHmUWH0WVqWq5JcSPS5qkeoDcTmkBBE";

            string IBrand.ApiKey => "xdZYrxH3X8xXn2KlhWqB3VB4TjZrHfp4rM5xbHfa8cU3oz6CZDL3HqcuZ07pPQFV";

            ICurrency[] IBrand.Currencies => new ICurrency[] 
            { 
                new CNYCurrency(), new IDRCurrency(), new IDR1000Currency(),
                new INRCurrency(), new VNDCurrency(), new VND1000Currency(),
                new THBCurrency()
            };

            ILanguage[] IBrand.Languages => new ILanguage[]
            {
                new CNLanguage(), new USLanguage(), new IDLanguage(),
                new VNLanguage(), new THLanguage()
            };

            string[] IBrand.GameProviders => new[]
            {
                //Consts.GameProvider.RedTiger,
                Consts.GameProvider.SBOBET
            };
        }
    }
}