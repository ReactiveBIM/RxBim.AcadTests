using System;
using System.Text;
using AcadTests.Nuke.Components;
using Bimlab.Nuke.Components;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using RxBim.Nuke.AutoCAD;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

/// <inheritdoc cref="RxBim.Nuke.AutoCAD.AutocadRxBimBuild" />
[GitHubActions("CI",
    GitHubActionsImage.WindowsLatest,
    FetchDepth = 0,
    OnPushBranches = new[] { DevelopBranch, FeatureBranches },
    InvokedTargets = new[] { nameof(Test), nameof(IPublish.Publish) },
    ImportSecrets = new[] { "NUGET_API_KEY", "ALL_PACKAGES" })]
[GitHubActions("Publish",
    GitHubActionsImage.WindowsLatest,
    FetchDepth = 0,
    OnPushBranches = new[] { MasterBranch, "release/**" },
    InvokedTargets = new[] { nameof(Test), nameof(IPublish.Publish) },
    ImportSecrets = new[] { "NUGET_API_KEY", "ALL_PACKAGES" })]
public class Build : AutocadRxBimBuild, IPublish, IRunIntegrationTests
{
    const string MasterBranch = "master";
    const string DevelopBranch = "develop";
    const string FeatureBranches = "feature/**";

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
    public static int Main() => Execute<Build>(x => x.From<IPublish>().List);

    Target CleanWorkDir =>
        targetDefinition => targetDefinition
            .Before<IPublish>(p => p.Release)
            .DependentFor<IPublish>(p => p.Pack)
            .After(Compile)
            .Executes(() =>
            {
                GitTasks.Git("reset --hard");
            });

    T From<T>()
        where T : INukeBuild =>
        (T)(object)this;
}