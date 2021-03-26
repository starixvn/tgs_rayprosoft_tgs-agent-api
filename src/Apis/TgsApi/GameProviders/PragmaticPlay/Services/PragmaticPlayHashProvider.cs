using System;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Security;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services
{
    public interface IPragmaticPlayHashProvider
    {
        string ComputeHash(IFormCollection collections, string[] additionalData);
    }

    public sealed class PragmaticPlayHashProvider : IPragmaticPlayHashProvider
    {
        private readonly IHashProvider _hashProvider;

        private readonly string[] BypassHashKeys =
        {
            PragmaticPlayConsts.Parameters.Hash, PragmaticPlayConsts.Parameters.BypassHashVerify
        };

        public PragmaticPlayHashProvider(IHashProvider hashProvider)
        {
            _hashProvider = hashProvider;
        }

        string IPragmaticPlayHashProvider.ComputeHash(IFormCollection collections, string[] additionalData) =>
            throw new NotImplementedException();
    }
}