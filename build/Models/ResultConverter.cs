using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using _build.Models;
using RazorLight;
using RxBim.Shared;
using Serilog;
using Result = _build.Models.Result;

namespace _build;

/// <summary>Converts RTF xml result into html.</summary>
public class ResultConverter
{
    /// <summary>Converts RTF xml result into html.</summary>
    /// <param name="resultSourcePath">RTF result file path.</param>
    /// <param name="resultPath">converted result path.</param>
    public async Task Convert(string resultSourcePath, string resultPath)
    {
        var testResultData = CreateTestResultData(await LoadSource(resultSourcePath));
        Log.Information("{Result}", testResultData.ToString());
        var result = await RenderResult(testResultData);
        await SaveResult(resultPath, result);
        Log.Information("Test results has been saved into {ResultPath}", resultPath);
    }

    TestResultData CreateTestResultData(XmlDocument doc)
    {
        var testResultData = InitTestResultData(doc);
        testResultData.Fixtures.AddRange(CreateFixturesData(doc));
        return testResultData;
    }

    static TestResultData InitTestResultData(XmlNode doc)
    {
        var fileName = Path.GetFileName((doc.SelectSingleNode("/test-suite")?.Attributes ??
                                         throw new ArgumentException("Result document has wrong structure!"))["fullname"]
            ?.Value);
        return new TestResultData()
        {
            AssemblyName = fileName
        };
    }

    IEnumerable<TestFixtureData> CreateFixturesData(
        XmlNode node)
    {
        var xmlNodeList = node.SelectNodes("//test-suite[@type=\"TestFixture\"]");
        if (xmlNodeList == null) yield break;
        foreach (XmlElement xmlElement in xmlNodeList)
        {
            var testFixtureData = new TestFixtureData()
            {
                Name = xmlElement.Attributes["name"]?.Value
            };
            var childNodes = xmlElement.SelectNodes("./test-case");
            if (childNodes != null)
            {
                foreach (XmlElement @case in childNodes)
                {
                    var testCaseData = CreateTestCaseData(@case);
                    testFixtureData.Cases.Add(testCaseData);
                }
            }

            yield return testFixtureData;
        }
    }

    TestCaseData CreateTestCaseData(XmlNode @case)
    {
        var testCaseData = new TestCaseData()
        {
            Name = @case.Attributes?["name"]?.Value,
            Success = @case.Attributes?["result"]?.Value == "Passed",
            ExecutionTime = @case.Attributes?["duration"]?.Value
        };
        testCaseData.Failure = testCaseData.Success
            ? "-"
            : @case.FirstChild?.FirstChild?.InnerText + @case.FirstChild?.LastChild?.InnerText;
        return testCaseData;
    }

    async Task<XmlDocument> LoadSource(string resultSourcePath)
    {
        var doc = new XmlDocument();
        var xmlDocument = doc;
        xmlDocument.LoadXml(await File.ReadAllTextAsync(resultSourcePath));
        var xmlDocument1 = doc;
        return xmlDocument1;
    }

    private async Task<string> RenderResult(TestResultData testResultData)
    {
        var engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(Assembly.GetExecutingAssembly())
            .UseMemoryCachingProvider()
            .UseOptions(new RazorLightOptions
            {
                DisableEncoding = true
            })
            .Build();

        var result = await engine.CompileRenderAsync(typeof(Result).FullName, testResultData);
        return result;
    }

    async Task SaveResult(string resultPath, string result) => await File.WriteAllTextAsync(resultPath, result);
}