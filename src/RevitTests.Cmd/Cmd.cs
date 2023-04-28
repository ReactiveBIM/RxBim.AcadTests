namespace RevitTests.Cmd;

using System.Diagnostics;
using AcadTests.SDK;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using JetBrains.Annotations;
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
    [UsedImplicitly]
    public PluginResult ExecuteCommand(
        AcadTestClient acadTestClient,
        ITestAssemblyRunner testAssemblyRunner,
        ITestFilter testFilter,
        UIApplication uiApplication,
        ITestListener testListener)
    {
        try
        {
            uiApplication.DialogBoxShowing += UiApplicationOnDialogBoxShowing;
            var options = acadTestClient.GetTestRunningOptions().GetAwaiter().GetResult();
            if (options.Debug)
                Debugger.Launch();
            var assembly = options.AssemblyPath;
            if (!File.Exists(assembly))
                throw new FileNotFoundException(assembly);

            Helper.UiApplication = uiApplication;
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

    private void UiApplicationOnDialogBoxShowing(object sender, DialogBoxShowingEventArgs e)
    {
        // Do not show the Revit dialog
        e.OverrideResult(1);
    }

    private void SendResults(AcadTestClient acadTestClient, ITestResult result)
    {
        var node = result.ToXml(true);
        acadTestClient.SendResult(node.OuterXml);
    }
}