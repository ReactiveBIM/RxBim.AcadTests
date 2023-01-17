namespace AutocadTestFrameworkCmd.Services
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using Autodesk.AutoCAD.ViewModel.PointCloudManager;
    using NUnit.Framework.Api;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    /// <inheritdoc />
    public class MyTestAssemblyBuilder : ITestAssemblyBuilder
    {
        private readonly ITestAssemblyBuilder _implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTestAssemblyBuilder"/> class.
        /// </summary>
        /// <param name="implementation"><see cref="ITestAssemblyBuilder"/></param>
        public MyTestAssemblyBuilder(ITestAssemblyBuilder implementation)
        {
            _implementation = implementation;
        }

        /// <inheritdoc />
        public ITest Build(Assembly assembly, IDictionary<string, object> options)
        {
            var baseTest = _implementation.Build(assembly, options);
            return Filter(baseTest, assembly.GetName().Name);
        }

        /// <inheritdoc />
        public ITest Build(string assemblyName, IDictionary<string, object> options)
        {
            var baseTest = _implementation.Build(assemblyName, options);
            return Filter(baseTest, assemblyName);
        }

        private ITest Filter(ITest baseTest, string assemblyName)
        {
            var fixtures = baseTest.Flatten(test => test is not TestFixture && test.HasChildren
                    ? test.Tests
                    : ImmutableArray<ITest>.Empty)
                .OfType<TestFixture>();
            var ans = new TestSuite(assemblyName);
            foreach (var fixture in fixtures)
            {
                ans.Tests.Add(fixture);
            }

            return ans;
        }
    }
}