namespace RxBim.AutocadTests.Cmd;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Autodesk.AutoCAD.EditorInput;
using JetBrains.Annotations;
using Newtonsoft.Json;
using NUnit;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using RxBim.Command.Autocad;
using Shared;
using Shared.Autocad;
using Tests.SDK;

/// <inheritdoc />
[RxBimCommandClass("AutocadTestFrameworkCommand")]
[PublicAPI]
public class Command : RxBimCommand
{
    private ITestAssemblyRunner _testAssemblyRunner = null!;
    private ITestFilter _testFilter = null!;
    private ITestListener _testListener = null!;

    /// <summary>
    ///     Executes command.
    /// </summary>
    /// <param name="editor"><see cref="Editor" /> instance.</param>
    /// <param name="acadTestClient">
    ///     <see cref="AcadTestClient" />
    /// </param>
    /// <param name="testAssemblyRunner">
    ///     <see cref="ITestAssemblyRunner" />
    /// </param>
    /// <param name="testFilter">
    ///     <see cref="ITestFilter" />
    /// </param>
    /// <param name="testListener">
    ///     <see cref="ITestListener" />
    /// </param>
    public PluginResult ExecuteCommand(Editor editor,
        AcadTestClient acadTestClient,
        ITestAssemblyRunner testAssemblyRunner,
        ITestFilter testFilter,
        ITestListener testListener)
    {
        _testListener = testListener;
        _testAssemblyRunner = testAssemblyRunner;
        _testFilter = testFilter;
        try
        {
            var options = acadTestClient.GetTestRunningOptions().GetAwaiter().GetResult();
            if (options.Debug)
                Debugger.Launch();
            var assembly = options.AssemblyPath;
            if (!File.Exists(assembly))
                throw new FileNotFoundException(assembly);

            var result = RunTests(assembly);
            SendResults(acadTestClient, result);
            return PluginResult.Succeeded;
        }
        catch (Exception e)
        {
            acadTestClient.SendResult(e.ToString());
            return PluginResult.Failed;
        }
    }

    private static void SaveResults(ITestResult result, string resultsPath)
    {
        var output = JsonConvert.SerializeObject(result, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = (_, args) => { args.ErrorContext.Handled = true; }
        });
        File.WriteAllText(resultsPath, output);
    }

    private ITestResult RunTests(string assemblyPath)
    {
        _testAssemblyRunner.Load(assemblyPath,
            new Dictionary<string, object>
            {
                { FrameworkPackageSettings.RunOnMainThread, true }
            });
        var result = _testAssemblyRunner.Run(_testListener, _testFilter);
        return result;
    }

    private void SendResults(AcadTestClient acadTestClient, ITestResult result)
    {
        var node = result.ToXml(true);
        acadTestClient.SendResult(node.OuterXml);
    }
}