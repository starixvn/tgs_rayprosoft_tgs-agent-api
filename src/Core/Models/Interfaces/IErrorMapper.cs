using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Interfaces
{
    public interface IErrorMapper
    {
        bool CanProcess(Exception exception) => true;

        int GetApiErrorByException(Exception exception);

        int GetHttpStatusByException(Exception exception);

        string GetMessageByException(Exception exception);

        string GetMessageKeyByException(Exception exception) => null;
    }
}
