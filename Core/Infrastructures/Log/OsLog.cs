using System;

namespace TGS.Infrastructures.Log
{
    public class OsLog : IOsLog
    {
        void IOsLog.WriteEventLogError(string errorMessage, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            Console.WriteLine(errorMessage);
        }

        void IOsLog.WriteEventLogInfo(string infoMessage, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            Console.WriteLine(infoMessage);
        }

        void IOsLog.WriteEventLogWarning(string warningMessage, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            Console.WriteLine(warningMessage);
        }
    }
}
