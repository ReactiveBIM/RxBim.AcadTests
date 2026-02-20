namespace RxBim.Tests.Nuke.Services;

using System.Text;
using global::Nuke.Common.IO;
using global::Nuke.Common.ProjectModel;

/// <summary>
/// Service for working with html test result files.
/// </summary>
public class TestResultFilesService(Solution solution)
{
    /// <summary>
    /// Delete test results (test output directory).
    /// </summary>
    public void DeleteTestResults()
    {
        var testOutputDirectory = solution.Directory / "testoutput";
        if (Directory.Exists(testOutputDirectory))
            Directory.Delete(testOutputDirectory, recursive: true);
    }

    /// <summary>
    /// Merges html test result files.
    /// </summary>
    public async Task MergeTestResultsAsync()
    {
        var testOutputDirectory = solution.Directory / "testoutput";
        if (!Directory.Exists(testOutputDirectory))
            return;

        var versionTestResultsDict = GetVersionTestResultsDict(testOutputDirectory);
        if (versionTestResultsDict.Count == 0)
            return;

        var mergedHtml = new StringBuilder();
        foreach (var versionTestResults in versionTestResultsDict.OrderBy(v => v.Key))
        {
            // В едином тестовом файле тесты разделяются по версиям Revit с помощью разделителя:
            var versionSplitter = $"<h2 style=\"background-color: yellow;\">Результаты тестов для Revit {versionTestResults.Key}</h2>";
            mergedHtml.Append(versionSplitter);

            foreach (var testResult in versionTestResults.Value)
            {
                var fileContent = await File.ReadAllTextAsync(testResult);
                mergedHtml.Append(fileContent);
            }
        }

        var mergedHtmlString = mergedHtml.ToString();
        var testFileName = "SummaryTestResult.html";
        var filePath = testOutputDirectory / testFileName;
        await File.WriteAllTextAsync(filePath, mergedHtmlString);
    }

    private Dictionary<int, List<string>> GetVersionTestResultsDict(AbsolutePath testOutputDirectory)
    {
        var testDirectories = new DirectoryInfo(testOutputDirectory)
            .GetDirectories();

        var versionTestResultsDict = new Dictionary<int, List<string>>();
        foreach (var testDirectory in testDirectories)
        {
            var directory = testDirectory.Name;
            if (!directory.Contains("IntegrationTests"))
                continue;

            var version = directory.Split("_").LastOrDefault();
            if (version is null || !int.TryParse(version, out var versionNumber))
                continue;

            var testResult = testOutputDirectory / directory / "result.html";
            if (!File.Exists(testResult))
                continue;

            if (versionTestResultsDict.TryGetValue(versionNumber, out var testResults))
            {
                testResults.Add(testResult);
            }
            else
            {
                versionTestResultsDict.Add(versionNumber, [testResult]);
            }
        }

        return versionTestResultsDict;
    }
}
