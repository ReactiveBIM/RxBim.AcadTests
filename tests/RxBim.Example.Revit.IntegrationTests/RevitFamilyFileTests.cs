namespace RxBim.Example.Revit.IntegrationTests;

using System;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Di;
using FluentAssertions;
using NUnit.Framework;
using RevitTests.TestingUtils;

/// <summary>
/// Tests in .rfa file
/// </summary>
[TestFixture]
public class RevitFamilyFileTests
{
    private Document _document;
    private UIApplication _uiApplication;

    [OneTimeSetUp]
    public void Setup()
    {
        var configurator = new TestingDiConfigurator();
        configurator.Configure(GetType().Assembly);
        var container = configurator.Container;

        _uiApplication = container.GetService<UIApplication>();
        var modelPath = Path.Combine(Environment.CurrentDirectory, "rac_basic_sample_family.rfa");
        _uiApplication.OpenAndActivateDocument(modelPath);
        _document = container.GetService<Document>();
    }

    [Test]
    public void IsFamilyDocTest()
    {
        _document.IsFamilyDocument.Should().BeTrue();
    }
}