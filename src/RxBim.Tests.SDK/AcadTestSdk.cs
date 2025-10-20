namespace RxBim.Tests.SDK;

using Abstractions;

/// <summary>
///     AcadTestSdk
/// </summary>
public class AcadTestSdk
{
    private const string PipeName = "testpipe";

    /// <summary>
    ///     Client
    /// </summary>
    public IAcadTestClient AcadTestClient { get; } = new AcadTestClient(PipeName);

    /// <summary>
    ///     Server
    /// </summary>
    public AcadTestServer AcadTestServer { get; } = new(PipeName);
}