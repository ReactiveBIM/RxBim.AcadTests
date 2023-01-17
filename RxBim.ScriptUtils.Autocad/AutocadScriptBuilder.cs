namespace RxBim.ScriptUtils.Autocad
{
    using System.Text;
    using Abstractions;

    /// <inheritdoc />
    public class AutocadScriptBuilder : IAutocadScriptBuilder
    {
        private readonly StringBuilder _builder = new();

        /// <inheritdoc />
        public IAutocadScriptBuilder AddCommand(string command)
        {
            _builder.AppendLine(command);
            return this;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}