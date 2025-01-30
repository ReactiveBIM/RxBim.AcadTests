namespace AcadTests.SDK;

/// <summary>
///     AcadTestSdk
/// </summary>
public class AcadTestSdk
{
    private const string PipeName = "testpipe";

    /// <summary>
    ///     Client
    /// </summary>
    public AcadTestClient AcadTestClient { get; } = new(PipeName);

    /// <summary>
    ///     Server
    /// </summary>
    public AcadTestServer AcadTestServer { get; } = new(PipeName);
}