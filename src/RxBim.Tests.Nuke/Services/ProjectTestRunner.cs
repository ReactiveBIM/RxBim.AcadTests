namespace RxBim.Tests.Nuke.Services;

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
    /// <param name="project">Project.</param>
    /// <param name="testTool">Path to console Dll.</param>
    /// <param name="isDebug">Is debug mode.</param>
    /// <param name="appVersion">App version.</param>
    /// <param name="configureBuildSettings">Configure build settings.</param>
    /// <exception cref="Exception">Exception occurs if at least one test fails.</exception>
    public async Task<TestResultData> RunTests(
        Project project,
        string testTool,
        bool isDebug,
        int appVersion,
        Func<DotNetBuildSettings, DotNetBuildSettings>? configureBuildSettings = null)
    {
        var outputDirectory = _solution.Directory / "testoutput" / $"{project.Name}_{appVersion}";
        if (Directory.Exists(outputDirectory))
            Directory.Delete(outputDirectory, true);
        DotNetTasks.DotNetBuild(settings =>
        {
            var configuredSettings = settings
                .SetProjectFile<DotNetBuildSettings>(project)
                .SetConfiguration("Debug")
                .SetOutputDirectory(outputDirectory);

            return configureBuildSettings?.Invoke(configuredSettings) ?? configuredSettings;
        });
        var assemblyName = project.Name + ".dll";
        var assemblyPath = outputDirectory / assemblyName;
        var xmlResultPath = outputDirectory / "result.xml";
        var arguments = $@"-a {assemblyPath} -r {xmlResultPath} -v {appVersion}{(isDebug ? " -d" : string.Empty)}";
        ProcessTasks
            .StartProcess(testTool, arguments)
            .WaitForExit();
        var testResultData = await TestResultDataXmlParseService.GetTestResultData(xmlResultPath);

        var htmlResultPath = outputDirectory / "result.html";
        await TestResultDataHtmlSaveService.SaveResultTestData(testResultData, htmlResultPath);

        return testResultData;
    }
}
