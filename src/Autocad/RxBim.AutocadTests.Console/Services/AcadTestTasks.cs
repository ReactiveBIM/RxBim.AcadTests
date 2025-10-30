namespace RxBim.AutocadTests.Console.Services;

using Models;
using ScriptUtils;
using ScriptUtils.Extensions;
using Tests.SDK;

/// <summary>
///     Класс для запуска тестирования.
/// </summary>
public class AcadTestTasks
{
    /// <summary>
    ///     Запускает тестирование.
    /// </summary>
    /// <param name="options">Параметры тестирования.</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    public async Task Run(TestRunningOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var server = new AcadTestSdk().AcadTestServer;
            var serverTask = server.Start(options, cancellationToken);
            var workDir = Path.GetDirectoryName(options.AssemblyPath)!;

            var runner = new AutocadScriptRunner
            {
                UseConsole = options.UseAcCoreConsole,
                AcadVersion = options.AcadVersion
            };

            var acadTask = runner.Run(builder => builder
                    .SetStartMode(false)
                    .SetFiledia(false)
                    .SetSecureLoad(false)
                    .NetLoadCommand(Path.Combine(workDir, $"RxBim.AutocadTests.Cmd.{options.AcadVersion}.dll"))
                    .AddCommand("AutocadTestFrameworkCommand")
                    .SetSecureLoad(true)
                    .SetFiledia(true)
                    .QuitCommand(),
                options.Debug,
                cancellationToken);
            await acadTask;
            var testResults = await serverTask;
            await File.WriteAllTextAsync(options.ResultsFilePath, testResults, cancellationToken);
        }
        catch (OperationCanceledException e)
        {
            System.Console.WriteLine(e.ToString());
        }
    }
}