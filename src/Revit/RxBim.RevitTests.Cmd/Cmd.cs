namespace RxBim.RevitTests.Cmd;

using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Command.Revit;
using JetBrains.Annotations;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using RxBim.AutocadTests.Cmd.Abstractions;
using Shared;
using Tests.SDK.Abstractions;

/// <inheritdoc />
[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class Cmd : RxBimCommand
{
#if NETCOREAPP
    /// <inheritdoc/>
    protected override bool RunInSeparatedContext => false;
#endif

    /// <inheritdoc />
    [UsedImplicitly]
    public PluginResult ExecuteCommand(
        IAcadTestClient acadTestClient,
        ITestAssemblyRunner testAssemblyRunner,
        ITestFilter testFilter,
        UIApplication uiApplication,
        ITestListener testListener,
        ITestService testService)
    {
        try
        {
            uiApplication.DialogBoxShowing += UiApplicationOnDialogBoxShowing;
            RevitContext.UiApplication = uiApplication;

            testService.ExecuteTest(acadTestClient, testAssemblyRunner, testFilter, testListener);

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
            default: // "Dialog_Revit_JournalAbort", "TaskDialog_Calculation_In_Progress", ...
                e.OverrideResult(1); // Ok
                break;
        }
    }
}