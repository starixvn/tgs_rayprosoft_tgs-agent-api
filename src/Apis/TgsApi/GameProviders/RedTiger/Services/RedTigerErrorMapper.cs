using System;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Interfaces;

namespace TGS.Apis.TgsApi.GameProviders.RedTiger.Services
{
    public sealed class RedTigerErrorMapper : IErrorMapper
    {
        int IErrorMapper.GetHttpStatusByException(Exception exception)
        {
            return StatusCodes.Status500InternalServerError;
        }

        string IErrorMapper.GetMessageByException(Exception exception)
        {
            return "test";
        }

        int IErrorMapper.GetApiErrorByException(Exception exception)
        {
            return 101;
        }
    }
}
