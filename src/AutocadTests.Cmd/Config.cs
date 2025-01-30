namespace AutocadTests.Cmd;

using AcadTests.SDK;
using Autodesk.AutoCAD.ApplicationServices.Core;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using RxBim.Di;
using Services;
using TestListener = Services.TestListener;

/// <inheritdoc />
[UsedImplicitly]
public class Config : ICommandConfiguration
{
    /// <inheritdoc />
    public void Configure(IServiceCollection services)
    {
        services.AddSingleton(_ => Application.DocumentManager);
        services.AddSingleton(_ => new AcadTestSdk().AcadTestClient);
        services.AddSingleton<ITestListener, TestListener>();

        services.AddTransient<ITestAssemblyBuilder, DefaultTestAssemblyBuilder>();
        services.Decorate<ITestAssemblyBuilder, MyTestAssemblyBuilder>();

        services.AddTransient<ITestAssemblyRunner>(sp =>
            new NUnitTestAssemblyRunner(sp.GetService<ITestAssemblyBuilder>()));
        services.AddSingleton<ITestFilter>(_ => TestFilter.Empty);
    }
}