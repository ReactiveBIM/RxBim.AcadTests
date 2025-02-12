namespace RxBim.Example.Autocad.IntegrationTests;

using System;
using System.IO;
using System.Reflection;
using AutocadTests.TestingUtils.Di;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

/// <summary>
///     Тесты
/// </summary>
[TestFixture]
public class OtherTests
{
    /// <summary>
    ///     Настройка тестов
    /// </summary>
    [OneTimeSetUp]
    public void Init()
    {
        var testingDiConfigurator = new TestingDiConfigurator();
        testingDiConfigurator.Configure(Assembly.GetExecutingAssembly());
        _container = testingDiConfigurator.Build();
        var docName = "drawing1.dwg";
        var docManager = _container.GetService<DocumentCollection>();
        var drawingPath = GetDrawingPath(docName);
        var openedDoc = docManager.Open(drawingPath, false);
        docManager.CurrentDocument = openedDoc;
        docManager.MdiActiveDocument = openedDoc;
    }


    /// <summary>
    /// <see cref="OneTimeTearDownAttribute" />
    /// </summary>
    [OneTimeTearDown]
    public void Cleanup()
    {
        
    }

    private IServiceProvider _container = null!;

    /// <summary>
    /// Тест на получение объектов из чертежа.
    /// </summary>
    [Test]
    public void CheckCirclesRadius()
    {
        // получаем текущую БД
        var db = HostApplicationServices.WorkingDatabase;

        // начинаем транзакцию
        using var tr = db.TransactionManager.StartTransaction();
        // получаем ссылку на пространство модели (ModelSpace)
        var ms = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);

        // "пробегаем" по всем объектам в пространстве модели
        foreach (var id in ms)
        {
            // приводим каждый из них к типу Entity
            var entity = (Entity)tr.GetObject(id, OpenMode.ForRead);

            if (entity is Circle circle)
                circle.Radius.Should().BeApproximately(10, 1e-6, "radius of the circle should be 10 mm");
        }

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