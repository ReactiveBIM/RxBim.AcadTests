namespace AcadTestFramework.SDK.Helpers;

using System;
using System.Threading.Tasks;

/// <summary>
/// asdasd
/// </summary>
/// <typeparam name="T">asdasd</typeparam>
public class AsyncLazy<T> : Lazy<Task<T>>
{
    /// <inheritdoc />
    public AsyncLazy(Func<T> valueFactory)
        : base(() => Task.Factory.StartNew(valueFactory))
    {
    }

    /// <inheritdoc />
    public AsyncLazy(Func<Task<T>> taskFactory)
        : base(() => Task.Factory.StartNew(taskFactory).Unwrap())
    {
    }
}