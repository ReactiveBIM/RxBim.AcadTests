namespace RevitTests.TestingUtils
{
    using AcadTests.TestingUtils.Di;
    using Cmd;
    using JetBrains.Annotations;
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
            Container
                .AddInstance(uiApp)
                .AddInstance(uiApp.Application)
                .AddTransient(() => uiApp.ActiveUIDocument)
                .AddTransient(() => uiApp.ActiveUIDocument.Document);
        }
    }
}