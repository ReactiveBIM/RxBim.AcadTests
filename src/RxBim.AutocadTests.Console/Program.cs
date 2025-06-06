﻿using System.Threading;
using RxBim.AutocadTests.Console.Services;

var options = new TestRunningOptionsFactory(args).GetTestRunningOptions();
var runner = new AcadTestTasks();
await runner.Run(options, CancellationToken.None);