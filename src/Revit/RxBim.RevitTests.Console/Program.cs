// See https://aka.ms/new-console-template for more information

using RxBim.RevitTests.Console.Services;

var options = new TestRunningOptionsFactory(args).GetTestRunningOptions();
var rt = new RevitTestTasks();
await rt.Run(options, CancellationToken.None);