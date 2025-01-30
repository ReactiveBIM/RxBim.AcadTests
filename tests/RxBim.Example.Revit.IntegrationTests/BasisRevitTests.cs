namespace RxBim.Example.Revit.IntegrationTests;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RevitTests.TestingUtils;

/// <summary>
/// Revit tests.
/// </summary>
[TestFixture]
public class BasisRevitTests
{
    private Document _document;
    private UIApplication _uiApplication;

    /// <summary>
    /// Setup.
    /// </summary>
    [OneTimeSetUp]
    public void Setup()
    {
        var configurator = new TestingDiConfigurator();
        configurator.Configure(GetType().Assembly);
        var container = configurator.Build();

        _uiApplication = container.GetService<UIApplication>();
        var modelPath = Path.Combine(Environment.CurrentDirectory, "rac_basic_sample_project.rvt");
        _uiApplication.OpenAndActivateDocument(modelPath);
        _document = container.GetService<Document>();
    }

    /// <summary>
    /// Finalize
    /// </summary>
    [OneTimeTearDown]
    public void Cleanup()
    {
    }

    /// <summary>
    /// Read properties of element.
    /// </summary>
    [Test]
    public void ReadPropertiesTest()
    {
#if !RVT2024 && !RVT2025
        var sampleChair = _document.GetElement(new ElementId(990317)) as FamilyInstance;
#else
        var sampleChair = _document.GetElement(new ElementId((long)990317)) as FamilyInstance;
#endif
        var lp = sampleChair?.Location as LocationPoint;
        lp.Should().NotBeNull();
        lp!.Point.Z.Should().BeApproximately(0, 1e-6);
    }

    /// <summary>
    /// Read properties of element.
    /// </summary>
    [Test]
    public void CreateTransactionTest()
    {
        using var tr = new Transaction(_document, "TestTransaction");
        tr.Start();
        tr.Commit().Should().Be(TransactionStatus.Committed);
    }

    /// <summary>
    /// Run FilteredElementsCollector.
    /// </summary>
    [Test]
    public void FilteredElementsCollectorTest()
    {
        new FilteredElementCollector(_document)
            .OfCategory(BuiltInCategory.OST_Walls)
            .WhereElementIsNotElementType()
            .ToElements()
            .Count.Should().Be(56);
    }


    /// <summary>
    /// Running a long test
    /// </summary>
    [Test]
    [Ignore("too long")]
    public void LongTest()
    {
        var timer = new Stopwatch();
        timer.Start();
        while (timer.ElapsedMilliseconds < 15 * 60 * 1000)
        {
            var wall = new FilteredElementCollector(_document)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .ToElements()
                .First();
            var tr = new Transaction(_document);
            tr.Start("set mark");
            wall.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(timer.ElapsedMilliseconds.ToString());
            tr.Commit();
        }
    }

    /// <summary>
    /// Create Family Instance Test.
    /// </summary>
    [Test]
    public void CreateFamilyInstanceTest()
    {
        using var tr = new Transaction(_document, "CreateChain");
#if !RVT2024 && !RVT2025
        var fs = _document.GetElement(new ElementId(990191)) as FamilySymbol;
#else
        var fs = _document.GetElement(new ElementId((long)990191)) as FamilySymbol;
#endif
        tr.Start();
        var newChain = _document.Create.NewFamilyInstance(
            new XYZ(-8.93862219885585, 6.15996740717654, 0),
            fs,
            null,
            StructuralType.NonStructural);
        tr.Commit().Should().Be(TransactionStatus.Committed);
        _document.GetElement(newChain.Id).Should().NotBeNull();
    }
}