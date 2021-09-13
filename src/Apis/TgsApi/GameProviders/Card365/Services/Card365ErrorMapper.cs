using System;
using TGS.Core.Models.Interfaces;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Services
{
    public class Card365ErrorMapper : IErrorMapper
    {
        public int GetApiErrorByException(Exception exception) => throw new NotImplementedException();

        public int GetHttpStatusByException(Exception exception) => throw new NotImplementedException();

        public string GetMessageByException(Exception exception) => throw new NotImplementedException();
    }
}