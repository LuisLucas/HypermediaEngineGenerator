using HypermediaEngineGenerator.Helper;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.ModelsGen.Models
{
    internal static class CollectionResourceModel
    {
        private const string FileName = "CollectionResource";
        public const string Class = @"
namespace Hypermedia.Models;

public class CollectionResource<T>
{
    public List<Resource<T>> Items { get; set; }

    public List<Link> Links { get; set; }
}";

        internal static IncrementalGeneratorInitializationContext AddToSource(this IncrementalGeneratorInitializationContext context)
        {
            return context.AddFileToSource(Class, FileName);
        }
    }
}
