using HypermediaEngineGenerator.ModelsGen.Models;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.ModelsGen
{
    internal static class ModelGenerator
    {
        internal static IncrementalGeneratorInitializationContext AddModels(this IncrementalGeneratorInitializationContext context)
        {
            LinkModel.AddToSource(context);
            ResourceModel.AddToSource(context);
            CollectionResourceModel.AddToSource(context);
            PaginatedResourceModel.AddToSource(context);
            ControllerActionModel.AddToSource(context);
            return context;
        }
    }
}
