using HypermediaEngineGenerator.Helper;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.ModelsGen.Models
{
    internal static class PaginatedResourceModel
    {
        private const string FileName = "PaginatedResource";
        public const string Class = @"
namespace Hypermedia.Models;

public class PaginatedResource<T>
{
    public List<Resource<T>> Items { get; set; }

    public List<Link> Links { get; set; }

    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => TotalItems > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 0;
}";

        internal static IncrementalGeneratorInitializationContext AddToSource(this IncrementalGeneratorInitializationContext context)
        {
            return context.AddFileToSource(Class, FileName);
        }
    }
}
