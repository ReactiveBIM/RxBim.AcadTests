namespace RevitTests.Cmd;

using Autodesk.Revit.UI;

/// <summary>
/// The revit context
/// </summary>
public static class RevitContext
{
    /// <summary>
    /// UI app
    /// </summary>
    public static UIApplication? UiApplication { get; internal set; }
}