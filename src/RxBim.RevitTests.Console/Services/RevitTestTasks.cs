namespace RxBim.RevitTests.Console.Services;

using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using Models;
using Tests.SDK;
using Console = System.Console;

/// <summary>
///     Класс для запуска тестирования.
/// </summary>
public class RevitTestTasks
{
    private const string CommandName = "RxBim.RevitTests.Cmd.Cmd";

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
            CreateAddIn(workDir, options);
            var journal = CreateJournal(workDir);
            await Run(journal, options, cancellationToken);
            var testResults = await serverTask;
            await File.WriteAllTextAsync(options.ResultsFilePath, testResults, cancellationToken);
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private string CreateAddIn(string workDir, TestRunningOptions options)
    {
        var assemblyPath = Path.Combine(workDir, $"RxBim.RevitTests.Cmd.{options.RevitVersion}.dll");
        var addins = new XElement("RevitAddIns");
        var addin = new XElement("AddIn", new XAttribute("Type", "Command"));
        addin.Add(new XElement("Text", "Revit tests sample"));
        addin.Add(new XElement("Description", "Revit tests sample"));
        addin.Add(new XElement("Assembly", assemblyPath));
        addin.Add(new XElement("FullClassName", CommandName));
        addin.Add(new XElement("AddInId", "3cdcad5f-8afe-4871-8ad0-a3f699946319"));
        addin.Add(new XElement("VendorId", "Rxbim"));
        addins.Add(addin);

        var path = Path.Combine(workDir, "testAddIn.addin");
        addins.Save(path);
        return path;
    }

    private string CreateJournal(string workDir)
    {
        var sb = new StringBuilder();
        sb.AppendLine("'Dim Jrn");
        sb.AppendLine("Set Jrn = CrsJournalScript ");
        sb.AppendLine(
            $"Jrn.RibbonEvent \"Execute external command:3cdcad5f-8afe-4871-8ad0-a3f699946319:{CommandName}\"");
        sb.AppendLine("Jrn.Command \"Internal\" , \"Quit the application; prompts to save projects , ID_APP_EXIT\"");
        var path = Path.Combine(workDir, "journal.txt");
        File.WriteAllText(path, sb.ToString());
        return path;
    }

    private async Task Run(string journal, TestRunningOptions options, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = $@"C:\Program Files\Autodesk\Revit {options.RevitVersion}\Revit.exe",
            Arguments = journal,
            UseShellExecute = false,
        };
        using var process = new Process();
        process.StartInfo = startInfo;
        try
        {
            process.Start();
            process.WaitForInputIdle();
            await process.WaitForExitAsync(cancellationToken);
            process.Close();
        }
        catch (OperationCanceledException)
        {
            process.Kill();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Time out");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            process.Kill();
            Console.WriteLine(ex.ToString());
        }
    }
}