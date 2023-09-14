namespace AcadTests.Nuke.Services
{
    using Bimlab.Nuke;
    using global::Nuke.Common.ProjectModel;

    /// <summary>
    /// <see cref="Project"/> provider of solution test projects.
    /// </summary>
    public class TestProjectProvider
    {
        private readonly Solution _solution;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestProjectProvider"/> class.
        /// </summary>
        /// <param name="solution"><see cref="Solution"/>.</param>
        public TestProjectProvider(Solution solution)
        {
            _solution = solution;
        }

        /// <summary>
        /// The collection of solution test projects.
        /// </summary>
        public Project[] Projects => GetProjects();

        /// <summary>
        /// Returns the projects selected by the user.
        /// </summary>
        public Project[] GetSelectedProjects()
        {
            var options = Projects
                .Select(project => (project.Name, project.Name))
                .Prepend(("All projects", "All projects"));

            var selectedProjectNames = SelectionUtility
                .PromptForOptions("Select projects with space bar:", true, options.ToArray());

            return Projects.Where(project => selectedProjectNames.Contains(project.Name)).ToArray();
        }

        private Project[] GetProjects()
        {
            return _solution
                .AllProjects
                .Where(project => project.Name.EndsWith(".IntegrationTests"))
                .ToArray();
        }
    }
}