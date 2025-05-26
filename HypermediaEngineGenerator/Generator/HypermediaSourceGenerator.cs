using HypermediaEngineGenerator.Attributes;
using HypermediaEngineGenerator.InterfacesGen;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.Generator
{
    [Generator]
    internal class HypermediaSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context
                .AddAttributes()
                .AddInterfaces();
        }
    }
}
