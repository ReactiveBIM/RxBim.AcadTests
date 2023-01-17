namespace AutocadTestFrameworkCmd
{
    using AcadTestFramework.SDK;
    using JetBrains.Annotations;
    using NUnit.Framework.Api;
    using NUnit.Framework.Interfaces;
    using RxBim.Di;
    using Services;

    /// <inheritdoc />
    [UsedImplicitly]
    public class Config : ICommandConfiguration
    {
        /// <inheritdoc />
        public void Configure(IContainer container)
        {
            container.AddInstance(Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager);
            container.AddInstance(new AcadTestSdk().AcadTestClient);
            container.AddSingleton<ITestListener, TestListener>();

            container.AddTransient<ITestAssemblyBuilder, DefaultTestAssemblyBuilder>();
            container.Decorate<ITestAssemblyBuilder, MyTestAssemblyBuilder>();

            container.AddTransient<ITestAssemblyRunner>(() =>
                new NUnitTestAssemblyRunner(container.GetService<ITestAssemblyBuilder>()));
            container.AddSingleton<ITestFilter>(() => NUnit.Framework.Internal.TestFilter.Empty);
        }
    }
}