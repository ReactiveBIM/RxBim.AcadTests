namespace RxBim.AutocadTests.Console.Models;

using System;
using System.IO;
using CommandLine;
using Tests.SDK.Abstractions;

/// <summary>
///     Параметры тестирования
/// </summary>
public class TestRunningOptions : ITestRunningOptions
{
    /// <summary>
    ///     Results file path.
    /// </summary>
    [Option('r', "results", Required = true, HelpText = "Set results file path.")]
    public string ResultsFilePath { get; set; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "results.xml");

    /// <summary>
    ///     Should use acad console.
    /// </summary>
    [Option('c', "useConsole", Required = false, HelpText = "Should use acad console.")]
    public bool UseAcCoreConsole { get; set; }

    /// <summary>
    ///     The version of autocad.
    /// </summary>
    [Option('v', "acadVersion", Required = false, HelpText = "The version of autocad.")]
    public int AcadVersion { get; set; }

    /// <inheritdoc />
    [Option('a', "assembly", Required = true, HelpText = "The full path to the assembly containing your tests.")]
    public string AssemblyPath { get; set; } = string.Empty;

    /// <summary>
    ///     Debug mode.
    /// </summary>
    [Option('d', "debug", Required = false, HelpText = "Set debug mode.")]
    public bool Debug { get; set; } = false;

    /// <inheritdoc />
    [Option('t', "testName", Required = false, HelpText = "The name of a test to run.")]
    public string? TestName { get; set; }
}