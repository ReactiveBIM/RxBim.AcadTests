namespace RxBim.Example.Autocad.IntegrationTests;

using System;
using System.Reflection;
using AutocadTestFrameworkCmd.Helpers;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Di;
using Di.Testing.Autocad.Di;
using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Примеры тестов.
/// </summary>
[TestFixture]
// TODO обсудить
[TestDrawing("./drawing1.dwg")]
public class SimpleTests
{
    private IContainer _container = null!;

    /// <summary>
    /// Настройка тестов.
    /// </summary>
    [OneTimeSetUp]
    public void Setup()
    {
        var testingDiConfigurator = new TestingDiConfigurator();
        testingDiConfigurator.Configure(Assembly.GetExecutingAssembly());
        _container = testingDiConfigurator.Container;
    }

    /// <summary>
    /// Пустой тест.
    /// </summary>
    [Test]
    public void FirstTest()
    {
    }

    /// <summary>
    /// Тест с ошибкой. Для проверки отчета.
    /// </summary>
    [Test]
    public void FailureTest()
    {
        throw new Exception(nameof(FailureTest));
    }

    /// <summary>
    /// Тест вывода сообщений из консоли в отчет.
    /// </summary>
    [Test]
    public void ConsoleTest()
    {
        // TODO текст не отображается в результатах. См. ResultConverter.cs
        Console.WriteLine("Any text");
    }

    /// <summary>
    /// Тест работы с автокадом.
    /// </summary>
    [Test]
    public void DrawNewCircleTest()
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
}