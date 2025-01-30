namespace RxBim.Example.Autocad.IntegrationTests;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AutocadTests.TestingUtils.Di;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

/// <summary>
/// Тесты.
/// </summary>
[TestFixture]
public class Drawing1Tests
{
    private IServiceProvider _container = null!;

    /// <summary>
    /// Настройка тестов
    /// </summary>
    [OneTimeSetUp]
    public void Init()
    {
        var testingDiConfigurator = new TestingDiConfigurator();
        testingDiConfigurator.Configure(Assembly.GetExecutingAssembly());
        _container = testingDiConfigurator.Build();

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