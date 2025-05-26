using HypermediaEngineGenerator.InterfacesGen.Interfaces;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.InterfacesGen
{
    internal static class InterfacesGenerator
    {
        internal static IncrementalGeneratorInitializationContext AddInterfaces(this IncrementalGeneratorInitializationContext context)
        {
            IHypermediaGenerator.AddToSource(context);
            return context;
        }
    }
}
