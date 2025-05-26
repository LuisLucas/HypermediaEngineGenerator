using HypermediaEngineGenerator.Helper;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.ModelsGen.Models
{
    internal static class LinkModel
    {
        private const string FileName = "Link";
        public const string Class = @"
namespace Hypermedia.Models;

public record class Link(string Href, string Rel, string Method);";

        internal static IncrementalGeneratorInitializationContext AddToSource(this IncrementalGeneratorInitializationContext context)
        {
            return context.AddFileToSource(Class, FileName);
        }
    }
}
