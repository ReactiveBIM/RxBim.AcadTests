namespace RxBim.AutocadTests.Cmd.Abstractions;

using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using RxBim.Tests.SDK.Abstractions;

/// <summary>
/// Test service.
/// </summary>
public interface ITestService
{
    /// <summary>
    /// Executes test.
    /// </summary>
    /// <param name="testClient"><see cref="IAcadTestClient"/>.</param>
    /// <param name="testAssemblyRunner"><see cref="ITestAssemblyRunner"/></param>
    /// <param name="testFilter"><see cref="ITestFilter"/></param>
    /// <param name="testListener"><see cref="ITestListener"/></param>
    void ExecuteTest(
        IAcadTestClient testClient,
        ITestAssemblyRunner testAssemblyRunner,
        ITestFilter testFilter,
        ITestListener testListener);
}