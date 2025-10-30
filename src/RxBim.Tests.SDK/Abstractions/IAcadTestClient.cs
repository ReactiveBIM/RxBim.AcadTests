namespace RxBim.Tests.SDK.Abstractions;

/// <summary>
/// Test client.
/// </summary>
public interface IAcadTestClient
{
    /// <summary>
    /// Посылает сообщение
    /// </summary>
    /// <param name="message">сообщение</param>
    void SendMessage(string message);

    /// <summary>
    /// comment
    /// </summary>
    /// <returns></returns>
    Task<ITestRunningOptions> GetTestRunningOptions();

    /// <summary>
    /// comment
    /// </summary>
    /// <param name="result">сообщение</param>
    void SendResult(string result);
}