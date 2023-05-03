using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using AcadTests.Nuke;
using Bimlab.Nuke.Components;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
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
                foreach (var project in projects)
                {
                    var outputDirectory = solution.Directory / "testoutput" / project.Name;
                    DotNetTasks.DotNetBuild(settings => DotNetBuildSettingsExtensions
                        .SetProjectFile<DotNetBuildSettings>(settings, project)
                        .SetConfiguration("Debug")
                        .SetOutputDirectory(outputDirectory));
                    var assemblyName = project.Name + ".dll";
                    var assemblyPath = outputDirectory / assemblyName;
                    var results = outputDirectory / "result.xml";

                    // TODO
                    /*var ts = new CancellationTokenSource(20000);
                var runner = new AcadTestTasks();
                await runner.Run(new TestRunningOptions()
                {
                    Debug = false,
                    AcadVersion = 2019,
                    AssemblyPath = assemblyPath,
                    ResultsFilePath = results,
                    UseAcCoreConsole = false
                }, ts.Token);*/
                    var startInfo = new ProcessStartInfo(
                        @"C:\Users\ivachevev\RiderProjects\RxBim.AcadTests\AcadTests.Console\bin\Debug\net472\AcadTests.Console.exe",
                        $@"-a {assemblyPath} -r {results} -v 2019");
                    var process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();
                    await process.WaitForExitAsync();
                    var resultPath = outputDirectory / "result.html";
                    await new ResultConverter()
                        .Convert(results, resultPath);
                }
            });

    Target RevitIntegrationTests =>
        _ => _
            .Executes(async () =>
            {
                var solution = Solution;
                var projects = solution.AllProjects
                    .Where(x => x.Name.EndsWith("Revit.IntegrationTests"))
                    .ToList();
                if (!projects.Any())
                    throw new ArgumentException("project not found");
                var consoleDllPath = typeof(RevitTests.Console.Services.RevitTestTasks).Assembly.Location;
                foreach (var project in projects)
                {
                    var outputDirectory = solution.Directory / "testoutput" / project.Name;
                    Directory.Delete(outputDirectory, true);
                    DotNetTasks.DotNetBuild(settings => DotNetBuildSettingsExtensions
                        .SetProjectFile<DotNetBuildSettings>(settings, project)
                        .SetConfiguration("Debug")
                        .SetOutputDirectory(outputDirectory));
                    var assemblyName = project.Name + ".dll";
                    var assemblyPath = outputDirectory / assemblyName;
                    var results = outputDirectory / "result.xml";

                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "dotnet",
                            Arguments = consoleDllPath + " " + $@"-a {assemblyPath} -r {results} -v 2019 -d",
                        }
                    };

                    process.Start();
                    process.WaitForExit();
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