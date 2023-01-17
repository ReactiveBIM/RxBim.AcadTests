namespace RxBim.ScriptUtils.Autocad.Abstractions
{
    /// <summary>
    /// Строитель для скрипта Autocad
    /// </summary>
    public interface IAutocadScriptBuilder
    {
        /// <summary>
        /// Добавляет команду
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        IAutocadScriptBuilder AddCommand(string command);
    }
}