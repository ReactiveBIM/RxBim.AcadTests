namespace AutocadTestFrameworkCmd.Helpers
{
    using System;
    using JetBrains.Annotations;

    /// <inheritdoc />
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class)]
    public class TestDrawingAttribute : Attribute
    {
        /// <inheritdoc />
        public TestDrawingAttribute(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Путь к тестовому чертежу
        /// </summary>
        public string Path { get; }
    }
}