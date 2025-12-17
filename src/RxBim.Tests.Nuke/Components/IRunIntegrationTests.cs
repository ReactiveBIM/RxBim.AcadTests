namespace RxBim.Tests.Nuke.Components;

using Bimlab.Nuke.Components;
using global::Nuke.Common;
using global::Nuke.Common.ProjectModel;
using global::Nuke.Common.Tools.DotNet;
using JetBrains.Annotations;
using RxBim.Tests.Nuke.Helpers;
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
    /// Test project names.
    /// </summary>
    [Parameter("Test project names")]
    string? ProjectNames => TryGetValue(() => ProjectNames);

    /// <summary>
    /// App version.
    /// </summary>
    [Parameter]
    string Version => TryGetValue<string?>(() => Version) ?? "2019";

    /// <summary>
    /// Test runner tool.
    /// </summary>
    [Parameter("Test runner tool")]
    TestTool TestToolName => TryGetValue(() => TestToolName) ?? TestTool.Acad;

    /// <summary>
    /// Test runner tool version.
    /// </summary>
    [Parameter("Test runner tool version")]
    string? TestToolVersion => TryGetValue(() => TestToolVersion);

    /// <summary>
    /// Is debug mode.
    /// </summary>
    [Parameter("Is debug mode")]
    bool IsDebug => TryGetValue<bool?>(() => IsDebug) ?? false;

    /// <summary>
    /// Skip update for test tool CLI.
    /// </summary>
    [Parameter("Skip update for test tool CLI.")]
    bool SkipUpdateTool => TryGetValue<bool?>(() => SkipUpdateTool) ?? false;

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

            if (ProjectNames is not null)
            {
                var names = ProjectNames.Split(',', StringSplitOptions.RemoveEmptyEntries);
                projects = TestProjectProvider.Projects
                    .Where(p => names.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToArray();
                return projects;
            }

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
        definition => definition
            .Requires(() => TestToolName)
            .OnlyWhenStatic(() => !SkipUpdateTool)
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
            .DependsOn(UpdateTestsTool)
            .Description("Starts execution of integration tests")
            .Executes(async () =>
            {
                var versions = VersionHelper.GetVersions(Version);
                foreach (var project in TestProjects)
                {
                    foreach (var version in versions)
                    {
                        await ProjectTestRunner.RunTests(
                            project,
                            TestToolName,
                            IsDebug,
                            version,
                            settings => ConfigureBuildSettings(settings, version));
                    }
                }
            });

    /// <summary>
    /// Configure build settings.
    /// </summary>
    /// <param name="settings">Build settings.</param>
    /// <param name="version">Version.</param>
    protected virtual DotNetBuildSettings ConfigureBuildSettings(DotNetBuildSettings settings, int version)
    {
        return settings.AddProperty("ApplicationVersion", version);
    }
}