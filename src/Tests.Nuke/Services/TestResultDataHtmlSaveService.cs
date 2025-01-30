namespace Tests.Nuke.Services;

using System.Reflection;
using Models;
using RazorLight;
using Serilog;

/// <summary>
/// Service for saving test results data in html format.
/// </summary>
public class TestResultDataHtmlSaveService
{
    /// <summary>
    /// Create a new instance of <see cref="TestResultDataHtmlSaveService"/>
    /// </summary>
    public static TestResultDataHtmlSaveService Create() => new();

    /// <summary>
    /// Saves test results data to an html document.
    /// </summary>
    /// <param name="testResultData"><see cref="TestResultData"/></param>
    /// <param name="htmlDocumentPath">Path to created html document.</param>
    public async Task SaveResultTestData(TestResultData testResultData, string htmlDocumentPath)
    {
        var renderedHtml = await RenderHtml(testResultData);
        await SaveHtmlDocument(htmlDocumentPath, renderedHtml);
        Log.Information("Test results has been saved into {ResultPath}", htmlDocumentPath);
    }

    private async Task<string> RenderHtml(TestResultData testResultData)
    {
        var engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(Assembly.GetExecutingAssembly())
            .UseMemoryCachingProvider()
            .UseOptions(new RazorLightOptions
            {
                DisableEncoding = true
            })
            .Build();

        return await engine.CompileRenderAsync(typeof(Result).FullName, testResultData);
    }

    private async Task SaveHtmlDocument(string resultPath, string htmlDocumentPath)
    {
        await File.WriteAllTextAsync(resultPath, htmlDocumentPath);
    }
}