namespace RxBim.Tests.Nuke.Services;

using System.Text;
using global::Nuke.Common.IO;
using global::Nuke.Common.ProjectModel;
using global::Nuke.Common.Tooling;
using global::Nuke.Common.Tools.DotNet;
using Models;

/// <summary>
/// Project test runner.
/// </summary>
public class ProjectTestRunner
{
    private readonly Solution _solution;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectTestRunner"/> class.
    /// </summary>
    /// <param name="solution"><see cref="Solution"/>.</param>
    public ProjectTestRunner(Solution solution)
    {
        _solution = solution;
    }

    /// <summary>
    /// Run project tests.
    /// </summary>
    /// <param name="projects">Collection of <see cref="Project"/>.</param>
    /// <param name="testTool">Path to console Dll.</param>
    /// <param name="isDebug">Is debug mode.</param>
    /// <param name="appVersions">Collection of app version.</param>
    /// <param name="configureBuildSettings">Configure build settings.</param>
    /// <exception cref="Exception">Exception occurs if at least one test fails.</exception>
    public async Task RunTests(
        Project[] projects,
        string testTool,
        bool isDebug,
        int[] appVersions,
        Func<DotNetBuildSettings, int, DotNetBuildSettings>? configureBuildSettings = null)
    {
        {
            var outputDirectory = _solution.Directory / "testoutput";
            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, true);

            var versionTestResults = new Dictionary<int, List<string>>();
            var testResultsData = new List<TestResultData>();
            foreach (var version in appVersions)
            {
                foreach (var project in projects)
                {
                    var testResult = await RunTest(
                        outputDirectory,
                        project,
                        testTool,
                        isDebug,
                        version,
                        configureBuildSettings);

                    testResultsData.Add(testResult.TestResultData);

                    if (versionTestResults.TryGetValue(version, out var testResults))
                        testResults.Add(testResult.HtmlResultPath);
                    else
                        versionTestResults.Add(version, new List<string> { testResult.HtmlResultPath });
                }
            }

            if (versionTestResults.Count > 0)
            {
                await MergeTestFilesAsync(versionTestResults, outputDirectory);
                CheckAllTestResults(testResultsData);
            }
        }
    }

    private static async Task<(string HtmlResultPath, TestResultData TestResultData)> RunTest(
        AbsolutePath outputDirectory,
        Project project,
        string testTool,
        bool isDebug,
        int appVersion,
        Func<DotNetBuildSettings, int, DotNetBuildSettings>? configureBuildSettings = null)
    {
        DotNetTasks.DotNetBuild(settings =>
        {
            var configuredSettings = settings
                .SetProjectFile<DotNetBuildSettings>(project)
                .SetConfiguration("Debug")
                .SetOutputDirectory(outputDirectory);

            return configureBuildSettings?.Invoke(configuredSettings, appVersion) ?? configuredSettings;
        });

        var assemblyName = project.Name + ".dll";
        var assemblyPath = outputDirectory / assemblyName;
        var testResultName = $"{project.Name}_{appVersion}_result";
        var xmlResultPath = outputDirectory / $"{testResultName}.xml";
        var arguments = $"-a {assemblyPath} -r {xmlResultPath} -v {appVersion}{(isDebug ? " -d" : string.Empty)}";

        ProcessTasks
            .StartProcess(testTool, arguments)
            .WaitForExit();

        var testResultData = await TestResultDataXmlParseService.GetTestResultData(xmlResultPath);
        var htmlResultPath = outputDirectory / $"{testResultName}.html";
        await TestResultDataHtmlSaveService.SaveResultTestData(testResultData, htmlResultPath);

        return (htmlResultPath, testResultData);
    }

    // Собирает единый тестовый html файл из тестовых фалов проектов.
    private static async Task MergeTestFilesAsync(
        Dictionary<int, List<string>> versionTestResults,
        AbsolutePath outputDirectory)
    {
        var mergedHtml = new StringBuilder();
        foreach (var versionTestResult in versionTestResults)
        {
            // В едином тестовом файле тесты разделяются по версиям Revit с помощью разделителя:
            var versionSplitter = $"<h2 style=\"background-color: yellow;\">Результаты тестов для Revit {versionTestResult.Key}</h2>";
            mergedHtml.Append(versionSplitter);

            foreach (var testResult in versionTestResult.Value)
            {
                var fileContent = await File.ReadAllTextAsync(testResult);
                mergedHtml.Append(fileContent);
            }
        }

        var mergedHtmlString = mergedHtml.ToString();
        var testFileName = "SummaryResult.html";
        var filePath = outputDirectory / testFileName;
        await File.WriteAllTextAsync(filePath, mergedHtmlString);

        foreach (var testResult in versionTestResults.SelectMany(v => v.Value))
        {
            File.Delete(testResult);
        }
    }

    // Проверяет все результаты тестов, если какой-нибудь тест не пройден - вызывает исключение.
    private static void CheckAllTestResults(List<TestResultData> testResultsData)
    {
        foreach (var testResultData in testResultsData)
            TestResultDataValidationService.ThrowIfNotAllTestsPassed(testResultData);
    }
}
