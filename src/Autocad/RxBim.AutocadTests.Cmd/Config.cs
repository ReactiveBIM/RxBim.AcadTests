namespace RxBim.AutocadTests.Cmd;

using Abstractions;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Di;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using RxBim.Tests.SDK.Abstractions;
using Services;
using Tests.SDK;
using TestListener = Services.TestListener;

/// <inheritdoc />
[UsedImplicitly]
public class Config : ICommandConfiguration
{
    /// <inheritdoc />
    public void Configure(IServiceCollection services)
    {
        services.AddSingleton(_ => Application.DocumentManager)
            .AddSingleton<IAcadTestClient>(_ => new AcadTestSdk().AcadTestClient)
            .AddSingleton<ITestListener, TestListener>()
            .AddSingleton<ITestAssemblyBuilder, DefaultTestAssemblyBuilder>()
            .Decorate<ITestAssemblyBuilder, MyTestAssemblyBuilder>()
            .AddSingleton<ITestAssemblyRunner>(sp => new NUnitTestAssemblyRunner(sp.GetService<ITestAssemblyBuilder>()))
            .AddSingleton<ITestFilter>(_ => TestFilter.Empty)
            .AddSingleton<ITestService, TestService>();
    }
}