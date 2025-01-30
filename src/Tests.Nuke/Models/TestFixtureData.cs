namespace Tests.Nuke.Models;

using System.Runtime.CompilerServices;

/// <summary>
/// Test fixture data.
/// </summary>
public class TestFixtureData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestFixtureData"/> class.
    /// </summary>
    /// <param name="name">Name of test fixture.</param>
    public TestFixtureData(string? name)
    {
        Name = name;
    }

    /// <summary>
    /// Test cases.
    /// </summary>
    public List<TestCaseData> Cases { get; } = new();

    /// <summary>
    /// Name.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Is fixture success.
    /// </summary>
    public bool Success => Cases.All(testCase => testCase.Success);

    /// <inheritdoc />
    public override string ToString()
    {
        var str1 = Success ? "✔" : "❌";
        var str2 = string.Join("\n", Cases.Select(x => x.ToString()));
        var interpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
        interpolatedStringHandler.AppendFormatted(Name);
        interpolatedStringHandler.AppendLiteral(" - ");
        interpolatedStringHandler.AppendFormatted(str1);
        interpolatedStringHandler.AppendLiteral("\n");
        interpolatedStringHandler.AppendFormatted(str2);
        return interpolatedStringHandler.ToStringAndClear();
    }
}