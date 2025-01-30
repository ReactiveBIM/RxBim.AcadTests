namespace RxBim.Tests.Nuke.Components;

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
public interface IRunIntegrationTests : IHasSolution
{
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
    /// Test runner tool
    /// </summary>
    [Parameter("Test runner tool")]
    TestTool TestToolName => TryGetValue(() => TestToolName) ?? TestTool.Acad;

    /// <summary>
    /// Test runner tool
    /// </summary>
    [Parameter("Test runner tool version")]
    string? TestToolVersion => TryGetValue(() => TestToolVersion);

    /// <summary>
    /// Is debug mode
    /// </summary>
    [Parameter("Is debug mode")]
    bool IsDebug => TryGetValue<bool?>(() => IsDebug) ?? false;

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
    /// Update tests runner.
    /// </summary>
    Target UpdateTestsTool =>
        _ => _
            .Requires(() => TestToolName)
            .Executes(() =>
                DotNetTasks.DotNetToolUpdate(settings =>
                {
                    settings = settings
                        .SetPackageName(TestToolName)
                        .ClearSources()
                        .EnableGlobal();
                    if (!string.IsNullOrEmpty(TestToolVersion))
                        settings = settings.SetVersion(TestToolVersion);
                    return settings;
                }));

    /// <summary>
    /// Runs integration tests for Revit.
    /// </summary>
    Target IntegrationTests =>
        definition => definition
            /*.DependsOn(UpdateTestsTool)*/
            .Description("Starts execution of integration tests")
            .Executes(async () =>
            {
                foreach (var project in TestProjects)
                {
                    await ProjectTestRunner.RunTests(project, TestToolName, IsDebug);
                }
            });
}