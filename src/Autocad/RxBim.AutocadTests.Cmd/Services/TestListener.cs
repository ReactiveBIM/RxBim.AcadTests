namespace RxBim.AutocadTests.Cmd.Services;

using NUnit.Framework.Interfaces;
using RxBim.Tests.SDK.Abstractions;

/// <inheritdoc />
internal class TestListener(IAcadTestClient acadTestClient) : ITestListener
{
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
        acadTestClient.SendMessage(message);
    }
}