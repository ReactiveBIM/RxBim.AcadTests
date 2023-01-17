namespace AcadTestFramework.SDK;

using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Helpers;

/// <summary>
///     Comment
/// </summary>
public class AcadTestServer
{
    private readonly CancellationTokenSource _cancelSource;
    private readonly CancellationToken _cancel;
    private readonly string _pipeName;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AcadTestServer" /> class.
    /// </summary>
    /// <param name="pipeName">pipeName</param>
    public AcadTestServer(string pipeName)
    {
        _pipeName = pipeName;
        _cancelSource = new CancellationTokenSource();
        _cancel = _cancelSource.Token;
    }

    /// <summary>
    ///     comment
    /// </summary>
    /// <param name="testRunningOptions">comment sad</param>
    public async Task<string> Start(ITestRunningOptions testRunningOptions)
    {
        using var pipeServer =
            new NamedPipeServerStream(_pipeName, PipeDirection.Out);
        Console.WriteLine("\r\n[thread: {0}] -> Waiting for client.", Thread.CurrentThread.ManagedThreadId);
        await pipeServer.WaitForConnectionAsync(_cancel);
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

        var listenTask = Task.Run(async () =>
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
            _cancel);

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
        await server.WaitForConnectionAsync(_cancel);
        var result = await ReadData(server);
        if (server.IsConnected)
        {
            server.Disconnect();
        }

        return result;
    }

    private async Task<string> ReadData(NamedPipeServerStream server)
    {
        var reader = new StreamReader(server);
        return await reader.ReadToEndAsync();

        /*var buffer = new byte[255];
        var length = await server.ReadAsync(buffer, 0, buffer.Length, _cancel);
        var chunk = new byte[length];
        Array.Copy(buffer, chunk, length);
        var content = Encoding.UTF8.GetString(chunk);

        if (content == "exit")
        {
            Stop();
        }

        return content;*/
    }

    private void Stop()
    {
        _cancelSource.Cancel();
    }
}