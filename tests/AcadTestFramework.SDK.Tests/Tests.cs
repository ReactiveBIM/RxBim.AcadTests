namespace AcadTestFramework.SDK.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;
using AcadTests.SDK;
using AcadTests.SDK.Helpers;
using FluentAssertions;
using NUnit.Framework;

/// <summary>
///     Тесты
/// </summary>
[TestFixture]
public class Tests
{
    private static readonly string PipeName = Guid.NewGuid().ToString();
    private static readonly string ResultMessage = "It' results";

    private async void AcadThread(object? data)
    {
        var client = new AcadTestClient(PipeName);
        var options = await client.GetTestRunningOptions();
        client.SendMessage("Hi!");
        client.SendMessage("Some message");
        client.SendResult(ResultMessage);
    }

    /// <summary>
    ///     Тестовый запуск
    /// </summary>
    [Test]
    public async Task UseAtfSdkTest()
    {
        var testRunningOptions = new TestRunningOptions();
        var acadServer = new AcadTestServer(PipeName);
        var resultsTask = acadServer.Start(testRunningOptions, CancellationToken.None);
        var acadClient = new Thread(AcadThread);
        acadClient.Start();
        var results = await resultsTask;
        results.Should().Be(ResultMessage);
    }
}