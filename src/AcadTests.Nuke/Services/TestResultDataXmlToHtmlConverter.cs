namespace AcadTests.Nuke.Services;

/// <summary>
/// Xml to html converter for test result data.
/// </summary>
public class TestResultDataXmlToHtmlConverter
{
    /// <summary>
    /// Initialize a new instance of <see cref="TestResultDataXmlToHtmlConverter"/>
    /// </summary>
    public static TestResultDataXmlToHtmlConverter Initialize() => new();

    /// <summary>
    /// Performs conversion of test data from xml to html.
    /// </summary>
    /// <param name="xmlDocumentPath">Xml document path.</param>
    /// <param name="htmlDocumentPath">Path to created html document.</param>
    public async Task Convert(string xmlDocumentPath, string htmlDocumentPath)
    {
        var testResultData = await TestResultDataXmlParserService
            .Initialize()
            .GetTestResultData(xmlDocumentPath);
        var allTestsArePassed = TestResultDataValidationService.Initialize().AreAllTestsPassed(testResultData);
        await TestResultDataHtmlSaverService.Initialize().SaveResultTestData(testResultData, htmlDocumentPath);
        if (!allTestsArePassed)
        {
            throw new Exception("Failed tests found");
        }
    }
}