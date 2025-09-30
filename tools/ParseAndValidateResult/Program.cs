using RxBim.Tests.Nuke.Services;

var xmlPath = string.Empty;
while (!File.Exists(xmlPath))
{
    Console.WriteLine("Путь к файлу тестов xml");
    xmlPath = Console.ReadLine()?.Trim('\"', '\'', ' ');
}

var resultPath = Path.Combine(Path.GetDirectoryName(xmlPath)!, "result.html");
var testResultData = await TestResultDataXmlParseService.GetTestResultData(xmlPath);
await TestResultDataHtmlSaveService.SaveResultTestData(testResultData, resultPath);
TestResultDataValidationService.ThrowIfNotAllTestsPassed(testResultData);