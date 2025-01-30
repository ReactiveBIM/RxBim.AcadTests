namespace AcadTests.SDK;

using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Abstractions;
using Helpers;

/// <summary>
///     Клиент
/// </summary>
public class AcadTestClient
{
    private readonly string _pipeName;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AcadTestClient" /> class.
    /// </summary>
    /// <param name="pipeName">Pipe name</param>
    public AcadTestClient(string pipeName)
    {
        _pipeName = pipeName;
    }

    /// <summary>
    ///     Посылает сообщение
    /// </summary>
    /// <param name="message">сообщение</param>
    public void SendMessage(string message)
    {
        using var pipe = new NamedPipeClientStream(".",
            "logPipe",
            PipeDirection.Out,
            PipeOptions.None,
            TokenImpersonationLevel.Impersonation);
        var msg = Encoding.UTF8.GetBytes(message);
        pipe.Connect(5000);
        pipe.Write(msg, 0, msg.Length);
    }

    /// <summary>
    ///     comment
    /// </summary>
    /// <returns></returns>
    public Task<ITestRunningOptions> GetTestRunningOptions()
    {
        using var pipeClient =
            new NamedPipeClientStream(
                ".",
                _pipeName,
                PipeDirection.In,
                PipeOptions.None,
                TokenImpersonationLevel.Impersonation);

        pipeClient.Connect(5000);
        var serializer = new DataContractSerializer(typeof(TestRunningOptions));
        var options = (ITestRunningOptions)serializer.ReadObject(pipeClient);
        return Task.FromResult(options);
    }

    /// <summary>
    ///     comment
    /// </summary>
    /// <param name="result">сообщение</param>
    public void SendResult(string result)
    {
        using var pipe = new NamedPipeClientStream(".",
            "resultPipe",
            PipeDirection.Out,
            PipeOptions.None,
            TokenImpersonationLevel.Impersonation);
        var msg = Encoding.UTF8.GetBytes(result);
        pipe.Connect(5000);
        pipe.Write(msg, 0, msg.Length);
    }
}