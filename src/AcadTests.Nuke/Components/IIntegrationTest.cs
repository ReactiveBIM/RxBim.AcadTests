namespace AcadTests.Nuke.Components;

using Bimlab.Nuke.Components;
using global::Nuke.Common;
using global::Nuke.Common.ProjectModel;
using global::Nuke.Common.Tools.DotNet;
using JetBrains.Annotations;
using Services;

/// <summary>
/// Contains targets for running integration tests.
/// </summary>
[PublicAPI]
public interface IIntegrationTest : IHazSolution
{
    /// <summary>
    /// RevitTestTool
    /// </summary>
    const string RevitTestTool = "RxBim.RevitTests.Console";

    /// <summary>
    /// AcadTestTool
    /// </summary>
    const string AcadTestTool = "RxBim.AcadTests.Console";

    /// <summary>
    /// TestProjectProvider.
    /// </summary>
    TestProjectProvider TestProjectProvider => new(Solution);

    /// <summary>
    /// ProjectTestRunner.
    /// </summary>
    ProjectTestRunner ProjectTestRunner => new(Solution);

    /// <summary>
    /// Test only selected projects
    /// </summary>
    [Parameter("Test only selected projects")]
    bool OnlySelectedProjects => TryGetValue<bool?>(() => OnlySelectedProjects) ?? false;

    /// <summary>
    /// Collection of test projects.
    /// </summary>
    [Parameter("Collection of test projects")]
    Project[] TestProjects
    {
        get
        {
            var projects = TryGetValue(() => TestProjects);
            if (projects is not null)
                return projects;

            projects = OnlySelectedProjects
                ? TestProjectProvider.GetSelectedProjects()
                : TestProjectProvider.Projects;

            return projects;
        }
    }

    /// <summary>
    /// Runs integration tests for Revit.
    /// </summary>
    Target RevitIntegrationTests =>
        definition => definition
            .Description("Starts execution of integration tests in the Revit environment")
            .Executes(async () =>
            {
                DotNetTasks.DotNetToolUpdate(settings => settings
                    .SetPackageName(RevitTestTool)
                    .SetVersion("1.0.1-dev003")
                    .EnableGlobal());
                foreach (var project in TestProjects)
                {
                    await ProjectTestRunner.RunTests(project, RevitTestTool);
                }
            });

    /// <summary>
    /// Runs integration tests for Autocad.
    /// </summary>
    Target AutocadIntegrationTests =>
        definition => definition
            .Description("Starts execution of integration tests in the Autocad environment")
            .Executes(async () =>
            {
                DotNetTasks.DotNetToolUpdate(settings => settings
                    .SetPackageName(AcadTestTool)
                    .SetVersion("1.0.1-dev003")
                    .EnableGlobal());
                foreach (var project in TestProjects)
                {
                    await ProjectTestRunner.RunTests(project, AcadTestTool);
                }
            });
}