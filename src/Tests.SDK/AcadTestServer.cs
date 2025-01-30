namespace AcadTests.SDK;

using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Helpers;

/// <summary>
///     Сервер для обработки сообщений.
/// </summary>
public class AcadTestServer
{
    private readonly string _pipeName;
    private CancellationTokenSource _cancelSource;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AcadTestServer" /> class.
    /// </summary>
    /// <param name="pipeName">pipeName</param>
    public AcadTestServer(string pipeName)
    {
        _pipeName = pipeName;
        _cancelSource = new CancellationTokenSource();
    }

    private CancellationToken Cancel => _cancelSource.Token;

    /// <summary>
    ///     comment
    /// </summary>
    /// <param name="testRunningOptions">comment sad</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    public async Task<string> Start(ITestRunningOptions testRunningOptions, CancellationToken cancellationToken)
    {
        _cancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        using var pipeServer =
            new NamedPipeServerStream(_pipeName, PipeDirection.Out);
        Console.WriteLine("\r\n[thread: {0}] -> Waiting for client.", Thread.CurrentThread.ManagedThreadId);
        await pipeServer.WaitForConnectionAsync(Cancel);
        Console.WriteLine("[thread: {0}] -> Client connected.", Thread.CurrentThread.ManagedThreadId);
        try
        {
            var sds = new DataContractSerializer(typeof(TestRunningOptions));
            sds.WriteObject(pipeServer, new TestRunningOptions(testRunningOptions));
            pipeServer.WaitForPipeDrain();
            pipeServer.Disconnect();
        }

        // Catch the IOException that is raised if the pipe is broken
        // or disconnected.
        catch (IOException e)
        {
            Console.WriteLine("ERROR: {0}", e.Message);
        }

        var unused = Task.Run(async () =>
            {
                while (true)
                {
                    var message = await Listener("logPipe");
                    Console.WriteLine("[thread: {0}] -> {1}: {2}",
                        Thread.CurrentThread.ManagedThreadId,
                        DateTime.Now,
                        message);
                }
            },
            Cancel);

        var result = await Listener("resultPipe");
        Stop();
        return result;
    }

    private async Task<string> Listener(string pipeName)
    {
        using var server = new NamedPipeServerStream(
            pipeName,
            PipeDirection.In,
            1,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous);
        await server.WaitForConnectionAsync(Cancel);
        var result = await ReadData(server);
        if (server.IsConnected)
            server.Disconnect();

        return result;
    }

    private async Task<string> ReadData(NamedPipeServerStream server)
    {
        var reader = new StreamReader(server);
        return await reader.ReadToEndAsync();
    }

    private void Stop()
    {
        _cancelSource.Cancel();
    }
}