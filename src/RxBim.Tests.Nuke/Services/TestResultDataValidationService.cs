namespace RxBim.Tests.Nuke.Services;

using Models;

/// <summary>
/// Service that checks test results data.
/// </summary>
public static class TestResultDataValidationService
{
    /// <summary>
    /// Throws exception if not all tests pass.
    /// </summary>
    /// <param name="testResultData"><see cref="TestResultData"/></param>
    public static void ThrowIfNotAllTestsPassed(TestResultData testResultData)
    {
        var allTestsArePassed =
            testResultData.Fixtures.All(testFixture => testFixture.Cases.All(testCase => testCase.Success));
        if (!allTestsArePassed)
            throw new Exception("Failed tests found");
    }
}