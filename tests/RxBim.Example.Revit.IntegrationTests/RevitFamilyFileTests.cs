namespace RxBim.Example.Revit.IntegrationTests;

using System;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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

    /// <summary>
    /// Настройки.
    /// </summary>
    [OneTimeSetUp]
    public void Setup()
    {
        var configurator = new TestingDiConfigurator();
        configurator.Configure(GetType().Assembly);
        var container = configurator.Build();

        _uiApplication = container.GetService<UIApplication>();
        var modelPath = Path.Combine(Environment.CurrentDirectory, "rac_basic_sample_family.rfa");
        _uiApplication.OpenAndActivateDocument(modelPath);
        _document = container.GetService<Document>();
    }

    /// <summary>
    /// Тест документа.
    /// </summary>
    [Test]
    public void IsFamilyDocTest()
    {
        _document.IsFamilyDocument.Should().BeTrue();
    }
}