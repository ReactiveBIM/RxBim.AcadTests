// See https://aka.ms/new-console-template for more information

using Tests.Nuke.Services;

var xmlPath = string.Empty;
while (!File.Exists(xmlPath))
{
    Console.WriteLine("Путь к файлу тестов xml");
    xmlPath = Console.ReadLine()?.Trim('\"', '\'', ' ');
}

var resultPath = Path.Combine(Path.GetDirectoryName(xmlPath)!, "result.html");
var testResultData = await TestResultDataXmlParseService
    .Create()
    .GetTestResultData(xmlPath);
var allTestsArePassed =
    TestResultDataValidationService.Create().AreAllTestsPassed(testResultData);
await TestResultDataHtmlSaveService.Create().SaveResultTestData(testResultData, resultPath);
if (!allTestsArePassed)
{
    throw new Exception("Failed tests found");
}