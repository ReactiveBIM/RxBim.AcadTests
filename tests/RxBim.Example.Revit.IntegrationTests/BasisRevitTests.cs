namespace RxBim.Example.Revit.IntegrationTests;

using System;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Di;
using FluentAssertions;
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
        var container = configurator.Container;

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
        var sampleChair = _document.GetElement(new ElementId(990317)) as FamilyInstance;
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
    /// Create Family Instance Test.
    /// </summary>
    [Test]
    public void CreateFamilyInstanceTest()
    {
        using var tr = new Transaction(_document, "CreateChain");
        var fs = _document.GetElement(new ElementId(990191)) as FamilySymbol;
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