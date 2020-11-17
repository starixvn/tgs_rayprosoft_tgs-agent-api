using System;
using TGS.Core.Models;

namespace TGS.Core.DataServices.DataInitializers
{
    partial class DataInitializer
    {
        internal interface ILicensee
        {
            Guid Id { get; }
            string Code { get; }
            string Name { get; }
            string TimeZoneId {  get; }
            string ClientId {  get; }
            string SecretKey {  get; }
            string ApiKey { get; }
            ICurrency[] Currencies {  get; }
            ILanguage[] Languages {  get; }
            string[] GameProviders { get; }
        }

        internal record TgsLicensee : ILicensee
        {
            Guid ILicensee.Id => new Guid("66C5FEF5-217B-4929-9A08-72C2479A8F4E");

            string ILicensee.Code => "TGS";

            string ILicensee.Name => "Turnkey Game System";

            string ILicensee.TimeZoneId => "Asia/Taipei";  // Linux ts timezone

            string ILicensee.ClientId => "TGS";

            string ILicensee.SecretKey => "GcHmUWH0WVqWq5JcSPS5qkeoDcTmkBBE";

            string ILicensee.ApiKey => "xdZYrxH3X8xXn2KlhWqB3VB4TjZrHfp4rM5xbHfa8cU3oz6CZDL3HqcuZ07pPQFV";

            ICurrency[] ILicensee.Currencies => new ICurrency[] 
            { 
                new CNYCurrency(), new IDRCurrency(), new IDR1000Currency(),
                new INRCurrency(), new VNDCurrency(), new VND1000Currency(),
                new THBCurrency()
            };

            ILanguage[] ILicensee.Languages => new ILanguage[]
            {
                new CNLanguage(), new USLanguage(), new IDLanguage(),
                new VNLanguage(), new THLanguage()
            };

            string[] ILicensee.GameProviders => new[]
            {
                Consts.GameProvider.RedTiger,
                Consts.GameProvider.SBOBET
            };
        }
    }
}