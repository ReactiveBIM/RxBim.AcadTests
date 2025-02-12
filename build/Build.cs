using System;
using System.Text;
using Bimlab.Nuke.Components;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
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
    public static int Main() => Execute<Build>(x => x.From<IPublish>().PackagesList);

    Target CleanWorkDir =>
        targetDefinition => targetDefinition
            .Before<IPublish>(p => p.Release)
            .DependentFor<IPublish>(p => p.Pack)
            .After(Compile)
            .Executes(() =>
            {
                GitTasks.Git("reset --hard");
            });

    string IVersionBuild.ProjectNamePrefix => "RxBim.";

    T From<T>()
        where T : INukeBuild =>
        (T)(object)this;
}