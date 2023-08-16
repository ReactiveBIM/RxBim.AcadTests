namespace AcadTests.Nuke.Components
{
    using Bimlab.Nuke.Components;
    using global::Nuke.Common;
    using global::Nuke.Common.ProjectModel;
    using JetBrains.Annotations;
    using Services;

    /// <summary>
    /// Contains targets for running integration tests.
    /// </summary>
    [PublicAPI]
    public interface IIntegrationTest : IHazSolution
    {
        /// <summary>
        /// TestProjectProvider.
        /// </summary>
        TestProjectProvider TestProjectProvider => new(Solution);

        /// <summary>
        /// ProjectTestRunner.
        /// </summary>
        ProjectTestRunner ProjectTestRunner => new(Solution);

        /// <summary>
        /// Test only selected projects
        /// </summary>
        [Parameter("Test only selected projects")]
        bool OnlySelectedProjects => TryGetValue<bool?>(() => OnlySelectedProjects) ?? false;

        /// <summary>
        /// Collection of test projects.
        /// </summary>
        [Parameter("Collection of test projects")]
        Project[] TestProjects
        {
            get
            {
                var projects = TryGetValue(() => TestProjects);
                if (projects is not null)
                    return projects;

                projects = OnlySelectedProjects
                    ? TestProjectProvider.GetSelectedProjects()
                    : TestProjectProvider.Projects;

                return projects;
            }
        }

        /// <summary>
        /// Runs integration tests for Revit.
        /// </summary>
        Target RevitIntegrationTests =>
            definition => definition
                .Description("Starts execution of integration tests in the Revit environment")
                .Executes(async () =>
                {
                    var consoleDllPath = typeof(RevitTests.Console.Services.RevitTestTasks).Assembly.Location;
                    foreach (var project in TestProjects)
                    {
                        await ProjectTestRunner.RunTests(project, consoleDllPath);
                    }
                });

        /// <summary>
        /// Runs integration tests for Autocad.
        /// </summary>
        Target AutocadIntegrationTests =>
            definition => definition
                .Description("Starts execution of integration tests in the Autocad environment")
                .Executes(async () =>
                {
                    var consoleDllPath = typeof(AcadTests.Console.Services.AcadTestTasks).Assembly.Location;
                    foreach (var project in TestProjects)
                    {
                        await ProjectTestRunner.RunTests(project, consoleDllPath);
                    }
                });
    }
}