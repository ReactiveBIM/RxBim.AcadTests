using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace _build;

using System;
using System.Diagnostics;
using System.Linq;
using AcadTests.Nuke;
using Bimlab.Nuke.Components;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Tools.DotNet;
using RxBim.Nuke.AutoCAD;

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

    Target IntegrationTests =>
        _ => _
            .Executes(async () =>
            {
                var solution = Solution;
                var projects = solution.AllProjects
                    .Where(x => x.Name.EndsWith(".IntegrationTests"))
                    .ToList();
                if (!projects.Any())
                    throw new ArgumentException("project not found");
                foreach (var project in projects)
                {
                    var outputDirectory = solution.Directory / "testoutput" / project.Name;
                    DotNetBuild(settings => settings
                        .SetProjectFile(project)
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
                        $@"-a {assemblyPath} -r {results} -v 2019 -d");
                    var process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();
                    await process.WaitForExitAsync();
                    var resultPath = outputDirectory / "result.html";
                    await new ResultConverter()
                        .Convert(results, resultPath);
                }
            });

    /// <summary>
    ///     blah
    /// </summary>
    /// <returns></returns>
    public static int Main() => Execute<Build>(x => x.Compile);
}