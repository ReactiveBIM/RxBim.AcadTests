namespace RxBim.RevitTests.Cmd;

using Di;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using RxBim.AutocadTests.Cmd.Abstractions;
using RxBim.AutocadTests.Cmd.Services;
using Tests.SDK;
using Tests.SDK.Abstractions;
using TestListener = RxBim.AutocadTests.Cmd.Services.TestListener;

/// <inheritdoc />
[UsedImplicitly]
public class Config : ICommandConfiguration
{
    /// <inheritdoc />
    public void Configure(IServiceCollection services)
    {
        services.AddSingleton<IAcadTestClient>(_ => new AcadTestSdk().AcadTestClient)
            .AddSingleton<ITestListener, TestListener>()
            .AddSingleton<ITestAssemblyBuilder, DefaultTestAssemblyBuilder>()
            .AddSingleton<ITestAssemblyRunner>(sp => new NUnitTestAssemblyRunner(sp.GetService<ITestAssemblyBuilder>()))
            .AddSingleton<ITestFilter>(_ => TestFilter.Empty)
            .AddSingleton<ITestService, TestService>();
    }
}