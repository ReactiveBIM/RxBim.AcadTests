namespace AcadTests.Nuke.Services;

using global::Nuke.Common.ProjectModel;
using global::Nuke.Common.Tooling;
using global::Nuke.Common.Tools.DotNet;

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
    /// <param name="isDebug">Is debug mode</param>
    /// <exception cref="Exception">Exception occurs if at least one test fails.</exception>
    public async Task RunTests(Project project, string testTool, bool isDebug)
    {
        var outputDirectory = _solution.Directory / "testoutput" / project.Name;
        if (Directory.Exists(outputDirectory))
            Directory.Delete(outputDirectory, true);
        DotNetTasks.DotNetBuild(settings => settings
            .SetProjectFile<DotNetBuildSettings>(project)
            .SetConfiguration("Debug")
            .SetOutputDirectory(outputDirectory));
        var assemblyName = project.Name + ".dll";
        var assemblyPath = outputDirectory / assemblyName;
        var xmlResultPath = outputDirectory / "result.xml";
        ProcessTasks
            .StartProcess(testTool, $@"-a {assemblyPath} -r {xmlResultPath} -v 2019{(isDebug ? " -d" : string.Empty)}")
            .WaitForExit();
        var htmlResultPath = outputDirectory / "result.html";

        var testResultData = await TestResultDataXmlParseService
            .Create()
            .GetTestResultData(xmlResultPath);
        var allTestsArePassed =
            TestResultDataValidationService.Create().AreAllTestsPassed(testResultData);
        await TestResultDataHtmlSaveService.Create()
            .SaveResultTestData(testResultData, htmlResultPath);
        if (!allTestsArePassed)
        {
            throw new Exception("Failed tests found");
        }
    }
}