using System.Runtime.CompilerServices;

namespace TGS.Infrastructures.Log
{
    public interface RequestAnalyzer
    {
        void WriteEventLogError(string errorMessage, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        void WriteEventLogWarning(string warningMessage, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        void WriteEventLogInfo(string infoMessage, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);
    }
}
