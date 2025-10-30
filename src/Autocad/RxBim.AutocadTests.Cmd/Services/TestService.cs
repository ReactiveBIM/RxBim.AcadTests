namespace RxBim.AutocadTests.Cmd.Services;

using System.Diagnostics;
using Abstractions;
using JetBrains.Annotations;
using NUnit;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using RxBim.Tests.SDK.Abstractions;

/// <inheritdoc />
[UsedImplicitly]
internal sealed class TestService : ITestService
{
    /// <inheritdoc />
    public void ExecuteTest(
        IAcadTestClient testClient,
        ITestAssemblyRunner testAssemblyRunner,
        ITestFilter testFilter,
        ITestListener testListener)
    {
        // В базовых библиотеках RxBim версий до 2025 (до перехода на .NET) есть встроенный резолвер.
        // Поэтому, его необходимо добавлять только для версий >= 2025.
#if NETCOREAPP
        using var resolver = new Shared.AssemblyResolver(System.Reflection.Assembly.GetExecutingAssembly());
#endif

        var options = testClient.GetTestRunningOptions().GetAwaiter().GetResult();
        if (options.Debug)
            Debugger.Launch();
        var assembly = options.AssemblyPath;
        if (!File.Exists(assembly))
            throw new FileNotFoundException(assembly);

        // todo: добавить обработку ошибки загрузки сборки
        var loadResult = testAssemblyRunner.Load(
            assembly,
            new Dictionary<string, object>
            {
                { FrameworkPackageSettings.RunOnMainThread, true }
            });
        var testResult = testAssemblyRunner.Run(testListener, testFilter);
        //// todo: добавить обработку ошибки в тесте (проверять статус)
        testClient.SendResult(testResult.ToXml(true).OuterXml);
    }
}