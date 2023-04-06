namespace AcadTests.Console.Services;

using System;
using Abstractions;
using CommandLine;
using Models;

/// <inheritdoc />
public class TestRunningOptionsFactory : ITestRunningOptionsFactory
{
    private readonly string[] _args;

    /// <summary>
    ///     ctr
    /// </summary>
    /// <param name="args">args</param>
    public TestRunningOptionsFactory(string[] args)
    {
        _args = args;
    }

    /// <inheritdoc />
    public TestRunningOptions GetTestRunningOptions()
    {
        var parserResult = Parser.Default.ParseArguments<TestRunningOptions>(_args);
        foreach (var error in parserResult.Errors)
            Console.WriteLine(error);

        return parserResult.Value;
    }
}