using HypermediaEngineGenerator.Attributes;
using HypermediaEngineGenerator.HypermediaGenerator;
using HypermediaEngineGenerator.InterfacesGen;
using HypermediaEngineGenerator.ModelsGen;
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
                .AddInterfaces()
                .AddModels();

            HypermediaFactory.AddToSource(context);
            LinkGenerator.AddToSource(context);
        }
    }
}
