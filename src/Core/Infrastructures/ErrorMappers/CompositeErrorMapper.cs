using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Interfaces;

namespace TGS.Core.Infrastructures.ErrorMappers
{
    internal class CompositeErrorMapper : IErrorMapper
    {
        private IErrorMapper[] _mappers;

        public CompositeErrorMapper(IServiceProvider serviceProvider, ErrorMapperTypeCollection types)
        {
            _mappers = types
                .Select(type =>
                    (IErrorMapper)ActivatorUtilities.CreateInstance(serviceProvider, type))
                .ToArray();
        }

        public int GetApiErrorByException(Exception exception)
        {
            foreach (var mapper in _mappers)
                if (mapper.CanProcess(exception))
                    return mapper.GetApiErrorByException(exception);

            return (int)TgsApiError.None;
        }

        public int GetHttpStatusByException(Exception exception)
        {
            foreach (var mapper in _mappers)
                if (mapper.CanProcess(exception))
                    return mapper.GetHttpStatusByException(exception);

            return StatusCodes.Status500InternalServerError;
        }

        public string GetMessageByException(Exception exception)
        {
            foreach (var mapper in _mappers)
                if (mapper.CanProcess(exception))
                    return mapper.GetMessageByException(exception);

            return exception.Message;
        }

        public string GetMessageKeyByException(Exception exception)
        {
            foreach (var mapper in _mappers)
                if (mapper.CanProcess(exception))
                    return mapper.GetMessageKeyByException(exception);

            return null;
        }
    }
}
