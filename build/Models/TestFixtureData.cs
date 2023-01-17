using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace _build.Models;

/// <summary>Test fixture result.</summary>
public class TestFixtureData
{
    /// <summary>Test cases.</summary>
    public List<TestCaseData> Cases { get; set; } = new();

    /// <summary>Name.</summary>
    public string? Name { get; set; }

    /// <summary>Is fixture success.</summary>
    public bool Success => Cases.All(x => x.Success);

    /// <inheritdoc />
    public override string ToString()
    {
        var str1 = Success ? "✔" : "❌";
        var str2 = string.Join("\n",
            Cases.Select(x => x.ToString()));
        var interpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 3);
        interpolatedStringHandler.AppendFormatted(Name);
        interpolatedStringHandler.AppendLiteral(" - ");
        interpolatedStringHandler.AppendFormatted(str1);
        interpolatedStringHandler.AppendLiteral("\n");
        interpolatedStringHandler.AppendFormatted(str2);
        return interpolatedStringHandler.ToStringAndClear();
    }
}