using System;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    internal interface ISmsServiceClient
    {
        ValueTask SendShortMessageAsync(string phone, string message, string currency, Guid brandId);
    }
}