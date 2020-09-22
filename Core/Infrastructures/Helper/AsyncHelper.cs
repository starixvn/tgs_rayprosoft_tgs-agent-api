using System;
using System.Threading;
using System.Threading.Tasks;

namespace TGS.Core.Infrastructures.Helper
{
    public static class AsyncHelper
    {
        private static readonly TaskFactory TaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return TaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            TaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        public static void RunAsync(Func<Task> func)
        {
            TaskFactory
                .StartNew(func)
                .Unwrap();
        }

        public static void RunAsync(Action action)
        {
            Task.Run(action);
        }

        public static void RunAsync(Func<object, Task> func, object arg0)
        {
            TaskFactory
                .StartNew(func, arg0)
                .Unwrap();
        }

        public static CancellationTokenSource RunLongRunningAsync(Func<CancellationToken, Task> func)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            TaskFactory
                .StartNew(() =>
                        func(cancellationTokenSource.Token),
                    cancellationTokenSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default)
                .Unwrap();
            return cancellationTokenSource;
        }
    }
}
