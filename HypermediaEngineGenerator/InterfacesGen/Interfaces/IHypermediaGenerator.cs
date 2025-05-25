using HypermediaEngineGenerator.Helper;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.InterfacesGen.Interfaces
{
    internal static class IHypermediaGenerator
    {
        private const string FileName = "IHypermediaGenerator";
        public const string Class = @"
namespace Hypermedia.Interfaces;

public interface IHypermediaGenerator<T>
{
    Resource<T> GenerateLinks(T item, HttpContext httpContext);

    Resource<T> GenerateLinks(T item, Type controller);

    CollectionResource<T> GenerateLinks(IEnumerable<T> items, Type controller);

    PaginatedResource<T> GenerateLinks(IEnumerable<T> items, Type controller, int page, int pageSize, int totalNumberOfRecords);
}";

        internal static IncrementalGeneratorInitializationContext AddToSource(this IncrementalGeneratorInitializationContext context)
        {
            return context.AddFileToSource(Class, FileName);
        }
    }
}
