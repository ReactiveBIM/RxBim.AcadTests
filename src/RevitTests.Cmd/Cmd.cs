namespace RevitTests.Cmd;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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

            while (uiApplication.Application.Documents
                   .Cast<Document>()
                   .Any(doc => doc.IsBackgroundCalculationInProgress()))
            {
                Thread.Sleep(1000);
                uiApplication.Application.WriteJournalComment("Thread.Sleep", true);
            }

            uiApplication.Application.WriteJournalComment("Cmd finished", true);
            /*var closeCmd = RevitCommandId.LookupPostableCommandId(PostableCommand.ExitRevit);
            uiApplication.PostCommand(closeCmd);*/

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
                { FrameworkPackageSettings.RunOnMainThread, true }
            });
        var result = testAssemblyRunner.Run(testListener, testFilter);
        return result;
    }

    private void UiApplicationOnDialogBoxShowing(object sender, DialogBoxShowingEventArgs e)
    {
        // todo можно попробовать так проверять все документы и пробовать закрыть ревит через журнал
        /*while ((sender as UIApplication)?.ActiveUIDocument.Document?.IsBackgroundCalculationInProgress() == true)
            Thread.Sleep(1000);*/

        // Do not show the Revit dialog
        e.OverrideResult(1);

        // todo возможно стоить добавить обработку по e.DialogId
        /*
TaskDialog_Save_File
Dialog_Revit_JournalAbort
TaskDialog_Calculation_In_Progress
*/
    }

    private void SendResults(AcadTestClient acadTestClient, ITestResult result)
    {
        var node = result.ToXml(true);
        acadTestClient.SendResult(node.OuterXml);
    }
}