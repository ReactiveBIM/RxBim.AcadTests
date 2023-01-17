namespace AutocadTestFrameworkCmd.Services
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using AcadTestFramework.SDK;
    using AutocadTestFrameworkCmd.Helpers;
    using Autodesk.AutoCAD.ApplicationServices;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    /// <inheritdoc />
    public class TestListener : ITestListener
    {
        private readonly AcadTestClient _acadTestClient;
        private readonly DocumentCollection _acDocMgr;

        /// <summary>
        /// ctr
        /// </summary>
        /// <param name="acadTestClient"><see cref="AcadTestClient"/></param>
        /// <param name="documentCollection"><see cref="DocumentCollection"/></param>
        public TestListener(AcadTestClient acadTestClient, DocumentCollection documentCollection)
        {
            _acadTestClient = acadTestClient;
            _acDocMgr = documentCollection;
        }

        /// <inheritdoc/>
        public void TestStarted(ITest test)
        {
            // TODO
            /*if (test is TestFixture)
            {
                var drawingPath = GetDrawingPath(test);
                if (File.Exists(drawingPath))
                    _acDocMgr.Open(drawingPath);
            }*/

            SendMessage($"Test started {test.FullName}");
        }

        /// <inheritdoc/>
        public void TestFinished(ITestResult result)
        {
            SendMessage($"Test finished {result.FullName} {result.Output} {result.Message}");
        }

        /// <inheritdoc/>
        public void TestOutput(TestOutput output)
        {
            SendMessage($"Test output {output.TestName} is \"{output.Text}\"");
        }

        /// <inheritdoc/>
        public void SendMessage(TestMessage message)
        {
            SendMessage($"Destination {message.Destination}/ Message {message.Message}");
        }

        private void SendMessage(string message)
        {
            _acadTestClient.SendMessage(message);
        }

        private string? GetDrawingPath(ITest test)
        {
            // TODO добавлять или нет рабочую директорию
            var workingDirectory = default(string);
            var attribute = test.Fixture?.GetType().GetCustomAttributes().OfType<TestDrawingAttribute>().FirstOrDefault();
            if (attribute is null)
                return null;
            string absolutePath;

            /*// We can't get the instantiated attribute from the assembly because we performed a ReflectionOnly load
            TestModelAttribute testModelAttribute = new TestModelAttribute((string)testModelAttrib.ConstructorArguments.First().Value);*/

            if (Path.IsPathRooted(attribute.Path))
            {
                absolutePath = attribute.Path;
            }
            else
            {
                if (workingDirectory == null)
                {
                    // If the working directory is not specified.
                    // Add the relative path to the assembly's path.
                    absolutePath = Path.GetFullPath(
                        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                            attribute.Path));
                }
                else
                {
                    absolutePath = Path.GetFullPath(Path.Combine(workingDirectory, attribute.Path));
                }
            }

            return absolutePath;
        }
    }
}