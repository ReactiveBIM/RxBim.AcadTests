namespace Tests.Nuke.Services;

using System.Xml;
using Models;
using Serilog;

/// <summary>
/// Service for obtaining data on test results based on an xml document.
/// </summary>
public class TestResultDataXmlParseService
{
    /// <summary>
    /// Create a new instance of <see cref="TestResultDataXmlParseService"/>
    /// </summary>
    public static TestResultDataXmlParseService Create() => new();

    /// <summary>
    /// Returns test result data based on an test xml document.
    /// </summary>
    /// <param name="resultXmlSourcePath">Tests result xml file path.</param>
    public async Task<TestResultData> GetTestResultData(string resultXmlSourcePath)
    {
        var xmlDocument = await LoadSource(resultXmlSourcePath);
        var testResultData = CreateTestResultData(xmlDocument);
        Log.Information("{Result}", testResultData.ToString());
        return testResultData;
    }

    private async Task<XmlDocument> LoadSource(string resultSourcePath)
    {
        var doc = new XmlDocument();
        doc.LoadXml(await File.ReadAllTextAsync(resultSourcePath));
        return doc;
    }

    private TestResultData CreateTestResultData(XmlDocument document)
    {
        var testResultData = InitTestResultData(document);
        var fixturesData = CreateFixturesData(document);
        testResultData.Fixtures.AddRange(fixturesData);
        return testResultData;
    }

    private TestResultData InitTestResultData(XmlDocument document)
    {
        var mainNodeAttributes = document.SelectSingleNode("/test-suite")?.Attributes;
        if (mainNodeAttributes is null)
        {
            throw new ArgumentException("Result document has wrong structure!");
        }

        var assemblyFullName = mainNodeAttributes["fullname"];
        if (assemblyFullName is null)
        {
            throw new ArgumentException("Result document does not contain assembly path");
        }

        var assemblyFileName = Path.GetFileName(assemblyFullName.Value);
        return new TestResultData(assemblyFileName);
    }

    private IEnumerable<TestFixtureData> CreateFixturesData(XmlNode node)
    {
        var xmlNodeList = node.SelectNodes("//test-suite[@type=\"TestFixture\"]");
        if (xmlNodeList == null)
            yield break;
        foreach (XmlElement xmlElement in xmlNodeList)
        {
            yield return CreateTestFixtureData(xmlElement);
        }
    }

    private TestFixtureData CreateTestFixtureData(XmlElement xmlElement)
    {
        var testFixtureName = xmlElement.Attributes["name"]?.Value;
        var testFixtureData = new TestFixtureData(testFixtureName);
        var testCases = xmlElement.SelectNodes(".//test-case");
        if (testCases == null)
        {
            return testFixtureData;
        }

        foreach (XmlElement testCase in testCases)
        {
            var testCaseData = CreateTestCaseData(testCase);
            testFixtureData.Cases.Add(testCaseData);
        }

        return testFixtureData;
    }

    private TestCaseData CreateTestCaseData(XmlNode testCase)
    {
        var testCaseData = new TestCaseData
        {
            Name = testCase.Attributes?["name"]?.Value,
            Success = testCase.Attributes?["result"]?.Value == "Passed",
            Skipped = testCase.Attributes?["result"]?.Value == "Skipped",
            ExecutionTime = testCase.Attributes?["duration"]?.Value
        };
        if (testCaseData is { Success: true, Skipped: false })
        {
            testCaseData.ResultMessage = "Test was successfully passed";
        }
        else if (testCaseData.Skipped)
        {
            testCaseData.ResultMessage = "Test was skipped";
        }
        else
        {
            testCaseData.ResultMessage = testCase.FirstChild?.FirstChild?.InnerText + testCase.FirstChild?.LastChild?.InnerText;
        }

        return testCaseData;
    }
}