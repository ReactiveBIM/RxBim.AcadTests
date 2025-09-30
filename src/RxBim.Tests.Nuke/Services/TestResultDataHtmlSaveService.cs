namespace RxBim.Tests.Nuke.Services;

using System.Reflection;
using Models;
using RazorLight;
using Serilog;

/// <summary>
/// Service for saving test results data in html format.
/// </summary>
public static class TestResultDataHtmlSaveService
{
    /// <summary>
    /// Saves test results data to an html document.
    /// </summary>
    /// <param name="testResultData"><see cref="TestResultData"/></param>
    /// <param name="htmlDocumentPath">Path to created html document.</param>
    public static async Task SaveResultTestData(TestResultData testResultData, string htmlDocumentPath)
    {
        var renderedHtml = await RenderHtml(testResultData);
        await SaveHtmlDocument(htmlDocumentPath, renderedHtml);
        Log.Information("Test results has been saved into {ResultPath}", htmlDocumentPath);
    }

    private static async Task<string> RenderHtml(TestResultData testResultData)
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

    private static async Task SaveHtmlDocument(string resultPath, string htmlDocumentPath)
    {
        await File.WriteAllTextAsync(resultPath, htmlDocumentPath);
    }
}