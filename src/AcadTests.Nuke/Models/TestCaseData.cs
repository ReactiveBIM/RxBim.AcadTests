namespace AcadTests.Nuke.Models;

using System.Runtime.CompilerServices;

/// <summary>
/// Test case data.
/// </summary>
public class TestCaseData
{
    private readonly bool _success;

    /// <summary>
    /// Name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Is case success.
    /// </summary>
    public bool Success
    {
        get => Skipped || _success;
        init => _success = value;
    }

    /// <summary>
    /// Is case skipped.
    /// </summary>
    public bool Skipped { get; init; }

    /// <summary>
    /// Execution time.
    /// </summary>
    public string? ExecutionTime { get; init; }

    /// <summary>
    /// Result message.
    /// </summary>
    public string? ResultMessage { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var str1 = Success ? Skipped ? "❔" : "✔" : "❌";
        var str2 = Success ? ResultMessage : "\n\t" + ResultMessage;
        var interpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 4);
        interpolatedStringHandler.AppendFormatted(Name);
        interpolatedStringHandler.AppendLiteral(" - ");
        interpolatedStringHandler.AppendFormatted(str1);
        interpolatedStringHandler.AppendLiteral(" - ");
        interpolatedStringHandler.AppendFormatted(ExecutionTime);
        interpolatedStringHandler.AppendLiteral(" - ");
        interpolatedStringHandler.AppendFormatted(str2);
        return interpolatedStringHandler.ToStringAndClear();
    }
}