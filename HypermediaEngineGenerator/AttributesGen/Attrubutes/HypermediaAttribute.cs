using HypermediaEngineGenerator.Helper;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.AttributesGen.Attrubutes
{
    internal static class HypermediaAttribute
    {
        internal const string FileName = "HypermediaAttribute";
        private const string Class = @"
namespace Hypermedia.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class HypermediaAttribute : Attribute
{
    public string Method { get; }
    public string Relation { get; }
    public string Property { get; }
    public string Controller { get; }

    public HypermediaAttribute(string method, string relation, string property, string controller = """")
    {
        Method = method;
        Relation = relation;
        Property = property;
        Controller = controller;
    }
}";

        internal static IncrementalGeneratorInitializationContext AddToSource(this IncrementalGeneratorInitializationContext context)
        {
            return context.AddFileToSource(Class, FileName);
        }
    }
}
