namespace Tests.Nuke.Models;

/// <summary>
/// Test result data.
/// </summary>
public class TestResultData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestResultData"/> class.
    /// </summary>
    /// <param name="assemblyFileName">Test assembly file name.</param>
    public TestResultData(string assemblyFileName)
    {
        AssemblyFileName = assemblyFileName;
    }

    /// <summary>
    /// Test fixtures.
    /// </summary>
    public List<TestFixtureData> Fixtures { get; } = new();

    /// <summary>
    /// Name of test assembly.
    /// </summary>
    public string AssemblyFileName { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return AssemblyFileName
               + "\n"
               + string.Join("\n", Fixtures.Select(x => x.ToString()));
    }
}