﻿namespace RxBim.RevitTests.Cmd;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Command.Revit;
using JetBrains.Annotations;
using NUnit;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using Shared;
using Tests.SDK;

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

            Assembly.Load(typeof(RevitContext).Assembly.Location);
            RevitContext.UiApplication = uiApplication;
            var result = RunTests(assembly, testAssemblyRunner, testFilter, testListener);
            SendResults(acadTestClient, result);

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

    private void UiApplicationOnDialogBoxShowing(object? sender, DialogBoxShowingEventArgs e)
    {
        switch (e.DialogId)
        {
            case "TaskDialog_Save_File":
                e.OverrideResult(7);
                break;
            case "TaskDialog_Changes_Not_Saved":
                e.OverrideResult(1003); // Не сохранять проект
                break;
            case "TaskDialog_Close_Project_Without_Saving":
                e.OverrideResult(1001); // Освободить все элементы и рабочие наборы
                break;
            case "Dialog_Revit_JournalAbort":
            case "TaskDialog_Calculation_In_Progress":
            default:
                e.OverrideResult(1); // Ok
                break;
        }
    }

    private void SendResults(AcadTestClient acadTestClient, ITestResult result)
    {
        var node = result.ToXml(true);
        acadTestClient.SendResult(node.OuterXml);
    }
}