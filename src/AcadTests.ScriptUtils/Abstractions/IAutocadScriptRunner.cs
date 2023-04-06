namespace AcadTests.ScriptUtils.Abstractions;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
///     Сервис для выполнения скрипта Autocad
/// </summary>
public interface IAutocadScriptRunner
{
    /// <summary>
    ///     Запускает скрипт
    /// </summary>
    /// <param name="scriptBuilder">
    ///     <see cref="IAutocadScriptBuilder" />
    /// </param>
    /// <param name="cancellationToken">Cansellation token</param>
    Task Run(Action<IAutocadScriptBuilder> scriptBuilder, CancellationToken cancellationToken);

    /// <summary>
    ///     Устанавливает файл шаблона для запуска
    /// </summary>
    /// <param name="path">Путь к файлу</param>
    IAutocadScriptRunner SetTemplateFile(string path);
}