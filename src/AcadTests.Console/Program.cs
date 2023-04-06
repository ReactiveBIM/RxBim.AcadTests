using System.Threading;
using AcadTests.Console.Services;

var options = new TestRunningOptionsFactory(args).GetTestRunningOptions();
var runner = new AcadTestTasks();
await runner.Run(options, CancellationToken.None);