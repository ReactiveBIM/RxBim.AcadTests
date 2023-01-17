namespace AcadTestFramework.SDK.Abstractions;

/// <summary>
/// comment
/// </summary>
public interface ITestRunningOptions
{
    /// <summary>
    /// The full path to the assembly containing your tests.
    /// </summary>
    public string AssemblyPath { get; }

    /// <summary>
    /// Debug mode.
    /// </summary>
    public bool Debug { get; }

    /// <summary>
    /// The name of a test to run.
    /// </summary>
    public string? TestName { get; set; }
}