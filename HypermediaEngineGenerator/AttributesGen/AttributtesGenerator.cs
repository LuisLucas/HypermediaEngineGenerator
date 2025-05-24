using HypermediaEngineGenerator.AttributesGen.Attrubutes;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.Attributes
{
    internal static class AttributtesGenerator
    {
        internal static IncrementalGeneratorInitializationContext AddAttributes(this IncrementalGeneratorInitializationContext context)
        {
            HypermediaEnableAttribute.AddAttributeToSource(context);
            return context;
        }
    }
}
