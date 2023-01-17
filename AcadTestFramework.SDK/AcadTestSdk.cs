namespace AcadTestFramework.SDK;

/// <summary>
/// asdas
/// </summary>
public class AcadTestSdk
{
    private const string PipeName = "testpipe";

    /// <summary>
    /// asdasd
    /// </summary>
    public AcadTestClient AcadTestClient { get; } = new AcadTestClient(PipeName);

    /// <summary>
    /// asdasd
    /// </summary>
    public AcadTestServer AcadTestServer { get; } = new AcadTestServer(PipeName);
}