namespace RxBim.Tests.Nuke.Components;

using System.ComponentModel;
using global::Nuke.Common.Tooling;

/// <inheritdoc />
[TypeConverter(typeof(TypeConverter<TestTool>))]
public class TestTool : Enumeration
{
    /// <summary>
    /// Autocad
    /// </summary>
    public static readonly TestTool Acad;

    /// <summary>
    /// Revit
    /// </summary>
    public static readonly TestTool Revit;

    static TestTool()
    {
        Acad = new TestTool() { Value = "RxBim.AutocadTests.Console" };
        Revit = new TestTool() { Value = "RxBim.RevitTests.Console" };
    }

    /// <summary>Implicit to string cast operator.</summary>
    /// <param name="profile">The profile.</param>
    public static implicit operator string(TestTool profile) => profile.Value;
}