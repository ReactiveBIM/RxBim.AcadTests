namespace AcadTests.Nuke.Models;

using System.Runtime.CompilerServices;

/// <summary> Test case data. </summary>
public class TestCaseData
{
    /// <summary>Name.</summary>
    public string? Name { get; set; }

    /// <summary>Is case success.</summary>
    public bool Success { get; set; }

    /// <summary>Execution time.</summary>
    public string? ExecutionTime { get; set; }

    /// <summary>Failure message.</summary>
    public string? Failure { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var str1 = Success ? "✔" : "❌";
        var str2 = Success ? string.Empty : "\n\t" + Failure;
        var interpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 4);
        interpolatedStringHandler.AppendFormatted(Name);
        interpolatedStringHandler.AppendLiteral(" - ");
        interpolatedStringHandler.AppendFormatted(str1);
        interpolatedStringHandler.AppendLiteral(" - ");
        interpolatedStringHandler.AppendFormatted(ExecutionTime);
        interpolatedStringHandler.AppendFormatted(str2);
        return interpolatedStringHandler.ToStringAndClear();
    }
}