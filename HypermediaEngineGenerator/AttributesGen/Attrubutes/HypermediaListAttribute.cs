using HypermediaEngineGenerator.Helper;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.AttributesGen.Attrubutes
{
    internal static class HypermediaListAttribute
    {
        private const string FileName = "HypermediaListAttribute";
        private const string Class = @"
namespace Hypermedia.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class HypermediaListAttribute : Attribute
{
    public string Method { get; }
    public string Relation { get; }

    public HypermediaListAttribute(string method, string relation)
    {
        Method = method;
        Relation = relation;
    }
}";

        internal static IncrementalGeneratorInitializationContext AddToSource(this IncrementalGeneratorInitializationContext context)
        {
            return context.AddFileToSource(Class, FileName);
        }
    }
}
