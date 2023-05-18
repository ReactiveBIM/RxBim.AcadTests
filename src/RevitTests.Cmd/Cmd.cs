namespace RevitTests.Cmd;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
using TestingUtils;

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

            Assembly.Load(typeof(Helper).Assembly.Location);
            Helper.UiApplication = uiApplication;
            var result = RunTests(assembly, testAssemblyRunner, testFilter, testListener);
            SendResults(acadTestClient, result);

            // не работает
            /*while (uiApplication.Application.Documents
                   .Cast<Document>()
                   .Any(doc => doc.IsBackgroundCalculationInProgress()))
            {
                Thread.Sleep(1000);
                uiApplication.Application.WriteJournalComment("Thread.Sleep", true);
            }*/

            uiApplication.Application.WriteJournalComment("Cmd finished", true);
            foreach (var revitWorker in Process.GetProcessesByName("RevitWorker"))
            {
                revitWorker.Kill();
            }

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
                { FrameworkPackageSettings.RunOnMainThread, true },
            });
        var result = testAssemblyRunner.Run(testListener, testFilter);
        return result;
    }

    private void UiApplicationOnDialogBoxShowing(object sender, DialogBoxShowingEventArgs e)
    {
        switch (e.DialogId)
        {
            case "TaskDialog_Save_File":
                e.OverrideResult(7);
                break;
            case "Dialog_Revit_JournalAbort":
            case "TaskDialog_Calculation_In_Progress":
            default:
                e.OverrideResult(1);
                break;
        }
    }

    private void SendResults(AcadTestClient acadTestClient, ITestResult result)
    {
        var node = result.ToXml(true);
        acadTestClient.SendResult(node.OuterXml);
    }
}