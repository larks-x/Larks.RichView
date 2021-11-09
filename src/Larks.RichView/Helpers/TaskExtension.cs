namespace Larks.RichView.Helpers
{
    /// <summary>
    /// Net3.5/Net4.0 Task.Run()
    /// </summary>
    internal static class TaskNet35
    {
        /// <summary>
        /// Queues the specified work to run on the thread pool and returns a System.Threading.Tasks.Task object that represents that work.
        /// </summary>
        /// <param name="action">The work to execute asynchronously.</param>
        /// <returns>A task that represents the work queued to execute in the ThreadPool.</returns>
        public static Task Run(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            var tcs = new TaskCompletionSource<object>();
            new Thread(() =>
            {
                action.Invoke();
                tcs.SetResult(null);
            })
            { IsBackground = true }.Start();
            return tcs.Task;
        }

        /// <summary>
        /// Queues the specified work to run on the thread pool and returns a proxy for the
        /// Task(TResult) returned by function. A cancellation token allows the work to be
        /// cancelled if it has not yet started.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by the proxy task.</typeparam>
        /// <param name="func">The work to execute asynchronously.</param>
        /// <returns></returns>
        public static Task<TResult> Run<TResult>(Func<TResult> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            var tcs = new TaskCompletionSource<TResult>();
            new Thread(() =>
            {
                try
                {
                    tcs.SetResult(func.Invoke());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            })
            { IsBackground = true }.Start();
            return tcs.Task;
        }

        /// <summary>
        /// Creates a task that completes after a specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to wait before completing the returned task</param>
        /// <returns>A task that represents the time delay.</returns>
        public static Task Delay(int milliseconds)
        {
            if (milliseconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(milliseconds));
            var tcs = new TaskCompletionSource<object>();
            var timer = new System.Timers.Timer(milliseconds) { AutoReset = false };
            timer.Elapsed += (s, e) => {
                timer.Dispose();
                tcs.SetResult(null);
            };
            timer.Start();
            return tcs.Task;
        }

    }
}
