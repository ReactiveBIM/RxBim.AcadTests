namespace RevitTests.Cmd;

using System.Diagnostics;
using AcadTests.SDK;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using NUnit;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using RxBim.Command.Revit;
using RxBim.Shared;

/// <inheritdoc />
[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class Cmd : RxBimCommand
{
    /// <inheritdoc />
    public PluginResult ExecuteCommand(
        AcadTestClient acadTestClient,
        ITestAssemblyRunner testAssemblyRunner,
        ITestFilter testFilter,
        ITestListener testListener)
    {
        try
        {
            var options = acadTestClient.GetTestRunningOptions().GetAwaiter().GetResult();
            if (options.Debug)
                Debugger.Launch();
            var assembly = options.AssemblyPath;
            if (!File.Exists(assembly))
                throw new FileNotFoundException(assembly);

            var result = RunTests(assembly, testAssemblyRunner, testFilter, testListener);
            SendResults(acadTestClient, result);
            return PluginResult.Succeeded;
        }
        catch (Exception e)
        {
            acadTestClient.SendResult(e.ToString());
            return PluginResult.Failed;
        }
    }

    /// <inheritdoc />
    public override bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
    {
        return true;
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

    private ITestResult RunTests(
        string assemblyPath,
        ITestAssemblyRunner testAssemblyRunner,
        ITestFilter testFilter,
        ITestListener testListener)
    {
        testAssemblyRunner.Load(assemblyPath,
            new Dictionary<string, object>
            {
                { FrameworkPackageSettings.RunOnMainThread, true }
            });
        var result = testAssemblyRunner.Run(testListener, testFilter);
        return result;
    }

    private void SendResults(AcadTestClient acadTestClient, ITestResult result)
    {
        var node = result.ToXml(true);
        acadTestClient.SendResult(node.OuterXml);
    }
}