namespace RxBim.AutocadTestFramework.Console.Services;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AcadTestFramework.SDK;
using Models;
using ScriptUtils.Autocad;
using ScriptUtils.Autocad.Extensions;

/// <summary>
/// Класс для запуска тестирования.
/// </summary>
public class AcadTestTasks
{
    /// <summary>
    /// Запускает тестирование.
    /// </summary>
    /// <param name="options">Параметры тестирования.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    public async Task Run(TestRunningOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var server = new AcadTestSdk().AcadTestServer;
            var serverTask = server.Start(options, cancellationToken);
            var runner = new AutocadScriptRunner
            {
                UseConsole = options.UseAcCoreConsole,
                AcadVersion = options.AcadVersion
            };

            var acadTask = runner.Run(builder => builder
                    .SetStartMode(false)
                    .SetFiledia(false)
                    .SetSecureLoad(false)
                    .NetLoadCommand(typeof(AutocadTestFrameworkCmd.Command).Assembly.Location)
                    .AddCommand("AutocadTestFrameworkCommand")
                    .SetSecureLoad(true)
                    .SetFiledia(true)
                    .QuitCommand(),
                cancellationToken);
            await acadTask;
            var testResults = await serverTask;
            File.WriteAllText(options.ResultsFilePath, testResults);
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}