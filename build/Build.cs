using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using AcadTests.Nuke;
using Bimlab.Nuke.Components;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
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
public class Build : AutocadRxBimBuild, IPublish
{
    const string MasterBranch = "master";
    const string DevelopBranch = "develop";
    const string FeatureBranches = "feature/**";

    const string RevitTestTool = "RxBim.RevitTests.Console";
    const string AcadTestTool = "RxBim.AcadTests.Console";

    public Build()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }

    Target IntegrationTests =>
        _ => _
            .Executes(async () =>
            {
                var solution = Solution;
                var projects = solution.AllProjects
                    .Where(x => x.Name.EndsWith("Autocad.IntegrationTests"))
                    .ToList();
                if (!projects.Any())
                    throw new ArgumentException("project not found");
                DotNetToolUpdate(settings => settings
                    .SetPackageName(AcadTestTool)
                    .SetVersion("1.0.1-dev003")
                    .EnableGlobal());
                foreach (var project in projects)
                {
                    var outputDirectory = solution.Directory / "testoutput" / project.Name;
                    if (Directory.Exists(outputDirectory))
                        Directory.Delete(outputDirectory, true);
                    DotNetBuild(settings => DotNetBuildSettingsExtensions
                        .SetProjectFile<DotNetBuildSettings>(settings, project)
                        .SetConfiguration("Debug")
                        .SetOutputDirectory(outputDirectory));
                    var assemblyName = project.Name + ".dll";
                    var assemblyPath = outputDirectory / assemblyName;
                    var results = outputDirectory / "result.xml";
                    ProcessTasks
                        .StartProcess(AcadTestTool, $@"-a {assemblyPath} -r {results} -v 2019")
                        .WaitForExit();
                    var resultPath = outputDirectory / "result.html";
                    await new ResultConverter()
                        .Convert(results, resultPath);
                }
            });

    Target UpdateRevitTestConsole => _ => _.Executes(() =>
        DotNetToolUpdate(settings => settings
            .SetPackageName("RxBim.RevitTests.Console")
            .SetVersion("1.0.1-dev003")
            .EnableGlobal()));

    Target RevitIntegrationTests =>
        _ => _
            .Executes(async () =>
            {
                var solution = Solution;
                List<Project> projects;
                projects = solution.AllProjects
                    .Where(x => x.Name.EndsWith("Revit.IntegrationTests"))
                    .ToList();
                if (!projects.Any())
                    throw new ArgumentException("project not found");
                DotNetToolUpdate(settings => settings
                    .SetPackageName(RevitTestTool)
                    .SetVersion("1.0.1-dev003")
                    .EnableGlobal());
                foreach (var project in projects)
                {
                    var outputDirectory = solution.Directory / "testoutput" / project.Name;
                    if (Directory.Exists(outputDirectory))
                        Directory.Delete(outputDirectory, true);
                    DotNetBuild(settings => DotNetBuildSettingsExtensions
                        .SetProjectFile<DotNetBuildSettings>(settings, project)
                        .SetConfiguration("Debug")
                        .SetOutputDirectory(outputDirectory));
                    var assemblyName = project.Name + ".dll";
                    var assemblyPath = outputDirectory / assemblyName;
                    var results = outputDirectory / "result.xml";
                    ProcessTasks
                        .StartProcess(RevitTestTool, $@"-a {assemblyPath} -r {results} -v 2019")
                        .WaitForExit();
                    var resultPath = outputDirectory / "result.html";
                    await new ResultConverter()
                        .Convert(results, resultPath);
                }
            });

    new Target Test =>
        targetDefinition => targetDefinition
            .Before(Clean)
            .Before<IRestore>()
            .Executes(() =>
            {
                DotNetTest(settings => settings
                    .SetProjectFile(From<IHazSolution>().Solution.Path)
                    .SetConfiguration(From<IHazConfiguration>().Configuration)
                    .SetFilter("FullyQualifiedName!~IntegrationTests"));
            });

    /// <summary>
    ///     Main
    /// </summary>
    public static int Main() => Execute<Build>(x => x.From<IPublish>().List);

    T From<T>()
        where T : INukeBuild =>
        (T)(object)this;
}