using System;
using System.Threading;
using System.Threading.Tasks;
using AcadTestFramework.SDK.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace AcadTestFramework.SDK.Tests;

/// <summary>
/// Тесты
/// </summary>
[TestFixture]
public class Tests
{
    private static readonly string PipeName = Guid.NewGuid().ToString();
    private static readonly string ResultMessage = "It' results";

    private static void AcadThread(object? data)
    {
        var client = new AcadTestClient(PipeName);
        var options = client.GetTestRunningOptions();
        client.SendMessage("Hi!");
        client.SendMessage("Some message");
        client.SendResult(ResultMessage);
    }

    /// <summary>
    /// Тестовый запуск
    /// </summary>
    [Test]
    public async Task UseAtfSdkTest()
    {
        var testRunningOptions = new TestRunningOptions();
        var acadServer = new AcadTestServer(PipeName);
        var resultsTask = acadServer.Start(testRunningOptions, CancellationToken.None);
        var acad = new Thread(AcadThread);
        acad.Start();
        var results = await resultsTask;
        results.Should().Be(ResultMessage);
    }
}