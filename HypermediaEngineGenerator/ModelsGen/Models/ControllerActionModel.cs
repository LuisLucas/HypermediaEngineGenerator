using HypermediaEngineGenerator.Helper;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.ModelsGen.Models
{
    internal static class ControllerActionModel
    {
        private const string FileName = "ControllerAction";
        public const string Class = @"
namespace Hypermedia.Models;

public record ControllerAction(string action, object? values, string rel, string method, string controller = """");

public record ControllerAction<T, R>(string action, Tuple<string, Func<T, R>> values, string rel, string method, string controller = """");";

        internal static IncrementalGeneratorInitializationContext AddToSource(this IncrementalGeneratorInitializationContext context)
        {
            return context.AddFileToSource(Class, FileName);
        }
    }
}
