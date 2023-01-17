using System.Collections.Generic;
using System.Linq;

namespace _build.Models;

public class TestResultData
{
    /// <summary>Test fixtures.</summary>
    public List<TestFixtureData> Fixtures { get; set; } = new List<TestFixtureData>();

    /// <summary>Name of test assembly.</summary>
    public string? AssemblyName { get; set; }

    /// <inheritdoc />
    public override string ToString() => AssemblyName + "\n" + string.Join("\n",
        Fixtures.Select(x => x.ToString()));
}