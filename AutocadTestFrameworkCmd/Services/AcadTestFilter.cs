namespace AutocadTestFrameworkCmd.Services;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

/// <inheritdoc />
public class AcadTestFilter : TestFilter
{
    /// <inheritdoc />
    public override bool Match(ITest test)
    {
        return true;
        /*return test.Fixture?.GetType().GetCustomAttributes(typeof(TestDrawingAttribute)).Any() == true;*/
    }

    /// <inheritdoc />
    public override TNode AddToXml(TNode parentNode, bool recursive)
    {
        return parentNode.AddElement(nameof(AcadTestFilter));
    }
}