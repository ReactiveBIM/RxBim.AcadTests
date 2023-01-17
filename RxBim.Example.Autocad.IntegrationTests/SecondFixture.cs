using System;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using AutocadTestFrameworkCmd.Helpers;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using FluentAssertions;
using NUnit.Framework;
using RxBim.Di;
using RxBim.Di.Testing.Autocad.Di;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace RxBim.Example.Autocad.IntegrationTests;

[TestFixture]
[TestDrawing("./drawing2.dwg")]
public class SecondFixture
{
    private IContainer _container = null!;

    [OneTimeSetUp]
    public void Init()
    {
        var testingDiConfigurator = new TestingDiConfigurator();
        testingDiConfigurator.Configure(Assembly.GetExecutingAssembly());
        _container = testingDiConfigurator.Container;
        /*OpenDoc("./drawing2.dwg");*/
    }


    [OneTimeTearDown]
    public void Cleanup()
    {
        /* ... */
    }

    [Test]
    public void SecondFixtureFirstTest()
    {
        // получаем текущую БД 
        var db = HostApplicationServices.WorkingDatabase;

        // начинаем транзакцию
        using var tr = db.TransactionManager.StartTransaction();
        // получаем ссылку на пространство модели (ModelSpace)
        var ms = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);

        // "пробегаем" по всем объектам в пространстве модели
        foreach (ObjectId id in ms)
        {
            // приводим каждый из них к типу Entity
            var entity = (Entity)tr.GetObject(id, OpenMode.ForRead);
       
            if (entity is Circle circle)
            {
                circle.Radius.Should().BeApproximately(10,1e-6,"radius of the circle should be 10 mm");
            }}

        tr.Commit();
    }

    [Test]
    public void SecondFixtureSecondTest()
    {
        var acCurDb = _container.GetService<Database>();
        using var acTrans = acCurDb.TransactionManager.StartTransaction();
        var acBlkTbl = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
        var acBlkTblRec = (BlockTableRecord)acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
        var acCircle = new Circle();
        acCircle.SetDatabaseDefaults();
        acCircle.Center = new Point3d();
        const int radius = 5;
        acCircle.Radius = 5;
        acBlkTblRec.AppendEntity(acCircle);
        acTrans.AddNewlyCreatedDBObject(acCircle, true);
        acTrans.Commit();
        acCircle.Area.Should()
            .BeApproximately(Math.PI * radius * radius, 1e-3, "wrong calculation of the area of the circle");

    }

    [Test]
    public void SecondFixtureThirdTest()
    {
    }

    private void OpenDoc(string drawing2Dwg)
    {
        Dispatcher.CurrentDispatcher.Invoke(() =>
        {
            Application.DocumentManager.Open(GetDrawingPath(drawing2Dwg));
        });
    }

    private string? GetDrawingPath(string path)
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