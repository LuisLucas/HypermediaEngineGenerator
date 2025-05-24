using HypermediaEngineGenerator.Attributes;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.Generator
{
    [Generator]
    internal class HypermediaSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.AddAttributes();
        }
    }
}
