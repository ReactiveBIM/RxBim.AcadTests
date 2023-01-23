using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using _build;
using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using RxBim.AutocadTestFramework.Console.Models;
using RxBim.AutocadTestFramework.Console.Services;
using RxBim.Nuke.AutoCAD;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

/// <inheritdoc />
public class Build : AutocadRxBimBuild
{
    /// <summary>
    /// blah
    /// </summary>
    /// <returns></returns>
    public static int Main() => Execute<Build>(x => x.Compile);

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
                        @"C:\Users\ivachevev\RiderProjects\RxBim.AcadTests\RxBim.AutocadTestFramework.Console\bin\Debug\net472\RxBim.AutocadTestFramework.Console.exe",
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
}