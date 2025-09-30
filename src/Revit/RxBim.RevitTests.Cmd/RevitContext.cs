namespace RxBim.RevitTests.Cmd;

using Autodesk.Revit.UI;

/// <summary>
/// The revit context
/// </summary>
/// <remarks>
/// This class should be in this project! If you move it to another project, Revit crashes with a fatal error.
/// </remarks>
public static class RevitContext
{
    /// <summary>
    /// UI app
    /// </summary>
    public static UIApplication? UiApplication { get; internal set; }
}