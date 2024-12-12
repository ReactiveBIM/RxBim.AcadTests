namespace AcadTests.TestingUtils.Di;

using Autodesk.AutoCAD.ApplicationServices.Core;
using Microsoft.Extensions.DependencyInjection;
using RxBim.Di;

/// <inheritdoc />
public class TestingDiConfigurator : DiConfigurator<ITestConfiguration>
{
    /// <inheritdoc />
    public TestingDiConfigurator()
    {
    }

    /// <inheritdoc />
    protected override void ConfigureBaseDependencies()
    {
        Services
            .AddSingleton(Application.DocumentManager)
            .AddSingleton(Application.DocumentManager.MdiActiveDocument)
            .AddSingleton(Application.DocumentManager.MdiActiveDocument.Database)
            .AddSingleton(Application.DocumentManager.MdiActiveDocument.Editor);
    }
}