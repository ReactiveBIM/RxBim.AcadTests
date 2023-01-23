namespace AcadTestFramework.SDK.Helpers;

using System.Runtime.Serialization;
using Abstractions;

/// <inheritdoc />
[DataContract]
internal class TestRunningOptions : ITestRunningOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestRunningOptions"/> class.
    /// </summary>
    public TestRunningOptions()
    {
        AssemblyPath = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestRunningOptions"/> class.
    /// </summary>
    /// <param name="testRunningOptionsImplementation">other</param>
    public TestRunningOptions(ITestRunningOptions testRunningOptionsImplementation)
    {
        AssemblyPath = testRunningOptionsImplementation.AssemblyPath;
        Debug = testRunningOptionsImplementation.Debug;
    }

    /// <inheritdoc />
    [DataMember]
    public string AssemblyPath { get; set; }

    /// <inheritdoc />
    [DataMember]
    public bool Debug { get; set; }

    /// <inheritdoc/>
    [DataMember]
    public string? TestName { get; set; }
}