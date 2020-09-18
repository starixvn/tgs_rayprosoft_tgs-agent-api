using System;

namespace TGS.Core.Models.Interfaces
{
    public interface IErrorMapper
    {
        int GetApiErrorByException(Exception exception);
        int GetHttpStatusByException(Exception exception);
        string GetMessageByException(Exception exception);
    }
}
