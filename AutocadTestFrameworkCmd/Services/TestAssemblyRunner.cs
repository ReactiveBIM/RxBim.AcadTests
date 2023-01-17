namespace AutocadTestFrameworkCmd.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Autodesk.AutoCAD.EditorInput;
    using NUnit.Framework.Api;
    using NUnit.Framework.Interfaces;

    /// <inheritdoc />
    public class TestAssemblyRunner : ITestAssemblyRunner
    {
        private readonly ITestAssemblyRunner _testAssemblyRunner;
        private readonly Editor _editor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAssemblyRunner"/> class.
        /// </summary>
        /// <param name="testAssemblyRunner"><see cref="ITestAssemblyRunner"/></param>
        /// <param name="editor"><see cref="Editor"/></param>
        public TestAssemblyRunner(ITestAssemblyRunner testAssemblyRunner, Editor editor)
        {
            _testAssemblyRunner = testAssemblyRunner;
            _editor = editor;
        }

        /// <inheritdoc />
        public ITest LoadedTest => _testAssemblyRunner.LoadedTest;

        /// <inheritdoc />
        public ITestResult Result => _testAssemblyRunner.Result;

        /// <inheritdoc />
        public bool IsTestLoaded => _testAssemblyRunner.IsTestLoaded;

        /// <inheritdoc />
        public bool IsTestRunning => _testAssemblyRunner.IsTestRunning;

        /// <inheritdoc />
        public bool IsTestComplete => _testAssemblyRunner.IsTestComplete;

        /// <inheritdoc />
        public ITest Load(string assemblyName, IDictionary<string, object> settings)
        {
            return _testAssemblyRunner.Load(assemblyName, settings);
        }

        /// <inheritdoc />
        public ITest Load(Assembly assembly, IDictionary<string, object> settings)
        {
            return _testAssemblyRunner.Load(assembly, settings);
        }

        /// <inheritdoc />
        public int CountTestCases(ITestFilter filter)
        {
            return _testAssemblyRunner.CountTestCases(filter);
        }

        /// <inheritdoc />
        public ITest ExploreTests(ITestFilter filter)
        {
            return _testAssemblyRunner.ExploreTests(filter);
        }

        /// <inheritdoc />
        public ITestResult Run(ITestListener listener, ITestFilter filter)
        {
            return _testAssemblyRunner.Run(listener, filter);
        }

        /// <inheritdoc />
        public void RunAsync(ITestListener listener, ITestFilter filter)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool WaitForCompletion(int timeout)
        {
            return _testAssemblyRunner.WaitForCompletion(timeout);
        }

        /// <inheritdoc />
        public void StopRun(bool force)
        {
            _testAssemblyRunner.StopRun(force);
        }
    }
}

/*/// <summary>
        /// asdasd
        /// </summary>
        public void DoSomething()
        {
            // Create a temporary application domain to load the assembly.
            var tempDomain = AppDomain.CreateDomain("temp_Domain");

            // найти тесты в сборке
            // NOTE: We use reflection only load here so that we don't have to resolve all binaries
            // This is an assumption by Dynamo tests which reference assemblies that can be resolved 
            // at runtime inside Revit.
            var assembly = Assembly.ReflectionOnlyLoadFrom(AssemplyPath);
            foreach (var fixtureType in assembly.GetTypes()
                         .Where(ft => ft.GetCustomAttributes()
                             .OfType<TestFixtureAttribute>()
                             .Any()))
            {
                foreach (var test in fixtureType.GetMethods())
                {
                    if (!test.GetCustomAttributes().OfType<TestAttribute>().Any())
                    {
                        continue;
                    }

                    string absoluteDrawingPath;
                    var drawingPath = test.GetCustomAttributes(typeof(TestDrawingAttribute))
                        .Cast<TestDrawingAttribute>()
                        .FirstOrDefault()?.Path;

                    if (drawingPath is null)
                    {
                        throw new Exception("TestDrawingAttribute error");
                    }

                    if (Path.IsPathRooted(drawingPath))
                    {
                        absoluteDrawingPath = drawingPath;
                    }
                    else
                    {
                        if (WorkingDirectory == null)
                        {
                            // If the working directory is not specified.
                            // Add the relative path to the assembly's path.
                            absoluteDrawingPath = Path.GetFullPath(
                                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                                    drawingPath));
                        }
                        else
                        {
                            absoluteDrawingPath = Path.GetFullPath(Path.Combine(WorkingDirectory, drawingPath));
                        }
                    }
                }
            }

            AppDomain.Unload(tempDomain);
        }*/