// See https://aka.ms/new-console-template for more information

using AcadTests.Nuke;

var xmlPath = string.Empty;
while (!File.Exists(xmlPath))
{
    Console.WriteLine("Путь к файлу тестов xml");
    xmlPath = Console.ReadLine()?.Trim('\"', '\'', ' ');
}

var resultPath = Path.Combine(Path.GetDirectoryName(xmlPath)!, "result.html");
await new ResultConverter().Convert(xmlPath, resultPath);