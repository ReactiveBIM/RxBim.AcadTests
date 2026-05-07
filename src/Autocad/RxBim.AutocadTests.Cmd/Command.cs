namespace RxBim.AutocadTests.Cmd;

using Abstractions;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using JetBrains.Annotations;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using RxBim.Command.Autocad;
using RxBim.Tests.SDK.Abstractions;
using Shared;
using Shared.Autocad;
using Tests.SDK;
using Exception = Exception;

/// <inheritdoc />
[RxBimCommandClass("AutocadTestFrameworkCommand", CommandFlags.Session)]
[PublicAPI]
public class Command : RxBimCommand
{
    /// <summary>Executes command.</summary>
    /// <param name="editor"><see cref="Editor" /> instance.</param>
    /// <param name="acadTestClient"><see cref="AcadTestClient" /></param>
    /// <param name="testAssemblyRunner"><see cref="ITestAssemblyRunner" /></param>
    /// <param name="testFilter"><see cref="ITestFilter" /></param>
    /// <param name="testListener"><see cref="ITestListener" /></param>
    /// <param name="testService"><see cref="ITestService"/></param>
    public PluginResult ExecuteCommand(Editor editor,
        IAcadTestClient acadTestClient,
        ITestAssemblyRunner testAssemblyRunner,
        ITestFilter testFilter,
        ITestListener testListener,
        ITestService testService)
    {
        try
        {
            testService.ExecuteTest(acadTestClient, testAssemblyRunner, testFilter, testListener);
            return PluginResult.Succeeded;
        }
        catch (Exception e)
        {
            acadTestClient.SendResult(e.ToString());
            return PluginResult.Failed;
        }
    }
}