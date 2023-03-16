namespace AcadTests.Cmd.Services;

using AcadTests.SDK;
using NUnit.Framework.Interfaces;

/// <inheritdoc />
public class TestListener : ITestListener
{
    private readonly AcadTestClient _acadTestClient;

    /// <summary>
    ///     ctr
    /// </summary>
    /// <param name="acadTestClient">
    ///     <see cref="AcadTestClient" />
    /// </param>
    public TestListener(AcadTestClient acadTestClient)
    {
        _acadTestClient = acadTestClient;
    }

    /// <inheritdoc />
    public void TestStarted(ITest test)
    {
        SendMessage($"Test started {test.FullName}");
    }

    /// <inheritdoc />
    public void TestFinished(ITestResult result)
    {
        SendMessage($"Test finished {result.FullName} {result.Output} {result.Message}");
    }

    /// <inheritdoc />
    public void TestOutput(TestOutput output)
    {
        SendMessage($"Test output {output.TestName} is \"{output.Text}\"");
    }

    /// <inheritdoc />
    public void SendMessage(TestMessage message)
    {
        SendMessage($"Destination {message.Destination}/ Message {message.Message}");
    }

    private void SendMessage(string message)
    {
        _acadTestClient.SendMessage(message);
    }
}