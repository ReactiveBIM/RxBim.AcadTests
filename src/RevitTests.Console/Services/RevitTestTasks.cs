namespace RevitTests.Console.Services;

using System.Diagnostics;
using AcadTests.SDK;
using Models;
using Console = System.Console;

/// <summary>
///     Класс для запуска тестирования.
/// </summary>
public class RevitTestTasks
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
            var workDir = Path.GetDirectoryName(options.AssemblyPath);
            var journal = CreateJournal(workDir!);
            await Run(journal, cancellationToken);
            var testResults = await serverTask;
            await File.WriteAllTextAsync(options.ResultsFilePath, testResults, cancellationToken);
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private string CreateAddIn(string workDir)
    {
        var assemblyPath = Path.Combine(workDir, "RevitTests.Cmd.dll");
        var addin = $@"<RevitAddIns>
    <AddIn Type=""Command"">
        <Text>Revit tests sample</Text>
		<Description>Revit tests sample</Description>
		<Assembly>{assemblyPath}</Assembly>
		<FullClassName>RevitTests.Cmd.Cmd</FullClassName>
		<AddInId>efeffffd-fb08-41df-b62b-50429e1383d9</AddInId>
		<VendorId>efeffffd-fb08-41df-b62b-50429e1383d9</VendorId>
	</AddIn>
</RevitAddIns>";
        var path = Path.Combine(workDir, "testAddIn.addin");
        File.WriteAllText(path, addin);
        return path;
    }

    private string CreateJournal(string workDir)
    {
        var addin = @"'Dim Jrn 
Set Jrn = CrsJournalScript 
Jrn.RibbonEvent ""Execute external command:efeffffd-fb08-41df-b62b-50429e1383d9:RevitTests.Cmd.Cmd""
Jrn.Command ""Internal"" , ""Close the active project , ID_REVIT_FILE_CLOSE"" 
Jrn.RibbonEvent ""Execute external command:7CF281FA-D8C0-499E-AA60-7A1CF582129F:RTF.Applications.RTFClientEndCmd"" 
Jrn.Command ""SystemMenu"" , ""Quit the application; prompts to save projects , ID_APP_EXIT""";
        var path = Path.Combine(workDir, "journal.txt");
        File.WriteAllText(path, addin);
        return path;
    }

    private async Task Run(string journal, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = @"C:\Program Files\Autodesk\Revit 2019\Revit.exe",
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