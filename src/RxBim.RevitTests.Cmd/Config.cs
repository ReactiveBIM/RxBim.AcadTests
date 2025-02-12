namespace RxBim.RevitTests.Cmd;

using Di;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using Tests.SDK;

/// <inheritdoc />
[UsedImplicitly]
public class Config : ICommandConfiguration
{
    /// <inheritdoc />
    public void Configure(IServiceCollection services)
    {
        services.AddSingleton(_ => new AcadTestSdk().AcadTestClient);
        services.AddSingleton<ITestListener, TestListener>();
        services.AddTransient<ITestAssemblyBuilder, DefaultTestAssemblyBuilder>();
        services.AddTransient<ITestAssemblyRunner>(sp =>
            new NUnitTestAssemblyRunner(sp.GetService<ITestAssemblyBuilder>()));
        services.AddSingleton<ITestFilter>(_ => TestFilter.Empty);
    }
}