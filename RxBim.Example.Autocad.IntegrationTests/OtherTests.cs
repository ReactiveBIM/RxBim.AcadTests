using System.IO;
using System.Reflection;
using AutocadTestFrameworkCmd.Helpers;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using FluentAssertions;
using NUnit.Framework;
using RxBim.Di;
using RxBim.Di.Testing.Autocad.Di;

namespace RxBim.Example.Autocad.IntegrationTests;

/// <summary>
/// Тесты
/// </summary>
[TestFixture]
[TestDrawing("./drawing2.dwg")]
public class OtherTests
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
    }


    /// <summary>
    /// <see cref="OneTimeTearDownAttribute"/>
    /// </summary>
    [OneTimeTearDown]
    public void Cleanup()
    {
        /* ... */
    }

    /// <summary>
    /// Тест на получение объектов из чертежа.
    /// </summary>
    [Test]
    public void GetObjectFromAcadTest()
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
            {
                circle.Radius.Should().BeApproximately(10, 1e-6, "radius of the circle should be 10 mm");
            }
        }

        tr.Commit();
    }

    /// <summary>
    /// Тест на открытие другого документа 
    /// </summary>
    [Test]
    [NonParallelizable]
    public void OpenOtherDocTest()
    {
        var docName = "drawing1.dwg";
        var docManager = _container.GetService<DocumentCollection>();
        var drawingPath = GetDrawingPath(docName);
        var openedDoc = docManager.Open(drawingPath, false);
        new FileInfo(openedDoc.Name).Name.Should().Be(docName);
    }

    private string GetDrawingPath(string path)
    {
        string absolutePath;

        if (Path.IsPathRooted(path))
        {
            absolutePath = path;
        }
        else
        {
            absolutePath = Path.GetFullPath(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                    path));
        }

        return absolutePath;
    }
}