using System.Text;
using Bimlab.Nuke.Components;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using RxBim.Nuke.AutoCAD;
using RxBim.Nuke.Versions;
using RxBim.Tests.Nuke.Components;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

/// <inheritdoc cref="RxBim.Nuke.AutoCAD.AutocadRxBimBuild" />
[UnsetVisualStudioEnvironmentVariables]
[GitHubActions("CI",
    GitHubActionsImage.WindowsLatest,
    FetchDepth = 0,
    OnPushBranches = new[]
    {
        DevelopBranch, FeatureBranches, BugfixBranches
    },
    InvokedTargets = new[]
    {
        nameof(Test), nameof(IPublish.Publish)
    },
    ImportSecrets = new[]
    {
        "NUGET_API_KEY", "ALL_PACKAGES"
    })]
[GitHubActions("Publish",
    GitHubActionsImage.WindowsLatest,
    FetchDepth = 0,
    OnPushBranches = new[]
    {
        MasterBranch, ReleaseBranches, HotfixBranches
    },
    InvokedTargets = new[]
    {
        nameof(Test), nameof(IPublish.Publish)
    },
    ImportSecrets = new[]
    {
        "NUGET_API_KEY", "ALL_PACKAGES"
    })]
public partial class Build : AutocadRxBimBuild, IRunIntegrationTests
{
    const string MasterBranch = "master";
    const string DevelopBranch = "develop";
    const string ReleaseBranches = "release/**";
    const string HotfixBranches = "hotfix/**";
    const string FeatureBranches = "feature/**";
    const string BugfixBranches = "bugfix/**";

    /// <inheritdoc />
    public Build()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }

    new Target Test =>
        targetDefinition => targetDefinition
            .Before(Clean)
            .Before<IRestore>()
            .Executes(() =>
            {
                DotNetTest(settings => settings
                    .SetProjectFile(From<IHasSolution>().Solution.Path)
                    .SetConfiguration(From<IHasConfiguration>().Configuration)
                    .SetFilter("FullyQualifiedName!~IntegrationTests"));
            });

    /// <summary>
    ///     Main
    /// </summary>
    public static int Main() => Execute<Build>(x => /*x.From<IPublish>().PackagesList*/ x.UpdateToolsLocal);

    Target CleanWorkDir =>
        targetDefinition => targetDefinition
            .Before<IPublish>(p => p.Release)
            .DependentFor<IPublish>(p => p.Pack)
            .After(Compile)
            .Executes(() =>
            {
                GitTasks.Git("reset --hard");
            });

    Target UpdateToolsLocal => targetDefinition => targetDefinition
        .Requires(() => From<IRunIntegrationTests>().TestToolName)
        .Executes(() =>
        {
            var testToolName = From<IRunIntegrationTests>().TestToolName;
            //// var project = Solution.GetProject(testToolName); // not working
            var project = Solution.GetAllProjects(testToolName).FirstOrDefault();
            if (project is null)
                throw new Exception($"Project {testToolName} does not exist");
                
            var outputDirectory = Solution.Directory / "nupkg" / project.Name;
            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, true);
            
            DotNetPack(settings => settings
                .SetProject(project)
                .SetConfiguration("Debug")
                .SetOutputDirectory(outputDirectory));

            try
            {
                DotNetToolUninstall(settings => settings
                    .SetPackageName(testToolName)
                    .EnableGlobal());
            }
            catch
            {
                // throws when CLI tool has not been installed before
            }
            
            DotNetToolUpdate(settings => settings
                .SetPackageName(testToolName)
                .EnableGlobal()
                .SetVersion(project.GetProperty("PackageVersion"))
                .SetSources(outputDirectory)
            );
        });

    string IVersionBuild.ProjectNamePrefix => "RxBim.";

    T From<T>()
        where T : INukeBuild =>
        (T)(object)this;
}