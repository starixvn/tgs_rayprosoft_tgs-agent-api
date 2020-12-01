using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application
{
    public abstract class Handler<T, TOut> : IRequestHandler<T, TOut> where T : IRequest<TOut>
    {
        public abstract Task<TOut> Handle(T request, CancellationToken cancellationToken);

        protected static TResponse CreateValidResponse<TResponse>() where TResponse : MessageResponse, new()
        {
            return new TResponse
            {
                Valid = true
            };
        }

        protected static TResponse CreateInvalidResponse<TResponse>(string message)
            where TResponse : MessageResponse, new()
        {
            return new TResponse
            {
                Valid = false,
                Message = message
            };
        }
    }
}