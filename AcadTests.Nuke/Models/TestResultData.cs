namespace AcadTests.Nuke.Models;

/// <summary>Test result data.</summary>
public class TestResultData
{
    /// <summary>Test fixtures.</summary>
    public List<TestFixtureData> Fixtures { get; set; } = new();

    /// <summary>Name of test assembly.</summary>
    public string? AssemblyName { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return AssemblyName + "\n" + string.Join("\n",
            Fixtures.Select(x => x.ToString()));
    }
}