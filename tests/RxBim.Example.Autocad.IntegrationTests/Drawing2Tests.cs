using System.IO;
using System.Linq;
using System.Reflection;
using AcadTests.TestingUtils.Di;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using FluentAssertions;
using NUnit.Framework;
using RxBim.Di;

namespace RxBim.Example.Autocad.IntegrationTests;

[TestFixture]
public class Drawing1Tests
{
    private IContainer _container = null!;

    /// <summary>
    /// Настройка тестов
    /// </summary>
    [OneTimeSetUp]
    public void Init()
    {
        var testingDiConfigurator = new TestingDiConfigurator();
        testingDiConfigurator.Configure(Assembly.GetExecutingAssembly());
        _container = testingDiConfigurator.Container;

        var docName = "drawing2.dwg";
        var docManager = _container.GetService<DocumentCollection>();
        var drawingPath = GetDrawingPath(docName);
        var openedDoc = docManager.Open(drawingPath, false);
        new FileInfo(openedDoc.Name).Name.Should().Be(docName);
        docManager.CurrentDocument = openedDoc;
        docManager.MdiActiveDocument = openedDoc;
    }

    /// <summary>
    /// Тест на открытие другого документа
    /// </summary>
    [Test]
    [NonParallelizable]
    public void CirclesCountMustBeSevenTest()
    {
        // получаем текущую БД 
        var db = HostApplicationServices.WorkingDatabase;

        // начинаем транзакцию
        using var tr = db.TransactionManager.StartTransaction();
        // получаем ссылку на пространство модели (ModelSpace)
        var ms = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);

        ms.Cast<ObjectId>()
            .Select(id => (Entity)tr.GetObject(id, OpenMode.ForRead))
            .OfType<Circle>()
            .Count()
            .Should().Be(7);
        tr.Commit();
    }

    private string GetDrawingPath(string path)
    {
        string absolutePath;

        if (Path.IsPathRooted(path))
            absolutePath = path;
        else
            absolutePath = Path.GetFullPath(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                    path));

        return absolutePath;
    }
}