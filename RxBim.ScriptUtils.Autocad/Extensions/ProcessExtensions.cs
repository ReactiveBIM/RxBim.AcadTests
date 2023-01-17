namespace RxBim.ScriptUtils.Autocad.Extensions;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// sadasd
/// </summary>
public static class ProcessExtensions
{
    /// <summary>
    /// Waits asynchronously for the process to exit.
    /// </summary>
    /// <param name="process">The process to wait for cancellation.</param>
    /// <param name="cancellationToken">A cancellation token. If invoked, the task will return 
    /// immediately as canceled.</param>
    /// <returns>A Task representing waiting for the process to end.</returns>
    public static Task WaitForExitAsync(this Process process,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (process.HasExited)
            return Task.CompletedTask;

        var tcs = new TaskCompletionSource<object>();
        process.EnableRaisingEvents = true;
        process.Exited += (sender, args) => tcs.TrySetResult(null!);
        if (cancellationToken != default(CancellationToken))
            cancellationToken.Register(() => tcs.SetCanceled());

        return process.HasExited ? Task.CompletedTask : tcs.Task;
    }
}