namespace AcadTests.Cmd;

using AcadTests.SDK;
using Autodesk.AutoCAD.ApplicationServices.Core;
using JetBrains.Annotations;
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
    public void Configure(IContainer container)
    {
        container.AddInstance(Application.DocumentManager);
        container.AddInstance(new AcadTestSdk().AcadTestClient);
        container.AddSingleton<ITestListener, TestListener>();

        container.AddTransient<ITestAssemblyBuilder, DefaultTestAssemblyBuilder>();
        container.Decorate<ITestAssemblyBuilder, MyTestAssemblyBuilder>();

        container.AddTransient<ITestAssemblyRunner>(() =>
            new NUnitTestAssemblyRunner(container.GetService<ITestAssemblyBuilder>()));
        container.AddSingleton<ITestFilter>(() => TestFilter.Empty);
    }
}