namespace RxBim.AutocadTestFramework.Console.Services;

using System.IO;
using System.Threading.Tasks;
using AcadTestFramework.SDK;
using Models;
using ScriptUtils.Autocad;
using ScriptUtils.Autocad.Extensions;

/// <summary>
/// asdas
/// </summary>
public class AcadTestTasks
{
    /// <summary>
    /// asdasd
    /// </summary>
    /// <param name="options">asdas</param>
    public async Task Run(TestRunningOptions options)
    {
        var server = new AcadTestSdk().AcadTestServer;
        var serverTask = server.Start(options);
        var runner = new AutocadScriptRunner
        {
            UseConsole = options.UseAcCoreConsole,
            AcadVersion = options.AcadVersion
        };
        var acadTask = runner.Run(builder => builder
            .SetStartMode(false)
            .SetSecureLoad(false)
            .NetLoadCommand(typeof(AutocadTestFrameworkCmd.Command).Assembly.Location)
            .AddCommand("AutocadTestFrameworkCommand")
            .SetSecureLoad(true)
            .QuitCommand());
        await acadTask;
        var testResults = await serverTask;
        File.WriteAllText(options.ResultsFilePath, testResults);
    }
}