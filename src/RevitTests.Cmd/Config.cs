namespace RevitTests.Cmd;

using AcadTests.SDK;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using RxBim.Di;

/// <inheritdoc />
public class Config : ICommandConfiguration
{
    /// <inheritdoc />
    public void Configure(IContainer container)
    {
        container.AddInstance(new AcadTestSdk().AcadTestClient);
        container.AddSingleton<ITestListener, TestListener>();
        container.AddTransient<ITestAssemblyBuilder, DefaultTestAssemblyBuilder>();
        container.AddTransient<ITestAssemblyRunner>(() =>
            new NUnitTestAssemblyRunner(container.GetService<ITestAssemblyBuilder>()));
        container.AddSingleton<ITestFilter>(() => TestFilter.Empty);
    }
}