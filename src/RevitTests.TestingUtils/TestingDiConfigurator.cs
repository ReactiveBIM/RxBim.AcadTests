namespace RevitTests.TestingUtils
{
    using AcadTests.TestingUtils.Di;
    using Cmd;
    using JetBrains.Annotations;
    using Microsoft.Extensions.DependencyInjection;
    using RxBim.Di;

    /// <inheritdoc />
    [PublicAPI]
    public class TestingDiConfigurator : DiConfigurator<ITestConfiguration>
    {
        /// <inheritdoc />
        public TestingDiConfigurator()
        {
        }

        /// <inheritdoc />
        protected override void ConfigureBaseDependencies()
        {
            var uiApp = RevitContext.UiApplication!;
            Services
                .AddSingleton(uiApp)
                .AddSingleton(uiApp.Application)
                .AddTransient(_ => uiApp.ActiveUIDocument)
                .AddTransient(_ => uiApp.ActiveUIDocument.Document);
        }
    }
}