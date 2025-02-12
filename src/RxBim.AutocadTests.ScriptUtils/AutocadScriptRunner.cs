namespace RxBim.AutocadTests.ScriptUtils;

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Extensions;

/// <inheritdoc />
public class AutocadScriptRunner : IAutocadScriptRunner
{
    private string? _templateFile;

    /// <summary>
    ///     blah
    /// </summary>
    public bool UseConsole { get; set; } = true;

    /// <summary>
    ///     The autocad version.
    /// </summary>
    public int AcadVersion { get; set; } = 2019;

    // $"C:\\Program Files\\Autodesk\\AutoCAD {year}\\acad.exe"
    private string AcadConsoleExePath => UseConsole
        ? $"C:\\Program Files\\Autodesk\\AutoCAD {AcadVersion}\\accoreconsole.exe"
        : $"C:\\Program Files\\Autodesk\\AutoCAD {AcadVersion}\\acad.exe";

    /// <inheritdoc />
    public async Task Run(Action<IAutocadScriptBuilder> action, CancellationToken cancellationToken)
    {
        var scriptBuilder = new AutocadScriptBuilder();
        action(scriptBuilder);
        var script = scriptBuilder.ToString();
        var arguments = GetParams(script);

        var startInfo = new ProcessStartInfo
        {
            FileName = AcadConsoleExePath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true
        };
        using var process = new Process();
        process.StartInfo = startInfo;
        try
        {
            var outSb = new StringBuilder();
            startInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += (_, e) => { outSb.AppendLine(e.Data); };
            startInfo.RedirectStandardError = true;
            process.ErrorDataReceived += (_, e) => { outSb.AppendLine(e.Data); };
            var encoding = Encoding.Unicode;
            process.StartInfo.StandardOutputEncoding = encoding;
            process.StartInfo.StandardErrorEncoding = encoding;
            process.Start();
            if (!UseConsole)
                process.WaitForInputIdle();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync(cancellationToken);
            process.Close();
            Console.WriteLine(outSb.ToString());
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

    /// <inheritdoc />
    public IAutocadScriptRunner SetTemplateFile(string path)
    {
        _templateFile = path;
        return this;
    }

    private string GetParams(string script)
    {
        var tempScriptFilePath = Path.GetTempFileName();
        tempScriptFilePath = tempScriptFilePath.Replace(".tmp", ".scr");
        File.WriteAllText(tempScriptFilePath, script);
        File.Move(tempScriptFilePath, tempScriptFilePath);
        var param = new StringBuilder();

        param.Append("/language \"ru-RU\"");
        if (!string.IsNullOrWhiteSpace(_templateFile))
            param.Append($" /t \"{_templateFile}\"");
        if (!string.IsNullOrWhiteSpace(script))
            param.Append($" /{(UseConsole ? "s" : "b")} \"{tempScriptFilePath}\"");
        return param.ToString();
    }
}