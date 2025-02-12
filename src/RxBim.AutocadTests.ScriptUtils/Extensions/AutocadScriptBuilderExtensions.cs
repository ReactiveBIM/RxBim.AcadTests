namespace RxBim.AutocadTests.ScriptUtils.Extensions;

using Abstractions;

/// <summary>
///     Методы расширения для <see cref="IAutocadScriptBuilder" />
/// </summary>
public static class AutocadScriptBuilderExtensions
{
    /// <summary>
    ///     Sets SECURELOAD
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IAutocadScriptBuilder" />
    /// </param>
    /// <param name="value">Value</param>
    public static IAutocadScriptBuilder SetSecureLoad(this IAutocadScriptBuilder builder, bool value)
    {
        builder.AddCommand($"SECURELOAD {(value ? "1" : "0")}");
        return builder;
    }

    /// <summary>
    ///     Sets STARTMODE
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IAutocadScriptBuilder" />
    /// </param>
    /// <param name="value">Value</param>
    public static IAutocadScriptBuilder SetStartMode(this IAutocadScriptBuilder builder, bool value)
    {
        builder.AddCommand($"STARTMODE {(value ? "1" : "0")}");
        return builder;
    }

    /// <summary>
    ///     The NETLOAD command
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IAutocadScriptBuilder" />
    /// </param>
    /// <param name="dllPath">Path</param>
    public static IAutocadScriptBuilder NetLoadCommand(this IAutocadScriptBuilder builder, string dllPath)
    {
        builder.AddCommand($"NETLOAD \"{dllPath}\"");
        return builder;
    }

    /// <summary>
    ///     Sets FILEDIA
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IAutocadScriptBuilder" />
    /// </param>
    /// <param name="value">Value</param>
    public static IAutocadScriptBuilder SetFiledia(this IAutocadScriptBuilder builder, bool value)
    {
        builder.AddCommand($"FILEDIA {(value ? "1" : "0")}");
        return builder;
    }

    /// <summary>
    /// The QUIT command
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IAutocadScriptBuilder" />
    /// </param>
    public static IAutocadScriptBuilder QuitCommand(this IAutocadScriptBuilder builder)
    {
        builder.AddCommand("_QUIT");
        builder.AddCommand("_Y");
        return builder;
    }
}