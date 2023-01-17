// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using AcadTestFramework.SDK;
using RxBim.AutocadTestFramework;
using RxBim.AutocadTestFramework.Console.Models;
using RxBim.AutocadTestFramework.Console.Services;
using RxBim.ScriptUtils.Autocad;
using RxBim.ScriptUtils.Autocad.Extensions;

Console.WriteLine("Hello, World!");

var options = new TestRunningOptionsFactory(args).GetTestRunningOptions();

/*var options = new TestRunningOptions()
{
    AssemblyPath = typeof(RxBim.Example.Autocad.IntegrationTests.Tests).Assembly.Location,
    Debug = false,
    AcadVersion = 2019,
    UseAcCoreConsole = true
};*/

var runner = new AcadTestTasks();
await runner.Run(options);

