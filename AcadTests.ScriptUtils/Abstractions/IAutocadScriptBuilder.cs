namespace AcadTests.ScriptUtils.Abstractions;

/// <summary>
///     Строитель для скрипта Autocad
/// </summary>
public interface IAutocadScriptBuilder
{
    /// <summary>
    ///     Adds the command to execute.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns></returns>
    IAutocadScriptBuilder AddCommand(string command);
}