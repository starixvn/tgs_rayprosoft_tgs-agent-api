using System;

namespace TGS.Infrastructures.Log
{
    public class OsLog : RequestAnalyzer
    {
        void RequestAnalyzer.WriteEventLogError(string errorMessage, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            Console.WriteLine(errorMessage);
        }

        void RequestAnalyzer.WriteEventLogInfo(string infoMessage, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            Console.WriteLine(infoMessage);
        }

        void RequestAnalyzer.WriteEventLogWarning(string warningMessage, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            Console.WriteLine(warningMessage);
        }
    }
}
