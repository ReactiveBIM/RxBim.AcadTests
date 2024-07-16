using Autodesk.AutoCAD.ApplicationServices.Core;

namespace RxBim.Example.Autocad.IntegrationTests;

using System;
using System.Reflection;
using AcadTests.TestingUtils.Di;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Di;
using FluentAssertions;
using NUnit.Framework;

/// <summary>
///     Примеры тестов.
/// </summary>
[TestFixture]
public class SimpleTests
{
    /// <summary>
    ///     Настройка тестов.
    /// </summary>
    [OneTimeSetUp]
    public void Setup()
    {
        var testingDiConfigurator = new TestingDiConfigurator();
        testingDiConfigurator.Configure(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    ///     Пустой тест.
    /// </summary>
    [Test]
    public void FirstTest()
    {
    }

    /// <summary>
    ///     Тест с ошибкой. Для проверки отчета.
    /// </summary>
    [Test()]
    [Ignore("для успешного завершения")]
    public void FailureTest()
    {
        throw new Exception(nameof(FailureTest));
    }

    /// <summary>
    ///     Тест вывода сообщений из консоли в отчет.
    /// </summary>
    [Test]
    public void ConsoleTest()
    {
        // TODO текст не отображается в результатах. См. ResultConverter.cs
        Console.WriteLine("Any text");
    }

    /// <summary>
    ///     Тест работы с автокадом.
    /// </summary>
    [Test]
    public void DrawNewCircleTest()
    {
        using var lockDoc = Application.DocumentManager.MdiActiveDocument.LockDocument();
        var acCurDb = Application.DocumentManager.MdiActiveDocument.Database;
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