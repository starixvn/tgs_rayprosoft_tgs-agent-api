using System;
using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TGS.Core.Models.Interfaces;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    public class JiliErrorMapper : IErrorMapper
    {
        private readonly ILogger<JiliErrorMapper> _logger;

        public JiliErrorMapper(
            ILogger<JiliErrorMapper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int GetApiErrorByException(Exception exception)
            => throw new NotImplementedException();

        public int GetHttpStatusByException(Exception exception)
            => StatusCodes.Status200OK;

        public string GetMessageByException(Exception exception)
            => exception.Message;
    }
}
