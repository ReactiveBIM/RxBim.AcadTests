namespace RxBim.Tests.Nuke.Services;

using Models;

/// <summary>
/// Service that checks test results data.
/// </summary>
public class TestResultDataValidationService
{
    /// <summary>
    /// Create a new instance of <see cref="TestResultDataValidationService"/>
    /// </summary>
    public static TestResultDataValidationService Create() => new();

    /// <summary>
    /// Checks if all tests pass.
    /// </summary>
    /// <param name="testResultData"><see cref="TestResultData"/></param>
    public bool AreAllTestsPassed(TestResultData testResultData)
    {
        return testResultData.Fixtures.All(testFixture => testFixture.Cases.All(testCase => testCase.Success));
    }
}