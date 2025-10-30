namespace RxBim.RevitTests.TestingUtils
{
    using AutocadTests.TestingUtils.Di;
    using Cmd;
    using Di;
    using JetBrains.Annotations;
    using Microsoft.Extensions.DependencyInjection;

    /// <inheritdoc />
    [PublicAPI]
    public class TestingDiConfigurator : DiConfigurator<ITestConfiguration>
    {
        /// <inheritdoc />
        public TestingDiConfigurator()
        {
        }

        /// <summary>
        /// Сервисы для тестов.
        /// </summary>
        public IServiceCollection TestServices => Services;

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